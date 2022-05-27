using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO
{
    public class InventarioValidaFoliosDTO
    {
        public int IdVirtual { get; set; }
        public int Id { get; set; }
        public int IdContenedor { get; set; }
        public int NumFolio { get; set; }
        public string Incidencia { get; set; }
        public string Empleado { get; set; }
        
        public string FechaIncidencia { get; set; }


    }
}
