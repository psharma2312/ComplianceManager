using ClosedXML.Excel;
using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Button = System.Web.UI.WebControls.Button;

namespace ComplianceManager
{
	public partial class MasterReports : System.Web.UI.Page
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
            ShowHideGridView("All",false);
            if (!IsPostBack)
            {
                BindDropdowns();
                //BindGrid("All");
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
                BindDropdown(ddlMaster, dal.PopulateMasterNames("Master"), "MasterName", "MasterID", "Select Master");
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

        private void ShowHideGridView(string gridName, bool showHide)
        {
            // If gridName is "All" and showHide is false, hide all grids
            if (gridName == "All" && showHide == false)
            {
                gvComplianceNature.Visible = false;
                gvCompliances.Visible = false;
                gvComplianceStatus.Visible = false;
                gvComplianceType.Visible = false;
                gvDepartment.Visible = false;
                gvDrivenBy.Visible = false;
                gvFrequencies.Visible = false;
                gvLawType.Visible = false;
                gvUsers.Visible = false;
            }
            // For specific grid name, show/hide only that grid and hide others
            else
            {
                // First, set all grids to false
                gvComplianceNature.Visible = false;
                gvCompliances.Visible = false;
                gvComplianceStatus.Visible = false;
                gvComplianceType.Visible = false;
                gvDepartment.Visible = false;
                gvDrivenBy.Visible = false;
                gvFrequencies.Visible = false;
                gvLawType.Visible = false;
                gvUsers.Visible = false;

                DAL dal = new DAL();
                
                // Then, set the specified grid's visibility based on showHide
                switch (gridName.ToLower())
                {
                    case "compliancenature":
                        gvComplianceNature.Visible = showHide;
                        break;
                    case "compliances":
                        gvCompliances.Visible = showHide;
                        break;
                    case "compliancestatus":
                        gvComplianceStatus.Visible = showHide;
                        break;
                    case "compliancetype":
                        gvComplianceType.Visible = showHide;
                        break;
                    case "department":
                        gvDepartment.Visible = showHide;
                        break;
                    case "drivenby":
                        gvDrivenBy.Visible = showHide;
                        break;
                    case "frequencies":
                        gvFrequencies.Visible = showHide;
                        break;
                    case "lawtype":
                        gvLawType.Visible = showHide;
                        break;
                    case "users":
                        gvUsers.Visible = showHide;
                        break;
                }
            }
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
            gvCompliances.Visible = true;
            DAL dal = new DAL();
            gvCompliances.PageIndex = e.NewPageIndex;
            gvCompliances.DataSource = dal.LoadComplianceMaster("All", 0);
            gvCompliances.DataBind();
        }

        //Default load without filters
        
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            int masterId = dropdownValue(ddlMaster);
            if (ddlMaster.SelectedIndex == 0)
            {
                ShowMessage("Please select at least one Master.", true);
                return;
            }

            lblErrorMessage.Visible = false;
            BindGridFiltered(masterId);
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
            lblErrorMessage.Visible = false;
            ddlMaster.SelectedIndex = 0;
            ShowHideGridView("All", false);
        }



        //Load Filtered records
        private void BindGridFiltered(int masterId = 0)
        {
            try
            {
                DAL dal = new DAL();

                // Then, set the specified grid's visibility based on showHide
                switch (masterId)
                {
                    case 13:
                        ShowHideGridView("compliancenature", true);
                        gvComplianceNature.DataSource = dal.GetAllNatureCompliance();
                        gvComplianceNature.DataBind();
                        break;
                    case 7:
                        ShowHideGridView("compliances", true);
                        gvCompliances.DataSource = dal.LoadComplianceMaster("All", 0);
                        gvCompliances.DataBind();
                        break;
                    case 16:
                        ShowHideGridView("compliancestatus", true);
                        gvComplianceStatus.DataSource = dal.GetAllComplianceStatus();
                        gvComplianceStatus.DataBind();
                        break;
                    case 14:
                        ShowHideGridView("compliancetype", true);
                        gvComplianceType.DataSource = dal.GetAllComplianceType(0);
                        gvComplianceType.DataBind(); 
                        break;
                    case 4:
                        ShowHideGridView("department", true);
                        gvDepartment.DataSource = dal.GetAllDepartments();
                        gvDepartment.DataBind();
                        break;
                    case 24:
                        ShowHideGridView("drivenby", true);
                        gvDrivenBy.DataSource = dal.GetAllDrivenBy();
                        gvDrivenBy.DataBind();
                        break;
                    case 12:
                        ShowHideGridView("frequencies", true);
                        gvFrequencies.DataSource = dal.GetAllFrequency();
                        gvFrequencies.DataBind();
                        break;
                    case 23:
                        ShowHideGridView("lawtype", true);
                        gvLawType.DataSource = dal.GetAllLawTypes();
                        gvLawType.DataBind();
                        break;
                    case 8:
                        ShowHideGridView("users", true);
                        gvUsers.DataSource = dal.GetAllUsersForReport();
                        gvUsers.DataBind();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading compliance data: {ex.Message}", true);
            }
        }
        protected void btnExport_Click(object sender, EventArgs e)
        {
            DateTime serverTime = DateTime.Now; // Current time in server timezone
            DateTime utcTime = serverTime.ToUniversalTime();
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // Convert to IST
            string excelName = "";
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.TabColor = XLColor.Black;
                worksheet.RowHeight = 12;
                worksheet.Row(1).Height = 20;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(1).Style.Font.Bold = true;

                int masterId = dropdownValue(ddlMaster);
                DAL dal = new DAL();
                switch (masterId)
                {
                    case 13:
                        var varCatalogue = dal.GetAllNatureCompliance();
                        worksheet.Cell("A1").Value = "ID";
                        worksheet.Cell("B1").Value = "COMPLIANCE NATURE";
                        worksheet.Cell("C1").Value = "DESCRIPTION";
                        int recordIndex = 2;
                        foreach (var _cat in varCatalogue)
                        {
                            worksheet.Cell(recordIndex, 1).Value = _cat.ComplianceNatureID;
                            worksheet.Cell(recordIndex, 2).Value = _cat.NatureOfCompliance?.Trim();
                            worksheet.Cell(recordIndex, 3).Value = _cat.Description?.Trim();
                            recordIndex++;
                        }
                        worksheet.Column(1).Width = 10;
                        worksheet.Column(2).Width = 30;
                        worksheet.Column(3).Width = 30;
                        excelName = "ComplianceNatureReport_" + localTime.ToString("dd_MMM_yy_hh_mm_ss_tt");
                        break;
                    case 7:
                        var varCatalogue1 = dal.LoadComplianceMaster("All", 0);
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
                        int recordIndex1 = 2;
                        foreach (var _cat in varCatalogue1)
                        {
                            worksheet.Cell(recordIndex1, 1).Value = _cat.ComplianceID;
                            worksheet.Cell(recordIndex1, 2).Value = _cat.ComplianceRef?.Trim();
                            worksheet.Cell(recordIndex1, 3).Value = _cat.BusinessUnitName?.Trim();
                            worksheet.Cell(recordIndex1, 4).Value = _cat.NatureOfComplianceName?.Trim();
                            worksheet.Cell(recordIndex1, 5).Value = _cat.ComplianceTypeName;
                            worksheet.Cell(recordIndex1, 6).Value = _cat.DrivenByName;
                            worksheet.Cell(recordIndex1, 7).Value = _cat.ActSectionReference;
                            worksheet.Cell(recordIndex1, 8).Value = _cat.LawName;
                            worksheet.Cell(recordIndex1, 9).Value = _cat.Territory;
                            worksheet.Cell(recordIndex1, 10).Value = _cat.DetailsOfComplianceRequirements;
                            worksheet.Cell(recordIndex1, 11).Value = _cat.NonCompliancePenalty;
                            worksheet.Cell(recordIndex1, 12).Value = _cat.Priority;
                            worksheet.Cell(recordIndex1, 13).Value = _cat.FrequencyName;
                            worksheet.Cell(recordIndex1, 14).Value = _cat.EffectiveFrom;
                            worksheet.Cell(recordIndex1, 15).Value = _cat.StandardDueDate;
                            worksheet.Cell(recordIndex1, 16).Value = _cat.FirstDueDate;
                            worksheet.Cell(recordIndex1, 17).Value = _cat.DueOnEvery;
                            worksheet.Cell(recordIndex1, 18).Value = _cat.FormsIfAny;
                            worksheet.Cell(recordIndex1, 19).Value = _cat.ActionsToBeTaken;
                            worksheet.Cell(recordIndex1, 20).Value = _cat.DepartmentName;
                            worksheet.Cell(recordIndex1, 21).Value = _cat.InitiatorName;
                            worksheet.Cell(recordIndex1, 22).Value = _cat.ApproverName;
                            recordIndex1++;
                        }
                        // Define date columns
                        int[] dateColumns = { 14, 15, 16 }; // EffectiveFrom, StandardDueDate, FirstDueDate
                        foreach (int col in dateColumns)
                        {
                            worksheet.Column(col).Style.DateFormat.Format = "dd-mm-yyyy";
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
                        excelName = "ComplianceMasterReport_" + localTime.ToString("dd_MMM_yy_hh_mm_ss_tt");
                        break;
                    case 16:
                        var varCatalogue2 = dal.GetAllComplianceStatus();
                        worksheet.Cell("A1").Value = "ID";
                        worksheet.Cell("B1").Value = "COMPLIANCE STATUS";
                        int recordIndex2 = 2;
                        foreach (var _cat in varCatalogue2)
                        {
                            worksheet.Cell(recordIndex2, 1).Value = _cat.ComplianceStatusID;
                            worksheet.Cell(recordIndex2, 2).Value = _cat.ComplianceStatusName?.Trim();
                            recordIndex2++;
                        }
                        worksheet.Column(1).Width = 10;
                        worksheet.Column(2).Width = 30;
                        excelName = "ComplianceStatusReport_" + localTime.ToString("dd_MMM_yy_hh_mm_ss_tt");
                        break;
                    case 14:
                        var varCatalogue3 = dal.GetAllComplianceType(0);
                        worksheet.Cell("A1").Value = "ID";
                        worksheet.Cell("B1").Value = "COMPLIANCE TYPE";
                        worksheet.Cell("C1").Value = "DESCRIPTION";
                        int recordIndex3 = 2;
                        foreach (var _cat in varCatalogue3)
                        {
                            worksheet.Cell(recordIndex3, 1).Value = _cat.ComplianceTypeID;
                            worksheet.Cell(recordIndex3, 2).Value = _cat.ComplianceTypeName?.Trim();
                            worksheet.Cell(recordIndex3, 3).Value = _cat.Description?.Trim();
                            recordIndex3++;
                        }
                        worksheet.Column(1).Width = 10;
                        worksheet.Column(2).Width = 30;
                        worksheet.Column(3).Width = 30;
                        excelName = "ComplianceTypeReport_" + localTime.ToString("dd_MMM_yy_hh_mm_ss_tt");
                        break;
                    case 4:
                        var varCatalogue4 = dal.GetAllDepartments();
                        worksheet.Cell("A1").Value = "ID";
                        worksheet.Cell("B1").Value = "DEPARTMENT NAME";
                        worksheet.Cell("C1").Value = "DESCRIPTION";
                        int recordIndex4 = 2;
                        foreach (var _cat in varCatalogue4)
                        {
                            worksheet.Cell(recordIndex4, 1).Value = _cat.DeptID;
                            worksheet.Cell(recordIndex4, 2).Value = _cat.DepartmentName?.Trim();
                            worksheet.Cell(recordIndex4, 3).Value = _cat.Description?.Trim();
                            recordIndex4++;
                        }
                        worksheet.Column(1).Width = 10;
                        worksheet.Column(2).Width = 30;
                        worksheet.Column(3).Width = 30;
                        excelName = "DepartmentReport_" + localTime.ToString("dd_MMM_yy_hh_mm_ss_tt");
                        break;
                    case 24:
                        var varCatalogue5 = dal.GetAllDrivenBy();
                        worksheet.Cell("A1").Value = "ID";
                        worksheet.Cell("B1").Value = "DRIVENBY NAME";
                        worksheet.Cell("C1").Value = "DESCRIPTION";
                        int recordIndex5 = 2;
                        foreach (var _cat in varCatalogue5)
                        {
                            worksheet.Cell(recordIndex5, 1).Value = _cat.DrivenId;
                            worksheet.Cell(recordIndex5, 2).Value = _cat.DrivenName?.Trim();
                            worksheet.Cell(recordIndex5, 3).Value = _cat.Description?.Trim();
                            recordIndex5++;
                        }
                        worksheet.Column(1).Width = 10;
                        worksheet.Column(2).Width = 30;
                        worksheet.Column(3).Width = 30;
                        excelName = "DrivenByReport_" + localTime.ToString("dd_MMM_yy_hh_mm_ss_tt");
                        break;
                    case 12:
                        var varCatalogue6 = dal.GetAllFrequency();
                        worksheet.Cell("A1").Value = "ID";
                        worksheet.Cell("B1").Value = "FREQUENCY NAME";
                        worksheet.Cell("C1").Value = "DESCRIPTION";
                        int recordIndex6 = 2;
                        foreach (var _cat in varCatalogue6)
                        {
                            worksheet.Cell(recordIndex6, 1).Value = _cat.FrequencyID;
                            worksheet.Cell(recordIndex6, 2).Value = _cat.FrequencyName?.Trim();
                            worksheet.Cell(recordIndex6, 3).Value = _cat.Description?.Trim();
                            recordIndex6++;
                        }
                        worksheet.Column(1).Width = 10;
                        worksheet.Column(2).Width = 30;
                        worksheet.Column(3).Width = 30;
                        excelName = "FrequencyReport_" + localTime.ToString("dd_MMM_yy_hh_mm_ss_tt");
                        break;
                    case 23:
                        var varCatalogue7 = dal.GetAllLawTypes();
                        worksheet.Cell("A1").Value = "ID";
                        worksheet.Cell("B1").Value = "LAWTYPE NAME";
                        worksheet.Cell("C1").Value = "DESCRIPTION";
                        int recordIndex7 = 2;
                        foreach (var _cat in varCatalogue7)
                        {
                            worksheet.Cell(recordIndex7, 1).Value = _cat.LawID;
                            worksheet.Cell(recordIndex7, 2).Value = _cat.LawName?.Trim();
                            worksheet.Cell(recordIndex7, 3).Value = _cat.Description?.Trim();
                            recordIndex7++;
                        }
                        worksheet.Column(1).Width = 10;
                        worksheet.Column(2).Width = 30;
                        worksheet.Column(3).Width = 30;
                        excelName = "LawTypeReport_" + localTime.ToString("dd_MMM_yy_hh_mm_ss_tt");
                        break;
                    case 8:
                        var varCatalogue8 = dal.GetAllUsersForReport();
                        worksheet.Cell("A1").Value = "USER NAME";
                        worksheet.Cell("B1").Value = "USER ID";
                        worksheet.Cell("C1").Value = "E MAIL";
                        worksheet.Cell("D1").Value = "MOBILE";
                        worksheet.Cell("E1").Value = "DEPARTMENT";
                        worksheet.Cell("F1").Value = "SUPERVISOR";
                        worksheet.Cell("G1").Value = "APPROVER";
                        worksheet.Cell("H1").Value = "PREPARER";
                        worksheet.Cell("I1").Value = "SUPERVISOR";
                        worksheet.Cell("J1").Value = "ACTIVE";
                        int recordIndex8 = 2;
                        foreach (var _cat in varCatalogue8)
                        {
                            worksheet.Cell(recordIndex8, 1).Value = _cat.Name;
                            worksheet.Cell(recordIndex8, 2).Value = _cat.UserName?.Trim();
                            worksheet.Cell(recordIndex8, 3).Value = _cat.Email?.Trim();
                            worksheet.Cell(recordIndex8, 4).Value = _cat.MobileNo;
                            worksheet.Cell(recordIndex8, 5).Value = _cat.DepartmentName;
                            worksheet.Cell(recordIndex8, 6).Value = _cat.SupervisorName;
                            worksheet.Cell(recordIndex8, 7).Value = _cat.IsApprover;
                            worksheet.Cell(recordIndex8, 8).Value = _cat.IsPreparer;
                            worksheet.Cell(recordIndex8, 9).Value = _cat.IsSupervisor;
                            worksheet.Cell(recordIndex8, 10).Value = _cat.IsActive;
                            recordIndex8++;
                        }
                        worksheet.Column(1).Width = 30;
                        worksheet.Column(2).Width = 20;
                        worksheet.Column(3).Width = 20;
                        worksheet.Column(4).Width = 10;
                        worksheet.Column(5).Width = 15;
                        worksheet.Column(6).Width = 15;
                        worksheet.Column(7).Width = 10;
                        worksheet.Column(8).Width = 10;
                        worksheet.Column(9).Width = 10;
                        worksheet.Column(10).Width = 10;
                        excelName = "UserReport_" + localTime.ToString("dd_MMM_yy_hh_mm_ss_tt");
                        break;
                }

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