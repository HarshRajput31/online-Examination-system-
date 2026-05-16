<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentProfile.aspx.cs"
    Inherits="OnlineExaminationSystem.StudentProfile" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">👤</span>
    <div>
        <h2 class="ce-page-title">My Profile</h2>
        <p class="ce-page-subtitle">Update your personal details and contact info.</p>
    </div>
</div>

<div class="ce-form-card" style="max-width:780px;">
    <asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

    <div class="ce-row-2">
        <div class="ce-field">
            <label class="ce-label">Student ID</label>
            <asp:TextBox ID="txtStudentId" runat="server" CssClass="ce-input" ReadOnly="true" />
        </div>
        <div class="ce-field">
            <label class="ce-label">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="ce-input" ReadOnly="true" />
        </div>
    </div>

    <div class="ce-row-2">
        <div class="ce-field">
            <label class="ce-label">Full Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="ce-input" />
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

    <div class="ce-field">
        <label class="ce-label">Roll Number</label>
        <asp:TextBox ID="txtRoll" runat="server" CssClass="ce-input" />
    </div>

    <hr class="ce-divider" />
    <div class="ce-btn-row">
        <asp:Button ID="btnSave" runat="server" Text="💾 Save changes" CssClass="ce-btn-create" OnClick="btnSave_Click" />
    </div>
</div>

</asp:Content>
