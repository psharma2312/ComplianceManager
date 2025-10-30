using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Configuration;
namespace ComplianceManager
{
    public class DAL
    {
        private readonly ActionLogger _logger;
        private readonly DbHelper _db;

        public DAL()
        {
            _logger = HttpContext.Current.Session["Logger"] as ActionLogger;
            _db = new DbHelper(connectionString, _logger);
        }


        //TODO: GetUserComplianceSummary  use this to send mail or anything 
        public string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        public DataSet ds = new DataSet();
        public SqlConnection GetConnection()
        {
            SqlConnection con = new SqlConnection(connectionString);
            return con;
        }
        public List<string> GetCode(string empName)
        {
            var empResult = new List<string>();
            try
            {
                _logger.Info($"Starting GetCode for empName: {empName}");
                var parameters = new Dictionary<string, object> { { "@SearchEmpName", empName } };
                var reader = _db.ExecuteQuery("SELECT Code FROM TProduct WHERE Code LIKE ''+@SearchEmpName+'%'", parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        empResult.Add(reader["Code"].ToString());
                    }
                    reader.Close();
                }

                _logger.Info($"GetCode completed. Results found: {empResult.Count}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception in GetCode: {ex.Message}");
            }

            return empResult;
        }

        public User CheckUserPassword(string username, string password, int unitId)
        {
            var user = new User();
            try
            {
                _logger.Info($"Starting CheckUserPassword for user: {username}");
                var parameters = new Dictionary<string, object>
            {
                { "@UserName", username },
                { "@Password", password }
            };

                var reader = _db.ExecuteReader("Compliance.uspGetLoginPassword", parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        user.UserId = Convert.ToInt32(reader["UserId"]);
                        user.UserName = reader["UserName"].ToString();
                        user.Password = reader["Password"].ToString();
                        user.Name = reader["Name"].ToString();
                        user.RoleId = Convert.ToInt32(reader["role_id"]);
                        user.RoleName = reader["role_name"].ToString();
                    }
                    reader.Close();
                }

                _logger.Info($"CheckUserPassword completed. User found: {(user.UserId > 0 ? "Yes" : "No")}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception in CheckUserPassword for user {username}: {ex.Message}");
            }

            return user;
        }


        public DataSet GetUserPhoneNumber(string username)
        {
            var stopwatch = Stopwatch.StartNew();
            var ds = new DataSet();
            try
            {
                _logger.Info($"Starting GetUserPhoneNumber for username: {username}");
                var parameters = new Dictionary<string, object> { { "@Username", username } };

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("Compliance.GetMobileNo", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    _db.AddParameters(cmd, parameters);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                }

                stopwatch.Stop();
                _logger.Info($"GetUserPhoneNumber completed in {stopwatch.ElapsedMilliseconds} ms. Rows: {ds.Tables[0].Rows.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetUserPhoneNumber: {ex.Message}\n{ex.StackTrace}");
            }

            return ds;
        }

        public List<UserNameLst> GetUsers()
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<UserNameLst>();
            try
            {
                _logger.Info("Starting GetUsers");
                var reader = _db.ExecuteReader("Compliance.UspUserDropDownValues", new Dictionary<string, object>());

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        data.Add(new UserNameLst
                        {
                            UserName = reader["UserName"].ToString(),
                            UserId = Convert.ToInt32(reader["UserId"]),
                        });
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"GetUsers completed in {stopwatch.ElapsedMilliseconds} ms. Total: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetUsers: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }


        public int GetUserCount(string userId)
        {
            try
            {
                _logger.Info($"Starting GetUserCount for userId: {userId}");
                var parameters = new Dictionary<string, object> { { "@userID", userId } };
                object result = _db.ExecuteScalar("Compliance.GetUserCount", parameters);
                int count = result != null ? Convert.ToInt32(result) : 0;
                _logger.Info($"GetUserCount result: {count}");
                return count;
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception in GetUserCount: {ex.Message}");
                return -1;
            }

        }

        public int GetNotificationCount(int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting GetNotificationCount for userId: {userId}");
                var parameters = new Dictionary<string, object> { { "@UserId", userId } };
                object result = _db.ExecuteScalar("Compliance.GetNotificationCount", parameters);
                int count = result != null ? Convert.ToInt32(result) : 0;
                stopwatch.Stop();
                _logger.Info($"GetNotificationCount completed in {stopwatch.ElapsedMilliseconds} ms. Count: {count}");
                return count;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetNotificationCount: {ex.Message}\n{ex.StackTrace}");
                return -1;
            }
        }

        public int GetTotalUserCount()
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info("Starting GetTotalUserCount");
                object result = _db.ExecuteScalar("Compliance.GetSystemUserCount", new Dictionary<string, object>());
                int count = result != null ? Convert.ToInt32(result) : 0;
                stopwatch.Stop();
                _logger.Info($"GetTotalUserCount completed in {stopwatch.ElapsedMilliseconds} ms. Count: {count}");
                return count;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetTotalUserCount: {ex.Message}\n{ex.StackTrace}");
                return -1;
            }
        }

        public List<User> GetAllUsers(int UserID = 0)
        {
            var stopwatch = Stopwatch.StartNew();
            var users = new List<User>();
            try
            {
                _logger.Info($"Starting GetAllUsers with UserID: {UserID}");
                var reader = _db.ExecuteReader("Compliance.GetAllUsers", new Dictionary<string, object>());

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            SupervisorId = (int)reader["SupervisorId"],
                            SupervisorName = Convert.ToString(reader["SupervisorName"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            IsApprover = Convert.ToBoolean(reader["IsApprover"]),
                            IsPreparer = Convert.ToBoolean(reader["IsPreparer"]),
                            DepartmentId = (int)reader["dept_id"],
                            DepartmentName = Convert.ToString(reader["DepartmentName"]),
                            Email = Convert.ToString(reader["email"]),
                            MobileNo = Convert.ToString(reader["mobile_no"]),
                            UserId = (int)reader["user_id"],
                            UserName = Convert.ToString(reader["user_name"]),
                            Name = Convert.ToString(reader["name"]),
                        });
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"GetAllUsers completed in {stopwatch.ElapsedMilliseconds} ms. Total users fetched: {users.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllUsers: {ex.Message}\n{ex.StackTrace}");
            }

            return users;
        }

        public List<string> GetMasterData(string name)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<string>();
            try
            {
                _logger.Info($"Starting GetMasterData for name: {name}");
                var parameters = new Dictionary<string, object> { { "@Name", name } };
                var reader = _db.ExecuteReader("Compliance.GetMasterData", parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        data.Add(reader["Name"].ToString());
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"GetMasterData completed in {stopwatch.ElapsedMilliseconds} ms. Items: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetMasterData: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }

        public void CreateUser(string username, string email, string mobile, string departmentId, string supervisorUsername, string hashedPassword, bool isApprover, bool isPreparer, bool isSupervisor, bool isActive)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting CreateUser for: {username}");
                var parameters = new Dictionary<string, object>
            {
                { "@user_name", username },
                { "@email", email },
                { "@mobile_no", mobile },
                { "@Dept_Id", departmentId },
                { "@SupervisorId", string.IsNullOrEmpty(supervisorUsername) ? (object)DBNull.Value : supervisorUsername },
                { "@current_pwd", hashedPassword },
                { "@IsApprover", isApprover },
                { "@IsPreparer", isPreparer },
                { "@IsSupervisor", isSupervisor },
                { "@IsActive", isActive }
            };

                _db.ExecuteNonQuery("compliance.USP_CreateUser_InsertHeader", parameters);
                stopwatch.Stop();
                _logger.Info($"CreateUser completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in CreateUser for {username}: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public int ChangeUserPassword(string username, string hashedPassword)
        {
            int rowsAffected = 0;
            try
            {
                _logger?.Info($"Starting ChangeUserPassword for user: {username}");
                var parameters = new Dictionary<string, object>
                {
                    { "@user_name", username },
                    { "@NewPassword", hashedPassword }
                };
                int result = _db.ExecuteNonQuery("compliance.usp_ChangePassword", parameters);
                _logger.Info($"ChangeUserPassword completed for user: {username} with result: {result}");

                return result;
            }
            catch (Exception ex)
            {
                _logger?.Error($"Error in ChangeUserPassword for user: {username}. Exception: {ex.Message}");
                return -1;
            }
        }

        public List<ComplianceType> GetAllComplianceType(int DeptID = 0)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceType>();
            try
            {
                _logger.Info($"Starting GetAllComplianceType for DeptID: {DeptID}");
                var parameters = new Dictionary<string, object> { { "@DeptId", DeptID } };
                var reader = _db.ExecuteReader("Compliance.GetAllComplianceType", parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        data.Add(new ComplianceType
                        {
                            ComplianceTypeID = Convert.ToInt32(reader["ComplianceTypeID"]),
                            ComplianceTypeName = reader["ComplianceTypeName"].ToString(),
                            Description = reader["Description"].ToString()
                        });
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"GetAllComplianceType completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllComplianceType: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }

        public void AssignComplianceToUser(int complianceId)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting AssignComplianceToUser for complianceId: {complianceId}");

                var parameters = new Dictionary<string, object>
        {
            { "@complianceId", complianceId }
        };

                _db.ExecuteNonQuery("compliance.AssignComplianceToUser", parameters);

                stopwatch.Stop();
                _logger.Info($"AssignComplianceToUser completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in AssignComplianceToUser: {ex.Message}\n{ex.StackTrace}");
            }
        }


        public string GetComplianceDetailFilePath(int complianceDetailId)
        {
            var stopwatch = Stopwatch.StartNew();
            string dirPath = string.Empty;
            try
            {
                _logger.Info($"Starting GetComplianceDetailFilePath for complianceDetailId: {complianceDetailId}");
                var parameters = new Dictionary<string, object> { { "@complianceDetailId", complianceDetailId } };
                object result = _db.ExecuteScalar("Compliance.GetComplianceDetailFilePath", parameters);
                dirPath = result?.ToString();
                stopwatch.Stop();
                _logger.Info($"GetComplianceDetailFilePath completed in {stopwatch.ElapsedMilliseconds} ms. Path: {dirPath}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetComplianceDetailFilePath: {ex.Message}\n{ex.StackTrace}");
            }

            return dirPath;
        }

        public List<ComplianceCertificate> GetComplianceCertificatePath(string periodicity, string userName)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceCertificate>();
            try
            {
                _logger.Info($"Starting GetComplianceCertificatePath for {userName}, periodicity: {periodicity}");
                var parameters = new Dictionary<string, object>
        {
            { "@periodicity", periodicity },
            { "@userName", userName }
        };
                var reader = _db.ExecuteReader("Compliance.GetComplianceCertificatePath", parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        data.Add(new ComplianceCertificate
                        {
                            ComplainceDetailID = Convert.ToInt32(reader["ComplainceDetailID"]),
                            CertificateId = Convert.ToInt32(reader["CertificateId"]),
                            CertificatePath = reader["CertificatePath"].ToString(),
                            UserName = reader["UserName"].ToString(),
                            GeneratedOn = Convert.ToDateTime(reader["GeneratedOn"]),
                            Period = reader["Period"].ToString(),
                        });
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"GetComplianceCertificatePath completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetComplianceCertificatePath: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }

        public int SaveComplianceCertificate(string periodicity, string generatedByUserName, int generatedByUserId, string certificateNo)
        {
            var stopwatch = Stopwatch.StartNew();
            int certificateId = 0;

            try
            {
                _logger.Info($"Starting SaveComplianceCertificate for user: {generatedByUserName}, periodicity: {periodicity}");

                using (SqlConnection con = GetConnection())
                using (SqlCommand cmd = new SqlCommand("compliance.SaveComplianceCertificate", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@periodicity", periodicity);
                    cmd.Parameters.AddWithValue("@generatedByUserName", generatedByUserName);
                    cmd.Parameters.AddWithValue("@generatedByUserId", generatedByUserId);
                    cmd.Parameters.AddWithValue("@certificateNo", certificateNo);

                    SqlParameter outputParam = new SqlParameter("@CertificateId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    certificateId = (int)outputParam.Value;
                    con.Close();
                }

                stopwatch.Stop();
                _logger.Info($"SaveComplianceCertificate completed in {stopwatch.ElapsedMilliseconds} ms. CertificateId: {certificateId}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in SaveComplianceCertificate: {ex.Message}\n{ex.StackTrace}");
                throw;
            }

            return certificateId;
        }

        public int SaveComplianceCertificateFilePath(string directoryName, string periodicity, string userName, int certificateUserId, string fileName)
        {
            var stopwatch = Stopwatch.StartNew();
            int certificateId = 0;
            try
            {
                _logger.Info($"Starting SaveComplianceCertificateFilePath for {userName}, file: {fileName}");

                using (SqlConnection con = GetConnection())
                using (SqlCommand cmd = new SqlCommand("compliance.SaveComplianceCertificate", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@directoryName", directoryName);
                    cmd.Parameters.AddWithValue("@periodicity", periodicity);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@certificateUserId", certificateUserId);
                    cmd.Parameters.AddWithValue("@fileName", fileName);

                    SqlParameter certificateIdParam = new SqlParameter("@CertificateId", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(certificateIdParam);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    certificateId = (int)certificateIdParam.Value;
                        con.Close(); 
                }
                stopwatch.Stop();
                _logger.Info($"SaveComplianceCertificateFilePath completed in {stopwatch.ElapsedMilliseconds} ms. CertificateId: {certificateId}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in SaveComplianceCertificateFilePath: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
           
            return certificateId;
        }

        public void UpdateCertificateIdInComplianceDetail(int certificateId, int complianceDetailId)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting UpdateCertificateIdInComplianceDetail for CertificateId: {certificateId}, ComplianceDetailId: {complianceDetailId}");
                var parameters = new Dictionary<string, object>
        {
            { "@CertificateId", certificateId },
            { "@ComplianceDetailId", complianceDetailId }
        };
                _db.ExecuteNonQuery("Compliance.sp_UpdateComplianceCertificateId", parameters);
                stopwatch.Stop();
                _logger.Info($"UpdateCertificateIdInComplianceDetail completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in UpdateCertificateIdInComplianceDetail: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void UpdateComplianceCertificateFilePath(int certificateId, string filePath, string fileName, string certificateNo)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting UpdateComplianceCertificateFilePath for CertificateId: {certificateId}");
                var parameters = new Dictionary<string, object>
        {
            { "@CertificateId", certificateId },
            { "@FilePath", filePath },
            { "@FileName", fileName },
            { "@CertificateNo", certificateNo }
        };
                _db.ExecuteNonQuery("Compliance.sp_UpdateComplianceCertificateFilePath", parameters);
                stopwatch.Stop();
                _logger.Info($"UpdateComplianceCertificateFilePath completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in UpdateComplianceCertificateFilePath: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public List<ComplianceMasterDetailCombined> GetDashboardData(string queryType, int userId)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceMasterDetailCombined>();
            try
            {
                _logger.Info($"Starting GetDashboardData for userId: {userId}, queryType: {queryType}");
                var parameters = new Dictionary<string, object>
        {
            { "@queryType", queryType },
            { "@userId", userId }
        };
                var reader = _db.ExecuteReader("Compliance.usp_GetComplianceMasterDashboard", parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        data.Add(new ComplianceMasterDetailCombined
                        {
                            ComplianceID = (int)reader["ComplianceID"],
                            ComplianceDetailID = (int)reader["ComplianceDetailID"],
                            ComplianceArea = reader["ComplianceArea"].ToString(),
                            ComplianceTypeID = (int)reader["ComplianceTypeID"],
                            ComplianceTypeName = reader["ComplianceTypeName"].ToString()
                        });
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"GetDashboardData completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetDashboardData: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }

        public void SaveFileInfo(int newComplianceId, string fileName, string filePath, string insertIntoTable)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting SaveFileInfo for ComplianceId: {newComplianceId}, file: {fileName}");

                string docQuery = insertIntoTable == "temp"
                    ? "INSERT INTO Compliance.ComplianceDocuments_Temp (ComplianceId, FileName, FilePath) VALUES (@ComplianceId, @FileName, @FilePath)"
                    : "INSERT INTO Compliance.ComplianceDocuments (ComplianceId, FileName, FilePath) VALUES (@ComplianceId, @FileName, @FilePath)";

                var parameters = new Dictionary<string, object>
        {
            { "@ComplianceId", newComplianceId },
            { "@FileName", fileName },
            { "@FilePath", filePath }
        };

                _db.ExecuteNonQuery(docQuery, parameters); // This assumes ExecuteNonQuery supports raw SQL

                stopwatch.Stop();
                _logger.Info($"SaveFileInfo completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in SaveFileInfo: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void SaveDate(DateTime effectiveDate)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting SaveDate with EffectiveFrom: {effectiveDate:yyyy-MM-dd}");

                var parameters = new Dictionary<string, object>
        {
            { "@EffectiveFrom", effectiveDate }
        };

                _db.ExecuteNonQuery("compliance.USP_SaveDateTest", parameters);

                stopwatch.Stop();
                _logger.Info($"SaveDate completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in SaveDate: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public bool IsRefUnique(string refToCheck)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting IsRefUnique for ref: {refToCheck}");
                var parameters = new Dictionary<string, object> { { "@ComplianceRef", refToCheck } };
                object result = _db.ExecuteScalar("SELECT COUNT(*) FROM Compliance.ComplianceMaster WHERE ComplianceRef = @ComplianceRef", parameters);
                int count = result != null ? Convert.ToInt32(result) : -1;
                stopwatch.Stop();
                _logger.Info($"IsRefUnique completed in {stopwatch.ElapsedMilliseconds} ms. Count: {count}");
                return count == 0;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in IsRefUnique: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        public int SaveComplianceMaster(ComplianceMasterr comp)
        {
            var stopwatch = Stopwatch.StartNew();
            int newComplianceId = 0;

            try
            {
                _logger.Info($"Starting SaveComplianceMaster for CreatedById: {comp.CreatedById}");

                var parameters = new Dictionary<string, object>
        {
            { "@buId", comp.BusinessUnitID },
            { "@compTypeId", comp.ComplianceTypeID },
            { "@drivenById", comp.DrivenById },
            { "@ClauseRef", comp.ClauseRef },
            { "@act", comp.ActSectionReference },
            { "@natureId", comp.ComplianceNatureID },
            { "@lawId", comp.LawId },
            { "@territoryId", comp.TerritoryId },
            { "@priorityId", comp.PriorityId },
            { "@frequencyId", comp.FrequencyID },
            { "@EffectiveFrom", comp.EffectiveFrom },
            { "@standardDate", comp.StandardDueDate },
            { "@firstDueDate", comp.FirstDueDate },
            { "@deptId", comp.DeptId },
            { "@initiatorId", comp.InitiatorId },
            { "@approverId", comp.ApproverId },
            { "@dueonEvery", comp.DueOnEvery },
            { "@createdById", comp.CreatedById },
            { "@details", comp.DetailsOfComplianceRequirements },
            { "@action", comp.ActionsToBeTaken },
            { "@penalty", comp.NonCompliancePenalty }
        };

                object result = _db.ExecuteScalar("compliance.SaveComplianceMaster", parameters);

                if (result != null && result != DBNull.Value)
                {
                    newComplianceId = Convert.ToInt32(result);
                }
                else
                {
                    throw new Exception("Failed to retrieve new ComplianceID. Check the INSERT query or database constraints.");
                }

                stopwatch.Stop();
                _logger.Info($"SaveComplianceMaster completed in {stopwatch.ElapsedMilliseconds} ms. NewComplianceId: {newComplianceId}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in SaveComplianceMaster: {ex.Message}\n{ex.StackTrace}");
            }

            return newComplianceId;
        }
        public List<ComplianceMasterDetailCombined> LoadComplianceForApproval(int approverId, int statusid)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceMasterDetailCombined>();
            try
            {
                _logger.Info($"Starting LoadComplianceForApproval for approverId: {approverId}, statusId: {statusid}");
                var parameters = new Dictionary<string, object>
        {
            { "@UserId", approverId },
            { "@StatusID", statusid }
        };

                var reader = _db.ExecuteReader("Compliance.usp_GetApproverComplianceMaster", parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        data.Add(new ComplianceMasterDetailCombined
                        {
                            ComplianceID = (int)reader["ComplianceID"],
                            ComplianceDetailID = (int)reader["ComplianceDetailID"],
                            ComplianceRef = reader["ComplianceRef"].ToString(),
                            ComplianceTypeID = (int)reader["ComplianceTypeID"],
                            ComplianceTypeName = reader["ComplianceTypeName"].ToString(),
                            ActSectionReference = reader["ActSectionReference"].ToString(),
                            ComplianceNatureID = (int)reader["NatureOfComplianceID"],
                            NatureOfComplianceName = reader["NatureOfComplianceName"].ToString(),
                            FrequencyID = (int)reader["FrequencyID"],
                            FrequencyName = reader["FrequencyName"].ToString(),
                            DeptId = (int)reader["DepartmentFunctionID"],
                            DepartmentName = reader["DepartmentFunctionName"].ToString(),
                            EffectiveFrom = (DateTime)reader["EffectiveFrom"],
                            StandardDueDate = (DateTime)reader["StandardDueDate"],
                            DetailsOfComplianceRequirements = reader["DetailsOfComplianceRequirements"].ToString(),
                            AssignedToID = (int)reader["AssignedToID"],
                            ComplianceStatusId = (int)reader["ComplianceStatusId"],
                            ComplianceStatusName = reader["ComplianceStatusName"].ToString()
                        });
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"LoadComplianceForApproval completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadComplianceForApproval: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public ComplianceMasterDetailCombined LoadComplianceDetailsForApproval(int approverId, int complianceDetailId, int commplianceId)
        {
            var stopwatch = Stopwatch.StartNew();
            var objSKU = new ComplianceMasterDetailCombined();
            try
            {
                _logger.Info($"Starting LoadComplianceDetailsForApproval for approverId: {approverId}");

                var parameters = new Dictionary<string, object>
        {
            { "@ApproverId", approverId },
            { "@complianceDetailId", complianceDetailId },
            { "@commplianceId", commplianceId }
        };

                var reader = _db.ExecuteReader("Compliance.usp_LoadComplianceDetailsForApproval", parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        objSKU.ComplianceID = (int)reader["ComplianceID"];
                        objSKU.ComplianceDetailID = (int)reader["ComplianceDetailID"];
                        objSKU.ComplianceRef = reader["ComplianceRef"].ToString();
                        objSKU.ComplianceTypeID = (int)reader["ComplianceTypeID"];
                        objSKU.ComplianceTypeName = reader["ComplianceTypeName"].ToString();
                        objSKU.ActSectionReference = reader["ActSectionReference"].ToString();
                        objSKU.ComplianceNatureID = (int)reader["NatureOfComplianceID"];
                        objSKU.NatureOfComplianceName = reader["NatureOfComplianceName"].ToString();
                        objSKU.FrequencyID = (int)reader["FrequencyID"];
                        objSKU.FrequencyName = reader["FrequencyName"].ToString();
                        objSKU.DeptId = (int)reader["DepartmentFunctionID"];
                        objSKU.DepartmentName = reader["DepartmentFunctionName"].ToString();
                        objSKU.EffectiveFrom = (DateTime)reader["EffectiveFrom"];
                        objSKU.StandardDueDate = (DateTime)reader["StandardDueDate"];
                        objSKU.DetailsOfComplianceRequirements = reader["DetailsOfComplianceRequirements"].ToString();
                        objSKU.UserComments = reader["UserComments"].ToString();
                        objSKU.ApproverComments = reader["ApproverComments"].ToString();
                        objSKU.AssignedToID = (int)reader["AssignedToID"];
                        objSKU.ComplianceStatusId = (int)reader["ComplianceStatusId"];
                        objSKU.ComplianceStatusName = reader["ComplianceStatusName"].ToString();
                        objSKU.AssignedToName = reader["UserName"].ToString();
                        objSKU.UserEmail = reader["UserEmail"].ToString();
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"LoadComplianceDetailsForApproval completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadComplianceDetailsForApproval: {ex.Message}\n{ex.StackTrace}");
            }

            return objSKU;
        }

        public List<SideMenu> GenerateSideMenu(int userID)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<SideMenu>();
            try
            {
                _logger.Info($"Starting GenerateSideMenu for userID: {userID}");
                var parameters = new Dictionary<string, object> { { "@userID", userID } };

                data = GetDataFromStoredProcedure<SideMenu>(
                    "Compliance.GenerateSideMenu",
                    parameters,
                    reader => new SideMenu
                    {
                        MenuId = Convert.ToInt32(reader["MenuId"]),
                        ParentMenuId = reader["ParentMenuId"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ParentMenuId"]) : null,
                        MenuText = reader["MenuText"].ToString(),
                        MenuUrl = reader["MenuUrl"].ToString()
                    });

                stopwatch.Stop();
                _logger.Info($"GenerateSideMenu completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GenerateSideMenu: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public ComplianceMasterDetailCombined LoadMyTeamComplianceDetails(int complianceDetailId, int complianceId)
        {
            return LoadComplianceDetails("Compliance.usp_GetTeamComplianceMasterByUserIDDetails", new Dictionary<string, object>
            {
                { "@ComplianceDetailId", complianceDetailId },
                { "@ComplianceId", complianceId }
            });
        }


        public ComplianceMasterDetailCombined LoadMyComplianceDetails(int userID, int complianceDetailId, int complianceId)
        {
            return LoadComplianceDetails("Compliance.usp_GetComplianceMasterByUserIDDetails", new Dictionary<string, object>
            {
                { "@UserId", userID },
                { "@ComplianceDetailId", complianceDetailId },
                { "@ComplianceId", complianceId }
            });
        }
        private ComplianceMasterDetailCombined LoadComplianceDetails(string storedProc, Dictionary<string, object> parameters)
        {
            var stopwatch = Stopwatch.StartNew();
            var obj = new ComplianceMasterDetailCombined();
            try
            {
                _logger.Info($"Starting LoadComplianceDetails: {storedProc}");
                var reader = _db.ExecuteReader(storedProc, parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        obj.ComplianceID = GetSafeInt(reader, "ComplianceID");
                        obj.ComplianceDetailID = GetSafeInt(reader, "ComplianceDetailID");
                        obj.ComplianceRef = GetSafeString(reader, "ComplianceRef");
                        obj.ComplianceArea = GetSafeString(reader, "ComplianceArea");
                        obj.ComplianceTypeID = GetSafeInt(reader, "ComplianceTypeID");
                        obj.ComplianceTypeName = GetSafeString(reader, "ComplianceTypeName");
                        obj.ActSectionReference = GetSafeString(reader, "ActSectionReference");
                        obj.ComplianceNatureID = GetSafeInt(reader, "NatureOfComplianceID");
                        obj.NatureOfComplianceName = GetSafeString(reader, "NatureOfComplianceName");
                        obj.FrequencyID = GetSafeInt(reader, "FrequencyID");
                        obj.FrequencyName = GetSafeString(reader, "FrequencyName");
                        obj.DeptId = GetSafeInt(reader, "DepartmentFunctionID");
                        obj.DepartmentName = GetSafeString(reader, "DepartmentFunctionName");
                        obj.EffectiveFrom = GetSafeDate(reader, "EffectiveFrom");
                        obj.StandardDueDate = GetSafeDate(reader, "StandardDueDate");
                        obj.DetailsOfComplianceRequirements = GetSafeString(reader, "DetailsOfComplianceRequirements");
                        obj.AssignedToID = GetSafeInt(reader, "AssignedToID");
                        obj.ComplianceStatusId = GetSafeInt(reader, "ComplianceStatusId");
                        obj.ComplianceStatusName = GetSafeString(reader, "ComplianceStatusName");
                        obj.UserComments = GetSafeString(reader, "UserComments");
                        obj.ApproverComments = GetSafeString(reader, "ApproverComments");
                        obj.ApproverEmail = GetSafeString(reader, "email");
                        obj.AssignedToName = GetSafeString(reader, "UserName");
                        obj.UserEmail = GetSafeString(reader, "UserEmail");
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"LoadComplianceDetails completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadComplianceDetails: {ex.Message}\n{ex.StackTrace}");
            }

            return obj;

        }
        private int GetSafeInt(SqlDataReader reader, string column)
        {
            return reader.HasColumn(column) && reader[column] != DBNull.Value ? Convert.ToInt32(reader[column]) : 0;
        }

        private string GetSafeString(SqlDataReader reader, string column)
        {
            return reader.HasColumn(column) && reader[column] != DBNull.Value ? reader[column].ToString() : string.Empty;
        }

        private DateTime GetSafeDate(SqlDataReader reader, string column)
        {
            return reader.HasColumn(column) && reader[column] != DBNull.Value ? Convert.ToDateTime(reader[column]) : DateTime.MinValue;
        }
        
        public List<T> PopulateDropdownValues<T>(string dtaType, Func<SqlDataReader, T> mapFunc)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<T>();
            try
            {
                _logger.Info($"Starting PopulateDropdownValues for type: {dtaType}");
                var parameters = new Dictionary<string, object> { { "@MenuType", dtaType } };
                var reader = _db.ExecuteReader("Compliance.GetDropDownValues", parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        data.Add(mapFunc(reader));
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"PopulateDropdownValues completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in PopulateDropdownValues: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        // Generic method for executing a stored procedure and mapping the result to a list of objects
        private List<T> GetDataFromStoredProcedure<T>(string storedProcedureName, Dictionary<string, object> parameters, Func<SqlDataReader, T> mapFunction)
        {
            var data = new List<T>();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.Info($"Executing stored procedure: {storedProcedureName}");
                var reader = _db.ExecuteReader(storedProcedureName, parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        data.Add(mapFunction(reader));
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"Stored procedure {storedProcedureName} completed in {stopwatch.ElapsedMilliseconds} ms. Rows: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetDataFromStoredProcedure<{typeof(T).Name}>: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }

        public List<ComplianceNature> GetAllNatureCompliance()
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceNature>();

            try
            {
                _logger.Info("Starting GetAllNatureCompliance");

                var parameters = new Dictionary<string, object>(); // No parameters

                data = GetDataFromStoredProcedure<ComplianceNature>(
                    "Compliance.GetAllNatureCompliance",
                    parameters,
                    reader => new ComplianceNature
                    {
                        ComplianceNatureID = Convert.ToInt32(reader["ComplianceNatureID"]),
                        NatureOfCompliance = reader["NatureOfCompliance"].ToString(),
                        Description = reader["Description"].ToString()
                    });

                stopwatch.Stop();
                _logger.Info($"GetAllNatureCompliance completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllNatureCompliance: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }

        public List<ComplianceStatuss> GetAllComplianceStatus()
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceStatuss>();

            try
            {
                _logger.Info("Starting GetAllComplianceStatus");

                var parameters = new Dictionary<string, object>(); // No parameters

                data = GetDataFromStoredProcedure<ComplianceStatuss>(
                    "Compliance.GetAllComplianceStatus",
                    parameters,
                    reader => new ComplianceStatuss
                    {
                        ComplianceStatusID = Convert.ToInt32(reader["ComplianceStatusID"]),
                        ComplianceStatusName = reader["ComplianceStatusName"].ToString()
                    });

                stopwatch.Stop();
                _logger.Info($"GetAllComplianceStatus completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllComplianceStatus: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<LawType> GetAllLawTypes()
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<LawType>();

            try
            {
                _logger.Info("Starting GetAllLawTypes");

                var parameters = new Dictionary<string, object>(); // No parameters

                data = GetDataFromStoredProcedure<LawType>(
                    "Compliance.GetAllLaws",
                    parameters,
                    reader => new LawType
                    {
                        LawID = Convert.ToInt32(reader["LawId"]),
                        LawName = reader["LawName"].ToString(),
                        Description = reader["Description"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });

                stopwatch.Stop();
                _logger.Info($"GetAllLawTypes completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllLawTypes: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<Department> GetAllDepartments()
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<Department>();

            try
            {
                _logger.Info("Starting GetAllDepartments");

                var parameters = new Dictionary<string, object>(); // No parameters

                data = GetDataFromStoredProcedure<Department>(
                    "Compliance.GetAllDepartments",
                    parameters,
                    reader => new Department
                    {
                        DeptID = Convert.ToInt32(reader["DeptId"]),
                        DepartmentName = reader["DepartmentName"].ToString(),
                        Description = reader["Description"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });

                stopwatch.Stop();
                _logger.Info($"GetAllDepartments completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllDepartments: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<DriveBy> GetAllDrivenBy()
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<DriveBy>();

            try
            {
                _logger.Info("Starting GetAllDrivenBy");

                var parameters = new Dictionary<string, object>(); // No parameters

                data = GetDataFromStoredProcedure<DriveBy>(
                    "Compliance.GetAllDrivenBy",
                    parameters,
                    reader => new DriveBy
                    {
                        DrivenId = Convert.ToInt32(reader["DivenById"]),
                        DrivenName = reader["DrivenByName"].ToString(),
                        Description = reader["Description"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });

                stopwatch.Stop();
                _logger.Info($"GetAllDrivenBy completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllDrivenBy: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<User> GetAllSupervisors()
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<User>();

            try
            {
                _logger.Info("Starting GetAllSupervisors");

                var parameters = new Dictionary<string, object>(); // No parameters

                data = GetDataFromStoredProcedure<User>(
                    "Compliance.GetAllSupervisors",
                    parameters,
                    reader => new User
                    {
                        SupervisorId = Convert.ToInt32(reader["SupervisorId"]),
                        SupervisorName = reader["SupervisorName"].ToString(),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });

                stopwatch.Stop();
                _logger.Info($"GetAllSupervisors completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllSupervisors: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<UserRights> LoadRights(int userID)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<UserRights>();

            try
            {
                _logger.Info($"Starting LoadRights for userID: {userID}");

                var parameters = new Dictionary<string, object> { { "@userID", userID } };

                data = GetDataFromStoredProcedure<UserRights>(
                    "Compliance.usp_display_module",
                    parameters,
                    reader => new UserRights
                    {
                        ObjectId = Convert.ToInt32(reader["ObjectId"]),
                        ObjectName = reader["ObjectName"].ToString(),
                        Rights = Convert.ToBoolean(reader["Rights"])
                    });

                stopwatch.Stop();
                _logger.Info($"LoadRights completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadRights: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<Department> GetAllDepartmentDropdown()
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<Department>();

            try
            {
                _logger.Info("Starting GetAllDepartmentDropdown");

                var parameters = new Dictionary<string, object>(); // No parameters

                data = GetDataFromStoredProcedure<Department>(
                    "Compliance.GetAllDepartment",
                    parameters,
                    reader => new Department
                    {
                        DeptID = Convert.ToInt32(reader["DeptID"]),
                        DepartmentName = reader["DepartmentName"].ToString()
                    });

                stopwatch.Stop();
                _logger.Info($"GetAllDepartmentDropdown completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllDepartmentDropdown: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<Frequency> GetAllFrequency(int DeptID = 0)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<Frequency>();

            try
            {
                _logger.Info($"Starting GetAllFrequency for DeptID: {DeptID}");

                var parameters = new Dictionary<string, object> { { "@DeptId", DeptID } };

                data = GetDataFromStoredProcedure<Frequency>(
                    "Compliance.GetAllFrequency",
                    parameters,
                    reader => new Frequency
                    {
                        FrequencyID = Convert.ToInt32(reader["FrequencyID"]),
                        FrequencyName = reader["FrequencyName"].ToString(),
                        Description = reader["OccursEvery"].ToString()
                    });

                stopwatch.Stop();
                _logger.Info($"GetAllFrequency completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllFrequency: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<User> GetAllUsersForReport()
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<User>();

            try
            {
                _logger.Info("Starting GetAllUsersForReport");

                var parameters = new Dictionary<string, object>(); // No parameters

                data = GetDataFromStoredProcedure<User>(
                    "Compliance.GetAllUsersForReport",
                    parameters,
                    reader => new User
                    {
                        UserId = Convert.ToInt32(reader["user_id"]),
                        Name = reader["Name"].ToString(),
                        SupervisorName = reader["SupervisorName"].ToString(),
                        DepartmentName = reader["DepartmentName"].ToString(),
                        Email = reader["email"].ToString(),
                        MobileNo = Convert.ToString(reader["mobile_no"]),
                        UserName = reader["user_name"].ToString(),
                        IsApprover = Convert.ToBoolean(reader["IsApprover"]),
                        IsPreparer = Convert.ToBoolean(reader["IsPreparer"]),
                        IsSupervisor = Convert.ToBoolean(reader["IsSupervisor"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });

                stopwatch.Stop();
                _logger.Info($"GetAllUsersForReport completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetAllUsersForReport: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }

        public List<Period> PopulatePeriod(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new Period
            {
                PeriodName = sdr["Code"].ToString(),
                Id = Convert.ToInt32(sdr["ID"]),
            });
        }
        public List<ComplianceType> PopulateComplianceType(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new ComplianceType
            {
                ComplianceTypeID = Convert.ToInt32(sdr["ID"]),
                ComplianceTypeName = sdr["Code"].ToString(),
            });
        }
        public List<Frequency> PopulateFrequency(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new Frequency
            {
                FrequencyName = sdr["Code"].ToString(),
                FrequencyID = Convert.ToInt32(sdr["ID"]),
            });
        }
        public List<Creator> PopulateCreator(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new Creator
            {
                CreatorName = sdr["Code"].ToString(),
                CreatorID = Convert.ToInt32(sdr["ID"]),
            });
        }
        public List<Approver> PopulateApprover(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new Approver
            {
                ApproverName = sdr["Code"].ToString(),
                ApproverID = Convert.ToInt32(sdr["ID"]),
            });
        }
        public List<Territory> PopulateTerritory(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new Territory
            {
                TerritoryName = sdr["Code"].ToString(),
                TerritoryId = Convert.ToInt32(sdr["ID"]),
            });
        }
        public List<Priority> PopulatePriority(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new Priority
            {
                PriorityName = sdr["Code"].ToString(),
                PriorityId = Convert.ToInt32(sdr["ID"]),
            });
        }
        public List<DocumentType> PopulateDocType(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new DocumentType
            {
                DocumentTypeName = sdr["Code"].ToString(),
                DocumentTypeId = Convert.ToInt32(sdr["ID"]),
            });
        }
        public List<DriveBy> PopulateDrivenBy(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new DriveBy
            {
                DrivenId = Convert.ToInt32(sdr["ID"]),
                DrivenName = sdr["Code"].ToString(),
            });
        }
        public List<BusinessUnit> PopulateBusinessUnit(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new BusinessUnit
            {
                BusinessUnitID = Convert.ToInt32(sdr["ID"]),
                BusinessUnitName = sdr["Code"].ToString(),
            });
        }
        public List<Department> PopulateDepartment(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new Department
            {
                DepartmentName = sdr["Code"].ToString(),
                DeptID = Convert.ToInt32(sdr["ID"]),
            });
        }
        public List<LawType> PopulateLawType(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new LawType
            {
                LawID = Convert.ToInt32(sdr["ID"]),
                LawName = sdr["Code"].ToString(),
            });
        }
        public List<ComplianceNature> PopulateComplianceNature(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new ComplianceNature
            {
                ComplianceNatureID = Convert.ToInt32(sdr["ID"]),
                NatureOfCompliance = sdr["Code"].ToString(),
            });
        }

        public List<ComplianceCertificate> LoadExistingCertificate(int generatedByUserId, string periodicity, int certificateId)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceCertificate>();

            try
            {
                _logger.Info($"Starting LoadExistingCertificate for userId: {generatedByUserId}, period: {periodicity}, certificateId: {certificateId}");

                var parameters = new Dictionary<string, object>
        {
            { "@GeneratedByUserId", generatedByUserId },
            { "@period", periodicity },
            { "@CertificateID", certificateId }
        };

                data = GetDataFromStoredProcedure<ComplianceCertificate>(
                    "Compliance.GetComplianceGeneratedCertificates",
                    parameters,
                    reader => new ComplianceCertificate
                    {
                        CertificateNo = reader["CertificateNo"].ToString(),
                        Period = reader["period"].ToString(),
                        FileName = reader["FileName"].ToString(),
                        UserName = reader["UserName"].ToString(),
                        CertificatePath = reader["CertificatePath"].ToString(),
                        GeneratedOn = Convert.ToDateTime(reader["GeneratedOn"])
                    });

                stopwatch.Stop();
                _logger.Info($"LoadExistingCertificate completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadExistingCertificate: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }

        public List<ComplianceCertificate> LoadMyComplianceCertificate(int generatedByUserId, string period, string loadType)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceCertificate>();

            try
            {
                _logger.Info($"Starting LoadMyComplianceCertificate for userId: {generatedByUserId}, period: {period}, loadType: {loadType}");

                var parameters = new Dictionary<string, object>
        {
            { "@GeneratedByUserId", generatedByUserId },
            { "@period", period },
            { "@loadType", loadType }
        };

                data = GetDataFromStoredProcedure<ComplianceCertificate>(
                    "Compliance.GetComplianceCertificates",
                    parameters,
                    reader => new ComplianceCertificate
                    {
                        CertificateNo = reader["CertificateNo"].ToString(),
                        ComplainceRef = reader["ComplianceRef"].ToString(),
                        Period = reader["period"].ToString(),
                        ActSection = reader["ActSection"].ToString(),
                        ComplianceTypeName = reader["ComplianceTypeName"].ToString(),
                        ClauseRef = reader["ClauseRef"].ToString(),
                        DetailsOfComplianceReq = reader["DetailsOfComplianceReq"].ToString(),
                        TypeOfNonCompliance = reader["TypeOfNonCompliance"].ToString(),
                        Remarks = reader["Remarks"].ToString(),
                        FileName = reader["FileName"].ToString(),
                        UserName = reader["UserName"].ToString(),
                        CertificatePath = reader["CertificatePath"].ToString(),
                        GeneratedOn = Convert.ToDateTime(reader["GeneratedOn"])
                    });

                stopwatch.Stop();
                _logger.Info($"LoadMyComplianceCertificate completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadMyComplianceCertificate: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }

        public List<MastersName> PopulateMasterNames(string dtaType)
        {
            return PopulateDropdownValues(dtaType, sdr => new MastersName
            {
                MasterID = Convert.ToInt32(sdr["ID"]),
                MasterName = sdr["Code"].ToString(),
            });
        }
        //
        public List<ComplianceDocuments> LoadComplianceDocumentsForReport(int deptId, int typeId, int nature, int priority)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceDocuments>();

            try
            {
                _logger.Info($"Starting LoadComplianceDocumentsForReport for Dept: {deptId}, Type: {typeId}, Nature: {nature}, Priority: {priority}");

                var parameters = new Dictionary<string, object>
        {
            { "@DepartmentID", deptId },
            { "@ComplianceTypeID", typeId },
            { "@ComplianceNatureID", nature },
            { "@PriorityID", priority }
        };

                data = GetDataFromStoredProcedure<ComplianceDocuments>(
                    "Compliance.GetComplianceDocumentsForReport",
                    parameters,
                    reader => new ComplianceDocuments
                    {
                        DocumentID = Convert.ToInt32(reader["DocumentID"]),
                        ComplianceID = Convert.ToInt32(reader["ComplianceID"]),
                        ComplianceRef = reader["ComplianceRef"].ToString(),
                        ComplianceType = reader["ComplianceTypeName"].ToString(),
                        ComplianceNature = reader["ComplianceNatureName"].ToString(),
                        Priority = reader["Priority"].ToString(),
                        Department = reader["DepartmentName"].ToString(),
                        FileName = reader["FileName"].ToString(),
                        UploadedDate = Convert.ToDateTime(reader["UploadDate"]),
                        DocTypeName = reader["DocTypeName"].ToString()
                    });

                stopwatch.Stop();
                _logger.Info($"LoadComplianceDocumentsForReport completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadComplianceDocumentsForReport: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<ComplianceDocuments> LoadComplianceDocuments(int complianceId, string directoryPath)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceDocuments>();

            try
            {
                _logger.Info($"Starting LoadComplianceDocuments for ComplianceId: {complianceId}");

                var parameters = new Dictionary<string, object> { { "@ComplianceID", complianceId } };

                data = GetDataFromStoredProcedure<ComplianceDocuments>(
                    "Compliance.GetComplianceDocuments",
                    parameters,
                    reader =>
                    {
                        string fileName = reader["FileName"].ToString();
                        string filePath = Path.Combine(directoryPath, fileName);

                        return new ComplianceDocuments
                        {
                            DocumentID = Convert.ToInt32(reader["DocumentID"]),
                            ComplianceID = Convert.ToInt32(reader["ComplianceID"]),
                            FileName = fileName,
                            FilePath = filePath,
                            UploadedDate = Convert.ToDateTime(reader["UploadDate"]),
                            DocTypeID = Convert.ToInt32(reader["DocTypeId"]),
                            DocTypeName = reader["DocTypeName"].ToString(),
                            DownloadPath = $"~/FilesRep/ComplianceDocuments/{complianceId}/{Path.GetFileName(fileName)}"
                        };
                    });

                stopwatch.Stop();
                _logger.Info($"LoadComplianceDocuments completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadComplianceDocuments: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<ComplianceMasterDetailCombined> LoadMyCompliance(int userID, int statusID, int month, string year, char isAdmin)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceMasterDetailCombined>();

            try
            {
                _logger.Info($"Starting LoadMyCompliance for userID: {userID}, statusID: {statusID}, month: {month}, year: {year}, isAdmin: {isAdmin}");

                var parameters = new Dictionary<string, object>
        {
            { "@UserId", userID },
            { "@StatusID", statusID },
            { "@month", month },
            { "@year", year },
            { "@IsAdmin", isAdmin }
        };

                data = GetDataFromStoredProcedure<ComplianceMasterDetailCombined>(
                    "Compliance.usp_GetMyCompliances",
                    parameters,
                    reader => new ComplianceMasterDetailCombined
                    {
                        ComplianceID = Convert.ToInt32(reader["ComplianceID"]),
                        ComplianceRef = reader["ComplianceRef"].ToString(),
                        ComplianceDetailID = Convert.ToInt32(reader["ComplianceDetailID"]),
                        ClauseRef = reader["ClauseRef"].ToString(),
                        ComplianceTypeID = Convert.ToInt32(reader["ComplianceTypeID"]),
                        ComplianceTypeName = reader["ComplianceTypeName"].ToString(),
                        ActSectionReference = reader["ActSectionReference"].ToString(),
                        ComplianceNatureID = Convert.ToInt32(reader["NatureOfComplianceID"]),
                        NatureOfComplianceName = reader["NatureOfComplianceName"].ToString(),
                        FrequencyID = Convert.ToInt32(reader["FrequencyID"]),
                        FrequencyName = reader["FrequencyName"].ToString(),
                        DeptId = Convert.ToInt32(reader["DepartmentFunctionID"]),
                        DepartmentName = reader["DepartmentFunctionName"].ToString(),
                        EffectiveFrom = Convert.ToDateTime(reader["EffectiveFrom"]),
                        StandardDueDate = Convert.ToDateTime(reader["StandardDueDate"]),
                        DetailsOfComplianceRequirements = reader["DetailsOfComplianceRequirements"].ToString(),
                        ComplianceStatusId = Convert.ToInt32(reader["ComplianceStatusId"]),
                        ComplianceStatusName = reader["ComplianceStatusName"].ToString(),
                        PriorityName = reader["PriorityName"].ToString(),
                        ApprovedByName = reader["ApprovedByName"].ToString(),
                        AssignedDate = Convert.ToDateTime(reader["AssignedDate"])
                    });

                stopwatch.Stop();
                _logger.Info($"LoadMyCompliance completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadMyCompliance: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public ComplianceMasterr LoadComplianceModalDetails(int complianceId)
        {
            var stopwatch = Stopwatch.StartNew();
            var obj = new ComplianceMasterr();

            try
            {
                _logger.Info($"Starting LoadComplianceModalDetails for complianceId: {complianceId}");

                var parameters = new Dictionary<string, object> { { "@complianceId", complianceId } };

                var reader = _db.ExecuteReader("Compliance.LoadComplianceModalDetails", parameters);

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        obj.NatureOfComplianceName = reader["ComplianceArea"].ToString();
                        obj.ComplianceRef = reader["ComplianceRef"].ToString();
                        obj.NonCompliancePenalty = reader["NonCompliancePenalty"].ToString();
                        obj.ActionsToBeTaken = reader["ActionToBeTaken"].ToString();
                        obj.DetailsOfComplianceRequirements = reader["DetailsOfComplianceRequirements"].ToString();
                        obj.ActSectionReference = reader["ActSection"].ToString();
                        obj.Territory = reader["TerritoryName"].ToString();
                        obj.Priority = reader["PriorityName"].ToString();
                        obj.DrivenByName = reader["DrivenByName"].ToString();
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"LoadComplianceModalDetails completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadComplianceModalDetails: {ex.Message}\n{ex.StackTrace}");
            }

            return obj;
        }
        public List<ComplianceMasterDetailCombined> LoadMyTeamCompliance(int userID, int superviseeId)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceMasterDetailCombined>();

            try
            {
                _logger.Info($"Starting LoadMyTeamCompliance for userID: {userID}, superviseeId: {superviseeId}");

                var parameters = new Dictionary<string, object>
        {
            { "@UserId", userID },
            { "@superviseeId", superviseeId }
        };

                data = GetDataFromStoredProcedure<ComplianceMasterDetailCombined>(
                    "Compliance.usp_GetTeamComplianceMasterByUserID",
                    parameters,
                    reader => new ComplianceMasterDetailCombined
                    {
                        ComplianceID = Convert.ToInt32(reader["ComplianceID"]),
                        ComplianceDetailID = Convert.ToInt32(reader["ComplianceDetailID"]),
                        ComplianceArea = reader["ComplianceArea"].ToString(),
                        ComplianceTypeID = Convert.ToInt32(reader["ComplianceTypeID"]),
                        ComplianceTypeName = reader["ComplianceTypeName"].ToString(),
                        ActSectionReference = reader["ActSectionReference"].ToString(),
                        ComplianceNatureID = Convert.ToInt32(reader["NatureOfComplianceID"]),
                        NatureOfComplianceName = reader["NatureOfComplianceName"].ToString(),
                        FrequencyID = Convert.ToInt32(reader["FrequencyID"]),
                        FrequencyName = reader["FrequencyName"].ToString(),
                        DeptId = Convert.ToInt32(reader["DepartmentFunctionID"]),
                        DepartmentName = reader["DepartmentFunctionName"].ToString(),
                        EffectiveFrom = Convert.ToDateTime(reader["EffectiveFrom"]),
                        StandardDueDate = Convert.ToDateTime(reader["StandardDueDate"]),
                        DetailsOfComplianceRequirements = reader["DetailsOfComplianceRequirements"].ToString(),
                        AssignedToID = Convert.ToInt32(reader["AssignedToID"]),
                        AssignedToName = reader["UserName"].ToString(),
                        ComplianceStatusId = Convert.ToInt32(reader["ComplianceStatusId"])
                    });

                stopwatch.Stop();
                _logger.Info($"LoadMyTeamCompliance completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadMyTeamCompliance: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }

        //CheckComplianceforPeriod
        public int CheckComplianceforPeriod(int userid, string period)
        {
            var stopwatch = Stopwatch.StartNew();
            int count = 0;

            try
            {
                _logger.Info($"Starting CheckComplianceforPeriod for userId: {userid}, period: {period}");

                var parameters = new Dictionary<string, object>
        {
            { "@GeneratedByUserId", userid },
            { "@period", period }
        };

                object result = _db.ExecuteScalar("Compliance.usp_CheckComplianceForPeriod", parameters);
                count = result != null ? Convert.ToInt32(result) : 0;

                stopwatch.Stop();
                _logger.Info($"CheckComplianceforPeriod completed in {stopwatch.ElapsedMilliseconds} ms. Result: {count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in CheckComplianceforPeriod: {ex.Message}\n{ex.StackTrace}");
            }

            return count;
        }
        public int CheckUsedPeriodforUser(int userid, string period)
        {
            var stopwatch = Stopwatch.StartNew();
            int count = 0;

            try
            {
                _logger.Info($"Starting CheckUsedPeriodforUser for userId: {userid}, period: {period}");

                var parameters = new Dictionary<string, object>
        {
            { "@UserId", userid },
            { "@Period", period }
        };

                object result = _db.ExecuteScalar("Compliance.usp_CheckPeriod", parameters);
                count = result != null ? Convert.ToInt32(result) : 0;

                stopwatch.Stop();
                _logger.Info($"CheckUsedPeriodforUser completed in {stopwatch.ElapsedMilliseconds} ms. Result: {count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in CheckUsedPeriodforUser: {ex.Message}\n{ex.StackTrace}");
            }

            return count;
        }
        public List<string> GetUsedPeriodsFromDB()
        {
            var stopwatch = Stopwatch.StartNew();
            var periods = new List<string>();

            try
            {
                _logger.Info("Starting GetUsedPeriodsFromDB");

                var reader = _db.ExecuteReader("Compliance.usp_LoadPeriod", new Dictionary<string, object>());

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        periods.Add(reader["Period"].ToString());
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"GetUsedPeriodsFromDB completed in {stopwatch.ElapsedMilliseconds} ms. Count: {periods.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetUsedPeriodsFromDB: {ex.Message}\n{ex.StackTrace}");
            }

            return periods;
        }

        public List<string> GetRolling12Months()
        {
            var stopwatch = Stopwatch.StartNew();
            var periods = new List<string>();

            try
            {
                _logger.Info("Starting GetRolling12Months");

                var reader = _db.ExecuteReader("Compliance.usp_GetRolling12Months", new Dictionary<string, object>());

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        periods.Add(reader["Period"].ToString());
                    }
                    reader.Close();
                }

                stopwatch.Stop();
                _logger.Info($"GetRolling12Months completed in {stopwatch.ElapsedMilliseconds} ms. Count: {periods.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in GetRolling12Months: {ex.Message}\n{ex.StackTrace}");
            }

            return periods;
        }

        public List<User> LoadAssignToDropdown(int userid = 0)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<User>();

            try
            {
                _logger.Info($"Starting LoadAssignToDropdown for userId: {userid}");

                var parameters = new Dictionary<string, object> { { "@userid", userid } };

                data = GetDataFromStoredProcedure<User>(
                    "Compliance.usp_LoadAssignToDropdown",
                    parameters,
                    reader => new User
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        UserName = reader["UserName"].ToString()
                    });

                stopwatch.Stop();
                _logger.Info($"LoadAssignToDropdown completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadAssignToDropdown: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<ComplianceAreas> LoadComplianceDropdown(int DeptID = 0)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceAreas>();

            try
            {
                _logger.Info($"Starting LoadComplianceDropdown for DeptID: {DeptID}");

                var parameters = new Dictionary<string, object> { { "@DeptId", DeptID } };

                data = GetDataFromStoredProcedure<ComplianceAreas>(
                    "Compliance.usp_LoadComplianceDropdown",
                    parameters,
                    reader => new ComplianceAreas
                    {
                        ComplianceAreaID = Convert.ToInt32(reader["ComplianceAreaID"]),
                        ComplianceAreaName = reader["ComplianceAreaName"].ToString()
                    });

                stopwatch.Stop();
                _logger.Info($"LoadComplianceDropdown completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadComplianceDropdown: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public List<ComplianceMasterDetailCombined> LoadComplianceMaster(int DeptID, int complianceTypeId, string complianceArea)
        {
            var stopwatch = Stopwatch.StartNew();
            var data = new List<ComplianceMasterDetailCombined>();

            try
            {
                _logger.Info($"Starting LoadComplianceMaster for DeptID: {DeptID}, TypeId: {complianceTypeId}, Area: {complianceArea}");

                var parameters = new Dictionary<string, object>
        {
            { "@DeptId", DeptID },
            { "@ComplianceTypeId", complianceTypeId },
            { "@ComplianceArea", complianceArea }
        };

                data = GetDataFromStoredProcedure<ComplianceMasterDetailCombined>(
                    "Compliance.usp_GetComplianceMasterByDepartment",
                    parameters,
                    reader => new ComplianceMasterDetailCombined
                    {
                        ComplianceID = Convert.ToInt32(reader["ComplianceID"]),
                        ComplianceDetailID = Convert.ToInt32(reader["ComplianceDetailID"]),
                        ComplianceArea = reader["ComplianceArea"].ToString(),
                        DeptId = Convert.ToInt32(reader["DepartmentFunctionID"]),
                        DepartmentName = reader["DepartmentFunctionName"].ToString(),
                        ComplianceTypeName = reader["ComplianceTypeName"].ToString(),
                        EffectiveFrom = Convert.ToDateTime(reader["EffectiveFrom"]),
                        StandardDueDate = Convert.ToDateTime(reader["StandardDueDate"]),
                        AssignedToID = Convert.ToInt32(reader["AssignedToID"]),
                        AssignedToName = reader["AssignedToName"].ToString(),
                        DetailsOfComplianceRequirements = reader["DetailsOfComplianceRequirements"].ToString(),
                        ComplianceStatusName = reader["ComplianceStatusName"].ToString(),
                        AssignedDate = Convert.ToDateTime(reader["AssignedDate"]),
                        PreparerId = Convert.ToInt32(reader["PreparerId"]),
                        PreparerName = reader["PreparerName"].ToString()
                    });

                stopwatch.Stop();
                _logger.Info($"LoadComplianceMaster completed in {stopwatch.ElapsedMilliseconds} ms. Count: {data.Count}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in LoadComplianceMaster: {ex.Message}\n{ex.StackTrace}");
            }

            return data;
        }
        public bool CreateInvalidComplianceMaster(ComplianceMasterr p)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting CreateInvalidComplianceMaster for ref: {p.ComplianceRef}");

                var parameters = new Dictionary<string, object>
        {
            { "@valid", "invalid" },
            { "@BusinessUnitName", p.BusinessUnitName },
            { "@ComplianceRef", p.ComplianceRef },
            { "@NatureOfComplianceName", p.NatureOfComplianceName },
            { "@ComplianceTypeName", p.ComplianceTypeName },
            { "@DrivenByName", p.DrivenByName },
            { "@ActSectionReference", p.ActSectionReference },
            { "@ClauseRef", p.ClauseRef },
            { "@LawName", p.LawName },
            { "@Territory", p.Territory },
            { "@DetailsOfComplianceRequirements", p.DetailsOfComplianceRequirements },
            { "@NonCompliancePenalty", p.NonCompliancePenalty },
            { "@Priority", p.Priority },
            { "@FrequencyName", p.FrequencyName },
            { "@EffectiveFrom", p.EffectiveFrom },
            { "@StandardDueDate", p.StandardDueDate },
            { "@FirstDueDate", p.FirstDueDate },
            { "@DueOnEvery", p.DueOnEvery },
            { "@FormsIfAny", p.FormsIfAny },
            { "@ActionsToBeTaken", p.ActionsToBeTaken },
            { "@DepartmentName", p.DepartmentName },
            { "@InitiatorName", p.InitiatorName },
            { "@ApproverName", p.ApproverName }
        };

                _db.ExecuteNonQuery("Compliance.USP_UploadComplianceMaster", parameters);

                stopwatch.Stop();
                _logger.Info($"CreateInvalidComplianceMaster completed in {stopwatch.ElapsedMilliseconds} ms.");
                return true;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in CreateInvalidComplianceMaster: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        public bool UploadComplianceMaster(ComplianceMasterr p)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting UploadComplianceMaster for ComplianceRef: {p.ComplianceRef}");

                var parameters = new Dictionary<string, object>
        {
            { "@valid", "valid" },
            { "@BusinessUnitName", p.BusinessUnitName },
            { "@ComplianceRef", p.ComplianceRef },
            { "@NatureOfComplianceName", p.NatureOfComplianceName },
            { "@ComplianceTypeName", p.ComplianceTypeName },
            { "@DrivenByName", p.DrivenByName },
            { "@ActSectionReference", p.ActSectionReference },
            { "@ClauseRef", p.ClauseRef },
            { "@LawName", p.LawName },
            { "@Territory", p.Territory },
            { "@DetailsOfComplianceRequirements", p.DetailsOfComplianceRequirements },
            { "@NonCompliancePenalty", p.NonCompliancePenalty },
            { "@Priority", p.Priority },
            { "@FrequencyName", p.FrequencyName },
            { "@EffectiveFrom", p.EffectiveFrom },
            { "@StandardDueDate", p.StandardDueDate },
            { "@FirstDueDate", p.FirstDueDate },
            { "@DueOnEvery", p.DueOnEvery },
            { "@FormsIfAny", p.FormsIfAny },
            { "@ActionsToBeTaken", p.ActionsToBeTaken },
            { "@DepartmentName", p.DepartmentName },
            { "@InitiatorName", p.InitiatorName },
            { "@ApproverName", p.ApproverName }
        };

                _db.ExecuteNonQuery("Compliance.USP_UploadComplianceMaster", parameters);

                stopwatch.Stop();
                _logger.Info($"UploadComplianceMaster completed in {stopwatch.ElapsedMilliseconds} ms.");
                return true;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in UploadComplianceMaster: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
        public void InsertInvalidRecords(InvalidRecordss p)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Starting InsertInvalidRecords for Row: {p.RowNumber}");

                var parameters = new Dictionary<string, object>
        {
            { "@row", p.RowNumber },
            { "@message", p.ErrorMessage },
            { "@InvalidColumn", p.InvalidColumn }
        };

                _db.ExecuteNonQuery("Compliance.USP_UploadInvalidComplianceMaster", parameters);

                stopwatch.Stop();
                _logger.Info($"InsertInvalidRecords completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"Exception in InsertInvalidRecords: {ex.Message}\n{ex.StackTrace}");
            }
        }
        // Generic method for executing stored procedures
        public bool ExecuteStoredProcedure(string storedProcedureName)
        {
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(storedProcedureName, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }
        // Generic method for executing stored procedures with parameters
        private bool ExecuteStoredProcedureWithParams(string storedProcedureName, Dictionary<string, object> parameters)
        {
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(storedProcedureName, con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    // Add parameters to the command
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value); // Handle null values
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }
        public bool EditDeleteLawType(Int32 lawId, string commandType, int UserID, string lawName = "", bool isActive = false, string description = "")
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@LawId", lawId },
                                { "@commandType", commandType },
                                { "@LawTypeName", lawName },
                                { "@isActive", isActive },
                                { "@UserID", UserID },
                                { "@Description", description }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_Delete_LawType", parameters);
        }
        public bool EditDeleteDrivenBy(Int32 drivenId, string commandType, int UserID, string drivenName = "", bool isActive = false, string description = "")
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@DrivenId", drivenId },
                                { "@commandType", commandType },
                                { "@drivenName", drivenName },
                                { "@isActive", isActive },
                                { "@UserID", UserID },
                                { "@Description", description }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_Delete_DrivenBy", parameters);
        }
        public bool EditDeleteDepartment(Int32 deptId, string commandType, int UserID, string deptName = "", bool isActive = false, string description = "")
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@DeptId", deptId },
                                { "@commandType", commandType },
                                { "@deptName", deptName },
                                { "@isActive", isActive },
                                { "@UserID", UserID },
                                { "@Description", description }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_Delete_Department", parameters);
        }
        public bool EditDeleteComplianceStatus(Int32 ComplianceStatusId, string commandType, int UserID, string ComplianceStatusName = "", bool isActive = false)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@ComplianceStatusId", ComplianceStatusId },
                                { "@commandType", commandType },
                                { "@ComplianceStatusName", ComplianceStatusName },
                                { "@isActive", isActive },
                                { "@UserID", UserID }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_DeleteComplianceStatus", parameters);
        }
        public bool EditDeleteFrequency(Int32 FreqId, string commandType, int UserID, string FreqName = "", bool isActive = false, string description = "")
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@FreqId", FreqId },
                                { "@commandType", commandType },
                                { "@FreqName", FreqName },
                                { "@isActive", isActive },
                                { "@UserID", UserID },
                                { "@Description", description }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_DeleteFrequency", parameters);
        }
        public bool EditDeleteNatureCompliance(Int32 ComplianceNatureID, string commandType, int UserID, string NatureOfCompliance = "", bool isActive = false, string description = "")
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@ComplianceNatureID", ComplianceNatureID },
                                { "@commandType", commandType },
                                { "@NatureOfCompliance", NatureOfCompliance },
                                { "@isActive", isActive },
                                { "@UserID", UserID },
                                { "@Description", description }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_DeleteNatureCompliance", parameters);
        }
        public bool DeleteComplianceAssignedToUser(int compDetailID) //TODO : make it generic "ExecuteStoredProcedure"
        {
            SqlConnection con = GetConnection();
            try
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand(
                            @"delete from compliance.ComplianceDetails where ComplianceDetailID=@compDetailID", con);
                cmd1.Parameters.AddWithValue("@compDetailID", compDetailID);
                cmd1.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }
        public bool DeleteComplianceMasteTemp()  //TODO : make it generic "ExecuteStoredProcedure"
        {
            SqlConnection con = GetConnection();
            try
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand(@"delete from compliance.ComplianceMaster_Temp; delete from compliance.InvalidCol;", con);
                cmd1.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : {ex.Message}");
                return false;

            }
            finally
            {
                con.Close();
            }
        }
        public bool DeleteUser(string code)//TODO : make it generic "ExecuteStoredProcedure"
        {
            //// Prepare parameters as a dictionary
            //var parameters = new Dictionary<string, object>
            //                {
            //                    { "@LawId", LawId },
            //                    { "@commandType", "delete" },
            //                    { "@isActive", false },
            //                    { "@LawTypeName", "" },
            //                    { "@UserID", userID },
            //                    { "@Description", "" }
            //                };

            //// Call the generic method to execute the stored procedure
            //return ExecuteStoredProcedureWithParams("compliance.usp_Delete_Department", parameters);
            SqlConnection con = GetConnection();
            try
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand(
                            @"delete from compliance.x_user_master where user_name=@Code", con);
                cmd1.Parameters.AddWithValue("@Code", code);
                cmd1.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : {ex.Message}");
                return false;
            }
            finally
            {
                con.Close();
            }
        }
        public bool EditDeleteComplianceType(Int32 ComplianceTypeID, string commandType, int UserID, string ComplianceTypeName = "", bool isActive = false, string description = "")
        {
            // Prepare parameters as a dictionary
            var parameters = new Dictionary<string, object>
                            {
                                { "@ComplianceTypeID", ComplianceTypeID },
                                { "@commandType", commandType },
                                { "@ComplianceTypeName", ComplianceTypeName },
                                { "@isActive", isActive },
                                { "@UserID", UserID },
                                { "@Description", description }
                            };

            // Call the generic method to execute the stored procedure
            return ExecuteStoredProcedureWithParams("compliance.usp_DeleteComplianceType", parameters);
        }
        public bool UpdateRights(UserRights p, int UserID)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@objectID", p.ObjectId },
                                { "@UserID", UserID },
                                { "@rights", p.Rights }
                            };
            return ExecuteStoredProcedureWithParams("compliance.USP_CreateUser_InsertDetails", parameters);
        }
        public bool DeleteDepartment(int LawId, int userID)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@LawId", LawId },
                                { "@commandType", "delete" },
                                { "@isActive", false },
                                { "@LawTypeName", "" },
                                { "@UserID", userID },
                                { "@Description", "" }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_Delete_Department", parameters);
        }
        public bool DeleteLawType(int lawId, int userID)//TODO : make it generic "ExecuteStoredProcedure"
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@LawId", lawId },
                                { "@commandType", "delete" },
                                { "@isActive", false },
                                { "@LawTypeName", "" },
                                { "@UserID", userID },
                                { "@Description", "" }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_Delete_LawType", parameters);
        }
        public bool DeleteDrivenBy(int drivenId, int userID)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@DrivenId", drivenId },
                                { "@commandType", "delete" },
                                { "@isActive", false },
                                { "@drivenName", "" },
                                { "@UserID", userID },
                                { "@Description", "" }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_Delete_DrivenBy", parameters);
        }
        public bool DeleteComplianceStatus(int ComplianceStatusId, int userID)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@ComplianceStatusId", ComplianceStatusId },
                                { "@commandType", "delete" },
                                { "@isActive", false },
                                { "@ComplianceStatusName", "" },
                                { "@UserID", userID }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_DeleteComplianceStatus", parameters);
        }
        public bool DeleteFrequency(int FreqId, int userID)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@FreqId", FreqId },
                                { "@commandType", "delete" },
                                { "@isActive", false },
                                { "@FreqName", "" },
                                { "@UserID", userID },
                                { "@OccursEvery", 0 }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_DeleteFrequency", parameters);
        }
        public bool DeleteNatureCompliance(int ComplianceNatureID, int userID)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@ComplianceNatureID", ComplianceNatureID },
                                { "@commandType", "delete" },
                                { "@isActive", false },
                                { "@NatureOfCompliance", "" },
                                { "@UserID", userID },
                                { "@Description", "" }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_DeleteNatureCompliance", parameters);
        }
        public bool DeleteComplianceType(int ComplianceTypeID, int userID)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@ComplianceTypeID", ComplianceTypeID },
                                { "@commandType", "delete" },
                                { "@isActive", false },
                                { "@ComplianceTypeName", "" },
                                { "@UserID", userID },
                                { "@Description", "" }
                            };
            return ExecuteStoredProcedureWithParams("compliance.usp_DeleteComplianceType", parameters);
        }
        public bool UpdateComplianceAssignmentDetails(ComplianceMasterDetails comp)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@compDetailId", comp.ComplianceDetailID },
                                { "@creatorId", comp.CreatedById },
                                { "@assignedToId", comp.AssignedToID }
                            };
            return ExecuteStoredProcedureWithParams("compliance.SaveComplianceAssignmentDetails", parameters);
        }
        public bool SaveComplianceDetailFilePath(string directoryName, int updatedBy, int complianceDetailId)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@directoryName", directoryName },
                                { "@cocomplianceDetailIdmplianceId", complianceDetailId },
                                { "@updatedBy", updatedBy }
                            };
            return ExecuteStoredProcedureWithParams("compliance.SaveComplianceDetailFilePath", parameters);
        }
        
        public bool SaveDocumentToDatabase(int complianceId, string fileName, string filePath, int docTypeId)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@ComplianceID", complianceId },
                                { "@FileName", fileName },
                                { "@FilePath", filePath },
                                { "@DocTypeId", docTypeId }
                            };
            return ExecuteStoredProcedureWithParams("compliance.SaveComplianceMasterDocument", parameters);
        }

        public bool SendForApproval(int complianceId, int complianceDetailId, string userComments) 
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@complianceId", complianceId },
                                { "@complianceDetailId", complianceDetailId },
                                { "@userComments", userComments }
                            };
            return ExecuteStoredProcedureWithParams("compliance.SendForApproval", parameters);
        }
        public bool UpdateApproverComments(int complianceId, int complianceDetailId, string userComments, int userId, string ApproveOrReject)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@complianceId", complianceId },
                                { "@complianceDetailId", complianceDetailId },
                                { "@userComments", userComments },
                                { "@updatedBy", userId },
                                {"@ApproveOrReject", ApproveOrReject }
                            };
            return ExecuteStoredProcedureWithParams("compliance.SaveApproverComments", parameters);
        }
        public bool UpdateUserComments(int complianceId, int complianceDetailId, string userComments, int userId)
        {
            var parameters = new Dictionary<string, object>
                            {
                                { "@complianceId", complianceId },
                                { "@complianceDetailId", complianceDetailId },
                                { "@userComments", userComments },
                                { "@updatedBy", userId }
                            };
            return ExecuteStoredProcedureWithParams("compliance.SaveUserComments", parameters);
        }
        public bool UpdateComplianceDetails()
        {
            return ExecuteStoredProcedure("compliance.UpdateComplianceDetails");
        }
        public bool UpdateComplianceMaster()
        {
            return ExecuteStoredProcedure("compliance.UpdateComplianceMaster");
        }

        public List<ComplianceMasterr> LoadNonCompliancesForReport(int deptId = 0, int typeId = 0, int priorityId = 0)
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Compliance.usp_GetNonCompliance", con);
            cmd.Parameters.AddWithValue("@deptId", deptId);
            cmd.Parameters.AddWithValue("@typeId", typeId);
            cmd.Parameters.AddWithValue("@priorityId", priorityId);
            cmd.CommandType = CommandType.StoredProcedure;
            List<ComplianceMasterr> lstSKUWithPO = new List<ComplianceMasterr>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ComplianceMasterr objSKU = new ComplianceMasterr();
                    objSKU.BusinessUnitID = (int)reader["BusinessUnitId"];
                    objSKU.BusinessUnitName = (string)reader["BusinessUnitName"];
                    objSKU.ComplianceID = (int)reader["ComplianceID"];
                    objSKU.ComplianceRef = (string)reader["ComplianceRef"];
                    objSKU.ComplianceTypeID = (int)reader["ComplianceTypeID"];
                    objSKU.ComplianceTypeName = (string)reader["ComplianceTypeName"];
                    objSKU.ActSectionReference = (string)reader["ActSectionReference"];
                    objSKU.ComplianceNatureID = (int)reader["NatureOfComplianceID"];
                    objSKU.NatureOfComplianceName = (string)reader["NatureOfComplianceName"];
                    objSKU.LawId = (int)reader["LawId"];
                    objSKU.LawName = (string)reader["LawName"];
                    objSKU.TerritoryId = (int)reader["TerritoryId"];
                    objSKU.Territory = (string)reader["Territory"];
                    objSKU.PriorityId = (int)reader["PriorityId"];
                    objSKU.Priority = (string)reader["Priority"];
                    objSKU.FrequencyID = (int)reader["FrequencyID"];
                    objSKU.FrequencyName = (string)reader["FrequencyName"];
                    objSKU.EffectiveFrom = (DateTime)reader["EffectiveFrom"];
                    objSKU.StandardDueDate = (DateTime)reader["StandardDueDate"];
                    objSKU.DeptId = (int)reader["DepartmentFunctionID"];
                    objSKU.DepartmentName = (string)reader["DepartmentFunctionName"];
                    objSKU.InitiatorId = (int)reader["PreparerId"];
                    objSKU.InitiatorName = (string)reader["PreparerName"];
                    objSKU.ApproverId = (int)reader["ApproverId"];
                    objSKU.ApproverName = (string)reader["ApproverName"];
                    //objSKU.FormsIfAny = (string)reader["FormsIfAny"];
                    objSKU.DueOnEvery = (int)reader["DueOnEvery"];
                    objSKU.FirstDueDate = (DateTime)reader["FirstDueDate"];
                    objSKU.DetailsOfComplianceRequirements = (string)reader["DetailsOfComplianceRequirements"];
                    objSKU.NonCompliancePenalty = (string)reader["Penalty"];
                    objSKU.ActionsToBeTaken = (string)reader["ActionToBeTaken"];
                    objSKU.DrivenById = (int)reader["DrivenById"];
                    objSKU.DrivenByName = (string)reader["DrivenBy"];
                    lstSKUWithPO.Add(objSKU);
                }
                reader.Close();
            }
            catch (SqlException err)
            {
                throw new ApplicationException("Data error.");
            }
            finally
            {
                con.Close();
            }
            return lstSKUWithPO;
        }

        public List<ComplianceMasterr> LoadUnApprovedCompliancesForReport(int deptId = 0, int typeId = 0, int approverId = 0)
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Compliance.usp_GetUnApprovedCompliances", con);
            cmd.Parameters.AddWithValue("@deptId", deptId);
            cmd.Parameters.AddWithValue("@typeId", typeId);
            cmd.Parameters.AddWithValue("@approverId", approverId);
            cmd.CommandType = CommandType.StoredProcedure;
            List<ComplianceMasterr> lstSKUWithPO = new List<ComplianceMasterr>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ComplianceMasterr objSKU = new ComplianceMasterr();
                    objSKU.BusinessUnitID = (int)reader["BusinessUnitId"];
                    objSKU.BusinessUnitName = (string)reader["BusinessUnitName"];
                    objSKU.ComplianceID = (int)reader["ComplianceID"];
                    objSKU.ComplianceRef = (string)reader["ComplianceRef"];
                    objSKU.ComplianceTypeID = (int)reader["ComplianceTypeID"];
                    objSKU.ComplianceTypeName = (string)reader["ComplianceTypeName"];
                    objSKU.ActSectionReference = (string)reader["ActSectionReference"];
                    objSKU.ComplianceNatureID = (int)reader["NatureOfComplianceID"];
                    objSKU.NatureOfComplianceName = (string)reader["NatureOfComplianceName"];
                    objSKU.LawId = (int)reader["LawId"];
                    objSKU.LawName = (string)reader["LawName"];
                    objSKU.TerritoryId = (int)reader["TerritoryId"];
                    objSKU.Territory = (string)reader["Territory"];
                    objSKU.PriorityId = (int)reader["PriorityId"];
                    objSKU.Priority = (string)reader["Priority"];
                    objSKU.FrequencyID = (int)reader["FrequencyID"];
                    objSKU.FrequencyName = (string)reader["FrequencyName"];
                    objSKU.EffectiveFrom = (DateTime)reader["EffectiveFrom"];
                    objSKU.StandardDueDate = (DateTime)reader["StandardDueDate"];
                    objSKU.DeptId = (int)reader["DepartmentFunctionID"];
                    objSKU.DepartmentName = (string)reader["DepartmentFunctionName"];
                    objSKU.InitiatorId = (int)reader["PreparerId"];
                    objSKU.InitiatorName = (string)reader["PreparerName"];
                    objSKU.ApproverId = (int)reader["ApproverId"];
                    objSKU.ApproverName = (string)reader["ApproverName"];
                    //objSKU.FormsIfAny = (string)reader["FormsIfAny"];
                    objSKU.DueOnEvery = (int)reader["DueOnEvery"];
                    objSKU.FirstDueDate = (DateTime)reader["FirstDueDate"];
                    objSKU.DetailsOfComplianceRequirements = (string)reader["DetailsOfComplianceRequirements"];
                    objSKU.NonCompliancePenalty = (string)reader["Penalty"];
                    objSKU.ActionsToBeTaken = (string)reader["ActionToBeTaken"];
                    objSKU.DrivenById = (int)reader["DrivenById"];
                    objSKU.DrivenByName = (string)reader["DrivenBy"];
                    lstSKUWithPO.Add(objSKU);
                }
                reader.Close();
            }
            catch (SqlException err)
            {
                throw new ApplicationException("Data error.");
            }
            finally
            {
                con.Close();
            }
            return lstSKUWithPO;
        }

        public List<ComplianceMasterr> LoadMasterReport(int masterId = 0)
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Compliance.usp_GetMastersDataForReport", con);
            cmd.Parameters.AddWithValue("@masterId", masterId);
            cmd.CommandType = CommandType.StoredProcedure;
            List<ComplianceMasterr> lstSKUWithPO = new List<ComplianceMasterr>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ComplianceMasterr objSKU = new ComplianceMasterr();
                    objSKU.BusinessUnitID = (int)reader["BusinessUnitId"];
                    objSKU.BusinessUnitName = (string)reader["BusinessUnitName"];
                    objSKU.ComplianceID = (int)reader["ComplianceID"];
                    objSKU.ComplianceRef = (string)reader["ComplianceRef"];
                    objSKU.ComplianceTypeID = (int)reader["ComplianceTypeID"];
                    objSKU.ComplianceTypeName = (string)reader["ComplianceTypeName"];
                    objSKU.ActSectionReference = (string)reader["ActSectionReference"];
                    objSKU.ComplianceNatureID = (int)reader["NatureOfComplianceID"];
                    objSKU.NatureOfComplianceName = (string)reader["NatureOfComplianceName"];
                    objSKU.LawId = (int)reader["LawId"];
                    objSKU.LawName = (string)reader["LawName"];
                    objSKU.TerritoryId = (int)reader["TerritoryId"];
                    objSKU.Territory = (string)reader["Territory"];
                    objSKU.PriorityId = (int)reader["PriorityId"];
                    objSKU.Priority = (string)reader["Priority"];
                    objSKU.FrequencyID = (int)reader["FrequencyID"];
                    objSKU.FrequencyName = (string)reader["FrequencyName"];
                    objSKU.EffectiveFrom = (DateTime)reader["EffectiveFrom"];
                    objSKU.StandardDueDate = (DateTime)reader["StandardDueDate"];
                    objSKU.DeptId = (int)reader["DepartmentFunctionID"];
                    objSKU.DepartmentName = (string)reader["DepartmentFunctionName"];
                    objSKU.InitiatorId = (int)reader["PreparerId"];
                    objSKU.InitiatorName = (string)reader["PreparerName"];
                    objSKU.ApproverId = (int)reader["ApproverId"];
                    objSKU.ApproverName = (string)reader["ApproverName"];
                    //objSKU.FormsIfAny = (string)reader["FormsIfAny"];
                    objSKU.DueOnEvery = (int)reader["DueOnEvery"];
                    objSKU.FirstDueDate = (DateTime)reader["FirstDueDate"];
                    objSKU.DetailsOfComplianceRequirements = (string)reader["DetailsOfComplianceRequirements"];
                    objSKU.NonCompliancePenalty = (string)reader["Penalty"];
                    objSKU.ActionsToBeTaken = (string)reader["ActionToBeTaken"];
                    objSKU.DrivenById = (int)reader["DrivenById"];
                    objSKU.DrivenByName = (string)reader["DrivenBy"];
                    lstSKUWithPO.Add(objSKU);
                }
                reader.Close();
            }
            catch (SqlException err)
            {
                throw new ApplicationException("Data error.");
            }
            finally
            {
                con.Close();
            }
            return lstSKUWithPO;
        }
        public List<ComplianceMasterr> LoadUnAssignedComplianceMasterForReport(int deptId = 0, int typeId = 0, int natureId = 0, int priorityId = 0)
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Compliance.usp_GetUnAssignedComplianceForReport", con);
            cmd.Parameters.AddWithValue("@deptId", deptId);
            cmd.Parameters.AddWithValue("@typeId", typeId);
            cmd.Parameters.AddWithValue("@natureId", natureId);
            cmd.Parameters.AddWithValue("@priorityId", priorityId);
            cmd.CommandType = CommandType.StoredProcedure;
            List<ComplianceMasterr> lstSKUWithPO = new List<ComplianceMasterr>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ComplianceMasterr objSKU = new ComplianceMasterr();
                    objSKU.BusinessUnitID = (int)reader["BusinessUnitId"];
                    objSKU.BusinessUnitName = (string)reader["BusinessUnitName"];
                    objSKU.ComplianceID = (int)reader["ComplianceID"];
                    objSKU.ComplianceRef = (string)reader["ComplianceRef"];
                    objSKU.ComplianceTypeID = (int)reader["ComplianceTypeID"];
                    objSKU.ComplianceTypeName = (string)reader["ComplianceTypeName"];
                    objSKU.ComplianceStatusId = (int)reader["ComplianceStatusId"];
                    objSKU.ComplianceStatusName = (string)reader["ComplianceStatusName"];
                    objSKU.ActSectionReference = (string)reader["ActSectionReference"];
                    objSKU.ComplianceNatureID = (int)reader["NatureOfComplianceID"];
                    objSKU.NatureOfComplianceName = (string)reader["NatureOfComplianceName"];
                    objSKU.LawId = (int)reader["LawId"];
                    objSKU.LawName = (string)reader["LawName"];
                    objSKU.TerritoryId = (int)reader["TerritoryId"];
                    objSKU.Territory = (string)reader["Territory"];
                    objSKU.PriorityId = (int)reader["PriorityId"];
                    objSKU.Priority = (string)reader["Priority"];
                    objSKU.FrequencyID = (int)reader["FrequencyID"];
                    objSKU.FrequencyName = (string)reader["FrequencyName"];
                    objSKU.EffectiveFrom = (DateTime)reader["EffectiveFrom"];
                    objSKU.StandardDueDate = (DateTime)reader["StandardDueDate"];
                    objSKU.DeptId = (int)reader["DepartmentFunctionID"];
                    objSKU.DepartmentName = (string)reader["DepartmentFunctionName"];
                    objSKU.InitiatorId = (int)reader["PreparerId"];
                    objSKU.InitiatorName = (string)reader["PreparerName"];
                    objSKU.ApproverId = (int)reader["ApproverId"];
                    objSKU.ApproverName = (string)reader["ApproverName"];
                    //objSKU.FormsIfAny = (string)reader["FormsIfAny"];
                    objSKU.DueOnEvery = (int)reader["DueOnEvery"];
                    objSKU.FirstDueDate = (DateTime)reader["FirstDueDate"];
                    objSKU.DetailsOfComplianceRequirements = (string)reader["DetailsOfComplianceRequirements"];
                    objSKU.NonCompliancePenalty = (string)reader["Penalty"];
                    objSKU.ActionsToBeTaken = (string)reader["ActionToBeTaken"];
                    objSKU.DrivenById = (int)reader["DrivenById"];
                    objSKU.DrivenByName = (string)reader["DrivenBy"];
                    lstSKUWithPO.Add(objSKU);
                }
                reader.Close();
            }
            catch (SqlException err)
            {
                throw new ApplicationException("Data error.");
            }
            finally
            {
                con.Close();
            }
            return lstSKUWithPO;
        }
        //public List<ComplianceMasterr> LoadComplianceDelayed(int deptId = 0, int typeId = 0, int natureId = 0, int priorityId = 0)

        

        public ComplianceDelayedResult LoadComplianceDelayed(int deptId = 0, int month = 0, int year = 0, int startyear=0)// string period="")
        {
            ComplianceDelayedResult result = new ComplianceDelayedResult
            {
                OverdueCompliances = new List<ComplianceMasterr>(),
                Summary = new ComplianceSummary()
            };
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Compliance.usp_GetComplianceDelayedForReport", con))
                {
                    cmd.Parameters.AddWithValue("@deptId", deptId);
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);
                   // cmd.Parameters.AddWithValue("@startyear", startyear);
                    //cmd.Parameters.AddWithValue("@period", period);
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Process first result set (Overdue Compliances)
                            while (reader.Read())
                            {
                                ComplianceMasterr objSKU = new ComplianceMasterr
                                {
                                    ComplianceID = (int)reader["ComplianceID"],
                                    ComplianceRef = reader["ComplianceRef"] != DBNull.Value ? (string)reader["ComplianceRef"] : string.Empty,
                                    DepartmentName = reader["DepartmentFunctionName"] != DBNull.Value ? (string)reader["DepartmentFunctionName"] : string.Empty,
                                    InitiatorName = reader["PreparerName"] != DBNull.Value ? (string)reader["PreparerName"] : string.Empty,
                                    DetailsOfComplianceRequirements = reader["DetailsOfComplianceRequirements"] != DBNull.Value ? (string)reader["DetailsOfComplianceRequirements"] : string.Empty,
                                    Remarks = reader["Remarks"] != DBNull.Value ? (string)reader["Remarks"] : string.Empty,
                                    ComplianceDueDate = (DateTime)reader["ComplianceDueDate"],
                                    PreparerRemarks = reader["PreparerRemarks"] != DBNull.Value ? (string)reader["PreparerRemarks"] : string.Empty, // Not in stored procedure; set to empty or map to another field if available
                                    ApproverRemarks = reader["ApproverRemarks"] != DBNull.Value ? (string)reader["ApproverRemarks"] : string.Empty   // Not in stored procedure; set to empty or map to another field if available
                                };
                                result.OverdueCompliances.Add(objSKU);
                            }
                            // Move to second result set (Summary)
                            if (reader.NextResult() && reader.Read())
                            {
                                result.Summary.TotalCompliances = (int)reader["TotalCompliances"];
                                result.Summary.PendingCompliances = (int)reader["PendingCompliances"];
                                result.Summary.OverdueCompliances = (int)reader["OverdueCompliances"];
                                result.Summary.SentForApproval = (int)reader["Complete"];
                            }
                        }

                    }
                    catch (SqlException err)
                    {
                        throw new ApplicationException("Data error: " + err.Message);
                    }
                    return result;
                }
            }
        }
        public List<ComplianceMasterr> LoadComplianceDueWithin(int deptId = 0, int typeId = 0, int natureId = 0, int priorityId = 0, int dueWithin=0)
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Compliance.usp_GetComplianceDueWithin", con);
            cmd.Parameters.AddWithValue("@deptId", deptId);
            cmd.Parameters.AddWithValue("@typeId", typeId);
            cmd.Parameters.AddWithValue("@natureId", natureId);
            cmd.Parameters.AddWithValue("@priorityId", priorityId);
            cmd.Parameters.AddWithValue("@duein", dueWithin);
            cmd.CommandType = CommandType.StoredProcedure;
            List<ComplianceMasterr> lstSKUWithPO = new List<ComplianceMasterr>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ComplianceMasterr objSKU = new ComplianceMasterr();
                    objSKU.BusinessUnitID = (int)reader["BusinessUnitId"];
                    objSKU.BusinessUnitName = (string)reader["BusinessUnitName"];
                    objSKU.ComplianceID = (int)reader["ComplianceID"];
                    objSKU.ComplianceRef = (string)reader["ComplianceRef"];
                    objSKU.ComplianceTypeID = (int)reader["ComplianceTypeID"];
                    objSKU.ComplianceTypeName = (string)reader["ComplianceTypeName"];
                    objSKU.ActSectionReference = (string)reader["ActSectionReference"];
                    objSKU.ComplianceNatureID = (int)reader["NatureOfComplianceID"];
                    objSKU.NatureOfComplianceName = (string)reader["NatureOfComplianceName"];
                    objSKU.LawId = (int)reader["LawId"];
                    objSKU.LawName = (string)reader["LawName"];
                    objSKU.TerritoryId = (int)reader["TerritoryId"];
                    objSKU.Territory = (string)reader["Territory"];
                    objSKU.PriorityId = (int)reader["PriorityId"];
                    objSKU.Priority = (string)reader["Priority"];
                    objSKU.FrequencyID = (int)reader["FrequencyID"];
                    objSKU.FrequencyName = (string)reader["FrequencyName"];
                    objSKU.EffectiveFrom = (DateTime)reader["EffectiveFrom"];
                    objSKU.StandardDueDate = (DateTime)reader["StandardDueDate"];
                    objSKU.DeptId = (int)reader["DepartmentFunctionID"];
                    objSKU.DepartmentName = (string)reader["DepartmentFunctionName"];
                    objSKU.InitiatorId = (int)reader["PreparerId"];
                    objSKU.InitiatorName = (string)reader["PreparerName"];
                    objSKU.ApproverId = (int)reader["ApproverId"];
                    objSKU.ApproverName = (string)reader["ApproverName"];
                    //objSKU.FormsIfAny = (string)reader["FormsIfAny"];
                    objSKU.DueOnEvery = (int)reader["DueOnEvery"];
                    objSKU.FirstDueDate = (DateTime)reader["FirstDueDate"];
                    objSKU.DetailsOfComplianceRequirements = (string)reader["DetailsOfComplianceRequirements"];
                    objSKU.NonCompliancePenalty = (string)reader["Penalty"];
                    objSKU.ActionsToBeTaken = (string)reader["ActionToBeTaken"];
                    objSKU.DrivenById = (int)reader["DrivenById"];
                    objSKU.DrivenByName = (string)reader["DrivenBy"];
                    lstSKUWithPO.Add(objSKU);
                }
                reader.Close();
            }
            catch (SqlException err)
            {
                throw new ApplicationException("Data error.");
            }
            finally
            {
                con.Close();
            }
            return lstSKUWithPO;
        }
        public List<ComplianceMasterr> LoadCompliancePendingForReport(int userId = 0)
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Compliance.usp_GetUserPendingCompliance", con);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.CommandType = CommandType.StoredProcedure;
            List<ComplianceMasterr> lstSKUWithPO = new List<ComplianceMasterr>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ComplianceMasterr objSKU = new ComplianceMasterr();
                    objSKU.BusinessUnitID = (int)reader["BusinessUnitId"];
                    objSKU.BusinessUnitName = (string)reader["BusinessUnitName"];
                    objSKU.ComplianceID = (int)reader["ComplianceID"];
                    objSKU.ComplianceRef = (string)reader["ComplianceRef"];
                    objSKU.ComplianceTypeID = (int)reader["ComplianceTypeID"];
                    objSKU.ComplianceTypeName = (string)reader["ComplianceTypeName"];
                    objSKU.ComplianceStatusId = (int)reader["ComplianceStatusId"];
                    objSKU.ComplianceStatusName = (string)reader["ComplianceStatusName"];
                    objSKU.ActSectionReference = (string)reader["ActSectionReference"];
                    objSKU.ComplianceNatureID = (int)reader["NatureOfComplianceID"];
                    objSKU.NatureOfComplianceName = (string)reader["NatureOfComplianceName"];
                    objSKU.LawId = (int)reader["LawId"];
                    objSKU.LawName = (string)reader["LawName"];
                    objSKU.TerritoryId = (int)reader["TerritoryId"];
                    objSKU.Territory = (string)reader["Territory"];
                    objSKU.PriorityId = (int)reader["PriorityId"];
                    objSKU.Priority = (string)reader["Priority"];
                    objSKU.FrequencyID = (int)reader["FrequencyID"];
                    objSKU.FrequencyName = (string)reader["FrequencyName"];
                    objSKU.EffectiveFrom = (DateTime)reader["EffectiveFrom"];
                    objSKU.StandardDueDate = (DateTime)reader["StandardDueDate"];
                    objSKU.DeptId = (int)reader["DepartmentFunctionID"];
                    objSKU.DepartmentName = (string)reader["DepartmentFunctionName"];
                    objSKU.InitiatorId = (int)reader["PreparerId"];
                    objSKU.InitiatorName = (string)reader["PreparerName"];
                    objSKU.ApproverId = (int)reader["ApproverId"];
                    objSKU.ApproverName = (string)reader["ApproverName"];
                    //objSKU.FormsIfAny = (string)reader["FormsIfAny"];
                    objSKU.DueOnEvery = (int)reader["DueOnEvery"];
                    objSKU.FirstDueDate = (DateTime)reader["FirstDueDate"];
                    objSKU.DetailsOfComplianceRequirements = (string)reader["DetailsOfComplianceRequirements"];
                    objSKU.NonCompliancePenalty = (string)reader["Penalty"];
                    objSKU.ActionsToBeTaken = (string)reader["ActionToBeTaken"];
                    objSKU.DrivenById = (int)reader["DrivenById"];
                    objSKU.DrivenByName = (string)reader["DrivenBy"];
                    lstSKUWithPO.Add(objSKU);
                }
                reader.Close();
            }
            catch (SqlException err)
            {
                throw new ApplicationException("Data error.");
            }
            finally
            {
                con.Close();
            }
            return lstSKUWithPO;
        }
        public List<ComplianceMasterr> LoadComplianceMasterForReport(int deptId = 0, int typeId = 0, int natureId = 0, int priorityId = 0)
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Compliance.usp_GetComplianceRegister", con);
            cmd.Parameters.AddWithValue("@deptId", deptId);
            cmd.Parameters.AddWithValue("@typeId", typeId);
            cmd.Parameters.AddWithValue("@natureId", natureId);
            cmd.Parameters.AddWithValue("@priorityId", priorityId);
            cmd.CommandType = CommandType.StoredProcedure;
            List<ComplianceMasterr> lstSKUWithPO = new List<ComplianceMasterr>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ComplianceMasterr objSKU = new ComplianceMasterr();
                    objSKU.BusinessUnitID = (int)reader["BusinessUnitId"];
                    objSKU.BusinessUnitName = (string)reader["BusinessUnitName"];
                    objSKU.ComplianceID = (int)reader["ComplianceID"];
                    objSKU.ComplianceRef = (string)reader["ComplianceRef"];
                    objSKU.ComplianceTypeID = (int)reader["ComplianceTypeID"];
                    objSKU.ComplianceTypeName = (string)reader["ComplianceTypeName"];
                    objSKU.ActSectionReference = (string)reader["ActSectionReference"];
                    objSKU.ComplianceNatureID = (int)reader["NatureOfComplianceID"];
                    objSKU.NatureOfComplianceName = (string)reader["NatureOfComplianceName"];
                    objSKU.LawId = (int)reader["LawId"];
                    objSKU.LawName = (string)reader["LawName"];
                    objSKU.TerritoryId = (int)reader["TerritoryId"];
                    objSKU.Territory = (string)reader["Territory"];
                    objSKU.PriorityId = (int)reader["PriorityId"];
                    objSKU.Priority = (string)reader["Priority"];
                    objSKU.FrequencyID = (int)reader["FrequencyID"];
                    objSKU.FrequencyName = (string)reader["FrequencyName"];
                    objSKU.EffectiveFrom = (DateTime)reader["EffectiveFrom"];
                    objSKU.StandardDueDate = (DateTime)reader["StandardDueDate"];
                    objSKU.DeptId = (int)reader["DepartmentFunctionID"];
                    objSKU.DepartmentName = (string)reader["DepartmentFunctionName"];
                    objSKU.InitiatorId = (int)reader["PreparerId"];
                    objSKU.InitiatorName = (string)reader["PreparerName"];
                    objSKU.ApproverId = (int)reader["ApproverId"];
                    objSKU.ApproverName = (string)reader["ApproverName"];
                    //objSKU.FormsIfAny = (string)reader["FormsIfAny"];
                    objSKU.DueOnEvery = (int)reader["DueOnEvery"];
                    objSKU.FirstDueDate = (DateTime)reader["FirstDueDate"];
                    objSKU.DetailsOfComplianceRequirements = (string)reader["DetailsOfComplianceRequirements"];
                    objSKU.NonCompliancePenalty = (string)reader["Penalty"];
                    objSKU.ActionsToBeTaken = (string)reader["ActionToBeTaken"];
                    objSKU.DrivenById = (int)reader["DrivenById"];
                    objSKU.DrivenByName = (string)reader["DrivenBy"];
                    lstSKUWithPO.Add(objSKU);
                }
                reader.Close();
            }
            catch (SqlException err)
            {
                throw new ApplicationException("Data error.");
            }
            finally
            {
                con.Close();
            }
            return lstSKUWithPO;
        }
        public List<ComplianceCertificate> LoadComplianceCertificate(int deptId = 0, string period="")
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Compliance.usp_GetComplianceCertificateForReport", con);
            cmd.Parameters.AddWithValue("@DepartmentID", deptId);
            cmd.Parameters.AddWithValue("@period", period);
            cmd.CommandType = CommandType.StoredProcedure;
            List<ComplianceCertificate> lstSKUWithPO = new List<ComplianceCertificate>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ComplianceCertificate objSKU = new ComplianceCertificate();
                    objSKU.CertificateNo = (string)reader["CertificateNo"];
                    objSKU.GeneratedOn = (DateTime)reader["IssueDate"];
                    objSKU.Period = (string)reader["Period"];
                    objSKU.CertificatePath = (string)reader["CertificatePath"];
                    lstSKUWithPO.Add(objSKU);
                }
                reader.Close();
            }
            catch (SqlException err)
            {
                throw new ApplicationException("Data error.");
            }
            finally
            {
                con.Close();
            }
            return lstSKUWithPO;
        }

        public List<Performance> LoadCompliancePerformance(int deptId = 0, int typeId = 0, int nature = 0, int priority = 0, int periodId = 0)
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Compliance.usp_GetCompliancePerformance", con);
            cmd.Parameters.AddWithValue("@ComplianceNatureID", nature);
            cmd.Parameters.AddWithValue("@ComplianceTypeID", typeId);
            cmd.Parameters.AddWithValue("@DepartmentID", deptId);
            cmd.Parameters.AddWithValue("@PriorityID", priority);
            cmd.Parameters.AddWithValue("@PeriodID", periodId);
            cmd.CommandType = CommandType.StoredProcedure;
            List<Performance> lstSKUWithPO = new List<Performance>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Performance objSKU = new Performance();
                    objSKU.DepartmentName = (string)reader["DepartmentName"];
                    objSKU.NoOfCompliance = (int)reader["NoOfCompliance"];
                    objSKU.ClosedWithinTimeline = (int)reader["ClosedWithinTimeline"];
                    objSKU.ClosedWithDelays = (int)reader["ClosedWithDelays"];
                    objSKU.PercentageOfDelays = (Double)reader["PercentageOfDelays"];
                    objSKU.PendingSurpassedDueDate = (int)reader["PendingSurpassedDueDate"];
                    
                    lstSKUWithPO.Add(objSKU);
                }
                reader.Close();
            }
            catch (SqlException err)
            {
                throw new ApplicationException("Data error.");
            }
            finally
            {
                con.Close();
            }
            return lstSKUWithPO;
        }
        public List<ComplianceMasterr> LoadComplianceMaster(string compMaster, int userId = 0)
        {
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Compliance.usp_GetComplianceMaster", con);
            cmd.Parameters.AddWithValue("@compMaster", compMaster);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.CommandType = CommandType.StoredProcedure;
            List<ComplianceMasterr> lstSKUWithPO = new List<ComplianceMasterr>();
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ComplianceMasterr objSKU = new ComplianceMasterr();
                    objSKU.BusinessUnitID = (int)reader["BusinessUnitId"];
                    objSKU.BusinessUnitName = (string)reader["BusinessUnitName"];
                    objSKU.ComplianceID = (int)reader["ComplianceID"];
                    objSKU.ComplianceRef = (string)reader["ComplianceRef"];
                    objSKU.ComplianceTypeID = (int)reader["ComplianceTypeID"];
                    objSKU.ComplianceTypeName = (string)reader["ComplianceTypeName"];
                    objSKU.ActSectionReference = (string)reader["ActSectionReference"];
                    objSKU.ComplianceNatureID = (int)reader["NatureOfComplianceID"];
                    objSKU.NatureOfComplianceName = (string)reader["NatureOfComplianceName"];
                    objSKU.LawId = (int)reader["LawId"];
                    objSKU.LawName = (string)reader["LawName"];
                    objSKU.TerritoryId = (int)reader["TerritoryId"];
                    objSKU.Territory = (string)reader["Territory"];
                    objSKU.PriorityId = (int)reader["PriorityId"];
                    objSKU.Priority = (string)reader["Priority"];
                    objSKU.FrequencyID = (int)reader["FrequencyID"];
                    objSKU.FrequencyName = (string)reader["FrequencyName"];
                    objSKU.EffectiveFrom = (DateTime)reader["EffectiveFrom"];
                    objSKU.StandardDueDate = (DateTime)reader["StandardDueDate"];
                    objSKU.DeptId = (int)reader["DepartmentFunctionID"];  
                    objSKU.DepartmentName = (string)reader["DepartmentFunctionName"];  
                    objSKU.InitiatorId = (int)reader["PreparerId"];
                    objSKU.InitiatorName = (string)reader["PreparerName"];
                    objSKU.ApproverId = (int)reader["ApproverId"];
                    objSKU.ApproverName = (string)reader["ApproverName"];
                    objSKU.DueOnEvery = (int)reader["DueOnEvery"];
                    objSKU.FirstDueDate = (DateTime)reader["FirstDueDate"];
                    objSKU.DetailsOfComplianceRequirements = (string)reader["DetailsOfComplianceRequirements"];
                    objSKU.NonCompliancePenalty = (string)reader["Penalty"];
                    objSKU.ActionsToBeTaken = (string)reader["ActionToBeTaken"];
                    objSKU.DrivenById = (int)reader["DrivenById"];
                    objSKU.DrivenByName = (string)reader["DrivenBy"];
                    lstSKUWithPO.Add(objSKU);
                }
                reader.Close();
            }
            catch (SqlException err)
            {
                throw new ApplicationException("Data error.");
            }
            finally
            {
                con.Close();
            }
            return lstSKUWithPO;
        }
        //public void UpdateComplianceRef(int complianceId, string complianceRef)
        //{
        //    string query = "UPDATE compliance.ComplianceMaster SET ComplianceRef = @ComplianceRef WHERE ComplianceID = @complianceId";

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@ComplianceRef", complianceRef);
        //            cmd.Parameters.AddWithValue("@complianceId", complianceId);
        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}
    }
}
//2643