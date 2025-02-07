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
 (N'SANF');

-- Iteramos sobre las bases de datos que queremos mantener
DECLARE db_cursor CURSOR FOR 
SELECT database_name FROM @databases;