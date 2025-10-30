using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class ComplianceTypeMaster : System.Web.UI.Page
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
                var complianceType = dal.GetAllComplianceType(0);
                gvComplianceType.DataSource = complianceType;
                gvComplianceType.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading Compliance Type: {ex.Message}", true);
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
            
            string ComplianceTypeName = txtComplianceType.Text.Trim();
            string description = txtDescription.Text.Trim();
            if (string.IsNullOrEmpty(ComplianceTypeName))
            {
                ShowMessage("Please enter a Compliance nature.", true);
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
                dal.EditDeleteComplianceType(0, "add", userId, ComplianceTypeName, true,description);
                ShowMessage("Compliance Type added successfully.", false);
                txtComplianceType.Text = "";
                txtDescription.Text = "";
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding Compliance Type: {ex.Message}", true);
            }
        }

        protected void gvComplianceType_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvComplianceType.EditIndex = e.NewEditIndex;
            BindGrid();
        }
        protected void gvComplianceType_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvComplianceType.EditIndex = -1;
            BindGrid();
            ShowMessage("Editing cancelled.", false);
        }

        protected void gvComplianceType_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Int32 ComplianceTypeID = Convert.ToInt32(gvComplianceType.DataKeys[e.RowIndex].Values[0].ToString());
            try
            {
                DAL mdal = new DAL();
                GridViewRow row = gvComplianceType.Rows[e.RowIndex];
                mdal.DeleteComplianceType(ComplianceTypeID, userId);
                ShowMessage("Compliance Type deleted successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting Compliance Type: {ex.Message}", true);
            }

        }

        protected void gvComplianceType_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int ComplianceTypeID = Convert.ToInt32(gvComplianceType.DataKeys[rowIndex].Values["ComplianceTypeID"].ToString());
            string ComplianceTypeName = ((TextBox)gvComplianceType.Rows[e.RowIndex].FindControl("txtName")).Text;
            string description = ((TextBox)gvComplianceType.Rows[e.RowIndex].FindControl("txtEditDescription")).Text;

            if (string.IsNullOrEmpty(ComplianceTypeName))
            {
                ShowMessage("Please enter a Compliance type.", true);
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
                dal.EditDeleteComplianceType(ComplianceTypeID, "update", userId, ComplianceTypeName, true, description);
                gvComplianceType.EditIndex = -1;
                ShowMessage("Compliance Type updated successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding Compliance Type: {ex.Message}", true);
            }
        }

        protected void gvComplianceType_RowDataBound(object sender, GridViewRowEventArgs e)
        { }

        

    }
}