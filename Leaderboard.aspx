<%@ Page Title="Leaderboard" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Leaderboard.aspx.cs"
    Inherits="OnlineExaminationSystem.LeaderboardPage" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="ce-page-header">
    <span class="ce-page-icon">🏆</span>
    <div>
        <h2 class="ce-page-title">Leaderboard</h2>
        <p class="ce-page-subtitle">Top performers across the platform.</p>
    </div>
</div>

<!-- Filter -->
<div class="ce-form-card" style="margin-bottom:20px;">
    <div class="ce-row-2">
        <div class="ce-field">
            <label class="ce-label">Subject</label>
            <asp:DropDownList ID="ddlSubject" runat="server" CssClass="ce-input" AutoPostBack="true"
                OnSelectedIndexChanged="OnFilterChange" />
        </div>
        <div class="ce-field">
            <label class="ce-label">Department</label>
            <asp:DropDownList ID="ddlDept" runat="server" CssClass="ce-input" AutoPostBack="true"
                OnSelectedIndexChanged="OnFilterChange" />
        </div>
    </div>
</div>

<!-- Top 3 -->
<asp:Panel ID="pnlTop3" runat="server" CssClass="exam-grid" Visible="false">
    <asp:Repeater ID="rptTop3" runat="server">
        <ItemTemplate>
            <div class="exam-card" style='border:1px solid <%# Container.ItemIndex == 0 ? "#fcd34d" :
                                                          Container.ItemIndex == 1 ? "#cbd5e1" : "#f59e0b" %>;'>
                <div class="ec-subject" style="font-size:24px; letter-spacing:0;">
                    <%# Container.ItemIndex == 0 ? "🥇" : Container.ItemIndex == 1 ? "🥈" : "🥉" %> Rank #<%# Container.ItemIndex + 1 %>
                </div>
                <div class="ec-title"><%# Eval("StudentName") %></div>
                <div class="ec-meta">
                    <span><i class="fa-solid fa-trophy"></i><%# Eval("AverageScore", "{0:F1}") %> avg</span>
                    <span><i class="fa-solid fa-list-check"></i><%# Eval("ExamsTaken") %> exams</span>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>

<!-- Full table -->
<h3 style="margin:30px 0 14px;">Full Rankings</h3>
<asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
    No results yet.
</asp:Panel>

<div>
    <asp:Repeater ID="rptAll" runat="server">
        <ItemTemplate>
            <div class="lb-row">
                <div class='lb-rank <%# Container.ItemIndex == 0 ? "gold" :
                                        Container.ItemIndex == 1 ? "silver" :
                                        Container.ItemIndex == 2 ? "bronze" : "" %>'>
                    #<%# Container.ItemIndex + 1 %>
                </div>
                <div>
                    <strong><%# Eval("StudentName") %></strong>
                    <div style="color:#94a3b8; font-size:12px;">
                        <%# Eval("Department") %>
                    </div>
                </div>
                <div style="text-align:right;">
                    <strong><%# Eval("AverageScore", "{0:F1}") %></strong>
                    <div style="font-size:11px; color:#94a3b8;">avg score</div>
                </div>
                <div style="text-align:right;">
                    <strong><%# Eval("ExamsTaken") %></strong>
                    <div style="font-size:11px; color:#94a3b8;">exams</div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>

</asp:Content>
