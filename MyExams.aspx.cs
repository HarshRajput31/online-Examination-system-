using System;
using System.Linq;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem
{
    public partial class MyExams : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || Session["RoleId"]?.ToString() != "3")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack) Load();
        }

        private void Load()
        {
            var col = MongoDbContext.GetCollection<BsonDocument>("exams");
            string fid = Session["UserId"].ToString();
            var exams = col.Find(Builders<BsonDocument>.Filter.Eq("createdBy", fid))
                .SortByDescending(d => d["createdAt"])
                .ToList()
                .Select(x => new
                {
                    examId = x.GetValue("examId", "").ToString(),
                    title = x.GetValue("title", "").ToString(),
                    subject = x.GetValue("subject", "").ToString(),
                    setNumber = x.GetValue("setNumber", "").ToString(),
                    status = x.GetValue("status", "Pending").ToString(),
                    questionCount = x.Contains("questions") && x["questions"].IsBsonArray
                        ? x["questions"].AsBsonArray.Count : 0
                }).ToList();

            pnlEmpty.Visible = exams.Count == 0;
            gv.DataSource = exams;
            gv.DataBind();
        }

        protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteExam") return;
            string id = e.CommandArgument?.ToString();
            if (string.IsNullOrEmpty(id)) return;

            MongoDbContext.GetCollection<BsonDocument>("exams")
                .DeleteOne(Builders<BsonDocument>.Filter.Eq("examId", id));
            MongoDbContext.GetCollection<BsonDocument>("questions")
                .DeleteMany(Builders<BsonDocument>.Filter.Eq("examId", id));

            lblMsg.Text = "🗑️ Exam and its questions removed.";
            lblMsg.Visible = true;
            lblMsg.ForeColor = System.Drawing.Color.LightGreen;
            Load();
        }
    }
}
