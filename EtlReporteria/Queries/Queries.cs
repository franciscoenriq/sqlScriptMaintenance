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
        { "InsertBackupHistory", @"INSERT INTO BackupHistory_Table (DatabaseName, BackupType, DeviceType, RecoveryModel,
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
                                bs.backup_size ,
                                bs.compressed_backup_size,
	                            server_name
                            FROM msdb.dbo.backupset bs
                            LEFT JOIN msdb.dbo.backupmediafamily bf
                                ON bs.[media_set_id] = bf.[media_set_id]
                            INNER JOIN msdb.dbo.backupmediaset bms
                                ON bs.[media_set_id] = bms.[media_set_id]
                            WHERE bs.backup_start_date > DATEADD(DAY, - 2, sysdatetime()) 
                            ORDER BY bs.Backup_Start_Date DESC, bs.database_name ASC" }
    };

        public static string GetQuery(string key)
        {
            if (QueryDictionary.TryGetValue(key, out var query))
                return query;
            throw new KeyNotFoundException($"Query '{key}' no encontrada.");
        }
    }
}

