DECLARE @ruta NVARCHAR(255);
DECLARE @ruta_completa NVARCHAR(255);
DECLARE @database_name NVARCHAR(255);
DECLARE @database_namelog NVARCHAR(255);
DECLARE @sql_command NVARCHAR(MAX);
-- definimos varianles para medir el tiempo. 
DECLARE @start_time DATETIME; 
DECLARE @end_time DATETIME; 
DECLARE @elapsed_time DECIMAL(10,2);
--------------------------------------------
DECLARE @time_shrinkfile DECIMAL(10,2) = 0;
DECLARE @time_reconstruccion DECIMAL(10,2) = 0;
-- Lista de bases de datos a procesar 
DECLARE @databases TABLE (database_name NVARCHAR(255));
INSERT INTO @databases VALUES 
 (N'VNT'),
 (N'VANTRUST'),
 (N'SANF');

-- Iteramos sobre las bases de datos que queremos mantener
DECLARE db_cursor CURSOR FOR 
SELECT database_name FROM @databases;

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @database_name;

WHILE @@FETCH_STATUS = 0 
BEGIN
    BEGIN TRY 
        SET @start_time = SYSDATETIME();
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
        SET @sql_command = N'
            USE [' + @database_name+ N']
            ALTER DATABASE ['+ @database_name+ N']
            SET RECOVERY SIMPLE;
            DBCC SHRINKFILE(['+ @database_namelog+ N'],10);
            
            ALTER DATABASE ['+ @database_name+ N'] 
            SET RECOVERY FULL
        '
        EXEC sp_executesql @sql_command;

        SET @end_time = SYSDATETIME();
        SET @elapsed_time = DATEDIFF(SECOND, @start_time, @end_time);
        SET @time_shrinkfile = @time_shrinkfile + @elapsed_time; 
        ---------------------------------------------------------------------------------------------
        SET @start_time = SYSDATETIME();
        -- Reconstruimos si la fragmentacion supera el 30%, esto es para la bd. 
        SET @sql_command = N'
            USE ['+ @database_name + N'];
            DECLARE @SchemaName NVARCHAR(128);
            DECLARE @TableName NVARCHAR(128);
            DECLARE @IndexName NVARCHAR(128);
            DECLARE @ReorganizeSQL NVARCHAR(MAX);

            DECLARE IndexCursor CURSOR FOR
            SELECT 
                s.name AS SchemaName,
                t.name AS TableName,
                i.name AS IndexName
            FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, ''LIMITED'') d
            JOIN sys.indexes i ON d.object_id = i.object_id AND d.index_id = i.index_id
            JOIN sys.tables t ON t.object_id = d.object_id
            JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE d.avg_fragmentation_in_percent > 30
            AND i.type_desc NOT IN (''HEAP'', ''XML'', ''FULLTEXT'')
            AND i.is_disabled = 0;

            OPEN IndexCursor;
            FETCH NEXT FROM IndexCursor INTO @SchemaName, @TableName, @IndexName;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                BEGIN TRY 
                    SET @ReorganizeSQL = ''ALTER INDEX ['' + @IndexName + ''] ON ['' + @SchemaName + ''].['' + @TableName + ''] REBUILD  ;'';
                    EXEC sp_executesql @ReorganizeSQL;
                END TRY 
                BEGIN CATCH
                    PRINT ''ERROR en reorganización de índices: '' + ERROR_MESSAGE();
                END CATCH;
                FETCH NEXT FROM IndexCursor INTO @SchemaName, @TableName, @IndexName;
            END;

            CLOSE IndexCursor;
            DEALLOCATE IndexCursor;
        ';
        EXEC sp_executesql @sql_command;
        SET @end_time = SYSDATETIME();
        SET @elapsed_time = DATEDIFF(SECOND, @start_time, @end_time);
        SET @time_reconstruccion = @time_reconstruccion + @elapsed_time; 
        ----------------------------------------------------------------------------------------------------
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

PRINT N'Tiempo transcurrido para SHRINKFILE(segundos): ' + CAST(@time_shrinkfile AS NVARCHAR);
PRINT N'Tiempo transcurrido para RECONSTRUCCION(segundos): ' + CAST(@time_reconstruccion AS NVARCHAR);
PRINT N'Tiempo total transcurrido (segundos): ' + CAST(@time_shrinkfile + @time_reconstruccion AS NVARCHAR);
-- Cerrar y liberar el cursor
CLOSE db_cursor;
DEALLOCATE db_cursor;