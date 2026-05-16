using System;
using System.Collections.Generic;
using MongoDB.Driver;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class MyResults : System.Web.UI.Page
    {
        private readonly MongoClient client =
            new MongoClient("mongodb://localhost:27017");

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 LOGIN CHECK
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadResults();
            }
        }

        private void LoadResults()
        {
            try
            {
                var db = client.GetDatabase("OnlineExamDB");
                var resultCollection = db.GetCollection<ExamResult>("results");

                string studentId = Session["UserId"].ToString();

                var results = resultCollection
                    .Find(x => x.StudentId == studentId)
                    .SortByDescending(x => x.SubmittedAt)
                    .ToList();

                if (results.Count == 0)
                {
                    lblMsg.Text = "No results found.";
                    return;
                }

                List<object> resultList = new List<object>();

                foreach (var r in results)
                {
                    // ✅ CALCULATE PERCENTAGE SAFELY
                    double percentage = (r.TotalQuestions == 0)
                        ? 0
                        : (r.Score / r.TotalQuestions) * 100;

                    resultList.Add(new
                    {
                        ExamId = r.ExamId,
                        ExamName = r.ExamName,

                        // ✅ FIX (since Subject not in model)
                        Subject = r.ExamName,

                        Score = r.Score,

                        // ✅ FORMAT PERCENTAGE
                        Percentage = percentage.ToString("0.00") + "%",

                        // ✅ PASS / FAIL LOGIC
                        Status = percentage >= 40 ? "PASS" : "FAIL",

                        // ✅ FIX DATE (already string)
                        Date = r.SubmittedAt
                    });
                }

                gvResults.DataSource = resultList;
                gvResults.DataBind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
            }
        }
    }
}