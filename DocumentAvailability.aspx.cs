using ClosedXML.Excel;
using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
	public partial class DocumentAvailability : System.Web.UI.Page
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
                BindGridFiltered();
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

        protected void gvDocuments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Retrieve filter parameters from ViewState
            int deptId = ViewState["DeptId"] != null ? Convert.ToInt32(ViewState["DeptId"]) : 0;
            int typeId = ViewState["TypeId"] != null ? Convert.ToInt32(ViewState["TypeId"]) : 0;
            int nature = ViewState["Nature"] != null ? Convert.ToInt32(ViewState["Nature"]) : 0;
            int priority = ViewState["Priority"] != null ? Convert.ToInt32(ViewState["Priority"]) : 0;

            gvDocuments.PageIndex = e.NewPageIndex;
            BindGridFiltered(deptId, typeId, nature, priority);
        }

        
        protected void btnLoad_Click(object sender, EventArgs e)
        {
            int typeId = dropdownValue(ddlTypeOfCompliance);
            int nature = dropdownValue(ddlNatureOfCompliance);
            int priority = dropdownValue(ddlPriority);
            int deptId = dropdownValue(ddlDepartmentFunction);
            // Store filter parameters in ViewState (optional, for paging)
            ViewState["DeptId"] = deptId;
            ViewState["TypeId"] = typeId;
            ViewState["Nature"] = nature;
            ViewState["Priority"] = priority;

            if (ddlTypeOfCompliance.SelectedIndex == 0 && ddlDepartmentFunction.SelectedIndex == 0 && ddlNatureOfCompliance.SelectedIndex == 0 && ddlPriority.SelectedIndex == 0)
            {
                ShowMessage("Please select at least one from Department / Nature / Type / Priority.", true);
                return;
            }

            lblErrorMessage.Visible = false;
            BindGridFiltered(deptId, typeId, nature, priority);
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
            ddlNatureOfCompliance.SelectedIndex = 0;
            ddlDepartmentFunction.SelectedIndex = 0;
            ddlPriority.SelectedIndex = 0;
            ddlTypeOfCompliance.SelectedIndex = 0;
            lblErrorMessage.Visible = false;
            BindGridFiltered();
        }



        //Load Filtered records
        private void BindGridFiltered(int deptId = 0, int typeId = 0, int nature = 0, int priority = 0)
        {
            try
            {
                DAL dal = new DAL();
                gvDocuments.DataSource = dal.LoadComplianceDocumentsForReport(deptId, typeId, nature,priority);
                gvDocuments.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading compliance data: {ex.Message}", true);
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Retrieve filter parameters from ViewState
            int deptId = ViewState["DeptId"] != null ? Convert.ToInt32(ViewState["DeptId"]) : 0;
            int typeId = ViewState["TypeId"] != null ? Convert.ToInt32(ViewState["TypeId"]) : 0;
            int nature = ViewState["Nature"] != null ? Convert.ToInt32(ViewState["Nature"]) : 0;
            int priority = ViewState["Priority"] != null ? Convert.ToInt32(ViewState["Priority"]) : 0;


            DAL dal = new DAL();
            var varCatalogue = dal.LoadComplianceDocumentsForReport(deptId, typeId, nature, priority);

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
                worksheet.Cell("C1").Value = "COMPLIANCE NATURE";
                worksheet.Cell("D1").Value = "COMPLIANCE TYPE";
                worksheet.Cell("E1").Value = "DEPARTMENT";
                worksheet.Cell("F1").Value = "PRIORITY";
                worksheet.Cell("G1").Value = "DOCUMENT TYPE";
                worksheet.Cell("H1").Value = "UPLOAD DATE";

                int recordIndex = 2;
                foreach (var _cat in varCatalogue)
                {
                    worksheet.Cell(recordIndex, 1).Value = _cat.ComplianceID;
                    worksheet.Cell(recordIndex, 2).Value = _cat.ComplianceRef?.Trim();
                    worksheet.Cell(recordIndex, 3).Value = _cat.ComplianceNature?.Trim();
                    worksheet.Cell(recordIndex, 4).Value = _cat.ComplianceType;
                    worksheet.Cell(recordIndex, 5).Value = _cat.Department;
                    worksheet.Cell(recordIndex, 6).Value = _cat.Priority;
                    worksheet.Cell(recordIndex, 7).Value = _cat.DocTypeName;
                    worksheet.Cell(recordIndex, 8).Value = _cat.FileName;
                    worksheet.Cell(recordIndex, 9).Value = _cat.UploadedDate;

                    recordIndex++;
                }
                // Define date columns (adjust column numbers based on your data)
                int[] dateColumns = { 9 }; // Example: upload date

                // Format date columns
                foreach (int col in dateColumns)
                {
                    worksheet.Column(col).Style.DateFormat.Format = "dd-mm-yyyy";
                }

                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 10;
                worksheet.Column(3).Width = 25;
                worksheet.Column(4).Width = 20;
                worksheet.Column(5).Width = 20;
                worksheet.Column(6).Width = 10;
                worksheet.Column(7).Width = 10;
                worksheet.Column(8).Width = 15;
                worksheet.Column(9).Width = 10;

                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local
                string excelName = "ComplianceDocumentReport_" + localTime.ToString("dd_MMM_yy");
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