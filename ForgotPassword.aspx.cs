using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((Label)Master.FindControl("lblErrorMessage")).Text = "";
            }

        }
        protected void btnRetrieve_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            Label lblError = (Label)Master.FindControl("lblErrorMessage");

            if (string.IsNullOrEmpty(username))
            {
                lblError.Text = "Please enter your username.";
                lblError.Visible = true;
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT Password FROM Users WHERE Username = @Username";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    conn.Open();
                    string password = (string)cmd.ExecuteScalar();
                    conn.Close();

                    if (!string.IsNullOrEmpty(password))
                    {
                        lblResult.Text = $"Your password is: {password}";
                        lblResult.ForeColor = System.Drawing.Color.Green;
                        lblResult.Visible = true;
                        lblError.Visible = false;
                    }
                    else
                    {
                        lblError.Text = "Username not found.";
                        lblError.Visible = true;
                        lblResult.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Error retrieving password: {ex.Message}";
                lblError.Visible = true;
                lblResult.Visible = false;
            }
        }
        protected void btnBackToLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Login.aspx");
        }
    }
}