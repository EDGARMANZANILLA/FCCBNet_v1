using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO
{
    public class DatosAdeducionesIPD_DTO
    {
        public int IdVirtualDeduccion { get; set; }
        public string Cla_dedu { get; set; }
        public decimal Monto { get; set; }
        public string Cvegasto { get; set; }
        public string BENEF { get; set; }

        /***    ESTE MONTO SOLO ES PARA LLEVAR EL MONTO INICIAL REAL DE LA DEDUCCION   ***/
        public decimal MontoReal { get; set; }

    }
}
