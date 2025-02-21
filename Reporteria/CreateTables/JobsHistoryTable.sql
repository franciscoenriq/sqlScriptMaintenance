CREATE TABLE JobHistoryDetails (
    job_name NVARCHAR(128),               -- Nombre del job
    enabled BIT,                          -- Estado habilitado (1=habilitado, 0=deshabilitado)
    category_id INT,                      -- ID de la categoría
    category_name NVARCHAR(128),          -- Nombre de la categoría
    step_name NVARCHAR(128),              -- Nombre del paso del job
    sql_severity INT,                     -- Severidad SQL
    message NVARCHAR(MAX),                -- Mensaje del job
    run_status INT,                       -- Estado de ejecución (0=Failed, 1=Succeeded, 2=Retry, 3=Canceled)
    run_status_description NVARCHAR(50),  -- Descripción del estado (Failed, Succeeded, etc.)
    run_date DATE,                        -- Fecha de ejecución del job
    run_time TIME,                        -- Hora de ejecución del job (tipo TIME)
    run_duration TIME                      -- Duración (tipo TIME)
);

