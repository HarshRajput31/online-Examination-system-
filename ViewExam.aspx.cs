using System;
using MongoDB.Bson;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;

namespace OnlineExaminationSystem
{
    public partial class ViewExam : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/Login.aspx"); return; }
            string id = Request.QueryString["examId"] ?? Request.QueryString["id"];
            if (string.IsNullOrEmpty(id)) { lblNotFound.Visible = true; return; }

            var col = MongoDbContext.GetCollection<BsonDocument>("exams");
            var doc = col.Find(Builders<BsonDocument>.Filter.Eq("examId", id)).FirstOrDefault();
            if (doc == null) { lblNotFound.Visible = true; return; }

            litExamId.Text = id;
            litTitle.Text = Get(doc, "title");
            litSubject.Text = Get(doc, "subject");
            litSet.Text = Get(doc, "setNumber");
            litDuration.Text = Get(doc, "duration");
            litMarks.Text = Get(doc, "totalMarks");
            litStatus.Text = Get(doc, "status");
            litCreator.Text = Get(doc, "createdBy");
            litCreated.Text = doc.Contains("createdAt") && !doc["createdAt"].IsBsonNull
                ? doc["createdAt"].ToUniversalTime().ToString("dd MMM yyyy") : "";
            litCount.Text = doc.Contains("questions") && doc["questions"].IsBsonArray
                ? doc["questions"].AsBsonArray.Count.ToString() : "0";
        }

        private static string Get(BsonDocument d, string k) =>
            d.Contains(k) && !d[k].IsBsonNull ? d[k].ToString() : "";
    }
}
