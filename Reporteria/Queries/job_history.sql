
-----------------------------------------
 SELECT  
	jobs.name,
	jobs.enabled,
	jobs.category_id,
	historyjobs.step_name,
	historyjobs.sql_severity,
	historyjobs.message,
	historyjobs.run_status,
	historyjobs.message,
	historyjobs.run_date,
	historyjobs.run_time,
	historyjobs.run_duration
 FROM msdb.dbo.sysjobhistory historyjobs
 LEFT JOIN msdb.dbo.sysjobs jobs ON historyjobs.job_id = jobs.job_id
 ORDER BY run_date desc, run_time