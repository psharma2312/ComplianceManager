using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class ModifyUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] == null)
            {
                Response.Redirect("../AuthPages/Login.aspx");
            }
            if (!Page.IsPostBack)
            {
                BindDropDown();
                BindGridView(ddlUserId.SelectedItem.Value);
            }
        }
        private void BindDropDown()
        {
            DAL dal = new DAL();
            ddlUserId.DataSource = dal.GetUsers();
            ddlUserId.DataBind();
            ddlUserId.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Please Select", String.Empty));
            ddlUserId.SelectedIndex = 1;

        }
        public void BindGridView(string userId)
        {
            DAL dal = new DAL();
            gvModifyModule.DataSource = dal.LoadRights(Convert.ToInt32(userId));
            gvModifyModule.DataBind();
        }
        protected void ddlUserId_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGridView(ddlUserId.SelectedItem.Value);
        }

        protected void gvModifyModule_RowDataBound(object sender, GridViewRowEventArgs e) { }

        protected void btnupdate_Click(object sender, EventArgs e)
        {
            try
            {
                // InsertHeader();
                InsertDetails();
                ddlUserId.SelectedIndex = 0;
                lblError.Visible = true;
                lblError.Style.Add(HtmlTextWriterStyle.Color, "Blue");
                lblError.Text = "User modified successfully....";
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
                foreach (GridViewRow row in gvModifyModule.Rows)
                {
                    DAL dal = new DAL();
                    UserRights ur = new UserRights();
                    ur.ObjectId = Convert.ToInt32(gvModifyModule.DataKeys[row.RowIndex].Values[0]);
                    ur.Rights = (row.FindControl("Chk_right") as CheckBox).Checked;

                    dal.UpdateRights(ur, Convert.ToInt32(ddlUserId.SelectedItem.Value));
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
        //private void InsertHeader()
        //{
        //    try
        //    {

        //        sqlCon = new SqlConnection(strConn);
        //        sqlCmd = new SqlCommand("USP_CreateUser_InsertHeader", sqlCon);
        //        sqlCmd.CommandType = CommandType.StoredProcedure;
        //        sqlCon.Open();

        //        sqlCmd.Parameters.Add(new SqlParameter("@user_name", SqlDbType.VarChar, 75));
        //        sqlCmd.Parameters["@user_name"].Value = txtusername.Text.Trim();

        //        sqlCmd.Parameters.Add(new SqlParameter("@current_pwd", SqlDbType.VarChar, 50));
        //        sqlCmd.Parameters["@current_pwd"].Value = txtconpassword.Text.Trim();

        //        sqlCmd.ExecuteNonQuery();

        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.Message);
        //    }
        //    finally
        //    {
        //        sqlCmd.Dispose();
        //        sqlCon.Close();
        //        sqlCon.Dispose();
        //    }

        //}


        //public string encryptPassword(string pwd)
        //{
        //    string encryptedPwd = "";
        //    char[] charArr;
        //    int AsciiVal;
        //    charArr = pwd.ToCharArray();
        //    foreach (char chr in charArr)
        //    {
        //        AsciiVal = 999 - (int)chr;
        //        encryptedPwd += AsciiVal.ToString();
        //    }
        //    return encryptedPwd;
        //}
        //public string decryptPassword(string encryptedPwd)
        //{
        //    string decryptedPwd = "";
        //    string pwdPart1, pwd;
        //    pwd = encryptedPwd;
        //    while (pwd.Length > 0)
        //    {
        //        pwdPart1 = pwd.Substring(0, 3);
        //        if (pwd.Length > 3)
        //            pwd = pwd.Substring(3, pwd.Length - 3);
        //        else
        //            pwd = "";
        //        decryptedPwd += ((char)(999 - int.Parse(pwdPart1))).ToString();
        //    }
        //    return decryptedPwd;
        //}

        //public static void fillListQuery(DropDownList lst, string strQuery, string strvmember, string strdmember, string strconstring)
        //{
        //    int i;
        //    SqlConnection objcon;
        //    SqlCommand objcom;
        //    DataSet objds = new DataSet();
        //    SqlDataAdapter objda;

        //    objcon = new SqlConnection(strconstring);
        //    objcom = new SqlCommand(strQuery, objcon);
        //    objda = new SqlDataAdapter(strQuery, objcon);
        //    objda.Fill(objds, "MyTable");
        //    i = objds.Tables["MyTable"].Rows.Count;
        //    if (i > 0)
        //    {
        //        lst.DataSource = objds.Tables["MyTable"].DefaultView;
        //        lst.DataTextField = strdmember;
        //        lst.DataValueField = strvmember;
        //        lst.DataBind();
        //    }
        //    else
        //    { //lst.Items.Insert(0, new ListItem("---- Select ----", "0"));
        //    }
        //    objds.Dispose();
        //    objda.Dispose();
        //    objcom.Dispose();
        //    objcon.Close();
        //    objcon.Dispose();
        //}
        //public static DataSet fillGrid_Proc_parameter(GridView objgv, string strProcName, string strParameter, string strConnection)
        //{
        //    SqlConnection objcon;

        //    SqlCommand sqlCmd;
        //    DataSet objDs = new DataSet();
        //    objcon = new SqlConnection(strConnection);
        //    sqlCmd = new SqlCommand(strProcName, objcon);
        //    sqlCmd.CommandType = CommandType.StoredProcedure;
        //    sqlCmd.Parameters.Add(new SqlParameter("@userid", SqlDbType.VarChar, 30));
        //    sqlCmd.Parameters["@userid"].Value = strParameter.Trim();
        //    SqlDataAdapter objda = new SqlDataAdapter(sqlCmd);
        //    objda.Fill(objDs);
        //    objgv.DataSource = objDs.Tables[0].DefaultView;
        //    objgv.DataBind();
        //    return objDs;

        //}
    }
}