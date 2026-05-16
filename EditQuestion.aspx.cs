using System;
using MongoDB.Driver;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class EditQuestion : System.Web.UI.Page
    {
        // ✅ Reusable Mongo Client (Best Practice)
        MongoClient client = new MongoClient("mongodb://localhost:27017");

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
                LoadQuestion();
            }
        }

        private void LoadQuestion()
        {
            string questionId = Request.QueryString["id"];

            if (string.IsNullOrEmpty(questionId))
            {
                lblMsg.Text = "Invalid Question!";
                return;
            }

            var database = client.GetDatabase("OnlineExamDB");
            var questionCollection = database.GetCollection<Question>("questions");

            var question = questionCollection
                .Find(q => q.QuestionId == questionId)
                .FirstOrDefault();

            if (question == null)
            {
                lblMsg.Text = "Question not found!";
                return;
            }

            // ✅ Populate Controls
            txtQuestion.Text = question.QuestionText;
            txtA.Text = question.OptionA;
            txtB.Text = question.OptionB;
            txtC.Text = question.OptionC;
            txtD.Text = question.OptionD;
            txtCorrect.Text = question.CorrectAnswer;

            ddlDifficulty.SelectedValue = question.Difficulty;

            txtMarks.Text = question.Marks.ToString();
            txtNegative.Text = question.NegativeMarks.ToString();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string questionId = Request.QueryString["id"];

                if (string.IsNullOrEmpty(questionId))
                {
                    lblMsg.Text = "Invalid Update Request!";
                    return;
                }

                var database = client.GetDatabase("OnlineExamDB");
                var questionCollection = database.GetCollection<Question>("questions");

                // ✅ Strong Input Validation
                double marks, negativeMarks;

                if (!double.TryParse(txtMarks.Text, out marks) ||
                    !double.TryParse(txtNegative.Text, out negativeMarks))
                {
                    lblMsg.Text = "Marks values must be numeric!";
                    return;
                }

                var filter = Builders<Question>.Filter.Eq("questionId", questionId);

                var update = Builders<Question>.Update
                    .Set("questionText", txtQuestion.Text.Trim())
                    .Set("optionA", txtA.Text.Trim())
                    .Set("optionB", txtB.Text.Trim())
                    .Set("optionC", txtC.Text.Trim())
                    .Set("optionD", txtD.Text.Trim())
                    .Set("correctAnswer", txtCorrect.Text.Trim().ToUpper())
                    .Set("difficulty", ddlDifficulty.SelectedValue)
                    .Set("marks", marks)
                    .Set("negativeMarks", negativeMarks);

                questionCollection.UpdateOne(filter, update);

                lblMsg.Text = "Question Updated Successfully!";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
            }
        }
    }
}