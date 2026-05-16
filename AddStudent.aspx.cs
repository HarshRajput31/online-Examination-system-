using System;
using System.Configuration;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class AddStudent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "1")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                string email = txtEmail.Text.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                { Show("Name and email are required.", false); return; }

                var users = MongoDbContext.GetCollection<BsonDocument>("users");
                var students = MongoDbContext.GetCollection<BsonDocument>("students");

                if (users.Find(Builders<BsonDocument>.Filter.Eq("email", email)).Any())
                { Show("A user already exists with that email.", false); return; }

                string studentId = GenerateStudentId(students);
                string token = GenerateToken();
                string hash = FacultyAccountService.HashToken(token);

                users.InsertOne(new BsonDocument
                {
                    { "userId", studentId },
                    { "name", name },
                    { "email", email },
                    { "personalEmail", email },
                    { "roleId", 2 },
                    { "role", "Student" },
                    { "rollNumber", txtRoll.Text.Trim() },
                    { "mobile", txtMobile.Text.Trim() },
                    { "course", txtCourse.Text.Trim() },
                    { "department", txtDept.Text.Trim() },
                    { "isActive", false },
                    { "isBlocked", false },
                    { "mustSetPassword", true },
                    { "passwordSetupToken", token },
                    { "passwordSetupTokenHash", hash },
                    { "passwordSetupTokenExpiresAt", DateTime.UtcNow.AddHours(72) },
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
                    { "isActive", false },
                    { "createdAt", DateTime.UtcNow }
                });

                string baseUrl = ConfigurationManager.AppSettings["AppBaseUrl"] ??
                                 Request.Url.GetLeftPart(UriPartial.Authority);
                string link = baseUrl.TrimEnd('/') +
                              "/SetStudentPassword.aspx?token=" + Uri.EscapeDataString(token);

                try
                {
                    EmailService.SendInviteEmail(email, name, email, link);
                    Show("✅ Created. Invite emailed to " + email + ".<br/>Setup link: <a href='" + link + "'>" + link + "</a>", true);
                }
                catch
                {
                    Show("✅ Created. Email could not be sent. Share this setup link:<br/><a href='" + link + "'>" + link + "</a>", true);
                }

                txtName.Text = txtEmail.Text = txtRoll.Text = txtMobile.Text =
                    txtCourse.Text = txtDept.Text = "";
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
                            .Limit(1).FirstOrDefault();
            int next = 1;
            if (latest != null && latest.Contains("studentId"))
            {
                string num = new string(latest["studentId"].AsString.Where(char.IsDigit).ToArray());
                if (int.TryParse(num, out int p)) next = p + 1;
            }
            return "STU" + next.ToString("D3");
        }

        private static string GenerateToken()
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                byte[] buf = new byte[32];
                rng.GetBytes(buf);
                return Convert.ToBase64String(buf)
                    .Replace("+", "-").Replace("/", "_").Replace("=", "");
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
