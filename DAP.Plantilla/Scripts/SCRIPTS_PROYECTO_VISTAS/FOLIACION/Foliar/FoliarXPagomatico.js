




function DibujarTablaEsFoliada() {
    $("#TablaEsFoliada").append(

        "<table id='DetallesSiEstaFoliadaNominas'    class='table table-striped table-bordered table-hover' cellspacing='0' style='width:100%'     >" +
        " <caption class='text-uppercase'>Resumen de nominas disponibles en la quincena</caption>" +
        "<thead class='tabla'>" +

        "<tr>" +
        "<th>IdNom</th>" +
        "<th>Nomina</th>" +
        "<th>Adicional</th>" +
        "<th>Nombre nomina</th>" +
        "<th>Registros a foliar</th>" +
        "<th>Esta Foliada</th>" +
        "<th>Foliar</th>" +
        "<th>Revisar PDF</th>" +
        "</tr>" +

        "</thead>" +
        "</table>"
    );
};

function PintarResultadoEsFoliada(datos) {

    $('#DetallesSiEstaFoliadaNominas').DataTable({
        "dom": 'Blfrtip',
        "buttons": [
            {
                extend: 'excel',
                className: 'btn btn-primary ',
                text: '<i class="fas fa-file-download"></i>  &nbsp  EXCEL',
                filename: `Detalle_Nominas_EXEL`,
                title: `DETALLE CON LAS SIGUIENTES NOMINAS : `
            },
            {
                extend: 'pdf',
                className: 'btn btn-primary ',
                text: ' <i class="fas fa-download"></i>  &nbsp PDF ',
                filename: `Detalle_Nominas_PDF`,
                title: `DETALLE CON LAS SIGUIENTES NOMINAS :`
            },
            {
                extend: 'print',
                className: 'btn btn-primary ',
                text: '<i class="fas fa-print"></i>  &nbsp Imprimir',
                filename: `Detalle_Nominas_`,
                title: `DETALLE CON LAS SIGUIENTES NOMINAS: `
            }
        ],
        "bDestroy": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "paging": true,
        "lengthMenu": [5, 15, 20, 40],
        "language":
        {
            "processing": "Procesando...",
            "lengthMenu": "Mostrar _MENU_ registros",
            "zeroRecords": "No se encontraron resultados",
            "emptyTable": "Ningún dato disponible en esta tabla",
            "infoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "infoFiltered": "(filtrado de un total de _MAX_ registros)",
            "search": "Buscar:",
            "info": "Mostrando de _START_ a _END_ de _TOTAL_ entradas",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "data": datos,
        "columns": [


            { "data": "Id_Nom" },
            { "data": "NumeroNomina" },
            { "data": "Adicional" },
            { "data": "NombreNomina" },
            { "data": "NumeroRegistrosAFoliar" },
            {
                "data": "IdEstaFoliada",
                render: function (data, type, row) {
                    // console.log("hola :" + typeof data);
                    // console.log( data);
                    //  console.log( data.row.IdAtencion);
                    if (data == 0) {
                        return '<h4 class="text-danger text-uppercase"> <i class="fas fa-times-circle"></i>  </h4>';
                    }
                    else if (data == 1) {

                        return '<h4 class="  text-success text-uppercase"> <i class="fas fa-check"></i>  </h4>';
                    } else if (data == 2) {

                        return '<button class="btn btn-info  text-uppercase text-light" onclick="BaseSinPagomaticos()"  > S/N PAGOMATICOS  </button>';
                    } else if (data == 3) {

                        return '<button class=" btn-danger btn text-uppercase text-light"  onclick="BaseNoImportada()" > Base no importada </button>';
                    }
                    else if (data == 4) {

                        return '<button class=" btn-danger btn text-uppercase text-light"  onclick="ErrorSinRegistrosEnFCCBNet()"> ERROR </button>';
                    }
                    else if (data == 5) {

                        return '<button class="btn btn-warning text-uppercase text-light"  onclick="RefoliarPagomatico('+row.Id_Nom+')"> Algo sucedio, ¡verifique! </button>';
                    }
                }
            },
            {
                render: function (data, type, row) {
                    //console.log(row);
                    if (row.IdEstaFoliada == 0) {
                        return '<h4 class="bg-success btn  text-uppercase text-light" onclick="FoliarNomina(' + row.Id_Nom + ')" > Foliar </h4>';
                    }
                    return '';
                }


            },
            {
                render: function (data, type, row) {
                    if (row.IdEstaFoliada < 2) {
                        //SOLO ENTRAN LAS NOMINAS QUE NO ESTEN FOLIADOS O LOS CUALES SI ESTAN FOLIADOS
                        return '<h4 class="bg-success btn  text-uppercase text-light"  onclick="ImprimeNomina(' + row.Id_Nom + ')"  > <i class="fas fa-print"></i> </h4>';
                    }
                    return '';
                }

            }

        ],
        "columnDefs": [


            { className: "text-center col-1", visible: true, "targets": 0, },
            { className: "text-center col-1", visible: true, "targets": 1, },
            { className: "text-center col-2", visible: true, "targets": 2, },
            { className: "text-center col-3", visible: true, "targets": 3, },
            { className: "text-center col-1", visible: true, "targets": 4, },
            { className: "text-center col-2", visible: true, "targets": 5, },
            { className: "text-center col-1", visible: true, "targets": 6, },
            { className: "text-center col-1", visible: true, "targets": 7, }


        ],

        "order": [[1, 'asc']]


    });

};





function LimpiarTablaResumenFoliados()
{
    $('#TablaEsFoliada').empty();
}




function VerificarNominaPagomatico() {

    LimpiarTablaResumenFoliados();

    let VerificaIdNom = document.getElementById("SeleccionarNominaFoliar").value;
    let quincenaPagomaticoNom = document.getElementById("QuincenaFoliacion").value;
   // console.log(quincenaPagomaticoNom);


    MensajeCargando();
    axios.post('/Foliar/EstaFoliadaIdNominaPagomatico', {
        IdNom: VerificaIdNom ,
        NumeroQuincena: quincenaPagomaticoNom
    })
    .then(function (response) {

        if (response.data.RespuestaServidor == "201") {
            $('#TablaEsFoliada').empty();
            DibujarTablaEsFoliada();
            PintarResultadoEsFoliada(response.data.DetalleTabla);
        } else if (response.data.RespuestaServidor == "500") {
            MensajeErrorSweet(response.data.Error);
        }

        //OcultarMensajeCargando();
    })
        .catch(function (error) {
            MensajeErrorSweet("Intente la verificar de nuevo", "Ocurrio un error " + error);
          
    });
    OcultarMensajeCargando();

   

}

function VerificarPagomaticoTodasNominas() {

    LimpiarTablaResumenFoliados();
    let quincenaPagomatico = document.getElementById("QuincenaFoliacion").value;
   // console.log(quincenaPagomatico);

    MensajeCargando();
    axios.post('/Foliar/EstanFoliadasTodasNominaPagomatico', {
        NumeroQuincena:  quincenaPagomatico
    })
        .then(function (response) {

            if (response.data.RespuestaServidor == "201") {
                $('#TablaEsFoliada').empty();
                DibujarTablaEsFoliada();
                PintarResultadoEsFoliada(response.data.DetalleTabla);
            } else if (response.data.RespuestaServidor == "500") {
                MensajeErrorSweet(response.data.Error);
            }

            OcultarMensajeCargando();
        })
        .catch(function (error) {
            MensajeErrorSweet("Intente la verificar de nuevo", "Ocurrio un error " + error);
            OcultarMensajeCargando();

        });
  



}




function CheckNomina() {
    let checkNom = document.getElementById('checkNomina').checked;

    if (checkNom) {
        document.getElementById('checkTodasNominas').checked = false;
        document.getElementById('DetalleTodasLasNominas').style.display = 'none';

        $('#TablaEsFoliada').empty();

        document.getElementById('SeleccionarNominaFoliar').style.display = "block";
        document.getElementById('VerificarNomina').style.display = "block";




    } else {
        document.getElementById('checkNomina').checked = false;


        document.getElementById('SeleccionarNominaFoliar').style.display = "none";
        document.getElementById('VerificarNomina').style.display = "none";
    }

    //console.log(checkNom)
}

function CheckTodasNominas() {
    let checkTodasNom = document.getElementById('checkTodasNominas').checked;

    if (checkTodasNom) {
        document.getElementById('checkNomina').checked = false;
        document.getElementById('SeleccionarNominaFoliar').style.display = "none";
        document.getElementById('VerificarNomina').style.display = "none";

        $('#TablaEsFoliada').empty();

        document.getElementById('DetalleTodasLasNominas').style.display = 'Block';

    } else {
        document.getElementById('checkTodasNominas').checked = false;

        document.getElementById('DetalleTodasLasNominas').style.display = 'none';
    }

}

function FolearQuincenaPagomatico()
{
   //VerificarPagomaticoTodasNominasDisponiblesFoliarPagomaticos();

    let quincenaParaFoliar = document.getElementById("QuincenaFoliacion").value;

    MensajeCargando();
    axios.post('/Foliar/VerificarNominasQuincaDisponiblesFoliarPagomaticos', {
        NumeroQuincena: quincenaParaFoliar
    })
        .then(function (response) {

            //console.log(response.data);

            LimpiarTablaResumenFoliados();
            DibujarTablaEsFoliada();
            PintarResultadoEsFoliada(response.data);
            OcultarMensajeCargando();




            Swal.fire({
                title: '¿Estas seguro , seguro ?',
                text: 'Este proceso tomara un tiempo largo de espera dado a que se folean todos los pagomaticos de toda la quincena',
                icon: 'info',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Si, Continuar!',
                cancelButtonText: 'No, Cancelar!',
                footer: '<a href="#">Contactar al desarrollador?</a>'
            }).then((result) => {
                if (result.isConfirmed) {
                    MensajeCargando();
                    axios.post('/Foliar/FoliarQuincenaPagomatico', {
                        NumeroQuincena: quincenaParaFoliar
                    })
                        .then(function (response) {

                            if (response.data.bandera) {
                                MensajeCorrectoSweet(response.data.mensaje);
                            } else {
                                MensajeErrorSweet(response.data.mensaje, 'INCIDENCIAS ENCONTRADAS');
                            }

                            LimpiarTablaResumenFoliados();
                            DibujarTablaEsFoliada();
                            PintarResultadoEsFoliada(response.data.resultadoServer);
                            OcultarMensajeCargando();

                        })
                        .catch(function (error) {
                            MensajeErrorSweet(error);
                            $('#TablaEsFoliada').empty();
                            OcultarMensajeCargando();
                            console.log(error);
                        });



                }
            })



        })
    .catch(function (error) {
            MensajeErrorSweet(error);
            LimpiarTablaResumenFoliados();
            OcultarMensajeCargando();

            console.log(error);
    });

 



}




function FoliarNomina(IdNom) {

    let quincenaParaFoliar = document.getElementById("QuincenaFoliacion").value;
    //console.log(quincenaParaFoliar);

    Swal.fire({
        title: '¿Estas seguro de querer foliar?',
        text: "",
        icon: 'warning',
        showCancelButton: true,
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, continuar!',
        cancelButtonText: 'No, cancelar!',
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {


            MensajeCargando();
            axios.post('/Foliar/FoliarPorIdNominaPagomatico', {
                IdNomina: IdNom ,
                NumeroQuincena: quincenaParaFoliar
            })
                .then(function (response) {
                    if (response.data.bandera) {
                        $('#TablaEsFoliada').empty();
                        DibujarTablaEsFoliada();
                        PintarResultadoEsFoliada(response.data.resultadoServer.Data.DetalleTabla);
                        MensajeCorrectoSweet("La nomina se folio correctamente");

                    } else {
                        MensajeErrorSweet(response.data.DBFAbierta[0].Detalle, response.data.DBFAbierta[0].Solucion);
                    }

                    OcultarMensajeCargando();
            })
                .catch(function (error) {
                    MensajeErrorSweet("Ocurrio un error intente de nuevo " + error)
                OcultarMensajeCargando();
            });


            //let EnviarRevicion = "{'IdNomina': '" + IdNom + "','NumeroQuincena': '" + quincenaParaFoliar + "'}";

            //MensajeCargando();
            //$.ajax({
            //    url: 'Foliar/FoliarPorIdNominaPagomatico',
            //    data: EnviarRevicion,
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    success: function (response) {




            //        if (response.bandera) {
            //            $('#TablaEsFoliada').empty();
            //            DibujarTablaEsFoliada();
            //            PintarResultadoEsFoliada(response.resultadoServer.Data.DetalleTabla);
            //            MensajeCorrectoSweet("La nomina se folio correctamente");

            //        } else {
            //            MensajeErrorSweet(response.DBFAbierta[0].Detalle, response.DBFAbierta[0].Solucion);
            //        }

            //        OcultarMensajeCargando();


            //    }, error: function (jqXHR, textStatus) {
            //        OcultarMensajeCargando();
            //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)

            //    }
            //});



        } else if (result.dismiss === Swal.DismissReason.cancel) {

            MensajeInformacionSweet('Cancelado con exito', 'La base esta a salvo sin ninguna modificacion :)');

        }
    })


}

function ImprimeNomina(IdNom) {

    let quincenaParaImprimir = document.getElementById("QuincenaFoliacion").value;
    //console.log(quincenaParaImprimir);

    MensajeCargando();
    axios.post('/Foliar/RevisarReportePDFPagomaticoPorIdNomina', {
        IdNomina: IdNom ,
        Quincena: quincenaParaImprimir
    })
    .then(function (response) {

            $("example").empty();
            /// PDFObject.embed(response, "#example");
            PDFObject.embed("data:application/pdf;base64," + response.data + " ", "#example");
            //PDFObject.embed("../Reportes/ReportesPDFSTemporales/RevicionNomina" + nominaSeleccionadaFoliar.value + ".pdf", "#example");
            $('#ModalPDFVisualizador').modal('show');

            OcultarMensajeCargando();
    })
    .catch(function (error) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo " + error)
            OcultarMensajeCargando();
    });

    let EnviarRevicion = "{'IdNomina': '" + IdNom + "', 'Quincena': '" + quincenaParaImprimir + "'}";


    //MensajeCargando();
    //$.ajax({
    //    url: 'Foliar/RevisarReportePDFPagomaticoPorIdNomina',
    //    data: EnviarRevicion,
    //    type: "POST",
    //    contentType: "application/json; charset=utf-8",
    //    success: function (response) {


    //        $("example").empty();
    //        /// PDFObject.embed(response, "#example");
    //        PDFObject.embed("data:application/pdf;base64," + response + " ", "#example");
    //        //PDFObject.embed("../Reportes/ReportesPDFSTemporales/RevicionNomina" + nominaSeleccionadaFoliar.value + ".pdf", "#example");
    //        $('#ModalPDFVisualizador').modal('show');

    //        OcultarMensajeCargando();
    //    }, error: function (jqXHR, textStatus) {
    //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus + " || "+ jqXHR)
    //        OcultarMensajeCargando();
    //    }
    //});

   // OcultarMensajeCargando();

}


function BaseNoImportada()
{
    MensajeErrorSweet('Informe al personal encargado de subir las bases ','NO SE IMPORTO LA BASE DBF HACIA SQL');
}


function BaseSinPagomaticos()
{
    MensajeWarningSweet('Esta base al parecer no contiene ningun pago con tarjeta', 'NO HAY PAGOMATICOS POR FOLIAR EN ESTA BASE')
}

function ErrorSinRegistrosEnFCCBNet()
{
    //4 LA BASE EN SQL ESTA FOLIADA POR ALGUNA RAZON, PERO NO HAY REGISTRO EN FCCBNetDB => VErificar con el desarrollador por que sucedio (Se resuelve limpiando la base de sql de AN)
    MensajeErrorSweet("Limpie los campos de foliacion de la base AN en SQL", "LA BASE AN EN SQL ESTA FOLIADA POR ALGUNA RAZON, PERO NO HAY REGISTRO EN FCCBNetDB");
}


function RefoliarPagomatico(IdNom)
{
    //5 LA BASE EN SQL NO ESTA FOLIADA POR ALGUNA RAZON, PERO SI HAY REGISTROS EN FCCBNetDB que indican que en algun momento fue foleada => VErificar con el desarrollador por que sucedio (En este caso debe solo actualizar los datos)
    console.log("ERROR 5 => LA BASE EN SQL NO ESTA FOLIADA POR ALGUNA RAZON, PERO SI HAY REGISTROS EN FCCBNetDB que indican que en algun momento fue foleada => Verifica con el desarrollador por que sucedio (En este caso debe solo actualizar los datos)");
    Swal.fire({
        title: '¿Esta seguro de querer folear los pagomaticos de nuevo?',
        text: ' LA BASE EN SQL NO ESTA FOLIADA POR ALGUNA RAZON, PERO SI HAY REGISTROS EN FCCBNetDB',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, Folear de nuevo!',
        cancelButtonText: 'No, cancelar!',
        footer: '<a href="#">Contactar al desarrollador?</a>'
    }).then((result) => {
        if (result.isConfirmed) {

           // MensajeEstamosEnConstrucion();
            let quincenaParaFoliar = document.getElementById("QuincenaFoliacion").value;

            MensajeCargando();
            axios.post('/Foliar/ReFoliarPorIdNominaPagomatico', {
                IdNomina: IdNom,
                NumeroQuincena: quincenaParaFoliar
            })
                .then(function (response) {
                   // console.log(response.data);
                    

                    if (response.data.bandera) {

                        $('#TablaEsFoliada').empty();
                        DibujarTablaEsFoliada();
                        PintarResultadoEsFoliada(response.data.resultadoServer.Data.DetalleTabla);
                        MensajeCorrectoSweet("La nomina se folio correctamente");

                    } else {
                        MensajeErrorSweet(response.data.DBFAbierta[0].Detalle, response.data.DBFAbierta[0].Solucion);
                    }

                    OcultarMensajeCargando();

                })
                .catch(function (error) {
                    MensajeErrorSweet("error" + error);
                    OcultarMensajeCargando();
                   
                });
            

        }
    })
}


//Esta funcion se manda a llamar desde la vista a la que pertenece y solo sirve para limpiar la tabla informativa que se pinta 
function LimpiarTablaPagomatico()
{
    $('#TablaEsFoliada').empty();
}