using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Image = iTextSharp.text.Image;

namespace ComplianceManager
{
	public partial class DelayedCompliances : System.Web.UI.Page
	{
        Int32 userId = 0;
        int initialYear = 0;
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
                Helper.BindDropdown(ddlDepartmentFunction, dal.PopulateDepartment("Department"), "DepartmentName", "DeptID", "All");
                for (int i = 1; i <= 12; i++)
                {
                    //string monthName = new DateTime(2000, i, 1).ToString("MMM"); // Use any year
                    //ddlMonth.Items.Add(new ListItem(monthName, i.ToString()));

                    ddlMonth.Items.Add(new System.Web.UI.WebControls.ListItem(i.ToString(), i.ToString()));
                }
                ddlMonth.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All", "0"));
                ddlMonth.SelectedIndex = 0;

                int currentYear = DateTime.Now.Year;
                //for (int i = -2; i <= 1; i++) //ascending order
                //{
                //    int financialYear = currentYear + i;
                //    ddlYear.Items.Add(new ListItem(financialYear.ToString(), financialYear.ToString()));
                //}
                for (int i = 1; i >= -2; i--) //descending order
                {
                    int endYear = currentYear + 1;
                    string financialYearText = $"{currentYear}-{endYear}";

                    ddlYear.Items.Add(new System.Web.UI.WebControls.ListItem(financialYearText, currentYear.ToString()));
                    currentYear--;
                    //int financialYear = currentYear + i;
                    //ddlYear.Items.Add(new System.Web.UI.WebControls.ListItem(financialYear.ToString(), financialYear.ToString()));
                }
                ddlYear.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All", "0"));
                ddlYear.SelectedIndex = 0;
                ViewState["InitialYear"] = currentYear;
                //ddlPeriod.Items.Clear();
                //ddlPeriod.DataSource = dal.GetUsedPeriodsFromDB(); // Fetch all period ["04/2024", "04/2025"]
                //ddlPeriod.DataBind();
                //ddlPeriod.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Period", string.Empty));
                //ddlPeriod.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Helper.ShowMessage($"Error loading dropdowns: {ex.Message}", true, lblErrorMessage);
            }
        }
        protected void Display(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showForgotModal", "showModal();", true);
            int rowIndex = ((sender as System.Web.UI.WebControls.Button).NamingContainer as GridViewRow).RowIndex;
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
        protected void gvCompliances_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            int deptId = ViewState["DeptId"] != null ? Convert.ToInt32(ViewState["DeptId"]) : 0;
            int month = ViewState["Month"] != null ? Convert.ToInt32(ViewState["Month"]) : 0;
            int year = ViewState["Year"] != null ? Convert.ToInt32(ViewState["Year"]) : 0;
            //string period = (string)(ViewState["Period"] != null ? ViewState["Period"] : "");
            gvCompliances.PageIndex = e.NewPageIndex;
            BindGridFiltered(deptId, month,year );
        }
        protected void btnLoad_Click(object sender, EventArgs e)
        {
          
            int deptId = Helper.dropdownValue(ddlDepartmentFunction);
            int month = Helper.dropdownValue(ddlMonth); //Convert.ToInt32(ddlMonth.SelectedItem.Value);
            int year = Helper.dropdownValue(ddlYear); //Convert.ToInt32(ddlYear.SelectedItem.Value);
            int startyear = 0;
            if (year==0)
            {
                startyear = (int)ViewState["InitialYear"];
            }
            //string period = ddlPeriod.SelectedValue;
            // Store filter parameters in ViewState
            ViewState["DeptId"] = deptId;
            ViewState["Month"] = month;
            ViewState["Year"] = year;
            //ViewState["Period"] = period;
            //if (ddlYear.SelectedIndex == 0)// && ddlPeriod.SelectedIndex ==0)
            //{
            //    Helper.ShowMessage("Please select year.", true, lblErrorMessage); //SelectDeptPeriod
            //    return;
            //}
            lblErrorMessage.Visible = false;
            BindGridFiltered(deptId, month, year, startyear);// period);
        }
        
        protected void btnAll_Click(object sender, EventArgs e) //RESET BUTTON
        {
            ddlDepartmentFunction.SelectedIndex = 0;
            //ddlPeriod.SelectedIndex = 0;
            ddlMonth.SelectedIndex = 0;
            ddlYear.SelectedIndex = 0;
            lblErrorMessage.Visible = false;
            gvCompliances.Visible = false;
        }

        //Load Filtered records
        private void BindGridFiltered(int deptId = 0, int month=0, int year=0, int startyear=0)//string period = "")
        {
            try
            {
                DAL dal = new DAL();
                
                ComplianceDelayedResult compliances = dal.LoadComplianceDelayed(deptId, month, year,startyear);// period);
                // Bind first GridView (Overdue Compliances)
                gvCompliances.DataSource = compliances.OverdueCompliances;
                gvCompliances.DataBind();

                // Bind second GridView (Summary)
                // Convert single ComplianceSummary to a list for binding
                GridView2.DataSource = new[] { compliances.Summary };
                GridView2.DataBind();
                
            }
            catch (Exception ex)
            {
                Helper.ShowMessage($"Error loading compliance data: {ex.Message}", true, lblErrorMessage); //ErrorData
            }
        }

        private void GenerateCertificate(int deptId, int month, int year)
        {
            try
            {
                DAL dal = new DAL();
                int startyear = (int)ViewState["InitialYear"];

                ComplianceDelayedResult compliances = dal.LoadComplianceDelayed(deptId, month, year, startyear);// period);

                using (MemoryStream ms = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4, 36f, 36f, 10f, 10f);
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();

                    PdfPTable table = new PdfPTable(20);
                    table.WidthPercentage = 100;

                    AddHeader(table);
                    string period = month + "-" + year;
                    AddCertificateBody(table, compliances, period);

                    document.Add(table);
                    document.Close();
                    writer.Close();

                    HttpContext.Current.Response.ContentType = "application/pdf";
                    HttpContext.Current.Response.AddHeader("content-disposition", $"attachment;filename=DelayedCompliance_{GetLocalTIme():dd-MM-yyyy}.pdf");
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                    HttpContext.Current.Response.OutputStream.Flush();
                    HttpContext.Current.Response.Flush();
                }
            }
            catch (Exception ex)
            {
                Helper.ShowMessage($"Error generating pdf: {ex.Message}", true, lblErrorMessage);
            }
        }

        private void AddHeader(PdfPTable table)
        {
            var fontHeading = FontFactory.GetFont("Arial", 17, Font.BOLD, BaseColor.BLACK);
            var fontAddress = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);
            string logoPath = HttpContext.Current.Server.MapPath("~/image/Logo/delhi-golf-course-logo.png");
            Image logo = Image.GetInstance(logoPath);
            logo.ScaleToFit(80f, 80f);

            AddBlankRow(table, 20);
            AddBlankRow(table, 20);

            PdfPCell cell = new PdfPCell(new Phrase("")) { Colspan = 6, Rowspan = 7, Border = Rectangle.NO_BORDER };
            cell.AddElement(logo);
            table.AddCell(cell);

            table.AddCell(new PdfPCell(new Phrase("")) { Colspan = 4, Rowspan = 7, Border = Rectangle.NO_BORDER });
            table.AddCell(CreateCell("Delhi Golf Club", fontHeading, 10, Element.ALIGN_RIGHT, Element.ALIGN_MIDDLE));
            table.AddCell(CreateCell("Delhi Golf Club, Dr Zakir Hussain Road", fontAddress, 10, Element.ALIGN_RIGHT));
            table.AddCell(CreateCell("New Delhi, South Delhi, Delhi, 110003.", fontAddress, 10, Element.ALIGN_RIGHT));
            table.AddCell(CreateCell("E-Mail: general@delhigolfclub.org", fontAddress, 10, Element.ALIGN_RIGHT));
            table.AddCell(CreateCell("Website :  www.delhigolfclub.org", fontAddress, 10, Element.ALIGN_RIGHT));
            table.AddCell(CreateCell("Tel :  +91 011-24307100 , +91 011-24362235", fontAddress, 10, Element.ALIGN_RIGHT));
            table.AddCell(CreateCell("+91 011-24360002", fontAddress, 10, Element.ALIGN_RIGHT));

            AddBlankRow(table, 20);
            AddUnderline(table, 20);
            AddBlankRow(table, 20);
            AddBlankRow(table, 20);

            string poDate = "Date : " + GetLocalTIme().ToString("dd-MM-yyyy");
            table.AddCell(CreateCell(poDate, fontAddress, 20));
            AddBlankRow(table, 20);
            AddBlankRow(table, 20);
        }

        private void AddCertificateBody(PdfPTable table, ComplianceDelayedResult compliances, string periodicity)
        {
            var fontDetail = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK);
            var fontHeading = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK);
            var fontHeadingRed = FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.RED);

            

            table.AddCell(CreateCell("Compliance Delayed for the period " + periodicity, fontHeading, 20));
            AddBlankRow(table, 20);
                
            table.AddCell(CreateCell("Total Compliance for the period : " + compliances.Summary.TotalCompliances, fontHeading, 20));
            AddBlankRow(table, 20);
            table.AddCell(CreateCell("Complete Compliances : " + compliances.Summary.SentForApproval, fontHeading, 20));
            AddBlankRow(table, 20);
            table.AddCell(CreateCell("Pending Compliances : " + compliances.Summary.PendingCompliances, fontHeading, 20));
            AddBlankRow(table, 20);
            table.AddCell(CreateCell("Overdue Compliances : " + compliances.Summary.OverdueCompliances, fontHeadingRed, 20));
            AddBlankRow(table, 20);

            table.AddCell(CreateCellWithBorder("Comp Ref.", fontHeading, 2, Element.ALIGN_CENTER));
            table.AddCell(CreateCellWithBorder("Department", fontHeading, 3, Element.ALIGN_CENTER));
            table.AddCell(CreateCellWithBorder("Preparer", fontHeading, 3, Element.ALIGN_CENTER));
            table.AddCell(CreateCellWithBorder("Due Date", fontHeading, 2, Element.ALIGN_CENTER));
            table.AddCell(CreateCellWithBorder("Details", fontHeading, 7, Element.ALIGN_CENTER));
            table.AddCell(CreateCellWithBorder("Remarks", fontHeading, 3, Element.ALIGN_CENTER));

            foreach (var comp in compliances.OverdueCompliances)
            {
                table.AddCell(CreateCellWithBorder(comp.ComplianceRef, fontDetail, 2));
                table.AddCell(CreateCellWithBorder(comp.DepartmentName, fontDetail, 3));
                table.AddCell(CreateCellWithBorder(comp.InitiatorName, fontDetail, 3));
                table.AddCell(CreateCellWithBorder(comp.ComplianceDueDate.ToString("dd-MM-yyyy"), fontDetail, 2));
                table.AddCell(CreateCellWithBorder(comp.DetailsOfComplianceRequirements, fontDetail, 7));
                table.AddCell(CreateCellWithBorder(comp.Remarks, fontDetail, 3, Element.ALIGN_RIGHT));
            }

            AddBlankRow(table, 20);
            AddBlankRow(table, 20);
        }

        private PdfPCell CreateCell(string text, Font font, int colspan = 1, int horizontalAlign = Element.ALIGN_LEFT, int verticalAlign = Element.ALIGN_MIDDLE)
        {
            return new PdfPCell(new Phrase(text, font))
            {
                Colspan = colspan,
                HorizontalAlignment = horizontalAlign,
                VerticalAlignment = verticalAlign,
                Border = Rectangle.NO_BORDER,
                PaddingTop = 2f,
                PaddingBottom = 4f,
                PaddingLeft = 2f,
                PaddingRight = 2f
            };
        }

        private PdfPCell CreateCellWithBorder(string text, Font font, int colspan = 1, int horizontalAlign = Element.ALIGN_LEFT, int verticalAlign = Element.ALIGN_MIDDLE)
        {
            return new PdfPCell(new Phrase(text, font))
            {
                Colspan = colspan,
                HorizontalAlignment = horizontalAlign,
                VerticalAlignment = verticalAlign,
                Border = Rectangle.BOX,
                PaddingTop = 2f,
                PaddingBottom = 4f,
                PaddingLeft = 2f,
                PaddingRight = 2f
            };
        }

        private void AddBlankRow(PdfPTable table, int colspan)
        {
            table.AddCell(new PdfPCell(new Phrase(" "))
            {
                Colspan = colspan,
                MinimumHeight = 10f,
                Border = Rectangle.NO_BORDER
            });
        }

        private void AddUnderline(PdfPTable table, int colspan)
        {
            table.AddCell(new PdfPCell(new Phrase(" "))
            {
                Colspan = colspan,
                Border = Rectangle.BOTTOM_BORDER,
                MinimumHeight = 1f
            });
        }

        public DateTime GetLocalTIme()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        }


        //private void GenerateCertificate(int deptId, string periodicity)
        //{
        //    try
        //    {
        //        DAL dal = new DAL();
        //        List<ComplianceMasterr> compliances = new List<ComplianceMasterr>();
        //        compliances = dal.LoadComplianceDelayed(deptId, periodicity);

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            PdfPTable table = new PdfPTable(20);
        //            float left = 0;
        //            float right = 0;
        //            float top = 20;
        //            float bottom = 10;
        //            Document document = new Document(PageSize.A4, left, right, top, bottom);
        //            //document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
        //            PdfWriter writer = PdfWriter.GetInstance(document, ms);
        //            document.Open();
        //            Font fntTableNormalAddress = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);//PdfFont
        //            Font fntTableNormal = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);
        //            Font fntTableNormalDet = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK);
        //            Font fntTableNormalHeading = FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK);
        //            Font fntTableHeading = FontFactory.GetFont("Arial", 17, Font.BOLD, BaseColor.BLACK);
        //            Font fntTableUnderLine = FontFactory.GetFont("Arial", 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

        //            string logoPath = HttpContext.Current.Server.MapPath("~/image/Logo/delhi-golf-course-logo.png");//LogoPath
        //            Image logo = Image.GetInstance(logoPath);
        //            logo.ScaleToFit(80f, 80f);
        //            DateTime poLocaldate = GetLocalTIme();

        //            AddBlankRow(table, 20);
        //            AddBlankRow(table, 20);
        //            PdfPCell cell = new PdfPCell(new Phrase(""));
        //            cell.Colspan = 6;
        //            cell.Rowspan = 7;
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.Border = Rectangle.NO_BORDER;
        //            cell.AddElement(logo);
        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase(""));
        //            cell.Colspan = 4;
        //            cell.Rowspan = 7;
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.Border = Rectangle.NO_BORDER;
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("Delhi Golf Club", fntTableHeading)); //PdfHeading
        //            cell.Colspan = 10;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            cell.Border = Rectangle.NO_BORDER;
        //            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //            table.AddCell(cell);

        //            table.AddCell(CreateCell("Delhi Golf Club, Dr Zakir Hussain Road", fntTableNormalAddress, 10, Element.ALIGN_RIGHT)); //PdfAddressLine1
        //            table.AddCell(CreateCell("New Delhi, South Delhi, Delhi, 110003.", fntTableNormalAddress, 10, Element.ALIGN_RIGHT)); //PdfAddressLine2
        //            table.AddCell(CreateCell("E-Mail: general@delhigolfclub.org", fntTableNormalAddress, 10, Element.ALIGN_RIGHT));//PdfEmail
        //            table.AddCell(CreateCell("Website :  www.delhigolfclub.org", fntTableNormalAddress, 10, Element.ALIGN_RIGHT)); //PdfWebsite
        //            table.AddCell(CreateCell("Tel :  +91 011-24307100 , +91 011-24362235", fntTableNormalAddress, 10, Element.ALIGN_RIGHT));//PdfTelephone1
        //            table.AddCell(CreateCell("+91 011-24360002", fntTableNormalAddress, 10, Element.ALIGN_RIGHT));//PdfTelephone2
        //            AddBlankRow(table, 20);
        //            AddUnderline(table, 20);
        //            AddBlankRow(table, 20);
        //            AddBlankRow(table, 20);

        //            string poDate = "Date : " + poLocaldate.ToString("dd-MM-yyyy");

        //            table.AddCell(CreateCell(poDate, fntTableNormal, 20, Element.ALIGN_LEFT));

        //            AddBlankRow(table, 20);
        //            AddBlankRow(table, 20);
        //            table.AddCell(CreateCell("Compliance Delayed for the period " + periodicity, fntTableNormal, 20, Element.ALIGN_LEFT));
        //            AddBlankRow(table, 20);
        //            AddBlankRow(table, 20);

        //            AddBlankRow(table, 20);

        //            cell = new PdfPCell(new Phrase("Comp Ref.", fntTableNormalHeading));
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.Colspan = 2;
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("Department", fntTableNormalHeading));
        //            cell.Colspan = 3;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("Preparer", fntTableNormalHeading));
        //            cell.Colspan = 3;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("Due Date", fntTableNormalHeading));
        //            cell.Colspan = 2;
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("Details", fntTableNormalHeading));
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.Colspan = 7;
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("Remarks", fntTableNormalHeading));
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.Colspan = 3;
        //            table.AddCell(cell);

        //            //SAVE certificate number and get certificate Id
        //            int counter = 0;
        //            foreach (var comp in compliances)
        //            {
        //                counter++;
        //                table.AddCell(CreateCellWithBorder(comp.ComplianceRef.ToString(), fntTableNormalDet, 2, Element.ALIGN_LEFT));
        //                table.AddCell(CreateCellWithBorder(comp.DepartmentName, fntTableNormalDet, 3, Element.ALIGN_LEFT));
        //                table.AddCell(CreateCellWithBorder(comp.InitiatorName, fntTableNormalDet, 3, Element.ALIGN_LEFT));
        //                table.AddCell(CreateCellWithBorder(comp.ComplianceDueDate.ToString("dd-MM-yyyy"), fntTableNormalDet, 2, Element.ALIGN_LEFT));//ComplianceAREA TODO
        //                table.AddCell(CreateCellWithBorder(comp.DetailsOfComplianceRequirements, fntTableNormalDet, 7, Element.ALIGN_LEFT));
        //                table.AddCell(CreateCellWithBorder(comp.Remarks, fntTableNormalDet, 3, Element.ALIGN_RIGHT));

        //            }
        //            AddBlankRow(table, 20);
        //            AddBlankRow(table, 20);

        //            document.Add(table);
        //            document.Close();

        //            writer.Close();

        //            // Send the PDF as a response to the user's browser
        //            HttpContext.Current.Response.ContentType = "application/pdf";
        //            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=DelayedCompliance_" + poLocaldate.ToString("dd-MM-yyyy").ToUpper() + ".pdf");
        //            HttpContext.Current.Response.Buffer = true;
        //            HttpContext.Current.Response.Clear();
        //            HttpContext.Current.Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
        //            HttpContext.Current.Response.OutputStream.Flush();
        //            HttpContext.Current.Response.Flush();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowMessage($"Error generating pdf: {ex.Message}", true);
        //    }
        //}
        //public DateTime GetLocalTIme()
        //{
        //    var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
        //    return now;
        //}
        //public void AddBlankRow(PdfPTable table, int colspan)
        //{
        //    PdfPCell cell = CreateCell("", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK), colspan, Element.ALIGN_CENTER);
        //    cell.MinimumHeight = 10f;
        //    table.AddCell(cell);
        //}
        //public PdfPCell CreateCell(string text, iTextSharp.text.Font font, int colspan = 1, int horizontalAlignment = Element.ALIGN_LEFT)
        //{
        //    PdfPCell cell = new PdfPCell(new Phrase(text, font));
        //    cell.Colspan = colspan;
        //    cell.HorizontalAlignment = horizontalAlignment;
        //    cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
        //    return cell;
        //}
        //public void AddUnderline(PdfPTable table, int colspan)
        //{
        //    PdfPCell cell = new PdfPCell(new Phrase(""));
        //    cell.Colspan = colspan;
        //    cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
        //    table.AddCell(cell);
        //}
        //public PdfPCell CreateCellWithBorder(string text, iTextSharp.text.Font font, int colspan = 1, int horizontalAlignment = Element.ALIGN_LEFT)
        //{
        //    PdfPCell cell = new PdfPCell(new Phrase(text, font));
        //    cell.Colspan = colspan;
        //    cell.HorizontalAlignment = horizontalAlignment;
        //    return cell;
        //}

        //private Font GetFont(int size, int style = Font.NORMAL)
        //{
        //    return FontFactory.GetFont("Arial", size, style, BaseColor.BLACK);
        //}
        protected void btnExport_Click(object sender, EventArgs e)
        {

            if (ddlDepartmentFunction.SelectedIndex == 0)
            {
                Helper.ShowMessage("Please select at least one from Department & Period.", true, lblErrorMessage);
                return;
            }

            lblErrorMessage.Visible = false;

            // Retrieve filter parameters from ViewState
            int deptId = ViewState["DeptId"] != null ? Convert.ToInt32(ViewState["DeptId"]) : 0;
            //string period = (string)(ViewState["Period"] != null ? ViewState["Period"] : "");
            int month = ViewState["Month"] != null ? Convert.ToInt32(ViewState["Month"]) : 0;
            int year = ViewState["Year"] != null ? Convert.ToInt32(ViewState["Year"]) : 0;
            GenerateCertificate(deptId, month, year);
            
            //DAL dal = new DAL();
            //var varCatalogue = dal.LoadComplianceDelayed(deptId, period);
            //ExcelPackage excel = new ExcelPackage();
            //var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            //workSheet.TabColor = System.Drawing.Color.Black;
            //workSheet.DefaultRowHeight = 12;
            //workSheet.Row(1).Height = 20;
            //workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //workSheet.Row(1).Style.Font.Bold = true;
            //workSheet.Cells[1, 1].Value = "COMPLIANCE ID";
            //workSheet.Cells[1, 2].Value = "COMPLIANCE REF";
           
            //workSheet.Cells[1, 3].Value = "DEPARTMENT";
            //workSheet.Cells[1, 4].Value = "DUEDATE";
            //workSheet.Cells[1, 5].Value = "DETAILS";
            //workSheet.Cells[1, 6].Value = "REMARKS";
         
            //int recordIndex = 2;
            //foreach (var _cat in varCatalogue)
            //{
            //    workSheet.Cells[recordIndex, 1].Value = _cat.ComplianceID;
            //    workSheet.Cells[recordIndex, 2].Value = _cat.ComplianceRef.Trim();
            //    workSheet.Cells[recordIndex, 3].Value = _cat.DepartmentName;
            //    workSheet.Cells[recordIndex, 4].Value = _cat.ComplianceDueDate;
            //    workSheet.Cells[recordIndex, 5].Value = _cat.DetailsOfComplianceRequirements;
            //    workSheet.Cells[recordIndex, 6].Value = _cat.Remarks;
            //    recordIndex++;
            //}
            //// Define date columns (adjust column numbers based on your data)
            //int[] dateColumns = { 4 }; // Example: EffectiveFrom, StandardDueDate, CreationDate

            //// Format date columns
            //foreach (int col in dateColumns)
            //{
            //    workSheet.Column(col).Style.Numberformat.Format = "dd-mm-yyyy"; // Or your preferred format
            //}

            //workSheet.Column(1).Width = 10;
            //workSheet.Column(2).Width = 10;
            //workSheet.Column(3).Width = 20;
            //workSheet.Column(4).Width = 20;
            //workSheet.Column(5).Width = 30;
            //workSheet.Column(6).Width = 20;
            //           DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            //DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer
            //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            //DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to local
            //string excelName = "ComplianceClosedDelayedReport_" + localTime.ToString("dd_MMM_yy");
            //Response.Clear();
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AddHeader("content-disposition", "attachment; filename=" + excelName + ".xlsx");
            //using (var memoryStream = new MemoryStream())
            //{
            //    excel.SaveAs(memoryStream);
            //    memoryStream.WriteTo(Response.OutputStream);
            //    memoryStream.Close();
            //}
            //Response.Flush();
            //Response.End();
        }
    }
}