using System.IO;
using System.Web;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using static ComplianceManager.Dashboard;
using System.Configuration;
using System.Data.SqlClient;
using iTextSharp.text;

namespace ComplianceManager
{
    /// <summary>
    /// Summary description for FileDeleteHandler
    /// </summary>
    public class FileDeleteHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
           //Approver will never delete the file, he can review and ask the owner to delete and upload other file with changes
            
            string fileName = context.Request.QueryString["fileName"];  //full Path
            string complianceId = context.Request.QueryString["complianceId"];
            var userId = context.Session["UserId"];
            string documentId = context.Request.QueryString["documentID"]; // To know, call is coming from compliancemaster
            // Access Username from session
            string username = context.Session["UserName"]?.ToString();

            //string uploadDirectory = context.Server.MapPath("~/FilesRep/ComplianceDocuments/");
            //string filePath = Path.Combine(uploadDirectory, fileName);
            // Define the upload directory and construct the full file path
            string filePath = context.Server.MapPath(fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                if (documentId !="-1" )
                {
                    UpdateFileStatus(Convert.ToInt32(documentId),Convert.ToInt32(complianceId));
                }
                context.Session["ErrorMessage"] = "File deleted successfully.";
                context.Response.Write("Success");
            }
            else
            {
                context.Session["ErrorMessage"] = "File not found.";
                context.Response.Write("Failure");
            }
        }
        private void UpdateFileStatus(int documentId, int complianceId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "DELETE FROM Compliance.ComplianceDocuments WHERE ComplianceID = @ComplianceID AND DocumentId = @DocumentId";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ComplianceID", complianceId);
                    cmd.Parameters.AddWithValue("@DocumentId", documentId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
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