using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DAP.Plantilla.Models
{
    public class AgregarInventarioModel
    {
        // IteradorDeContenedores, FInicial, FFinal, TotalFormas
        public int id { get; set; }
        public int iteradorContenedor { get; set; }

        public string folioInicial { get; set; }

        public string folioFinal { get; set; }

        public int TotalFormas { get; set; }

    }
}