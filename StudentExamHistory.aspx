<%@ Page Title="Exam History" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentExamHistory.aspx.cs"
    Inherits="OnlineExaminationSystem.StudentExamHistory" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">🕒</span>
    <div>
        <h2 class="ce-page-title">Exam History</h2>
        <p class="ce-page-subtitle">A timeline of every exam you've attempted.</p>
    </div>
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    No history yet.
</asp:Panel>

<asp:Repeater ID="rptHistory" runat="server">
    <ItemTemplate>
        <div class="list-row">
            <div style="flex:1;">
                <strong><%# Eval("ExamName") %></strong>
                <div style="color:#94a3b8; font-size:12px;">
                    <%# Eval("Subject") %> &middot; <%# Eval("SubmittedAt", "{0:dd MMM yyyy, hh:mm tt}") %>
                </div>
            </div>
            <div style="text-align:right; min-width:160px;">
                <div style="font-weight:700;"><%# Eval("Score") %> / <%# Eval("TotalMarks") %></div>
                <div style="color:#94a3b8; font-size:12px;"><%# Eval("Percentage", "{0:F1}%") %></div>
                <span class='<%# (bool)Eval("Passed") ? "status-badge status-pass" : "status-badge status-fail" %>'>
                    <%# (bool)Eval("Passed") ? "PASS" : "FAIL" %>
                </span>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

</asp:Content>
