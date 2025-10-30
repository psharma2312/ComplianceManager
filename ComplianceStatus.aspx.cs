using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class ComplianceStatus1 : System.Web.UI.Page
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
                var department = dal.GetAllComplianceStatus();
                gvComplianceStatus.DataSource = department;
                gvComplianceStatus.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading Compliance Status: {ex.Message}", true);
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
            string ComplianceStatusName = txtComplianceStatusName.Text.Trim();
            if (string.IsNullOrEmpty(ComplianceStatusName))
            {
                ShowMessage("Please enter a Compliance Status.", true);
                return;
            }
            try
            {
                DAL dal = new DAL();
                dal.EditDeleteComplianceStatus(0, "add", userId, ComplianceStatusName, true);
                ShowMessage("Compliance Status added successfully.", false);
                txtComplianceStatusName.Text = "";
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding Compliance Status: {ex.Message}", true);
            }
           
        }
       
        protected void gvComplianceStatus_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvComplianceStatus.EditIndex = e.NewEditIndex;
            BindGrid();
        }
        protected void gvComplianceStatus_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvComplianceStatus.EditIndex = -1;
            BindGrid();
            ShowMessage("Editing cancelled.", false);
        }


        protected void gvComplianceStatus_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Int32 ComplianceStatusId = Convert.ToInt32(gvComplianceStatus.DataKeys[e.RowIndex].Values["ComplianceStatusID"].ToString());
            try
            {
                DAL mdal = new DAL();
                GridViewRow row = gvComplianceStatus.Rows[e.RowIndex];
                mdal.DeleteComplianceStatus(ComplianceStatusId, userId);
                ShowMessage("Compliance Status deleted successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting Compliance Status: {ex.Message}", true);
            }
        }

        protected void gvComplianceStatus_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int ComplianceStatusID = Convert.ToInt32(gvComplianceStatus.DataKeys[rowIndex].Values["ComplianceStatusID"].ToString());
            string ComplianceStatusName = ((TextBox)gvComplianceStatus.Rows[e.RowIndex].FindControl("txtEditComplianceStatusName")).Text;
            if (string.IsNullOrEmpty(ComplianceStatusName))
            {
                ShowMessage("Please enter a Compliance Status.", true);
                return;
            }
            try
            {
                DAL dal = new DAL();
                dal.EditDeleteComplianceStatus(ComplianceStatusID, "update", userId, ComplianceStatusName, true);
                gvComplianceStatus.EditIndex = -1;
                ShowMessage("Compliance Status updated successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding Compliance Status: {ex.Message}", true);
            }
        }
    }
}