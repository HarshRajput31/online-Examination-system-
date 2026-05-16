<%@ Page Title="Exam Result" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Result.aspx.cs" Inherits="OnlineExaminationSystem.Result" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="result-page">

    <%-- HEADER --%>
    <div class="result-header">
        <div class="result-header-icon">🎉</div>
        <h2 class="result-header-title">Exam Result</h2>
        <p class="result-header-sub">Your performance summary</p>
    </div>

    <asp:Label ID="lblMsg" runat="server" Visible="false" CssClass="result-error" />

    <div class="result-grid">
        <%-- LEFT: STUDENT CARD --%>
        <div class="result-card result-student-card">
            <div class="rc-title">👤 Student Details</div>
            <div class="rc-avatar">
                <div class="rc-avatar-circle">
                    <asp:Literal ID="litInitial" runat="server" Text="S" />
                </div>
            </div>
            <div class="rc-info-row">
                <span class="rc-label">Name</span>
                <span class="rc-val"><asp:Label ID="lblStudentName" runat="server" /></span>
            </div>
            <div class="rc-info-row">
                <span class="rc-label">Student ID</span>
                <span class="rc-val"><asp:Label ID="lblStudentId" runat="server" /></span>
            </div>
            <div class="rc-info-row">
                <span class="rc-label">Department</span>
                <span class="rc-val"><asp:Label ID="lblDept" runat="server" /></span>
            </div>
            <div class="rc-info-row">
                <span class="rc-label">Exam ID</span>
                <span class="rc-val"><asp:Label ID="lblExamId" runat="server" /></span>
            </div>
            <div class="rc-info-row">
                <span class="rc-label">Subject</span>
                <span class="rc-val"><asp:Label ID="lblSubject" runat="server" /></span>
            </div>
            <div class="rc-info-row">
                <span class="rc-label">Exam Title</span>
                <span class="rc-val"><asp:Label ID="lblExamTitle" runat="server" /></span>
            </div>

            <div class="rc-rank-box">
                <div class="rc-rank-icon">🏆</div>
                <div class="rc-rank-num"><asp:Label ID="lblRank" runat="server" /></div>
                <div class="rc-rank-label">Your Rank</div>
            </div>
        </div>

        <%-- RIGHT: SCORE CARD --%>
        <div class="result-card result-score-card">
            <div class="rc-title">📊 Score Summary</div>
            <div class="rc-score-circle">
                <div class="rc-score-num"><asp:Label ID="lblScore" runat="server" /></div>
                <div class="rc-score-pct"><asp:Label ID="lblPct" runat="server" /></div>
            </div>

            <div class="rc-status-center">
                <asp:Label ID="lblStatus" runat="server" CssClass="rc-status-badge" />
            </div>

            <div class="rc-stats-grid">
                <div class="rc-stat rc-stat-total">
                    <div class="rc-stat-num"><asp:Label ID="lblTotalQ" runat="server" Text="0" /></div>
                    <div class="rc-stat-label">Total</div>
                </div>
                <div class="rc-stat rc-stat-correct">
                    <div class="rc-stat-num"><asp:Label ID="lblCorrect" runat="server" Text="0" /></div>
                    <div class="rc-stat-label">Correct</div>
                </div>
                <div class="rc-stat rc-stat-wrong">
                    <div class="rc-stat-num"><asp:Label ID="lblWrong" runat="server" Text="0" /></div>
                    <div class="rc-stat-label">Wrong</div>
                </div>
                <div class="rc-stat rc-stat-skip">
                    <div class="rc-stat-num"><asp:Label ID="lblNotAtt" runat="server" Text="0" /></div>
                    <div class="rc-stat-label">Skipped</div>
                </div>
            </div>

            <%-- PERFORMANCE BARS --%>
            <div class="rc-bars">
                <div class="rc-bar-row">
                    <span class="rc-bar-label">✅ Correct</span>
                    <div class="rc-bar-track">
                        <%-- Logic: Width is bound to the Literal text value --%>
                        <div class="rc-bar-fill rc-bar-green" style='width: <%= litCorrectPct.Text %>%'></div>
                    </div>
                    <span class="rc-bar-pct"><asp:Literal ID="litCorrectPct" runat="server" />%</span>
                </div>
                <div class="rc-bar-row">
                    <span class="rc-bar-label">❌ Wrong</span>
                    <div class="rc-bar-track">
                        <div class="rc-bar-fill rc-bar-red" style='width: <%= litWrongPct.Text %>%'></div>
                    </div>
                    <span class="rc-bar-pct"><asp:Literal ID="litWrongPct" runat="server" />%</span>
                </div>
                <div class="rc-bar-row">
                    <span class="rc-bar-label">⬜ Skipped</span>
                    <div class="rc-bar-track">
                        <div class="rc-bar-fill rc-bar-gray" style='width: <%= litNotAttPct.Text %>%'></div>
                    </div>
                    <span class="rc-bar-pct"><asp:Literal ID="litNotAttPct" runat="server" />%</span>
                </div>
            </div>
            
            <%-- Secondary Literals for C# logic compatibility --%>
            <asp:Literal ID="litCorrectPct2" runat="server" Visible="false" />
            <asp:Literal ID="litWrongPct2" runat="server" Visible="false" />
            <asp:Literal ID="litNotAttPct2" runat="server" Visible="false" />
        </div>
    </div>

    <%-- ACTION BUTTONS --%>
    <div class="result-actions">
        <asp:Button ID="btnReview" runat="server" Text="🔍 Review Exam" CssClass="result-btn result-btn-review" OnClick="btnReview_Click" />
        <asp:Button ID="btnDownloadPdf" runat="server" Text="📄 Download PDF" CssClass="result-btn result-btn-pdf" OnClick="btnDownloadPdf_Click" />
        <a href="StudentDashboard.aspx" class="result-btn result-btn-home">🏠 Dashboard</a>
    </div>
</div>
</asp:Content>