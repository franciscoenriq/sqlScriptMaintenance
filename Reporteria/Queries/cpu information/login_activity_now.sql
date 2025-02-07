SELECT 
    s.session_id,
    s.login_name,
    s.host_name,
    s.program_name,
    s.status,
    s.cpu_time,
    s.memory_usage,
    s.login_time,
    s.last_request_start_time,
    s.last_request_end_time
FROM 
    sys.dm_exec_sessions s
WHERE 
    s.is_user_process = 1    -- Solo procesos de usuarios
ORDER BY  login_name;