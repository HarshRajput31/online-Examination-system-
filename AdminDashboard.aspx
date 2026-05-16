<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="OnlineExaminationSystem.AdminDashboard" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <!-- EnablePageMethods is CRITICAL for the chart to update -->
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

    <div class="container-fluid dashboard-container">
        <h2 class="dashboard-title mb-4">🚀 Admin Dashboard</h2>

        <!-- Stat Cards -->
        <div class="row g-4 text-white">
            <div class="col-md-4">
                <div class="card stat-card bg-dark shadow-sm">
                    <div class="card-body text-center">
                        <h6 class="text-secondary">STUDENTS</h6>
                        <h2 class="display-4 font-weight-bold"><asp:Label ID="lblStudents" runat="server" Text="0" /></h2>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card stat-card bg-dark shadow-sm">
                    <div class="card-body text-center">
                        <h6 class="text-secondary">FACULTY</h6>
                        <h2 class="display-4 font-weight-bold"><asp:Label ID="lblFaculty" runat="server" Text="0" /></h2>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card stat-card bg-dark shadow-sm">
                    <div class="card-body text-center">
                        <h6 class="text-secondary">TOTAL USERS</h6>
                        <h2 class="display-4 font-weight-bold"><asp:Label ID="lblUsers" runat="server" Text="0" /></h2>
                    </div>
                </div>
            </div>
        </div>

        <!-- Charts Section -->
        <div class="row mt-5">
            <div class="col-md-8">
                <div class="card bg-dark border-0 shadow-sm p-3">
                    <h5 class="text-white mb-3">📈 System Activity (Current Week)</h5>
                    <div style="position: relative; height:300px; width:100%;">
                        <canvas id="activityChart"></canvas>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card bg-dark border-0 shadow-sm p-3">
                    <h5 class="text-white mb-3">📊 User Distribution</h5>
                    <div style="position: relative; height:300px; width:100%;">
                        <canvas id="pieChart"></canvas>
                    </div>
                </div>
            </div>
        </div>

        <!-- Pending Approvals -->
        <div class="mt-5 text-white">
            <h4 class="mb-4">📋 Pending Exam Approvals</h4>
            <asp:Repeater ID="rptPendingExams" runat="server" OnItemCommand="rptPendingExams_ItemCommand">
                <ItemTemplate>
                    <div class="d-flex justify-content-between align-items-center bg-secondary p-3 rounded mb-2">
                        <div>
                            <strong><%# Eval("Title") %></strong><br />
                            <small>By: <%# Eval("FacultyName") %> | Subject: <%# Eval("Subject") %></small>
                        </div>
                        <div>
                            <asp:Button runat="server" CommandName="Approve" CommandArgument='<%# Eval("ExamId") %>' Text="Approve" CssClass="btn btn-success btn-sm" />
                            <asp:Button runat="server" CommandName="Reject" CommandArgument='<%# Eval("ExamId") %>' Text="Reject" CssClass="btn btn-danger btn-sm" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- Chart Scripts -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script>
        let activityChart;

        function initCharts() {
            // 1. Setup Activity Line Chart
            const ctxActivity = document.getElementById('activityChart').getContext('2d');
            activityChart = new Chart(ctxActivity, {
                type: 'line',
                data: {
                    labels: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
                    datasets: [{
                        label: 'Logins',
                        data: [0, 0, 0, 0, 0, 0, 0], // Initial values
                        borderColor: '#3b82f6',
                        backgroundColor: 'rgba(59, 130, 246, 0.1)',
                        fill: true,
                        tension: 0.4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        y: { beginAtZero: true, ticks: { color: '#94a3b8', stepSize: 1 } },
                        x: { ticks: { color: '#94a3b8' } }
                    },
                    plugins: { legend: { display: false } }
                }
            });

            // 2. Setup Distribution Pie Chart
            const ctxPie = document.getElementById('pieChart').getContext('2d');
            new Chart(ctxPie, {
                type: 'doughnut',
                data: {
                    labels: ['Students', 'Faculty', 'Admins'],
                    datasets: [{
                        // window.studentCount etc. are injected from the C# LoadStats method
                        data: [window.studentCount || 0, window.facultyCount || 0, window.adminCount || 0],
                        backgroundColor: ['#3b82f6', '#10b981', '#f97316'],
                        borderWidth: 0
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { position: 'bottom', labels: { color: '#94a3b8' } } }
                }
            });
        }

        // 3. Fetch data from C# GetLiveDashboardData
        function updateActivityData() {
            if (typeof PageMethods !== 'undefined') {
                PageMethods.GetLiveDashboardData(function (result) {
                    // ASP.NET WebMethods wrap data in 'd'. Check for it.
                    let data = result.hasOwnProperty('d') ? result.d : result;

                    if (data && data.activity) {
                        console.log("Activity Data Updated:", data.activity);
                        activityChart.data.datasets[0].data = data.activity;
                        activityChart.update();
                    }
                }, function (err) {
                    console.error("Graph Data Error:", err.get_message());
                });
            }
        }

        // Initialize on Load
        window.onload = function () {
            initCharts();
            updateActivityData();
            // Optional: Auto-refresh data every 30 seconds
            setInterval(updateActivityData, 30000);
        };
    </script>
</asp:Content>