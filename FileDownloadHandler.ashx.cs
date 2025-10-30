using iTextSharp.text.pdf.parser;
using System;
using System.IO;
using System.Web;

namespace ComplianceManager
{
    /// <summary>
    /// Summary description for FileDownloadHandler
    /// </summary>
    public class FileDownloadHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            // Get the file name from the query string with full Path
            string fileName = context.Request.QueryString["fileName"];
            if (string.IsNullOrEmpty(fileName))
            {
                context.Response.Write("No file specified.");
                return;
            }
            // Define the upload directory and construct the full file path
            string filePath = context.Server.MapPath(fileName);

            //string filePath = Path.Combine(uploadDirectory, fileName);
            fileName = System.IO.Path.GetFileName(fileName);
            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Set the response headers for file download
                context.Response.ContentType = "application/octet-stream";
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                context.Response.WriteFile(filePath);
                context.Response.End();
            }
            else
            {
                // Return an error if the file is not found
                context.Response.Write("File not found.");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}