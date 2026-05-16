using System;
using System.Linq;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem
{
    public partial class UserManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "1")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack) Load();
        }

        protected void btnFilter_Click(object sender, EventArgs e) => Load();

        private void Load()
        {
            var col = MongoDbContext.GetCollection<BsonDocument>("users");
            var fb = Builders<BsonDocument>.Filter;
            var filter = fb.Empty;

            string q = (txtSearch.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(q))
            {
                var rx = new BsonRegularExpression(q, "i");
                filter &= fb.Or(fb.Regex("name", rx), fb.Regex("email", rx));
            }
            string role = ddlRole.SelectedValue;
            if (!string.IsNullOrEmpty(role) && int.TryParse(role, out int rid))
            {
                filter &= fb.Eq("roleId", rid);
            }

            var rows = col.Find(filter).Limit(200).ToList()
                .Select(d => new
                {
                    UserId = d.GetValue("userId", "").ToString(),
                    Name = d.GetValue("name", "").ToString(),
                    Email = d.GetValue("email", "").ToString(),
                    Role = RoleName(d.GetValue("roleId", 0).ToInt32()),
                    Blocked = d.GetValue("isBlocked", false).ToBoolean()
                }).ToList();

            gvUsers.DataSource = rows;
            gvUsers.DataBind();
        }

        private static string RoleName(int id)
        {
            switch (id) { case 1: return "Admin"; case 2: return "Student"; case 3: return "Faculty"; default: return "Unknown"; }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Toggle") return;
            string id = e.CommandArgument?.ToString();
            if (string.IsNullOrEmpty(id)) return;

            var col = MongoDbContext.GetCollection<BsonDocument>("users");
            var doc = col.Find(Builders<BsonDocument>.Filter.Eq("userId", id)).FirstOrDefault();
            bool now = doc != null && doc.GetValue("isBlocked", false).ToBoolean();
            col.UpdateOne(
                Builders<BsonDocument>.Filter.Eq("userId", id),
                Builders<BsonDocument>.Update.Set("isBlocked", !now));

            lblMsg.Text = !now ? "🚫 User blocked." : "✅ User unblocked.";
            lblMsg.Visible = true;
            lblMsg.ForeColor = System.Drawing.Color.LightGreen;
            Load();
        }
    }
}
