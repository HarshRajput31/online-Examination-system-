<%@ Page Title="View Student" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ViewStudent.aspx.cs"
    Inherits="OnlineExaminationSystem.ViewStudent" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">🎓</span>
    <div>
        <h2 class="ce-page-title">Student Profile</h2>
        <p class="ce-page-subtitle">Read-only view.</p>
    </div>
</div>

<div class="ce-form-card">
    <asp:Label ID="lblNotFound" runat="server" CssClass="alert alert-warning" Visible="false" Text="Student not found." />
    <table class="table">
        <tr><th>Student ID</th> <td><asp:Literal ID="litId" runat="server" /></td></tr>
        <tr><th>Name</th>       <td><asp:Literal ID="litName" runat="server" /></td></tr>
        <tr><th>Email</th>      <td><asp:Literal ID="litEmail" runat="server" /></td></tr>
        <tr><th>Roll No.</th>   <td><asp:Literal ID="litRoll" runat="server" /></td></tr>
        <tr><th>Course</th>     <td><asp:Literal ID="litCourse" runat="server" /></td></tr>
        <tr><th>Department</th> <td><asp:Literal ID="litDept" runat="server" /></td></tr>
        <tr><th>Mobile</th>     <td><asp:Literal ID="litMobile" runat="server" /></td></tr>
    </table>
</div>

</asp:Content>
