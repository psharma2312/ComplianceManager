using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace ComplianceManager
{
    public partial class MyCompliances : System.Web.UI.Page
    {
        int userId = 0;
        char IsAdmin = 'N';
        private readonly string _smtpHost = ConfigurationManager.AppSettings["SmtpServer"];
        private readonly int _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        private readonly string _smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
        private readonly string _smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
        private readonly string _fromEmail = ConfigurationManager.AppSettings["FromEmail"];

        protected void Page_Load(object sender, EventArgs e)
        {
            userId = Convert.ToInt32(Session["UserId"].ToString());
            if (Session["UserName"] == null)
            {
                Response.Redirect("../AuthPages/Login.aspx");
            }
            else
            {
                if (Session["UserName"].ToString().ToLower() == "dgcadmin")
                {
                    IsAdmin = 'Y';
                }
                
            }
                ScriptManager.GetCurrent(this).RegisterPostBackControl(btnUpload);

            UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
            if (updatePanel != null)
            {
                string script = $"var updatePanelClientId = '{updatePanel.ClientID}';";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "setUpdatePanelClientId", script, true);
            }
            if (!Page.IsPostBack)
            {
                BindStatusDropdown();
                refreshdata(userId, 0,0,"0",IsAdmin); //StatusId =0 Get all the records irrespective of the status
                //if (lblStatus.Text == "")
                //    lblStatus.Text = "1";
            }
            else
            {
                // Check for custom postback event
                string eventArgument = Request["__EVENTARGUMENT"];
                if (!string.IsNullOrEmpty(eventArgument) && eventArgument.StartsWith("Reload|"))
                {
                    string[] args = eventArgument.Split('|');
                    if (args.Length == 3 && args[0] == "Reload")
                    {
                        string complianceDetailId = args[1];
                        string complianceId = args[2];
                        DisplayUploadedFiles(complianceDetailId, "", Convert.ToInt32(complianceId)); 
                        if (updatePanel != null) updatePanel.Update();
                    }
                }
            }
        }
        private void BindStatusDropdown()
        {
            ddlStatus.DataSource = PopulateStatus("Status");
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, new ListItem("All", "0"));
            ddlStatus.SelectedIndex = 0;

            for (int i = 1; i <= 12; i++)
            {
                ddlMonth.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            ddlMonth.Items.Insert(0, new ListItem("All", "0"));
            ddlMonth.SelectedIndex = 0;

            int currentYear = DateTime.Now.Year;
           
            for (int i = 1; i >= -2; i--) //descending order
            {
                int fyEndYear = currentYear + i;
                int fyStartYear = fyEndYear - 1;
                string displayText = $"{fyStartYear}-{fyEndYear.ToString().Substring(2)}"; //for UI
                string value = $"{fyStartYear.ToString().Substring(2)}-{fyEndYear.ToString().Substring(2)}"; // for SP
                ddlYear.Items.Add(new ListItem(displayText, value));
                //ddlYear.Items.Add(new ListItem(displayText, fyEndYear.ToString()));

            }
            ddlYear.Items.Insert(0, new ListItem("All", "0"));
            ddlYear.SelectedIndex = 0;
        }
        public List<ComplianceStatuss> PopulateStatus(string dtaType)
        {
            List<ComplianceStatuss> data = new List<ComplianceStatuss>();
            string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = "Compliance.GetDropDownValues";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@MenuType", SqlDbType.VarChar).Value = dtaType;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            data.Add(new ComplianceStatuss
                            {
                                ComplianceStatusName = sdr["Code"].ToString(),
                                ComplianceStatusID = Convert.ToInt32(sdr["ID"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return data;
        }

        // Method to truncate details
        protected string GetTruncatedDetails(object details)
        {
            if (details == null) return "";
            string detailsStr = details.ToString();
            if (string.IsNullOrEmpty(detailsStr)) return "";
            var words = detailsStr.Split(' ');
            return string.Join(" ", words.Take(20)) + "...";
        }
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblErrorMessage.Text))
            {
                lblErrorMessage.Visible = false;
            }
            // Store filter parameters in ViewState
            ViewState["CompStatusId"] = Convert.ToInt32(ddlStatus.SelectedItem.Value);
            ViewState["Month"] = Convert.ToInt32(ddlMonth.SelectedItem.Value);
            ViewState["Year"] = ddlYear.SelectedItem.Value;
           
            //lblStatus.Text = ddlStatus.SelectedItem.Value;
            refreshdata(userId, Convert.ToInt32(ddlStatus.SelectedItem.Value), 
                                Convert.ToInt32(ddlMonth.SelectedItem.Value), 
                               ddlYear.SelectedItem.Value, IsAdmin);

            UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
            if (updatePanel != null) updatePanel.Update();
        }
        protected void Display(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showForgotModal", "showModal();", true);
            int rowIndex = ((sender as LinkButton).NamingContainer as GridViewRow).RowIndex;
            int complianceID = Convert.ToInt32(GridView1.DataKeys[rowIndex].Values[0]);
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
        protected void btnupdate_Click(object sender, EventArgs e)
        {
            DAL dal = new DAL();
            if (txtNewComments.Text == "")
            {
                ShowMessage("Please provide some comments", true);
            }
            else
            {
                dal.UpdateUserComments(Convert.ToInt32(txtDetailsComplianceID.Text), Convert.ToInt32(txtDetailsComplianceDetailID.Text), txtNewComments.Text, userId);
                ShowMessage("Comments updated successfully....", false);
                txtNewComments.Text = "";
                UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
                if (updatePanel != null) updatePanel.Update();
            }
        }

        protected void btnSubmitForApproval_Click(object sender, EventArgs e)
        {
            DAL dal = new DAL();
            if (txtNewComments.Text == "")
            {
                ShowMessage("Please provide some comments", true);
            }
            else
            {
                dal.SendForApproval(Convert.ToInt32(txtDetailsComplianceID.Text), Convert.ToInt32(txtDetailsComplianceDetailID.Text), txtNewComments.Text);
                ShowMessage("Sent for Approval successfully....", false);



                //Send Email to Approver
                string userName = Session["UserName"]?.ToString() ?? "Unknown User";
                string complianceId = txtDetailsComplianceDetailID.Text;
                string complianceRef = txtComplianceRef.Text;
                string toEmail = txtEmail.Text; // Approver's email from the form
                string subject = "New Compliance Submitted for Approval";
                string body = $"A new compliance (ID: {complianceId} , Ref: {complianceRef}) has been submitted for your approval by {userName}.";
                SendEmail(toEmail, subject, body);

                txtNewComments.Text = "";
                UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
                if (updatePanel != null) updatePanel.Update();
            }
        }
        private void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_smtpHost, _smtpPort))
                {
                    client.Credentials = new System.Net.NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = true;

                    using (var message = new MailMessage(_fromEmail, toEmail, subject, body))
                    {
                        message.IsBodyHtml = false;
                        client.Send(message);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void refreshdata(int userId, int statusId, int month, string year, char IsAdmin)
        {
            DAL dal = new DAL();
            GridView1.DataSource = dal.LoadMyCompliance(userId, statusId, month, year, IsAdmin);
            GridView1.DataBind();
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == GridView1.SelectedIndex)
                {
                    e.Row.CssClass += " selected-row";
                }
                else
                {
                    e.Row.CssClass = e.Row.CssClass.Replace("selected-row", "").Trim();
                }
            }
        }
        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Disable LinkButtons inside GridView based on compStatusId
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int compStatusId = Convert.ToInt32(ViewState["CompStatusId"]);
                // Optional: Ensure no row is highlighted unless newly selected
                if (e.Row.RowIndex == GridView1.SelectedIndex)
                {
                    e.Row.CssClass += " selected-row"; // Apply highlight
                }
                else
                {
                    e.Row.CssClass = e.Row.CssClass.Replace("selected-row", "").Trim(); // Remove highlight
                }

                
                // Get the LinkButtons inside GridView
                LinkButton lnkDownload = (LinkButton)e.Row.FindControl("lnkDownload");
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                // Disable the button if compStatusId is 2 : Approved , 4 :Pending-Approval and 5: Complete
                bool enableLinks = !(compStatusId == 2 || compStatusId == 4 || compStatusId == 5);
                if (lnkDownload != null) lnkDownload.Enabled = enableLinks;
                if (lnkDelete != null) lnkDelete.Enabled = enableLinks;
            }
        }
        protected void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ClearControls(); // Reset all controls first
            if (GridView1.SelectedIndex >= 0)
            {
                lblDocUpload.Visible = true;
                lblUploadedDocuments.Visible = true;
                lblUploadedDocuments1.Visible = true;
                GridViewRow row = GridView1.SelectedRow;
                int selectedIndex = GridView1.SelectedIndex;

                string compDetailID = (GridView1.DataKeys[selectedIndex]["ComplianceDetailID"]).ToString();
                string compID = (GridView1.DataKeys[selectedIndex]["ComplianceID"]).ToString();
                int complianceID = Convert.ToInt32(compID);
                int complianceDetailID = Convert.ToInt32(compDetailID);

                DAL dal = new DAL();
                ComplianceMasterDetailCombined compApproval = dal.LoadMyComplianceDetails(userId, complianceDetailID, complianceID);

                txtDetailsComplianceID.Text = compApproval.ComplianceID.ToString();
                txtDetailsComplianceDetailID.Text = compApproval.ComplianceDetailID.ToString();
                txtEmail.Text = compApproval.ApproverEmail.ToString();
                txtComplianceRef.Text = compApproval.ComplianceRef.ToString();
                string[] UserCommentArray = compApproval.UserComments.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                compApproval.UserComments = string.Join(Environment.NewLine, UserCommentArray);
                txtUpdatedComments.Text = compApproval.UserComments;
                txtUpdatedComments.Visible = true;

                EnableDisableControls(compApproval.ComplianceStatusId);
                //ViewState["CompStatusId"] = compApproval.ComplianceStatusId;


                //If Status is Approved or Rejected then show Approver comments box also.
                string[] ApproverCommentArray = compApproval.ApproverComments.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                compApproval.ApproverComments = string.Join(Environment.NewLine, ApproverCommentArray);
                txtApproverComments.Text = compApproval.ApproverComments;
                txtApproverComments.Visible = true;
                DisplayUploadedFilesMaster(complianceID);
                DisplayUploadedFiles(txtDetailsComplianceDetailID.Text, "", complianceID);
            }
            else
            {
                ClearControls(); // Clear controls if no row is selected
            }
        }
        private void DisplayUploadedFilesMaster(int ComplianceId)
        {
            string directoryPath = Server.MapPath("~/FilesRep/ComplianceDocuments/" + ComplianceId);
            DAL dal = new DAL();
            List<ComplianceDocuments> documents = dal.LoadComplianceDocuments(ComplianceId, directoryPath) ?? new List<ComplianceDocuments>();
            if (documents.Any())
            {
                // Bind directly to the list (GridView can handle IEnumerable)
                GridView3.DataSource = documents;
                GridView3.DataBind();
                GridView3.Visible = true;
            }
            else
            {
                // Create an empty DataTable with the same structure as GridView3 columns
                DataTable dt = new DataTable();
                dt.Columns.Add("DocumentID", typeof(int));
                dt.Columns.Add("DocTypeName", typeof(string));
                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("UploadedDate", typeof(DateTime));
                dt.Columns.Add("DownloadPath", typeof(string));

                GridView3.DataSource = dt;
                GridView3.DataBind();
                GridView3.Visible = true;
            }
        }
        private void EnableDisableControls(int compStatusId)
        {
            if (string.IsNullOrEmpty(lblErrorMessage.Text))
            {
                lblErrorMessage.Visible = false;
            }
            lblFileUpload.Visible = true;
            FileUpload1.Visible = true;
            btnUpload.Visible = true;
            lblUpdatedComments.Visible = true;
            txtUpdatedComments.Visible = true;
            lblApproverComments.Visible = true;
            txtApproverComments.Visible = true;
            btnupdate.Visible = true;
            btnSubmitForApproval.Visible = true;
            txtNewComments.Visible = true;
            lblNewUserComments.Visible = true;
            btnupdate.Visible = true;
            btnSubmitForApproval.Visible = true;
            // Disable the button if compStatusId is 2 : Approved , 4 :Pending-Approval and 5: Complete
            btnUpload.Enabled = !(compStatusId == 2 || compStatusId == 4 || compStatusId == 5);
            btnupdate.Enabled = btnUpload.Enabled;
            btnSubmitForApproval.Enabled = btnUpload.Enabled;
            GridView2.Enabled = btnUpload.Enabled;
            UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
            if (updatePanel != null) updatePanel.Update();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Visible = false;
            string userName = Session["UserName"]?.ToString() ?? string.Empty;
            string complianceId = txtDetailsComplianceID.Text;
            string complianceDetailId = txtDetailsComplianceDetailID.Text;

            string directoryName = $"{userName}_{complianceDetailId}";
            string directoryPath = Server.MapPath("~/FilesRep/Details/" + directoryName);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            foreach (HttpPostedFile file in FileUpload1.PostedFiles)
            {
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(directoryPath, fileName);
                file.SaveAs(filePath);
            }
            int compDetailId = Convert.ToInt32(complianceDetailId);
            SaveComplianceDetailFilePath("~/FilesRep/Details/" + directoryName, compDetailId); //18jan25 : Not sending dirPath as it will show complete server path, we can merge the path at runtime as the upload directory is fixed we only need directory name where the files are beign uploaded for a user.

            DisplayUploadedFiles(complianceDetailId, directoryPath="", Convert.ToInt32(complianceId));
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.SelectedIndex=-1;
            GridView1.PageIndex = e.NewPageIndex;
            ClearControls(); // Clear controls on page change
           
            int compStatusId = ViewState["CompStatusId"] != null ? Convert.ToInt32(ViewState["CompStatusId"]) : 0;// Convert.ToInt32(ViewState["CompStatusId"]);
            int month = ViewState["Month"] != null ? Convert.ToInt32(ViewState["Month"]) : 0; //Convert.ToInt32(ViewState["Month"]);
            
            string year = ViewState["Year"] != null ? ViewState["Year"].ToString() : "0"; // Default to "0" (All)

            if (compStatusId < 0) compStatusId = 0;
            if (month < 0 || month > 12) month = 0;
            if (string.IsNullOrWhiteSpace(year)) year = "";

            refreshdata(userId, compStatusId, month, year, IsAdmin);
        }
        private void ClearControls()
        {
            lblDocUpload.Visible = false;
            lblUploadedDocuments.Visible = false;
            lblUpdatedComments.Visible = false;
            lblUploadedDocuments1.Visible = false;
            lblApproverComments.Visible = false;
            lblFileUpload.Visible = false;
            lblNewUserComments.Visible = false;
            txtDetailsComplianceID.Text = string.Empty;
            txtDetailsComplianceDetailID.Text = string.Empty;
            txtUpdatedComments.Text = string.Empty;
            txtUpdatedComments.Visible = false;
            txtApproverComments.Text = string.Empty;
            txtApproverComments.Visible = false;
            txtNewComments.Text = string.Empty;
            txtNewComments.Visible = false;
            // Clear other controls if needed (e.g., GridView2, GridView3 data)
            GridView2.DataSource = null;
            GridView2.DataBind();
            GridView2.Visible = false;
            GridView3.DataSource = null;
            GridView3.DataBind();
            GridView3.Visible = false;
            FileUpload1.Visible = false;
            btnUpload.Visible = false;

            btnupdate.Visible = false;
            btnSubmitForApproval.Visible = false;

        }
        private void SaveComplianceDetailFilePath(string directoryName, int complianceDetailId)
        {
            DAL dal = new DAL();
            dal.SaveComplianceDetailFilePath(directoryName, userId, complianceDetailId);
        }
        private void DisplayUploadedFiles(string ComplianceDetailId, string directoryPath, int complianceID)
        {
            string userName = Session["UserName"].ToString();
            string complianceDetailId = txtDetailsComplianceDetailID.Text;
            string directoryName = $"{userName}_{complianceDetailId}";
            string dirtoryPath = Server.MapPath("~/FilesRep/Details/" + directoryName);
            if (Directory.Exists(dirtoryPath))
            {
                string[] fileNames = Directory.GetFiles(dirtoryPath);
                var files = fileNames.Select(fileName => new
                {
                    ID = ComplianceDetailId,
                    FullPath = dirtoryPath,
                    ComplianceId = complianceID,
                    FileName = Path.GetFileName(fileName),
                    DownloadPath = $"~/FilesRep/Details/{directoryName}/{Path.GetFileName(fileName)}"
                }).ToList();

                GridView2.DataSource = files;

                GridView2.DataBind();
                GridView2.Visible = true;
            }
            else
            {
                // Create an empty DataTable with the same structure as the GridView columns
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(string));
                dt.Columns.Add("ComplianceId", typeof(int));
                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("DownloadPath", typeof(string)); // Match the anonymous type properties

                GridView2.DataSource = dt;
                GridView2.DataBind();
                GridView2.Visible = true;
            }
}
        
        private void ShowMessage(string message, bool isError)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.ForeColor = isError ? System.Drawing.Color.FromArgb(0x72, 0x1C, 0x24) : System.Drawing.Color.Green;
            lblErrorMessage.Visible = true;
        }
    }
}