using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO.FoliacionPagomatico
{
    public class ActualizarFoliacionPagomaticoTblPagoDTO
    {
        public int IdPago { get; set; }
        public int IdTbl_CuentaBancaria_BancoPagador { get; set; }
        public int FolioCheque { get; set; }
        public string Integridad_HashMD5 { get; set; }
    }
}
