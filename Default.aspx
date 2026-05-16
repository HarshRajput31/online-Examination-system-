<%@ Page Title="Online Examination System" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="OnlineExaminationSystem._Default" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<!-- ================ HERO ================ -->
<section class="hero-section glass-card fade-in" style="text-align:center; padding:60px 30px; margin-bottom:30px;">
    <div style="font-size:14px; letter-spacing:3px; color:#38bdf8; text-transform:uppercase; margin-bottom:14px;">
        Next-Gen Examination Platform
    </div>
    <h1 style="font-size:46px; font-weight:800; line-height:1.15; margin:0 0 18px;
               background:linear-gradient(135deg,#fff,#cbd5e1); -webkit-background-clip:text; background-clip:text; color:transparent;">
        Conduct Secure Online Exams<br/>From Anywhere.
    </h1>
    <p style="max-width:680px; margin:0 auto 30px; color:#94a3b8; font-size:16px; line-height:1.6;">
        A complete, role-based examination platform for Admins, Faculty, and Students &mdash;
        with auto-grading, recheck workflow, leaderboards, and anti-cheat exam mode.
    </p>

    <div style="display:flex; gap:14px; justify-content:center; flex-wrap:wrap;">
        <asp:HyperLink runat="server" NavigateUrl="~/Login.aspx" CssClass="btn btn-primary"
            Text='<i class="fa-solid fa-right-to-bracket"></i> &nbsp; Login to your account' />
        <asp:HyperLink runat="server" NavigateUrl="~/StudentRegistration.aspx" CssClass="btn btn-secondary"
            Text='<i class="fa-solid fa-user-plus"></i> &nbsp; Register as Student' />
    </div>
</section>

<!-- ================ ROLE CARDS ================ -->
<div class="exam-grid" style="margin-top:30px;">

    <div class="exam-card">
        <div class="ec-subject"><i class="fa-solid fa-shield-halved"></i> &nbsp; Admin</div>
        <div class="ec-title">Manage everything in one place</div>
        <div class="ec-meta">
            <span><i class="fa-solid fa-users"></i> Faculty &amp; students</span>
            <span><i class="fa-solid fa-check"></i> Approve exams</span>
            <span><i class="fa-solid fa-bell"></i> Notifications</span>
        </div>
    </div>

    <div class="exam-card">
        <div class="ec-subject"><i class="fa-solid fa-chalkboard-user"></i> &nbsp; Faculty</div>
        <div class="ec-title">Author papers and grade with ease</div>
        <div class="ec-meta">
            <span><i class="fa-solid fa-pen"></i> Create exams</span>
            <span><i class="fa-solid fa-list-check"></i> MCQ + descriptive</span>
            <span><i class="fa-solid fa-rotate"></i> Recheck</span>
        </div>
    </div>

    <div class="exam-card">
        <div class="ec-subject"><i class="fa-solid fa-user-graduate"></i> &nbsp; Student</div>
        <div class="ec-title">Take exams &amp; track results</div>
        <div class="ec-meta">
            <span><i class="fa-solid fa-clock"></i> Live timer</span>
            <span><i class="fa-solid fa-trophy"></i> Leaderboard</span>
            <span><i class="fa-solid fa-file-pdf"></i> PDF report</span>
        </div>
    </div>

</div>

<!-- ================ FEATURE STRIP ================ -->
<section class="glass-card" style="padding:28px; margin-top:30px;">
    <div class="row text-center" style="display:grid; grid-template-columns:repeat(auto-fit,minmax(180px,1fr)); gap:24px;">
        <div>
            <div style="font-size:28px; color:#38bdf8;"><i class="fa-solid fa-lock"></i></div>
            <div style="margin-top:8px; font-weight:700;">Anti-cheat exam mode</div>
            <div style="color:#94a3b8; font-size:12px;">Tab-switch detection &amp; auto-submit</div>
        </div>
        <div>
            <div style="font-size:28px; color:#10b981;"><i class="fa-solid fa-bolt"></i></div>
            <div style="margin-top:8px; font-weight:700;">Auto MCQ grading</div>
            <div style="color:#94a3b8; font-size:12px;">Instant scores &amp; ranks</div>
        </div>
        <div>
            <div style="font-size:28px; color:#a855f7;"><i class="fa-solid fa-rotate"></i></div>
            <div style="margin-top:8px; font-weight:700;">Recheck workflow</div>
            <div style="color:#94a3b8; font-size:12px;">Student &rarr; Admin &rarr; Faculty</div>
        </div>
        <div>
            <div style="font-size:28px; color:#f59e0b;"><i class="fa-solid fa-trophy"></i></div>
            <div style="margin-top:8px; font-weight:700;">Leaderboards</div>
            <div style="color:#94a3b8; font-size:12px;">Department &amp; subject toppers</div>
        </div>
    </div>
</section>

</asp:Content>
