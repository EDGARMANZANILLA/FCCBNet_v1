using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO
{
   public class InventarioDetalleDTO
    {
        public int Id { get; set; }
        public int IdContenedor { get; set; }
        public int NumFolio { get; set; }
        public Nullable<int> IdIncidencia { get; set; }

    }
}
