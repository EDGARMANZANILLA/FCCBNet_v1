using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.BuscardorChequeModels
{
    public class DetallesBusquedaModels
    {

        public int IdRegistro { get; set; }
        public int Id_nom { get; set; }
        public string ReferenciaBitacora { get; set; }
        public int Quincena { get; set; }
        public int NumEmpleado { get; set; }
        public string NombreBeneficiaro { get; set; }
        public string NumBene { get; set; }

        public int FolioCheque { get; set; }
        public decimal Liquido { get; set; }
        public string EstadoCheque { get; set; }

        public string TipoPago { get; set; }

    }
}