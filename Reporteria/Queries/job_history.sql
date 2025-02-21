
-----------------------------------------
 SELECT  
	jobs.name,
	jobs.enabled,
	jobs.category_id,
	historyjobs.step_name,
	historyjobs.sql_severity,
	historyjobs.run_status,
	historyjobs.message,
	historyjobs.run_date,
	historyjobs.run_time,
	historyjobs.run_duration
 FROM msdb.dbo.sysjobhistory historyjobs
 LEFT JOIN msdb.dbo.sysjobs jobs ON historyjobs.job_id = jobs.job_id
 ORDER BY run_date desc, run_time

 --- usamos esta query 

 SELECT  
    jobs.name,
    jobs.enabled,
    jobs.category_id,
    categories.name AS category_name,  -- Nombre de la categoría
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
    END AS run_status_description,  -- Descripción del estado
    run_date = CAST(CONVERT(DATE, CAST(historyjobs.run_date AS CHAR(8)), 112) AS DATE),
    run_time = STUFF(STUFF(RIGHT('000000' + CAST(historyjobs.run_time AS VARCHAR(6)), 6), 3, 0, ':'), 6, 0, ':'), 
    run_duration = STUFF(STUFF(RIGHT('000000' + CAST(historyjobs.run_duration AS VARCHAR(6)), 6), 3, 0, ':'), 6, 0, ':') -- Duración formateada
FROM msdb.dbo.sysjobhistory historyjobs
LEFT JOIN msdb.dbo.sysjobs jobs ON historyjobs.job_id = jobs.job_id
LEFT JOIN msdb.dbo.syscategories categories ON jobs.category_id = categories.category_id  -- Join para categorías
WHERE CAST(CONVERT(DATE, CAST(historyjobs.run_date AS CHAR(8)), 112) AS DATE) 
      >= DATEADD(DAY, -2, CAST(SYSDATETIME() AS DATE))
ORDER BY historyjobs.run_date DESC, historyjobs.run_time DESC;