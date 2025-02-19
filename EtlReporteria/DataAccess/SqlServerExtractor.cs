using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using EtlReporteria.Models;

namespace EtlReporteria.DataAccess
{
    internal class SqlServerExtractor
        /*
         * Con esta clase podremos extraer la informacion dada la query que le coloquemos 
         */
    {
            private SqlServerConnection _connection;

            public SqlServerExtractor(SqlServerConnection connection)
            {
                _connection = connection;
            }
            public IEnumerable<BackupHistory_Table> Extract_BackupHistory_Data(string query)
            {
                var result = new List<BackupHistory_Table>();
                _connection.OpenConnection();

                using (var command = new SqlCommand(query, _connection.Connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new BackupHistory_Table
                        {
                            DatabaseName = reader["database_name"].ToString(),
                            BackupType = reader["BackupType"].ToString(),
                            DeviceType = reader["DeviceType"].ToString(),
                            RecoveryModel = reader["recovery_model"].ToString(),
                            CompatibilityLevel = reader["compatibility_level"] as int?,
                            BackupStartDate = (DateTime)reader["BackupStartDate"],
                            BackupFinishDate = (DateTime)reader["BackupFinishDate"],
                            LatestBackupLocation = reader["LatestBackupLocation"].ToString(),
                            BackupSizeMB = Convert.ToInt64(reader["BackupSizeMB"]) / 1024 / 1024,
                            CompressedBackupSizeMB = Convert.ToInt64(reader["CompressedBackupSizeMB"]) / 1024 / 1024,
                            ServerName = reader["server_name"].ToString()
                        };
                        result.Add(row);
                    }
                }
                _connection.CloseConnection();
                return result;
            }
        }
    }


