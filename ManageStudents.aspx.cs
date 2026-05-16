using System;
using System.Web.UI;

namespace OnlineExaminationSystem
{
    /// <summary>Spec alias - just forwards to the existing Student/StudentList page.</summary>
    public partial class ManageStudents : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Student/StudentList.aspx");
        }
    }
}
