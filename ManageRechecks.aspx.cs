using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class ManageRechecks : System.Web.UI.Page
    {
        // Bound by repeater for the per-row faculty dropdown.
        public List<ListItem> FacultyOptions { get; private set; } = new List<ListItem>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "1")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            // Always recompute faculty list (cheap; needed every render)
            BuildFacultyOptions();
            if (!IsPostBack) Load();
        }

        private void BuildFacultyOptions()
        {
            var users = MongoDbContext.GetCollection<BsonDocument>("users");
            var faculty = users.Find(Builders<BsonDocument>.Filter.Eq("roleId", 3))
                .ToList();

            FacultyOptions = new List<ListItem> { new ListItem("-- Choose faculty --", "") };
            foreach (var f in faculty)
            {
                FacultyOptions.Add(new ListItem(
                    f.GetValue("name", "(Unnamed)").ToString(),
                    f.GetValue("userId", "").ToString()));
            }
        }

        private void Load()
        {
            var rcCol = MongoDbContext.GetCollection<RecheckRequest>("recheck_requests");
            var users = MongoDbContext.GetCollection<BsonDocument>("users");

            var pending = rcCol.Find(r => r.Status == "Pending")
                               .SortByDescending(r => r.RequestedAt)
                               .ToList();

            // Annotate with student name
            var rows = pending.Select(r =>
            {
                var u = users.Find(Builders<BsonDocument>.Filter.Eq("userId", r.StudentId)).FirstOrDefault();
                return new
                {
                    r.RecheckId, r.ExamTitle, r.Status, r.OldScore, r.Reason, r.RequestedAt,
                    StudentName = u != null ? u.GetValue("name", r.StudentId).ToString() : r.StudentId
                };
            }).ToList();

            pnlEmpty.Visible = rows.Count == 0;
            rptRequests.DataSource = rows;
            rptRequests.DataBind();
        }

        protected void rptRequests_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string arg = e.CommandArgument?.ToString() ?? "";

            if (e.CommandName == "Assign")
            {
                // arg = "RC123|rowIndex"
                var parts = arg.Split('|');
                if (parts.Length != 2) return;
                string recheckId = parts[0];
                int rowIdx = int.Parse(parts[1]);
                var ddl = (DropDownList)e.Item.FindControl("ddlFaculty");
                string facultyId = ddl?.SelectedValue;

                if (string.IsNullOrEmpty(facultyId))
                { Show("Please pick a faculty member first.", false); return; }

                var rcCol = MongoDbContext.GetCollection<RecheckRequest>("recheck_requests");
                var rc = rcCol.Find(x => x.RecheckId == recheckId).FirstOrDefault();
                if (rc == null) { Show("Request not found.", false); return; }

                rcCol.UpdateOne(
                    Builders<RecheckRequest>.Filter.Eq(x => x.RecheckId, recheckId),
                    Builders<RecheckRequest>.Update
                        .Set(x => x.Status, "Assigned")
                        .Set(x => x.AssignedFacultyId, facultyId)
                        .Set(x => x.AssignedAt, DateTime.UtcNow));

                NotificationService.Push(facultyId, "recheck_assigned",
                    "Recheck assigned to you",
                    "You've been asked to recheck " + (rc.ExamTitle ?? rc.ExamId) + ".",
                    "~/RecheckRequests.aspx");
                NotificationService.Push(rc.StudentId, "recheck_assigned",
                    "Recheck in progress",
                    "Your recheck request has been assigned to a faculty member.",
                    "~/StudentResults.aspx");

                Show("Assigned successfully.", true);
            }
            else if (e.CommandName == "Reject")
            {
                string recheckId = arg;
                var rcCol = MongoDbContext.GetCollection<RecheckRequest>("recheck_requests");
                var rc = rcCol.Find(x => x.RecheckId == recheckId).FirstOrDefault();
                if (rc == null) return;

                rcCol.UpdateOne(
                    Builders<RecheckRequest>.Filter.Eq(x => x.RecheckId, recheckId),
                    Builders<RecheckRequest>.Update.Set(x => x.Status, "Rejected"));

                MongoDbContext.GetCollection<ExamResult>("results").UpdateOne(
                    Builders<ExamResult>.Filter.Eq(r => r.ResultId, rc.ResultId),
                    Builders<ExamResult>.Update.Set(r => r.Status, "Auto-Graded"));

                NotificationService.Push(rc.StudentId, "exam_rejected",
                    "Recheck rejected",
                    "Your recheck request was reviewed and rejected.",
                    "~/StudentResults.aspx");

                Show("Request rejected.", true);
            }

            Load();
        }

        private void Show(string msg, bool ok)
        {
            lblMsg.Text = (ok ? "✅ " : "❌ ") + msg;
            lblMsg.Visible = true;
            lblMsg.ForeColor = ok ? System.Drawing.Color.LightGreen : System.Drawing.Color.IndianRed;
        }
    }
}
