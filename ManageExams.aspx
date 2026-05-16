<%@ Page Title="Exam Approvals" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageExams.aspx.cs" Inherits="OnlineExaminationSystem.ManageExams" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Style.css" rel="stylesheet" type="text/css" />

    <div class="admin-approval-container">
        <div class="approval-header">
            <div class="header-content">
                <h1>🛡️ Exam Approval Portal</h1>
                <p>Review, verify, and publish faculty-created exams to the student dashboard.</p>
            </div>
        </div>

        <asp:Label ID="lblStatus" runat="server" CssClass="status-toast" Visible="false"></asp:Label>

        <div class="exam-approval-grid">
            <asp:Repeater ID="rptPendingExams" runat="server" OnItemCommand="rptPendingExams_ItemCommand">
                <ItemTemplate>
                    <div class="approval-card">
                        <div class="card-badge">Set <%# Eval("setNumber") %></div>
                        
                        <div class="card-body">
                            <h3 class="exam-title"><%# Eval("title") %></h3>
                            
                            <div class="exam-meta">
                                <span>📚 <b>Subject:</b> <%# Eval("subject") %></span>
                                <span>👤 <b>Faculty:</b> <%# Eval("facultyName") %></span>
                                <span>⏱️ <b>Duration:</b> <%# Eval("duration") %> Min</span>
                                <span>📝 <b>Total Qs:</b> <%# Eval("totalQuestions") %></span>
                            </div>

                            <div class="card-actions">
                                <asp:LinkButton ID="btnApprove" runat="server" 
                                    CommandName="Approve" 
                                    CommandArgument='<%# Eval("examId") %>' 
                                    CssClass="btn-approve">
                                    🚀 Approve & Publish
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnReject" runat="server" 
                                    CommandName="Reject" 
                                    CommandArgument='<%# Eval("examId") %>' 
                                    CssClass="btn-reject" 
                                    OnClientClick="return confirm('Are you sure you want to REJECT and DELETE this exam?');">
                                    🗑️ Reject
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="empty-state">
            <div class="empty-icon" style="font-size: 50px; margin-bottom: 15px;">☕</div>
            <h3>All caught up!</h3>
            <p>No exams are currently pending approval. Relax!</p>
        </asp:Panel>
    </div>
</asp:Content>