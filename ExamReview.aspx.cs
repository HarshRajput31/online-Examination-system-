using System;
using System.Collections.Generic;
using MongoDB.Driver;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class ExamReview : System.Web.UI.Page
    {
        private readonly MongoClient client =
            new MongoClient("mongodb://localhost:27017");

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 Login protection
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // 🔐 Exam protection
            if (Session["ExamId"] == null)
            {
                Response.Redirect("StudentDashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadReview();
            }
        }

        private void LoadReview()
        {
            try
            {
                var database = client.GetDatabase("OnlineExamDB");

                var examCollection = database.GetCollection<Exam>("exams");
                var questionCollection = database.GetCollection<Question>("questions");
                var resultCollection = database.GetCollection<ExamResult>("results");

                string studentId = Session["UserId"].ToString();
                string examId = Session["ExamId"].ToString();

                // ✅ Get latest result
                var result = resultCollection
                    .Find(x => x.StudentId == studentId && x.ExamId == examId)
                    .SortByDescending(x => x.SubmittedAt)
                    .FirstOrDefault();

                if (result == null)
                {
                    lblMsg.Text = "❌ No attempt found for review.";
                    return;
                }

                var exam = examCollection
                    .Find(x => x.ExamId == examId)
                    .FirstOrDefault();

                if (exam == null || exam.QuestionIds == null || exam.QuestionIds.Count == 0)
                {
                    lblMsg.Text = "❌ Exam data not found.";
                    return;
                }

                // 🔥 Get all questions
                var questions = questionCollection
                    .Find(q => exam.QuestionIds.Contains(q.QuestionId))
                    .ToList();

                List<ReviewItem> reviewList = new List<ReviewItem>();
                double runningScore = 0;

                foreach (var question in questions)
                {
                    string qId = question.QuestionId;

                    string yourAnswer = "Not Answered";

                    if (result.Answers != null && result.Answers.ContainsKey(qId))
                        yourAnswer = result.Answers[qId];

                    double marksAwarded = 0;

                    // ✅ MARKING LOGIC
                    if (yourAnswer == question.CorrectAnswer)
                    {
                        marksAwarded = question.Marks;
                        runningScore += question.Marks;
                    }
                    else if (yourAnswer != "Not Answered")
                    {
                        marksAwarded = -question.NegativeMarks;
                        runningScore -= question.NegativeMarks;
                    }

                    string resultText;

                    if (yourAnswer == "Not Answered")
                        resultText = "Not Answered";
                    else if (yourAnswer == question.CorrectAnswer)
                        resultText = "✅ Correct";
                    else
                        resultText = "❌ Wrong";

                    reviewList.Add(new ReviewItem
                    {
                        QuestionText = question.QuestionText,
                        StudentAnswer = yourAnswer,   // ✅ FIXED NAME (GridView match)
                        CorrectAnswer = question.CorrectAnswer,
                        Result = resultText,          // ✅ FIXED NAME (GridView match)

                        Marks = question.Marks,
                        NegativeMarks = question.NegativeMarks,
                        MarksAwarded = marksAwarded
                    });
                }

                gvReview.DataSource = reviewList;
                gvReview.DataBind();

                lblMsg.Text = "🎯 Final Score: " + runningScore.ToString("0.##");
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
            }
        }
    }

    // ✅ MODEL FOR GRIDVIEW (IMPORTANT)
    public class ReviewItem
    {
        public string QuestionText { get; set; }
        public string StudentAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public string Result { get; set; }

        public double Marks { get; set; }
        public double NegativeMarks { get; set; }
        public double MarksAwarded { get; set; }
    }
}