using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class DrivenBy : Page
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
                var department = dal.GetAllDrivenBy();
                gvDrivenBy.DataSource = department;
                gvDrivenBy.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading DrivenBy: {ex.Message}", true);
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

            string drivenByName = txtDrivenByName.Text.Trim();
            string description = txtDescription.Text.Trim();
            if (string.IsNullOrEmpty(drivenByName))
            {
                ShowMessage("Please enter a drivenby name.", true);
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
                dal.EditDeleteDrivenBy(0, "add", userId, drivenByName, true, description);
                ShowMessage("drivenby added successfully.", false);
                txtDrivenByName.Text = "";
                txtDescription.Text = "";
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding drivenby: {ex.Message}", true);
            }

        }
        protected void gvDrivenBy_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDrivenBy.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvDrivenBy_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvDrivenBy.EditIndex = e.NewEditIndex;
            BindGrid();
        }
        protected void gvDrivenBy_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvDrivenBy.EditIndex = -1;
            BindGrid();
            ShowMessage("Editing cancelled.", false);
        }


        protected void gvDrivenBy_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Int32 drivenId = Convert.ToInt32(gvDrivenBy.DataKeys[e.RowIndex].Values[0].ToString());
            try
            {
                DAL mdal = new DAL();
                GridViewRow row = gvDrivenBy.Rows[e.RowIndex];
                mdal.DeleteDrivenBy(drivenId, userId);
                BindGrid();
                ShowMessage("drivenby deleted successfully.", false);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting drivenby: {ex.Message}", true);
            }
        }

        protected void gvDrivenBy_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int drivenId = Convert.ToInt32(gvDrivenBy.DataKeys[rowIndex].Values["DrivenId"].ToString());
            string drivenName = ((TextBox)gvDrivenBy.Rows[e.RowIndex].FindControl("txtEditDrivenByName")).Text;
            string description = ((TextBox)gvDrivenBy.Rows[e.RowIndex].FindControl("txtEditDescription")).Text;
            if (string.IsNullOrEmpty(drivenName))
            {
                ShowMessage("Please enter a drivenby Name", true);
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
                dal.EditDeleteDrivenBy(drivenId, "update", userId, drivenName, true, description);
                gvDrivenBy.EditIndex = -1;
                ShowMessage("drivenby updated successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding drivenby: {ex.Message}", true);
            }
        }
    }
}