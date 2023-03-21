using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPD
{
    public class ResumenGeneralXNominaDTO
    {
        public int IdVirtual { get; set; }
        public string NombreDescripcionNomina { get; set; }
        public int PP_Regs { get; set; }
        public decimal PP_Monto { get; set; }
        public int CP_Regs { get; set; }
        public decimal CP_Monto { get; set; }
        public int Total_Regs { get; set; }
        public decimal Total_Monto { get; set; }
    }
}
