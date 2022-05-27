using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO
{
    public class DetallesRegistrosDentroReferenciaDTO
    {
        public int IdVirtual { get; set; }
        public int Id { get; set; }
        public string EsPenA { get; set; }
        public string Deleg { get; set; }
        public string Num { get; set; }
        public string Beneficiario { get; set; }
        public int FolioCheque { get; set; }
        public decimal Liquido { get; set; }
        public int Id_Nom { get; set; }
        public string Nomina { get; set; }
        public int Quincena { get; set; } 
        public string Referencia { get; set; } 
        public string CuentaPagadora { get; set; }
        public string BancoPagador { get; set; }

    }
}
