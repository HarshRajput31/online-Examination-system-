<%@ Page Title="View Exam" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ViewExam.aspx.cs"
    Inherits="OnlineExaminationSystem.ViewExam" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">📄</span>
    <div>
        <h2 class="ce-page-title">Exam Details</h2>
        <p class="ce-page-subtitle">Read-only view of an exam.</p>
    </div>
</div>

<div class="ce-form-card">
    <asp:Label ID="lblNotFound" runat="server" CssClass="alert alert-warning" Visible="false" Text="Exam not found." />

    <table class="table" style="width:100%;">
        <tr><th>Exam ID</th>     <td><asp:Literal ID="litExamId" runat="server" /></td></tr>
        <tr><th>Title</th>       <td><asp:Literal ID="litTitle" runat="server" /></td></tr>
        <tr><th>Subject</th>     <td><asp:Literal ID="litSubject" runat="server" /></td></tr>
        <tr><th>Set</th>         <td><asp:Literal ID="litSet" runat="server" /></td></tr>
        <tr><th>Duration</th>    <td><asp:Literal ID="litDuration" runat="server" /> min</td></tr>
        <tr><th>Total Marks</th> <td><asp:Literal ID="litMarks" runat="server" /></td></tr>
        <tr><th>Status</th>      <td><asp:Literal ID="litStatus" runat="server" /></td></tr>
        <tr><th>Created By</th>  <td><asp:Literal ID="litCreator" runat="server" /></td></tr>
        <tr><th>Created At</th>  <td><asp:Literal ID="litCreated" runat="server" /></td></tr>
        <tr><th>Question Count</th><td><asp:Literal ID="litCount" runat="server" /></td></tr>
    </table>
</div>

</asp:Content>
