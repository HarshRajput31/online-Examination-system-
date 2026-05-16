using System;
using MongoDB.Driver;
using OnlineExaminationSystem.App_Start;
using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem
{
    public partial class ViewQuestion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) { Response.Redirect("~/Login.aspx"); return; }
            string id = Request.QueryString["id"] ?? Request.QueryString["questionId"];
            if (string.IsNullOrEmpty(id)) { lblNotFound.Visible = true; return; }

            var col = MongoDbContext.GetCollection<Question>("questions");
            var q = col.Find(x => x.QuestionId == id).FirstOrDefault();
            if (q == null) { lblNotFound.Visible = true; return; }

            litId.Text = q.QuestionId;
            litType.Text = q.QuestionType;
            litExam.Text = q.ExamId;
            litSubject.Text = Server.HtmlEncode(q.Subject ?? "");
            litText.Text = Server.HtmlEncode(q.QuestionText ?? "");
            litA.Text = Server.HtmlEncode(q.OptionA ?? "");
            litB.Text = Server.HtmlEncode(q.OptionB ?? "");
            litC.Text = Server.HtmlEncode(q.OptionC ?? "");
            litD.Text = Server.HtmlEncode(q.OptionD ?? "");
            litCorrect.Text = q.CorrectAnswer ?? "";
            litMarks.Text = q.Marks.ToString();
        }
    }
}
