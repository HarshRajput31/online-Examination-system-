<%@ Page Title="Recheck Requests" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="RecheckRequests.aspx.cs"
    Inherits="OnlineExaminationSystem.RecheckRequestsPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">📨</span>
    <div>
        <h2 class="ce-page-title">Assigned Recheck Requests</h2>
        <p class="ce-page-subtitle">Update the score for each request and submit. The student is notified.</p>
    </div>
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    No recheck requests assigned to you.
</asp:Panel>
<asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

<asp:Repeater ID="rptRequests" runat="server" OnItemCommand="rptRequests_ItemCommand">
    <ItemTemplate>
        <div class="list-row" style="flex-direction:column; align-items:stretch; gap:14px;">
            <div>
                <strong><%# Eval("ExamTitle") %></strong>
                <span class="status-badge status-pending" style="margin-left:8px;"><%# Eval("Status") %></span>
                <div style="color:#94a3b8; font-size:13px; margin-top:3px;">
                    Student: <%# Eval("StudentName") %> &middot; Old Score:
                    <strong><%# Eval("OldScore") %></strong>
                </div>
                <div style="color:#cbd5e1; font-size:13px; margin-top:6px;">
                    <em>"<%# Eval("Reason") %>"</em>
                </div>
            </div>
            <div class="ce-row-2">
                <div class="ce-field">
                    <label class="ce-label">New Score</label>
                    <asp:TextBox runat="server" ID="txtNewScore" CssClass="ce-input"
                        TextMode="Number" placeholder="Updated score" />
                </div>
                <div class="ce-field">
                    <label class="ce-label">Comments</label>
                    <asp:TextBox runat="server" ID="txtComments" CssClass="ce-input"
                        placeholder="Brief justification..." />
                </div>
            </div>
            <div class="ce-btn-row">
                <asp:Button runat="server" Text="✅ Submit Recheck"
                    CssClass="ce-btn-create"
                    CommandName="Complete" CommandArgument='<%# Eval("RecheckId") %>' />
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

</asp:Content>
