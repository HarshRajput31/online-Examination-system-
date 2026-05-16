using System;
using System.Web.UI;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class SetStudentPassword : Page
    {
        private IMongoCollection<BsonDocument> usersCollection;
        private IMongoCollection<BsonDocument> studentsCollection;

        protected void Page_Load(object sender, EventArgs e)
        {
            ConnectDB();

            if (!IsPostBack)
            {
                string token = Request.QueryString["token"];

                if (string.IsNullOrWhiteSpace(token))
                {
                    DisableForm("No setup token found. Please use the email link.");
                    return;
                }

                hfToken.Value = token;
                LoadStudentByToken(token);
            }
        }

        protected void btnSetPassword_Click(object sender, EventArgs e)
        {
            string token = hfToken.Value;
            var user = FindValidTokenUser(token);

            if (user == null)
            {
                DisableForm("Invalid or expired setup link.");
                return;
            }

            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter a password.");
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Password must be at least 6 characters.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Passwords do not match.");
                return;
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            string studentId = GetStringValue(user, "userId");

            usersCollection.UpdateOne(
                Builders<BsonDocument>.Filter.Eq("_id", user["_id"]),
                Builders<BsonDocument>.Update
                    .Set("passwordHash", hash)
                    .Set("mustSetPassword", false)
                    .Set("isActive", true)
                    .Unset("passwordSetupToken")
                    .Unset("passwordSetupTokenHash")
                    .Unset("passwordSetupTokenExpiresAt"));

            studentsCollection.UpdateOne(
                Builders<BsonDocument>.Filter.Eq("studentId", studentId),
                Builders<BsonDocument>.Update.Set("isActive", true));

            txtPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
            btnSetPassword.Enabled = false;

            lblMsg.Visible = true;
            lblMsg.ForeColor = System.Drawing.Color.LightGreen;
            lblMsg.Text =
                "Password set successfully.<br/><br/>" +
                "Login Email: <b>" + Server.HtmlEncode(GetStringValue(user, "email")) + "</b><br/><br/>" +
                "<a href='Login.aspx'>Go to Login</a>";
        }

        private void LoadStudentByToken(string token)
        {
            var user = FindValidTokenUser(token);

            if (user == null)
            {
                DisableForm("Invalid or expired setup link.");
                return;
            }

            txtLoginEmail.Text = GetStringValue(user, "email");
            btnSetPassword.Enabled = true;
        }

        private BsonDocument FindValidTokenUser(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            string tokenHash = FacultyAccountService.HashToken(token);

            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("passwordSetupTokenHash", tokenHash),
                    Builders<BsonDocument>.Filter.Eq("passwordSetupToken", token)
                ),
                Builders<BsonDocument>.Filter.Eq("roleId", 2),
                Builders<BsonDocument>.Filter.Eq("mustSetPassword", true),
                Builders<BsonDocument>.Filter.Gt("passwordSetupTokenExpiresAt", DateTime.UtcNow)
            );

            return usersCollection.Find(filter).FirstOrDefault();
        }

        private void ConnectDB()
        {
            var db = MongoDbContext.Database;
            usersCollection = db.GetCollection<BsonDocument>("users");
            studentsCollection = db.GetCollection<BsonDocument>("students");
        }

        private void DisableForm(string message)
        {
            btnSetPassword.Enabled = false;
            txtPassword.Enabled = false;
            txtConfirmPassword.Enabled = false;
            ShowError(message);
        }

        private void ShowError(string message)
        {
            lblMsg.Visible = true;
            lblMsg.Text = message;
            lblMsg.ForeColor = System.Drawing.Color.IndianRed;
        }

        private static string GetStringValue(BsonDocument doc, string key)
        {
            if (doc == null || !doc.Contains(key) || doc[key].IsBsonNull)
            {
                return string.Empty;
            }

            return doc[key].ToString();
        }
    }
}
