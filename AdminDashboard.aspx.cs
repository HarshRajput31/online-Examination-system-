using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Web.UI.WebControls;

namespace OnlineExaminationSystem
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        // Static connection for WebMethods to access
        private static string connectionString = "mongodb://localhost:27017";
        private IMongoDatabase database;

        protected void Page_Load(object sender, EventArgs e)
        {
            var client = new MongoClient(connectionString);
            database = client.GetDatabase("OnlineExamDB");

            if (!IsPostBack)
            {
                LoadStats();
                LoadPendingExams();
            }
        }

        private void LoadStats()
        {
            var usersCol = database.GetCollection<BsonDocument>("users");
            long studentCount = usersCol.CountDocuments(Builders<BsonDocument>.Filter.Eq("role", "Student"));
            long facultyCount = usersCol.CountDocuments(Builders<BsonDocument>.Filter.Eq("role", "Faculty"));
            long total = usersCol.CountDocuments(_ => true);

            lblStudents.Text = studentCount.ToString();
            lblFaculty.Text = facultyCount.ToString();
            lblUsers.Text = total.ToString();

            // Inject counts into JavaScript for the Pie Chart
            long adminCount = total - studentCount - facultyCount;
            string script = $"window.studentCount = {studentCount}; window.facultyCount = {facultyCount}; window.adminCount = {adminCount};";
            ClientScript.RegisterStartupScript(this.GetType(), "initCounts", script, true);
        }

        private void LoadPendingExams()
        {
            var examsCol = database.GetCollection<BsonDocument>("exams");
            var usersCol = database.GetCollection<BsonDocument>("users");

            var pending = examsCol.Find(Builders<BsonDocument>.Filter.Eq("status", "Pending")).ToList();

            var displayData = pending.Select(ex => {
                var faculty = usersCol.Find(Builders<BsonDocument>.Filter.Eq("userId", ex["createdBy"].ToString())).FirstOrDefault();
                return new
                {
                    ExamId = ex["examId"]?.ToString(),
                    Title = ex["title"]?.ToString(),
                    Subject = ex["subject"]?.ToString(),
                    FacultyName = faculty != null ? faculty["name"].ToString() : "Unknown"
                };
            }).ToList();

            rptPendingExams.DataSource = displayData;
            rptPendingExams.DataBind();
        }

        [WebMethod]
        public static object GetLiveDashboardData()
        {
            try
            {
                var client = new MongoClient(connectionString);
                var db = client.GetDatabase("OnlineExamDB");
                var auditCol = db.GetCollection<BsonDocument>("login_audit");

                int[] dailyStats = new int[7];
                DateTime today = DateTime.Today;
                // Find the Monday of the current week
                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime startOfWeek = today.AddDays(-diff);

                // Fetch logs from start of current week (Monday 00:00:00)
                var filter = Builders<BsonDocument>.Filter.Gte("loginTime", startOfWeek);
                var logs = auditCol.Find(filter).ToList();

                foreach (var log in logs)
                {
                    if (log.Contains("loginTime") && !log["loginTime"].IsBsonNull)
                    {
                        // Convert UTC from MongoDB to Local Time for accurate charting
                        DateTime logDate = log["loginTime"].ToUniversalTime().ToLocalTime();
                        int dayIndex = ((int)logDate.DayOfWeek + 6) % 7; // Adjusting so Mon=0, Sun=6

                        if (dayIndex >= 0 && dayIndex < 7)
                            dailyStats[dayIndex]++;
                    }
                }

                return new { activity = dailyStats };
            }
            catch (Exception)
            {
                // Return zeros if there is a database error so the chart doesn't crash
                return new { activity = new int[] { 0, 0, 0, 0, 0, 0, 0 } };
            }
        }

        protected void rptPendingExams_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("OnlineExamDB");
            var examsCol = db.GetCollection<BsonDocument>("exams");

            string examId = e.CommandArgument.ToString();
            string newStatus = e.CommandName == "Approve" ? "Approved" : "Rejected";

            examsCol.UpdateOne(
                Builders<BsonDocument>.Filter.Eq("examId", examId),
                Builders<BsonDocument>.Update.Set("status", newStatus)
            );

            LoadPendingExams();
        }
    }
}