using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados
{
    public class CuentasBancariasAnualesDTO
    {
        public string NombreNomina { get; set; }
        public decimal SumaLiquido { get; set; }
        public int TotalRegistros { get; set; }
        public string NombreCuentaBanco { get; set; }
    }
}
