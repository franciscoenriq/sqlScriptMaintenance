DECLARE @ruta NVARCHAR(255);
DECLARE @ruta_completa NVARCHAR(255);
DECLARE @database_name NVARCHAR(255);
DECLARE @database_namelog NVARCHAR(255);
DECLARE @sql_command NVARCHAR(MAX);

-- Lista de bases de datos a procesar 
DECLARE @databases TABLE (database_name NVARCHAR(255));
INSERT INTO @databases VALUES 
    (N'VNT');

-- Iteramos sobre las bases de datos que queremos mantener
DECLARE db_cursor CURSOR FOR 
SELECT database_name FROM @databases;

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @database_name;

WHILE @@FETCH_STATUS = 0 
BEGIN
    BEGIN TRY 
        
        -- Obtener nombre del archivo de log
        SET @sql_command = N'
            SELECT @database_namelog_OUT = [name]
            FROM [' + @database_name + N'].sys.database_files
            WHERE [type_desc] = ''LOG'';
        ';
        EXEC sp_executesql 
            @sql_command,
            N'@database_namelog_OUT NVARCHAR(255) OUTPUT',
            @database_namelog_OUT = @database_namelog OUTPUT;

        PRINT N'Archivo de log: ' + ISNULL(@database_namelog, N'No encontrado');
        -- Realizamos Shrink a los logs
        --Primero sesteamos recovery en simple.
        SET @sql_command = N'USE [' + @database_name + N']; ALTER DATABASE ' + @database_name + N' SET RECOVERY SIMPLE;'
        EXEC sp_executesql @sql_command;

        --Shrinkfile 
        SET @sql_command = N'DBCC SHRINKFILE (' + @database_namelog + N', 10); '
        EXEC sp_executesql @sql_command;
        -- volvemos nuevamente a modo full 

        SET @sql_command = N'USE [' + @database_name + N']; ALTER DATABASE ' + @database_name + N' SET RECOVERY FULL;'
        EXEC sp_executesql @sql_command; 

        -- Reconstruimos si la fragmentacion supera el 30%, esto es para la bd. 
        SET @sql_command = N' USE ['+ @database_name + N'];
            DECLARE @TableName NVARCHAR(128);
            DECLARE @IndexName NVARCHAR(128);
            DECLARE @ReorganizeSQL NVARCHAR(MAX);

            DECLARE IndexCursor CURSOR FOR
            SELECT 
                t.name AS TableName,
                i.name AS IndexName
            FROM 
                sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, ''LIMITED'') d
                JOIN sys.indexes i ON d.object_id = i.object_id AND d.index_id = i.index_id
                JOIN sys.tables t ON t.object_id = d.object_id
            WHERE 
                d.avg_fragmentation_in_percent > 30 -- Fragmentación alta
                AND i.type > 0 -- Ignorar índices tipo HEAP
                AND i.is_disabled = 0; -- Ignorar índices deshabilitados
            
            OPEN IndexCursor;
            FETCH NEXT FROM IndexCursor INTO @TableName, @IndexName;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                SET @ReorganizeSQL = ''ALTER INDEX ['' + @IndexName + ''] ON ['' + @TableName + ''] REBUILD;'';
                EXEC sp_executesql @ReorganizeSQL;
                FETCH NEXT FROM IndexCursor INTO @TableName, @IndexName;
            END;

            CLOSE IndexCursor;
            DEALLOCATE IndexCursor;
        ';
        EXEC sp_executesql @sql_command;

       
        PRINT N' Plan semanal completado para: ' + @database_name;
    END TRY

    BEGIN CATCH
        PRINT N' Error en la base de datos: ' + @database_name;
        PRINT N'Mensaje de error: ' + ERROR_MESSAGE();
        PRINT N'Numero de error: ' + CAST(ERROR_NUMBER() AS NVARCHAR);
        PRINT N'Linea del error: ' + CAST(ERROR_LINE() AS NVARCHAR);
    END CATCH;

    FETCH NEXT FROM db_cursor INTO @database_name;
END;

-- Cerrar y liberar el cursor
CLOSE db_cursor;
DEALLOCATE db_cursor;