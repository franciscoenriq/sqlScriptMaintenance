USE [VNT];
DECLARE @SchemaName NVARCHAR(128);
DECLARE @TableName NVARCHAR(128);
DECLARE @IndexName NVARCHAR(128);
DECLARE @FragPercent FLOAT;
DECLARE @SQL NVARCHAR(MAX);

DECLARE IndexCursor CURSOR FOR
SELECT 
    s.name AS SchemaName,
    t.name AS TableName,
    i.name AS IndexName,
    d.avg_fragmentation_in_percent AS FragPercent
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') d
JOIN sys.indexes i ON d.object_id = i.object_id AND d.index_id = i.index_id
JOIN sys.tables t ON t.object_id = d.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE d.avg_fragmentation_in_percent > 5
AND i.type_desc NOT IN ('HEAP', 'XML', 'FULLTEXT')
AND i.is_disabled = 0;

OPEN IndexCursor;
FETCH NEXT FROM IndexCursor INTO @SchemaName, @TableName, @IndexName, @FragPercent;

WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY 
        IF @FragPercent BETWEEN 5 AND 30
            SET @SQL = 'ALTER INDEX [' + @IndexName + '] ON [' + @SchemaName + '].[' + @TableName + '] REORGANIZE;';
        ELSE
            SET @SQL = 'ALTER INDEX [' + @IndexName + '] ON [' + @SchemaName + '].[' + @TableName + '] REBUILD;';

        EXEC sp_executesql @SQL;
    END TRY 
    BEGIN CATCH
        PRINT 'ERROR en mantenimiento de índices: ' + ERROR_MESSAGE();
    END CATCH;

    FETCH NEXT FROM IndexCursor INTO @SchemaName, @TableName, @IndexName, @FragPercent;
END;

CLOSE IndexCursor;
DEALLOCATE IndexCursor;

-- REALIZAR SHRINKFILE
USE VNT; 
ALTER DATABASE VNT SET RECOVERY SIMPLE; 
DBCC SHRINKFILE (VNT, 0);
ALTER DATABASE VNT SET RECOVERY FULL; 

-- VOLVER A EJECUTAR EL PROCESO DE REORGANIZACIÓN Y RECONSTRUCCIÓN DE ÍNDICES
DECLARE IndexCursor CURSOR FOR
SELECT 
    s.name AS SchemaName,
    t.name AS TableName,
    i.name AS IndexName,
    d.avg_fragmentation_in_percent AS FragPercent
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') d
JOIN sys.indexes i ON d.object_id = i.object_id AND d.index_id = i.index_id
JOIN sys.tables t ON t.object_id = d.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE d.avg_fragmentation_in_percent > 5
AND i.type_desc NOT IN ('HEAP', 'XML', 'FULLTEXT')
AND i.is_disabled = 0;

OPEN IndexCursor;
FETCH NEXT FROM IndexCursor INTO @SchemaName, @TableName, @IndexName, @FragPercent;

WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY 
        IF @FragPercent BETWEEN 5 AND 30
            SET @SQL = 'ALTER INDEX [' + @IndexName + '] ON [' + @SchemaName + '].[' + @TableName + '] REORGANIZE;';
        ELSE
            SET @SQL = 'ALTER INDEX [' + @IndexName + '] ON [' + @SchemaName + '].[' + @TableName + '] REBUILD;';

        EXEC sp_executesql @SQL;
    END TRY 
    BEGIN CATCH
        PRINT 'ERROR en mantenimiento de índices: ' + ERROR_MESSAGE();
    END CATCH;

    FETCH NEXT FROM IndexCursor INTO @SchemaName, @TableName, @IndexName, @FragPercent;
END;

CLOSE IndexCursor;
DEALLOCATE IndexCursor;



SELECT 
    s.name AS SchemaName,
    t.name AS TableName,
    i.name AS IndexName,
    d.avg_fragmentation_in_percent AS FragPercent
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') d
JOIN sys.indexes i ON d.object_id = i.object_id AND d.index_id = i.index_id
JOIN sys.tables t ON t.object_id = d.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE d.avg_fragmentation_in_percent > 25
AND i.type_desc NOT IN ('HEAP', 'XML', 'FULLTEXT')
AND i.is_disabled = 0;