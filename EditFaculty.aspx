﻿﻿<%@ Page Title="Edit Faculty"
Language="C#"
MasterPageFile="~/Site.Master"
AutoEventWireup="true"
CodeBehind="EditFaculty.aspx.cs"
Inherits="OnlineExaminationSystem.EditFaculty" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="form-wrapper">
    <h2 class="page-title">✏️ Edit Faculty</h2>
    <div class="form-card">
        <!-- NAME -->
        <div class="form-group">
            <label>Faculty Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control-custom" />
        </div>
        <!-- PERSONAL EMAIL -->
        <div class="form-group">
            <label>Personal Email</label>
            <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control-custom" />
        </div>
        <!-- LOGIN EMAIL -->
        <div class="form-group">
            <label>Generated Login Email</label>
            <asp:TextBox ID="txtLoginEmail" runat="server" CssClass="form-control-custom" ReadOnly="true" />
            <small style="color:#94a3b8;">Faculty uses this email to log in. Admin cannot edit passwords.</small>
        </div>
        <!-- DEPARTMENT -->
        <div class="form-group">
            <label>Department</label>
            <asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control-custom" />
        </div>
        <!-- MOBILE -->
        <div class="form-group">
            <label>Mobile Number</label>
            <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control-custom" MaxLength="10" />
        </div>
        <!-- COURSE -->
        <div class="form-group">
            <label>Course Teaching</label>
            <asp:TextBox ID="txtCourse" runat="server" CssClass="form-control-custom" />
        </div>
        <!-- UPDATE BUTTON -->
        <asp:Button 
            ID="btnUpdate" 
            runat="server" 
            Text="Update Faculty" 
            CssClass="btn-modern"
            OnClick="btnUpdate_Click" />
        <br /><br />
        <asp:Label ID="lblMsg" runat="server" CssClass="success-label" />
    </div>
</div>
</asp:Content>