using System;
using System.Linq;
using System.Web.UI.WebControls;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class RequestRecheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "2")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack) Load();
        }

        private void Load()
        {
            string sid = Session["UserId"].ToString();
            var results = MongoDbContext.GetCollection<ExamResult>("results")
                .Find(r => r.StudentId == sid)
                .SortByDescending(r => r.SubmittedAt)
                .ToList();

            ddlResult.Items.Clear();
            ddlResult.Items.Add(new ListItem("-- Select your result --", ""));
            foreach (var r in results)
            {
                string label = string.Format("{0} ({1}/{2}) - {3:dd MMM}",
                    r.ExamName ?? r.ExamId, r.Score, r.TotalMarks, r.SubmittedAt);
                ddlResult.Items.Add(new ListItem(label, r.ResultId ?? r.ExamId));
            }

            // Pre-select if ?resultId= is given
            string preselect = Request.QueryString["resultId"];
            if (!string.IsNullOrEmpty(preselect) && ddlResult.Items.FindByValue(preselect) != null)
            {
                ddlResult.SelectedValue = preselect;
                var picked = results.FirstOrDefault(x => x.ResultId == preselect);
                if (picked != null)
                    txtScore.Text = picked.Score + " / " + picked.TotalMarks;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string sid = Session["UserId"].ToString();
            string resultId = ddlResult.SelectedValue;
            string reason = (txtReason.Text ?? "").Trim();

            if (string.IsNullOrEmpty(resultId))
            { Show("Please select an exam.", false); return; }
            if (reason.Length < 10)
            { Show("Please describe your reason in at least 10 characters.", false); return; }

            var result = MongoDbContext.GetCollection<ExamResult>("results")
                .Find(r => r.ResultId == resultId).FirstOrDefault();
            if (result == null) { Show("Result not found.", false); return; }

            // Check duplicates
            var rcCol = MongoDbContext.GetCollection<RecheckRequest>("recheck_requests");
            bool already = rcCol.Find(r =>
                r.StudentId == sid &&
                r.ResultId == resultId &&
                r.Status != "Completed" &&
                r.Status != "Rejected").Any();
            if (already)
            { Show("You already have an open recheck for this exam.", false); return; }

            var rec = new RecheckRequest
            {
                RecheckId = "RC" + DateTime.UtcNow.Ticks.ToString().Substring(8),
                StudentId = sid,
                ExamId = result.ExamId,
                ExamTitle = result.ExamName,
                Subject = result.Subject,
                ResultId = result.ResultId,
                Reason = reason,
                Status = "Pending",
                OldScore = result.Score,
                RequestedAt = DateTime.UtcNow
            };
            rcCol.InsertOne(rec);

            // Mark result so it shows "Recheck Pending"
            MongoDbContext.GetCollection<ExamResult>("results").UpdateOne(
                Builders<ExamResult>.Filter.Eq(r => r.ResultId, result.ResultId),
                Builders<ExamResult>.Update.Set(r => r.Status, "Recheck Pending"));

            // Notify all admins
            var admins = MongoDbContext.GetCollection<MongoDB.Bson.BsonDocument>("users")
                .Find(MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("roleId", 1))
                .ToList();
            foreach (var a in admins)
            {
                NotificationService.Push(
                    a.GetValue("userId", "").ToString(),
                    "recheck_requested",
                    "New recheck request",
                    string.Format("{0} requested a recheck on {1}.",
                        result.StudentName ?? sid, result.ExamName ?? result.ExamId),
                    "~/ManageRechecks.aspx");
            }

            Show("Your recheck request has been submitted. The admin will assign a faculty member.", true);
            pnlForm.Visible = false;
        }

        private void Show(string msg, bool ok)
        {
            lblMsg.Text = (ok ? "✅ " : "❌ ") + msg;
            lblMsg.Visible = true;
            lblMsg.ForeColor = ok ? System.Drawing.Color.LightGreen : System.Drawing.Color.IndianRed;
        }
    }
}
