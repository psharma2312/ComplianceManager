using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ComplianceManager
{
    public class DbHelper
    {
        private readonly string _connectionString;
        private readonly ActionLogger _logger;

        public DbHelper(string connectionString, ActionLogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public int ExecuteNonQuery(string storedProc, Dictionary<string, object> parameters)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Executing NonQuery: {storedProc}");
                _logger.Debug($"Parameters: {FormatParams(parameters)}");

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(storedProc, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    AddParameters(cmd, parameters);
                    conn.Open();
                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                    stopwatch.Stop();
                    _logger.Info($"NonQuery {storedProc} completed in {stopwatch.ElapsedMilliseconds} ms");
                    return result;
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"ExecuteNonQuery failed: {ex.Message}\n{ex.StackTrace}");
                return -1;
            }
        }

        public SqlDataReader ExecuteReader(string storedProc, Dictionary<string, object> parameters)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Executing Reader: {storedProc}");
                _logger.Debug($"Parameters: {FormatParams(parameters)}");

                SqlConnection conn = new SqlConnection(_connectionString);
                SqlCommand cmd = new SqlCommand(storedProc, conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                AddParameters(cmd, parameters);
                conn.Open();
                stopwatch.Stop();
                _logger.Info($"Reader {storedProc} opened in {stopwatch.ElapsedMilliseconds} ms");
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"ExecuteReader failed: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        public object ExecuteScalar(string storedProc, Dictionary<string, object> parameters)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Executing Scalar: {storedProc}");
                _logger.Debug($"Parameters: {FormatParams(parameters)}");

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(storedProc, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    AddParameters(cmd, parameters);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();
                    stopwatch.Stop();
                    _logger.Info($"Scalar {storedProc} completed in {stopwatch.ElapsedMilliseconds} ms");
                    return result;
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"ExecuteScalar failed: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        public SqlDataReader ExecuteQuery(string sql, Dictionary<string, object> parameters)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                _logger.Info($"Executing Query: {sql}");
                _logger.Debug($"Parameters: {FormatParams(parameters)}");

                SqlConnection conn = new SqlConnection(_connectionString);
                SqlCommand cmd = new SqlCommand(sql, conn);
                AddParameters(cmd, parameters);
                conn.Open();
                stopwatch.Stop();
                _logger.Info($"Query opened in {stopwatch.ElapsedMilliseconds} ms");
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error($"ExecuteQuery failed: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        public void AddParameters(SqlCommand cmd, Dictionary<string, object> parameters)
        {
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        private string FormatParams(Dictionary<string, object> parameters)
        {
            return string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}"));
        }

    }
}