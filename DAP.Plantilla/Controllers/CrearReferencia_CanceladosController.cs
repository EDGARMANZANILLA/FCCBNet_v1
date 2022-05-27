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

namespace DAP.Plantilla.Controllers
{
    public class CrearReferencia_CanceladosController : Controller
    {
        // GET: CrearReferencia_Cancelados
        public ActionResult Index()
        {
            //  List<CrearReferenciaDTO> listaEncontrada =  CrearReferencia_CanceladosNegocios.ObtenerReferenciasAnioActual(DateTime.Now.Year);

            List<ReferenciaCanceladoModel> listaEncontrada = Mapper.Map<List<CrearReferenciaDTO>, List<ReferenciaCanceladoModel>>(CrearReferencia_CanceladosNegocios.ObtenerReferenciasAnioActual(DateTime.Now.Year));

          

            return View(listaEncontrada);
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

        public FileContentResult DescargarReporteNominaAnual(int IdReferencia)
        {
            byte[] bytes = null;

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferencia);

            string pathACrear = Path.Combine(Server.MapPath("~/"), @"Reportes\IpdParaDescargas\ReporteNominaAnual");

            //string pathACrear = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas\ReporteNominaAnual";

            string pathdestino = null;
            string slash = @"\";
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                    //var registrosPorAnio = pagosEncontrados.Where(x => x.Anio == 2022).ToList();

                    var nominaAnualRegistros = CrearReferencia_CanceladosNegocios.ObtenerRegistrosNominaAnual(IdReferencia, anioSeleccionado);

                    decimal sumaTotalGeneralSinCAPAE = 0;
                    DAP.Plantilla.Reportes.Datasets.DsNominaAnual nominaAnual = new DAP.Plantilla.Reportes.Datasets.DsNominaAnual();
                    nominaAnual.datosReferencia.AdddatosReferenciaRow(datosReferencia.Numero_Referencia, Convert.ToString(anioSeleccionado));
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
                string pathZip =  server + @"Reportes\IpdParaDescargas\NominasAnuales" + datosReferencia.Numero_Referencia + ".Zip";
               // string pathZip = Path.Combine(Server.MapPath("~/"), "/Reportes/IpdParaDescargas/NominasAnuales"+datosReferencia.Numero_Referencia+".Zip");
                
               // string pathZip = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas\NominasAnuales" + datosReferencia.Numero_Referencia + ".Zip";


                ZipFile.CreateFromDirectory(pathACrear, pathZip);

                if (System.IO.File.Exists(pathZip))
                {
                    bytes = System.IO.File.ReadAllBytes(pathZip);
                    System.IO.File.Delete(pathZip);
                    Directory.Delete(pathACrear, true);
                }
            }

            return File(bytes, "application /zip", "NominasAnuales_" + datosReferencia.Numero_Referencia + ".Zip");
        }


        public FileContentResult DescargarReporteCuentaBancariaAnual(int IdReferencia)
        {
            byte[] bytes = null;

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferencia);
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), @"Reportes\IpdParaDescargas");
            //string pathPrincipal = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas";

            string pathACrear = Path.Combine( Server.MapPath("~/") , @"Reportes\IpdParaDescargas\ReporteCuentaBancariaAnual");
            //string pathACrear = @"C:\Users\Israel\Desktop\RAMA DE FOLIACION\FoliacionRemasterizado\DAP.Plantilla\Reportes\IpdParaDescargas\ReporteCuentaBancariaAnual";

            string pathdestino = null;
            string slash = @"\";
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                    //var registrosPorAnio = pagosEncontrados.Where(x => x.Anio == 2022).ToList();

                  //  var nominaAnualRegistros = CrearReferencia_CanceladosNegocios.ObtenerRegistrosNominaAnual(IdReferencia, anioSeleccionado);
                    var registrosCuentabancariaObtenidos = CrearReferencia_CanceladosNegocios.ObtenerRegistrosBancosAnual(IdReferencia, anioSeleccionado);

                    DAP.Plantilla.Reportes.Datasets.DSCuentaBancariaAnual_CC cuentaBancariaAnual = new DAP.Plantilla.Reportes.Datasets.DSCuentaBancariaAnual_CC();
                    cuentaBancariaAnual.DatosReferencia.AddDatosReferenciaRow(datosReferencia.Numero_Referencia, Convert.ToString(anioSeleccionado));
                    foreach (var nuevoregistro in registrosCuentabancariaObtenidos)
                    {
                        cuentaBancariaAnual.NominaPorBanco.AddNominaPorBancoRow(nuevoregistro.NombreNomina, nuevoregistro.SumaLiquido, nuevoregistro.TotalRegistros , nuevoregistro.NombreCuentaBanco);
                    }

                    pathdestino = pathACrear + slash + "CuentaBancariaAnual"+anioSeleccionado+".pdf";
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/ReporteCuentaBancariaAnualCC.rpt"));
                    rd.SetDataSource(cuentaBancariaAnual);
                    rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathdestino);
                }

                string pathZip = pathPrincipal+slash+"CuentasBancariasAnuales_"+datosReferencia.Numero_Referencia+".Zip";


                ZipFile.CreateFromDirectory(pathACrear, pathZip);

                if (System.IO.File.Exists(pathZip))
                {
                    bytes = System.IO.File.ReadAllBytes(pathZip);
                    System.IO.File.Delete(pathZip);
                    Directory.Delete(pathACrear, true);
                }
            }



            return File(bytes, "application/zip", "ReporteCuentasBancariasAnuales_"+datosReferencia.Numero_Referencia+".Zip");
        }


        public FileContentResult DescargarReportePensionAlimenticia(int IdReferencia)
        {
            byte[] bytes = null;

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferencia);
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), @"Reportes\IpdParaDescargas");
            string pathACrear = Path.Combine(Server.MapPath("~/"), @"Reportes\IpdParaDescargas\ReportePensionAlimenticia");

            string pathdestino = null;
            string slash = @"\";
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
                        dsRepoPensionAlimenticia.DatosReferencia.AddDatosReferenciaRow(datosReferencia.Numero_Referencia, ""+anioRecorrido+"");
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


                ZipFile.CreateFromDirectory(pathACrear, pathZip);

                if (System.IO.File.Exists(pathZip))
                {
                    bytes = System.IO.File.ReadAllBytes(pathZip);
                    System.IO.File.Delete(pathZip);
                    Directory.Delete(pathACrear, true);
                }


            }


            return File(bytes, "application/zip", "PensionesAlimenticiasAnuales_"+datosReferencia.Numero_Referencia+".Zip");
        }



        /******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /****************************************                            Metodos para Descargar Reportes del IPD y el propio IPD                           *******************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/


        public ActionResult DescargarIPD(int IdReferencia)
        {
            // string path = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas/IPDVacio.dbf");
            string path = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas");

            string nombreReferecnia = CrearReferencia_CanceladosNegocios.ObtenerNombreReferecnia(IdReferencia);



            string pathOrigen = path + "/IPDVacio.dbf";
            string pathDestino = path + "/" + nombreReferecnia + ".dbf";


            string resultado = CrearReferencia_CanceladosNegocios.GenerarNuevoIPD(pathOrigen, pathDestino, nombreReferecnia, IdReferencia);



            if (resultado.Contains("CORRECTO"))
            {
                // Verifico si el cliente aún sigue conectado a la applicación en dado caso que no sea así no descargará el


                try
                {
                    if (Response.IsClientConnected)
                    {
                        // Obtengo la información del archivo
                        FileInfo info = new FileInfo(pathDestino);

                        string rr = info.Length.ToString();
                        // En response adjunto los datos del archivo y los transmito al cliente
                        Response.ClearContent();

                        Response.AppendHeader("Content-Disposition", "attachment;filename=" + pathDestino);

                        Response.AddHeader("Content-Length", info.Length.ToString());

                        Response.ContentType = "application/dbf";

                        Response.TransmitFile(pathDestino);

                        Response.Flush();

                        Response.End();
                    }
                }
                catch (Exception E)
                {
                    return Content(E.Message);
                }
                finally
                {
                    if (System.IO.File.Exists(pathDestino))
                    {
                        System.IO.File.Delete(pathDestino);
                    }
                    Response.Close();
                }


            }

            return Content("No hay datos que cargar");
        }


        public FileContentResult DescargarIPDPorAnio(int IdReferencia)
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
                    string nombreArchivoDBFAnual = "IPD_"+datosReferencia.Numero_Referencia+"_"+anioSeleccionado;
                    string pathDestino = pathACrear + "/"+nombreArchivoDBFAnual+".dbf";

                    bool resultadoBandera = CrearReferencia_CanceladosNegocios.GeneralNuevoIPDxAnio(pathPrincipal , pathOrigen, pathDestino, anioSeleccionado, nombreArchivoDBFAnual, datosReferencia.Numero_Referencia,  IdReferencia);

                    if (!resultadoBandera) 
                    {
                        return File(CrearReferencia_CanceladosNegocios.ObtenerLogErrorParaUsuario(pathPrincipal+"/ERROR.txt"), "text/plain", "ERROR_"+datosReferencia.Numero_Referencia+"_"+anioSeleccionado+".txt");
                    }
                }

                string pathZip = pathPrincipal + slash + "IPD_Anuales_"+datosReferencia.Numero_Referencia+".Zip";
                ZipFile.CreateFromDirectory(pathACrear, pathZip);

                if (System.IO.File.Exists(pathZip))
                {
                    bytes = System.IO.File.ReadAllBytes(pathZip);
                    System.IO.File.Delete(pathZip);
                    Directory.Delete(pathACrear, true);
                }
            }

            return File(bytes, "application/zip", "IPD_Anuales_" + datosReferencia.Numero_Referencia + ".Zip");
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

        public FileContentResult DescargarIPDCompensadoPorAnio(int IdReferencia)
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
                    string nombreArchivoDBFAnual = "IPDC_"+datosReferencia.Numero_Referencia+"_"+anioSeleccionado;
                    string pathDestino = pathACrear + "/" + nombreArchivoDBFAnual + ".dbf";

                    bool resultadoBandera = CrearReferencia_CanceladosNegocios.GeneralNuevoIPDCxAnio(pathACrear, pathOrigen, pathDestino, anioSeleccionado, nombreArchivoDBFAnual, datosReferencia.Numero_Referencia, IdReferencia);

                }

                string pathZip = pathPrincipal + slash + "IPDC_Anuales_" + datosReferencia.Numero_Referencia + ".Zip";
                ZipFile.CreateFromDirectory(pathACrear, pathZip);

                if (System.IO.File.Exists(pathZip))
                {
                    bytes = System.IO.File.ReadAllBytes(pathZip);
                    System.IO.File.Delete(pathZip);
                    Directory.Delete(pathACrear, true);
                }
            }

            return File(bytes, "application/zip", "IPDCompensados_Anuales_" + datosReferencia.Numero_Referencia + ".Zip");
        }



    }
}