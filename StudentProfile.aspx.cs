using System;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem
{
    public partial class StudentProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "2")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack) Load();
        }

        private void Load()
        {
            var users = MongoDbContext.GetCollection<BsonDocument>("users");
            var u = users.Find(Builders<BsonDocument>.Filter.Eq(
                "userId", Session["UserId"].ToString())).FirstOrDefault();
            if (u == null) return;

            txtStudentId.Text = Get(u, "userId");
            txtEmail.Text = Get(u, "email");
            txtName.Text = Get(u, "name");
            txtMobile.Text = Get(u, "mobile");
            txtCourse.Text = Get(u, "course");
            txtDept.Text = Get(u, "department");
            txtRoll.Text = Get(u, "rollNumber");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var users = MongoDbContext.GetCollection<BsonDocument>("users");
                var students = MongoDbContext.GetCollection<BsonDocument>("students");
                string id = Session["UserId"].ToString();

                var update = Builders<BsonDocument>.Update
                    .Set("name", txtName.Text.Trim())
                    .Set("mobile", txtMobile.Text.Trim())
                    .Set("course", txtCourse.Text.Trim())
                    .Set("department", txtDept.Text.Trim())
                    .Set("rollNumber", txtRoll.Text.Trim())
                    .Set("updatedAt", DateTime.UtcNow);

                users.UpdateOne(Builders<BsonDocument>.Filter.Eq("userId", id), update);
                students.UpdateOne(Builders<BsonDocument>.Filter.Eq("studentId", id), update);

                lblMsg.Text = "✅ Profile updated successfully.";
                lblMsg.Visible = true;
                lblMsg.ForeColor = System.Drawing.Color.LightGreen;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "❌ " + ex.Message;
                lblMsg.Visible = true;
                lblMsg.ForeColor = System.Drawing.Color.IndianRed;
            }
        }

        private static string Get(BsonDocument d, string k) =>
            d.Contains(k) && !d[k].IsBsonNull ? d[k].ToString() : "";
    }
}
