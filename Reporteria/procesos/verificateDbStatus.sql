-- este script consulta si alguna db 
DECLARE @DatabaseName NVARCHAR(128),
        @StateDesc NVARCHAR(60),
        @Body NVARCHAR(MAX) = 'Las siguientes bases de datos no están en línea:' + CHAR(13) + CHAR(10);

DECLARE db_cursor CURSOR FOR
SELECT name, state_desc
FROM sys.databases
WHERE state_desc != 'ONLINE';

OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @DatabaseName, @StateDesc;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Body = @Body + @DatabaseName + ' - Estado: ' + @StateDesc + CHAR(13) + CHAR(10);
    FETCH NEXT FROM db_cursor INTO @DatabaseName, @StateDesc;
END

CLOSE db_cursor;
DEALLOCATE db_cursor;

-- Enviar un correo si hay bases de datos fuera de línea
IF LEN(@Body) > 50 -- Si el cuerpo tiene contenido, significa que hay bases de datos fuera de línea
BEGIN
    EXEC msdb.dbo.sp_send_dbmail
        @profile_name = 'SQLServer Alerts', 
        @recipients = 'tu_correo@dominio.com', 
        @subject = 'Alerta: Bases de Datos Fuera de Línea',
        @body = @Body;
END
