DECLARE @ruta NVARCHAR(255);
DECLARE @ruta_completa NVARCHAR(255);
DECLARE @database_name NVARCHAR(255);
DECLARE @sql_command NVARCHAR(MAX);
-- definimos varianles para medir el tiempo. 
DECLARE @start_time DATETIME; 
DECLARE @end_time DATETIME; 
DECLARE @elapsed_time DECIMAL(10,2);
-------------------------------
DECLARE @time_backup DECIMAL(10,2) = 0; 
DECLARE @time_reoganizacion DECIMAL(10,2) = 0; 
DECLARE @time_actualizar_estadisticas DECIMAL(10,2) = 0;
DECLARE @time_reconstruccion DECIMAL(10,2) = 0;
-- Definimos Ruta Backup 
SET @ruta = N'C:\backup-softland\';

-- Lista de bases de datos a procesar 
-- en insert into coloque todas las bd quiera que sean parte del plan de mantenimiento
DECLARE @databases TABLE (database_name NVARCHAR(255));
INSERT INTO @databases VALUES 
 (N'VANTRUST');

-- Iteramos sobre las bases de datos que queremos mantener
DECLARE db_cursor CURSOR FOR 
SELECT database_name FROM @databases;

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @database_name;

WHILE @@FETCH_STATUS = 0 
BEGIN
    BEGIN TRY 
        -- Crear ruta completa
        SET @ruta_completa = @ruta + @database_name + N'-Backup.bak';
        
        -- Realizar el backup
        SET @start_time = SYSDATETIME();
        SET @sql_command = N'BACKUP DATABASE [' + @database_name + N'] TO DISK = N''' + @ruta_completa + N''' WITH  STATS = 100';
        EXEC sp_executesql @sql_command;

        SET @end_time = SYSDATETIME(); 
        SET @elapsed_time = DATEDIFF(SECOND, @start_time, @end_time);
        SET @time_backup = @time_backup + @elapsed_time; 

        -------------------------------------------------------------------------------------------
        SET @start_time = SYSDATETIME();
        -- Reorganizamos índices que tengan una fragmentacion entre el 5 y el 30%
        PRINT N'Reorganizando indices para la base de datos: ' + @database_name;

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
            WHERE d.avg_fragmentation_in_percent BETWEEN 5 AND 30
            AND i.type_desc NOT IN (''HEAP'', ''XML'', ''FULLTEXT'')
            AND i.is_disabled = 0;

            OPEN IndexCursor;
            FETCH NEXT FROM IndexCursor INTO @SchemaName, @TableName, @IndexName;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                BEGIN TRY 
                    SET @ReorganizeSQL = ''ALTER INDEX ['' + @IndexName + ''] ON ['' + @SchemaName + ''].['' + @TableName + ''] REORGANIZE  ;'';
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
        PRINT N'Indices reorganizados para la base de datos: ' + @database_name;
        SET @end_time = SYSDATETIME();
        SET @elapsed_time = DATEDIFF(SECOND, @start_time, @end_time);
        SET @time_reoganizacion = @time_reoganizacion + @elapsed_time; 
        ----------------------------------------------------------------
        -- Actualizamos estadísticas de todas las tablas
        SET @start_time = SYSDATETIME();
        SET @sql_command = N'
            USE [' + @database_name + N'];
            DECLARE @sql_update NVARCHAR(MAX);
            DECLARE @TableName NVARCHAR(255);

            DECLARE TableCursor CURSOR FOR
            SELECT QUOTENAME(SCHEMA_NAME(t.schema_id)) + ''.'' + QUOTENAME(t.name) AS TableName
            FROM sys.tables t
            WHERE t.is_ms_shipped = 0; -- Excluir tablas del sistema

            OPEN TableCursor;
            FETCH NEXT FROM TableCursor INTO @TableName;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                SET @sql_update = ''UPDATE STATISTICS '' + @TableName + '' WITH FULLSCAN;'';
                EXEC sp_executesql @sql_update;
                FETCH NEXT FROM TableCursor INTO @TableName;
            END;

            CLOSE TableCursor;
            DEALLOCATE TableCursor;
            ';

        EXEC sp_executesql @sql_command; 
        SET @end_time = SYSDATETIME(); 
        SET @elapsed_time = DATEDIFF(SECOND, @start_time, @end_time);
        SET @time_actualizar_estadisticas = @time_actualizar_estadisticas + @elapsed_time; 

        PRINT N'Estadisticas actualizadas para la base de datos: ' + @database_name;

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
        --EXEC sp_executesql @sql_command;
        SET @end_time = SYSDATETIME();
        SET @elapsed_time = DATEDIFF(SECOND, @start_time, @end_time);
        SET @time_reconstruccion = @time_reconstruccion + @elapsed_time; 


        PRINT N'Backup completado para la base de datos: ' + @database_name;
    END TRY

    BEGIN CATCH
        PRINT N'Error en la base de datos: ' + @database_name;
        PRINT N'Mensaje de error: ' + ERROR_MESSAGE();
        PRINT N'Número de error: ' + CAST(ERROR_NUMBER() AS NVARCHAR);
        PRINT N'Línea del error: ' + CAST(ERROR_LINE() AS NVARCHAR);
    END CATCH;

    FETCH NEXT FROM db_cursor INTO @database_name;
END;

PRINT N'Tiempo transcurrido para actualizar estadisticas(segundos) : ' + CAST(@time_actualizar_estadisticas AS NVARCHAR) ;
PRINT N'Tiempo transcurrido para reoganizar indices(segundos) : ' + CAST(@time_reoganizacion AS NVARCHAR) ;
--PRINT N'Tiempo transcurrido para reconstruir indices(segundos):' + CAST(@time_reconstruccion AS NVARCHAR); 
PRINT N'Tiempo transcurrido para backup(segundos) : ' + CAST(@time_backup AS NVARCHAR) ;
PRINT N'Tiempo total transcurrido (segundos): ' + CAST(@time_actualizar_estadisticas + @time_reoganizacion + @time_backup AS NVARCHAR);

-- Cerrar y liberar el cursor
CLOSE db_cursor;
DEALLOCATE db_cursor;

