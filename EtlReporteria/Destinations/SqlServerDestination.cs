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

    }
}
