using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class UserCreation : Page
    {
        private int userId;
        private const int MAX_USERS = 15;
        protected void Page_Load(object sender, EventArgs e)
        {
            userId = Convert.ToInt32(Session["UserId"]);
            if (Session["UserName"] == null)
            {
                Response.Redirect("../AuthPages/Login.aspx");
            }

            if (!Page.IsPostBack)
            {
                PopulateDepartmentDropdown();
                PopulateSupervisorDropdown();
                BindGrid();
                lblErrorMessage.Visible = false;
            }
        }
        private void PopulateDepartmentDropdown()
        {
            try
            {
                DAL dal = new DAL();
                var department = dal.GetAllDepartments();
                ddlDepartment.DataSource = department;
                ddlDepartment.DataTextField = "DepartmentName";
                ddlDepartment.DataValueField = "DeptId";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading Departments: {ex.Message}", true);
            }

        }
        private void PopulateSupervisorDropdown()
        {
            try
            {
                DAL dal = new DAL();
                var supervisor = dal.GetAllSupervisors();
                ddlSupervisor.DataSource = supervisor;
                ddlSupervisor.DataBind();
                ddlSupervisor.DataTextField = "SupervisorName";
                ddlSupervisor.DataValueField = "SupervisorId";
                ddlSupervisor.DataBind();
                ddlSupervisor.Items.Insert(0, new ListItem("Select Supervisor", ""));
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading Supervisor: {ex.Message}", true);
            }
        }

        protected void btnCreateUser_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Visible = false;
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string mobile = txtMobile.Text.Trim();
            string departmentId = ddlDepartment.SelectedValue; //ddlDepartment1.SelectedItem.Value
            string supervisorUsername = ddlSupervisor.SelectedValue;
            string password = txtPassword.Text;
            bool isApprover = chkIsApprover.Checked;
            bool isPreparer = chkIsPreparer.Checked;
            bool isSupervisor = chkIsSupervisor.Checked;
            bool isActive = true;

            // Validation
            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("Username cannot be empty.", true);
                return;
            }
            if (string.IsNullOrEmpty(email))
            {
                ShowMessage("Email cannot be empty.", true);
                return;
            }
            if (string.IsNullOrEmpty(mobile))
            {
                ShowMessage("Mobile cannot be empty.", true);
                return;
            }
            if (string.IsNullOrEmpty(departmentId))
            {
                ShowMessage("Please select a department.", true);
                return;
            }
            if (string.IsNullOrEmpty(password))
                {
                    ShowMessage("Password cannot be empty.", true);
                    return;
                }
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                DAL dal = new DAL();
                //var userCount = dal.GetUserCount(username);
                //if (userCount > 0) {
                //    ShowMessage("Username already exists.", true);
                //    return;
                //}
                var totalUserCount = dal.GetUserCount(username); //dal.GetTotalUserCount();
                if (totalUserCount >= MAX_USERS)
                {
                    ShowMessage("This product is licensed to 15 users only. please contact administrator", true);
                    return;
                }
                //mobile = "+91" + mobile;
                dal.CreateUser(username, email, mobile, departmentId, supervisorUsername, hashedPassword, isApprover, isPreparer, isSupervisor, isActive);
                ShowMessage("User created successfully.", false);
                ClearForm();
                //PopulateSupervisorDropdown(); // Refresh supervisor list with new user
                BindGrid();
            }
            catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation
            {
                ShowMessage("Username already exists.", true);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error creating user: {ex.Message}", true);
            }
        }
       
        public void BindGrid()
        {
            try
            {
                DAL dal = new DAL();
                gvUsers.DataSource = dal.GetAllUsers();
                gvUsers.DataBind();

            }
            catch (Exception ex)
            { 
                ShowMessage($"Error loading users: {ex.Message}", true);
            }
        }

        protected void gvUsers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvUsers.EditIndex = e.NewEditIndex;
            BindGrid();
            PopulateEditDropdowns(e.NewEditIndex);
        }
        protected void gvUsers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvUsers.EditIndex = -1;
            BindGrid();
            ShowMessage("Editing cancelled.", false);
        }
        protected void gvUsers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            lblErrorMessage.Visible = false;
            string username = gvUsers.DataKeys[e.RowIndex].Value.ToString();
            //string email = ((TextBox)gvUsers.Rows[e.RowIndex].FindControl("txtEditEmail")).Text.Trim();
            string mobile = ((TextBox)gvUsers.Rows[e.RowIndex].FindControl("txtEditMobile")).Text.Trim();
            string departmentId = ((DropDownList)gvUsers.Rows[e.RowIndex].FindControl("ddlEditDepartment")).SelectedValue;
            string supervisorUsername = ((DropDownList)gvUsers.Rows[e.RowIndex].FindControl("ddlEditSupervisor")).SelectedValue;
            bool isApprover = ((CheckBox)gvUsers.Rows[e.RowIndex].FindControl("chkEditIsApprover")).Checked;
            bool isPreparer = ((CheckBox)gvUsers.Rows[e.RowIndex].FindControl("chkEditIsPreparer")).Checked;
            //bool isSupervisor = ((CheckBox)gvUsers.Rows[e.RowIndex].FindControl("chkEditIsSupervisor")).Checked;
            bool isActive = ((CheckBox)gvUsers.Rows[e.RowIndex].FindControl("chkEditIsActive")).Checked;
            int roleId = 0;
            
            if (string.IsNullOrEmpty(mobile))
            {
                ShowMessage("Mobile cannot be empty.", true);
                return;
            }
            if (string.IsNullOrEmpty(departmentId))
            {
                ShowMessage("Please select a department.", true);
                return;
            }
            try
            {
                if (isApprover)  
                    roleId = 2;
                    else 
                    roleId=3;

                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Compliance.x_user_master SET dept_id = @dept_id, isApprover=@isApprover, " +
                    "isPreparer=@isPreparer, mobile_no=@mobile_no, SupervisorId =@supervisorId," +
                    "isActive=@IsActive, role_id=@roleId  WHERE User_Name = @UserName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserName", username);
                    //cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@mobile_no", mobile);
                    cmd.Parameters.AddWithValue("@dept_id", departmentId);
                    cmd.Parameters.AddWithValue("@supervisorId", supervisorUsername == "" ? (object)DBNull.Value : supervisorUsername);
                    cmd.Parameters.AddWithValue("@IsApprover", isApprover);
                    cmd.Parameters.AddWithValue("@IsPreparer", isPreparer);
                    cmd.Parameters.AddWithValue("@roleId", roleId);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    gvUsers.EditIndex = -1;
                    ShowMessage("User updated successfully.", false);
                    BindGrid();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error updating user: {ex.Message}", true);
            }
        }

        protected void gvUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            lblErrorMessage.Visible = false;
            string username = gvUsers.DataKeys[e.RowIndex].Value.ToString();
            try
            {
                DAL dal = new DAL();
                dal.DeleteUser(gvUsers.DataKeys[e.RowIndex].Value.ToString());
                ShowMessage("User deleted successfully.", false);
                PopulateSupervisorDropdown(); // Refresh supervisor list
                BindGrid();
            }
            catch (Exception ex) {
                ShowMessage($"Error deleting user: {ex.Message}", true);
            }
        }
        private void PopulateEditDropdowns(int rowIndex)
        {
            DropDownList ddlEditDepartment = (DropDownList)gvUsers.Rows[rowIndex].FindControl("ddlEditDepartment");
            DropDownList ddlEditSupervisor = (DropDownList)gvUsers.Rows[rowIndex].FindControl("ddlEditSupervisor");
            HiddenField hfEditDepartmentId = (HiddenField)gvUsers.Rows[rowIndex].FindControl("hfEditDepartmentId");
            HiddenField hfEditSupervisorId = (HiddenField)gvUsers.Rows[rowIndex].FindControl("hfEditSupervisorId");
            try
            {
                // Populate Department dropdown
                DAL dal = new DAL();
                var department = dal.GetAllDepartments();

                ddlEditDepartment.DataSource = department;
                ddlEditDepartment.DataTextField = "DepartmentName";
                ddlEditDepartment.DataValueField = "DeptId";
                ddlEditDepartment.DataBind();
                ddlEditDepartment.Items.Insert(0, new ListItem("Select Department", ""));
                if (!string.IsNullOrEmpty(hfEditDepartmentId.Value))
                {
                    ddlEditDepartment.SelectedValue = hfEditDepartmentId.Value;
                }
               
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading departments: {ex.Message}", true);
            }

            // Populate Supervisor dropdown
            try
            {
                DAL dal = new DAL();
                var supervisor = dal.GetAllSupervisors();

                ddlEditSupervisor.DataSource = supervisor;
                ddlEditSupervisor.DataTextField = "SupervisorName";
                ddlEditSupervisor.DataValueField = "SupervisorId";
                ddlEditSupervisor.DataBind();
                ddlEditSupervisor.Items.Insert(0, new ListItem("Select Supervisor", ""));
                if (!string.IsNullOrEmpty(hfEditSupervisorId.Value))
                {
                    ddlEditSupervisor.SelectedValue = hfEditSupervisorId.Value;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading supervisors: {ex.Message}", true);
            }
        }
        private void ClearForm()
        {
            txtUsername.Text = "";
            txtEmail.Text = "";
            txtMobile.Text = "";
            ddlDepartment.SelectedIndex = 0;
            ddlSupervisor.SelectedIndex = 0;
            txtPassword.Text = "";
            lblErrorMessage.Visible = false;
            chkIsApprover.Checked = false;
            chkIsPreparer.Checked = false;
            chkIsSupervisor.Checked = false;
        }
        private void ShowMessage(string message, bool isError)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.ForeColor = isError ? System.Drawing.Color.FromArgb(0x72, 0x1C, 0x24) : System.Drawing.Color.Green;
            lblErrorMessage.Visible = true;
        }
    }
}