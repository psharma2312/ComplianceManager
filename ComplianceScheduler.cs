using ComplianceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComplianceManager
{
    public class ComplianceScheduler
    {
        private readonly List<ComplianceMasterr> _complianceMasterList;
        private readonly List<ComplianceMasterDetails> _complianceDetailsList;
        private readonly Dictionary<string, int> _frequencyDictionary;


        public ComplianceScheduler(List<ComplianceMasterr> complianceMasterList, Dictionary<string, int> frequencyDictionary)
        {
            _complianceMasterList = complianceMasterList;
            _complianceDetailsList = new List<ComplianceMasterDetails>();
            _frequencyDictionary = frequencyDictionary;
        }

        public void RunScheduler()
        {
            DateTime today = DateTime.Now;

            foreach (var compliance in _complianceMasterList)
            {
                DateTime nextDueDate = compliance.StandardDueDate;
                TimeSpan frequencySpan = GetFrequencySpan(compliance.FrequencyName);

                while (nextDueDate <= today.AddYears(1))
                {
                    if (nextDueDate >= today)
                    {
                        _complianceDetailsList.Add(new ComplianceMasterDetails
                        {
                            ComplianceID = compliance.ComplianceID,
                            ComplianceDetailID = 0,
                            //ComplianceArea = compliance.ComplianceArea, TODO
                            AssignedDate = DateTime.Now,
                            CreationDate = DateTime.Now,
                            DeptId = 0,
                            DocumentsUploaded = "N",
                            ComplianceStatusId = 1,
                            AssignedToID = compliance.AssignedToID,
                            IsActive = true
                        });
                    }
                    nextDueDate = nextDueDate.Add(frequencySpan);
                }
            }
            SaveToDatabase(_complianceDetailsList);
        }

        private TimeSpan GetFrequencySpan(string frequencyName)
        {
            if (_frequencyDictionary.TryGetValue(frequencyName.ToLower(), out int days))
            {
                return TimeSpan.FromDays(days);
            }

            throw new Exception($"Frequency '{frequencyName}' not found in the database.");
        }

        //private TimeSpan GetFrequencySpan(string frequency)
        //{
        //    return frequency.ToLower() switch
        //    {
        //        "monthly" => TimeSpan.FromDays(30),
        //        "yearly" => TimeSpan.FromDays(365),
        //        "weekly" => TimeSpan.FromDays(7),
        //        "bi-monthly" => TimeSpan.FromDays(60),
        //        "every 6 months" => TimeSpan.FromDays(182),
        //        "every 3 months" => TimeSpan.FromDays(91),
        //        _ => TimeSpan.Zero
        //    };
        //}

        private void SaveToDatabase(List<ComplianceMasterDetails> complianceDetails)
        {
            // Implement database saving logic using EF Core or ADO.NET
            Console.WriteLine($"Saved {complianceDetails.Count} compliance details to the database.");
        }
    }
}



//class Program
//{
//    static void Main(string[] args)
//    {
//        var complianceMasterList = GetComplianceMasterFromDatabase();
//          var frequencyDictionary = GetFrequencyFromDatabase();
//        var scheduler = new ComplianceScheduler(complianceMasterList, frequencyDictionary);

//        scheduler.RunScheduler();
//    }

//    static List<ComplianceMaster> GetComplianceMasterFromDatabase()
//    {
//        // Fetch data from the ComplianceMaster table
//        // Simulating with sample data for now
//        return new List<ComplianceMaster>
//        {
//            new ComplianceMaster
//            {
//                ComplianceId = 4,
//                ComplianceArea = "Food",
//                TypeOfCompliance = "Rule",
//                GoverningLegislation = "FSSAI",
//                ActSectionRef = "Schedule 1",
//                NatureOfCompliance = "Contractor responsibility",
//                DetailsOfComplianceRequirements = "FSSAI License renewal",
//                EffectiveFromDate = new DateTime(2023, 10, 20),
//                Frequency = "every 6 months",
//                StandardDueDate = new DateTime(2024, 08, 20),
//                Creator = "user2"
//            }
//            // Add more compliance records here
//        };
//    }
//}

//static Dictionary<string, int> GetFrequencyFromDatabase()
//{
//    // Simulating database call
//    // In real implementation, use ADO.NET or EF Core to fetch data
//    return new Dictionary<string, int>
//        {
//            { "monthly", 30 },
//            { "yearly", 365 },
//            { "weekly", 7 },
//            { "bi-monthly", 60 },
//            { "every 6 months", 182 },
//            { "every 3 months", 91 }
//        };
//}
//static Dictionary<string, int> GetFrequencyFromDatabase()
//{
//    using (var context = new YourDbContext())
//    {
//        return context.FrequencyMaster
//                      .ToDictionary(f => f.FrequencyName.ToLower(), f => f.DurationInDays);
//    }
//}

