using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.FoliacionModels
{
    public class DatosAFoliarNominaConChequeraModel
    {


        public int IdNomina { get; set; }
        public int IdDelegacion { get; set; }
        public int IdGrupoFoliacion { get; set; }
        public int Sindicato { get; set; }
        public int Confianza { get; set; }
        public int Otros { get; set; }
        public int IdBancoPagador { get; set; }
        public int RangoInicial { get; set; }
        public bool Inhabilitado { get; set; }
        public int RangoInhabilitadoInicial { get; set; }
        public int RangoInhabilitadoFinal { get; set; }
        public string Quincena { get; set; }

    }
}