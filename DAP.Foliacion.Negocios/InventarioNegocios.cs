using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAP.Foliacion.Entidades;
using DAP.Foliacion.Datos;
using System.Data.Objects.SqlClient;
using DAP.Foliacion.Entidades.DTO;
using Datos;
using System.Data.Common;

namespace DAP.Foliacion.Negocios
{
    public class InventarioNegocios
    {
        public static IEnumerable<Tbl_CuentasBancarias> ObtenerInventarioActivo()
        {

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var InventariosActivos = repositorio.ObtenerPorFiltro(x => x.Activo == true && x.Tbl_Inventario.Activo == true);


            return InventariosActivos;
        }

        public static int ObtenerIdInventario(int IdBanco)
        {
            int idBanco = 0;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);


            Tbl_CuentasBancarias registroBancoEncontardo = repositorio.Obtener(x => x.Id == IdBanco && x.IdInventario != null && x.Activo == true);

            int idInventario = 0;
            if (registroBancoEncontardo.IdInventario != null)
            {
                idInventario = Convert.ToInt32(registroBancoEncontardo.IdInventario);
            }


            return idInventario;
        }

     

        public static bool GuardarInventarioContenedores(int idBanco, string numeroOrden, int numeroContenedor, string FInicial, string FFinal, int TotalFormas, DateTime fechaExterna)
        {
            int idInventario = ObtenerIdInventario(idBanco);
            bool bandera = false;

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_Inventario>(transaccion);


            var inventarioFiltrado = repositorio.ObtenerPorFiltro(x => x.Id == idInventario && x.Activo == true).ToList();



            try
            {
                //Actualiza datos en la tabla inventario
                foreach (Tbl_Inventario inventarioObtenido in inventarioFiltrado)
                {
                    inventarioObtenido.FormasDisponibles += TotalFormas;
                    inventarioObtenido.UltimoFolioInventario = FFinal;


                }



                //  agrega contenedores con folios para que coincida con el inventario
                Tbl_InventarioContenedores nuevoContenedor = new Tbl_InventarioContenedores();

                nuevoContenedor.IdInventario = idInventario;
                nuevoContenedor.NumOrden = numeroOrden;
                nuevoContenedor.NumContenedor = numeroContenedor;
                nuevoContenedor.FolioInicial = FInicial;
                nuevoContenedor.FolioFinal = FFinal;
                nuevoContenedor.FormasTotalesContenedor = TotalFormas;
                nuevoContenedor.FormasDisponiblesActuales = TotalFormas;
                nuevoContenedor.FormasInhabilitadas = 0;
                nuevoContenedor.FormasAsignadas = 0;
                nuevoContenedor.FormasFoliadas = 0;
                nuevoContenedor.FechaAlta = fechaExterna;
                nuevoContenedor.Activo = true;

                var repositorioContenedor = new Repositorio<Tbl_InventarioContenedores>(transaccion);

                var contenedorAgregado = repositorioContenedor.Agregar(nuevoContenedor);




                //Agrega el detalle de cada contenedor para saber cada uno de sus folios 
                var repositorioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);

                for (int i = Convert.ToInt32(FInicial); i <= Convert.ToInt32(FFinal); i++)
                {
                    Tbl_InventarioDetalle nuevoDetalle = new Tbl_InventarioDetalle();

                    nuevoDetalle.IdContenedor = contenedorAgregado.Id;
                    nuevoDetalle.NumFolio = i;
                    nuevoDetalle.Activo = true;

                    repositorioDetalle.Agregar(nuevoDetalle);
                }
                // transaccion.Dispose();
                //se transacciona por si algo va mal se pueda hacer un roll over de todo
                transaccion.GuardarCambios();
                bandera = true;

            }
            catch (Exception e)
            {
                transaccion.Dispose();
                bandera = false;
            }






            return bandera;

        }






        // pendiente de revision en solicitud
        public static Dictionary<int, string> ObtenerBancosConChequera()
        {
            Dictionary<int, string> CuentasBancariasConCheque = new Dictionary<int, string>();
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var BancosConChequeraActivos = repositorio.ObtenerPorFiltro(x => x.IdCuentaBancaria_TipoPagoCuenta != 1 && x.Activo == true);

            foreach (Tbl_CuentasBancarias cuentaEncontrada in BancosConChequeraActivos) 
            {
                CuentasBancariasConCheque.Add(cuentaEncontrada.Id,  cuentaEncontrada.NombreBanco + " || " + cuentaEncontrada.Cuenta + " || FechaCreacion : "+cuentaEncontrada.FechaCreacion.ToString("yyyy MMMM")+"");
            }

            return CuentasBancariasConCheque;
        }




        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************              Metodos de solicitud          ***********************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/

        #region solicitar formas de pago
        //un medoto para asignar , 
     

        //regresa el id de un registro del inventario filtrado por IdCuentaBancaria
        public static int ObtenerIdInventarioPorIdCuentaBancaria(int Idbanco)
        {
            var transaccion = new Transaccion();


            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            int? VerificaIdInventario = repositorio.Obtener(x => x.Id == Idbanco && x.Activo == true).IdInventario;

            if (VerificaIdInventario != null)
            {
                return Convert.ToInt32( VerificaIdInventario );
            }
            else 
            {
                return 0;
            }

        }


        public static List<InventarioContenedoresDTO> ObtenerInfoContendoresPorBanco(int IdBanco)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            Tbl_CuentasBancarias cuentaEncontrada = repositorio.Obtener(x => x.Id == IdBanco && x.Activo == true);

            int IdInventario = Convert.ToInt32(cuentaEncontrada.IdInventario);

            var repositorio2 = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var contenedoresEncontrados = repositorio2.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.FormasDisponiblesActuales > 0 && x.Activo == true).ToList();



            List<InventarioContenedoresDTO> listaContenedores = new List<InventarioContenedoresDTO>();
            foreach (var contenedor in contenedoresEncontrados)
            {
                InventarioContenedoresDTO nuevoInhabilitado = new InventarioContenedoresDTO();

                nuevoInhabilitado.Banco = cuentaEncontrada.NombreBanco;
                nuevoInhabilitado.Cuenta = cuentaEncontrada.Cuenta;
                nuevoInhabilitado.Orden = contenedor.NumOrden;
                nuevoInhabilitado.Contenedor = contenedor.NumContenedor;
                nuevoInhabilitado.FolioInicial = contenedor.FolioInicial;
                nuevoInhabilitado.FolioFinal = contenedor.FolioFinal;
                nuevoInhabilitado.FormasTotalesContenedor = contenedor.FormasTotalesContenedor;
                nuevoInhabilitado.FormasDisponiblesActuales = contenedor.FormasDisponiblesActuales;
                nuevoInhabilitado.FormasInhabilitadas = contenedor.FormasInhabilitadas;
                nuevoInhabilitado.FormasAsignadas = contenedor.FormasAsignadas;
                nuevoInhabilitado.FechaAlta = contenedor.FechaAlta.ToString("dd/MM/yyyy");


                listaContenedores.Add(nuevoInhabilitado);

            }



            return listaContenedores;
        }


        /// <summary>
        /// Metodo que devuelve los numeros de ordenes activas por el banco filtrado como parametro el IdInventario funciona para Inhabilitar como para Ajustar
        /// </summary>
        /// <param name="IdInventario"></param>
        /// <returns></returns>
        /// Verisado
        public static List<string> ObtenerNumeroOrdenesBancoActivo(int IdInventario)
        {
            string contenedoresEncontrados = "";

            List<string> numerosOrden = new List<string>();

            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);

            var numerosOrdenEncontrados = repositorio.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.FormasDisponiblesActuales > 0  && x.Activo == true);

            foreach (var orden in numerosOrdenEncontrados)
            {
                if (!numerosOrden.Contains(orden.NumOrden))
                {
                    contenedoresEncontrados = orden.NumOrden;

                    numerosOrden.Add(contenedoresEncontrados);
                }
            }
            return numerosOrden;
        }


        /// <summary>
        /// Guarda las solicitud con n cantidad de bancos creadas en la DB y devuelve el numero de memorandum con el que se asigno  
        /// </summary>
        /// <param name="solicitudCreada"></param>
        /// <returns></returns>
        public static int GuardarSolicitudCreada(List<SolicitudCreadaDTO> solicitudCreada /*,string CuentaBanco, string CantidadFormas, string FInicial*/)
        {
            bool bandera = false;

            var transaccion = new Transaccion();
            var repositorioCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            var repositorioSolicitud = new Repositorio<Tbl_Solicitudes>(transaccion);




            //var solicitudesEncontrada = repositorioSolicitud.ObtenerPorFiltro(x => x.Activo == true).ToList();
            var numeroMaximoMemos = repositorioSolicitud.ObtenerPorFiltro(x => x.Activo == true).Select(x => x.NumeroMemo).Max();


            //List<int> numMemosEncontrados = new List<int>();
            //foreach (Tbl_Solicitudes solicitud in solicitudesEncontrada)
            //{
            //    numMemosEncontrados.Add(solicitud.NumeroMemo);
            //}

            // int numMemo;

            int numMemo = numeroMaximoMemos == 0 ? 0 : numeroMaximoMemos;

            //      int numMemoGuardar = numMemo == 0 ? 1 : numMemosEncontrados.Max();

            numMemo += 1;

            int valorADevolver = 0;
            foreach (SolicitudCreadaDTO solicitud in solicitudCreada)
            {
                Tbl_CuentasBancarias cuentaEncontrada = repositorioCuentaBancaria.Obtener(x => x.Id == solicitud.IdBanco);

                Tbl_Solicitudes nuevoBancoSolicitud = new Tbl_Solicitudes();

                nuevoBancoSolicitud.NumeroMemo = numMemo;
                nuevoBancoSolicitud.IdCuentaBancaria = cuentaEncontrada.Id;
                nuevoBancoSolicitud.Cantidad = Convert.ToInt32(solicitud.cantidadFormas);
                nuevoBancoSolicitud.FolioInicial = solicitud.fInicial;
                nuevoBancoSolicitud.FechaSolicitud = DateTime.Now.Date;
                nuevoBancoSolicitud.Activo = true;

                var entidadGuardada = repositorioSolicitud.Agregar(nuevoBancoSolicitud);

                if (entidadGuardada != null)
                    valorADevolver = numMemo;
            }

            return valorADevolver;
        }

        public static IEnumerable<Tbl_Solicitudes> ObtenerSolicitudes()
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_Solicitudes>(transaccion);

            return repositorio.ObtenerPorFiltro(x => x.Activo == true);

        }

        public static bool EliminarMemorandum(int NumeroMemorandum)
        {
            bool bandera = false;
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_Solicitudes>(transaccion);

            var bancosMemorandumEncontrados = repositorio.ObtenerPorFiltro(x => x.Activo == true && x.NumeroMemo == NumeroMemorandum).ToList();

            if (bancosMemorandumEncontrados.Count() > 0)
            {
                foreach (Tbl_Solicitudes banco in bancosMemorandumEncontrados)
                {
                    banco.Activo = false;

                    var bancoInhabilitado = repositorio.Modificar(banco);

                    bandera = bancoInhabilitado != null ? true : false;
                }

            }

            return bandera;
        }


        public static IEnumerable<Tbl_Solicitudes> ObtenerSolicitudesPorNumeroMemo(int NumMemorandum)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_Solicitudes>(transaccion);

            //Si el NumMemorandum == 0 entonces se escarga el ultimo folio
            if (NumMemorandum != 0)
            {
                return repositorio.ObtenerPorFiltro(x => x.NumeroMemo == NumMemorandum && x.Activo == true);
            }
            else
            {
                int ultimoNumeroMemoEnRegistar = repositorio.ObtenerPorFiltro(x => x.Activo == true).Select(x => x.NumeroMemo).Max();
                return repositorio.ObtenerPorFiltro(x => x.NumeroMemo == ultimoNumeroMemoEnRegistar);
            }

        }

        #endregion



        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************              Verificacion de folios para Inhabilitar y Asignar          ***********************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/

        public static List<InventarioValidaFoliosDTO> ValidarFoliosFormasPago(int IdCuentaBancaria, int FolioInicial, int FolioFinal)
        {
            List<InventarioValidaFoliosDTO> foliosValidadosConError = new List<InventarioValidaFoliosDTO>();


            //Encontrar idInventrio en cuanta bancaria
            var transaccion = new Transaccion();
            var repositorioCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            int IdInventario = repositorioCuentaBancaria.Obtener(x => x.Id == IdCuentaBancaria && x.Activo == true).IdInventario.GetValueOrDefault();


            if (IdInventario > 0)
            {


                string nombreDb = ObtenerConexionesDB.ObtenerNombreDBValidacionFoliosDeploy();
                //Busca los Id's de los contenedores que tengan formas de pagos disponibles que existen en la DB 
                string subQuery = " SELECT Id FROM ["+nombreDb+"].[dbo].[Tbl_InventarioContenedores] where IdInventario = " + IdInventario + " and FormasDisponiblesActuales > 0 ";

                string query = "SELECT *   FROM ["+nombreDb+"].[dbo].[Tbl_InventarioDetalle] where IdContenedor in (" + subQuery + ")";

                List<Tbl_InventarioDetalle> listaDetalleEncontrados = new List<Tbl_InventarioDetalle>();
                listaDetalleEncontrados = InventarioConsultasDBSinEntity.ObtenerRegistrosDetallesFolios(query);



                //TransaccionSQL transaccionSQL = new TransaccionSQL("ConexionDB");
                //RepositorioSQL repositorioSQL = new RepositorioSQL(transaccionSQL);
                //transaccionSQL.Conectar();

                //DbDataReader LeerDatos = repositorioSQL.Ejecutar_Leer(query);
               
                //while (LeerDatos.Read())
                //{
                //    Tbl_InventarioDetalle nuevoDetalle = new Tbl_InventarioDetalle();

                //    nuevoDetalle.Id = LeerDatos.GetInt32(0);
                //    nuevoDetalle.IdContenedor = LeerDatos.GetInt32(1);
                //    nuevoDetalle.NumFolio = LeerDatos.GetInt32(2);
                //    string a = LeerDatos[3].ToString().Trim();
                //    if (LeerDatos[3].ToString().Trim() != "")
                //    { nuevoDetalle.IdIncidencia = LeerDatos.GetInt32(3); }
                //    if (LeerDatos[4].ToString().Trim() != "") { nuevoDetalle.FechaIncidencia = LeerDatos.GetDateTime(4); }
                //    if (LeerDatos[5].ToString().Trim() != "") { nuevoDetalle.IdEmpleado = LeerDatos.GetInt32(5); }
                //    nuevoDetalle.Activo = LeerDatos.GetBoolean(6);

                //    listaDetalleEncontrados.Add(nuevoDetalle);
                //}
                //transaccionSQL.Desconectar();




                //Buscar Folios con Incidencias
                //CalcularFolios
                int idVitual = 0;
                for (int i = FolioInicial; i <= FolioFinal; i++)
                {
                    Tbl_InventarioDetalle registroConIncidencia = listaDetalleEncontrados.Where(x => x.NumFolio == i && x.Activo == true).FirstOrDefault();

                    idVitual++;

                    if (registroConIncidencia == null)
                    {
                        //No existe el folio
                        InventarioValidaFoliosDTO nuevaIncidenciaEncontrada = new InventarioValidaFoliosDTO();
                        nuevaIncidenciaEncontrada.IdVirtual = idVitual;
                        nuevaIncidenciaEncontrada.NumFolio = i;
                        nuevaIncidenciaEncontrada.Incidencia = "NO EXISTE EL FOLIO";
                        nuevaIncidenciaEncontrada.Empleado = "No Implementado";
                        foliosValidadosConError.Add(nuevaIncidenciaEncontrada);
                    }
                    else if (registroConIncidencia != null && registroConIncidencia.IdIncidencia != null)
                    {
                        //Contiene Incidencia
                        var repositorioIncidencia = new Repositorio<Tbl_InventarioTipoIncidencia>(transaccion);
                        InventarioValidaFoliosDTO nuevaIncidenciaEncontrada = new InventarioValidaFoliosDTO();

                        nuevaIncidenciaEncontrada.IdVirtual = idVitual;
                        nuevaIncidenciaEncontrada.Id = registroConIncidencia.Id;
                        nuevaIncidenciaEncontrada.IdContenedor = registroConIncidencia.IdContenedor;
                        nuevaIncidenciaEncontrada.NumFolio = registroConIncidencia.NumFolio;
                        nuevaIncidenciaEncontrada.Incidencia = repositorioIncidencia.Obtener(x => x.Id == registroConIncidencia.IdIncidencia).Descrip_Incidencia;
                        nuevaIncidenciaEncontrada.Empleado = "No Implementado";
                        nuevaIncidenciaEncontrada.FechaIncidencia = registroConIncidencia.FechaIncidencia.GetValueOrDefault().ToString("MM/dd/yyyy");

                        foliosValidadosConError.Add(nuevaIncidenciaEncontrada);
                    }


                }



            }

            return foliosValidadosConError;
        }

        /// <summary>
        /// Devuelve una lista de los folios que ya fueron ocupados dentro de un contenedor en especifico para que no se puedan usar
        /// </summary>
        /// <param name="IdContenedor"></param>
        /// <param name="FolioInicial"></param>
        /// <param name="FolioFinal"></param>
        /// <returns></returns>
        public static List<InventarioValidaFoliosDTO> ValidarFoliosPorContenedor(int IdContenedor)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            var repositorioIncidencias = new Repositorio<Tbl_InventarioTipoIncidencia>(transaccion);

            List<Tbl_InventarioDetalle> contenedorConFoliosEncontrados = repositorio.ObtenerPorFiltro(x => x.IdContenedor == IdContenedor && x.Activo == true).ToList();



            List<InventarioValidaFoliosDTO> ListafoliosNoDisponiblesContenedor = new List<InventarioValidaFoliosDTO>();
            int idVirtual = 1;
            foreach (Tbl_InventarioDetalle nuevoDetalle in contenedorConFoliosEncontrados)
            {
                if (nuevoDetalle.IdIncidencia != null)
                {
                    InventarioValidaFoliosDTO nuevaIncidenciaEncontrada = new InventarioValidaFoliosDTO();
                    nuevaIncidenciaEncontrada.IdVirtual = idVirtual++;
                    nuevaIncidenciaEncontrada.Id = nuevoDetalle.Id;
                    nuevaIncidenciaEncontrada.IdContenedor = nuevoDetalle.IdContenedor;
                    nuevaIncidenciaEncontrada.NumFolio = nuevoDetalle.NumFolio;
                    nuevaIncidenciaEncontrada.Incidencia = repositorioIncidencias.Obtener(x => x.Id == nuevoDetalle.IdIncidencia).Descrip_Incidencia;
                    nuevaIncidenciaEncontrada.Empleado = "No Implementado";
                    nuevaIncidenciaEncontrada.FechaIncidencia = nuevoDetalle.FechaIncidencia.GetValueOrDefault().ToString("MM/dd/yyyy");

                    ListafoliosNoDisponiblesContenedor.Add(nuevaIncidenciaEncontrada);
                }
            }

            return ListafoliosNoDisponiblesContenedor;
        }


        public static Dictionary<int, string> ObtenerPersonalActivo()
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);
            var personalEncontrado = repositorio.ObtenerPorFiltro(x => x.Activo == true);
            Dictionary<int, String> nombrePersonalEncontrado = new Dictionary<int, string>();
            foreach (Tbl_InventarioAsignacionPersonal personal in personalEncontrado)
            {
                nombrePersonalEncontrado.Add(personal.IdEmpleado, personal.NombrePersonal);
            }

            return nombrePersonalEncontrado;
        }






        /******************************************************************************************************************************************************************************/
        /******************************************************************************************************************************************************************************/
        /******************************************************        Metodos Inhabilitar            *******************************************************************************/
        /******************************************************************************************************************************************************************************/
        /******************************************************************************************************************************************************************************/
        #region metodos para inhabilitar y 

        /// <summary>
        /// Devuelve los contenedores que contiene cada Numero de orden existente donde haya formas actuales y este activo; Funciona para Inhabilitar como para Ajustar
        /// </summary>
        /// <param name="IdInventario"></param>
        /// <param name="OrdenSelecionada"></param>
        /// <returns></returns>
        public static List<Tbl_InventarioContenedores> ObtenerContenedoresActivosPorIdInventario(int IdInventario, string OrdenSelecionada)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);

            var contenedoresEncontrados = repositorio.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.NumOrden == OrdenSelecionada && x.FormasDisponiblesActuales > 0 && x.Activo == true).ToList();


            return contenedoresEncontrados;
        }


        /// <summary>
        /// Decuelve la entidad que contiene los numeros de folio inicial y final del contenedor y la orden seleccionada
        /// </summary>
        /// <param name="IdInventario"></param>
        /// <param name="OrdenSeleccionada"></param>
        /// <param name="ContenedorSeleccionado"></param>
        /// <returns></returns>
        public static Tbl_InventarioContenedores ObtenerFoliosPorContenedor(int IdInventario, string OrdenSeleccionada, int ContenedorSeleccionado)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioContenedores>(transaccion);

            return repositorio.Obtener(x => x.IdInventario == IdInventario && x.NumOrden == OrdenSeleccionada && x.NumContenedor == ContenedorSeleccionado && x.FormasDisponiblesActuales > 0 && x.Activo == true);
        }


        public static int InhabilitarRangoFolios(int IdCuentaBancaria, int FolioInicial, int FolioFinal)
        {
            var transaccion = new Transaccion();
            var repositorioCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);
            var repositorioContenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repositorioInventarioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            int IdInventario = repositorioCuentaBancaria.Obtener(x => x.Id == IdCuentaBancaria && x.Activo == true).IdInventario.GetValueOrDefault();



            string nombreDb = ObtenerConexionesDB.ObtenerNombreDBValidacionFoliosDeploy();
            //Busca los Id's de los contenedores que tengan formas de pagos disponibles que existen en la DB 
            string subQuery = " SELECT Id FROM [" + nombreDb + "].[dbo].[Tbl_InventarioContenedores] where IdInventario = " + IdInventario + " and FormasDisponiblesActuales > 0 ";

            string query = "SELECT *   FROM [" + nombreDb + "].[dbo].[Tbl_InventarioDetalle] where IdContenedor in (" + subQuery + ")";

            List<Tbl_InventarioDetalle> listaDetalleEncontrados = new List<Tbl_InventarioDetalle>();
            listaDetalleEncontrados = InventarioConsultasDBSinEntity.ObtenerRegistrosDetallesFolios(query);




            //Buscar Folios con Incidencias
            //CalcularFolios
            int foliosInhabilitados = 0;
            for (int i = FolioInicial; i <= FolioFinal; i++)
            {
                Tbl_InventarioDetalle registroSinIncidencia = listaDetalleEncontrados.Where(x => x.NumFolio == i && x.IdIncidencia == null && x.Activo == true).FirstOrDefault();

                if (registroSinIncidencia != null) 
                {

                    registroSinIncidencia.IdIncidencia = 1 /*Inhabilitado*/;
                    registroSinIncidencia.FechaIncidencia = DateTime.Now;

                    Tbl_InventarioContenedores contenedorEncontardo = repositorioContenedores.Obtener(x => x.Id == registroSinIncidencia.IdContenedor);
                    contenedorEncontardo.FormasDisponiblesActuales -= 1;
                    contenedorEncontardo.FormasInhabilitadas += 1;

                    Tbl_Inventario inventarioEncontrado = repositorioInventario.Obtener(x => x.Id == contenedorEncontardo.IdInventario);
                    inventarioEncontrado.FormasDisponibles -= 1;

                    repositorioContenedores.Modificar_Transaccionadamente(contenedorEncontardo);
                    repositorioInventario.Modificar_Transaccionadamente(inventarioEncontrado);

                    //registroSinIncidencia.Tbl_InventarioContenedores.FormasDisponiblesActuales -= 1;
                    //registroSinIncidencia.Tbl_InventarioContenedores.FormasInhabilitadas += 1;
                    //registroSinIncidencia.Tbl_InventarioContenedores.Tbl_Inventario.FormasDisponibles -= 1;


                    int id = repositorioInventarioDetalle.Modificar_Transaccionadamente(registroSinIncidencia).Id;

                    if (id > 0)
                    {
                        foliosInhabilitados += 1;
                        transaccion.GuardarCambios();
                    }
                }

            }




            //List<int> inhabilitarFoliosEncontrados = repositorioInventario.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.FormasDisponiblesActuales > 0 && x.Activo == true).Select(y => y.Id).ToList();

            //List<Tbl_InventarioDetalle> foliosEncontrados = repositorioInventarioDetalle.ObtenerPorFiltro(x => x.IdContenedor >= inhabilitarFoliosEncontrados.Min() && x.IdContenedor <= inhabilitarFoliosEncontrados.Max() && x.IdIncidencia == null).ToList();

            //List<Tbl_InventarioDetalle> foliosFiltrados = foliosEncontrados.Where(x => x.NumFolio >= FolioInicial && x.NumFolio <= FolioFinal).ToList();

            //int foliosInhabilitados = 0;
            //foreach (Tbl_InventarioDetalle nuevoDetalle in foliosFiltrados)
            //{

            //    nuevoDetalle.IdIncidencia = 1 /*Inhabilitado*/;
            //    nuevoDetalle.FechaIncidencia = DateTime.Now;
            //    nuevoDetalle.Tbl_InventarioContenedores.FormasDisponiblesActuales -= 1;
            //    nuevoDetalle.Tbl_InventarioContenedores.FormasInhabilitadas += 1;
            //    nuevoDetalle.Tbl_InventarioContenedores.Tbl_Inventario.FormasDisponibles -= 1;

            //    int id = repositorioInventarioDetalle.Modificar(nuevoDetalle).Id;

            //    if (id > 0)
            //        foliosInhabilitados += 1;
            //}
            return foliosInhabilitados;
        }

        /* INHABILITAR UN CONTENEDOR COMPLETO*/
        public static int InhabilitarContenedor(int IdContenedor)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_InventarioDetalle>(transaccion);

            List<Tbl_InventarioDetalle> inhabilitarFoliosEncontrados = repositorio.ObtenerPorFiltro(x => x.IdContenedor == IdContenedor && x.IdIncidencia == null && x.Activo == true).ToList();

            int foliosInhabilitados = 0;
            foreach (Tbl_InventarioDetalle nuevoDetalle in inhabilitarFoliosEncontrados)
            {

                nuevoDetalle.IdIncidencia = 1 /*Inhabilitado*/;
                nuevoDetalle.FechaIncidencia = DateTime.Now;
                nuevoDetalle.Tbl_InventarioContenedores.FormasDisponiblesActuales -= 1;
                nuevoDetalle.Tbl_InventarioContenedores.FormasInhabilitadas += 1;
                nuevoDetalle.Tbl_InventarioContenedores.Tbl_Inventario.FormasDisponibles -= 1;
                int id = repositorio.Modificar(nuevoDetalle).Id;

                if (id > 0)
                    foliosInhabilitados += 1;
            }
            return foliosInhabilitados;
        }


        #endregion



        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /******************************************************              Asignar           *************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/

        public static int AsignarRangoFolios(int IdPersonal, int IdCuentaBancaria, int FolioInicial, int FolioFinal)
        {
            var transaccion = new Transaccion();
            var repositorioCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            var repositorioInventario = new Repositorio<Tbl_Inventario>(transaccion);
            var repositorioContenedores = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repositorioInventarioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            var repoPersonal = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);
            int IdInventario = repositorioCuentaBancaria.Obtener(x => x.Id == IdCuentaBancaria && x.Activo == true).IdInventario.GetValueOrDefault();




            string nombreDb = ObtenerConexionesDB.ObtenerNombreDBValidacionFoliosDeploy();
            //Busca los Id's de los contenedores que tengan formas de pagos disponibles que existen en la DB 
            string subQuery = " SELECT Id FROM [" + nombreDb + "].[dbo].[Tbl_InventarioContenedores] where IdInventario = " + IdInventario + " and FormasDisponiblesActuales > 0 ";

            string query = "SELECT *   FROM [" + nombreDb + "].[dbo].[Tbl_InventarioDetalle] where IdContenedor in (" + subQuery + ")";

            List<Tbl_InventarioDetalle> listaDetalleEncontrados = new List<Tbl_InventarioDetalle>();
            listaDetalleEncontrados = InventarioConsultasDBSinEntity.ObtenerRegistrosDetallesFolios(query);


            int foliosInhabilitados = 0;
            for (int i = FolioInicial; i <= FolioFinal; i++)
            {
                Tbl_InventarioDetalle registroSinIncidencia = listaDetalleEncontrados.Where(x => x.NumFolio == i && x.IdIncidencia == null && x.Activo == true).FirstOrDefault();

                if (registroSinIncidencia != null)
                {

                    registroSinIncidencia.IdIncidencia = 2 /*Asignado*/;
                    registroSinIncidencia.FechaIncidencia = DateTime.Now;
                    registroSinIncidencia.IdEmpleado = repoPersonal.Obtener(x => x.IdEmpleado == IdPersonal).Id;

                    Tbl_InventarioContenedores contenedorEncontardo = repositorioContenedores.Obtener(x => x.Id == registroSinIncidencia.IdContenedor);
                    contenedorEncontardo.FormasDisponiblesActuales -= 1;
                    contenedorEncontardo.FormasAsignadas += 1;

                    Tbl_Inventario inventarioEncontrado = repositorioInventario.Obtener(x => x.Id == contenedorEncontardo.IdInventario);
                    inventarioEncontrado.FormasDisponibles -= 1;

                    int idContenedor = repositorioContenedores.Modificar_Transaccionadamente(contenedorEncontardo).Id;
                    int idInventario = repositorioInventario.Modificar_Transaccionadamente(inventarioEncontrado).Id;
                    int idDetalle = repositorioInventarioDetalle.Modificar_Transaccionadamente(registroSinIncidencia).Id;

                    if ( idContenedor > 0 && idInventario > 0 && idDetalle > 0)
                    {
                        foliosInhabilitados += 1;
                        transaccion.GuardarCambios();
                    }
                }

            }


            return foliosInhabilitados;
        }

        public static int AsignarContenedor(int IdPersonal,int IdContenedor)
        {
            var transaccion = new Transaccion();

            var repositorio = new Repositorio<Tbl_InventarioDetalle>(transaccion);
            List<Tbl_InventarioDetalle> inhabilitarFoliosEncontrados = repositorio.ObtenerPorFiltro(x => x.IdContenedor == IdContenedor && x.IdIncidencia == null && x.Activo == true).ToList();

            var repoPersonal = new Repositorio<Tbl_InventarioAsignacionPersonal>(transaccion);
            int IdEmpleado = repoPersonal.Obtener(x => x.IdEmpleado == IdPersonal).Id;


            int foliosInhabilitados = 0;
            foreach (Tbl_InventarioDetalle nuevoDetalle in inhabilitarFoliosEncontrados)
            {
                nuevoDetalle.IdIncidencia = 2 /*Asignado a Chequera*/;
                nuevoDetalle.FechaIncidencia = DateTime.Now;
                nuevoDetalle.IdEmpleado = IdEmpleado;
                nuevoDetalle.Tbl_InventarioContenedores.FormasDisponiblesActuales -= 1;
                nuevoDetalle.Tbl_InventarioContenedores.FormasAsignadas += 1;
                nuevoDetalle.Tbl_InventarioContenedores.Tbl_Inventario.FormasDisponibles -= 1;
                int id = repositorio.Modificar(nuevoDetalle).Id;

                if (id > 0)
                    foliosInhabilitados += 1;
            }
            return foliosInhabilitados;
        }













        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /******************************************************                DetalleBanco            *************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        public static IEnumerable<Tbl_InventarioDetalle> obtenerDetallesTabla(int IdInventario )
        {
            var transaccion = new Transaccion();
            var repositorioContenedor = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repositorioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);


            var contenedoresEncontrados = repositorioContenedor.ObtenerPorFiltro(x => x.IdInventario == IdInventario && x.FormasDisponiblesActuales > 0 && x.Activo == true);

            List<int> IdContenedores = new List<int>();

            foreach (Tbl_InventarioContenedores nuevoContenedor in contenedoresEncontrados)
            {
                IdContenedores.Add(nuevoContenedor.Id);     
            }

           
                int idContenedorInicial = IdContenedores.Min();
                int idContenedorFinal = IdContenedores.Max();

                var detallesEncontradosContenedores = repositorioDetalle.ObtenerPorFiltro(x => x.IdContenedor >= idContenedorInicial && x.IdContenedor <= idContenedorFinal && x.Activo == true);

           
            return detallesEncontradosContenedores;
        }



        public static IQueryable<Tbl_InventarioDetalle> Obtener_TodosLosDetallesCargarDetalleBanco(int IdInventario )
        {
            var transaccion = new Transaccion();
            var repositorioTblConte = new Repositorio<Tbl_InventarioContenedores>(transaccion);

            var contenedoresEncontrados = repositorioTblConte.ObtenerPorFiltro( x => x.IdInventario == IdInventario && x.Activo == true).Select(x => x.Id);

            var repositorioTblDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion) ;

          return   repositorioTblDetalle.ObtenerPorFiltro(x => x.IdContenedor >= contenedoresEncontrados.Min() && x.IdContenedor <= contenedoresEncontrados.Max());
        }

   








        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /******************************************************               BTN descargar reporte de Inventario             *************************************************************************************/
        /****************************************************************************************************************************************************************************/
        /****************************************************************************************************************************************************************************/


        public static List<ReporteInventarioGeneralDTO> ObtenerInventarioGeneralDatosReporte(int MesSelecionado, int AnioCurso)
        {
            var transaccion = new Transaccion();
            var repositorioCuentaBancaria = new Repositorio<Tbl_CuentasBancarias>(transaccion);
            var repositorioBanco = new Repositorio<Tbl_Inventario>(transaccion);
            var repositorioContenedor = new Repositorio<Tbl_InventarioContenedores>(transaccion);
            var repositorioDetalle = new Repositorio<Tbl_InventarioDetalle>(transaccion);

            //obtener inventario para usar sus ID's
            var inventarioEncontrado = repositorioBanco.ObtenerPorFiltro(x => x.FormasDisponibles > 0 && x.Activo == true);


            List<ReporteInventarioGeneralDTO> datosReporteInventarioGeneral = new List<ReporteInventarioGeneralDTO>();
            // List<int> IdContenedor;

            foreach (var inventario in inventarioEncontrado)
            {
                //obtener contenedores por el Id de su inventario
                var contenedoresEncontradosPorIdInventario = repositorioContenedor.ObtenerPorFiltro(x => x.IdInventario == inventario.Id).ToList();

                //selecciona los ids de los contenedores encontrados
                var idsContenedoresEncontrados = contenedoresEncontradosPorIdInventario.Select(x => x.Id).ToList();

                //obtiene los registros de numeros de folios encontrados 
                var formasDePagoConsumidadMes = repositorioDetalle.ObtenerPorFiltro(x => x.IdContenedor >= idsContenedoresEncontrados.Min() && x.IdContenedor <= idsContenedoresEncontrados.Max() && x.IdIncidencia != null && x.FechaIncidencia.Value.Year == AnioCurso && x.FechaIncidencia.Value.Month == MesSelecionado && x.Activo == true).ToList();

               
                var detellesEncontrados = repositorioDetalle.ObtenerPorFiltro(x => x.IdContenedor >= idsContenedoresEncontrados.Min() && x.IdContenedor <= idsContenedoresEncontrados.Max()  && x.IdIncidencia == null && x.Activo == true).Select(y => y.Id).ToList();
                 
                    Tbl_CuentasBancarias cuentaEncontrada = repositorioCuentaBancaria.Obtener(x => x.IdInventario == inventario.Id && x.Activo == true); 
                    ReporteInventarioGeneralDTO nuevoDato = new ReporteInventarioGeneralDTO();


                if (detellesEncontrados.Count() > 0) {

                    // var a =  repositorioDetalle.Obtener(x => x.Id == detellesEncontrados.Min()).NumFolio;
                    int idMinimo = detellesEncontrados.Min();
                    int idMaximo = detellesEncontrados.Max();
                    //Tbl_InventarioDetalle detalleminimo = repositorioDetalle.Obtener(x => x.Id == idMinimo);


                    nuevoDato.NombreBanco = cuentaEncontrada.NombreBanco;
                    nuevoDato.Cuenta = cuentaEncontrada.Cuenta;
                    nuevoDato.FolioInicialExistente = repositorioDetalle.Obtener(x => x.Id == idMinimo).NumFolio;
                    nuevoDato.FolioFinalExistente = repositorioDetalle.Obtener(x => x.Id == idMaximo).NumFolio;
                    nuevoDato.TotalFormasPago = detellesEncontrados.Count();
                    nuevoDato.ConsumoMensualAproximado = Convert.ToString( formasDePagoConsumidadMes.Count());

                    //if (inventario.FormasUsadasQuincena1 != null && inventario.FormasUsadasQuincena2 != null)
                    //{
                    //    nuevoDato.ConsumoMensualAproximado = Convert.ToString(inventario.FormasDisponibles / (inventario.FormasUsadasQuincena1 + inventario.FormasUsadasQuincena2));
                    //}
                    //if (inventario.FormasUsadasQuincena1 != null)
                    //{
                    //    nuevoDato.ConsumoMensualAproximado = Convert.ToString(inventario.FormasDisponibles / (inventario.FormasUsadasQuincena1 * 2));
                    //}

                    //Saber si se necesitan pedir formas de pago a cada 4 meses 
                    if (nuevoDato.ConsumoMensualAproximado != null && Convert.ToInt32(nuevoDato.ConsumoMensualAproximado) != 0)
                    {
                        if ((inventario.FormasDisponibles / Convert.ToDecimal(nuevoDato.ConsumoMensualAproximado)) <= 4)
                        {
                            nuevoDato.SolicitarFormas = "si";
                        }else
                        {
                            nuevoDato.SolicitarFormas = "No";

                        }
                    }
                    else 
                    {
                        nuevoDato.SolicitarFormas = "No";

                    }



                      //  nuevoDato.SolicitarFormas = ( inventario.FormasDisponibles / Convert.ToDecimal(nuevoDato.ConsumoMensualAproximado)) <= 4 ? nuevoDato.SolicitarFormas = "si" : nuevoDato.SolicitarFormas = "No";


                    datosReporteInventarioGeneral.Add(nuevoDato);

                }
               

            }
            return datosReporteInventarioGeneral;
        }



        /***************************************** Obtener Nombre y cuenta bancaria Correcta *******************************************/
        public static string ObtenerNombreBanco(int IdCuentaBancaria)
        {
            var transaccion = new Transaccion();
            var repositorio = new Repositorio<Tbl_CuentasBancarias>(transaccion);

            Tbl_CuentasBancarias bancoEncontrado = repositorio.Obtener(x =>  x.Id == IdCuentaBancaria && x.Activo == true  );


            return bancoEncontrado.NombreBanco + " || " + bancoEncontrado.Cuenta;
        }

    }
}
