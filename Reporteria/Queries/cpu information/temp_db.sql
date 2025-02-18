USE tempdb;
GO

SELECT 
    name AS FileName,
    physical_name AS FilePath,
    type_desc AS FileType,
    size * 8 / 1024 AS SizeMB
FROM sys.master_files
WHERE database_id = DB_ID('tempdb');