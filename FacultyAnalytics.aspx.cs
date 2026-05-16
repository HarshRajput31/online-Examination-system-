using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class FacultyAnalytics : System.Web.UI.Page
    {
        private IMongoCollection<BsonDocument> examsCol;
        private IMongoCollection<BsonDocument> resultsCol;
        private IMongoCollection<BsonDocument> usersCol;
        private string facultyId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null ||
                Session["RoleId"]?.ToString() != "3")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            facultyId = Session["UserId"].ToString();
            ConnectDB();

            if (!IsPostBack)
            {
                LoadSubjects();
                LoadExamsBySubject();
                LoadAnalytics();
            }
        }

        // ================= DB =================
        private void ConnectDB()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("OnlineExamDB");

            examsCol = db.GetCollection<BsonDocument>("exams");
            resultsCol = db.GetCollection<BsonDocument>("results"); // ✅ FIXED
            usersCol = db.GetCollection<BsonDocument>("users");
        }

        // ================= SUBJECT =================
        private void LoadSubjects()
        {
            var subjects = examsCol.Find(
                Builders<BsonDocument>.Filter.Eq("createdBy", facultyId))
                .ToList()
                .Select(e => e.GetValue("subject", "").ToString())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ddlSubject.Items.Clear();
            ddlSubject.Items.Add(new ListItem("All Subjects", ""));

            foreach (var s in subjects)
                ddlSubject.Items.Add(new ListItem(s, s));
        }

        // ================= EXAMS =================
        private void LoadExamsBySubject()
        {
            string subject = ddlSubject.SelectedValue;

            var filter = string.IsNullOrEmpty(subject)
                ? Builders<BsonDocument>.Filter.Eq("createdBy", facultyId)
                : Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("createdBy", facultyId),
                    Builders<BsonDocument>.Filter.Eq("subject", subject));

            var exams = examsCol.Find(filter).ToList();

            ddlExam.Items.Clear();
            ddlExam.Items.Add(new ListItem("All Exams", ""));

            foreach (var ex in exams)
            {
                string id = ex.GetValue("examId", "").ToString();
                string title = ex.GetValue("title", "").ToString();

                ddlExam.Items.Add(new ListItem(title, id));
            }
        }

        // ================= MAIN =================
        private void LoadAnalytics()
        {
            string examId = ddlExam.SelectedValue;

            var examFilter = string.IsNullOrEmpty(examId)
                ? Builders<BsonDocument>.Filter.Eq("createdBy", facultyId)
                : Builders<BsonDocument>.Filter.Eq("examId", examId);

            var exams = examsCol.Find(examFilter).ToList();
            var examIds = exams.Select(e => e["examId"].AsString).ToList();

            if (examIds.Count == 0)
            {
                ShowEmpty();
                return;
            }

            // ✅ FIXED FILTER
            var resultFilter = Builders<BsonDocument>.Filter.In("examId", examIds);

            var results = resultsCol.Find(resultFilter).ToList();

            var students = usersCol.Find(
                Builders<BsonDocument>.Filter.Eq("roleId", 2))
                .ToList();

            var rows = new List<ResultRow>();

            foreach (var r in results)
            {
                string sid = r.GetValue("studentId", "").ToString();
                string exam = r.GetValue("examId", "").ToString();
                int marks = r.GetValue("score", 0).ToInt32();

                var ex = exams.FirstOrDefault(x => x["examId"] == exam);
                var stu = students.FirstOrDefault(s => s["userId"] == sid);

                rows.Add(new ResultRow
                {
                    StudentId = sid,
                    StudentName = stu?.GetValue("name", "Unknown").ToString(),
                    Department = stu?.GetValue("course", "").ToString(),
                    Subject = ex?.GetValue("subject", "").ToString(),
                    SetNumber = ex?.GetValue("setNumber", "").ToString(),
                    ObtainedMarks = marks,
                    TotalMarks = ex?.GetValue("totalMarks", 0).ToInt32() ?? 0,
                    Percentage = 0,
                    Passed = marks >= 40,
                    Attempted = true,
                    StatusLabel = marks >= 40 ? "✅ Pass" : "❌ Fail",
                    StatusClass = marks >= 40 ? "pass" : "fail",
                    AttemptLabel = "✅ Attempted",
                    AttemptClass = "attempted"
                });
            }

            // ================= STATS =================
            lblTotal.Text = rows.Count.ToString();
            lblAttempted.Text = rows.Count.ToString();
            lblNotAttempted.Text = "0";
            lblPassed.Text = rows.Count(x => x.Passed).ToString();
            lblFailed.Text = rows.Count(x => !x.Passed).ToString();
            lblAvgMarks.Text = rows.Count > 0
                ? rows.Average(x => x.ObtainedMarks).ToString("0.0")
                : "0";

            // ================= TOP 3 =================
            var top = rows.OrderByDescending(x => x.ObtainedMarks).Take(3).ToList();

            pnlToppers.Visible = top.Count > 0;
            rptToppers.DataSource = top;
            rptToppers.DataBind();

            // ================= TABLE =================
            pnlTable.Visible = true;
            pnlEmpty.Visible = false;

            rptResults.DataSource = rows.OrderByDescending(x => x.ObtainedMarks);
            rptResults.DataBind();
        }

        // ================= EVENTS =================
        protected void ddlSubject_Changed(object sender, EventArgs e)
        {
            LoadExamsBySubject();
            LoadAnalytics();
        }

        protected void ddlExam_Changed(object sender, EventArgs e)
        {
            LoadAnalytics();
        }

        protected void ddlDept_Changed(object sender, EventArgs e)
        {
            LoadAnalytics();
        }

        // ================= HELPERS =================
        public string GetRankClass(int rank)
        {
            if (rank == 1) return "gold";
            if (rank == 2) return "silver";
            if (rank == 3) return "bronze";
            return "";
        }

        public string GetRankIcon(int rank)
        {
            if (rank == 1) return "🥇";
            if (rank == 2) return "🥈";
            if (rank == 3) return "🥉";
            return "";
        }

        private void ShowEmpty()
        {
            pnlToppers.Visible = false;
            pnlTable.Visible = false;
            pnlEmpty.Visible = true;
        }
    }
}