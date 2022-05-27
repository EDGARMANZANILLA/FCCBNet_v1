using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class ReporteEmpleadosNominaXDelegacionFoliacion
    {
        public string Nomina { get; set; }
        public string Id_nom { get; set; }
        public string Coment { get; set; }
        public string Adicional { get; set; }
        public string RutaNomina { get; set; }
        public int Confianza { get; set; }
        public int Sindicalizado { get; set; }
        public int Otros { get; set; }
        public string Delegacion { get; set; }
        

    }
}
