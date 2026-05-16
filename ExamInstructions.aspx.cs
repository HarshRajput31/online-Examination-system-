using System;
using MongoDB.Driver;
using MongoDB.Bson;

namespace OnlineExaminationSystem
{
    public partial class ExamInstructions : System.Web.UI.Page
    {
        private readonly MongoClient client =
            new MongoClient("mongodb://localhost:27017");

        protected void Page_Load(object sender, EventArgs e)
        {
            // ✅ GET ExamId FROM URL
            string examId = Request.QueryString["ExamId"];

            // ❌ If missing → go back
            if (string.IsNullOrEmpty(examId))
            {
                Response.Redirect("StudentDashboard.aspx");
                return;
            }

            // ✅ STORE IN SESSION (VERY IMPORTANT)
            Session["ExamId"] = examId;

            // 🔐 USER CHECK
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadExamDetails();
            }
        }

        private void LoadExamDetails()
        {
            try
            {
                var db = client.GetDatabase("OnlineExamDB");
                var col = db.GetCollection<BsonDocument>("exams");

                string examId = Session["ExamId"].ToString();

                // ✅ HANDLE BOTH CASES (ExamId / examId)
                var filter = Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("ExamId", examId),
                    Builders<BsonDocument>.Filter.Eq("examId", examId)
                );

                var exam = col.Find(filter).FirstOrDefault();

                if (exam == null)
                {
                    lblMsg.Text = "❌ Exam not found";
                    return;
                }

                // ✅ SAFE FIELD READING
                lblTitle.Text = exam.Contains("Title")
                    ? exam["Title"].ToString()
                    : exam.GetValue("title", "N/A").ToString();

                lblDuration.Text = exam.Contains("Duration")
                    ? exam["Duration"].ToString()
                    : exam.GetValue("duration", 0).ToString();

                lblMarks.Text = exam.Contains("TotalMarks")
                    ? exam["TotalMarks"].ToString()
                    : exam.GetValue("totalMarks", 0).ToString();

                // ✅ QUESTION COUNT FIX (MOST IMPORTANT)
                if (exam.Contains("QuestionIds"))
                    lblQuestions.Text = exam["QuestionIds"].AsBsonArray.Count.ToString();
                else if (exam.Contains("questions"))
                    lblQuestions.Text = exam["questions"].AsBsonArray.Count.ToString();
                else
                    lblQuestions.Text = "0";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "❌ Error: " + ex.Message;
            }
        }

        // ✅ FINAL BUTTON (100% WORKING)
        protected void btnStartExam_Click(object sender, EventArgs e)
        {
            if (Session["ExamId"] == null)
            {
                lblMsg.Text = "❌ Session expired. Try again.";
                return;
            }

            // 🔥 FINAL REDIRECT
            Response.Redirect("StartExam.aspx?ExamId=" + Session["ExamId"]);
        }
    }
}