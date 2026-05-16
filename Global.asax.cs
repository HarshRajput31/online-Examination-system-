using System;
using System.Web;
using System.Web.Routing;
using System.Web.UI;

namespace OnlineExaminationSystem
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // ✅ Register jQuery mapping
            ScriptManager.ScriptResourceMapping.AddDefinition(
                "jquery",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/jquery-3.7.0.min.js",
                    DebugPath = "~/Scripts/jquery-3.7.0.js",
                    CdnPath = "https://ajax.googleapis.com/ajax/libs/jquery/3.7.0/jquery.min.js",
                    CdnDebugPath = "https://ajax.googleapis.com/ajax/libs/jquery/3.7.0/jquery.js",
                    CdnSupportsSecureConnection = true,
                    LoadSuccessExpression = "window.jQuery"
                });
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            System.Diagnostics.Debug.WriteLine("App Error: " + ex?.Message);
        }
    }
}