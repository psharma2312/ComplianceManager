using System;
namespace ComplianceManager
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        int roleId = 0;
        int userId = 0;
        private ActionLogger _logger;
        private DAL _dal;

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeLogger();

            roleId = Convert.ToInt32(Session["Role"]);
            userId = Convert.ToInt32(Session["UserId"]);
            _dal = new DAL();

            if (!IsPostBack)
            {
                FillUserName();
            }
        }
        private void InitializeLogger()
        {
            if (Session["CorrelationId"] == null)
            {
                Session["CorrelationId"] = Guid.NewGuid().ToString();
            }

            if (Session["Logger"] == null && Session["UserName"] != null)
            {
                string correlationId = Session["CorrelationId"].ToString();
                _logger = new ActionLogger(Session["UserName"].ToString(), correlationId);
                Session["Logger"] = _logger;
                _logger.Info("Logger initialized with correlation ID.");
            }
            else
            {
                _logger = Session["Logger"] as ActionLogger;
            }
        }

        protected void FillUserName()
        {
            try
            {
                if (roleId == 4) //roleId=4 "DGCAdmin"
                {
                    ddlUsername.Visible = true;
                    txtUserName.Visible = false;
                    ddlUsername.DataSource = _dal.GetAllUsers(0);
                    ddlUsername.DataTextField = "UserName";
                    ddlUsername.DataValueField = "UserId";
                    ddlUsername.DataBind();
                    ddlUsername.Items.Insert(0, "Select User Name");
                }
                else
                {
                    ddlUsername.Visible = false;
                    txtUserName.Visible = true;
                    txtUserName.Text = Session["UserName"].ToString();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading usernames: {ex.Message}", true);
                    _logger?.Error($"FillUserName failed: {ex.Message}");

            }
        }
       
        protected void btnSave_Click(object sender, EventArgs e)
        {
            var logger = Session["Logger"] as ActionLogger;

            string username = "";
            if (roleId == 4) //roleId=4 "DGCAdmin"
            {
                username = ddlUsername.SelectedItem.Text;
                userId = Convert.ToInt32(ddlUsername.SelectedItem.Value);
            }
            else { 
                username = txtUserName.Text;
            }
            //int userId = Convert.ToInt32(ddlUsername.SelectedItem.Value);
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            // Validation
            if (string.IsNullOrEmpty(username) || username == "")
            {
                ShowMessage("Please select a username.", true);
                logger?.Info("Validation failed: Username is empty.");

                return;
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                ShowMessage("New password cannot be empty.", true);
                logger?.Info("Validation failed: New password is empty.");

                return;
            }
            if (newPassword != confirmPassword)
            {
                ShowMessage("New password and confirm password do not match.", true);
                logger?.Info("Validation failed: Passwords do not match.");

                return;
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            DAL dal = new DAL();
            logger?.Info($"Attempting password change for user: {username}");

            int rowsAffected = dal.ChangeUserPassword(username, hashedPassword);
            ShowMessage("User created successfully.", false);
            if (rowsAffected > 0)
            {
                ShowMessage("Password changed successfully.", false);
                logger?.Info($"Password changed successfully for user: {username}");

                txtNewPassword.Text = "";
                txtConfirmPassword.Text = "";
            }
            else
            {
                ShowMessage("Username not found.", true);
                logger?.Info($"Password change failed: Username '{username}' not found.");

            }

        }
        private void ShowMessage(string message, bool isError)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.ForeColor = isError ? System.Drawing.Color.FromArgb(0x72, 0x1C, 0x24) : System.Drawing.Color.Green;
            lblErrorMessage.Visible = true;
        }
    }
}