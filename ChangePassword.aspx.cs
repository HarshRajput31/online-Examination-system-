using System;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem
{
    public partial class ChangePasswordPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) Response.Redirect("~/Login.aspx");
        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            try
            {
                string current = txtCurrent.Text;
                string fresh = txtNew.Text;
                string confirm = txtConfirm.Text;

                if (string.IsNullOrEmpty(current) || string.IsNullOrEmpty(fresh))
                { Show("Please fill in all fields.", false); return; }
                if (fresh.Length < 6) { Show("New password must be at least 6 chars.", false); return; }
                if (fresh != confirm) { Show("New passwords do not match.", false); return; }

                var users = MongoDbContext.GetCollection<BsonDocument>("users");
                var u = users.Find(Builders<BsonDocument>.Filter.Eq(
                    "userId", Session["UserId"].ToString())).FirstOrDefault();
                if (u == null) { Show("User not found.", false); return; }

                string stored = u.Contains("passwordHash") ? u["passwordHash"].AsString : "";
                bool ok = !string.IsNullOrEmpty(stored) &&
                          (stored.StartsWith("$2") ? BCrypt.Net.BCrypt.Verify(current, stored)
                                                   : current == stored);
                if (!ok) { Show("Current password is incorrect.", false); return; }

                string newHash = BCrypt.Net.BCrypt.HashPassword(fresh);
                users.UpdateOne(
                    Builders<BsonDocument>.Filter.Eq("_id", u["_id"]),
                    Builders<BsonDocument>.Update
                        .Set("passwordHash", newHash)
                        .Set("passwordChangedAt", DateTime.UtcNow));

                Show("✅ Password updated successfully.", true);
                txtCurrent.Text = txtNew.Text = txtConfirm.Text = "";
            }
            catch (Exception ex)
            {
                Show("Error: " + ex.Message, false);
            }
        }

        private void Show(string msg, bool ok)
        {
            lblMsg.Text = msg;
            lblMsg.Visible = true;
            lblMsg.ForeColor = ok ? System.Drawing.Color.LightGreen : System.Drawing.Color.IndianRed;
        }
    }
}
