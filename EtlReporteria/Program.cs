using EtlReporteria.DataAccess;
using EtlReporteria.Destinations;
using EtlReporteria.Transformations;
using EtlReporteria.Models;
using EtlReporteria.Extractor;
using static EtlReporteria.Queries.Queries;
class Program
{
    static void Main()
    {
        // Fuente de datos a la que vamos a consultar 
        string servidorSql_target = EtlReporteria.Properties.Settings_etl.Default.ServidorSqlTarget;
        string usuarioSql_target = EtlReporteria.Properties.Settings_etl.Default.UsuarioSqlTarget;
        string contrasenaSql_target = EtlReporteria.Properties.Settings_etl.Default.ContrasenaSqlTarget;
        // en este caso no necesitamos especificar la db porque consultaremos sobre bd msdb(es de la master) 

        // Donde los vamos a guardar
        string servidorSql_destino = EtlReporteria.Properties.Settings_etl.Default.ServidorSqlDestino;
        string usuarioSql_destino = EtlReporteria.Properties.Settings_etl.Default.UsuarioSqlDestino;
        string contrasenaSql_destino = EtlReporteria.Properties.Settings_etl.Default.ContrasenaSqlDestino;
        string baseDatos_destino = EtlReporteria.Properties.Settings_etl.Default.DatabaseDestino;

        //Cadenas de conexion 
        string cadenaConexion_target = $"Server={servidorSql_target};User Id={usuarioSql_target};Password={contrasenaSql_target};TrustServerCertificate=True;";
        string cadenaConexion_destino = $"Server={servidorSql_destino};User Id={usuarioSql_destino};Password={contrasenaSql_destino};Initial Catalog={baseDatos_destino};TrustServerCertificate=True;";


        var sourceConnection = new SqlServerConnection(cadenaConexion_target);
        var extractor = new SqlServerExtractor(sourceConnection);

        var rows_BackupHisotry = extractor.Extract_BackupHistory_Data(GetQuery("SelectBackupHistory"));
        var rows_JobsHistory = extractor.Extract_JobsHistory_Data(GetQuery("SelectJobsHistory"));
        var transformer = new BackupHistoryTransformation();
        var transformedRows = transformer.Transform_Backup_History(rows_BackupHisotry);

        var destinationConnection = new SqlServerConnection(cadenaConexion_destino);
        var destination = new SqlServerDestination(destinationConnection);
        destination.InsertData_backupHistory(transformedRows);
        destination.InsertData_JobsHiistory(rows_JobsHistory);
    }
}