using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string effectiveFromText = txtEffectiveFromDate.Text.Trim();
            DateTime EffectiveFrom = Convert.ToDateTime(effectiveFromText);
            DAL dal = new DAL();
           dal.SaveDate(EffectiveFrom);
        }
        public DateTime formatDate(string datetime)
        {
            // Convert to DateTime
            DateTime parsedDate = DateTime.ParseExact(datetime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            // Set time to 18:56:03.150
            //DateTime finalDate = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 18, 56, 03, 150);


            // Update time to 18:56:03.150
            DateTime finalDate = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, 18, 56, 03, 150);



            // Convert to required format
            //string formattedDate = finalDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return finalDate;
        }
    }
}