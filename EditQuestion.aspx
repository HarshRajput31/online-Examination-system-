<%@ Page Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="EditQuestion.aspx.cs"
    Inherits="OnlineExaminationSystem.EditQuestion" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-box">
        <div class="form-box">

            <h2>Edit Question</h2>

            <asp:HiddenField ID="hfQuestionId" runat="server" />

            Question:
            <asp:TextBox ID="txtQuestion"
                runat="server"
                TextMode="MultiLine" />

            Option A:
            <asp:TextBox ID="txtA" runat="server" />

            Option B:
            <asp:TextBox ID="txtB" runat="server" />

            Option C:
            <asp:TextBox ID="txtC" runat="server" />

            Option D:
            <asp:TextBox ID="txtD" runat="server" />

            Correct Answer (A/B/C/D):
            <asp:TextBox ID="txtCorrect" runat="server" />

            Difficulty:
            <asp:DropDownList ID="ddlDifficulty" runat="server">
                <asp:ListItem>Easy</asp:ListItem>
                <asp:ListItem>Medium</asp:ListItem>
                <asp:ListItem>Hard</asp:ListItem>
            </asp:DropDownList>

            Marks:
            <asp:TextBox ID="txtMarks" runat="server" />

            Negative Marks:
            <asp:TextBox ID="txtNegative" runat="server" />

            <br /><br />

            <asp:Button ID="btnUpdate"
                runat="server"
                Text="Update Question"
                CssClass="aspNetButton"
                OnClick="btnUpdate_Click" />

            <br /><br />

            <asp:Label ID="lblMsg"
                runat="server" />

        </div>
    </div>

</asp:Content>