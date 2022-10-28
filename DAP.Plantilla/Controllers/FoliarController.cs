using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using DAP.Foliacion.Negocios;
using DAP.Plantilla.Reportes.Datasets;
using DAP.Plantilla.Models;
using DAP.Foliacion.Entidades;
using DAP.Plantilla.Models.FoliacionModels;
using System.Security.Principal;
using AutoMapper;
using System.Threading.Tasks;

namespace DAP.Plantilla.Controllers
{
    public class FoliarController : Controller
    {
        //VISTAS
        public ActionResult Index()
        {
            ViewBag.UltimaQuincenaEncontrada = FoliarNegocios.ObtenerUltimaQuincenaFoliada();

            return View();
        }

        public ActionResult FoliarXPagomatico(string NumeroQuincena)
        {
            try
            {
              //  Session.Remove("NumeroQuincena");
              //  Session["NumeroQuincena"] = Convert.ToInt32(NumeroQuincena.Substring(1, 3));

                int anio = Convert.ToInt32( DateTime.Now.Year.ToString().Substring(0, 2) + NumeroQuincena.Substring(0, 2));

                ViewBag.NumeroQuincena = NumeroQuincena;
                Dictionary<int, string> ListaNombresNominaQuincena = FoliarNegocios.ObtenerNombreNominasEnQuincena(NumeroQuincena, anio );
                
                if (ListaNombresNominaQuincena.Count() > 0)
                {
                    return PartialView(ListaNombresNominaQuincena);
                }

            }
            catch (Exception E)
            {
                return Json(new
                {
                    RespuestaServidor = "500",
                    MensajeError = "Ocurrio un error verifique que la quincena sea correcta"
                });


            }

            return Json(new
            {
                RespuestaServidor = "500",
                MensajeError = "No se encuentran cargadas las nominas de la quincena seleccionada"
            });
        }

        public ActionResult FoliarXFormasPago(string Quincena)
        {
            try
            {

                //Session.Remove("NumeroQuincena");
                //Session["NumeroQuincena"] = Convert.ToInt32(Quincena.Substring(1, 3));

                int anio = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(0, 2) + Quincena.Substring(0, 2));

                ViewBag.NumeroQuincena = Quincena;
                Dictionary<int, string> ListaNombresNominaQuincena = FoliarNegocios.ObtenerNombreNominasEnQuincena(Quincena, anio);
                ViewBag.BancosConTarjeta = FoliarNegocios.ObtenerBancoParaFormasPago();

                if (ListaNombresNominaQuincena.Count() > 0)
                {
                    return PartialView(ListaNombresNominaQuincena);
                }
            }
            catch (Exception E)
            {
                return Json(new
                {
                    RespuestaServidor = "500",
                    MensajeError = "Ocurrio un error verifique que la quincena sea correcta"
                });


            }
       


            return Json(new
            {
                RespuestaServidor = "500",
                MensajeError = "No se encuentran cargadas las nominas de la quincena seleccionada"
            });
        }

        public ActionResult RecuperarFolios()
        {
            ViewBag.BancosConTarjeta = FoliarNegocios.ObtenerBancoParaFormasPago();
            return View();
        }







        [HttpPost]
        //Actualiza la tabla de cuantos cheques por banco quedan disponibles cuando carga la vista de Foliar index 
        public ActionResult ActualizarTablaResumenBanco(string Dato) 
        {
            return Json(FoliarNegocios.ObtenerDetalleBancoFormasDePago(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult ObtenerNombreNominas(string NumeroQuincena)
        {
            int anio = FoliarNegocios.ObtenerAnioDeQuincena(NumeroQuincena);

            var nombreNominasQuincena = FoliarNegocios.ObtenerNombreNominasEnQuincena(NumeroQuincena, anio);


            //Se crea la variable de session cada vez que inicia el proceso de foliar



            return Json(nombreNominasQuincena, JsonRequestBehavior.AllowGet);
        }


















        #region Metodos para la Revicion de la Foliacion por medio de Formas de pago
        
        //*******************************************************************************************************************************************************************************************************************//
        //*******************************************************************************************************************************************************************************************************************//
        //****************************************************         Metodos para FOLIAR nominas con cheques(FORMAS DE PAGO => CHEQUES)    ********************************************************************************//
        //*******************************************************************************************************************************************************************************************************************//
        //*******************************************************************************************************************************************************************************************************************//

        public async System.Threading.Tasks.Task<ActionResult> FoliarNominaFormaPago(DatosAFoliarNominaConChequeraModel NuevaFoliacionNomina)
        {
            int totalDeRegistrosAFoliar = 0;
            if (NuevaFoliacionNomina.IdGrupoFoliacion == 0)
            {
                if (NuevaFoliacionNomina.Confianza > 0 && NuevaFoliacionNomina.Sindicato == 0)
                {
                    //son de confianza
                    totalDeRegistrosAFoliar = NuevaFoliacionNomina.Confianza;
                }
                else if (NuevaFoliacionNomina.Confianza == 0 && NuevaFoliacionNomina.Sindicato > 0)
                {
                    //Son sindicalizados
                    totalDeRegistrosAFoliar = NuevaFoliacionNomina.Sindicato;
                }
            }
            else if (NuevaFoliacionNomina.IdGrupoFoliacion == 1)
            {
                totalDeRegistrosAFoliar = NuevaFoliacionNomina.Otros;
            }

            bool hayFoliosConsecutivosSinUsar = FoliarNegocios.VerificarFoliosConsecutivos(NuevaFoliacionNomina.IdBancoPagador, NuevaFoliacionNomina.RangoInicial);

            if (hayFoliosConsecutivosSinUsar)
            {
                //Informa al usuario que exiten cheques consecutivos que se saltaran durante la foliacion  
                int rangoSaltadoInicial = NuevaFoliacionNomina.RangoInicial - 6;
                int rangoSaltadoFinal = NuevaFoliacionNomina.RangoInicial;
                return Json(new
                {
                    resultServer = 501,
                    FoliosSaltados = "Esta intentanto foliar con un salto de folios consecutivos sin usar aun,  va del rango inicial " + rangoSaltadoInicial + " al rango final " + rangoSaltadoFinal + ". Se aconseja no saltar folios sin usar."
                });
            }
            else
            {

                List<FoliosAFoliarInventario> chequesVerificadosFoliar = FoliarNegocios.verificarDisponibilidadFoliosEnInventarioDetalle(NuevaFoliacionNomina.IdBancoPagador, NuevaFoliacionNomina.RangoInicial, totalDeRegistrosAFoliar, NuevaFoliacionNomina.Inhabilitado, NuevaFoliacionNomina.RangoInhabilitadoInicial, NuevaFoliacionNomina.RangoInhabilitadoFinal).ToList();

                List<FoliosAFoliarInventario> foliosNoDisponibles = chequesVerificadosFoliar.Where(y => y.Incidencia != "").ToList();

                if (foliosNoDisponibles.Count() > 0)
                {
                    return Json(new
                    {
                        resultServer = 0,
                        FoliosConIncidencias = foliosNoDisponibles
                    });

                }
                else
                {

                    FoliarFormasPagoDTO foliarNomina = new FoliarFormasPagoDTO();

                    foliarNomina.IdNomina = NuevaFoliacionNomina.IdNomina;
                    foliarNomina.IdDelegacion = NuevaFoliacionNomina.IdDelegacion;
                    foliarNomina.Sindicato = NuevaFoliacionNomina.Sindicato;
                    foliarNomina.Confianza = NuevaFoliacionNomina.Confianza;
                    foliarNomina.IdBancoPagador = NuevaFoliacionNomina.IdBancoPagador;
                    foliarNomina.RangoInicial = NuevaFoliacionNomina.RangoInicial;

                    // por si el usuario habilita la casilla inhabilitados aqui se rescatan
                    foliarNomina.Inhabilitado = NuevaFoliacionNomina.Inhabilitado;
                    foliarNomina.RangoInhabilitadoInicial = NuevaFoliacionNomina.RangoInhabilitadoInicial;
                    foliarNomina.RangoInhabilitadoFinal = NuevaFoliacionNomina.RangoInhabilitadoFinal;


                    //propiedad usada para saber a que grupo de nomina corresponde
                    // 1 = le pertenece a las nominas general y descentralizada
                    // 2 = le pertenece a cualquier otra nomina que no se folea por sindicato y confianza
                    foliarNomina.IdGrupoFoliacion = NuevaFoliacionNomina.IdGrupoFoliacion;
                    foliarNomina.AnioInterfaz = FoliarNegocios.ObtenerAnioDeQuincena(NuevaFoliacionNomina.Quincena);

                    string Observa = "CHEQUE";
                    //List<AlertasAlFolearPagomaticosDTO> resultadoFoliacion = await FoliarNegocios.FoliarChequesPorNomina(foliarNomina, Observa, chequesVerificadosFoliar);
                    List<AlertasAlFolearPagomaticosDTO> resultadoFoliacion = await FoliarNegocios.FoliarChequesPorNomina_TIEMPO_DE_RESPUESTA_MEJORADO(foliarNomina, Observa, chequesVerificadosFoliar);

                    var DbfAbierta = resultadoFoliacion.Where(x => x.IdAtencion == 4).Select(x => new { x.Id_Nom, x.Detalle, x.Solucion }).ToList();


                    if (DbfAbierta.Count() > 0)
                    {
                        return Json(new
                        {
                            resultServer = 500,
                            DBFAbierta = DbfAbierta
                        });
                    }


                    return Json(new
                    {
                        resultServer = 200,
                        resultadoFoliacion = resultadoFoliacion
                    });

                }



            }


        }




            //*************************************************************************************************************************************************************************************************************************************************//
            //****************************   OBTIENE UN RESUMEN POR DELEGACION DE UNA NOMINA ELEGIDA PARA VISUALIZAR ES UN MODAL CUANTOS REGISTROS HAY POR DELEGACION PARA FORLIAR POR CHEQUERA (CHEQUE)      **************************************************//
            //************************************************************************************************************************************************************************************************************************************************//
            public ActionResult ObtenerResumenDetalle_NominaCheques(int IdNomina , string Quincena )
            {
                int anioInterface = FoliarNegocios.ObtenerAnioDeQuincena(Quincena);
                var resumenDatosTablaModal = FoliarNegocios.ObtenerResumenDetalleNomina_Cheques( IdNomina, anioInterface).OrderBy(X => X.IdDelegacion);

                return Json(resumenDatosTablaModal, JsonRequestBehavior.AllowGet);
            }

            //***************************************************************************************************************************************************************************************************************************************************//
            //***********************        GENERA REPORTE EN PDF DE CHEQUES DONDE SE VISUALIZA CADA EMPLEADO COMO SE ENCUENTRA EN SQL PARA VERIFICAR SI ESTA BIEN FOLIADO O NO DE ACUERDO A LOS FOLIOS Y LA CHEQUERA QUE SE UTILIZO    *************************//
            //***************************************************************************************************************************************************************************************************************************************************//
            public ActionResult RevisarReportePDFChequeIdNominaPorDelegacion(GenerarReportePorDelegacionChequeModels GenerarReporteDelegacion) 
            {
                int anioInterface = FoliarNegocios.ObtenerAnioDeQuincena(GenerarReporteDelegacion.Quincena);
                  string visitaAnioInterface = FoliarNegocios.ObtenerCadenaAnioInterface(anioInterface);
                var datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraFILTRO(GenerarReporteDelegacion.IdNomina, visitaAnioInterface);

                List<ResumenRevicionNominaPDFModel> ResumenRevicionNominaPDF = new List<ResumenRevicionNominaPDFModel>();

                if (GenerarReporteDelegacion.GrupoFoliacion == 0)
                {
                    //El grupo de foliacion {0} de los cheques pertenece a las nominas GENERAL Y DESCENTRALIZADOS

                    bool EsSindicalizado;
                    if (GenerarReporteDelegacion.Sindicato > 0 && GenerarReporteDelegacion.Confianza == 0)
                    {
                        EsSindicalizado = true;
                        ResumenRevicionNominaPDF = Mapper.Map<List<ResumenRevicionNominaPDFDTO>, List<ResumenRevicionNominaPDFModel>>(FoliarNegocios.ObtenerDatosPersonalesDelegacionNominaGeneralDesce_ReporteCheque(GenerarReporteDelegacion.IdNomina, anioInterface, EsSindicalizado, GenerarReporteDelegacion.IdDelegacion));
                              
                    }
                    else if (GenerarReporteDelegacion.Sindicato == 0 && GenerarReporteDelegacion.Confianza > 0) 
                    {
                        EsSindicalizado = false;
                        ResumenRevicionNominaPDF = Mapper.Map<List<ResumenRevicionNominaPDFDTO>, List<ResumenRevicionNominaPDFModel>>(FoliarNegocios.ObtenerDatosPersonalesDelegacionNominaGeneralDesce_ReporteCheque(GenerarReporteDelegacion.IdNomina, anioInterface, EsSindicalizado, GenerarReporteDelegacion.IdDelegacion));

                    }


                }
                else 
                {
                //El grupo de foliacion {1} de los cheques pertenece a TODAS LAS DEMAS NOMINAS INCLUYE LA DE PENSION ALIMENTICIA
                ResumenRevicionNominaPDF = Mapper.Map<List<ResumenRevicionNominaPDFDTO>, List<ResumenRevicionNominaPDFModel>>(FoliarNegocios.ObtenerDatosPersonalesDelegacionOtrasNominas_ReporteCheque(GenerarReporteDelegacion.IdNomina, anioInterface, GenerarReporteDelegacion.IdDelegacion ));
                }



            string archivoBase64;
            string NombreDelegacionSeleccionada = FoliarNegocios.ObtenerNombreDelegacion(GenerarReporteDelegacion.IdDelegacion);
                //    using (new Foliacion.Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
                //   {

            DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNominaCheques dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNominaCheques();
                
                dtsRevicionFolios.RutaDelegacionesCheque.AddRutaDelegacionesChequeRow(datosCompletosNomina.Coment, NombreDelegacionSeleccionada, datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina);
                // dtsRevicionFolios.Ruta.AddRutaRow(FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(IdNomina), " " );


                foreach (var resultado in ResumenRevicionNominaPDF)
                {
                    //dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(resultado.Id), resultado.Partida, resultado.Nombre, resultado.Deleg, resultado.Num_Che, resultado.Liquido, resultado.CuentaBancaria, resultado.Num, resultado.Nom);
                    dtsRevicionFolios.DatosRevicionCheque.AddDatosRevicionChequeRow(resultado.Contador, resultado.Partida, resultado.CadenaNumEmpleado, resultado.NombreEmpleado, resultado.Delegacion, resultado.Nomina ,resultado.NUM_CHE, resultado.Liquido, resultado.Cuenta );
                }

                string pathPdf = @"C:\Reporte\FoliacionRevicionPDF";

                if (!Directory.Exists(pathPdf))
                {
                    Directory.CreateDirectory(pathPdf);
                }




                string pathCompleto = pathPdf + "\\" + "RevicionDelegacion"+NombreDelegacionSeleccionada+ "Nomina" + datosCompletosNomina.Id_nom+".pdf";
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNominaCheques.rpt"));
                rd.SetDataSource(dtsRevicionFolios);
                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathCompleto);



                byte[] archivo = ObtenerBytes(pathCompleto);


                archivoBase64 = Convert.ToBase64String(archivo);



                if (System.IO.File.Exists(pathCompleto))
                {
                    System.IO.File.Delete(pathCompleto);
                }

                //}



                return Json(archivoBase64, JsonRequestBehavior.AllowGet);
            }

            public ActionResult RevisarReportePDFChequeIdNomina(int IdNomina , string Quincena)
            {
                int anioInterface =FoliarNegocios.ObtenerAnioDeQuincena(Quincena);

                string visitaAnioInterface = FoliarNegocios.ObtenerCadenaAnioInterface(anioInterface);
                var datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraFILTRO(IdNomina, visitaAnioInterface);

                List<ResumenRevicionNominaPDFModel> ResumenRevicionNominaPDF = new List<ResumenRevicionNominaPDFModel>();

            

                if (datosCompletosNomina.Nomina == "01" || datosCompletosNomina.Nomina == "02")
                {
                    ResumenRevicionNominaPDF = Mapper.Map<List<ResumenRevicionNominaPDFDTO>, List<ResumenRevicionNominaPDFModel>>( FoliarNegocios.ObtenerDetallePersonalRevicionPDF_GeneralDescentralizada( IdNomina, anioInterface));
                }
                else 
                {
                    ResumenRevicionNominaPDF = Mapper.Map<List<ResumenRevicionNominaPDFDTO>, List<ResumenRevicionNominaPDFModel>>(  FoliarNegocios.ObtenerDetallePersonalRevicionPDF_OtrasNominasYPenA(IdNomina, anioInterface) );
                }





                string archivoBase64;
                //    using (new Foliacion.Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
                //   {

                DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNominaCheques dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNominaCheques();

                dtsRevicionFolios.RutaDelegacionesCheque.AddRutaDelegacionesChequeRow(datosCompletosNomina.Coment,"" /*Sin delegacion por ser la nomina completa*/ ,  datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina);
                // dtsRevicionFolios.Ruta.AddRutaRow(FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(IdNomina), " " );


                foreach (var resultado in ResumenRevicionNominaPDF)
                {
                    //dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(resultado.Id), resultado.Partida, resultado.Nombre, resultado.Deleg, resultado.Num_Che, resultado.Liquido, resultado.CuentaBancaria, resultado.Num, resultado.Nom);
                    dtsRevicionFolios.DatosRevicionCheque.AddDatosRevicionChequeRow(resultado.Contador, resultado.Partida, resultado.CadenaNumEmpleado , resultado.NombreEmpleado, resultado.Delegacion, resultado.Nomina ,  resultado.NUM_CHE, resultado.Liquido, resultado.Cuenta);
                }

                string pathPdf = @"C:\Reporte\FoliacionRevicionPDF";

                if (!Directory.Exists(pathPdf))
                {
                    Directory.CreateDirectory(pathPdf);
                }




                string pathCompleto = pathPdf + "\\" + "RevicionNominaCheques" + datosCompletosNomina.Id_nom + ".pdf";
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNominaCheques.rpt"));
                rd.SetDataSource(dtsRevicionFolios);
                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathCompleto);



                byte[] archivo = ObtenerBytes(pathCompleto);


                archivoBase64 = Convert.ToBase64String(archivo);



                if (System.IO.File.Exists(pathCompleto))
                {
                    System.IO.File.Delete(pathCompleto);
                }

                //}



                return Json(archivoBase64, JsonRequestBehavior.AllowGet);
            }

        #endregion











        #region METODOS PARA LA FOLIACION Y LA REVICION POR MEDIO DE PAGOMATICOS 

        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        //************************************      Metodos para FOLIAR nominas con PAGOMATICOS por nomina o todas las nominas      *************************************************//
        //***************************************************************************************************************************************************************************//
        //***************************************************************************************************************************************************************************//
        public async System.Threading.Tasks.Task<ActionResult> FoliarPorIdNominaPagomatico (int IdNomina, string NumeroQuincena) 
        {
            try
            {
                int anioInterfaz = FoliarNegocios.ObtenerAnioDeQuincena(NumeroQuincena);
                //List<AlertasAlFolearPagomaticosDTO> errores = await FoliarNegocios.FolearPagomaticoPorNomina_TIEMPO_DE_RESPUESTA_MEJORADO(IdNomina, anioInterfaz, NumeroQuincena);
                List<AlertasAlFolearPagomaticosDTO> alertasObtenidas = new List<AlertasAlFolearPagomaticosDTO>();
                alertasObtenidas.Add( await FoliarNegocios.FolearPagomaticoPorNominaaAsincrono(IdNomina, anioInterfaz ) );

                var DBFAbierta = alertasObtenidas.Where(x => x.IdAtencion == 4).Select(x => new { x.Id_Nom, x.Detalle, x.Solucion }).ToList();

                if (DBFAbierta.Count() > 0)
                {
                    return Json(new
                    {
                        bandera = false,
                        DBFAbierta = DBFAbierta
                    });
                }
                else
                {
                    return Json(new
                    {
                        bandera = true,
                        resultadoServer = EstanFoliadasTodasNominaPagomatico(NumeroQuincena)
                    });
                }
            }
            catch (Exception E) 
            {
                int a = 0;

                return Json(new
                {
                    bandera = true,
                    resultadoServer = EstanFoliadasTodasNominaPagomatico(NumeroQuincena)
                });
            }
            
        }

        public async System.Threading.Tasks.Task<ActionResult> FoliarQuincenaPagomatico( string NumeroQuincena)
        {
            try
            {
                int anioInterfaz = FoliarNegocios.ObtenerAnioDeQuincena(NumeroQuincena);
                //List<AlertasAlFolearPagomaticosDTO> errores = await FoliarNegocios.FolearPagomaticoPorNomina(IdNomina, anioInterfaz, NumeroQuincena);
              //  List<AlertaDeNominasFoliadasPagomatico> detallesTodasNominas = FoliarNegocios.VerificarTodasNominaPagoMatico(NumeroQuincena).Where(x => x.IdEstaFoliada == 0).ToList();
                List<AlertaDeNominasFoliadasPagomatico> nominasConPagomaticoParaFoliar = FoliarNegocios.VerificarTodasNominaPagoMatico(NumeroQuincena).Where(x => x.IdEstaFoliada == 0).ToList();

                List<AlertasAlFolearPagomaticosDTO> alertasObtenidasFoliarTodasNominasPagomaticos = new List<AlertasAlFolearPagomaticosDTO>();

                
                foreach (AlertaDeNominasFoliadasPagomatico nuevaNomina in nominasConPagomaticoParaFoliar) 
                {
                    alertasObtenidasFoliarTodasNominasPagomaticos.Add( await FoliarNegocios.FolearPagomaticoPorNominaaAsincrono(nuevaNomina.Id_Nom, anioInterfaz) );
                }

                List<AlertasAlFolearPagomaticosDTO> inicenciasEncontradas = alertasObtenidasFoliarTodasNominasPagomaticos.Where(x => x.IdAtencion != 0).ToList();
                List<AlertasAlFolearPagomaticosDTO> casosFoliadosExitos = alertasObtenidasFoliarTodasNominasPagomaticos.Where(x => x.IdAtencion == 0).ToList();


                List<AlertaDeNominasFoliadasPagomatico> nuevoResultadoDespuesFoliacioPagomatico = FoliarNegocios.VerificarTodasNominaPagoMatico(NumeroQuincena);
                if (casosFoliadosExitos.Count() > 0)
                {
                    return Json(new
                    {
                        bandera = true,
                        mensaje  = "ALGUNAS NOMINAS CON PAGOMATICOS SE FOLIARON CORRECTAMENTE REVISE LA INFORMACION COMPLETA DEBAJO DE ESTE MODAL",
                        resultadoServer = nuevoResultadoDespuesFoliacioPagomatico.Where( x => x.IdEstaFoliada < 2)
                    });
                }
                else
                {
                    return Json(new
                    {
                        bandera = false,
                        mensaje = "HUBO INCIDENCIAS AL FOLIAR TODO EL PAQUETE DE NOMINAS DE LA QUINCENA O NO CONTENIAN PAGOMATICOS: '" +NumeroQuincena+"',  SE RECOMIENDA FOLEAR UNA A UNA O INTENTAR DE NUEVO",
                        resultadoServer = nuevoResultadoDespuesFoliacioPagomatico
                    });
                }
            }
            catch (Exception E)
            {
                return Json(new
                {
                    bandera = false,
                    mensaje = E.Message,
                    resultadoServer = EstanFoliadasTodasNominaPagomatico(NumeroQuincena)
                });
            }

        }

        public async Task<ActionResult> ReFoliarPorIdNominaPagomatico(int IdNomina , string NumeroQuincena) 
        {
            try
            {
                //List<AlertasAlFolearPagomaticosDTO> errores = await FoliarNegocios.FolearPagomaticoPorNomina_TIEMPO_DE_RESPUESTA_MEJORADO(IdNomina, anioInterfaz, NumeroQuincena);
                List<AlertasAlFolearPagomaticosDTO> errores = await FoliarNegocios.RefoliarFolearPagomaticoPorNominaaAsincrono(IdNomina,  NumeroQuincena);

                var DBFAbierta = errores.Where(x => x.IdAtencion == 4).Select(x => new { x.Id_Nom, x.Detalle, x.Solucion }).ToList();

                if (DBFAbierta.Count() > 0)
                {
                    return Json(new
                    {
                        bandera = false,
                        DBFAbierta = DBFAbierta
                    });
                }
                else
                {
                    return Json(new
                    {
                        bandera = true,
                        resultadoServer = EstanFoliadasTodasNominaPagomatico(NumeroQuincena)
                    });
                }
            }
            catch (Exception E)
            {
                int a = 0;

                return Json(new
                {
                    bandera = true,
                    resultadoServer = EstanFoliadasTodasNominaPagomatico(NumeroQuincena)
                });
            }
        }

        //******************************************************************************************************************************************************************************************************//
        //*******************************************************************************************************************************************************************************************************//
        //*******************************************************          VERFIFICAR LAS NOMINAS SI YA ESTAN FOLIADAS O NO  (VERIFICA en SQL)    ***************************************************************//
        //*******************************************************************************************************************************************************************************************************//
        //*******************************************************************************************************************************************************************************************************//
        /*******  Pinta tabla con el detalle de una nomina para saber si esta foliada y su detalle para pagomaticos  ******/
        public ActionResult EstaFoliadaIdNominaPagomatico(int IdNom, string NumeroQuincena) 
        {
            int anioInterface = FoliarNegocios.ObtenerAnioDeQuincena(NumeroQuincena);
            List<AlertaDeNominasFoliadasPagomatico> resultadoAlertas = new List<AlertaDeNominasFoliadasPagomatico>();
            resultadoAlertas.Add(FoliarNegocios.EstaFoliadaNominaSeleccionadaPagoMatico(IdNom, anioInterface));

           // return Json(resultadoAlertas, JsonRequestBehavior.AllowGet);
            if (resultadoAlertas.Count() > 0)
            {
                return Json(new
                {
                    RespuestaServidor = "201",
                    DetalleTabla = resultadoAlertas.OrderBy(x => x.Id_Nom)
                }) ;

                // respuestaServer = "201";
            }
            else
            {
                return Json(new
                {
                    RespuestaServidor = "500",
                    Error = "ERROR AL CARGAR LOS DETALLES DE TODAS LAS NOMINAS, 'INTENTE DE NUEVO' "
                });
                //respuestaServer = "500";

            }
        }

        /****** pinta una tabla con el detalle de todas las nominas para saber si estan foliadas y sus detalles ******/
        public ActionResult EstanFoliadasTodasNominaPagomatico(string NumeroQuincena)
        {
           // List<AlertaDeNominasFoliadasPagomatico> detallesTodasNominas = FoliarNegocios.VerificarTodasNominaPagoMatico(NumeroQuincena).Where(x => x.IdEstaFoliada != 2 ).ToList();
            List<AlertaDeNominasFoliadasPagomatico> detallesTodasNominas = FoliarNegocios.VerificarTodasNominaPagoMatico(NumeroQuincena).ToList().OrderBy( x=> x.NumeroNomina).ThenBy( x => x.Id_Nom).ToList();


            if (detallesTodasNominas.Count() > 0)
            {
                return Json(new
                {
                    RespuestaServidor = "201",
                    DetalleTabla = detallesTodasNominas
                });

                // respuestaServer = "201";
            }
            else
            {
                return Json(new
                {
                    RespuestaServidor = "500",
                    Error = "ERROR AL CARGAR LOS DETALLES DE TODAS LAS NOMINAS, 'INTENTE DE NUEVO' " 
                });
                //respuestaServer = "500";

            }

        }

        public ActionResult VerificarNominasQuincaDisponiblesFoliarPagomaticos(string NumeroQuincena) 
        {
            List<AlertaDeNominasFoliadasPagomatico> nominasConPagomaticoParaFoliar = FoliarNegocios.VerificarTodasNominaPagoMatico(NumeroQuincena).Where(x => x.IdEstaFoliada < 2).ToList();
            return Json(nominasConPagomaticoParaFoliar, JsonRequestBehavior.AllowGet);
        }



        //*****************************************************************************************************************************************************************************************************************//
        //*****************************************************************************************************************************************************************************************************************//
        //************************************          GENERA REPORTE EN PDF DONDE SE VISUALIZA EL CADA EMPLEADO COMO SE ENCUENTRA EN SQL PARA VERIFICAR SI ESTA BIEN FOLIADO O NO    ************************************//
        //*****************************************************************************************************************************************************************************************************************//
        //*****************************************************************************************************************************************************************************************************************//
        public ActionResult RevisarReportePDFPagomaticoPorIdNomina(int IdNomina, string Quincena)
        {
            string archivoBase64 = "no entre";

            int anio = FoliarNegocios.ObtenerAnioDeQuincena(Quincena);

            string visitaAnioInterface = FoliarNegocios.ObtenerCadenaAnioInterface(anio);
            var datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraFILTRO(IdNomina, visitaAnioInterface);

            var resumenRevicionNominaReportePDF = FoliarNegocios.ObtenerDatosPersonalesNominaReportePagomatico(datosCompletosNomina.An, anio, datosCompletosNomina.Nomina);



            DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina dtsRevicionFolios = new DAP.Plantilla.Reportes.Datasets.RevicionDeFoliacionPorNomina();

            dtsRevicionFolios.Ruta.AddRutaRow(datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina, datosCompletosNomina.Coment);
            // dtsRevicionFolios.Ruta.AddRutaRow(FoliarNegocios.ObtenerRutaCOmpletaArchivoIdNomina(IdNomina), " " );


            foreach (var resultado in resumenRevicionNominaReportePDF)
            {
                //dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(Convert.ToString(resultado.Id), resultado.Partida, resultado.Nombre, resultado.Deleg, resultado.Num_Che, resultado.Liquido, resultado.CuentaBancaria, resultado.Num, resultado.Nom);
                dtsRevicionFolios.DatosRevicion.AddDatosRevicionRow(resultado.Contador, resultado.Partida, resultado.NombreEmpleado, resultado.Delegacion, resultado.NUM_CHE, resultado.Liquido, resultado.Cuenta, resultado.CadenaNumEmpleado, resultado.Nomina , resultado.Suspencion);
            }

            string pathPdf = "C:\\Reporte\\FoliacionRevicionPDF";

            if (!Directory.Exists(pathPdf))
            {
                Directory.CreateDirectory(pathPdf);
            }


            // string a = Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/RevicionFoliacionNomina.rpt");


            string pathCompleto = pathPdf + "\\" + "RevicionNomina" + IdNomina + ".pdf";

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reportes/Crystal"), "RevicionFoliacionNomina.rpt"));
            rd.SetDataSource(dtsRevicionFolios);
            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, "C:\\Reporte\\FoliacionRevicionPDF\\" + "RevicionNomina" + IdNomina + ".pdf");



            byte[] archivo = ObtenerBytes("C:\\Reporte\\FoliacionRevicionPDF\\" + "RevicionNomina" + IdNomina + ".pdf");


            //// if (archivo != null) 
            archivoBase64 = Convert.ToBase64String(archivo);



            if (System.IO.File.Exists(pathCompleto))
            {
                System.IO.File.Delete(pathCompleto);
            }
            rd.Close();
            rd.Dispose();
            dtsRevicionFolios.Dispose();



            return Json(archivoBase64, JsonRequestBehavior.AllowGet);
        }

        #endregion




        public byte[] ObtenerBytes(string path) => System.IO.File.ReadAllBytes(path);

        public int ObtenerAnioDeQuincena(string Quincena)
        {
            return Convert.ToInt32(DateTime.Now.Year.ToString().Substring(0, 2) + Quincena.Substring(0, 2));
        }


  






        //**********************************************************************************************************************************************************************************************************************//
        //****************************************************         Metodo para Recuperar Cheques (Recrese el cheque a un estado inicial cuando sucede un error humano de foliacion)         ********************************//
        //**********************************************************************************************************************************************************************************************************************//
        public ActionResult BuscarChequesARecuperar(int IdCuentaBancaria , int RangoInicial , int RangoFinal )
        {
            return Json(FoliarNegocios.BuscarFormasPagoCoincidentes(IdCuentaBancaria, RangoInicial, RangoFinal).ToList(), JsonRequestBehavior.AllowGet);
        }






        //**********************************************************************************************************************************************************************************************************************//
        //****************************************************         Restaura el folio de un cheque de un idPago para devolvelo al inventario como si nunca hubo tenido una incidencia       ********************************//
        public ActionResult RestaurarFolioChequeDeIdPagoSeleccionado(int IdPago)
        {
            string respuestaServer = FoliarNegocios.RestaurarFolioChequeDeIdPago(IdPago);

            string mostrarTexto ="";
            int resultado = 0;
            if (respuestaServer.Contains("CORRECTO"))
            {
                //Ok
                resultado = 200;
            }
            else if (respuestaServer.Contains("El folio seleccionado")) 
            {
                resultado = 500;
                mostrarTexto = respuestaServer;
            }
            else if(respuestaServer.Contains("LA BASE : ||"))
            {
                //error
                resultado = 500;
                mostrarTexto = respuestaServer;
            }
            else
            {
                //error
                resultado = 500;
                mostrarTexto = "Intentelo de nuevo mas tarde , No se pudo recuperar el folio seleccionado";
            }



            return Json(new
            {
                resultServer = resultado,
                texto = mostrarTexto
            });

        }
    }


}