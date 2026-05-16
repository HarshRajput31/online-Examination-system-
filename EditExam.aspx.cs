using System;
using MongoDB.Driver;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class EditExam : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // ✅ Admin Security Check
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "1")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string examId = Request.QueryString["examId"];

                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("OnlineExamDB");

                var examCollection = database.GetCollection<Exam>("exams");

                var exam = examCollection
                    .Find(x => x.ExamId == examId)
                    .FirstOrDefault();

                if (exam != null)
                {
                    hfExamId.Value = exam.ExamId;
                    txtTitle.Text = exam.Title;
                    txtDuration.Text = exam.Duration.ToString();
                    txtMarks.Text = exam.TotalMarks.ToString();
                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("OnlineExamDB");

            var examCollection = database.GetCollection<Exam>("exams");

            var filter = Builders<Exam>.Filter.Eq("examId", hfExamId.Value);

            var update = Builders<Exam>.Update
                .Set("title", txtTitle.Text.Trim())
                .Set("duration", Convert.ToInt32(txtDuration.Text.Trim()))
                .Set("totalMarks", Convert.ToInt32(txtMarks.Text.Trim()));

            examCollection.UpdateOne(filter, update);

            lblMsg.Text = "Exam Updated Successfully!";
        }
    }
}
