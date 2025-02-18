using System;
using System.Configuration;
using System.Diagnostics;

string servidor_sql = ConfigurationManager.AppSettings["servidor_sql"]!; 
string usuario_sql = ConfigurationManager.AppSettings["usuario_sql"]!;
string contrasena_sql = ConfigurationManager.AppSettings["contrasena_sql"]! ;

string ruta_base = ConfigurationManager.AppSettings["ruta_base"]!; 
string archivoSql = ConfigurationManager.AppSettings["archivo_sql"]!;

string ruta_completa = Path.Combine(ruta_base, archivoSql);


// Determinar el nombre de la carpeta dependiendo del archivo SQL
string nombreCarpetaLog = archivoSql switch
{
    "backup_softland.sql" => "BackupSoftland_logs",
    "mantenimientos.sql" => "Mantenimiento_logs",
    _ => "Otros" // Para otros casos no previstos
};
// Ruta de la carpeta de logs
string ruta_logs = Path.Combine(ruta_base, "Logs", nombreCarpetaLog);

// Verificar si la carpeta existe
if (!Directory.Exists(ruta_logs))
{
    Directory.CreateDirectory(ruta_logs);
}

if (!File.Exists(ruta_completa))
{
    throw new FileNotFoundException($"El archivo SQL no existe en la ruta: {ruta_completa}");
}

// Nombre del archivo de log con timestamp
string archivoLog = Path.Combine(ruta_logs, $"log_{archivoSql}_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
string comando = $"sqlcmd -S {servidor_sql} -U {usuario_sql} -P {contrasena_sql} -i \"{ruta_completa}\"";

try
{
    // Crear un proceso para ejecutar el comando
    ProcessStartInfo startInfo = new ProcessStartInfo("cmd", "/C " + comando)
    {
        RedirectStandardOutput = true,   // Redirigir la salida estándar para leerla
        RedirectStandardError = true,   //  Redirigir errores
        UseShellExecute = false,       //   No usar la shell del sistema
        CreateNoWindow = true         //    No mostrar la ventana de la CMD
    };

    // Ejecutar el proceso
    using (Process process = Process.Start(startInfo))
    {
        // Capturar y mostrar la salida del comando de manera segura
        string output = process.StandardOutput?.ReadToEnd() ?? string.Empty;
        string error = process.StandardError?.ReadToEnd() ?? string.Empty;

        // Esperar a que el proceso termine
        process.WaitForExit();

        // Escribir la salida en el archivo de log
        using (StreamWriter logWriter = new StreamWriter(archivoLog, true))
        {
            logWriter.WriteLine($"Fecha y hora: {DateTime.Now}");
            logWriter.WriteLine("Salida del comando:");
            logWriter.WriteLine(output);

            if (!string.IsNullOrEmpty(error))
            {
                logWriter.WriteLine("Errores:");
                logWriter.WriteLine(error);
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ocurrió un error al ejecutar el comando: {ex.Message}");
    Console.WriteLine($"Detalle del error: {ex.StackTrace}");
}
        
