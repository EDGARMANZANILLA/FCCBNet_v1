using DAP.Foliacion.Entidades;
using DAP.Foliacion.Entidades.DTO.BuscardorChequeDTO;
using DAP.Foliacion.Entidades.DTO.ConfiguracionesDTO;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using DAP.Plantilla.Models.BuscardorChequeModels;
using DAP.Plantilla.Models.ConfiguracionesModels.FormaPagosDesinhabilitarModels;
using DAP.Plantilla.Models.CrearReferencia_CanceladosModels;
using DAP.Plantilla.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DAP.Plantilla
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);



            AutoMapper.Mapper.Initialize(cfg => {
                cfg.CreateMap<ResultadoObtenidoParaSelect2, ElementosBuscador>();
                cfg.CreateMap<DetallesBusqueda, DetallesBusquedaModels>();
                cfg.CreateMap<DetallesRegistroDTO, DetallesInformativosChequeModel>();
                cfg.CreateMap<ResumenFoliosDesinhabilitarDTO, DesinhabilitarFormasPagoVerificarModels>();
                cfg.CreateMap<CrearReferenciaDTO, ReferenciaCanceladoModel> ();
                cfg.CreateMap<ResumenRevicionNominaPDFDTO, Models.FoliacionModels.ResumenRevicionNominaPDFModel>();
               
            
            });

        }
    }
}
