using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPD
{
    public class RegistrosCuotasPatronalesDTO
    {
        public string NombreNomina { get; set; }
        public string NumeroRamo { get; set; }
        public string DescripRamo { get; set; }
        public string DescripUnidad { get; set; }

        public int Cantidad { get; set; }
        public decimal MontoPositivo { get; set; }
   
        public int TotalRamo_Cantidad { get; set; }
        public decimal TotalRamo_Monto { get; set; }

    }
}
