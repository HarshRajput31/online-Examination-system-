using System;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class StudentExamHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "2")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack) Load();
        }

        private void Load()
        {
            var col = MongoDbContext.GetCollection<ExamResult>("results");
            var list = col.Find(r => r.StudentId == Session["UserId"].ToString())
                          .SortByDescending(r => r.SubmittedAt)
                          .ToList();

            pnlEmpty.Visible = list.Count == 0;
            rptHistory.DataSource = list;
            rptHistory.DataBind();
        }
    }
}
