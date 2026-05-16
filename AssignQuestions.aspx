<%@ Page Title="Assign Questions"
Language="C#"
MasterPageFile="~/Site.Master"
AutoEventWireup="true"
CodeBehind="AssignQuestions.aspx.cs"
Inherits="OnlineExaminationSystem.AssignQuestions" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="main-content">

<div class="form-wrapper">

<h2 class="page-title">Assign Questions to Exam</h2>

<div class="form-card">

<!-- ===============================
     SELECT EXAM
================================ -->

<div class="form-group">

<label>Select Exam</label>

<asp:DropDownList ID="ddlExams"
    runat="server"
    CssClass="form-control-custom"
    AutoPostBack="true"
    OnSelectedIndexChanged="ddlExams_SelectedIndexChanged">
</asp:DropDownList>

</div>

<br />

<!-- ===============================
     QUESTION LIST
================================ -->

<div class="table-card">

<asp:GridView ID="gvQuestions"
    runat="server"
    AutoGenerateColumns="false"
    CssClass="faculty-table"
    Width="100%">

<Columns>

<!-- SELECT CHECKBOX -->

<asp:TemplateField HeaderText="Select">
<ItemTemplate>

<asp:CheckBox ID="chkSelect"
    runat="server" />

</ItemTemplate>
</asp:TemplateField>

<!-- QUESTION ID -->

<asp:BoundField
    DataField="QuestionId"
    HeaderText="Question ID" />

<!-- QUESTION TEXT -->

<asp:BoundField
    DataField="QuestionText"
    HeaderText="Question" />

<!-- DIFFICULTY -->

<asp:BoundField
    DataField="Difficulty"
    HeaderText="Difficulty" />

<!-- MARKS -->

<asp:BoundField
    DataField="Marks"
    HeaderText="Marks" />

</Columns>

</asp:GridView>

</div>

<br />

<!-- ===============================
     SAVE BUTTON
================================ -->

<asp:Button ID="btnSave"
    runat="server"
    Text="Save Assignment"
    CssClass="btn-modern"
    OnClick="btnSave_Click" />

<br /><br />

<!-- ===============================
     MESSAGE LABEL
================================ -->

<asp:Label ID="lblMsg"
    runat="server"
    CssClass="success-msg" />

</div>

</div>

</div>

</asp:Content>