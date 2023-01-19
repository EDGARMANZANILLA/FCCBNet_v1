using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAP.Foliacion.Entidades
{
    public class Class1
    {
        public int IteradorDeContenedores { get; set; }

        public string FInicial { get; set; }

        public string FFinal { get; set; }

        public int TotalFormas { get; set; }




        //public static List<VerificarFoliacionNominasQuincenaDTO> VerificacionFoliacionNominasQuincena(string Quincena)
        //{
        //    List<VerificarFoliacionNominasQuincenaDTO> listaVerificacion = new List<VerificarFoliacionNominasQuincenaDTO>();
        //    int anioInterfasQuincena = ObtenerAnioDeQuincena(Quincena);
        //    int quincenaSeleccionada = Convert.ToInt32(Quincena);
        //    string VisitaAnioInterfas = ObtenerCadenaAnioInterface(anioInterfasQuincena);

        //    List<DatosCompletosBitacoraDTO> nominasEnQuincena = ObtenerDatosCompletosBitacoraFILTRO(VisitaAnioInterfas, Quincena);
        //    int iterador = 0;
        //    foreach (DatosCompletosBitacoraDTO Nomina in nominasEnQuincena)
        //    {
        //        VerificarFoliacionNominasQuincenaDTO nuevaVerificacion = new VerificarFoliacionNominasQuincenaDTO();
        //        nuevaVerificacion.Id = iterador += 1;
        //        nuevaVerificacion.Id_Nom = Nomina.Id_nom;
        //        nuevaVerificacion.Comentario = Nomina.Coment;
        //        nuevaVerificacion.EstaImportado = Nomina.Importado;

        //        if (nuevaVerificacion.EstaImportado)
        //        {
        //            nuevaVerificacion.TotalRegistros = FoliarConsultasDBSinEntity.ObtenerTotalRegistrosEnANDeNomina(VisitaAnioInterfas, Nomina.An);


        //            //REGISTROS TOTALES CONTENIDOS EN FCCBNetDB
        //            Transaccion transaccion = new Transaccion();
        //            var repoTblPagos = new Repositorio<Tbl_Pagos>(transaccion);
        //            int totalRegistradosFCCBNet = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioInterfasQuincena && x.Quincena == quincenaSeleccionada && x.Id_nom == Nomina.Id_nom).Count();


        //            if (nuevaVerificacion.TotalRegistros == totalRegistradosFCCBNet)
        //            {
        //                nuevaVerificacion.EstaFoliadoCorrectamente = true;
        //            }
        //            else
        //            {
        //                nuevaVerificacion.EstaFoliadoCorrectamente = false;
        //                nuevaVerificacion.RegistrosNoFoliados = nuevaVerificacion.TotalRegistros - totalRegistradosFCCBNet;


        //                List<string> bancosContenidosEnAn = FoliarConsultasDBSinEntity.VerificarCamposBancoContieneAN(Nomina.An, Nomina.Anio);
        //                //REGISTROS TOTALES CONTENIDOS EN AN
        //                //Cheques
        //                string condicionBancosCheques = consultasCheques.ConvertirListaBancosEnCondicionParaCheques(bancosContenidosEnAn);
        //                int totalRegistrosCheques = FoliarConsultasDBSinEntity.ObtenerTotalRegistrosNoFoliadosSegunCondicion(VisitaAnioInterfas, Nomina.An, condicionBancosCheques);
        //                //Pagomaticos
        //                string condicionesBancosPagomatico = consultasPagomaticos.ConvertirListaBancosEnCondicionParaPagomaticos(bancosContenidosEnAn);
        //                int totalRegistrosPagomaticos = FoliarConsultasDBSinEntity.ObtenerTotalRegistrosNoFoliadosSegunCondicion(VisitaAnioInterfas, Nomina.An, condicionesBancosPagomatico);

        //                //REGISTROS CONTENIDOS EN FCCBNetDB
        //                int pagomaticosRegistradosFCCBnetDB = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioInterfasQuincena && x.Quincena == quincenaSeleccionada && x.Id_nom == Nomina.Id_nom && x.IdCat_FormaPago_Nacimiento == 2).Count();
        //                int chequesRegistradosFCCBnetDB = repoTblPagos.ObtenerPorFiltro(x => x.Anio == anioInterfasQuincena && x.Quincena == quincenaSeleccionada && x.Id_nom == Nomina.Id_nom && x.IdCat_FormaPago_Nacimiento == 1).Count();



        //                nuevaVerificacion.Cheques = totalRegistrosCheques - chequesRegistradosFCCBnetDB;
        //                nuevaVerificacion.Pagomaticos = totalRegistrosPagomaticos - pagomaticosRegistradosFCCBnetDB;
        //            }

        //        }



        //        listaVerificacion.Add(nuevaVerificacion);
        //    }

        //    return listaVerificacion;
        //}
    }
}
