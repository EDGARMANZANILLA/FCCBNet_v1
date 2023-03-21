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



        /*******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /****************************************                            METODOS PARA DESCARGAR REPORTES DEL IPD Y EL IPD.DBF                          *****************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/

        //CREA EL IPD Y EL IPD DEL COMPESANDO Y LO GUARDA EN LA BASE DE DATOS PARA UN MAYOR RENDIMIENTO POSTERIORMENTE
        public JsonResult ValidaCreaIPD(int IdReferenciaCancelado)
        {
            bool bandera = false;

            if (CrearReferencia_CanceladosNegocios.EstaActualizadoIpdGuardado(IdReferenciaCancelado))
            {
                bandera = true;
            }
            else 
            {
                //SI NO SE ENCUENTRA ACTUALIZADO CORRECTAMENTE LOS DATOS DEL IPD SE PROCEDE A ELIMINAR LOS DATOS GUARDADOS Y VOLVER A GENERAR LOS DATOS BORRADOS PARA QUE SE HAGAN LAS ACTUALIZACIONES
                if (CrearReferencia_CanceladosNegocios.BorrarIPDGuardadoDeIdReferencia(IdReferenciaCancelado)) 
                {
                    List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferenciaCancelado);
                    foreach (int anioSeleccionado in aniosContenidos)
                    {
                        bandera = CrearReferencia_CanceladosNegocios.GeneraYGuardaInterfaceIPDxAnio( anioSeleccionado, IdReferenciaCancelado);
                    }
                }
            }

            return Json(new  { bandera = bandera  });
        }



        public ActionResult DescargarIPDPorAnio_AXIOS(int IdReferencia)
        {
           // byte[] bytes = null;

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferencia);
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas");
            string pathACrear = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas/IPDxAnio");
            
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

                    bool resultadoBandera = CrearReferencia_CanceladosNegocios.GeneralNuevoIPDxAnio_2023(pathOrigen, pathDestino, nombreArchivoDBFAnual, IdReferencia);
                    //bool resultadoBandera = CrearReferencia_CanceladosNegocios.GeneralNuevoIPDxAnio(pathPrincipal, pathOrigen, pathDestino, anioSeleccionado, nombreArchivoDBFAnual, datosReferencia.Numero_Referencia, IdReferencia);

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


    



        /*************************************************************************************************************************************************************/
        /*******************************************************    DescargarTGCxNomina_2023   ***********************************************************************/
        /*************************************************************************************************************************************************************/
        /*************    CONTEXTO PARA CREAR DOS CABECEZARAS O MEJOR DICHO DOS REPORTES EN UNO, HABLANDO DE UN MISMO CONTEXTO DE DATOS           ********************/
        /***   1-.ENTIENDADE QUE EL REPORTE FUE VISTO DESDE UNA PERSPECTIVA EN 3D COMO SI FUERA UN CUBO DE 2X2 PERO PUEDEN HABER DE MAS DIMENCIONES.               ***/
        /***     ESTO AL VERLO EN UN ESPACIO TRIDIMENCIONAL NOS DAMOS CUENTA QUE LOS PRIMEROS 2 CUADRANTES 1 Y 2 CORRESPONDEN AL PRIMER REPORTE CON SU ENCABEZADO  ***/
        /***     Y LOS OTROS 2 DE ABAJO ESTAN TRANSPARENTE Y EN EL SEGMENTO DE ATRAS DEL 2X2 SOLO SE OCUPAN LOS CUADRANTES 3 Y 4 CORRESPONDEN AL 2DO REPORTE       ***/
        /***     QUEDANDO VACIAS LOS CUADRANTES 3 Y 4 DEL DEL PRIMER SEGMENTO Y 1 Y 2 DEL SEGUNDO SEGMENTO. ESTO AL VERLO DE FRENTE Y EN 2 DIMENCIONES LOGRAMOS    ***/
        /***     UN REPORTE BIDIMENCIONAL A COMO EN UN INICIO LO NECESITAMOS.                                                                                      ***/
        /***   2-.EN EL TEMA DE LLENADO DE DATOS SE VUELVE UN POCO MAS SIMPRE POR QUE LA IDEA ERA TRATARLO COMO UNA MATRIZ PERO SE VUELVE UN POBLEMA               ***/
        /***     YA QUE LA MATRIZ TENDRIA QUE SER DINAMICA PARA AMBOS SEGMENTOS Y ESTO GENERARIA UN PROBLEMA DE BUSQUEDA Y CORRELACIONES POR ESO DECIDI USAR       ***/
        /***     LISTAS DINAMICAS, LAS CUALES SE CARGAN INICIALMENTE EN UN MODELO DEFINIDO CON TODOS LOS CAMPOS DE LOS N REPORTES                                  ***/
        /***     EN EL 1ER PASO SE OBTIENE EL DETALLE DE LA INFORMACION DE LA DB DE ACUERDO DE LA REFERENCIA DE CANCELACION Y SE CALCULA Y AGREGA UN RESUMN        ***/
        /***     EN EL 2DO PASO SE CALCULA LOS RESUMEN TOTALES DE LAS PP Y DD DE CADA RAMO Y EL RESUMEN Y SE ACTUALIZADA LA LISTA DE ACUERDO A CADA RAMO           ***/
        /***     EN EL 3ER PASO SE AGRUPAN Y SE CARGAN AL DATA SET                                                                                                 ***/
        /***   3-.HABLEMOS DEL REPORTE                                                                                                                             ***/
        /***     DEBES DE CREAR UN REPORTE Y SUBREPORTE EN CRYSTAL. EL REPORTE PRINCIPAL USARA LOS CUADRANTES 1 Y 2 DEL PRIMER SEGMENTE POR ENDE ES EL QUE LLEVARA ***/
        /***     EL ENCABEZADO Y EN ESTE CASO PARA LOS DETALLES DE DEBE DE AGRUPAR POR RAMO Y EN EL PRIMER REPORTE SOLO HABLAR DE LAS PP                           ***/
        /***     EN EL SUBREPORTE ES NECESARIO CARGAR EL MISMO DATASET QUE EN EL PRIMER REPORTE Y EN ESTE CASO YA NO SE AGRUPA SOLO SE PONEN LOS DATOS DE DD       ***/
        /***     EN ESTE 2DO REPORTE.                                                                                                                              ***/
        /***   4.INCRUSTANDO EL SUBREPORTE AL REPORTE PRINCIPAL                                                                                                    ***/
        /***     INSERTE EL SUBREPORTE EN EL PIE DEL GRUPO CREADO EN EL REPORTE PRONCIPAL Y HAGA QUE EL REPORTE PUEDA CRECER SEGUN SUS NECESIDADES                 ***/
        /***   5.ARREGLANDO PROBLEMAS DE FORMATO                                                                                                                   ***/
        /***     PROBLEMA => EN ESTE PUNDO EL REPORTE SE ENCUENTRA COMPLETO EN INFORMACION PERO APARECEN LINEAS EN BLANCO O CANTIDADES EN "0" EN DONDE NO          ***/
        /***     SE ENCUENTRA INFORMACION.                                                                                                                         ***/
        /***     SOLUCION => HAY QUE CREAR UN CAMPO DE FORMULA EN CADA REPORTE SOLO PARA TENER ACCESO AL EDITOR DE CODIGO DE VISUAL BASIC DE CRYSTAL.              ***/
        /***     UNA VEZ DENTRO DIRIJASE A DENTRO DEL LADO IZQUIERDO VAYA A -> FORMULAS DE SELECCION -> SELECCION DE GRUPO Y COPIE:                                ***/
        /***     IF {DatosTGC_Resumen.PP_Cantidad} <> "0"                                                                                                          ***/
        /***     Then                                                                                                                                              ***/
        /***     {DatosTGC_Resumen.PP_Cantidad}                                                                                                                    ***/
        /***     EXPLICACION =>EL LENGUAJE ES VISUAL BASIC Y COMO YA SABEMOS QUE EN ALGUN MOMENTO LAS FILAS QUE NO DEBEN DE APARECER CONTIENEN                     ***/
        /***     UNA CANTIDAD DE PP O DD DEBEN DE APARECER CONTIENEN UNA CANTIDAD DE PP O DD EN 0 PUES HACEMOS QUE CUANDO EL RENDER SE ENCUENTRE                   ***/
        /***     CON UNA CANTIDAD EN 0; SOLO LO DEBE IGNORAR Y REAJUSTARSE PARA  LOS DATOS QUE CONTINUAN                                                           ***/
        /*************************************************************************************************************************************************************/
        /*******************************************************    DescargarTGCxNomina_2023   ***********************************************************************/
        /*************************************************************************************************************************************************************/
        public ActionResult DescargarTGCxNomina_2023(int IdReferencia)
        {
          
            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos =  CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferenciaIPD(IdReferencia);
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas");
            string pathACrear = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas/ReporteTotalGeneralConceptosPorNomina");
            
            string pathdestino = null;
            string slash = @"\";
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                    string pathACrearxAnio = pathACrear + "//" + anioSeleccionado + "";

                    Directory.CreateDirectory(pathACrearxAnio);
                    if (Directory.Exists(pathACrearxAnio))
                    {
                        /*** OBTIENE LAS CONSULTAS QUE LE CORRESPONDEN AL REPORTE SELECCIONADO  ***/
                        List<string> consultasDeNominasParaTGCXNomina = CrearReferencia_CanceladosNegocios.ObtenerConsultasReporteTGCxNomina_CuotasPatronales_IPD(datosReferencia.Id, anioSeleccionado, "IPD", "TGCNomina");

                        foreach (string nuevaConsulta in consultasDeNominasParaTGCXNomina)
                        {
                            /***    OBTIENE LOS DETALLES DE LA CONSULTA SELECCIONADA INCLUYENDO SU RESUMEN ***/
                            List<RegistrosTGCxNominaDTO> registrosObtenidos = CrearReferencia_CanceladosNegocios.TotalesGeneralesXConcepto(datosReferencia.Id, anioSeleccionado, nuevaConsulta);
                            string nombreNominaIterada = registrosObtenidos.Where(x => x.Partida != "00-00").FirstOrDefault().NombreNomina.ToUpper();

                            /***    INICIA EL DATA SET Y SE INICIA LLENANDO CON LOS DATOS QUE TENDRA EL ENCABEZADO      ***/
                            DAP.Plantilla.Reportes.Datasets.DsReportesCC.DsIPD.DsTGCxNomina dsTGCxNomina = new Reportes.Datasets.DsReportesCC.DsIPD.DsTGCxNomina();
                            dsTGCxNomina.DatosReferencia.AddDatosReferenciaRow(datosReferencia.Numero_Referencia, "" + anioSeleccionado + "", nombreNominaIterada);

                            var agrupado = registrosObtenidos.GroupBy(x => x.Partida);
                            foreach (var nuevoGrupo in agrupado)
                            {
                                foreach (RegistrosTGCxNominaDTO item in nuevoGrupo)
                                {

                                    dsTGCxNomina.DatosTGC_Resumen.AddDatosTGC_ResumenRow(item.NombreNomina, item.Ramo, item.Partida, item.PP_TipoClave, item.PP_Cantidad.ToString(), item.PP_CvePD, item.pp_DescripcionCvePD, item.PP_SumatoriaPositiva, item.PP_SumatoriaNegativa, item.DD_TipoClave, item.DD_Cantidad.ToString(), item.DD_CvePD, item.DD_DescripcionCvePD, item.DD_SumatoriaPositiva, item.DD_SumatoriaNegativa, item.PP_TotalPositivo, item.PP_TotalNegativo, item.DD_TotalPositivo, item.DD_TotalNegativo, item.Liquido);

                                }
                            }


                            pathdestino = pathACrearxAnio + slash + "TGCxNomina" + anioSeleccionado + "_"+nombreNominaIterada+".pdf";
                            ReportDocument rd = new ReportDocument();
                            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/ReportesCC/IPD/ReporteTGCxNomina.rpt"));
                            rd.SetDataSource(dsTGCxNomina);
                            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathdestino);

                            

                        }


                    }



                }

                string pathZip = pathPrincipal + slash + "ReporteTotalGeneralConceptosPorNomina_" + datosReferencia.Numero_Referencia + ".Zip";


                //ZipFile.CreateFromDirectory(pathACrear, pathZip);

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

        public ActionResult DescargarTGCxCuentaBancaria_2023(int IdReferencia)
        {

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferenciaIPD(IdReferencia);
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas");
            string pathACrear = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas/ReporteTotalGeneralConceptosXCuentaBancaria");

            string pathdestino = null;
            string slash = @"\";
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                    string pathACrearxAnio = pathACrear + "//" + anioSeleccionado + "";

                    Directory.CreateDirectory(pathACrearxAnio);
                    if (Directory.Exists(pathACrearxAnio))
                    {
                        List<RegistrosTGCxCuentaBancariaDTO> ResumenTGCxCB = CrearReferencia_CanceladosNegocios.ResumenTGCxCuentaBancaria(datosReferencia.Id, anioSeleccionado);
                         var agrupadoPorBanco = ResumenTGCxCB.GroupBy(x => x.NombreCuentaBancaria).ToList();
                      
                        /****************************************************************************************************************************************************************/
                        /****************************************************************************************************************************************************************/
                        /*************    AGREGAR UNA NUEVO DATA SET PARA EL TGCXCUENTA BANCARIA PATA CARGARLE ESTOS DAROS QUE YA SE ENCUANTRAN BIEN      *******************************/
                        /****************************************************************************************************************************************************************/
                        /****************************************************************************************************************************************************************/
                        DAP.Plantilla.Reportes.Datasets.DsReportesCC.DsIPD.DsTGCxCuentaBancaria dsTGCxCuentaBancaria = new Reportes.Datasets.DsReportesCC.DsIPD.DsTGCxCuentaBancaria();
                        dsTGCxCuentaBancaria.DatosReferencia.AddDatosReferenciaRow(datosReferencia.Numero_Referencia, "" + anioSeleccionado + "");

                        foreach (var nuevoGrupo in agrupadoPorBanco) 
                        {
                            foreach (RegistrosTGCxCuentaBancariaDTO item in nuevoGrupo)
                            {
                                dsTGCxCuentaBancaria.DatosTGCxCuentaBancaria_Resumen.AddDatosTGCxCuentaBancaria_ResumenRow(item.NombreCuentaBancaria, item.PP_TipoClave , item.PP_Cantidad.ToString(), item.PP_CvePD, item.pp_DescripcionCvePD, item.PP_SumatoriaPositiva, item.PP_SumatoriaNegativa , item.DD_TipoClave, item.DD_Cantidad.ToString() , item.DD_CvePD, item.DD_DescripcionCvePD, item.DD_SumatoriaPositiva, item.DD_SumatoriaNegativa, item.PP_TotalPositivo, item.PP_TotalNegativo, item.DD_TotalPositivo, item.DD_TotalPositivo, item.Liquido);
                            }
                           
                        }

                       
                        pathdestino = pathACrearxAnio + slash + "TGCxCuentaBancaria" + anioSeleccionado + "_" + datosReferencia.Numero_Referencia + ".pdf";
                        ReportDocument rd = new ReportDocument();
                        rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/ReportesCC/IPD/ReporteTGCxCuentaBancaria.rpt"));
                        rd.SetDataSource(dsTGCxCuentaBancaria);
                        rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathdestino);

                    }

                }

                string pathZip = pathPrincipal + slash + "ReporteTotalGeneralConceptosPorNomina_" + datosReferencia.Numero_Referencia + ".Zip";


                //ZipFile.CreateFromDirectory(pathACrear, pathZip);

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

        public ActionResult DescargarCuotasPatronalesxNomina_2023(int IdReferencia)
        {
            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferenciaIPD(IdReferencia);
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas");
            string pathACrear = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas/ReporteCuotasPatronalesAnioXNomina");
            

            string pathdestino = null;
            string slash = @"\";
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                    string pathACrearxAnio = pathACrear + "//" + anioSeleccionado + "";

                    Directory.CreateDirectory(pathACrearxAnio);
                    if (Directory.Exists(pathACrearxAnio))
                    {
                        List<string> listaConsutalParaTGCXNomina = CrearReferencia_CanceladosNegocios.ObtenerConsultasReporteTGCxNomina_CuotasPatronales_IPD(IdReferencia, anioSeleccionado, "IPD", "CuotasPatronales");
                        foreach (string consulta in listaConsutalParaTGCXNomina)
                        {
                            List<RegistrosCuotasPatronalesDTO> detallesNomina = CrearReferencia_CanceladosNegocios.ObtenerDetallesCuotasPatronales(consulta);

                            if (detallesNomina.Count > 0)
                            {
                                   /***    INICIA EL DATASET PARA CARGARLE DATOS    ***/
                                Reportes.Datasets.DsReportesCC.DsIPD.DsCoutasPatronalesxNomina dsCoutasPatronales = new Reportes.Datasets.DsReportesCC.DsIPD.DsCoutasPatronalesxNomina();

                                string nombreNominaConsultada = detallesNomina.FirstOrDefault().NombreNomina;
                                   /***  CARGA LA TABLA DATOSREDFERENCIA DEL DATA SET(AQUI VAN EL ENCABEZADO DEL REPORTE)   ***/
                                dsCoutasPatronales.DatosReferencia.AddDatosReferenciaRow(datosReferencia.Numero_Referencia, "" + anioSeleccionado + "", nombreNominaConsultada);


                                foreach (RegistrosCuotasPatronalesDTO ramoSelecionado in detallesNomina)
                                {
                                        /***    CARGA DATOS A LA TABLA RAMOUNIDAD DEL DATASET(AQUIN VAN DATOS QUE CONTIENE EL INFORME AGRUPADOS POR RAMO)   ***/    
                                        dsCoutasPatronales.RamoUnidad.AddRamoUnidadRow(ramoSelecionado.DescripRamo, ramoSelecionado.DescripUnidad, ramoSelecionado.Cantidad, ramoSelecionado.MontoPositivo, ramoSelecionado.TotalRamo_Cantidad, ramoSelecionado.TotalRamo_Monto);
                                }

                                
                                /***    CARGA DATOS AL RESUMEN(AQUI VA LA SUMATORIA DEL RESUMEN GENERAL DEL REPORTE )   ***/
                                dsCoutasPatronales.Resumen.AddResumenRow(detallesNomina.Sum(x => x.Cantidad), detallesNomina.Sum(X => X.MontoPositivo) );
                                
                                pathdestino = pathACrearxAnio + slash + "CuotasPatronales"+anioSeleccionado+"_"+nombreNominaConsultada+".pdf";
                                ReportDocument rd = new ReportDocument();
                                rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/ReportesCC/IPD/ReporteCuotasPatronalesRamoUnidad.rpt"));
                                rd.SetDataSource(dsCoutasPatronales);
                                rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathdestino);


                            }

                        }

                    }


                }





                string pathZip = pathPrincipal + slash + "ReporteTotalGeneralConceptosPorNomina_" + datosReferencia.Numero_Referencia + ".Zip";



                try
                {

                    if (Response.IsClientConnected)
                    {

                        if (!Directory.Exists(pathZip))
                        {
                            ZipFile.CreateFromDirectory(pathACrear, pathZip);
                            Response.Clear();
                            Response.BufferOutput = false;
                            /* Response.ContentType = "application/zip";*/
                            /*Response.ContentType = "application/octet-stream";*/

                            /*Response.AppendHeader("content-disposition", "attachment; filename= IPD_Anuales_" + datosReferencia.Numero_Referencia + ".zip");*/
                            /* AddHeader("content-disposition", "attachment; filename=  Partida_" + partidasADescargar + "_.zip");*/
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
                        /* System.IO.File.ReadAllBytes(pathZip);*/
                        System.IO.File.Delete(pathZip);
                        Directory.Delete(pathACrear, true);
                        Response.Close();
                    }

                }



            }

            return Content("No hay datos que cargar");
        }

        public ActionResult DescargarReporte_ResumenGeneralPercepcionesCuotasPatronales_XIdReferencia_2023(int IdReferencia)
        {

            var datosReferencia = CrearReferencia_CanceladosNegocios.ObtenerDatosIdReferenciaCancelacion(IdReferencia);
            List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferenciaIPD(IdReferencia);
            string pathPrincipal = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas");
            string pathACrear = Path.Combine(Server.MapPath("~/"), "Reportes/IpdParaDescargas/ReporteResumenGeneralPorAnio");


            string pathdestino = null;
            string slash = @"\";
            Directory.CreateDirectory(pathACrear);
            if (Directory.Exists(pathACrear))
            {
                foreach (int anioSeleccionado in aniosContenidos)
                {
                    string pathACrearxAnio = pathACrear + "//" + anioSeleccionado + "";
                    Directory.CreateDirectory(pathACrearxAnio);
                    if (Directory.Exists(pathACrearxAnio))
                    {
                        List<ResumenGeneralXNominaDTO> listaResumenNominasXAnio = CrearReferencia_CanceladosNegocios.ResumenPercepcionesCuotasPatronalesXAnio(datosReferencia.Id, anioSeleccionado);
                        


                            Reportes.Datasets.DsReportesCC.DsIPD.DsResumenGeneral dsResumenGeneralXAnio = new Reportes.Datasets.DsReportesCC.DsIPD.DsResumenGeneral();
                            dsResumenGeneralXAnio.DatosReferencia.AddDatosReferenciaRow(datosReferencia.Numero_Referencia, anioSeleccionado.ToString());


                            foreach (ResumenGeneralXNominaDTO itenNomina in listaResumenNominasXAnio)
                            {
                                dsResumenGeneralXAnio.ResumenNominas.AddResumenNominasRow(itenNomina.NombreDescripcionNomina, itenNomina.PP_Regs, itenNomina.PP_Monto, itenNomina.CP_Regs, itenNomina.CP_Monto, itenNomina.Total_Regs, itenNomina.Total_Monto);
                            }

                            dsResumenGeneralXAnio.TotalesGenerales.AddTotalesGeneralesRow(listaResumenNominasXAnio.Sum(x => x.PP_Regs), listaResumenNominasXAnio.Sum(x => x.PP_Monto), listaResumenNominasXAnio.Sum(x => x.CP_Regs), listaResumenNominasXAnio.Sum(x => x.CP_Monto), listaResumenNominasXAnio.Sum(x => x.Total_Regs), listaResumenNominasXAnio.Sum(x => x.Total_Monto));

                            pathdestino = pathACrearxAnio + slash + "ResumenGeneral_" + anioSeleccionado + ".pdf";
                            ReportDocument rd = new ReportDocument();
                            rd.Load(Path.Combine(Server.MapPath("~/"), "Reportes/Crystal/ReportesCC/IPD/ReporteGeneral.rpt"));
                            rd.SetDataSource(dsResumenGeneralXAnio);
                            rd.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, pathdestino);

                        



                    }

                    string pathZip = pathPrincipal + slash + "ReporteResumenGeneral_" + datosReferencia.Numero_Referencia + ".Zip";

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

                                //Response.AppendHeader("content-disposition", "attachment; filename= IPD_Anuales_" + datosReferencia.Numero_Referencia + ".zip");
                                //AddHeader("content-disposition", "attachment; filename=  Partida_" + partidasADescargar + "_.zip");
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
                            //bytes = System.IO.File.ReadAllBytes(pathZip);
                            System.IO.File.Delete(pathZip);
                            Directory.Delete(pathACrear, true);
                            Response.Close();
                        }

                    }

                }
            }
                return Content("No hay datos que cargar");
        }





        /******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /****************************************                            Metodos para Descargar el IPD_Compensado                          *******************************************************/
        /*******************************************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************************************/

        public JsonResult ValidaCreaIPDCompensado(int IdReferenciaCancelado)
        {
            bool bandera = false;

            if (CrearReferencia_CanceladosNegocios.EstaActualizadoIpdCompensadoGuardado(IdReferenciaCancelado))
            {
                bandera = true;
            }
            else
            {
                //SI NO SE ENCUENTRA ACTUALIZADO CORRECTAMENTE LOS DATOS DEL IPD SE PROCEDE A ELIMINAR LOS DATOS GUARDADOS Y VOLVER A GENERAR LOS DATOS BORRADOS PARA QUE SE HAGAN LAS ACTUALIZACIONES
                if (CrearReferencia_CanceladosNegocios.BorrarIPDCompensadoGuardadoDeIdReferencia(IdReferenciaCancelado))
                {
                    List<int> aniosContenidos = CrearReferencia_CanceladosNegocios.ObtenerListaAniosDentroReferencia(IdReferenciaCancelado);
                    foreach (int anioSeleccionado in aniosContenidos)
                    {
                        bandera = CrearReferencia_CanceladosNegocios.GeneraYGuardaInterfaceIPDCompensadoxAnio(anioSeleccionado, IdReferenciaCancelado);
                    }
                }
            }

            return Json(new { bandera = bandera });
        }


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

                    bool resultadoBandera = CrearReferencia_CanceladosNegocios.GeneralNuevoIPDCompensadoxAnio_2023( pathOrigen,  pathDestino,  nombreArchivoDBFAnual, IdReferencia);
                        //bool resultadoBandera = CrearReferencia_CanceladosNegocios.GeneralNuevoIPDCxAnio(pathACrear, pathOrigen, pathDestino, anioSeleccionado, nombreArchivoDBFAnual, datosReferencia.Numero_Referencia, IdReferencia);

                    }

                    string pathZip = pathPrincipal + slash + "IPDCompensados_Anuales_" + datosReferencia.Numero_Referencia + ".Zip";
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