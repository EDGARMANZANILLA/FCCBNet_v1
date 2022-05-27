using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.FoliacionModels
{
    public class GenerarReportePorDelegacionChequeModels
    {
        public int IdNomina { get; set; }
        public int IdDelegacion { get; set; }
        public int GrupoFoliacion { get; set; }
        public int Sindicato { get; set; }
        public int Confianza { get; set; }
        public int Otros { get; set; }
        public string Quincena { get; set; }

    }
}