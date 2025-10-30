using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class ComplianceDocs : System.Web.UI.Page
    {
        List<int> Indexes = new List<int>();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["UPLOAD_PRODUCTS"] = null;
                BindDropDown();
            }
            else
            {
                if (Session["UPLOAD_PRODUCTS"] == null)
                {
                    IntializeFields();
                }
                else
                {
                }
            }
        }

        protected void BindDropDown()
        {
            //DAL dal = new DAL();
            //ddlFileType.DataSource = dal.PopulateData("LabelInput", 'N');
            //ddlFileType.DataBind();
            //ddlFileType.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Please Select File", String.Empty));
            //ddlFileType.SelectedIndex = 0;


           
        }
        protected void IntializeFields()
        {
            
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            IntializeFields();
            if (FileUpload1.HasFile)
            {

                if (ddlFileType.SelectedItem.ToString() == "INVOICE - (General Format)")
                {
                    UploadFromInvoice();
                    btnPrintLabel.Enabled = true;
                }
               
               
            }
            else
            {
                labelError.Text = "Please select a file";
            }
        }

        private void UploadFromInvoice()
        {
           
        }

        private void UploadFromTransfer()
        {
          
        }
        private void UploadFromPurchase()
        {
           
        }
        private void UploadFromPickList()
        {
           
        }
        private int GetMRP(string SKUCode)
        {
            return 0;

        }
        
     
        private int GetQTY(string strQuantity)
        {
            try
            {
                strQuantity = strQuantity.Trim();
                bool slash = strQuantity.Contains('/');
                string[] results;
                if (slash)
                {
                    results = strQuantity.Split('/');
                }
                else
                {
                    results = strQuantity.Split(' ');
                }

                decimal d = Convert.ToDecimal(results[0].Trim());
                return Convert.ToInt32(d);
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error in getting Quantity " + ex.ToString());
                return 0;
            }

        }
       

      
        protected void btnPrintLabel_Click(object sender, EventArgs e)
        {
            GeneraratePDFFile();
        }

        protected void ddlFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileUpload1.Enabled = true;
            btnUpload.Enabled = true;
        }
        public void GeneraratePDFFile()
        {
           
        }

        protected void btnSample_Click(object sender, EventArgs e)
        {
            string fileName = "GeneralInvoice.xls";

            if (ddlFileType.SelectedItem.ToString() == "GENERAL INVOICE")
            {
                fileName = "GeneralInvoice.xls";
            }
            ExportSample(fileName);
        }
        private void ExportSample(string filename)
        {
            string filepath = ServerMapPath(filename);
            HttpResponse res = GetHttpResponse();
            res.Clear();
            res.AppendHeader("content-disposition", "attachment; filename=" + filename);
            res.ContentType = "application/octet-stream";
            res.WriteFile(filepath);
            res.Flush();
            res.End();
        }
        public string ServerMapPath(string path)
        {
            string path1 = Server.MapPath("~/Templates/" + path);
            return path1;
        }
        public static HttpResponse GetHttpResponse()
        {
            return HttpContext.Current.Response;
        }

    }
}