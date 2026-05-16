using System;
using System.Web.UI;

namespace OnlineExaminationSystem
{
    /// <summary>Spec alias - forwards to Student/FacultyList.</summary>
    public partial class ManageFaculty : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Student/FacultyList.aspx");
        }
    }
}
