<%@ Page Title="Student Registration" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentRegistration.aspx.cs"
    Inherits="OnlineExaminationSystem.StudentRegistration" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">🎓</span>
    <div>
        <h2 class="ce-page-title">Register as Student</h2>
        <p class="ce-page-subtitle">Create your account &mdash; you'll receive an email link to set your password.</p>
    </div>
</div>

<div class="ce-form-card" style="max-width:680px; margin:0 auto;">
    <asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

    <div class="ce-row-2">
        <div class="ce-field">
            <label class="ce-label">Full Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="ce-input" />
        </div>
        <div class="ce-field">
            <label class="ce-label">Email (login)</label>
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
            <asp:TextBox ID="txtCourse" runat="server" CssClass="ce-input" placeholder="e.g. B.Tech CSE" />
        </div>
        <div class="ce-field">
            <label class="ce-label">Department</label>
            <asp:TextBox ID="txtDept" runat="server" CssClass="ce-input" placeholder="e.g. Computer Science" />
        </div>
    </div>

    <div class="ce-row-2">
        <div class="ce-field">
            <label class="ce-label">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="ce-input" />
        </div>
        <div class="ce-field">
            <label class="ce-label">Confirm Password</label>
            <asp:TextBox ID="txtConfirm" runat="server" TextMode="Password" CssClass="ce-input" />
        </div>
    </div>

    <hr class="ce-divider" />

    <div class="ce-btn-row">
        <asp:Button ID="btnRegister" runat="server" Text="🚀 Create Account"
            CssClass="ce-btn-create" OnClick="btnRegister_Click" />
        <asp:HyperLink runat="server" NavigateUrl="~/Login.aspx" CssClass="ce-btn-questions"
            Text="Already have an account? Login" />
    </div>
</div>

</asp:Content>
