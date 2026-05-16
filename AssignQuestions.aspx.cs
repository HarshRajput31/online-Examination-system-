using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using MongoDB.Driver;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class AssignQuestions : System.Web.UI.Page
    {
        // MongoDB Client
        private readonly MongoClient client =
            new MongoClient("mongodb://localhost:27017");

        // Database reference
        private IMongoDatabase database;

        protected void Page_Load(object sender, EventArgs e)
        {
            // ============================
            // ADMIN SECURITY CHECK
            // ============================
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "1")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            database = client.GetDatabase("OnlineExamDB");

            if (!IsPostBack)
            {
                LoadExams();
                LoadQuestions();
            }
        }

        // ==================================================
        // LOAD EXAMS INTO DROPDOWN
        // ==================================================
        private void LoadExams()
        {
            try
            {
                var examCollection =
                    database.GetCollection<Exam>("exams");

                var exams = examCollection.Find(_ => true).ToList();

                ddlExams.DataSource = exams;
                ddlExams.DataTextField = "Title";
                ddlExams.DataValueField = "ExamId";
                ddlExams.DataBind();

                ddlExams.Items.Insert(0,
                    new ListItem("-- Select Exam --", ""));
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error loading exams: " + ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        // ==================================================
        // LOAD QUESTIONS INTO GRIDVIEW
        // ==================================================
        private void LoadQuestions()
        {
            try
            {
                var questionCollection =
                    database.GetCollection<Question>("questions");

                var questions = questionCollection
                    .Find(_ => true)
                    .ToList();

                gvQuestions.DataSource = questions;
                gvQuestions.DataBind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error loading questions: " + ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        // ==================================================
        // EXAM DROPDOWN CHANGE EVENT
        // ==================================================
        protected void ddlExams_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMsg.Text = "";
        }

        // ==================================================
        // SAVE ASSIGNED QUESTIONS
        // ==================================================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // ===============================
                // VALIDATE EXAM SELECTION
                // ===============================
                if (string.IsNullOrEmpty(ddlExams.SelectedValue))
                {
                    lblMsg.Text = "Please select an exam!";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                List<string> selectedQuestions = new List<string>();

                // ===============================
                // READ CHECKED QUESTIONS
                // ===============================
                foreach (GridViewRow row in gvQuestions.Rows)
                {
                    CheckBox chk =
                        (CheckBox)row.FindControl("chkSelect");

                    if (chk != null && chk.Checked)
                    {
                        // Column index of QuestionId
                        string questionId = row.Cells[1].Text.Trim();

                        if (!string.IsNullOrEmpty(questionId))
                        {
                            selectedQuestions.Add(questionId);
                        }
                    }
                }

                if (selectedQuestions.Count == 0)
                {
                    lblMsg.Text = "Please select at least one question!";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                // ===============================
                // UPDATE EXAM DOCUMENT
                // ===============================
                var examCollection =
                    database.GetCollection<Exam>("exams");

                var filter =
                    Builders<Exam>.Filter.Eq("examId",
                        ddlExams.SelectedValue);

                var update =
                    Builders<Exam>.Update.Set("questions",
                        selectedQuestions);

                examCollection.UpdateOne(filter, update);

                lblMsg.Text = "Questions assigned successfully!";
                lblMsg.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}