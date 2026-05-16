using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Web.UI.WebControls;

namespace OnlineExaminationSystem
{
    public partial class AddQuestion : System.Web.UI.Page
    {
        private IMongoCollection<BsonDocument> questionsCollection;
        private IMongoCollection<BsonDocument> examsCollection;
        private string facultyId;

        // ================= PAGE LOAD =================
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "3")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            facultyId = Session["UserId"].ToString();
            ConnectDB();

            if (!IsPostBack)
            {
                LoadExams();

                string examId = Request.QueryString["examId"];
                if (!string.IsNullOrEmpty(examId))
                {
                    if (ddlExam.Items.FindByValue(examId) != null)
                    {
                        ddlExam.SelectedValue = examId;
                        lblExamDisplay.Text = examId;
                    }
                }

                LoadCount();
            }
        }

        // ================= PUBLISH =================
        protected void btnFinalPublish_Click(object sender, EventArgs e)
        {
            try
            {
                string examId = ddlExam.SelectedValue;

                if (string.IsNullOrEmpty(examId))
                {
                    ShowMsg("❌ Select an exam first.", false);
                    return;
                }

                long count = questionsCollection.CountDocuments(
                    Builders<BsonDocument>.Filter.Eq("examId", examId));

                if (count == 0)
                {
                    ShowMsg("❌ Cannot publish empty exam!", false);
                    return;
                }

                var filter = Builders<BsonDocument>.Filter.Eq("examId", examId);

                var update = Builders<BsonDocument>.Update
                    .Set("status", "Pending Admin Approval")
                    .Set("isPublished", true)
                    .Set("publishedDate", DateTime.UtcNow);

                examsCollection.UpdateOne(filter, update);

                Response.Redirect("CreateExam.aspx?status=published");
            }
            catch (Exception ex)
            {
                ShowMsg("❌ Error: " + ex.Message, false);
            }
        }

        // ================= LOAD EXAMS =================
        private void LoadExams()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("createdBy", facultyId);

            var exams = examsCollection.Find(filter)
                .SortByDescending(e => e["createdAt"])
                .ToList();

            ddlExam.Items.Clear();
            ddlExam.Items.Add(new ListItem("-- Select Exam --", ""));

            foreach (var ex in exams)
            {
                string id = ex.GetValue("examId", "").ToString();
                string title = ex.GetValue("title", "").ToString();
                string subject = ex.GetValue("subject", "").ToString();

                ddlExam.Items.Add(
                    new ListItem($"{title} — {subject} ({id})", id));
            }
        }

        // ================= LOAD COUNT =================
        private void LoadCount()
        {
            string examId = ddlExam.SelectedValue;

            if (string.IsNullOrEmpty(examId))
            {
                lblCount.Text = "0";
                return;
            }

            long count = questionsCollection.CountDocuments(
                Builders<BsonDocument>.Filter.Eq("examId", examId));

            lblCount.Text = count.ToString();
        }

        // ================= BUTTONS =================
        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveQuestion(false);
        }

        protected void btnSaveAnother_Click(object sender, EventArgs e)
        {
            SaveQuestion(true);
        }

        // ================= MAIN SAVE =================
        private void SaveQuestion(bool clearAfter)
        {
            try
            {
                string examId = ddlExam.SelectedValue;
                string set = rblSet.SelectedValue;
                string qText = txtQuestion.Text.Trim();
                string qType = hfQuestionType.Value;

                if (string.IsNullOrEmpty(examId))
                {
                    ShowMsg("❌ Select exam", false);
                    return;
                }

                if (string.IsNullOrWhiteSpace(qText))
                {
                    ShowMsg("❌ Enter question", false);
                    return;
                }

                // 🔥 FETCH SUBJECT FROM EXAM (IMPORTANT FIX)
                var examDoc = examsCollection.Find(
                    Builders<BsonDocument>.Filter.Eq("examId", examId)
                ).FirstOrDefault();

                string examSubject = examDoc != null
                    ? examDoc.GetValue("subject", "").ToString()
                    : "";

                // ================= MAIN DOC =================
                var doc = new BsonDocument
                {
                    { "questionId", "Q" + DateTime.Now.Ticks.ToString().Substring(8) },
                    { "examId", examId },
                    { "subject", examSubject },   // ✅ FIXED
                    { "setNumber", set },
                    { "questionText", qText },
                    { "questionType", qType },
                    { "createdBy", facultyId },
                    { "createdAt", DateTime.UtcNow }
                };

                // ================= MCQ =================
                if (qType == "mcq")
                {
                    if (string.IsNullOrEmpty(txtOptA.Text) ||
                        string.IsNullOrEmpty(txtOptB.Text) ||
                        string.IsNullOrEmpty(txtOptC.Text) ||
                        string.IsNullOrEmpty(txtOptD.Text))
                    {
                        ShowMsg("❌ Fill all options", false);
                        return;
                    }

                    if (string.IsNullOrEmpty(rblCorrect.SelectedValue))
                    {
                        ShowMsg("❌ Select correct answer", false);
                        return;
                    }

                    doc.Add("optionA", txtOptA.Text.Trim());
                    doc.Add("optionB", txtOptB.Text.Trim());
                    doc.Add("optionC", txtOptC.Text.Trim());
                    doc.Add("optionD", txtOptD.Text.Trim());

                    doc.Add("correctAnswer", rblCorrect.SelectedValue);
                    doc.Add("marks", int.TryParse(txtMarks.Text, out int m) ? m : 1);

                    bool hasSubQ = hfHasSubQ.Value == "true";
                    doc.Add("hasSubQuestions", hasSubQ);

                    if (hasSubQ && !string.IsNullOrWhiteSpace(hfSubQuestions.Value))
                    {
                        var subArr = BsonSerializer.Deserialize<BsonArray>(hfSubQuestions.Value);
                        doc.Add("subQuestions", subArr);
                    }
                }
                else
                {
                    doc.Add("modelAnswer", txtModelAnswer.Text.Trim());
                    doc.Add("maxWords", int.TryParse(txtMaxWords.Text, out int mw) ? mw : 500);
                    doc.Add("marks", int.TryParse(txtDescMarks.Text, out int dm) ? dm : 10);
                }

                // ================= INSERT =================
                questionsCollection.InsertOne(doc);

                ShowMsg("✅ Question saved successfully!", true);

                LoadCount();

                if (clearAfter)
                    ClearForm();
            }
            catch (Exception ex)
            {
                ShowMsg("❌ Error: " + ex.Message, false);
            }
        }

        // ================= CLEAR =================
        private void ClearForm()
        {
            txtQuestion.Text = "";
            txtOptA.Text = "";
            txtOptB.Text = "";
            txtOptC.Text = "";
            txtOptD.Text = "";
            txtMarks.Text = "";
            txtModelAnswer.Text = "";
            txtMaxWords.Text = "";
            txtDescMarks.Text = "";

            rblCorrect.ClearSelection();

            hfSubQuestions.Value = "";
            hfHasSubQ.Value = "false";
        }

        // ================= DB =================
        private void ConnectDB()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("OnlineExamDB");

            questionsCollection = db.GetCollection<BsonDocument>("questions");
            examsCollection = db.GetCollection<BsonDocument>("exams");
        }

        // ================= MESSAGE =================
        private void ShowMsg(string msg, bool success)
        {
            lblMsg.Text = msg;
            lblMsg.Visible = true;
            lblMsg.ForeColor = success
                ? System.Drawing.Color.LightGreen
                : System.Drawing.Color.IndianRed;
        }
    }
}