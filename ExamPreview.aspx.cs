using System;
using System.Collections.Generic;
using MongoDB.Driver;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class ExamPreview : System.Web.UI.Page
    {
        MongoClient client = new MongoClient("mongodb://localhost:27017");

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 Admin Security Check
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "1")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadExams();
            }
        }

        private void LoadExams()
        {
            var database = client.GetDatabase("OnlineExamDB");
            var examCollection = database.GetCollection<Exam>("exams");

            var exams = examCollection.Find(_ => true).ToList();

            ddlExams.DataSource = exams;
            ddlExams.DataTextField = "Title";
            ddlExams.DataValueField = "ExamId";
            ddlExams.DataBind();

            ddlExams.Items.Insert(0, "-- Select Exam --");
        }

        protected void ddlExams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlExams.SelectedIndex == 0)
            {
                rptQuestions.DataSource = null;
                rptQuestions.DataBind();
                return;
            }

            LoadExamQuestions();
        }

        private void LoadExamQuestions()
        {
            try
            {
                var database = client.GetDatabase("OnlineExamDB");

                var examCollection = database.GetCollection<Exam>("exams");
                var questionCollection = database.GetCollection<Question>("questions");

                string examId = ddlExams.SelectedValue;

                var examData = examCollection
                    .Find(x => x.ExamId == examId)
                    .FirstOrDefault();

                // ✅ FIXED HERE
                if (examData == null || examData.QuestionIds == null || examData.QuestionIds.Count == 0)
                {
                    lblMsg.Text = "No questions assigned to this exam.";
                    rptQuestions.DataSource = null;
                    rptQuestions.DataBind();
                    return;
                }

                // 🔥 OPTIMIZED QUERY (NO LOOP)
                var questions = questionCollection
                    .Find(q => examData.QuestionIds.Contains(q.QuestionId))
                    .ToList();

                rptQuestions.DataSource = questions;
                rptQuestions.DataBind();

                lblMsg.Text = "";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "❌ Error: " + ex.Message;
            }
        }
    }
}