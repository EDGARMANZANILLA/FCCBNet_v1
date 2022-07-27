using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class InventarioModel
    {


        public int IdCuentaBancaria { get; set; }
        public string NombreBanco { get; set; }
        public string Cuenta { get; set; }

        public string FormasDisponibles { get; set; }

        public string UltimoFolioInventario { get; set; }

        public string UltimoFolioUtilizado { get; set; }


        public Nullable<decimal> EstimadoMeses { get; set; }



    }
}