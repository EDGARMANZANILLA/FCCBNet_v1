using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class DatosBitacoraParaCheque
    {
        public string Quincena { get; set; }
        public string Nomina { get; set; }
        public string Comentario { get; set; }
        public int Id_nom { get; set; }
        public string An { get; set; }
        public bool Importado { get; set; }
    }
}
