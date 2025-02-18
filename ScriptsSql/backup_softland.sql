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

-- Definimos Ruta Backup 
SET @ruta = N'C:\backup-softland\';
------------------------ NO MODIFICAR--------
-- Lista de bases de datos a procesar 
-- en insert into coloque todas las bd quiera que sean parte del plan de mantenimiento
DECLARE @databases TABLE (database_name NVARCHAR(255));
INSERT INTO @databases VALUES 
 (N'SANF'),
 (N'AGFVNT'),
 (N'NOTASVNT'),
 (N'SERVFINANCIEROS');

-- Iteramos sobre las bases de datos que queremos mantener
DECLARE db_cursor CURSOR FOR 
SELECT database_name FROM @databases;
------------------- ACA PUEDE EMPEZAR A MODIFICAR NUEVAMENTE -------------
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

PRINT N'Tiempo transcurrido para backup(segundos) : ' + CAST(@time_backup AS NVARCHAR) ;
-- Cerrar y liberar el cursor
CLOSE db_cursor;
DEALLOCATE db_cursor;
