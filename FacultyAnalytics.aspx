<%@ Page Title="Analytics" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="FacultyAnalytics.aspx.cs"
    Inherits="OnlineExaminationSystem.FacultyAnalytics" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<div class="fa-page">

    <!-- HEADER -->
    <div class="fa-header">
        <span class="fa-header-icon">📊</span>
        <div>
            <h2 class="fa-title">Exam Analytics</h2>
            <p class="fa-subtitle">
                Subject-wise results, rankings and performance analysis
            </p>
        </div>
    </div>

    <!-- FILTER BAR -->
    <div class="fa-filter-bar">
        <div class="fa-filter-item">
            <label class="fa-filter-label">📚 Subject</label>
            <asp:DropDownList ID="ddlSubject" runat="server"
                CssClass="fa-select"
                AutoPostBack="true"
                OnSelectedIndexChanged="ddlSubject_Changed" />
        </div>
        <div class="fa-filter-item">
            <label class="fa-filter-label">🗂️ Exam / Set</label>
            <asp:DropDownList ID="ddlExam" runat="server"
                CssClass="fa-select"
                AutoPostBack="true"
                OnSelectedIndexChanged="ddlExam_Changed" />
        </div>
        <div class="fa-filter-item">
            <label class="fa-filter-label">🏫 Department</label>
            <asp:DropDownList ID="ddlDept" runat="server"
                CssClass="fa-select"
                AutoPostBack="true"
                OnSelectedIndexChanged="ddlDept_Changed">
                <asp:ListItem Value="">All Departments</asp:ListItem>
                <asp:ListItem Value="Computer Science">Computer Science</asp:ListItem>
                <asp:ListItem Value="Information Technology">Information Technology</asp:ListItem>
                <asp:ListItem Value="Electronics">Electronics</asp:ListItem>
                <asp:ListItem Value="Mechanical">Mechanical</asp:ListItem>
                <asp:ListItem Value="Civil">Civil</asp:ListItem>
                <asp:ListItem Value="MBA">MBA</asp:ListItem>
                <asp:ListItem Value="BCA">BCA</asp:ListItem>
                <asp:ListItem Value="MCA">MCA</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <!-- STATS CARDS -->
    <div class="fa-stats-row">
        <div class="fa-stat-card fa-stat-blue">
            <div class="fa-stat-icon">👥</div>
            <div class="fa-stat-num">
                <asp:Label ID="lblTotal" runat="server" Text="0" />
            </div>
            <div class="fa-stat-label">Total Students</div>
        </div>
        <div class="fa-stat-card fa-stat-green">
            <div class="fa-stat-icon">✅</div>
            <div class="fa-stat-num">
                <asp:Label ID="lblAttempted" runat="server" Text="0" />
            </div>
            <div class="fa-stat-label">Attempted</div>
        </div>
        <div class="fa-stat-card fa-stat-orange">
            <div class="fa-stat-icon">❌</div>
            <div class="fa-stat-num">
                <asp:Label ID="lblNotAttempted" runat="server" Text="0" />
            </div>
            <div class="fa-stat-label">Not Attempted</div>
        </div>
        <div class="fa-stat-card fa-stat-purple">
            <div class="fa-stat-icon">🏆</div>
            <div class="fa-stat-num">
                <asp:Label ID="lblPassed" runat="server" Text="0" />
            </div>
            <div class="fa-stat-label">Passed</div>
        </div>
        <div class="fa-stat-card fa-stat-red">
            <div class="fa-stat-icon">📉</div>
            <div class="fa-stat-num">
                <asp:Label ID="lblFailed" runat="server" Text="0" />
            </div>
            <div class="fa-stat-label">Failed</div>
        </div>
        <div class="fa-stat-card fa-stat-teal">
            <div class="fa-stat-icon">📈</div>
            <div class="fa-stat-num">
                <asp:Label ID="lblAvgMarks" runat="server" Text="0" />
            </div>
            <div class="fa-stat-label">Avg Marks</div>
        </div>
    </div>

    <!-- TOP 3 RANK HOLDERS -->
    <asp:Panel ID="pnlToppers" runat="server" Visible="false">
        <div class="fa-section-title">
            🏆 Top 3 Rank Holders
        </div>
        <div class="fa-toppers-row">
            <asp:Repeater ID="rptToppers" runat="server">
                <ItemTemplate>
                    <div class='fa-topper-card rank-<%# Container.ItemIndex + 1 %>'>
                        <div class="fa-rank-badge">
                            <%# Container.ItemIndex == 0 ? "🥇" :
                                Container.ItemIndex == 1 ? "🥈" : "🥉" %>
                            #<%# Container.ItemIndex + 1 %>
                        </div>
                        <div class="fa-topper-name">
                            <%# Eval("StudentName") %>
                        </div>
                        <div class="fa-topper-dept">
                            <%# Eval("Department") %>
                        </div>
                        <div class="fa-topper-score">
                            <%# Eval("ObtainedMarks") %> /
                            <%# Eval("TotalMarks") %>
                        </div>
                        <div class="fa-topper-pct">
                            <%# Eval("Percentage") %>%
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>

    <!-- RESULTS TABLE -->
    <div class="fa-section-title">
        📋 Student Results
        <div class="fa-table-search-wrap">
            <input type="text" id="tableSearch"
                   class="fa-table-search"
                   placeholder="🔍 Search student..."
                   onkeyup="filterResults()" />
        </div>
    </div>

    <asp:Panel ID="pnlTable" runat="server" Visible="false">
        <div class="fa-table-card">
            <table class="fa-table" id="resultsTable">
                <thead>
                    <tr>
                        <th>Rank</th>
                        <th>Student Name</th>
                        <th>Department</th>
                        <th>Subject</th>
                        <th>Set</th>
                        <th>Marks</th>
                        <th>Percentage</th>
                        <th>Status</th>
                        <th>Attempt</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptResults" runat="server">
                        <ItemTemplate>
                            <tr data-name='<%# Eval("StudentName") %>'>
                                <td>
                                    <span class='fa-rank <%# GetRankClass(Container.ItemIndex + 1) %>'>
                                        <%# GetRankIcon(Container.ItemIndex + 1) %>
                                        <%# Container.ItemIndex + 1 %>
                                    </span>
                                </td>
                                <td class="fa-student-name">
                                    <%# Eval("StudentName") %>
                                </td>
                                <td>
                                    <span class="fa-dept-badge">
                                        <%# Eval("Department") %>
                                    </span>
                                </td>
                                <td><%# Eval("Subject") %></td>
                                <td>
                                    <span class="fa-set-chip">
                                        <%# Eval("SetNumber") %>
                                    </span>
                                </td>
                                <td>
                                    <span class="fa-marks">
                                        <%# Eval("ObtainedMarks") %>
                                        <span class="fa-marks-total">
                                            /<%# Eval("TotalMarks") %>
                                        </span>
                                    </span>
                                </td>
                                <td>
                                    <div class="fa-pct-wrap">
                                        <div class="fa-pct-bar"
                                             style='width:<%# Eval("Percentage") %>%'></div>
                                        <span class="fa-pct-txt">
                                            <%# Eval("Percentage") %>%
                                        </span>
                                    </div>
                                </td>
                                <td>
                                    <span class='fa-status-badge <%# Eval("StatusClass") %>'>
                                        <%# Eval("StatusLabel") %>
                                    </span>
                                </td>
                                <td>
                                    <span class='fa-attempt-badge <%# Eval("AttemptClass") %>'>
                                        <%# Eval("AttemptLabel") %>
                                    </span>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:Panel>

    <!-- EMPTY STATE -->
    <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
        <div class="fa-empty">
            <div class="fa-empty-icon">📭</div>
            <p>No results found for the selected filters.</p>
        </div>
    </asp:Panel>

</div>

<script>
function filterResults() {
    var s = document.getElementById('tableSearch')
                    .value.toLowerCase();
    var rows = document.querySelectorAll(
        '#resultsTable tbody tr');
    rows.forEach(function(r) {
        var name = (r.getAttribute('data-name') || '')
                       .toLowerCase();
        r.style.display = name.includes(s) ? '' : 'none';
    });
}
</script>

</asp:Content>