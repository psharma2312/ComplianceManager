using ComplianceManager;
using System;
using System.Data;
//using Twilio;
//using Twilio.Rest.Verify.V2.Service;
//using Twilio.Exceptions;
using System.Linq;
using System.Net.PeerToPeer;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ComplianceManager
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Logger"] != null)
            {
                var logger = (ActionLogger)Session["Logger"];
                var repo = new DAL();
            }


                if (!IsPostBack)
            {
                ((Label)Master.FindControl("lblErrorMessage")).Text = "";
            }
        }
        protected void LoginButton_Click(object sender, EventArgs e)
        {
            // var logger = Session["Logger"] as ActionLogger;
            // Initialize logger AFTER setting UserName
            
            Label lblError = (Label)Master.FindControl("lblErrorMessage");
            String strUserName = txtUsername.Text.Trim();
            String strPassword = txtPassword.Text.Trim();
            if (Session["CorrelationId"] == null)
            {
                Session["CorrelationId"] = Guid.NewGuid().ToString();
            }

            var logger = new ActionLogger(strUserName, Session["CorrelationId"].ToString());
            Session["Logger"] = logger;
            logger?.Info("User log in starts.");

            DAL objDal = new DAL();
            try
            {
                var dsUser = objDal.CheckUserPassword(strUserName, strPassword, 1);
                    string hashedPassword = dsUser.Password;
                    if (!string.IsNullOrEmpty(hashedPassword) && BCrypt.Net.BCrypt.Verify(strPassword, hashedPassword))
                    {
                        // REMOVE THIS SECTION IF 2FA IMPLEMENTED
                        Session["IsAuthenticated"] = true;
                        Session["UserName"] = dsUser.UserName;
                        Session["UserId"] = dsUser.UserId;
                        Session["Name"] = dsUser.Name;
                        Session["Role"] = dsUser.RoleId;
                        Session["UserType"] = dsUser.RoleName;
                    

                    Response.Redirect("~/Dashboard.aspx", false);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "wrongPasswordModal", "showwrongModal();", true);
                    }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Error during login: {ex.Message}";
                lblError.Visible = true;
            }
            }

        protected void btnCancelTwoFA_Click(object sender, EventArgs e)
        {
        }
        private string GetUserPhoneNumber(string username)
        {
            // Implement this method to retrieve the phone number from your database
            // Example: Return dsUser.Tables[0].Rows[0]["PhoneNumber"].ToString() if available
            DAL objDal = new DAL();
            DataSet ds = objDal.GetUserPhoneNumber(username); // Assume this method exists
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["PhoneNumber"].ToString(); // Adjust column name as per your DB
            }
            return null;
        }
        private string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return null;

            // Remove any non-digit characters
            phoneNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Assume +91 for India if no country code, adjust based on your needs
            if (phoneNumber.Length == 10 && phoneNumber.All(char.IsDigit))
            {
                return $"+91{phoneNumber}";
            }
            else if (phoneNumber.Length > 10 && phoneNumber.StartsWith("0"))
            {
                return $"+91{phoneNumber.Substring(1)}"; // Remove leading 0 and add +91
            }
            else if (!phoneNumber.StartsWith("+"))
            {
                // Add a default country code (e.g., +1 for US) or throw an error
                return $"+1{phoneNumber}"; // Adjust to your default country code
            }

            return phoneNumber;
        }

        private bool IsValidE164(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return false;

            return phoneNumber.StartsWith("+") && phoneNumber.Length >= 9 && phoneNumber.Length <= 15 && phoneNumber.All(c => char.IsDigit(c) || c == '+');
        }
        protected void btnForgotPassword_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showForgotModal", "showModal();", true);
        }
    }
}


//public partial class Login : System.Web.UI.Page
//{
//    //private string twilioAccountSid = "AC342031bd5c8c4325d32384f3761c85ae"; // Replace with your Twilio Account SID
//    //private string twilioAuthToken = "4b36ec64337802cbbc7f381d7e5eb74b"; // Replace with your Twilio Auth Token
//    //private string verifyServiceSid = "VA13afa0778ad53602c3d2a58077f476d3"; // Replace with your Twilio Verify Service SID  "DGCVerify"
//    //private string verificationSid; // To store the verification SID
//    //  private bool isTwoFARequired = false; // Flag to track 2FA state
//    //var accountSid = "AC342031bd5c8c4325d32384f3761c85ae";
//    //var authToken = "4b36ec64337802cbbc7f381d7e5eb74b";


//    protected void Page_Load(object sender, EventArgs e)
//    {
//        if (Session["Logger"] != null)
//        {
//            var logger = (ActionLogger)Session["Logger"];
//            var repo = new DAL();
//        }


//        if (!IsPostBack)
//        {
//            ((Label)Master.FindControl("lblErrorMessage")).Text = "";
//            //twofaContainer.Style["display"] = "none";    UNCOMMENT IF 2FA IS IMPLEMENED
//        }
//        // -----------------UNCOMMENT IF 2FA IS IMPLEMENED
//        //else    UNCOMMENT IF 2FA IS IMPLEMENED
//        //{
//        //    if (ViewState["IsTwoFARequired"] != null && (bool)ViewState["IsTwoFARequired"])
//        //    {
//        //        twofaContainer.Style["display"] = "block";
//        //    }
//        //}
//        // -----------------UNCOMMENT IF 2FA IS IMPLEMENED
//    }
//    protected void LoginButton_Click(object sender, EventArgs e)
//    {
//        var logger = Session["Logger"] as ActionLogger;
//        Label lblError = (Label)Master.FindControl("lblErrorMessage");
//        // -----------------UNCOMMENT IF 2FA IS IMPLEMENED
//        //if (ViewState["IsTwoFARequired"] == null || !(bool)ViewState["IsTwoFARequired"])    UNCOMMENT IF 2FA IS IMPLEMENED
//        //{
//        // -----------------UNCOMMENT IF 2FA IS IMPLEMENED
//        String strUserName = txtUsername.Text.Trim();
//        String strPassword = txtPassword.Text.Trim();
//        logger?.Info("User log in starts.");

//        DAL objDal = new DAL();
//        try
//        {
//            var dsUser = objDal.CheckUserPassword(strUserName, strPassword, 1);
//            //DataSet dsUser = objDal.CheckUserPassword(strUserName, strPassword, 1);
//            //if (dsUser.Tables[0].Rows.Count > 0)
//            //{
//            string hashedPassword = dsUser.Password;
//            if (!string.IsNullOrEmpty(hashedPassword) && BCrypt.Net.BCrypt.Verify(strPassword, hashedPassword))
//            {
//                // REMOVE THIS SECTION IF 2FA IMPLEMENTED
//                Session["IsAuthenticated"] = true;
//                Session["UserName"] = dsUser.UserName;
//                Session["UserId"] = dsUser.UserId;
//                Session["Name"] = dsUser.Name;
//                Session["Role"] = dsUser.RoleId;
//                Session["UserType"] = dsUser.RoleName;

//                Response.Redirect("~/Dashboard.aspx", false);
//                // REMOVE TILL THIS LINE

//                // -----------------UNCOMMENT IF 2FA IS IMPLEMENED
//                // string phoneNumber = GetUserPhoneNumber(strUserName);  
//                //if (string.IsNullOrEmpty(phoneNumber))
//                //{
//                //    lblError.Text = "Phone number not found for this user.";
//                //    lblError.Visible = true;
//                //    return;
//                //}
//                //TwilioClient.Init(twilioAccountSid, twilioAuthToken);
//                //var verificationOptions = new CreateVerificationOptions(
//                //    to: phoneNumber,
//                //    channel: "sms",
//                //    pathServiceSid: verifyServiceSid){};
//                //var verification = VerificationResource.Create(verificationOptions);
//                //verificationSid = verification.Sid;

//                //twofaContainer.Style["display"] = "block";
//                //ViewState["IsTwoFARequired"] = true; // Store state in ViewState
//                //txtTwoFACode.Focus();
//                // ------------------UNCOMMMENT TILL HERE
//            }
//            else
//            {
//                ScriptManager.RegisterStartupScript(this, GetType(), "wrongPasswordModal", "showwrongModal();", true);
//                //lblError.Text = "Invalid username or password";
//                //lblError.Visible = true;
//            }
//            //}
//            // -----------------UNCOMMENT IF 2FA IS IMPLEMENED
//            //else
//            //{
//            //    lblError.Text = "Invalid username or password";
//            //    lblError.Visible = true;
//            //}
//            // -----------------UNCOMMENT IF 2FA IS IMPLEMENED
//        }
//        // -----------------UNCOMMENT IF 2FA IS IMPLEMENED
//        //catch (TwilioException ex)
//        //{
//        //    lblError.Text = $"Twilio error: {ex.Message}";
//        //    lblError.Visible = true;
//        //}
//        // -----------------UNCOMMENT IF 2FA IS IMPLEMENED
//        catch (Exception ex)
//        {
//            lblError.Text = $"Error during login: {ex.Message}";
//            lblError.Visible = true;
//        }
//    }
//    // } // -----------------UNCOMMENT IF 2FA IS IMPLEMENED
//    //protected void btnVerifyTwoFA_Click(object sender, EventArgs e)
//    //{
//    //    Label lblError = (Label)Master.FindControl("lblErrorMessage");
//    //    string code = txtTwoFACode.Text.Trim();

//    //    // Check state from ViewState
//    //    if (ViewState["IsTwoFARequired"] != null && (bool)ViewState["IsTwoFARequired"])
//    //    {
//    //        try
//    //        {
//    //            TwilioClient.Init(twilioAccountSid, twilioAuthToken);
//    //            string phoneNumber = GetUserPhoneNumber(txtUsername.Text.Trim());

//    //            var verificationCheck = VerificationCheckResource.Create(
//    //                to: phoneNumber,
//    //                code: code,
//    //                pathServiceSid: verifyServiceSid);

//    //            if (verificationCheck.Status == "approved")
//    //            {
//    //                Session["IsAuthenticated"] = true;
//    //                Response.Redirect("~/Dashboard.aspx", false);
//    //            }
//    //            else
//    //            {
//    //                lblError.Text = "Invalid verification code. Please try again.";
//    //                lblError.Visible = true;
//    //            }
//    //        }
//    //        catch (TwilioException ex)
//    //        {
//    //            lblError.Text = $"Twilio verification error: {ex.Message}";
//    //            lblError.Visible = true;
//    //        }
//    //        catch (Exception ex)
//    //        {
//    //            lblError.Text = $"Error verifying code: {ex.Message}";
//    //            lblError.Visible = true;
//    //        }
//    //    }
//    //    else
//    //    {
//    //        lblError.Text = "2FA process not initiated. Please log in again.";
//    //        lblError.Visible = true;
//    //    }
//    //}

//    protected void btnCancelTwoFA_Click(object sender, EventArgs e)
//    {
//        //twofaContainer.Style["display"] = "none";
//        //ViewState["IsTwoFARequired"] = false; // Reset state in ViewState
//        //txtTwoFACode.Text = "";
//        //((Label)Master.FindControl("lblErrorMessage")).Text = "";
//        //txtUsername.Text = "";
//        //txtPassword.Text = "";
//    }
//    private string GetUserPhoneNumber(string username)
//    {
//        // Implement this method to retrieve the phone number from your database
//        // Example: Return dsUser.Tables[0].Rows[0]["PhoneNumber"].ToString() if available
//        DAL objDal = new DAL();
//        DataSet ds = objDal.GetUserPhoneNumber(username); // Assume this method exists
//        if (ds.Tables[0].Rows.Count > 0)
//        {
//            return ds.Tables[0].Rows[0]["PhoneNumber"].ToString(); // Adjust column name as per your DB
//        }
//        return null;
//    }
//    private string FormatPhoneNumber(string phoneNumber)
//    {
//        if (string.IsNullOrEmpty(phoneNumber))
//            return null;

//        // Remove any non-digit characters
//        phoneNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());

//        // Assume +91 for India if no country code, adjust based on your needs
//        if (phoneNumber.Length == 10 && phoneNumber.All(char.IsDigit))
//        {
//            return $"+91{phoneNumber}";
//        }
//        else if (phoneNumber.Length > 10 && phoneNumber.StartsWith("0"))
//        {
//            return $"+91{phoneNumber.Substring(1)}"; // Remove leading 0 and add +91
//        }
//        else if (!phoneNumber.StartsWith("+"))
//        {
//            // Add a default country code (e.g., +1 for US) or throw an error
//            return $"+1{phoneNumber}"; // Adjust to your default country code
//        }

//        return phoneNumber;
//    }

//    private bool IsValidE164(string phoneNumber)
//    {
//        if (string.IsNullOrEmpty(phoneNumber))
//            return false;

//        return phoneNumber.StartsWith("+") && phoneNumber.Length >= 9 && phoneNumber.Length <= 15 && phoneNumber.All(c => char.IsDigit(c) || c == '+');
//    }
//    protected void btnForgotPassword_Click(object sender, EventArgs e)
//    {
//        ScriptManager.RegisterStartupScript(this, GetType(), "showForgotModal", "showModal();", true);
//    }
//}
//}