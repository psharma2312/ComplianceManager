using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class LawTypes : System.Web.UI.Page
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
                var lawType = dal.GetAllLawTypes();
                gvLawType.DataSource = lawType;
                gvLawType.DataBind();
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

            string LawName = txtLawTypeName.Text.Trim();
            string description = txtDescription.Text.Trim();
            if (string.IsNullOrEmpty(LawName))
            {
                ShowMessage("Please enter a Law Type.", true);
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
                dal.EditDeleteLawType(0, "add", userId, LawName, true, description);
                ShowMessage("Law Type added successfully.", false);
                txtLawTypeName.Text = "";
                txtDescription.Text = "";
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding Law Type: {ex.Message}", true);
            }

        }
        protected void gvLawType_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLawType.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvLawType_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvLawType.EditIndex = e.NewEditIndex;
            BindGrid();
        }
        protected void gvLawType_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvLawType.EditIndex = -1;
            BindGrid();
            ShowMessage("Editing cancelled.", false);
        }


        protected void gvLawType_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Int32 LawId = Convert.ToInt32(gvLawType.DataKeys[e.RowIndex].Values[0].ToString());
            try
            {
                DAL mdal = new DAL();
                GridViewRow row = gvLawType.Rows[e.RowIndex];
                mdal.DeleteLawType(LawId, userId);
                BindGrid();
                ShowMessage("Law Type deleted successfully.", false);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting Law Type: {ex.Message}", true);
            }
        }

        protected void gvLawType_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int rowIndex = e.RowIndex;
            int lawId = Convert.ToInt32(gvLawType.DataKeys[rowIndex].Values["LawId"].ToString());
            string lawName = ((TextBox)gvLawType.Rows[e.RowIndex].FindControl("txtEditLawTypeName")).Text;
            string description = ((TextBox)gvLawType.Rows[e.RowIndex].FindControl("txtEditDescription")).Text;
            if (string.IsNullOrEmpty(lawName))
            {
                ShowMessage("Please enter a LawType Name", true);
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
                dal.EditDeleteLawType(lawId, "update", userId, lawName, true, description);
                gvLawType.EditIndex = -1;
                ShowMessage("Law Type updated successfully.", false);
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding Law Type: {ex.Message}", true);
            }
        }
    }
}