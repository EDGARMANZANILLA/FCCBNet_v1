using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class BancosConChequeraDTO
    {

        public string  NombreBanco { get; set; }
        public string  Cuenta { get; set; }
        public int  FormasDisponiblesInventario { get; set; }
        public string  UltimoFolioUtilizadoInventario  { get; set; }

    }
}
