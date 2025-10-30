using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class Dashboard : Page
    {
        int userId = 0;
        
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        private readonly string _smtpHost = ConfigurationManager.AppSettings["SmtpServer"];
        private readonly int _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        private readonly string _smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
        private readonly string _smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
        private readonly string _fromEmail = ConfigurationManager.AppSettings["FromEmail"];

        private string currentFilter = "All"; // Track the current filter
        protected void Page_Load(object sender, EventArgs e)
        {
            // Validate session
            if (!ValidateSession())
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            userId = Convert.ToInt32(Session["UserId"]);
            string userType = Session["UserType"]?.ToString() ?? "Default"; // e.g., "Approver 2" or "Preparer 3"
            if (userType =="admin")
            {
                btnMails.Visible = true;
            }
            if (!IsPostBack)
            {
                BindDashboardData(userType);
                //RunComplianceLogic();
                //BindGrid();
                if (userType == "admin")
                {
                    btnMails.Visible = true;
                }
            }
        }
        public void RunComplianceLogic()
        {
            try
            {
                DAL dal = new DAL();
                bool isFullExecution = DetermineExecutionMode();
                dal.ExecuteStoredProcedure("Compliance.GenerateComplianceDueDates");
                SendNotificationsAndReminders();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void SendNotificationsAndReminders()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT cd.ComplianceID, cd.ComplianceRef, cd.ComplianceDueDate, cd.AssignedToUserId, u.Email
                    FROM Compliance.ComplianceDetails cd
                    INNER JOIN compliance.x_user_master u ON cd.AssignedToUserId = u.user_id
                    WHERE cd.ComplianceDueDate <= DATEADD(DAY, 7, GETDATE())
                    AND cd.CompletionDate IS NULL
                    AND cd.IsActive = 1";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int complianceId = reader.GetInt32(0);
                            string complianceRef = reader.GetString(1);
                            DateTime dueDate = reader.GetDateTime(2);
                            int assignedToUserId = reader.GetInt32(3);
                            string email = reader.GetString(4);

                            string subject = dueDate < DateTime.Now
                                ? $"Overdue Compliance: {complianceRef}"
                                : $"Reminder: Compliance {complianceRef} Due Soon";
                            string body = dueDate < DateTime.Now
                                ? $"Compliance {complianceRef} (ID: {complianceId}) was due on {dueDate:dd-MM-yyyy}. Please complete it urgently."
                                : $"Compliance {complianceRef} (ID: {complianceId}) is due on {dueDate:dd-MM-yyyy}. Please ensure completion on time.";

                            SendEmail(email, subject, body);
                        }
                    }
                }
            }
        }
        private void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_smtpHost, _smtpPort))
                {
                    client.Credentials = new System.Net.NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = true;

                    using (var message = new MailMessage(_fromEmail, toEmail, subject, body))
                    {
                        message.IsBodyHtml = false;
                        client.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private bool DetermineExecutionMode()
        {
            // Check last run date to avoid duplicate runs on the same day
            DateTime currentDate = DateTime.Now;
            DateTime today = currentDate.Date;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT LastProcessedFinancialYearStart, LastRunDateTime " +
                    "FROM Compliance.ProcessingControl " +
                    "WHERE ControlId = (SELECT MAX(ControlId) FROM Compliance.ProcessingControl)", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DateTime? lastRunDate = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1);
                            DateTime? lastYearStart = reader.IsDBNull(0) ? (DateTime?)null : reader.GetDateTime(0);
                            if (lastRunDate.HasValue && lastRunDate.Value.Date == today)
                            {
                                return false; // Skip if already ran today
                            }
                            if (!lastRunDate.HasValue || !lastYearStart.HasValue)
                            {
                                return true;
                            }
                            DateTime financialYearStart = new DateTime(currentDate.Month >= 4 ? currentDate.Year + 1 : currentDate.Year, 4, 1);
                            bool isDifferentYear = lastYearStart.Value.Year != financialYearStart.Year;
                            bool isOverYearAgo = (currentDate - lastRunDate.Value).TotalDays > 365;
                            return isDifferentYear || isOverYearAgo;
                        }
                    }
                }
            }
            return true;
        }
        private bool ValidateSession()
        {
            if (Session["UserId"] == null || Session["UserName"] == null)
            {
                return false;
            }
            return true;
        }

       
        private void ShowMessage(string message, bool isError)
        {
            Label lblErrorMessage = (Label)Master.FindControl("lblErrorMessage");
            if (lblErrorMessage != null)
            {
                lblErrorMessage.Text = message;
                lblErrorMessage.ForeColor = isError ? System.Drawing.Color.FromArgb(0x72, 0x1C, 0x24) : System.Drawing.Color.Green;
                lblErrorMessage.Visible = true;
            }
        }
        private void BindDashboardData(string userType)
        {
            try
            {
                // Fetch data for charts
                var complianceStatus = GetComplianceStatus(userType);
                var closureStatus = GetClosureStatus(userType);
                var actionableItems = GetActionableItems(userType);

                // Fetch data for calendar
                var dueDates = GetComplianceDueDates(userType);

                // Serialize data to JSON for client-side
                var dashboardData = new
                {
                    completedCount = complianceStatus.CompletedCount,
                    inProgressCount = complianceStatus.InProgressCount,
                    pendingCount = complianceStatus.PendingCount,
                    rejectedCount = complianceStatus.RejectedCount,
                    closedOnTimeCount = closureStatus.ClosedOnTimeCount,
                    closedWithDelayCount = closureStatus.ClosedWithDelayCount,
                    due30DaysCount = actionableItems.Due30DaysCount,
                    due60DaysCount = actionableItems.Due60DaysCount,
                    dueAfter60DaysCount = actionableItems.DueAfter60DaysCount,
                    dueDates = dueDates.Select(d => new
                    {
                        title = d.Title,
                        date = d.Date.ToString("yyyy-MM-dd"),
                        description = d.Description,
                        compliancetype = d.ComplianceType
                    }).ToList()
                };

                hfDashboardData.Value = new JavaScriptSerializer().Serialize(dashboardData);
                System.Diagnostics.Debug.WriteLine("Serialized Dashboard Data: " + hfDashboardData.Value); // Debug log
                // Bind grid
                BindNonComplianceGrid(userType);
               }
            catch (Exception ex)
            {
                ShowMessage($"Error loading dashboard data: {ex.Message}", true);
                // Ensure UI renders even on error
                hfDashboardData.Value = "{}"; // Empty JSON to render empty charts
            }
        }
        private ComplianceStatusData GetComplianceStatus(string userType)
        {
            var data = new ComplianceStatusData();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT s.ComplianceStatusName as StatusName, COUNT(cd.ComplianceDetailId) as Count
                    FROM Compliance.ComplianceDetails cd
                    INNER JOIN Compliance.ComplianceStatus s ON cd.StatusId = s.ComplianceStatusId
                    WHERE cd.AssignedToUserId = @UserId AND cd.IsActive = 1
                    AND MONTH(cd.ComplianceDueDate) = MONTH(DATEADD(MINUTE, 330, GETUTCDATE()))";
                if (currentFilter != "All")
                {
                    query += " AND s.ComplianceStatusName = @StatusName";
                }
                query += " GROUP BY s.ComplianceStatusName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    if (currentFilter != "All")
                    {
                        cmd.Parameters.AddWithValue("@StatusName", currentFilter);
                    }
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string statusName = reader["StatusName"].ToString();
                            int count = Convert.ToInt32(reader["Count"]);
                            switch (statusName)
                            {
                                case "Approved":
                                    data.CompletedCount = count;
                                    break;
                                case "Pending":
                                    data.PendingCount = count;
                                    break;
                                case "Pending-Approval":
                                    data.InProgressCount = count;
                                    break;
                                case "Rejected":
                                    data.RejectedCount= count;
                                    break;
                            }
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine($"Compliance Status - Completed: {data.CompletedCount}, In Progress: {data.InProgressCount}, Pending: {data.PendingCount}"); // Debug log
            return data;
        }

        private ClosureStatusData GetClosureStatus(string userType)
        {
            var data = new ClosureStatusData();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        SUM(CASE WHEN cd.SentForApprovalDate <= cd.ComplianceDueDate THEN 1 ELSE 0 END) as ClosedOnTime,
                        SUM(CASE WHEN cd.SentForApprovalDate > cd.ComplianceDueDate THEN 1 ELSE 0 END) as ClosedWithDelay
                    FROM Compliance.ComplianceDetails cd
                    INNER JOIN Compliance.ComplianceStatus s ON cd.StatusId = s.ComplianceStatusId
                    WHERE cd.AssignedToUserId = @UserId AND cd.IsActive = 1 AND s.ComplianceStatusName = 'Approved'
                    AND MONTH(cd.ComplianceDueDate) = MONTH(DATEADD(MINUTE, 330, GETUTCDATE()))";
                if (currentFilter != "All")
                {
                    query += " AND s.ComplianceStatusName = @StatusName";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    if (currentFilter != "All")
                    {
                        cmd.Parameters.AddWithValue("@StatusName", currentFilter);
                    }
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.ClosedOnTimeCount = reader["ClosedOnTime"] != DBNull.Value ? Convert.ToInt32(reader["ClosedOnTime"]) : 0;
                            data.ClosedWithDelayCount = reader["ClosedWithDelay"] != DBNull.Value ? Convert.ToInt32(reader["ClosedWithDelay"]) : 0;
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine($"Closure Status - On Time: {data.ClosedOnTimeCount}, With Delay: {data.ClosedWithDelayCount}"); // Debug log
            return data;
        }

        private ActionableItemsData GetActionableItems(string userType)
        {
            var data = new ActionableItemsData();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        SUM(CASE WHEN DATEDIFF(DAY, DATEADD(MINUTE, 330, GETUTCDATE()), cd.ComplianceDueDate) <= 30 THEN 1 ELSE 0 END) as Due30Days,
                        SUM(CASE WHEN DATEDIFF(DAY, DATEADD(MINUTE, 330, GETUTCDATE()), cd.ComplianceDueDate) BETWEEN 31 AND 60 THEN 1 ELSE 0 END) as Due60Days,
                        SUM(CASE WHEN DATEDIFF(DAY, DATEADD(MINUTE, 330, GETUTCDATE()), cd.ComplianceDueDate) > 60 THEN 1 ELSE 0 END) as DueAfter60Days
                    FROM Compliance.ComplianceDetails cd
                    INNER JOIN Compliance.ComplianceStatus s ON cd.StatusId = s.ComplianceStatusId
                    WHERE cd.AssignedToUserId = @UserId AND cd.IsActive = 1 AND cd.ComplianceDueDate >= DATEADD(MINUTE, 330, GETUTCDATE())
AND MONTH(cd.ComplianceDueDate) = MONTH(DATEADD(MINUTE, 330, GETUTCDATE()))";
                if (currentFilter != "All")
                {
                    query += " AND s.ComplianceStatusName = @StatusName";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    if (currentFilter != "All")
                    {
                        cmd.Parameters.AddWithValue("@StatusName", currentFilter);
                    }
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data.Due30DaysCount = reader["Due30Days"] != DBNull.Value ? Convert.ToInt32(reader["Due30Days"]) : 0;
                            data.Due60DaysCount = reader["Due60Days"] != DBNull.Value ? Convert.ToInt32(reader["Due60Days"]) : 0;
                            data.DueAfter60DaysCount = reader["DueAfter60Days"] != DBNull.Value ? Convert.ToInt32(reader["DueAfter60Days"]) : 0;
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine($"Actionable Items - 30 Days: {data.Due30DaysCount}, 31-60 Days: {data.Due60DaysCount}, After 60 Days: {data.DueAfter60DaysCount}"); // Debug log
            return data;
        }

        private List<CalendarEvent> GetComplianceDueDates(string userType)
        {
            var events = new List<CalendarEvent>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT cm.ComplianceRef, cd.ComplianceDueDate as DueDate, cm.DetailsOfComplianceReq as DetailsOfComplianceRequirements,  cm.ActSection as ComplianceType
                    FROM Compliance.ComplianceDetails cd
                    INNER JOIN Compliance.ComplianceMaster cm ON cd.ComplianceID = cm.ComplianceID
                    INNER JOIN Compliance.ComplianceStatus s ON cd.StatusId = s.ComplianceStatusId
                    WHERE cd.AssignedToUserId = @UserId AND cd.IsActive = 1 AND cd.ComplianceDueDate IS NOT NULL
                    AND MONTH(cd.ComplianceDueDate) = MONTH(DATEADD(MINUTE, 330, GETUTCDATE()))";
                if (currentFilter != "All")
                {
                    query += " AND s.ComplianceStatusName = @StatusName";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    if (currentFilter != "All")
                    {
                        cmd.Parameters.AddWithValue("@StatusName", currentFilter);
                    }
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {


                            events.Add(new CalendarEvent
                            {
                                Title = reader["ComplianceRef"].ToString(),
                                Date = Convert.ToDateTime(reader["DueDate"]),
                                Description = reader["DetailsOfComplianceRequirements"].ToString(),
                                ComplianceType = reader["ComplianceType"].ToString()
                            });
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine($"Calendar Events Count: {events.Count}"); // Debug log
            return events;
        }


        private void BindNonComplianceGrid(string userType)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT nci.complianceId, nci.compliancedetailId,cd.complianceRef,
                        nci.ComplianceType, nci.NonComplianceReasons
                        FROM Compliance.NonComplianceIssues nci
                        INNER JOIN Compliance.ComplianceDetails cd ON nci.ComplianceDetailID = cd.ComplianceDetailID
                        INNER JOIN Compliance.ComplianceStatus s ON cd.StatusId = s.ComplianceStatusId
                        WHERE cd.AssignedToUserId = @UserId AND cd.IsActive = 1
                        AND MONTH(cd.ComplianceDueDate) = MONTH(DATEADD(MINUTE, 330, GETUTCDATE()))";
                    if (currentFilter != "All")
                    {
                        query += " AND s.ComplianceStatusName = @StatusName";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        if (currentFilter != "All")
                        {
                            cmd.Parameters.AddWithValue("@StatusName", currentFilter);
                        }
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            System.Diagnostics.Debug.WriteLine($"Non-Compliance Grid Rows: {dt.Rows.Count}"); // Debug log
                            if (dt.Rows.Count == 0)
                            {
                                ShowMessage("No non-compliance issues found for the selected filter.", false);
                            }
                            gvNonCompliance.DataSource = dt;
                            gvNonCompliance.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading non-compliance issues: {ex.Message}", true);
                gvNonCompliance.DataSource = null;
                gvNonCompliance.DataBind();
            }
        }

        protected void btnFilterAll_Click(object sender, EventArgs e)
        {
            currentFilter = "All";
            string userType = Session["UserType"]?.ToString() ?? "Default";
            BindDashboardData(userType);
        }


        protected void btnMails_Click(object sender, EventArgs e)
        {
            SendNotificationsAndReminders();
        }

        
        protected void btnFilterCompleted_Click(object sender, EventArgs e)
        {
            currentFilter = "Completed";
            string userType = Session["UserType"]?.ToString() ?? "Default";
            BindDashboardData(userType);
        }

        protected void btnFilterInProgress_Click(object sender, EventArgs e)
        {
            currentFilter = "In Progress";
            string userType = Session["UserType"]?.ToString() ?? "Default";
            BindDashboardData(userType);
        }

        protected void btnFilterPending_Click(object sender, EventArgs e)
        {
            currentFilter = "Pending";
            string userType = Session["UserType"]?.ToString() ?? "Default";
            BindDashboardData(userType);
        }

        protected void gvNonCompliance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvNonCompliance.PageIndex = e.NewPageIndex;
            string userType = Session["UserType"]?.ToString() ?? "Default";
            BindNonComplianceGrid(userType);
            // Register startup script to update charts and calendar after paging
            string script = "if (typeof updateDashboardFromHiddenField === 'function') { updateDashboardFromHiddenField(); } else { console.error('updateDashboardFromHiddenField is not defined'); }";
            ScriptManager.RegisterStartupScript(this, GetType(), "updateDashboard", script, true);
        }
    }

    // Data classes
    public class ComplianceStatusData
    {
        public int CompletedCount { get; set; } //Approved
        public int InProgressCount { get; set; } //Pending Approval
        public int PendingCount { get; set; } //pending
        public int RejectedCount { get; set; } //Rejected
    }

    public class ClosureStatusData
    {
        public int ClosedOnTimeCount { get; set; }
        public int ClosedWithDelayCount { get; set; }
    }

    public class ActionableItemsData
    {
        public int Due30DaysCount { get; set; }
        public int Due60DaysCount { get; set; }
        public int DueAfter60DaysCount { get; set; }
    }

    public class CalendarEvent
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string ComplianceType { get; set; }
    }
}