
using System; 
using System.Diagnostics; 
using System.IO; 

namespace sqlMaintenance{
    internal class Program{
        static void Main(string[] args){

            string servidor_sql = "192.168.1.101\\GPICB";
            string usuario_sql = "falmeida_uat";
            string contrasena_sql = "Sys.aqswfa";

            string ruta_base = @"C:\Users\falmeida\Documents\querysautomaticas\";
            string archivoSql = "mantenimiento_diario.sql"; 

            string ruta_completa = Path.Combine(ruta_base,archivoSql);
            string ruta_logs = Path.Combine(ruta_base,"Logs");
            
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
            string archivoLog = Path.Combine(ruta_logs, $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
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
            catch(Exception ex){
                 Console.WriteLine($"Ocurrió un error al ejecutar el comando: {ex.Message}");
            }
        }
    }
}

