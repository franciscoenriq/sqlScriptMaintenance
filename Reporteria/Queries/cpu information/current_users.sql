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
    s.last_request_end_time,
    r.start_time AS request_start_time,  -- Cuándo inició la solicitud actual
    r.command AS current_command,       -- Qué comando está ejecutando la sesión
    r.cpu_time AS request_cpu_time,     -- CPU usada por la solicitud actual
    r.total_elapsed_time AS elapsed_time, -- Tiempo total de ejecución de la solicitud actual
    c.client_net_address,               -- Dirección IP del cliente
    c.local_net_address,                -- Dirección IP del servidor
    c.connect_time      -- Hora en que se estableció la conexión
FROM 
    sys.dm_exec_sessions s
LEFT JOIN sys.dm_exec_requests r ON s.session_id = r.session_id  -- Une las solicitudes activas
LEFT JOIN sys.dm_exec_connections c ON s.session_id = c.session_id -- Une las conexiones activas
WHERE 
    s.is_user_process = 1    -- Solo sesiones de usuario
ORDER BY 
    s.login_name;