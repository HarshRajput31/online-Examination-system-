using System;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class StudentResults : System.Web.UI.Page
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
            string id = Session["UserId"].ToString();
            var col = MongoDbContext.GetCollection<ExamResult>("results");
            var list = col.Find(r => r.StudentId == id)
                          .SortByDescending(r => r.SubmittedAt)
                          .ToList();

            pnlEmpty.Visible = list.Count == 0;
            gvResults.DataSource = list;
            gvResults.DataBind();
        }
    }
}
