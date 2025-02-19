using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EtlReporteria.Models; 
namespace EtlReporteria.Transformations
{
    internal class BackupHistoryTransformation
    {
        public IEnumerable<BackupHistory_Table> Transform_Backup_History(IEnumerable<BackupHistory_Table> rows)
        {
            foreach (var row in rows)
            {
                // Calcular la duración en minutos
                row.Duracion_minutos = (row.BackupFinishDate - row.BackupStartDate).TotalMinutes;

             
                yield return row;
            }
        }

    }
}

