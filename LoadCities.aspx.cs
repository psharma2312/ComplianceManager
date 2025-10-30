using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class LoadCities : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.BindGrid();
            }
        }
        private void BindGrid()
        {
            string sql = "SELECT TOP 10 CustomerID, CustomerName as ContactName, City FROM Compliance.Customers";
            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter(sql, con))
                {
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        gvCustomers.DataSource = dt;
                        gvCustomers.DataBind();
                    }
                }
            }
        }
        protected void EditCustomer(object sender, GridViewEditEventArgs e)
        {
            gvCustomers.EditIndex = e.NewEditIndex;
            this.BindGrid();
        }

        protected void CancelEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvCustomers.EditIndex = -1;
            this.BindGrid();
        }

        protected void RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && gvCustomers.EditIndex == e.Row.RowIndex)
            {
                DropDownList ddlCities = (DropDownList)e.Row.FindControl("ddlCities");
                string sql = "SELECT DISTINCT City FROM Compliance.Customers";
                string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(sql, con))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            ddlCities.DataSource = dt;
                            ddlCities.DataTextField = "City";
                            ddlCities.DataValueField = "City";
                            ddlCities.DataBind();
                            string selectedCity = DataBinder.Eval(e.Row.DataItem, "City").ToString();
                            ddlCities.Items.FindByValue(selectedCity).Selected = true;
                        }
                    }
                }
            }
        }

        protected void UpdateCustomer(object sender, GridViewUpdateEventArgs e)
        {
            string city = (gvCustomers.Rows[e.RowIndex].FindControl("ddlCities") as DropDownList).SelectedItem.Value;
            string customerId = gvCustomers.DataKeys[e.RowIndex].Value.ToString();
            string conString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(conString))
            {
                string query = "UPDATE Compliance.Customers SET City = @City WHERE CustomerId = @CustomerId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.Parameters.AddWithValue("@City", city);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    Response.Redirect(Request.Url.AbsoluteUri);
                }
            }
        }
    }
}