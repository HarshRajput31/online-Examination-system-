using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem
{
    public partial class StudentExams : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "2")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack) Load();
        }

        private void Load()
        {
            var col = MongoDbContext.GetCollection<BsonDocument>("exams");
            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("isApproved", true),
                Builders<BsonDocument>.Filter.Eq("isPublished", true));

            var list = col.Find(filter).SortByDescending(e => e["createdAt"]).ToList()
                .Select(x => new
                {
                    ExamId = x.GetValue("examId", "").ToString(),
                    Title = x.GetValue("title", "Untitled").ToString(),
                    Subject = x.GetValue("subject", "General").ToString(),
                    Duration = x.GetValue("duration", 0).ToInt32(),
                    TotalMarks = x.GetValue("totalMarks", 0).ToInt32(),
                    QuestionCount = x.Contains("questions") && x["questions"].IsBsonArray
                        ? x["questions"].AsBsonArray.Count : 0
                }).ToList();

            pnlEmpty.Visible = list.Count == 0;
            rptExams.DataSource = list;
            rptExams.DataBind();
        }
    }
}
