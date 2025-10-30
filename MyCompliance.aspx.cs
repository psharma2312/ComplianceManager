using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class MyCompliance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int userID = Convert.ToInt32(Session["UserId"].ToString());
            string userName = Session["UserName"].ToString();
            if (!this.IsPostBack)
            {
                LoadCompliance(userName, userID);
            }
        }

        private void LoadCompliance(string userName, int userID)
        {
            DAL dal = new DAL();

            //gvComplDetails.DataSource = dal.LoadCompliance(userName, userID);
            //gvComplDetails.DataBind();
        }
    }
}