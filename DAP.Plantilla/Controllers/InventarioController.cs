using DAP.Plantilla.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAP.Foliacion.Negocios;
using DAP.Foliacion.Entidades.DTO;
using DAP.Plantilla.Reportes;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Net.Http;
using DAP.Plantilla.ObjetosExtras;

namespace DAP.Foliacion.Plantilla.Controllers
{
    public class InventarioController : Controller
    {
        // GET: Inventario
        public ActionResult Index()
        {

            List<InventarioModel> BancosMostrar = new List<InventarioModel>();

            var InventariosActivos = Negocios.InventarioNegocios.ObtenerInventarioActivo();  
            foreach (var inventarioBanco in InventariosActivos)
            {
                InventarioModel NuevoBanco = new InventarioModel();

              
                NuevoBanco.IdCuentaBancaria = inventarioBanco.Id;
                NuevoBanco.NombreBanco = inventarioBanco.NombreBanco;
                NuevoBanco.Cuenta = inventarioBanco.Cuenta;
                NuevoBanco.FormasDisponibles = inventarioBanco.Tbl_Inventario.FormasDisponibles;
                NuevoBanco.UltimoFolioInventario = inventarioBanco.Tbl_Inventario.UltimoFolioInventario;
                NuevoBanco.UltimoFolioUtilizado = inventarioBanco.Tbl_Inventario.UltimoFolioUtilizado;
                NuevoBanco.EstimadoMeses = inventarioBanco.Tbl_Inventario.EstimadoMeses;

                BancosMostrar.Add(NuevoBanco);

             }


            return View(BancosMostrar);
        }


        public ActionResult Solicitar()
        {


            return View(Negocios.InventarioNegocios.ObtenerBancosConChequera());
        }


        public ActionResult Agregar(int IdCuentaBancaria)
        {
            ViewBag.NombreBancoSeleccionado = Negocios.InventarioNegocios.ObtenerNombreBanco(IdCuentaBancaria);

            ViewBag.IdBancoSeleccionado = IdCuentaBancaria;

            return View();
        }


        public ActionResult Incidencias(int IdCuentaBancaria) 
        {
            ViewBag.NombreBancoSeleccionado = Negocios.InventarioNegocios.ObtenerNombreBanco(IdCuentaBancaria);

            ViewBag.IdBancoSeleccionado = IdCuentaBancaria;

            return View();
        }
        public ActionResult Inhabilitar(int IdCuentaBancaria)
        {
            //  ViewBag.NombreBanco = NombreBanco;

            ViewBag.NombreBancoSeleccionado = Negocios.InventarioNegocios.ObtenerNombreBanco(IdCuentaBancaria);

            ViewBag.IdBancoSeleccionado = IdCuentaBancaria;



            /*Pendiente*/
            ViewBag.OrdenesEncontradas = Negocios.InventarioNegocios.ObtenerNumeroOrdenesBancoActivo(1);


            return View();
        }
        public ActionResult Asignar(int IdCuentaBancaria)
        {
            ViewBag.NombreBancoSeleccionado = Negocios.InventarioNegocios.ObtenerNombreBanco(IdCuentaBancaria);

            ViewBag.IdBancoSeleccionado = IdCuentaBancaria;


            int idInventario = InventarioNegocios.ObtenerIdInventarioPorIdCuentaBancaria(IdCuentaBancaria);

            ViewBag.OrdenesEncontradas = Negocios.InventarioNegocios.ObtenerNumeroOrdenesBancoActivo(idInventario);


            ViewBag.ListaNombrePersonal = Negocios.InventarioNegocios.ObtenerPersonalActivo();

            return View();
        }




        public ActionResult DetalleBanco(int IdCuentaBancaria)
        {

            ViewBag.NombreBancoSeleccionado = Negocios.InventarioNegocios.ObtenerNombreBanco(IdCuentaBancaria);

            ViewBag.IdBancoSeleccionado = IdCuentaBancaria;


            int idInventario = InventarioNegocios.ObtenerIdInventarioPorIdCuentaBancaria(IdCuentaBancaria);
            ViewBag.IdInventario = idInventario;

            Session["IdCuentaBancaria"] = null;

            Session["IdCuentaBancaria"] = IdCuentaBancaria;

       
            return View(/*listaDetalle*/);
        }


        
      
      


      


        //metodos por post
        [HttpPost]

        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /*********************************************************                Agregar              ******************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        public JsonResult GuardarInventarioAgregado(List<AgregarInventarioModel> listaContenedores, string NumOrden, int IdBanco)
        {
            bool bandera = false;
            try
            {

                foreach (AgregarInventarioModel nuevoContenedor in listaContenedores.OrderBy( x => x.iteradorContenedor)) 
                {
                    bandera = Negocios.InventarioNegocios.GuardarInventarioContenedores( IdBanco, NumOrden, nuevoContenedor.iteradorContenedor, nuevoContenedor.folioInicial, nuevoContenedor.folioFinal, nuevoContenedor.TotalFormas, DAP.Plantilla.ObjetosExtras.ObtenerHoraReal.ObtenerDateTimeFechaReal());
                }

            }
            catch (Exception e)
            {
                bandera = false;
            }

            return Json(bandera, JsonRequestBehavior.AllowGet);
        }




    


        ////Metodos para Solicitar Formas de pago para uno o mas de un banco
        //public JsonResult ObtenerCuentaBancaria(string BancoSeleccionado) 
        //{
        //    return Json(Negocios.InventarioNegocios.ObtenerCuentaBancariaPorNombreBanco(BancoSeleccionado), JsonRequestBehavior.AllowGet);
        //}




        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************              Ver tablaDetalles para Inhabilitados y Asignados           ***********************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        //Metodo Para crear tablas de Info de la tabla Inventariocontenedores
        public ActionResult CrearTablaInhabilitadosOAsignacion(int IdCuentaBancaria)
        {
            var contenedoresEncontrados = Negocios.InventarioNegocios.ObtenerInfoContendoresPorBanco(IdCuentaBancaria).ToList();

            return Json(contenedoresEncontrados, JsonRequestBehavior.AllowGet);
        }




        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************              Verificacion de folios para Inhabilitar y Asignar          ***********************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/

        //Metodos para verificar disponibilidad de folios para poder saber si se puede inhabilitar o Asignar PD: se utiliza tambien en configuraciones para 
        //las cuentas bancarias que alguna vez tuvieron cheques pero ahora solo se paga con tarjetas
        public JsonResult VerificarDisponibilidadFolios(int IdCuentaBancaria, int FolioInicial, int FolioFinal  /*int IdInventario,string FolioInicial, string FolioFinal*/)
        {
            var prueba = Negocios.InventarioNegocios.ValidarFoliosFormasPago(IdCuentaBancaria, FolioInicial, FolioFinal);

            return Json(prueba, JsonRequestBehavior.AllowGet);
        }


        public JsonResult VerificarDisponibilidadContenedor(int IdContenedor)
        {
            List<InventarioValidaFoliosDTO> foliosNoDisponiblesContenedor = Negocios.InventarioNegocios.ValidarFoliosPorContenedor(IdContenedor);

            if (foliosNoDisponiblesContenedor.Count() > 0)
            {
                return Json(new
                {
                    bandera = true,
                    TotalFoliosNoDisponibles = foliosNoDisponiblesContenedor
                });
            }
            else
            {
                return Json(new
                {
                    bandera = false
                });
            }

        }


        //Metodo para Obtener Numeros de orden de la tabla InventarioContenedores funciona para Inhabilitar o Asignar por contenedor 
        public JsonResult ObtenerNumerosContenedores(int IdBanco, string OrdenSeleccionada)
        {
            List<InventarioClaveValorGenericaModel> numeroDeContenedores = new List<InventarioClaveValorGenericaModel>();

            // Dictionary<string, string> numeroDeContenedores = new Dictionary<string, string>();

            int idInventario = InventarioNegocios.ObtenerIdInventarioPorIdCuentaBancaria(IdBanco);


            var contenedoresEncontrados = InventarioNegocios.ObtenerContenedoresActivosPorIdInventario(idInventario, OrdenSeleccionada);


            if (contenedoresEncontrados.Count < 1 || numeroDeContenedores == null)
            {
                numeroDeContenedores = null;
            }
            else
            {

                foreach (var contenedor in contenedoresEncontrados)
                {
                    InventarioClaveValorGenericaModel nuevoContenedorEncontrado = new InventarioClaveValorGenericaModel();
                    nuevoContenedorEncontrado.Llave = contenedor.Id;
                    nuevoContenedorEncontrado.Valor = Convert.ToString(contenedor.NumContenedor);
                    numeroDeContenedores.Add(nuevoContenedorEncontrado);
                }

            }

            return Json(numeroDeContenedores, JsonRequestBehavior.AllowGet);
        }








        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************              Inhabilitar             ************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/

        //Metodo para Obtener Los numeros de folios del contenedor seleccionado de la tabla InventarioContenedores funciona para Inhabilitar  o Asignar por contenedor 
        public JsonResult ObtenerFoliosDelContenedor(int IdInventario, string OrdenSeleccionada, int ContenedorSeleccionado)
        {
            List<string> foliosContenedor = new List<string>();

            //int idBanco = Negocios.InventarioNegocios.ObtenerIdBanco(banco);

            var foliosDeContenedorEncontrado = InventarioNegocios.ObtenerFoliosPorContenedor(IdInventario, OrdenSeleccionada, ContenedorSeleccionado);

            foliosContenedor.Add(Convert.ToString(foliosDeContenedorEncontrado.Id));
            foliosContenedor.Add(Convert.ToString(foliosDeContenedorEncontrado.FolioInicial));
            foliosContenedor.Add(foliosDeContenedorEncontrado.FolioFinal);

            return Json(foliosContenedor, JsonRequestBehavior.AllowGet);
        }

     
        public JsonResult InhabilitarRango(int IdCuentaBancaria, int FolioInicial, int FolioFinal  )
        {
            return Json(Negocios.InventarioNegocios.InhabilitarRangoFolios(IdCuentaBancaria, FolioInicial, FolioFinal), JsonRequestBehavior.AllowGet);
        }

        

        public JsonResult InhabilitarContenedor(int IdContenedor)
        {
            return Json(Negocios.InventarioNegocios.InhabilitarContenedor(IdContenedor), JsonRequestBehavior.AllowGet);
        }




        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /******************************************************              Asignar           *************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/

        public JsonResult AsignarRango(int IdPersonal,  int IdCuentaBancaria, int FolioInicial, int FolioFinal)
        {
            return Json(Negocios.InventarioNegocios.AsignarRangoFolios(IdPersonal,IdCuentaBancaria, FolioInicial, FolioFinal), JsonRequestBehavior.AllowGet);
        }


        public JsonResult AsignarContenedor(int IdPersonal,  int IdContenedor)
        {
            return Json(Negocios.InventarioNegocios.AsignarContenedor( IdPersonal, IdContenedor), JsonRequestBehavior.AllowGet);
        }









        /*************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************/
        /*************************************************************          DetalleBanco         **************************************************************************************************/
        /************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************/

        //Carga de datos al datatable por medio de httpRequest: POST (De esa manera data table nos envia su info como draw, start)
   
        public JsonResult Paginar_CargarDetalleBanco() 
        {
            int IdCuentaBancaria = (int)Session["IdCuentaBancaria"];
            int idInventario = InventarioNegocios.ObtenerIdInventarioPorIdCuentaBancaria(IdCuentaBancaria);


            //logistica datatable
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
        //    string sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            string searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
           
             int pageSize, skip, recordsTotal;

            pageSize = length != null ? Convert.ToInt32(length) : 0;
            skip = start != null ? Convert.ToInt32(start) : 0;
            recordsTotal = 0;


            var queryable_Tbl_InventarioDetalle = Negocios.InventarioNegocios.Obtener_TodosLosDetallesCargarDetalleBanco(idInventario);
            recordsTotal = queryable_Tbl_InventarioDetalle.Count();





            List<DetalleBancoModel> listaDetalle = new List<DetalleBancoModel>();


            //Filtra una busqueda por el numero de Folio
            if (searchValue != "")
            {
                try
                {
                    int buscarFolio = Convert.ToInt32(searchValue);

                    queryable_Tbl_InventarioDetalle = queryable_Tbl_InventarioDetalle.Where(d => d.NumFolio == buscarFolio);
                }
                catch (Exception E) 
                {

                    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaDetalle });
                }

            }




            var listaDetalleFiltrada = queryable_Tbl_InventarioDetalle.OrderBy(x => x.NumFolio).Skip(skip).Take(pageSize).ToList();




                foreach (var detalle in listaDetalleFiltrada)
                {
                    DetalleBancoModel nuevoDetalle = new DetalleBancoModel();

                    nuevoDetalle.NumeroOrden = detalle.Tbl_InventarioContenedores.NumOrden;
                    nuevoDetalle.NumeroContenedor = detalle.Tbl_InventarioContenedores.NumContenedor;
                    nuevoDetalle.NumeroFolio = detalle.NumFolio;

                    if (detalle.IdIncidencia != null)
                        nuevoDetalle.Incidencia = detalle.Tbl_InventarioTipoIncidencia.Descrip_Incidencia;

                    if (detalle.IdEmpleado != null)
                        nuevoDetalle.NombreEmpleado = detalle.Tbl_InventarioAsignacionPersonal.NombrePersonal;

                    if (detalle.FechaIncidencia != null)
                        nuevoDetalle.FechaIncidencia = detalle.FechaIncidencia?.ToString("dd/MM/yyyy");

                    listaDetalle.Add(nuevoDetalle);
                }
          

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listaDetalle });

        }






        /*************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************/
        /*************************************************************            Solicitar             **************************************************************************************************/
        /************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************/

        //crea y Guarda la solicitud en la base de datos 
        public ActionResult CrearNuevaSolicitud(List<SolicitudCreadaDTO> listaBancosSolicitados)
        {
            bool bandera = false;
            //Session["ListaBancosSolicitados"] = null;
            //Session["NumeroMemo"] = null;

            int numMemoDevuelto = Negocios.InventarioNegocios.GuardarSolicitudCreada(listaBancosSolicitados);


            if (numMemoDevuelto > 0)
            {
                //    Session["ListaBancosSolicitados"] = listaBancosSolicitados;
                //    Session["NumeroMemo"] = numMemoDevuelto;
                bandera = true;
            }

            return Json(bandera, JsonRequestBehavior.AllowGet);
        }

        //obtiene las solicitudes para el historico de las solicitudes
        public ActionResult ObtenerHistoricoSolicitudes() 
        {
            var solicitudesObtenidas = Negocios.InventarioNegocios.ObtenerSolicitudes().Select(p => new { p.NumeroMemo, p.FechaSolicitud }).Distinct().ToList();
            List<SolicitudFormasPagoModel> solicitudes = new List<SolicitudFormasPagoModel>();

            foreach (var solicitud in solicitudesObtenidas)
            {
                SolicitudFormasPagoModel nuevaSolicitud = new SolicitudFormasPagoModel();

                nuevaSolicitud.NumeroMemo = solicitud.NumeroMemo;
                nuevaSolicitud.FechaSolicitud = solicitud.FechaSolicitud.ToString("dd/MM/yyyy");

                solicitudes.Add(nuevaSolicitud);

            }

            return Json(solicitudes, JsonRequestBehavior.AllowGet);
        }

        //Eliminar una solicitud recibiendo un numero de memorandum
        public ActionResult RemoverSolicitudCreada(int NumeroMemorandum)
        {
            bool bandera = Negocios.InventarioNegocios.EliminarMemorandum(NumeroMemorandum);
           

            return Json(bandera, JsonRequestBehavior.AllowGet);
        }

        //obtiene la solicitud realizada por el numero de memorandum
        public ActionResult ObtenerSolicitudPorMemo(int NumeroMemo)
        {
            var solicitudEncontrada = Negocios.InventarioNegocios.ObtenerSolicitudesPorNumeroMemo(NumeroMemo).ToList();

            List<SolicitudDetalleModel> detalleSolicitud = new List<SolicitudDetalleModel>();

            foreach (var solicitud in solicitudEncontrada )
            {
                SolicitudDetalleModel nuevaCuenta = new SolicitudDetalleModel(); 

                nuevaCuenta.NumeroMemo = solicitud.NumeroMemo;
                nuevaCuenta.NombreBanco = solicitud.Tbl_CuentasBancarias.NombreBanco;
                nuevaCuenta.NumeroCuenta = solicitud.Tbl_CuentasBancarias.Cuenta ;
                nuevaCuenta.Cantidad = solicitud.Cantidad;
                nuevaCuenta.FolioInicial = solicitud.FolioInicial;
                nuevaCuenta.FechaSolicitud = solicitud.FechaSolicitud.ToString("dd/MM/yyyy");

                detalleSolicitud.Add(nuevaCuenta);

            }

            return Json(detalleSolicitud, JsonRequestBehavior.AllowGet);
        }


        //exporta el pfd que se guardo anteriormente usando sessiones para guardar datos 
        public FileResult GenerarReporteSolicitud(int NumMemorandum)
        {

           
                var solicitudesMemoEncontradas =  Negocios.InventarioNegocios.ObtenerSolicitudesPorNumeroMemo(NumMemorandum).ToList();

            List<SolicitudCreadaDTO> solicitudDescargar = new List<SolicitudCreadaDTO>();
            if (solicitudesMemoEncontradas.Count() > 0)
            {
                foreach (var solicitud in solicitudesMemoEncontradas) 
                {
                        SolicitudCreadaDTO nuevaSolicitud = new SolicitudCreadaDTO();

                        nuevaSolicitud.cadenaNombreBanco = solicitud.Tbl_CuentasBancarias.NombreBanco;
                        nuevaSolicitud.cuentaBanco = solicitud.Tbl_CuentasBancarias.Cuenta;
                        nuevaSolicitud.fInicial = solicitud.FolioInicial;
                        nuevaSolicitud.cantidadFormas = Convert.ToString( solicitud.Cantidad);

                        solicitudDescargar.Add(nuevaSolicitud);
                }

            }


           // List<SolicitudFormasPagoModel> listaBancosSolicitadosSolictudReciente = new List<SolicitudFormasPagoModel>();
            DAP.Plantilla.Reportes.Datasets.SolicitudFormasPago dtsSolicitusFormasPago = new DAP.Plantilla.Reportes.Datasets.SolicitudFormasPago();

           
                int numeroMemoRegistrado = solicitudesMemoEncontradas.Select(x => x.NumeroMemo).FirstOrDefault();

                //Cargar el numero del memo
                dtsSolicitusFormasPago.NumeroMemo.AddNumeroMemoRow(Convert.ToString(numeroMemoRegistrado));


                //cargar datos al dataset para el reporte
                foreach (var solicitudRecuperada in solicitudDescargar)
                {
                   // solicitudRecuperada.fInicial = solicitudRecuperada.fInicial != null ? solicitudRecuperada.fInicial : " . ";
                 
                    dtsSolicitusFormasPago.FormaPago.AddFormaPagoRow(solicitudRecuperada.cadenaNombreBanco, solicitudRecuperada.cuentaBanco, solicitudRecuperada.fInicial, solicitudRecuperada.cantidadFormas);

                 }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/SolicitarFormasDePagoBancos.rpt"));

            rd.SetDataSource(dtsSolicitusFormasPago);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "Solicitud.pdf");
        }







        /*************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************/
        /*************************************************************         BTN descargar reporte de Inventario            **************************************************************************************************/
        /************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************/


        public FileResult GenerarReporteFormasChequesExistentes(int MesSelecionado)
        {
            ////Obtener y convertir a 
            //string fechaExterna = Convert.ToString(ObtenerFechaServerGoogle());
            DateTime AnioCurso = ObtenerHoraReal.ObtenerDateTimeFechaReal();/*Convert.ToDateTime(fechaExterna);*/


            string nombreMes = "";

            switch (MesSelecionado)
            {
                case 1:
                    nombreMes = "ENERO";
                    break;
                case 2:
                    nombreMes = "FEBRERO";
                    break;
                case 3:
                    nombreMes = "MARZO";
                    break;
                case 4:
                    nombreMes = "ABRIL";
                    break;
                case 5:
                    nombreMes = "MAYO";
                    break;
                case 6:
                    nombreMes = "JUNIO";
                    break;
                case 7:
                    nombreMes = "JULIO";
                    break;
                case 8:
                    nombreMes = "AGOSTO";
                    break;
                case 9:
                    nombreMes = "SEPTIEMBRE";
                    break;
                case 10:
                    nombreMes = "OCTUBRE";
                    break;
                case 11:
                    nombreMes = "NOVIEMBRE";
                    break;
                case 12:
                    nombreMes = "DICIEMBRE";
                    break;
            }



            var datosReporte = Negocios.InventarioNegocios.ObtenerInventarioGeneralDatosReporte(MesSelecionado, AnioCurso.Year);


            DAP.Plantilla.Reportes.Datasets.FormasChequesExistentes dtsFormasExistentes = new DAP.Plantilla.Reportes.Datasets.FormasChequesExistentes();

            if (datosReporte.Count() > 0)
            {
                //Cargar el numero del memo
                dtsFormasExistentes.Fecha.AddFechaRow(DateTime.Now.ToString("dddd, dd MMMM yyyy").ToUpper());


                //cargar datos al dataset para el reporte
                foreach (var inventarioBanco in datosReporte)
                {

                    dtsFormasExistentes.FormasExistentes.AddFormasExistentesRow(inventarioBanco.NombreBanco, inventarioBanco.Cuenta, Convert.ToString(inventarioBanco.FolioInicialExistente ), Convert.ToString(inventarioBanco.FolioFinalExistente ), Convert.ToString(inventarioBanco.TotalFormasPago), inventarioBanco.ConsumoMensualAproximado, inventarioBanco.SolicitarFormas);
                }

            }


            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/FormasDeChequesExistentes.rpt"));

            rd.SetDataSource(dtsFormasExistentes);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "REPORTE INVENTARIO "+nombreMes+" "+AnioCurso.Year+".pdf");
        }









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



    }



}