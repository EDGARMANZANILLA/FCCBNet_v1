using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.FoliacionModels
{
    public class ResumenRevicionNominaPDFModel
    {
        //sirve como cumero cuando toca hacer una revicion de la nomina
        public string Contador { get; set; }
        public string Partida { get; set; }
        public string CadenaNumEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public string Delegacion { get; set; }
        public string Nomina { get; set; }
        public string NUM_CHE { get; set; }
        public string Liquido { get; set; }

        // Bancox - [cuenta_x]
        public string Cuenta { get; set; }
    }
}