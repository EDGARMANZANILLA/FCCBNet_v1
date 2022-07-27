using DAP.Foliacion.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using DAP.Foliacion.Entidades;
using System.Data.Entity;
using System.IO;
using Datos;
using System.Data.Common;
using System.Threading;
using DAP.Foliacion.Datos.ClasesParaDBF;
using System.Security.Principal;
using System.Runtime.InteropServices;
using DAP.Foliacion.Entidades.DTO.FoliarDTO.RecuperarFolios;
using DAP.Foliacion.Negocios.ObtenerConsultasChequesFoliarNegocios;
using DAP.Foliacion.Negocios.ObtenerConsultasPagomaticosFoliarNegocios;
using DAP.Foliacion.Entidades.DTO.FoliarDTO.FoliacionPagomatico;
using DAP.Foliacion.Negocios.FoliarDBF;

namespace DAP.Foliacion.Negocios
{
    public class FoliarNegocios
    {
        public static int ObtenerUltimaQuincenaFoliada()
        {
            int UltimaQuincenaObtenida = 0;
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Pagos>(transaccion);

            List<int> listaQuincenasRegistradas = repositorio.ObtenerPorFiltro(x => x.Quincena > 0).Select(y => y.Quincena).ToList();

            if (listaQuincenasRegistradas.Count > 0)
            {
                return listaQuincenasRegistradas.Max();
            }

            return UltimaQuincenaObtenida;
        }


        public static Dictionary<int, string> ObtenerNominasXNumeroQuincena(string NumeroQuincena, int AnioInterface)
        {
            List<NombreNominasDTO> nombresNomina = FoliarConsultasDBSinEntity.ObtenerNombreNominas(NumeroQuincena, AnioInterface);

            Dictionary<int, string> nombresListosNomina = new Dictionary<int, string>();

            int i = 0;
            foreach (NombreNominasDTO NuevaNomina in nombresNomina)
            {
                i += 01;
                if (NuevaNomina.Adicional == "")
                {
                    nombresListosNomina.Add(Convert.ToInt32(NuevaNomina.Id_nom), "  " + i + " -" + "-" + "- " + " [ " + NuevaNomina.Quincena + " ]   [ " + NuevaNomina.Id_nom + " ]  [ " + NuevaNomina.Nomina + " ] " + " [ " + NuevaNomina.RutaNomina + " ] " + " -" + "-" + " [ " + NuevaNomina.Ruta + NuevaNomina.RutaNomina + " ] " + " -" + "-" + "-" + "-" + "-" + "-" + "-" + "-" + " [ " + NuevaNomina.Coment + " ] ");
                }
                else
                {
                    nombresListosNomina.Add(Convert.ToInt32(NuevaNomina.Id_nom), "  " + i + " -" + "-" + "- " + " [ " + NuevaNomina.Quincena + " ]    [ " + NuevaNomina.Id_nom + " ]  [ " + NuevaNomina.Nomina + " ] " + " [ " + NuevaNomina.RutaNomina + " ] " + " -" + "-" + "- " + "ADICIONAL" + " -" + "-" + "- " + NuevaNomina.Adicional + " -" + "- " + " [ " + NuevaNomina.Ruta + NuevaNomina.RutaNomina + " ] " + " -" + "-" + "-" + "-" + "-" + "-" + "-" + "-" + " [ " + NuevaNomina.Coment + " ] ");
                }


            }

            return nombresListosNomina;
        }

        public static Dictionary<int, string> ObtenerBancosParaPagomatico()
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var bancosEncontrados = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true);

            Dictionary<int, string> bancosMostrar = new Dictionary<int, string>();

            foreach (Tbl_CuentasBancarias cuentaPagomatico in bancosEncontrados)
            {
                bancosMostrar.Add(cuentaPagomatico.Id, " " + cuentaPagomatico.NombreBanco + " " + " - " + " [ " + cuentaPagomatico.Cuenta + " ] ");
            }

            return bancosMostrar;
        }

        /// <summary>
        /// Obtiene los bancos con lo que se pueden 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> ObtenerBancoParaFormasPago()
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var bancosEncontrados = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true && x.InicioBaja == null);

            Dictionary<int, string> bancosMostrar = new Dictionary<int, string>();

            foreach (Tbl_CuentasBancarias cuentaPagomatico in bancosEncontrados)
            {
                bancosMostrar.Add(cuentaPagomatico.Id, " " + cuentaPagomatico.NombreBanco + " " + " - " + " [ " + cuentaPagomatico.Cuenta + " ] ");
            }

            return bancosMostrar;
        }






        public static string ObtenerBancoPorID(int Id)
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            Tbl_CuentasBancarias resultado = repositorio.Obtener(x => x.Id == Id && x.Activo == true);

            return resultado.NombreBanco + " - [ " + resultado.Cuenta + " ] ";
        }




        /*Obtener Detalle (Id, Nombre, cuenta, IdInventaria)De banco para Formas de Cheques*/

        public static List<BancosConChequeraDTO> ObtenerDetalleBancoFormasDePago()
        {
            List<BancosConChequeraDTO> InventariosBancosFiltradosActivos = new List<BancosConChequeraDTO>();


            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);

            List<Tbl_Inventario> inventarioActivo = repositorioInventario.ObtenerPorFiltro(x => x.Activo == true).ToList();

            List<Tbl_CuentasBancarias> cuentasViaChequera = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true && x.InicioBaja == null).ToList();



            foreach (Tbl_Inventario inventarioSeleccionado in inventarioActivo)
            {
                BancosConChequeraDTO InventarioBanco = new BancosConChequeraDTO();


                Tbl_CuentasBancarias bancoEncontrado = cuentasViaChequera.Where(x => x.IdInventario == inventarioSeleccionado.Id).FirstOrDefault();

                if (bancoEncontrado != null)
                {
                    InventarioBanco.NombreBanco = bancoEncontrado.NombreBanco;
                    InventarioBanco.Cuenta = bancoEncontrado.Cuenta;
                    InventarioBanco.FormasDisponiblesInventario = inventarioSeleccionado.FormasDisponibles;
                    InventarioBanco.UltimoFolioUtilizadoInventario = inventarioSeleccionado.UltimoFolioUtilizado;

                    InventariosBancosFiltradosActivos.Add(InventarioBanco);
                }


            }

            return InventariosBancosFiltradosActivos;
        }



        //Obtener Datos del A-N
        public static DatosCompletosBitacoraDTO ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(int IdNom, int AnioInterface)
        {
            return FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(IdNom, AnioInterface);
        }














        /// <summary>
        /// Devuelve el an de como se encuentra en bitacora de una nomina especifica para revisarla o foliarla
        /// </summary>
        /// <param name="IdNum">numero de la nomina que corresponde la seleccion del usuario</param>
        /// <returns></returns>
        public static List<string> ObtenerAnApAdNominaBitacoraPorIdNumConexion(int IdNum)
        {
            return FoliarConsultasDBSinEntity.ObtenerAnApAdNominaBitacoraPorIdNumConexion(IdNum);

        }



        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosFoliadosPorNominaRevicion(string NumeroNomina, string An, int Quincena)
        {
            //obtener los nombres y cuentas de los bancos para saber con que se le pagara a cada trabajador 
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            List<int> idCuentasConTarjetas = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true).Select(y => y.Id).OrderBy(z => z).ToList();

            List<string> NombresBanco = new List<string>();
            foreach (int id in idCuentasConTarjetas)
            {
                NombresBanco.Add(ObtenerBancoPorID(id));
            }


            return FoliarConsultasDBSinEntity.ObtenerDatosIdNominaPagomatico(NumeroNomina, An, Quincena, NombresBanco);

        }

        public static List<DatosReporteRevisionNominaDTO> ObtenerDatosFoliadosPorNominaPENALRevicion(string NumeroNomina, string An, int Quincena)
        {
            //obtener los nombres y cuentas de los bancos para saber con que se le pagara a cada trabajador 
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            List<int> idCuentasConTarjetas = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true).Select(y => y.Id).OrderBy(z => z).ToList();

            List<string> NombresBanco = new List<string>();
            foreach (int id in idCuentasConTarjetas)
            {
                NombresBanco.Add(ObtenerBancoPorID(id));
            }

            var DatosNomina = FoliarConsultasDBSinEntity.ObtenerDatosIdNominaPenalPagomatico(NumeroNomina, An, Quincena, NombresBanco);

            return DatosNomina;
        }


        public static string ObtenerRutaCOmpletaArchivoIdNomina(int IdNomina)
        {
            return FoliarConsultasDBSinEntity.ObtenerRutaIdNomina(IdNomina);
        }



        public static string ObtenerNumeroNominaXIdNumBitacora(int IdNum)
        {
            return FoliarConsultasDBSinEntity.ObtenerNumeroNominaXIdNum(IdNum);
        }



        /// <summary>
        /// Obtine una lista de las nominas que pueden ser Foliadas para que se foleen todas de un solo jalon
        /// recibe como parametro el numero de quincena ejem 2112
        /// </summary>
        /// <param name="Quincena"></param>
        /// <returns></returns>
        public static List<DatosRevicionTodasNominasDTO> ObtenerTodasNominasXQuincena(string NumeroQuincena, int AnioInterface)
        {

            return FoliarConsultasDBSinEntity.ObtenerListaDTOTodasNominasXquincena(NumeroQuincena, AnioInterface);
        }


        public static DatosBitacoraParaCheque ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(int IdNomina)
        {
            return FoliarConsultasDBSinEntity.ObtenerQuincenaNominaComentId_nomAnImportadoBitacoraParaCheques(IdNomina);
        }




        //********************************************************************************************************************************************************************//




        #region METODOS DE PAGOMATICOS => DISPERSIONES POR TARJETA


        //************************************************************************************************************************************************************************************//
        //****************************************          VERFIFICAR LAS NOMINAS CON PAGOMATICO SI YA ESTAN FOLIADAS O NO  (VERIFICA en SQL)    ********************************************//
        //************************************************************************************************************************************************************************************//
        /*VERIFICA SI YA ESTA FOLIADO UNA NOMINA QUE CONTIENE PAGAMATICO*/
        public static List<AlertaDeNominasFoliadasPagomatico> EstaFoliadaNominaSeleccionadaPagoMatico(int IdNom, int AnioInterface)
        {
            //IdEstaFoliada 
            //0 para decir que no esta foliada      { False }
            //1 para los que estan foliadas         { true  }
            //2 para los que no tienen pagomaticos  { else  }
            List<AlertaDeNominasFoliadasPagomatico> AlertasEncontradas = new List<AlertaDeNominasFoliadasPagomatico>();




            DatosCompletosBitacoraDTO detalleNominaObtenido = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(IdNom, AnioInterface);
            ////int registrosFoliados = FoliarConsultasDBSinEntity.ObtenerRegitrosFoliados(detalleNominaObtenido.An, AnioInterface);
            ////int registrosAFoliar = FoliarConsultasDBSinEntity.ObtenerRegistrosAFoliar(detalleNominaObtenido.An, AnioInterface);

            string consultaRegitrosFoliados = consultasPagomaticos.ObtenerRegitrosFoliados_NominaPagomatico(detalleNominaObtenido.An, detalleNominaObtenido.Anio);
            int registrosFoliados = FoliarConsultasDBSinEntity.ObtenerRegistro_FoliacionPagomatico(consultaRegitrosFoliados);
            string consultaRegistrosAFoliar = consultasPagomaticos.ObtenerRegistrosAFoliar_NominaPagomatico(detalleNominaObtenido.An, detalleNominaObtenido.Anio);
            int registrosAFoliar = FoliarConsultasDBSinEntity.ObtenerRegistro_FoliacionPagomatico(consultaRegistrosAFoliar);


            AlertaDeNominasFoliadasPagomatico nuevaAlerta = new AlertaDeNominasFoliadasPagomatico();

            nuevaAlerta.Id_Nom = detalleNominaObtenido.Id_nom;
            nuevaAlerta.NumeroNomina = detalleNominaObtenido.Nomina;
            nuevaAlerta.Adicional = detalleNominaObtenido.Adicional;
            nuevaAlerta.NombreNomina = detalleNominaObtenido.Coment;
            nuevaAlerta.NumeroRegistrosAFoliar = registrosAFoliar;
            if (registrosAFoliar > 0 && registrosFoliados == registrosAFoliar)
            {
                //Esta Foliado
                nuevaAlerta.IdEstaFoliada = 1;
            }
            else if (registrosAFoliar == 0 && registrosFoliados == 0)
            {
                //si no lee ningun registro es por que no cuenta con pagomaticos 
                nuevaAlerta.IdEstaFoliada = 2;
            }
            else if (registrosAFoliar > 0 && registrosFoliados != registrosAFoliar)
            {
                //No esta Foliado
                nuevaAlerta.IdEstaFoliada = 0;
            }
            AlertasEncontradas.Add(nuevaAlerta);


            return AlertasEncontradas;
        }

        /*VERIFICA SI YA ESTAN FOLIADAS TODAS LAS NOMINAS QUE CONTIENEN PAGOMATICOS CON SUS RESPECTIVOS REGISTROS TOTALES A FOLIAR*/
        public static List<AlertaDeNominasFoliadasPagomatico> VerificarTodasNominaPagoMatico(string Quincena)
        {
            //IdEstaFoliada 
            //0 para decir que no esta foliada      { False }
            //1 para los que estan foliadas         { true  }
            //2 para los que no tienen pagomaticos  { else  }
            List<AlertaDeNominasFoliadasPagomatico> AlertasEncontradas = new List<AlertaDeNominasFoliadasPagomatico>();

            int anio = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(0, 2) + Quincena.Substring(0, 2));

            List<DatosRevicionTodasNominasDTO> DetallesNominasObtenidos = ObtenerTodasNominasXQuincena(Quincena, anio);

            foreach (DatosRevicionTodasNominasDTO nuevaNominaObtenida in DetallesNominasObtenidos)
            {
                // int registrosFoliados = FoliarConsultasDBSinEntity.ObtenerRegitrosFoliados(nuevaNominaObtenida.An, anio);
                //int registrosAFoliar = FoliarConsultasDBSinEntity.ObtenerRegistrosAFoliar(nuevaNominaObtenida.An, anio);

                string consultaRegitrosFoliados = consultasPagomaticos.ObtenerRegitrosFoliados_NominaPagomatico(nuevaNominaObtenida.An, anio);
                int registrosFoliados = FoliarConsultasDBSinEntity.ObtenerRegistro_FoliacionPagomatico(consultaRegitrosFoliados);
                string consultaRegistrosAFoliar = consultasPagomaticos.ObtenerRegistrosAFoliar_NominaPagomatico(nuevaNominaObtenida.An, anio);
                int registrosAFoliar = FoliarConsultasDBSinEntity.ObtenerRegistro_FoliacionPagomatico(consultaRegistrosAFoliar);



                DatosCompletosBitacoraDTO detalleNominaObtenido = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(Convert.ToInt32(nuevaNominaObtenida.Id_Nom), anio);

                AlertaDeNominasFoliadasPagomatico nuevaAlerta = new AlertaDeNominasFoliadasPagomatico();

                nuevaAlerta.Id_Nom = detalleNominaObtenido.Id_nom;
                nuevaAlerta.NumeroNomina = detalleNominaObtenido.Nomina;
                nuevaAlerta.Adicional = detalleNominaObtenido.Adicional;
                nuevaAlerta.NombreNomina = detalleNominaObtenido.Coment;
                nuevaAlerta.NumeroRegistrosAFoliar = registrosAFoliar;

                if (registrosAFoliar > 0 && registrosFoliados == registrosAFoliar)
                {
                    //Esta Foliado
                    nuevaAlerta.IdEstaFoliada = 1;
                }
                else if (registrosAFoliar == 0 && registrosFoliados == 0)
                {
                    //si no lee ningun registro es por que no cuenta con pagomaticos 
                    nuevaAlerta.IdEstaFoliada = 2;
                }
                else if (registrosAFoliar > 0 && registrosFoliados != registrosAFoliar)
                {
                    //No esta Foliado
                    nuevaAlerta.IdEstaFoliada = 0;
                }
                AlertasEncontradas.Add(nuevaAlerta);

            }

            return AlertasEncontradas;
        }


        //****************************************************************************************************************************************************************************************************//
        //*****************        Revicion de Nomina PAGOMATICO => SIRVE PARA LLENAR EL REPORTE DE COMO SE ENCUANTRA EL AN EN SQL Y SABER SI ESTA O NO FOLEADA     ******************************************//
        //****************************************************************************************************************************************************************************************************//
        public static List<ResumenRevicionNominaPDFDTO> ObtenerDatosPersonalesNominaReportePagomatico(string An, int AnioInterface, string Nomina)
        {
            string consulta = consultasPagomaticos.ObtenerConsultaDetalleDatosPersonalesNomina_ReportePagomatico(An, AnioInterface);

           // return FoliarConsultasDBSinEntity.ObtenerDatosPersonalesNomina_ReporteXNominaPagomatico(consulta, Nomina);
            return FoliarConsultasDBSinEntity.ObtenerResumenDatosComoSeEncuentraFolidoEnAN(consulta, Nomina);
        }





        #endregion



        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        //******************************************************* PERMISOS PARA ACCEDER A LOS ARCHIVOS DE UN SERVER  *********************************************************//
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        /* PERMISOS  */
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        //************************************************************************* FOLEAR NOMINAS CON PAGOMATICOS   *********************************************************//
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        public static async Task<List<AlertasAlFolearPagomaticosDTO>> FolearPagomaticoPorNomina(int IdNom, int anio, string Quincena)
        {
            List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();

            AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

         //   List<Tbl_CuentasBancarias> resultadoDatosBanco = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.Activo == true).ToList();

            //Se crea una transaccion para poder actualizar o insertar en la tbl_pagos de la DB foliacion
            var repositorioTblPago = new Repositorio<Tbl_Pagos>(transaccion);

            // int anio = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(0, 2) + Quincena.Substring(0, 2));

            DatosCompletosBitacoraDTO datosCompletosNomina = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(IdNom, anio);



            int numeroRegistrosActualizados_BaseDBF = 0;
            int numeroRegistrosActualizados_AlPHA = 0;
            int registrosInsertadosOActualizados_Foliacion = 0;
            /********************************************************************************************************************************************************/
            /********************************************************************************************************************************************************/
            /************  En caso de un cambio de cuentas bancarias resuelvalo directamente en SQL dentro de la tabla FiltroConsultaDatosPersonalesNomina **********/
            List<Tbl_CuentasBancarias> resultadoDatosBanco = new List<Tbl_CuentasBancarias>();
            
            if(datosCompletosNomina.Anio > 2021)
            {
                //Obtiene los bancos actuales de los cambios de cuentas del 2022
                resultadoDatosBanco = repositorio.ObtenerPorFiltro(x => x.InicioBaja == null && x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.NombreBanco != "BANAMEX").ToList();
            }else if (datosCompletosNomina.Anio < 2022)
            {
                //Obtiene bancos antes del cambio de cuentas del 2021
                resultadoDatosBanco = repositorio.ObtenerPorFiltro(x => x.InicioBaja == true && x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.NombreBanco != "BANAMEX").ToList();
            }

            //Funciona para agregar el banco de banamex segun si sea PENA y el ANNIO
            if (datosCompletosNomina.EsPenA)
            {
                Tbl_CuentasBancarias cuentaBanamexPenA = repositorio.Obtener(x => x.EsPenA == true);
                resultadoDatosBanco.Add( cuentaBanamexPenA );
            }
            else 
            {
                Tbl_CuentasBancarias cuentaBanamex = new Tbl_CuentasBancarias();
                if (datosCompletosNomina.Anio > 2021)
                {
                    //Obtiene los bancos actuales de los cambios de cuentas del 2022
                    cuentaBanamex = repositorio.Obtener(x => x.NombreBanco == "BANAMEX"  && x.EsPenA == null && x.InicioBaja == null);
                }
                else if (datosCompletosNomina.Anio < 2022)
                {
                    //Obtiene bancos antes del cambio de cuentas del 2021
                    cuentaBanamex = repositorio.Obtener(x => x.NombreBanco == "BANAMEX" && x.EsPenA == null && x.InicioBaja == true);
                }
                resultadoDatosBanco.Add(cuentaBanamex);
            }



            int complementoQuincena = Convert.ToInt32(Quincena.Substring(1, 3));
            string ConsultaDetalle_FoliacionPagomatico = consultasPagomaticos.ObtenerConsultaDetalle_FoliacionPagomatico(datosCompletosNomina.An, datosCompletosNomina.Anio, datosCompletosNomina.EsPenA ,   resultadoDatosBanco );
            List<ResumenPersonalAFoliarDTO> resumenPersonalAFoliar = FoliarConsultasDBSinEntity.ObtenerDatosPersonalesNomina_FoliacionPAGOMATICO(ConsultaDetalle_FoliacionPagomatico ,  complementoQuincena, datosCompletosNomina.EsPenA);


            //var repoConsultaDatosPersonalesNomina = new Repositorio<cat_FiltroConsultaDatosPersonalesNomina>(transaccion);
            //bool contieneBancoAzteca = FoliarConsultasDBSinEntity.VerificarSiTieneELCampo_AZTECA(datosCompletosNomina.An, datosCompletosNomina.Anio);
            //string queryFiltroDatosPersonales = repoConsultaDatosPersonalesNomina.Obtener(x => x.EsPena == datosCompletosNomina.EsPenA && x.EsPagomatico == true && x.ContieneAzteca == contieneBancoAzteca ).Consulta;

            //if (anio == Convert.ToInt32(DateTime.Now.Year))
            //{
            //    queryFiltroDatosPersonales = queryFiltroDatosPersonales.Replace("[ANIO]", "");
            //    queryFiltroDatosPersonales = queryFiltroDatosPersonales.Replace("[AN]", "" + datosCompletosNomina.An + "");
            //}
            //else
            //{
            //    queryFiltroDatosPersonales = queryFiltroDatosPersonales.Replace("[ANIO]", "" + anio + "");
            //    queryFiltroDatosPersonales = queryFiltroDatosPersonales.Replace("[AN]", "" + datosCompletosNomina.An + "");
            //}
            //queryFiltroDatosPersonales = queryFiltroDatosPersonales.Replace("\r", "");
            //queryFiltroDatosPersonales = queryFiltroDatosPersonales.Replace("\n", "");
            //queryFiltroDatosPersonales = queryFiltroDatosPersonales.Replace("\t", "");


            //List<ResumenPersonalAFoliarDTO> resumenPersonalAFoliar2 = FoliarConsultasDBSinEntity.ObtenerDatosPersonalesNominaPAGOMATICO(queryFiltroDatosPersonales, Convert.ToInt32(Quincena.Substring(1, 3)), datosCompletosNomina.EsPenA);

            if (resumenPersonalAFoliar == null)
            {
                nuevaAlerta.IdAtencion = 3;
                Advertencias.Add(nuevaAlerta);
                return Advertencias;
            }
            else if (resumenPersonalAFoliar.Count() > 0)
            {

                /********************************************************************************************************************************************************************/
                /********************************************************************************************************************************************************************/
                /***********************             Permite el acceso a una carpeta que se encuentra compartida dentro del servidor            ************************************/

                WindowsImpersonationContext impersonationContext = null;
                IntPtr userHandle = IntPtr.Zero;
                const int LOGON_TYPE_NEW_CREDENTIALS = 9;
                const int LOGON32_PROVIDER_WINNT50 = 3;
                // string domain = @"\\172.19.3.171\";
                // string user = "finanzas" + @"\" + "diego.ruz";
                // string password = "Analista101";
                string domain = @"\\172.19.3.173\";
                string user = "Administrador";
                string password = "Procesosnomina1";

                if (domain == "")
                    domain = System.Environment.MachineName;
                // Llame a LogonUser para obtener un token para el usuario
                bool loggedOn = LogonUser(user,
                                            domain,
                                            password,
                                            LOGON_TYPE_NEW_CREDENTIALS,
                                            LOGON32_PROVIDER_WINNT50,
                                            ref userHandle);

                if (!loggedOn)
                {
                    nuevaAlerta.IdAtencion = 4;
                    Advertencias.Add(nuevaAlerta);
                    return Advertencias;

                }




                using (new Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
                {
                    /*****************************************************************************************************************************************************************/
                    /**********************************************     Actualiza la DFB en una Ruta en RED             **************************************************************/
                    //PRIMER HILO DE EJECUCION


                    //string pathBasesServidor47 = @"\\172.19.3.173\f2\";
                    string pathBasesServidor47 = @"\\172.19.3.173\";



                    string letraRuta = datosCompletosNomina.Ruta.Substring(0, 2);

                    //Si esta en Modo debug entrara
                    //if (System.Diagnostics.Debugger.IsAttached)
                    //{
                    if (letraRuta.ToUpper() == "F:")
                    {
                        pathBasesServidor47 = pathBasesServidor47 + "F2";

                    }
                    else if (letraRuta.ToUpper() == "J:")
                    {
                        pathBasesServidor47 = pathBasesServidor47 + "J2";
                    }
                    //}
                    //else
                    //{
                    //    if (letraRuta.ToUpper() == "F:")
                    //    {
                    //        pathBasesServidor47 = pathBasesServidor47 + "F";

                    //    }
                    //    else if (letraRuta.ToUpper() == "J:")
                    //    {
                    //        pathBasesServidor47 = pathBasesServidor47 + "J";
                    //    }

                    //}

                    //Cuando termina esta if queda algo como  @"\\172.19.3.173\F";
                    datosCompletosNomina.Ruta = datosCompletosNomina.Ruta.Replace("" + letraRuta + "", ""); // \SAGITARI\AYUDAS\ARCHIVOS\            
                    datosCompletosNomina.Ruta = pathBasesServidor47 + datosCompletosNomina.Ruta; //\\172.19.3.173\F\SAGITARI\AYUDAS\ARCHIVOS\



                    Task<string> task_resultadoRegitrosActualizadosDBF_Cadena = Task.Run(() =>
                    {
                        return ActualizacionDFBS.ActualizarDBF_Pagomaticos(datosCompletosNomina.Ruta, datosCompletosNomina.RutaNomina, resumenPersonalAFoliar, datosCompletosNomina.EsPenA);
                        // return NuevaActualizacionDFBS.ActualizarDBF_Pagomaticos(datosCompletosNomina.Ruta, datosCompletosNomina.RutaNomina, resumenPersonalAFoliar, datosCompletosNomina.EsPenA);
                    });


                    string resultado_ActualizacionDBF = await task_resultadoRegitrosActualizadosDBF_Cadena;
                    if (resultado_ActualizacionDBF.Contains("Cannot open file") )
                    {
                        datosCompletosNomina.Ruta = datosCompletosNomina.Ruta.Replace("" + pathBasesServidor47 + "", "***.**.**.**");
                        nuevaAlerta.IdAtencion = 4;
                        nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                        nuevaAlerta.Detalle = "LA BASE : || " + datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina + " || SE ENCUENTRA ABIERTA EN OTRA PC";
                        nuevaAlerta.Solucion = "CIERRE LA BASE E INTENTE FOLIAR DE NUEVO";
                        Advertencias.Add(nuevaAlerta);
                        return Advertencias;
                    }
                    else if ( resultado_ActualizacionDBF.Contains("does not exist."))
                    {
                        datosCompletosNomina.Ruta = datosCompletosNomina.Ruta.Replace("" + pathBasesServidor47 + "", "***.**.**.**");
                        nuevaAlerta.IdAtencion = 4;
                        nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                        nuevaAlerta.Detalle = "LA BASE : || " + datosCompletosNomina.Ruta + datosCompletosNomina.RutaNomina + " || NO EXISTE EN EL SERVER ";
                        nuevaAlerta.Solucion = "COMUNIQUESE CON EL DESARROLLADOR INMEDIATAMENTE";
                        Advertencias.Add(nuevaAlerta);
                        return Advertencias;
                    }


                    /*****************************************************************************************************************************************************************/
                    /**********************************************     Actualiza la base cargada en SQL            **************************************************************/

                    //SEGUNDO HILO DE EJECUCION
                    Task<int> task_resultadoRegitrosActualizados_InterfacesAlPHA = Task.Run(() =>
                    {
                        //return FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql(resumenPersonalAFoliar, datosCompletosNomina, anio);
                        return FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql_transaccionado(resumenPersonalAFoliar, datosCompletosNomina, anio);
                    });








                    foreach (ResumenPersonalAFoliarDTO nuevaPersona in resumenPersonalAFoliar)
                    {

                        Tbl_Pagos pagoAmodificar = null;

                        if (datosCompletosNomina.EsPenA)
                        {
                            pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosCompletosNomina.Anio && x.Id_nom == datosCompletosNomina.Id_nom && x.IdCat_FormaPago_Pagos == 2 /*Por ser pagomatico*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido && x.NumBeneficiario == nuevaPersona.NumBeneficiario);
                        }
                        else
                        {
                            pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosCompletosNomina.Anio && x.Id_nom == datosCompletosNomina.Id_nom && x.IdCat_FormaPago_Pagos == 2 /*Por ser pagomatico*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido);
                        }

                        //Si pagoEncontrado no es null es por que ya fue foliada al menos una vez ya que existe el registro y no es necesario hacer un insert solo un Update
                        ///Insertar un regitro o actualizar en la DB foliacion segun se el caso 
                        if (pagoAmodificar != null)
                        {

                            /***************************************************************************************************************************************************/
                            /***************************************************************************************************************************************************/
                            /*********************             SI ENTRA ES PORQUE YA FUE FOLIADA Y SOLO SE HARA UN UPDATE          *********************************************/

                            pagoAmodificar.Id_nom = datosCompletosNomina.Id_nom;
                            pagoAmodificar.Nomina = datosCompletosNomina.Nomina;
                            pagoAmodificar.An = datosCompletosNomina.An;
                            pagoAmodificar.Adicional = datosCompletosNomina.Adicional;
                            pagoAmodificar.Anio = datosCompletosNomina.Anio;
                            pagoAmodificar.Mes = datosCompletosNomina.Mes;
                            pagoAmodificar.Quincena = Convert.ToInt32(datosCompletosNomina.Quincena);
                            pagoAmodificar.ReferenciaBitacora = datosCompletosNomina.ReferenciaBitacora;
                            pagoAmodificar.Partida = nuevaPersona.Partida;
                            pagoAmodificar.Delegacion = nuevaPersona.Delegacion;
                            pagoAmodificar.RfcEmpleado = nuevaPersona.RFC;
                            pagoAmodificar.NumEmpleado = nuevaPersona.NumEmpleado;
                            pagoAmodificar.NombreEmpleado = nuevaPersona.Nombre;


                            if (datosCompletosNomina.EsPenA)
                            {
                                pagoAmodificar.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(nuevaPersona.CadenaNumEmpleado); //conectarse a funcion y buscar nombre 
                                pagoAmodificar.EsPenA = datosCompletosNomina.EsPenA;
                                pagoAmodificar.BeneficiarioPenA = nuevaPersona.Nombre;
                                pagoAmodificar.NumBeneficiario = string.IsNullOrEmpty(nuevaPersona.NumBeneficiario) ? "B?" : nuevaPersona.NumBeneficiario;
                            }
                            else
                            {
                                pagoAmodificar.NombreEmpleado = nuevaPersona.Nombre;
                                pagoAmodificar.EsPenA = false;
                                pagoAmodificar.BeneficiarioPenA = null;
                                pagoAmodificar.NumBeneficiario = null;
                            }


                            pagoAmodificar.ImporteLiquido = nuevaPersona.Liquido;
                            pagoAmodificar.FolioCheque = nuevaPersona.NumChe;
                            pagoAmodificar.FolioCFDI = nuevaPersona.FolioCFDI;



                            pagoAmodificar.IdTbl_CuentaBancaria_BancoPagador = nuevaPersona.IdBancoPagador;
                            pagoAmodificar.IdCat_FormaPago_Pagos = 2; //1 = cheque , 2 = Pagomatico 
                            pagoAmodificar.IdCat_EstadoPago_Pagos = 1; //1 = Transito, 2= Pagado, 3 = Precancelado , 4 No definido

                            //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado



                            string cadenaDeIntegridad = datosCompletosNomina.Id_nom + " || " + datosCompletosNomina.Nomina + " || " + datosCompletosNomina.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.BancoX + " || " + nuevaPersona.CuentaX + " || " + nuevaPersona.NumBeneficiario;
                            EncriptarCadena encriptar = new EncriptarCadena();
                            pagoAmodificar.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                            pagoAmodificar.Activo = true;



                         //   Tbl_Pagos pagoModificado = repositorioTblPago.Modificar_Transaccionadamente(pagoAmodificar);

                            //if (!string.IsNullOrEmpty(Convert.ToString(pagoModificado.FolioCheque)))
                            //{
                               registrosInsertadosOActualizados_Foliacion++;

                            //}


                        }
                        else
                        {
                            /***************************************************************************************************************************************************/
                            /***************************************************************************************************************************************************/
                            /*********************             SI ENTRA ES PORQUE AUN NO ESTA FOLIADO Y SE HARAN INSERTS A DBfOLIACION         *********************************************/

                            Tbl_Pagos nuevoPago = new Tbl_Pagos();

                            nuevoPago.Id_nom = datosCompletosNomina.Id_nom;
                            nuevoPago.Nomina = datosCompletosNomina.Nomina;
                            nuevoPago.An = datosCompletosNomina.An;
                            nuevoPago.Adicional = datosCompletosNomina.Adicional;
                            nuevoPago.Anio = datosCompletosNomina.Anio;
                            nuevoPago.Mes = datosCompletosNomina.Mes;
                            nuevoPago.Quincena = Convert.ToInt32(datosCompletosNomina.Quincena);
                            nuevoPago.ReferenciaBitacora = datosCompletosNomina.ReferenciaBitacora;
                            nuevoPago.Partida = nuevaPersona.Partida;
                            nuevoPago.Delegacion = nuevaPersona.Delegacion;
                            nuevoPago.RfcEmpleado = nuevaPersona.RFC;
                            nuevoPago.NumEmpleado = nuevaPersona.NumEmpleado;
                            nuevoPago.NombreEmpleado = nuevaPersona.Nombre;

                            if (datosCompletosNomina.EsPenA)
                            {
                                nuevoPago.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(nuevaPersona.CadenaNumEmpleado); //conectarse a funcion y buscar nombre 
                                nuevoPago.EsPenA = true;
                                nuevoPago.BeneficiarioPenA = nuevaPersona.Nombre;
                                nuevoPago.NumBeneficiario = nuevaPersona.NumBeneficiario;

                            }
                            else
                            {
                                nuevoPago.NombreEmpleado = nuevaPersona.Nombre;
                                nuevoPago.EsPenA = false;
                                nuevoPago.BeneficiarioPenA = null;
                                nuevoPago.NumBeneficiario = null;
                            }

                            nuevoPago.ImporteLiquido = nuevaPersona.Liquido;
                            nuevoPago.FolioCheque = nuevaPersona.NumChe;
                            nuevoPago.FolioCFDI = nuevaPersona.FolioCFDI;

                            nuevoPago.IdTbl_CuentaBancaria_BancoPagador = nuevaPersona.IdBancoPagador;
                            nuevoPago.IdCat_FormaPago_Pagos = 2; //1 = cheque , 2 = Pagomatico 

                            nuevoPago.IdCat_EstadoPago_Pagos = 1; //1 = Transito, 2= Pagado
                                                                  //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado


                            string cadenaDeIntegridad = datosCompletosNomina.Id_nom + " || " + datosCompletosNomina.Nomina + " || " + datosCompletosNomina.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.BancoX + " || " + nuevaPersona.CuentaX + " || " + nuevaPersona.NumBeneficiario;
                            EncriptarCadena encriptar = new EncriptarCadena();
                            nuevoPago.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                            nuevoPago.Activo = true;



                            Tbl_Pagos pagoInsertado = repositorioTblPago.Agregar_Transaccionadamente(nuevoPago);


                            if (!string.IsNullOrEmpty(Convert.ToString(pagoInsertado.FolioCheque)))
                            {
                                registrosInsertadosOActualizados_Foliacion++;

                            }

                        }

                    }



                    numeroRegistrosActualizados_AlPHA = await task_resultadoRegitrosActualizados_InterfacesAlPHA;


                    numeroRegistrosActualizados_BaseDBF = Convert.ToInt32(resultado_ActualizacionDBF);


                    ///Guarda Un lote de transacciones tanto para modificar o hacer inserts  
                    if ((registrosInsertadosOActualizados_Foliacion + numeroRegistrosActualizados_AlPHA) == (numeroRegistrosActualizados_BaseDBF * 2))
                    {


                        nuevaAlerta.IdAtencion = 0;
                        nuevaAlerta.NumeroNomina = datosCompletosNomina.Nomina;
                        nuevaAlerta.NombreNomina = datosCompletosNomina.Coment;
                        nuevaAlerta.Detalle = "";
                        nuevaAlerta.Solucion = "";
                        nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                        nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados_AlPHA;
                        nuevaAlerta.UltimoFolioUsado = resumenPersonalAFoliar.Max(x => x.NumChe);


                        Advertencias.Add(nuevaAlerta);
                        transaccion.GuardarCambios();
                    }
                    else
                    {

                        //Si entra en esta condicion es por que uno o mas registros no se foliaron y se necesita refoliar toda la nomina 

                        nuevaAlerta.IdAtencion = 0;
                        nuevaAlerta.NumeroNomina = datosCompletosNomina.Nomina;
                        nuevaAlerta.NombreNomina = datosCompletosNomina.Coment;
                        nuevaAlerta.Detalle = "OCURRIO UN ERROR EN LA FOLIACION";
                        nuevaAlerta.Solucion = "IFNN";
                        nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                        nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados_AlPHA;
                        nuevaAlerta.UltimoFolioUsado = resumenPersonalAFoliar.Max(x => x.NumChe);

                        numeroRegistrosActualizados_AlPHA = 0;

                        Advertencias.Add(nuevaAlerta);
                      //  FoliarConsultasDBSinEntity.LimpiarBaseNominaEnSql(datosCompletosNomina, anio);
                        transaccion.Dispose();
                    }





                }
            }





            return Advertencias;
        }

        public static async Task<List<AlertasAlFolearPagomaticosDTO>> FolearPagomaticoPorNomina_TIEMPO_DE_RESPUESTA_MEJORADO(int IdNom, int anio, string Quincena)
        {
            List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();
            AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            int numeroRegistrosActualizados_BaseDBF = 0;
            int numeroRegistrosActualizados_AlPHA = 0;
            int registrosInsertadosOActualizados_Foliacion = 0;
            DatosCompletosBitacoraDTO datosCompletosNomina = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(IdNom, anio);

            /********************************************************************************************************************************************************/
            /************  En caso de un cambio de cuentas bancarias resuelvalo directamente en SQL dentro de la tabla FiltroConsultaDatosPersonalesNomina **********/
            /********************************************************************************************************************************************************/
            List<Tbl_CuentasBancarias> resultadoDatosBanco = new List<Tbl_CuentasBancarias>();

            if (datosCompletosNomina.Anio > 2021)
            {
                //Obtiene los bancos actuales de los cambios de cuentas del 2022
                resultadoDatosBanco = repositorio.ObtenerPorFiltro(x => x.InicioBaja == null && x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.NombreBanco != "BANAMEX").ToList();
            }
            else if (datosCompletosNomina.Anio < 2022)
            {
                //Obtiene bancos antes del cambio de cuentas del 2021
                resultadoDatosBanco = repositorio.ObtenerPorFiltro(x => x.InicioBaja == true && x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.NombreBanco != "BANAMEX").ToList();
            }

            //Funciona para agregar el banco de banamex segun si sea PENA y el ANNIO
            if (datosCompletosNomina.EsPenA)
            {
                Tbl_CuentasBancarias cuentaBanamexPenA = repositorio.Obtener(x => x.EsPenA == true);
                resultadoDatosBanco.Add(cuentaBanamexPenA);
            }
            else
            {
                Tbl_CuentasBancarias cuentaBanamex = new Tbl_CuentasBancarias();
                if (datosCompletosNomina.Anio > 2021)
                {
                    //Obtiene los bancos actuales de los cambios de cuentas del 2022
                    cuentaBanamex = repositorio.Obtener(x => x.NombreBanco == "BANAMEX" && x.EsPenA == null && x.InicioBaja == null);
                }
                else if (datosCompletosNomina.Anio < 2022)
                {
                    //Obtiene bancos antes del cambio de cuentas del 2021
                    cuentaBanamex = repositorio.Obtener(x => x.NombreBanco == "BANAMEX" && x.EsPenA == null && x.InicioBaja == true);
                }
                resultadoDatosBanco.Add(cuentaBanamex);
            }
            int complementoQuincena = Convert.ToInt32(Quincena.Substring(1, 3));
            string ConsultaDetalle_FoliacionPagomatico = consultasPagomaticos.ObtenerConsultaDetalle_FoliacionPagomatico(datosCompletosNomina.An, datosCompletosNomina.Anio, datosCompletosNomina.EsPenA, resultadoDatosBanco);
            List<ResumenPersonalAFoliarDTO> resumenPersonalAFoliar = FoliarConsultasDBSinEntity.ObtenerDatosPersonalesNomina_FoliacionPAGOMATICO(ConsultaDetalle_FoliacionPagomatico, complementoQuincena, datosCompletosNomina.EsPenA);

            if (resumenPersonalAFoliar == null)
            {
                nuevaAlerta.IdAtencion = 3;
                Advertencias.Add(nuevaAlerta);
                return Advertencias;
            }
            else if (resumenPersonalAFoliar.Count() > 0)
            {
                /********************************************************************************************************************************************************************/
                /***********************             Permite el acceso a una carpeta que se encuentra compartida dentro del servidor            ************************************/
                /********************************************************************************************************************************************************************/
                AlertasAlFolearPagomaticosDTO alertaEncontrada = FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(1 /* Se redireccionara a foliar una DBF */ , datosCompletosNomina, resumenPersonalAFoliar , "" /* no lleva una condicion ya que el update se hace con un filtro especifico*/);
                if (alertaEncontrada.IdAtencion != 200 ) 
                {
                    Advertencias.Add(alertaEncontrada);
                    return Advertencias;
                }



                /*******************************************************************************************************************************************************************************/
                /**********************************************     Actualiza la base cargada en SQL            ********************************************************************************/
                /*******************************************************************************************************************************************************************************/
                //HILO DE EJECUCION EN SEGUNDO PLANO
                Task<int> task_resultadoRegitrosActualizados_InterfacesAlPHA = Task.Run(() =>
                {
                   return FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql_transaccionado(resumenPersonalAFoliar, datosCompletosNomina, anio);
                });


                /*******************************************************************************************************************************************************************************/
                /**********************************************     Actualiza o Inserta resgistros     *****************************************************************************************/
                /*******************************************************************************************************************************************************************************/
                //INSERTAR PAGOS
                 List<Tbl_Pagos> pagosNuevosAinsertar = new List<Tbl_Pagos>();
                //ACTUALIZAR PAGOS
                 List<ActualizarFoliacionPagomaticoTblPagoDTO> ActualizarPagosEnTblPagos = new List<ActualizarFoliacionPagomaticoTblPagoDTO>();

                 EncriptarCadena encriptar = new EncriptarCadena();
                 bool Actualizar = false;
                
                 var repositorioTblPago = new Repositorio<Tbl_Pagos>(transaccion);
                 foreach (ResumenPersonalAFoliarDTO nuevaPersona in resumenPersonalAFoliar)
                 {
                   Tbl_Pagos pagoAmodificar = null;

                   if (datosCompletosNomina.EsPenA)
                   {
                     pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosCompletosNomina.Anio && x.Id_nom == datosCompletosNomina.Id_nom && x.IdCat_FormaPago_Pagos == 2 /*Por ser pagomatico*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido && x.NumBeneficiario == nuevaPersona.NumBeneficiario);
                   }
                   else
                   {
                     pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosCompletosNomina.Anio && x.Id_nom == datosCompletosNomina.Id_nom && x.IdCat_FormaPago_Pagos == 2 /*Por ser pagomatico*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido);
                   }

                    //Si pagoEncontrado no es null es por que ya fue foliada al menos una vez ya que existe el registro y no es necesario hacer un insert solo un Update
                    ///Insertar un regitro o actualizar en la DB foliacion segun se el caso 
                    if (pagoAmodificar != null)
                    {
                       Actualizar = true;
                       /*******************************************************************************************************************************************************************************/
                       /*********************                         SI ENTRA ES PORQUE YA FUE FOLIADA Y SOLO SE HARA UN UPDATE                 *******************************************************/
                       /*******************************************************************************************************************************************************************************/
                       ActualizarFoliacionPagomaticoTblPagoDTO nuevaActualizacion = new ActualizarFoliacionPagomaticoTblPagoDTO();
                       nuevaActualizacion.IdPago = pagoAmodificar.Id;
                       nuevaActualizacion.IdTbl_CuentaBancaria_BancoPagador = nuevaPersona.IdBancoPagador;
                       nuevaActualizacion.FolioCheque = nuevaPersona.NumChe;
                       string cadenaDeIntegridad = datosCompletosNomina.Id_nom + " || " + datosCompletosNomina.Nomina + " || " + datosCompletosNomina.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.BancoX + " || " + nuevaPersona.CuentaX + " || " + nuevaPersona.NumBeneficiario;
                       nuevaActualizacion.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                       ActualizarPagosEnTblPagos.Add(nuevaActualizacion);




                        //si sucede algun problema que no este foliando revisar con estas lineas comentadas por que no esta actualizando la misma cantidad de registros
                        //El detalle sucedio en desarrollo por que al verificar una suspencion canmbio su IdCat_FormaPago_Pagos a 1 y es por eso que no lo encuentra, en teorio esto nunca deberia suceder ya no normalmente una vez que se folea no se suspende y se vuelve a refoliar sino es antes 
                        registrosInsertadosOActualizados_Foliacion++;

                        //if (ActualizarPagosEnTblPagos.Count() != registrosInsertadosOActualizados_Foliacion)
                        //{
                        //        int nocorrecto = 0;
                        //}


                    }
                    else
                    {
                       /*********************             SI ENTRA ES PORQUE AUN NO ESTA FOLIADO Y SE HARAN INSERTS A DBfOLIACION         *********************************************/
                       Tbl_Pagos nuevoPago = new Tbl_Pagos();
                       nuevoPago.Id_nom = datosCompletosNomina.Id_nom;
                       nuevoPago.Nomina = datosCompletosNomina.Nomina;
                       nuevoPago.An = datosCompletosNomina.An;
                       nuevoPago.Adicional = datosCompletosNomina.Adicional;
                       nuevoPago.Anio = datosCompletosNomina.Anio;
                       nuevoPago.Mes = datosCompletosNomina.Mes;
                       nuevoPago.Quincena = Convert.ToInt32(datosCompletosNomina.Quincena);
                       nuevoPago.ReferenciaBitacora = datosCompletosNomina.ReferenciaBitacora;
                       nuevoPago.Partida = nuevaPersona.Partida;
                       nuevoPago.Delegacion = nuevaPersona.Delegacion;
                       nuevoPago.RfcEmpleado = nuevaPersona.RFC;
                       nuevoPago.NumEmpleado = nuevaPersona.NumEmpleado;
                       nuevoPago.NombreEmpleado = nuevaPersona.Nombre;

                      if (datosCompletosNomina.EsPenA)
                      {
                         nuevoPago.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(nuevaPersona.CadenaNumEmpleado); //conectarse a funcion y buscar nombre 
                         nuevoPago.EsPenA = true;
                         nuevoPago.BeneficiarioPenA = nuevaPersona.Nombre;
                         nuevoPago.NumBeneficiario = nuevaPersona.NumBeneficiario;

                      }
                      else
                      {
                         nuevoPago.NombreEmpleado = nuevaPersona.Nombre;
                         nuevoPago.EsPenA = false;
                         nuevoPago.BeneficiarioPenA = null;
                         nuevoPago.NumBeneficiario = null;
                      }

                         nuevoPago.ImporteLiquido = nuevaPersona.Liquido;
                         nuevoPago.FolioCheque = nuevaPersona.NumChe;
                         nuevoPago.FolioCFDI = nuevaPersona.FolioCFDI;

                         nuevoPago.IdTbl_CuentaBancaria_BancoPagador = nuevaPersona.IdBancoPagador;
                         nuevoPago.IdCat_FormaPago_Pagos = 2; //1 = cheque , 2 = Pagomatico 

                         nuevoPago.IdCat_EstadoPago_Pagos = 1; //1 = Transito, 2= Pagado
                                                                  //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado


                         string cadenaDeIntegridad = datosCompletosNomina.Id_nom + " || " + datosCompletosNomina.Nomina + " || " + datosCompletosNomina.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.BancoX + " || " + nuevaPersona.CuentaX + " || " + nuevaPersona.NumBeneficiario;
                         nuevoPago.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                         nuevoPago.Activo = true;

                         pagosNuevosAinsertar.Add(nuevoPago);

                         registrosInsertadosOActualizados_Foliacion++;
                        }

                    }


                /*****************************************************************************************************************************************************************************************/
                /**********************************************            Total de registros Actualizados e Insertados          *************************************************************************/
                /*****************************************************************************************************************************************************************************************/
                int resultadoActualizacionOInsercionSegunElCaso = 0;
                    if (Actualizar)
                    {
                        resultadoActualizacionOInsercionSegunElCaso = FoliarConsultasDBSinEntity.ActualizarTblPagos_DespuesDeFoliacion(ActualizarPagosEnTblPagos);
                    }
                    else 
                    {
                        resultadoActualizacionOInsercionSegunElCaso = repositorioTblPago.Agregar_EntidadesMasivamente(pagosNuevosAinsertar);
                    }

                    //Espera el Hilo donde se esta actualizando el AN en alpha
                    numeroRegistrosActualizados_AlPHA = await task_resultadoRegitrosActualizados_InterfacesAlPHA;

                    //Convierte la cadena de registros en el numero de registros actualizados en la DBF
                    if(alertaEncontrada.IdAtencion == 200)
                    numeroRegistrosActualizados_BaseDBF = alertaEncontrada.NumeroRegistrosActualizados;



                /*****************************************************************************************************************************************************************************************/
                /**********************************************                 Validacion del estandart de Foliacion                *********************************************************************/
                /*****************************************************************************************************************************************************************************************/
                ///Guarda Un lote de transacciones tanto para modificar o hacer inserts  
                ///
                int registrosFoliados = resultadoActualizacionOInsercionSegunElCaso + numeroRegistrosActualizados_AlPHA + numeroRegistrosActualizados_BaseDBF;
                if ((resultadoActualizacionOInsercionSegunElCaso + numeroRegistrosActualizados_AlPHA) == (numeroRegistrosActualizados_BaseDBF * 2))
                {
                    nuevaAlerta.IdAtencion = 0;
                    nuevaAlerta.NumeroNomina = datosCompletosNomina.Nomina;
                    nuevaAlerta.NombreNomina = datosCompletosNomina.Coment;
                    nuevaAlerta.Detalle = "";
                    nuevaAlerta.Solucion = "";
                    nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                    nuevaAlerta.RegistrosFoliados = registrosFoliados / 3;
                    nuevaAlerta.UltimoFolioUsado = 0;
                    Advertencias.Add(nuevaAlerta);
                }
                else
                {
                    //Si entra en esta condicion es por que uno o mas registros no se foliaron y se necesita refoliar toda la nomina 38409
                    List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, datosCompletosNomina.Anio);
                    string condicionParaSerPagomatico = consultasPagomaticos.ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);
                       
                    int registrosLimpiadosAN = FoliarConsultasDBSinEntity.LimpiarANSql_IncumplimientoCalidadFoliacion(datosCompletosNomina.Anio, datosCompletosNomina.An, condicionParaSerPagomatico , datosCompletosNomina.Id_nom );
                    int registrosLimpiadosTblPagos = FoliarConsultasDBSinEntity.LimpiarTblPagos_IncumplimientoCalidadFoliacion(datosCompletosNomina.Anio , Convert.ToInt32( datosCompletosNomina.Quincena) , datosCompletosNomina.Id_nom , 2 /* Tipo de pago 2 por ser DISPERCION */);

                    List<ResumenPersonalAFoliarDTO> resumenPersonalVacio = new List<ResumenPersonalAFoliarDTO>();
                    DatosCompletosBitacoraDTO datosCompletosNominaNuevo = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(IdNom, anio);
                    AlertasAlFolearPagomaticosDTO alertaEncontradaLimpiezaDBF = FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(6 /* Numero para redireccionar a una limpiaza de la base con una condicion */ , datosCompletosNominaNuevo, resumenPersonalVacio /* Este paramatro se envia vavio por que no se ocupa para la esta opcion */ , condicionParaSerPagomatico);
                    if (alertaEncontradaLimpiezaDBF.IdAtencion != 200)
                    {
                        Advertencias.Add(alertaEncontradaLimpiezaDBF);
                        return Advertencias;
                    }

                    nuevaAlerta.IdAtencion = 4;
                    nuevaAlerta.NumeroNomina = datosCompletosNomina.Nomina;
                    nuevaAlerta.NombreNomina = datosCompletosNomina.Coment;
                    nuevaAlerta.Detalle =  registrosLimpiadosAN == registrosLimpiadosTblPagos? "OCURRIO UN ERROR EN LA FOLIACION" : "ERROR EN LA FOLIACION, NO SE PUDIERON LIMPIERAR LOS CAMPOS AFECTADOS CORRECTAMENTE EN TBL_PAGOS";
                    nuevaAlerta.Solucion = "VUELVA A FOLIAR DE NUEVO";
                    nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                    nuevaAlerta.RegistrosFoliados = registrosFoliados/3 ;
                    nuevaAlerta.UltimoFolioUsado = 0;
                    numeroRegistrosActualizados_AlPHA = 0;
                    Advertencias.Add(nuevaAlerta);
                    return Advertencias;
                }
            }
            return Advertencias;
        }







        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        //*************************************************************************     FOLEAR NOMINAS CON CHEQUES   *********************************************************//
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        public static async Task<List<AlertasAlFolearPagomaticosDTO>> FoliarChequesPorNomina(FoliarFormasPagoDTO NuevaNominaFoliar, string Observa, List<FoliosAFoliarInventario> chequesVerificadosFoliar)
        {

            List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();
            AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();


            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            Tbl_CuentasBancarias bancoEncontrado = repositorio.Obtener(x => x.Id == NuevaNominaFoliar.IdBancoPagador && x.Activo == true);

            var filtroDelegaciones = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);

            //Se crea una transaccion para poder actualizar o insertar en la tbl_pagos de la DB foliacion
            var repositorioTblPago = new Repositorio<Tbl_Pagos>(transaccion);


            //sirve para modificar los datos del inventario
            var repositorioInventarioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            var repositorioContenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);



            DatosCompletosBitacoraDTO datosNominaCompleto = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(NuevaNominaFoliar.IdNomina, NuevaNominaFoliar.AnioInterfaz);
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosNominaCompleto.An, datosNominaCompleto.Anio);

            string delegacionSeleccionada = filtroDelegaciones.Obtener(x => x.GrupoImpresionDelegacion == NuevaNominaFoliar.IdDelegacion).DelegacionesIncluidas.ToString();

            bool EsSindi = false;
            string consultaLista = "";
            if (NuevaNominaFoliar.IdGrupoFoliacion == 0)
            {
                if (NuevaNominaFoliar.Confianza > 0 && NuevaNominaFoliar.Sindicato == 0)
                {
                    //son de confianza
                    consultaLista = consultasCheques.ObtenerConsultaOrdenDeFoliacionPorDelegacion_GeneralDescentralizada(datosNominaCompleto.An, datosNominaCompleto.Anio, delegacionSeleccionada, EsSindi, bancosContenidosEnAn);
                }
                else if (NuevaNominaFoliar.Confianza == 0 && NuevaNominaFoliar.Sindicato > 0)
                {
                    //Son sindicalizados
                    EsSindi = true;
                    consultaLista = consultasCheques.ObtenerConsultaOrdenDeFoliacionPorDelegacion_GeneralDescentralizada( datosNominaCompleto.An, datosNominaCompleto.Anio, delegacionSeleccionada, EsSindi, bancosContenidosEnAn);
                }
            }else if (NuevaNominaFoliar.IdGrupoFoliacion == 1)
            {
                //consultaLista = ConsultasSQLOtrasNominasConCheques.ObtenerCadenaConsultaReportePDF_GenericOtrasNominas_PartidaCompleta(datosNominaCompleto.An, NuevaNominaFoliar.AnioInterfaz, delegacionSeleccionada, datosNominaCompleto.EsPenA);
                consultaLista = consultasCheques.ObtenerConsultaOrdenDeFoliacionPorDelegacion_OtrasNominasYPenA(datosNominaCompleto.An, datosNominaCompleto.Anio, delegacionSeleccionada, datosNominaCompleto.EsPenA, bancosContenidosEnAn);
            }

            List<ResumenPersonalAFoliarDTO> resumenPersonalFoliar = new List<ResumenPersonalAFoliarDTO>();
            resumenPersonalFoliar = FoliarConsultasDBSinEntity.ObtenerResumenDatosFormasDePagoFoliar(datosNominaCompleto.EsPenA, Observa, consultaLista, bancoEncontrado, NuevaNominaFoliar.RangoInicial, NuevaNominaFoliar.Inhabilitado, Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoInicial), Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoFinal));

            /******************************************************************************/
            //Foliar en DBF

            int numeroRegistrosActualizados_BaseDBF = 0;
            int numeroRegistrosActualizados_AlPHA = 0;
            int registrosInsertadosOActualizados_Foliacion = 0;

            WindowsImpersonationContext impersonationContext = null;
            IntPtr userHandle = IntPtr.Zero;
            const int LOGON_TYPE_NEW_CREDENTIALS = 9;
            const int LOGON32_PROVIDER_WINNT50 = 3;
            // string domain = @"\\172.19.3.171\";
            // string user = "finanzas" + @"\" + "diego.ruz";
            // string password = "Analista101";
            string domain = @"\\172.19.3.173\";
            string user = "Administrador";
            string password = "Procesosnomina1";

            if (domain == "")
                domain = System.Environment.MachineName;
            // Llame a LogonUser para obtener un token para el usuario
            bool loggedOn = LogonUser(user,
                                        domain,
                                        password,
                                        LOGON_TYPE_NEW_CREDENTIALS,
                                        LOGON32_PROVIDER_WINNT50,
                                        ref userHandle);

            if (!loggedOn)
            {
                nuevaAlerta.IdAtencion = 4;
                Advertencias.Add(nuevaAlerta);
                return Advertencias;

            }




            using (new Negocios.NetworkConnection(domain, new System.Net.NetworkCredential(user, password)))
            {


                //string pathBasesServidor47 = @"\\172.19.3.173\f2\";
                string pathBasesServidor47 = @"\\172.19.3.173\";



                string letraRuta = datosNominaCompleto.Ruta.Substring(0, 2);

                //Si esta en Modo debug entrara
                //if (System.Diagnostics.Debugger.IsAttached)
                //{
                if (letraRuta.ToUpper() == "F:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "F2";

                }
                else if (letraRuta.ToUpper() == "J:")
                {
                    pathBasesServidor47 = pathBasesServidor47 + "J2";
                }
                //}
                //else
                //{
                //    if (letraRuta.ToUpper() == "F:")
                //    {
                //        pathBasesServidor47 = pathBasesServidor47 + "F";

                //    }
                //    else if (letraRuta.ToUpper() == "J:")
                //    {
                //        pathBasesServidor47 = pathBasesServidor47 + "J";
                //    }

                //}

                //Cuando termina esta if queda algo como  @"\\172.19.3.173\F";
                datosNominaCompleto.Ruta = datosNominaCompleto.Ruta.Replace("" + letraRuta + "", ""); // \SAGITARI\AYUDAS\ARCHIVOS\            
                datosNominaCompleto.Ruta = pathBasesServidor47 + datosNominaCompleto.Ruta; //\\172.19.3.173\F\SAGITARI\AYUDAS\ARCHIVOS\



                Task<string> task_resultadoRegitrosActualizadosDBF_Cadena = Task.Run(() =>
                {
                    return ActualizacionDFBS.ActualizarDBF_Cheques(datosNominaCompleto.Ruta, datosNominaCompleto.RutaNomina, resumenPersonalFoliar, datosNominaCompleto.EsPenA);
                    //return NuevaActualizacionDFBS.ActualizarDBF_Cheques(datosNominaCompleto.Ruta, datosNominaCompleto.RutaNomina, resumenPersonalFoliar, datosNominaCompleto.EsPenA);
                });


                string resultado_ActualizacionDBF = await task_resultadoRegitrosActualizadosDBF_Cadena;
                if (resultado_ActualizacionDBF.Contains("Cannot open file"))
                {
                    datosNominaCompleto.Ruta = datosNominaCompleto.Ruta.Replace("" + pathBasesServidor47 + "", "***.**.**.**");
                    nuevaAlerta.IdAtencion = 4;
                    nuevaAlerta.Id_Nom = Convert.ToString(datosNominaCompleto.Id_nom);
                    nuevaAlerta.Detalle = "LA BASE : || " + datosNominaCompleto.Ruta + datosNominaCompleto.RutaNomina + " || SE ENCUENTRA ABIERTA EN OTRA PC";
                    nuevaAlerta.Solucion = "CIERRE LA BASE E INTENTE FOLIAR DE NUEVO";
                    Advertencias.Add(nuevaAlerta);
                    return Advertencias;
                }


                /*****************************************************************************************************************************************************************/
                /**********************************************     Actualiza la base cargada en SQL            **************************************************************/

                //SEGUNDO HILO DE EJECUCION
                Task<int> task_resultadoRegitrosActualizados_InterfacesAlPHA = Task.Run(() =>
                {
                    //return FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql(resumenPersonalAFoliar, datosCompletosNomina, anio);
                    return FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql_transaccionado_Cheque(resumenPersonalFoliar, datosNominaCompleto, NuevaNominaFoliar.AnioInterfaz);
                });





                foreach (ResumenPersonalAFoliarDTO nuevaPersona in resumenPersonalFoliar)
                {

                    Tbl_Pagos pagoAmodificar = null;

                    if (datosNominaCompleto.EsPenA)
                    {
                        pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosNominaCompleto.Anio && x.Id_nom == datosNominaCompleto.Id_nom && x.IdCat_FormaPago_Pagos == 1 /*Por ser cheque*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido && x.NumBeneficiario == nuevaPersona.NumBeneficiario);
                    }
                    else
                    {
                        pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosNominaCompleto.Anio && x.Id_nom == datosNominaCompleto.Id_nom && x.IdCat_FormaPago_Pagos == 1 /*Por ser cheuqe*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido);
                    }

                    //Si pagoEncontrado no es null es por que ya fue foliada al menos una vez ya que existe el registro y no es necesario hacer un insert solo un Update
                    ///Insertar un regitro o actualizar en la DB foliacion segun se el caso 
                    if (pagoAmodificar != null)
                    {

                        /***************************************************************************************************************************************************/
                        /***************************************************************************************************************************************************/
                        /*********************             SI ENTRA ES PORQUE YA FUE FOLIADA Y SOLO SE HARA UN UPDATE          *********************************************/

                        pagoAmodificar.Id_nom = datosNominaCompleto.Id_nom;
                        pagoAmodificar.Nomina = datosNominaCompleto.Nomina;
                        pagoAmodificar.An = datosNominaCompleto.An;
                        pagoAmodificar.Adicional = datosNominaCompleto.Adicional;
                        pagoAmodificar.Anio = datosNominaCompleto.Anio;
                        pagoAmodificar.Mes = datosNominaCompleto.Mes;
                        pagoAmodificar.Quincena = Convert.ToInt32(datosNominaCompleto.Quincena);
                        pagoAmodificar.ReferenciaBitacora = datosNominaCompleto.ReferenciaBitacora;
                        pagoAmodificar.Partida = nuevaPersona.Partida;
                        pagoAmodificar.Delegacion = nuevaPersona.Delegacion;
                        pagoAmodificar.RfcEmpleado = nuevaPersona.RFC;
                        pagoAmodificar.NumEmpleado = nuevaPersona.NumEmpleado;
                        pagoAmodificar.NombreEmpleado = nuevaPersona.Nombre;


                        if (datosNominaCompleto.EsPenA)
                        {
                            pagoAmodificar.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(nuevaPersona.CadenaNumEmpleado); //conectarse a funcion y buscar nombre 
                            pagoAmodificar.EsPenA = datosNominaCompleto.EsPenA;
                            pagoAmodificar.BeneficiarioPenA = nuevaPersona.Nombre;
                            pagoAmodificar.NumBeneficiario = string.IsNullOrEmpty(nuevaPersona.NumBeneficiario) ? "B?" : nuevaPersona.NumBeneficiario;
                        }
                        else
                        {
                            pagoAmodificar.NombreEmpleado = nuevaPersona.Nombre;
                            pagoAmodificar.EsPenA = false;
                            pagoAmodificar.BeneficiarioPenA = null;
                            pagoAmodificar.NumBeneficiario = null;
                        }


                        pagoAmodificar.ImporteLiquido = nuevaPersona.Liquido;
                        pagoAmodificar.FolioCheque = nuevaPersona.NumChe;
                        pagoAmodificar.FolioCFDI = nuevaPersona.FolioCFDI;



                        pagoAmodificar.IdTbl_CuentaBancaria_BancoPagador = nuevaPersona.IdBancoPagador;
                        pagoAmodificar.IdCat_FormaPago_Pagos = 1; //1 = cheque , 2 = Pagomatico 
                        pagoAmodificar.IdCat_EstadoPago_Pagos = 1; //1 = Transito, 2= Pagado, 3 = Precancelado , 4 No definido
                                                                   //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado


                        string cadenaDeIntegridad = datosNominaCompleto.Id_nom + " || " + datosNominaCompleto.Nomina + " || " + datosNominaCompleto.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.BancoX + " || " + nuevaPersona.CuentaX + " || " + nuevaPersona.NumBeneficiario;
                        EncriptarCadena encriptar = new EncriptarCadena();
                        pagoAmodificar.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                        pagoAmodificar.Activo = true;


                        /*Guardar los cheques y descontarlos del inventario*/
                        FoliosAFoliarInventario foliodisponileEncontrado = chequesVerificadosFoliar.Where(x => x.Folio == pagoAmodificar.FolioCheque).FirstOrDefault();
                        Tbl_InventarioDetalle folioEnInventarioEncontrado = repositorioInventarioDetalle.Obtener(x => x.Id == foliodisponileEncontrado.Id);
                        folioEnInventarioEncontrado.IdIncidencia = 3; //3 porque ya fue foliado por primera vez
                        folioEnInventarioEncontrado.FechaIncidencia = DateTime.Now;
                        pagoAmodificar.IdTbl_InventarioDetalle = folioEnInventarioEncontrado.Id;
                        repositorioInventarioDetalle.Modificar_Transaccionadamente(folioEnInventarioEncontrado);


                        Tbl_InventarioContenedores descontarFolioDelContenedor = repositorioContenedores.Obtener(x => x.Id == folioEnInventarioEncontrado.IdContenedor);
                        descontarFolioDelContenedor.FormasFoliadas += 1;
                        descontarFolioDelContenedor.FormasDisponiblesActuales -= 1;
                        repositorioContenedores.Modificar_Transaccionadamente(descontarFolioDelContenedor);

                        Tbl_Inventario descontarFolioDeInventario = repositorioInventario.Obtener(x => x.Id == descontarFolioDelContenedor.IdInventario);
                        descontarFolioDeInventario.FormasDisponibles -= 1;
                        descontarFolioDeInventario.UltimoFolioUtilizado = Convert.ToString(folioEnInventarioEncontrado.NumFolio);
                        repositorioInventario.Modificar_Transaccionadamente(descontarFolioDeInventario);





                        Tbl_Pagos pagoModificado = repositorioTblPago.Modificar_Transaccionadamente(pagoAmodificar);

                        if (!string.IsNullOrEmpty(Convert.ToString(pagoModificado.FolioCheque)))
                        {
                            registrosInsertadosOActualizados_Foliacion++;

                        }


                    }
                    else
                    {
                        /***************************************************************************************************************************************************/
                        /***************************************************************************************************************************************************/
                        /*********************             SI ENTRA ES PORQUE AUN NO ESTA FOLIADO Y SE HARAN INSERTS A DBfOLIACION         *********************************************/

                        Tbl_Pagos nuevoPago = new Tbl_Pagos();

                        nuevoPago.Id_nom = datosNominaCompleto.Id_nom;
                        nuevoPago.Nomina = datosNominaCompleto.Nomina;
                        nuevoPago.An = datosNominaCompleto.An;
                        nuevoPago.Adicional = datosNominaCompleto.Adicional;
                        nuevoPago.Anio = datosNominaCompleto.Anio;
                        nuevoPago.Mes = datosNominaCompleto.Mes;
                        nuevoPago.Quincena = Convert.ToInt32(datosNominaCompleto.Quincena);
                        nuevoPago.ReferenciaBitacora = datosNominaCompleto.ReferenciaBitacora;
                        nuevoPago.Partida = nuevaPersona.Partida;
                        nuevoPago.Delegacion = nuevaPersona.Delegacion;
                        nuevoPago.RfcEmpleado = nuevaPersona.RFC;
                        nuevoPago.NumEmpleado = nuevaPersona.NumEmpleado;
                        nuevoPago.NombreEmpleado = nuevaPersona.Nombre;

                        if (datosNominaCompleto.EsPenA)
                        {
                            nuevoPago.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(nuevaPersona.CadenaNumEmpleado); //conectarse a funcion y buscar nombre 
                            nuevoPago.EsPenA = true;
                            nuevoPago.BeneficiarioPenA = nuevaPersona.Nombre;
                            nuevoPago.NumBeneficiario = nuevaPersona.NumBeneficiario;

                        }
                        else
                        {
                            nuevoPago.NombreEmpleado = nuevaPersona.Nombre;
                            nuevoPago.EsPenA = false;
                            nuevoPago.BeneficiarioPenA = null;
                            nuevoPago.NumBeneficiario = null;
                        }

                        nuevoPago.ImporteLiquido = nuevaPersona.Liquido;
                        nuevoPago.FolioCheque = nuevaPersona.NumChe;
                        nuevoPago.FolioCFDI = nuevaPersona.FolioCFDI;

                        nuevoPago.IdTbl_CuentaBancaria_BancoPagador = nuevaPersona.IdBancoPagador;
                        nuevoPago.IdCat_FormaPago_Pagos = 1; //1 = cheque , 2 = Pagomatico 

                        nuevoPago.IdCat_EstadoPago_Pagos = 1; //1 = Transito, 2= Pagado
                                                              //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado

                        string cadenaDeIntegridad = datosNominaCompleto.Id_nom + " || " + datosNominaCompleto.Nomina + " || " + datosNominaCompleto.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.BancoX + " || " + nuevaPersona.CuentaX + " || " + nuevaPersona.NumBeneficiario;
                        EncriptarCadena encriptar = new EncriptarCadena();
                        nuevoPago.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                        nuevoPago.Activo = true;

                        /*Guardar los cheques y descontarlos del inventario*/
                        FoliosAFoliarInventario foliodisponileEncontrado = chequesVerificadosFoliar.Where(x => x.Folio == nuevoPago.FolioCheque).FirstOrDefault();
                        Tbl_InventarioDetalle folioEnInventarioEncontrado = repositorioInventarioDetalle.Obtener(x => x.Id == foliodisponileEncontrado.Id);
                        folioEnInventarioEncontrado.IdIncidencia = 3; //3 porque ya fue foliado por primera vez
                        folioEnInventarioEncontrado.FechaIncidencia = DateTime.Now;
                        nuevoPago.IdTbl_InventarioDetalle = folioEnInventarioEncontrado.Id;
                        repositorioInventarioDetalle.Modificar_Transaccionadamente(folioEnInventarioEncontrado);


                        Tbl_InventarioContenedores descontarFolioDelContenedor = repositorioContenedores.Obtener(x => x.Id == folioEnInventarioEncontrado.IdContenedor);
                        descontarFolioDelContenedor.FormasFoliadas += 1;
                        descontarFolioDelContenedor.FormasDisponiblesActuales -= 1;
                        repositorioContenedores.Modificar_Transaccionadamente(descontarFolioDelContenedor);

                        Tbl_Inventario descontarFolioDeInventario = repositorioInventario.Obtener(x => x.Id == descontarFolioDelContenedor.IdInventario);
                        descontarFolioDeInventario.FormasDisponibles -= 1;
                        descontarFolioDeInventario.UltimoFolioUtilizado = Convert.ToString(folioEnInventarioEncontrado.NumFolio);
                        repositorioInventario.Modificar_Transaccionadamente(descontarFolioDeInventario);

                        Tbl_Pagos pagoInsertado = repositorioTblPago.Agregar_Transaccionadamente(nuevoPago);


                        if (!string.IsNullOrEmpty(Convert.ToString(pagoInsertado.FolioCheque)))
                        {
                            registrosInsertadosOActualizados_Foliacion++;

                        }

                    }

                }



                numeroRegistrosActualizados_AlPHA = await task_resultadoRegitrosActualizados_InterfacesAlPHA;


                numeroRegistrosActualizados_BaseDBF = Convert.ToInt32(resultado_ActualizacionDBF);


                ///Guarda Un lote de transacciones tanto para modificar o hacer inserts  
                if ((registrosInsertadosOActualizados_Foliacion + numeroRegistrosActualizados_AlPHA) == (numeroRegistrosActualizados_BaseDBF * 2))
                {


                    nuevaAlerta.IdAtencion = 0;
                    nuevaAlerta.NumeroNomina = datosNominaCompleto.Nomina;
                    nuevaAlerta.NombreNomina = datosNominaCompleto.Coment;
                    nuevaAlerta.Detalle = "";
                    nuevaAlerta.Solucion = "";
                    nuevaAlerta.Id_Nom = Convert.ToString(datosNominaCompleto.Id_nom);
                    nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados_AlPHA;
                    nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);


                    Advertencias.Add(nuevaAlerta);
                    transaccion.GuardarCambios();
                }
                else
                {

                    //Si entra en esta condicion es por que uno o mas registros no se foliaron y se necesita refoliar toda la nomina 

                    nuevaAlerta.IdAtencion = 1;
                    nuevaAlerta.NumeroNomina = datosNominaCompleto.Nomina;
                    nuevaAlerta.NombreNomina = datosNominaCompleto.Coment;
                    nuevaAlerta.Detalle = "OCURRIO UN ERROR EN LA FOLIACION";
                    nuevaAlerta.Solucion = "IFNN";
                    nuevaAlerta.Id_Nom = Convert.ToString(datosNominaCompleto.Id_nom);
                    nuevaAlerta.RegistrosFoliados = numeroRegistrosActualizados_AlPHA;
                    nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);

                    numeroRegistrosActualizados_AlPHA = 0;

                    Advertencias.Add(nuevaAlerta);
                    FoliarConsultasDBSinEntity.LimpiarBaseDelegacionNominaEnSql_Cheque(datosNominaCompleto, NuevaNominaFoliar.AnioInterfaz, delegacionSeleccionada, EsSindi);
                    transaccion.Dispose();
                }



            }


            return Advertencias;
        }

        public static async Task<List<AlertasAlFolearPagomaticosDTO>> FoliarChequesPorNomina_TIEMPO_DE_RESPUESTA_MEJORADO(FoliarFormasPagoDTO NuevaNominaFoliar, string Observa, List<FoliosAFoliarInventario> chequesVerificadosFoliar)
        {
            List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();
            AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();


            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            Tbl_CuentasBancarias bancoEncontrado = repositorio.Obtener(x => x.Id == NuevaNominaFoliar.IdBancoPagador && x.Activo == true);

            var filtroDelegaciones = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            //Se crea una transaccion para poder actualizar o insertar en la tbl_pagos de la DB foliacion
            var repositorioTblPago = new Repositorio<Tbl_Pagos>(transaccion);


            //sirve para modificar los datos del inventario
            var repositorioInventarioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            var repositorioContenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);



            DatosCompletosBitacoraDTO datosNominaCompleto = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(NuevaNominaFoliar.IdNomina, NuevaNominaFoliar.AnioInterfaz);
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosNominaCompleto.An, datosNominaCompleto.Anio);

            string delegacionSeleccionada = filtroDelegaciones.Obtener(x => x.GrupoImpresionDelegacion == NuevaNominaFoliar.IdDelegacion).DelegacionesIncluidas.ToString();

            bool EsSindi = false;
            string consultaLista = "";
            if (NuevaNominaFoliar.IdGrupoFoliacion == 0)
            {
                if (NuevaNominaFoliar.Confianza > 0 && NuevaNominaFoliar.Sindicato == 0)
                {
                    //son de confianza
                    consultaLista = consultasCheques.ObtenerConsultaOrdenDeFoliacionPorDelegacion_GeneralDescentralizada(datosNominaCompleto.An, datosNominaCompleto.Anio, delegacionSeleccionada, EsSindi, bancosContenidosEnAn);
                }
                else if (NuevaNominaFoliar.Confianza == 0 && NuevaNominaFoliar.Sindicato > 0)
                {
                    //Son sindicalizados
                    EsSindi = true;
                    consultaLista = consultasCheques.ObtenerConsultaOrdenDeFoliacionPorDelegacion_GeneralDescentralizada(datosNominaCompleto.An, datosNominaCompleto.Anio, delegacionSeleccionada, EsSindi, bancosContenidosEnAn);
                }
            }
            else if (NuevaNominaFoliar.IdGrupoFoliacion == 1)
            {
                //consultaLista = ConsultasSQLOtrasNominasConCheques.ObtenerCadenaConsultaReportePDF_GenericOtrasNominas_PartidaCompleta(datosNominaCompleto.An, NuevaNominaFoliar.AnioInterfaz, delegacionSeleccionada, datosNominaCompleto.EsPenA);
                consultaLista = consultasCheques.ObtenerConsultaOrdenDeFoliacionPorDelegacion_OtrasNominasYPenA(datosNominaCompleto.An, datosNominaCompleto.Anio, delegacionSeleccionada, datosNominaCompleto.EsPenA, bancosContenidosEnAn);
            }

            List<ResumenPersonalAFoliarDTO> resumenPersonalFoliar = new List<ResumenPersonalAFoliarDTO>();
            resumenPersonalFoliar = FoliarConsultasDBSinEntity.ObtenerResumenDatosFormasDePagoFoliar(datosNominaCompleto.EsPenA, Observa, consultaLista, bancoEncontrado, NuevaNominaFoliar.RangoInicial, NuevaNominaFoliar.Inhabilitado, Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoInicial), Convert.ToInt32(NuevaNominaFoliar.RangoInhabilitadoFinal));

            /******************************************************************************/
            //Foliar en DBF

            int numeroRegistrosActualizados_BaseDBF = 0;
            int numeroRegistrosActualizados_AlPHA = 0;
            int registrosInsertadosOActualizados_Foliacion = 0;

            /********************************************************************************************************************************************************************/
            /***********************             Permite el acceso a una carpeta que se encuentra compartida dentro del servidor            ************************************/
            /********************************************************************************************************************************************************************/
            AlertasAlFolearPagomaticosDTO alertaEncontrada = FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(1 /* Se redireccionara a foliar una DBF */ , datosNominaCompleto , resumenPersonalFoliar , "" /* no lleva una condicion ya que el update se hace con un filtro especifico*/);
            if (alertaEncontrada.IdAtencion != 200)
            {
                Advertencias.Add(alertaEncontrada);
                return Advertencias;
            }



            /*****************************************************************************************************************************************************************/
            /**********************************************         Actualiza la base cargada en SQL            **************************************************************/
            /*****************************************************************************************************************************************************************/
            //HILO DE EJECUCION SECUNDARIO
            Task<int> task_resultadoRegitrosActualizados_InterfacesAlPHA = Task.Run(() =>
            {
               return FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql_transaccionado_Cheque(resumenPersonalFoliar, datosNominaCompleto, NuevaNominaFoliar.AnioInterfaz);
            });



            /*******************************************************************************************************************************************************************************/
            /**********************************************     Actualiza o Inserta resgistros     *****************************************************************************************/
            /*******************************************************************************************************************************************************************************/
            //INSERTAR PAGOS
            List<Tbl_Pagos> pagosNuevosAinsertar = new List<Tbl_Pagos>();
            //ACTUALIZAR PAGOS
            List<ActualizarFoliacionPagomaticoTblPagoDTO> ActualizarPagosEnTblPagos = new List<ActualizarFoliacionPagomaticoTblPagoDTO>();

            EncriptarCadena encriptar = new EncriptarCadena();
            bool actualizar = false;
            foreach (ResumenPersonalAFoliarDTO nuevaPersona in resumenPersonalFoliar)
            {

               Tbl_Pagos pagoAmodificar = null;

               if (datosNominaCompleto.EsPenA)
               {
                   pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosNominaCompleto.Anio && x.Id_nom == datosNominaCompleto.Id_nom && x.IdCat_FormaPago_Pagos == 1 /*Por ser cheque*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido && x.NumBeneficiario == nuevaPersona.NumBeneficiario);
               }
               else
               {
                   pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == datosNominaCompleto.Anio && x.Id_nom == datosNominaCompleto.Id_nom && x.IdCat_FormaPago_Pagos == 1 /*Por ser cheuqe*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido);
               }

                    //Si pagoEncontrado no es null es por que ya fue foliada al menos una vez ya que existe el registro y no es necesario hacer un insert solo un Update
                    ///Insertar un regitro o actualizar en la DB foliacion segun se el caso 
               if (pagoAmodificar != null)
               {
                   actualizar = true;
                    /*******************************************************************************************************************************************************************************/
                    /*********************                         SI ENTRA ES PORQUE YA FUE FOLIADA Y SOLO SE HARA UN UPDATE                 *******************************************************/
                    /*******************************************************************************************************************************************************************************/
                    ActualizarFoliacionPagomaticoTblPagoDTO nuevaActualizacion = new ActualizarFoliacionPagomaticoTblPagoDTO();
                    nuevaActualizacion.IdPago = pagoAmodificar.Id;
                    nuevaActualizacion.IdTbl_CuentaBancaria_BancoPagador = nuevaPersona.IdBancoPagador;
                    nuevaActualizacion.FolioCheque = nuevaPersona.NumChe;
                    string cadenaDeIntegridad = datosNominaCompleto.Id_nom + " || " + datosNominaCompleto.Nomina + " || " + datosNominaCompleto.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.BancoX + " || " + nuevaPersona.CuentaX + " || " + nuevaPersona.NumBeneficiario;
                    nuevaActualizacion.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                    ActualizarPagosEnTblPagos.Add(nuevaActualizacion);


                    /********************************************************************************************************************************************************************************/
                    /***************************************                  Guardar los cheques y descontarlos del inventario              ********************************************************/
                    /********************************************************************************************************************************************************************************/

                    FoliosAFoliarInventario foliodisponileEncontrado = chequesVerificadosFoliar.Where(x => x.Folio == nuevaActualizacion.FolioCheque).FirstOrDefault();
                    Tbl_InventarioDetalle folioEnInventarioEncontrado = repositorioInventarioDetalle.Obtener(x => x.Id == foliodisponileEncontrado.Id);
                    folioEnInventarioEncontrado.IdIncidencia = 3; //3 porque ya fue foliado por primera vez
                    folioEnInventarioEncontrado.FechaIncidencia = DateTime.Now;
                    pagoAmodificar.IdTbl_InventarioDetalle = folioEnInventarioEncontrado.Id;
                    repositorioInventarioDetalle.Modificar_Transaccionadamente(folioEnInventarioEncontrado);


                    Tbl_InventarioContenedores descontarFolioDelContenedor = repositorioContenedores.Obtener(x => x.Id == folioEnInventarioEncontrado.IdContenedor);
                    descontarFolioDelContenedor.FormasFoliadas += 1;
                    descontarFolioDelContenedor.FormasDisponiblesActuales -= 1;
                    repositorioContenedores.Modificar_Transaccionadamente(descontarFolioDelContenedor);

                    Tbl_Inventario descontarFolioDeInventario = repositorioInventario.Obtener(x => x.Id == descontarFolioDelContenedor.IdInventario);
                    descontarFolioDeInventario.FormasDisponibles -= 1;
                    descontarFolioDeInventario.UltimoFolioUtilizado = Convert.ToString(folioEnInventarioEncontrado.NumFolio);
                    repositorioInventario.Modificar_Transaccionadamente(descontarFolioDeInventario);

                    registrosInsertadosOActualizados_Foliacion++;
               }
               else
               {
                    /********************************************************************************************************************************************************************************/
                    /*********************                      SI ENTRA ES PORQUE AUN NO ESTA FOLIADO Y SE HARAN INSERTS A DBfOLIACION                 *********************************************/
                    /********************************************************************************************************************************************************************************/

                    Tbl_Pagos nuevoPago = new Tbl_Pagos();
                    nuevoPago.Id_nom = datosNominaCompleto.Id_nom;
                    nuevoPago.Nomina = datosNominaCompleto.Nomina;
                    nuevoPago.An = datosNominaCompleto.An;
                    nuevoPago.Adicional = datosNominaCompleto.Adicional;
                    nuevoPago.Anio = datosNominaCompleto.Anio;
                    nuevoPago.Mes = datosNominaCompleto.Mes;
                    nuevoPago.Quincena = Convert.ToInt32(datosNominaCompleto.Quincena);
                    nuevoPago.ReferenciaBitacora = datosNominaCompleto.ReferenciaBitacora;
                    nuevoPago.Partida = nuevaPersona.Partida;
                    nuevoPago.Delegacion = nuevaPersona.Delegacion;
                    nuevoPago.RfcEmpleado = nuevaPersona.RFC;
                    nuevoPago.NumEmpleado = nuevaPersona.NumEmpleado;
                    nuevoPago.NombreEmpleado = nuevaPersona.Nombre;

                    if (datosNominaCompleto.EsPenA)
                    {
                        nuevoPago.NombreEmpleado = FoliarConsultasDBSinEntity.ObtenerNombreEmpleadoSegunAlpha(nuevaPersona.CadenaNumEmpleado); //conectarse a funcion y buscar nombre 
                        nuevoPago.EsPenA = true;
                        nuevoPago.BeneficiarioPenA = nuevaPersona.Nombre;
                        nuevoPago.NumBeneficiario = nuevaPersona.NumBeneficiario;
                    }
                    else
                    {
                        nuevoPago.NombreEmpleado = nuevaPersona.Nombre;
                        nuevoPago.EsPenA = false;
                        nuevoPago.BeneficiarioPenA = null;
                        nuevoPago.NumBeneficiario = null;
                    }

                        nuevoPago.ImporteLiquido = nuevaPersona.Liquido;
                        nuevoPago.FolioCheque = nuevaPersona.NumChe;
                        nuevoPago.FolioCFDI = nuevaPersona.FolioCFDI;

                        nuevoPago.IdTbl_CuentaBancaria_BancoPagador = nuevaPersona.IdBancoPagador;
                        nuevoPago.IdCat_FormaPago_Pagos = 1; //1 = cheque , 2 = Pagomatico 

                        nuevoPago.IdCat_EstadoPago_Pagos = 1; //1 = Transito, 2= Pagado
                                                              //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado

                        string cadenaDeIntegridad = datosNominaCompleto.Id_nom + " || " + datosNominaCompleto.Nomina + " || " + datosNominaCompleto.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.BancoX + " || " + nuevaPersona.CuentaX + " || " + nuevaPersona.NumBeneficiario;
                        nuevoPago.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                        nuevoPago.Activo = true;


                        pagosNuevosAinsertar.Add(nuevoPago);


                    /********************************************************************************************************************************************************************************/
                    /***************************************                  Guardar los cheques y descontarlos del inventario              ********************************************************/
                    /********************************************************************************************************************************************************************************/
                    
                    FoliosAFoliarInventario foliodisponileEncontrado = chequesVerificadosFoliar.Where(x => x.Folio == nuevoPago.FolioCheque).FirstOrDefault();
                    Tbl_InventarioDetalle folioEnInventarioEncontrado = repositorioInventarioDetalle.Obtener(x => x.Id == foliodisponileEncontrado.Id);
                    folioEnInventarioEncontrado.IdIncidencia = 3; //3 porque ya fue foliado por primera vez
                    folioEnInventarioEncontrado.FechaIncidencia = DateTime.Now;
                    nuevoPago.IdTbl_InventarioDetalle = folioEnInventarioEncontrado.Id;
                    repositorioInventarioDetalle.Modificar_Transaccionadamente(folioEnInventarioEncontrado);


                    Tbl_InventarioContenedores descontarFolioDelContenedor = repositorioContenedores.Obtener(x => x.Id == folioEnInventarioEncontrado.IdContenedor);
                    descontarFolioDelContenedor.FormasFoliadas += 1;
                    descontarFolioDelContenedor.FormasDisponiblesActuales -= 1;
                    repositorioContenedores.Modificar_Transaccionadamente(descontarFolioDelContenedor);

                    Tbl_Inventario descontarFolioDeInventario = repositorioInventario.Obtener(x => x.Id == descontarFolioDelContenedor.IdInventario);
                    descontarFolioDeInventario.FormasDisponibles -= 1;
                    descontarFolioDeInventario.UltimoFolioUtilizado = Convert.ToString(folioEnInventarioEncontrado.NumFolio);
                    repositorioInventario.Modificar_Transaccionadamente(descontarFolioDeInventario);

                    registrosInsertadosOActualizados_Foliacion++;


                }

            }




            /*****************************************************************************************************************************************************************************************/
            /**********************************************            Total de registros Actualizados e Insertados          *************************************************************************/
            /*****************************************************************************************************************************************************************************************/
            int resultadoActualizacionOInsercionSegunElCaso = 0;
            if (actualizar)
            {
                resultadoActualizacionOInsercionSegunElCaso = FoliarConsultasDBSinEntity.ActualizarTblPagos_DespuesDeFoliacion(ActualizarPagosEnTblPagos);
            }
            else
            {
                resultadoActualizacionOInsercionSegunElCaso = repositorioTblPago.Agregar_EntidadesMasivamente(pagosNuevosAinsertar);
            }


            numeroRegistrosActualizados_AlPHA = await task_resultadoRegitrosActualizados_InterfacesAlPHA;

            //Registros Actualizados en DBF
            if (alertaEncontrada.IdAtencion == 200)
                numeroRegistrosActualizados_BaseDBF = alertaEncontrada.NumeroRegistrosActualizados;



            /*****************************************************************************************************************************************************************************************/
            /**********************************************                 Validacion del estandart de Foliacion                *********************************************************************/
            /*****************************************************************************************************************************************************************************************/
            ///Guarda Un lote de transacciones tanto para modificar o hacer inserts  
            ///
            //resultadoActualizacionOInsercionSegunElCaso -= 1; /// Probar que entre en el else del metodo de abajo y funcione correctamente
            int registrosFoliados = resultadoActualizacionOInsercionSegunElCaso + numeroRegistrosActualizados_AlPHA + numeroRegistrosActualizados_BaseDBF;
            if ((resultadoActualizacionOInsercionSegunElCaso + numeroRegistrosActualizados_AlPHA) == (numeroRegistrosActualizados_BaseDBF * 2))
            {


               nuevaAlerta.IdAtencion = 0;
               nuevaAlerta.NumeroNomina = datosNominaCompleto.Nomina;
               nuevaAlerta.NombreNomina = datosNominaCompleto.Coment;
               nuevaAlerta.Detalle = "";
               nuevaAlerta.Solucion = "";
               nuevaAlerta.Id_Nom = Convert.ToString(datosNominaCompleto.Id_nom);
               nuevaAlerta.RegistrosFoliados = registrosFoliados /3;
               nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);

               Advertencias.Add(nuevaAlerta);
               transaccion.GuardarCambios();
            }
            else
            {
                //Si entra en esta condicion es por que uno o mas registros no se foliaron y se necesita refoliar toda la nomina 
                transaccion.Dispose();

                DatosCompletosBitacoraDTO datosCompletosNominaNuevo = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraPorIdNom(datosNominaCompleto.Id_nom, datosNominaCompleto.Anio);

                string condicionParaSerChequePorDelegacion = "";
                if (NuevaNominaFoliar.IdGrupoFoliacion == 0)
                {
                    //APlica para GeneralYDESCE y en el campo EsSindise sabe si son de confianza = false o sindicalizados = true 
                    condicionParaSerChequePorDelegacion = consultasCheques.ObtenerConsultaLimpiarRegistrosPorDelegacion_GeneralDescentralizada(datosCompletosNominaNuevo.An, datosCompletosNominaNuevo.Anio, delegacionSeleccionada, EsSindi, bancosContenidosEnAn);
                }
                else if (NuevaNominaFoliar.IdGrupoFoliacion == 1)
                {
                    condicionParaSerChequePorDelegacion = consultasCheques.ObtenerConsultaLimpiarRegistrosPorDelegacion_OtrasNominasYPenA(datosCompletosNominaNuevo.An, datosCompletosNominaNuevo.Anio, delegacionSeleccionada, datosCompletosNominaNuevo.EsPenA, bancosContenidosEnAn);
                }

                int registrosLimpiadosAN = FoliarConsultasDBSinEntity.LimpiarANSql_IncumplimientoCalidadFoliacion(datosCompletosNominaNuevo.Anio, datosCompletosNominaNuevo.An, condicionParaSerChequePorDelegacion, datosCompletosNominaNuevo.Id_nom);
                int registrosLimpiadosTblPagos = FoliarConsultasDBSinEntity.LimpiarTblPagos_IncumplimientoCalidadFoliacion(datosCompletosNominaNuevo.Anio, Convert.ToInt32(datosCompletosNominaNuevo.Quincena), datosCompletosNominaNuevo.Id_nom, 1 /* Por ser tipo de pago cheque*/);

                List<ResumenPersonalAFoliarDTO> resumenPersonalVacio = new List<ResumenPersonalAFoliarDTO>();                
                AlertasAlFolearPagomaticosDTO alertaEncontradaLimpiezaDBF = FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(6 /* Numero para redireccionar a una limpiaza de la base con una condicion */ , datosCompletosNominaNuevo, resumenPersonalVacio /*No se necesita esta dato*/, condicionParaSerChequePorDelegacion);
                if (alertaEncontradaLimpiezaDBF.IdAtencion  != 200)
                {
                    Advertencias.Add(alertaEncontradaLimpiezaDBF);
                    return Advertencias;
                }





                nuevaAlerta.IdAtencion = 4;
                nuevaAlerta.NumeroNomina = datosNominaCompleto.Nomina;
                nuevaAlerta.NombreNomina = datosNominaCompleto.Coment;
                nuevaAlerta.Detalle = registrosLimpiadosAN == registrosLimpiadosTblPagos ? "OCURRIO UN ERROR EN LA FOLIACION, NO SE CUMPLIO CON EL ESTANDAR DE FOLIACION " : "ERROR EN LA FOLIACION, NO SE PUDIERON LIMPIERAR LOS CAMPOS AFECTADOS CORRECTAMENTE EN TBL_PAGOS";
                nuevaAlerta.Solucion = "VUELVA A FOLIAR DE NUEVO";
                nuevaAlerta.Id_Nom = Convert.ToString(datosNominaCompleto.Id_nom);
                nuevaAlerta.RegistrosFoliados = registrosFoliados / 3;
                nuevaAlerta.UltimoFolioUsado = resumenPersonalFoliar.Max(x => x.NumChe);
               
                Advertencias.Add(nuevaAlerta);
                return Advertencias;


            }



            return Advertencias;
        }











        #region METODOS PARA FORMAS DE PAGO => CHEQUES 
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /********************************************************    METODOS PARA FORMAS DE PAGO => CHEQUES      ***************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/


        //**********************************************************  DATOS PERSONALES DE UNA NOMINA POR DELEGACION O NOMINA PARA EL PDF DEL REPORTE        ***************************************//
        public static List<ResumenRevicionNominaPDFDTO> ObtenerDatosPersonalesDelegacionNominaGeneralDesce_ReporteCheque(int IdNomina, int AnioInterface, bool EsSindicalizado, int IdDelegacion)
        {

            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            IQueryable<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos();


            DatosCompletosBitacoraDTO datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(IdNomina, AnioInterface);
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, AnioInterface);
            string delegacionSeleccionada = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == IdDelegacion).FirstOrDefault().DelegacionesIncluidas;
            string consulta = consultasCheques.ObtenerConsultaDetallePersonalEnAN_GeneralDescentralizada(datosCompletosNomina.An, AnioInterface, delegacionSeleccionada, EsSindicalizado, bancosContenidosEnAn);
            //string consulta = ConsultasSQLSindicatoGeneralYDesc.ObtenerCadenaConsultaReportePDF_XSindicato(datosCompletosNomina.An, AnioInterface, delegacionesIncluidas, EsSindicalizado);

            return FoliarConsultasDBSinEntity.ObtenerResumenDatosComoSeEncuentraFolidoEnAN(consulta, datosCompletosNomina.Nomina);
        }


        public static List<ResumenRevicionNominaPDFDTO> ObtenerDatosPersonalesDelegacionOtrasNominas_ReporteCheque(int IdNomina, int AnioInterface, int IdDelegacion)
        {

            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            IQueryable<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos();


            DatosCompletosBitacoraDTO datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(IdNomina, AnioInterface);
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, AnioInterface);
            string delegacionSeleccionada = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == IdDelegacion).FirstOrDefault().DelegacionesIncluidas;
            string consulta = consultasCheques.ObtenerConsultaDetallePersonalEnAN_OtrasNominasYPenA(datosCompletosNomina.An, AnioInterface, delegacionSeleccionada, datosCompletosNomina.EsPenA, bancosContenidosEnAn);
            //string consulta = ConsultasSQLOtrasNominasConCheques.ObtenerCadenaConsultaReportePDF_GenericOtrasNominas(datosCompletosNomina.An, AnioInterface, delegacionesIncluidas, datosCompletosNomina.EsPenA);


            return FoliarConsultasDBSinEntity.ObtenerResumenDatosComoSeEncuentraFolidoEnAN(consulta, datosCompletosNomina.Nomina);
        }


        // sirve para revisar la nominas la primera vez en antes de foliarla : comentario escrito el 01/07/2022
        public static List<ResumenRevicionNominaPDFDTO> ObtenerDetallePersonalRevicionPDF_GeneralDescentralizada(int IdNomina, int AnioInterface)
        {
            List<ResumenRevicionNominaPDFDTO> resumenOrdenadoNominaXGrupoImpresion = new List<ResumenRevicionNominaPDFDTO>();

            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            IQueryable<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos();


            DatosCompletosBitacoraDTO datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(IdNomina, AnioInterface);

            List<string> delegacionesIncluidas = filtrodelegaciones.Select(x => x.DelegacionesIncluidas).ToList();

            int Iterador = 0;
            List<bool> EsSindicato = new List<bool>() { false, true };

            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, AnioInterface);
            foreach (bool EsSindi in EsSindicato)
            {

                foreach (string delegacion in delegacionesIncluidas)
                {
                    string consulta = consultasCheques.ObtenerConsultaDetallePersonalEnAN_GeneralDescentralizada(datosCompletosNomina.An, AnioInterface, delegacion, EsSindi, bancosContenidosEnAn);
                    //string consulta = ConsultasSQLSindicatoGeneralYDesc.ObtenerCadenaConsultaReportePDF_XSindicato(datosCompletosNomina.An, AnioInterface, delegacion, EsSindi);

                    List<ResumenRevicionNominaPDFDTO> resumenDelegacion = FoliarConsultasDBSinEntity.ObtenerResumenDatosComoSeEncuentraFolidoEnAN(consulta, datosCompletosNomina.Nomina);

                    foreach (ResumenRevicionNominaPDFDTO nuevoResumen in resumenDelegacion)
                    {
                        nuevoResumen.Contador = Convert.ToString(++Iterador);
                        resumenOrdenadoNominaXGrupoImpresion.Add(nuevoResumen);
                    }
                }
            }

            return resumenOrdenadoNominaXGrupoImpresion;
        }

        public static List<ResumenRevicionNominaPDFDTO> ObtenerDetallePersonalRevicionPDF_OtrasNominasYPenA(int IdNomina, int AnioInterface)
        {
            List<ResumenRevicionNominaPDFDTO> resumenOrdenadoNominaXGrupoImpresion = new List<ResumenRevicionNominaPDFDTO>();

            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            IQueryable<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos();


            DatosCompletosBitacoraDTO datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(IdNomina, AnioInterface);

            List<string> delegacionesIncluidas = filtrodelegaciones.Select(x => x.DelegacionesIncluidas).ToList();
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, AnioInterface);
            int Iterador = 0;
            foreach (string delegacion in delegacionesIncluidas)
            {
                //string consulta = ConsultasSQLOtrasNominasConCheques.ObtenerCadenaConsultaReportePDF_GenericOtrasNominas(datosCompletosNomina.An, AnioInterface, delegacion, datosCompletosNomina.EsPenA);

                string consulta = consultasCheques.ObtenerConsultaDetallePersonalEnAN_OtrasNominasYPenA(datosCompletosNomina.An, AnioInterface, delegacion, datosCompletosNomina.EsPenA, bancosContenidosEnAn);  /*  ObtenerCadenaConsultaReportePDF_GenericOtrasNominas(datosCompletosNomina.An, AnioInterface, delegacion, datosCompletosNomina.EsPenA);*/

                List<ResumenRevicionNominaPDFDTO> resumenDelegacion = FoliarConsultasDBSinEntity.ObtenerResumenDatosComoSeEncuentraFolidoEnAN(consulta, datosCompletosNomina.Nomina);

                foreach (ResumenRevicionNominaPDFDTO nuevoResumen in resumenDelegacion)
                {
                    nuevoResumen.Contador = Convert.ToString(++Iterador);
                    resumenOrdenadoNominaXGrupoImpresion.Add(nuevoResumen);
                }
            }

            return resumenOrdenadoNominaXGrupoImpresion;
        }




        //************************************************************************************************************************************************************************//
        //**********    VERIFICA QUE EL ESTATUS DE LA NOMINA SELECCIONADA DE COMO SE ENCUANTRA EL AN SI ESTAN LLENOS SUS CAMPOS DE FOLIACION DE ACUERDO A LAS DELEGACIONES   *****//
        //************************************************************************************************************************************************************************//
        public static List<ResumenNominaChequeDTO> ObtenerResumenDetalleNomina_Cheques(int IdNomina, int AnioInterface)
        {
            List<ResumenNominaDTO> listaResumenNomina = new List<ResumenNominaDTO>();


            DatosCompletosBitacoraDTO datosCompletosNomina = ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(IdNomina, AnioInterface);
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            IQueryable<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos();
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, AnioInterface);

            bool esNominaGeneraloDescentralizada = false;
            if (datosCompletosNomina.Nomina == "01" || datosCompletosNomina.Nomina == "02")
            {
                esNominaGeneraloDescentralizada = true;
                List<string> listaFiltroConsultaTotalesConfianzaSindicato = consultasCheques.ObtenerConsultasTotalPersonal_ConfianzaSindicaliza(datosCompletosNomina.An, AnioInterface, bancosContenidosEnAn);
                List<TotalRegistrosXDelegacionDTO> registrosTotalesXDelegacion = FoliarConsultasDBSinEntity.ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina(listaFiltroConsultaTotalesConfianzaSindicato, esNominaGeneraloDescentralizada);

                List<ResumenNominaChequeDTO> resumenNominaChequeGeneralDesce = new List<ResumenNominaChequeDTO>();
                int iterador = 0;
                foreach (TotalRegistrosXDelegacionDTO nuevaDelegacion in registrosTotalesXDelegacion)
                {
                    ResumenNominaChequeDTO nuevoResumen = new ResumenNominaChequeDTO();
                    nuevoResumen.IdVirtual = ++iterador;
                    nuevoResumen.IdDelegacion = Convert.ToInt32(nuevaDelegacion.Delegacion);
                    nuevoResumen.GrupoFoliacion = 0; // El grupo de foliacion {0} es para los de la general y los decentralizados 
                    nuevoResumen.Coment = datosCompletosNomina.Coment;
                    nuevoResumen.IdNomina = datosCompletosNomina.Id_nom;
                    /*OBTENER DELEGACION*/
                    nuevoResumen.NombreDelegacion = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == nuevoResumen.IdDelegacion).FirstOrDefault().NombreComun;

                    string delegacionEncontrada = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == nuevoResumen.IdDelegacion).FirstOrDefault().DelegacionesIncluidas;

                    if (Convert.ToBoolean(nuevaDelegacion.Sindicato))
                    {
                        nuevoResumen.Sindicato = nuevaDelegacion.Total;
                    }
                    else
                    {
                        nuevoResumen.Confianza = nuevaDelegacion.Total;
                    }

                    string consultaTotalRegistrosXDelegacion = consultasCheques.ObtenerConsultaTotalRegistrosNoFoliadosxDelegacion_GeneralDescentralizada(datosCompletosNomina.An, AnioInterface, delegacionEncontrada, nuevaDelegacion.Sindicato, bancosContenidosEnAn);
                    nuevoResumen.EstaFoliadoCorrectamente = FoliarConsultasDBSinEntity.EstaFoliadacorrectamenteDelegacion_Cheque(consultaTotalRegistrosXDelegacion, nuevaDelegacion.Total);

                    resumenNominaChequeGeneralDesce.Add(nuevoResumen);
                }

                return resumenNominaChequeGeneralDesce;

            }
            else if (datosCompletosNomina.Nomina == "08")
            {
                esNominaGeneraloDescentralizada = false;
                //List<string> listaFiltroConsultaTotalesPensionAlimenticia = ConsultasSQLOtrasNominasConCheques.ObtenerConsultas_TotalesXPencionAlimenticia(datosCompletosNomina.An, AnioInterface);
                List<string> listaFiltroConsultaTotalesPensionAlimenticia = consultasCheques.ObtenerConsultasTotalPersonal_OtrasNominasYPenA(datosCompletosNomina.An, AnioInterface, bancosContenidosEnAn);
                List<TotalRegistrosXDelegacionDTO> registrosTotalesXDelegacionPenA = FoliarConsultasDBSinEntity.ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina(listaFiltroConsultaTotalesPensionAlimenticia, esNominaGeneraloDescentralizada);

                List<ResumenNominaChequeDTO> resumenNominaChequePenA = new List<ResumenNominaChequeDTO>();
                int iterador = 0;
                foreach (TotalRegistrosXDelegacionDTO nuevaDelegacionPenA in registrosTotalesXDelegacionPenA)
                {
                    ResumenNominaChequeDTO nuevoResumen = new ResumenNominaChequeDTO();
                    nuevoResumen.IdVirtual = ++iterador;
                    nuevoResumen.IdDelegacion = Convert.ToInt32(nuevaDelegacionPenA.Delegacion);
                    nuevoResumen.GrupoFoliacion = 1; // El grupo de foliacion {1} es para todas las demas nominas que no sean General y Descentralizados 
                    nuevoResumen.Coment = datosCompletosNomina.Coment;
                    nuevoResumen.IdNomina = datosCompletosNomina.Id_nom;
                    /*OBTENER DELEGACION*/
                    nuevoResumen.NombreDelegacion = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == nuevoResumen.IdDelegacion).FirstOrDefault().NombreComun;

                    nuevoResumen.Otros = nuevaDelegacionPenA.Total;

                    string delegacionesIncluidas = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == nuevoResumen.IdDelegacion).FirstOrDefault().DelegacionesIncluidas;
                    string consultaObtenida = consultasCheques.ObtenerConsultaTotalRegistrosNoFoliadosxDelegacion_OtrasNominasYPenA(datosCompletosNomina.An, AnioInterface, delegacionesIncluidas, bancosContenidosEnAn);
                    //string consultaObtenida = ConsultasSQLOtrasNominasConCheques.ObtenerCadenaConsulta_GenericaOtrasNomina(datosCompletosNomina.An, AnioInterface, delegacionesIncluidas);
                    nuevoResumen.EstaFoliadoCorrectamente = FoliarConsultasDBSinEntity.EstaFoliadacorrectamenteDelegacion_Cheque(consultaObtenida, nuevaDelegacionPenA.Total);

                    resumenNominaChequePenA.Add(nuevoResumen);
                }

                return resumenNominaChequePenA;
            }
            else
            {
                List<string> listaFiltroConsultaTotalesOtrasNominas = consultasCheques.ObtenerConsultasTotalPersonal_OtrasNominasYPenA(datosCompletosNomina.An, AnioInterface, bancosContenidosEnAn);
                List<TotalRegistrosXDelegacionDTO> registrosTotalesXDelegacionOtrasNominas = FoliarConsultasDBSinEntity.ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina(listaFiltroConsultaTotalesOtrasNominas, false);

                List<ResumenNominaChequeDTO> resumenNominaChequeOtrasNominas = new List<ResumenNominaChequeDTO>();
                int iterador = 0;
                foreach (TotalRegistrosXDelegacionDTO nuevaDelegacionRestoDeNominas in registrosTotalesXDelegacionOtrasNominas)
                {
                    ResumenNominaChequeDTO nuevoResumen = new ResumenNominaChequeDTO();
                    nuevoResumen.IdVirtual = ++iterador;
                    nuevoResumen.IdDelegacion = Convert.ToInt32(nuevaDelegacionRestoDeNominas.Delegacion);
                    nuevoResumen.GrupoFoliacion = 1; // El grupo de foliacion {1} es para todas las demas nominas que no sean General y Descentralizados 
                    nuevoResumen.Coment = datosCompletosNomina.Coment;
                    nuevoResumen.IdNomina = datosCompletosNomina.Id_nom;
                    /*OBTENER DELEGACION*/
                    nuevoResumen.NombreDelegacion = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == nuevoResumen.IdDelegacion).FirstOrDefault().NombreComun;

                    nuevoResumen.Otros = nuevaDelegacionRestoDeNominas.Total;
                    string delegacionesIncluidas = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == nuevoResumen.IdDelegacion).FirstOrDefault().DelegacionesIncluidas;
                    string consultaObtenida = consultasCheques.ObtenerConsultaTotalRegistrosNoFoliadosxDelegacion_OtrasNominasYPenA(datosCompletosNomina.An, AnioInterface, delegacionesIncluidas, bancosContenidosEnAn);
                    //string consultaObtenida = ConsultasSQLOtrasNominasConCheques.ObtenerCadenaConsulta_GenericaOtrasNomina(datosCompletosNomina.An, AnioInterface, delegacionesIncluidas);
                    nuevoResumen.EstaFoliadoCorrectamente = FoliarConsultasDBSinEntity.EstaFoliadacorrectamenteDelegacion_Cheque(consultaObtenida, nuevaDelegacionRestoDeNominas.Total);

                    resumenNominaChequeOtrasNominas.Add(nuevoResumen);
                }

                return resumenNominaChequeOtrasNominas;
            }


        }



        //*************************************************************************************************************************************************************************//
        //*************************       RECUPERAR CHEQUES QUE POR ERROR HUMANO NO TENDRIAN QUE TENER NINGUNA INCIDENCIA        **************************************************//
        //*************************************************************************************************************************************************************************//
        public static List<FoliosARecuperarDTO> BuscarFormasPagoCoincidentes(int IdCuentaBancaria, int RangoInicial, int RangoFinal)
        {
            var transaccion = new Transaccion();
            var repoBanco = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            string nombreDb = ObtenerConexionesDB.ObtenerNombreDBValidacionFoliosDeploy();

            /*Paso 1 : Seleccionar un banco para saber su idDelInventario */
            Tbl_CuentasBancarias cuentaEncontrada = repoBanco.Obtener(x => x.Id == IdCuentaBancaria);

            string consultaFinal = "";
            string cuentaBancaria = "";


            if (cuentaEncontrada != null)
            {
                cuentaBancaria = cuentaEncontrada.NombreBanco + " - " + cuentaEncontrada.Cuenta;
                int idInventario = Convert.ToInt32(cuentaEncontrada.IdInventario);

                /*Paso 1 : Obtener los ids de los detalles de los cheques que no esten nullos y coincidan con el rango de folio buscados	*/
                string consulta2 = "SELECT id FROM [FCCBNetDB].[dbo].[Tbl_InventarioDetalle] where IdInventario = " + cuentaEncontrada.IdInventario + "  and IdIncidencia is not null and NumFolio >= " + RangoInicial + "  and NumFolio <= " + RangoFinal + " ";

                /*Paso 2 : Obtener los registros donde coincidan los beneficiarios de los cheques para mostrarselos al usuario */
                consultaFinal = "select id ,  Anio, Id_nom, Nomina, Quincena , Delegacion, CASE EsPenA When 0 Then NombreEmpleado When 1 Then BeneficiarioPenA end 'NombreBeneficiarioCheque' , NumEmpleado , ImporteLiquido , FolioCheque , IdTbl_CuentaBancaria_BancoPagador   FROM " + nombreDb + ".dbo.Tbl_Pagos where IdTbl_CuentaBancaria_BancoPagador = " + cuentaEncontrada.Id + "  and IdTbl_InventarioDetalle in ( " + consulta2 + " )  order by FolioCheque";
            }

            return FoliarConsultasDBSinEntity.ObtenerRegistrosChequesConIncidenciaPorError(consultaFinal, cuentaBancaria);
        }



        //************************************************************************************************************************************************************************//
        //**********              VERIFICA QUE NO SE SALTEN MAS DE 4 FOLIOS CONSECUTIVOS SIN INCIDENCIAS PARA PODER INICIAR A FOLIAR LOS CAMPOS CON CHEQUE                   *****//
        //************************************************************************************************************************************************************************//
        public static bool VerificarFoliosConsecutivos(int IdBanco, int FInicial)
        {
            bool bandera = false;
            int verificarFolioApartir = FInicial - 4;

            var transaccion = new Transaccion();
            var repositorioTblBanco = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            Tbl_CuentasBancarias bancoEncontrado = repositorioTblBanco.Obtener(x => x.Id == IdBanco && x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true);


            //obtiene los numero de folios como deben de encontrarse en la tabla inventarios
            List<int> foliosEnordenRecuperados = new List<int>();

            for (int i = verificarFolioApartir; i < FInicial; i++)
            {
                foliosEnordenRecuperados.Add(i);
            }





            var repositorioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            IQueryable<Tbl_InventarioDetalle> queryable_FoliosEnInventarioDetalle = repositorioDetalle.ObtenerPorFiltro(x => x.IdInventario == bancoEncontrado.IdInventario && x.Activo == true);
            int cantidadFoliosSaltados = 0;
            foreach (int folioObtenido in foliosEnordenRecuperados)
            {
                Tbl_InventarioDetalle folioEncontradoEnInventario = queryable_FoliosEnInventarioDetalle.Where(x => x.NumFolio == folioObtenido && x.Activo == true).FirstOrDefault();

                if (folioEncontradoEnInventario != null && folioEncontradoEnInventario.IdIncidencia == null)
                {
                    cantidadFoliosSaltados += 1;
                }
            }


            if (cantidadFoliosSaltados == 4)
            {
                //Si existen folios consecutivos que se estan saltando
                bandera = true;
            }
            return bandera;
        }


        //*** VERIFICAR DISPONIBILIDAD DE FOLIOS ************************************************************************************************************************************************************************************************************//
        public static List<FoliosAFoliarInventario> verificarDisponibilidadFoliosEnInventarioDetalle(int IdBanco, int FInicial, int TotalRegistrosAFoliar, bool Inhabilitado, int InhabilitadoInicial, int InhabilitadoFinal)
        {
            List<FoliosAFoliarInventario> listaFoliosVerificados = new List<FoliosAFoliarInventario>();



            var transaccion = new Transaccion();
            var repositorioTblBanco = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            Tbl_CuentasBancarias bancoEncontrado = repositorioTblBanco.Obtener(x => x.Id == IdBanco && x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true);
            var repoDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            IQueryable<Tbl_InventarioDetalle> queryable_FoliosEnInventarioDetalle = repoDetalle.ObtenerPorFiltro(x => x.IdInventario == bancoEncontrado.IdInventario && x.Activo == true);


            //obtiene los numero de folios como deben de encontrarse en la tabla inventarios
            List<int> foliosEnordenRecuperados = FoliarConsultasDBSinEntity.ObtenerListaDefolios(FInicial, TotalRegistrosAFoliar, Inhabilitado, InhabilitadoInicial, InhabilitadoFinal);

            foreach (int folioObtenido in foliosEnordenRecuperados)
            {
                Tbl_InventarioDetalle folioEncontradoEnInventario = queryable_FoliosEnInventarioDetalle.Where(x => x.NumFolio == folioObtenido && x.Activo == true).FirstOrDefault();
                if (folioEncontradoEnInventario != null)
                {
                    FoliosAFoliarInventario nuevoFolioAFoliar = new FoliosAFoliarInventario();
                    nuevoFolioAFoliar.Id = folioEncontradoEnInventario.Id;
                    nuevoFolioAFoliar.Folio = folioEncontradoEnInventario.NumFolio;

                    if (folioEncontradoEnInventario.IdIncidencia == null)
                    {
                        nuevoFolioAFoliar.Incidencia = "";
                    }
                    else
                    {
                        nuevoFolioAFoliar.Incidencia = folioEncontradoEnInventario.Tbl_InventarioTipoIncidencia.Descrip_Incidencia.Trim();

                        if (nuevoFolioAFoliar.Incidencia.Contains("Asignado"))
                        {

                            nuevoFolioAFoliar.FechaIncidencia = folioEncontradoEnInventario.FechaAsignacionExterna?.ToString("dd/MM/yyyy");
                        }
                        else 
                        {

                            nuevoFolioAFoliar.FechaIncidencia = folioEncontradoEnInventario.FechaIncidencia?.ToString("dd/MM/yyyy");
                        }
                    }

                    nuevoFolioAFoliar.IdContenedor = folioEncontradoEnInventario.IdContenedor;
                    nuevoFolioAFoliar.NombreBancoCuenta = bancoEncontrado.NombreBanco +" || " + bancoEncontrado.Cuenta ;
                    listaFoliosVerificados.Add(nuevoFolioAFoliar);

                }
                else
                {
                    FoliosAFoliarInventario nuevoFolioAFoliar = new FoliosAFoliarInventario();
                    nuevoFolioAFoliar.Folio = folioObtenido;
                    nuevoFolioAFoliar.Incidencia = "No existe el folio";

                    listaFoliosVerificados.Add(nuevoFolioAFoliar);
                    //entra si el folio inicial no fue encontrado en los contenedore lo que quieredecir que no hay un registro de ese folio por lo tnato no existe

                }
            }

            return listaFoliosVerificados;
        }




        #endregion










        /****** Recuper Folio de IdPago seleccionado ****************************************/
        public static string RestaurarFolioChequeDeIdPago_ANTIGUO(int IdPago)
        {
            var transaccion = new Transaccion();
            var repoTbl_Pago = new Repositorio<Tbl_Pagos>(transaccion);
            var repoTbl_InventarioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            var repoTbl_Contenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repoTbl_Inventario = new Repositorio<Tbl_Inventario>(transaccion);

            Tbl_Pagos pagoEncontrado = repoTbl_Pago.Obtener(x => x.Id == IdPago && x.Activo == true);


            if (pagoEncontrado.IdTbl_Referencias_Cancelaciones != null) 
            {
                //verifica que el cheque no este en cargada dentro de una cancelacion, si lo esta no dejara recuperarlo
                return "El folio seleccionado no se puede recuperar ya que actualmente se encuentra cargado en la referencia "+ pagoEncontrado.Tbl_Referencias_Cancelaciones.Numero_Referencia;
            }

            DatosCompletosBitacoraDTO datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(pagoEncontrado.Id_nom, pagoEncontrado.Anio);


            string numeroEmpleado5Digitos = Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(pagoEncontrado.NumEmpleado);

            string totalRegistrosLimpiosEnDBF = FoliarDBF.FolearDBFEnServerNegocios.LimpiarRegitroBaseDBF(datosCompletosNomina.Ruta, numeroEmpleado5Digitos, datosCompletosNomina, pagoEncontrado);


            if (totalRegistrosLimpiosEnDBF.Equals("1"))
            {
                /* Limpiar datos de foliacion del AN de en SQL */
                int registroAnLimpiado = FoliarConsultasDBSinEntity.LimpiarUnRegitroCamposFoliacionAN(datosCompletosNomina.Anio, datosCompletosNomina.An, numeroEmpleado5Digitos, pagoEncontrado.ImporteLiquido, datosCompletosNomina.EsPenA, ""/*No contine Beneficiario pena*/);

                if (registroAnLimpiado == 1) 
                {
                   Tbl_InventarioDetalle detalleEncontrado = repoTbl_InventarioDetalle.Obtener(x => x.Id == pagoEncontrado.IdTbl_InventarioDetalle);
                    
                    Tbl_InventarioContenedores contenedorLocalizado = repoTbl_Contenedores.Obtener(x => x.Id == detalleEncontrado.IdContenedor);

                    contenedorLocalizado.FormasDisponiblesActuales += 1;
                     switch (detalleEncontrado.IdIncidencia)
                    {
                        case 1:
                            //Estaba Inhabilitada
                            contenedorLocalizado.FormasInhabilitadas -= 1;
                            break;
                        case 2:
                            //Estaba Asignada
                            contenedorLocalizado.FormasAsignadas -= 1;
                            break;
                        case 3:
                            //Estaba Foliada correctamente
                            contenedorLocalizado.FormasFoliadas -=1;
                            break;
                        case 4:
                            //Estaba Inhabilitada por algun defento de la impresora
                            contenedorLocalizado.FormasInhabilitadas -= 1;
                            break;
                    }


                    //limpiart campos del detalle del folio
                    detalleEncontrado.IdIncidencia = null;
                    detalleEncontrado.FechaIncidencia = null;
                    detalleEncontrado.IdEmpleado = null;




                    Tbl_Inventario invetarioaEncontrado = repoTbl_Inventario.Obtener(x => x.Id == contenedorLocalizado.IdInventario);
                    invetarioaEncontrado.FormasDisponibles += 1;

                    if(detalleEncontrado != null && contenedorLocalizado != null && invetarioaEncontrado != null )
                    {

                        pagoEncontrado.FolioCheque = 0;
                        pagoEncontrado.FolioCFDI = 0;
                        pagoEncontrado.Integridad_HashMD5 = "CHEQUE RECUPERADO EXITOSAMENTE, VUELVA A FOLEAR DE NUEVO";
                        pagoEncontrado.IdTbl_CuentaBancaria_BancoPagador = 0;
                        pagoEncontrado.IdCat_EstadoPago_Pagos = 4;
                        pagoEncontrado.IdTbl_InventarioDetalle = null;
                       

                        transaccion.GuardarCambios();
                        return "CORRECTO";

                    }

                }
            
            }
            else 
            {
                return totalRegistrosLimpiosEnDBF;
            }



            return "Error";
        }

        public static string RestaurarFolioChequeDeIdPago(int IdPago)
        {
            var transaccion = new Transaccion();
            var repoTbl_Pago = new Repositorio<Tbl_Pagos>(transaccion);

            Tbl_Pagos pagoEncontrado = repoTbl_Pago.Obtener(x => x.Id == IdPago && x.Activo == true);
            if (pagoEncontrado.IdTbl_Referencias_Cancelaciones != null)
            {
                //verifica que el cheque no este en cargada dentro de una cancelacion, si lo esta no dejara recuperarlo
                return "El folio seleccionado no se puede recuperar ya que actualmente se encuentra cargado en la referencia " + pagoEncontrado.Tbl_Referencias_Cancelaciones.Numero_Referencia;
            }

            var repoTbl_InventarioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            var repoTbl_Contenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repoTbl_Inventario = new Repositorio<Tbl_Inventario>(transaccion);

            
            DatosCompletosBitacoraDTO datosCompletosNomina = FoliarNegocios.ObtenerDatosCompletosBitacoraPorIdNom_paraControlador(pagoEncontrado.Id_nom, pagoEncontrado.Anio);
            string numeroEmpleado5Digitos = Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(pagoEncontrado.NumEmpleado);

            
            int numeroRegistroLimpiadoDBF = 0;
            AlertasAlFolearPagomaticosDTO alerta = FolearDBFEnServerNegocios.ModificacionDBF_SuspenderReponer_YLimpiarUnRegistro(3 /* Opcion 3 por blanquear un registro en DBF*/, datosCompletosNomina, numeroEmpleado5Digitos, pagoEncontrado, "" /*  No contiene una reposicion  */);
            if (alerta.IdAtencion == 200)
            {
                numeroRegistroLimpiadoDBF = alerta.NumeroRegistrosActualizados;
            }
            else
            {
                return alerta.Detalle;
            }



            if (numeroRegistroLimpiadoDBF == 1)
            {
                /* Limpiar datos de foliacion del AN de en SQL */
                int registroAnLimpiado = FoliarConsultasDBSinEntity.LimpiarUnRegitroCamposFoliacionAN(datosCompletosNomina.Anio, datosCompletosNomina.An, numeroEmpleado5Digitos, pagoEncontrado.ImporteLiquido, datosCompletosNomina.EsPenA, pagoEncontrado.BeneficiarioPenA);

                if (registroAnLimpiado == 1)
                {
                    Tbl_InventarioDetalle detalleEncontrado = repoTbl_InventarioDetalle.Obtener(x => x.Id == pagoEncontrado.IdTbl_InventarioDetalle);

                    Tbl_InventarioContenedores contenedorLocalizado = repoTbl_Contenedores.Obtener(x => x.Id == detalleEncontrado.IdContenedor);

                    contenedorLocalizado.FormasDisponiblesActuales += 1;
                    switch (detalleEncontrado.IdIncidencia)
                    {
                        case 1:
                            //Estaba Inhabilitada
                            contenedorLocalizado.FormasInhabilitadas -= 1;
                            break;
                        case 2:
                            //Estaba Asignada
                            contenedorLocalizado.FormasAsignadas -= 1;
                            break;
                        case 3:
                            //Estaba Foliada correctamente
                            contenedorLocalizado.FormasFoliadas -= 1;
                            break;
                        case 4:
                            //Estaba Inhabilitada por algun defento de la impresora
                            contenedorLocalizado.FormasInhabilitadas -= 1;
                            break;
                    }


                    //limpiart campos del detalle del folio
                    detalleEncontrado.IdIncidencia = null;
                    detalleEncontrado.FechaIncidencia = null;
                    detalleEncontrado.IdEmpleado = null;




                    Tbl_Inventario invetarioaEncontrado = repoTbl_Inventario.Obtener(x => x.Id == contenedorLocalizado.IdInventario);
                    invetarioaEncontrado.FormasDisponibles += 1;

                    if (detalleEncontrado != null && contenedorLocalizado != null && invetarioaEncontrado != null)
                    {

                        pagoEncontrado.FolioCheque = 0;
                        pagoEncontrado.Integridad_HashMD5 = "CHEQUE RECUPERADO EXITOSAMENTE, VUELVA A FOLEAR DE NUEVO";
                        pagoEncontrado.IdTbl_CuentaBancaria_BancoPagador = 0;
                        pagoEncontrado.IdCat_EstadoPago_Pagos = 4;
                        pagoEncontrado.IdTbl_InventarioDetalle = null;


                        transaccion.GuardarCambios();
                        return "CORRECTO";

                    }

                }

            }
           


            return "Error";
        }





    }
}
