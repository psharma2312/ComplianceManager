using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Timers;
using System.Web;

namespace ComplianceManager
{
    public class ComplianceEmailScheduler
    {
        //static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //private static Timer timer;
        //private static readonly string SmtpServer = ConfigurationManager.AppSettings["SmtpServer"];
        //private static readonly int SmtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
        //private static readonly string SmtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
        //private static readonly string SmtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
        //private static readonly string FromEmail = ConfigurationManager.AppSettings["FromEmail"];

//        public static void Start()
//        {
//            //timer = new Timer(24 * 60 * 60 * 1000); // Run every 24 hours
//            //timer.Elapsed += CheckComplianceDueDates;
//            //timer.AutoReset = true;
//            //timer.Start();
//            //CheckComplianceDueDates(null, null); // Run immediately on start
//        }

//        private static void CheckComplianceDueDates(object sender, ElapsedEventArgs e)
//        {
//                DateTime now = DateTime.Now;
//                List<ComplianceMasterr> compliances = GetPendingCompliances();

//                foreach (var compliance in compliances)
//                {
//                    DateTime dueDate = compliance.StandardDueDate;
//                    bool isOverdue = dueDate < now;
//                    bool isApproaching = dueDate.Date <= now.AddDays(7).Date && dueDate.Date >= now.Date;

//                    if (isApproaching || isOverdue)
//                    {
//                        SendComplianceEmail(compliance, isOverdue);
//                        if (isOverdue)
//                        {
//                            SendEscalationEmail(compliance);
//                        }
//                    }
//                }
//        }

//        private static List<ComplianceMasterr> GetPendingCompliances()
//        {
//            List<ComplianceMasterr> compliances = new List<ComplianceMasterr>();
//            string query = "SELECT ComplianceID, ComplianceDetailID, ComplianceArea, ComplianceTypeName, GovernmentLegislation, " +
//                           "ActSectionReference, NatureOfComplianceName, EffectiveFrom, StandardDueDate, ComplianceStatusName, UserID, SupervisorID " +
//                           "FROM Compliances WHERE ComplianceStatusName != 'Completed'";

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    conn.Open();
//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            compliances.Add(new ComplianceMasterr
//                            {
//                                ComplianceID = reader.GetInt32(0),
//                               // ComplianceDetailID = reader.GetInt32(1),
//                                ComplianceArea = reader.GetString(2),
//                                ComplianceTypeName = reader.GetString(3),
//                                GovernmentLegislation = reader.GetString(4),
//                                ActSectionReference = reader.GetString(5),
//                                NatureOfComplianceName = reader.GetString(6),
//                                EffectiveFrom = reader.GetDateTime(7),
//                                StandardDueDate = reader.GetDateTime(8),
//                                ComplianceStatusName = reader.GetString(9),
//                                AssignedToID = reader.GetInt32(10),
//                              //  SupervisorID = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11)
//                            });
//                        }
//                    }
//                }
//            }
//            return compliances;
//        }
//        private static void SendComplianceEmail(ComplianceMasterr compliance, bool isOverdue)
//        {
//            var user = GetUserById(compliance.AssignedToID); // Method to fetch user from Users table
//            if (user == null || string.IsNullOrEmpty(user.Email)) return;

//            string subject = isOverdue
//                ? $"OVERDUE: Compliance {compliance.ComplianceArea} Due Date Passed"
//                : $"Reminder: Compliance {compliance.ComplianceArea} Due in {compliance.StandardDueDate.Subtract(DateTime.Now).Days} Days";
//            string body = $@"Dear {user.Email.Split('@')[0]},

//This is a reminder about the compliance task: {compliance.ComplianceArea}.
//Details:
//- Type: {compliance.ComplianceTypeName}
//- Due Date: {compliance.StandardDueDate:dd-MMM-yyyy}
//- Status: {compliance.ComplianceStatusName}

//Please complete or update the status in the Compliance Manager system.

//Best regards,
//Compliance Manager Team";

//            SendEmail(user.Email, subject, body);
//        }

//        private static void SendEscalationEmail(ComplianceMasterr compliance)
//        {
//            var user = GetUserById(compliance.AssignedToID);
//            var supervisor = GetUserById(user.SupervisorId); // Fetch supervisor
//            if (supervisor == null || string.IsNullOrEmpty(supervisor.Email)) return;

//            string subject = $"ESCALATION: Overdue Compliance {compliance.ComplianceArea}";
//            string body = $@"Dear {supervisor.Email.Split('@')[0]},

//This is an escalation notice regarding an overdue compliance task assigned to {user?.Email.Split('@')[0] ?? "Unknown User"}.
//Details:
//- Compliance Area: {compliance.ComplianceArea}
//- Type: {compliance.ComplianceTypeName}
//- Due Date: {compliance.StandardDueDate:dd-MMM-yyyy}
//- Status: {compliance.ComplianceStatusName}

//Please follow up with the responsible party to ensure completion.

//Best regards,
//Compliance Manager Team";

//            SendEmail(supervisor.Email, subject, body);
//        }

//        private static void SendEmail(string toEmail, string subject, string body)
//        {
//            try
//            {
//                using (MailMessage mail = new MailMessage(FromEmail, toEmail))
//                {
//                    mail.Subject = subject;
//                    mail.Body = body;
//                    mail.IsBodyHtml = true;

//                    using (SmtpClient smtp = new SmtpClient(SmtpServer, SmtpPort))
//                    {
//                        smtp.Credentials = new System.Net.NetworkCredential(SmtpUsername, SmtpPassword);
//                        smtp.EnableSsl = true;
//                        smtp.Send(mail);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                // Log the exception (e.g., using log4net, Serilog, or a simple file)
//                System.Diagnostics.Debug.WriteLine($"Email sending failed: {ex.Message}");
//            }
//        }

//        private static User GetUserById(int userId)
//        {
//            User user = null;
//            string query = "SELECT user_id, Email, SupervisorID, user_name FROM compliance.x_user_master WHERE UserID = @UserID";

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@UserID", userId);
//                    conn.Open();
//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            user = new User
//                            {
//                                UserId = reader.GetInt32(0),
//                                Email = reader.GetString(1),
//                                SupervisorId = reader.GetInt32(2)
//                            };
//                        }
//                    }
//                }
//            }
//            return user;
//        }
    }

}