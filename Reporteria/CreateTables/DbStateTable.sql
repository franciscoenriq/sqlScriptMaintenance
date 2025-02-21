CREATE TABLE DbStateHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Logic_db_name NVARCHAR(128) NOT NULL,
    File_State NVARCHAR(60) NOT NULL,
    Tipo_Archivo NVARCHAR(20) NOT NULL,
    Size_MB INT NOT NULL,
    Tamano_maximo BIGINT NOT NULL,
    Fecha_ejecucion DATE NOT NULL,
    Hora_ejecucion TIME(0) NOT NULL
);
