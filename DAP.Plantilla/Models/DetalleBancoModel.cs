using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class DetalleBancoModel
    {
        public int IdTblDetalle { get; set; }
        public string Banco { get; set; }
        public string Cuenta { get; set; }
        public string NumeroOrden { get; set; }
        public int NumeroContenedor { get; set; }
        public int NumeroFolio { get; set; }
        public string Incidencia { get; set; }
        public int TipoNumeroIncidencia { get; set; }
        public string NombreEmpleado { get; set; }
        public string FechaIncidencia { get; set; }
        public string FechaAsignacionExterna { get; set; }


    }
}