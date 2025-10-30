using ClosedXML.Excel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class ComplianceRegister : System.Web.UI.Page
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
                BindGrid("All");
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
                BindDropdown(ddlTypeOfCompliance, dal.PopulateComplianceType("ComplianceType"), "ComplianceTypeName", "ComplianceTypeID", "Select Compliance Type");
                BindDropdown(ddlNatureOfCompliance, dal.PopulateComplianceNature("ComplianceNature"), "NatureOfCompliance", "ComplianceNatureID", "Select ComplianceNature");
                BindDropdown(ddlDepartmentFunction, dal.PopulateDepartment("Department"), "DepartmentName", "DeptID", "Select Department");
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

        protected void gvCompliances_RowCommand(object sender, GridViewCommandEventArgs e) { }
        protected void gvCompliances_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCompliances.PageIndex = e.NewPageIndex;
            BindGrid("All");
        }

        //Default load without filters
        private void BindGrid(string compMaster)
        {
            try
            {
                DAL dal = new DAL();
                List<ComplianceMasterr> compliances = new List<ComplianceMasterr>();
                compliances = dal.LoadComplianceMaster(compMaster, 0);
                gvCompliances.DataSource = compliances;
                gvCompliances.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading compliance data: {ex.Message}", true);
            }
        }
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            int typeOfCompliance = dropdownValue(ddlTypeOfCompliance);
            int natureOfCompliance = dropdownValue(ddlNatureOfCompliance);
            int priorityId = dropdownValue(ddlPriority);
            int deptId = dropdownValue(ddlDepartmentFunction);
            if (ddlTypeOfCompliance.SelectedIndex == 0 && ddlDepartmentFunction.SelectedIndex == 0 && ddlNatureOfCompliance.SelectedIndex == 0 && ddlPriority.SelectedIndex == 0)
            {
                ShowMessage("Please select at least one from Department / Nature / Type / Priority.", true);
                return;
            }

            lblErrorMessage.Visible = false;
            BindGridFiltered(deptId, typeOfCompliance, natureOfCompliance, priorityId);
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

        protected void btnUpload_Click(object sender, EventArgs e)
        {

        }
        protected void btnAll_Click(object sender, EventArgs e)
        {
            ddlNatureOfCompliance.SelectedIndex = 0;
            ddlDepartmentFunction.SelectedIndex = 0;
            ddlPriority.SelectedIndex = 0;
            ddlTypeOfCompliance.SelectedIndex = 0;
            lblErrorMessage.Visible = false;
            BindGrid("All");
        }



        //Load Filtered records
        private void BindGridFiltered(int deptId = 0, int typeId = 0, int nature = 0, int priority = 0)
        {
            try
            {
                DAL dal = new DAL();
                List<ComplianceMasterr> compliances = new List<ComplianceMasterr>();
                compliances = dal.LoadComplianceMasterForReport(deptId, typeId, nature, priority);
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

            int typeId = dropdownValue(ddlTypeOfCompliance);
            int nature = dropdownValue(ddlNatureOfCompliance);
            int priority = dropdownValue(ddlPriority);
            int deptId = dropdownValue(ddlDepartmentFunction);

            DAL dal = new DAL();
            var varCatalogue = dal.LoadComplianceMasterForReport(deptId, typeId, nature, priority);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.TabColor = XLColor.Black;
                worksheet.RowHeight = 12;
                worksheet.Row(1).Height = 20;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Cell("A1").Value = "COMPLIANCE ID";
                worksheet.Cell("B1").Value = "COMPLIANCE REF";
                worksheet.Cell("C1").Value = "BUSINESS UNIT";
                worksheet.Cell("D1").Value = "COMPLIANCE NATURE";
                worksheet.Cell("E1").Value = "COMPLIANCE TYPE";
                worksheet.Cell("F1").Value = "DRIVEN BY";
                worksheet.Cell("G1").Value = "ACT SECTION";
                worksheet.Cell("H1").Value = "LAW TYPE";
                worksheet.Cell("I1").Value = "TERRITORY";
                worksheet.Cell("J1").Value = "DETAILS";
                worksheet.Cell("K1").Value = "PENALTY";
                worksheet.Cell("L1").Value = "PRIORITY";
                worksheet.Cell("M1").Value = "FREQUENCY";
                worksheet.Cell("N1").Value = "EFFECTIVE FROM";
                worksheet.Cell("O1").Value = "EFFECTIVE TILL";
                worksheet.Cell("P1").Value = "FIRST DUE DATE";
                worksheet.Cell("Q1").Value = "DUE ON EVERY";
                worksheet.Cell("R1").Value = "FORMS IF ANY";
                worksheet.Cell("S1").Value = "ACTIONS TO BE TAKEN";
                worksheet.Cell("T1").Value = "DEPARTMENT";
                worksheet.Cell("U1").Value = "PREPARER";
                worksheet.Cell("V1").Value = "APPROVER";
                int recordIndex = 2;
                foreach (var _cat in varCatalogue)
                {
                    worksheet.Cell(recordIndex, 1).Value = _cat.ComplianceID;
                    worksheet.Cell(recordIndex, 2).Value = _cat.ComplianceRef.Trim();
                    worksheet.Cell(recordIndex, 3).Value = _cat.BusinessUnitName.Trim();
                    worksheet.Cell(recordIndex, 4).Value = _cat.NatureOfComplianceName.Trim();
                    worksheet.Cell(recordIndex, 5).Value = _cat.ComplianceTypeName;
                    worksheet.Cell(recordIndex, 6).Value = _cat.DrivenByName;
                    worksheet.Cell(recordIndex, 7).Value = _cat.ActSectionReference;
                    worksheet.Cell(recordIndex, 8).Value = _cat.LawName;
                    worksheet.Cell(recordIndex, 9).Value = _cat.Territory;
                    worksheet.Cell(recordIndex, 10).Value = _cat.DetailsOfComplianceRequirements;
                    worksheet.Cell(recordIndex, 11).Value = _cat.NonCompliancePenalty;
                    worksheet.Cell(recordIndex, 12).Value = _cat.Priority;
                    worksheet.Cell(recordIndex, 13).Value = _cat.FrequencyName;
                    worksheet.Cell(recordIndex, 14).Value = _cat.EffectiveFrom;
                    worksheet.Cell(recordIndex, 15).Value = _cat.StandardDueDate;
                    worksheet.Cell(recordIndex, 16).Value = _cat.FirstDueDate;
                    worksheet.Cell(recordIndex, 17).Value = _cat.DueOnEvery;
                    worksheet.Cell(recordIndex, 18).Value = _cat.FormsIfAny;
                    worksheet.Cell(recordIndex, 19).Value = _cat.ActionsToBeTaken;
                    worksheet.Cell(recordIndex, 20).Value = _cat.DepartmentName;
                    worksheet.Cell(recordIndex, 21).Value = _cat.InitiatorName;
                    worksheet.Cell(recordIndex, 22).Value = _cat.ApproverName;
                    recordIndex++;
                }

                // Define date columns (adjust column numbers based on your data)
                int[] dateColumns = { 14, 15, 16 }; // Example: EffectiveFrom, StandardDueDate, FirstDueDate

                // Format date columns
                foreach (int col in dateColumns)
                {
                    worksheet.Column(col).Style.DateFormat.Format = "dd-mm-yyyy"; // ClosedXML uses DateFormat for dates
                }

                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 10;
                worksheet.Column(3).Width = 10;
                worksheet.Column(4).Width = 30;
                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 15;
                worksheet.Column(7).Width = 50;
                worksheet.Column(8).Width = 15;
                worksheet.Column(9).Width = 10;
                worksheet.Column(10).Width = 35;
                worksheet.Column(11).Width = 20;
                worksheet.Column(12).Width = 10;
                worksheet.Column(13).Width = 15;
                worksheet.Column(14).Width = 20;
                worksheet.Column(15).Width = 20;
                worksheet.Column(16).Width = 20;
                worksheet.Column(17).Width = 10;
                worksheet.Column(18).Width = 20;
                worksheet.Column(19).Width = 20;
                worksheet.Column(20).Width = 15;
                worksheet.Column(21).Width = 20;
                worksheet.Column(22).Width = 20;

                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local
                string excelName = "ComplianceRegisterReport_" + localTime.ToString("dd_MMM_yy");
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