DECLARE @ruta NVARCHAR(255);
DECLARE @ruta_completa NVARCHAR(255);
DECLARE @database_name NVARCHAR(255);
DECLARE @sql_command NVARCHAR(MAX);

-- Definimos Ruta Backup 
SET @ruta = N'C:\backup\';

-- Lista de bases de datos a procesar 
DECLARE @databases TABLE (database_name NVARCHAR(255));
INSERT INTO @databases VALUES 
    (N'beneficiarios'),
    (N'SCL_BD');

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
        PRINT N'Procesando Base de Datos: ' + @database_name;

        -- Actualizamos estadisticas de todas las tablas
        SET @sql_command = N'USE [' + @database_name + N'];
                             EXEC sp_MSforeachtable @command1 = N''UPDATE STATISTICS ? WITH FULLSCAN'';';
        EXEC sp_executesql @sql_command; 

        PRINT N'Estadisticas actualizadas para la base de datos: ' + @database_name;

        -- Reorganizamos los indices de todas las tablas
        SET @sql_command = N'USE [' + @database_name + N'];
                             EXEC sp_MSforeachtable @command1 = N''ALTER INDEX ALL ON ? REORGANIZE'';';
        EXEC sp_executesql @sql_command;

        PRINT N'indices reorganizados para la base de datos: ' + @database_name;

        -- Realizar el backup
        SET @sql_command = N'BACKUP DATABASE [' + @database_name + N'] TO DISK = N''' + @ruta_completa + N''' WITH COMPRESSION, STATS = 100';
        EXEC sp_executesql @sql_command;

        PRINT N' Backup completado para la base de datos: ' + @database_name;
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
