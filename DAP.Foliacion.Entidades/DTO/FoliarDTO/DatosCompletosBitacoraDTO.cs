using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
   public class DatosCompletosBitacoraDTO
   {
        public int Id_nom { get; set; }
        public string Nomina { get; set; }
        public string An { get; set; }
        public string Adicional { get; set; }
        public int Anio { get; set; }
        public string Quincena { get; set; }
        public int Mes { get; set; }
        public string ReferenciaBitacora { get; set; }
        public bool EsPenA { get; set; }
        public string Coment { get; set; }
        public string Ruta { get; set; }
        public string RutaNomina { get; set; }
        public string Subnomina { get; set; }
        public bool Importado { get; set; }
        public string  Ap{ get; set; }
        public string  Ad{ get; set; }



    }
}
