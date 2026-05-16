<%@ Page Title="Notifications" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Notifications.aspx.cs"
    Inherits="OnlineExaminationSystem.NotificationsPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

<div class="ce-page-header">
    <span class="ce-page-icon">🔔</span>
    <div style="flex:1;">
        <h2 class="ce-page-title">Notifications</h2>
        <p class="ce-page-subtitle">Updates about your exams, results, and recheck requests.</p>
    </div>
    <asp:Button ID="btnMarkAll" runat="server" Text="✅ Mark all read"
        CssClass="ce-btn-questions" OnClick="btnMarkAll_Click" />
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    You have no notifications.
</asp:Panel>

<asp:Repeater ID="rptNotifs" runat="server" OnItemCommand="rptNotifs_ItemCommand">
    <ItemTemplate>
        <div class='<%# (bool)Eval("IsRead") ? "list-row" : "list-row" %>'
             style='<%# (bool)Eval("IsRead") ? "" : "border-color:rgba(56,189,248,0.45);" %>'>
            <div style="font-size:22px; min-width:36px; text-align:center;">
                <%# IconForType(Eval("Type").ToString()) %>
            </div>
            <div style="flex:1;">
                <strong><%# Eval("Title") %></strong>
                <div style="color:#94a3b8; font-size:13px; margin-top:3px;"><%# Eval("Message") %></div>
                <div style="color:#64748b; font-size:11px; margin-top:6px;">
                    <%# Eval("CreatedAt", "{0:dd MMM yyyy, hh:mm tt}") %>
                </div>
            </div>
            <div style="display:flex; gap:6px; align-items:center;">
                <asp:HyperLink runat="server" Visible='<%# Eval("Link") != null && Eval("Link").ToString().Length > 0 %>'
                    NavigateUrl='<%# Eval("Link") %>'
                    CssClass="action-link" Text="Open" />
                <asp:LinkButton runat="server"
                    Visible='<%# !(bool)Eval("IsRead") %>'
                    CssClass="action-link"
                    CommandName="MarkRead"
                    CommandArgument='<%# Eval("NotificationId") %>'
                    Text="Mark read" />
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

</asp:Content>
