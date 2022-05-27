using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;
using Datos;

namespace Datos
{
   public class RepositorioSQL
   {
        private DbCommand comando = null;

        private TransaccionSQL _ContextoTransaccionalBD;



        public RepositorioSQL( TransaccionSQL Transaccion_Sql )
        {
            _ContextoTransaccionalBD = Transaccion_Sql;
        }




        /*************************************************************************************************************************************************************************************/
        /*************************************************************************************************************************************************************************************/
        /********************************************************       METODOS SIMPLES SIN TRANSACCION        *******************************************************************************/
        /*************************************************************************************************************************************************************************************/
        /*************************************************************************************************************************************************************************************/





        /// <summary>
        /// Ejecuta una consulta en base a una sentencia SQL y retorna el resultado de la consulta.
        /// Ejemplo:
        /// <code>SELECT * FROM Tabla WHERE campo1=@campo1, campo2=@campo2</code>
        /// </summary>        
        /// <param name="sentenciaSQL">La sentencia SQL </param>
        /// <exception cref="BaseDatosException">Si ocurre un error al ejecutar el comando.</exception>
        /// <returns>El resultado de la consulta.</returns>
        public DbDataReader Ejecutar_Leer(string SentenciaSQL)
        {
            try
            {
                this.comando = _ContextoTransaccionalBD.factory.CreateCommand();
                this.comando.Connection = _ContextoTransaccionalBD.conexion;
                this.comando.CommandType = CommandType.Text;
                this.comando.CommandText = SentenciaSQL;
                return this.comando.ExecuteReader();
            }
            catch (Exception E) 
            {
                throw new Exception("Error al ejecutar la sentencia sql", E);
            }
        }





        /// <summary>
        /// Ejecuta una ACTUALIZACION, INSERCION O BORRADO de una consulta sql "SIMPLE"
        /// </summary>
        /// <param name="SentenciaSQL"></param>
        /// <returns></returns>
        public int Ejecutar_Upadate_Delete_Insert(string SentenciaSQL)
        {
            try
            {

                this.comando = _ContextoTransaccionalBD.factory.CreateCommand();
                this.comando.Connection = _ContextoTransaccionalBD.conexion;
                this.comando.CommandType = CommandType.Text;
                this.comando.CommandText = SentenciaSQL;
                return this.comando.ExecuteNonQuery();
            }
            catch (Exception E)
            {
                throw new Exception("Error al ejecutar la sentencia sql", E);
            }
        }








        /*************************************************************************************************************************************************************************************/
        /*************************************************************************************************************************************************************************************/
        /***************************************************************      METODOS TRANSACCIONADOS     ************************************************************************************/
        /*************************************************************************************************************************************************************************************/
        /*************************************************************************************************************************************************************************************/


        /// <summary>
        /// Ejecuta Una ACTUALIZACION, INSERCION O BORRADO de una consulta sql "TRASACCIONADAMENTE"
        /// </summary>
        /// <param name="SentenciaSQL">Consulta en lenguaje transac</param>
        /// <param name="Transaccion">recibe una transaccion activa </param>
        public int Ejecutar_Upadate_Delete_Insert_Transaccionado(string SentenciaSQL, DbTransaction Transaccion)
        {
            try 
            {
                this.comando = _ContextoTransaccionalBD.factory.CreateCommand();
                this.comando.Connection = _ContextoTransaccionalBD.conexion;
                this.comando.CommandType = CommandType.Text;
                this.comando.CommandText = SentenciaSQL;
                    if (Transaccion != null)
                    {
                        this.comando.Transaction = Transaccion;
                    }
                return  this.comando.ExecuteNonQuery();
            }
            catch (Exception E)
            {
                throw new Exception("Error al ejecutar la sentencia sql transaccionado", E);
            }
        }














    }
}
