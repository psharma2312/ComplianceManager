using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class Approval : Page
    {
        int userId = 0;
        int currentComplianceId = 0;
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
            // Register btnUpload for full postback to fix FileUpload issue with UpdatePanel
            UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent"); // Adjust ID
            if (updatePanel != null)
            {
                string script = $"var updatePanelClientId = '{updatePanel.ClientID}';";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "setUpdatePanelClientId", script, true);
            }
            if (!Page.IsPostBack)
            {
                BindStatusDropdown();
                refreshdata(userId, Convert.ToInt32(ddlStatus.SelectedItem.Value));
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
                        DisplayUploadedFiles(complianceDetailId, "", Convert.ToInt32(complianceId), "assignedToName"); // Rebind GridView2
                    }
                }
            }
        }

        private void BindStatusDropdown()
        {
            ddlStatus.DataSource = PopulateStatus("ApproverStatus");
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, new ListItem("All", "0"));
            ddlStatus.SelectedIndex = 3;
        }
        public List<ComplianceStatuss> PopulateStatus(string dtaType)
        {
            try
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
            finally
            {

            }
        }
        protected void btnLoad_Click(object sender, EventArgs e)
        {

            lblErrorMessage.Visible = false;
            refreshdata(userId, Convert.ToInt32(ddlStatus.SelectedItem.Value));
            UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
            if (updatePanel != null) updatePanel.Update();
        }
        public void refreshdata(int approverId, int statusId=0)
        {
            DAL dal = new DAL();
            var compApproval = dal.LoadComplianceForApproval(userId, statusId);
            GridView1.DataSource = compApproval;
            GridView1.DataBind();
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            refreshdata(userId, 0);
        }
        protected void GridView2_RowCommand(object sender, GridViewCommandEventArgs e)
        {}
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
        protected void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = GridView1.SelectedRow;
            int selectedIndex = GridView1.SelectedIndex;
            //lblCompReqDetails.Visible = true;
            lblErrorMessage.Visible = false;
            lblDocUpload.Visible = true;
            lblUploadedDocuments.Visible = true;
            lblUploadedDocuments1.Visible = true;
            //txtDetails.Visible = true;
            lblUserComments.Visible = true;
            txtUserComments.Visible = true;
            lblUpdatedComments.Visible = true;
            txtUpdatedComments.Visible = true;
            txtNewComments.Visible = true;
            lblNewUserComments.Visible = true;
            btnApprove.Visible = true;
            btnReject.Visible = true;
            string customID = (GridView1.DataKeys[selectedIndex]["ComplianceDetailID"]).ToString();
            string compID = (GridView1.DataKeys[selectedIndex]["ComplianceID"]).ToString();
            int complianceID = Convert.ToInt32(compID);
            int complianceDetailID = Convert.ToInt32(customID);
            int approverid = userId;
            currentComplianceId = complianceID;
            DAL dal = new DAL();
            ComplianceMasterDetailCombined objcomApproval = new ComplianceMasterDetailCombined();
            var compApproval = dal.LoadComplianceDetailsForApproval(approverid, complianceDetailID, complianceID);

            txtDetailsComplianceID.Text = compApproval.ComplianceID.ToString();
            txtDetailsComplianceDetailID.Text = compApproval.ComplianceDetailID.ToString();
            txtEmail.Text = compApproval.UserEmail.ToString();
            txtComplianceRef.Text = compApproval.ComplianceRef.ToString();
            
            Int32 compStatusId = compApproval.ComplianceStatusId;
            // Split comments by delimiter and join them with new line
            string[] UserCommentArray = compApproval.UserComments.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
            compApproval.UserComments = string.Join(Environment.NewLine, UserCommentArray);
            txtUserComments.Text = compApproval.UserComments;

            // Split comments by delimiter and join them with new line
            string[] ApproverCommentArray = compApproval.ApproverComments.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
            compApproval.ApproverComments = string.Join(Environment.NewLine, ApproverCommentArray);
            txtUpdatedComments.Text = compApproval.ApproverComments;

            btnApprove.Visible = true;
            btnReject.Visible = true;
            // Disable the button if compStatusId is 2 :Approved , 3 : Rejected, or 5 : Complete
            btnApprove.Enabled = !(compStatusId == 2 || compStatusId == 3 || compStatusId == 5);
            btnReject.Enabled = btnApprove.Enabled;
            txtNewComments.Enabled = btnApprove.Enabled;
            DisplayUploadedFilesMaster(complianceID);
            DisplayUploadedFiles(txtDetailsComplianceDetailID.Text, "", currentComplianceId, compApproval.AssignedToName);
        }
        private void DisplayUploadedFilesMaster(int ComplianceId)
        {
            string directoryPath = Server.MapPath("~/FilesRep/ComplianceDocuments/" + ComplianceId);
            DAL dal = new DAL();
            GridView3.DataSource = dal.LoadComplianceDocuments(ComplianceId, directoryPath);
            GridView3.DataBind();
        }
        private void DisplayUploadedFiles(string ComplianceDetailId, string directoryPath, int complianceID, string assignedToName)
        {
            //lblUploadedDocuments.Visible = true;
            string userName = Session["UserName"].ToString();
            string complianceDetailId = txtDetailsComplianceDetailID.Text;
            string dirPath = directoryPath;
            //Get DirectoryPath from database, to check if path exists otherwise directoryPath will come from calling method
            if (directoryPath == "")
                dirPath = ComplianceDetailId;
            string directoryName = $"{assignedToName}_{complianceDetailId}";
            string dirtoryPath = Server.MapPath("~/FilesRep/Details/" + directoryName);
            //string dirtoryPath = Server.MapPath("~/FilesRep/ComplianceDetailDocuments/" + dirPath);
            if (Directory.Exists(dirtoryPath))
            {
                string[] fileNames = Directory.GetFiles(dirtoryPath);
                GridView2.DataSource = fileNames.Select(fileName => new 
                { 
                    ID = ComplianceDetailId,
                    FullPath = dirtoryPath,
                    ComplianceId = complianceID,  
                    FileName = Path.GetFileName(fileName),
                    // Convert the full physical path back to a server - relative path
                    DownloadPath = $"~/FilesRep/Details/{directoryName}/{Path.GetFileName(fileName)}"
                });
                GridView2.DataBind();
            }
            else
            {
                GridView2.Visible = true;
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNewComments.Text.Length > 0)
                {
                    DAL dal = new DAL();
                    dal.UpdateApproverComments(Convert.ToInt32(txtDetailsComplianceID.Text), Convert.ToInt32(txtDetailsComplianceDetailID.Text), txtNewComments.Text, userId, "Approved");
                    ShowMessage("Compliance Approved successfully....", false);

                    //Send email to user
                    string userName = Session["UserName"]?.ToString() ?? "Unknown User";
                    string complianceId = txtDetailsComplianceDetailID.Text;
                    string complianceRef = txtComplianceRef.Text; 
                    string toEmail = txtEmail.Text; // User's email from the form
                    string subject = "Your Compliance is Approved";
                    string body = $"A new compliance (ID: {complianceId} , Ref: {complianceRef}) has been approved by {userName}.";
                    SendEmail(toEmail, subject, body);

                    btnApprove.Enabled = false;
                    btnReject.Enabled = false;
                    UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
                    if (updatePanel != null) updatePanel.Update();
                }
                else
                {
                    ShowMessage("Please provide Comments before Approving", true);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
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
        // Method to truncate details
        protected string GetTruncatedDetails(object details)
        {
            if (details == null) return "";
            string detailsStr = details.ToString();
            if (string.IsNullOrEmpty(detailsStr)) return "";
            var words = detailsStr.Split(' ');
            return string.Join(" ", words.Take(20)) + "...";
        }
        protected void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNewComments.Text.Length > 0)
                {
                    DAL dal = new DAL();
                    dal.UpdateApproverComments(Convert.ToInt32(txtDetailsComplianceID.Text), Convert.ToInt32(txtDetailsComplianceDetailID.Text), txtNewComments.Text, userId, "Rejected");
                    ShowMessage("Compliance Rejected Successfully....", false);
                    //Send email to user
                    string userName = Session["UserName"]?.ToString() ?? "Unknown User";
                    string complianceId = txtDetailsComplianceDetailID.Text;
                    string complianceRef = txtComplianceRef.Text;
                    string toEmail = txtEmail.Text; // User's email from the form
                    string subject = "Your Compliance is Rejected";
                    string body = $"A  compliance (ID: {complianceId} , Ref: {complianceRef}) has been rejected by {userName}.";
                    SendEmail(toEmail, subject, body);
                    btnApprove.Enabled = false;
                    btnReject.Enabled = false;
                    UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
                    if (updatePanel != null) updatePanel.Update();
                }
                else
                {
                    ShowMessage("Please provide Comments before Rejecting", true);
                    
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
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