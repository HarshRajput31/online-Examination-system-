using System;
using MongoDB.Driver;
using MongoDB.Bson;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                var database = MongoDbContext.Database;
                var usersCol = database.GetCollection<User>("users");

                string email = txtEmail.Text.Trim().ToLower();
                string question = ddlQuestion.SelectedValue;
                string answer = txtAnswer.Text.Trim();
                string newPassword = txtNewPassword.Text.Trim();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(answer) || string.IsNullOrEmpty(newPassword))
                {
                    ShowMessage("❌ All fields are required.", System.Drawing.Color.OrangeRed);
                    return;
                }

                // 1. Find user by Email AND Security Details
                var filter = Builders<User>.Filter.And(
                    Builders<User>.Filter.Eq(u => u.Email, email),
                    Builders<User>.Filter.Eq("SecurityQuestion", question),
                    Builders<User>.Filter.Eq("SecurityAnswer", answer)
                );

                var user = usersCol.Find(filter).FirstOrDefault();

                if (user != null)
                {
                    // 2. Hash the new password using BCrypt
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                    // 3. Update User document
                    var update = Builders<User>.Update
                        .Set(u => u.PasswordHash, hashedPassword)
                        .Set(u => u.MustSetPassword, false); // User is now active

                    usersCol.UpdateOne(filter, update);

                    ShowMessage("✅ Password reset successful! Redirecting...", System.Drawing.Color.SpringGreen);

                    // Delay redirect to show success message
                    Response.AddHeader("REFRESH", "3;URL=Login.aspx");
                }
                else
                {
                    ShowMessage("❌ Identity verification failed. Check email or answer.", System.Drawing.Color.LightPink);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, System.Drawing.Color.Red);
            }
        }

        private void ShowMessage(string msg, System.Drawing.Color color)
        {
            lblMsg.Text = msg;
            lblMsg.ForeColor = color;
            lblMsg.Visible = true;
        }
    }
}