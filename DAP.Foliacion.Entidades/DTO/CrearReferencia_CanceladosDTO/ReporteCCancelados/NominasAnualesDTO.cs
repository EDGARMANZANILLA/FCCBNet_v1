using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados
{
    public class NominasAnualesDTO
    {
        public string Quincena { get; set; }
        public string Cheque { get; set; }
        public string Num { get; set; }
        public string NombreBenefirioCheque { get; set; }
        public string Deleg { get; set; }
        public decimal Liquido { get; set; }
        public string NombreNomina { get; set; }

    }
}
