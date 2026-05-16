<%@ Page Title="Create Exam" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CreateExam.aspx.cs" Inherits="OnlineExaminationSystem.CreateExam" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="create-exam-page">

    <div class="ce-page-header">
        <span class="ce-page-icon">📝</span>
        <div>
            <h2 class="ce-page-title">Manage & Create Exams</h2>
            <p class="ce-page-subtitle">Fill details, schedule timing, and publish for Admin review.</p>
        </div>
    </div>

    <div class="ce-form-card">
        <div class="ce-card-glow"></div>
        <asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

        <%-- FORM FIELDS --%>
        <div class="ce-field">
            <label class="ce-label">📋 Exam Title</label>
            <asp:TextBox ID="txtTitle" runat="server" CssClass="ce-input" placeholder="e.g. Java Programming Final" />
        </div>

        <div class="ce-field">
            <label class="ce-label">📚 Subject</label>
            <asp:TextBox ID="txtSubject" runat="server" CssClass="ce-input" placeholder="e.g. Computer Science" />
        </div>

        <div class="ce-row-2">
            <div class="ce-field">
                <label class="ce-label">⏱️ Duration (Min)</label>
                <asp:TextBox ID="txtDuration" runat="server" CssClass="ce-input" TextMode="Number" />
            </div>
            <div class="ce-field">
                <label class="ce-label">🏆 Total Marks</label>
                <asp:TextBox ID="txtMarks" runat="server" CssClass="ce-input" TextMode="Number" />
            </div>
        </div>

        <div class="ce-row-2">
            <div class="ce-field">
                <label class="ce-label">📅 Start Date & Time</label>
                <asp:TextBox ID="txtStartDate" runat="server" CssClass="ce-input" TextMode="DateTimeLocal" />
            </div>
            <div class="ce-field">
                <label class="ce-label">⏰ Due Date & Time</label>
                <asp:TextBox ID="txtDueDate" runat="server" CssClass="ce-input" TextMode="DateTimeLocal" />
            </div>
        </div>

        <div class="ce-field">
            <label class="ce-label">🗂️ Select Set</label>
            <div class="ce-set-selector">
                <asp:RadioButtonList ID="rblSet" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="ce-set-rbl">
                    <asp:ListItem Value="Set 1" Text="Set 1" Selected="True" />
                    <asp:ListItem Value="Set 2" Text="Set 2" />
                    <asp:ListItem Value="Set 3" Text="Set 3" />
                    <asp:ListItem Value="Set 4" Text="Set 4" />
                </asp:RadioButtonList>
            </div>
        </div>

        <hr class="ce-divider" />

        <div class="ce-btn-row">
            <asp:Button ID="btnCreate" runat="server" Text="💾 Save Exam Details" CssClass="ce-btn-create" OnClick="btnCreate_Click" />
            <asp:Button ID="btnAddQuestions" runat="server" Text="➕ Add Questions →" CssClass="ce-btn-questions" OnClick="btnAddQuestions_Click" />
        </div>
    </div>

    <%-- EXAM LIST SECTION --%>
    <div class="exam-list-section">
        <h3 class="list-title"><span>📋</span> Existing Exam Sets</h3>
        <div class="ce-grid-container">
            <asp:GridView ID="gvExams" runat="server" AutoGenerateColumns="False" DataKeyNames="examId" 
                OnRowCommand="gvExams_RowCommand" CssClass="ce-grid" Width="100%" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="examId" HeaderText="ID" />
                    <asp:BoundField DataField="title" HeaderText="Title" />
                    <asp:BoundField DataField="subject" HeaderText="Subject" />
                    <asp:BoundField DataField="status" HeaderText="Status" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkDel" runat="server" CommandName="DeleteExam" CommandArgument='<%# Eval("examId") %>' 
                                Text="🗑️ Delete" OnClientClick="return confirm('Are you sure you want to delete?');" CssClass="action-link delete" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="ce-grid-header" />
                <RowStyle CssClass="ce-grid-row" />
            </asp:GridView>
        </div>
    </div>
</div>
</asp:Content>