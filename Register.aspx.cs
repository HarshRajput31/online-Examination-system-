using System;
using System.Web.UI;

namespace OnlineExaminationSystem
{
    /// <summary>Spec alias - forwards to StudentRegistration.</summary>
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/StudentRegistration.aspx");
        }
    }
}
