<%@ Page Title="Admin Results"
Language="C#"
MasterPageFile="~/Site.Master"
AutoEventWireup="true"
CodeBehind="AdminResults.aspx.cs"
Inherits="OnlineExaminationSystem.AdminResults" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="container-box">
<div class="form-box">

<h2>All Student Results</h2>

<asp:GridView ID="gvResults"
runat="server"
AutoGenerateColumns="false"
Width="100%"
CssClass="gridview">

<Columns>

<asp:BoundField DataField="StudentName"
HeaderText="Student" />

<asp:BoundField DataField="ExamName"
HeaderText="Exam" />

<asp:BoundField DataField="Score"
HeaderText="Score" />

<asp:BoundField DataField="TotalQuestions"
HeaderText="Total Questions" />

<asp:BoundField DataField="SubmittedAt"
HeaderText="Date" />

</Columns>

</asp:GridView>

<br />

<asp:Label ID="lblMsg"
runat="server"
CssClass="error-msg" />

</div>
</div>

</asp:Content>