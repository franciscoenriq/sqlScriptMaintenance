using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtlReporteria.Models
{
    public class BackupHistory_Table
    {
        public string DatabaseName { get; set; }
        public string BackupType { get; set; }
        public string DeviceType { get; set; }
        public string RecoveryModel { get; set; }
        public int? CompatibilityLevel { get; set; }
        public DateTime BackupStartDate { get; set; }
        public DateTime BackupFinishDate { get; set; }
        public Double Duracion_minutos { get; set; }
        public string LatestBackupLocation { get; set; }
        public long BackupSizeMB { get; set; }
        public long CompressedBackupSizeMB { get; set; }
        public string ServerName { get; set; }

    }
}
