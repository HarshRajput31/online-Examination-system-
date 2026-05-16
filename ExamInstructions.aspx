<%@ Page Title="Exam Instructions"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ExamInstructions.aspx.cs"
    Inherits="OnlineExaminationSystem.ExamInstructions" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-box">

        <div class="form-box">

            <h2>📘 Exam Instructions</h2>

            <hr />

            <!-- Exam Details -->

            <p><strong>Exam Title:</strong> 
                <asp:Label ID="lblTitle" runat="server" />
            </p>

            <p><strong>Subject:</strong> 
                <asp:Label ID="lblSubject" runat="server" />
            </p>

            <p><strong>Total Questions:</strong> 
                <asp:Label ID="lblQuestions" runat="server" />
            </p>

            <p><strong>Duration:</strong> 
                <asp:Label ID="lblDuration" runat="server" /> minutes
            </p>

            <p><strong>Total Marks:</strong> 
                <asp:Label ID="lblMarks" runat="server" />
            </p>

            <p><strong>Negative Marking:</strong> 
                0.25 marks will be deducted for each wrong answer.
            </p>

            <hr />

            <!-- Instructions -->

            <h4>⚠️ Important Instructions</h4>

            <ul>
                <li>Do not refresh the page during the exam.</li>
                <li>Do not switch browser tabs.</li>
                <li>Right-click is disabled during the exam.</li>
                <li>The exam will auto-submit when time ends.</li>
                <li>You can navigate using Next / Previous.</li>
                <li>Click <strong>Submit</strong> before leaving.</li>
            </ul>

            <br />

            <!-- START BUTTON (FINAL FIXED) -->

            <asp:Button ID="btnStartExam"
                runat="server"
                Text="🚀 Start Exam"
                CssClass="aspNetButton"
                OnClick="btnStartExam_Click"
                UseSubmitBehavior="true" />

            <br /><br />

            <!-- MESSAGE -->

            <asp:Label ID="lblMsg"
                runat="server"
                CssClass="error-msg" />

        </div>

    </div>

</asp:Content>