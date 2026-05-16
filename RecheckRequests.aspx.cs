using System;
using System.Linq;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class RecheckRequestsPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "3")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack) Load();
        }

        private void Load()
        {
            string fid = Session["UserId"].ToString();
            var rcCol = MongoDbContext.GetCollection<RecheckRequest>("recheck_requests");
            var users = MongoDbContext.GetCollection<BsonDocument>("users");

            var assigned = rcCol.Find(r =>
                r.AssignedFacultyId == fid && r.Status == "Assigned")
                .SortByDescending(r => r.AssignedAt)
                .ToList();

            var rows = assigned.Select(r =>
            {
                var u = users.Find(Builders<BsonDocument>.Filter.Eq("userId", r.StudentId)).FirstOrDefault();
                return new
                {
                    r.RecheckId, r.ExamTitle, r.Status, r.OldScore, r.Reason,
                    StudentName = u != null ? u.GetValue("name", r.StudentId).ToString() : r.StudentId
                };
            }).ToList();

            pnlEmpty.Visible = rows.Count == 0;
            rptRequests.DataSource = rows;
            rptRequests.DataBind();
        }

        protected void rptRequests_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Complete") return;

            string rid = e.CommandArgument?.ToString();
            var newScoreBox = (TextBox)e.Item.FindControl("txtNewScore");
            var commentsBox = (TextBox)e.Item.FindControl("txtComments");

            if (!double.TryParse(newScoreBox?.Text ?? "", out double newScore))
            { Show("Enter a valid score.", false); return; }

            var rcCol = MongoDbContext.GetCollection<RecheckRequest>("recheck_requests");
            var rc = rcCol.Find(x => x.RecheckId == rid).FirstOrDefault();
            if (rc == null) { Show("Request not found.", false); return; }

            // Update the recheck record
            rcCol.UpdateOne(
                Builders<RecheckRequest>.Filter.Eq(x => x.RecheckId, rid),
                Builders<RecheckRequest>.Update
                    .Set(x => x.Status, "Completed")
                    .Set(x => x.NewScore, newScore)
                    .Set(x => x.FacultyComments, commentsBox?.Text ?? "")
                    .Set(x => x.CompletedAt, DateTime.UtcNow));

            // Update the underlying result
            var rCol = MongoDbContext.GetCollection<ExamResult>("results");
            var result = rCol.Find(r => r.ResultId == rc.ResultId).FirstOrDefault();
            if (result != null)
            {
                double pct = result.TotalMarks > 0 ? (newScore / result.TotalMarks) * 100 : 0;
                rCol.UpdateOne(
                    Builders<ExamResult>.Filter.Eq(r => r.ResultId, result.ResultId),
                    Builders<ExamResult>.Update
                        .Set(r => r.Score, newScore)
                        .Set(r => r.Percentage, pct)
                        .Set(r => r.Passed, pct >= 40)
                        .Set(r => r.Status, "Reviewed")
                        .Set(r => r.ReviewedBy, Session["UserId"].ToString())
                        .Set(r => r.ReviewedAt, DateTime.UtcNow));
            }

            // Notify student
            NotificationService.Push(rc.StudentId, "recheck_completed",
                "Recheck completed",
                string.Format("Your {0} recheck is done. New score: {1}.",
                    rc.ExamTitle ?? rc.ExamId, newScore),
                "~/StudentResults.aspx");

            Show("Recheck submitted. Student notified.", true);
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
