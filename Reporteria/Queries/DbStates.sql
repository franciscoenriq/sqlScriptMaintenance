SELECT 
    name AS DatabaseName,
    state_desc AS Estado,
    CASE 
        WHEN state_desc = 'ONLINE' THEN 'Activa'
        ELSE ' Inactiva o con problemas'
    END AS EstadoDetallado,
	GETDATE() AS FechaHoraEjecucion
FROM sys.databases