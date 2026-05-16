<%@ Page Title="Add Faculty"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="AddFaculty.aspx.cs"
    Inherits="OnlineExaminationSystem.AddFaculty" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

<div class="container-fluid mt-4">

    <!-- PAGE TITLE -->
    <h2 class="page-title">
        👤 Add Faculty
    </h2>

    <div class="form-wrapper">

        <div class="form-card">

            <!-- NAME -->
            <div class="form-group">
                <label>Faculty Name</label>

                <asp:TextBox
                    ID="txtName"
                    runat="server"
                    CssClass="form-control">
                </asp:TextBox>
            </div>

            <!-- EMAIL -->
            <div class="form-group">
                <label>Email</label>

                <asp:TextBox
                    ID="txtEmail"
                    runat="server"
                    TextMode="Email"
                    CssClass="form-control">
                </asp:TextBox>
            </div>

            <!-- DEPARTMENT -->
            <div class="form-group">
                <label>Department</label>

                <asp:TextBox
                    ID="txtDepartment"
                    runat="server"
                    CssClass="form-control">
                </asp:TextBox>
            </div>

            <!-- MOBILE -->
            <div class="form-group">
                <label>Mobile Number</label>

                <asp:TextBox
                    ID="txtMobile"
                    runat="server"
                    CssClass="form-control">
                </asp:TextBox>
            </div>

            <!-- COURSE -->
            <div class="form-group">
                <label>Course Teaching</label>

                <asp:TextBox
                    ID="txtCourse"
                    runat="server"
                    CssClass="form-control">
                </asp:TextBox>
            </div>

            <br />

            <!-- BUTTON -->
            <asp:Button
                ID="btnAddFaculty"
                runat="server"
                Text="➕ Add Faculty"
                CssClass="btn btn-primary"
                OnClick="btnAddFaculty_Click" />

            <br /><br />

            <!-- MESSAGE -->
            <asp:Label
                ID="lblMsg"
                runat="server">
            </asp:Label>

        </div>

    </div>

</div>

</asp:Content>