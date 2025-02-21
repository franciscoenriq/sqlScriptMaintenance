using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtlReporteria.Models
{
    /*
     * Clase con la cual guardamos los datos del estado de las bd. 
     * Tenemos el nombre, el estado , el tipo de archivo y el tamaño de la bd y el tamaño maximo si que posee
     * Fecha de ejecucion y Hora de ejecucion representan a la hora que fueron consultados los demas datos. 
     */
    internal class DbState_Table
    {
        public int Id { get; set; }
        public string LogicDbName { get; set; }
        public string FileState { get; set; }
        public string TipoArchivo { get; set; }
        public int SizeMB { get; set; }
        public long TamanoMaximo { get; set; }
        public DateTime FechaEjecucion { get; set; }
        public TimeSpan HoraEjecucion { get; set; }
    }
}
