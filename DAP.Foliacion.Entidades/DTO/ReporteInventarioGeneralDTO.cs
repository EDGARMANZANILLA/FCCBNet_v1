using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO
{
   public class ReporteInventarioGeneralDTO
   {
        public string NombreBanco { get; set; }
        public string Cuenta { get; set; }
        public int FolioInicialExistente { get; set; }
        public int FolioFinalExistente { get; set; }
        public int TotalFormasPago { get; set; }
        public string ConsumoMensualAproximado { get; set; }
        public string SolicitarFormas { get; set; }


    }
}
