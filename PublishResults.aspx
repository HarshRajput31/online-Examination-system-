<%@ Page Title="Publish Results" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="PublishResults.aspx.cs"
    Inherits="OnlineExaminationSystem.PublishResults" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">📢</span>
    <div>
        <h2 class="ce-page-title">Publish Results</h2>
        <p class="ce-page-subtitle">Pick an exam and publish results to all students.</p>
    </div>
</div>

<asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

<div class="ce-form-card" style="margin-bottom:20px;">
    <div class="ce-field">
        <label class="ce-label">Exam</label>
        <asp:DropDownList ID="ddlExam" runat="server" CssClass="ce-input" AutoPostBack="true"
            OnSelectedIndexChanged="ddlExam_Changed" />
    </div>
</div>

<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    No results to publish for this exam.
</asp:Panel>

<asp:Panel ID="pnlList" runat="server">
    <div class="ce-grid-container">
        <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false"
            CssClass="ce-grid" Width="100%" GridLines="None">
            <Columns>
                <asp:BoundField DataField="StudentName" HeaderText="Student" />
                <asp:BoundField DataField="Score"      HeaderText="Score" />
                <asp:BoundField DataField="TotalMarks" HeaderText="Out of" />
                <asp:BoundField DataField="Percentage" HeaderText="%" DataFormatString="{0:F1}%" />
                <asp:BoundField DataField="Status"     HeaderText="Status" />
            </Columns>
            <HeaderStyle CssClass="ce-grid-header" />
        </asp:GridView>
    </div>

    <div class="ce-btn-row" style="margin-top:18px;">
        <asp:Button ID="btnPublish" runat="server" Text="📢 Publish to all students"
            CssClass="ce-btn-create" OnClick="btnPublish_Click" />
    </div>
</asp:Panel>

</asp:Content>
