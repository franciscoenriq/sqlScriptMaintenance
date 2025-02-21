using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtlReporteria.Models
{
    public class JobsHistory_Table
    {
        public string JobName { get; set; }              // Nombre del job
        public bool Enabled { get; set; }                 // Estado habilitado (1=habilitado, 0=deshabilitado)
        public int CategoryId { get; set; }               // ID de la categoría
        public string CategoryName { get; set; }          // Nombre de la categoría
        public string StepName { get; set; }              // Nombre del paso del job
        public int SqlSeverity { get; set; }              // Severidad SQL
        public string Message { get; set; }               // Mensaje del job
        public int RunStatus { get; set; }                // Estado de ejecución (0=Failed, 1=Succeeded, 2=Retry, 3=Canceled)
        public string RunStatusDescription { get; set; }  // Descripción del estado (Failed, Succeeded, etc.)
        public DateTime RunDate { get; set; }             // Fecha de ejecución del job
        public TimeSpan RunTime { get; set; }             // Hora de ejecución del job (tipo TIME)
        public TimeSpan RunDuration { get; set; }         // Duración (tipo TIME)
    }
}
