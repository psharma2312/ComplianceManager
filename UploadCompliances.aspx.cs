using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class UploadCompliances : Page
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
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnUpload);
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnExport);
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnSample);
            if (!IsPostBack)
                LoadcompMaster("All");
        }
        protected void LoadcompMaster(string compMaster)
        {
            DAL dal = new DAL();
            gvCompliances.DataSource = dal.LoadComplianceMaster(compMaster,0);
            gvCompliances.DataBind();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                lblErrorMessage.Visible = false;
                Upload(sender, e); // TODO : CHECK IF compl ref no already exist dont let the file upload.
                LoadcompMaster("All");
            }
            else
                ShowMessage("Please select a file", true);
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

        private DataTable LoadExcelData(string filePath)
        {
            DataTable dt = new DataTable();
            using (var workbook = new XLWorkbook(filePath)) // Load the Excel file
            {
                var worksheet = workbook.Worksheet(1); // Get the first sheet (1-based index)
                if (worksheet == null)
                    throw new Exception("Worksheet not found.");

                // Get the range of used cells
                var range = worksheet.RangeUsed();
                if (range == null)
                    throw new Exception("No data found in the worksheet.");

                int colCount = range.ColumnCount();
                int rowCount = range.RowCount();

                // Add columns to DataTable from the first row (header)
                var headerRow = worksheet.Row(1);
                for (int col = 1; col <= colCount; col++)
                {
                    var cellValue = headerRow.Cell(col).GetValue<string>() ?? $"Column{col}";
                    dt.Columns.Add(cellValue);
                }

                // Read rows and add data to DataTable starting from row 2
                for (int row = 2; row <= rowCount; row++)
                {
                    DataRow dr = dt.NewRow();
                    for (int col = 1; col <= colCount; col++)
                    {
                        var cellValue = worksheet.Row(row).Cell(col).GetValue<string>() ?? "";
                        dr[col - 1] = cellValue;
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        //private DataTable LoadExcelData(string filePath)
        //{
        //    DataTable dt = new DataTable();
        //    FileInfo fileInfo = new FileInfo(filePath);
        //    using (ExcelPackage package = new ExcelPackage(fileInfo))
        //    {
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"]; // Get the first sheet
        //        if (worksheet == null)
        //            throw new Exception("Worksheet not found.");
                
        //        int colCount = worksheet.Dimension.End.Column; // Get number of columns
        //        int rowCount = worksheet.Dimension.End.Row;   // Get number of rows
                
        //        // Add columns to DataTable
        //        for (int col = 1; col <= colCount; col++)
        //            dt.Columns.Add(worksheet.Cells[1, col].Text);
                
        //        // Read rows and add data to DataTable
        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            DataRow dr = dt.NewRow();
        //            for (int col = 1; col <= colCount; col++)
        //                dr[col - 1] = worksheet.Cells[row, col].Text;
        //            dt.Rows.Add(dr);
        //        }

        //    }
        //    return dt;
        //}
        private void RemoveNullRows(DataTable dt)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i][0] == DBNull.Value || string.IsNullOrWhiteSpace(dt.Rows[i][0].ToString()))
                    dt.Rows[i].Delete();
            }
            dt.AcceptChanges();
        }

        private void ProcessExcelData(DataTable dtExcelData, DAL dal, List<string> masterBUs,List<string> masterNatureOfCompliance, List<string> masterComplianceTypes, 
                                        List<string> masterDrivenBy, List<string> masterLawsType, List<string> masterTerritory, List<string> masterPriority, 
                                        List<string> masterFrequencies,List<string> masterDepartments, List<string> masterInitiator, List<string> masterApprover) //, List<string> masterOccursEvery
        {
            foreach (DataRow row in dtExcelData.Rows)
            {
                string invalidColumn = ValidateRow(row, masterBUs, masterNatureOfCompliance, masterComplianceTypes,
                    masterDrivenBy, masterLawsType, masterTerritory, masterPriority, masterFrequencies,
                    masterDepartments, masterInitiator, masterApprover);// --masterOccursEvery,

                if (!string.IsNullOrEmpty(invalidColumn))
                {
                    //Sending param "invalid" to insert invalid records in "InvalidComplianceMaster" table.
                    dal.CreateInvalidComplianceMaster(CreateComplianceMaster(row));

                    //This can be removed as we are already storing the whole record in the "InvalidComplianceMaster" table.
                    dal.InsertInvalidRecords(CreateInvalidRecord(row, invalidColumn));
                }
                else
                {
                    //sending param "valid" to insert valid records in compliancemaster table.
                    dal.UploadComplianceMaster(CreateComplianceMaster(row));
                }
            }
        }

        private string ValidateRow(DataRow row, List<string> masterBUs, List<string> masterNatureOfCompliance,List<string> masterComplianceTypes, 
                                    List<string> masterDrivenBy, List<string> masterLawsType, List<string> masterTerritory, List<string> masterPriority, 
                                    List<string> masterFrequencies,List<string> masterDepartments, List<string> masterInitiator,List<string> masterApprover)//List<string> masterOccursEvery, 
        {
            var columnMapping = new Dictionary<string, (int index, List<string> masterList)>
            {
                { "Business Unit", (1, masterBUs) },
                { "NatureOfCompliance", (2, masterNatureOfCompliance) },
                { "ComplianceType", (3, masterComplianceTypes) },
                { "Driven By", (4, masterDrivenBy) },
                { "Laws Type", (7, masterLawsType) },
                { "Territory", (8, masterTerritory) },
                { "Priority", (11, masterPriority) },
                { "Frequency", (12, masterFrequencies) },
                { "Department", (19, masterDepartments) },
                { "Initiator", (20, masterInitiator) },
                { "Approver", (21, masterApprover) }
            };
            foreach (var kvp in columnMapping)
            {
                string value = row[kvp.Value.index] != DBNull.Value ? row[kvp.Value.index].ToString().Trim().ToUpper() : string.Empty;
                if (!kvp.Value.masterList.Contains(value.ToUpper()))
                    return kvp.Key;
            }
            return string.Empty;
        }

        
        private InvalidRecordss CreateInvalidRecord(DataRow row, string invalidColumn)
        {

            return new InvalidRecordss
            {
                InvalidColumn = invalidColumn,
                ErrorMessage = row[0]?.ToString() ?? string.Empty,
                RowNumber = 0,
            };
        }

        private ComplianceMasterr CreateComplianceMaster(DataRow row)
        {
            return new ComplianceMasterr
            {
                ComplianceRef = row[0]?.ToString().Trim() ?? string.Empty,
                BusinessUnitName = row[1]?.ToString().Trim() ?? string.Empty,
                NatureOfComplianceName = row[2]?.ToString().Trim() ?? string.Empty,
                ComplianceTypeName = row[3]?.ToString().Trim() ?? string.Empty,
                DrivenByName = row[4]?.ToString().Trim() ?? string.Empty,
                ActSectionReference = row[5]?.ToString().Trim() ?? string.Empty,
                ClauseRef = row[6]?.ToString().Trim() ?? string.Empty,
                LawName = row[7]?.ToString().Trim() ?? string.Empty,
                Territory = row[8]?.ToString().Trim() ?? string.Empty,
                DetailsOfComplianceRequirements = row[9]?.ToString().Trim() ?? string.Empty,
                NonCompliancePenalty = row[10]?.ToString().Trim() ?? string.Empty,
                Priority = row[11]?.ToString().Trim() ?? string.Empty,
                FrequencyName = row[12]?.ToString().Trim() ?? string.Empty,
                EffectiveFrom = row[13] != DBNull.Value ? Convert.ToDateTime(row[13]) : new DateTime(1753, 1, 1),
                StandardDueDate = row[14] != DBNull.Value ? Convert.ToDateTime(row[14]) : new DateTime(1753, 1, 1),
                FirstDueDate = row[15] != DBNull.Value ? Convert.ToDateTime(row[15]) : new DateTime(1753, 1, 1),
                DueOnEvery = row[16] != DBNull.Value ? Convert.ToInt32(row[16]) : 0,
                FormsIfAny = row[17]?.ToString().Trim() ?? string.Empty,
                ActionsToBeTaken = row[18]?.ToString().Trim() ?? string.Empty,
                DepartmentName = row[19]?.ToString().Trim() ?? string.Empty,
                InitiatorName = row[20]?.ToString().Trim() ?? string.Empty,
                ApproverName = row[21]?.ToString().Trim() ?? string.Empty,
            };
        }
        protected void Upload(object sender, EventArgs e)
        {
            DAL dal = new DAL();
            dal.DeleteComplianceMasteTemp();
            string excelPath = Server.MapPath("~/FilesRep/ComplianceDocuments/") + Path.GetFileName(FileUpload1.PostedFile.FileName);
            FileUpload1.SaveAs(excelPath);

            DataTable dtExcelData = LoadExcelData(excelPath);
            RemoveNullRows(dtExcelData);
            List<string> masterBUs = dal.GetMasterData("BU");
            List<string> masterNatureOfCompliance = dal.GetMasterData("NATURE");
            List<string> masterComplianceTypes = dal.GetMasterData("TYPE");
            List<string> masterDrivenBy = dal.GetMasterData("DRIVEN");
            List<string> masterLawsType = dal.GetMasterData("LAW");
            List<string> masterTerritory = dal.GetMasterData("TERRITORY");
            List<string> masterPriority = dal.GetMasterData("PRIORITY");
            List<string> masterFrequencies = dal.GetMasterData("FREQUENCY"); //List<string> masterOccursEvery = dal.GetMasterData("OCCURS");
            List<string> masterDepartments = dal.GetMasterData("DEPT");
            List<string> masterInitiator = dal.GetMasterData("INITIATOR");
            List<string> masterApprover = dal.GetMasterData("APPROVER");

            // Get all unique users from both lists
            //HashSet<string> allUsers = new HashSet<string>(masterInitiator.Concat(masterApprover));

            //dal.updateUserRole(masterInitiator, masterApprover);
            ProcessExcelData(dtExcelData, dal, masterBUs, masterNatureOfCompliance, masterComplianceTypes,masterDrivenBy, masterLawsType, masterTerritory, 
                        masterPriority, masterFrequencies, masterDepartments, masterInitiator, masterApprover); //masterOccursEvery,

            // Inside this procedure we are calling the "[Compliance].[GenerateComplianceDueDates]" 
            // to generate and assign compliances for the whole year.
            dal.UpdateComplianceMaster();
        }
        protected void btnSample_Click(object sender, EventArgs e)
        {
            ExportSample();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSKUSearch.Text))
            {
                ShowMessage("Please Enter a Search term.", true);
                return;
            }
            else
            {
                lblErrorMessage.Visible = false;
                LoadcompMaster(txtSKUSearch.Text.ToUpper());
            }
        }
        protected void btnAll_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Visible = false;
            LoadcompMaster("All");
        }
        private void ExportSample()
        {
            string excelName = "ComplianceMaster_Template.xls";
            string filepath = ServerMapPath(excelName);
            HttpResponse res = GetHttpResponse();
            res.Clear();
            res.AppendHeader("content-disposition", "attachment; filename=" + excelName);
            res.ContentType = "application/octet-stream";
            res.WriteFile(filepath);
            res.Flush();
            res.End();
        }
        public static HttpResponse GetHttpResponse()
        {
            return HttpContext.Current.Response;
        }
        public string ServerMapPath(string path)
        {
            string path1 = Server.MapPath("~/Templates/" + path);
            return path1;
        }
        public override void VerifyRenderingInServerForm(Control control){}
        protected void gvComplDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCompliances.PageIndex = e.NewPageIndex;
            LoadcompMaster("All");
        }
        //protected void btnExport_Click(object sender, EventArgs e)
        //{
        //    DAL dal = new DAL();
        //    var varCatalogue = dal.LoadComplianceMaster("All",0);
        //    ExcelPackage excel = new ExcelPackage();
        //    var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
        //    workSheet.TabColor = System.Drawing.Color.Black;
        //    workSheet.DefaultRowHeight = 12;
        //    workSheet.Row(1).Height = 20;
        //    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    workSheet.Row(1).Style.Font.Bold = true;
        //    workSheet.Cells[1, 1].Value = "COMPLIANCE REF";
        //    workSheet.Cells[1, 2].Value = "BUSINESS UNIT";
        //    workSheet.Cells[1, 3].Value = "COMPLIANCE NATURE";
        //    workSheet.Cells[1, 4].Value = "COMPLIANCE TYPE";
        //    workSheet.Cells[1, 5].Value = "DRIVEN BY";
        //    workSheet.Cells[1, 6].Value = "ACT SECTION";
        //    workSheet.Cells[1, 7].Value = "LAW TYPE";
        //    workSheet.Cells[1, 8].Value = "TERRITORY";
        //    workSheet.Cells[1, 9].Value = "DETAILS";
        //    workSheet.Cells[1, 10].Value = "PENALTY";
        //    workSheet.Cells[1, 11].Value = "PRIORITY";
        //    workSheet.Cells[1, 12].Value = "FREQUENCY";
        //    workSheet.Cells[1, 13].Value = "EFFECTIVE FROM";
        //    workSheet.Cells[1, 14].Value = "EFFECTIVE TILL";
        //    workSheet.Cells[1, 15].Value = "FIRST DUE DATE";
        //    workSheet.Cells[1, 16].Value = "DUE ON EVERY";
        //    workSheet.Cells[1, 17].Value = "FORMS IF ANY";
        //    workSheet.Cells[1, 18].Value = "ACTIONS TO BE TAKEN";
        //    workSheet.Cells[1, 19].Value = "DEPARTMENT";
        //    workSheet.Cells[1, 20].Value = "PREPARER";
        //    workSheet.Cells[1, 21].Value = "APPROVER";
        //    int recordIndex = 2;
        //    foreach (var _cat in varCatalogue)
        //    {
        //        workSheet.Cells[recordIndex, 1].Value = _cat.ComplianceRef.Trim();
        //        workSheet.Cells[recordIndex, 2].Value = _cat.BusinessUnitName.Trim();
        //        workSheet.Cells[recordIndex, 3].Value = _cat.NatureOfComplianceName.Trim();
        //        workSheet.Cells[recordIndex, 4].Value = _cat.ComplianceTypeName;
        //        workSheet.Cells[recordIndex, 5].Value = _cat.DrivenByName;
        //        workSheet.Cells[recordIndex, 6].Value = _cat.ActSectionReference;
        //        workSheet.Cells[recordIndex, 7].Value = _cat.LawName;
        //        workSheet.Cells[recordIndex, 8].Value = _cat.Territory;
        //        workSheet.Cells[recordIndex, 9].Value = _cat.DetailsOfComplianceRequirements;
        //        workSheet.Cells[recordIndex, 10].Value = _cat.NonCompliancePenalty;
        //        workSheet.Cells[recordIndex, 11].Value = _cat.Priority;
        //        workSheet.Cells[recordIndex, 12].Value = _cat.FrequencyName;
        //        workSheet.Cells[recordIndex, 13].Value = _cat.EffectiveFrom;
        //        workSheet.Cells[recordIndex, 14].Value = _cat.StandardDueDate;
        //        workSheet.Cells[recordIndex, 15].Value = _cat.FirstDueDate;
        //        workSheet.Cells[recordIndex, 16].Value = _cat.DueOnEvery;
        //        workSheet.Cells[recordIndex, 17].Value = _cat.FormsIfAny;
        //        workSheet.Cells[recordIndex, 18].Value = _cat.ActionsToBeTaken;
        //        workSheet.Cells[recordIndex, 19].Value = _cat.DepartmentName;
        //        workSheet.Cells[recordIndex, 20].Value = _cat.InitiatorName;
        //        workSheet.Cells[recordIndex, 21].Value = _cat.ApproverName;
        //        recordIndex++;
        //    }
        //    for (int i = 1; i < 22; i++)
        //    {
        //        workSheet.Column(i).Width = 40;
        //    }
        //    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
        //    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer
        //    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local
        //    string excelName = "ComplianceMasterSheet_" + localTime.ToString("dd MMM yy");
        //    Response.Clear();
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        excel.SaveAs(memoryStream);
        //        memoryStream.WriteTo(Response.OutputStream);
        //        memoryStream.Close();
        //    }
        //    Response.Flush();
        //    Response.End();
        //}
        protected void btnExport_Click(object sender, EventArgs e)
        {
            DAL dal = new DAL();
            var varCatalogue = dal.LoadComplianceMaster("All", 0);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.TabColor = XLColor.Black;
                worksheet.RowHeight = 12;
                worksheet.Row(1).Height = 20;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Cell("A1").Value = "COMPLIANCE REF";
                worksheet.Cell("B1").Value = "BUSINESS UNIT";
                worksheet.Cell("C1").Value = "COMPLIANCE NATURE";
                worksheet.Cell("D1").Value = "COMPLIANCE TYPE";
                worksheet.Cell("E1").Value = "DRIVEN BY";
                worksheet.Cell("F1").Value = "ACT SECTION";
                worksheet.Cell("G1").Value = "LAW TYPE";
                worksheet.Cell("H1").Value = "TERRITORY";
                worksheet.Cell("I1").Value = "DETAILS";
                worksheet.Cell("J1").Value = "PENALTY";
                worksheet.Cell("K1").Value = "PRIORITY";
                worksheet.Cell("L1").Value = "FREQUENCY";
                worksheet.Cell("M1").Value = "EFFECTIVE FROM";
                worksheet.Cell("N1").Value = "EFFECTIVE TILL";
                worksheet.Cell("O1").Value = "FIRST DUE DATE";
                worksheet.Cell("P1").Value = "DUE ON EVERY";
                worksheet.Cell("Q1").Value = "FORMS IF ANY";
                worksheet.Cell("R1").Value = "ACTIONS TO BE TAKEN";
                worksheet.Cell("S1").Value = "DEPARTMENT";
                worksheet.Cell("T1").Value = "PREPARER";
                worksheet.Cell("U1").Value = "APPROVER";
                int recordIndex = 2;
                foreach (var _cat in varCatalogue)
                {
                    worksheet.Cell(recordIndex, 1).Value = _cat.ComplianceRef.Trim();
                    worksheet.Cell(recordIndex, 2).Value = _cat.BusinessUnitName.Trim();
                    worksheet.Cell(recordIndex, 3).Value = _cat.NatureOfComplianceName.Trim();
                    worksheet.Cell(recordIndex, 4).Value = _cat.ComplianceTypeName;
                    worksheet.Cell(recordIndex, 5).Value = _cat.DrivenByName;
                    worksheet.Cell(recordIndex, 6).Value = _cat.ActSectionReference;
                    worksheet.Cell(recordIndex, 7).Value = _cat.LawName;
                    worksheet.Cell(recordIndex, 8).Value = _cat.Territory;
                    worksheet.Cell(recordIndex, 9).Value = _cat.DetailsOfComplianceRequirements;
                    worksheet.Cell(recordIndex, 10).Value = _cat.NonCompliancePenalty;
                    worksheet.Cell(recordIndex, 11).Value = _cat.Priority;
                    worksheet.Cell(recordIndex, 12).Value = _cat.FrequencyName;
                    worksheet.Cell(recordIndex, 13).Value = _cat.EffectiveFrom;
                    worksheet.Cell(recordIndex, 14).Value = _cat.StandardDueDate;
                    worksheet.Cell(recordIndex, 15).Value = _cat.FirstDueDate;
                    worksheet.Cell(recordIndex, 16).Value = _cat.DueOnEvery;
                    worksheet.Cell(recordIndex, 17).Value = _cat.FormsIfAny;
                    worksheet.Cell(recordIndex, 18).Value = _cat.ActionsToBeTaken;
                    worksheet.Cell(recordIndex, 19).Value = _cat.DepartmentName;
                    worksheet.Cell(recordIndex, 20).Value = _cat.InitiatorName;
                    worksheet.Cell(recordIndex, 21).Value = _cat.ApproverName;
                    recordIndex++;
                }

                for (int i = 1; i <= 21; i++)
                {
                    worksheet.Column(i).Width = 40;
                }

                DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local
                string excelName = "ComplianceMasterSheet_" + localTime.ToString("dd MMM yy");
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