<%@ Page Title="Admin Analytics"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="AdminAnalytics.aspx.cs"
    Inherits="OnlineExaminationSystem.AdminAnalytics" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-box">
        <div class="form-box">

            <h2>System Analytics Dashboard</h2>

            <!-- SUMMARY CARDS -->
            <div style="display:flex; gap:20px; flex-wrap:wrap;">

                <div class="analytics-card">
                    <h3>Total Students</h3>
                    <asp:Label ID="lblStudents" runat="server" />
                </div>

                <div class="analytics-card">
                    <h3>Total Exams</h3>
                    <asp:Label ID="lblExams" runat="server" />
                </div>

                <div class="analytics-card">
                    <h3>Total Questions</h3>
                    <asp:Label ID="lblQuestions" runat="server" />
                </div>

                <div class="analytics-card">
                    <h3>Total Attempts</h3>
                    <asp:Label ID="lblAttempts" runat="server" />
                </div>

                <div class="analytics-card">
                    <h3>Average Score</h3>
                    <asp:Label ID="lblAverage" runat="server" />
                </div>

                <div class="analytics-card">
                    <h3>Pass %</h3>
                    <asp:Label ID="lblPassPercent" runat="server" />
                </div>

            </div>

            <br /><br />

            <h3>Top 5 Students</h3>

            <asp:GridView ID="gvTopStudents"
                runat="server"
                AutoGenerateColumns="false"
                CssClass="gridview">

                <Columns>
                    <asp:BoundField DataField="StudentId" HeaderText="Student ID" />
                    <asp:BoundField DataField="ExamId" HeaderText="Exam" />
                    <asp:BoundField DataField="Score" HeaderText="Score" />
                </Columns>

            </asp:GridView>

        </div>
    </div>

</asp:Content>