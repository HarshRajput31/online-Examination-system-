using System;
using System.Web.Services;
using System.Web.UI.WebControls;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class NotificationsPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/Login.aspx"); return; }
            if (!IsPostBack) Load();
        }

        private void Load()
        {
            string id = Session["UserId"].ToString();
            var col = MongoDbContext.GetCollection<Notification>("notifications");
            var list = col.Find(n => n.UserId == id)
                          .SortByDescending(n => n.CreatedAt)
                          .Limit(100)
                          .ToList();

            pnlEmpty.Visible = list.Count == 0;
            rptNotifs.DataSource = list;
            rptNotifs.DataBind();
        }

        protected void rptNotifs_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "MarkRead") return;
            string nid = e.CommandArgument?.ToString();
            NotificationService.MarkRead(nid, Session["UserId"].ToString());
            Load();
        }

        protected void btnMarkAll_Click(object sender, EventArgs e)
        {
            NotificationService.MarkAllRead(Session["UserId"].ToString());
            Load();
        }

        public string IconForType(string type)
        {
            switch (type)
            {
                case "exam_approved":     return "✅";
                case "exam_rejected":     return "❌";
                case "exam_published":    return "📢";
                case "result_published":  return "📊";
                case "recheck_requested": return "🔄";
                case "recheck_assigned":  return "📨";
                case "recheck_completed": return "🎯";
                default: return "🔔";
            }
        }

        // Used by Scripts/notifications.js polling
        [WebMethod(EnableSession = true)]
        public static object GetUnreadCount()
        {
            var ctx = System.Web.HttpContext.Current;
            string id = ctx?.Session?["UserId"]?.ToString();
            if (string.IsNullOrEmpty(id)) return new { count = 0 };
            return new { count = NotificationService.UnreadCount(id) };
        }
    }
}
