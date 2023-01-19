using AutoMapper;
using DAP.Foliacion.Negocios;
using DAP.Plantilla.Models.CrearReferencia_CanceladosModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO;
using System.IO;
using System.Threading.Tasks;
using CrystalDecisions.CrystalReports.Engine;
using System.IO.Compression;
using DAP.Foliacion.Entidades.DTO.CrearReferencia_CanceladosDTO.ReporteCCancelados.IPD;
using DAP.Plantilla.Models.PermisosModels;
using DAP.Foliacion.Plantilla.Filters;

namespace DAP.Plantilla.Controllers
{
    public class CrearReferencia_CanceladosController : Controller
    {
        [SessionSecurityFilter]
        public ActionResult Index()
        {
            //  List<CrearReferenciaDTO> listaEncontrada =  CrearReferencia_CanceladosNegocios.ObtenerReferenciasAnioActual(DateTime.Now.Year);

            List<ReferenciaCanceladoModel> listaEncontrada = Mapper.Map<List<CrearReferenciaDTO>, List<ReferenciaCanceladoModel>>(CrearReferencia_CanceladosNegocios.ObtenerReferenciasAnioActual(DateTime.Now.Year));

            ViewBag.ReferenciasCancelaciones = listaEncontrada;


            // ************************************************************************************************************* //
            // EL MODELO QUE SE ENVIA ES PARA QUE EL SIDEBAR CONTENGA LOS LINK’S REFERENTE A LOS PERMISOS QUE PUEDE VISITAR //
            // ************************************************************************************************************ //
            return View();
        }


        public ActionResult DetallesReferenciaSelecionada(string NumeroQuincena)
        {
            

            return PartialView();
        }








        [HttpPost]
        public ActionResult CrearReferenciaCancelado(int NuevoNumeroReferencia) 
        {
            bool bandera = false;
            int idDevuelto = CrearReferencia_CanceladosNegocios.CrearNuevaReferenciaCancelados(Convert.ToString(NuevoNumeroReferencia));

            if (idDevuelto > 0)
            {
                bandera = true;
            }

            return Json(bandera, JsonRequestBehavior.AllowGet);
        }


        public ActionResult InactivarReferenciaCancelado(int IdReferenciaCancelacion)
        {
            bool bandera = false;
            //Obtener cuantos cheques hay en la referencia a cancelar
            int totalChequesCargadosReferencia = CrearReferencia_CanceladosNegocios.ObtenerTotalChequesEnReferenciaCancelacion(IdReferenciaCancelacion);

            int idDevuelto = CrearReferencia_CanceladosNegocios.InactivarReferenciaCancelados(IdReferenciaCancelacion);

            if (totalChequesCargadosReferencia == idDevuelto) 
            {
                bandera = true;
            }


            return Json(new
            {
                bandera = bandera,
                respuestaServer = idDevuelto
            });
        }





        [HttpPost]
        public JsonResult ImportarArchivo(string FolioDocumento, int FinalizarIdReferencia)
        {

            var archivo = Request.Files[0];

            byte[] ArchivoBLOB = ReadFully(archivo.InputStream);
            int totalChequesCargadosReferencia = CrearReferencia_CanceladosNegocios.ObtenerTotalChequesEnReferenciaCancelacion(FinalizarIdReferencia);
            
            int totalChequesCancelados = 0;
            bool bandera = false;
            string mensaje = "";
            if (totalChequesCargadosReferencia == 0)
            {
                mensaje = "No se puede finalizar una referencia que no contiene formas de pagos cargadas en ella.";
            }
            else 
            {
                 totalChequesCancelados = CrearReferencia_CanceladosNegocios.FinalizarIdReferenciaCancelacion(FinalizarIdReferencia, FolioDocumento, ArchivoBLOB);

               
                if (totalChequesCargadosReferencia == totalChequesCancelados)
                {
                    bandera = true;
                }

            }



            return Json(new
            {
                bandera = bandera,
                mensaje = mensaje,
                respuestaServer = totalChequesCancelados
            });
        }




        /*******************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************/
        /****************************************           Metodoa para ver detalles de la referencia o anular su cancelacion          *******************************************************/
        public JsonResult ObtenerDetalleReferenciasParaModal(int IdReferencia) 
        {
            var detallesDentroReferencia = CrearReferencia_CanceladosNegocios.ObtenerDetallesDentroIdReferencia(IdReferencia);

            bool bandera = false;
            
            if (detallesDentroReferencia.Count() > 0)
            {
                bandera = true;
            }


            return Json(new
            {
                bandera = bandera,
                respuestaServer = detallesDentroReferencia
            });
        }



        public JsonResult AnularCancelacion(int IdPago)
        {
            bool bandera = CrearReferencia_CanceladosNegocios.AnularCancelacion(IdPago);
            

            return Json(new
            {
                bandera = bandera
            });
        }






        /******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /****************************************           Metodo para ver obtener el previw del documento que avala la cancelacion de cheques          *******************************************************/
        public JsonResult ObtenerPdfReferenciaCancelada(int IdReferenciaCancelado) 
        {
           bool bandera = false;
           byte [] documentoEncontrado =  CrearReferencia_CanceladosNegocios.ObtenerBytesDocumentoXIdReferencia(IdReferenciaCancelado);


            string archivoBase64 = "";
            if (documentoEncontrado != null) 
            {
                archivoBase64 = Convert.ToBase64String(documentoEncontrado);
                bandera = true;
            }

            return Json(new
            {
                bandera = bandera,
                respuestaServer = archivoBase64
            });
        }








        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }




        /******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /****************************************                            Metodos para Descargar Reportes Basico o principales de cheques cancelados                          *******************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/

        public ActionResult DescargarReporteNominaAnual_AXIOS(int IdReferencia)
        {
            byte[] bytes = null;

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferencia);

            string pathACrear = Path.Combine(Server.MapPath("~/"), @"Reportes\IpdParaDescargas\ReporteNominaAnual");

            //string pathACrear = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas\ReporteNominaAnual";

            string pathdestino = null;
            string slash = @"\";
            string ultimosDosDigitosAnio;
            string  referenciaSinAnio;
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                    //var registrosPorAnio = pagosEncontrados.Where(x => x.Anio == 2022).ToList();

                    var nominaAnualRegistros = CrearReferencia_CanceladosNegocios.ObtenerRegistrosNominaAnual(IdReferencia, anioSeleccionado);

                    decimal sumaTotalGeneralSinCAPAE = 0;
                    DAP.Plantilla.Reportes.Datasets.DsNominaAnual nominaAnual = new DAP.Plantilla.Reportes.Datasets.DsNominaAnual();

                    ultimosDosDigitosAnio = anioSeleccionado.ToString().Substring(2,2);
                    referenciaSinAnio = datosReferencia.Numero_Referencia.Substring(2);
                    nominaAnual.datosReferencia.AdddatosReferenciaRow(ultimosDosDigitosAnio + referenciaSinAnio, Convert.ToString(anioSeleccionado));
                    foreach (var nuevoregistro in nominaAnualRegistros)
                    {
                       
                        nominaAnual.NominaAnual.AddNominaAnualRow(nuevoregistro.Quincena, nuevoregistro.Cheque, nuevoregistro.Num, nuevoregistro.NombreBenefirioCheque, nuevoregistro.Deleg, nuevoregistro.Liquido, nuevoregistro.NombreNomina);

                        if (!nuevoregistro.NombreNomina.Contains("XX - NOMINAS DE CAPAE"))
                        {
                            sumaTotalGeneralSinCAPAE += nuevoregistro.Liquido;
                        }
                    }

                    nominaAnual.Nomina.AddNominaRow("", sumaTotalGeneralSinCAPAE);
                    pathdestino = pathACrear + slash + "NominaAnual" + anioSeleccionado + ".pdf";
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/ReporteNominaAnualCC.rpt"));
                    rd.SetDataSource(nominaAnual);
                    rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathdestino);
                }

                string server = Server.MapPath("~/");
                string pathZip = server + @"Reportes\IpdParaDescargas\NominasAnuales"+datosReferencia.Numero_Referencia+".Zip";


                try
                {

                    if (Response.IsClientConnected)
                    {

                        if (!Directory.Exists(pathZip))
                        {
                            ZipFile.CreateFromDirectory(pathACrear, pathZip);
                            Response.Clear();
                            Response.BufferOutput = false;
                           // Response.ContentType = "application/zip";
                            //Response.ContentType = "application/octet-stream";

                            //Response.AppendHeader("content-disposition", "attachment; filename= ReporteAnualesCC_"+datosReferencia.Numero_Referencia+".zip");
                            // AddHeader("content-disposition", "attachment; filename=  Partida_" + partidasADescargar + "_.zip");
                            Response.TransmitFile(pathZip);
                            Response.Flush();
                            Response.End();

                        }
                    }
                }
                catch (Exception E)
                {
                    return Content(E.Message);
                }
                finally
                {
                    if (System.IO.File.Exists(pathZip))
                    {
                        // bytes = System.IO.File.ReadAllBytes(pathZip);
                        System.IO.File.Delete(pathZip);
                        Directory.Delete(pathACrear, true);
                        Response.Close();
                    }

                }

            }

            return Content("No hay datos que cargar");
        }

  

        public ActionResult DescargarReporteCuentaBancariaAnual_AXIOS(int IdReferencia)
        {
            byte[] bytes = null;

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferencia);
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), @"Reportes\IpdParaDescargas");
            //string pathPrincipal = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas";

            string pathACrear = Path.Combine(Server.MapPath("~/"), @"Reportes\IpdParaDescargas\ReporteCuentaBancariaAnual");
            //string pathACrear = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas\ReporteCuentaBancariaAnual";

            string pathdestino = null;
            string slash = @"\";
            string ultimosDosDigitosAnio;
            string referenciaSinAnio;
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                    //var registrosPorAnio = pagosEncontrados.Where(x => x.Anio == 2022).ToList();

                    //  var nominaAnualRegistros = CrearReferencia_CanceladosNegocios.ObtenerRegistrosNominaAnual(IdReferencia, anioSeleccionado);
                    var registrosCuentabancariaObtenidos = CrearReferencia_CanceladosNegocios.ObtenerRegistrosBancosAnual(IdReferencia, anioSeleccionado);

                    DAP.Plantilla.Reportes.Datasets.DSCuentaBancariaAnual_CC cuentaBancariaAnual = new DAP.Plantilla.Reportes.Datasets.DSCuentaBancariaAnual_CC();
                    //cuentaBancariaAnual.DatosReferencia.AddDatosReferenciaRow(datosReferencia.Numero_Referencia, Convert.ToString(anioSeleccionado));

                    ultimosDosDigitosAnio = anioSeleccionado.ToString().Substring(2, 2);
                    referenciaSinAnio = datosReferencia.Numero_Referencia.Substring(2);
                    cuentaBancariaAnual.DatosReferencia.AddDatosReferenciaRow(ultimosDosDigitosAnio+referenciaSinAnio, Convert.ToString(anioSeleccionado));
                    foreach (var nuevoregistro in registrosCuentabancariaObtenidos)
                    {
                        cuentaBancariaAnual.NominaPorBanco.AddNominaPorBancoRow(nuevoregistro.NombreNomina, nuevoregistro.SumaLiquido, nuevoregistro.TotalRegistros, nuevoregistro.NombreCuentaBanco);
                    }

                    pathdestino = pathACrear + slash + "CuentaBancariaAnual" + anioSeleccionado + ".pdf";
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/ReporteCuentaBancariaAnualCC.rpt"));
                    rd.SetDataSource(cuentaBancariaAnual);
                    rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathdestino);
                }

                string pathZip = pathPrincipal + slash + "CuentasBancariasAnuales_" + datosReferencia.Numero_Referencia + ".Zip";


                try
                {

                    if (Response.IsClientConnected)
                    {

                        if (!Directory.Exists(pathZip))
                        {
                            ZipFile.CreateFromDirectory(pathACrear, pathZip);
                            Response.Clear();
                            Response.BufferOutput = false;
                            //Response.ContentType = "application/zip";
                            //Response.ContentType = "application/octet-stream";

                           // Response.AppendHeader("content-disposition", "attachment; filename= ReporteAnualesCC_" + datosReferencia.Numero_Referencia + ".zip");
                            // AddHeader("content-disposition", "attachment; filename=  Partida_" + partidasADescargar + "_.zip");
                            Response.TransmitFile(pathZip);
                            Response.Flush();
                            Response.End();

                        }
                    }
                }
                catch (Exception E)
                {
                    return Content(E.Message);
                }
                finally
                {
                    if (System.IO.File.Exists(pathZip))
                    {
                        // bytes = System.IO.File.ReadAllBytes(pathZip);
                        System.IO.File.Delete(pathZip);
                        Directory.Delete(pathACrear, true);
                        Response.Close();
                    }

                }



            }



            return Content("No hay datos que cargar");
        }


        public ActionResult DescargarReportePensionAlimenticia_AXIOS(int IdReferencia)
        {
            byte[] bytes = null;

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferencia);
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), @"Reportes\IpdParaDescargas");
            string pathACrear = Path.Combine(Server.MapPath("~/"), @"Reportes\IpdParaDescargas\ReportePensionAlimenticia");

            string pathdestino = null;
            string slash = @"\";
            string ultimosDosDigitosAnio;
            string referenciaSinAnio;
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                
                foreach (int anioRecorrido in aniosContenidos) 
                {
                    List<int> anioSeleccionado = new List<int>();
                    anioSeleccionado.Add(anioRecorrido);

                    var registrosPensionAlimenticia = CrearReferencia_CanceladosNegocios.ObtenerRegistrosPensionAlimenticia(IdReferencia, anioSeleccionado);

                    if (registrosPensionAlimenticia.Count() > 0) 
                    {
                       
                        decimal sumaGeneral = 0;
                        DAP.Plantilla.Reportes.Datasets.DsPensionAlimenticiaCC dsRepoPensionAlimenticia = new DAP.Plantilla.Reportes.Datasets.DsPensionAlimenticiaCC();

                        ultimosDosDigitosAnio = anioRecorrido.ToString().Substring(2, 2);
                        referenciaSinAnio = datosReferencia.Numero_Referencia.Substring(2);

                        //dsRepoPensionAlimenticia.DatosReferencia.AddDatosReferenciaRow(datosReferencia.Numero_Referencia, ""+anioRecorrido+"");

                        dsRepoPensionAlimenticia.DatosReferencia.AddDatosReferenciaRow(ultimosDosDigitosAnio + referenciaSinAnio, "" + anioRecorrido + "");
                        foreach (var nuevoregistro in registrosPensionAlimenticia)
                        {
                            sumaGeneral += nuevoregistro.Liquido;
                            dsRepoPensionAlimenticia.PensionAlimenticia.AddPensionAlimenticiaRow(nuevoregistro.Ramo, nuevoregistro.Unidad, nuevoregistro.Quincena, nuevoregistro.Num_che, nuevoregistro.NumEmpleado, nuevoregistro.NombreBeneficiario, nuevoregistro.Delegacion, nuevoregistro.Liquido);
                        }
                        dsRepoPensionAlimenticia.General.AddGeneralRow(sumaGeneral);

                        pathdestino = pathACrear + slash + "PensionAlimenticia_"+datosReferencia.Numero_Referencia+"_"+anioRecorrido+".pdf";
                        ReportDocument rd = new ReportDocument();
                        rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/ReportePensionAlimenticiaCC.rpt"));
                        rd.SetDataSource(dsRepoPensionAlimenticia);
                        rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathdestino);

                    }


                }


                CrearReferencia_CanceladosNegocios.VerificarNumeroArchivosEnDirectorio(pathACrear);

                string pathZip = pathPrincipal + slash + "PensionesAlimenticias_" + datosReferencia.Numero_Referencia + ".Zip";



                try
                {

                    if (Response.IsClientConnected)
                    {

                        if (!Directory.Exists(pathZip))
                        {
                            ZipFile.CreateFromDirectory(pathACrear, pathZip);
                            Response.Clear();
                            Response.BufferOutput = false;
                            //Response.ContentType = "application/zip";
                            //Response.ContentType = "application/octet-stream";

                            // Response.AppendHeader("content-disposition", "attachment; filename= ReporteAnualesCC_" + datosReferencia.Numero_Referencia + ".zip");
                            // AddHeader("content-disposition", "attachment; filename=  Partida_" + partidasADescargar + "_.zip");
                            Response.TransmitFile(pathZip);
                            Response.Flush();
                            Response.End();

                        }
                    }
                }
                catch (Exception E)
                {
                    return Content(E.Message);
                }
                finally
                {
                    if (System.IO.File.Exists(pathZip))
                    {
                        // bytes = System.IO.File.ReadAllBytes(pathZip);
                        System.IO.File.Delete(pathZip);
                        Directory.Delete(pathACrear, true);
                        Response.Close();
                    }

                }

            }

            return Content("No hay datos que cargar");
        }



        /******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /****************************************                            Metodos para Descargar Reportes del IPD y el propio IPD                           *******************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/




        public ActionResult DescargarIPDPorAnio_AXIOS(int IdReferencia)
        {
            byte[] bytes = null;

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferencia);
            //string pathPrincipal = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas";
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas");
            string pathACrear = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas/IPDxAnio");
            //string pathACrear = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas\ReporteTotalGeneralConceptosPorNomina";

            string pathOrigen = pathPrincipal + "/IPDVacio.dbf";

            string slash = @"\";
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                    string dosDigitosAnio = Convert.ToString(anioSeleccionado).Substring(2, 2);
                    string referenciaSinAnio = datosReferencia.Numero_Referencia.Substring(2);

                    string nombreArchivoDBFAnual = "IPD" + dosDigitosAnio + referenciaSinAnio;
                    string pathDestino = pathACrear + "/" + nombreArchivoDBFAnual + ".dbf";

                    bool resultadoBandera = CrearReferencia_CanceladosNegocios.GeneralNuevoIPDxAnio(pathPrincipal, pathOrigen, pathDestino, anioSeleccionado, nombreArchivoDBFAnual, datosReferencia.Numero_Referencia, IdReferencia);

                    if (!resultadoBandera)
                    {   TextWriter Escribir = new StreamWriter(pathACrear + "/LEER_ERRORES_IPD"+anioSeleccionado+".txt");
                        Escribir.WriteLine("Ocurrio un error al general el IPD: "+datosReferencia.Numero_Referencia+"_"+anioSeleccionado+", intente de nuevo o contacte con un desarrollador");
                        Escribir.Close();
                    }
                }

                string pathZip = pathPrincipal + slash + "IPD_Anuales_" + datosReferencia.Numero_Referencia + ".Zip";


                try
                {

                    if (Response.IsClientConnected)
                    {

                        if (!Directory.Exists(pathZip))
                        {
                            ZipFile.CreateFromDirectory(pathACrear, pathZip);
                            Response.Clear();
                            Response.BufferOutput = false;
                            // Response.ContentType = "application/zip";
                            //Response.ContentType = "application/octet-stream";

                            //Response.AppendHeader("content-disposition", "attachment; filename= IPD_Anuales_"+datosReferencia.Numero_Referencia+".zip");
                            // AddHeader("content-disposition", "attachment; filename=  Partida_" + partidasADescargar + "_.zip");
                            Response.TransmitFile(pathZip);
                            Response.Flush();
                            Response.End();

                        }
                    }
                }
                catch (Exception E)
                {
                    return Content(E.Message);
                    Response.Close();
                }
                finally
                {
                    if (System.IO.File.Exists(pathZip))
                    {
                       // bytes = System.IO.File.ReadAllBytes(pathZip);
                        System.IO.File.Delete(pathZip);
                        Directory.Delete(pathACrear, true);
                        Response.Close();
                    }

                }

            }

            return Content("No hay datos que cargar");
        }



        public FileContentResult DescargarTGCxNomina(int IdReferencia) 
        {
            byte[] bytes = null;

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferencia);
            //string pathPrincipal = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas";
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas");
            string pathACrear = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas/ReporteTotalGeneralConceptosPorNomina");
            //string pathACrear = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas\ReporteTotalGeneralConceptosPorNomina";


            string pathdestino = null;
            string slash = @"\";
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                   string pathACrearxAnio = pathACrear + "//"+anioSeleccionado+"";

                    Directory.CreateDirectory(pathACrearxAnio);
                    if (Directory.Exists(pathACrearxAnio))
                    {
                        List<string> listaConsutalParaTGCXNomina = CrearReferencia_CanceladosNegocios.ObtenerConsultasReportesIPD(IdReferencia, anioSeleccionado, "IPD", "TGCNomina");
                        foreach (string consulta in listaConsutalParaTGCXNomina)
                        {
                            List<RegistrosTGCxNominaDTO> registrosObtenidos = CrearReferencia_CanceladosNegocios.ObtenerRegistrosTGCxNomina(consulta);
                            DAP.Plantilla.Reportes.Datasets.DsReportesCC.DsIPD.DsTGCxNomina dsTGCxNomina = new Reportes.Datasets.DsReportesCC.DsIPD.DsTGCxNomina();

                            if (registrosObtenidos.Count() > 0)
                            {
                                Reportes.Datasets.DsReportesCC.DsIPD.DsTGCxNomina.TGCxNominaPercepcionesRow prueba = null; 
                               dsTGCxNomina.DatosReferencia.AddDatosReferenciaRow(datosReferencia.Numero_Referencia, "" + anioSeleccionado + "", registrosObtenidos.FirstOrDefault().NombreNomina);
                                var TGCxNomina = CrearReferencia_CanceladosNegocios.TotalesGeneralesXConcepto(IdReferencia, anioSeleccionado, registrosObtenidos).OrderBy(x => x.RU).ThenByDescending(x => x.EsPercepcion);

                                decimal ppMontoPositivo = 0M;
                                decimal ppMontoNegativo = 0M;

                                decimal ddMontoPositivo = 0M;
                                decimal ddMontoNegativo = 0M;
                                decimal TotalLiquido = 0M;

                                string ruGuardado = "";
                                
                                foreach (var nomina in TGCxNomina)
                                {
                                    if (ruGuardado != nomina.RU) 
                                    {
                                        ppMontoPositivo = 0M;
                                        ppMontoNegativo = 0M;

                                        ddMontoPositivo = 0M;
                                        ddMontoNegativo = 0M;
                                        TotalLiquido = 0M;
                                    }



                                    if (nomina.EsPercepcion == true)
                                    {
                                        ppMontoPositivo += nomina.MontoPositivo;
                                        ppMontoNegativo += nomina.MontoNegativo;
                                        TotalLiquido = (ppMontoPositivo + ddMontoNegativo) - (ppMontoNegativo - ddMontoPositivo);
                                        dsTGCxNomina.TGCxNominaPercepciones.AddTGCxNominaPercepcionesRow(nomina.Clave, nomina.Descripcion, nomina.MontoPositivo, nomina.MontoNegativo, nomina.RU, "", ppMontoPositivo, ppMontoNegativo, ddMontoPositivo, ddMontoNegativo, TotalLiquido );
                                    

                                    }
                                    else if (nomina.EsPercepcion == false)
                                    {
                                        nomina.MontoPositivo = nomina.MontoPositivo > 0 ? nomina.MontoPositivo * -1 : nomina.MontoPositivo;
                                        nomina.MontoNegativo = nomina.MontoNegativo < 0 ? nomina.MontoNegativo * -1 : nomina.MontoNegativo;
                                        ddMontoPositivo += nomina.MontoPositivo;
                                        ddMontoNegativo += nomina.MontoNegativo;

                                        TotalLiquido = (ppMontoPositivo + ddMontoNegativo) - (ppMontoNegativo - ddMontoPositivo);
                                        dsTGCxNomina.TGCxNominaPercepciones.AddTGCxNominaPercepcionesRow("", nomina.Descripcion, nomina.MontoPositivo, nomina.MontoNegativo, nomina.RU, nomina.Clave, ppMontoPositivo, ppMontoNegativo, ddMontoPositivo, ddMontoNegativo, TotalLiquido);
                                    
                                        
                                    }
                                    ruGuardado = nomina.RU;

                                }



                                pathdestino = pathACrearxAnio + slash + "TGCxNomina" + anioSeleccionado + "_" + registrosObtenidos.FirstOrDefault().NombreNomina + ".pdf";
                                ReportDocument rd = new ReportDocument();
                                rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/ReportesCC/IPD/ReporteTGCxNomina.rpt"));
                                rd.SetDataSource(dsTGCxNomina);
                                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathdestino);

                            }

                        }


                    }



                }

                string pathZip = pathPrincipal + slash + "ReporteTotalGeneralConceptosPorNomina_" + datosReferencia.Numero_Referencia + ".Zip";


                ZipFile.CreateFromDirectory(pathACrear, pathZip);

                if (System.IO.File.Exists(pathZip))
                {
                    bytes = System.IO.File.ReadAllBytes(pathZip);
                    System.IO.File.Delete(pathZip);
                    Directory.Delete(pathACrear, true);
                }
            }

            /**/


            // byte[] bytes = null;
            return File(bytes, "application/zip", "ReporteTotalGeneralConceptosPorNomina_Anuales_"+datosReferencia.Numero_Referencia+".Zip");
        }


        public FileContentResult DescargarTGCxBanco(int IdReferencia) 
        {
            byte[] bytes = null;
            return File(bytes, "application/pdf", "PensionAlimenticia_" + 1 + ".Pdf");
        }



        /******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /****************************************                            Metodos para Descargar el IPD_Compensado                          *******************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/

   
        public ActionResult DescargarIPDCompensadoPorAnio_AXIOS(int IdReferencia)
        {
            byte[] bytes = null;

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferencia);
            //string pathPrincipal = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas";
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas");
            string pathACrear = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas/IPDCompensadoxAnio");
            //string pathACrear = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas\ReporteTotalGeneralConceptosPorNomina";

            string pathOrigen = pathPrincipal + "/IPDCVacio.dbf";

            string slash = @"\";
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                    string dosDigitosAnio = Convert.ToString(anioSeleccionado).Substring(2, 2);
                    string referenciaSinAnio = datosReferencia.Numero_Referencia.Substring(2);

                    string nombreArchivoDBFAnual = "IPD" + dosDigitosAnio + referenciaSinAnio + "c";

                    string pathDestino = pathACrear + "/" + nombreArchivoDBFAnual + ".dbf";

                    bool resultadoBandera = CrearReferencia_CanceladosNegocios.GeneralNuevoIPDCxAnio(pathACrear, pathOrigen, pathDestino, anioSeleccionado, nombreArchivoDBFAnual, datosReferencia.Numero_Referencia, IdReferencia);

                }

                string pathZip = pathPrincipal + slash + "IPDC_Anuales_" + datosReferencia.Numero_Referencia + ".Zip";
               // ZipFile.CreateFromDirectory(pathACrear, pathZip);




                try
                {

                    if (Response.IsClientConnected)
                    {

                        if (!Directory.Exists(pathZip))
                        {
                            ZipFile.CreateFromDirectory(pathACrear, pathZip);
                            Response.Clear();
                            Response.BufferOutput = false;
                            Response.ContentType = "application/zip";
                            //Response.ContentType = "application/octet-stream";

                            //Response.AppendHeader("content-disposition", "attachment; filename= IPDCompensados_Anuales_"+datosReferencia.Numero_Referencia+".Zip");
                            // AddHeader("content-disposition", "attachment; filename=  Partida_" + partidasADescargar + "_.zip");
                            Response.TransmitFile(pathZip);
                            Response.Flush();
                            Response.End();

                        }
                    }
                }
                catch (Exception E)
                {
                    return Content(E.Message);
                }
                finally
                {
                    if (System.IO.File.Exists(pathZip))
                    {
                        // bytes = System.IO.File.ReadAllBytes(pathZip);
                        System.IO.File.Delete(pathZip);
                        Directory.Delete(pathACrear, true);
                        Response.Close();
                    }

                }

            }
            return Content("No hay datos que cargar");
        }


    }
}