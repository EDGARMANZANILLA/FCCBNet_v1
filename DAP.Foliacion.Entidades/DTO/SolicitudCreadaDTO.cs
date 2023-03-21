using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO
{
   public class SolicitudCreadaDTO
   {
        public int Id { get; set; }
        public int IdBanco { get; set; }
        public string cadenaNombreBanco { get; set; }
        public string cuentaBanco { get; set; }
        public string cantidadFormas { get; set; }
        public string fInicial { get; set; }
        public string FolioMuestra { get; set; }
        public string UsoAproximadoMeses { get; set; }
    }
}
