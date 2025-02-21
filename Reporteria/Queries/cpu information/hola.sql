SELECT 
    mp.id_malla,
    mp.id_proceso,
    tbp.nombre 
FROM dbo.tb_det_malla_procesos mp
JOIN dbo.tb_procesos tbp ON mp.id_proceso = tbp.id_proceso;

select * from [dbo].[tb_procesos] ORDER BY 1 DESC 
SELECT * FROM dbo.tb_det_malla_procesos where id_proceso = 71 ORDER BY inicio DESC 

SELECT 
	mplans.name AS nombre_mantenimiento,
	msubplan.subplan_name AS nombre_subplan,
	mplans.owner AS creador,
	mplans.create_date,
	mlog.start_time AS inicio,
	mlog.end_time AS termino,
	DATEDIFF(SECOND, mlog.start_time, mlog.end_time) AS Duracion_segundos ,
	CASE 
		WHEN mlog.succeeded = 1 THEN 'ejecución exitosa'
		ELSE  'ejecución con errores' 
	END AS estado_ejecucion , 
	mlog.succeeded 
FROM msdb.dbo.sysmaintplan_log mlog
LEFT JOIN msdb.dbo.sysmaintplan_plans mplans ON mplans.id = mlog.plan_id 
LEFT JOIN msdb.dbo.sysmaintplan_subplans msubplan ON msubplan.subplan_id = mlog.subplan_id
WHERE mlog.start_time > DATEADD(HOUR, 17, CAST(DATEADD(DAY, -1, GETDATE()) AS DATETIME)) 
ORDER BY CAST(mlog.start_time AS DATE)  DESC,
		mplans.name ASC


SELECT 
    mf.name as Logic_db_name,
	mf.state_desc AS File_State,
	CASE 
		WHEN mf.type = 0 THEN 'Archivo de datos'
		WHEN mf.type = 1 THEN 'Archivo Log'
	END AS Tipo_Archivo, 
    mf.size * 8 / 1024 AS Size_MB,
	 mf.max_size AS Tamano_maximo,
	CAST(GETDATE() AS DATE) AS Fecha_ejecucion,
	CONVERT(TIME(0), GETDATE()) AS Hora_ejecucion
    
FROM sys.master_files mf
JOIN sys.databases d ON mf.database_id = d.database_id
ORDER BY d.name;


SELECT DB_NAME(ps.database_id) AS database_name, COUNT(*) AS execution_count
FROM sys.dm_exec_procedure_stats ps  
GROUP BY DB_NAME(ps.database_id)
ORDER BY execution_count DESC;


SELECT  
    ps.database_id,  
    DB_NAME(ps.database_id) AS database_name,  
    OBJECT_NAME(ps.object_id, ps.database_id) AS procedure_name,  
    ps.type_desc,  
    ps.cached_time,  
    ps.last_execution_time,  
    ps.execution_count,  
    ps.total_worker_time  
FROM sys.dm_exec_procedure_stats ps  
WHERE LOWER(OBJECT_NAME(ps.object_id, ps.database_id)) LIKE 'rcp_ods%'
ORDER BY ps.cached_time DESC;


select * from sys.procedures order by create_date desc

