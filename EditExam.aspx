<%@ Page Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="EditExam.aspx.cs"
    Inherits="OnlineExaminationSystem.EditExam" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-box">
        <div class="form-box">

            <h2>Edit Exam</h2>

            Title:
            <asp:TextBox ID="txtTitle" runat="server" />

            Duration (Minutes):
            <asp:TextBox ID="txtDuration" runat="server" />

            Total Marks:
            <asp:TextBox ID="txtMarks" runat="server" />

            <!-- Hidden Field to Store ExamId -->
            <asp:HiddenField ID="hfExamId" runat="server" />

            <asp:Button ID="btnUpdate"
                runat="server"
                Text="Update Exam"
                CssClass="aspNetButton"
                OnClick="btnUpdate_Click" />

            <asp:Label ID="lblMsg"
                runat="server"
                CssClass="success-msg" />

        </div>
    </div>

</asp:Content>
