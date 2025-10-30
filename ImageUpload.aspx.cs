using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace ComplianceManager
{
    public partial class ImageUpload : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString());
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserName"] == null)
                {
                    Response.Redirect("../AuthPages/Login.aspx");
                }
                if (!Page.IsPostBack)
                {
                    BindDepartmentDropdown();
                    //refreshdata();
                }
                
            }
        }
        private void BindDepartmentDropdown()
        {
            ddlDepartment.DataSource = PopulateDepartment("Department");
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("Please Select", String.Empty));
            ddlDepartment.SelectedIndex = 0;
        }
        public List<Department> PopulateDepartment(string dtaType)
        {
            try
            {
                List<Department> data = new List<Department>();
                string constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    string query = "Compliance.GetDropDownValues";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@MenuType", SqlDbType.VarChar).Value = dtaType;
                        con.Open();
                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                data.Add(new Department
                                {
                                    DepartmentName = sdr["Code"].ToString(),
                                    DeptID = Convert.ToInt32(sdr["ID"]),
                                });
                            }
                        }
                        con.Close();
                    }
                }

                return data;
            }
            finally
            {

            }
        }
        public void refreshdata(string deptid="")
        {

            try
            {
                con.Open();
                DAL dal = new DAL();
                GridView1.DataSource = dal.LoadComplianceMaster(Convert.ToInt32("1"));
                GridView1.DataBind();
                con.Close();
            }
            finally
            {
                con.Close();
            }

        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshdata(ddlDepartment.SelectedItem.Value);
            btnupdate.Visible = true;
        }
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    con.Open();
                    DropDownList DropDownList1 = (e.Row.FindControl("DropDownList1") as DropDownList);


                    SqlCommand cmd = new SqlCommand("SELECT [user_id] as UserId, [user_name] as UserName FROM [Compliance].[x_user_master]", con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    DropDownList1.DataSource = dt;

                    DropDownList1.DataTextField = "UserName";
                    DropDownList1.DataValueField = "UserId";
                    DropDownList1.DataBind();
                    DropDownList1.Items.Insert(0, new ListItem("--Select Qualification--", "0"));


                }
            }
            finally
            {
                con.Close();
            }

        }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                InsertDetails();
                ddlDepartment.SelectedIndex = 0;
                labelError.Visible = true;
                labelError.Style.Add(HtmlTextWriterStyle.Color, "Blue");
                labelError.Text = "Compliance assigned successfully....";
            }

            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
            }

        }
        private void InsertDetails()
        {
            try
            {
                foreach (GridViewRow row in GridView1.Rows)
                {
                    DAL dal = new DAL();
                    //UserRights ur = new UserRights();
                    //ur.ObjectId = Convert.ToInt32(gvAssignComp.DataKeys[row.RowIndex].Values[0]);
                    //ur.Rights = (row.FindControl("Chk_right") as CheckBox).Checked;

                    //dal.UpdateRights(ur, Convert.ToInt32(ddlDepartment.SelectedItem.Value));

                    ComplianceMasterDetails comp = new ComplianceMasterDetails();
                    comp.ComplianceID = Convert.ToInt32(GridView1.DataKeys[row.RowIndex].Values[0]);
                    comp.AssignedToID = Convert.ToInt32((row.FindControl("DropDownList1") as DropDownList).SelectedItem.Value);
                    //comp.ComplianceArea = ((TextBox)gvAssignComp.Rows[e.RowIndex].FindControl("txtGridComplianceArea")).Text;
                    //comp.ComplianceTypeID = Convert.ToInt32((gvAssignComp.Rows[e.RowIndex].FindControl("ddlGridComplianceType") as DropDownList).SelectedItem.Value);
                    //comp.GovernmentLegislation = ((TextBox)gvAssignComp.Rows[e.RowIndex].FindControl("txtGridGovernmentLegislation")).Text;
                    //comp.ActSectionReference = ((TextBox)gvAssignComp.Rows[e.RowIndex].FindControl("txtGridActSectionReference")).Text;

                    //comp.ComplianceNatureID = Convert.ToInt32((gvAssignComp.Rows[e.RowIndex].FindControl("ddlGridNatureOfCompliance") as DropDownList).SelectedItem.Value);
                    //comp.EffectiveFrom = Convert.ToDateTime(((TextBox)gvAssignComp.Rows[e.RowIndex].FindControl("txtGridEffectiveFrom")).Text);
                    //comp.StandardDueDate = Convert.ToDateTime(((TextBox)gvAssignComp.Rows[e.RowIndex].FindControl("txtGridStandardDueDate")).Text);
                    //comp.FrequencyID = Convert.ToInt32((gvAssignComp.Rows[e.RowIndex].FindControl("ddlGridFrequencyName") as DropDownList).SelectedItem.Value);
                    //comp.DeptId = Convert.ToInt32((gvAssignComp.Rows[e.RowIndex].FindControl("ddlGridDepartmentFunctionName") as DropDownList).SelectedItem.Value);
                    //comp.DetailsOfComplianceRequirements = ((TextBox)gvAssignComp.Rows[e.RowIndex].FindControl("txtGridDetailsOfComplianceRequirements")).Text;
                    comp.CreatedById = Convert.ToInt32(Session["UserId"].ToString());
                    //comp.UpdatedBy = Convert.ToInt32(Session["UserId"].ToString());
                    dal.SaveComplianceAssignmentDetails(comp, "test");
                    GridView1.EditIndex = -1;
                    //BindGridView(ddlDepartment.SelectedItem.Value);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {

            }
        }
    }
}