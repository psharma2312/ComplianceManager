using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class Notification1 : System.Web.UI.Page
    {
        int userId = 0;
        string userName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            userId = Convert.ToInt32(Session["UserId"]?.ToString() ?? "0");
            userName = Session["UserName"]?.ToString();
            if (userId == 0)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadNotifications(userId);
                // Update the notification count on initial load
                ComplianceM masterPage = this.Page.Master as ComplianceM;
                if (masterPage != null)
                {
                    masterPage.UpdateNotificationCount();
                }
            }
            
        }
        private void LoadNotifications(int userId)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    using (var cmd = new SqlCommand("compliance.usp_GetUnreadNotifications", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            var notifications = new List<Notification>();
                            while (reader.Read())
                            {
                                notifications.Add(new Notification
                                {
                                    NotificationId = Convert.ToInt32(reader["NotificationId"]),
                                    ComplianceDetailId = Convert.ToInt32(reader["ComplianceDetailId"]),
                                    NotificationType = reader["NotificationType"].ToString(),
                                    NotificationMessage = reader["NotificationMessage"].ToString(),
                                    ComplianceRef = reader["ComplianceRef"].ToString(),
                                    NotificationDate = Convert.ToDateTime(reader["NotificationDate"]),
                                });
                            }
                            GridView1.DataSource = notifications;
                            GridView1.DataBind();
                          
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading notifications: {ex.Message}", true);
            }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            LoadNotifications(userId);
        }
        private void ShowMessage(string message, bool isError)
        {
            //lblErrorMessage.Text = message;
            //lblErrorMessage.ForeColor = isError ? System.Drawing.Color.FromArgb(0x72, 0x1C, 0x24) : System.Drawing.Color.Green;
            //lblErrorMessage.Visible = true;
        }
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MarkAsRead")
            {
                int notificationId = Convert.ToInt32(e.CommandArgument);
                MarkNotificationAsRead(notificationId);
                LoadNotifications(userId);
                // Update the notification count in the master page after marking as read
                ComplianceM masterPage = this.Page.Master as ComplianceM;
                if (masterPage != null)
                {
                    masterPage.UpdateNotificationCount();
                }
            }
        }

        private void MarkNotificationAsRead(int notificationId)
        {
            string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                using (var cmd = new SqlCommand("compliance.sp_MarkNotificationAsRead", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NotificationId", notificationId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}