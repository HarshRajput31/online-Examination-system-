using System;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class AddFaculty : System.Web.UI.Page
    {
        private IMongoCollection<Faculty> facultyCollection;
        private IMongoCollection<BsonDocument> usersCollection;

        protected void Page_Load(object sender, EventArgs e)
        {
            // ✅ ONLY ADMIN CAN ACCESS
            if (Session["UserId"] == null ||
                Session["RoleId"] == null ||
                Session["RoleId"].ToString() != "1")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            ConnectDB();
        }

        // ─────────────────────────────────────────────
        // DATABASE CONNECTION
        // ─────────────────────────────────────────────
        private void ConnectDB()
        {
            var client = new MongoClient("mongodb://localhost:27017");

            var database = client.GetDatabase("OnlineExamDB");

            facultyCollection =
                database.GetCollection<Faculty>("faculty");

            usersCollection =
                database.GetCollection<BsonDocument>("users");
        }

        // ─────────────────────────────────────────────
        // ADD FACULTY BUTTON
        // ─────────────────────────────────────────────
        protected void btnAddFaculty_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                string personalEmail = txtEmail.Text.Trim();
                string department = txtDepartment.Text.Trim();
                string mobile = txtMobile.Text.Trim();
                string course = txtCourse.Text.Trim();

                // ✅ VALIDATION
                if (string.IsNullOrWhiteSpace(name))
                {
                    ShowError("Faculty Name is required.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(personalEmail))
                {
                    ShowError("Email is required.");
                    return;
                }

                // ✅ CHECK DUPLICATE EMAIL
                var existingFaculty =
                    facultyCollection.Find(x =>
                        x.Email == personalEmail).FirstOrDefault();

                if (existingFaculty != null)
                {
                    ShowError("Faculty already exists with this email.");
                    return;
                }

                // ✅ CREATE FACULTY OBJECT
                var newFaculty = new Faculty
                {
                    FacultyId = "",

                    Name = name,

                    Email = personalEmail,

                    PersonalEmail = personalEmail,

                    Department = department,

                    Mobile = mobile,

                    Course = course,

                    CreatedAt = DateTime.UtcNow
                };

                // ✅ INSERT FACULTY
                facultyCollection.InsertOne(newFaculty);

                // ✅ CREATE LOGIN ACCOUNT
                FacultyInviteResult invite =
                    FacultyAccountService.EnsureFacultyLogin(
                        newFaculty,
                        facultyCollection,
                        usersCollection,
                        GetAppBaseUrl(),
                        sendEmail: true,
                        forceNewSetupToken: true
                    );

                // ✅ SUCCESS MESSAGE
                string msg =
                    "✅ Faculty Added Successfully!<br/><br/>" +

                    "<b>Faculty ID :</b> " +
                    invite.FacultyId + "<br/>" +

                    "<b>Login Email :</b> " +
                    invite.LoginEmail + "<br/>" +

                    "<b>Personal Email :</b> " +
                    invite.PersonalEmail + "<br/><br/>";

                if (invite.EmailSent)
                {
                    msg += "📨 " + invite.EmailMessage;
                }
                else
                {
                    msg += "⚠️ " + invite.EmailMessage;

                    if (!string.IsNullOrWhiteSpace(invite.SetupLink))
                    {
                        msg +=
                            "<br/><br/>" +
                            "<b>Password Setup Link:</b><br/>" +

                            "<a href='" + invite.SetupLink + "'>" +
                            invite.SetupLink +
                            "</a>";
                    }
                }

                lblMsg.Text = msg;
                lblMsg.ForeColor =
                    System.Drawing.Color.LightGreen;

                // ✅ CLEAR FORM
                txtName.Text = "";
                txtEmail.Text = "";
                txtDepartment.Text = "";
                txtMobile.Text = "";
                txtCourse.Text = "";
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        // ─────────────────────────────────────────────
        // ERROR MESSAGE
        // ─────────────────────────────────────────────
        private void ShowError(string message)
        {
            lblMsg.Text = "❌ " + message;

            lblMsg.ForeColor =
                System.Drawing.Color.Red;
        }

        // ─────────────────────────────────────────────
        // APP BASE URL
        // ─────────────────────────────────────────────
        private string GetAppBaseUrl()
        {
            string configured =
                System.Configuration.ConfigurationManager
                .AppSettings["AppBaseUrl"];

            return !string.IsNullOrWhiteSpace(configured)
                ? configured
                : Request.Url.GetLeftPart(UriPartial.Authority);
        }
    }
}