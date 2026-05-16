using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class StartExam : System.Web.UI.Page
    {
        // Exposed to markup so timer + palette can read them
        public string ExamId { get; private set; }
        public int DurationSeconds { get; private set; } = 1800;

        private List<Question> _questions = new List<Question>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Auth: only students
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "2")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            ExamId = Request.QueryString["ExamId"]
                  ?? (Session["ExamId"] != null ? Session["ExamId"].ToString() : null);

            if (string.IsNullOrWhiteSpace(ExamId))
            {
                Response.Redirect("~/StudentDashboard.aspx");
                return;
            }
            Session["ExamId"] = ExamId;

            if (!IsPostBack) LoadExam();
        }

        private void LoadExam()
        {
            var db = MongoDbContext.Database;
            var examsCol = db.GetCollection<BsonDocument>("exams");
            var qCol = db.GetCollection<Question>("questions");

            var exam = examsCol.Find(
                Builders<BsonDocument>.Filter.Eq("examId", ExamId)).FirstOrDefault();
            if (exam == null) { pnlExam.Visible = false; pnlEmpty.Visible = true; return; }

            litExamTitle.Text = Server.HtmlEncode(exam.GetValue("title", "Exam").ToString());
            litSubject.Text = Server.HtmlEncode(exam.GetValue("subject", "").ToString());
            litMarks.Text = exam.GetValue("totalMarks", 0).ToString();

            int durationMin = exam.GetValue("duration", 30).ToInt32();
            DurationSeconds = Math.Max(60, durationMin * 60);

            // 1) Try the embedded "questions" array on the exam doc (may be IDs or full docs)
            _questions = qCol.Find(
                Builders<Question>.Filter.Eq(q => q.ExamId, ExamId)).ToList();

            if (_questions.Count == 0 && exam.Contains("questions") && exam["questions"].IsBsonArray)
            {
                var ids = exam["questions"].AsBsonArray.Select(v => v.ToString()).ToList();
                _questions = qCol.Find(
                    Builders<Question>.Filter.In(q => q.QuestionId, ids)).ToList();
            }

            if (_questions.Count == 0) { pnlExam.Visible = false; pnlEmpty.Visible = true; return; }

            // Save question IDs in session so submit can match form fields
            Session["StartExam_QuestionIds"] = string.Join(",", _questions.Select(q => q.QuestionId));

            rptQuestions.DataSource = _questions;
            rptQuestions.DataBind();
        }

        protected void rptQuestions_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            // No-op; markup handles type-based visibility via Eval.
        }

        // ---------- SUBMIT + AUTO-GRADE ----------
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string studentId = Session["UserId"].ToString();
            var db = MongoDbContext.Database;

            var examsCol = db.GetCollection<BsonDocument>("exams");
            var qCol = db.GetCollection<Question>("questions");
            var rCol = db.GetCollection<ExamResult>("results");
            var users = db.GetCollection<BsonDocument>("users");

            var exam = examsCol.Find(Builders<BsonDocument>.Filter.Eq("examId", ExamId)).FirstOrDefault();
            if (exam == null) { Response.Redirect("~/StudentDashboard.aspx"); return; }

            // Pull the question list we used at render time
            string qIdsCsv = Session["StartExam_QuestionIds"] as string ?? "";
            var qIds = qIdsCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var questions = qCol.Find(Builders<Question>.Filter.In(q => q.QuestionId, qIds)).ToList();

            var answers = new Dictionary<string, string>();
            int correct = 0, wrong = 0, notAtt = 0;
            double score = 0, totalMarks = 0;
            bool hasDescriptive = false;

            foreach (var q in questions)
            {
                totalMarks += q.Marks;
                string field = "ans_" + q.QuestionId;
                string given = (Request.Form[field] ?? "").Trim();
                answers[q.QuestionId] = given;

                if (string.IsNullOrEmpty(given))
                {
                    notAtt++;
                    continue;
                }

                if (q.QuestionType == "mcq")
                {
                    if (string.Equals(given, q.CorrectAnswer ?? "",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        correct++;
                        score += q.Marks;
                    }
                    else
                    {
                        wrong++;
                        score -= q.NegativeMarks;
                    }
                }
                else
                {
                    // Descriptive: leave for faculty grading
                    hasDescriptive = true;
                }
            }

            if (score < 0) score = 0;
            double pct = totalMarks > 0 ? (score / totalMarks) * 100.0 : 0;
            bool passed = pct >= 40;

            var me = users.Find(Builders<BsonDocument>.Filter.Eq("userId", studentId)).FirstOrDefault();
            string studentName = me != null && me.Contains("name") ? me["name"].ToString() : studentId;

            var result = new ExamResult
            {
                ResultId = "R" + DateTime.UtcNow.Ticks.ToString().Substring(8),
                StudentId = studentId,
                StudentName = studentName,
                ExamId = ExamId,
                ExamName = exam.GetValue("title", "").ToString(),
                Subject = exam.GetValue("subject", "").ToString(),
                Score = score,
                TotalQuestions = questions.Count,
                TotalMarks = totalMarks,
                CorrectAnswers = correct,
                WrongAnswers = wrong,
                NotAttempted = notAtt,
                Percentage = pct,
                Passed = passed,
                Status = hasDescriptive ? "Pending Review" : "Auto-Graded",
                Answers = answers,
                SubmittedAt = DateTime.UtcNow
            };
            rCol.InsertOne(result);

            // Stash for Result.aspx
            Session["ExamResult"] = result.ToBsonDocument();
            Session.Remove("StartExam_QuestionIds");

            Response.Redirect("~/Result.aspx");
        }
    }
}
