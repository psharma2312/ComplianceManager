using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace ComplianceManager 
{
    public class BLL : DAL
    {
    public DataSet GetPasswodDetails(string strUsername)
    {
        DataSet objDataSet = null;
        try
        {
            SqlParameter[] oParam = new SqlParameter[1];
            oParam[0] = new SqlParameter("@user_name", strUsername);
            //objDataSet = ExecuteDataSet("usp_GetPasswordDetail", oParam); 
        }
        catch (Exception ex)
        {            
            throw ex;
        }
        return objDataSet;
       
    }

    public void UpdatePasswordDetails(string strUsername, string strCurrentpwd)
    {        
        try
        {
            SqlParameter[] oParam = new SqlParameter[2];
            oParam[0] = new SqlParameter("@user_name", strUsername);
            oParam[1] = new SqlParameter("@current_pwd", strCurrentpwd);
            //ExecuteNonQuery("usp_UpdatePassword", oParam);
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    }
}