using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FoliarDTO
{
    public class ReporteFoliacionDTO
    {

        public string Id { get; set; }
        public string Partida { get; set; }
        public string Nombre { get; set; }
        public string Delegacion { get; set; }
        public string Num_Che { get; set; }
        public string Importe_Liquido { get; set; }
        public string CuentaBancaria { get; set; }
    }
}
