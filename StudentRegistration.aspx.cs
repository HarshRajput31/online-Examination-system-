using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class StudentRegistration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                string email = txtEmail.Text.Trim().ToLower();
                string pwd = txtPassword.Text;
                string confirm = txtConfirm.Text;

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                { Show("Name and Email are required.", false); return; }
                if (pwd.Length < 6)
                { Show("Password must be at least 6 characters.", false); return; }
                if (pwd != confirm)
                { Show("Passwords do not match.", false); return; }

                var db = MongoDbContext.Database;
                var users = db.GetCollection<BsonDocument>("users");
                var students = db.GetCollection<BsonDocument>("students");

                // Duplicate check
                if (users.Find(Builders<BsonDocument>.Filter.Eq("email", email)).Any())
                { Show("An account with that email already exists. Try logging in.", false); return; }

                // Generate StudentId (STU001, ...)
                string studentId = GenerateStudentId(students);
                string hash = BCrypt.Net.BCrypt.HashPassword(pwd);

                users.InsertOne(new BsonDocument
                {
                    { "userId", studentId },
                    { "name", name },
                    { "email", email },
                    { "passwordHash", hash },
                    { "roleId", 2 },
                    { "role", "Student" },
                    { "rollNumber", txtRoll.Text.Trim() },
                    { "mobile", txtMobile.Text.Trim() },
                    { "course", txtCourse.Text.Trim() },
                    { "department", txtDept.Text.Trim() },
                    { "isActive", true },
                    { "isBlocked", false },
                    { "mustSetPassword", false },
                    { "createdAt", DateTime.UtcNow }
                });

                students.InsertOne(new BsonDocument
                {
                    { "studentId", studentId },
                    { "name", name },
                    { "email", email },
                    { "rollNumber", txtRoll.Text.Trim() },
                    { "mobile", txtMobile.Text.Trim() },
                    { "course", txtCourse.Text.Trim() },
                    { "department", txtDept.Text.Trim() },
                    { "isActive", true },
                    { "createdAt", DateTime.UtcNow }
                });

                Show("Account created. Redirecting to login...", true);
                Response.AddHeader("REFRESH", "2;URL=Login.aspx");
            }
            catch (Exception ex)
            {
                Show("Error: " + ex.Message, false);
            }
        }

        private static string GenerateStudentId(IMongoCollection<BsonDocument> col)
        {
            var latest = col.Find(Builders<BsonDocument>.Filter.Regex("studentId", "^STU"))
                            .Sort(Builders<BsonDocument>.Sort.Descending("studentId"))
                            .Limit(1)
                            .FirstOrDefault();
            int next = 1;
            if (latest != null && latest.Contains("studentId"))
            {
                string num = new string(latest["studentId"].AsString.Where(char.IsDigit).ToArray());
                if (int.TryParse(num, out int p)) next = p + 1;
            }
            return "STU" + next.ToString("D3");
        }

        private void Show(string msg, bool ok)
        {
            lblMsg.Text = (ok ? "✅ " : "❌ ") + msg;
            lblMsg.Visible = true;
            lblMsg.ForeColor = ok ? System.Drawing.Color.LightGreen : System.Drawing.Color.IndianRed;
        }
    }
}
