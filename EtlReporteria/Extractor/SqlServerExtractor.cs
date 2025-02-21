using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using EtlReporteria.Models;
using EtlReporteria.DataAccess;

namespace EtlReporteria.Extractor
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

        /*
         * Metodo para extraer la data del historial de backups. 
         */
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
        /*
         * Metodo para extraer la informacion de los jobs acontecidos durante un periodo definido en la query. 
         */
        public IEnumerable<JobsHistory_Table> Extract_JobsHistory_Data(string query)
        {
            var result = new List<JobsHistory_Table>();
            _connection.OpenConnection();
            using (var command = new SqlCommand(query, _connection.Connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new JobsHistory_Table
                    {
                        JobName = reader["name"].ToString(),
                        Enabled = Convert.ToBoolean(reader["enabled"]),
                        CategoryId = Convert.ToInt32(reader["category_id"]),
                        CategoryName = reader["category_name"].ToString(),
                        StepName = reader["step_name"].ToString(),
                        SqlSeverity = Convert.ToInt32(reader["sql_severity"]),
                        Message = reader["message"].ToString(),
                        RunStatus = Convert.ToInt32(reader["run_status"]),
                        RunStatusDescription = reader["run_status_description"].ToString(),
                        RunDate = Convert.ToDateTime(reader["run_date"]),
                        RunTime = (TimeSpan)reader["run_time"],
                        RunDuration = (TimeSpan)reader["run_duration"]
                    };
                    result.Add(row);
                }
                _connection.CloseConnection();
                return result;
            }
        }
        /*
         * Metodo para extrar los datos del estado de las bases de datos.
         */
        public IEnumerable<DbState_Table> Extract_DbState_Data(string query)
        {
            var result = new List<DbState_Table>();
            _connection.OpenConnection();
            using (var command = new SqlCommand(query, _connection.Connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new DbState_Table
                    {
                        LogicDbName = reader["Logic_db_name"].ToString(),
                        FileState = reader["File_State"].ToString(),
                        TipoArchivo = reader["Tipo_Archivo"].ToString(),
                        SizeMB = Convert.ToInt32(reader["Size_MB"]),
                        TamanoMaximo = Convert.ToInt32(reader["Tamano_maximo"]),
                        FechaEjecucion = Convert.ToDateTime(reader["Fecha_ejecucion"]),
                        HoraEjecucion = (TimeSpan)reader["Hora_ejecucion"]
                    };
                    result.Add(row);
                }
                _connection.CloseConnection();
                return result;
            }
        }
    }
}
       

