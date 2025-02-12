-- Esta consulta va a buscar la data a la tabla sysmaintplan_log la cual contiene la info de como 
-- se ejecutaron los subplanes de cada plan. A parte de esto le coloque mas info para poder extraer 
-- el nombre de los subplanes, del plan  y cuanto se demora en ejecutarse 

SELECT 
	mplans.name AS nombre_mantenimiento,
	msubplan.subplan_name AS nombre_subplan,
	mplans.owner AS creador,
	mplans.create_date,
	mlog.start_time AS inicio,
	mlog.end_time AS termino,
CASE 
    WHEN mlog.succeeded = 1 THEN 'ejecución exitosa'
    ELSE  'ejecución con errores' 
END AS estado_ejecucion
FROM msdb.dbo.sysmaintplan_log mlog
LEFT JOIN msdb.dbo.sysmaintplan_plans mplans ON mplans.id = mlog.plan_id 
LEFT JOIN msdb.dbo.sysmaintplan_subplans msubplan ON msubplan.subplan_id = mlog.subplan_id
ORDER BY CAST(mlog.start_time AS DATE)  DESC,
		mplans.name ASC


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
ORDER BY CAST(mlog.start_time AS DATE)  DESC,
		mplans.name ASC