<%@ Page Title="Faculty Dashboard"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="FacultyDashboard.aspx.cs"
    Inherits="OnlineExaminationSystem.FacultyDashboard" %>

<asp:Content ID="FacultyDashboardContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid dashboard-container">

        <h2 class="dashboard-title mb-4">Faculty Dashboard</h2>

        <!-- CARDS -->
        <div class="row g-4">

            <div class="col-md-3">
                <div class="card shadow-sm text-center p-3">
                    <h6>Total Exams</h6>
                    <h2><asp:Label ID="lblTotalExams" runat="server" Text="0" /></h2>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm text-center p-3">
                    <h6>Pending Exams</h6>
                    <h2><asp:Label ID="lblPendingExams" runat="server" Text="0" /></h2>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm text-center p-3">
                    <h6>Approved Exams</h6>
                    <h2><asp:Label ID="lblApprovedExams" runat="server" Text="0" /></h2>
                </div>
            </div>

            <div class="col-md-3">
                <div class="card shadow-sm text-center p-3">
                    <h6>Total Questions</h6>
                    <h2><asp:Label ID="lblQuestions" runat="server" Text="0" /></h2>
                </div>
            </div>

        </div>

        <!-- CHARTS -->
        <div class="row mt-5 g-4">

            <div class="col-md-8">
                <canvas id="activityChart" height="120"></canvas>
            </div>

            <div class="col-md-4">
                <canvas id="pieChart" height="120"></canvas>
            </div>

        </div>

    </div>

    <script>
        let activityChart;
        let pieChart;

        function initCharts() {
            const activityCanvas = document.getElementById('activityChart');
            const pieCanvas = document.getElementById('pieChart');

            activityChart = new Chart(activityCanvas, {
                type: 'bar',
                data: {
                    labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                    datasets: [{
                        label: 'Exams Created',
                        data: [0, 0, 0, 0, 0, 0, 0],
                        backgroundColor: '#6366f1',
                        borderRadius: 6
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            display: true
                        }
                    },
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                precision: 0
                            }
                        }
                    }
                }
            });

            pieChart = new Chart(pieCanvas, {
                type: 'doughnut',
                data: {
                    labels: ['Pending', 'Approved'],
                    datasets: [{
                        data: [0, 0],
                        backgroundColor: ['#facc15', '#22c55e']
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'bottom'
                        }
                    }
                }
            });
        }

        function renderCharts(data) {
            if (!data) {
                return;
            }

            const activity = Array.isArray(data.activity) ? data.activity : [0, 0, 0, 0, 0, 0, 0];
            const pending = Number(data.pending || 0);
            const approved = Number(data.approved || 0);

            activityChart.data.datasets[0].data = activity;
            activityChart.update();

            pieChart.data.datasets[0].data = [pending, approved];
            pieChart.update();

            if (data.error) {
                console.warn(data.error);
            }
        }

        function loadDashboardData() {
            fetch('FacultyDashboard.aspx/GetFacultyDashboardData', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json; charset=utf-8'
                },
                body: '{}'
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Dashboard request failed');
                    }

                    return response.json();
                })
                .then(function (response) {
                    renderCharts(response.d);
                })
                .catch(function (error) {
                    console.error(error);
                    renderCharts({
                        activity: [0, 0, 0, 0, 0, 0, 0],
                        pending: 0,
                        approved: 0
                    });
                });
        }

        document.addEventListener('DOMContentLoaded', function () {
            initCharts();
            loadDashboardData();
        });
    </script>

</asp:Content>
