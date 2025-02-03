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
    