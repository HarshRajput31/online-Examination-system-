using System;
using System.Web.UI;

namespace OnlineExaminationSystem
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If user is already logged in, redirect them to their dashboard.
            if (Session["RoleId"] == null) return;
            string role = Session["RoleId"].ToString();
            switch (role)
            {
                case "1": Response.Redirect("~/AdminDashboard.aspx"); break;
                case "2": Response.Redirect("~/StudentDashboard.aspx"); break;
                case "3": Response.Redirect("~/FacultyDashboard.aspx"); break;
            }
        }
    }
}
