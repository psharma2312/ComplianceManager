using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class ModuleMaster : System.Web.UI.Page
    {
        String strConn = ConfigurationManager.ConnectionStrings["dpsCon"].ToString();
        SqlConnection sqlCon;
        SqlCommand sqlCmd;
        SqlDataReader objReader;
        SqlDataAdapter sqlDa;
        string strQuery;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] == null)
            {
                Response.Redirect("../AuthPages/Login.aspx");
            }

            if (!Page.IsPostBack)
            {
                lblError.Visible = false;
                string strQ;
                try
                {
                    //strQ = "SELECT role_id, role_name FROM x_role_master ORDER BY role_name";
                    //Utility.classDropDownListBox.fillListQuery(ddlRole, strQ, "role_id", "role_name", strConn);
                    //ddlRole.Items.Insert(0, new ListItem("Please Select", "0"));
                    //ddlRole.SelectedIndex = 0;
                    ddlRole.Items.Insert(0, new ListItem("Please Select Role", "0"));


                    //ddlRole.Items.Insert(0, new ListItem("[select]", "0"));
                    ddlRole.Items.Insert(1, new ListItem("Business Development Executive", "BDE"));
                    ddlRole.Items.Insert(2, new ListItem("Area Sales Manager ", "ASM"));
                    //ddl_level.Items.Insert(3, new ListItem("Zonal Sales Manager ", "ZSM")); 
                    //ddl_level.Items.Insert(4, new ListItem("Head Retail Sales", "HRS")); 
                    //ddl_level.Items.Insert(5, new ListItem("Sales Director", "SDI")); 
                    //ddl_level.Items.Insert(6, new ListItem("Director Consumer Products", "DCP")); 
                    //ddl_level.Items.Insert(7, new ListItem("L'Oreal India Head ", "LIH")); 
                    ddlRole.Items.Insert(3, new ListItem("Zonal Sales Manager / Regional Sales Manager", "ZSM"));
                    ddlRole.Items.Insert(4, new ListItem("India Head", "LIH"));
                    ddlRole.SelectedIndex = 0;

                    strQ = "select object_id,object_name from x_object_master";
                    Utility.classDropDownListBox.fillListQuery(ddlModule, strQ, "object_id", "object_name", strConn);
                    ddlModule.Items.Insert(0, new ListItem("Please Select Module ", "0"));
                    ddlModule.SelectedIndex = 0;
                    fillGrid(gvMapping, "usp_ModuleMapping_detail", strConn);
                }
                catch (Exception ex)
                { lblError.Text = ex.Message; }


            }
        }
        protected void gvRole_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

        }
        protected void btnModule_Click(object sender, EventArgs e)
        {
            try
            {

                CreateModuleMapping();

                gridBind();

            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        public void gridBind()
        {
            fillGrid(gvMapping, "usp_ModuleMapping_detail", strConn);
        }
        private void CreateModuleMapping()
        {
            try
            {

                sqlCon = new SqlConnection(strConn);
                sqlCmd = new SqlCommand("USP_Create_ModuleMapping", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCon.Open();
                sqlCmd.Parameters.Add(new SqlParameter("@role_id", SqlDbType.VarChar, 3));
                sqlCmd.Parameters["@role_id"].Value = ddlRole.SelectedItem.Value;
                sqlCmd.Parameters.Add(new SqlParameter("@object_id", SqlDbType.VarChar, 60));
                sqlCmd.Parameters["@object_id"].Value = ddlModule.SelectedItem.Value;

                sqlCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                sqlCmd.Dispose();
                sqlCon.Close();
                sqlCon.Dispose();
            }

        }

        public void fillGrid(GridView objgv, string strProcName, string strConnection)
        {

            try
            {
                SqlConnection objcon;
                SqlDataAdapter objda;
                SqlCommand sqlCmd;
                sqlCmd = new SqlCommand();
                //SqlDataReader sqlReader;
                DataSet objDs = new DataSet();
                objcon = new SqlConnection(strConnection);
                sqlCmd.Connection = objcon;
                sqlCmd.CommandText = strProcName;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                objcon.Open();

                objda = new SqlDataAdapter();
                objda.SelectCommand = sqlCmd;
                objda.Fill(objDs);
                objgv.DataSource = objDs;
                objgv.DataBind();

                objcon.Close();
                objcon.Dispose();
            }
            catch (Exception ex)
            { lblError.Text = ex.Message; }


        }
        protected void gvMapping_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }
        protected void gvMapping_RowEditing1(object sender, GridViewEditEventArgs e)
        {
            gvMapping.EditIndex = e.NewEditIndex;
            gridBind();
        }
        protected void gvMapping_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvMapping.EditIndex = -1;
            gridBind();

        }
        protected void gvMapping_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string _oRights;
            string object_id;
            try
            {

                object_id = ((Label)gvMapping.Rows[e.RowIndex].FindControl("lblRoleID")).Text;

                string roleid = ((TextBox)gvMapping.Rows[e.RowIndex].FindControl("txtRoleId")).Text;

                bool strRights = ((CheckBox)gvMapping.Rows[e.RowIndex].FindControl("chkRights")).Checked;
                if (strRights == true)
                {
                    _oRights = "1";
                }
                else
                { _oRights = "0"; }


                sqlCon = new SqlConnection(strConn);
                sqlCon.Open();
                strQuery = "select user_id from x_user_master where role_id='" + roleid + "'";
                sqlCmd = new SqlCommand(strQuery, sqlCon);
                SqlDataReader sqlReader;
                sqlReader = sqlCmd.ExecuteReader();
                string user_id = "";
                while (sqlReader.Read())
                {
                    user_id = sqlReader["user_id"].ToString();
                    CreateModuleMapping(user_id, object_id, roleid, _oRights);
                }
                sqlCon.Close();
                sqlCon.Dispose();
                gvMapping.EditIndex = -1;
                gridBind();

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

        }
        private void CreateModuleMapping(string user_id, string object_id, string roleid, string rights)
        {
            try
            {

                sqlCon = new SqlConnection(strConn);
                sqlCmd = new SqlCommand("USP_module_role_mapping", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCon.Open();
                sqlCmd.Parameters.Add(new SqlParameter("@user_id", SqlDbType.VarChar, 15));
                sqlCmd.Parameters["@user_id"].Value = user_id.Trim();
                sqlCmd.Parameters.Add(new SqlParameter("@object_id", SqlDbType.Int));
                sqlCmd.Parameters["@object_id"].Value = object_id.Trim();
                sqlCmd.Parameters.Add(new SqlParameter("@role_id", SqlDbType.Char, 3));
                sqlCmd.Parameters["@role_id"].Value = roleid.ToString();
                sqlCmd.Parameters.Add(new SqlParameter("@rights", SqlDbType.Char, 1));
                sqlCmd.Parameters["@rights"].Value = rights;


                sqlCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                sqlCmd.Dispose();
                sqlCon.Close();
                sqlCon.Dispose();
            }

        }
        protected void gvMapping_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    if ((e.Row.RowState & DataControlRowState.Edit) > 0)
            //    {
            //        string strQuery;
            //        strQuery = "select object_id,object_name from x_object_master";
            //        DropDownList ddlobjectID = (DropDownList)e.Row.FindControl("ddlObjectID");
            //        TextBox txtRole = (TextBox)e.Row.FindControl("txtObjectID");
            //        Utility.classDropDownListBox.fillListQuery(ddlobjectID, strQuery, "object_id", "object_name", strConn);

            //        //ddlRole.SelectedItem.Text = "select ";
            //    }
            //}
        }
    }
}