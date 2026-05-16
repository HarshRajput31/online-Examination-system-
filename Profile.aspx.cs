using System;
using System.Web.UI;

namespace OnlineExaminationSystem
{
    /// <summary>Role-aware profile redirect.</summary>
    public partial class ProfilePage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Session["RoleId"]?.ToString();
            switch (role)
            {
                case "1": Response.Redirect("~/AdminDashboard.aspx"); break;
                case "2": Response.Redirect("~/StudentProfile.aspx"); break;
                case "3": Response.Redirect("~/FacultyProfile.aspx"); break;
                default:  Response.Redirect("~/Login.aspx"); break;
            }
        }
    }
}
