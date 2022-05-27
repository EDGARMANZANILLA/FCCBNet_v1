using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.FoliacionModels
{
    public class DetallesBancoInventario
    {
        public string NombreBanco { get; set; }

        public  string Cuenta { get; set; }
        public int FormasDisponibles { get; set; }

        public string UltimoFolioUsado { get; set; }
    }
}