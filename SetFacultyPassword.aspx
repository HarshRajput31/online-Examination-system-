<%@ Page Title="Set Password" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="SetFacultyPassword.aspx.cs"
    Inherits="OnlineExaminationSystem.SetFacultyPassword" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="setpassword-page">
  <div class="setpassword-card">

    <!-- HEADER -->
    <div class="sp-header">
        <span class="sp-icon">🔐</span>
        <h2 class="sp-title">Set Your Password</h2>
        <p class="sp-subtitle">Create your login password to activate your account.</p>
    </div>

    <!-- EMAIL -->
    <div class="sp-field">
        <label class="sp-label">📧 Your Login Email</label>
        <asp:TextBox ID="txtLoginEmail" runat="server"
            CssClass="sp-input" ReadOnly="true" />
    </div>

    <!-- PASSWORD -->
    <div class="sp-field">
        <label class="sp-label">🔑 New Password</label>
        <div class="sp-input-wrap">
            <asp:TextBox ID="txtPassword" runat="server"
                TextMode="Password"
                CssClass="sp-input"
                placeholder="Min. 6 characters"
                autocomplete="new-password" />
            <span class="eye-btn"
                onclick="togglePass('<%= txtPassword.ClientID %>', this)">👁</span>
        </div>
    </div>

    <!-- CONFIRM PASSWORD -->
    <div class="sp-field">
        <label class="sp-label">🔒 Confirm Password</label>
        <div class="sp-input-wrap">
            <asp:TextBox ID="txtConfirmPassword" runat="server"
                TextMode="Password"
                CssClass="sp-input"
                placeholder="Re-enter password"
                autocomplete="new-password" />
            <span class="eye-btn"
                onclick="togglePass('<%= txtConfirmPassword.ClientID %>', this)">👁</span>
        </div>
    </div>

    <!-- PASSWORD STRENGTH -->
    <div class="sp-field">
        <label class="sp-label">💪 Password Strength</label>
        <div class="strength-bar-bg">
            <div class="strength-bar" id="strengthBar"></div>
        </div>
        <span class="strength-label" id="strengthLabel">Type a password...</span>
    </div>

    <hr class="sp-divider" />

    <!-- TOKEN -->
    <asp:HiddenField ID="hfToken" runat="server" />

    <!-- BUTTON -->
    <asp:Button ID="btnSetPassword" runat="server"
        Text="✅ Set Password & Activate Account"
        CssClass="sp-btn"
        OnClick="btnSetPassword_Click"
        OnClientClick="return validateForm();" />

    <!-- MESSAGE -->
    <asp:Label ID="lblMsg" runat="server"
        CssClass="sp-msg"
        Visible="false" />

  </div>
</div>

<!-- SCRIPT -->
<script>
    function togglePass(id, btn) {
        var inp = document.getElementById(id);
        if (!inp) return;

        inp.type = inp.type === 'password' ? 'text' : 'password';
        btn.textContent = inp.type === 'password' ? '👁' : '🙈';
    }

    function validateForm() {
        var pass = document.getElementById('<%= txtPassword.ClientID %>').value;
        var confirm = document.getElementById('<%= txtConfirmPassword.ClientID %>').value;

        if (pass.length < 6) {
            alert("Password must be at least 6 characters.");
            return false;
        }

        if (pass !== confirm) {
            alert("Passwords do not match.");
            return false;
        }

        return true;
    }

    document.addEventListener('DOMContentLoaded', function () {
        var inp = document.getElementById('<%= txtPassword.ClientID %>');
        var bar = document.getElementById('strengthBar');
        var label = document.getElementById('strengthLabel');

        if (!inp) return;

        inp.addEventListener('input', function () {
            var v = this.value;
            var score = 0;

            if (v.length >= 6) score++;
            if (v.length >= 10) score++;
            if (/[A-Z]/.test(v)) score++;
            if (/[0-9]/.test(v)) score++;
            if (/[^A-Za-z0-9]/.test(v)) score++;

            var colors = ['#ef4444', '#f97316', '#facc15', '#22c55e', '#16a34a'];
            var labels = ['Very Weak', 'Weak', 'Fair', 'Strong', 'Very Strong'];
            var widths = ['20%', '40%', '60%', '80%', '100%'];

            if (!v.length) {
                bar.style.width = '0%';
                bar.style.background = '#ccc';
                label.textContent = 'Type a password...';
                label.style.color = '#64748b';
            } else {
                var i = Math.max(0, Math.min(score - 1, 4));
                bar.style.width = widths[i];
                bar.style.background = colors[i];
                label.textContent = labels[i];
                label.style.color = colors[i];
            }
        });
    });
</script>

</asp:Content>