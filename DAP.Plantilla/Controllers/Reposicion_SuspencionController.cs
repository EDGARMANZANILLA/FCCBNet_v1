using AutoMapper;
using DAP.Foliacion.Entidades.DTO.BuscardorChequeDTO;
using DAP.Plantilla.Models.BuscardorChequeModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAP.Foliacion.Negocios;

namespace DAP.Plantilla.Controllers
{
    public class Reposicion_SuspencionController : Controller
    {
        // GET: Reposicion_Suspencion
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult BuscarDetalleSuspencion(int IdRegistroAbuscar)
        {

            DetallesInformativosChequeModel detalleObtenido = Mapper.Map<DetallesRegistroDTO, DetallesInformativosChequeModel>(Reposicion_SuspencionNegocios.ObtenerDetalleCompletoIdRegistro(IdRegistroAbuscar));



            if (detalleObtenido.IdRegistro == IdRegistroAbuscar)
            {
                return PartialView(detalleObtenido);
            }


            return Json(new
            {
                RespuestaServidor = 500,
                MensajeError = "ocurrio un error contacte al desarrollador"
            });

        }


        [HttpPost]
        public ActionResult BuscarDetalleReponer(int IdRegistroAbuscar)
        {

            DetallesInformativosChequeModel detalleObtenido = Mapper.Map<DetallesRegistroDTO, DetallesInformativosChequeModel>(Reposicion_SuspencionNegocios.ObtenerDetalleCompletoIdRegistro(IdRegistroAbuscar));


            if (detalleObtenido.IdRegistro == IdRegistroAbuscar)
            {
                return PartialView(detalleObtenido);
            }

            return Json(new
            {
                RespuestaServidor = 500,
                MensajeError = "ocurrio un error contacte al desarrollador"
            });

        }







        [HttpPost]
        public ActionResult Localizar(int IdFiltro,  int LocalizarEsteElemento) 
        {
           List<DetallesBusquedaModels> formaDePagoLocalizados = null;


            if (LocalizarEsteElemento > 0)
            {
                // 1 => Localiza una sola forma de pago a la vez 
               
                formaDePagoLocalizados = Mapper.Map<List<DetallesBusqueda>, List<DetallesBusquedaModels>>(Foliacion.Negocios.Reposicion_SuspencionNegocios.ObtenerDetallesDeCheque(IdFiltro ,   LocalizarEsteElemento));

                if (formaDePagoLocalizados != null)
                {

                     return Json(new
                     {
                        RespuestaServidor = 200,
                        FormaPagoLocalizada = formaDePagoLocalizados
                            
                     });

                }
                else 
                {
                    return Json(new
                    {
                        RespuestaServidor = 201,
                        Error = "'Registro no encontrado contacte con el desarrolador '",
                        Solucion = "Ingreso un folio valido para su localizacion"

                    });

                }
                
            }
            else 
            {
                return Json(new
                {
                    RespuestaServidor = 500,
                    Error = "'El valor ingresado no es valido'",
                    Solucion = "Ingreso un folio valido para su localizacion"

                });

            }


        }






        public JsonResult BuscarHistorico(int IdRegistro)
        {
            var listaHistorico = BuscadorChequeNegocios.ObtenerHistoricoReposiciones(IdRegistro).OrderByDescending(y => y.Id);

            if (listaHistorico.Count() >= 1)
            {
                return Json(new
                {
                    RespuestaServidor = 200,
                    Data = listaHistorico
                });
            }
            else
            {
                return Json(new
                {
                    Error = "'No se encontro ningun historico para la forma de pago seleccionada'",
                    Solucion = "Intente de nuevo por favor o contecte con el desarrollador"
                });
            }

        }






        public ActionResult SuspenderIdFormaPago(int IdRegistroPago) 
        {
            // REPORNER
            // SUSPENDER
            //NO_EXISTE
            
            string quePuedoHacer = Reposicion_SuspencionNegocios.VerificaFormaPagoEsActivoYQueSePuedeHacer(IdRegistroPago);


            int respuestaServidor =0;
            string solucion = "";
            if (quePuedoHacer.ToUpper().Trim().Equals(("SUSPENDER").ToUpper().Trim()))
            {
                //Continua con la suspencion

                string respuesta = Reposicion_SuspencionNegocios.SuspenderFormaPago(IdRegistroPago);

                if (respuesta.Contains("LA BASE"))
                {
                    respuestaServidor = 0;
                    solucion = respuesta;
                }
                else if (respuesta.Contains("Error"))
                {
                    respuestaServidor = 1;
                    solucion = "No se pudo suspender la dispersion seleccionada";
                }
                else if (respuesta.Contains("Correcto"))
                {
                    respuestaServidor = 2;
                    solucion = "Se a suspendido de manera exitosa ";
                }
                else
                {
                    respuestaServidor = 1;
                    solucion = respuesta;
                }


            }
            else if (quePuedoHacer.ToUpper().Trim().Equals(("REPONER").ToUpper().Trim()))
            {
                respuestaServidor = 1;
                solucion = "Este pago ya no forma parte de una dispersion por ende no se puede suspender por sistema, informe al administrativo correspondiente para la retencion del cheque";
            }
            else if (quePuedoHacer.ToUpper().Trim().Equals(("REFERENCIACANCELADO").ToUpper().Trim()))
            {
                respuestaServidor = 1;
                solucion = "Este pago se encuentra es un proceso de cancelacion verifique por favor ";
            }

            return Json(new
            {
                respuestaServidor = respuestaServidor,
                solucion = solucion
            });
        }





        public ActionResult ReponerIdFormaPago(int IdRegistroPago,  int ReponerNuevoFolio)
        {

            string quePuedoHacer = Reposicion_SuspencionNegocios.VerificaFormaPagoEsActivoYQueSePuedeHacer(IdRegistroPago);


            int respuestaServidor = 0;
            string solucion = "";
            if (quePuedoHacer.ToUpper().Trim().Equals(("REPONER").ToUpper().Trim()))
            {
                //Continua con la reposicion
               string respuesta =  Reposicion_SuspencionNegocios.ReponerNuevaFormaPago( IdRegistroPago,  ReponerNuevoFolio, "CAMBIO DE FOLIO" );

                if (respuesta.Contains("LA BASE"))
                {
                    respuestaServidor = 0;
                    solucion = respuesta;
                }
                else if (respuesta.Contains("Error"))
                {
                    respuestaServidor = 1;
                    solucion = "No se pudo suspender la dispersion seleccionada";
                }
                else if (respuesta.Contains("CORRECTO"))
                {
                    respuestaServidor = 2;
                    solucion = "Se a repuesto de manera exitosa el folio : " + ReponerNuevoFolio;
                }
                else
                {
                    respuestaServidor = 1;
                    solucion = respuesta;
                }

          
            

            }
            else if (quePuedoHacer.ToUpper().Trim().Equals(("REFERENCIACANCELADO").ToUpper().Trim()))
            {
                respuestaServidor = 1;
                solucion = "Asegurese de removerlo de la referencia primero antes de reponer " + "  ''No se puede reponer porque se encuentra cargado a una referencia de cancelacion''";
            }
            else if (quePuedoHacer.ToUpper().Trim().Equals(("NO_EXISTE").ToUpper().Trim()))
            {
                respuestaServidor = 1;
                solucion = "'No se pudo pudo encontrar el pago seleccionado' " + " Contacte con el desarrollador";
            }
            else 
            {
                respuestaServidor = 1;
                solucion = "Asegurese de haber suspendido la dispercion antes de querer reponer la forma de pago " + " ' No se puede reponer esta forma de pago, porque aun es una dispersion'";
            }

            return Json(new
            {
                respuestaServidor = respuestaServidor,
                solucion = solucion
            });
        }








    }
}



