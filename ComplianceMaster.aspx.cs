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
    public partial class ComplianceMaster : Page
    {
        private int userId;
        private const string UploadDirectory = "~/Uploads/ComplianceDocuments/";
        // Maximum file size in bytes (10MB)
        private const long MaxFileSize = 1024 * 1024 * 10; // 10MB
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
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnUpload);
            if (!Page.IsPostBack)
            {
                BindDropdowns();
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
        // Helper method to truncate text to a specified number of words
        public string TruncateToWords(string text, int wordCount)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            var words = text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > wordCount)
            {
                return string.Join(" ", words.Take(wordCount)) + "…";
            }
            return text;
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
            btnUpload.Visible = true;
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
        protected void gvCompliances_RowCommand(object sender, GridViewCommandEventArgs e){}
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            string userName = Session["UserName"].ToString();
            string ComplianceId = hdnComplianceID.Text;
            int iComplianceId = Convert.ToInt32(hdnComplianceID.Text);
            string directoryPath = Server.MapPath("~/FilesRep/ComplianceDocuments/") + ComplianceId;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string docType = ddlDocType.SelectedValue;
            int docTypeId = Convert.ToInt32(ddlDocType.SelectedValue);
            if (FileUpload1.HasFiles && !string.IsNullOrEmpty(ddlDocType.SelectedValue))
            {
                foreach (HttpPostedFile uploadedFile in FileUpload1.PostedFiles)
                {
                    if (uploadedFile.ContentLength > MaxFileSize)
                    {
                        ShowMessage($"File {uploadedFile.FileName} exceeds the maximum size of 10MB.", true);
                        return;
                    }
                    string fileName = Path.GetFileName(uploadedFile.FileName);
                    string filePath = Path.Combine(directoryPath, fileName);
                    uploadedFile.SaveAs(filePath);
                    // Store the document in the database with DocType
                    SaveDocumentToDatabase(iComplianceId, fileName, filePath, docTypeId);
                }
                ShowMessage("Documents uploaded successfully.", false);
            }
            else
            {
                ShowMessage("Please select a document type and at least one file to upload.", true);
            }
            DisplayUploadedFiles(iComplianceId); //// Display uploaded files in a new GridView
        }
        private void SaveDocumentToDatabase(int complianceId, string fileName, string filePath, int docTypeId)
        {
            DAL dal = new DAL();
            dal.SaveDocumentToDatabase(complianceId, fileName, filePath, docTypeId);
            ShowMessage("Document saved successfully.", false);
        }
        private void BindDropdowns()
        {
            try
            {
                DAL dal = new DAL();
                BindDropdown(ddlBusinessUnit, dal.PopulateBusinessUnit("BusinessUnit"), "BusinessUnitName", "BusinessUnitID", "Select BusinessUnit");
                BindDropdown(ddlTypeOfCompliance, dal.PopulateComplianceType("ComplianceType"), "ComplianceTypeName", "ComplianceTypeID", "Select ComplianceType");
                BindDropdown(ddlDrivenBy, dal.PopulateDrivenBy("DrivenBy"), "DrivenName", "DrivenId", "Select Driven By");
                BindDropdown(ddlNatureOfCompliance, dal.PopulateComplianceNature("ComplianceNature"), "NatureOfCompliance", "ComplianceNatureID", "Select ComplianceNature");
                BindDropdown(ddlLawType, dal.PopulateLawType("LawType"), "LawName", "LawId", "LawType");
                BindDropdown(ddlFrequency, dal.PopulateFrequency("Frequency"), "FrequencyName", "FrequencyID", "Select Frequency");
                BindDropdown(ddlDepartmentFunction, dal.PopulateDepartment("Department"), "DepartmentName", "DeptID", "Select Department");
                //Creator roleid (1 = creator, 2= Approver/Supervisor, 3= normal user)
                BindDropdown(ddlInitiator, dal.PopulateCreator("Initiator"), "CreatorName", "CreatorID", "Select Initiator");
                BindDropdown(ddlApprover, dal.PopulateApprover("Approver"), "ApproverName", "ApproverID", "Select Approver");
                BindDropdown(ddlTerritory, dal.PopulateTerritory("Territory"), "TerritoryName", "TerritoryId", "Select Territory");
                BindDropdown(ddlPriority, dal.PopulatePriority("Priority"), "PriorityName", "PriorityId", "Select Priority");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading dropdowns: {ex.Message}", true);
            }
        }
        private void BindDropdown(DropDownList dropdown, object dataSource, string textField, string valueField, string defaultText)
        {
            dropdown.DataSource = dataSource;
            dropdown.DataTextField = textField;
            dropdown.DataValueField = valueField;
            dropdown.DataBind();
            dropdown.Items.Insert(0, new ListItem(defaultText, ""));
        }
        private void ShowMessage(string message, bool isError)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.ForeColor = isError ? System.Drawing.Color.FromArgb(0x72, 0x1C, 0x24) : System.Drawing.Color.Green;
            lblErrorMessage.Visible = true;
        }
        private void ClearForm()
        {
            txtComplianceArea.Text = ""; //clauseref
            ddlTypeOfCompliance.SelectedIndex = 0;
            txtActSectionRef.Text = "";
            ddlNatureOfCompliance.SelectedIndex = 0;
            txtEffectiveFromDate.Text = "";
            txtStandardDueDate.Text = "";
            txtFirstDueDate.Text = "";
            ddlFrequency.SelectedIndex = 0;
            ddlDepartmentFunction.SelectedIndex = 0;
            txtDetailsOfComplianceRequirements.Text = "";
            txtPenaltyForNonCompliance.Text = "";
            txtAction.Text = "";
            txtDueOnEvery.Text = "";
            ddlBusinessUnit.SelectedIndex = 0;
            ddlDrivenBy.SelectedIndex = 0;
            ddlLawType.SelectedIndex = 0;
            ddlInitiator.SelectedIndex = 0;
            ddlApprover.SelectedIndex = 0;
            ddlTerritory.SelectedIndex = 0;
            ddlPriority.SelectedIndex = 0;
            lblErrorMessage.Visible = false;
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCompliances.PageIndex = e.NewPageIndex;
            BindGrid();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Clear any previous error messages
            lblErrorMessage.Visible = false;
            DAL dal = new DAL();
            string actSectionRef = txtActSectionRef.Text.Trim();
            string clauseRef = txtComplianceArea.Text.Trim();
            if (string.IsNullOrEmpty(clauseRef))
            {
                ShowMessage("Clause Ref cannot be empty.", true);
                return;
            }
            if (ddlTypeOfCompliance.SelectedIndex == 0)
            {
                ShowMessage("Please select a Type of Compliance.", true);
                return;
            }
            if (ddlDepartmentFunction.SelectedIndex == 0)
            {
                ShowMessage("Please select a Department", true);
                return;
            }
            if (string.IsNullOrEmpty(txtEffectiveFromDate.Text))
            {
                ShowMessage("Please enter an Effective From Date.", true);
                return;
            }
            if (string.IsNullOrEmpty(txtStandardDueDate.Text))
            {
                ShowMessage("Please enter an Effective Till Date.", true);
                return;
            }
            string effectiveFromText = txtEffectiveFromDate.Text.Trim();
            string standardDueDateText = txtStandardDueDate.Text.Trim();
            string firstDueText = txtFirstDueDate.Text.Trim();
            if (ddlFrequency.SelectedIndex == 0)
            {
                ShowMessage("Please select a Frequency", true);
                return;
            }
            if (ddlBusinessUnit.SelectedIndex == 0)
            {
                ShowMessage("Please select a Business Unit", true);
                return;
            }
            if (ddlDrivenBy.SelectedIndex == 0)
            {
                ShowMessage("Please select Driven By", true);
                return;
            }
            if (ddlNatureOfCompliance.SelectedIndex == 0)
            {
                ShowMessage("Please select Nature of Compliance", true);
                return;
            }
            if (ddlLawType.SelectedIndex == 0)
            {
                ShowMessage("Please select Law Type", true);
                return;
            }
            if (ddlInitiator.SelectedIndex == 0)
            {
                ShowMessage("Please select a Initiator", true);
                return;
            }
            if (ddlApprover.SelectedIndex == 0)
            {
                ShowMessage("Please select a Approver", true);
                return;
            }
            if (ddlTerritory.SelectedIndex == 0)
            {
                ShowMessage("Please select a Territory", true);
                return;
            }
            if (ddlPriority.SelectedIndex == 0)
            {
                ShowMessage("Please select a Priority", true);
                return;
            }

            if (!Regex.IsMatch(txtDueOnEvery.Text, @"^\d+$"))
            {
                ShowMessage("Only numbers are allowed in Due On Every!", true);
                return;
            }
            int businussUnitId = Convert.ToInt32(ddlBusinessUnit.SelectedValue);
            int typeOfCompliance = Convert.ToInt32(ddlTypeOfCompliance.SelectedValue);
            int drivenById = Convert.ToInt32(ddlDrivenBy.SelectedValue);
            int natureOfCompliance = Convert.ToInt32(ddlNatureOfCompliance.SelectedValue);
            int lawTypeId = Convert.ToInt32(ddlLawType.SelectedValue);
            int territoryId = Convert.ToInt32(ddlTerritory.SelectedValue);
            int priorityId  = Convert.ToInt32(ddlPriority.SelectedValue);
            int frequency = Convert.ToInt32(ddlFrequency.SelectedValue);
            int departmentFunction = Convert.ToInt32(ddlDepartmentFunction.SelectedValue);
            int initiatorId = Convert.ToInt32(ddlInitiator.SelectedValue);
            int approverId = Convert.ToInt32(ddlApprover.SelectedValue);
            int dueOnEvery = Convert.ToInt32(txtDueOnEvery.Text.Trim());
            string detailsOfComplianceRequirements = txtDetailsOfComplianceRequirements.Text.Trim();
            string penalty = txtPenaltyForNonCompliance.Text.Trim();
            string actiontobetaken = txtAction.Text.Trim();
            
            ComplianceMasterr comp = new ComplianceMasterr();
            comp.ComplianceID = 0;
            comp.BusinessUnitID = businussUnitId;
            comp.ComplianceTypeID = typeOfCompliance;
            comp.DrivenById = drivenById;
            comp.ActSectionReference = actSectionRef;
            comp.ClauseRef = clauseRef;
            comp.ComplianceNatureID = natureOfCompliance;
            comp.LawId = lawTypeId;
            comp.TerritoryId = territoryId;
            comp.PriorityId = priorityId;
            comp.FrequencyID = frequency;
            comp.EffectiveFrom = Convert.ToDateTime(effectiveFromText);// effectiveFromDate;
            comp.StandardDueDate = Convert.ToDateTime(standardDueDateText);// standardDueDate;
            comp.FirstDueDate = Convert.ToDateTime(firstDueText);// firstDueDate;
            comp.DeptId = departmentFunction;
            comp.InitiatorId = initiatorId;
            comp.ApproverId = approverId;
            comp.DueOnEvery = dueOnEvery;
            comp.DetailsOfComplianceRequirements = detailsOfComplianceRequirements;
            comp.NonCompliancePenalty = txtPenaltyForNonCompliance.Text.Trim();
            comp.ActionsToBeTaken = actiontobetaken;
            comp.CreatedById = Convert.ToInt32(Session["UserId"].ToString());
            try
            {
                int newComplianceId = dal.SaveComplianceMaster(comp);//, "insert");
                dal.AssignComplianceToUser(newComplianceId); //update compliancedetails : Assign compliance to the user (Initiator)
                ShowMessage("Compliance record saved successfully. No documents uploaded.", false);
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error creating compliance record and documents: {ex.Message}", true);
            }
        }
        protected void gvCompliances_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCompliances.PageIndex = e.NewPageIndex;
            BindGrid();
        }
    }
}