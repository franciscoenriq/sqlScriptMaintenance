SELECT 
    name AS DatabaseName,
    state_desc AS Estado,
    CASE 
        WHEN state_desc = 'ONLINE' THEN 'Activa'
        ELSE ' Inactiva o con problemas'
    END AS EstadoDetallado,
	GETDATE() AS FechaHoraEjecucion
FROM sys.databases
---------------------------------------------------------------------------
SELECT 
    name AS DatabaseName,
    state_desc AS Estado
FROM sys.databases
WHERE state_desc <> 'ONLINE';
---------------------------------------------------------------------------
SELECT 
    database_id, 
    name AS DatabaseName, 
    state_desc AS Estado, 
    recovery_model_desc AS RecoveryModel, 
    compatibility_level AS CompatLevel, 
    create_date AS FechaCreacion 
FROM sys.databases;
--------------------------------------------------------------------------
SELECT 
    d.name AS DatabaseName,
    SUM(f.size * 8) / 1024 AS SizeMB,
    SUM(f.size * 8) / 1024 / 1024 AS SizeGB
FROM sys.databases d
JOIN sys.master_files f ON d.database_id = f.database_id
WHERE f.type_desc = 'ROWS'
GROUP BY d.name;