using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
	public partial class InvalidComplianceMasster : System.Web.UI.Page
	{
        private int userId;
        private const string UploadDirectory = "~/Uploads/ComplianceDocuments/";
        // Maximum file size in bytes (10MB)
        private const long MaxFileSize = 1024 * 1024 * 10; // 10MB
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        private readonly List<string> allowedFileTypes = new List<string> { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt" }; // Allowed file extensions

        protected void Page_Load(object sender, EventArgs e)
		{
            userId = Convert.ToInt32(Session["UserId"]);
            if (Session["UserName"] == null)
            {
                Response.Redirect("../AuthPages/Login.aspx");
            }
            UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
            if (updatePanel != null)
            {
                string script = $"var updatePanelClientId = '{updatePanel.ClientID}';";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "setUpdatePanelClientId", script, true);
            }
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
            else
            {
                // Check for custom postback event
                string eventArgument = Request["__EVENTARGUMENT"];
                if (!string.IsNullOrEmpty(eventArgument) && eventArgument.StartsWith("Reload|"))
                {
                    string[] args = eventArgument.Split('|');
                    if (args.Length == 2 && args[0] == "Reload")
                    {
                        string complianceId = args[1];
                        DisplayUploadedFiles(Convert.ToInt32(complianceId));
                        if (updatePanel != null) updatePanel.Update();
                    }
                }
            }

        }
        private void BindGrid()
        {
            try
            {
                DAL dal = new DAL();
                List<ComplianceMasterr> compliances = new List<ComplianceMasterr>();
                compliances = dal.LoadComplianceMaster("All", 0);
                gvCompliances.DataSource = compliances;
                gvCompliances.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading compliance data: {ex.Message}", true);
            }
        }
        protected void Display(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showForgotModal", "showModal();", true);
            int rowIndex = ((sender as Button).NamingContainer as GridViewRow).RowIndex;
            int complianceID = Convert.ToInt32(gvCompliances.DataKeys[rowIndex].Values[0]);

            DAL dal = new DAL();
            ComplianceMasterr compliances = new ComplianceMasterr();
            compliances = dal.LoadComplianceModalDetails(complianceID);
            lblComplianceRef.Text = compliances.ComplianceRef;
            lblComplianceAreaValue.Text = compliances.NatureOfComplianceName;
            lblDetails.Text = compliances.DetailsOfComplianceRequirements;
            lblNonCompliance.Text = compliances.NonCompliancePenalty;
            lblActionToBeTaken.Text = compliances.ActionsToBeTaken;
            lblActSec.Text = compliances.ActSectionReference;
            lblPrio.Text = compliances.Priority;
            lblLawt.Text = compliances.LawName;
            lblDriv.Text = compliances.DrivenByName;
        }
        protected void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            lblDocUpload.Visible = true;
            lblFileUpload.Visible = true;
            FileUpload1.Visible = true;
            GridView2.Visible = true;
            GridView2.Enabled = true;
            ddlDocType.Visible = true;
            lblDocType.Visible = true;
            GridViewRow row = gvCompliances.SelectedRow;
            int selectedIndex = gvCompliances.SelectedIndex;
            string compID = (gvCompliances.DataKeys[selectedIndex]["ComplianceId"]).ToString();
            int complianceID = Convert.ToInt32(compID);

            DAL dal = new DAL();
            ddlDocType.DataSource = dal.PopulateDocType("DocType");
            ddlDocType.DataTextField = "DocumentTypeName";
            ddlDocType.DataValueField = "DocumentTypeId";
            ddlDocType.DataBind();
            ddlDocType.Items.Insert(0, new ListItem("Select Doc Type", ""));

            DisplayUploadedFiles(complianceID);
            hdnComplianceID.Text = complianceID.ToString();
            UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
            if (updatePanel != null) updatePanel.Update();
        }
        private void DisplayUploadedFiles(int ComplianceId)
        {
            string directoryPath = Server.MapPath("~/FilesRep/ComplianceDocuments/" + ComplianceId);
            DAL dal = new DAL();
            GridView2.DataSource = dal.LoadComplianceDocuments(ComplianceId, directoryPath);
            GridView2.DataBind();
        }
        protected void gvCompliances_RowCommand(object sender, GridViewCommandEventArgs e) { }
        
        private void ShowMessage(string message, bool isError)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.ForeColor = isError ? System.Drawing.Color.FromArgb(0x72, 0x1C, 0x24) : System.Drawing.Color.Green;
            lblErrorMessage.Visible = true;
        }
        
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCompliances.PageIndex = e.NewPageIndex;
            BindGrid();
        }
       
        protected void gvCompliances_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCompliances.PageIndex = e.NewPageIndex;
            BindGrid();
        }
    }
}