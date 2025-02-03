CREATE TABLE BackupReport (
    report_id INT IDENTITY(1,1) PRIMARY KEY,
    database_name NVARCHAR(255),
    backup_start_date DATETIME,
    backup_finish_date DATETIME,
    backup_size BIGINT,
    backup_type CHAR(1),  -- 'D' = Completo, 'I' = Diferencial, 'L' = Log
    is_copy_only BIT,
    capture_date DATETIME DEFAULT GETDATE()  -- Fecha en que se captura el dato
);