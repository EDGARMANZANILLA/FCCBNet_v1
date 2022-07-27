using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO
{
    public class InventarioIncidenciasAnuales
    {
        public string NombreBanco { get; set; }
        public string Cuenta { get; set; }
        public int anioResumen { get; set; }
        public int FormasInhabilitadas { get; set; }
        public int FormasAsignadas { get; set; }
        public int FormasFoliadasCorrectamente { get; set; }
    }
}
