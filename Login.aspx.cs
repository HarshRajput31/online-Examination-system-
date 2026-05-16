using System;
using System.Web.UI;
using MongoDB.Driver;
using MongoDB.Bson; // Added for BsonDocument
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear any leftover session if they land on login page
                Session.Clear();
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var database = MongoDbContext.Database;
                var users = database.GetCollection<User>("users");
                // Use BsonDocument for audits to ensure field name consistency with the Dashboard chart
                var audits = database.GetCollection<BsonDocument>("login_audit");

                string inputEmail = txtEmail.Text.Trim().ToLower();
                string inputPassword = txtPassword.Text.Trim();
                string ip = Request.UserHostAddress;

                if (string.IsNullOrEmpty(inputEmail) || string.IsNullOrEmpty(inputPassword))
                {
                    ShowError("❌ Please enter both Email and Password.");
                    return;
                }

                // 🔍 FIND USER
                var user = users.Find(u => u.Email == inputEmail).FirstOrDefault();

                if (user == null)
                {
                    ShowError("❌ User not found!");
                    return;
                }

                // 🔐 CHECK IF PASSWORD IS NOT SET (For Invited Users)
                if ((user.RoleId == 2 || user.RoleId == 3) && user.MustSetPassword)
                {
                    ShowError("⚠️ Account not activated. Please use 'Forgot Password' to set your first password.");
                    return;
                }

                // 🔑 PASSWORD VALIDATION
                bool isValid = false;
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    if (user.PasswordHash.StartsWith("$2"))
                    {
                        // Secure BCrypt check
                        isValid = BCrypt.Net.BCrypt.Verify(inputPassword, user.PasswordHash);
                    }
                    else
                    {
                        // Fallback plain-text check
                        isValid = inputPassword == user.PasswordHash;
                    }
                }

                if (!isValid)
                {
                    ShowError("❌ Invalid password!");
                    return;
                }

                // 🚫 ACCOUNT STATUS CHECK
                if (!user.IsActive || user.IsBlocked)
                {
                    ShowError("🚫 Account disabled! Contact Admin.");
                    return;
                }

                // 📝 LOGIN AUDIT (Fixed for Admin Dashboard Compatibility)
                var auditRecord = new BsonDocument
                {
                    { "userId", user.UserId },
                    { "email", user.Email },
                    { "loginTime", DateTime.UtcNow }, // Use 'loginTime' lowercase to match AdminDashboard WebMethod
                    { "ipAddress", ip },
                    { "status", "Success" }
                };
                audits.InsertOne(auditRecord);

                // 🔄 UPDATE LAST LOGIN DATE
                users.UpdateOne(
                    Builders<User>.Filter.Eq(u => u.Email, user.Email),
                    Builders<User>.Update.Set(u => u.LastLogin, DateTime.Now)
                );

                // 🧠 SESSION SETUP
                Session["UserId"] = user.UserId.ToUpper();
                Session["Email"] = user.Email;
                Session["RoleId"] = user.RoleId.ToString(); // Store as string for easier comparison

                if (user.RoleId == 3) // Faculty
                {
                    Session["FacultyId"] = user.UserId.ToUpper();
                }

                // 🚀 REDIRECT
                RedirectUser(user.RoleId);
            }
            catch (Exception ex)
            {
                ShowError("Login error: " + ex.Message);
            }
        }

        private void RedirectUser(int roleId)
        {
            switch (roleId)
            {
                case 1: Response.Redirect("AdminDashboard.aspx"); break;
                case 2: Response.Redirect("StudentDashboard.aspx"); break;
                case 3: Response.Redirect("FacultyDashboard.aspx"); break;
                default: ShowError("❌ Unknown Role."); break;
            }
        }

        private void ShowError(string message)
        {
            lblMsg.Text = message;
            lblMsg.Visible = true;
        }
    }
}