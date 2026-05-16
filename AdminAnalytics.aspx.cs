using System;
using System.Linq;
using MongoDB.Driver;
using OnlineExaminationSystem.Models;
using System.Collections.Generic;

namespace OnlineExaminationSystem
{
    public partial class AdminAnalytics : System.Web.UI.Page
    {
        private readonly MongoClient client =
            new MongoClient("mongodb://localhost:27017");

        protected void Page_Load(object sender, EventArgs e)
        {
            // Admin only
            if (Session["RoleId"] == null ||
                Session["RoleId"].ToString() != "1")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
                LoadAnalytics();
        }

        private void LoadAnalytics()
        {
            var db = client.GetDatabase("OnlineExamDB");

            var userCol = db.GetCollection<User>("users");
            var examCol = db.GetCollection<Exam>("exams");
            var questionCol = db.GetCollection<Question>("questions");
            var resultCol = db.GetCollection<ExamResult>("results");

            var students = userCol.Find(x => x.RoleId == 2).ToList();
            var exams = examCol.Find(_ => true).ToList();
            var questions = questionCol.Find(_ => true).ToList();
            var results = resultCol.Find(_ => true).ToList();

            lblStudents.Text = students.Count.ToString();
            lblExams.Text = exams.Count.ToString();
            lblQuestions.Text = questions.Count.ToString();
            lblAttempts.Text = results.Count.ToString();

            if (results.Count > 0)
            {
                double avg = results.Average(x => x.Score);
                lblAverage.Text = avg.ToString("0.00");

                double passCount =
                    results.Count(x =>
                        (x.Score / x.TotalQuestions) * 100 >= 40);

                double passPercent =
                    (passCount / results.Count) * 100;

                lblPassPercent.Text =
                    passPercent.ToString("0.00") + "%";

                var topStudents = results
                    .OrderByDescending(x => x.Score)
                    .Take(5)
                    .ToList();

                gvTopStudents.DataSource = topStudents;
                gvTopStudents.DataBind();
            }
        }
    }
}