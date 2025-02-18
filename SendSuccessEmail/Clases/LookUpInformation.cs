using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration; 
namespace SendSuccessEmail.Clases
{
    internal class LookUpInformation
    {
        private string logPath_backup;
        private string logPath_mantenimiento;
        public List<string> failedDB;
        public Dictionary<string,string> successDB; 
        public Dictionary <string,string> failedDBMaintenance;

        public LookUpInformation()
        {
            logPath_backup = SendSuccessEmail.Configuracion.Default.logPath_backup;
            logPath_mantenimiento = SendSuccessEmail.Configuracion.Default.logPath_mantenimiento;
            failedDB = new List<string>();
            successDB = new Dictionary<string, string>();
            failedDBMaintenance = new Dictionary<string, string>();
        }
        // ------------------------------------------- funciones para leer el log de backups ------------------------------------
        private string GetLatestLogFile_backup()
        {
            if (!Directory.Exists(logPath_backup))
            {
                return null;
            }
            var files = new DirectoryInfo(logPath_backup).GetFiles("*.txt").OrderByDescending(f =>f.LastWriteTime).ToList();
            return files.Count > 0 ? files[0].FullName:null;
        }

        private void ReadLogFile_backup(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            string currentDatabase = "";

            foreach (string line in lines)
            {
                if (line.StartsWith("Error en la base de datos:"))
                {
                    currentDatabase = line.Substring(line.IndexOf(":") + 1).Trim();
                    failedDB.Add(currentDatabase);
                }
                else if (line.StartsWith("Backup completado para la base de datos:"))
                {
                    int indexStart = line.IndexOf(":") + 1;
                    int indexFinal = line.IndexOf(" se demoro: ");

                    string db = line.Substring(indexStart, indexFinal - indexStart).Trim();
                    string tiempo = line.Substring(indexFinal + 12).Trim();

                    successDB[db] = tiempo; 
                }
                else
                {

                }
            }
        }

        public void ProcessLatestLogFile_backup()
        {
            string latestLogFile = GetLatestLogFile_backup();
            if (string.IsNullOrEmpty(latestLogFile))
            {
                Console.WriteLine("No se encontraron archivos de log.");
                return;
            }
            Console.WriteLine("procesando archivo:" + latestLogFile);
            ReadLogFile_backup(latestLogFile);
        }

        // ------------------------------------------- funciones para leer el log de mantenimiento ------------------------------------

        private string GetLatestLogFile_mantenimiento()
        {
            if (!Directory.Exists(logPath_mantenimiento))
            {
                return null;
            }
            var files = new DirectoryInfo(logPath_mantenimiento).GetFiles("*.txt").OrderByDescending(f => f.LastWriteTime).ToList();
            return files.Count > 0 ? files[0].FullName : null;
        }
        private void ReadLogFile_mantenimiento(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            string currentDatabase = "";

            foreach (string line in lines)
            {
                if (line.StartsWith("SHRINKFILE realizado correctamente para el log de:"))
                {
                    int indexStart = line.IndexOf(":") + 1;
                    int indexFinal = line.IndexOf(" se demoro: ");
                    currentDatabase = line.Substring(indexStart, indexFinal- indexStart).Trim();
                    string db_subplan = currentDatabase + "_SHRINKFILE";
                    string tiempo = line.Substring(indexFinal + 12).Trim();
                    successDB[db_subplan]= tiempo;

                }
                else if (line.StartsWith("Indices reorganizados para la base de datos:"))
                {

                    int indexStart = line.IndexOf(":") + 1;
                    int indexFinal = line.IndexOf(" se demoro: ");
                    currentDatabase =line.Substring(indexStart, indexFinal - indexStart).Trim();
                    string db_subplan = currentDatabase + "_REORGANIZE";
                    string tiempo = line.Substring(indexFinal + 12).Trim();
                    successDB[db_subplan] = tiempo;
                }
                else if (line.StartsWith("Estadisticas actualizadas para la base de datos:"))
                {

                    int indexStart = line.IndexOf(":") + 1;
                    int indexFinal = line.IndexOf(" se demoro: ");
                    currentDatabase = line.Substring(indexStart, indexFinal - indexStart).Trim();
                    string db_subplan = currentDatabase + "_UPDATE_STADISTICS";
                    string tiempo = line.Substring(indexFinal + 12).Trim();
                    successDB[db_subplan] = tiempo;
                }
                else if (line.StartsWith("Indices reconstruidos para la base de datos:"))
                {

                    int indexStart = line.IndexOf(":") + 1;
                    int indexFinal = line.IndexOf(" se demoro: ");
                    currentDatabase = line.Substring(indexStart, indexFinal - indexStart).Trim();
                    string db_subplan = currentDatabase + "_REBUILD";
                    string tiempo = line.Substring(indexFinal + 12).Trim();
                    successDB[db_subplan] = tiempo;
                }
                else if (line.StartsWith("ERROR:"))
                {
                    int indexStart = line.IndexOf("Fallo en") + 9;
                    int indexFinal = line.IndexOf(" para ");
                    string error_type = line.Substring(indexStart, indexFinal - indexStart).Trim();
                    int indexStart_detalle = line.IndexOf(". Detalle: ");
                    currentDatabase = line.Substring(indexFinal + 6, indexStart_detalle - indexFinal).Trim(); 

                    string db_subplan_error = currentDatabase + error_type;
                    string error = line.Substring(indexStart_detalle + 11).Trim();
                    failedDBMaintenance[db_subplan_error] = error;
                }
                else
                {

                }
            }
        }
        public void ProcessLatestLogFile_mantenimiento()
        {
            string latestLogFile = GetLatestLogFile_mantenimiento();
            if (string.IsNullOrEmpty(latestLogFile))
            {
                Console.WriteLine("No se encontraron archivos de log.");
                return;
            }
            Console.WriteLine("procesando archivo:" + latestLogFile);
            ReadLogFile_mantenimiento(latestLogFile);
        }
    }
}
