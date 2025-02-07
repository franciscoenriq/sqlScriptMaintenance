--Query que nos dice los backups realizados en los ultimos n dias ( hay que reemplazar el n)
--Da el nombre de la bd, inicio y termino del backup, cuato demora en minutos, el tamaño del backup en Mb , donde queda guardada. 
--El tipo de backup, D: es full backup por ejemplo. is_copy_only indica si el backup es un backup especial que no interrumpe la cadena de backups. 

SELECT
    b.database_name,
    b.backup_start_date,
    b.backup_finish_date,
    CAST(b.backup_size / 1048576.0 AS DECIMAL(10,2)) AS backup_size_mb,  -- Tamaño en MB
    b.type,
    b.is_copy_only,
    CAST(DATEDIFF(SECOND, b.backup_start_date, b.backup_finish_date) AS FLOAT) / 60 AS backup_duration_minutes,
    bm.physical_device_name AS backup_file_path
FROM
    msdb.dbo.backupset b
JOIN
    msdb.dbo.backupmediafamily bm ON b.media_set_id = bm.media_set_id
WHERE
    b.backup_finish_date > DATEADD(DAY, -n, GETDATE())  -- Backups de los ultimos n dias. 
ORDER BY
	b.database_name,
    b.backup_finish_date DESC;

--- QUERY MEJORADA 
SELECT bs.database_name,
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
    bms.software_name AS backup_software,
    bs.recovery_model,
    bs.compatibility_level,
    BackupStartDate = bs.Backup_Start_Date,
    BackupFinishDate = bs.Backup_Finish_Date,
	BackupDurationMinutes = CAST(DATEDIFF(SECOND, bs.Backup_Start_Date,Backup_Finish_Date) AS FLOAT)/60,
    LatestBackupLocation = bf.physical_device_name,
    backup_size_mb = CONVERT(DECIMAL(10, 2), bs.backup_size / 1024. / 1024.),
    compressed_backup_size_mb = CONVERT(DECIMAL(10, 2), bs.compressed_backup_size / 1024. / 1024.),
    database_backup_lsn, -- For tlog and differential backups, this is the checkpoint_lsn of the FULL backup it is based on.
    checkpoint_lsn,
    begins_log_chain,
    bms.is_password_protected
FROM msdb.dbo.backupset bs
LEFT JOIN msdb.dbo.backupmediafamily bf
    ON bs.[media_set_id] = bf.[media_set_id]
INNER JOIN msdb.dbo.backupmediaset bms
    ON bs.[media_set_id] = bms.[media_set_id]
WHERE bs.backup_start_date > DATEADD(DAY, - 10, sysdatetime()) --only look at last two months
ORDER BY bs.database_name ASC,
    bs.Backup_Start_Date DESC;