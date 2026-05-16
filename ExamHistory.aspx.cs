using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongoDB.Driver;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class ExamHistory : System.Web.UI.Page
    {
        private readonly MongoClient client =
            new MongoClient("mongodb://localhost:27017");

        protected void Page_Load(object sender, EventArgs e)
        {
            // 🔐 SESSION PROTECTION
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadHistory();
            }
        }

        private void LoadHistory()
        {
            try
            {
                var database = client.GetDatabase("OnlineExamDB");

                var resultCollection =
                    database.GetCollection<ExamResult>("results");

                string studentId = Session["UserId"].ToString();

                // 🔥 CASE-INSENSITIVE + SORT (LATEST FIRST)
                List<ExamResult> history =
                    resultCollection
                    .Find(x => x.StudentId.ToLower() == studentId.ToLower())
                    .SortByDescending(x => x.SubmittedAt)
                    .ToList();

                if (history == null || history.Count == 0)
                {
                    lblMsg.Text = "No exam attempts found.";
                    gvHistory.DataSource = null;
                    gvHistory.DataBind();
                    return;
                }

                lblMsg.Text = ""; // clear message

                gvHistory.DataSource = history;
                gvHistory.DataBind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
            }
        }

        // 🔥 FINAL STATUS LOGIC (CORRECT CALCULATION)
        protected void gvHistory_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");

                if (lblStatus == null) return;

                var data = (ExamResult)e.Row.DataItem;

                double percentage = 0;

                if (data.TotalQuestions > 0)
                {
                    // 🔥 FIX: DOUBLE CALCULATION (IMPORTANT)
                    percentage = ((double)data.Score / data.TotalQuestions) * 100;
                }

                // ✅ PASS / FAIL
                if (percentage >= 40)
                {
                    lblStatus.Text = "PASS";
                    lblStatus.CssClass = "status-badge status-pass";
                }
                else
                {
                    lblStatus.Text = "FAIL";
                    lblStatus.CssClass = "status-badge status-fail";
                }
            }
        }
    }
}