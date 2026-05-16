<%@ Page Title="Student Dashboard" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentDashboard.aspx.cs"
    Inherits="OnlineExaminationSystem.StudentDashboard" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="dashboard-container">

    <!-- HERO -->
    <div class="ce-page-header" style="margin-bottom:24px;">
        <span class="ce-page-icon">🎓</span>
        <div>
            <h2 class="ce-page-title">Welcome back, <asp:Literal ID="litStudentName" runat="server" Text="Student" /></h2>
            <p class="ce-page-subtitle">Here are your upcoming exams, latest results, and announcements.</p>
        </div>
    </div>

    <!-- STAT CARDS -->
    <div class="row g-4">
        <div class="col-md-3 col-sm-6">
            <div class="stat-card text-center">
                <h6>UPCOMING</h6>
                <h2><asp:Label ID="lblUpcoming" runat="server" Text="0" /></h2>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="stat-card text-center">
                <h6>COMPLETED</h6>
                <h2><asp:Label ID="lblCompleted" runat="server" Text="0" /></h2>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="stat-card text-center">
                <h6>AVG SCORE</h6>
                <h2><asp:Label ID="lblAvgScore" runat="server" Text="0%" /></h2>
            </div>
        </div>
        <div class="col-md-3 col-sm-6">
            <div class="stat-card text-center">
                <h6>NOTIFICATIONS</h6>
                <h2><asp:Label ID="lblNotifications" runat="server" Text="0" /></h2>
            </div>
        </div>
    </div>

    <!-- AVAILABLE EXAMS -->
    <h3 style="margin:36px 0 16px;">📝 Available Exams</h3>
    <asp:Panel ID="pnlNoExams" runat="server" Visible="false" CssClass="alert alert-info">
        No published exams are available right now. Check back soon.
    </asp:Panel>

    <div class="exam-grid">
        <asp:Repeater ID="rptExams" runat="server">
            <ItemTemplate>
                <div class="exam-card">
                    <div class="ec-subject"><i class="fa-solid fa-book"></i> <%# Eval("Subject") %></div>
                    <div class="ec-title"><%# Eval("Title") %></div>
                    <div class="ec-meta">
                        <span><i class="fa-solid fa-clock"></i><%# Eval("Duration") %> min</span>
                        <span><i class="fa-solid fa-trophy"></i><%# Eval("TotalMarks") %> marks</span>
                        <span><i class="fa-solid fa-list-ol"></i><%# Eval("QuestionCount") %> Qs</span>
                    </div>
                    <a class="btn btn-primary" style="width:100%;"
                       href='<%# "ExamInstructions.aspx?ExamId=" + Eval("ExamId") %>'>
                        <i class="fa-solid fa-play"></i> &nbsp; Start Exam
                    </a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <!-- RECENT RESULTS -->
    <h3 style="margin:36px 0 16px;">📊 Recent Results</h3>
    <asp:Panel ID="pnlNoResults" runat="server" Visible="false" CssClass="alert alert-info">
        You haven't completed any exams yet.
    </asp:Panel>
    <asp:Repeater ID="rptRecent" runat="server">
        <ItemTemplate>
            <div class="list-row">
                <div style="flex:1;">
                    <strong><%# Eval("ExamName") %></strong>
                    <div style="color:#94a3b8; font-size:12px;">
                        <%# Eval("Subject") %> &middot; <%# Eval("SubmittedAt", "{0:dd MMM yyyy}") %>
                    </div>
                </div>
                <div style="text-align:right; min-width:140px;">
                    <div style="font-weight:700;">
                        <%# Eval("Score") %> / <%# Eval("TotalQuestions") %>
                    </div>
                    <span class='<%# (bool)Eval("Passed") ? "status-badge status-pass" : "status-badge status-fail" %>'>
                        <%# (bool)Eval("Passed") ? "PASS" : "FAIL" %>
                    </span>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

</div>
</asp:Content>
