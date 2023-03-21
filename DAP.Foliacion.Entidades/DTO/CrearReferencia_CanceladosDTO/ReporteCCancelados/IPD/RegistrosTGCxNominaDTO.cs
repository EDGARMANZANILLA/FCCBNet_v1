﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPD
{
    public class RegistrosTGCxNominaDTO
    {
        //public string NombreNomina { get; set; }
        //public int Id_nom { get; set; }
        //public int Anio { get; set; }
        //public string Partida { get; set; }
        //public int NumEmpleado { get; set; }


        /***    CREADO EL 02/03/2023  COMO MANERA DE UNA ACTUALIZACION DE DOS REPORTES EN UNO DEL TGCXNOMINA ***/
        /*******************************/
        /***    DATOS GENERALES     ****/
        public string NombreNomina { get; set; }
        public string Ramo { get; set; }
        public string Partida { get; set; }

        /******************************/
        /***    DATOS PERCEPCIONES  ***/
        /******************************/
        public string PP_TipoClave { get; set; }
        public int PP_Cantidad { get; set; }
        public string PP_CvePD { get; set; }
        public string pp_DescripcionCvePD { get; set; }
        public decimal PP_SumatoriaPositiva { get; set; }
        public decimal PP_SumatoriaNegativa { get; set; }

        /******************************/
        /***    DATOS DEDUCCIONES  ***/
        /******************************/
        public string DD_TipoClave { get; set; }
        public int DD_Cantidad { get; set; }
        public string DD_CvePD { get; set; }
        public string DD_DescripcionCvePD { get; set; }
        public decimal DD_SumatoriaPositiva { get; set; }
        public decimal DD_SumatoriaNegativa { get; set; }


        /************************************/
        /*********      Totales       *******/
        /************************************/
        public decimal PP_TotalPositivo { get; set; }
        public decimal PP_TotalNegativo { get; set; }

        public decimal DD_TotalPositivo { get; set; }
        public decimal DD_TotalNegativo { get; set; }
        public decimal Liquido { get; set; }
    }
}
