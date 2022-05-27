using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class InventarioAsignarAjustarModel
    {
        public string NombrePersona { get; set; }
        public string NombreBanco { get; set; }
        public string Cuenta { get; set; }
        public string NumeroOrden { get; set; }
        public int Contenedor { get; set; }
        public int FoliosAsignados { get; set; }
        public string FolioInicial { get; set; }
        public string FolioFinal { get; set; }
        public System.DateTime FechaAsignacion { get; set; }
    }
}