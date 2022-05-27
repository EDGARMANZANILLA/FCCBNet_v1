using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO
{
    public class DatosAnIPD_DTO
    {
        public string Num { get; set; }
        public string Partida { get; set; }
        public string FolioCfdi { get; set; }
        public string Cve_Presup { get; set; }
        public decimal Patronal_ISSTE { get; set; }
        public decimal Patronal_ISSSTECAM { get; set; }
        public string TipoPago { get; set; }
        public string Cla_Pto { get; set; }
    }
}
