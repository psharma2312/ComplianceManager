using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ComplianceManager
{
    public class Constants
    {
        /// <summary>
        /// The value used to represent a null DateTime value 
        /// </summary>
        public static DateTime NullDateTime = DateTime.MinValue;

        /// <summary>
        /// The value used to represent a null decimal value
        /// </summary>
        public static decimal NullDecimal = decimal.MinValue;

        /// <summary>
        /// The value used to represent a null double value
        /// </summary>
        public static double NullDouble = double.MinValue;

        /// <summary>
        /// The value used to represent a null Guid value
        /// </summary>
        public static Guid NullGuid = Guid.Empty;

        /// <summary>
        /// The value used to represent a null int value
        /// </summary>
        public static int NullInt = int.MinValue;

        /// <summary>
        /// The value used to represent a null long value
        /// </summary>
        public static long NullLong = long.MinValue;

        /// <summary>
        /// The value used to represent a null float value
        /// </summary>
        public static float NullFloat = float.MinValue;

        /// <summary>
        /// The value used to represent a null string value
        /// </summary>
        public static string NullString = string.Empty;

        public static byte Nullbyte = byte.MinValue;

        /// <summary>
        /// Maximum DateTime value allowed by SQL Server
        /// </summary>
        public static DateTime SqlMaxDate = new DateTime(9999, 1, 3, 23, 59, 59);

        /// <summary>
        /// Minimum DateTime value allowed by SQL Server
        /// </summary>
        public static DateTime SqlMinDate = new DateTime(1753, 1, 1, 00, 00, 00);
    }
    public class ErrorHandler
    {

        private static string _className = String.Empty;
        private static string _methodName = string.Empty;
        public static string ErrorHandle(System.Exception ex, string ClassName, string MethodName, string sErrNum)
        {
            string sError;
            string sSpecialMsg;
            sSpecialMsg = ErrMsg(sErrNum);
            _className = ClassName;
            _methodName = MethodName;
            sError = ex.StackTrace + "\n" + "Error Message : " + ex.Message + "\t" + ex.HelpLink;
            WriteToLog(sError);
            return (sErrNum + "\t" + sSpecialMsg);
        }

        public static string ErrorHandle(Exception ex, string ClassName, string MethodName)
        {
            string sError;
            string sSpecialMsg;
            sSpecialMsg = ErrMsg(ex.Message);
            _className = ClassName;
            _methodName = MethodName;
            sError = "Error Occured " + ex.StackTrace + "\t" + ex.Message + "\t" + ex.HelpLink;

            WriteToLog(ex);
            return (ex.Message + "\t" + sSpecialMsg);
        }
        public static string ErrorHandle(Exception ex, int sErrNum)
        {
            string sError;
            string sSpecialMsg;
            sSpecialMsg = ErrMsg(System.Convert.ToString(sErrNum));
            sError = "Error Occured" + ex.StackTrace + "\t" + ex.Message + "\t" + ex.HelpLink;
            WriteToLog(sError);
            return (sErrNum + "\t" + sSpecialMsg);
        }

        public static string ErrorHandle(Exception ex, string strErrInfo)
        {
            string sError;
            string sSpecialMsg;
            sSpecialMsg = ErrMsg(ex.Message);
            sError = "Error Occured" + ex.StackTrace + "\t" + ex.Message + "\t" + ex.HelpLink;
            if (strErrInfo == "")
            {
                WriteToLog(sError);
            }
            else
            {
                WriteToLog(sError, strErrInfo);
            }
            return (ex.Message + "\t" + sSpecialMsg);
        }

        public static string ErrorHandle(Exception ex)
        {
            string sError;
            string sSpecialMsg;
            sSpecialMsg = ErrMsg(ex.Message);
            sError = "Error Occured" + ex.StackTrace + "\t" + ex.Message + "\t" + ex.HelpLink;
            WriteToLog(sError);
            return (ex.Message + "\t" + sSpecialMsg);
        }

        public static string ErrMsg(string sErrNum)
        {
            if (sErrNum == "0")
            {
                return "";
            }
            else if (sErrNum == "1")
            {
                return "Another error";
            }
            else
            {
                return "Needs to be Fixed. Call Customer Service";
            }
        }

        public static void WriteToLog(string sError)
        {
            try
            {
                StreamWriter objStreamWriter;
                FileInfo objFileInfo;
                string sFilePath = System.Configuration.ConfigurationManager.AppSettings["ErrorLog"]; //LogFile; 
                objFileInfo = new FileInfo(sFilePath);
                if ((objFileInfo.Exists == false))
                {
                    objStreamWriter = objFileInfo.CreateText();
                }
                else
                {
                    objStreamWriter = new StreamWriter(sFilePath, true);
                }
                if (_className != null)
                    objStreamWriter.WriteLine("Class name : " + _className);
                if (_methodName != null)
                    objStreamWriter.WriteLine("Method name : " + _methodName);
                objStreamWriter.WriteLine("Time :" + System.Convert.ToString(System.DateTime.Now) + "\n Error: " + sError);
                objStreamWriter.WriteLine("============================================");
                objStreamWriter.Close();
            }
            catch (Exception ex)
            {
                string sExError;
                sExError = ex.Message;
            }
            finally
            {
            }
        }
        public static void WriteToLog(System.Exception oex)
        {
            try
            {
                StreamWriter objStreamWriter;
                FileInfo objFileInfo;
                string sFilePath = System.Configuration.ConfigurationManager.AppSettings["ErrorLog"]; //LogFile; 
                objFileInfo = new FileInfo(sFilePath);
                if ((objFileInfo.Exists == false))
                {
                    objStreamWriter = objFileInfo.CreateText();
                }
                else
                {
                    objStreamWriter = new StreamWriter(sFilePath, true);
                }
                objStreamWriter.WriteLine("Time : " + System.Convert.ToString(System.DateTime.Now));

                if (_className != null)
                    objStreamWriter.WriteLine("\t Class name : " + _className);

                if (_methodName != null)
                    objStreamWriter.WriteLine("\t Method name : " + _methodName);

                if (oex.Message != string.Empty)
                    objStreamWriter.WriteLine("\t Message : " + oex.Message);

                if (oex.StackTrace != string.Empty)
                    objStreamWriter.WriteLine("\t StackTrace : " + oex.StackTrace);

                if (oex.HelpLink != null)
                    objStreamWriter.WriteLine("\t HelpLink : " + oex.HelpLink);

                objStreamWriter.WriteLine("==============================================================");
                objStreamWriter.Close();
            }
            catch (Exception ex)
            {
                string sExError;
                sExError = ex.Message;
            }
            finally
            {
            }
        }
        public static void WriteToLog(string sError, string strErrInfo)
        {
            try
            {
                StreamWriter objStreamWriter;
                FileInfo objFileInfo;
                string sFilePath = System.Configuration.ConfigurationManager.AppSettings["ErrorLog"]; //LogFile; 
                objFileInfo = new FileInfo(sFilePath);
                if ((objFileInfo.Exists == false))
                {
                    objStreamWriter = objFileInfo.CreateText();
                }
                else
                {
                    objStreamWriter = new StreamWriter(sFilePath, true);
                }
                if (_className != null)
                    objStreamWriter.WriteLine("Class name : " + _className);
                if (_methodName != null)
                    objStreamWriter.WriteLine("Method name : " + _methodName);
                objStreamWriter.WriteLine(System.Convert.ToString(System.DateTime.Now) + " " + sError);
                objStreamWriter.WriteLine(strErrInfo);
                objStreamWriter.WriteLine("============================================");
                objStreamWriter.Close();
            }
            catch (Exception ex)
            {
                string sExError;
                sExError = ex.Message;
            }
            finally
            {
            }
        }
    }
}