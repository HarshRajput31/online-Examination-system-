<%@ Page Title="Check Descriptive Answers" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="CheckAnswers.aspx.cs"
    Inherits="OnlineExaminationSystem.CheckAnswers" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">📝</span>
    <div>
        <h2 class="ce-page-title">Grade Descriptive Answers</h2>
        <p class="ce-page-subtitle">Award marks for each descriptive question and submit to finalize the result.</p>
    </div>
</div>

<asp:Panel ID="pnlNoSubmissions" runat="server" Visible="false" CssClass="alert alert-info">
    No submissions waiting for descriptive grading.
</asp:Panel>

<asp:Label ID="lblMsg" runat="server" CssClass="ce-msg" Visible="false" />

<asp:Repeater ID="rptSubmissions" runat="server" OnItemCommand="rptSubmissions_ItemCommand">
    <ItemTemplate>
        <div class="ce-form-card" style="margin-bottom:18px;">
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:12px;">
                <div>
                    <strong><%# Eval("ExamName") %></strong>
                    <span class="status-badge status-pending" style="margin-left:8px;">Pending</span>
                    <div style="color:#94a3b8; font-size:12px;">
                        Student <%# Eval("StudentName") %> &middot; Submitted
                        <%# Eval("SubmittedAt", "{0:dd MMM yyyy hh:mm tt}") %>
                    </div>
                </div>
                <div style="text-align:right;">
                    <div style="font-size:11px; color:#94a3b8;">Auto-graded MCQ score</div>
                    <strong><%# Eval("Score") %> / <%# Eval("TotalMarks") %></strong>
                </div>
            </div>

            <asp:Repeater runat="server" ID="rptDescAnswers" DataSource='<%# Eval("DescriptiveAnswers") %>'>
                <ItemTemplate>
                    <div class="list-row" style="flex-direction:column; align-items:stretch; gap:8px;">
                        <div>
                            <strong>Q.</strong> <%# Eval("QuestionText") %>
                            <span style="float:right; color:#94a3b8; font-size:12px;">
                                Max marks: <%# Eval("MaxMarks") %>
                            </span>
                        </div>
                        <div style="background:rgba(255,255,255,0.03); padding:10px; border-radius:8px; font-size:13px;">
                            <%# Eval("StudentAnswer") %>
                        </div>
                        <div style="display:flex; gap:8px; align-items:center;">
                            <span style="font-size:12px; color:#94a3b8;">Marks awarded:</span>
                            <input type="number" class="ce-input" style="max-width:100px;"
                                   name='descmark_<%# Eval("QuestionId") %>'
                                   min="0" max='<%# Eval("MaxMarks") %>' step="0.5" value="0" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <div class="ce-btn-row" style="margin-top:16px;">
                <asp:Button runat="server" Text="✅ Save & Finalize"
                    CssClass="ce-btn-create"
                    CommandName="Finalize" CommandArgument='<%# Eval("ResultId") %>' />
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

</asp:Content>
