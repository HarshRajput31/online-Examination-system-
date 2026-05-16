using System;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OnlineExaminationSystem
{
    public partial class DownloadResultPdf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null) return;

            string studentId = Session["UserId"].ToString();

            var client = new MongoClient(
                "mongodb://localhost:27017");
            var db = client.GetDatabase("OnlineExamDB");
            var resultsCol = db.GetCollection<BsonDocument>(
                "examResults");
            var usersCol = db.GetCollection<BsonDocument>(
                "users");

            var result = resultsCol
                .Find(Builders<BsonDocument>.Filter
                    .Eq("studentId", studentId))
                .SortByDescending(r => r["submittedAt"])
                .FirstOrDefault();

            if (result == null) { Response.Write("No result found."); return; }

            var student = usersCol.Find(
                Builders<BsonDocument>.Filter
                    .Eq("userId", studentId)
            ).FirstOrDefault();

            string sName = student?.GetValue("name", "Student").ToString() ?? studentId;
            string sDept = student?.GetValue("course", "").ToString() ?? "";
            string examId = result.GetValue("examId", "").ToString();

            // Rank
            var allR = resultsCol.Find(
                Builders<BsonDocument>.Filter.Eq("examId", examId))
                .SortByDescending(r => r["totalScore"]).ToList();
            int rank = 1;
            for (int i = 0; i < allR.Count; i++)
                if (allR[i].GetValue("studentId", "").ToString() == studentId)
                { rank = i + 1; break; }

            string html = BuildPdfHtml(result, sName, sDept, rank);

            Response.Clear();
            Response.ContentType = "text/html";
            Response.AddHeader("Content-Disposition",
                "inline; filename=ExamResult_" + studentId + ".pdf");
            Response.Write(html);
            Response.End();
        }

        private string BuildPdfHtml(
            BsonDocument r, string name, string dept, int rank)
        {
            string subject = r.GetValue("subject", "").ToString();
            string title = r.GetValue("examTitle", "").ToString();
            int totalQ = r.GetValue("totalQuestions", 0).ToInt32();
            int correct = r.GetValue("correctAnswers", 0).ToInt32();
            int wrong = r.GetValue("wrongAnswers", 0).ToInt32();
            int notAtt = r.GetValue("notAttempted", 0).ToInt32();
            int score = r.GetValue("totalScore", 0).ToInt32();
            int totalM = r.GetValue("totalMarks", 0).ToInt32();
            double pct = r.GetValue("percentage", 0).ToDouble();
            bool passed = r.GetValue("passed", false).ToBoolean();
            string date = r.GetValue("submittedAt", DateTime.UtcNow)
                                .ToUniversalTime().ToLocalTime()
                                .ToString("dd MMM yyyy, hh:mm tt");

            string rankIcon = rank == 1 ? "🥇" : rank == 2 ? "🥈" : rank == 3 ? "🥉" : "#" + rank;
            string initial = name.Length > 0 ? name[0].ToString().ToUpper() : "S";

            return $@"<!DOCTYPE html>
<html>
<head><meta charset='utf-8'/>
<title>Exam Result — {name}</title>
<style>
  * {{ margin:0; padding:0; box-sizing:border-box; }}
  body {{ font-family:'Segoe UI',Arial,sans-serif;
          background:#f8fafc; color:#1a1a1a; padding:40px; }}
  .card {{ background:white; border-radius:16px;
           padding:32px; margin-bottom:24px;
           box-shadow:0 2px 12px rgba(0,0,0,0.08); }}
  .header {{ background:linear-gradient(135deg,#1e3a5f,#2d5a9b);
             color:white; border-radius:16px; padding:32px;
             margin-bottom:24px; display:flex;
             justify-content:space-between; align-items:center; }}
  .header-left h1 {{ font-size:26px; font-weight:800; margin-bottom:4px; }}
  .header-left p  {{ font-size:13px; opacity:0.8; }}
  .header-right {{ text-align:right; font-size:13px; opacity:0.8; }}
  .avatar {{ width:80px; height:80px; border-radius:50%;
             background:rgba(255,255,255,0.2);
             display:flex; align-items:center;
             justify-content:center; font-size:32px;
             font-weight:800; color:white;
             margin:0 auto 16px; border:3px solid rgba(255,255,255,0.4); }}
  .grid {{ display:grid; grid-template-columns:1fr 1fr; gap:24px; }}
  .section-title {{ font-size:14px; font-weight:800;
                    color:#1e3a5f; margin-bottom:16px;
                    padding-bottom:8px;
                    border-bottom:2px solid #e2e8f0;
                    text-transform:uppercase; letter-spacing:0.5px; }}
  .info-row {{ display:flex; justify-content:space-between;
               align-items:center; padding:8px 0;
               border-bottom:1px solid #f1f5f9; font-size:13px; }}
  .info-label {{ color:#64748b; font-weight:600; }}
  .info-val {{ color:#1a1a1a; font-weight:700; }}
  .score-circle {{ text-align:center; padding:24px;
                   background:#f8fafc; border-radius:12px;
                   margin-bottom:16px; }}
  .score-big {{ font-size:36px; font-weight:800;
                color:#1e3a5f; display:block; }}
  .score-pct {{ font-size:22px; font-weight:700;
                color:#64748b; }}
  .status {{ display:inline-block; padding:8px 20px;
             border-radius:20px; font-size:14px;
             font-weight:800; }}
  .pass {{ background:#dcfce7; color:#16a34a; }}
  .fail {{ background:#fee2e2; color:#dc2626; }}
  .stats-grid {{ display:grid;
                 grid-template-columns:repeat(4,1fr);
                 gap:12px; margin-top:16px; }}
  .stat-box {{ text-align:center; padding:14px 10px;
               border-radius:10px; }}
  .stat-box.total   {{ background:#eff6ff; }}
  .stat-box.correct {{ background:#f0fdf4; }}
  .stat-box.wrong   {{ background:#fef2f2; }}
  .stat-box.skip    {{ background:#f9fafb; }}
  .stat-num {{ font-size:24px; font-weight:800; display:block; }}
  .stat-lbl {{ font-size:11px; font-weight:600;
               color:#64748b; text-transform:uppercase; }}
  .stat-box.total   .stat-num {{ color:#1d4ed8; }}
  .stat-box.correct .stat-num {{ color:#16a34a; }}
  .stat-box.wrong   .stat-num {{ color:#dc2626; }}
  .stat-box.skip    .stat-num {{ color:#64748b; }}
  .bar-row {{ display:flex; align-items:center;
              gap:10px; margin-bottom:10px; font-size:12px; }}
  .bar-label {{ width:80px; color:#64748b; font-weight:600; }}
  .bar-track {{ flex:1; height:8px;
                background:#f1f5f9; border-radius:10px;
                overflow:hidden; }}
  .bar-fill {{ height:100%; border-radius:10px; }}
  .bar-green {{ background:#22c55e; }}
  .bar-red   {{ background:#ef4444; }}
  .bar-gray  {{ background:#94a3b8; }}
  .bar-pct {{ width:35px; font-weight:700; color:#1a1a1a; }}
  .rank-box {{ text-align:center; padding:20px;
               background:linear-gradient(135deg,#fefce8,#fef9c3);
               border-radius:12px; border:2px solid #fbbf24;
               margin-top:16px; }}
  .rank-icon {{ font-size:36px; display:block;
                margin-bottom:6px; }}
  .rank-num {{ font-size:28px; font-weight:800;
               color:#92400e; }}
  .rank-label {{ font-size:12px; color:#78350f;
                 font-weight:600; }}
  .review-table {{ width:100%; border-collapse:collapse;
                   font-size:12px; }}
  .review-table th {{ background:#f1f5f9; padding:10px;
                      text-align:left; font-weight:700;
                      color:#64748b; border:1px solid #e2e8f0; }}
  .review-table td {{ padding:10px; border:1px solid #e2e8f0; }}
  .r-correct {{ color:#16a34a; font-weight:700; }}
  .r-wrong   {{ color:#dc2626; font-weight:700; }}
  .r-skip    {{ color:#64748b; font-weight:700; }}
  .footer {{ text-align:center; margin-top:24px;
             font-size:11px; color:#94a3b8; }}
  .print-btn {{ position:fixed; bottom:20px; right:20px;
                padding:12px 24px; background:#1e3a5f;
                color:white; border:none; border-radius:10px;
                font-size:14px; font-weight:700;
                cursor:pointer; box-shadow:0 4px 16px rgba(0,0,0,0.3); }}
  @media print {{ .print-btn {{ display:none; }} }}
</style>
</head>
<body>
<button class='print-btn' onclick='window.print()'>
    🖨️ Print / Save PDF
</button>

<div class='header'>
  <div class='header-left'>
    <h1>🎓 Online Examination System</h1>
    <p>Exam Result Report — Generated {DateTime.Now:dd MMM yyyy, hh:mm tt}</p>
  </div>
  <div class='header-right'>
    <div style='font-size:40px;'>{rankIcon}</div>
    <div style='font-weight:800;font-size:16px;'>Rank</div>
  </div>
</div>

<div class='grid'>

  <%-- STUDENT CARD --%>
  <div class='card'>
    <div class='section-title'>👤 Student Information</div>
    <div class='avatar'>{initial}</div>
    <div class='info-row'>
      <span class='info-label'>Full Name</span>
      <span class='info-val'>{name}</span>
    </div>
    <div class='info-row'>
      <span class='info-label'>Student ID</span>
      <span class='info-val'>{r.GetValue("studentId", "")}</span>
    </div>
    <div class='info-row'>
      <span class='info-label'>Department</span>
      <span class='info-val'>{dept}</span>
    </div>
    <div class='info-row'>
      <span class='info-label'>Exam ID</span>
      <span class='info-val'>{r.GetValue("examId", "")}</span>
    </div>
    <div class='info-row'>
      <span class='info-label'>Subject</span>
      <span class='info-val'>{subject}</span>
    </div>
    <div class='info-row'>
      <span class='info-label'>Exam</span>
      <span class='info-val'>{title}</span>
    </div>
    <div class='info-row'>
      <span class='info-label'>Date</span>
      <span class='info-val'>{date}</span>
    </div>
    <div class='rank-box'>
      <span class='rank-icon'>{rankIcon}</span>
      <div class='rank-num'>Rank #{rank}</div>
      <div class='rank-label'>in this exam</div>
    </div>
  </div>

  <%-- SCORE CARD --%>
  <div class='card'>
    <div class='section-title'>📊 Score Summary</div>
    <div class='score-circle'>
      <span class='score-big'>{score} / {totalM}</span>
      <span class='score-pct'>{pct:F1}%</span>
    </div>
    <div style='text-align:center;margin-bottom:16px;'>
      <span class='status {(passed ? "pass" : "fail")}'>
        {(passed ? "✅ PASSED" : "❌ FAILED")}
      </span>
    </div>
    <div class='stats-grid'>
      <div class='stat-box total'>
        <span class='stat-num'>{totalQ}</span>
        <span class='stat-lbl'>Total</span>
      </div>
      <div class='stat-box correct'>
        <span class='stat-num'>{correct}</span>
        <span class='stat-lbl'>Correct</span>
      </div>
      <div class='stat-box wrong'>
        <span class='stat-num'>{wrong}</span>
        <span class='stat-lbl'>Wrong</span>
      </div>
      <div class='stat-box skip'>
        <span class='stat-num'>{notAtt}</span>
        <span class='stat-lbl'>Skipped</span>
      </div>
    </div>
    <div style='margin-top:20px;'>
      <div class='bar-row'>
        <span class='bar-label'>✅ Correct</span>
        <div class='bar-track'>
          <div class='bar-fill bar-green'
               style='width:{(totalQ > 0 ? correct * 100 / totalQ : 0)}%'></div>
        </div>
        <span class='bar-pct'>
          {(totalQ > 0 ? correct * 100 / totalQ : 0)}%
        </span>
      </div>
      <div class='bar-row'>
        <span class='bar-label'>❌ Wrong</span>
        <div class='bar-track'>
          <div class='bar-fill bar-red'
               style='width:{(totalQ > 0 ? wrong * 100 / totalQ : 0)}%'></div>
        </div>
        <span class='bar-pct'>
          {(totalQ > 0 ? wrong * 100 / totalQ : 0)}%
        </span>
      </div>
      <div class='bar-row'>
        <span class='bar-label'>⬜ Skipped</span>
        <div class='bar-track'>
          <div class='bar-fill bar-gray'
               style='width:{(totalQ > 0 ? notAtt * 100 / totalQ : 0)}%'></div>
        </div>
        <span class='bar-pct'>
          {(totalQ > 0 ? notAtt * 100 / totalQ : 0)}%
        </span>
      </div>
    </div>
  </div>

</div>

<div class='footer'>
  Online Examination System | {name} | {subject} | {date}
</div>
</body></html>";
        }
    }
}