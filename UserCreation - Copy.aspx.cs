using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class UserCreation : System.Web.UI.Page
    {
        String strConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        SqlConnection sqlCon;
        SqlCommand sqlCmd;
        SqlDataAdapter sqlDa;
        string strQuery;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindDropDown();
                txtusername.Text = "";
                txtpassword.Text = "";
                txtconpassword.Text = "";
                txtemail.Text = "";
                txtmobile.Text = "";
                ddlDepartment1.SelectedIndex = 0;
                lblError.Visible = false;
                gridFill();
                
            }
           
        }

        private void BindDropDown()
        {
            DAL dal = new DAL();
            var department = dal.GetAllDepartments();

            ddlDepartment1.DataSource = department;
            ddlDepartment1.DataBind();
            ddlDepartment1.DataTextField = "DepartmentName";
            ddlDepartment1.DataValueField = "DeptId";
            ddlDepartment1.DataBind();
            ddlDepartment1.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", ""));

            var supervisor = dal.GetAllSupervisors();

            ddlSupervisor.DataSource = supervisor;
            ddlSupervisor.DataBind();
            ddlSupervisor.DataTextField = "SupervisorName";
            ddlSupervisor.DataValueField = "SupervisorId";
            ddlSupervisor.DataBind();
            ddlSupervisor.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", ""));

        }

        protected void btncreate_Click(object sender, System.EventArgs e)
        {
            try
            {

                string password = txtpassword.Text.Trim();
                string curpass = txtconpassword.Text.Trim();
                DAL dal = new DAL();
                var userCount = dal.GetUserCount(txtusername.Text.Trim());
                var totalUserCount = dal.GetTotalUserCount();
                if (userCount > 0)
                {
                    lblError.Text = "User ID Already exist";
                }
                else
                {
                    if (totalUserCount > 15)
                    {
                        lblError.Text = "This product is licensed for 15 Users only";
                    }
                    else
                    {
                        if (txtpassword.Text.Trim() != txtconpassword.Text.Trim())
                        {
                            lblError.Text = "Password and Confirm Password are not same.";
                        }
                        else
                        {
                            InsertHeader();
                            lblError.Visible = true;
                            lblError.Text = "User Successfully Created ";
                            txtusername.Text = "";
                            txtpassword.Text = "";
                            txtconpassword.Text = "";
                        }
                    }
                    if (sqlCon.State == ConnectionState.Open)
                        sqlCon.Close();
                    gridFill();

                    txtusername.Text = "";
                    txtpassword.Text = "";
                    txtconpassword.Text = "";
                    txtemail.Text = "";
                    txtmobile.Text = "";
                    ddlDepartment1.SelectedIndex = 0;
                    ddlSupervisor.SelectedIndex = 0;
                    chkApprover.Checked = false;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
       
        protected void gvModifyModule_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && gvModifyModule.EditIndex == e.Row.RowIndex)
            {

                DropDownList ddlDepartment = (DropDownList)e.Row.FindControl("ddlDepartment");
                DAL dal = new DAL();
                var department = dal.GetAllDepartments();
                ddlDepartment.DataSource = department;
                ddlDepartment.DataTextField = "DepartmentName";
                ddlDepartment.DataValueField = "DeptId";
                ddlDepartment.DataBind();
                string selectedDeptCode = DataBinder.Eval(e.Row.DataItem, "DeptId").ToString();
                ddlDepartment.Items.FindByValue(selectedDeptCode).Selected = true;

                DropDownList ddlSupervisor = (DropDownList)e.Row.FindControl("ddlSupervisor");
                var supervisor = dal.GetAllSupervisors();
                ddlSupervisor.DataSource = supervisor;
                ddlSupervisor.DataTextField = "SupervisorName";
                ddlSupervisor.DataValueField = "SupervisorId";
                ddlSupervisor.DataBind();
                string selectedGridddlSupervisor = DataBinder.Eval(e.Row.DataItem, "SupervisorId").ToString();
                ddlSupervisor.Items.FindByValue(selectedGridddlSupervisor).Selected = true;
            }
        }
        public void gridFill()
        {
            try
            {
                sqlCon = new SqlConnection(strConn);
                if (sqlCon.State == ConnectionState.Closed) sqlCon.Open();
                //strQuery = "SELECT dept.DeptId, dept.DepartmentName, xm.email, xm.mobile_no, xm.user_name FROM Compliance.Department dept INNER JOIN Compliance.x_user_master xm ON dept.DeptId = xm.dept_id where dept.Isactive=1";
                strQuery = "SELECT DISTINCT u1.SupervisorID AS 'SupervisorId', u2.user_name AS 'SupervisorName', u1.IsActive, u1.dept_id, dp.DepartmentName, u1.email, u1.mobile_no, u1.user_name, u1.user_id , ISNULL(u1.IsApprover,0) as IsApprover FROM [Compliance].x_user_master u1 INNER JOIN [Compliance].x_user_master u2 ON u1.SupervisorID = u2.user_id inner join Compliance.Department dp on u1.dept_id = dp.DeptId where u1.IsActive=1 order by 2 asc";
                sqlDa = new SqlDataAdapter(strQuery, sqlCon);
                DataSet ds = new DataSet();
                sqlDa.Fill(ds);
                gvModifyModule.DataSource = ds;
                gvModifyModule.DataBind();
            }
            catch (Exception ex)
            { lblError.Text = ex.Message; }
            finally
            {
                sqlCon.Close();
                sqlCon.Dispose();
            }
        }

        [Obsolete]
        protected void btnupdate_Click(object sender, System.EventArgs e)
        {
            try
            {
                string password = txtpassword.Text.Trim();
                string curpass = txtconpassword.Text.Trim();
                sqlCon = new SqlConnection(strConn);
                if (sqlCon.State == ConnectionState.Closed) sqlCon.Open();
                // Check condition User already exist into database or not 
                strQuery = "select count(user_id) as userid from compliance.x_user_master where user_name='" + txtusername.Text.Trim() + "'";
                sqlCmd = new SqlCommand(strQuery, sqlCon);
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                if (count > 0)
                {
                    RegisterClientScriptBlock("msgBo", "<script>alert('User ID Already exist');</script>");
                }
                else
                {
                    strQuery = "";
                    InsertHeader();
                    sqlCon.Close();
                    
                    lblError.Visible = true;
                    lblError.Text = "User Successfully Created ";
                    txtusername.Text = "";
                    txtpassword.Text = "";
                    txtconpassword.Text = "";
                    gridFill();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
     
        private void InsertHeader()
        {
            try
            {

                sqlCon = new SqlConnection(strConn);
                sqlCmd = new SqlCommand("compliance.USP_CreateUser_InsertHeader", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCon.Open();
                sqlCmd.Parameters.Add(new SqlParameter("@user_name", SqlDbType.VarChar, 75));
                sqlCmd.Parameters["@user_name"].Value = txtusername.Text.Trim();
                sqlCmd.Parameters.Add(new SqlParameter("@current_pwd", SqlDbType.VarChar, 50));
                sqlCmd.Parameters["@current_pwd"].Value = txtconpassword.Text.Trim();
                sqlCmd.Parameters.Add(new SqlParameter("@email", SqlDbType.VarChar, 75));
                sqlCmd.Parameters["@email"].Value = txtemail.Text.Trim();

                sqlCmd.Parameters.Add(new SqlParameter("@mobile_no", SqlDbType.VarChar, 10));
                sqlCmd.Parameters["@mobile_no"].Value = txtmobile.Text.Trim();

                sqlCmd.Parameters.Add(new SqlParameter("@Dept_Id", SqlDbType.Int, 3));
                sqlCmd.Parameters["@Dept_Id"].Value = ddlDepartment1.SelectedItem.Value;

                sqlCmd.Parameters.Add(new SqlParameter("@SupervisorId", SqlDbType.Int, 3));
                sqlCmd.Parameters["@SupervisorId"].Value = ddlSupervisor.SelectedItem.Value;

                sqlCmd.Parameters.Add(new SqlParameter("@IsApprover", SqlDbType.Bit, 3));
                sqlCmd.Parameters["@IsApprover"].Value = chkApprover.Checked;

                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                sqlCmd.Dispose();
                sqlCon.Close();
                sqlCon.Dispose();
            }
        }

       

        protected void gvModifyModule_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvModifyModule.EditIndex = e.NewEditIndex;
            gridFill();
        }
        protected void gvModifyModule_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvModifyModule.EditIndex = -1;
            gridFill();
        }
        protected void gvModifyModule_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            DropDownList ddlDepartment = gvModifyModule.Rows[e.RowIndex].FindControl("ddlDepartment") as DropDownList;
            string dept_id = (gvModifyModule.Rows[e.RowIndex].FindControl("ddlDepartment") as DropDownList).SelectedItem.Value;
            int departmentID = Convert.ToInt32(ddlDepartment.SelectedValue);
            string UserName = gvModifyModule.DataKeys[e.RowIndex].Value.ToString();
            string txtEmail = ((TextBox)gvModifyModule.Rows[e.RowIndex].FindControl("txtEmail")).Text;
            string txtMobile = ((TextBox)gvModifyModule.Rows[e.RowIndex].FindControl("txtMobile")).Text;
            string supervisorId = (gvModifyModule.Rows[e.RowIndex].FindControl("ddlSupervisor") as DropDownList).SelectedItem.Value;
            bool isApprover = (gvModifyModule.Rows[e.RowIndex].FindControl("chkApprover") as CheckBox).Checked;

            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "UPDATE Compliance.x_user_master SET dept_id = @dept_id, isApprover=@isApprover, SupervisorId =@supervisorId  WHERE User_Name = @UserName";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@dept_id", dept_id);
                    cmd.Parameters.AddWithValue("@isApprover", isApprover);
                    cmd.Parameters.AddWithValue("@supervisorId", supervisorId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    Response.Redirect(Request.Url.AbsoluteUri);
                }
            }

         
            gridFill();
        }

        protected void gvModifyModule_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                DAL dal = new DAL();
                dal.DeleteUser(gvModifyModule.DataKeys[e.RowIndex].Value.ToString());
                gridFill();
            }
            catch (Exception) { }

        }

      
    }
}