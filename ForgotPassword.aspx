<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="OnlineExaminationSystem.ForgotPassword" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Reset Password | Online Examination System</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; font-family: 'Segoe UI', sans-serif; }
        body {
            height: 100vh;
            background: url('Images/login-image.jpg') no-repeat center center;
            background-size: cover;
            display: flex; justify-content: center; align-items: center;
        }
        body::before {
            content: ""; position: absolute; width: 100%; height: 100%;
            background: rgba(0, 0, 0, 0.7); backdrop-filter: blur(8px);
        }
        .reset-container {
            position: relative; width: 420px; padding: 40px; border-radius: 15px;
            background: rgba(255, 255, 255, 0.1); backdrop-filter: blur(20px);
            box-shadow: 0 10px 30px rgba(0,0,0,0.5); color: white; text-align: center;
        }
        h2 { margin-bottom: 10px; color: #00c6ff; }
        .instruction { font-size: 13px; color: #ccc; margin-bottom: 25px; }
        .input-group { text-align: left; margin-bottom: 15px; }
        .input-group label { font-size: 13px; display: block; margin-bottom: 5px; }
        .input-box {
            width: 100%; padding: 12px; border-radius: 8px; border: none;
            background: rgba(255,255,255,0.9); outline: none; transition: 0.3s;
        }
        .reset-btn {
            width: 100%; padding: 12px; border-radius: 8px; border: none;
            background: linear-gradient(45deg, #00c6ff, #0072ff);
            color: white; font-weight: bold; cursor: pointer; margin-top: 10px;
        }
        .back-link { display: block; margin-top: 20px; font-size: 13px; color: #00c6ff; text-decoration: none; }
        .msg-label { display: block; margin-top: 15px; font-size: 14px; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="reset-container">
            <h2>🔒 Reset Password</h2>
            <p class="instruction">Verify your identity to set a new password.</p>

            <asp:Label ID="lblMsg" runat="server" CssClass="msg-label" Visible="false"></asp:Label>

            <div class="input-group">
                <label>Registered Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="input-box" placeholder="example@email.com"></asp:TextBox>
            </div>

            <div class="input-group">
                <label>Security Question</label>
                <asp:DropDownList ID="ddlQuestion" runat="server" CssClass="input-box">
                    <asp:ListItem Text="What is your pet's name?" Value="Pet"></asp:ListItem>
                    <asp:ListItem Text="Your favorite teacher's name?" Value="Teacher"></asp:ListItem>
                    <asp:ListItem Text="What was your first school?" Value="School"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="input-group">
                <label>Security Answer</label>
                <asp:TextBox ID="txtAnswer" runat="server" CssClass="input-box" placeholder="Your Answer"></asp:TextBox>
            </div>

            <div class="input-group">
                <label>New Password</label>
                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="input-box" TextMode="Password" placeholder="Minimum 6 characters"></asp:TextBox>
            </div>

            <asp:Button ID="btnReset" runat="server" Text="Update Password" OnClick="btnReset_Click" CssClass="reset-btn" />

            <a href="Login.aspx" class="back-link">← Back to Login</a>
        </div>
    </form>
</body>
</html>