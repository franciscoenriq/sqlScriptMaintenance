-- aca podemos saber el estado de la bd, tanto sus nombres como su 

SELECT 
    d.name AS DatabaseName,
    mf.name as name,
	mf.state_desc AS FileStatem,
	mf.physical_name,
	CASE 
		WHEN mf.type = 0 THEN 'Archivo de datos'
		WHEN mf.type = 1 THEN 'Archivo Log'
	END AS Tipo_Archivo, 
    mf.size * 8 / 1024 AS Size_MB,
	CASE 
		WHEN mf.max_size = -1 THEN 'Tamano ilimitado' 
		ELSE CAST(mf.max_size AS VARCHAR)
	END  AS Tamano_maximo,
	d.recovery_model,
	d.create_date
    
FROM sys.master_files mf
JOIN sys.databases d ON mf.database_id = d.database_id
ORDER BY d.name;


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