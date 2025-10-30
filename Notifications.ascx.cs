using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class Notifications : System.Web.UI.UserControl
    {
        int userId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            userId = Convert.ToInt32(Session["UserId"]?.ToString() ?? "0");

            if (userId == 0)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            // Load notifications on every postback to ensure the GridView persists
            LoadNotifications(userId);


        }
        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            // Register the GridView's Mark as Read button as a PostBackTrigger
            RegisterPostBackTrigger();
        }
        private void RegisterPostBackTrigger()
        {
            // Find the UpdatePanel in the master page
            UpdatePanel updatePanel = FindUpdatePanel(this.Page);
            if (updatePanel != null)
            {
                ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
                if (scriptManager != null)
                {
                    // Register each Mark as Read button as a postback control
                    foreach (GridViewRow row in GridView1.Rows)
                    {
                        Button btnMarkAsRead = row.FindControl("btnMarkAsRead") as Button;
                        if (btnMarkAsRead != null)
                        {
                            scriptManager.RegisterPostBackControl(btnMarkAsRead);
                        }
                    }
                }
            }
        }

        private UpdatePanel FindUpdatePanel(Control control)
        {
            foreach (Control child in control.Controls)
            {
                if (child is UpdatePanel)
                {
                    return child as UpdatePanel;
                }
                if (child.Controls.Count > 0)
                {
                    UpdatePanel found = FindUpdatePanel(child);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }

        private void LoadNotifications(int userId)
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
                                NotificationDate = Convert.ToDateTime(reader["NotificationDate"]),
                            });
                        }
                        GridView1.DataSource = notifications;
                        GridView1.DataBind();
                    }
                }
            }
        }
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MarkAsRead")
            {
                int notificationId = Convert.ToInt32(e.CommandArgument);
                MarkNotificationAsRead(notificationId);
                LoadNotifications(userId);
                // Update the notification count in the master page
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
        private void UpdateNotificationCount()
        {
            //string username = Session["Username"]?.ToString() ?? "User";
            //int userID = Convert.ToInt32(Session["UserId"].ToString());
            //if (userID == 0)
            //{
            //    lblNotificationCount.Visible = false;
            //    return;
            //}
            //try
            //{
            //    string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            //    using (SqlConnection conn = new SqlConnection(connectionString))
            //    {
            //        string query = "SELECT COUNT(*) FROM Compliance.Notifications WHERE UserId = @UserId";
            //        SqlCommand cmd = new SqlCommand(query, conn);
            //        cmd.Parameters.AddWithValue("@UserId", userID);
            //        conn.Open();
            //        int count = (int)cmd.ExecuteScalar();
            //        conn.Close();

            //        lblNotificationCount.Text = count.ToString();
            //        lblNotificationCount.Visible = count > 0; // Hide if no notifications
            //    }
            //}
            //catch (Exception ex)
            //{
            //    lblErrorMessage.Text = $"Error fetching notification count: {ex.Message}";
            //    lblErrorMessage.Visible = true;
            //    lblNotificationCount.Visible = false;
            //}

        }
    }
}