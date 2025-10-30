using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class MyTeam : System.Web.UI.Page
    {
        int userId = 0;
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        protected void Page_Load(object sender, EventArgs e)
        {
            userId = Convert.ToInt32(Session["UserId"].ToString());
            if (Session["UserName"] == null)
            {
                Response.Redirect("../AuthPages/Login.aspx");
            }
            if (!Page.IsPostBack)
            {
                BindTeamDropdown(userId);
                refreshdata(userId, 0); //superviseeid=0
                if (lblStatus.Text == "")
                    lblStatus.Text = "1";
            }
        }
        private void BindTeamDropdown(int userId)
        {
            ddlStatus.DataSource = PopulateTeam(userId);
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, new ListItem("Please Select", String.Empty));
            ddlStatus.SelectedIndex = 0;
        }
        public List<User> PopulateTeam(int userId)
        {
            try
            {
                List<User> data = new List<User>();
                string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "Compliance.GetTeamBySupervisor";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                data.Add(new User
                                {
                                    UserId = Convert.ToInt32(sdr["user_id"]),
                                    UserName = sdr["user_name"].ToString()
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
            labelError.Visible = false;
            if (Convert.ToInt32(ddlStatus.SelectedItem.Value) > 0)
            {
                lblStatus.Text = ddlStatus.SelectedItem.Value;
                refreshdata(userId, Convert.ToInt32(ddlStatus.SelectedItem.Value));//, 0, 0);
                // btnupdate.Visible = true;
            }
        }
        protected void btnSubmitForApproval_Click(object sender, EventArgs e)
        {
            try
            {
                DAL dal = new DAL();
                //dal.SendForApproval(Convert.ToInt32(txtDetailsComplianceID.Text), Convert.ToInt32(txtDetailsComplianceDetailID.Text));
                labelError.Visible = true;
                labelError.Style.Add(HtmlTextWriterStyle.Color, "Blue");
                labelError.Text = "Sent for Approval successfully....";
            }

            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
            }
        }

        public void refreshdata(int SupervisorId, int superviseeId)
        {
            try
            {
                con.Open();
                DAL dal = new DAL();
                GridView1.DataSource = dal.LoadMyTeamCompliance(SupervisorId, superviseeId);//,complianceDetailID, complianceid);
                GridView1.DataBind();
                con.Close();
            }
            finally
            {
                con.Close();
            }

        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        { }

        protected void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected row index
            //int selectedRowIndex = GridView1.SelectedIndex;

            //// Fetch the DataKey value (ComplianceID)
            //if (selectedRowIndex >= 0)
            //{
            //    int complianceID2 = Convert.ToInt32(GridView1.DataKeys[selectedRowIndex].Value);

            //    // Display or use the DataKey value
            //    Response.Write("Selected ComplianceID: " + complianceID2);
            //}

            GridViewRow row = GridView1.SelectedRow;
            int selectedIndex = GridView1.SelectedIndex;
            lblCompReqDetails.Visible = true;
            labelError.Visible = false;
            txtDetails.Visible = true;
            lblFileUpload.Visible = true;
            FileUpload1.Visible = true;
            btnUpload.Visible = true;
            lblUpdatedComments.Visible = true;
            txtUpdatedComments.Visible = true;
            btnupdate.Visible = true;
            btnSubmitForApproval.Visible = true;
            txtNewComments.Visible = true;
            lblNewUserComments.Visible = true;




            string compDetailID = (GridView1.DataKeys[selectedIndex]["ComplianceDetailID"]).ToString();
            string compID = (GridView1.DataKeys[selectedIndex]["ComplianceID"]).ToString();
            int complianceID = Convert.ToInt32(compID);
            int intforumid = Convert.ToInt32(GridView1.DataKeys[row.RowIndex].Values[0]);
            int complianceDetailID = Convert.ToInt32(compDetailID);
            int statusID = Convert.ToInt32(lblStatus.Text);

            DAL dal = new DAL();
            ComplianceMasterDetailCombined objcomApproval = new ComplianceMasterDetailCombined();
            var compApproval = dal.LoadMyTeamComplianceDetails(complianceDetailID, complianceID);
            //var compApproval = dal.LoadMyComplianceDetails(userId, statusID, complianceID);

            txtDetailsComplianceID.Text = compApproval.ComplianceID.ToString();
            txtDetailsComplianceDetailID.Text = compApproval.ComplianceDetailID.ToString();
            txtDetailsComplArea.Text = compApproval.ComplianceArea;// row.Cells[1].Text;
            ////txtDetailsComplianceType.Text = compApproval.ComplianceTypeName;// row.Cells[2].Text;
            ////txtDetailsGovLegislation.Text = compApproval.GovernmentLegislation;// row.Cells[3].Text;
            ////txtDetailsActSection.Text = compApproval.ActSectionReference;// row.Cells[4].Text;
            ////txtDetailsNatureOfCompliance.Text = compApproval.NatureOfComplianceName;// row.Cells[5].Text;
            ////txtDetailsEffectiveDate.Text = compApproval.EffectiveFrom.ToString("dd-MM-yyyy");// row.Cells[6].Text;
            ////txtDetailsStandardDate.Text = compApproval.StandardDueDate.ToString("dd-MM-yyyy");// row.Cells[7].Text;

            ////txtDetailsFrequencyName.Text = compApproval.FrequencyName;// row.Cells[8].Text;
            txtDetailsDepartmentName.Text = compApproval.DepartmentName;// row.Cells[0].Text;
            txtDetails.Text = compApproval.DetailsOfComplianceRequirements;// row.Cells[0].Text;
            ////txtUpdatedComments.Text = compApproval.UserComments;
            //// Split comments by delimiter and join them with new line
            string[] UserCommentArray = compApproval.UserComments.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
            compApproval.UserComments = string.Join(Environment.NewLine, UserCommentArray);
            txtUpdatedComments.Text = compApproval.UserComments;



            btnupdate.Visible = true;
            btnSubmitForApproval.Visible = true;

            DisplayUploadedFiles(txtDetailsComplianceDetailID.Text, "");
        }
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            labelError.Visible = false;
            string userName = Session["UserName"].ToString();
            string complianceId = txtDetailsComplianceID.Text;
            string complianceDetailId = txtDetailsComplianceDetailID.Text;
            string complianceArea = txtDetailsComplArea.Text;
            string DepartmentName = txtDetailsDepartmentName.Text;

            // Create directory name based on selected record and current date/time
            //string directoryName = $"{userName}_{DepartmentName}_{complianceDetailId}_{complianceArea}_{DateTime.Today}";
            string directoryName = $"{userName}_{DepartmentName}_{complianceDetailId}_{complianceArea}";
            string directoryPath = Server.MapPath("~/FilesRep/" + directoryName);


            //Check whether Directory (Folder) exists.
            if (!Directory.Exists(directoryPath))
            {
                //If Directory (Folder) does not exists. Create it.
                Directory.CreateDirectory(directoryPath);
            }
            // Upload files to the newly created directory
            foreach (HttpPostedFile file in FileUpload1.PostedFiles)
            {
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(directoryPath, fileName);
                file.SaveAs(filePath);
            }
            int compDetailId = Convert.ToInt32(complianceDetailId);
            //Save the path to the database for the compliancedetailid
            SaveComplianceDetailFilePath(directoryName, compDetailId); //18jan25 : Not sending dirPath as it will show complete server path, we can merge the path at runtime as the upload directory is fixed we only need directory name where the files are beign uploaded for a user.

            // Display uploaded files in a new GridView
            DisplayUploadedFiles(complianceDetailId, directoryPath = "");
        }

        private void SaveComplianceDetailFilePath(string directoryName, int complianceDetailId)
        {
            DAL dal = new DAL();
            dal.SaveComplianceDetailFilePath(directoryName, userId, complianceDetailId);
        }
        private void DisplayUploadedFiles(string ComplianceDetailId, string directoryPath)
        {
            lblUploadedDocuments.Visible = true;
            string dirPath = directoryPath;
            //Get DirectoryPath from database, to check if path exists otherwise directoryPath will come from calling method
            if (directoryPath == "")
            {
                DAL dal = new DAL();

                dirPath = dal.GetComplianceDetailFilePath(Convert.ToInt32(ComplianceDetailId));
            }
            if (dirPath == "")
            {
                GridView2.DataSource = null;
                GridView2.DataBind();
            }
            else
            {
                string dirtoryPath = Server.MapPath("~/FilesRep/" + dirPath);
                if (Directory.Exists(dirtoryPath))
                {
                    string[] fileNames = Directory.GetFiles(dirtoryPath);
                    GridView2.DataSource = fileNames.Select(fileName => new { ID = ComplianceDetailId, FileName = Path.GetFileName(fileName) });
                    GridView2.DataBind();
                    //GridView2.Visible = fileNames.Length > 0;
                }
                else
                {
                    // If directory doesn't exist, hide GridView2
                    // GridView2.Visible = false;
                }
            }

        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                DAL dal = new DAL();
                dal.UpdateUserComments(Convert.ToInt32(txtDetailsComplianceID.Text), Convert.ToInt32(txtDetailsComplianceDetailID.Text), txtNewComments.Text, userId);
                labelError.Visible = true;
                labelError.Style.Add(HtmlTextWriterStyle.Color, "Blue");
                labelError.Text = "Comments updated successfully....";
            }

            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
            }

        }
        protected void DownloadFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            string userName = Session["UserName"].ToString();
            string DepartmentName = txtDetailsDepartmentName.Text;
            string complianceDetailId = txtDetailsComplianceDetailID.Text;
            string complianceArea = txtDetailsComplArea.Text;
            string directoryName = $"{userName}_{DepartmentName}_{complianceDetailId}_{complianceArea}";
            string directoryPath = Server.MapPath("~/FilesRep/" + directoryName);
            filePath = Path.Combine(directoryPath, filePath);
            Response.ContentType = ContentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filePath);
            Response.WriteFile(filePath);
            Response.End();
        }
        protected void DeleteFile(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            string userName = Session["UserName"].ToString();
            string DepartmentName = txtDetailsDepartmentName.Text;
            string complianceDetailId = txtDetailsComplianceDetailID.Text;
            string complianceArea = txtDetailsComplArea.Text;
            string directoryName = $"{userName}_{DepartmentName}_{complianceDetailId}_{complianceArea}";
            string directoryPath = Server.MapPath("~/FilesRep/" + directoryName);
            filePath = Path.Combine(directoryPath, filePath);
            File.Delete(filePath);
            // Display uploaded files in a new GridView
            DisplayUploadedFiles(complianceDetailId, directoryPath = "");
            //Response.Redirect(Request.Url.AbsoluteUri);
        }
    }
}