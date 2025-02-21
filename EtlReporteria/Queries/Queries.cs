using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtlReporteria.Queries
{
    public static class Queries
    {
        public static readonly Dictionary<string, string> QueryDictionary = new()
    {
        { "InsertBackupHistory", @"INSERT INTO BackupHistory (DatabaseName, BackupType, DeviceType, RecoveryModel,
                                    CompatibilityLevel, BackupStartDate, BackupFinishDate, Duracion_minutos, LatestBackupLocation, BackupSize,
                                    CompressedBackupSize, ServerName) VALUES (@DatabaseName, @BackupType, @DeviceType, @RecoveryModel, @CompatibilityLevel,
                                    @BackupStartDate, @BackupFinishDate, @Duracion_minutos, @LatestBackupLocation, @BackupSize, @CompressedBackupSize, @ServerName)" },

        { "SelectBackupHistory", @"SELECT bs.database_name,
                                backuptype = CASE 
                                    WHEN bs.type = 'D' AND bs.is_copy_only = 0 THEN 'Full Database'
                                    WHEN bs.type = 'D' AND bs.is_copy_only = 1 THEN 'Full Copy-Only Database'
                                    WHEN bs.type = 'I' THEN 'Differential database backup'
                                    WHEN bs.type = 'L' THEN 'Transaction Log'
                                    WHEN bs.type = 'F' THEN 'File or filegroup'
                                    WHEN bs.type = 'G' THEN 'Differential file'
                                    WHEN bs.type = 'P' THEN 'Partial'
                                    WHEN bs.type = 'Q' THEN 'Differential partial'
                                    END + ' Backup',
                                CASE bf.device_type
                                    WHEN 2 THEN 'Disk'
                                    WHEN 5 THEN 'Tape'
                                    WHEN 7 THEN 'Virtual device'
                                    WHEN 9 THEN 'Azure Storage'
                                    WHEN 105 THEN 'A permanent backup device'
                                    ELSE 'Other Device'
                                    END AS DeviceType,
                                bs.recovery_model,
                                bs.compatibility_level,
                                BackupStartDate = bs.Backup_Start_Date,
                                BackupFinishDate = bs.Backup_Finish_Date,
                                LatestBackupLocation = bf.physical_device_name,
                                BackupSizeMB = bs.backup_size ,
                                CompressedBackupSizeMB = bs.compressed_backup_size ,
	                            server_name
                            FROM msdb.dbo.backupset bs
                            LEFT JOIN msdb.dbo.backupmediafamily bf
                                ON bs.[media_set_id] = bf.[media_set_id]
                            INNER JOIN msdb.dbo.backupmediaset bms
                                ON bs.[media_set_id] = bms.[media_set_id]
                            WHERE bs.backup_start_date > DATEADD(DAY, - 2, sysdatetime()) 
                            ORDER BY bs.Backup_Start_Date DESC, bs.database_name ASC" },
            {"SelectJobsHistory",@"
                                SELECT  
                                jobs.name,
                                jobs.enabled,
                                jobs.category_id,
                                categories.name AS category_name,
                                historyjobs.step_name,
                                historyjobs.sql_severity,
                                historyjobs.message,
                                historyjobs.run_status,
                                CASE historyjobs.run_status
                                    WHEN 0 THEN 'Failed'
                                    WHEN 1 THEN 'Succeeded'
                                    WHEN 2 THEN 'Retry'
                                    WHEN 3 THEN 'Canceled'
                                    ELSE 'Unknown'
                                END AS run_status_description,
                                CAST(CONVERT(DATE, CAST(historyjobs.run_date AS CHAR(8)), 112) AS DATE) AS run_date,
                                -- 🔹 Convertimos a TIME para que sea compatible con TimeSpan en C#
                                CAST( 
                                    STUFF(STUFF(RIGHT('000000' + CAST(historyjobs.run_time AS VARCHAR(6)), 6), 3, 0, ':'), 6, 0, ':') 
                                    AS TIME
                                ) AS run_time,
                                CAST(
                                    DATEADD(SECOND, historyjobs.run_duration % 100 + 
                                        ((historyjobs.run_duration / 100) % 100) * 60 + 
                                        (historyjobs.run_duration / 10000) * 3600, '00:00:00') AS TIME
                                ) AS run_duration
                            FROM msdb.dbo.sysjobhistory historyjobs
                            LEFT JOIN msdb.dbo.sysjobs jobs ON historyjobs.job_id = jobs.job_id
                            LEFT JOIN msdb.dbo.syscategories categories ON jobs.category_id = categories.category_id
                            WHERE CAST(CONVERT(DATE, CAST(historyjobs.run_date AS CHAR(8)), 112) AS DATE) 
                                  >= DATEADD(DAY, -2, CAST(SYSDATETIME() AS DATE))
                            ORDER BY historyjobs.run_date DESC, historyjobs.run_time DESC;

            " },
            {"InsertJobsHistory",@"INSERT INTO JobHistoryDetails (
                                    job_name,
                                    enabled,
                                    category_id,
                                    category_name,
                                    step_name,
                                    sql_severity,
                                    message,
                                    run_status,
                                    run_status_description,
                                    run_date,
                                    run_time,
                                    run_duration
                                )
                                VALUES (
                                    @JobName,
                                    @Enabled,
                                    @CategoryId,
                                    @CategoryName,
                                    @StepName,
                                    @SqlSeverity,
                                    @Message,
                                    @RunStatus,
                                    @RunStatusDescription,
                                    @RunDate,
                                    @RunTime,
                                    @RunDuration
                                );
            " }
    };

        public static string GetQuery(string key)
        {
            if (QueryDictionary.TryGetValue(key, out var query))
                return query;
            throw new KeyNotFoundException($"Query '{key}' no encontrada.");
        }
    }
}

