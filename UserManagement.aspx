<%@ Page Title="User Management" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs"
    Inherits="OnlineExaminationSystem.UserManagement" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">👥</span>
    <div>
        <h2 class="ce-page-title">User Management</h2>
        <p class="ce-page-subtitle">Search, filter, block, or remove user accounts.</p>
    </div>
</div>

<div class="ce-form-card" style="margin-bottom:18px;">
    <div class="ce-row-2">
        <div class="ce-field">
            <label class="ce-label">Search</label>
            <asp:TextBox ID="txtSearch" runat="server" CssClass="ce-input" placeholder="Name or email..." />
        </div>
        <div class="ce-field">
            <label class="ce-label">Role</label>
            <asp:DropDownList ID="ddlRole" runat="server" CssClass="ce-input">
                <asp:ListItem Value=""  Text="All roles" />
                <asp:ListItem Value="1" Text="Admin" />
                <asp:ListItem Value="2" Text="Student" />
                <asp:ListItem Value="3" Text="Faculty" />
            </asp:DropDownList>
        </div>
    </div>
    <div class="ce-btn-row" style="margin-top:14px;">
        <asp:Button ID="btnFilter" runat="server" Text="🔍 Apply" CssClass="ce-btn-create" OnClick="btnFilter_Click" />
    </div>
</div>

<div class="ce-grid-container">
    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false"
        CssClass="ce-grid" Width="100%" GridLines="None"
        OnRowCommand="gvUsers_RowCommand">
        <Columns>
            <asp:BoundField DataField="UserId" HeaderText="ID" />
            <asp:BoundField DataField="Name"   HeaderText="Name" />
            <asp:BoundField DataField="Email"  HeaderText="Email" />
            <asp:BoundField DataField="Role"   HeaderText="Role" />
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <span class='<%# (bool)Eval("Blocked") ? "status-badge status-fail" : "status-badge status-pass" %>'>
                        <%# (bool)Eval("Blocked") ? "BLOCKED" : "ACTIVE" %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:LinkButton runat="server" CssClass="action-link"
                        CommandName="Toggle" CommandArgument='<%# Eval("UserId") %>'
                        Text='<%# (bool)Eval("Blocked") ? "Unblock" : "Block" %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="ce-grid-header" />
    </asp:GridView>
</div>

<asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

</asp:Content>
