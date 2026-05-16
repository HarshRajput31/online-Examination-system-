<%@ Page Title="My Exams" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="MyExams.aspx.cs"
    Inherits="OnlineExaminationSystem.MyExams" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">📚</span>
    <div style="flex:1;">
        <h2 class="ce-page-title">My Exams</h2>
        <p class="ce-page-subtitle">Exams you have created. Drill in to add questions or edit details.</p>
    </div>
    <asp:HyperLink runat="server" NavigateUrl="~/CreateExam.aspx" CssClass="ce-btn-create"
        Text='<i class="fa-solid fa-plus"></i> &nbsp; Create exam' />
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    You haven't created any exams yet.
</asp:Panel>

<div class="ce-grid-container">
    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false"
        CssClass="ce-grid" Width="100%" GridLines="None"
        OnRowCommand="gv_RowCommand">
        <Columns>
            <asp:BoundField DataField="examId"  HeaderText="ID" />
            <asp:BoundField DataField="title"   HeaderText="Title" />
            <asp:BoundField DataField="subject" HeaderText="Subject" />
            <asp:BoundField DataField="setNumber" HeaderText="Set" />
            <asp:BoundField DataField="status"  HeaderText="Status" />
            <asp:TemplateField HeaderText="Questions">
                <ItemTemplate><%# Eval("questionCount") %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <a class="action-link edit" href='<%# "EditExam.aspx?examId=" + Eval("examId") %>'>Edit</a>
                    <a class="action-link" href='<%# "AddQuestion.aspx?examId=" + Eval("examId") %>'>Add Q</a>
                    <a class="action-link" href='<%# "ExamPreview.aspx?examId=" + Eval("examId") %>'>Preview</a>
                    <asp:LinkButton runat="server" CssClass="action-link delete"
                        CommandName="DeleteExam" CommandArgument='<%# Eval("examId") %>'
                        OnClientClick="return confirm('Delete this exam?');"
                        Text="Delete" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="ce-grid-header" />
    </asp:GridView>
</div>

<asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

</asp:Content>
