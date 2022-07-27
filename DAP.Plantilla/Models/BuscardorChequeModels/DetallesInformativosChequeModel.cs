using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.BuscardorChequeModels
{
    public class DetallesInformativosChequeModel
    {
        public int IdRegistro { get; set; }
        public string Id_nom { get; set; }
        public string ReferenciaBitacora { get; set; }





        public int Quincena { get; set; }
        public int NumEmpleado { get; set; }
        public string NombreEmpleado { get; set; }





        public int Delegacion { get; set; }
        public int Folio { get; set; }
        public decimal Liquido { get; set; }


        public string EsSuspencion { get; set; }
        public string EstadoCheque { get; set; }
        public string BancoPagador { get; set; }
        public string CuentaPagadora { get; set; }
        public string TipoPago { get; set; }



        public bool? EsPenA { get; set; }
        public string NumBeneficiarioPenA { get; set; }
        public string NombreBeneficiarioPenA { get; set; }





        public bool? EsRefoliado { get; set; }


    }
}