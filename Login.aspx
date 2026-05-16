<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="OnlineExaminationSystem.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Login | Online Examination System</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; font-family: 'Segoe UI', sans-serif; }
        body {
            height: 100vh;
            background: url('Images/login-image.jpg') no-repeat center center;
            background-size: cover;
            display: flex;
            justify-content: center;
            align-items: center;
            position: relative;
        }
        body::before {
            content: "";
            position: absolute;
            width: 100%; height: 100%;
            background: rgba(0, 0, 0, 0.65);
            backdrop-filter: blur(6px);
        }
        .login-container {
            position: relative;
            width: 400px;
            padding: 40px;
            border-radius: 15px;
            background: rgba(255, 255, 255, 0.1);
            backdrop-filter: blur(15px);
            box-shadow: 0 10px 30px rgba(0,0,0,0.5);
            color: white;
            text-align: center;
            animation: fadeIn 1s ease-in-out;
        }
        .login-container h2 { margin-bottom: 10px; }
        .subtitle { margin-bottom: 10px; font-size: 14px; color: #ddd; }
        .login-register-note { font-size: 13px; margin-bottom: 20px; color: #ccc; }
        .login-register-btn { color: #00c6ff; text-decoration: none; font-weight: bold; margin-left: 5px; transition: 0.3s; }
        .login-register-btn:hover { color: #fff; text-decoration: underline; }
        .input-group { text-align: left; margin-bottom: 20px; }
        .input-group label { font-size: 14px; }
        .input-box { width: 100%; padding: 10px; border-radius: 8px; border: none; margin-top: 5px; outline: none; transition: 0.3s; }
        .input-box:focus { box-shadow: 0 0 10px #00c6ff; }
        .login-btn { width: 100%; padding: 10px; border-radius: 8px; border: none; background: linear-gradient(45deg, #00c6ff, #0072ff); color: white; font-weight: bold; cursor: pointer; transition: 0.3s; margin-bottom: 15px; }
        .login-btn:hover { transform: scale(1.05); background: linear-gradient(45deg, #0072ff, #00c6ff); }
        
        /* 🔥 FORGOT PASSWORD STYLING */
        .forgot-password-link { display: block; font-size: 13px; color: #aaa; text-decoration: none; transition: 0.3s; }
        .forgot-password-link:hover { color: #00c6ff; }

        .error-msg { color: #ff4d4d; margin-bottom: 15px; display: block; }
        @keyframes fadeIn { from { opacity: 0; transform: translateY(-20px); } to { opacity: 1; transform: translateY(0); } }
    </style>
</head>
<body>
<form id="form1" runat="server">
    <div class="login-container">
        <h2>Welcome Back 👋</h2>
        <p class="subtitle">Online Examination System</p>

        <p class="login-register-note">
            🎓 New Student?
            <a href="StudentRegistration.aspx" class="login-register-btn">Register First →</a>
        </p>

        <asp:Label ID="lblMsg" runat="server" CssClass="error-msg" />

        <div class="input-group">
            <label>Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="input-box" placeholder="Enter your email" />
        </div>

        <div class="input-group">
            <label>Password</label>
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="input-box" placeholder="Enter your password" />
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="login-btn" OnClick="btnLogin_Click" />

        <!-- ✅ FORGOT PASSWORD BUTTON -->
        <asp:HyperLink ID="lnkForgot" runat="server" NavigateUrl="~/ForgotPassword.aspx" CssClass="forgot-password-link">
            Forgot Password? Click here to reset 🔑
        </asp:HyperLink>
    </div>
</form>
</body>
</html>