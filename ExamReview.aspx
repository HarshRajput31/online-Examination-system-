<%@ Page Title="Exam Review"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ExamReview.aspx.cs"
    Inherits="OnlineExaminationSystem.ExamReview" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="container-box">
    <div class="form-box">

        <h2>📝 Exam Review</h2>

        <asp:GridView ID="gvReview"
            runat="server"
            AutoGenerateColumns="False"
            Width="100%"
            CssClass="table"
            HeaderStyle-BackColor="#0f172a"
            HeaderStyle-ForeColor="White"
            RowStyle-BackColor="#1e293b"
            RowStyle-ForeColor="White"
            AlternatingRowStyle-BackColor="#334155">

            <Columns>

                <asp:BoundField DataField="QuestionText" HeaderText="Question" />
                <asp:BoundField DataField="StudentAnswer" HeaderText="Your Answer" />
                <asp:BoundField DataField="CorrectAnswer" HeaderText="Correct Answer" />
                <asp:BoundField DataField="Result" HeaderText="Result" />
                <asp:BoundField DataField="Marks" HeaderText="Marks" />
                <asp:BoundField DataField="NegativeMarks" HeaderText="Negative" />
                <asp:BoundField DataField="MarksAwarded" HeaderText="Awarded" />

            </Columns>

        </asp:GridView>

        <br />

        <asp:Label ID="lblMsg"
            runat="server"
            CssClass="error-msg" />

    </div>
</div>

</asp:Content>