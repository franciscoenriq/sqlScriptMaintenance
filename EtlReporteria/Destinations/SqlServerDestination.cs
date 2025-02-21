using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EtlReporteria.Models;
using EtlReporteria.DataAccess;
using EtlReporteria.Queries;
using Microsoft.Data.SqlClient;
namespace EtlReporteria.Destinations
{
    internal class SqlServerDestination
    {
        private SqlServerConnection _connection;

        public SqlServerDestination(SqlServerConnection connection)
        {
            _connection = connection;
        }
        public void InsertData_backupHistory(IEnumerable<BackupHistory_Table> rows)
        {
            _connection.OpenConnection();
            Console.WriteLine("insertando data"); 
            foreach (var row in rows)
            {
                var query_insert = Queries.Queries.GetQuery("InsertBackupHistory");

                using (var command = new SqlCommand(query_insert, _connection.Connection))
                {
                    command.Parameters.AddWithValue("@DatabaseName", row.DatabaseName);
                    command.Parameters.AddWithValue("@BackupType", row.BackupType);
                    command.Parameters.AddWithValue("@DeviceType", row.DeviceType);
                    command.Parameters.AddWithValue("@RecoveryModel", row.RecoveryModel ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CompatibilityLevel", row.CompatibilityLevel ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@BackupStartDate", row.BackupStartDate);
                    command.Parameters.AddWithValue("@BackupFinishDate", row.BackupFinishDate);
                    command.Parameters.AddWithValue("@Duracion_minutos", row.Duracion_minutos);
                    command.Parameters.AddWithValue("@LatestBackupLocation", row.LatestBackupLocation ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@BackupSize", row.BackupSizeMB);
                    command.Parameters.AddWithValue("@CompressedBackupSize", row.CompressedBackupSizeMB);
                    command.Parameters.AddWithValue("@ServerName", row.ServerName ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
            _connection.CloseConnection();
        }
        public void InsertData_JobsHiistory(IEnumerable<JobsHistory_Table> rows)
        {
            _connection.OpenConnection();
            Console.WriteLine("insertando data_jobs");
            foreach (var row in rows)
            {
                var query_insert = Queries.Queries.GetQuery("InsertJobsHistory");

                using (var command = new SqlCommand(query_insert, _connection.Connection))
                {
                    command.Parameters.AddWithValue("@JobName", row.JobName);
                    command.Parameters.AddWithValue("@Enabled", row.Enabled);
                    command.Parameters.AddWithValue("@CategoryId", row.CategoryId);
                    command.Parameters.AddWithValue("@CategoryName", row.CategoryName ?? (object)DBNull.Value); // Usamos DBNull si es nulo
                    command.Parameters.AddWithValue("@StepName", row.StepName ?? (object)DBNull.Value); // Usamos DBNull si es nulo
                    command.Parameters.AddWithValue("@SqlSeverity", row.SqlSeverity);
                    command.Parameters.AddWithValue("@Message", row.Message ?? (object)DBNull.Value); // Usamos DBNull si es nulo
                    command.Parameters.AddWithValue("@RunStatus", row.RunStatus);
                    command.Parameters.AddWithValue("@RunStatusDescription", row.RunStatusDescription ?? (object)DBNull.Value); // Usamos DBNull si es nulo
                    command.Parameters.AddWithValue("@RunDate", row.RunDate);
                    command.Parameters.AddWithValue("@RunTime", row.RunTime); // TimeSpan se pasa directamente
                    command.Parameters.AddWithValue("@RunDuration", row.RunDuration); // TimeSpan se pasa directamente

                    command.ExecuteNonQuery();
                }
            }

            _connection.CloseConnection();
        }

    }
}
