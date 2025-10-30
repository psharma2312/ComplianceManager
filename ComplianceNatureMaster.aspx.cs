using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class ComplianceNatureMaster : System.Web.UI.Page
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
                var complianceNatures = dal.GetAllNatureCompliance();
                gvComplianceNature.DataSource = complianceNatures;
                gvComplianceNature.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading Compliance Frequency: {ex.Message}", true);
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
            
                string natureOfCompliance = txtComplianceNature.Text.Trim(); 
                string description = txtDescription.Text.Trim();
                if (string.IsNullOrEmpty(natureOfCompliance))
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
                dal.EditDeleteNatureCompliance(0, "add", userId, natureOfCompliance, true, description);
                ShowMessage("Compliance Nature added successfully.", false);
                txtComplianceNature.Text = "";
                txtDescription.Text = "";
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding Compliance Frequency: {ex.Message}", true);
            }
        }
        protected void gvComplianceNature_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvComplianceNature.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvComplianceNature_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvComplianceNature.EditIndex = e.NewEditIndex;
            BindGrid();
        }
        protected void gvComplianceNature_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvComplianceNature.EditIndex = -1;
            BindGrid();
            ShowMessage("Editing cancelled.", false);
        }

        protected void gvComplianceNature_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Int32 compNatureId = Convert.ToInt32(gvComplianceNature.DataKeys[e.RowIndex].Values["ComplianceNatureId"].ToString());
            try
            {
                DAL mdal = new DAL();
                GridViewRow row = gvComplianceNature.Rows[e.RowIndex];
                mdal.DeleteNatureCompliance(compNatureId, userId);
                ShowMessage("Compliance Nature deleted successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting Compliance Nature: {ex.Message}", true);
            }
        }

        protected void gvComplianceNature_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int ComplianceNatureID = Convert.ToInt32(gvComplianceNature.DataKeys[e.RowIndex].Values["ComplianceNatureId"].ToString());
            string natureOfCompliance = ((TextBox)gvComplianceNature.Rows[e.RowIndex].FindControl("txtEditComplianceNature")).Text;
            string description = ((TextBox)gvComplianceNature.Rows[e.RowIndex].FindControl("txtEditDescription")).Text;
            if (string.IsNullOrEmpty(natureOfCompliance))
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
                dal.EditDeleteNatureCompliance(ComplianceNatureID, "update", userId, natureOfCompliance, true, description);
                gvComplianceNature.EditIndex = -1;
                ShowMessage("Compliance Nature updated successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding Compliance Nature: {ex.Message}", true);
            }
        }
    }
}