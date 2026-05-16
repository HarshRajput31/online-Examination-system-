using System;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem
{
    public partial class ViewFaculty : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "1")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            string id = Request.QueryString["id"];
            if (string.IsNullOrEmpty(id)) { lblNotFound.Visible = true; return; }

            var col = MongoDbContext.GetCollection<Faculty>("faculty");
            var f = col.Find(x => x.FacultyId == id).FirstOrDefault();
            if (f == null) { lblNotFound.Visible = true; return; }

            litId.Text = f.FacultyId;
            litName.Text = Server.HtmlEncode(f.Name ?? "");
            litLoginEmail.Text = Server.HtmlEncode(f.LoginEmail ?? f.Email ?? "");
            litEmail.Text = Server.HtmlEncode(f.PersonalEmail ?? "");
            litDept.Text = Server.HtmlEncode(f.Department ?? "");
            litMobile.Text = Server.HtmlEncode(f.Mobile ?? "");
            litCourse.Text = Server.HtmlEncode(f.Course ?? "");
        }
    }
}
