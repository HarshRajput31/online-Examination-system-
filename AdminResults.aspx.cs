using System;
using MongoDB.Driver;
using OnlineExaminationSystem.Models;
using System.Collections.Generic;

namespace OnlineExaminationSystem
{
    public partial class AdminResults : System.Web.UI.Page
    {
        private readonly MongoClient client =
            new MongoClient("mongodb://localhost:27017");

        protected void Page_Load(object sender, EventArgs e)
        {
            // Admin security
            if (Session["UserId"] == null || Session["RoleId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Convert.ToInt32(Session["RoleId"]) != 1)
            {
                Response.Redirect("StudentDashboard.aspx");
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
                var database = client.GetDatabase("OnlineExamDB");

                var resultCollection =
                    database.GetCollection<ExamResult>("results");

                var results = resultCollection.Find(_ => true).ToList();

                if (results.Count == 0)
                {
                    lblMsg.Text = "No exam results found.";
                    return;
                }

                gvResults.DataSource = results;
                gvResults.DataBind();
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.Message;
            }
        }
    }
}