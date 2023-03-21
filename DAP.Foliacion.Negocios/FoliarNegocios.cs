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
using DAP.Foliacion.Entidades.DTO.FoliarDTO.FoliacionChequesDTO;


namespace DAP.Foliacion.Negocios
{
    public class FoliarNegocios
    {
        public static string ObtenerCadenaAnioInterface(int AnioInterface) 
        {
            string anio = "";
            if (AnioInterface != Convert.ToInt32(DateTime.Now.Year))
            {
                anio = "" + AnioInterface + "";
            }

            return anio;
        }

        public static int ObtenerAnioDeQuincena(string Quincena)
        {
            return Convert.ToInt32(DateTime.Now.Year.ToString().Substring(0, 2) + Quincena.Substring(0, 2));
        }

        public static string ObtenerNumeroFolio8DIgitosPagomaticoNUMCHE(int number)
        {
            return String.Format("{0:D8}", number);
        }
        
        



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


        public static Dictionary<int, string> ObtenerNombreNominasEnQuincena(string NumeroQuincena, int AnioInterface)
        {
            string visitaAnioInterface = ObtenerCadenaAnioInterface(AnioInterface);
            List<DatosCompletosBitacoraDTO> nombresNomina = ObtenerDatosCompletosBitacoraFILTRO(visitaAnioInterface, NumeroQuincena);

            Dictionary<int, string> nombresListosNomina = new Dictionary<int, string>();

            int i = 0;
            foreach (DatosCompletosBitacoraDTO NuevaNomina in nombresNomina)
            {
                i += 01;
                string EsImportado = NuevaNomina.Importado ? "" : "NO IMPORTADO";
                if (NuevaNomina.Adicional == "")
                {
                    nombresListosNomina.Add(Convert.ToInt32(NuevaNomina.Id_nom), "  " + i + " -" + "-" + "- " + EsImportado+ " [ " + NuevaNomina.Quincena + " ]   [ " + NuevaNomina.Id_nom + " ]  [ " + NuevaNomina.Nomina + " ] " + " [ " + NuevaNomina.RutaNomina + " ] " + " -" + "-" + " [ " + NuevaNomina.Ruta + NuevaNomina.RutaNomina + " ] " + " -" + "-" + "-" + "-" + "-" + "-" + "-" + "-" + " [ " + NuevaNomina.Coment + " ] ");
                }
                else
                {
                    nombresListosNomina.Add(Convert.ToInt32(NuevaNomina.Id_nom), "  " + i + " -" + "-" + "- " + EsImportado+ " [ " + NuevaNomina.Quincena + " ]    [ " + NuevaNomina.Id_nom + " ]  [ " + NuevaNomina.Nomina + " ] " + " [ " + NuevaNomina.RutaNomina + " ] " + " -" + "-" + "- " + "ADICIONAL" + " -" + "-" + "- " + NuevaNomina.Adicional + " -" + "- " + " [ " + NuevaNomina.Ruta + NuevaNomina.RutaNomina + " ] " + " -" + "-" + "-" + "-" + "-" + "-" + "-" + "-" + " [ " + NuevaNomina.Coment + " ] ");
                }


            }

            return nombresListosNomina;
        }

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




        /********************************************************************************************************************************************************/
        /****************************************   SOBRE CARGA DE METODOS PARA OBTENER LOS DATOS COMPLETOS DE LA BITACORA SEGUN LAS NECESIDADES    *************/
        /********************************************************************************************************************************************************/
        /// <summary>
        /// Obtiene la base An elegida por el IdNom y el anio de la interface elegida
        /// </summary>
        /// <param name="IdNom"></param>
        /// <param name="AnioInterface"></param>
        /// <returns>Retorna un objeto del tipo DatosCompletosBitacoraDTO </returns>
        public static DatosCompletosBitacoraDTO ObtenerDatosCompletosBitacoraFILTRO(int IdNom, string AnioInterface)
        {
            return FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraFILTRO(IdNom, AnioInterface);
        }

        /// <summary>
        /// Obtiene todas las nominas de la quincena y anioInterfas elegidos
        /// </summary>
        /// <param name="AnioInterface"></param>
        /// <param name="Quincena"></param>
        /// <returns>Retorna una lista de objetos del tipo DatosCompletosBitacoraDTO</returns>
        public static List<DatosCompletosBitacoraDTO> ObtenerDatosCompletosBitacoraFILTRO(string AnioInterface, string Quincena)
        {
            return FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraFILTRO( AnioInterface, Quincena);
        }








        //********************************************************************************************************************************************************************//




        #region METODOS DE PAGOMATICOS => DISPERSIONES POR TARJETA


        //************************************************************************************************************************************************************************************//
        //****************************************          VERFIFICAR LAS NOMINAS CON PAGOMATICO SI YA ESTAN FOLIADAS O NO  (VERIFICA en SQL)    ********************************************//
        //************************************************************************************************************************************************************************************//
        /*VERIFICA SI YA ESTA FOLIADO UNA NOMINA QUE CONTIENE PAGAMATICO*/
        public static AlertaDeNominasFoliadasPagomatico EstaFoliadaNominaSeleccionadaPagoMatico(int IdNom, int AnioInterface)
        {
            //IdEstaFoliada 
            //0 para decir que no esta foliada ni en SQL ni en FCCBNet     { False }
            //1 para los que estan foliadas  en SQL como en FCCBNetDB       { true  }
            //2 para los que no tienen pagomaticos  { else  }
            //3 para los AN que no se han importado de la dbf hacia SQL segun la bitacora  
            //4 LA BASE EN SQL ESTA FOLIADA POR ALGUNA RAZON, PERO NO HAY REGISTRO EN FCCBNetDB => VErificar con el desarrollador por que sucedio (se resuelve limpiando la base de sql de AN)
            //5 LA BASE EN SQL NO FOLIADA POR ALGUNA RAZON, PERO SI HAY REGISTROS EN FCCBNetDB que indican que en algun momento fue foleada => VErificar con el desarrollador por que sucedio (En este caso debe solo actualizar los datos)

            List<AlertaDeNominasFoliadasPagomatico> AlertasEncontradas = new List<AlertaDeNominasFoliadasPagomatico>();

            string visitaAnioInterface = ObtenerCadenaAnioInterface( AnioInterface);
            DatosCompletosBitacoraDTO detalleNominaObtenido = ObtenerDatosCompletosBitacoraFILTRO(IdNom, visitaAnioInterface);
            int anioInterfasQuincena = ObtenerAnioDeQuincena(detalleNominaObtenido.Quincena);
            string visitaAnioInterfaceQuinceSeleccionada = ObtenerCadenaAnioInterface(anioInterfasQuincena);
            int quincenaSeleccionada = Convert.ToInt32(detalleNominaObtenido.Quincena);

            

            AlertaDeNominasFoliadasPagomatico nuevaAlerta = new AlertaDeNominasFoliadasPagomatico();

            if (detalleNominaObtenido.Importado)
            {
                // REGISTROS TOTALES CONTENIDOS EN FCCBNetDB
                Transaccion transaccion = new Transaccion();
                var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
                int pagomaticosRegistradosFCCBnetDB = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioInterfasQuincena && x.Quincena == quincenaSeleccionada && x.Id_nom == detalleNominaObtenido.Id_nom && x.IdCat_FormaPago_Nacimiento == 2).Count();

                List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(detalleNominaObtenido.An, visitaAnioInterfaceQuinceSeleccionada);
                //Pagomaticos
                string condicionesBancosPagomatico = consultasPagomaticos.ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);
                int totalRegistrosPagomaticos = FoliarConsultasDBSinEntity.ObtenerTotalRegistrosNoFoliadosSegunCondicion(visitaAnioInterfaceQuinceSeleccionada, detalleNominaObtenido.An, condicionesBancosPagomatico);

                nuevaAlerta.NumeroRegistrosAFoliar = totalRegistrosPagomaticos;


                // se compara con 0 por queque puede ser que no tenga pagomaticos y solo cheques 
                if (totalRegistrosPagomaticos == 0)
                {
                    //2 para los que no tienen pagomaticos  { else  }
                    nuevaAlerta.IdEstaFoliada = 2;
                    nuevaAlerta.Id_Nom = detalleNominaObtenido.Id_nom;
                    nuevaAlerta.NumeroNomina = detalleNominaObtenido.Nomina;
                    nuevaAlerta.Adicional = detalleNominaObtenido.Adicional;
                    nuevaAlerta.NombreNomina = detalleNominaObtenido.Coment;
                    return nuevaAlerta;

                }

                if(pagomaticosRegistradosFCCBnetDB != totalRegistrosPagomaticos ) 
                {
                    //0 para decir que no esta foliada en FCCBNet y probablemente tampoco en  AN-sql  
                    nuevaAlerta.IdEstaFoliada = 0;
                }
                if (pagomaticosRegistradosFCCBnetDB == totalRegistrosPagomaticos) 
                {
                    //Si el total de regitros en FCCBNet es igual al Total de regitros en An es porque alguna vez ya fue foleado
                    
                    //Obtine los registros que se encuentran foleados dentro de la nomina
                    string consultaRegitrosFoliados = consultasPagomaticos.ObtenerRegitrosFoliados_NominaPagomatico(visitaAnioInterface, detalleNominaObtenido.An, condicionesBancosPagomatico);
                    int registrosTotalesPagomaticosFoliados = FoliarConsultasDBSinEntity.ObtenerRegistro_FoliacionPagomatico(consultaRegitrosFoliados);
                    
                    
                    if (totalRegistrosPagomaticos > 0 && pagomaticosRegistradosFCCBnetDB != registrosTotalesPagomaticosFoliados )
                    {
                        //si ya existen registros en FCCBNet pero el total de registros es diferente a los foliados en AN-SQL solo se deberia actualizar el AN a como esta en FCCBNet
                        //5 LA BASE EN SQL NO FOLIADA POR ALGUNA RAZON, PERO SI HAY REGISTROS EN FCCBNetDB que indican que en algun momento fue foleada => VErificar con el desarrollador por que sucedio (En este caso debe solo actualizar los datos de FCCBNet a AN-SQL)
                        nuevaAlerta.IdEstaFoliada = 5;
                    }
                    else 
                    {
                        //1 para los que estan foliadas  en SQL como en FCCBNetDB {true}
                        nuevaAlerta.IdEstaFoliada = 1;

                    }

                }






                ////List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(detalleNominaObtenido.An, detalleNominaObtenido.Anio);
                //string condicionBancosParaSerPagomatico = consultasPagomaticos.ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);

                ////Obtiene los registros totales de la nomina con la condicion de pagomaticos que se deberian de foliar
                //string consultaRegistrosAFoliar = consultasPagomaticos.ObtenerRegistrosAFoliar_NominaPagomatico(visitaAnioInterface, detalleNominaObtenido.An, condicionBancosParaSerPagomatico);
                //int registrosTotalesAFoliar = FoliarConsultasDBSinEntity.ObtenerRegistro_FoliacionPagomatico(consultaRegistrosAFoliar);

                ////Obtine los registros que se encuentran foleados dentro de la nomina
                //string consultaRegitrosFoliados = consultasPagomaticos.ObtenerRegitrosFoliados_NominaPagomatico(visitaAnioInterface,  detalleNominaObtenido.An, condicionBancosParaSerPagomatico);
                //int registrosTotalesFoliados = FoliarConsultasDBSinEntity.ObtenerRegistro_FoliacionPagomatico(consultaRegitrosFoliados);


                //var (existenRegistrosEnFCCBNetDB, cantidadRegistrosFCCBNetDB) = ExistenRegistrosPagomaticosCantidadEnNominaSelecionadaEnFCCBNetDB(anioInterfasQuincena, detalleNominaObtenido.Quincena, detalleNominaObtenido.Id_nom);



                //nuevaAlerta.NumeroRegistrosAFoliar = registrosTotalesAFoliar;
                //if (registrosTotalesAFoliar > 0 && registrosTotalesFoliados == registrosTotalesAFoliar)
                //{
                //    //Esta Foliado tanto en la base de sql como en FCCBNetDB
                //    if (existenRegistrosEnFCCBNetDB)
                //    {
                //        nuevaAlerta.IdEstaFoliada = 1;
                //    }
                //    else
                //    {
                //        nuevaAlerta.IdEstaFoliada = 4;
                //    }

                //}
                //else if (registrosTotalesAFoliar == 0 && registrosTotalesFoliados == 0)
                //{
                //    //si no lee ningun registro es por que no cuenta con pagomaticos 
                //    nuevaAlerta.IdEstaFoliada = 2;
                //}
                //else if (registrosTotalesAFoliar > 0 && registrosTotalesFoliados != registrosTotalesAFoliar)
                //{

                //    if (!existenRegistrosEnFCCBNetDB)
                //    {
                //        //No esta Foliado en la base sql y tampoco hay registros en FCCBNetDB (Todo es correcto y puede folear)
                //        nuevaAlerta.IdEstaFoliada = 0;
                //    }
                //    else 
                //    {
                //        //5 LA BASE EN SQL NO FOLIADA POR ALGUNA RAZON, PERO SI HAY REGISTROS EN FCCBNetDB => esta parte servira para en algun futuro poder refoliar la nomina con pagomaticos
                //        //tomando en cuanta que cuando se vuelva a refoliar donde el campo observa != "TALON X CHEQUE"
                //        nuevaAlerta.IdEstaFoliada = 5;
                //    }

                //}
            }
            else 
            {
                //3 para los AN que no se han importado de la dbf hacia SQL segun la bitacora  
                nuevaAlerta.NumeroRegistrosAFoliar = 0;
                nuevaAlerta.IdEstaFoliada = 3;
            }

            nuevaAlerta.Id_Nom = detalleNominaObtenido.Id_nom;
            nuevaAlerta.NumeroNomina = detalleNominaObtenido.Nomina;
            nuevaAlerta.Adicional = detalleNominaObtenido.Adicional;
            nuevaAlerta.NombreNomina = detalleNominaObtenido.Coment;

            //AlertasEncontradas.Add(nuevaAlerta);
            //return AlertasEncontradas;
            return nuevaAlerta;
        }

        /*VERIFICA SI YA ESTAN FOLIADAS TODAS LAS NOMINAS QUE CONTIENEN PAGOMATICOS CON SUS RESPECTIVOS REGISTROS TOTALES A FOLIAR*/
        public static List<AlertaDeNominasFoliadasPagomatico> VerificarTodasNominaPagoMatico(string Quincena)
        {
            //IdEstaFoliada 
            //0 para decir que no esta foliada      { False }
            //1 para los que estan foliadas         { true  }
            //2 para los que no tienen pagomaticos  { else  }
            //3 para los AN que no se han importado de la dbf hacia SQL segun la bitacora  
            //4 LA BASE EN SQL ESTA FOLIADA POR ALGUNA RAZON, PERO NO HAY REGISTRO EN FCCBNetDB => VErificar con el desarrollador por que sucedio (SE resuelve limpiando la base de sql de AN)
            //5 LA BASE EN SQL NO FOLIADA POR ALGUNA RAZON, PERO SI HAY REGISTROS EN FCCBNetDB que indican que en algun momento fue foleada => VErificar con el desarrollador por que sucedio (En este caso debe solo actualizar los datos)

            List<AlertaDeNominasFoliadasPagomatico> AlertasEncontradas = new List<AlertaDeNominasFoliadasPagomatico>();

            int anio = ObtenerAnioDeQuincena(Quincena); 
            string visitaAnioInterface = ObtenerCadenaAnioInterface(anio);
            List<DatosCompletosBitacoraDTO> DetallesNominasObtenidos = ObtenerDatosCompletosBitacoraFILTRO(visitaAnioInterface, Quincena);

            foreach (DatosCompletosBitacoraDTO nuevaNominaObtenida in DetallesNominasObtenidos)
            {
                AlertasEncontradas.Add(EstaFoliadaNominaSeleccionadaPagoMatico(nuevaNominaObtenida.Id_nom, anio));
            }

            return AlertasEncontradas;
        }

        /// <summary>
        /// verifica que existen registros y cuantos en la base de datos FCCBNetDB de acuerdo a la nomina
        /// </summary>
        /// <param name="Anio"></param>
        /// <param name="Quincena"></param>
        /// <param name="IdNom"></param>
        /// <returns></returns>
        public static (bool, int) ExistenRegistrosPagomaticosCantidadEnNominaSelecionadaEnFCCBNetDB(int Anio, string Quincena, int IdNom) 
        {
            Transaccion transaccion = new Transaccion();
            var repoTblPagos =new Repositorio<Tbl_Pagos>(transaccion);
            int quincenaSeleccionada = Convert.ToInt32(Quincena);
            int registrosObtenidos = repoTblPagos.ObtenerPorFiltro(x => x.Anio == Anio && x.Quincena == quincenaSeleccionada && x.Id_nom == IdNom && x.IdCat_FormaPago_Nacimiento == 2).Count();

            bool existenRegistros = false;
            int totalDeRegistros = 0;
            if (registrosObtenidos > 0) 
            {
                existenRegistros = true;
                totalDeRegistros = registrosObtenidos;
            }

            return (existenRegistros, totalDeRegistros);
        }



        //****************************************************************************************************************************************************************************************************//
        //*****************        Revicion de Nomina PAGOMATICO => SIRVE PARA LLENAR EL REPORTE DE COMO SE ENCUANTRA EL AN EN SQL Y SABER SI ESTA O NO FOLEADA     ******************************************//
        //****************************************************************************************************************************************************************************************************//
        public static List<ResumenRevicionNominaPDFDTO> ObtenerDatosPersonalesNominaReportePagomatico(string An, string visitaAnioInterface , string Nomina )
        {
            
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(An, visitaAnioInterface);
            string condicionBancosParaSerPagomatico = consultasPagomaticos.ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);

            
            string consulta = consultasPagomaticos.ObtenerConsultaDetalleDatosPersonalesNomina_ReportePagomatico( visitaAnioInterface , An , condicionBancosParaSerPagomatico);

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
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        //************************************************************************* FOLEAR NOMINAS CON PAGOMATICOS   *********************************************************//
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        //********************************************************************************************************************************************************************//
        public static async Task<List<Tbl_CuentasBancarias>> obtenerCuentasBancariasVigentesPorNomina(DatosCompletosBitacoraDTO datosCompletosNomina)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            List<Tbl_CuentasBancarias> resultadoDatosBanco = new List<Tbl_CuentasBancarias>();

            if (datosCompletosNomina.Anio > 2021)
            {
                //Obtiene los bancos actuales de los cambios de cuentas del 2022
                resultadoDatosBanco = await repositorio.ObtenerPorFiltro(x => x.InicioBaja == null && x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.NombreBanco != "BANAMEX").ToListAsync();
            }
            else if (datosCompletosNomina.Anio < 2022)
            {
                //Obtiene bancos antes del cambio de cuentas del 2021
                resultadoDatosBanco = await repositorio.ObtenerPorFiltro(x => x.InicioBaja == true && x.IdCuentaBancaria_TipoPagoCuenta != 2 && x.NombreBanco != "BANAMEX").ToListAsync();
            }

            /****************************************************************************************************************************************************************************/
            //******Agregar el banco de banamex segun si es PENSION ALIMENTICIA (PENA) y el ANIO AL QUE PERTENECE LA NOMINCA YA QUE EN EL 2022 HUBO UN CAMBIO DE CUENTAS BANCARIAS*******//

            var cuentaBanamexSegunAnioNomina = repositorio.ObtenerPorFiltro(x => x.NombreBanco == "BANAMEX");
            if (datosCompletosNomina.EsPenA)
            {
                resultadoDatosBanco.Add(await cuentaBanamexSegunAnioNomina.Where(x => x.EsPenA == true).FirstOrDefaultAsync());
            }
            else
            {
                if (datosCompletosNomina.Anio > 2021)
                {
                    // obtiene las nuevas cuentas bancarias del 2022
                    resultadoDatosBanco.Add(await cuentaBanamexSegunAnioNomina.Where(x => x.EsPenA == null && x.InicioBaja == null || x.InicioBaja == false  ).FirstOrDefaultAsync());
                }
                else if (datosCompletosNomina.Anio < 2022)
                {
                    //Obtiene la cuenta de banamex que estaba activa el año 2021 ya que con esa se foleaba y en el 2022 hubo un cambio de cuantas bancarias 
                    resultadoDatosBanco.Add(await cuentaBanamexSegunAnioNomina.Where(x => x.EsPenA == null && x.InicioBaja == true).FirstOrDefaultAsync());
                }
            }
            return resultadoDatosBanco;
        }

        public static async Task<AlertasAlFolearPagomaticosDTO> FolearPagomaticoPorNominaaAsincrono(int IdNom, string NumeroQuincena)
        {
            /************************************************************************/
            /**   1 == (USADO x DBF) FOLIACION PARA AMBAS FORMAS DE PAGO (PAGAMATICO Y CHEQUES)  **/
            /**   2 == **/
            /**   3 == (USADO x SQL) NO TIENE REGISTROS PARA FOLIAR CON PAGOMATICOS O ALGUN OTRO ERROR QUE CAUSE NULL EN LA RESPUESTA**/
            /**   4 == **/
            /**   5 == (USADO x DBF) LIMPIEZA DE CAMPOS DBF CONDICIONADOS A LOS NUMEROS DE EMPLEADOS EN CONDICION POR INCUMPLIMIENDO DEL AJUSTE DE INHABILITADOS EN LA FOLIACION **/
            /**   6 == (USADO x DBF) LIMPIEZA DE CAMPOS DBF CONDICIONADOS CUANDO LA FOLIACION DEL PUNTO 1 NO CUMPLE CON EL ESTANDART PARA AMBAS FORMAS DE PAGO**/
            /**   7 == **/
            /**   8 == **/
            /**   200 == (USADO x DBF) TODO SALIO CORRECTO Y SIN NINGUN ERROE EN LA BASE **/
            /***********************************************************************/
            //List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();
            //AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();
            var transaccion = new Transaccion();
            var repositorioBancos = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            int numeroRegistrosActualizados_BaseDBF = 0;
            int numeroRegistrosActualizados_AlPHA = 0;
            int registrosInsertados_FCCBNetDB = 0;

            int anioInterfaz = FoliarNegocios.ObtenerAnioDeQuincena(NumeroQuincena);
            string visitaAnioInterface = ObtenerCadenaAnioInterface(anioInterfaz);
            DatosCompletosBitacoraDTO datosCompletosNomina = ObtenerDatosCompletosBitacoraFILTRO(IdNom, visitaAnioInterface);
            /*Obtiene los bancos disponibles con lo que se podria foliar la nomina seleccionada, tambien considera que si es una nomina 08(PENA) Debe traer la nomina de Banamex de Pension sino debe traer la cuenta de banamex en general dependiendo del anio (Ya que al entrar al 2022 hubo un cambio de cuentas bancarias) */
            List<Tbl_CuentasBancarias> cuentasBancosDisponibles = await obtenerCuentasBancariasVigentesPorNomina(datosCompletosNomina);


            
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, visitaAnioInterface);
            string condicionBancosParaSerPagomatico = consultasPagomaticos.ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);
            string condicionDeIdCuentaBancaria = consultasPagomaticos.ValidaBancosExistentenEnNominaSeleccionada_FoliacionPagomaticos(cuentasBancosDisponibles, bancosContenidosEnAn);


            int complementoQuincena = Convert.ToInt32(datosCompletosNomina.Quincena.Substring(1, 3));
            string ConsultaDetalle_FoliacionPagomatico = consultasPagomaticos.ObtenerConsultaDetalle_FoliacionPagomatico(datosCompletosNomina.An, visitaAnioInterface, datosCompletosNomina.EsPenA, cuentasBancosDisponibles);
            List<ResumenPersonalAFoliarDTO> resumenPersonalAFoliar = FoliarConsultasDBSinEntity.ObtenerDatosPersonalesNomina_FoliacionPAGOMATICO(ConsultaDetalle_FoliacionPagomatico, complementoQuincena, datosCompletosNomina.EsPenA);

            if (resumenPersonalAFoliar == null)
            {
                AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();
                nuevaAlerta.IdAtencion = 3;
                return nuevaAlerta;
                //Advertencias.Add(nuevaAlerta);
                //return Advertencias;
            }
            else if (resumenPersonalAFoliar.Count() > 0)
            {
                /********************************************************************************************************************************************************************/
                /***********************             Permite el acceso a una carpeta que se encuentra compartida dentro del servidor            ************************************/
                /***********************                                 Primer hilo de Ejecucion en secundo plano                               ************************************/
                /********************************************************************************************************************************************************************/
                Task<AlertasAlFolearPagomaticosDTO> alertaEncontradaTarea = Task.Run(() =>
                {
                    return FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(1 /* Se redireccionara a foliar una DBF */ , datosCompletosNomina, resumenPersonalAFoliar, "" /* no lleva una condicion ya que el update se hace con un filtro especifico*/);
                });



                /*******************************************************************************************************************************************************************************/
                /***************************************                    Actualiza la base cargada en SQL                *********************************************************************/
                /***********************                              SEGUNDO HILO DE EJECUCION EN SEGUNDO PLANO                                            ************************************/
                /*******************************************************************************************************************************************************************************/
                CancellationTokenSource mitokenOrigen = new CancellationTokenSource();
                CancellationToken cancelaToken = mitokenOrigen.Token;

                Task<int> task_resultadoRegitrosActualizados_InterfacesAlPHA = Task.Run(() =>
                {
                    return FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql_transaccionado_CANCELACIONHILO(resumenPersonalAFoliar, datosCompletosNomina, visitaAnioInterface, cancelaToken);
                });



                /*******************************************************************************************************************************************************************************/
                /********************************************        ACTUALIZA O INSERTA REGISTROS EN LA BASE DE DATOS DEL SISTEMA  (FCCBNetDB)   ***********************************************/
                /***********************                             SE EMPIEZAN A CARGAR EL DTO PARA INSERTAR EN EL HILO PRINCIPAL                         ************************************/
                /*******************************************************************************************************************************************************************************/
                //INSERTAR PAGOS
                List<Tbl_Pagos> pagosNuevosAinsertar = new List<Tbl_Pagos>();

                EncriptarCadena encriptar = new EncriptarCadena();
                bool Actualizar = false;
                Repositorio<Tbl_Pagos> repositorioTblPago = new Repositorio<Tbl_Pagos>(transaccion);

                foreach (ResumenPersonalAFoliarDTO nuevaPersona in resumenPersonalAFoliar)
                {
                    /*********************             SI ENTRA ES PORQUE AUN NO ESTA FOLIADO Y SE HARAN INSERTS A DBFOLIACION         *********************************************/
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
                    nuevoPago.FolioCheque = Convert.ToInt32( nuevaPersona.NumChe );
                    nuevoPago.FolioCFDI = nuevaPersona.FolioCFDI;

                    nuevoPago.IdTbl_CuentaBancaria_BancoPagador = nuevaPersona.IdBancoPagador;
                    nuevoPago.IdCat_FormaPago_Nacimiento = 2;
                    nuevoPago.IdCat_FormaPago_Pagos = 2; //1 = cheque , 2 = Pagomatico 

                    nuevoPago.IdCat_EstadoPago_Pagos = 1; //1 = Transito, 2= Pagado
                                                          //Toda forma de pago al foliarse inicia en transito y cambia hasta que el banco envia el estado de cuenta (para cheches) o el conta (transmite el recurso) //Ocupa un disparador para los pagamaticos para actualizar el estado


                    string cadenaDeIntegridad = datosCompletosNomina.Id_nom + " || " + datosCompletosNomina.Nomina + " || " + datosCompletosNomina.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.BancoX + " || " + nuevaPersona.CuentaX + " || " + nuevaPersona.NumBeneficiario;
                    nuevoPago.Integridad_HashMD5 = encriptar.EncriptarCadenaInicial(cadenaDeIntegridad);
                    nuevoPago.Activo = true;

                    pagosNuevosAinsertar.Add(nuevoPago);

                    registrosInsertados_FCCBNetDB++;

                }






                /*******************************************************************************************************************************************************************************/
                /***************************************                    ESPERA QUE EL PRIMER HILO SE HAYA FOLEADO CORRECTAMENTE             ************************************************/
                /*****************************      SI HUBO ALGUN ERROR SE CANCELA EL SEGUNDO HILO Y SI YA HABIA TERMINADO BLANQUEA LA BASE EN SQL          ************************************/
                /*******************************************************************************************************************************************************************************/
                AlertasAlFolearPagomaticosDTO alertaEncontrada = await alertaEncontradaTarea;

               if (alertaEncontrada.IdAtencion != 200)
                {
                   //SI HAY UN ERROR EN LA ACTUALIZACION DE DBF CREA UNA ADVERTENCIA, CANCELA EL SEGUNDO HILO Y POR SEGURIDAD HACE UN ROLBACK A LA BASE EN SQL 
                  //  Advertencias.Add(alertaEncontrada);
                    //Cancela la TARE DEL HILO 2
                    mitokenOrigen?.Cancel();
                    //HACER UN ROLLBACK A LA BASE EN SQL AN
                    FoliarConsultasDBSinEntity.LimpiarANSql_IncumplimientoCalidadFoliacion(datosCompletosNomina.Anio, datosCompletosNomina.An, condicionBancosParaSerPagomatico, datosCompletosNomina.Id_nom);
                    //LIMPIAR LA BASE DBF EN LA RUTA => SOLO LIMPIA LOS CAMPOS (NUM_CHE , CUENTA_x , BANCO_x , OBSERVA) A 
                    FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(6 /* Numero para redireccionar a una limpiaza de la base con una condicion */ , datosCompletosNomina, new List<ResumenPersonalAFoliarDTO>() /* Este paramatro se envia vavio por que no se ocupa para la esta opcion */ , condicionBancosParaSerPagomatico);

                    return alertaEncontrada;
                   // return Advertencias;
                }

                /*******************************************************************************************************************************************************************************/
                /***************************************                    ESPERA QUE EL SEGUNDO HILO SE HAYA FOLEADO CORRECTAMENTE             ***********************************************/
                /*******************************************************************************************************************************************************************************/
                numeroRegistrosActualizados_AlPHA = await task_resultadoRegitrosActualizados_InterfacesAlPHA;


              


                //Convierte la cadena de registros en el numero de registros actualizados en la DBF
                if (alertaEncontrada.IdAtencion == 200)
                    numeroRegistrosActualizados_BaseDBF = alertaEncontrada.NumeroRegistrosActualizados;



                /*****************************************************************************************************************************************************************************************/
                /**********************************************                 Validacion del estandart de Foliacion                *********************************************************************/
                /*****************************************************************************************************************************************************************************************/
                ///Guarda Un lote de transacciones tanto para modificar o hacer inserts  
                ///
                int registrosFoliados = numeroRegistrosActualizados_AlPHA + numeroRegistrosActualizados_BaseDBF;
                DatosCompletosBitacoraDTO datosCompletosNominaFoleada = ObtenerDatosCompletosBitacoraFILTRO(IdNom, visitaAnioInterface);
                if ( numeroRegistrosActualizados_AlPHA == numeroRegistrosActualizados_BaseDBF )
                {
                    /*****************************************************************************************************************************************************************************************/
                    /**********************************************            SE INSERTAR LOS REGISTROS DENTRO DE FCCBNetDB        **************************************************************************/
                    /*****************************************************************************************************************************************************************************************/
                    int resultadoActualizacionOInsercionSegunElCaso = repositorioTblPago.Agregar_EntidadesMasivamente(pagosNuevosAinsertar);

                    AlertasAlFolearPagomaticosDTO alertaExito = new AlertasAlFolearPagomaticosDTO();
                    alertaExito.IdAtencion = 0;
                    alertaExito.NumeroNomina = datosCompletosNomina.Nomina;
                    alertaExito.NombreNomina = datosCompletosNomina.Coment;
                    alertaExito.Detalle = "";
                    alertaExito.Solucion = "";
                    alertaExito.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                    alertaExito.RegistrosFoliados = resultadoActualizacionOInsercionSegunElCaso;
                    alertaExito.UltimoFolioUsado = 0;
                    return alertaExito;
                    //Advertencias.Add(nuevaAlerta);
                }
                else
                {
                    AlertasAlFolearPagomaticosDTO alertaFracasoTotal = new AlertasAlFolearPagomaticosDTO(); 
                    int registrosLimpiadosAN = FoliarConsultasDBSinEntity.LimpiarANSql_IncumplimientoCalidadFoliacion(datosCompletosNomina.Anio, datosCompletosNomina.An, condicionBancosParaSerPagomatico, datosCompletosNomina.Id_nom);
                   // int registrosLimpiadosTblPagos = FoliarConsultasDBSinEntity.LimpiarTblPagos_IncumplimientoCalidadFoliacion(datosCompletosNomina.Anio, Convert.ToInt32(datosCompletosNomina.Quincena), datosCompletosNomina.Id_nom, 2 /* Tipo de pago 2 por ser DISPERCION */);
                    AlertasAlFolearPagomaticosDTO alertaEncontradaLimpiezaDBF = FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(6 /* Numero para redireccionar a una limpiaza de la base con una condicion */ , datosCompletosNomina, new List<ResumenPersonalAFoliarDTO>() /* Este paramatro se envia vavio por que no se ocupa para la esta opcion */ , condicionBancosParaSerPagomatico);
                    if (alertaEncontradaLimpiezaDBF.IdAtencion != 200)
                    {
                        return alertaEncontradaLimpiezaDBF;
                 
                    }

                    alertaFracasoTotal.IdAtencion = 4;
                    alertaFracasoTotal.NumeroNomina = datosCompletosNomina.Nomina;
                    alertaFracasoTotal.NombreNomina = datosCompletosNomina.Coment;
                    alertaFracasoTotal.Detalle = registrosLimpiadosAN == alertaEncontradaLimpiezaDBF.NumeroRegistrosActualizados ? "OCURRIO UN ERROR EN LA FOLIACION POR ENDE NO SE AFECTO NINGUNA BASE, INTENTE DE NUEVO" : "ERROR EN LA FOLIACION, NO SE PUDIERON LIMPIEAR LOS CAMPOS AFECTADOS CORRECTAMENTE EN LAS BASES DBF Y SQL ";
                    alertaFracasoTotal.Solucion = "VUELVA A FOLIAR DE NUEVO";
                    alertaFracasoTotal.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                    alertaFracasoTotal.RegistrosFoliados = registrosFoliados / 3;
                    alertaFracasoTotal.UltimoFolioUsado = 0;
                    return alertaFracasoTotal;
                    //numeroRegistrosActualizados_AlPHA = 0;
                    //Advertencias.Add(nuevaAlerta);
                    //return Advertencias;
                }
            }

            //nunca deberia utilizarse estas sentencia al menos que la nomina que se intente foliar no contenga pagomaticos 
            return new AlertasAlFolearPagomaticosDTO();
        }

        //Esta opcion es solo para folear de nuevo un pagomatico que anteriormente ya se habia foleado y no toma en cuenta las suspenciones que se han hecho despues de haber foliado la base en SQl
        public static async Task<List<AlertasAlFolearPagomaticosDTO>> RefoliarFolearPagomaticoPorNominaaAsincrono(int IdNom, string Quincena)
        {
            List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();
            AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();
            var transaccion = new Transaccion();
          
            int numeroRegistrosActualizados_BaseDBF = 0;
            int numeroRegistrosActualizados_AlPHA = 0;
            
            int quincena = Convert.ToInt32(Quincena);
            int anioInterfaz = FoliarNegocios.ObtenerAnioDeQuincena(Quincena);
            string visitaAnioInterface = ObtenerCadenaAnioInterface(anioInterfaz);
           
            DatosCompletosBitacoraDTO datosCompletosNomina = ObtenerDatosCompletosBitacoraFILTRO(IdNom, visitaAnioInterface);

            List<Tbl_CuentasBancarias> cuentasBancosDisponibles = await obtenerCuentasBancariasVigentesPorNomina(datosCompletosNomina);
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, visitaAnioInterface);
            string condicionBancosParaSerPagomatico = consultasPagomaticos.ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);
            
            var repoActualizaRegistrosTblPago = new Repositorio<Tbl_Pagos>(transaccion);
            List<Tbl_Pagos> registrosActualizacionesObtenidos = repoActualizaRegistrosTblPago.ObtenerPorFiltro(x => x.Anio == anioInterfaz && x.Quincena == quincena && x.Id_nom == IdNom && x.IdCat_FormaPago_Nacimiento == 2 /*2 por que cuando nacio era un pagomatico*/).ToList();


           
            if (registrosActualizacionesObtenidos == null)
            {
                nuevaAlerta.IdAtencion = 3;
                Advertencias.Add(nuevaAlerta);
                return Advertencias;
            }
            else if (registrosActualizacionesObtenidos.Count() > 0)
            {
                List<ResumenPersonalAFoliarDTO> resumenPersonalARefoliar = new List<ResumenPersonalAFoliarDTO>();
                var repositorioBancos = new Repositorio<Tbl_CuentasBancarias>(transaccion);
                foreach (Tbl_Pagos nuevoPago in  registrosActualizacionesObtenidos ) 
                {
                    ResumenPersonalAFoliarDTO nuevoRegistro = new ResumenPersonalAFoliarDTO();

                    //si es nacio como tarjeta y se cambio a cheque (normalmente por suspencion) entonces se queda con los digitos del cheque pero si aun sigue siendo pagomatico es a 8 digitos el num_che
                    nuevoRegistro.NumChe = nuevoPago.IdCat_FormaPago_Nacimiento == 2 && nuevoPago.IdCat_FormaPago_Pagos == 1 ? Convert.ToString(nuevoPago.FolioCheque) : ObtenerNumeroFolio8DIgitosPagomaticoNUMCHE(nuevoPago.FolioCheque);
                    nuevoRegistro.CadenaNumEmpleado = Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos( nuevoPago.NumEmpleado );
                    nuevoRegistro.NumEmpleado = nuevoPago.NumEmpleado;

                    nuevoRegistro.RFC = nuevoPago.RfcEmpleado;
                    nuevoRegistro.Nombre = nuevoPago.NombreEmpleado;
                    nuevoRegistro.Liquido = nuevoPago.ImporteLiquido;

                    nuevoRegistro.IdBancoPagador = nuevoPago.IdTbl_CuentaBancaria_BancoPagador;

                    Tbl_CuentasBancarias CuentaEncontrada = repositorioBancos.Obtener(x => x.Id == nuevoRegistro.IdBancoPagador);
                    nuevoRegistro.BancoX = CuentaEncontrada.NombreBanco;
                    nuevoRegistro.CuentaX = CuentaEncontrada.Cuenta;
                    nuevoRegistro.Observa = nuevoPago.IdCat_FormaPago_Nacimiento == 2 && nuevoPago.IdCat_FormaPago_Pagos == 1 ? "TALON POR CHEQUE" : "TARJETA";


                    nuevoRegistro.Delegacion = nuevoPago.Delegacion;
                    nuevoRegistro.Partida = nuevoPago.Partida;

                    nuevoRegistro.FolioCFDI = nuevoPago.FolioCFDI !=  null ? Convert.ToInt32(nuevoPago.FolioCFDI) : 0;

                    if (Convert.ToBoolean(nuevoPago.EsPenA))
                    {
                        nuevoRegistro.NumBeneficiario = nuevoPago.NumBeneficiario;
                        nuevoRegistro.Nombre = nuevoPago.BeneficiarioPenA;
                    }
                    else {
                        nuevoRegistro.Nombre = nuevoPago.NombreEmpleado;
                    }

                    resumenPersonalARefoliar.Add(nuevoRegistro);
                }

                /********************************************************************************************************************************************************************/
                /***********************             Permite el acceso a una carpeta que se encuentra compartida dentro del servidor            ************************************/
                /***********************                                 Primer hilo de Ejecucion en secundo plano                               ************************************/
                /********************************************************************************************************************************************************************/
                Task<AlertasAlFolearPagomaticosDTO> alertaActualizarDBFenRed = Task.Run(() =>
                {
                    return FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(1 /* Se redireccionara a foliar una DBF */ , datosCompletosNomina, resumenPersonalARefoliar, "" /* no lleva una condicion ya que el update se hace con un filtro especifico*/);
                });



                /*******************************************************************************************************************************************************************************/
                /***************************************                    Actualiza la base cargada en SQL                *********************************************************************/
                /***********************                              SEGUNDO HILO DE EJECUCION EN SEGUNDO PLANO                                            ************************************/
                /*******************************************************************************************************************************************************************************/
                CancellationTokenSource mitokenOrigen = new CancellationTokenSource();
                CancellationToken cancelaToken = mitokenOrigen.Token;

                Task<int> task_resultadoRegitrosActualizados_InterfacesAlPHA = Task.Run(() =>
                {
                    return FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql_transaccionado_CANCELACIONHILO(resumenPersonalARefoliar, datosCompletosNomina, visitaAnioInterface, cancelaToken);
                });



                /*******************************************************************************************************************************************************************************/
                /***************************************                    ESPERA QUE EL PRIMER HILO SE HAYA FOLEADO CORRECTAMENTE             ************************************************/
                /*****************************      SI HUBO ALGUN ERROR SE CANCELA EL SEGUNDO HILO Y SI YA HABIA TERMINADO BLANQUEA LA BASE EN SQL          ************************************/
                /*******************************************************************************************************************************************************************************/
                AlertasAlFolearPagomaticosDTO alertaEncontrada = await alertaActualizarDBFenRed;

                if (alertaEncontrada.IdAtencion != 200 || alertaEncontrada.NumeroRegistrosActualizados != registrosActualizacionesObtenidos.Count)
                {
                    //SI HAY UN ERROR EN LA ACTUALIZACION DE DBF CREA UNA ADVERTENCIA, CANCELA EL SEGUNDO HILO Y POR SEGURIDAD HACE UN ROLBACK A LA BASE EN SQL 
                    Advertencias.Add(alertaEncontrada);
                    //Cancela la TARE DEL HILO 2
                    mitokenOrigen?.Cancel();

                    //HACER UN ROLLBACK A LA BASE EN SQL AN
                    FoliarConsultasDBSinEntity.LimpiarANSql_IncumplimientoCalidadFoliacion(datosCompletosNomina.Anio, datosCompletosNomina.An, condicionBancosParaSerPagomatico, datosCompletosNomina.Id_nom);
                    //LIMPIAR LA BASE DBF EN LA RUTA => SOLO LIMPIA LOS CAMPOS (NUM_CHE , CUENTA_x , BANCO_x , OBSERVA) A 
                    FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(6 /* Numero para redireccionar a una limpiaza de la base con una condicion */ , datosCompletosNomina, new List<ResumenPersonalAFoliarDTO>() /* Este paramatro se envia vavio por que no se ocupa para la esta opcion */ , condicionBancosParaSerPagomatico);
                    return Advertencias;
                }
                else if (alertaEncontrada.IdAtencion == 200)
                {
                    numeroRegistrosActualizados_BaseDBF = alertaEncontrada.NumeroRegistrosActualizados;
                }

                /*******************************************************************************************************************************************************************************/
                /***************************************                    ESPERA QUE EL SEGUNDO HILO SE HAYA FOLEADO CORRECTAMENTE             ***********************************************/
                /*******************************************************************************************************************************************************************************/
                numeroRegistrosActualizados_AlPHA = await task_resultadoRegitrosActualizados_InterfacesAlPHA;


                /*****************************************************************************************************************************************************************************************/
                /**********************************************                 Validacion del estandart de Foliacion                *********************************************************************/
                /*****************************************************************************************************************************************************************************************/
                int registrosFoliados = registrosActualizacionesObtenidos.Count + numeroRegistrosActualizados_AlPHA + numeroRegistrosActualizados_BaseDBF;
                if ((registrosActualizacionesObtenidos.Count + numeroRegistrosActualizados_AlPHA) == (numeroRegistrosActualizados_BaseDBF * 2))
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
                    //LIMPIA LA BASE EN SQL Y EN DBF
                    int registrosLimpiadosAN = FoliarConsultasDBSinEntity.LimpiarANSql_IncumplimientoCalidadFoliacion(datosCompletosNomina.Anio, datosCompletosNomina.An, condicionBancosParaSerPagomatico, datosCompletosNomina.Id_nom);
                    AlertasAlFolearPagomaticosDTO alertaEncontradaLimpiezaDBF = FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(6 /* Numero para redireccionar a una limpiaza de la base con una condicion */ , datosCompletosNomina, new List<ResumenPersonalAFoliarDTO>() /* Este paramatro se envia vavio por que no se ocupa para la esta opcion */ , condicionBancosParaSerPagomatico);
                   
                    nuevaAlerta.IdAtencion = 5;
                    nuevaAlerta.NumeroNomina = datosCompletosNomina.Nomina;
                    nuevaAlerta.NombreNomina = datosCompletosNomina.Coment;
                    nuevaAlerta.Detalle = registrosLimpiadosAN == alertaEncontradaLimpiezaDBF.NumeroRegistrosActualizados ? "OCURRIO UN ERROR EN LA REFOLIACION Y LAS BASES (DBF , SQL) NO SE VIERON AFECTADAS, INTENTE REFOLIAR DE NUEVO" : "ERROR EN LA REFOLIACION, NO SE PUDIERON LIMPIERAR LOS CAMPOS AFECTADOS CORRECTAMENTE DE LAS BASES ID_NOM : "+datosCompletosNomina.Id_nom ;
                    nuevaAlerta.Solucion = "VUELVA A REFOLIAR DE NUEVO";
                    nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                    nuevaAlerta.RegistrosFoliados = registrosFoliados / 3;
                    nuevaAlerta.UltimoFolioUsado = 0;
                    numeroRegistrosActualizados_AlPHA = 0;
                    Advertencias.Add(nuevaAlerta);
                    return Advertencias;
                }
            }
            return Advertencias;
        }



        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        //**************************************************************************************************************************************************************************//
        //**************************************************************************************************************************************************************************//
        //*************************************************************************     FOLEAR NOMINAS CON CHEQUES   ***************************************************************//
        //**************************************************************************************************************************************************************************//
        //**************************************************************************************************************************************************************************//
        //**************************************************************************************************************************************************************************//
        //**************************************************************************************************************************************************************************//

        public static async Task<List<AlertasAlFolearPagomaticosDTO>> FoliarChequesPorNomina_TIEMPO_DE_RESPUESTA_MEJORADO(FoliarFormasPagoDTO NuevaNominaFoliar, string Observa , List<FoliosAFoliarInventario> chequesVerificadosFoliar)
        {
            List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();
            AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();

            //Esta lista sirve para que si algo falla durante la foliacion y los registros tanto de la DBF , SQL y FCCBNet no sol los mismos se proceda a blanquear los registros afectados en cada base
            List<int> regitrosNumEmpleadoLimpiarCamposFoliacionDBF_SQL_FCCBNet = new List<int>();

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


            string visitaAnioInterface = ObtenerCadenaAnioInterface(NuevaNominaFoliar.AnioInterfaz);
            DatosCompletosBitacoraDTO datosNominaCompleto = ObtenerDatosCompletosBitacoraFILTRO(NuevaNominaFoliar.IdNomina, visitaAnioInterface);
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosNominaCompleto.An, visitaAnioInterface);
            string condicionBancos = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);

            string delegacionSeleccionada = filtroDelegaciones.Obtener(x => x.GrupoImpresionDelegacion == NuevaNominaFoliar.IdDelegacion).DelegacionesIncluidas.ToString();

            bool EsSindi = false;
            string consultaLista = "";
            if (NuevaNominaFoliar.IdGrupoFoliacion == 0)
            {
                if (NuevaNominaFoliar.Confianza > 0 && NuevaNominaFoliar.Sindicato == 0)
                {
                    //son de confianza
                    consultaLista = consultasCheques.ObtenerConsultaOrdenDeFoliacionPorDelegacion_GeneralDescentralizada(datosNominaCompleto.An, visitaAnioInterface, condicionBancos , delegacionSeleccionada, EsSindi);
                }
                else if (NuevaNominaFoliar.Confianza == 0 && NuevaNominaFoliar.Sindicato > 0)
                {
                    //Son sindicalizados
                    EsSindi = true;
                    consultaLista = consultasCheques.ObtenerConsultaOrdenDeFoliacionPorDelegacion_GeneralDescentralizada(datosNominaCompleto.An, visitaAnioInterface, condicionBancos , delegacionSeleccionada, EsSindi);
                }
            }
            else if (NuevaNominaFoliar.IdGrupoFoliacion == 1)
            {
                //consultaLista = ConsultasSQLOtrasNominasConCheques.ObtenerCadenaConsultaReportePDF_GenericOtrasNominas_PartidaCompleta(datosNominaCompleto.An, NuevaNominaFoliar.AnioInterfaz, delegacionSeleccionada, datosNominaCompleto.EsPenA);
                consultaLista = consultasCheques.ObtenerConsultaOrdenDeFoliacionPorDelegacion_OtrasNominasYPenA(datosNominaCompleto.An, visitaAnioInterface, condicionBancos, delegacionSeleccionada, datosNominaCompleto.EsPenA);
            }

            List<ResumenPersonalAFoliarDTO> resumenPersonalFoliar = new List<ResumenPersonalAFoliarDTO>();

            //Resolvio Problema "DE ANGELINA POR ANDAR EN EL CHISMECITO"
            //consultaLista = @"select    Substring(Inte.PARTIDA,2,5) , Inte.NUM, Inte.NOMBRE , per.deleg 'DELEGANTIGUA', Inte.NUM_CHE, Inte.LIQUIDO, Inte.FolioCFDI, Inte.BANCO_X, Inte.CUENTA_X , Inte.RFC   from Interfaces.dbo.ANIN2302000036 as Inte
            //                    inner join Nomina.dbo.nom_tbl_personal as per
            //                    on Inte.NUM = per.num
            //                    where per.baja = 0 and Inte.AZTECA = ''  and Inte.BANCOMER = ''  and Inte.BANORTE = ''  and Inte.HSBC = ''  and Inte.INVERLAT = ''  and Inte.SERFIN = ''  and Inte.TARJETA = ''
            //                    and Inte.deleg in ('00', '01', '02', '08', '09', '10', '12', '13', '14', '15', '16')
            //                    order by   IIF(isnull(Inte.NOM_ESP, 0) = 1, '1', '2'), per.DELEG, RTRIM(LTRIM(SUBSTRING(Inte.PARTIDA, 2, 8))), REPLACE(REPLACE(REPLACE(Inte.NOMBRE, '¥', 'Y'), 'Ð', 'Y'), '¾', 'Y')   collate Modern_Spanish_CI_AS
            //                    ";

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
               return FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql_transaccionado_Cheque(resumenPersonalFoliar, datosNominaCompleto, visitaAnioInterface);
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
               regitrosNumEmpleadoLimpiarCamposFoliacionDBF_SQL_FCCBNet.Add( nuevaPersona.NumEmpleado);

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
                    
                    nuevaActualizacion.FolioCheque = Convert.ToInt32( nuevaPersona.NumChe );
                    
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
                    nuevoPago.NombreEmpleado = nuevaPersona.Nombre.Replace("¾", "¥");
                    nuevoPago.IdCat_FormaPago_Nacimiento = 1; /*Por Nacer como cheque cheque*/
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
                        nuevoPago.FolioCheque = Convert.ToInt32( nuevaPersona.NumChe);
                        nuevoPago.FolioCFDI = nuevaPersona.FolioCFDI;

                        nuevoPago.IdTbl_CuentaBancaria_BancoPagador = nuevaPersona.IdBancoPagador;
                        nuevoPago.IdCat_FormaPago_Nacimiento = 1; //1 = cheque 
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
            /**********************************************            ESPERA AL HILO QUE ESTA ACTUALIZANDO EN SQL           *************************************************************************/
            /*****************************************************************************************************************************************************************************************/

            numeroRegistrosActualizados_AlPHA = await task_resultadoRegitrosActualizados_InterfacesAlPHA;

            //Registros Actualizados en DBF
            if (alertaEncontrada.IdAtencion == 200)
                numeroRegistrosActualizados_BaseDBF = alertaEncontrada.NumeroRegistrosActualizados;




            /*****************************************************************************************************************************************************************************************/
            /**********************************************            Total de registros Actualizados e Insertados          *************************************************************************/
            /*****************************************************************************************************************************************************************************************/
            int resultadoActualizacionOInsercionSegunElCaso = 0;

            if (numeroRegistrosActualizados_AlPHA == numeroRegistrosActualizados_BaseDBF)
            {
                if (actualizar)
                {
                    resultadoActualizacionOInsercionSegunElCaso = FoliarConsultasDBSinEntity.ActualizarTblPagos_DespuesDeFoliacion(ActualizarPagosEnTblPagos);
                }
                else
                {
                    resultadoActualizacionOInsercionSegunElCaso = repositorioTblPago.Agregar_EntidadesMasivamente(pagosNuevosAinsertar);
                }
            }
            else 
            {
                nuevaAlerta.IdAtencion = 4;
                nuevaAlerta.Id_Nom = Convert.ToString( datosNominaCompleto.Id_nom);
                nuevaAlerta.Detalle = "Los registros Foliados en DBF y SQL No coinciden : DBF = "+numeroRegistrosActualizados_BaseDBF + " y SQL = "+ numeroRegistrosActualizados_AlPHA ;
                nuevaAlerta.Solucion = "Intente Foliar de nuevo o contacte al desarrollador";
                Advertencias.Add(nuevaAlerta);
                return Advertencias;
            }


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

               List<int> foliosUsados = resumenPersonalFoliar.Select(x => Convert.ToInt32(x.NumChe)).ToList();
               nuevaAlerta.UltimoFolioUsado = foliosUsados.Max();

                Advertencias.Add(nuevaAlerta);
                transaccion.GuardarCambios();
                return Advertencias;
               
            }
            else
            {
                //Si entra en esta condicion es por que uno o mas registros no se foliaron y se necesita refoliar toda la nomina 
                transaccion.Dispose();

                DatosCompletosBitacoraDTO datosCompletosNominaNuevo = ObtenerDatosCompletosBitacoraFILTRO(datosNominaCompleto.Id_nom, visitaAnioInterface);

                string condicionParaSerChequePorDelegacion = "";
                if (NuevaNominaFoliar.IdGrupoFoliacion == 0)
                {
                    //APlica para GeneralYDESCE y en el campo EsSindise sabe si son de confianza = false o sindicalizados = true 
                    condicionParaSerChequePorDelegacion = consultasCheques.ObtenerCondicionParaLimpiarRegistrosChequePorDelegacion_GeneralDescentralizada(datosCompletosNominaNuevo.An, condicionBancos ,  delegacionSeleccionada, EsSindi );
                }
                else if (NuevaNominaFoliar.IdGrupoFoliacion == 1)
                {
                    condicionParaSerChequePorDelegacion = consultasCheques.ObtenerCondicionParaLimpiarRegistrosChequePorDelegacion_OtrasNominasYPenA(datosCompletosNominaNuevo.An , condicionBancos , delegacionSeleccionada , datosCompletosNominaNuevo.EsPenA);
                }

                //Encontrar empleados para solo los registros donde se encuentren esas bases 
                int registrosLimpiadosAN = FoliarConsultasDBSinEntity.LimpiarANSql_IncumplimientoCalidadFoliacion(datosCompletosNominaNuevo.Anio, datosCompletosNominaNuevo.An, condicionParaSerChequePorDelegacion, datosCompletosNominaNuevo.Id_nom);

                int registrosLimpiadosTblPagos = 0;
                //Punto 7 de la hoja de los issues
                if (resultadoActualizacionOInsercionSegunElCaso > 0) 
                {
                    LimpiarCamposChequesUsadosPorErrorDeFoliacion(chequesVerificadosFoliar);
                    registrosLimpiadosTblPagos = LimpiarCamposNumEmpleadosFCCBNetCheques(datosCompletosNominaNuevo.Anio, Convert.ToInt32(datosCompletosNominaNuevo.Quincena), datosCompletosNominaNuevo.Id_nom, regitrosNumEmpleadoLimpiarCamposFoliacionDBF_SQL_FCCBNet);
                }


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
                nuevaAlerta.Detalle = registrosLimpiadosAN == alertaEncontradaLimpiezaDBF.RegistrosFoliados?  "ERROR EN LA FOLIACION, NO SE PUDIERON LIMPIAR LOS CAMPOS AFECTADOS CORRECTAMENTE EN TBL_PAGOS, SQL O DBF" : "OCURRIO UN ERROR EN LA FOLIACION, NO SE CUMPLIO CON EL ESTANDAR DE FOLIACION SE HAN LIMPIADO LOS CAMPOS DESTINADOS EN DBF , SQL. TBLPagos NO FUE MODIFICADO " ;
                nuevaAlerta.Solucion = "VUELVA A FOLIAR DE NUEVO";
                nuevaAlerta.Id_Nom = Convert.ToString(datosNominaCompleto.Id_nom);
                nuevaAlerta.RegistrosFoliados = registrosFoliados / 3;
                List<int> foliosUsados = resumenPersonalFoliar.Select(x => Convert.ToInt32(x.NumChe)).ToList();
                nuevaAlerta.UltimoFolioUsado = foliosUsados.Max();  /*resumenPersonalFoliar.Max(x => x.NumChe);*/
               
                Advertencias.Add(nuevaAlerta);
                return Advertencias;


            }



            return Advertencias;
        }


        public static int LimpiarCamposNumEmpleadosFCCBNetCheques( int AnioInterface, int quincenaEnEnteros , int IdNom ,  List<int> regitrosNumEmpleadoLimpiarCamposFoliacionDBF_SQL_FCCBNet)
        {
            var transaccion = new Transaccion();
            var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
            List<Tbl_Pagos> LimpiarCamposPagos = repoTblPagos.ObtenerPorFiltro(x => x.Anio == AnioInterface && x.Quincena == quincenaEnEnteros && x.Id_nom == IdNom && x.IdCat_FormaPago_Nacimiento == 1 && x.IdCat_FormaPago_Pagos == 1 && x.TieneSeguimientoHistorico == null).Where(y => regitrosNumEmpleadoLimpiarCamposFoliacionDBF_SQL_FCCBNet.Contains(y.NumEmpleado)).ToList();
            int registrosModificados = 0;
            foreach (Tbl_Pagos limpiarNuevoPago in LimpiarCamposPagos) 
            {
                limpiarNuevoPago.IdTbl_CuentaBancaria_BancoPagador = 0;
                limpiarNuevoPago.FolioCheque = 0;
                limpiarNuevoPago.Integridad_HashMD5 = "Campos (IdTbl_CuentaBancaria_BancoPagador ,  FolioCheque , Integridad_HashMD5 )   limpiados por no cumplir el estandar de calidad en la foliacion";

                var elementoModificado = repoTblPagos.Modificar(limpiarNuevoPago);
                if (elementoModificado != null) 
                {
                    registrosModificados += 1;
                }
            }
            return registrosModificados;
        }

        public static void LimpiarCamposChequesUsadosPorErrorDeFoliacion(List<FoliosAFoliarInventario> limpiarChequesVerificadosFoliar)
        {
            var transaccion = new Transaccion();
            var repoTblInventarioCheque = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            var repoTblContenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repoTblInventario = new Repositorio<Tbl_Inventario>(transaccion);

            foreach (FoliosAFoliarInventario limpiarNuevoFolio in limpiarChequesVerificadosFoliar) 
            {
                Tbl_InventarioDetalle detalleEncontrado = repoTblInventarioCheque.Obtener(x => x.Id == limpiarNuevoFolio.Id);
                detalleEncontrado.IdIncidencia = null;
                detalleEncontrado.FechaIncidencia = null;
                repoTblInventarioCheque.Modificar(detalleEncontrado);
                

                Tbl_InventarioContenedores sumarFolioDelContenedor =repoTblContenedores.Obtener(x => x.Id == detalleEncontrado.IdContenedor);
                sumarFolioDelContenedor.FormasFoliadas -= 1;
                sumarFolioDelContenedor.FormasDisponiblesActuales += 1;
                repoTblContenedores.Modificar(sumarFolioDelContenedor);

                Tbl_Inventario sumarFolioDeInventario = repoTblInventario.Obtener(x => x.Id == detalleEncontrado.IdInventario);
                sumarFolioDeInventario.FormasDisponibles += 1;
                repoTblInventario.Modificar(sumarFolioDeInventario);

            }

        }






        #region METODOS PARA FORMAS DE PAGO => CHEQUES 
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /********************************************************    METODOS PARA FORMAS DE PAGO => CHEQUES      ***************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/

        public static string ObtenerNombreDelegacion(int IdDelegacion) 
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            return repositorio.Obtener(x => x.GrupoImpresionDelegacion == IdDelegacion).NombreComun;
        }

        //**********************************************************  DATOS PERSONALES DE UNA NOMINA POR DELEGACION O NOMINA PARA EL PDF DEL REPORTE        ***************************************//
        public static List<ResumenRevicionNominaPDFDTO> ObtenerDatosPersonalesDelegacionNominaGeneralDesce_ReporteCheque(int IdNomina, int AnioInterface, bool EsSindicalizado, int IdDelegacion)
        {

            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            IQueryable<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos();

            string visitaAnioInterface = ObtenerCadenaAnioInterface(AnioInterface);
            DatosCompletosBitacoraDTO datosCompletosNomina = ObtenerDatosCompletosBitacoraFILTRO(IdNomina, visitaAnioInterface);
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, visitaAnioInterface);
            string condicionBancos = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            string delegacionSeleccionada = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == IdDelegacion).FirstOrDefault().DelegacionesIncluidas;

            string consulta = consultasCheques.ObtenerConsultaDetallePersonalEnAN_GeneralDescentralizada(datosCompletosNomina.An, visitaAnioInterface , condicionBancos, delegacionSeleccionada, EsSindicalizado );
            //string consulta = ConsultasSQLSindicatoGeneralYDesc.ObtenerCadenaConsultaReportePDF_XSindicato(datosCompletosNomina.An, AnioInterface, delegacionesIncluidas, EsSindicalizado);

            return FoliarConsultasDBSinEntity.ObtenerResumenDatosComoSeEncuentraFolidoEnAN(consulta, datosCompletosNomina.Nomina);
        }

        public static List<ResumenRevicionNominaPDFDTO> ObtenerDatosPersonalesDelegacionOtrasNominas_ReporteCheque(int IdNomina, int AnioInterface, int IdDelegacion)
        {

            var transaccion = new Transaccion();
            var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            IQueryable<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos();

            string visitaAnioInterface = ObtenerCadenaAnioInterface(AnioInterface);
            DatosCompletosBitacoraDTO datosCompletosNomina = ObtenerDatosCompletosBitacoraFILTRO(IdNomina, visitaAnioInterface);
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, visitaAnioInterface);
            string condicionBancos = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            string delegacionesIncluidas = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == IdDelegacion).FirstOrDefault().DelegacionesIncluidas;
            string consulta = consultasCheques.ObtenerConsultaDetallePersonalEnAN_OtrasNominasYPenA(datosCompletosNomina.An, visitaAnioInterface, condicionBancos, delegacionesIncluidas , datosCompletosNomina.EsPenA);
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

            string visitaAnioInterface = ObtenerCadenaAnioInterface(AnioInterface);
            DatosCompletosBitacoraDTO datosCompletosNomina = ObtenerDatosCompletosBitacoraFILTRO(IdNomina, visitaAnioInterface);

            List<string> delegacionesIncluidas = filtrodelegaciones.Select(x => x.DelegacionesIncluidas).ToList();

            int Iterador = 0;
            List<bool> EsSindicato = new List<bool>() { false, true };

            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, visitaAnioInterface);
            string condicionBancos = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            foreach (bool EsSindi in EsSindicato)
            {

                foreach (string delegacion in delegacionesIncluidas)
                {
                    string consulta = consultasCheques.ObtenerConsultaDetallePersonalEnAN_GeneralDescentralizada(datosCompletosNomina.An, visitaAnioInterface, condicionBancos,  delegacion, EsSindi);
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

            string visitaAnioInterface = ObtenerCadenaAnioInterface(AnioInterface);
            DatosCompletosBitacoraDTO datosCompletosNomina = ObtenerDatosCompletosBitacoraFILTRO(IdNomina, visitaAnioInterface);

            List<string> delegacionesIncluidas = filtrodelegaciones.Select(x => x.DelegacionesIncluidas).ToList();
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, visitaAnioInterface);
            string condicionBancos = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            int Iterador = 0;
            foreach (string delegacion in delegacionesIncluidas)
            {
                //string consulta = ConsultasSQLOtrasNominasConCheques.ObtenerCadenaConsultaReportePDF_GenericOtrasNominas(datosCompletosNomina.An, AnioInterface, delegacion, datosCompletosNomina.EsPenA);

                string consulta = consultasCheques.ObtenerConsultaDetallePersonalEnAN_OtrasNominasYPenA(datosCompletosNomina.An, visitaAnioInterface, condicionBancos, delegacion, datosCompletosNomina.EsPenA);  /*  ObtenerCadenaConsultaReportePDF_GenericOtrasNominas(datosCompletosNomina.An, AnioInterface, delegacion, datosCompletosNomina.EsPenA);*/

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
        //**********    VERIFICA QUE EL ESTATUS DE LA NOMINA SELECCIONADA DE COMO SE ENCUENTRA EL AN SI ESTAN LLENOS SUS CAMPOS DE FOLIACION DE ACUERDO A LAS DELEGACIONES   *****//
        //************************************************************************************************************************************************************************//
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/

        public static AlertaDeNominasFoliadasPagomatico EstaFoliadaNominaSeleccionadaCHEQUE(int IdNom, int AnioInterface)
        {
            //IdEstaFoliada 
            //0 para decir que no esta foliada ni en SQL ni en FCCBNet     { False }
            //1 para los que estan foliadas  en SQL como en FCCBNetDB       { true  }
            //2 para los que no tienen pagomaticos  { else  }
            //3 para los AN que no se han importado de la dbf hacia SQL segun la bitacora  
            //4 LA BASE EN SQL ESTA FOLIADA POR ALGUNA RAZON, PERO NO HAY REGISTRO EN FCCBNetDB => VErificar con el desarrollador por que sucedio (SE resuelve limpiando la base de sql de AN)
            //5 LA BASE EN SQL NO FOLIADA POR ALGUNA RAZON, PERO SI HAY REGISTROS EN FCCBNetDB que indican que en algun momento fue foleada => VErificar con el desarrollador por que sucedio (En este caso debe solo actualizar los datos)

            List<AlertaDeNominasFoliadasPagomatico> AlertasEncontradas = new List<AlertaDeNominasFoliadasPagomatico>();

            string visitaAnioInterface = ObtenerCadenaAnioInterface(AnioInterface);
            DatosCompletosBitacoraDTO detalleNominaObtenido = ObtenerDatosCompletosBitacoraFILTRO(IdNom, visitaAnioInterface);
            int anioDeQuincena = ObtenerAnioDeQuincena(detalleNominaObtenido.Quincena);

            AlertaDeNominasFoliadasPagomatico nuevaAlerta = new AlertaDeNominasFoliadasPagomatico();

            if (detalleNominaObtenido.Importado)
            {
                List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(detalleNominaObtenido.An, visitaAnioInterface);


                /**/
                string condicionBancosParaSerPagomatico = consultasPagomaticos.ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);

                //Obtiene los registros totales de la nomina con la condicion de pagomaticos que se deberian de foliar
                string consultaRegistrosAFoliar = consultasPagomaticos.ObtenerRegistrosAFoliar_NominaPagomatico(visitaAnioInterface, detalleNominaObtenido.An, condicionBancosParaSerPagomatico);
                int registrosTotalesAFoliar = FoliarConsultasDBSinEntity.ObtenerRegistro_FoliacionPagomatico(consultaRegistrosAFoliar);

                //Obtine los registros que se encuentran foleados dentro de la nomina
                string consultaRegitrosFoliados = consultasPagomaticos.ObtenerRegitrosFoliados_NominaPagomatico(visitaAnioInterface, detalleNominaObtenido.An, condicionBancosParaSerPagomatico);
                int registrosTotalesFoliados = FoliarConsultasDBSinEntity.ObtenerRegistro_FoliacionPagomatico(consultaRegitrosFoliados);


                var (existenRegistrosEnFCCBNetDB, cantidadRegistrosFCCBNetDB) = ExistenRegistrosPagomaticosCantidadEnNominaSelecionadaEnFCCBNetDB(anioDeQuincena, detalleNominaObtenido.Quincena, detalleNominaObtenido.Id_nom);



                nuevaAlerta.NumeroRegistrosAFoliar = registrosTotalesAFoliar;
                if (registrosTotalesAFoliar > 0 && registrosTotalesFoliados == registrosTotalesAFoliar)
                {
                    //Esta Foliado tanto en la base de sql como en FCCBNetDB
                    if (existenRegistrosEnFCCBNetDB)
                    {
                        nuevaAlerta.IdEstaFoliada = 1;
                    }
                    else
                    {
                        nuevaAlerta.IdEstaFoliada = 4;
                    }

                }
                else if (registrosTotalesAFoliar == 0 && registrosTotalesFoliados == 0)
                {
                    //si no lee ningun registro es por que no cuenta con pagomaticos 
                    nuevaAlerta.IdEstaFoliada = 2;
                }
                else if (registrosTotalesAFoliar > 0 && registrosTotalesFoliados != registrosTotalesAFoliar)
                {

                    if (!existenRegistrosEnFCCBNetDB)
                    {
                        //No esta Foliado en la base sql y tampoco hay registros en FCCBNetDB (Todo es correcto y puede folear)
                        nuevaAlerta.IdEstaFoliada = 0;
                    }
                    else
                    {
                        //5 LA BASE EN SQL NO FOLIADA POR ALGUNA RAZON, PERO SI HAY REGISTROS EN FCCBNetDB => esta parte servira para en algun futuro poder refoliar la nomina con pagomaticos
                        //tomando en cuanta que cuando se vuelva a refoliar donde el campo observa != "TALON X CHEQUE"
                        nuevaAlerta.IdEstaFoliada = 5;
                    }

                }
            }
            else
            {
                nuevaAlerta.NumeroRegistrosAFoliar = 0;
                nuevaAlerta.IdEstaFoliada = 3;
            }

            nuevaAlerta.Id_Nom = detalleNominaObtenido.Id_nom;
            nuevaAlerta.NumeroNomina = detalleNominaObtenido.Nomina;
            nuevaAlerta.Adicional = detalleNominaObtenido.Adicional;
            nuevaAlerta.NombreNomina = detalleNominaObtenido.Coment;

            //AlertasEncontradas.Add(nuevaAlerta);
            //return AlertasEncontradas;
            return nuevaAlerta;
        }







        public static List<ResumenNominaChequeDTO> ObtenerResumenDetalleNomina_Cheques(int IdNomina, int AnioInterface)
        {
            List<ResumenNominaDTO> listaResumenNomina = new List<ResumenNominaDTO>();

            string visitaAnioInterface = ObtenerCadenaAnioInterface(AnioInterface);
            DatosCompletosBitacoraDTO datosCompletosNomina = ObtenerDatosCompletosBitacoraFILTRO(IdNomina, visitaAnioInterface);
            //var transaccion = new Transaccion();
            //var repositorio = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            //IQueryable<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repositorio.ObtenerTodos();
            List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, visitaAnioInterface);

            bool esNominaGeneraloDescentralizada = false;
            if (datosCompletosNomina.Nomina == "01" || datosCompletosNomina.Nomina == "02")
            {
                if (datosCompletosNomina.Subnomina.Equals("22") || datosCompletosNomina.Subnomina.Equals("29"))
                {
                    //son Partes Proporcionales de Aguinaldo (PPA) y Pagos por separacion (PSEP)
                    return ResumenCheques_OtrasNomina(datosCompletosNomina, AnioInterface, bancosContenidosEnAn);
                }
                else 
                {
                    return ResumenCheques_GeneralYDesce(datosCompletosNomina, AnioInterface, bancosContenidosEnAn);
                }
            }
            else if (datosCompletosNomina.Nomina == "08")
            {
                return ResumenCheques_OtrasNomina(datosCompletosNomina, AnioInterface, bancosContenidosEnAn);
            }
            else
            {
                //Nominas que no son de la general (01) o (02) decentralizada ni 
                return ResumenCheques_OtrasNomina(datosCompletosNomina, AnioInterface, bancosContenidosEnAn);
            }


        }


        public static List<ResumenNominaChequeDTO> ResumenCheques_GeneralYDesce( DatosCompletosBitacoraDTO datosCompletosNomina, int AnioInterface, List<string> bancosContenidosEnAn) 
        {
            //No incluye la parte proporcional ni mucho menos la parte del pago por separacion => eso se imprime en solo una tira de cheques asi que no importa si son sincicalizados o de confianza por ende el resumen cae en ResumenCheques_OtrasNomina
            var transaccion = new Transaccion();
            var repoFiltroDelegacionesIncluidas = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            IQueryable<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repoFiltroDelegacionesIncluidas.ObtenerTodos();

            string visitaAnioInterface = FoliarNegocios.ObtenerCadenaAnioInterface(AnioInterface);
            string condicionBancos = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);

            List<string> listaFiltroConsultaTotalesConfianzaSindicato = consultasCheques.ObtenerConsultasTotalPersonal_ConfianzaSindicaliza(datosCompletosNomina.An, AnioInterface, bancosContenidosEnAn);
            List<TotalRegistrosXDelegacionDTO> registrosTotalesXDelegacion = FoliarConsultasDBSinEntity.ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina(listaFiltroConsultaTotalesConfianzaSindicato, true);

            List<ResumenNominaChequeDTO> resumenNominaChequeGeneralDesce = new List<ResumenNominaChequeDTO>();
            int iterador = 0;
            var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
            int quincenaEnEnteros = Convert.ToInt32(datosCompletosNomina.Quincena);
            foreach (TotalRegistrosXDelegacionDTO nuevaDelegacion in registrosTotalesXDelegacion)
            {

                int grupoImpresionDelegacion = (Convert.ToInt32(nuevaDelegacion.Delegacion));
                string delegacionesIncluidas = repoFiltroDelegacionesIncluidas.Obtener(x => x.GrupoImpresionDelegacion == grupoImpresionDelegacion).DelegacionesIncluidas;

                //
                string consultaNumerosEmpleadosEnDelegacion = consultasCheques.ObtenerConsultaNumerosEmpleadosEnDelegacionGENERALYDESCE(datosCompletosNomina.An, visitaAnioInterface, condicionBancos, delegacionesIncluidas, nuevaDelegacion.Sindicato);
                List<int> numEmpleadosContenidosEnDelegacion = FoliarConsultasDBSinEntity.NumEmpleadosContenidosEnDelegacion_cheque(consultaNumerosEmpleadosEnDelegacion);
                List<Tbl_Pagos> pagosChequeEncontrados = repoTblPagos.ObtenerPorFiltro(x => x.Anio == datosCompletosNomina.Anio && x.Quincena == quincenaEnEnteros && x.IdCat_FormaPago_Nacimiento == 1 && x.IdCat_FormaPago_Pagos == 1 && x.Id_nom == datosCompletosNomina.Id_nom && x.FolioCheque != 0).Where(y => numEmpleadosContenidosEnDelegacion.Contains(y.NumEmpleado)).ToList();
                bool estaFoliadoTblPagos = nuevaDelegacion.Total == pagosChequeEncontrados.Count() ? true : false;


                string consultaTotalRegistrosXDelegacion = consultasCheques.ObtenerConsultaTotalRegistrosNoFoliadosxDelegacion_GeneralDescentralizada(datosCompletosNomina.An, visitaAnioInterface, condicionBancos , delegacionesIncluidas, nuevaDelegacion.Sindicato);
                bool estaFoliadoSQL = FoliarConsultasDBSinEntity.EstaFoliadacorrectamenteDelegacion_Cheque(consultaTotalRegistrosXDelegacion, nuevaDelegacion.Total);


                ResumenNominaChequeDTO nuevoResumen = new ResumenNominaChequeDTO();
                nuevoResumen.IdVirtual = ++iterador;
                nuevoResumen.IdDelegacion = Convert.ToInt32(nuevaDelegacion.Delegacion);
                nuevoResumen.GrupoFoliacion = 0; // El grupo de foliacion {0} es para los de la general y los decentralizados 
                nuevoResumen.Coment = datosCompletosNomina.Coment;
                nuevoResumen.IdNomina = datosCompletosNomina.Id_nom;
                /*OBTENER DELEGACION*/
                nuevoResumen.NombreDelegacion = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == nuevoResumen.IdDelegacion).FirstOrDefault().NombreComun;

                if (Convert.ToBoolean(nuevaDelegacion.Sindicato))
                {
                    nuevoResumen.Sindicato = nuevaDelegacion.Total;
                }
                else
                {
                    nuevoResumen.Confianza = nuevaDelegacion.Total;
                }


                if (estaFoliadoTblPagos && estaFoliadoSQL)
                {
                    nuevoResumen.EstaFoliadoCorrectamente = true;
                }
                else
                {
                    nuevoResumen.EstaFoliadoCorrectamente = false;
                }


                resumenNominaChequeGeneralDesce.Add(nuevoResumen);
            }

            return resumenNominaChequeGeneralDesce;

        }


        public static List<ResumenNominaChequeDTO> ResumenCheques_OtrasNomina(DatosCompletosBitacoraDTO datosCompletosNomina, int AnioInterface, List<string> bancosContenidosEnAn) 
        {
            var transaccion = new Transaccion();
            var repoFiltroDelegacionesIncluidas = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
            IQueryable<cat_FiltroGrupoImpresionDelegaciones> filtrodelegaciones = repoFiltroDelegacionesIncluidas.ObtenerTodos();

            string visitaAnioInterface = FoliarNegocios.ObtenerCadenaAnioInterface(AnioInterface);
            string condicionBancos = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
            
            List<string> listaFiltroConsultaTotalesOtrasNominas = consultasCheques.ObtenerConsultasTotalPersonal_OtrasNominasYPenA(datosCompletosNomina.An, visitaAnioInterface, condicionBancos );
            List<TotalRegistrosXDelegacionDTO> registrosTotalesXDelegacionOtrasNominas = FoliarConsultasDBSinEntity.ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina(listaFiltroConsultaTotalesOtrasNominas, false);

            List<ResumenNominaChequeDTO> resumenNominaChequeOtrasNominas = new List<ResumenNominaChequeDTO>();
            int iterador = 0;
            var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
            int quincenaEnEnteros = Convert.ToInt32(datosCompletosNomina.Quincena);
            foreach (TotalRegistrosXDelegacionDTO nuevaDelegacionRestoDeNominas in registrosTotalesXDelegacionOtrasNominas)
            {
                int grupoImpresionDelegacion = (Convert.ToInt32(nuevaDelegacionRestoDeNominas.Delegacion));
                string delegacionesIncluidas = repoFiltroDelegacionesIncluidas.Obtener(x => x.GrupoImpresionDelegacion == grupoImpresionDelegacion).DelegacionesIncluidas;
                
                //
                string consultaNumerosEmpleadosEnDelegacion = consultasCheques.ObtenerConsultaNumerosEmpleadosEnDelegacion(datosCompletosNomina.An, visitaAnioInterface, condicionBancos, delegacionesIncluidas);
                List<int> numEmpleadosContenidosEnDelegacion = FoliarConsultasDBSinEntity.NumEmpleadosContenidosEnDelegacion_cheque(consultaNumerosEmpleadosEnDelegacion);
                List<Tbl_Pagos> pagosChequeEncontrados = repoTblPagos.ObtenerPorFiltro(x => x.Anio == datosCompletosNomina.Anio && x.Quincena == quincenaEnEnteros && x.IdCat_FormaPago_Nacimiento == 1 && x.IdCat_FormaPago_Pagos == 1 && x.Id_nom == datosCompletosNomina.Id_nom && x.FolioCheque != 0).Where(y => numEmpleadosContenidosEnDelegacion.Contains(y.NumEmpleado)).ToList();
                bool estaFoliadoTblPagos = nuevaDelegacionRestoDeNominas.Total == pagosChequeEncontrados.Count() ? true : false; 

                //
                string consultaObtenida = consultasCheques.ObtenerConsultaTotalRegistrosNoFoliadosxDelegacion_OtrasNominasYPenA(datosCompletosNomina.An, visitaAnioInterface, condicionBancos,  delegacionesIncluidas);
                bool estaFoliadoSQL = FoliarConsultasDBSinEntity.EstaFoliadacorrectamenteDelegacion_Cheque(consultaObtenida, nuevaDelegacionRestoDeNominas.Total);

                ResumenNominaChequeDTO nuevoResumen = new ResumenNominaChequeDTO();              
                nuevoResumen.IdVirtual = ++iterador;
                nuevoResumen.IdDelegacion = Convert.ToInt32(nuevaDelegacionRestoDeNominas.Delegacion);
                nuevoResumen.GrupoFoliacion = 1; // El grupo de foliacion {1} es para todas las demas nominas que no sean General y Descentralizados 
                nuevoResumen.Coment = datosCompletosNomina.Coment;
                nuevoResumen.IdNomina = datosCompletosNomina.Id_nom;
                /*OBTENER DELEGACION*/
                nuevoResumen.NombreDelegacion = filtrodelegaciones.Where(x => x.GrupoImpresionDelegacion == nuevoResumen.IdDelegacion).FirstOrDefault().NombreComun;
                nuevoResumen.Otros = nuevaDelegacionRestoDeNominas.Total;
                if (estaFoliadoTblPagos && estaFoliadoSQL) 
                {
                    nuevoResumen.EstaFoliadoCorrectamente = true;
                }
                else 
                {
                    nuevoResumen.EstaFoliadoCorrectamente = false;
                }
                resumenNominaChequeOtrasNominas.Add(nuevoResumen);
            }

            return resumenNominaChequeOtrasNominas;
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
                consultaFinal = "select id ,  Anio, Id_nom, Nomina, Quincena , Delegacion, CASE EsPenA When 0 Then NombreEmpleado When 1 Then BeneficiarioPenA end 'NombreBeneficiarioCheque' , NumEmpleado , ImporteLiquido , FolioCheque , IdTbl_CuentaBancaria_BancoPagador , IdTbl_InventarioDetalle  FROM " + nombreDb + ".dbo.Tbl_Pagos where IdTbl_CuentaBancaria_BancoPagador = " + cuentaEncontrada.Id + "  and IdTbl_InventarioDetalle in ( " + consulta2 + " )  order by FolioCheque";
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





        #region METODOS PARA VERIFICAR SI LAS NOMINAS DE LA QUINCENA ESTAN FOLIADAS

        public static List<VerificarFoliacionNominasQuincenaDTO> VerificacionFoliacionNominasQuincena(string Quincena) 
        {
            List<VerificarFoliacionNominasQuincenaDTO> listaVerificacion = new List<VerificarFoliacionNominasQuincenaDTO>();
            int anioInterfasQuincena = ObtenerAnioDeQuincena(Quincena);
            int quincenaSeleccionada = Convert.ToInt32(Quincena); 
            string VisitaAnioInterfas = ObtenerCadenaAnioInterface(anioInterfasQuincena);

            List<DatosCompletosBitacoraDTO> nominasEnQuincena = ObtenerDatosCompletosBitacoraFILTRO(VisitaAnioInterfas, Quincena);
            int iterador = 0;
            foreach (DatosCompletosBitacoraDTO Nomina in nominasEnQuincena) 
            {
                VerificarFoliacionNominasQuincenaDTO nuevaVerificacion = new VerificarFoliacionNominasQuincenaDTO();
                nuevaVerificacion.Id = iterador += 1; 
                nuevaVerificacion.Id_Nom = Nomina.Id_nom;
                nuevaVerificacion.Comentario = Nomina.Coment;
                nuevaVerificacion.EstaImportado = Nomina.Importado;

                if (nuevaVerificacion.EstaImportado) 
                {
                    nuevaVerificacion.TotalRegistros = FoliarConsultasDBSinEntity.ObtenerTotalRegistrosEnANDeNomina(VisitaAnioInterfas, Nomina.An);

             
                    //REGISTROS TOTALES CONTENIDOS EN FCCBNetDB
                    Transaccion transaccion = new Transaccion();
                    var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
                    int totalRegistradosFCCBNet = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioInterfasQuincena && x.Quincena == quincenaSeleccionada && x.Id_nom == Nomina.Id_nom && x.FolioCheque != 0).Count();
                    

                    if (nuevaVerificacion.TotalRegistros == totalRegistradosFCCBNet)
                    {
                        nuevaVerificacion.EstaFoliadoCorrectamente = true;
                    }
                    else 
                    {
                        nuevaVerificacion.EstaFoliadoCorrectamente = false;
                        nuevaVerificacion.RegistrosNoFoliados = nuevaVerificacion.TotalRegistros - totalRegistradosFCCBNet;

                        
                        List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(Nomina.An, VisitaAnioInterfas);
                        //REGISTROS TOTALES CONTENIDOS EN AN
                        //Cheques
                        string condicionBancosCheques = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
                        int totalRegistrosCheques = FoliarConsultasDBSinEntity.ObtenerTotalRegistrosNoFoliadosSegunCondicion(VisitaAnioInterfas, Nomina.An, condicionBancosCheques);
                        //Pagomaticos
                        string condicionesBancosPagomatico = consultasPagomaticos.ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);
                        int totalRegistrosPagomaticos = FoliarConsultasDBSinEntity.ObtenerTotalRegistrosNoFoliadosSegunCondicion(VisitaAnioInterfas, Nomina.An, condicionesBancosPagomatico);

                        //REGISTROS CONTENIDOS EN FCCBNetDB
                        int pagomaticosRegistradosFCCBnetDB = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioInterfasQuincena && x.Quincena == quincenaSeleccionada && x.Id_nom == Nomina.Id_nom && x.IdCat_FormaPago_Nacimiento == 2 && x.FolioCheque != 0).Count();
                        int chequesRegistradosFCCBnetDB = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioInterfasQuincena && x.Quincena == quincenaSeleccionada && x.Id_nom == Nomina.Id_nom && x.IdCat_FormaPago_Nacimiento == 1 && x.FolioCheque != 0).Count();



                        nuevaVerificacion.Cheques = totalRegistrosCheques - chequesRegistradosFCCBnetDB;
                        nuevaVerificacion.Pagomaticos = totalRegistrosPagomaticos - pagomaticosRegistradosFCCBnetDB;
                    }

                }
               


                listaVerificacion.Add(nuevaVerificacion);
            }

            return listaVerificacion;
        }


        #endregion




        //**********************************************************************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************************************************************//
        //****************************************************      Recuper Folio de IdPago seleccionado  (Recresa el cheque a un estado inicial cuando sucede un error humano de foliacion)         ********************************//
        //**********************************************************************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************************************************************//
        //**********************************************************************************************************************************************************************************************************************//
        public static (string , int) RestaurarRangoDeFolios(int IdCuentaBancaria, int RangoInicial, int RangoFinal) 
        {
            string resultado = "ERROR";
            int totalChequesRestaurados = 0;
            
            List<FoliosARecuperarDTO> restaurarCheques = FoliarNegocios.BuscarFormasPagoCoincidentes(IdCuentaBancaria, RangoInicial, RangoFinal).OrderBy( x => x.NumEmpleado).ToList();

            var id_nonCotenidos = restaurarCheques.Select(x => new { x.Id_nom, x.Anio } ).Distinct().ToList();



            foreach (var nuevoidNom in id_nonCotenidos) 
            {
                string visitaAnioInterfas =  ObtenerCadenaAnioInterface(Convert.ToInt32(nuevoidNom.Anio));
                DatosCompletosBitacoraDTO nominaEncontrada = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraFILTRO(Convert.ToInt32(nuevoidNom.Id_nom), visitaAnioInterfas);

                List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(nominaEncontrada.An, visitaAnioInterfas);
                string condicionBancos = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);

                


                int foliosRecuperadosExitosamente = 0;

                //Solo funciona si un una nomina no es De pension Alimenticia
               
                string condicionParaLimpiarCampos = "";
                
                string numEmpleadosLimpiarCampos = "";


                /******************************************************************************************************************************************/
                //***************************    Crear metodo para limpiar una dbf con numero de empleado por lotes     ***********************************/
                int numeroInsertar = 100;
                int numeroAcumuladorskips = 0;
                int numeroLotes = Convert.ToInt32(restaurarCheques.Count() / numeroInsertar) + 1;

                List<int> numerosDeSkip = new List<int>();
                for (int i = 1; i <= numeroLotes; i++) 
                {
                    if (i == 1) 
                    {
                        numerosDeSkip.Add(0);
                    }
                    else 
                    {
                        numeroAcumuladorskips += numeroInsertar;
                        numerosDeSkip.Add(numeroAcumuladorskips);
                    }
                    //numerosDeSkip.Add(i);
                }

                AlertasAlFolearPagomaticosDTO alertaDbf = new AlertasAlFolearPagomaticosDTO();
                int numeroRegistroLimpiadoDBF = 0;
                string condicionParaLimpiarCamposDbF = "";
                foreach (int skip in numerosDeSkip.OrderBy(x => x) )
                {
                    
                    string numEmpleadosLimpiarCamposDbf = "";
                    /* Para que no se pierda la ruta del archivo copiando muchas vecez la ip del servidro */
                    DatosCompletosBitacoraDTO usarRutasNomina = FoliarConsultasDBSinEntity.ObtenerDatosCompletosBitacoraFILTRO(Convert.ToInt32(nuevoidNom.Id_nom), visitaAnioInterfas); 

                    List<string> nuevaLista = restaurarCheques.Skip(skip).Take(numeroInsertar).Select( x => x.NumEmpleado ).ToList();

                    foreach (string NuevoFolio in nuevaLista)
                    {
                        if (numEmpleadosLimpiarCamposDbf != "")
                        {
                            numEmpleadosLimpiarCamposDbf += " , '" + Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(Convert.ToInt32(NuevoFolio)) + "'  ";
                        }
                        else
                        {
                            numEmpleadosLimpiarCamposDbf += " '" + Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(Convert.ToInt32(NuevoFolio)) + "'  ";
                        }

                    }

                  //  condicionParaLimpiarCamposDbF = " INLIST (num,  " + numEmpleadosLimpiarCamposDbf + " )";
                    condicionParaLimpiarCamposDbF = " num IN ( " + numEmpleadosLimpiarCamposDbf + " ) and "+ condicionBancos + " ";

                    alertaDbf = FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(7, usarRutasNomina, null, condicionParaLimpiarCamposDbF);
                    if (alertaDbf.IdAtencion == 200)
                    {
                        numeroRegistroLimpiadoDBF += alertaDbf.NumeroRegistrosActualizados;
                    }
                    else
                    {
                        return (alertaDbf.Detalle, totalChequesRestaurados);
                    }

                }
                /******************************************************************************************************************************************/
                /******************************************************************************************************************************************/




                foreach (FoliosARecuperarDTO NuevoFolio in restaurarCheques)
                {
                    if (numEmpleadosLimpiarCampos != "")
                    {
                        numEmpleadosLimpiarCampos += " , '" + Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(Convert.ToInt32(NuevoFolio.NumEmpleado)) + "'  ";
                    }
                    else
                    {
                        numEmpleadosLimpiarCampos += " '" + Reposicion_SuspencionNegocios.ObtenerNumeroEmpleadoCincoDigitos(Convert.ToInt32(NuevoFolio.NumEmpleado)) + "'  ";
                    }

                }

               // condicionParaLimpiarCamposDbF = " INLIST (num,  " + numEmpleadosLimpiarCampos + " )";
                condicionParaLimpiarCampos = " num in ( " + numEmpleadosLimpiarCampos + " ) and "+condicionBancos+" ";


                if (numeroRegistroLimpiadoDBF > 1 )
                {
                
                    /* Limpiar datos de foliacion del AN de en SQL */
                    int registroAnLimpiados = FoliarConsultasDBSinEntity.LimpiarCamposPorRecuperacionDeFolios(visitaAnioInterfas, nominaEncontrada.An, condicionParaLimpiarCampos, nominaEncontrada.Id_nom);

                    if (registroAnLimpiados >= 1)
                    {
                        int idnomSeleccionado = Convert.ToInt32(nuevoidNom.Id_nom);
                        int anioSeleccionado = Convert.ToInt32(nuevoidNom.Anio);


                        /**************************************************************************************************************************************************************************/
                        /**********************************************         ACTUALIZAR Tbl_InventarioDetalle        ***************************************************************************/
                        /**************************************************************************************************************************************************************************/
                        List<int> idsTbl_InventarioDetalle = restaurarCheques.Where(x => x.Id_nom == nuevoidNom.Id_nom && x.Anio == nuevoidNom.Anio).Select(x => x.IdTbl_InventarioDetalle).ToList();
                        string limpiarDetallesrIds = "";
                        foreach (int nuevoDetalleId in idsTbl_InventarioDetalle)
                        {
                            if (limpiarDetallesrIds == "")
                            {
                                limpiarDetallesrIds = "" + nuevoDetalleId;
                            }
                            else
                            {
                                limpiarDetallesrIds += " , " + nuevoDetalleId + "";
                            }

                        }
                        List<int> contenedoresEncontrados = DAP.Foliacion.Datos.TablasAfectadasFCCBNetDB.Tbl_InventarioContenedores_DbSinEntity.ObtenerContenedoresEnRecuperarFolios(limpiarDetallesrIds);

                        Transaccion transaccion = new Transaccion();
                        var repoBanco = new Repositorio<Tbl_CuentasBancarias>(transaccion);
                        int idInventario = Convert.ToInt32( repoBanco.Obtener(x => x.Id == IdCuentaBancaria).IdInventario ); 
                        var repoInventario = new Repositorio<Tbl_Inventario>(transaccion);
                        

                        int TotalRegistroActulizadoTBLDetalles = 0;
                        foreach (int nuevoContenedor in contenedoresEncontrados)
                        {
                            int registrosLimpiadosTBLInventarioDetalle = DAP.Foliacion.Datos.TablasAfectadasFCCBNetDB.Tbl_InventarioDetalle_DbSinEntity.LimpiarCamposTbl_InventarioDetalleRecuperacionDeFolios(idnomSeleccionado, anioSeleccionado, limpiarDetallesrIds, nuevoContenedor);
                            DAP.Foliacion.Datos.TablasAfectadasFCCBNetDB.Tbl_InventarioContenedores_DbSinEntity.ActualizarFormasDisponiblesXContenedor(registrosLimpiadosTBLInventarioDetalle, nuevoContenedor);
                            //actualizarInventario.FormasDisponibles += registrosLimpiadosTBLInventarioDetalle;
                            
                            TotalRegistroActulizadoTBLDetalles += registrosLimpiadosTBLInventarioDetalle;
                        }
                        /************************************************************************************************************************************/
                        /*******************************    Guardar Formas disponibles en el inventario    **************************************************/
                        Tbl_Inventario actualizarInventario = repoInventario.Obtener(x => x.Id == idInventario);
                        actualizarInventario.FormasDisponibles += TotalRegistroActulizadoTBLDetalles;
                        repoInventario.Modificar(actualizarInventario);



                        /**********************************************************************************************************************************************************************/
                        /**********************************************         ACTUALIZAR TBL_PAGOS        ***********************************************************************************/
                        /**********************************************************************************************************************************************************************/
                        List<int> idsTbl_Pagos = restaurarCheques.Where(x => x.Id_nom == nuevoidNom.Id_nom && x.Anio == nuevoidNom.Anio).Select(x => x.IdPago).ToList();
                        string limpiarTbl_PagosIds = "";
                        foreach (int nuevoPagoId in idsTbl_Pagos)
                        {
                            if (limpiarTbl_PagosIds == "")
                            {
                                limpiarTbl_PagosIds = "" + nuevoPagoId;
                            }
                            else
                            {
                                limpiarTbl_PagosIds += " , " + nuevoPagoId + "";
                            }

                        }
                        int registrosLimpiadosTBLPagos = DAP.Foliacion.Datos.TablasAfectadasFCCBNetDB.Tbl_Pagos_DbSinEntity.LimpiarCamposTBLPagosRecuperacionDeFolios(idnomSeleccionado, anioSeleccionado, limpiarTbl_PagosIds);

                        if (registrosLimpiadosTBLPagos == TotalRegistroActulizadoTBLDetalles)
                        {
                            resultado = "CORRECTO";
                            totalChequesRestaurados = registrosLimpiadosTBLPagos;

                        }
                    }

                }
                
            }
            return (resultado  , totalChequesRestaurados);
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

            string visitaAnioInterface = ObtenerCadenaAnioInterface(pagoEncontrado.Anio);
            DatosCompletosBitacoraDTO datosCompletosNomina = ObtenerDatosCompletosBitacoraFILTRO(pagoEncontrado.Id_nom, visitaAnioInterface);
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
                int registroAnLimpiado = FoliarConsultasDBSinEntity.LimpiarUnRegitroCamposFoliacionAN(datosCompletosNomina.Anio, datosCompletosNomina.An, numeroEmpleado5Digitos, pagoEncontrado.ImporteLiquido, datosCompletosNomina.EsPenA, pagoEncontrado.NumBeneficiario);

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




        #region AJUSTA CHEQUES INHABILITADOS DESPUES DE UNA CORRECTA FOLIACION
        // VERIFICA SI EL GRUPO QUE SE SELECCION DE LA NOMINA SE ENCUENTRA FOLIADO CORRECTAMENTE, SINO NO PODRA ACCEDER A UN AJUSTE
        public static async Task<List<AlertasAlFolearPagomaticosDTO>> RealizarAjusteFoliacion(FoliarFormasPagoDTO  AjustarDelegacionNomina)
        {

            List<AlertasAlFolearPagomaticosDTO> Advertencias = new List<AlertasAlFolearPagomaticosDTO>();
            AlertasAlFolearPagomaticosDTO nuevaAlerta = new AlertasAlFolearPagomaticosDTO();

            /***    SE LOCALIZAN EL NUMERO TOTAL DE CHEQUES QUE SE VAN A UTILIZAR    ***/
            int totalFolios = 0;

            if (AjustarDelegacionNomina.IdGrupoFoliacion == 0)
            {
                totalFolios = AjustarDelegacionNomina.Confianza > 0 ? AjustarDelegacionNomina.Confianza : AjustarDelegacionNomina.Sindicato;
            }
            else 
            {
                totalFolios = AjustarDelegacionNomina.Otros;
            }



            
            if (AjustarDelegacionNomina.Inhabilitado) 
            {
                //obtiene los numero de folios como deben de encontrarse en la tabla inventarios
                List<int> foliosUtilizados = new List<int>();
                int iterador = 0;
                while (iterador <=  totalFolios)
                {
                    int nuevoFolio = AjustarDelegacionNomina.RangoInicial + iterador++;
                    foliosUtilizados.Add(nuevoFolio);
                }

                /***    SE OBTIENEN LOS FOLIOS A RECUPERAR  ***/
                List<int> foliosARecuperar = foliosUtilizados.Where(x => x >= AjustarDelegacionNomina.RangoInhabilitadoInicial).OrderBy(x => x).ToList();

                //int a = AjustarDelegacionNomina.IdBancoPagador;
                int cantidadTotalAjustar = foliosARecuperar.Count();
                int nuevoInicioRangoFoliacion = foliosARecuperar.Min();
               // int c = foliosARecuperar.Max();



                /***    SE RECUPERAN LOS FOLIOS APARTIR DEL FOLIO INICIAL DE INHABILITACION  ***/
                //List<int> listaFoliosInicialmenteUsados = FoliarConsultasDBSinEntity.ObtenerListaDefolios(AjustarDelegacionNomina.RangoInicial, totalFolios, AjustarDelegacionNomina.Inhabilitado, AjustarDelegacionNomina.RangoInhabilitadoInicial , AjustarDelegacionNomina.RangoInhabilitadoFinal );

                ///***    LISTA DE FOLIOS QUE SE USARAN PARA EL NUEVO AJUSTE DESCONTANDO LOS INHABILITADOS ***/
                //List<int> listaFoliosFinalUsar = listaFoliosInicialmenteUsados.Where(x => x > nuevoInicioRangoFoliacion).ToList();




                /***    SE RECUPERAN LOS FOLIOS APARTIR DEL FOLIO INICIAL DE INHABILITACION  ***/
                var (resultadoObtenido, totalChequesRecuperados) = RestaurarRangoDeFolios(AjustarDelegacionNomina.IdBancoPagador, foliosARecuperar.Min(), foliosARecuperar.Max());

                /***    SE VERIFICAN QUE EFECTIVAMENTE TODOS LOS FOLIOS DE CHEQUES SE HAYAN RECUPERADO DE FORMA EXITOSA     ***/
                List<FoliosAFoliarInventario> chequesVerificadosFoliar = FoliarNegocios.verificarDisponibilidadFoliosEnInventarioDetalle(AjustarDelegacionNomina.IdBancoPagador, nuevoInicioRangoFoliacion, cantidadTotalAjustar, AjustarDelegacionNomina.Inhabilitado, AjustarDelegacionNomina.RangoInhabilitadoInicial, AjustarDelegacionNomina.RangoInhabilitadoFinal).ToList();
                int cantidadFoliosNoDisponibles = chequesVerificadosFoliar.Where(y => y.Incidencia != "").Count();
                //var foliosNoDisponibles = chequesVerificadosFoliar.Where(y => y.Incidencia != "").Select( x => x.Folio );
                /***********        poner un error que no se recuperaron adecuaqdamente algunos folios        ***********/

                if (cantidadFoliosNoDisponibles == 0)
                {
                    /** Si los folios fueron restaurados correctamente **/
                    string visitaAnioInterface = ObtenerCadenaAnioInterface(AjustarDelegacionNomina.AnioInterfaz);
                    DatosCompletosBitacoraDTO datosCompletosNomina = ObtenerDatosCompletosBitacoraFILTRO(AjustarDelegacionNomina.IdNomina, visitaAnioInterface);

                    List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(datosCompletosNomina.An, visitaAnioInterface);
                    string condicionBancos = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);

                    var transaccion = new Transaccion();
                    var repoFiltroDelegacionesIncluidas = new Repositorio<cat_FiltroGrupoImpresionDelegaciones>(transaccion);
                    var repoCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);
                    //OBTENIENDO EL FILTRO DE LA DELEGACIÓN QUE EL USUARIO SELECCIONO
                    string delegacionSeleccionada = repoFiltroDelegacionesIncluidas.Obtener(x => x.GrupoImpresionDelegacion == AjustarDelegacionNomina.IdDelegacion).DelegacionesIncluidas.ToString();

                    //SEGÚN EL GRUPO DE IMPRESIÓN VA A BUSCAR A LOS REGISTROS QUE SE LIMPIARON CON LA RECUPERACIÓN DE FOLIOS Y LOS INDEXA PARA SU NUEVA FOLIACIÓN   
                    bool EsSindi = false;
                    string consultaLista = "";
                    if (AjustarDelegacionNomina.IdGrupoFoliacion == 0)
                    {
                        if (AjustarDelegacionNomina.Confianza > 0 && AjustarDelegacionNomina.Sindicato == 0)
                        {
                            //son de confianza
                            consultaLista = consultasCheques.ObtenerConsultaParaAJuste_GeneralDescentralizada(datosCompletosNomina.An, visitaAnioInterface, condicionBancos, delegacionSeleccionada, EsSindi);
                        }
                        else if (AjustarDelegacionNomina.Confianza == 0 && AjustarDelegacionNomina.Sindicato > 0)
                        {
                            //Son sindicalizados
                            EsSindi = true;
                            consultaLista = consultasCheques.ObtenerConsultaParaAJuste_GeneralDescentralizada(datosCompletosNomina.An, visitaAnioInterface, condicionBancos, delegacionSeleccionada, EsSindi);
                        }
                    }
                    else if (AjustarDelegacionNomina.IdGrupoFoliacion == 1)
                    {
                        //consultaLista = ConsultasSQLOtrasNominasConCheques.ObtenerCadenaConsultaReportePDF_GenericOtrasNominas_PartidaCompleta(datosNominaCompleto.An, NuevaNominaFoliar.AnioInterfaz, delegacionSeleccionada, datosNominaCompleto.EsPenA);
                        consultaLista = consultasCheques.ObtenerConsultaParaAJuste_OtrasNominasYPenA(datosCompletosNomina.An, visitaAnioInterface, condicionBancos, delegacionSeleccionada, datosCompletosNomina.EsPenA);
                    }

                    List<ResumenPersonalAFoliarDTO> resumenPersonalFoliar = new List<ResumenPersonalAFoliarDTO>();
                    resumenPersonalFoliar = FoliarConsultasDBSinEntity.ObtenerResumenDatosFormasDePagoFoliar(datosCompletosNomina.EsPenA, "CHEQUE", consultaLista, repoCuentaBancaria.Obtener(x => x.Id == AjustarDelegacionNomina.IdBancoPagador), nuevoInicioRangoFoliacion, AjustarDelegacionNomina.Inhabilitado, AjustarDelegacionNomina.RangoInhabilitadoInicial, AjustarDelegacionNomina.RangoInhabilitadoFinal);

                    /***    VALIDA QUE  EL RESUMEN DE PERSONAS AJUSTAR SEA LA MISMA CANTIDAD DE LAS QUE SE RECUPERARON SUS CHEQUES   ***/
                    if (resumenPersonalFoliar.Count() == cantidadTotalAjustar)
                    {
                        /**********************************************************************************/
                        /***    INICIA EL PROCESESO PARA FOLIAR LAS PERSONAS A LAS QUE SE DEBEN AJUSTAR ***/
                        /**********************************************************************************/

                        int numeroRegistrosActualizados_BaseDBF = 0;
                        int numeroRegistrosActualizados_AlPHA = 0;
                        int registrosInsertadosOActualizados_Foliacion = 0;

                        /********************************************************************************************************************************************************************/
                        /***********************             Permite el acceso a una carpeta que se encuentra compartida dentro del servidor            ************************************/
                        /********************************************************************************************************************************************************************/
                        AlertasAlFolearPagomaticosDTO alertaEncontrada = FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(1 /* Se redireccionara a foliar una DBF */ , datosCompletosNomina, resumenPersonalFoliar, "" /* no lleva una condicion ya que el update se hace con un filtro especifico*/);
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
                            return FoliarConsultasDBSinEntity.ActualizarBaseNominaEnSql_transaccionado_Cheque(resumenPersonalFoliar, datosCompletosNomina, visitaAnioInterface);
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
                        var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);
                        var repositorioTblPago = new Repositorio<Tbl_Pagos>(transaccion);
                        var repositorioInventarioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
                        var repositorioContenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);

                        foreach (ResumenPersonalAFoliarDTO nuevaPersona in resumenPersonalFoliar)
                        {
                            Tbl_Pagos pagoAmodificar = null;

                            if (datosCompletosNomina.EsPenA)
                            {
                                pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == AjustarDelegacionNomina.AnioInterfaz && x.Id_nom == datosCompletosNomina.Id_nom && x.IdCat_FormaPago_Pagos == 1 /*Por ser cheque*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido && x.NumBeneficiario == nuevaPersona.NumBeneficiario);
                            }
                            else
                            {
                                pagoAmodificar = repositorioTblPago.Obtener(x => x.Anio == AjustarDelegacionNomina.AnioInterfaz && x.Id_nom == datosCompletosNomina.Id_nom && x.IdCat_FormaPago_Pagos == 1 /*Por ser cheuqe*/  && x.NumEmpleado == nuevaPersona.NumEmpleado && x.ImporteLiquido == nuevaPersona.Liquido);
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
                                nuevaActualizacion.FolioCheque = Convert.ToInt32(nuevaPersona.NumChe);
                                string cadenaDeIntegridad = datosCompletosNomina.Id_nom + " || " + datosCompletosNomina.Nomina + " || " + datosCompletosNomina.Quincena + " || " + nuevaPersona.CadenaNumEmpleado + " || " + nuevaPersona.Liquido + " || " + nuevaPersona.NumChe + " || " + nuevaPersona.BancoX + " || " + nuevaPersona.CuentaX + " || " + nuevaPersona.NumBeneficiario;
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
                                repositorioInventarioDetalle.Modificar(folioEnInventarioEncontrado);


                                Tbl_InventarioContenedores descontarFolioDelContenedor = repositorioContenedores.Obtener(x => x.Id == folioEnInventarioEncontrado.IdContenedor);
                                descontarFolioDelContenedor.FormasFoliadas += 1;
                                descontarFolioDelContenedor.FormasDisponiblesActuales -= 1;
                                repositorioContenedores.Modificar(descontarFolioDelContenedor);

                                Tbl_Inventario descontarFolioDeInventario = repositorioInventario.Obtener(x => x.Id == descontarFolioDelContenedor.IdInventario);
                                descontarFolioDeInventario.FormasDisponibles -= 1;
                                descontarFolioDeInventario.UltimoFolioUtilizado = Convert.ToString(folioEnInventarioEncontrado.NumFolio);
                                repositorioInventario.Modificar(descontarFolioDeInventario);

                                registrosInsertadosOActualizados_Foliacion++;
                            }

                        }





                        /*****************************************************************************************************************************************************************************************/
                        /**********************************************            ESPERA AL HILO QUE ESTA ACTUALIZANDO EN SQL           *************************************************************************/
                        /*****************************************************************************************************************************************************************************************/

                        numeroRegistrosActualizados_AlPHA = await task_resultadoRegitrosActualizados_InterfacesAlPHA;

                        /*****************************************************************************************************************************************************************************************/
                        /**********************************************         VALIDA QUE LA FOLIACION EN DBF HAYA SIDO LA CORRECTA      ************************************************************************/
                        /*****************************************************************************************************************************************************************************************/
                        if (alertaEncontrada.IdAtencion == 200)
                            numeroRegistrosActualizados_BaseDBF = alertaEncontrada.NumeroRegistrosActualizados;




                        /*****************************************************************************************************************************************************************************************/
                        /**********************************************            TOTAL REGISTROS ACTUALIZADOS EN LA DB CENTRALIZADA DE FCCBNetDB          ******************************************************/
                        /*****************************************************************************************************************************************************************************************/
                        int resultadoActualizacionFCCBNetBD = 0;
                        if (numeroRegistrosActualizados_AlPHA == numeroRegistrosActualizados_BaseDBF)
                        {
                            if (actualizar)
                            {
                                resultadoActualizacionFCCBNetBD = FoliarConsultasDBSinEntity.ActualizarTblPagos_DespuesDeFoliacion(ActualizarPagosEnTblPagos);
                            }
                        }
                        else
                        {
                            nuevaAlerta.IdAtencion = 4;
                            nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                            nuevaAlerta.Detalle = "Los registros Foliados en DBF y SQL No coinciden : DBF = " + numeroRegistrosActualizados_BaseDBF + " y SQL = " + numeroRegistrosActualizados_AlPHA;
                            nuevaAlerta.Solucion = "Intente Foliar de nuevo o contacte al desarrollador";
                            Advertencias.Add(nuevaAlerta);
                            return Advertencias;
                        }


                        /*****************************************************************************************************************************************************************************************/
                        /**********************************************                 Validacion del estandart de Foliacion                *********************************************************************/
                        /*****************************************************************************************************************************************************************************************/
                        if (resultadoActualizacionFCCBNetBD == numeroRegistrosActualizados_AlPHA)
                        {
                            nuevaAlerta.IdAtencion = 0;
                            nuevaAlerta.NumeroNomina = datosCompletosNomina.Nomina;
                            nuevaAlerta.NombreNomina = datosCompletosNomina.Coment;
                            nuevaAlerta.Detalle = "";
                            nuevaAlerta.Solucion = "";
                            nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                            nuevaAlerta.RegistrosFoliados = resultadoActualizacionFCCBNetBD;

                            List<int> foliosUsados = resumenPersonalFoliar.Select(x => Convert.ToInt32(x.NumChe)).ToList();
                            nuevaAlerta.UltimoFolioUsado = foliosUsados.Max();

                            Advertencias.Add(nuevaAlerta);
                            //transaccion.GuardarCambios();
                            return Advertencias;
                        }
                        else
                        {
                            /*******************************************************************************************************************************************************************************************/
                            /**********************************************                 LIMPIAR LOS CAMPOS DE LAS PERSONAS AFECTADAS                ****************************************************************/
                            /*******************************************************************************************************************************************************************************************/

                            //Si entra en esta condicion es por que uno o mas registros no se foliaron y se necesita refoliar toda la nomina 
                            transaccion.Dispose();

                            //Encontrar empleados para solo los registros donde se encuentren esas bases 
                            int registrosLimpiadosAN = FoliarConsultasDBSinEntity.LimpiarANSql_IncumplimientoAjuste(resumenPersonalFoliar, visitaAnioInterface, datosCompletosNomina.An);


                            LimpiarCamposChequesUsadosPorErrorDeFoliacion(chequesVerificadosFoliar);
                            List<int> NumEmpleadosLimpiarAjusteFCCBNetDB = resumenPersonalFoliar.Select(x => x.NumEmpleado).ToList();
                            int registrosLimpiadosTblPagos = LimpiarCamposNumEmpleadosFCCBNetCheques(datosCompletosNomina.Anio, Convert.ToInt32(datosCompletosNomina.Quincena), datosCompletosNomina.Id_nom, NumEmpleadosLimpiarAjusteFCCBNetDB);

                            List<string> NumEmpleadosLimpiarAjuste = resumenPersonalFoliar.Select(x => x.CadenaNumEmpleado).ToList();
                            string condicionNumEmpleadosLimpiar = consultasCheques.ConvertirListaNumEmpledosEnCondicionParaLimpiarAjuste(NumEmpleadosLimpiarAjuste);
                            List<ResumenPersonalAFoliarDTO> resumenPersonalVacio = new List<ResumenPersonalAFoliarDTO>();
                            AlertasAlFolearPagomaticosDTO alertaEncontradaLimpiezaDBF = FolearDBFEnServerNegocios.BaseDBF_CruceroParaConexionServer47DBF(5 /* Redireccionar a una limpieza de los num en la condicion */ , datosCompletosNomina, resumenPersonalVacio /*No se necesita esta dato*/, condicionNumEmpleadosLimpiar);
                            if (alertaEncontradaLimpiezaDBF.IdAtencion != 200)
                            {
                                Advertencias.Add(alertaEncontradaLimpiezaDBF);
                                return Advertencias;
                            }





                            nuevaAlerta.IdAtencion = 4;
                            nuevaAlerta.NumeroNomina = datosCompletosNomina.Nomina;
                            nuevaAlerta.NombreNomina = datosCompletosNomina.Coment;
                            nuevaAlerta.Detalle = registrosLimpiadosAN == alertaEncontradaLimpiezaDBF.RegistrosFoliados ? "ERROR EN LA FOLIACION, NO SE PUDIERON LIMPIAR LOS CAMPOS AFECTADOS CORRECTAMENTE EN TBL_PAGOS, SQL O DBF" : "OCURRIO UN ERROR EN LA FOLIACION, NO SE CUMPLIO CON EL ESTANDAR DE FOLIACION SE HAN LIMPIADO LOS CAMPOS DESTINADOS EN DBF , SQL. TBLPagos NO FUE MODIFICADO ";
                            nuevaAlerta.Solucion = "VUELVA A REALIZAR EL AJUSTE DE NUEVO";
                            nuevaAlerta.Id_Nom = Convert.ToString(datosCompletosNomina.Id_nom);
                            nuevaAlerta.RegistrosFoliados = NumEmpleadosLimpiarAjusteFCCBNetDB.Count;
                            List<int> foliosUsados = resumenPersonalFoliar.Select(x => Convert.ToInt32(x.NumChe)).ToList();
                            nuevaAlerta.UltimoFolioUsado = foliosUsados.Max();  /*resumenPersonalFoliar.Max(x => x.NumChe);*/

                            Advertencias.Add(nuevaAlerta);
                            return Advertencias;


                        }
                    }
                }
                else 
                {
                    /***    EL TOTAL DE FOLIOS RECUPERADOS  NO ES EL MISMO QUE LOS QUE DEBERIAN RECUPERAR  POR ENDE HAY ALGUNOS FOLIOS QUE AUN TIENEN ALGUNA INCIDENCIA Y NO VA A DEJAR USARALOS EL SISTEMA      ***/

                    List<int> foliosNoDisponibles = chequesVerificadosFoliar.Where(y => y.Incidencia != "").Select(x => x.Folio).ToList();
                    string comentarioFoliosNoDisponibles ="" ;
                    foreach (int nuevoFolioNDis in foliosNoDisponibles) 
                    {
                        comentarioFoliosNoDisponibles += comentarioFoliosNoDisponibles + " " +nuevoFolioNDis +" , " ;
                    }

                    nuevaAlerta.Id_Nom = AjustarDelegacionNomina.IdNomina.ToString();
                    nuevaAlerta.Detalle = "HAY FOLIOS QUE NO SE HAN RECUPERADO CORRECTAMENTE, INTENTE LO SIG :";
                    nuevaAlerta.Solucion = comentarioFoliosNoDisponibles;
                    Advertencias.Add(nuevaAlerta);
                    return Advertencias;

                }

            }

            return Advertencias;
        }



        #endregion

    }
}
