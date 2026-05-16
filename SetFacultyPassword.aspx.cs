using System;
using System.Web.UI;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class SetFacultyPassword : Page
    {
        private IMongoCollection<BsonDocument> usersCollection;

        protected void Page_Load(object sender, EventArgs e)
        {
            ConnectDB();

            if (!IsPostBack)
            {
                string token = Request.QueryString["token"];

                if (string.IsNullOrWhiteSpace(token))
                {
                    DisableForm("❌ Invalid setup link.");
                    return;
                }

                hfToken.Value = token;
                LoadFacultyByToken(token);
            }
        }

        protected void btnSetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                string token = hfToken.Value;

                var user = FindValidTokenUser(token);

                if (user == null)
                {
                    DisableForm("❌ This link is expired or invalid.");
                    return;
                }

                string password = txtPassword.Text.Trim();
                string confirmPassword = txtConfirmPassword.Text.Trim();

                // =========================
                // 🔐 VALIDATION
                // =========================
                if (string.IsNullOrWhiteSpace(password))
                {
                    ShowError("⚠️ Please enter a password.");
                    return;
                }

                if (password.Length < 6)
                {
                    ShowError("⚠️ Password must be at least 6 characters.");
                    return;
                }

                if (password != confirmPassword)
                {
                    ShowError("⚠️ Passwords do not match.");
                    return;
                }

                // =========================
                // 🔐 HASH PASSWORD
                // =========================
                string hash = BCrypt.Net.BCrypt.HashPassword(password);

                var filter = Builders<BsonDocument>.Filter.Eq("_id", user["_id"]);

                var update = Builders<BsonDocument>.Update
                    .Set("passwordHash", hash)
                    .Set("mustSetPassword", false)
                    .Set("isActive", true)
                    .Set("updatedAt", DateTime.UtcNow) // ✅ added tracking
                    .Unset("passwordSetupToken")
                    .Unset("passwordSetupTokenHash")
                    .Unset("passwordSetupTokenExpiresAt");

                usersCollection.UpdateOne(filter, update);

                // =========================
                // 🔄 RESET UI
                // =========================
                txtPassword.Text = "";
                txtConfirmPassword.Text = "";
                btnSetPassword.Enabled = false;

                string email = GetStringValue(user, "email");

                lblMsg.Visible = true;
                lblMsg.ForeColor = System.Drawing.Color.LightGreen;
                lblMsg.Text =
                    "✅ <b>Password set successfully!</b><br/><br/>" +
                    "Login Email: <b>" + Server.HtmlEncode(email) + "</b><br/><br/>" +
                    "<a href='Login.aspx'>➡ Go to Login</a>";
            }
            catch (Exception ex)
            {
                ShowError("❌ Error: " + ex.Message);
            }
        }

        // =========================
        // 🔍 LOAD USER BY TOKEN
        // =========================
        private void LoadFacultyByToken(string token)
        {
            var user = FindValidTokenUser(token);

            if (user == null)
            {
                DisableForm("❌ Invalid or expired setup link.");
                return;
            }

            txtLoginEmail.Text = GetStringValue(user, "email");
            btnSetPassword.Enabled = true;
        }

        // =========================
        // 🔍 FIND USER BY TOKEN
        // =========================
        private BsonDocument FindValidTokenUser(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            string tokenHash = FacultyAccountService.HashToken(token);

            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("passwordSetupTokenHash", tokenHash),
                    Builders<BsonDocument>.Filter.Eq("passwordSetupToken", token)
                ),
                Builders<BsonDocument>.Filter.Eq("roleId", 3),
                Builders<BsonDocument>.Filter.Eq("mustSetPassword", true),
                Builders<BsonDocument>.Filter.Gt("passwordSetupTokenExpiresAt", DateTime.UtcNow)
            );

            return usersCollection.Find(filter).FirstOrDefault();
        }

        // =========================
        // 🔗 DB CONNECTION
        // =========================
        private void ConnectDB()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("OnlineExamDB");

            usersCollection = db.GetCollection<BsonDocument>("users");
        }

        // =========================
        // ❌ DISABLE FORM
        // =========================
        private void DisableForm(string message)
        {
            btnSetPassword.Enabled = false;
            txtPassword.Enabled = false;
            txtConfirmPassword.Enabled = false;

            ShowError(message);
        }

        // =========================
        // ❗ SHOW ERROR
        // =========================
        private void ShowError(string message)
        {
            lblMsg.Visible = true;
            lblMsg.Text = message;
            lblMsg.ForeColor = System.Drawing.Color.IndianRed;
        }

        // =========================
        // 🔧 SAFE STRING GETTER
        // =========================
        private static string GetStringValue(BsonDocument doc, string key)
        {
            if (doc == null || !doc.Contains(key) || doc[key].IsBsonNull)
                return "";

            return doc[key].ToString();
        }
    }
}