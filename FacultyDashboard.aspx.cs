using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OnlineExaminationSystem
{
    public partial class FacultyDashboard : Page
    {
        private static readonly string connectionString = "mongodb://localhost:27017";
        private static readonly string databaseName = "OnlineExamDB";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"] == null || Session["RoleId"].ToString() != "3")
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboardCounts();
            }
        }

        private void LoadDashboardCounts()
        {
            try
            {
                string facultyId = GetLoggedInFacultyId();
                if (string.IsNullOrEmpty(facultyId))
                    return;

                var client = new MongoClient(connectionString);
                var db = client.GetDatabase(databaseName);

                var examsCollection = db.GetCollection<BsonDocument>("exams");
                var questionsCollection = db.GetCollection<BsonDocument>("questions");

                var facultyExamFilter = Builders<BsonDocument>.Filter.Eq("createdBy", facultyId);
                var exams = examsCollection.Find(facultyExamFilter).ToList();

                int totalExams = exams.Count;
                int pendingExams = exams.Count(e => GetStringValue(e, "status") == "Pending");
                int approvedExams = exams.Count(e => GetStringValue(e, "status") == "Approved");

                int totalQuestions = (int)questionsCollection.CountDocuments(new BsonDocument());

                lblTotalExams.Text = totalExams.ToString();
                lblPendingExams.Text = pendingExams.ToString();
                lblApprovedExams.Text = approvedExams.ToString();
                lblQuestions.Text = totalQuestions.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Dashboard Error: " + ex.Message);
            }
        }

        private static string GetLoggedInFacultyId()
        {
            HttpContext context = HttpContext.Current;

            if (context == null || context.Session == null)
                return null;

            if (context.Session["RoleId"] == null || context.Session["RoleId"].ToString() != "3")
                return null;

            if (context.Session["UserId"] != null)
                return context.Session["UserId"].ToString();

            if (context.Session["userId"] != null)
                return context.Session["userId"].ToString();

            return null;
        }

        private static string GetStringValue(BsonDocument document, string key)
        {
            if (document == null || !document.Contains(key) || document[key].IsBsonNull)
                return string.Empty;

            return document[key].ToString();
        }

        private static bool TryGetCreatedAt(BsonDocument exam, out DateTime createdAt)
        {
            createdAt = DateTime.MinValue;

            if (exam == null || !exam.Contains("createdAt") || exam["createdAt"].IsBsonNull)
                return false;

            BsonValue value = exam["createdAt"];

            if (value.IsValidDateTime)
            {
                createdAt = value.ToUniversalTime();
                return true;
            }

            return DateTime.TryParse(value.ToString(), out createdAt);
        }

        [WebMethod(EnableSession = true)]
        public static object GetFacultyDashboardData()
        {
            try
            {
                string facultyId = GetLoggedInFacultyId();
                if (string.IsNullOrEmpty(facultyId))
                {
                    return new
                    {
                        activity = new int[] { 0, 0, 0, 0, 0, 0, 0 },
                        pending = 0,
                        approved = 0,
                        error = "Session expired. Please login again."
                    };
                }

                var client = new MongoClient(connectionString);
                var db = client.GetDatabase(databaseName);
                var examsCollection = db.GetCollection<BsonDocument>("exams");

                var exams = examsCollection.Find(
                    Builders<BsonDocument>.Filter.Eq("createdBy", facultyId)
                ).ToList();

                int[] activity = new int[7];

                foreach (var exam in exams)
                {
                    if (!TryGetCreatedAt(exam, out DateTime createdAt))
                        continue;

                    int dayIndex = ((int)createdAt.DayOfWeek + 6) % 7;
                    activity[dayIndex]++;
                }

                int pending = exams.Count(e => GetStringValue(e, "status") == "Pending");
                int approved = exams.Count(e => GetStringValue(e, "status") == "Approved");

                return new
                {
                    activity = activity,
                    pending = pending,
                    approved = approved
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    activity = new int[] { 0, 0, 0, 0, 0, 0, 0 },
                    pending = 0,
                    approved = 0,
                    error = ex.Message
                };
            }
        }
    }
}
