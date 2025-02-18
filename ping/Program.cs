using System.Configuration;
using Microsoft.Extensions.Configuration;
using ping;



// Configurar acceso al archivo appsettings.json
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory) // Ruta base del proyecto
    .AddJsonFile("appsettings.json") // Archivo de configuración
    .Build();
string excel_path = config["excel_path"]!;

var ips = IpReader.ReadIpFromExcel(excel_path);

Console.WriteLine(ips);    