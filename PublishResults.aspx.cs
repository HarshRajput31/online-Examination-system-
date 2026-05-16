using System;
using System.Linq;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.Services;

namespace OnlineExaminationSystem
{
    public partial class PublishResults : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "1")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadExams();
                LoadResults();
            }
        }

        private void LoadExams()
        {
            var col = MongoDbContext.GetCollection<BsonDocument>("exams");
            var exams = col.Find(_ => true).SortByDescending(d => d["createdAt"]).Limit(100).ToList();

            ddlExam.Items.Clear();
            ddlExam.Items.Add(new ListItem("-- Choose an exam --", ""));
            foreach (var x in exams)
            {
                string id = x.GetValue("examId", "").ToString();
                string title = x.GetValue("title", id).ToString();
                ddlExam.Items.Add(new ListItem(title + " (" + id + ")", id));
            }
        }

        protected void ddlExam_Changed(object sender, EventArgs e) => LoadResults();

        private void LoadResults()
        {
            string examId = ddlExam.SelectedValue;
            if (string.IsNullOrEmpty(examId))
            {
                pnlList.Visible = false;
                pnlEmpty.Visible = false;
                return;
            }

            var col = MongoDbContext.GetCollection<ExamResult>("results");
            var list = col.Find(r => r.ExamId == examId)
                          .SortByDescending(r => r.Percentage)
                          .ToList();

            pnlEmpty.Visible = list.Count == 0;
            pnlList.Visible = list.Count > 0;
            gvResults.DataSource = list;
            gvResults.DataBind();
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            string examId = ddlExam.SelectedValue;
            if (string.IsNullOrEmpty(examId))
            { Show("Pick an exam first.", false); return; }

            var rCol = MongoDbContext.GetCollection<ExamResult>("results");
            var examsCol = MongoDbContext.GetCollection<BsonDocument>("exams");
            var exam = examsCol.Find(Builders<BsonDocument>.Filter.Eq("examId", examId)).FirstOrDefault();
            string examTitle = exam != null ? exam.GetValue("title", examId).ToString() : examId;

            var results = rCol.Find(r => r.ExamId == examId).ToList();
            int notified = 0;
            foreach (var r in results)
            {
                NotificationService.Push(r.StudentId, "result_published",
                    "Result published",
                    "Your result for " + examTitle + " is now available.",
                    "~/StudentResults.aspx");
                notified++;
            }

            // Mark exam as result-published
            examsCol.UpdateOne(
                Builders<BsonDocument>.Filter.Eq("examId", examId),
                Builders<BsonDocument>.Update.Set("resultsPublished", true)
                                              .Set("resultsPublishedAt", DateTime.UtcNow));

            Show("Results published. " + notified + " student(s) notified.", true);
        }

        private void Show(string msg, bool ok)
        {
            lblMsg.Text = (ok ? "✅ " : "❌ ") + msg;
            lblMsg.Visible = true;
            lblMsg.ForeColor = ok ? System.Drawing.Color.LightGreen : System.Drawing.Color.IndianRed;
        }
    }
}
