using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPD
{
    public class TotalesGeneralesPorConceptoDTO
    {
        public bool EsPercepcion { get; set; }
        public string RU { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public decimal MontoPositivo { get; set; }
        public decimal MontoNegativo { get; set; }
    }
}
