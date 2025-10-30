using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;

namespace ComplianceManager
{
	public partial class GeneratedCertificates : System.Web.UI.Page
	{
        Int32 userId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] == null)
                Response.Redirect("../AuthPages/Login.aspx");
            userId = Convert.ToInt32(Session["UserId"].ToString());
            UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
            if (updatePanel != null)
            {
                string script = $"var updatePanelClientId = '{updatePanel.ClientID}';";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "setUpdatePanelClientId", script, true);
            }
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnAll);
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnExport);
            if (!IsPostBack)
            {
                BindDropdowns();
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
                        if (updatePanel != null) updatePanel.Update();
                    }
                }
            }
        }
        private void BindDropdowns()
        {
            try
            {
                DAL dal = new DAL();
                BindDropdown(ddlDepartmentFunction, dal.PopulateDepartment("Department"), "DepartmentName", "DeptID", "Select Department");
                ddlPeriod.Items.Clear();
                ddlPeriod.DataSource = dal.GetRolling12Months(); // Fetch all period ["04/2024", "04/2025"]
                ddlPeriod.DataBind();
                ddlPeriod.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Period", string.Empty));
                ddlPeriod.SelectedIndex = 0;
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
        protected void gvCompliances_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            int deptId = ViewState["DeptId"] != null ? Convert.ToInt32(ViewState["DeptId"]) : 0;
            string period = ViewState["Period"] != null ? ViewState["Period"].ToString() : "";
            gvCompliances.PageIndex = e.NewPageIndex;
            BindGridFiltered(deptId, period);
        }
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            int deptId = dropdownValue(ddlDepartmentFunction);
            string period = ddlPeriod.SelectedItem.Text;
            // Store filter parameters in ViewState
            ViewState["DeptId"] = deptId;
            ViewState["Period"] = period;

            if (ddlDepartmentFunction.SelectedIndex == 0 && ddlPeriod.SelectedIndex == 0)
            {
                ShowMessage("Please select at least one from Department / Period.", true);
                return;
            }
            lblErrorMessage.Visible = false;
            BindGridFiltered(deptId, period);
        }
        private int dropdownValue(DropDownList ddl)
        {
            int index = 0;
            if (ddl.SelectedIndex != 0)
            {
                index = Convert.ToInt32(ddl.SelectedValue);
            }
            return index;
        }
        protected void btnAll_Click(object sender, EventArgs e)
        {
            ddlDepartmentFunction.SelectedIndex = 0;
            ddlPeriod.SelectedIndex = 0;
            lblErrorMessage.Visible = false;
            gvCompliances.Visible = false;
            //BindGridFiltered();
        }
        private void BindGridFiltered(int deptId = 0, string period="")
        {
            try
            {
                DAL dal = new DAL();
                List<ComplianceCertificate> compliances = new List<ComplianceCertificate>();
                compliances = dal.LoadComplianceCertificate(deptId, period);
                gvCompliances.DataSource = compliances;
                gvCompliances.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading compliance data: {ex.Message}", true);
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            int deptId = ViewState["DeptId"] != null ? Convert.ToInt32(ViewState["DeptId"]) : 0;
            string period = ViewState["Period"] != null ? ViewState["Period"].ToString() : "";

            DAL dal = new DAL();
            var varCatalogue = dal.LoadComplianceCertificate(deptId, period);
            using (var workbook = new XLWorkbook())
            {

                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.TabColor = XLColor.Black;
                worksheet.RowHeight = 12;
                worksheet.Row(1).Height = 20;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(1).Style.Font.Bold = true;

                worksheet.Cell("A1").Value = "S.NO.";
                worksheet.Cell("B1").Value = "CERTIFICATE NO";
                worksheet.Cell("C1").Value = "ISSUE DATE";
                worksheet.Cell("D1").Value = "PERIOD";

                int recordIndex = 2;
                int serialNo = 1;
                foreach (var _cat in varCatalogue)
                {
                    worksheet.Cell(recordIndex, 1).Value = serialNo;
                    worksheet.Cell(recordIndex, 2).Value = _cat.CertificateNo;
                    worksheet.Cell(recordIndex, 3).Value = _cat.GeneratedOn;
                    worksheet.Cell(recordIndex, 4).Value = _cat.Period;
                    recordIndex++;
                    serialNo++;
                }
                for (int col = 1; col <= 4; col++)
                {
                    worksheet.Column(col).Width = 30;
                }
                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local
                string excelName = "CertificatesGeneratedReport_" + localTime.ToString("dd_MMM_yy");
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    memoryStream.Close();
                }
                Response.Flush();
                Response.End();
            }
        }
    }
}