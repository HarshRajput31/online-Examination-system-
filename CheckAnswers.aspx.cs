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
    public partial class CheckAnswers : System.Web.UI.Page
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

            // Find this faculty's exams that are pending descriptive review
            var examsCol = MongoDbContext.GetCollection<BsonDocument>("exams");
            var myExamIds = examsCol.Find(
                Builders<BsonDocument>.Filter.Eq("createdBy", fid))
                .ToList()
                .Select(e => e.GetValue("examId", "").ToString())
                .ToList();
            if (myExamIds.Count == 0) { pnlNoSubmissions.Visible = true; return; }

            var rCol = MongoDbContext.GetCollection<ExamResult>("results");
            var pending = rCol.Find(
                Builders<ExamResult>.Filter.And(
                    Builders<ExamResult>.Filter.In(r => r.ExamId, myExamIds),
                    Builders<ExamResult>.Filter.Eq(r => r.Status, "Pending Review")))
                .ToList();

            if (pending.Count == 0) { pnlNoSubmissions.Visible = true; return; }

            // Pull related questions and student names
            var qCol = MongoDbContext.GetCollection<Question>("questions");
            var users = MongoDbContext.GetCollection<BsonDocument>("users");

            var rows = pending.Select(r =>
            {
                // Find descriptive questions for this exam
                var descQs = qCol.Find(
                    Builders<Question>.Filter.And(
                        Builders<Question>.Filter.Eq(q => q.ExamId, r.ExamId),
                        Builders<Question>.Filter.Eq(q => q.QuestionType, "desc"))).ToList();

                var answers = descQs.Select(q => new
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    MaxMarks = q.Marks,
                    StudentAnswer = (r.Answers != null && r.Answers.ContainsKey(q.QuestionId))
                        ? r.Answers[q.QuestionId] : "(no answer)"
                }).ToList();

                var u = users.Find(Builders<BsonDocument>.Filter.Eq("userId", r.StudentId)).FirstOrDefault();
                return new
                {
                    r.ResultId, r.ExamName, r.Score, r.TotalMarks, r.SubmittedAt,
                    StudentName = u != null ? u.GetValue("name", r.StudentId).ToString() : r.StudentId,
                    DescriptiveAnswers = answers
                };
            }).ToList();

            rptSubmissions.DataSource = rows;
            rptSubmissions.DataBind();
        }

        protected void rptSubmissions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Finalize") return;
            string resultId = e.CommandArgument?.ToString();
            if (string.IsNullOrEmpty(resultId)) return;

            var rCol = MongoDbContext.GetCollection<ExamResult>("results");
            var r = rCol.Find(x => x.ResultId == resultId).FirstOrDefault();
            if (r == null) return;

            // Pull descriptive question marks from the form
            var qCol = MongoDbContext.GetCollection<Question>("questions");
            var descQs = qCol.Find(
                Builders<Question>.Filter.And(
                    Builders<Question>.Filter.Eq(q => q.ExamId, r.ExamId),
                    Builders<Question>.Filter.Eq(q => q.QuestionType, "desc"))).ToList();

            double extra = 0;
            var marksMap = new Dictionary<string, double>();
            foreach (var q in descQs)
            {
                string raw = Request.Form["descmark_" + q.QuestionId] ?? "0";
                double.TryParse(raw, out double m);
                if (m < 0) m = 0; if (m > q.Marks) m = q.Marks;
                marksMap[q.QuestionId] = m;
                extra += m;
            }

            double newScore = r.Score + extra;
            double newPct = r.TotalMarks > 0 ? (newScore / r.TotalMarks) * 100 : 0;

            rCol.UpdateOne(
                Builders<ExamResult>.Filter.Eq(x => x.ResultId, resultId),
                Builders<ExamResult>.Update
                    .Set(x => x.Score, newScore)
                    .Set(x => x.Percentage, newPct)
                    .Set(x => x.Passed, newPct >= 40)
                    .Set(x => x.Status, "Reviewed")
                    .Set(x => x.DescriptiveMarks, marksMap)
                    .Set(x => x.ReviewedAt, DateTime.UtcNow)
                    .Set(x => x.ReviewedBy, Session["UserId"].ToString()));

            NotificationService.Push(r.StudentId, "result_published",
                "Result finalized",
                "Your descriptive answers were graded. Final score: " + newScore + "/" + r.TotalMarks + ".",
                "~/StudentResults.aspx");

            lblMsg.Text = "✅ Finalized for " + (r.StudentName ?? r.StudentId);
            lblMsg.Visible = true;
            lblMsg.ForeColor = System.Drawing.Color.LightGreen;
            Load();
        }
    }
}
