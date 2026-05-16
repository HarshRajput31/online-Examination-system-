<%@ Page Title="Set Student Password" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="SetStudentPassword.aspx.cs"
    Inherits="OnlineExaminationSystem.SetStudentPassword" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="setpassword-page">
  <div class="setpassword-card">
    <div class="sp-header">
        <span class="sp-icon">🔐</span>
        <h2 class="sp-title">Set Your Student Password</h2>
        <p class="sp-subtitle">Create your password to activate your student account.</p>
    </div>

    <div class="sp-field">
        <label class="sp-label">📧 Your Login Email</label>
        <asp:TextBox ID="txtLoginEmail" runat="server"
            CssClass="sp-input" ReadOnly="true" />
    </div>

    <div class="sp-field">
        <label class="sp-label">🔑 New Password</label>
        <div class="sp-input-wrap">
            <asp:TextBox ID="txtPassword" runat="server"
                TextMode="Password"
                CssClass="sp-input"
                placeholder="Min. 6 characters" />
            <span class="eye-btn"
                onclick="togglePass('<%= txtPassword.ClientID %>', this)">👁</span>
        </div>
    </div>

    <div class="sp-field">
        <label class="sp-label">🔒 Confirm Password</label>
        <div class="sp-input-wrap">
            <asp:TextBox ID="txtConfirmPassword" runat="server"
                TextMode="Password"
                CssClass="sp-input"
                placeholder="Re-enter password" />
            <span class="eye-btn"
                onclick="togglePass('<%= txtConfirmPassword.ClientID %>', this)">👁</span>
        </div>
    </div>

    <asp:HiddenField ID="hfToken" runat="server" />

    <asp:Button ID="btnSetPassword" runat="server"
        Text="✅ Set Password & Activate Account"
        CssClass="sp-btn"
        OnClick="btnSetPassword_Click" />

    <asp:Label ID="lblMsg" runat="server"
        CssClass="sp-msg"
        Visible="false" />
  </div>
</div>

<script>
    function togglePass(id, btn) {
        var inp = document.getElementById(id);
        if (!inp) return;
        inp.type = inp.type === 'password' ? 'text' : 'password';
        btn.textContent = inp.type === 'password' ? '👁' : '🙈';
    }
</script>

</asp:Content>
