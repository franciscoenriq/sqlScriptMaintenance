SELECT 
    r.session_id, 
    r.start_time, 
    r.command, 
    q.text AS query_text,
    r.cpu_time, 
    r.total_elapsed_time, 
    r.logical_reads,         -- Logical reads performed by the query
    r.writes,                -- Writes performed by the query
    r.reads,                 -- Physical reads performed
    r.wait_type,             -- Type of wait if the query is waiting
    r.wait_time,             -- Total wait time
    s.login_name,            -- Who executed the query
    s.host_name,             -- Machine name of the user
    s.program_name,          -- Application that is running the query
    DB_NAME(r.database_id) AS database_name -- Database where the query is running
FROM sys.dm_exec_requests r
JOIN sys.dm_exec_sessions s ON r.session_id = s.session_id -- Get session details
CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) q -- Get SQL text
WHERE r.session_id <> @@SPID --para no consultarse a si misma 
ORDER BY r.total_elapsed_time DESC;


SELECT 
    qs.creation_time, 
    qs.execution_count, 
    qs.total_elapsed_time, 

    qs.total_logical_reads, 
    qs.total_physical_reads,
    qs.total_logical_writes,
    q.text AS query_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.plan_handle) q
ORDER BY qs.creation_time DESC;
