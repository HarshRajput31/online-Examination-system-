using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OnlineExaminationSystem
{
    public partial class Result : System.Web.UI.Page
    {
        // Use a single MongoClient instance for better performance
        private static readonly MongoClient client = new MongoClient("mongodb://localhost:27017");
        private IMongoDatabase db;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Session Protection
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            db = client.GetDatabase("OnlineExamDB");

            if (!IsPostBack)
                LoadResult();
        }

        private void LoadResult()
        {
            try
            {
                string studentId = Session["UserId"].ToString();
                var resultsCol = db.GetCollection<BsonDocument>("examResults");
                var usersCol = db.GetCollection<BsonDocument>("users");

                // 1. Retrieve the most recent result for this user
                BsonDocument resultDoc = Session["ExamResult"] as BsonDocument;

                if (resultDoc == null)
                {
                    // If session is empty, fetch the latest submission from the database
                    var filter = Builders<BsonDocument>.Filter.Eq("studentId", studentId);
                    resultDoc = resultsCol.Find(filter)
                                          .SortByDescending(r => r.Contains("submittedAt") ? r["submittedAt"] : BsonValue.Create(DateTime.MinValue))
                                          .FirstOrDefault();
                }

                if (resultDoc == null)
                {
                    lblMsg.Text = "❌ No exam result found for your account.";
                    lblMsg.Visible = true;
                    return;
                }

                // 2. Student Information & UI Personalization
                var student = usersCol.Find(Builders<BsonDocument>.Filter.Eq("userId", studentId)).FirstOrDefault();
                string name = student != null ? student.GetValue("name", "Student").ToString() : "Student";

                lblStudentName.Text = name;
                lblStudentId.Text = studentId;
                lblDept.Text = student != null ? student.GetValue("course", "General").ToString() : "N/A";

                // Get first letter for the profile icon
                litInitial.Text = !string.IsNullOrEmpty(name) ? name.Substring(0, 1).ToUpper() : "S";

                // 3. Mapping Scores and Stats
                int totalQ = resultDoc.GetValue("totalQuestions", 0).ToInt32();
                int correct = resultDoc.GetValue("correctAnswers", 0).ToInt32();
                int wrong = resultDoc.GetValue("wrongAnswers", 0).ToInt32();
                int notAtt = resultDoc.GetValue("notAttempted", 0).ToInt32();
                int totalM = resultDoc.GetValue("totalMarks", 0).ToInt32();
                int score = resultDoc.GetValue("totalScore", 0).ToInt32();
                double pct = resultDoc.GetValue("percentage", 0).ToDouble();
                bool passed = resultDoc.GetValue("passed", false).ToBoolean();

                string examId = resultDoc.GetValue("examId", "").ToString();
                lblExamId.Text = examId;
                lblSubject.Text = resultDoc.GetValue("subject", "").ToString();
                lblExamTitle.Text = resultDoc.GetValue("examTitle", "").ToString();

                lblTotalQ.Text = totalQ.ToString();
                lblCorrect.Text = correct.ToString();
                lblWrong.Text = wrong.ToString();
                lblNotAtt.Text = notAtt.ToString();
                lblScore.Text = $"{score} / {totalM}";
                lblPct.Text = pct.ToString("F1") + "%";

                // Update Status Display
                lblStatus.Text = passed ? "✅ PASSED" : "❌ FAILED";
                lblStatus.ForeColor = passed ? System.Drawing.Color.MediumSeaGreen : System.Drawing.Color.Crimson;

                // 4. Rank Calculation Logic
                if (!string.IsNullOrEmpty(examId))
                {
                    var allResults = resultsCol.Find(Builders<BsonDocument>.Filter.Eq("examId", examId))
                                               .SortByDescending(r => r["totalScore"])
                                               .ToList();

                    int rank = allResults.FindIndex(r => r["studentId"].ToString() == studentId) + 1;
                    lblRank.Text = rank > 0 ? "#" + rank : "N/A";
                }

                // 5. Circular/Horizontal Progress Bar Logic (Percentage Strings)
                if (totalQ > 0)
                {
                    string cStr = ((double)correct / totalQ * 100).ToString("F0");
                    string wStr = ((double)wrong / totalQ * 100).ToString("F0");
                    string sStr = ((double)notAtt / totalQ * 100).ToString("F0");

                    litCorrectPct.Text = litCorrectPct2.Text = cStr;
                    litWrongPct.Text = litWrongPct2.Text = wStr;
                    litNotAttPct.Text = litNotAttPct2.Text = sStr;
                }
                else
                {
                    litCorrectPct.Text = litCorrectPct2.Text = "0";
                    litWrongPct.Text = litWrongPct2.Text = "0";
                    litNotAttPct.Text = litNotAttPct2.Text = "0";
                }

                // Store in session for the PDF generator
                Session["ResultForPdf"] = resultDoc;
            }
            catch (Exception ex)
            {
                lblMsg.Text = "⚠️ An error occurred while loading your results. Please try again.";
                lblMsg.Visible = true;
                // Consider logging 'ex.Message' for debugging purposes
            }
        }

        protected void btnReview_Click(object sender, EventArgs e) => Response.Redirect("~/ExamReview.aspx");

        protected void btnDownloadPdf_Click(object sender, EventArgs e) => Response.Redirect("~/DownloadResultPdf.aspx");
    }
}