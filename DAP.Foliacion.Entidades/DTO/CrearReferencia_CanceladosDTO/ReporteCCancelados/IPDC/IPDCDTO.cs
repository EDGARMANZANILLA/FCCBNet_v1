using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPDC
{
    public class IPDCDTO
    {
        public string TipoNom { get; set; }
        public string Cve_presup { get; set; }
        public string CveGto { get; set; }
        public decimal Monto { get; set; }
        public string TipoClave { get; set; }
        public string Num_che { get; set; }
        public string CveReal { get; set; }
        public string CveCompen { get; set; }
        public string fecha { get; set; }
        public int IdctaBanca { get; set; }
        public int IdBanco { get; set; }
        public string Num { get; set; }
        public string NomAlpha { get; set; }
        public string Quincena { get; set; }
        public string Adicional { get; set; }
        public string Cla_pto { get; set; }
        public int Ldf_6d { get; set; }
    }
}
