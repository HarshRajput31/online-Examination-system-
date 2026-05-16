using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OnlineExaminationSystem
{
    public partial class CreateExam : Page
    {
        // Static client to optimize connection pooling
        private static readonly MongoClient client = new MongoClient("mongodb://localhost:27017");
        private IMongoDatabase db;
        private IMongoCollection<BsonDocument> examsCollection;
        private string facultyId;

        // ================= PAGE LOAD =================
        protected void Page_Load(object sender, EventArgs e)
        {
            // Security Check
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            facultyId = Session["UserId"].ToString();

            try
            {
                db = client.GetDatabase("OnlineExamDB");
                examsCollection = db.GetCollection<BsonDocument>("exams");

                if (!IsPostBack)
                {
                    LoadExams();
                }
            }
            catch (Exception ex)
            {
                ShowMsg("❌ Database Connection Error: " + ex.Message, false);
            }
        }

        // ================= LOAD EXAMS =================
        private void LoadExams()
        {
            try
            {
                // Retrieve only exams created by this specific faculty member
                var filter = Builders<BsonDocument>.Filter.Eq("createdBy", facultyId);
                var exams = examsCollection.Find(filter).SortByDescending(x => x["createdAt"]).ToList();

                var displayList = exams.Select(x => {
                    // Calculate Question Count safely
                    int qCount = 0;
                    if (x.Contains("questions") && x["questions"].IsBsonArray)
                    {
                        qCount = x["questions"].AsBsonArray.Count;
                    }

                    return new
                    {
                        examId = x.GetValue("examId", "N/A").ToString(),
                        title = x.GetValue("title", "Untitled").ToString(),
                        subject = x.GetValue("subject", "N/A").ToString(),
                        duration = x.GetValue("duration", 0).ToString() + " Mins",
                        totalMarks = x.GetValue("totalMarks", 0).ToString(),
                        setNumber = "Set " + x.GetValue("setNumber", "1").ToString(),
                        qCount = qCount.ToString(),
                        status = x.GetValue("status", "Pending").ToString()
                    };
                }).ToList();

                gvExams.DataSource = displayList;
                gvExams.DataBind();
            }
            catch (Exception ex)
            {
                ShowMsg("❌ Error Loading Grid: " + ex.Message, false);
            }
        }

        // ================= CREATE EXAM =================
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Data Collection
                string title = txtTitle.Text.Trim();
                string subject = txtSubject.Text.Trim();
                string durationStr = txtDuration.Text.Trim();
                string marksStr = txtMarks.Text.Trim();
                string set = rblSet.SelectedValue;

                // 2. Validation
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(subject))
                {
                    ShowMsg("❌ Title and Subject are required.", false);
                    return;
                }

                if (!int.TryParse(durationStr, out int dur) || !int.TryParse(marksStr, out int totalMarks))
                {
                    ShowMsg("❌ Duration and Marks must be valid numbers.", false);
                    return;
                }

                // 3. Date Validation Logic
                DateTime? start = null;
                DateTime? due = null;
                if (DateTime.TryParse(txtStartDate.Text, out DateTime sDate)) start = sDate.ToUniversalTime();
                if (DateTime.TryParse(txtDueDate.Text, out DateTime dDate)) due = dDate.ToUniversalTime();

                if (start.HasValue && due.HasValue && due <= start)
                {
                    ShowMsg("❌ Due Date must be after the Start Date.", false);
                    return;
                }

                // 4. Duplicate Prevention (Subject + Set + Faculty)
                var dupFilter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("subject", subject),
                    Builders<BsonDocument>.Filter.Eq("setNumber", set),
                    Builders<BsonDocument>.Filter.Eq("createdBy", facultyId)
                );

                if (examsCollection.Find(dupFilter).Any())
                {
                    ShowMsg($"⚠️ You already have a '{set}' for {subject}.", false);
                    return;
                }

                // 5. Generate Unique Exam ID
                string examId = "EX" + DateTime.Now.Ticks.ToString().Substring(10);

                // 6. Construct Document
                var exam = new BsonDocument
                {
                    { "examId", examId },
                    { "title", title },
                    { "subject", subject },
                    { "duration", dur },
                    { "totalMarks", totalMarks },
                    { "setNumber", set },
                    { "createdBy", facultyId },
                    { "status", "Pending" },
                    { "isApproved", false },
                    { "isPublished", false },
                    { "createdAt", DateTime.UtcNow },
                    { "questions", new BsonArray() }
                };

                if (start.HasValue) exam.Add("startDate", start.Value);
                if (due.HasValue) exam.Add("dueDate", due.Value);

                // 7. Insert & Redirect
                examsCollection.InsertOne(exam);
                Session["CurrentExamId"] = examId;

                ShowMsg("✅ Exam Created! You can now add questions.", true);

                ClearForm();
                LoadExams();
            }
            catch (Exception ex)
            {
                ShowStatusError(ex.Message);
            }
        }

        private void ClearForm()
        {
            txtTitle.Text = txtSubject.Text = txtDuration.Text = txtMarks.Text = "";
            txtStartDate.Text = txtDueDate.Text = "";
            rblSet.SelectedIndex = 0;
        }

        // ================= GRIDVIEW COMMANDS =================
        protected void gvExams_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument == null || string.IsNullOrEmpty(e.CommandArgument.ToString())) return;
            string examId = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "DeleteExam":
                    examsCollection.DeleteOne(Builders<BsonDocument>.Filter.Eq("examId", examId));
                    ShowMsg("🗑️ Exam deleted successfully.", true);
                    LoadExams();
                    break;

                case "EditExam":
                    Response.Redirect($"EditExam.aspx?examId={examId}");
                    break;

                case "AddQ":
                    Response.Redirect($"AddQuestion.aspx?examId={examId}");
                    break;
            }
        }

        protected void btnAddQuestions_Click(object sender, EventArgs e)
        {
            if (Session["CurrentExamId"] != null)
            {
                Response.Redirect("AddQuestion.aspx?examId=" + Session["CurrentExamId"]);
            }
            else
            {
                ShowMsg("❌ No active exam session found. Use 'Add Q' in the table below.", false);
            }
        }

        // ================= UI HELPERS =================
        private void ShowMsg(string msg, bool success)
        {
            lblMsg.Text = msg;
            lblMsg.Visible = true;
            lblMsg.ForeColor = success ? System.Drawing.Color.MediumSpringGreen : System.Drawing.Color.LightPink;
        }

        private void ShowStatusError(string error)
        {
            lblMsg.Text = "❌ System Error: " + error;
            lblMsg.Visible = true;
            lblMsg.ForeColor = System.Drawing.Color.Yellow;
        }
    }
}