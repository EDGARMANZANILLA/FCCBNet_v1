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
            // return @"Data Source=172.19.2.31; Initial Catalog=Nomina; User=sa; PassWord=s3funhwonre2";
            // return @"Data Source=172.19.3.170; Initial Catalog=Nomina; User=sa; PassWord=dbadmin";
             return @"Data Source=172.19.62.71; Initial Catalog=Nomina; User=sa; PassWord=dbadmin";
        }


        public static string obtenerCadenaConexionDeploy()
        {
            // return @"Data Source=172.19.2.31; Initial Catalog=Nomina; User=sa; PassWord=s3funhwonre2";
            // return @"Data Source=172.19.3.170; Initial Catalog=Nomina; User=sa; PassWord=dbadmin";
             return @"Data Source=172.19.62.71; Initial Catalog=Nomina; User=sa; PassWord=dbadmin";
        }
        //public static string obtenerCadenaConexionDeployEXCEPCION()
        //{
        //    //return @"Data Source=172.19.2.31; Initial Catalog=Nomina; User=sa; PassWord=s3funhwonre2";
        //    // return @"Data Source=172.19.3.170; Initial Catalog=Nomina; User=sa; PassWord=dbadmin";
        //    return @"Data Source=172.19.62.71; Initial Catalog=Nomina; User=sa; PassWord=dbadmin";
        //}

        public static string ObtenerNombreDBValidacionFoliosDeploy() 
        {
            //return "FoliacionDeploy";
            return "FCCBNetDB";
        }
    }
}
