using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO
{
    public class IPDDTO
    {
        public string Referencia { get; set; }
        public string TipoNom { get; set; }
        public string Cve_presup { get; set; }
        public string Cvegto { get; set; }
        public string Cvepd { get; set; }
        public decimal Monto { get; set; }
        public string Tipoclave { get; set; }
        public string Adicional { get; set; }
        public string Partida { get; set; }
        public string Num { get; set; }
        public string Nombre { get; set; }
        public string Num_che { get; set; }
        public int Foliocdfi { get; set; }
        public string Deleg { get; set; }
        public int Idctabanca { get; set; }
        public int IdBanco { get; set; }
        public string Pagomat { get; set; }
        public string Tipo_pagom { get; set; }
        public string Numtarjeta { get; set; }
        public int Orden { get; set; }
        public string Quincena { get; set; }
        public string Nomalpha { get; set; }
        public string Fecha { get; set; }
        public string Cvegasto { get; set; }
        public string Cla_pto { get; set; }
    }
}
