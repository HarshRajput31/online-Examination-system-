<%@ Page Title="Manage Students" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StudentList.aspx.cs"
    Inherits="OnlineExaminationSystem.Student.StudentList" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">👨‍🎓</span>
    <div style="flex:1;">
        <h2 class="ce-page-title">Manage Students</h2>
        <p class="ce-page-subtitle">All registered students.</p>
    </div>
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    No students yet.
</asp:Panel>

<div class="ce-grid-container">
    <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns="false"
        CssClass="ce-grid" Width="100%" GridLines="None"
        OnRowCommand="gvStudents_RowCommand" DataKeyNames="UserId">
        <Columns>
            <asp:BoundField DataField="UserId"     HeaderText="ID" />
            <asp:BoundField DataField="Name"       HeaderText="Name" />
            <asp:BoundField DataField="Email"      HeaderText="Email" />
            <asp:BoundField DataField="Department" HeaderText="Department" />
            <asp:BoundField DataField="Course"     HeaderText="Course" />
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <span class='<%# (bool)Eval("IsBlocked") ? "status-badge status-fail" : "status-badge status-pass" %>'>
                        <%# (bool)Eval("IsBlocked") ? "BLOCKED" : "ACTIVE" %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:LinkButton runat="server" CssClass="action-link"
                        CommandName="ToggleBlock" CommandArgument='<%# Eval("UserId") %>'
                        Text='<%# (bool)Eval("IsBlocked") ? "Unblock" : "Block" %>' />
                    <asp:LinkButton runat="server" CssClass="action-link delete"
                        CommandName="DeleteUser" CommandArgument='<%# Eval("UserId") %>'
                        Text="Delete"
                        OnClientClick="return confirm('Delete this student?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="ce-grid-header" />
    </asp:GridView>
</div>

<asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

</asp:Content>
