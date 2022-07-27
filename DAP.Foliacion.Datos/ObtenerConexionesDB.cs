using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Datos
{
    public class ObtenerConexionesDB
    {
        public static string obtnercadenaConexionAlpha()
        {
            return @"Data Source=172.19.2.31; Initial Catalog=Nomina; User=sa; PassWord=s3funhwonre2";
        }


        public static string obtenerCadenaConexionLocalInterfaces()
        {
            return @"Data Source=172.19.62.71; Initial Catalog=Interfaces; User=sa; PassWord=dbadmin";
        }


        //cambiar cadena de conexion por la de interafces en alpha (actualmente apunta en local por que esta en pruebas) 
        public static string obtenerCadenaConexionInterfacesAlpha()
        {
            return @"Data Source=172.19.62.71; Initial Catalog=Interfaces; User=sa; PassWord=dbadmin";
        }


        /************************************************************************************************/
        /**********************     al subir el proyecto hay que ********************************************************/
        /************************************************************************************************/

        public static string obtenerCadenaConexionProductiva()
        {
            return @"Data Source=172.19.3.31; Initial Catalog=Nomina; User=sa; PassWord=s3funhwonre2";
        }

        public static string obtenerCadenaConexionDeploy()
        {
            return @"Data Source=172.19.3.170; Initial Catalog=Nomina; User=sa; PassWord=dbadmin";
        }

        public static string ObtenerNombreDBValidacionFoliosDeploy() 
        {
            //return "FoliacionDeploy";
            return "FCCBNetDB";
        }
    }
}
