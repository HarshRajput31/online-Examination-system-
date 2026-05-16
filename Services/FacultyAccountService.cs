using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OnlineExaminationSystem.Services
{
    /// <summary>
    /// Wraps faculty/student account creation: generates faculty IDs,
    /// creates "users" rows with a setup-password token, and emails
    /// the invite link. Used by AddFaculty, EditFaculty, and the
    /// SetFacultyPassword / SetStudentPassword landing pages.
    /// </summary>
    public static class FacultyAccountService
    {
        private const string FacultyIdPrefix = "FAC";
        private const int InviteTokenLifetimeHours = 72;

        /// <summary>
        /// Ensures the given Faculty has a matching "users" doc and a
        /// fresh setup token (when forceNewSetupToken is true). If
        /// sendEmail is true and SMTP is configured, sends the invite.
        /// </summary>
        public static FacultyInviteResult EnsureFacultyLogin(
            Faculty faculty,
            IMongoCollection<Faculty> facultyCol,
            IMongoCollection<BsonDocument> usersCol,
            string baseUrl,
            bool sendEmail,
            bool forceNewSetupToken = false)
        {
            if (faculty == null) throw new ArgumentNullException(nameof(faculty));

            // 1) Ensure faculty has an ID
            if (string.IsNullOrWhiteSpace(faculty.FacultyId))
            {
                faculty.FacultyId = GenerateFacultyId(facultyCol);
                facultyCol.UpdateOne(
                    Builders<Faculty>.Filter.Eq(f => f.Id, faculty.Id),
                    Builders<Faculty>.Update.Set(f => f.FacultyId, faculty.FacultyId));
            }

            // 2) Build login email if missing
            string loginEmail = !string.IsNullOrWhiteSpace(faculty.LoginEmail)
                ? faculty.LoginEmail
                : BuildLoginEmail(faculty.FacultyId);

            facultyCol.UpdateOne(
                Builders<Faculty>.Filter.Eq(f => f.FacultyId, faculty.FacultyId),
                Builders<Faculty>.Update.Set(f => f.LoginEmail, loginEmail));

            // 3) Find or create "users" doc
            var existing = usersCol.Find(
                Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("userId", faculty.FacultyId),
                    Builders<BsonDocument>.Filter.Eq("email", loginEmail))
                ).FirstOrDefault();

            string token = null;
            string tokenHash = null;
            DateTime tokenExpires = DateTime.UtcNow.AddHours(InviteTokenLifetimeHours);

            if (existing == null || forceNewSetupToken)
            {
                token = GenerateToken();
                tokenHash = HashToken(token);
            }

            if (existing == null)
            {
                var doc = new BsonDocument
                {
                    { "userId", faculty.FacultyId },
                    { "name", faculty.Name ?? "" },
                    { "email", loginEmail },
                    { "personalEmail", faculty.PersonalEmail ?? faculty.Email ?? "" },
                    { "roleId", 3 },
                    { "role", "Faculty" },
                    { "department", faculty.Department ?? "" },
                    { "mobile", faculty.Mobile ?? "" },
                    { "course", faculty.Course ?? "" },
                    { "isActive", true },
                    { "isBlocked", false },
                    { "mustSetPassword", true },
                    { "passwordSetupToken", token },
                    { "passwordSetupTokenHash", tokenHash },
                    { "passwordSetupTokenExpiresAt", tokenExpires },
                    { "createdAt", DateTime.UtcNow }
                };
                usersCol.InsertOne(doc);
            }
            else if (forceNewSetupToken)
            {
                usersCol.UpdateOne(
                    Builders<BsonDocument>.Filter.Eq("_id", existing["_id"]),
                    Builders<BsonDocument>.Update
                        .Set("passwordSetupToken", token)
                        .Set("passwordSetupTokenHash", tokenHash)
                        .Set("passwordSetupTokenExpiresAt", tokenExpires)
                        .Set("mustSetPassword", true));
            }

            // 4) Build setup link
            string setupLink = !string.IsNullOrEmpty(token)
                ? CombineUrl(baseUrl, "/SetFacultyPassword.aspx?token=" + Uri.EscapeDataString(token))
                : null;

            // 5) Send email if requested
            bool emailSent = false;
            string emailMsg = "Email sending was not requested.";
            if (sendEmail && setupLink != null && !string.IsNullOrWhiteSpace(faculty.PersonalEmail))
            {
                try
                {
                    EmailService.SendInviteEmail(
                        faculty.PersonalEmail,
                        faculty.Name,
                        loginEmail,
                        setupLink);
                    emailSent = true;
                    emailMsg = "Invitation sent to " + faculty.PersonalEmail;
                }
                catch (Exception ex)
                {
                    emailMsg = "Could not send email: " + ex.Message + ". Use the link below.";
                }
            }
            else if (sendEmail && setupLink != null)
            {
                emailMsg = "No personal email on file; share the setup link manually.";
            }

            return new FacultyInviteResult
            {
                FacultyId = faculty.FacultyId,
                LoginEmail = loginEmail,
                PersonalEmail = faculty.PersonalEmail ?? "",
                SetupLink = setupLink,
                EmailSent = emailSent,
                EmailMessage = emailMsg
            };
        }

        /// <summary>SHA-256 hash of a setup token (hex, lowercase).</summary>
        public static string HashToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return string.Empty;
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
                var sb = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        // ---- helpers ----
        private static string GenerateFacultyId(IMongoCollection<Faculty> col)
        {
            // Find the latest FAC### and increment.
            var latest = col.Find(Builders<Faculty>.Filter.Regex("facultyId", "^" + FacultyIdPrefix))
                            .Sort(Builders<Faculty>.Sort.Descending("facultyId"))
                            .Limit(1)
                            .FirstOrDefault();

            int next = 1;
            if (latest != null && !string.IsNullOrEmpty(latest.FacultyId))
            {
                string numPart = new string(latest.FacultyId.Where(char.IsDigit).ToArray());
                if (int.TryParse(numPart, out int parsed)) next = parsed + 1;
            }
            return FacultyIdPrefix + next.ToString("D3");
        }

        private static string BuildLoginEmail(string facultyId)
        {
            return facultyId.ToLower() + "@onlineexam.local";
        }

        private static string GenerateToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] buf = new byte[32];
                rng.GetBytes(buf);
                return Convert.ToBase64String(buf)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");
            }
        }

        private static string CombineUrl(string baseUrl, string path)
        {
            if (string.IsNullOrEmpty(baseUrl)) return path;
            return baseUrl.TrimEnd('/') + path;
        }
    }
}
