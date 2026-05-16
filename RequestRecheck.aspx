<%@ Page Title="Request Recheck" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="RequestRecheck.aspx.cs"
    Inherits="OnlineExaminationSystem.RequestRecheck" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">🔄</span>
    <div>
        <h2 class="ce-page-title">Request a Recheck</h2>
        <p class="ce-page-subtitle">Ask the admin to assign a faculty member to review your paper.</p>
    </div>
</div>

<div class="ce-form-card" style="max-width:760px;">
    <asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />
    <asp:Panel ID="pnlForm" runat="server">

        <div class="ce-row-2">
            <div class="ce-field">
                <label class="ce-label">Exam</label>
                <asp:DropDownList ID="ddlResult" runat="server" CssClass="ce-input" />
            </div>
            <div class="ce-field">
                <label class="ce-label">Current Score</label>
                <asp:TextBox ID="txtScore" runat="server" CssClass="ce-input" ReadOnly="true" />
            </div>
        </div>

        <div class="ce-field">
            <label class="ce-label">Reason for recheck</label>
            <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine" Rows="5"
                CssClass="ce-input" placeholder="Explain why you believe the result should be re-evaluated..." />
        </div>

        <hr class="ce-divider" />

        <div class="ce-btn-row">
            <asp:Button ID="btnSubmit" runat="server" Text="📨 Submit Request"
                CssClass="ce-btn-create" OnClick="btnSubmit_Click" />
            <asp:HyperLink runat="server" NavigateUrl="~/StudentResults.aspx"
                CssClass="ce-btn-questions" Text="← Back to results" />
        </div>

    </asp:Panel>
</div>

</asp:Content>
