<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs"
    Inherits="OnlineExaminationSystem.ChangePasswordPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">🔐</span>
    <div>
        <h2 class="ce-page-title">Change Password</h2>
        <p class="ce-page-subtitle">Enter your current password and a new one.</p>
    </div>
</div>

<div class="ce-form-card" style="max-width:560px;">
    <asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

    <div class="ce-field">
        <label class="ce-label">Current password</label>
        <asp:TextBox ID="txtCurrent" runat="server" TextMode="Password" CssClass="ce-input" />
    </div>
    <div class="ce-field">
        <label class="ce-label">New password (min 6 chars)</label>
        <asp:TextBox ID="txtNew" runat="server" TextMode="Password" CssClass="ce-input" />
    </div>
    <div class="ce-field">
        <label class="ce-label">Confirm new password</label>
        <asp:TextBox ID="txtConfirm" runat="server" TextMode="Password" CssClass="ce-input" />
    </div>

    <hr class="ce-divider" />
    <div class="ce-btn-row">
        <asp:Button ID="btnChange" runat="server" Text="🔐 Update password"
            CssClass="ce-btn-create" OnClick="btnChange_Click" />
    </div>
</div>

</asp:Content>
