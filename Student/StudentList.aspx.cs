using System;
using System.Linq;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem.Student
{
    public partial class StudentList : System.Web.UI.Page
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

        private void Load()
        {
            var col = MongoDbContext.GetCollection<BsonDocument>("users");
            var list = col.Find(Builders<BsonDocument>.Filter.Eq("roleId", 2))
                .SortByDescending(d => d["createdAt"])
                .ToList()
                .Select(d => new
                {
                    UserId = d.GetValue("userId", "").ToString(),
                    Name = d.GetValue("name", "").ToString(),
                    Email = d.GetValue("email", "").ToString(),
                    Department = d.GetValue("department", "").ToString(),
                    Course = d.GetValue("course", "").ToString(),
                    IsBlocked = d.GetValue("isBlocked", false).ToBoolean()
                }).ToList();

            pnlEmpty.Visible = list.Count == 0;
            gvStudents.DataSource = list;
            gvStudents.DataBind();
        }

        protected void gvStudents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string id = e.CommandArgument?.ToString();
            if (string.IsNullOrEmpty(id)) return;
            var users = MongoDbContext.GetCollection<BsonDocument>("users");

            if (e.CommandName == "DeleteUser")
            {
                users.DeleteOne(Builders<BsonDocument>.Filter.Eq("userId", id));
                lblMsg.Text = "✅ Student deleted.";
            }
            else if (e.CommandName == "ToggleBlock")
            {
                var doc = users.Find(Builders<BsonDocument>.Filter.Eq("userId", id)).FirstOrDefault();
                bool now = doc != null && doc.GetValue("isBlocked", false).ToBoolean();
                users.UpdateOne(
                    Builders<BsonDocument>.Filter.Eq("userId", id),
                    Builders<BsonDocument>.Update.Set("isBlocked", !now));
                lblMsg.Text = !now ? "🚫 Student blocked." : "✅ Student unblocked.";
            }

            lblMsg.Visible = true;
            lblMsg.ForeColor = System.Drawing.Color.LightGreen;
            Load();
        }
    }
}
