using System;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem
{
    public partial class ViewStudent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/Login.aspx"); return; }
            string id = Request.QueryString["id"];
            if (string.IsNullOrEmpty(id)) { lblNotFound.Visible = true; return; }

            var col = MongoDbContext.GetCollection<BsonDocument>("users");
            var d = col.Find(Builders<BsonDocument>.Filter.Eq("userId", id)).FirstOrDefault();
            if (d == null) { lblNotFound.Visible = true; return; }

            litId.Text = id;
            litName.Text = Get(d, "name");
            litEmail.Text = Get(d, "email");
            litRoll.Text = Get(d, "rollNumber");
            litCourse.Text = Get(d, "course");
            litDept.Text = Get(d, "department");
            litMobile.Text = Get(d, "mobile");
        }

        private static string Get(BsonDocument d, string k) =>
            d.Contains(k) && !d[k].IsBsonNull ? d[k].ToString() : "";
    }
}
