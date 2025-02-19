using EtlReporteria.DataAccess;
using EtlReporteria.Destinations;
using EtlReporteria.Transformations;
using EtlReporteria.Models;
using static EtlReporteria.Queries.Queries;
class Program
{
    static void Main()
    {
        // Fuente de datos a la que vamos a consultar 
        string servidorSql_target = EtlReporteria.Properties.Settings1.Default.ServidorSqlTarget;
        string usuarioSql_target = EtlReporteria.Properties.Settings1.Default.UsuarioSqlTarget;
        string contrasenaSql_target = EtlReporteria.Properties.Settings1.Default.ContrasenaSqlTarget;
        // en este caso no necesitamos especificar la db porque consultaremos sobre bd msdb(es de la master) 

        // Donde los vamos a guardar
        string servidorSql_destino = EtlReporteria.Properties.Settings1.Default.ServidorSqlDestino;
        string usuarioSql_destino = EtlReporteria.Properties.Settings1.Default.UsuarioSqlDestino;
        string contrasenaSql_destino = EtlReporteria.Properties.Settings1.Default.ContrasenaSqlDestino;
        string baseDatos_destino = EtlReporteria.Properties.Settings1.Default.DatabaseDestino;

        //Cadenas de conexion 
        string cadenaConexion_target = $"Server={servidorSql_target};User Id={usuarioSql_target};Password={contrasenaSql_target};TrustServerCertificate=True;";
        string cadenaConexion_destino = $"Server={servidorSql_destino};User Id={usuarioSql_destino};Password={contrasenaSql_destino};Initial Catalog={baseDatos_destino};TrustServerCertificate=True;";


        var sourceConnection = new SqlServerConnection(cadenaConexion_target);
        var extractor = new SqlServerExtractor(sourceConnection);
        var rows = extractor.Extract_BackupHistory_Data(GetQuery("SelectBackupHistory")); 

        var transformer = new BackupHistoryTransformation();
        var transformedRows = transformer.Transform_Backup_History(rows);

        var destinationConnection = new SqlServerConnection(cadenaConexion_destino);
        var destination = new SqlServerDestination(destinationConnection);
        destination.InsertData_backupHistory(transformedRows);
    }
}