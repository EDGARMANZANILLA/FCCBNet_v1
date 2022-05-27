using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models.FoliacionModels
{
    public class DatosParaFoliarChequesModel
    {
        public int IdNomina { get; set; }
        public string Quincena { get; set; }
        public int Delegacion { get; set; }
        // propiedad usada para saber a que grupo de nomina corresponde 
        // 0 = le pertenece a las nominas general y descentralizada
        // 1 = le pertenece a cualquier otra Nomina incluyendo a la de Pension Alimenticia 
        public int GrupoFoliacion { get; set; }

        //si el grupo de foliacion es {0}; ya que solo pertenecen la descentralizado y la general se valida con Sindicado > 0 && Confianza == 0 => Son sindicalizados |Y| viceversa Sindicato ==0 && Confianza > 0 => son de Confianza
        public int Sindicato { get; set; }
        public int Confianza { get; set; }
        public int Otros { get; set; }
        public int IdBancoPagador { get; set; }
        public int RangoInicial { get; set; }


        //por si se habilita la casilla inhabilitados 
        public bool Inhabilitado { get; set; }
        public int RangoInhabilitadoInicial { get; set; }
        public int RangoInhabilitadoFinal { get; set; }
    }
}