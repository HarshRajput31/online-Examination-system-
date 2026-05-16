<%@ Page Title="Available Exams" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentExams.aspx.cs"
    Inherits="OnlineExaminationSystem.StudentExams" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">📚</span>
    <div>
        <h2 class="ce-page-title">Available Exams</h2>
        <p class="ce-page-subtitle">All approved &amp; published exams you can attempt.</p>
    </div>
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    No exams are available at the moment.
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
                    <i class="fa-solid fa-play"></i>&nbsp; Start
                </a>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>

</asp:Content>
