using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.FinalizarReferencia_CanceladosDTO
{
    public class DetallePagoDTO
    {
        public int Id { get; set; }
        public int IdRegistro { get; set; }
        public int IdNom { get; set; }
        public int Quicena { get; set; }
        public string ReferenciaOriginal { get; set; }
        public string Nomina { get; set; }
        public string Delegacion { get; set; }
        public string Partida { get; set; }
        public string EsPena { get; set; }
        public string Nombre { get; set; }
        public int Num { get; set; }
        public int Folio { get; set; }
        public decimal Liquido { get; set; }
        public string CFDI { get; set; }
        public string Banco { get; set; }
        public string  CTA_Bancaria { get; set; }

    }
}
