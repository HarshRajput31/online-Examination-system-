using System;
using System.IO;
using System.Web;
using System.Web.UI;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OnlineExaminationSystem
{
    public partial class FacultyProfile : Page
    {
        private readonly MongoClient client = new MongoClient("mongodb://localhost:27017");
        private IMongoCollection<BsonDocument> usersCollection;
        private IMongoCollection<Faculty> facultyCollection;
        private string facultyId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"] == null || Session["RoleId"].ToString() != "3")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            facultyId = Session["UserId"].ToString();

            var db = client.GetDatabase("OnlineExamDB");
            usersCollection = db.GetCollection<BsonDocument>("users");
            facultyCollection = db.GetCollection<Faculty>("faculty");

            if (!IsPostBack)
            {
                LoadProfile();
            }
        }

        private void LoadProfile()
        {
            var filter = GetFacultyFilter();
            var user = usersCollection.Find(filter).FirstOrDefault();

            if (user == null)
            {
                txtFacultyId.Text = facultyId;
                lblNameDisplay.Text = "Faculty";
                imgProfile.ImageUrl = "~/Images/default-user.png";
                return;
            }

            string name = GetStringValue(user, "name", "Name");
            string email = GetStringValue(user, "email", "Email");
            string mobile = GetStringValue(user, "mobile", "Mobile");
            string course = GetStringValue(user, "course", "Course");
            string department = GetStringValue(user, "department", "Department");
            string photo = GetStringValue(user, "profilePhoto", "ProfilePhoto");
            string photoVersion = GetPhotoVersion(user);

            var faculty = FindFacultyProfile(email);

            if (faculty != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                    name = faculty.Name;

                if (string.IsNullOrWhiteSpace(email))
                    email = faculty.Email;

                if (string.IsNullOrWhiteSpace(mobile))
                    mobile = faculty.Mobile;

                if (string.IsNullOrWhiteSpace(course))
                    course = faculty.Course;

                if (string.IsNullOrWhiteSpace(department))
                    department = faculty.Department;
            }

            txtFacultyId.Text = facultyId;
            txtEmail.Text = email;
            txtName.Text = name;
            txtMobile.Text = mobile;
            txtCourse.Text = course;
            lblNameDisplay.Text = string.IsNullOrWhiteSpace(name) ? "Faculty" : name;

            if (ddlDepartment.Items.FindByValue(department) != null)
            {
                ddlDepartment.SelectedValue = department;
            }

            imgProfile.ImageUrl = string.IsNullOrWhiteSpace(photo)
                ? "~/Images/default-user.png"
                : "~/Uploads/" + photo + "?v=" + photoVersion;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var filter = GetFacultyFilter();

                var update = Builders<BsonDocument>.Update
                    .Set("name", txtName.Text.Trim())
                    .Set("email", txtEmail.Text.Trim())
                    .Set("mobile", txtMobile.Text.Trim())
                    .Set("course", txtCourse.Text.Trim())
                    .Set("department", ddlDepartment.SelectedValue);

                var result = usersCollection.UpdateOne(filter, update);

                if (result.MatchedCount == 0)
                {
                    lblMsg.Text = "Profile user not found in MongoDB for " + facultyId + ".";
                    lblMsg.ForeColor = System.Drawing.Color.IndianRed;
                    return;
                }

                lblNameDisplay.Text = string.IsNullOrWhiteSpace(txtName.Text) ? "Faculty" : txtName.Text.Trim();
                lblMsg.Text = "Profile saved successfully!";
                lblMsg.ForeColor = System.Drawing.Color.LightGreen;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Profile could not be saved: " + ex.Message;
                lblMsg.ForeColor = System.Drawing.Color.IndianRed;
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string uploadFolder = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                byte[] photoBytes;
                string extension;

                if (fuPhoto.HasFile)
                {
                    if (!TryReadUploadedPhoto(fuPhoto.PostedFile, out photoBytes, out extension))
                    {
                        return;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(hfCapturedPhoto.Value))
                {
                    if (!TryReadCapturedPhoto(hfCapturedPhoto.Value, out photoBytes, out extension))
                    {
                        return;
                    }
                }
                else
                {
                    lblPhotoMsg.Text = "Please choose a photo or capture one from camera first.";
                    lblPhotoMsg.ForeColor = System.Drawing.Color.IndianRed;
                    return;
                }

                string fileName = facultyId + "_profile" + extension;
                string filePath = Path.Combine(uploadFolder, fileName);
                File.WriteAllBytes(filePath, photoBytes);

                DateTime uploadedAt = DateTime.UtcNow;

                var filter = GetFacultyFilter();
                var update = Builders<BsonDocument>.Update
                    .Set("profilePhoto", fileName)
                    .Set("profilePhotoUpdatedAt", uploadedAt);

                var result = usersCollection.UpdateOne(filter, update);

                if (result.MatchedCount == 0)
                {
                    lblPhotoMsg.Text = "Photo saved, but MongoDB user was not found for " + facultyId + ".";
                    lblPhotoMsg.ForeColor = System.Drawing.Color.IndianRed;
                    return;
                }

                imgProfile.ImageUrl = "~/Uploads/" + fileName + "?v=" + uploadedAt.Ticks;
                lblPhotoMsg.Text = "Photo uploaded successfully!";
                lblPhotoMsg.ForeColor = System.Drawing.Color.LightGreen;
            }
            catch (Exception ex)
            {
                lblPhotoMsg.Text = "Photo could not be uploaded: " + ex.Message;
                lblPhotoMsg.ForeColor = System.Drawing.Color.IndianRed;
            }
        }

        private bool TryReadUploadedPhoto(HttpPostedFile postedFile, out byte[] photoBytes, out string extension)
        {
            photoBytes = null;
            extension = string.Empty;

            if (postedFile == null || postedFile.ContentLength == 0)
            {
                lblPhotoMsg.Text = "Please select a photo first.";
                lblPhotoMsg.ForeColor = System.Drawing.Color.IndianRed;
                return false;
            }

            extension = Path.GetExtension(postedFile.FileName).ToLowerInvariant();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            if (Array.IndexOf(allowedExtensions, extension) == -1)
            {
                lblPhotoMsg.Text = "Only JPG, JPEG, PNG, GIF, and WEBP images are allowed.";
                lblPhotoMsg.ForeColor = System.Drawing.Color.IndianRed;
                return false;
            }

            if (postedFile.ContentLength > 2 * 1024 * 1024)
            {
                lblPhotoMsg.Text = "Photo size must be 2 MB or less.";
                lblPhotoMsg.ForeColor = System.Drawing.Color.IndianRed;
                return false;
            }

            using (var memoryStream = new MemoryStream())
            {
                postedFile.InputStream.CopyTo(memoryStream);
                photoBytes = memoryStream.ToArray();
            }

            return true;
        }

        private bool TryReadCapturedPhoto(string dataUrl, out byte[] photoBytes, out string extension)
        {
            photoBytes = null;
            extension = ".jpg";

            const string jpegPrefix = "data:image/jpeg;base64,";
            const string pngPrefix = "data:image/png;base64,";

            string base64;

            if (dataUrl.StartsWith(jpegPrefix, StringComparison.OrdinalIgnoreCase))
            {
                base64 = dataUrl.Substring(jpegPrefix.Length);
                extension = ".jpg";
            }
            else if (dataUrl.StartsWith(pngPrefix, StringComparison.OrdinalIgnoreCase))
            {
                base64 = dataUrl.Substring(pngPrefix.Length);
                extension = ".png";
            }
            else
            {
                lblPhotoMsg.Text = "Captured photo format is invalid.";
                lblPhotoMsg.ForeColor = System.Drawing.Color.IndianRed;
                return false;
            }

            try
            {
                photoBytes = Convert.FromBase64String(base64);
            }
            catch (FormatException)
            {
                lblPhotoMsg.Text = "Captured photo data is invalid.";
                lblPhotoMsg.ForeColor = System.Drawing.Color.IndianRed;
                return false;
            }

            if (photoBytes.Length > 2 * 1024 * 1024)
            {
                lblPhotoMsg.Text = "Captured photo size must be 2 MB or less.";
                lblPhotoMsg.ForeColor = System.Drawing.Color.IndianRed;
                return false;
            }

            return true;
        }

        private FilterDefinition<BsonDocument> GetFacultyFilter()
        {
            var builder = Builders<BsonDocument>.Filter;

            return builder.Or(
                builder.Eq("userId", facultyId),
                builder.Eq("UserId", facultyId)
            );
        }

        private Faculty FindFacultyProfile(string email)
        {
            var builder = Builders<Faculty>.Filter;
            var filter = builder.Eq("facultyId", facultyId);

            if (!string.IsNullOrWhiteSpace(email))
            {
                filter = builder.Or(
                    builder.Eq("facultyId", facultyId),
                    builder.Eq("email", email)
                );
            }

            return facultyCollection.Find(filter).FirstOrDefault();
        }

        private static string GetStringValue(BsonDocument document, params string[] keys)
        {
            if (document == null || keys == null)
            {
                return string.Empty;
            }

            foreach (string key in keys)
            {
                if (document.Contains(key) && !document[key].IsBsonNull)
                {
                    return document[key].ToString();
                }
            }

            return string.Empty;
        }

        private static string GetPhotoVersion(BsonDocument document)
        {
            if (document == null)
            {
                return DateTime.UtcNow.Ticks.ToString();
            }

            BsonValue value = null;

            if (document.Contains("profilePhotoUpdatedAt") && !document["profilePhotoUpdatedAt"].IsBsonNull)
            {
                value = document["profilePhotoUpdatedAt"];
            }
            else if (document.Contains("ProfilePhotoUpdatedAt") && !document["ProfilePhotoUpdatedAt"].IsBsonNull)
            {
                value = document["ProfilePhotoUpdatedAt"];
            }

            if (value != null)
            {
                if (value.IsValidDateTime)
                {
                    return value.ToUniversalTime().Ticks.ToString();
                }

                if (DateTime.TryParse(value.ToString(), out DateTime parsedDate))
                {
                    return parsedDate.ToUniversalTime().Ticks.ToString();
                }
            }

            return DateTime.UtcNow.Ticks.ToString();
        }
    }
}