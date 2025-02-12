USE msdb;
GO
DECLARE @ayer INT = CONVERT(INT, CONVERT(VARCHAR(8), DATEADD(DAY, -1, GETDATE()), 112));
EXEC sp_help_jobhistory 
    @start_run_date = @ayer;
GO

USE VNT; 
--EXEC sp_spaceused;

ALTER DATABASE VNT SET RECOVERY SIMPLE; 
DBCC SHRINKFILE (VNT, 0, TRUNCATEONLY);

ALTER DATABASE VNT SET RECOVERY FULL ; 
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