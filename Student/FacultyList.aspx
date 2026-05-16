<%@ Page Title="Manage Faculty" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="FacultyList.aspx.cs"
    Inherits="OnlineExaminationSystem.Student.FacultyList" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">👨‍🏫</span>
    <div style="flex:1;">
        <h2 class="ce-page-title">Manage Faculty</h2>
        <p class="ce-page-subtitle">All faculty accounts. Use Add Faculty to invite a new member.</p>
    </div>
    <asp:HyperLink runat="server" NavigateUrl="~/AddFaculty.aspx" CssClass="ce-btn-create"
        Text='<i class="fa-solid fa-plus"></i> &nbsp; Add Faculty' />
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    No faculty members yet.
</asp:Panel>

<div class="ce-grid-container">
    <asp:GridView ID="gvFaculty" runat="server" AutoGenerateColumns="false"
        CssClass="ce-grid" Width="100%" GridLines="None" OnRowCommand="gvFaculty_RowCommand"
        DataKeyNames="FacultyId">
        <Columns>
            <asp:BoundField DataField="FacultyId"  HeaderText="ID" />
            <asp:BoundField DataField="Name"       HeaderText="Name" />
            <asp:BoundField DataField="LoginEmail" HeaderText="Login Email" />
            <asp:BoundField DataField="Department" HeaderText="Department" />
            <asp:BoundField DataField="Mobile"     HeaderText="Mobile" />
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <a class="action-link edit"
                       href='<%# "../EditFaculty.aspx?id=" + Eval("FacultyId") %>'>Edit</a>
                    <asp:LinkButton runat="server" CssClass="action-link delete"
                        CommandName="DeleteFac" CommandArgument='<%# Eval("FacultyId") %>'
                        Text="Delete"
                        OnClientClick="return confirm('Remove this faculty?');" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="ce-grid-header" />
    </asp:GridView>
</div>

<asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

</asp:Content>
