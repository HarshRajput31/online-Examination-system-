using System;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class EditFaculty : System.Web.UI.Page
    {
        private IMongoCollection<Faculty> facultyCollection;
        private IMongoCollection<BsonDocument> usersCollection;
        private string facultyId;
        private string facultyEmail;

        protected void Page_Load(object sender, EventArgs e)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("OnlineExamDB");
            facultyCollection = db.GetCollection<Faculty>("faculty");
            usersCollection = db.GetCollection<BsonDocument>("users");

            facultyId = Request.QueryString["id"];
            facultyEmail = Request.QueryString["email"];

            if (!IsPostBack)
            {
                LoadFaculty();
            }
        }

        private void LoadFaculty()
        {
            Faculty faculty = FindFaculty();

            if (faculty == null)
            {
                lblMsg.Text = "Faculty not found.";
                lblMsg.ForeColor = System.Drawing.Color.Red;
                return;
            }

            FacultyInviteResult invite = FacultyAccountService.EnsureFacultyLogin(
                faculty,
                facultyCollection,
                usersCollection,
                GetAppBaseUrl(),
                false);

            facultyId = invite.FacultyId;

            txtName.Text = faculty.Name;
            txtEmail.Text = string.IsNullOrWhiteSpace(faculty.PersonalEmail) ? faculty.Email : faculty.PersonalEmail;
            txtLoginEmail.Text = invite.LoginEmail;
            txtDepartment.Text = faculty.Department;
            txtMobile.Text = faculty.Mobile;
            txtCourse.Text = faculty.Course;
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                Faculty faculty = FindFaculty();

                if (faculty == null)
                {
                    lblMsg.Text = "Faculty not found.";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                    return;
                }

                string personalEmail = txtEmail.Text.Trim();

                var facultyFilter = Builders<Faculty>.Filter.Eq(f => f.FacultyId, faculty.FacultyId);
                var facultyUpdate = Builders<Faculty>.Update
                    .Set(f => f.Name, txtName.Text.Trim())
                    .Set(f => f.Email, personalEmail)
                    .Set(f => f.PersonalEmail, personalEmail)
                    .Set(f => f.LoginEmail, txtLoginEmail.Text.Trim())
                    .Set(f => f.Department, txtDepartment.Text.Trim())
                    .Set(f => f.Mobile, txtMobile.Text.Trim())
                    .Set(f => f.Course, txtCourse.Text.Trim());

                facultyCollection.UpdateOne(facultyFilter, facultyUpdate);

                var userFilter = Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("userId", faculty.FacultyId),
                    Builders<BsonDocument>.Filter.Eq("email", txtLoginEmail.Text.Trim()));

                var userUpdate = Builders<BsonDocument>.Update
                    .Set("name", txtName.Text.Trim())
                    .Set("personalEmail", personalEmail)
                    .Set("department", txtDepartment.Text.Trim())
                    .Set("mobile", txtMobile.Text.Trim())
                    .Set("course", txtCourse.Text.Trim())
                    .Set("roleId", 3)
                    .Set("role", "Faculty");

                usersCollection.UpdateOne(userFilter, userUpdate);

                lblMsg.Text = "Faculty updated successfully. Password was not changed or shown.";
                lblMsg.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        private Faculty FindFaculty()
        {
            var builder = Builders<Faculty>.Filter;

            if (!string.IsNullOrWhiteSpace(facultyId))
            {
                return facultyCollection.Find(builder.Eq(f => f.FacultyId, facultyId)).FirstOrDefault();
            }

            if (!string.IsNullOrWhiteSpace(facultyEmail))
            {
                var filter = builder.Or(
                    builder.Eq(f => f.Email, facultyEmail),
                    builder.Eq(f => f.PersonalEmail, facultyEmail),
                    builder.Eq(f => f.LoginEmail, facultyEmail));

                return facultyCollection.Find(filter).FirstOrDefault();
            }

            return null;
        }

        private string GetAppBaseUrl()
        {
            string configuredBaseUrl = System.Configuration.ConfigurationManager.AppSettings["AppBaseUrl"];
            if (!string.IsNullOrWhiteSpace(configuredBaseUrl))
            {
                return configuredBaseUrl;
            }

            return Request.Url.GetLeftPart(UriPartial.Authority);
        }
    }
}