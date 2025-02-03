USE [NombreDeTuBaseDeDatos];
GO

SELECT 
    OBJECT_NAME(ix.OBJECT_ID) AS Tabla,
    ix.name AS Indice,
    ix.type_desc AS TipoIndice,
    ps.avg_fragmentation_in_percent AS FragmentacionPromedio,
    ps.page_count AS NumeroDePaginas
FROM 
    sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'DETAILED') ps
INNER JOIN 
    sys.indexes ix ON ps.object_id = ix.object_id AND ps.index_id = ix.index_id
WHERE 
    ps.avg_fragmentation_in_percent > 10
    AND ps.page_count > 100 -- Filtrar índices con más de 100 páginas para relevancia
ORDER BY 
    ps.avg_fragmentation_in_percent DESC;


-- Consulta para saber si las tablas tienen fragmentacion moderada , entre el 5 y el 30 porciento. 
SELECT
    t.name AS TableName,
    i.name AS IndexName,
    d.avg_fragmentation_in_percent AS Fragmentation
FROM
    sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') d
    JOIN sys.indexes i ON d.object_id = i.object_id AND d.index_id = i.index_id
    JOIN sys.tables t ON t.object_id = d.object_id
WHERE
    d.avg_fragmentation_in_percent BETWEEN 5 AND 30 -- Fragmentación moderada
    AND i.type > 0 -- Ignorar índices tipo HEAP
    AND i.is_disabled = 0 -- Ignorar índices deshabilitados
ORDER BY d.avg_fragmentation_in_percent DESC;

--consulta para saber la fragmentacion de tablas tipo heap y que tengan fragmentacion sobre 30 porciento
SELECT
    OBJECT_NAME(d.object_id) AS TableName,
    d.avg_fragmentation_in_percent,
    d.forwarded_record_count
FROM
    sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'DETAILED') d
WHERE
    d.index_id = 0 -- Solo tablas HEAP
    AND d.avg_fragmentation_in_percent > 30;


-- onsulta para obtener tamaño de las bases de datos 
USE master;
GO
SELECT 
    name AS DatabaseName,
    size * 8 / 1024 AS SizeMB -- Tamaño en MB
FROM sys.master_files
WHERE type_desc = 'ROWS';

--consulta para saber actualizaciones de estadisticas de las bases de datos :
USE [NombreDeTuBaseDeDatos];

SELECT 
    s.name AS StatisticName,
    OBJECT_NAME(s.object_id) AS TableName,
    sp.last_updated AS LastUpdated,
    sp.rows_sampled AS RowsSampled,
    sp.unfiltered_rows AS UnfilteredRowCount,
    sp.modification_counter AS ModificationCounter
FROM 
    sys.stats s
CROSS APPLY 
    sys.dm_db_stats_properties(s.object_id, s.stats_id) sp
WHERE 
    sp.last_updated IS NOT NULL
ORDER BY 
    sp.last_updated DESC;

