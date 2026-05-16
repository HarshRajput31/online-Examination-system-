<%@ Page Title="Manage Rechecks" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ManageRechecks.aspx.cs"
    Inherits="OnlineExaminationSystem.ManageRechecks" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">🛠️</span>
    <div>
        <h2 class="ce-page-title">Manage Recheck Requests</h2>
        <p class="ce-page-subtitle">Review pending recheck requests and assign them to a faculty member.</p>
    </div>
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    No recheck requests right now.
</asp:Panel>
<asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

<asp:Repeater ID="rptRequests" runat="server" OnItemCommand="rptRequests_ItemCommand">
    <ItemTemplate>
        <div class="list-row" style="flex-direction:column; align-items:stretch; gap:12px;">
            <div style="display:flex; gap:14px; align-items:flex-start;">
                <div style="font-size:24px;">🔄</div>
                <div style="flex:1;">
                    <strong><%# Eval("ExamTitle") %></strong>
                    <span class="status-badge status-pending" style="margin-left:8px;"><%# Eval("Status") %></span>
                    <div style="color:#94a3b8; font-size:13px; margin-top:3px;">
                        Student <%# Eval("StudentName") %> &middot; Score <%# Eval("OldScore") %>
                    </div>
                    <div style="color:#cbd5e1; font-size:13px; margin-top:6px;">
                        <em>"<%# Eval("Reason") %>"</em>
                    </div>
                    <div style="color:#64748b; font-size:11px; margin-top:6px;">
                        Submitted <%# Eval("RequestedAt", "{0:dd MMM yyyy hh:mm tt}") %>
                    </div>
                </div>
            </div>
            <div style="display:flex; gap:8px; align-items:center; flex-wrap:wrap;">
                <asp:DropDownList runat="server" ID="ddlFaculty"
                    CssClass="ce-input" Width="280px"
                    DataSource='<%# FacultyOptions %>'
                    DataTextField="Text" DataValueField="Value" />
                <asp:Button runat="server" Text="Assign" CssClass="ce-btn-create"
                    CommandName="Assign"
                    CommandArgument='<%# Eval("RecheckId") + "|" + Container.ItemIndex %>' />
                <asp:Button runat="server" Text="Reject" CssClass="btn btn-danger"
                    CommandName="Reject" CommandArgument='<%# Eval("RecheckId") %>'
                    OnClientClick="return confirm('Reject this recheck request?');" />
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

</asp:Content>
