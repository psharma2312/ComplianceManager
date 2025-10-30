using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class Frequency1 : System.Web.UI.Page
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
                var frequency = dal.GetAllFrequency();
                gvFrequencies.DataSource = frequency;
                gvFrequencies.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading frequencies: {ex.Message}", true);
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
            string frequencyName = txtFrequencyName.Text.Trim();
            string descriptionText = txtDescription.Text.Trim();
            if (string.IsNullOrEmpty(frequencyName))
            {
                ShowMessage("Please enter a frequency name.", true);
                return;
            }
            if (string.IsNullOrEmpty(descriptionText))
            {
                ShowMessage("Please enter a valid Description.", true);
                return;
            }
            try
            {
                DAL dal = new DAL();
                dal.EditDeleteFrequency(0, "add", userId, frequencyName, true, descriptionText);
                ShowMessage("Frequency added successfully.", false);
                txtFrequencyName.Text = "";
                txtDescription.Text = "";
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding frequency: {ex.Message}", true);
            }
        }
        protected void gvFrequencies_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvFrequencies.EditIndex = e.NewEditIndex;
            BindGrid();
            //// In a real app, you'd implement full edit functionality (e.g., update database)
            //string frequencyName = ((TextBox)gvFrequencies.Rows[e.NewEditIndex].Cells[0].Controls[0]).Text;
            //txtFrequencyName.Text = frequencyName;
            //ShowMessage("Edit the frequency name in the textbox and click Add to update.", false);
        }
        protected void gvFrequencies_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFrequencies.PageIndex = e.NewPageIndex;
            BindGrid();

        }
        protected void gvFrequencies_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvFrequencies.EditIndex = -1;
            BindGrid();
            ShowMessage("Editing cancelled.", false);
        }
        protected void gvFrequencies_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int frequencyId = Convert.ToInt32(gvFrequencies.DataKeys[e.RowIndex].Value);
            try
            {
                DAL mdal = new DAL();
                GridViewRow row = gvFrequencies.Rows[e.RowIndex];
                mdal.DeleteFrequency(frequencyId, userId);
                ShowMessage("Frequency deleted successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting frequency: {ex.Message}", true);
            }
        }

        protected void gvFrequencies_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int frequencyId = Convert.ToInt32(gvFrequencies.DataKeys[e.RowIndex].Value);
            string frequencyName = ((TextBox)gvFrequencies.Rows[e.RowIndex].FindControl("txtEditFrequencyName")).Text;
            string descriptionText = ((TextBox)gvFrequencies.Rows[e.RowIndex].FindControl("txtEditDescription")).Text.Trim();
            if (string.IsNullOrEmpty(frequencyName))
            {
                ShowMessage("Frequency name cannot be empty.", true);
                return;
            }
            if (string.IsNullOrEmpty(descriptionText))
            {
                ShowMessage("Enter a valid description", true);
                return;
            }
            try
            {
                DAL dal = new DAL();
                dal.EditDeleteFrequency(frequencyId, "update", userId, frequencyName, true, descriptionText);
                gvFrequencies.EditIndex = -1;
                ShowMessage("Frequency updated successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding frequency: {ex.Message}", true);
            }
        }
    }
}