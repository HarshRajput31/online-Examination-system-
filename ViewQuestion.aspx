<%@ Page Title="View Question" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ViewQuestion.aspx.cs"
    Inherits="OnlineExaminationSystem.ViewQuestion" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">❓</span>
    <div>
        <h2 class="ce-page-title">Question Details</h2>
        <p class="ce-page-subtitle">Read-only view.</p>
    </div>
</div>

<div class="ce-form-card">
    <asp:Label ID="lblNotFound" runat="server" CssClass="alert alert-warning" Visible="false" Text="Question not found." />
    <table class="table">
        <tr><th>Question ID</th><td><asp:Literal ID="litId" runat="server" /></td></tr>
        <tr><th>Type</th>       <td><asp:Literal ID="litType" runat="server" /></td></tr>
        <tr><th>Exam ID</th>    <td><asp:Literal ID="litExam" runat="server" /></td></tr>
        <tr><th>Subject</th>    <td><asp:Literal ID="litSubject" runat="server" /></td></tr>
        <tr><th>Question</th>   <td><asp:Literal ID="litText" runat="server" /></td></tr>
        <tr><th>Option A</th>   <td><asp:Literal ID="litA" runat="server" /></td></tr>
        <tr><th>Option B</th>   <td><asp:Literal ID="litB" runat="server" /></td></tr>
        <tr><th>Option C</th>   <td><asp:Literal ID="litC" runat="server" /></td></tr>
        <tr><th>Option D</th>   <td><asp:Literal ID="litD" runat="server" /></td></tr>
        <tr><th>Correct</th>    <td><asp:Literal ID="litCorrect" runat="server" /></td></tr>
        <tr><th>Marks</th>      <td><asp:Literal ID="litMarks" runat="server" /></td></tr>
    </table>
</div>

</asp:Content>
