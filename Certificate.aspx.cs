using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Org.BouncyCastle.Ocsp;
using Image = iTextSharp.text.Image;

namespace ComplianceManager
{
    public partial class Certificate : System.Web.UI.Page
    {
        Int32 userId = 0;
        Int32 generatedByUserId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            userId = Convert.ToInt32(Session["UserId"].ToString());
            if (Session["UserName"] == null)
                Response.Redirect("../AuthPages/Login.aspx");

            UpdatePanel updatePanel = (UpdatePanel)Master.FindControl("upMainContent");
            if (updatePanel != null)
            {
                string script = $"var updatePanelClientId = '{updatePanel.ClientID}';";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "setUpdatePanelClientId", script, true);
            }
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnGenerate);
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnSearch);
            if (!Page.IsPostBack)
            {
                BindUserNameDropdown();
            }
        }

        private void BindUserNameDropdown()
        {
            DAL dal = new DAL();
            ddlUserName.Items.Clear();
            //ddlUserName.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select User",string.Empty));
            //ddlUserName.SelectedIndex = 0;
            if (Convert.ToInt32(Session["Role"].ToString()) == 4)
            {
                ddlUserName.DataSource = dal.LoadAssignToDropdown();//GENERATED  BY ID,
                ddlUserName.DataBind();
                ddlUserName.Enabled = true;
                ddlUserName.SelectedIndex = 0;
            }
            else {
                ddlUserName.DataSource = dal.LoadAssignToDropdown(Convert.ToInt32(Session["UserId"].ToString()));//GENERATED  BY ID,
                ddlUserName.DataBind();
                
            }

                ddlPeriod.Items.Clear();
            ddlPeriod.DataSource = dal.GetUsedPeriodsFromDB(); // Fetch all period ["04/2024", "04/2025"]
            ddlPeriod.DataBind();
            ddlPeriod.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Select Period", string.Empty));
            ddlPeriod.SelectedIndex = 0;
        }

        protected void btnSearch_Click(object sender, EventArgs e)  //Load Data Button
        {
            lblErrorMessage.Visible = false;
            
            generatedByUserId = Convert.ToInt32(ddlUserName.SelectedValue);
            string period = ddlPeriod.SelectedValue;
            ViewState["period"] = period;
            refreshdata(generatedByUserId, period);
        }
        public void refreshdata(int generatedByUserId=0, string period="")
        {
            DAL dal = new DAL();
            GridView1.DataSource = dal.LoadExistingCertificate(generatedByUserId, period, 0);
            GridView1.DataBind();
        }
        private void ShowMessage(string message, bool isError)
        {
            lblErrorMessage.Text = message;
            lblErrorMessage.ForeColor = isError ? System.Drawing.Color.FromArgb(0x72, 0x1C, 0x24) : System.Drawing.Color.Green;
            lblErrorMessage.Visible = true;
        }
        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Visible = false;
            if (ddlPeriod.SelectedIndex == 0)
            {
                ShowMessage("Periodicity cannot be empty.", true);
                return;
            }
            string periodicity = ddlPeriod.SelectedItem.Text;
            generatedByUserId = Convert.ToInt32(ddlUserName.SelectedValue);//GeneratedById
            string generatedByUserName = ddlUserName.SelectedItem.Text; //GeneratedBYName

            DateTime currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); // 01-Apr-2025
            DateTime startMonth = currentMonth.AddMonths(-11); // 01-May-2024
            DAL dal = new DAL();
            int usedPeriod = dal.CheckUsedPeriodforUser(userId, periodicity); //If certificate exists in compliancecertificate table.
            if (usedPeriod > 0) 
            {
                ShowMessage("Certificate already generated, Please select a another period.", true);
                return;
            }
            int noComplianceforSelectedPeriod = dal.CheckComplianceforPeriod(generatedByUserId, periodicity);
            if (noComplianceforSelectedPeriod <=0)
            {
                ShowMessage("No Compliance exists for the selected period.", true);
                return;
            }

            // Create directory name based on selected record and current date/time
            string directoryName = $"{generatedByUserName}_{periodicity}";
            string directoryPath = Server.MapPath("~/FilesRep/Certificates/" + generatedByUserName);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            
            // Call GenerateCertificate and get the tuple (serverFilePath, relativePath)
            var (pdfFilePath, relativePath, certificateId) = GenerateCertificate(directoryPath, periodicity, generatedByUserName);
            
            // Check if the generation was successful (i.e., pdfFilePath is not empty)
            if (string.IsNullOrEmpty(pdfFilePath))
            {
                ShowMessage("Certificate not generated, try again.", true);
                return; // Error message is already shown in GenerateCertificate via ShowMessage
            }
            Session["DownloadFilePath"] = relativePath;
            GridView1.DataSource = dal.LoadExistingCertificate(generatedByUserId, periodicity, certificateId);
            GridView1.DataBind();
            //refreshdata(generatedByUserId, periodicity, certificateId);
        }
        private (string serverFilePath, string relativePath, int CertificateId) GenerateCertificate(string directoryPath, string periodicity, string generatedByUserName)
        {
            try
            {
                DAL dal = new DAL();
                List<ComplianceCertificate> compliances = new List<ComplianceCertificate>();
                compliances = dal.LoadMyComplianceCertificate(generatedByUserId, periodicity, "new");
                
                using (MemoryStream ms = new MemoryStream())
                {
                    PdfPTable table = new PdfPTable(20);
                    float left = 0;
                    float right = 0;
                    float top = 20;
                    float bottom = 10;
                    Document document = new Document(PageSize.A4, left, right, top, bottom);
                    //document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                    PdfWriter writer = PdfWriter.GetInstance(document, ms);
                    document.Open();
                    Font fntTableNormalAddress = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);
                    Font fntTableNormal = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);
                    Font fntTableNormalDet = FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK);
                    Font fntTableNormalHeading = FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK);
                    Font fntTableHeading = FontFactory.GetFont("Arial", 17, Font.BOLD, BaseColor.BLACK);
                    Font fntTableUnderLine = FontFactory.GetFont("Arial", 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                    string fileName = "DGC_" + generatedByUserName + "_" + periodicity + ".pdf";

                    string logoPath = HttpContext.Current.Server.MapPath("~/image/Logo/delhi-golf-course-logo.png");
                    Image logo = Image.GetInstance(logoPath);
                    logo.ScaleToFit(80f, 80f);
                    DateTime poLocaldate = GetLocalTIme();

                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    PdfPCell cell = new PdfPCell(new Phrase(""));
                    cell.Colspan = 6;
                    cell.Rowspan = 7;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Border = Rectangle.NO_BORDER;
                    cell.AddElement(logo);
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(""));
                    cell.Colspan = 4;
                    cell.Rowspan = 7;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Border = Rectangle.NO_BORDER;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Delhi Golf Club", fntTableHeading));
                    cell.Colspan = 10;
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Border = Rectangle.NO_BORDER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell);

                    table.AddCell(CreateCell("Delhi Golf Club, Dr Zakir Hussain Road", fntTableNormalAddress, 10, Element.ALIGN_RIGHT));
                    table.AddCell(CreateCell("New Delhi, South Delhi, Delhi, 110003.", fntTableNormalAddress, 10, Element.ALIGN_RIGHT));
                    table.AddCell(CreateCell("E-Mail: general@delhigolfclub.org", fntTableNormalAddress, 10, Element.ALIGN_RIGHT));
                    table.AddCell(CreateCell("Website :  www.delhigolfclub.org", fntTableNormalAddress, 10, Element.ALIGN_RIGHT));
                    table.AddCell(CreateCell("Tel :  +91 011-24307100 , +91 011-24362235", fntTableNormalAddress, 10, Element.ALIGN_RIGHT));
                    table.AddCell(CreateCell("+91 011-24360002", fntTableNormalAddress, 10, Element.ALIGN_RIGHT));
                    AddBlankRow(table, 20);
                    AddUnderline(table, 20);
                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    table.AddCell(CreateCell("Compliance Certificate", fntTableUnderLine, 20, Element.ALIGN_CENTER));
                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    string poDate = "Date : " + poLocaldate.ToString("dd-MM-yyyy");
                    string certificateNo = "DGC_" + generatedByUserId + "_" + periodicity + "_" + poLocaldate.ToString("dd-MM-yyyy");
                    string displayCertificateNo = "Certificate No : DGC_" + periodicity + "_" + poLocaldate.ToString("dd-MM-yyyy");
                    table.AddCell(CreateCell(displayCertificateNo, fntTableNormal, 20, Element.ALIGN_RIGHT));
                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    table.AddCell(CreateCell("To", fntTableNormal, 20, Element.ALIGN_LEFT));
                    table.AddCell(CreateCell("General Committee", fntTableNormal, 20, Element.ALIGN_LEFT));
                    table.AddCell(CreateCell("The Delhi Golf Club", fntTableNormal, 20, Element.ALIGN_LEFT));
                    table.AddCell(CreateCell(poDate, fntTableNormal, 20, Element.ALIGN_LEFT));

                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    table.AddCell(CreateCell("Subject : Compliance Certificate for the period " + periodicity, fntTableNormal, 20, Element.ALIGN_LEFT));
                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    table.AddCell(CreateCell("I hereby certify that “Delhi Golf Club” has complied with all applicable " +
                        "laws, regulations and internal policies for the ", fntTableNormal, 20, Element.ALIGN_LEFT));
                    table.AddCell(CreateCell("said period to the best of my knowledge and " +
                            "based on the examination conducted.", fntTableNormal, 20, Element.ALIGN_LEFT));
                    AddBlankRow(table, 20);
                    table.AddCell(CreateCell("I would like to highlight below exception cases, wherein the requirements " +
                        "have been complied with delay or not", fntTableNormal, 20, Element.ALIGN_LEFT));
                    table.AddCell(CreateCell("complied as of the date:", fntTableNormal, 20, Element.ALIGN_LEFT));
                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);

           
                    cell = new PdfPCell(new Phrase("Comp Ref.", fntTableNormalHeading));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 1;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Type of Compliance", fntTableNormalHeading));
                    cell.Colspan = 3;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Act/Section Ref", fntTableNormalHeading));
                    cell.Colspan = 3;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Clause Ref.", fntTableNormalHeading));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 2;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Compliance Des.", fntTableNormalHeading));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 6;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Type of Non Comp", fntTableNormalHeading));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 3;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Remarks", fntTableNormalHeading));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Colspan = 2;
                    table.AddCell(cell);

                    //SAVE certificate number and get certificate Id
                    int certificateId = dal.SaveComplianceCertificate(periodicity, generatedByUserName, generatedByUserId, certificateNo);


                    int counter = 0;
                    foreach (var comp in compliances)
                    {
                        counter++;
                        //table.AddCell(CreateCellWithBorder(counter.ToString(), fntTableNormalDet, 1, Element.ALIGN_LEFT));
                        table.AddCell(CreateCellWithBorder(comp.ComplainceRef.ToString(), fntTableNormalDet, 1, Element.ALIGN_LEFT));
                        table.AddCell(CreateCellWithBorder(comp.ComplianceTypeName, fntTableNormalDet, 3, Element.ALIGN_LEFT));
                        table.AddCell(CreateCellWithBorder(comp.ActSection, fntTableNormalDet, 3, Element.ALIGN_LEFT));//ComplianceAREA TODO
                        table.AddCell(CreateCellWithBorder(comp.ClauseRef, fntTableNormalDet, 2, Element.ALIGN_LEFT));
                        table.AddCell(CreateCellWithBorder(comp.DetailsOfComplianceReq, fntTableNormalDet, 6, Element.ALIGN_LEFT));
                        table.AddCell(CreateCellWithBorder(comp.TypeOfNonCompliance, fntTableNormalDet, 3, Element.ALIGN_RIGHT));
                        table.AddCell(CreateCellWithBorder(comp.Remarks, fntTableNormalDet, 2, Element.ALIGN_RIGHT));

                        //UPDATE CertificateId in ComplianceDEtails tables to make connection between certificate and compliancedetails
                        dal.UpdateCertificateIdInComplianceDetail(certificateId, comp.ComplainceDetailID);
                    }
                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    AddBlankRow(table, 20);
                    table.AddCell(CreateCell("Mr. abcd xyz", fntTableNormal, 20, Element.ALIGN_LEFT));
                    table.AddCell(CreateCell("The Secretary", fntTableNormal, 20, Element.ALIGN_LEFT));
                    table.AddCell(CreateCell("The Delhi Golf Club", fntTableNormal, 20, Element.ALIGN_LEFT));
                    AddBlankRow(table, 20);
                    document.Add(table);
                    document.Close();

                    writer.Close();

                    string tempFilePath = "";
                    string tempFileName = "Certificate_DGC_" + generatedByUserName + "_" + poLocaldate.ToString("dd-MM-yyyy").ToUpper() + ".pdf";
                    //int certificateId = dal.SaveComplianceCertificateFilePath(tempFilePath, periodicity, generatedByUserName, generatedByUserId, tempFileName);

                    string finalFileName = certificateId + "_Certificate_DGC_" + generatedByUserName + "_" + poLocaldate.ToString("dd-MM-yyyy").ToUpper() + ".pdf";
                    string serverFilePath = Path.Combine(directoryPath, finalFileName);
                    File.WriteAllBytes(serverFilePath, ms.ToArray());

                    string relativePath = "~/FilesRep/Certificates/" + generatedByUserName + "/" + finalFileName;
                    dal.UpdateComplianceCertificateFilePath(certificateId, relativePath, finalFileName, certificateNo);
                    //HttpContext.Current.Response.ContentType = "application/pdf";
                    //HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + finalFileName);
                    //HttpContext.Current.Response.Buffer = true;
                    //HttpContext.Current.Response.Clear();
                    //HttpContext.Current.Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                    //HttpContext.Current.Response.OutputStream.Flush();
                    //HttpContext.Current.Response.Flush();

                    return (serverFilePath, relativePath, certificateId);

                    //    // Save the PDF to the server
                    //    string serverDirectory = HttpContext.Current.Server.MapPath("~/FilesRep/Certificates/");
                    //    string serverFilePath = Path.Combine(directoryPath, "Certificate_DGC_" + userName + "_" + poLocaldate.ToString("dd-MM-yyyy").ToUpper() + ".pdf");
                    //    File.WriteAllBytes(serverFilePath, ms.ToArray());
                    //    dal.SaveComplianceCertificateFilePath(serverFilePath, periodicity, userName, generatedByUserId, "Certificate_DGC_" + userName + "_" + poLocaldate.ToString("dd-MM-yyyy").ToUpper() + ".pdf");

                    
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating certificate: {ex.Message}", true);
                return (string.Empty, string.Empty,0);
            }
        }


        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            refreshdata(generatedByUserId, ViewState["period"].ToString());
        }
        private void DisplayUploadedFiles(string periodicity, string directoryPath, string userName)
        {
            string dirPath = directoryPath;
            List<ComplianceCertificate> cc = new List<ComplianceCertificate>();
            if (directoryPath == "")
            {
                DAL dal = new DAL();
                cc = dal.GetComplianceCertificatePath(periodicity, userName);
                // Assign dirPath from the first non-empty certificate path found
                if (cc.Count > 0)
                {
                    dirPath = cc.FirstOrDefault()?.CertificatePath ?? "";
                }
            }
            if (dirPath == "")
            {
                GridView1.DataSource = null;
                GridView1.DataBind();
            }
            else
            {
                string dirtoryPath = Server.MapPath("~/FilesRep/" + dirPath);
                if (Directory.Exists(dirtoryPath))
                {
                    string[] fileNames = Directory.GetFiles(dirtoryPath);
                    GridView1.DataSource = cc; //fileNames.Select(fileName => new { ID = ComplianceDetailId, FileName = Path.GetFileName(fileName) });
                    GridView1.DataBind();
                }
                else{}
            }
        }
        protected void DownloadFile(object sender, EventArgs e)
        {
            //string filePath = (sender as LinkButton).CommandArgument;
            //string userName = Session["UserName"].ToString();
            //string DepartmentName = txtDetailsDepartmentName.Text;
            //string complianceDetailId = txtDetailsComplianceDetailID.Text;
            //string complianceArea = txtDetailsComplArea.Text;
            //string directoryName = $"{userName}_{DepartmentName}_{complianceDetailId}_{complianceArea}";
            //string directoryPath = Server.MapPath("~/FilesRep/" + directoryName);
            //filePath = Path.Combine(directoryPath, filePath);
            //Response.ContentType = ContentType;
            //Response.AppendHeader("Content-Disposition", "attachment; filename=" + filePath);
            //Response.WriteFile(filePath);
            //Response.End();
        }
        public DateTime GetLocalTIme()
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
            return now;
        }
        public void AddBlankRow(PdfPTable table, int colspan)
        {
            PdfPCell cell = CreateCell("", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK), colspan, Element.ALIGN_CENTER);
            cell.MinimumHeight = 10f;
            table.AddCell(cell);
        }
        public PdfPCell CreateCell(string text, iTextSharp.text.Font font, int colspan = 1, int horizontalAlignment = Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.Colspan = colspan;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            return cell;
        }
        public void AddUnderline(PdfPTable table, int colspan)
        {
            PdfPCell cell = new PdfPCell(new Phrase(""));
            cell.Colspan = colspan;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
            table.AddCell(cell);
        }
        public PdfPCell CreateCellWithBorder(string text, iTextSharp.text.Font font, int colspan = 1, int horizontalAlignment = Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.Colspan = colspan;
            cell.HorizontalAlignment = horizontalAlignment;
            return cell;
        }
        
    }
}