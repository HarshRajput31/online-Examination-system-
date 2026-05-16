using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class StudentDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Only students (RoleId = 2)
            if (Session["UserId"] == null || Session["RoleId"] == null ||
                Session["RoleId"].ToString() != "2")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack) LoadDashboard();
        }

        private void LoadDashboard()
        {
            string studentId = Session["UserId"].ToString();
            var db = MongoDbContext.Database;

            // ---- Student name ----
            var users = db.GetCollection<BsonDocument>("users");
            var meDoc = users.Find(Builders<BsonDocument>.Filter.Eq("userId", studentId)).FirstOrDefault();
            litStudentName.Text = meDoc != null && meDoc.Contains("name")
                ? Server.HtmlEncode(meDoc["name"].AsString) : "Student";

            // ---- Available exams (approved + published) ----
            var examsCol = db.GetCollection<BsonDocument>("exams");
            var publishedFilter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("isApproved", true),
                Builders<BsonDocument>.Filter.Eq("isPublished", true));

            var exams = examsCol.Find(publishedFilter)
                                .SortByDescending(e => e["createdAt"])
                                .Limit(12)
                                .ToList();

            var resultsCol = db.GetCollection<ExamResult>("results");
            var myResults = resultsCol.Find(r => r.StudentId == studentId).ToList();
            var attemptedExamIds = myResults.Select(r => r.ExamId).ToHashSet();

            var examCards = exams
                .Where(x => !attemptedExamIds.Contains(x.GetValue("examId", "").AsString))
                .Select(x =>
                {
                    int qCount = 0;
                    if (x.Contains("questions") && x["questions"].IsBsonArray)
                        qCount = x["questions"].AsBsonArray.Count;
                    return new
                    {
                        ExamId = x.GetValue("examId", "").ToString(),
                        Title = x.GetValue("title", "Untitled").ToString(),
                        Subject = x.GetValue("subject", "General").ToString(),
                        Duration = x.GetValue("duration", 0).ToInt32(),
                        TotalMarks = x.GetValue("totalMarks", 0).ToInt32(),
                        QuestionCount = qCount
                    };
                }).ToList();

            rptExams.DataSource = examCards;
            rptExams.DataBind();
            pnlNoExams.Visible = examCards.Count == 0;

            // ---- Stats ----
            lblUpcoming.Text = examCards.Count.ToString();
            lblCompleted.Text = myResults.Count.ToString();
            lblAvgScore.Text = myResults.Count == 0
                ? "0%"
                : myResults.Average(r => r.Percentage).ToString("F1") + "%";
            lblNotifications.Text = NotificationService.UnreadCount(studentId).ToString();

            // ---- Recent results (top 5) ----
            var recent = myResults
                .OrderByDescending(r => r.SubmittedAt)
                .Take(5)
                .ToList();

            rptRecent.DataSource = recent;
            rptRecent.DataBind();
            pnlNoResults.Visible = recent.Count == 0;
        }
    }
}
