using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Entidades.DTO.FoliarDTO;
using DAP.Foliacion.Entidades;
using Datos;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using DAP.Foliacion.Entidades.DTO.FoliarDTO.RecuperarFolios;
using DAP.Foliacion.Entidades.DTO.FoliarDTO.FoliacionPagomatico;
using System.Threading;

namespace DAP.Foliacion.Datos
{
    public class FoliarConsultasDBSinEntity
    {

        /// <summary>
        /// Consulta el detalle de personal total de una nomina para saber el total de cheques por delegacion como se imprimen desde nomina foxito Ing.Gabriela
        /// Recibe como parametro una lista de consulta de totales y un boleano para saber si es general, Descentralizada o pertenece a otra nomina  
        /// </summary>
        /// <param name="ListaConsultas"></param>
        /// <param name=" EsNominaGeneralODesc"> true si es nomina 01 o 02 o false si no </param>
        /// <returns>Regresa una lista de Totales de Registros X Delegacion en la cual vienen cuantos cheques hay por delegacion "TotalRegistrosXDelegacionDTO"</returns>
        public static List<TotalRegistrosXDelegacionDTO> ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina(List<string> ListaConsultas, bool EsNominaGeneralODesc)
        {
            List<TotalRegistrosXDelegacionDTO> TotalRegistros = new List<TotalRegistrosXDelegacionDTO>();


            foreach (string consulta in ListaConsultas)
            {

                try
                {
                    using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                    {
                        connection.Open();
                        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consulta, connection);
                        System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                        while (reader.Read())
                        {
                            //posicion 0 = sindicato -> 0 u 1
                            //posicion 1 = delegacion -> cadena
                            //posicion 2 = Total  -> numero de registros casteable a int
                            int registroTotal = Convert.ToInt32(reader[2].ToString().Trim());
                            if (registroTotal > 0)
                            {
                                TotalRegistrosXDelegacionDTO nuevoRegistro = new TotalRegistrosXDelegacionDTO();

                                if (EsNominaGeneralODesc)
                                {
                                    int castBoleano = Convert.ToInt32(reader[0].ToString().Trim());
                                    nuevoRegistro.Sindicato = Convert.ToBoolean(castBoleano);
                                }
                                nuevoRegistro.Delegacion = reader[1].ToString().Trim();
                                nuevoRegistro.Total = Convert.ToInt32(reader[2].ToString().Trim());


                                TotalRegistros.Add(nuevoRegistro);
                            }



                        }
                    }


                }
                catch (Exception E)
                {
                    var transaccion = new Transaccion();

                    var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                    LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                    NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                    NuevaExcepcion.Metodo = "ObtenerTotalDePersonasEnNominaPorDelegacionConsultaCualquierNomina";
                    NuevaExcepcion.Usuario = null;
                    NuevaExcepcion.Excepcion = E.Message;
                    NuevaExcepcion.Comentario = "problema al extraer la data para la tabla del modal de pagoXFormas de pago al cargar una nomina";
                    NuevaExcepcion.Fecha = DateTime.Now;

                    repositorio.Agregar(NuevaExcepcion);

                }



            }

            return TotalRegistros;

        }









        /*************************************************************************************************************************************************************************************************************************/
        /*************************************************************************************************************************************************************************************************************************/
        /*******************************************************************                    Devuelve un boleano de si esta foliada la nomina en SQL                             **********************************************/
        /*************************************************************************************************************************************************************************************************************************/
        /*************************************************************************************************************************************************************************************************************************/
        public static bool EstaFoliadacorrectamenteDelegacion_Cheque(string ConsultaPreparada, int TotalFolios)
        {
            /* EL RESULTADO OBTENIDO SON CUENTOS REGISTROS NO ESTAN FOLIADOS POR ENDE SI OBTENEMOS LA DIFERENCIA ENTRE EL TOTALFOLIOS HACEN LOS QUE YA ESTAN FOLIADOS*/
        bool EstaFoliadoDelegacion = false;

            int registrosFoliados = 0;
            List<string> listaNumcheFoliados = new List<string>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(ConsultaPreparada, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        int registrosNoFoliados = Convert.ToInt32(reader[0].ToString().Trim());

                        if (registrosNoFoliados > 0)
                        {
                            registrosFoliados = TotalFolios - registrosNoFoliados;
                        }
                        else 
                        {
                            registrosFoliados = TotalFolios - registrosNoFoliados;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "EstaFoliadacorrectamenteDelegacion_Cheque";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "problema al ejecutar una consulta para saber si esta foliada o no para modal de pagoXFormas de pago al cargar una nomina";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }



            if (registrosFoliados == TotalFolios)
            {
                EstaFoliadoDelegacion = true;
            }

            return EstaFoliadoDelegacion;

        }


        /*OBTIENE UNA LISTA DE NUMEROS DE EMPLEADOS CONTENIDOS DENTRO DE LA DELEGACION SELECCIONADA*/
        public static List<int> NumEmpleadosContenidosEnDelegacion_cheque(string ExecutarConsulta)
        {
            /* EL RESULTADO OBTENIDO SON LA LISTA DE EMPLEADOS QUE SE ENCUENTRAN DENTRO DE LA DELEGACION SELECCIONADA */
            List<int> listaNumerosEmpleados = new List<int>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(ExecutarConsulta, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        listaNumerosEmpleados.Add( Convert.ToInt32( reader[0].ToString().Trim() ) );
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "NumEmpleadosContenidosEnDelegacion_cheque";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "Problema al ejecutar una consulta para saber el numero de empleados que estan contenidos dentro de la delegacion seleccionada";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }

            return listaNumerosEmpleados;

        }




        /*************************************************************************************************************************************************************************************************************************/
        /*************************************************************************************************************************************************************************************************************************/
        /**********************************       Obtienen los detalles de las bases en botacora segun sea el caso ( una en especifico o todas) segun al sobre carga de metodo que use                  *************************/
        /*************************************************************************************************************************************************************************************************************************/
        /*************************************************************************************************************************************************************************************************************************/
        /// <summary>
        /// Obtiene la base An elegida por el IdNom y el anio de la interface elegida
        /// </summary>
        /// <param name="IdNom"></param>
        /// <param name="AnioInterface"></param>
        /// <returns>Retorna un objeto del tipo DatosCompletosBitacoraDTO </returns>
        public static DatosCompletosBitacoraDTO ObtenerDatosCompletosBitacoraFILTRO(int IdNom, string AnioInterface)
        {
            DatosCompletosBitacoraDTO DatosCompletosBitacoraIdNom = new DatosCompletosBitacoraDTO();

            try
            {
               
                string query = "select id_nom, nomina, an, adicional, quincena,mes, referencia, CASE nomina  WHEN '08' THEN 'True' ELSE 'False' END 'EsPenA' ,coment, ruta , rutanomina , fechaPago , subnomina ,  importado    from interfaces" + AnioInterface + ".dbo.bitacora where id_nom = " + IdNom + " order by id_nom";

                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {

                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(query, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        DatosCompletosBitacoraIdNom.Id_nom = Convert.ToInt32(reader[0].ToString().Trim());
                        DatosCompletosBitacoraIdNom.Nomina = reader[1].ToString().Trim();
                        DatosCompletosBitacoraIdNom.An = reader[2].ToString().Trim();
                        DatosCompletosBitacoraIdNom.Adicional = reader[3].ToString().Trim();
                        DatosCompletosBitacoraIdNom.Quincena = reader[4].ToString().Trim();
                        DatosCompletosBitacoraIdNom.Mes = Convert.ToInt32(reader[5].ToString().Trim());
                        DatosCompletosBitacoraIdNom.ReferenciaBitacora = reader[6].ToString().Trim();
                        DatosCompletosBitacoraIdNom.EsPenA = Convert.ToBoolean(reader[7].ToString().Trim());
                        DatosCompletosBitacoraIdNom.Coment = reader[8].ToString().Trim();
                        DatosCompletosBitacoraIdNom.Ruta = reader[9].ToString().Trim();
                        DatosCompletosBitacoraIdNom.RutaNomina = reader[10].ToString().Trim();
                        DatosCompletosBitacoraIdNom.Anio = Convert.ToInt32(reader[11].ToString().Trim().Substring(6, 4));
                        DatosCompletosBitacoraIdNom.Subnomina = reader[12].ToString().Trim();
                        DatosCompletosBitacoraIdNom.Importado = Convert.ToBoolean(reader[13].ToString().Trim());

                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDatosCompletosBitacoraFILTRO(int IdNom, int AnioInterface)";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o el archivo no existe";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

                return new DatosCompletosBitacoraDTO();
            }


            return DatosCompletosBitacoraIdNom;
        }


        /// <summary>
        /// Obtiene todas las nominas de la quincena y anioInterfas elegidos
        /// </summary>
        /// <param name="AnioInterface"></param>
        /// <param name="Quincena"></param>
        /// <returns>Retorna una lista de objetos del tipo DatosCompletosBitacoraDTO</returns>
        public static List<DatosCompletosBitacoraDTO> ObtenerDatosCompletosBitacoraFILTRO(string VisitaAnioInterface, string Quincena)
        {
            List<DatosCompletosBitacoraDTO> DatosCompletosBitacoraQuincena = new List< DatosCompletosBitacoraDTO>();

            try
            {
                string query = "select id_nom, nomina, an, adicional, quincena,mes, referencia, CASE nomina  WHEN '08' THEN 'True' ELSE 'False' END 'EsPenA' ,coment, ruta , rutanomina , fechaPago , subnomina ,  importado , ap, ad from interfaces" + VisitaAnioInterface + ".dbo.bitacora where quincena = "+Quincena+ " order by  nomina, id_nom";

               
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {

                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(query, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        DatosCompletosBitacoraDTO NuevoDetalle = new DatosCompletosBitacoraDTO();

                        NuevoDetalle.Id_nom = Convert.ToInt32(reader[0].ToString().Trim());
                        NuevoDetalle.Nomina = reader[1].ToString().Trim();
                        NuevoDetalle.An = reader[2].ToString().Trim();
                        NuevoDetalle.Adicional = reader[3].ToString().Trim();
                        NuevoDetalle.Quincena = reader[4].ToString().Trim();
                        NuevoDetalle.Mes = Convert.ToInt32(reader[5].ToString().Trim());
                        NuevoDetalle.ReferenciaBitacora = reader[6].ToString().Trim();
                        NuevoDetalle.EsPenA = Convert.ToBoolean(reader[7].ToString().Trim());
                        NuevoDetalle.Coment = reader[8].ToString().Trim();
                        NuevoDetalle.Ruta = reader[9].ToString().Trim();
                        NuevoDetalle.RutaNomina = reader[10].ToString().Trim();
                        //string anioDeQuincena = Quincena.Substring(1, 2).


                        NuevoDetalle.Anio = Convert.ToInt32(reader[11].ToString().Trim().Substring(6, 4));
                        NuevoDetalle.Subnomina = reader[12].ToString().Trim();
                        NuevoDetalle.Importado = Convert.ToBoolean(reader[13].ToString().Trim());
                        NuevoDetalle.Ap = reader[14].ToString().Trim();
                        NuevoDetalle.Ad = reader[15].ToString().Trim();
                        DatosCompletosBitacoraQuincena.Add(NuevoDetalle);
                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDatosCompletosBitacoraFILTRO( int AnioInterface, string Quincena)";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o el archivo no existe";
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);

                return new List<DatosCompletosBitacoraDTO>();
            }


            return DatosCompletosBitacoraQuincena;
        }

      








        /*************************************************************************************************************************************************************************************************************************/
        /*************************************************************************************************************************************************************************************************************************/
        /*******************************************************************          Verificar cuandos registros No estan Foliados en los pagomaticos de un A-N                    **********************************************/
        /*************************************************************************************************************************************************************************************************************************/
        /*************************************************************************************************************************************************************************************************************************/
        public static int ObtenerRegistro_FoliacionPagomatico(string EjecutarConsulta)
        {
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(EjecutarConsulta , connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        //es true si el campo esta vacio o en blanco 
                        if (!string.IsNullOrWhiteSpace(reader[0].ToString().Trim()))
                        {
                            return reader.GetInt32(0);
                        }

                    }
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerRegistro_FoliacionPagomatico";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o el archivo no existe";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);
            }


            return 0;
        }








        public static bool VerificarSiTieneELCampo_AZTECA(string An , int Anio)
        {
            string verificaSiContieneCampoAZTECA = "select top (1) * from Interfaces[ANIO].dbo.[AN]";

            if (Anio == 2022)
            {
                verificaSiContieneCampoAZTECA = verificaSiContieneCampoAZTECA.Replace("[ANIO]", "");
                verificaSiContieneCampoAZTECA = verificaSiContieneCampoAZTECA.Replace("[AN]", An);
            }



            bool tieneElcampo_AZTECA = false;
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
            {
                connection.Open();
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(verificaSiContieneCampoAZTECA, connection);
                System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

                var ColumnasMayus = columns.Select(a => a.ToUpper()).ToList();

                while (reader.Read())
                {
                    tieneElcampo_AZTECA = ColumnasMayus.Contains("AZTECA");
                }
            }
            return tieneElcampo_AZTECA;
        }

        public static List<string> VerificarCamposBancoContieneAN(string An, string visititaAnioInterface)
        {

            List<string> camposContenidoEnAN = new List<string>();


            string verificarCampos = "select top (1) * from Interfaces"+visititaAnioInterface+".dbo."+An+"";

            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
            {
                connection.Open();
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(verificarCampos, connection);
                System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

                List<string> ColumnasMayus = columns.Select(a => a.ToUpper()).ToList();

                List<string> camposBancosDisponibles = ObtenerCamposBancosDisponibles();

               // int i = 0;
                foreach (string campo in camposBancosDisponibles) 
                {

                    bool existeElCampo = ColumnasMayus.Contains(campo);

                    if (existeElCampo)
                    {
                            camposContenidoEnAN.Add(campo);
                    }
                   
                }

           
            }
            return camposContenidoEnAN;
        }


        public static List<string> ObtenerCamposBancosDisponibles()
        {
            string executar= "select distinct(NombreCampoAN) from nomina.dbo.tblnm_ctabanca where NombreCampoAN is not null";

            List<string> camposBancoOntenidos = new List<string>();
            //bool tieneElcampo_AZTECA = false;
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtnercadenaConexionAlpha()))
            {
                connection.Open();
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(executar, connection);
                System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    camposBancoOntenidos.Add( reader[0].ToString().Trim() );
                }
            }
            return camposBancoOntenidos;
        }
        /************************************************************************************************************************************************************************************************************************************/
        /************************************************************************************************************************************************************************************************************************************/








        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        /**********************   REVISION DE COMO SE ENCUNTRA FOLIADO AN EN SQL SEGUN UNA LA CONSULTA "FUNCIONA PARA CHEQUESPAGOMATICOS, VERIFICA QUE TODOS LOS REGISTROS ESTEN FOLIADOS   ************/
        /***********************************************************************************************************************************************************************************************/
        /***********************************************************************************************************************************************************************************************/
        public static List<ResumenRevicionNominaPDFDTO> ObtenerResumenDatosComoSeEncuentraFolidoEnAN(string consulta, string Nomina)
        {

            List<ResumenRevicionNominaPDFDTO> resumenNomina = new List<ResumenRevicionNominaPDFDTO>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consulta, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();


                    int iterator = 0;
                    while (reader.Read())
                    {
                        ResumenRevicionNominaPDFDTO nuevoRegistro = new ResumenRevicionNominaPDFDTO();

                        nuevoRegistro.Contador = Convert.ToString(++iterator);
                        nuevoRegistro.Partida = reader[0].ToString().Trim();
                        nuevoRegistro.CadenaNumEmpleado = reader[1].ToString().Trim();
                        nuevoRegistro.NombreEmpleado = reader[2].ToString().Trim();
                        nuevoRegistro.Delegacion = reader[3].ToString().Trim();
                        nuevoRegistro.Nomina = Nomina;
                        nuevoRegistro.NUM_CHE = reader[4].ToString().Trim();
                        nuevoRegistro.Liquido = reader[5].ToString().Trim();
                        nuevoRegistro.Cuenta = !string.IsNullOrEmpty(reader[6].ToString().Trim()) ? reader[6].ToString().Trim() + "-[" + reader[7].ToString().Trim() + "]" : "NO FOLIADO";

                        
                        nuevoRegistro.Suspencion = reader[8].ToString().Trim().Equals("TALON POR CHEQUE") ? "xx" : "";

                        resumenNomina.Add(nuevoRegistro);
                    }
                    connection.Close();
                }

            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDatosPersonalesDelegacionNomina_ReporteCheque";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }
            return resumenNomina;
        }






        /*****************************************************************************************************************************************************************************************/
        /*****************************************************************************************************************************************************************************************/
        /**************************************************     METODOS PARA FOLIAR PAGOMATICOS DE UNA NOMINA EN ESPECIFICO***********************************************************************/
        /*****************************************************************************************************************************************************************************************/
        /*****************************************************************************************************************************************************************************************/

        public static List<ResumenPersonalAFoliarDTO> ObtenerDatosPersonalesNomina_FoliacionPAGOMATICO(string ExecutarQuery, int complementoQuincena, bool EsPena)
        {
            List<ResumenPersonalAFoliarDTO> foliarPersonalPagomatico = new List<ResumenPersonalAFoliarDTO>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(ExecutarQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    var transaccion = new Transaccion();
                    var repoTblCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion); 

                    while (reader.Read())
                    {
                        ResumenPersonalAFoliarDTO nuevoRegistro = new ResumenPersonalAFoliarDTO();
                        //  NumRfcNombreLiquidoDTO  = new NumRfcNombreLiquidoDTO();
                        int numchePAGOMATICOConvertir8Digitos =  Convert.ToInt32(reader[0].ToString().Trim() + complementoQuincena);
                        nuevoRegistro.NumChe = String.Format("{0:D8}", numchePAGOMATICOConvertir8Digitos);
                        nuevoRegistro.CadenaNumEmpleado = reader[0].ToString().Trim();
                        nuevoRegistro.NumEmpleado = Convert.ToInt32(reader[0].ToString().Trim());

                        nuevoRegistro.RFC = reader[1].ToString().Trim();
                        nuevoRegistro.Nombre = reader[2].ToString().Trim();
                        nuevoRegistro.Liquido = Convert.ToDecimal(reader[3].ToString().Trim());

                        nuevoRegistro.IdBancoPagador = Convert.ToInt32(reader[4].ToString().Trim());

                        Tbl_CuentasBancarias CuentaEncontrada = repoTblCuentaBancaria.Obtener(x => x.Id == nuevoRegistro.IdBancoPagador);
                        nuevoRegistro.BancoX = CuentaEncontrada.NombreBanco;
                        nuevoRegistro.CuentaX = CuentaEncontrada.Cuenta;
                        nuevoRegistro.Observa = "TARJETA";

                        
                        nuevoRegistro.Delegacion = reader[5].ToString().Trim();
                        nuevoRegistro.Partida = reader[6].ToString().Trim();

                        nuevoRegistro.FolioCFDI = string.IsNullOrEmpty(reader[7].ToString().Trim()) ? 0 : Convert.ToInt32(reader[7].ToString().Trim());

                        if (EsPena)
                        {
                            nuevoRegistro.NumBeneficiario = reader[8].ToString().Trim();
                        }

                        foliarPersonalPagomatico.Add(nuevoRegistro);
                    }
                    connection.Close();
                }



            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerDatosPersonalesNominaPAGOMATICO";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o el archivo no existe";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);


                List<ResumenPersonalAFoliarDTO> ListaError = null;


                return ListaError;
            }


            return foliarPersonalPagomatico;
        }



        public static async Task<int> ActualizarBaseNominaEnSql_transaccionado_CANCELACIONHILO(List<ResumenPersonalAFoliarDTO> GuardarResumenPersonalFoliado, DatosCompletosBitacoraDTO datosCompletosNomina, string visitaAnioInterface , CancellationToken cancelaToken)
        {

            int registrosActualizados = 0;

            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
            {
                await connection.OpenAsync();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction();

                try
                {
                    //string anioInterface = "";
                    //if (Anio == Convert.ToInt32(DateTime.Now.Year))
                    //{
                    //    anioInterface = "";
                    //}
                    //else
                    //{
                    //    anioInterface = "" + Anio + "";
                    //}

                    foreach (ResumenPersonalAFoliarDTO guardaNuevaPersona in GuardarResumenPersonalFoliado)
                    {



                        string queryActualizaInterfacesSQL;
                        if (datosCompletosNomina.EsPenA)
                        {
                            queryActualizaInterfacesSQL = "UPDATE interfaces" + visitaAnioInterface + ".dbo." + datosCompletosNomina.An + " SET Num_che = '" + guardaNuevaPersona.NumChe + "', Banco_x = '" + guardaNuevaPersona.BancoX + "', Cuenta_x = '" + guardaNuevaPersona.CuentaX + "', Observa = '" + guardaNuevaPersona.Observa + "' WHERE NUM = '" + guardaNuevaPersona.CadenaNumEmpleado + "' and RFC = '" + guardaNuevaPersona.RFC + "' and LIQUIDO = '" + guardaNuevaPersona.Liquido + "' and NOMBRE = '" + guardaNuevaPersona.Nombre + "' and DELEG = '" + guardaNuevaPersona.Delegacion + "' and BENEF = '" + guardaNuevaPersona.NumBeneficiario + "'  ";
                        }
                        else
                        {
                            queryActualizaInterfacesSQL = "UPDATE interfaces" + visitaAnioInterface + ".dbo." + datosCompletosNomina.An + " SET Num_che = '" + guardaNuevaPersona.NumChe + "', Banco_x = '" + guardaNuevaPersona.BancoX + "', Cuenta_x = '" + guardaNuevaPersona.CuentaX + "', Observa = '" + guardaNuevaPersona.Observa + "' WHERE NUM = '" + guardaNuevaPersona.CadenaNumEmpleado + "' and RFC = '" + guardaNuevaPersona.RFC + "' and LIQUIDO = '" + guardaNuevaPersona.Liquido + "' and NOMBRE = '" + guardaNuevaPersona.Nombre + "' and DELEG = '" + guardaNuevaPersona.Delegacion + "' ";
                        }

                        // Must assign both transaction object and connection
                        // to Command object for a pending local transaction
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryActualizaInterfacesSQL;
                        registrosActualizados += await command.ExecuteNonQueryAsync();

                        //if (registrosActualizados == 0) 
                        //{
                        //    int depura = 1;
                        //}
                        if (cancelaToken.IsCancellationRequested) 
                        {
                            transaction.Rollback();
                            return 0;
                        }

                    }


                    transaction.Commit();

                }
                catch (Exception E)
                {

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // Este bloque catch manejará cualquier error que pueda haber ocurrido
                        // en el servidor que haría que la reversión fallara, como
                        // una conexión cerrada.
                        var transaccion = new Transaccion();

                        var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                        LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                        NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                        NuevaExcepcion.Metodo = "ActualizarBaseNominaEnSql_transaccionado";
                        NuevaExcepcion.Usuario = null;
                        NuevaExcepcion.Excepcion = E.Message + " || " + ex2.Message;
                        NuevaExcepcion.Comentario = "Rollback Exception  : " + ex2.GetType();
                        NuevaExcepcion.Fecha = DateTime.Now;

                        repositorio.Agregar(NuevaExcepcion);
                    }



                }




            }



            return registrosActualizados;
        }


        public static int ActualizarBaseNominaEnSql_transaccionado_Cheque(List<ResumenPersonalAFoliarDTO> GuardarResumenPersonalFoliado, DatosCompletosBitacoraDTO datosCompletosNomina, string visitaAnioInterface)
        {

            int registrosActualizados = 0;

            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction();



                try
                {
                    //string anioInterface = "";
                    //if (Anio == Convert.ToInt32(DateTime.Now.Year))
                    //{
                    //    anioInterface = "";
                    //}
                    //else
                    //{
                    //    anioInterface = "" + Anio + "";
                    //}

                    foreach (ResumenPersonalAFoliarDTO guardaNuevaPersona in GuardarResumenPersonalFoliado)
                    {

                        string queryActualizaInterfacesSQL;
                        if (datosCompletosNomina.EsPenA)
                        {
                            queryActualizaInterfacesSQL = "UPDATE interfaces" + visitaAnioInterface + ".dbo." + datosCompletosNomina.An + " SET Num_che = '" + guardaNuevaPersona.NumChe + "', Banco_x = '" + guardaNuevaPersona.BancoX + "', Cuenta_x = '" + guardaNuevaPersona.CuentaX + "', Observa = '" + guardaNuevaPersona.Observa + "' WHERE NUM = '" + guardaNuevaPersona.CadenaNumEmpleado + "' and LIQUIDO = '" + guardaNuevaPersona.Liquido + "' and NOMBRE = '" + guardaNuevaPersona.Nombre + "' and DELEG = '" + guardaNuevaPersona.Delegacion + "' and BENEF = '" + guardaNuevaPersona.NumBeneficiario + "'  ";
                        }
                        else
                        {
                            //SE QUITO EL REQUERIMIENTO A QUE PERTENESCA A UNA DELEGACION YA QUE CON EL NUMERO DE EMPLEADO DEBE SER SUFICIENTE Y LA DELEGACION NO ES TAN IMPORTANTE
                            //queryActualizaInterfacesSQL = "UPDATE interfaces" + visitaAnioInterface + ".dbo." + datosCompletosNomina.An + " SET Num_che = '" + guardaNuevaPersona.NumChe + "', Banco_x = '" + guardaNuevaPersona.BancoX + "', Cuenta_x = '" + guardaNuevaPersona.CuentaX + "', Observa = '" + guardaNuevaPersona.Observa + "' WHERE NUM = '" + guardaNuevaPersona.CadenaNumEmpleado + "' and LIQUIDO = '" + guardaNuevaPersona.Liquido + "' and NOMBRE = '" + guardaNuevaPersona.Nombre + "' and DELEG = '" + guardaNuevaPersona.Delegacion + "' ";
                            queryActualizaInterfacesSQL = "UPDATE interfaces" + visitaAnioInterface + ".dbo." + datosCompletosNomina.An + " SET Num_che = '" + guardaNuevaPersona.NumChe + "', Banco_x = '" + guardaNuevaPersona.BancoX + "', Cuenta_x = '" + guardaNuevaPersona.CuentaX + "', Observa = '" + guardaNuevaPersona.Observa + "' WHERE NUM = '" + guardaNuevaPersona.CadenaNumEmpleado + "' and LIQUIDO = '" + guardaNuevaPersona.Liquido + "' and NOMBRE = '" + guardaNuevaPersona.Nombre + "'  ";
                        }

                        // Must assign both transaction object and connection
                        // to Command object for a pending local transaction
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandType = CommandType.Text;
                        command.CommandText = queryActualizaInterfacesSQL;
                        registrosActualizados += command.ExecuteNonQuery();
                    }


                    transaction.Commit();

                }
                catch (Exception E)
                {

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {

                        // Este bloque catch manejará cualquier error que pueda haber ocurrido
                        // en el servidor que haría que la reversión fallara, como
                        // una conexión cerrada.

                        var transaccion = new Transaccion();

                        var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                        LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                        NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                        NuevaExcepcion.Metodo = "ActualizarBaseNominaEnSql_transaccionado_Cheque";
                        NuevaExcepcion.Usuario = null;
                        NuevaExcepcion.Excepcion = E.Message + " || " + ex2.Message;
                        NuevaExcepcion.Comentario = "Rollback Exception  : " + ex2.GetType();
                        NuevaExcepcion.Fecha = DateTime.Now;

                        repositorio.Agregar(NuevaExcepcion);
                    }



                }




            }



            return registrosActualizados;
        }








        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /**************************************************     METODOS PARA LIMPIAR AN DE SQL CUANDO NO SE CUMPLA LA CALIDAD DE LA FOLIACION     *************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        public static int LimpiarANSql_IncumplimientoCalidadFoliacion(int Anio, string AN, string condicionACumplir, int Id_Nom)
        {
            int registrosBorrados = 0;
            try
            {
                string anioInterface = "";
                if (Anio == Convert.ToInt32(DateTime.Now.Year))
                {
                    anioInterface = "";
                }
                else
                {
                    anioInterface = "" + Anio + "";
                }


                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();

                        string queryActualizaInterfacesSQL = "UPDATE interfaces"+anioInterface+".dbo."+AN+" SET Num_che = '', Banco_x = '', Cuenta_x = '', Observa = '' where "+condicionACumplir+" ";
                        

                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(queryActualizaInterfacesSQL, connection);
                    registrosBorrados += command.ExecuteNonQuery();
                   
                    connection.Close();


                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "LimpiarANSql_IncumplimientoCalidadFoliacion";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se Borraron los datos en la nomina  "+Id_Nom+" del anio "+ Anio;
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return registrosBorrados;
        }


        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /**************************************************     METODOS PARA LIMPIAR AN DE SQL CUANDO NO SE CUMPLA LA CALIDAD DE LA FOLIACION     *************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        public static int LimpiarCamposPorRecuperacionDeFolios(string visitaAnioInterfas, string AN, string condicionACumplir, int Id_Nom)
        {
            int registrosLimpiados = 0;
            try
            {
              
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();

                    string queryActualizaInterfacesSQL = "UPDATE interfaces" + visitaAnioInterfas + ".dbo." + AN + " SET Num_che = '', Banco_x = '', Cuenta_x = '', Observa = '' where " + condicionACumplir + " ";


                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(queryActualizaInterfacesSQL, connection);
                    registrosLimpiados += command.ExecuteNonQuery();

                    connection.Close();

                }

            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "LimpiarCamposPorRecuperacionDeFolios";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se Limpiaron todos los campos en la nomina  " + Id_Nom + " del anio " + visitaAnioInterfas+ " || Datos que debieron Limpiarse "+ condicionACumplir;
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return registrosLimpiados;
        }








        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /**************************************************     METODOS PARA ACTUALIZAR TBL_PAGO CUANDO CUANDO YA SE HAYA FOLIADOS UNA NOMINA ANTERIORMENTE SOLO HAY QUE ACTUALIZAR LOS DATOS EN TBL_PAGOS     **********************/
        /*******************************************************************            para el actualizar el dbf y el AN existen otros metodos           ***************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        public static int ActualizarTblPagos_DespuesDeFoliacion(List<ActualizarFoliacionPagomaticoTblPagoDTO> ActualizarPersonal)
        {
            int registrosActualizados = 0;
            try
            {
                string nombreDB = ObtenerConexionesDB.ObtenerNombreDBValidacionFoliosDeploy();
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    foreach(ActualizarFoliacionPagomaticoTblPagoDTO nuevaPersona in ActualizarPersonal)
                    {
                        string queryActualizaInterfacesSQL = "UPDATE " + nombreDB + ".dbo.Tbl_Pagos SET IdTbl_CuentaBancaria_BancoPagador = "+nuevaPersona.IdTbl_CuentaBancaria_BancoPagador+ " , FolioCheque = "+nuevaPersona.FolioCheque+" ,  Integridad_HashMD5 = '"+nuevaPersona.Integridad_HashMD5+"'  where Id = "+nuevaPersona.IdPago+"  ";
                        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(queryActualizaInterfacesSQL, connection);
                        
                        int registroActualizadao = command.ExecuteNonQuery();

                        if (registroActualizadao == 1)
                        {
                            registrosActualizados += registroActualizadao;
                        }
                        else 
                        {
                            return 0;
                        }

                    }
                    connection.Close();
                }

            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ActualizarTblPagos_DespuesDeFoliacion";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "ocurrio un error al intentar actualizar Tbl_Pagos despues de la foliacion";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return registrosActualizados;
        }





        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /**************************************************  </REPOSICION DE CHEQUE />    METODOS PARA ACTUALIZAR UN REGISTRO EN AN CUENDO SE REPONE UN FOLIO    *************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        public static int ReponerCheque(DatosCompletosBitacoraDTO datosNominaCompleto , int ReponerNuevoFolio , string CadenaNumEmpleado , Tbl_Pagos reponerFormaPago)
        {
            int registrosBorrados = 0;
            string queryActualizaInterfacesSQL = "";
            try
            {
                string anioInterface = "";
                if (datosNominaCompleto.Anio == Convert.ToInt32(DateTime.Now.Year))
                {
                    anioInterface = "";
                }
                else
                {
                    anioInterface = ""+datosNominaCompleto.Anio+"";
                }

              
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    if (datosNominaCompleto.EsPenA)
                    {
                        queryActualizaInterfacesSQL = "UPDATE interfaces"+anioInterface+".dbo."+datosNominaCompleto.An+" SET NUM_CHE = '" + ReponerNuevoFolio + "'  WHERE NUM = '" + CadenaNumEmpleado + "' and Liquido = " + reponerFormaPago.ImporteLiquido + " and deleg = '" + reponerFormaPago.Delegacion + "' and BENEF = '" + reponerFormaPago.NumBeneficiario + "'";
                    }
                    else
                    {
                        queryActualizaInterfacesSQL = "UPDATE interfaces"+anioInterface+".dbo."+datosNominaCompleto.An+" SET NUM_CHE = '" + ReponerNuevoFolio + "'  WHERE NUM = '" + CadenaNumEmpleado + "' and Liquido = " + reponerFormaPago.ImporteLiquido + " and deleg = '" + reponerFormaPago.Delegacion + "'";
                    }

                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(queryActualizaInterfacesSQL, connection);
                    registrosBorrados += command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ReponerCheque";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo ejecutar el query '"+queryActualizaInterfacesSQL+"'";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);
            }


            return registrosBorrados;
        }
       
        
        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /**************************************************  </SUSPENDER UNA DISPERCION (SUPENCION DE PAGOMATICOS) />    METODOS PARA ACTUALIZAR UN REGISTRO EN AN CUENDO SE REPONE UN FOLIO    *************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        public static int SupenderDispercionDePagomatico(DatosCompletosBitacoraDTO datosNominaCompleto,  string CadenaNumEmpleado, Tbl_Pagos SuspenderDispercion)
        {
            int registrosBorrados = 0;
            string queryActualizaInterfacesSQL = "";
            try
            {
                string anioInterface = "";
                if (datosNominaCompleto.Anio == Convert.ToInt32(DateTime.Now.Year))
                {
                    anioInterface = "";
                }
                else
                {
                    anioInterface = "" + datosNominaCompleto.Anio + "";
                }
                //var transaccion = new Transaccion();
                //var repositorioTblBanco = new Repositorio<Tbl_CuentasBancarias>(transaccion);
                //Tbl_CuentasBancarias cuentaEncontrada = repositorioTblBanco.Obtener(x => x.Id == SuspenderDispercion.IdTbl_CuentaBancaria_BancoPagador);

                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    if (datosNominaCompleto.EsPenA)
                    {
                        queryActualizaInterfacesSQL = "UPDATE interfaces"+anioInterface+".dbo."+datosNominaCompleto.An+ " SET NUM_CHE = '11111111', OBSERVA = 'TALON POR CHEQUE', TALONXCH = 1  WHERE NUM = '" + CadenaNumEmpleado+"' and Liquido = "+SuspenderDispercion.ImporteLiquido+" and deleg = "+SuspenderDispercion.Delegacion+" and BENEF = '"+SuspenderDispercion.NumBeneficiario+"'";
                    }
                    else
                    {
                        queryActualizaInterfacesSQL = "UPDATE interfaces"+anioInterface+".dbo."+datosNominaCompleto.An+ " SET NUM_CHE = '11111111', OBSERVA = 'TALON POR CHEQUE', TALONXCH = 1  WHERE NUM = '" + CadenaNumEmpleado+"' and Liquido = "+SuspenderDispercion.ImporteLiquido+" and deleg = "+SuspenderDispercion.Delegacion+"";
                    }

                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(queryActualizaInterfacesSQL, connection);
                    registrosBorrados += command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "SupenderDispercionDePagomatico";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo ejecutar el query '" + queryActualizaInterfacesSQL + "'";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);
            }

            return registrosBorrados;
        }


        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /**************************************************  </SUSPENDER UNA DISPERCION (SUPENCION DE PAGOMATICOS) />    METODOS PARA ACTUALIZAR UN REGISTRO EN AN CUENDO SE REPONE UN FOLIO    *************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************************************************************/
        public static int RechazoBancarioDispercionDePagomatico(DatosCompletosBitacoraDTO datosNominaCompleto, string CadenaNumEmpleado, Tbl_Pagos SuspenderDispercion)
        {
            int registrosBorrados = 0;
            string queryActualizaInterfacesSQL = "";
            try
            {
                string anioInterface = "";
                if (datosNominaCompleto.Anio == Convert.ToInt32(DateTime.Now.Year))
                {
                    anioInterface = "";
                }
                else
                {
                    anioInterface = "" + datosNominaCompleto.Anio + "";
                }
                //var transaccion = new Transaccion();
                //var repositorioTblBanco = new Repositorio<Tbl_CuentasBancarias>(transaccion);
                //Tbl_CuentasBancarias cuentaEncontrada = repositorioTblBanco.Obtener(x => x.Id == SuspenderDispercion.IdTbl_CuentaBancaria_BancoPagador);

                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                  
                    queryActualizaInterfacesSQL = "UPDATE interfaces"+anioInterface+".dbo."+datosNominaCompleto.An+" SET NUM_CHE = '11111111', OBSERVA = 'RECHAZO'  WHERE NUM = '" + CadenaNumEmpleado + "' and Liquido = " + SuspenderDispercion.ImporteLiquido + " and deleg = " + SuspenderDispercion.Delegacion + "";
                
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(queryActualizaInterfacesSQL, connection);
                    registrosBorrados += command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "RechazoBancarioDispercionDePagomatico";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se pudo ejecutar el query '" + queryActualizaInterfacesSQL + "'";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);
            }

            return registrosBorrados;
        }













        public static string ObtenerNombreEmpleadoSegunAlpha(string NumEmpleado)
        {
            string nombreEncontrado = null;
            string connectionString = @"Data Source=172.19.2.31; Initial Catalog=Nomina; User=sa; PassWord=s3funhwonre2";

            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(" select nomina.dbo.fNombre('" + NumEmpleado + "') ", connection);
                System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    nombreEncontrado = reader[0].ToString().Trim();
                }
            }

            return nombreEncontrado;
        }







        /// <summary>
        /// Obtiene data para la revicion de la nomina de formas de pago filtrada por delegacion 
        /// </summary>
        /// <param name="NumeroNomina"></param>
        /// <param name="AnSegunBitacora"></param>
        /// <param name="quincena"></param>
        /// <param name="NombresBanco"></param>
        /// <returns></returns>
        /// consultaPersonal, bancoEncontrado,  NuevaNominaFoliar.RangoInicial, NuevaNominaFoliar.Inhabilitado, NuevaNominaFoliar.RangoInhabilitadoInicial, NuevaNominaFoliar.RangoInhabilitadoFinal
        public static List<ResumenPersonalAFoliarDTO> ObtenerResumenDatosFormasDePagoFoliar(bool EsPena, string Observa, string ConsultaSql, Tbl_CuentasBancarias BancoEncontrado, int NumeroChequeInicial, bool Inhabilitado, int RangoInhabilitadoInicial, int RangoInhabilitadoFinal)
        {
            List<ResumenPersonalAFoliarDTO> ListaDatosReporteFoliacionPorNomina = new List<ResumenPersonalAFoliarDTO>();

            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(ConsultaSql, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    int TotalInhabilitados = 0;
                    if (Inhabilitado)
                    {
                        TotalInhabilitados = (RangoInhabilitadoFinal - RangoInhabilitadoInicial) + 1;
                    }
                   

                    int TotalInhabilitadosStatico = TotalInhabilitados;
                    int iterador = 0;

                    while (reader.Read())
                    {
                        ResumenPersonalAFoliarDTO NuevaPersona = new ResumenPersonalAFoliarDTO();

                        NuevaPersona.Partida = reader[0].ToString().Trim();
                        NuevaPersona.CadenaNumEmpleado = reader[1].ToString().Trim();
                        NuevaPersona.NumEmpleado = Convert.ToInt32( reader[1].ToString().Trim());
                        
                        NuevaPersona.Nombre = reader[2].ToString().Trim();
                        NuevaPersona.Delegacion = reader[3].ToString().Trim();
                        NuevaPersona.Liquido = Convert.ToDecimal(reader[5].ToString().Trim());
                 
                       
                        NuevaPersona.FolioCFDI = Convert.ToInt32( reader[6].ToString().Trim() );

                        NuevaPersona.RFC = reader[9].ToString().Trim();
                        if (EsPena)
                        {
                            NuevaPersona.NumBeneficiario = reader[10].ToString().Trim();
                        }

                        NuevaPersona.BancoX = BancoEncontrado.NombreBanco;
                        NuevaPersona.CuentaX = BancoEncontrado.Cuenta;
                        NuevaPersona.Observa = Observa;
                        NuevaPersona.IdBancoPagador = BancoEncontrado.Id;

                        int enteroNumChe = 0;
                        if (Inhabilitado)
                        {
                            if (NumeroChequeInicial < RangoInhabilitadoInicial)
                            {
                                enteroNumChe = NumeroChequeInicial++;
                            }
                            else
                            {
                                if (TotalInhabilitados > TotalInhabilitadosStatico)
                                {
                                    enteroNumChe = ++NumeroChequeInicial;
                                }
                                else
                                {
                                    NumeroChequeInicial += TotalInhabilitados++;
                                    enteroNumChe = NumeroChequeInicial;
                                }
                            }
                        }
                        else
                        {
                            enteroNumChe = NumeroChequeInicial++;
                        }

                        NuevaPersona.NumChe = Convert.ToString(enteroNumChe);

                        ListaDatosReporteFoliacionPorNomina.Add(NuevaPersona);
                    }
                }


            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();

                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);

                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerResumenDatosFormasDePagoFoliar";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se han podido leer los datos correctamente o el archivo no existe para crear la revicion del pdf de cada nomina";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);


            }


            return ListaDatosReporteFoliacionPorNomina;
        }






        public static List<int> ObtenerListaDefolios(int FInicial, int TotalRegistrosAFoliar, bool Inhabilitado, int InhabilitadoInicial, int InhabilitadoFinal)
        {
            List<int> listaFolios = new List<int>();



            int TotalInhabilitados = 0;

            if (Inhabilitado)
            {
                TotalInhabilitados = (InhabilitadoFinal - InhabilitadoInicial) + 1;
            }
          

            int TotalInhabilitadosStatico = TotalInhabilitados;
            //int iterador = 0;

            int registros =  (FInicial + TotalRegistrosAFoliar);
            //int registros =  TotalRegistrosAFoliar;

            for (int i=FInicial; i< registros; i++)
            {

                if (Inhabilitado)
                {
                    if (FInicial < InhabilitadoInicial)
                    {
                        listaFolios.Add(FInicial++);
                    }
                    else
                    {
                        if (TotalInhabilitados > TotalInhabilitadosStatico)
                        {
                            listaFolios.Add(++FInicial);
                        }
                        else
                        {
                            FInicial += TotalInhabilitados++;
                            listaFolios.Add(FInicial);
                        }
                    }
                }
                else
                {
                    listaFolios.Add(FInicial++);
                }

            }

            return listaFolios;
        }



















        /*********************************************************************************************************************************************************************************************/
        /*********************************************************************************************************************************************************************************************/
        /*****************************************            REGISTROS A RECUPER CHEQUE CUANDO SE HAYA TENIDO UNA INCIDENCIA POR ERROR HUMANO          **********************************************/
        /*********************************************************************************************************************************************************************************************/
        /*********************************************************************************************************************************************************************************************/
        public static List<FoliosARecuperarDTO> ObtenerRegistrosChequesConIncidenciaPorError(string consulta , string CuentaBancaria )
        {
            List<FoliosARecuperarDTO> resumenNomina = new List<FoliosARecuperarDTO>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consulta, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    int iterator = 0;
                    while (reader.Read())
                    {
                        FoliosARecuperarDTO nuevoRegistro = new FoliosARecuperarDTO();
                        nuevoRegistro.Id = Convert.ToString(++iterator);
                        nuevoRegistro.IdPago = reader.GetInt32(0); 
                        nuevoRegistro.Anio = reader[1].ToString().Trim();
                        nuevoRegistro.Id_nom = reader[2].ToString().Trim();
                        nuevoRegistro.Nomina = reader[3].ToString().Trim();
                        nuevoRegistro.Quincena = reader[4].ToString().Trim();
                        nuevoRegistro.Delegacion = reader[5].ToString().Trim();
                        nuevoRegistro.Beneficiario = reader[6].ToString().Trim();
                        nuevoRegistro.NumEmpleado = reader[7].ToString().Trim();
                        nuevoRegistro.Liquido = reader[8].ToString().Trim();
                        nuevoRegistro.FolioCheque = reader[9].ToString().Trim();
                        nuevoRegistro.IdTbl_InventarioDetalle = Convert.ToInt32( reader[11].ToString().Trim());
                        nuevoRegistro.CuentaBancaria = CuentaBancaria;
                        
                        resumenNomina.Add(nuevoRegistro);
                    }
                    connection.Close();
                }

            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "ObtenerRegistrosChequesConIncidenciaPorError";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return resumenNomina;
        }




        /* Limpiar campos de foliacion del AN de interfaces  */
        public static int LimpiarUnRegitroCamposFoliacionAN(int anio, string AN , string NumEmpleado5Digitos, decimal ImporteLiquido, bool EsPenA , string BeneficiarioPena)
        {
            string anioInterfas = "";
            if (anio != DateTime.Now.Year) 
            {
                anioInterfas = ""+anio+"";
            }

            string consulta = "";
            if (EsPenA)
            {
               consulta = "Update Interfaces" + anioInterfas + ".dbo." + AN + " set NUM_CHE = '', BANCO_X = '', CUENTA_X = '', OBSERVA = '' where NUM = '" + NumEmpleado5Digitos + "' and Liquido = " + ImporteLiquido + " and BENEF = '"+BeneficiarioPena+"' ";
            }
            else 
            {
               consulta = "Update Interfaces" + anioInterfas + ".dbo." + AN + " set NUM_CHE = '', BANCO_X = '', CUENTA_X = '', OBSERVA = '' where NUM = '" + NumEmpleado5Digitos + "' and Liquido = " + ImporteLiquido + "";
            }


            List<FoliosARecuperarDTO> resumenNomina = new List<FoliosARecuperarDTO>();
            try
            {
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(consulta, connection);
                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas == 1)
                    {
                        return filasAfectadas;
                    }
                    
                    connection.Close();
                }

            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();

                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "LimpiarUnRegitroCamposFoliacionAN";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "";
                NuevaExcepcion.Fecha = DateTime.Now;

                repositorio.Agregar(NuevaExcepcion);

            }


            return 0;
        }




        #region METODOS PARA VERIFICAR SI LAS NOMINAS DE LA QUINCENA ESTAN FOLIADAS  
   
        public static int ObtenerTotalRegistrosEnANDeNomina(string VisitaaAnioInterfas , string An)
        {
            string query = "SELECT COUNT(*) FROM Interfaces"+VisitaaAnioInterfas+".dbo."+An+"";
            int totalRegistros = 0;
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
            {
                connection.Open();
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(query, connection);
                System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    totalRegistros = Convert.ToInt32( reader[0].ToString().Trim() );
                }
            }
            return totalRegistros;
        }

    

        public static int ObtenerTotalRegistrosNoFoliadosSegunCondicion(string VisitaaAnioInterfas, string An, string condicionChequeOPagomatico)
        {
            string query = "SELECT COUNT(*) FROM Interfaces" + VisitaaAnioInterfas + ".dbo." + An + " where "+condicionChequeOPagomatico+" ";
            int totalRegistrosNoFoliados = 0;
            using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
            {
                connection.Open();
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(query, connection);
                System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    totalRegistrosNoFoliados = Convert.ToInt32(reader[0].ToString().Trim());
                }
            }
            return totalRegistrosNoFoliados;
        }

        #endregion



        #region LIMPIA LOS CAMPOS DE LAS PERSONAS A LAS QUE SE DEBERIA AJUSTAR SU FOLIO POR ALGUN DETALLE DEL PROCESO DE FOLIACION

        public static int LimpiarANSql_IncumplimientoAjuste(List<ResumenPersonalAFoliarDTO> resumenPersonalAjustar, string VisitaAnioInterfas , string AN)
        {
            int registrosBorrados = 0;
            try
            {
               
                using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ObtenerConexionesDB.obtenerCadenaConexionDeploy()))
                {
                    connection.Open();

                    foreach (ResumenPersonalAFoliarDTO nuevaPersona in resumenPersonalAjustar) 
                    {
                        string queryActualizaInterfacesSQL = "UPDATE interfaces" + VisitaAnioInterfas + ".dbo." + AN + " SET Num_che = '', Banco_x = '', Cuenta_x = '', Observa = '' where num = '"+ nuevaPersona.NumEmpleado+"' and liquido = "+nuevaPersona.Liquido+" ";

                        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(queryActualizaInterfacesSQL, connection);
                        registrosBorrados += command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception E)
            {
                var transaccion = new Transaccion();
                var repositorio = new Repositorio<LOG_EXCEPCIONES>(transaccion);
                
                LOG_EXCEPCIONES NuevaExcepcion = new LOG_EXCEPCIONES();
                NuevaExcepcion.Clase = "FoliarConsultasDBSinEntity";
                NuevaExcepcion.Metodo = "LimpiarANSql_IncumplimientoAjuste";
                NuevaExcepcion.Usuario = null;
                NuevaExcepcion.Excepcion = E.Message;
                NuevaExcepcion.Comentario = "No se Borraron algunos datos al realizar un ajuste ";
                NuevaExcepcion.Fecha = DateTime.Now;
                repositorio.Agregar(NuevaExcepcion);
            }
            return registrosBorrados;
        }


        #endregion

    }
}
