using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OnlineExaminationSystem
{
    public partial class ManageExams : System.Web.UI.Page
    {
        // MongoDB Connection
        private static readonly MongoClient client =
            new MongoClient("mongodb://localhost:27017");

        private IMongoDatabase db;
        private IMongoCollection<BsonDocument> examsCol;

        // ================= PAGE LOAD =================
        protected void Page_Load(object sender, EventArgs e)
        {
            // ADMIN SECURITY CHECK
            if (Session["RoleId"] == null ||
                Session["RoleId"].ToString() != "1")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            try
            {
                db = client.GetDatabase("OnlineExamDB");

                examsCol = db.GetCollection<BsonDocument>("exams");

                if (!IsPostBack)
                {
                    LoadPendingExams();
                }
            }
            catch (Exception ex)
            {
                ShowStatus(
                    "❌ Database Connection Error : " + ex.Message,
                    false
                );
            }
        }

        // ================= LOAD EXAMS =================
        private void LoadPendingExams()
        {
            try
            {
                // Show only unapproved exams
                var filter =
                    Builders<BsonDocument>.Filter.Or(

                        Builders<BsonDocument>.Filter.Eq(
                            "isApproved",
                            false
                        ),

                        Builders<BsonDocument>.Filter.Exists(
                            "isApproved",
                            false
                        )
                    );

                var pendingExams = examsCol
                    .Find(filter)
                    .SortByDescending(x => x["createdAt"])
                    .ToList();

                if (pendingExams.Any())
                {
                    var examList = pendingExams.Select(x => new
                    {
                        examId =
                            x.GetValue("examId", "")
                             .ToString(),

                        title =
                            x.GetValue(
                                "title",
                                "Untitled Exam"
                            ).ToString(),

                        subject =
                            x.GetValue(
                                "subject",
                                "General"
                            ).ToString(),

                        setNumber =
                            x.GetValue(
                                "setNumber",
                                "1"
                            ).ToString(),

                        duration =
                            x.GetValue(
                                "duration",
                                "0"
                            ).ToString(),

                        totalMarks =
                            x.GetValue(
                                "totalMarks",
                                "0"
                            ).ToString(),

                        totalQuestions =
                            x.Contains("questions")
                            ? x["questions"]
                                .AsBsonArray
                                .Count
                                .ToString()
                            : "0",

                        status =
                            x.GetValue(
                                "status",
                                "Pending"
                            ).ToString(),

                        facultyName =
                            x.GetValue(
                                "createdBy",
                                "Unknown Faculty"
                            ).ToString(),

                        createdAt =
                            x.Contains("createdAt")
                            ? Convert.ToDateTime(
                                x["createdAt"]
                              ).ToString("dd MMM yyyy")
                            : "N/A"
                    }).ToList();

                    rptPendingExams.DataSource = examList;
                    rptPendingExams.DataBind();

                    rptPendingExams.Visible = true;
                    pnlNoData.Visible = false;
                }
                else
                {
                    rptPendingExams.Visible = false;
                    pnlNoData.Visible = true;
                }
            }
            catch (Exception ex)
            {
                ShowStatus(
                    "❌ Error Loading Exams : " + ex.Message,
                    false
                );
            }
        }

        // ================= APPROVE / REJECT =================
        protected void rptPendingExams_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (e.CommandArgument == null)
                return;

            try
            {
                string examId =
                    e.CommandArgument.ToString();

                var filter =
                    Builders<BsonDocument>.Filter.Eq(
                        "examId",
                        examId
                    );

                switch (e.CommandName)
                {
                    // ===== APPROVE =====
                    case "Approve":

                        // IMPORTANT:
                        // These fields are required
                        // for Student Dashboard visibility

                        var update =
                            Builders<BsonDocument>.Update

                            .Set("isApproved", true)

                            .Set("isPublished", true)

                            .Set("status", "Published")

                            .Set(
                                "approvedAt",
                                DateTime.UtcNow
                            );

                        var result =
                            examsCol.UpdateOne(
                                filter,
                                update
                            );

                        if (result.ModifiedCount > 0)
                        {
                            ShowStatus(
                                "🚀 Exam Approved & Published Successfully!",
                                true
                            );
                        }
                        else
                        {
                            ShowStatus(
                                "⚠️ Exam already approved.",
                                false
                            );
                        }

                        break;

                    // ===== REJECT =====
                    case "Reject":

                        var delResult =
                            examsCol.DeleteOne(filter);

                        if (delResult.DeletedCount > 0)
                        {
                            ShowStatus(
                                "🗑️ Exam Rejected & Removed.",
                                true
                            );
                        }

                        break;
                }

                // REFRESH GRID
                LoadPendingExams();
            }
            catch (Exception ex)
            {
                ShowStatus(
                    "❌ Process Error : " + ex.Message,
                    false
                );
            }
        }

        // ================= STATUS MESSAGE =================
        private void ShowStatus(
            string msg,
            bool success)
        {
            lblStatus.Text = msg;

            lblStatus.Visible = true;

            lblStatus.ForeColor =
                System.Drawing.Color.White;

            if (success)
            {
                lblStatus.CssClass =
                    "status-toast success";

                lblStatus.BackColor =
                    System.Drawing.Color.ForestGreen;
            }
            else
            {
                lblStatus.CssClass =
                    "status-toast error";

                lblStatus.BackColor =
                    System.Drawing.Color.Crimson;
            }
        }
    }
}