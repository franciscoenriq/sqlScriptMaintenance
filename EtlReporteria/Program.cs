using EtlReporteria.DataAccess;
using EtlReporteria.Destinations;
using EtlReporteria.Transformations;
using EtlReporteria.Models;
using static EtlReporteria.Queries.Queries;
class Program
{
    static void Main()
    {
        string sourceConnectionString = "tu_connection_string_a_sql_origen";
        string destinationConnectionString = "tu_connection_string_a_sql_destino";

        var sourceConnection = new SqlServerConnection(sourceConnectionString);
        var extractor = new SqlServerExtractor(sourceConnection);
        var rows = extractor.Extract_BackupHistory_Data(GetQuery("SelectBackupHistory")); // Tu consulta aquí

        var transformer = new BackupHistoryTransformation();
        var transformedRows = transformer.Transform_Backup_History(rows);

        var destinationConnection = new SqlServerConnection(destinationConnectionString);
        var destination = new SqlServerDestination(destinationConnection);
        destination.InsertData(transformedRows);
    }
}