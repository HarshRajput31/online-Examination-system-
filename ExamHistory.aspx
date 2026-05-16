<%@ Page Language="C#" MasterPageFile="~/Site.Master"
AutoEventWireup="true"
CodeBehind="ExamHistory.aspx.cs"
Inherits="OnlineExaminationSystem.ExamHistory" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="exam-history-page">

    <!-- PAGE TITLE -->
    <h2 class="exam-title">
        📊 Exam History
    </h2>

    <!-- GLASS CARD -->
    <div class="exam-card">
<asp:GridView ID="gvHistory"
    runat="server"
    AutoGenerateColumns="false"
    CssClass="exam-table"
    Width="100%"
    GridLines="None"
    OnRowDataBound="gvHistory_RowDataBound">

    <Columns>

        <asp:BoundField DataField="ResultId" HeaderText="Result ID" />

        <asp:BoundField DataField="StudentName" HeaderText="Student Name" />

        <asp:BoundField DataField="ExamName" HeaderText="Exam Name" />

        <asp:BoundField DataField="Score" HeaderText="Score" />

        <asp:BoundField DataField="TotalQuestions" HeaderText="Total Questions" />

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <asp:Label ID="lblStatus" runat="server"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField 
            DataField="SubmittedAt"
            HeaderText="Submitted At"
            DataFormatString="{0:dd-MM-yyyy HH:mm}" />

    </Columns>

</asp:GridView>

    </div>

    <br />

    <!-- MESSAGE -->
    <asp:Label ID="lblMsg"
        runat="server"
        CssClass="history-message" />

</div>

</asp:Content>