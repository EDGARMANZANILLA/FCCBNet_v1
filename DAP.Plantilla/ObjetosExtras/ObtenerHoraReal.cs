using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace DAP.Plantilla.ObjetosExtras
{
    public static class ObtenerHoraReal
    {
        public static DateTimeOffset? ObtenerFechaServerGoogle()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var result = client.GetAsync("https://google.com/",
                          HttpCompletionOption.ResponseHeadersRead).Result;
                    return result.Headers.Date;
                }
                catch
                {
                    return null;
                }
            }
        }


        public static DateTime ObtenerDateTimeFechaReal()
        {
            string fechaExterna = Convert.ToString(ObtenerFechaServerGoogle());

            return Convert.ToDateTime(fechaExterna);
        }

    }
}