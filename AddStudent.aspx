<%@ Page Title="Add Student" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="AddStudent.aspx.cs"
    Inherits="OnlineExaminationSystem.AddStudent" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">🎓</span>
    <div>
        <h2 class="ce-page-title">Add Student</h2>
        <p class="ce-page-subtitle">Create a student account. The student will receive an invite link to set their password.</p>
    </div>
</div>

<div class="ce-form-card" style="max-width:760px;">
    <asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

    <div class="ce-row-2">
        <div class="ce-field">
            <label class="ce-label">Full Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="ce-input" />
        </div>
        <div class="ce-field">
            <label class="ce-label">Personal Email</label>
            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="ce-input" />
        </div>
    </div>

    <div class="ce-row-2">
        <div class="ce-field">
            <label class="ce-label">Roll Number</label>
            <asp:TextBox ID="txtRoll" runat="server" CssClass="ce-input" />
        </div>
        <div class="ce-field">
            <label class="ce-label">Mobile</label>
            <asp:TextBox ID="txtMobile" runat="server" CssClass="ce-input" />
        </div>
    </div>

    <div class="ce-row-2">
        <div class="ce-field">
            <label class="ce-label">Course</label>
            <asp:TextBox ID="txtCourse" runat="server" CssClass="ce-input" />
        </div>
        <div class="ce-field">
            <label class="ce-label">Department</label>
            <asp:TextBox ID="txtDept" runat="server" CssClass="ce-input" />
        </div>
    </div>

    <hr class="ce-divider" />
    <div class="ce-btn-row">
        <asp:Button ID="btnAdd" runat="server" Text="📨 Create &amp; Invite"
            CssClass="ce-btn-create" OnClick="btnAdd_Click" />
    </div>
</div>

</asp:Content>
