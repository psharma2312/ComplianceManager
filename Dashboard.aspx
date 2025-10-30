<%@ Page Title="" Language="C#" MasterPageFile="~/ComplianceM.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="ComplianceManager.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cntPlaceHolder1" runat="server">
        <style type="text/css">
        .dashboard-container {
            padding: 0px; /*2px*/
            font-family: Arial, sans-serif;
        }
        .filter-row {
            display: flex;
            gap: 10px;
            justify-content: center;
            padding: 3px;
            background-color: #f9f9f9;
            border-radius: 4px;
        }
        .filter-button {
            background-color: #007bff; /* Blue */
            color: white;
            border: none;
            padding: 2px 8px;
            border-radius: 4px;
            cursor: pointer;
            font-size:12px;
        }
        .filter-button:hover {
            background-color: #0069d9;
        }
        .chart-section, .grid-section, .calendar-section {
            background-color: white;
            border: 1px solid #ddd;
            border-radius: 4px;
            padding: 5px;
            height:185px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
       
        .section-header {
            margin-top: 0;
            color: #0066cc;
            font-size: 14px;
            background-color: #e6f2ff; /* Light blue background for headers */
            padding: 5px;
            border-radius: 4px;
            border-bottom: 1px solid #0066cc; /* Darker blue border for header */
        }
        h3 {
            margin-top: 0;
            color: #0066cc;
            font-size: 18px;
        }
        
.frequency-grid {
    width: 100%;
    border-collapse: collapse;
    margin-top: 0px;
}

    .frequency-grid th {
        background-color: #0066cc;
        color: white;
        padding: 5px;
        text-align: left;
        font-size: 12px;
    }

    .frequency-grid td {
        padding: 3px;
        border-bottom: 1px solid #ddd;
        font-size: 11px;
    }
        .pager-style {
    padding: 10px 0;
    text-align: center;
    background-color: #f9f9f9;
    border-top: 1px solid #ddd;
    margin-top: 10px;
}

    .pager-style a, .pager-style span {
        display: inline-block;
        padding: 2px 8px;
        margin: 0 2px;
        background-color: white;
        color: #0066cc;
        border: 1px solid #ddd;
        border-radius: 4px;
        text-decoration: none;
        font-size: 11px;
        font-family: Arial, sans-serif;
    }

        .pager-style a:hover {
            background-color: #0066cc;
            color: white;
            border-color: #004d99;
        }

    .pager-style span {
        background-color: #0066cc;
        color: white;
        border-color: #004d99;
        cursor: default;
    }
        /* Modal Styles */
        .modal {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
            z-index: 1000;
        }
        .modal-content {
            background-color: #fff;
            margin: 15% auto;
            padding: 20px;
            border-radius: 5px;
            width: 80%;
            max-width: 500px;
            text-align: left;
            position: relative;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }
        .modal-content h3 {
            margin-top: 0;
            color: #0066cc;
            font-size: 18px;
            text-align: center;
        }
        .modal-content p {
            margin: 10px 0;
            font-size: 14px;
            line-height: 1.5;
        }
        .modal-close {
            position: absolute;
            top: 10px;
            right: 10px;
            font-size: 18px;
            cursor: pointer;
            color: #333;
        }
        .modal-close:hover {
            color: #000;
        }
       
        /* Custom CSS for FullCalendar to fit within container, make date boxes small, and reduce header margin */
        #calendar .fc .fc-toolbar.fc-header-toolbar {
            margin: 2px 0; /* Reduced margin for the toolbar (top and bottom) */
            padding: 5px; /* Reduced padding for compactness */
            font-size:9px;
        }
        #calendar .fc-daygrid-day {
            min-height: 20px; /* Reduced height for smaller date boxes */
            font-size: 9px; /* Smaller font size for dates */
        }
        #calendar .fc-daygrid-day-number {
            padding: 1px; /* Reduced padding for compactness */
            font-size: 8px; /* Smaller font size for day numbers */
        }
        #calendar .fc-daygrid-day-events {
            margin-top: 0px; /* Reduced margin for events */
            padding: 0px;
            font-size:6px;
        }
        #calendar .fc-daygrid-day-frame {
            border: 0px solid #ddd; /* Light gray border for better visibility */
        }
      
        #calendar .fc-header-toolbar {
            padding: 4px; /* Reduced padding for header */
            margin-bottom:4px;
        }

        .fc .fc-toolbar.fc-header-toolbar {
            margin-bottom: 4px;
        }
       .fc .fc-toolbar-title {
            font-size: 10px;
            margin: 0;
        }
       .fc .fc-button {
            font-size: 8px;
            line-height: .5;
            border-radius: .25em;
        }
       .fc .fc-button .fc-icon {
            font-size: 7px;
        }
        #calendar .fc-daygrid-day-top {
            margin: 1px; /* Reduced margin for top section */
        }
        #calendar .fc .fc-daygrid-day-bottom {
            font-size: 9px;
            padding: 0px 0px 0;
        }
        #calendar .fc-daygrid-event {
            margin: 0px 0; /* Reduced margin for events */
            font-size: 6px; /* Smaller font size for events */
            padding: 1px; /* Reduced padding for events */
        }
        #calendar .fc-daygrid-day-events {
            margin-top: 0px;
            padding: 0px;
            font-size: 6px;
            margin-bottom: 0px;
        }

        #calendar .fc-scrollgrid {
            /*border: none; /* Remove default border */
        }
        #calendar .fc-scrollgrid-sync-table{
            height:197px;
        }
        #calendar .fc-daygrid-day.fc-day-today {
            background-color: #e6f2ff; /* Light blue for today */
            border: 2px solid #0066cc; /* Darker blue border for today */
        }
        #calendar {
            overflow: hidden; /* Prevent overflow of calendar content */
            max-width: 100%; /* Ensure calendar doesn’t exceed container width */
            width: 80%; /* Reduced width to 80% for narrower calendar */
        }
        .calendar-section {
            overflow: hidden; /* Ensure the parent container prevents overflow */
        }
        .fc-event-main div{
            background-color: #007bff;
            color: white;
            padding: 2px;
            border-radius: 3px;
            font-size: 6px;
        }
    </style>
    <div class="dashboard-container">
        <!-- Full-width Filter Row -->
        <div class="filter-row" style="width: 100%; margin-bottom: 15px;">
            <asp:Button ID="btnFilterAll" runat="server" Text="All" OnClick="btnFilterAll_Click" CssClass="filter-button" />
            <asp:Button ID="btnFilterCompleted" runat="server" Text="Completed" OnClick="btnFilterCompleted_Click" CssClass="filter-button" />
            <asp:Button ID="btnFilterInProgress" runat="server" Text="In Progress" OnClick="btnFilterInProgress_Click" CssClass="filter-button" />
            <asp:Button ID="btnFilterPending" runat="server" Text="Pending" OnClick="btnFilterPending_Click" CssClass="filter-button" />
            <asp:Button ID="btnMails" runat="server" Text="Send Initial Mails" OnClick="btnMails_Click" CssClass="filter-button" Visible="false"/>
        
        </div>

        <!-- Charts Row (3 equal parts) -->
        <div class="charts-row" style="display: flex; gap: 20px; margin-bottom: 15px;">
            <!-- Bar Chart: Compliance Status -->
            <div class="chart-section" style="flex: 1; min-width: 0;">
                <h3 class="section-header">Compliance Status</h3>
                <canvas id="complianceStatusChart" style="max-height: 150px;"></canvas>
            </div>


            <!-- Pie Chart: Closure Status -->
            <div class="chart-section" style="flex: 1; min-width: 0; display: flex; flex-direction: column; align-items: center;">
            <%--<div class="chart-section" style="flex: 1; min-width: 0;">--%>
                <h3 class="section-header"  style="width: 100%;">Closure Status</h3>
                <canvas class="canvas_cl" id="closureStatusChart"  style="max-width: 300px;max-height: 170px;"></canvas>
            </div>

            <!-- Horizontal Bar Chart: Actionable Items -->
            <div class="chart-section" style="flex: 1; min-width: 0;">
                <h3 class="section-header">Actionable Items</h3>
                <canvas id="actionableItemsChart" style="max-height: 150px;"></canvas>
            </div>
        </div>

        <!-- Grid and Calendar Row (2 parts) -->
        <div class="bottom-row" style="display: flex; gap: 20px;height:290px;">
            <!-- Grid (spanning 2 columns) -->
            <div class="grid-section" style="flex: 2; min-width: 0;height:290px;">
                <h3 class="section-header">Non-Compliance Issues</h3>
                <asp:GridView ID="gvNonCompliance" runat="server" AutoGenerateColumns="false"   BorderColor="#E7E7FF"
                    AllowPaging="true" PageSize="5" GridLines="Both" BorderStyle="None"  BackColor="White"
                    OnPageIndexChanging="gvNonCompliance_PageIndexChanging"
                    CssClass="frequency-grid" Width="100%"  EmptyDataText="No Non-Compliance Issues." ShowHeaderWhenEmpty="true" >

                    <Columns>
                        <%--<asp:TemplateField HeaderText="S.No" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblSNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                       <%-- <asp:BoundField HeaderText="ID" DataField="ComplianceDetailId" ItemStyle-Width="150px" />--%>
                        <asp:BoundField HeaderText="Compliance Ref" DataField="ComplianceRef" ItemStyle-Width="90px" />
                        <asp:BoundField HeaderText="Compliance Type" DataField="ComplianceType" ItemStyle-Width="150px" />
                        <asp:BoundField HeaderText="Non-Compliance Reasons" DataField="NonComplianceReasons" ItemStyle-Width="300px" />
                    </Columns>
                    <RowStyle BackColor="#FFFFFF" /> 
                     <AlternatingRowStyle BackColor="#F0F0F0" /> 
                     <SelectedRowStyle BackColor="#FFFF00" /> 
                     <PagerStyle CssClass="pager-style" />
                     <EmptyDataTemplate>
                         <div align="center">No Non-Compliance Issues.</div>
                     </EmptyDataTemplate>
                </asp:GridView>
            </div>

            <div class="calendar-section" style="flex: 2; display: flex; flex-direction: column; font-size:xx-small;height:290px;">
                <h3 class="section-header" >Compliance Due Dates</h3>
                 <div id="calendar" style="height: 290px;  width: 100%; margin:2px auto;overflow:hidden; max-width:100%;"></div> <!-- Centered with margin: 0 auto -->
            </div>
          
        </div>
    </div>
    <!-- Modal for Event Details -->
    <div id="eventModal" class="modal">
        <div class="modal-content">
            <span class="modal-close" onclick="closeModal()">&times;</span>
            <h3>Compliance Details</h3>
            <p id="eventDetails"></p>
        </div>
    </div>

    <%-- Hidden fields to store chart and calendar data --%>
    <asp:HiddenField ID="hfDashboardData" runat="server" />

    <!-- Scripts for Charts and Calendar -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@5.11.0/main.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/fullcalendar@5.11.0/main.min.css" rel="stylesheet" />

    <script type="text/javascript">
        // Ensure global scope for functions
        window.updateDashboardFromHiddenField = updateDashboardFromHiddenField;
        window.closeModal = closeModal;
        window.initializeDashboard = initializeDashboard;
        // Initialize Charts
        var complianceStatusChart, closureStatusChart, actionableItemsChart, calendar;
        // Function to initialize charts and calendar
        function initializeDashboard() {
            console.log("Initializing dashboard components");

            // Destroy existing chart instances if they exist
            if (complianceStatusChart) {
                complianceStatusChart.destroy();
            }
            if (closureStatusChart) {
                closureStatusChart.destroy();
            }
            if (actionableItemsChart) {
                actionableItemsChart.destroy();
            }
            if (calendar) {
                calendar.destroy();
            }
            // Initialize Compliance Status Bar Chart
            var complianceStatusCtx = document.getElementById('complianceStatusChart');
            if (complianceStatusCtx) {
                complianceStatusChart = new Chart(complianceStatusCtx.getContext('2d'), {
            type: 'bar',
            data: {
//                labels: ['Completed', 'In Progress', 'Pending'],
                labels: ['Completed', 'Pending-Approval', 'Pending', 'Rejected'],
                datasets: [{
                    label: 'Compliance Status',
                    data: [0, 0, 0, 0],
                    backgroundColor: ['#28a745', '#007bff', '#28a755'  ,'#dc3545'],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        title: { display: true, text: 'Count', font: { size: 8 } },
                        ticks: { font: { size: 8 } }
                    },
                    x: {
                        ticks: { font: { size: 8 } }
                    }
                }
            }
        });
            } else {
                console.error("complianceStatusChart canvas not found");
            }

            // Initialize Closure Status Pie Chart
            var closureStatusCtx = document.getElementById('closureStatusChart');
            if (closureStatusCtx) {
                closureStatusChart = new Chart(closureStatusCtx.getContext('2d'), {
        
            type: 'pie',
            data: {
                labels: ['Closed On/Before Time', 'Closed With Delay'],
                datasets: [{
                    data: [0, 0],
                    backgroundColor: ['#28a745', '#dc3545']
                }]
            },
            options: {
                plugins: {
                    legend: {
                        position: 'bottom',
                        align: 'center',
                        labels: {
                            boxWidth: 10,
                            boxHeight: 10,
                            font: {
                                size: 8,
                                family: 'Arial, sans-serif'
                            },
                            padding: 2,
                            generateLabels: function (chart) {
                                return chart.data.labels.map((label, i) => ({
                                    text: label,
                                    fillStyle: chart.data.datasets[0].backgroundColor[i],
                                    strokeStyle: chart.data.datasets[0].backgroundColor[i],
                                    lineWidth: 0,
                                    hidden: chart.getDatasetMeta(0).data[i].hidden,
                                    index: i
                                }));
                            }
                        }
                    }
                },
                layout: {
                    padding: {
                        left: 10,
                        right: 10,
                        top: 10,
                        bottom: 20
                    }
                }
            }
        });

            } else {
                console.error("closureStatusChart canvas not found");
            }

            // Initialize Actionable Items Horizontal Bar Chart
            var actionableItemsCtx = document.getElementById('actionableItemsChart');
            if (actionableItemsCtx) {
                actionableItemsChart = new Chart(actionableItemsCtx.getContext('2d'), {
                    type: 'bar',
            data: {
                labels: ['Due within 30 days', 'Due within 31-60 days', 'Due after 60 days'],
                datasets: [{
                    label: 'Actionable Items',
                    data: [0, 0, 0],
                    backgroundColor: ['#007bff', '#28a745', '#dc3545'],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                indexAxis: 'y',
                scales: {
                    x: {
                        beginAtZero: true,
                        title: { display: true, text: 'Days', font: { size: 8 } },
                        ticks: { font: { size: 8 } }
                    },
                    y: {
                        ticks: { font: { size: 8 } }
                    }
                }
            }
        });
            } else {
                console.error("actionableItemsChart canvas not found");
            }

            // Initialize FullCalendar
            var calendarEl = document.getElementById('calendar');
            if (calendarEl) {
      
            calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridMonth',
                events: [],
                eventClick: function (info) {
                    // Format the event details with bold labels
                    var detailsHtml = `
                        <div><b>Compliance Ref:</b> ${info.event.title}</div>
                        <div><b>Details:</b> ${info.event.extendedProps.description}</div>
                        <div><b>Compliance Type:</b> ${info.event.extendedProps.compliancetype}</div>
                    `;
                    document.getElementById('eventDetails').innerHTML = detailsHtml;
                    document.getElementById('eventModal').style.display = 'block';

                    //alert('Compliance Ref : ' + info.event.title + ', Details : ' + info.event.extendedProps.description + ', Compliance Type : ' + info.event.extendedProps.compliancetype);
                },
                eventContent: function (info) {
                    return { html: '<div class="fc-event-main" style="background-color: #007bff; color: white; padding: 1px; border-radius: 2px; font-size: 6px;">' + info.event.title + '</div>' };
                },
                //eventContent: function (info) {
                //    return { html: '<div class="fc-event-main" style="background-color: #007bff; color: white; padding: 1px; border-radius: 2px; font-size: 6px;">' + info.event.title + '</div>' };
                //},
                slotDuration: '00:30:00',
                slotLabelInterval: '01:00',
                height: 'auto',
                contentHeight: 250,
                dayMaxEventRows: 2,
                moreLinkClick: 'popover',
                eventTimeFormat: {
                    hour: '2-digit',
                    minute: '2-digit',
                    meridiem: false
                },
                aspectRatio: 1,
                fixedWeekCount: false,
                dayCellContent: function (info) {
                    return { html: '<div style="font-size: 8px; padding: 0px;">' + info.dayNumberText + '</div>' };
                }
            });
            calendar.render();

            // Load initial dashboard data
            updateDashboardFromHiddenField();
            } else {
                console.error("calendar div not found");
            }

            console.log("Dashboard components initialized");
        }


        // Function to update charts and calendar with server data
        function updateDashboard(data) {
            console.log("Updating dashboard with data:", data); // Debug log
            try {
                // Ensure charts and calendar are initialized
                if (!complianceStatusChart || !closureStatusChart || !actionableItemsChart || !calendar) {
                    console.error("One or more UI components are not initialized.");
                    return;
                }
                // Update Compliance Status Chart
                complianceStatusChart.data.datasets[0].data = [
                    data && data.completedCount ? data.completedCount : 0,
                    data && data.inProgressCount ? data.inProgressCount : 0,
                    data && data.pendingCount ? data.pendingCount : 0,
                    data && data.rejectedCount ? data.rejectedCount : 0
                ];
                complianceStatusChart.update();

                // Update Closure Status Chart
                closureStatusChart.data.datasets[0].data = [
                    data && data.closedOnTimeCount ? data.closedOnTimeCount : 0,
                    data && data.closedWithDelayCount ? data.closedWithDelayCount : 0
                ];
                closureStatusChart.update();
                // Update Actionable Items Chart
                actionableItemsChart.data.datasets[0].data = [
                    data && data.due30DaysCount ? data.due30DaysCount : 0,
                    data && data.due60DaysCount ? data.due60DaysCount : 0,
                    data && data.dueAfter60DaysCount ? data.dueAfter60DaysCount : 0
                ];
                actionableItemsChart.update();
                // Update Calendar Events
                calendar.getEvents().forEach(event => event.remove());
                if (data && data.dueDates && Array.isArray(data.dueDates)) {
                    data.dueDates.forEach(function (event) {
                        calendar.addEvent({
                            title: event.title || 'N/A',
                            start: event.date,
                            description: event.description || 'Compliance due',
                            compliancetype: event.compliancetype || 'N/A'
                        });
                    });
                }
                calendar.render();
                console.log("Dashboard updated successfully");
            } catch (e) {
                console.error("Error updating dashboard:", e);
            }
        }

        // Function to load data from hidden field
        function updateDashboardFromHiddenField() {
            console.log("updateDashboardFromHiddenField called");
            try {
                var dashboardDataElement = document.getElementById('<%= hfDashboardData.ClientID %>');
                if (dashboardDataElement) {
                    var dashboardData = dashboardDataElement.value;
                    console.log("Raw dashboard data from hidden field:", dashboardData);
                    if (dashboardData) {
                        var data = JSON.parse(dashboardData);
                        console.log("Parsed dashboard data:", data);
                        updateDashboard(data);
                    } else {
                        console.warn("Dashboard data is empty. Rendering empty charts and calendar.");
                        updateDashboard(null); // Render empty charts and calendar
                    }
                } else {
                    console.error("Hidden field not found. Rendering empty charts and calendar.");
                    updateDashboard(null); // Render empty charts and calendar
                }
            } catch (e) {
                console.error("Error parsing dashboard data:", e);
                updateDashboard(null); // Render empty charts and calendar on error
            }
        }
      
        // Modal functions
        function closeModal() {
            document.getElementById('eventModal').style.display = 'none';
        }

        // Close modal when clicking outside of it
        window.onclick = function (event) {
            var modal = document.getElementById('eventModal');
            if (modal && event.target == modal) {
                modal.style.display = 'none';
            }
        };
        // Initial load
        document.addEventListener('DOMContentLoaded', function () {
            console.log("DOMContentLoaded event fired");
            initializeDashboard();
            updateDashboardFromHiddenField();
        });

        // Handle partial postback with ASP.NET AJAX
        if (typeof Sys !== 'undefined') {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
                console.log("Partial postback completed");
                // Since the UpdatePanel is in the Master Page, we can't check for a specific UpdatePanel ID
                // Re-initialize the dashboard for any postback affecting this page
                initializeDashboard();
                updateDashboardFromHiddenField();

                // Re-attach modal close event
                var modalClose = document.querySelector('.modal-close');
                if (modalClose) {
                    modalClose.onclick = closeModal;
                }
            });
        }
    </script>

  
</asp:Content>