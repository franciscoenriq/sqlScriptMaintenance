CREATE TABLE BackupHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DatabaseName NVARCHAR(128) NOT NULL,
    BackupType NVARCHAR(50) NOT NULL,
    DeviceType NVARCHAR(50) NOT NULL,
    RecoveryModel NVARCHAR(50) NULL,
    CompatibilityLevel INT NULL,
    BackupStartDate DATETIME NOT NULL,
    BackupFinishDate DATETIME NOT NULL,
    Duracion_minutos INT NULL,
    LatestBackupLocation NVARCHAR(500) NULL,
    BackupSize BIGINT NULL,
    CompressedBackupSize BIGINT NULL,
    ServerName NVARCHAR(128) NULL
);
