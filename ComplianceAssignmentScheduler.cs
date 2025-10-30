using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Timers;
using System.Configuration;

namespace ComplianceManager
{
    public class ComplianceAssignmentScheduler
    {
        private static Timer timer;
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ComplianceConnection"].ConnectionString;

        public static void Start()
        {
            timer = new Timer(24 * 60 * 60 * 1000); // Run every 24 hours
            timer.Elapsed += CheckAndAssignCompliances;
            timer.AutoReset = true;
            timer.Start();
            CheckAndAssignCompliances(null, null); // Run immediately on start
        }
        private static void CheckAndAssignCompliances(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;
            List<ComplianceMasterr> activeCompliances = GetActiveCompliances();

            foreach (var compliance in activeCompliances)
            {
                if (ShouldAssignNewInstance(compliance, now))
                {
                    AssignNewComplianceInstance(compliance, now);
                }
            }
        }
        private static List<ComplianceMasterr> GetActiveCompliances()
        {
            List<ComplianceMasterr> compliances = new List<ComplianceMasterr>();
            string query = "SELECT ComplianceID, ComplianceDetailID, ComplianceArea, ComplianceTypeName, GovernmentLegislation, " +
                           "ActSectionReference, NatureOfComplianceName, EffectiveFrom, StandardDueDate, ComplianceStatusName, UserID, SupervisorID, " +
                           "Frequency, OccursEvery, DayOfMonth " +
                           "FROM Compliances WHERE ComplianceStatusName != 'Completed'";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            compliances.Add(new ComplianceMasterr
                            {
                                //ComplianceID = reader.GetInt32(0),
                                //ComplianceDetailID = reader.GetInt32(1),
                                //ComplianceArea = reader.GetString(2),
                                //ComplianceTypeName = reader.GetString(3),
                                //GovernmentLegislation = reader.GetString(4),
                                //ActSectionReference = reader.GetString(5),
                                //NatureOfComplianceName = reader.GetString(6),
                                //EffectiveFrom = reader.GetDateTime(7),
                                //StandardDueDate = reader.GetDateTime(8),
                                //ComplianceStatusName = reader.GetString(9),
                                //AssignedToID = reader.GetInt32(10),
                                //SupervisorID = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                                //Frequency = reader.GetString(12),
                                //OccursEvery = reader.GetString(13),
                                //DayOfMonth = reader.GetInt32(14)
                            });
                        }
                    }
                }
            }
            return compliances;
        }

        private static bool ShouldAssignNewInstance(ComplianceMasterr compliance, DateTime now)
        {
            //if (compliance.ComplianceStatusName == "Completed") return false;

            // DateTime nextDueDate = CalculateNextDueDate(compliance.StandardDueDate, compliance.Frequency, compliance.OccursEvery, compliance.DayOfMonth);
            // return nextDueDate.Date == now.Date;
            return true;
        }

        private static DateTime CalculateNextDueDate(DateTime currentDueDate, string frequency, string occursEvery, int dayOfMonth)
        {
            DateTime nextDueDate = currentDueDate;
            int monthsToAdd = 0;

            switch (occursEvery.ToLower())
            {
                case "monthly":
                    monthsToAdd = 1;
                    break;
                case "quarterly":
                    monthsToAdd = 3;
                    break;
                case "annually":
                    monthsToAdd = 12;
                    break;
                default:
                    throw new ArgumentException("Invalid OccursEvery value");
            }

            nextDueDate = nextDueDate.AddMonths(monthsToAdd);

            // Ensure the due date is on the specified day of the month
            if (nextDueDate.Day != dayOfMonth)
            {
                nextDueDate = new DateTime(nextDueDate.Year, nextDueDate.Month, dayOfMonth);
                if (nextDueDate < DateTime.Now) // If the calculated date is in the past, move to next occurrence
                {
                    nextDueDate = nextDueDate.AddMonths(monthsToAdd);
                }
            }

            return nextDueDate;
        }
        private static void AssignNewComplianceInstance(ComplianceMasterr compliance, DateTime now)
        {
            //DateTime newDueDate = CalculateNextDueDate(compliance.StandardDueDate, compliance.Frequency, compliance.OccursEvery, compliance.DayOfMonth);

            //string insertQuery = "INSERT INTO Compliances (ComplianceDetailID, ComplianceArea, ComplianceTypeName, GovernmentLegislation, " +
            //                    "ActSectionReference, NatureOfComplianceName, EffectiveFrom, StandardDueDate, ComplianceStatusName, UserID, SupervisorID, Frequency, OccursEvery, DayOfMonth) " +
            //                    "VALUES (@ComplianceDetailID, @ComplianceArea, @ComplianceTypeName, @GovernmentLegislation, @ActSectionReference, " +
            //                    "@NatureOfComplianceName, @EffectiveFrom, @StandardDueDate, @ComplianceStatusName, @UserID, @SupervisorID, @Frequency, @OccursEvery, @DayOfMonth)";

            //using (SqlConnection conn = new SqlConnection(ConnectionString))
            //{
            //    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
            //    {
            //        cmd.Parameters.AddWithValue("@ComplianceDetailID", compliance.ComplianceDetailID);
            //        cmd.Parameters.AddWithValue("@ComplianceArea", compliance.ComplianceArea);
            //        cmd.Parameters.AddWithValue("@ComplianceTypeName", compliance.ComplianceTypeName);
            //        cmd.Parameters.AddWithValue("@GovernmentLegislation", compliance.GovernmentLegislation);
            //        cmd.Parameters.AddWithValue("@ActSectionReference", compliance.ActSectionReference);
            //        cmd.Parameters.AddWithValue("@NatureOfComplianceName", compliance.NatureOfComplianceName);
            //        cmd.Parameters.AddWithValue("@EffectiveFrom", now);
            //        cmd.Parameters.AddWithValue("@StandardDueDate", newDueDate);
            //        cmd.Parameters.AddWithValue("@ComplianceStatusName", "Pending");
            //        cmd.Parameters.AddWithValue("@UserID", compliance.UserID);
            //        cmd.Parameters.AddWithValue("@SupervisorID", (object)compliance.SupervisorID ?? DBNull.Value);
            //        cmd.Parameters.AddWithValue("@Frequency", compliance.Frequency);
            //        cmd.Parameters.AddWithValue("@OccursEvery", compliance.OccursEvery);
            //        cmd.Parameters.AddWithValue("@DayOfMonth", compliance.DayOfMonth);

            //        conn.Open();
            //        cmd.ExecuteNonQuery();
            //    }
            //}
        }

    }
}