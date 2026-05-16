<%@ Page Title="My Results" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentResults.aspx.cs"
    Inherits="OnlineExaminationSystem.StudentResults" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">📊</span>
    <div>
        <h2 class="ce-page-title">My Results</h2>
        <p class="ce-page-subtitle">All your exam attempts in one place.</p>
    </div>
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    No results yet. Take an exam to see results here.
</asp:Panel>

<div class="ce-grid-container">
    <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false"
        CssClass="ce-grid" Width="100%" GridLines="None">
        <Columns>
            <asp:BoundField DataField="ExamName" HeaderText="Exam" />
            <asp:BoundField DataField="Subject"  HeaderText="Subject" />
            <asp:BoundField DataField="Score"    HeaderText="Score" />
            <asp:BoundField DataField="TotalMarks" HeaderText="Out of" />
            <asp:BoundField DataField="Percentage" HeaderText="%" DataFormatString="{0:F1}%" />
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <span class='<%# (bool)Eval("Passed") ? "status-badge status-pass" : "status-badge status-fail" %>'>
                        <%# (bool)Eval("Passed") ? "PASS" : "FAIL" %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="SubmittedAt" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
            <asp:TemplateField HeaderText="Action">
                <ItemTemplate>
                    <a class="action-link"
                       href='<%# "RequestRecheck.aspx?resultId=" + Eval("ResultId") %>'>Recheck</a>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="ce-grid-header" />
    </asp:GridView>
</div>

</asp:Content>
