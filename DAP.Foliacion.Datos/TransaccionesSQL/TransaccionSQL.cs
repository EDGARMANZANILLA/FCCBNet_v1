using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;

namespace Datos
{
    public class TransaccionSQL
    {
        public DbConnection conexion = null;       
        public DbTransaction transaccion = null;
        public string cadenaConexion;
        public DbProviderFactory factory = null;



        public TransaccionSQL(string NombreDB)
        {
            Configurar(NombreDB);
          
        }
    
        private void Configurar(string NombreDB)
        {
            try
            {
                string proveedor = ConfigurationManager.AppSettings.Get(NombreDB);
                this.cadenaConexion = ConfigurationManager.ConnectionStrings[NombreDB].ConnectionString;
                factory = DbProviderFactories.GetFactory(proveedor);
            }
            catch (ConfigurationException ex)
            {
                throw new Exception("Error al cargar la configuración de la transaccion.", ex);
            }
        }

      


        /*******************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************/
        /**************************************************        METODOS DE CONEXION Y DESCONEXION          **************************************************************************/
        /*******************************************************************************************************************************************************************************/
        /*******************************************************************************************************************************************************************************/


        /// <summary>
        /// Se concecta a base de datos que anteriormente se instancio al pasar el nombre de la DB alojada como una variable local del proyecto.
        /// </summary>
        public void Conectar()
        {
            if (this.conexion != null && !this.conexion.State.Equals(ConnectionState.Closed))
            {
                throw new Exception("La conexión ya se encuentra abierta.");
            }
            try
            {
                if (this.conexion == null)
                {
                    this.conexion = factory.CreateConnection();
                    this.conexion.ConnectionString = cadenaConexion;
                }
               this.conexion.Open();
            }
            catch (DataException ex)
            {
                throw new Exception("Error al conectarse a la base de datos.", ex);
            }
        }





        /// <summary>
        /// Permite desconectarse de la base de datos que anteriormente se instancio al pasar el nombre de la DB alojada como una variable local del proyecto.
        /// </summary>
        public void Desconectar()
        {
            if (this.conexion.State.Equals(ConnectionState.Open))
            {
                this.conexion.Close();
            }
        }







        /**********************************************************************************************************************************************************************/
        /**********************************************************************************************************************************************************************/
        /****************************************************   METODOS PARA REALIZAR TRANSACCIONES ***************************************************************************/
        /**********************************************************************************************************************************************************************/
        /**********************************************************************************************************************************************************************/



        /// <summary>
        /// Metodo que se utiliza para transaccionar una conexion abierta.
        /// Todo lo que se ejecute luego de esta ionvocación estará dentro de una tranasacción.
        /// </summary>
        public void IniciarTransaccion()
        {
            Conectar();

            if (this.transaccion == null)
            {
                this.transaccion = this.conexion.BeginTransaction();
            }
        }


        /// <summary>
        /// Confirma todo los comandos ejecutados entre el <c>ComanzarTransaccion</c> y ésta invocación.
        /// </summary>
        public void GuardarTransaccion()
        {
            if (this.transaccion != null)
            {
                this.transaccion.Commit();
                Desconectar();
            }
        }




        /// <summary>
        /// Cancela la ejecución de una transacción.
        /// Todo lo ejecutado entre ésta invocación y su correspondiente <c>ComenzarTransaccion</c> será perdido.
        /// </summary>
        public void CancelarTransaccion()
        {
            if (this.transaccion != null)
            {
                this.transaccion.Rollback();
                Desconectar();
            }
        }

        /// <summary>
        /// Se invoca despues de haber invocado en metodo InicialTransaccion
        /// </summary>
        /// <returns>Regresa una DbTransaction de la transaccion que antes se incio</returns>
        public DbTransaction GetTransaction()
        {
            return this.transaccion;
        }



    }
}
