using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class SolicitudDetalleModel
    {
        public int NumeroMemo { get; set; }
        public string NombreBanco { get; set; }
        public string NumeroCuenta { get; set; }
        public int Cantidad { get; set; }
        public string FolioInicial { get; set; }
        public string FolioMuestra { get; set; }
        public string FechaSolicitud { get; set; }

    }
}