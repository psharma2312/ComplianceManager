using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Controls;

namespace ComplianceManager
{
    public partial class AssignCompliances : System.Web.UI.Page
    {
        Int32 userId = 0;
        Int32 deptId = 0;
        int complianceDetailID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            userId = Convert.ToInt32(Session["UserId"].ToString());
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
                LoadDepartmentDropdown();
                refreshdata();
            }
        }
        private void LoadDepartmentDropdown()
        {
            DAL dal = new DAL();
            var deptData = dal.GetAllDepartmentDropdown();
            ddlDeparmentLoad.DataSource = deptData;
            ddlDeparmentLoad.DataBind();
            ddlDeparmentLoad.Items.Insert(0, new ListItem("Please Select", String.Empty));
            ddlDeparmentLoad.SelectedIndex = 0;
        }

        protected void ddlDepartmentLoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadComplianceDropdownAssignment(ddlDeparmentLoad.SelectedItem.Value);
        }

        public void LoadComplianceDropdownAssignment(string deptid = "")
        {
            DAL dal = new DAL();
            ddlComplianceAreaLoad.Items.Clear();
            ddlComplianceAreaLoad.DataSource = dal.LoadComplianceDropdown(Convert.ToInt32(deptid));
            ddlComplianceAreaLoad.DataBind();
            ddlComplianceAreaLoad.Items.Insert(0, new ListItem("Please Select", ""));
            ddlComplianceAreaLoad.SelectedIndex = 0;
            LoadAllDropdown(deptid);
        }
        public void LoadAllDropdown(string deptid = "")
        {
            DAL dal = new DAL();
            var complianceType = dal.GetAllComplianceType(Convert.ToInt32(deptid));
            ddlComplianceType.DataSource = complianceType;
            ddlComplianceType.DataBind();
            ddlComplianceType.Items.Insert(0, new ListItem("Please Select", "0"));
            ddlComplianceType.SelectedIndex = 0;
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            deptId = Convert.ToInt32(ddlDeparmentLoad.SelectedItem.Value);
            Int32 complianceTypeId = Convert.ToInt32(ddlComplianceType.SelectedItem.Value);
            string complianceArea = ddlComplianceAreaLoad.SelectedItem.Value;
            Session["deptId"] = deptId.ToString();
            Session["complianceTypeId"] = complianceTypeId.ToString();
            Session["complianceArea"] = complianceArea.ToString();
            refreshdata(deptId, complianceTypeId, complianceArea);
            UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
            if (updatePanel != null) updatePanel.Update();
        }
        public void refreshdata(int deptid = 0, int complianceTypeId = 0, string complianceArea = "")
        {
            DAL dal = new DAL();
            GridView2.DataSource = dal.LoadComplianceMaster(deptid, complianceTypeId, complianceArea);
            GridView2.DataBind();
        }

        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView2.PageIndex = e.NewPageIndex;
            refreshdata();
        }

        protected void Display(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showForgotModal", "showModal();", true);
            int rowIndex = ((sender as LinkButton).NamingContainer as GridViewRow).RowIndex;
            int complianceID = Convert.ToInt32(GridView2.DataKeys[rowIndex].Values[0]);
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
            GridViewRow row = GridView2.SelectedRow;
            int selectedIndex = GridView2.SelectedIndex;
            lblDocUpload.Visible = true;
            string compDetailID = (GridView2.DataKeys[selectedIndex]["ComplianceDetailID"]).ToString();
            string compID = (GridView2.DataKeys[selectedIndex]["ComplianceID"]).ToString();
            string deptName = (GridView2.DataKeys[selectedIndex]["DeptId"]).ToString();
            int complianceID = Convert.ToInt32(compID);
            complianceDetailID = Convert.ToInt32(compDetailID);
            deptId = Convert.ToInt32(deptName);

            lblAssignTo.Visible = true;
            ddlAssignTo.Visible = true;
            btnSave.Visible = true;
            lblCompDetailId.Text = compDetailID;

            DAL dal = new DAL();
            ddlAssignTo.Items.Clear();
            ddlAssignTo.DataSource = dal.LoadAssignToDropdown(deptId); //Convert.ToInt32(userId),
            ddlAssignTo.DataBind();
            ddlAssignTo.Items.Insert(0, new ListItem("Please Select", String.Empty));
            ddlAssignTo.SelectedIndex = 0;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Int32 assignToId = Convert.ToInt32(ddlAssignTo.SelectedItem.Value);
            InsertDetails(Convert.ToInt32(lblCompDetailId.Text), assignToId);  
            refreshdata();
        }
        private void InsertDetails(Int32 compDetailId, Int32 assignToId)
        {
            DAL dal = new DAL();
            ComplianceMasterDetails comp = new ComplianceMasterDetails();
            comp.ComplianceDetailID = compDetailId;
            comp.AssignedToID = assignToId;
            comp.CreatedById = Convert.ToInt32(Session["UserId"].ToString());
            dal.UpdateComplianceAssignmentDetails(comp);
            ddlDeparmentLoad.SelectedIndex = 0;
            refreshdata();
        }
    }
}