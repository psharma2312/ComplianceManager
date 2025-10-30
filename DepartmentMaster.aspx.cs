using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class DepartmentMaster : System.Web.UI.Page
    {
        Int32 userId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] == null)
            {
                Response.Redirect("../AuthPages/Login.aspx");
            }
            userId = Convert.ToInt32(Session["UserId"].ToString());
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            try
            {
                DAL dal = new DAL();
                var department = dal.GetAllDepartments();
                gvDepartment.DataSource = department;
                gvDepartment.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading Department: {ex.Message}", true);
            }
        }
        private void ShowMessage(string message, bool isError)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.ForeColor = isError ? System.Drawing.Color.FromArgb(0x72, 0x1C, 0x24) : System.Drawing.Color.Green;
            lblErrorMessage.Visible = true;
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {

            string departmentName = txtDepartmentName.Text.Trim();
            string description = txtDescription.Text.Trim();
            if (string.IsNullOrEmpty(departmentName))
            {
                ShowMessage("Please enter a department name.", true);
                return;
            }
            if (string.IsNullOrEmpty(description))
            {
                ShowMessage("Please enter some description.", true);
                return;
            }
            try
            {
                DAL dal = new DAL();
                dal.EditDeleteDepartment(0, "add", userId, departmentName, true, description);
                ShowMessage("Department added successfully.", false);
                txtDepartmentName.Text = "";
                txtDescription.Text = "";
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding Department: {ex.Message}", true);
            }

        }
        protected void gvDepartment_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDepartment.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvDepartment_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvDepartment.EditIndex = e.NewEditIndex;
            BindGrid();
        }
        protected void gvDepartment_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvDepartment.EditIndex = -1;
            BindGrid();
            ShowMessage("Editing cancelled.", false);
        }


        protected void gvDepartment_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Int32 DeptId = Convert.ToInt32(gvDepartment.DataKeys[e.RowIndex].Values[0].ToString());
            try
            {
                DAL mdal = new DAL();
                GridViewRow row = gvDepartment.Rows[e.RowIndex];
                mdal.DeleteDepartment(DeptId, userId);
                BindGrid();
                ShowMessage("Department deleted successfully.", false);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting Department: {ex.Message}", true);
            }
        }

        protected void gvDepartment_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int deptId = Convert.ToInt32(gvDepartment.DataKeys[rowIndex].Values["DeptID"].ToString());
            string departmentName = ((TextBox)gvDepartment.Rows[e.RowIndex].FindControl("txtEditDepartmentName")).Text;
            string description = ((TextBox)gvDepartment.Rows[e.RowIndex].FindControl("txtEditDescription")).Text;
            if (string.IsNullOrEmpty(departmentName))
            {
                ShowMessage("Please enter a Department Name", true);
                return;
            }
            if (string.IsNullOrEmpty(description))
            {
                ShowMessage("Please enter some description.", true);
                return;
            }
            try
            {
                DAL dal = new DAL();
                dal.EditDeleteDepartment(deptId, "update", userId, departmentName, true, description);
                gvDepartment.EditIndex = -1;
                ShowMessage("Department updated successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding Department: {ex.Message}", true);
            }
        }
    }
}