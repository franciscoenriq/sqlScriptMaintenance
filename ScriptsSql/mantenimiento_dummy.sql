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
 (N'VNT'),
 (N'VANTRUST'),
 (N'SANF');

-- Iteramos sobre las bases de datos que queremos mantener
DECLARE db_cursor CURSOR FOR 
SELECT database_name FROM @databases;

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @database_name;
