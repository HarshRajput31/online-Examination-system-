using System;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem.Student
{
    public partial class FacultyList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Admin-only
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "1")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack) Load();
        }

        private void Load()
        {
            var col = MongoDbContext.GetCollection<Faculty>("faculty");
            var list = col.Find(_ => true)
                          .SortByDescending(f => f.CreatedAt)
                          .ToList();

            pnlEmpty.Visible = list.Count == 0;
            gvFaculty.DataSource = list;
            gvFaculty.DataBind();
        }

        protected void gvFaculty_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteFac") return;
            string id = e.CommandArgument?.ToString();
            if (string.IsNullOrEmpty(id)) return;

            var fac = MongoDbContext.GetCollection<Faculty>("faculty");
            var users = MongoDbContext.GetCollection<BsonDocument>("users");

            fac.DeleteOne(Builders<Faculty>.Filter.Eq(f => f.FacultyId, id));
            users.DeleteOne(Builders<BsonDocument>.Filter.Eq("userId", id));

            lblMsg.Text = "✅ Faculty removed.";
            lblMsg.Visible = true;
            lblMsg.ForeColor = System.Drawing.Color.LightGreen;
            Load();
        }
    }
}
