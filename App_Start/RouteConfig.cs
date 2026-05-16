using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

namespace OnlineExaminationSystem.App_Start
{
    /// <summary>
    /// Friendly-URLs configuration. We let ASP.NET FriendlyUrls
    /// resolve .aspx extensions automatically.
    /// </summary>
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings
            {
                AutoRedirectMode = RedirectMode.Permanent
            };

            routes.EnableFriendlyUrls(settings);
        }
    }
}
