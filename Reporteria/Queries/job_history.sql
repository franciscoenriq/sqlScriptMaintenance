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
SELECT job.name, job.job_id, run_status, run_date, run_time, run_duration, step_name, message
FROM msdb.dbo.sysjobhistory hist
JOIN msdb.dbo.sysjobs job ON hist.job_id = job.job_id
WHERE run_status = 0 
ORDER BY run_date DESC;

