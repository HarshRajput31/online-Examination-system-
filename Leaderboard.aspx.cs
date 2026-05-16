using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class LeaderboardPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/Login.aspx"); return; }
            if (!IsPostBack)
            {
                LoadFilters();
                LoadBoard();
            }
        }

        protected void OnFilterChange(object sender, EventArgs e) => LoadBoard();

        private void LoadFilters()
        {
            var rCol = MongoDbContext.GetCollection<ExamResult>("results");
            var subjects = rCol.Distinct(r => r.Subject, FilterDefinition<ExamResult>.Empty).ToList();

            ddlSubject.Items.Clear();
            ddlSubject.Items.Add(new ListItem("All subjects", ""));
            foreach (var s in subjects.Where(x => !string.IsNullOrEmpty(x)).OrderBy(x => x))
                ddlSubject.Items.Add(new ListItem(s, s));

            var users = MongoDbContext.GetCollection<BsonDocument>("users");
            var depts = users.Distinct<string>("department", new BsonDocument()).ToList();
            ddlDept.Items.Clear();
            ddlDept.Items.Add(new ListItem("All departments", ""));
            foreach (var d in depts.Where(x => !string.IsNullOrEmpty(x)).OrderBy(x => x))
                ddlDept.Items.Add(new ListItem(d, d));
        }

        private void LoadBoard()
        {
            string subject = ddlSubject.SelectedValue;
            string dept = ddlDept.SelectedValue;

            // Pull results with optional subject filter
            var rCol = MongoDbContext.GetCollection<ExamResult>("results");
            var fb = Builders<ExamResult>.Filter;
            var filter = fb.Empty;
            if (!string.IsNullOrEmpty(subject)) filter &= fb.Eq(r => r.Subject, subject);

            var results = rCol.Find(filter).ToList();

            // Pull all student users for department lookup + names
            var usersCol = MongoDbContext.GetCollection<BsonDocument>("users");
            var students = usersCol.Find(Builders<BsonDocument>.Filter.Eq("roleId", 2))
                .ToList()
                .ToDictionary(
                    u => u.GetValue("userId", "").ToString(),
                    u => new
                    {
                        Name = u.GetValue("name", "Student").ToString(),
                        Dept = u.GetValue("department", "").ToString()
                    });

            // Aggregate per student
            var board = results
                .GroupBy(r => r.StudentId)
                .Select(g =>
                {
                    students.TryGetValue(g.Key, out var s);
                    return new
                    {
                        StudentId = g.Key,
                        StudentName = s?.Name ?? g.Key,
                        Department = s?.Dept ?? "",
                        AverageScore = g.Average(x => x.Percentage),
                        ExamsTaken = g.Count()
                    };
                })
                .Where(x => string.IsNullOrEmpty(dept) || x.Department == dept)
                .OrderByDescending(x => x.AverageScore)
                .ThenByDescending(x => x.ExamsTaken)
                .ToList();

            pnlEmpty.Visible = board.Count == 0;
            pnlTop3.Visible = board.Count > 0;

            rptTop3.DataSource = board.Take(3).ToList();
            rptTop3.DataBind();

            rptAll.DataSource = board.Take(50).ToList();
            rptAll.DataBind();
        }
    }
}
