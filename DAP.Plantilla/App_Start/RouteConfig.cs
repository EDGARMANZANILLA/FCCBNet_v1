﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DAP.Plantilla
{
    public class RouteConfig
    {

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Validador", action = "ValidaToken", id = UrlParameter.Optional }
               // defaults: new { controller = "Inventario", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
