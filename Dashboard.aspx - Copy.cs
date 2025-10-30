using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;

namespace ComplianceManager
{
    public partial class Dashboard : System.Web.UI.Page
    {
        int userId = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            userId = Convert.ToInt32(Session["UserId"].ToString());
            if (Session["UserName"] == null)
            {
                Response.Redirect("../AuthPages/Login.aspx");
            }
            if (!IsPostBack)
            {
                LoadComplianceSummary();
                //lnkTab1_Click(sender, e);
            }
        }
        private void LoadComplianceSummary()
        {
            string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("Compliance.GetUserComplianceSummary", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Dictionary<string, int> summaryData = new Dictionary<string, int>();
                        if (reader.HasRows)
                        {
                            // 1️⃣ Compliance Due This Month
                           
                            if (reader.Read())
                            {
                                summaryData["due"] = reader.GetInt32(0);
                                lblComplianceDue.Text = reader.FieldCount.ToString();// "Due This Month: " + reader.FieldCount.ToString();
                            }
                            reader.NextResult();

                            // 2️⃣ Pending Approval Compliances
                            if (reader.Read())
                            {
                            summaryData["pending"] = reader.GetInt32(0);
                                lblPendingApproval.Text = reader["PendingApprovalCount"].ToString();// "Pending Approval: " + reader["PendingApprovalCount"].ToString();
                            }
                            reader.NextResult();

                            // 3️⃣ Delayed Compliances
                            if (reader.Read())
                            {
                            summaryData["delayed"] = reader.GetInt32(0);
                                lblDelayed.Text = reader["DelayedComplianceCount"].ToString();// "Delayed: " + reader["DelayedComplianceCount"].ToString();
                            }
                            reader.NextResult();

                            // 4️⃣ Total Assigned This Month
                            if (reader.Read())
                            {
                            summaryData["total"] = reader.GetInt32(0);
                                lblTotalAssigned.Text = reader["TotalCompliancesForMonth"].ToString();// "Total This Month: " + reader["TotalCompliancesForMonth"].ToString();
                            }
                        }
                        string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(summaryData);
                        ClientScript.RegisterStartupScript(this.GetType(), "LoadChart", "loadChart(" + jsonData + ");", true);
                    }
                }
            }
        }
        public class ComplianceDetail
        {
            public string ComplianceName { get; set; }
            public string DueDate { get; set; }
            public string Status { get; set; }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<ComplianceDetail> GetComplianceDetails(string status)
        {
            if (HttpContext.Current.Session["UserId"] == null)
            {
                HttpContext.Current.Response.StatusCode = 401; // Unauthorized
                return null;
            }
            string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            int userId = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
            List<ComplianceDetail> details = new List<ComplianceDetail>();

            using (SqlConnection conn = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("Compliance.GetUserComplianceDetails", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@StatusType", status);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            details.Add(new ComplianceDetail
                            {
                                ComplianceName = reader["ComplianceName"].ToString(),
                                DueDate = Convert.ToDateTime(reader["DueDate"]).ToString("yyyy-MM-dd"),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }
            return details;
        }

        //private void GetData(string query, GridView gvData, int userId)
        //{
        //    DAL dal = new DAL();
        //    var data = dal.GetDashboardData(query, userId);
        //    gvData.DataSource = data;
        //    gvData.DataBind();
        //}

        protected void lnkTab1_Click(object sender, EventArgs e)
        {
            //    DateTime date = DateTime.Now.Date;
            //    string sql = "Select CustomerId,Name,Country,convert(varchar,CreatedDate,106) as CreatedDate from Dashboard WHERE createdDate between '" + date + "' AND '" + DateTime.Now.Date + "' ";
            //    //GetData(sql, gvdaily);
            //    GetData("DueThisMonth", gvDueThisMonth, userId);
            //    MultiView1.ActiveViewIndex = 0;
            //    lnkTab1.ForeColor = Color.Yellow;
            //    lnkTab2.ForeColor = Color.White;
            //    lnkTab3.ForeColor = Color.White;
            //    lnkTab4.ForeColor = Color.White;
        }

        protected void lnkTab2_Click(object sender, EventArgs e)
        {
            //    DateTime date = DateTime.Now.AddDays(-7).Date;
            //    string sql = "Select CustomerId,Name,Country,convert(varchar,CreatedDate,106) as CreatedDate from Dashboard WHERE createdDate between '" + date + "' AND '" + DateTime.Now.Date + "' ";
            //    //GetData(sql, gvweekly);
            //    GetData("Submitted", gvSubmitted, userId);
            //    MultiView1.ActiveViewIndex = 1;
            //    lnkTab2.ForeColor = Color.Yellow;
            //    lnkTab3.ForeColor = Color.White;
            //    lnkTab4.ForeColor = Color.White;
            //    lnkTab1.ForeColor = Color.White;
        }

        protected void lnkTab3_Click(object sender, EventArgs e)
        {
            //    DateTime date = DateTime.Now.AddDays(-30).Date;
            //    string sql = "Select CustomerId,Name,Country,convert(varchar,CreatedDate,106) as CreatedDate from Dashboard WHERE createdDate between '" + date + "' AND '" + DateTime.Now.Date + "' ";
            //    //GetData(sql, gvweekly);
            //    GetData("NotSubmitted", gvNotSubmitted, userId);
            //    MultiView1.ActiveViewIndex = 2;

            //    lnkTab3.ForeColor = Color.Yellow;
            //    lnkTab4.ForeColor = Color.White;
            //    lnkTab1.ForeColor = Color.White;
            //    lnkTab2.ForeColor = Color.White;
        }
        protected void lnkTab4_Click(object sender, EventArgs e)
        {
            //    DateTime date = DateTime.Now.AddDays(-30).Date;
            //    string sql = "Select CustomerId,Name,Country,convert(varchar,CreatedDate,106) as CreatedDate from Dashboard WHERE createdDate between '" + date + "' AND '" + DateTime.Now.Date + "' ";
            //    //GetData(sql, gvmonthly);
            //    GetData("Pending", gvPending, userId);
            //    MultiView1.ActiveViewIndex = 2;
            //    lnkTab4.ForeColor = Color.Yellow;
            //    lnkTab1.ForeColor = Color.White;
            //    lnkTab2.ForeColor = Color.White;
            //    lnkTab3.ForeColor = Color.White;

        }
    }
}