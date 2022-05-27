using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados
{
    public class PensionAlimenticiaDTO
    {
        public string Ramo { get; set; }
        public string Unidad { get; set; }
        public string Quincena { get; set; }
        public string Num_che { get; set; }
        public string NumEmpleado { get; set; }
        public string NombreBeneficiario { get; set; }
        public string Delegacion { get; set; }
        public decimal Liquido { get; set; }
    }
}
