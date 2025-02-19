using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtlReporteria.Models
{
    /*
     * Clase que representa la tabla backup_history en la cual guardamos la informacion relevante de como han salido los backups en el servidor.  
     */
    public class BackupHistory_Table
    {
        public string DatabaseName { get; set; }
        public string BackupType { get; set; }
        public string DeviceType { get; set; }
        public string RecoveryModel { get; set; }
        public int? CompatibilityLevel { get; set; }
        public DateTime BackupStartDate { get; set; }
        public DateTime BackupFinishDate { get; set; }
        public TimeSpan Duracion_minutos { get; set; }
        public string LatestBackupLocation { get; set; }
        public double BackupSizeMB { get; set; }
        public double CompressedBackupSizeMB { get; set; }
        public string ServerName { get; set; }

    }
}
