

/* Tabla ddonde se muestra el banco, cuenta y los folios Dispo*/
function DibujarTablaDetalleProblemasCheques() {
    $("#TablaProblemasCheques").append(

        "<table id='ProblemasConCheques'  class='table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>detalles de problemas encontrados</caption>" +
        "<thead class='tabla'>" +

        "<tr class='text-center text-uppercase'>" +

        "<th>Id </th>" +
        "<th>Nombre Cuenta Banco</th>" +
        "<th>Folio</th>" +
        "<th>Incidencia</th>" +
        "<th>Fecha Incidencia</th>" +

        "</tr>" +
        "</thead>" +
        "</table>"
    );
};

function LlenarTablaDetalleProblemasCheques(datos) {

    $('#ProblemasConCheques').DataTable({
        "ordering": true,
        "info": false,
        "searching": false,
        "paging": false,
        "lengthMenu": [5, 10],
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
            { "data": "Id" },
            { "data": "NombreBancoCuenta" },
            { "data": "Folio" },
            { "data": "Incidencia" },
            { "data": "FechaIncidencia" }

        ],
        "columnDefs": [


            { className: "text-center col-1", visible: true, "targets": 0, },
            { className: "text-center col-3", visible: true, "targets": 1, },
            { className: "text-center col-1", visible: true, "targets": 2, },
            { className: "text-center col-2", visible: true, "targets": 3, },
            { className: "text-center col-2", visible: true, "targets": 4, }

        ],

        "order": [[0, 'asc']]
    });
};







function DibujarResumenNominaCheques() {
    $("#TablaResumenNomina").append(
        "<table id='NuevaTablaResumen'  class='table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Resumen de Nomina</caption>"
        + "<thead class='tabla'>" +
        "<tr class='text-center text-uppercase'>" +

        "<th>Id </th>" +
        "<th>Delegacion </th>" +
        "<th>Sindicalizado</th>" +
        "<th>Confianza</th>" +
        "<th>Otros</th>" +
        "<th>Esta Foliado Correctamente  </th>" +
        "<th>   </th>" +
        "<th>   </th>" +
        "</tr>" +
        "</thead>" +
        "</table>"
    );
};

function PintarConsultaTablaResumenNominaChequesEnModal(datos) {
    $('#NuevaTablaResumen').DataTable({
        "ordering": true,
        "info": false,
        "searching": false,
        "paging": false,
        "lengthMenu": [5, 10],
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
            { "data": "IdVirtual" },
            { "data": "NombreDelegacion" },
            {
                data: "Sindicato",
                render: function (data, type, row) {
                    //console.log(data);
                    if (data != 0) {
                        return data;
                    }
                    return '';
                }

            },
            {
                data: "Confianza",
                render: function (data, type, row) {
                    // console.log(data);
                    if (data != 0) {
                        return data;
                    }
                    return '';
                }

            },
            {
                data: "Otros",
                render: function (data, type, row) {
                    // console.log(data);
                    if (data != 0) {
                        return data;
                    }
                    return '';
                }

            },
            {
                data: "EstaFoliadoCorrectamente",
                render: function (data, type, row) {
                    //booleano que dice si estan todos los registros foliados por delegacion o No
                    if (data) {
                        return '<h4 class="  text-success text-uppercase"> <i class="fas fa-check"></i>  </h4>';
                    }
                    return '<h4 class=" text-danger   text-uppercase "> <i class="fas fa-times-circle"></i>  </h4>';
                }

            },
            {
                render: function (data, type, row) {
                    //console.log(row);
                    if (!row.EstaFoliadoCorrectamente) {
                            return `<h4 class="bg-success btn  text-uppercase text-light"  onclick="DelegacionSelecionadaCheque(${row.IdNomina}, ${row.IdDelegacion},  ${row.GrupoFoliacion},  ${row.Sindicato},  ${row.Confianza} ,  ${row.Otros},  '${row.Coment}' , '${row.NombreDelegacion}' )"> Seleccionar Delegacion <i class="fas fa-sort-numeric-down"></i>  </h4>`;
                    // return `<h4 class="bg-success btn"  onclick="DelegacionSelecionadaCheque('${row.NombreDelegacion}')" > Seleccionar Delegacion </h4>`;
                    }
                    return '';
                }

            },
            {
                render: function (data, type, row) {

                    return '<h4 class="bg-info btn btn-lg  text-uppercase text-light"  onclick="ImprimeNominaDelegacion(' + row.IdNomina + ' , ' + row.IdDelegacion + ' , ' + row.GrupoFoliacion + ', ' + row.Sindicato + ', ' + row.Confianza + ', ' + row.Otros + '  )"  > <i class="fas fa-print"></i> </h4>';

                }

            }
        ],
        "order": [[1, 'asc']],
        "columnDefs":
            [

                { className: "text-center col-1", visible: true, "targets": 0, },
                { className: "text-center col-3", visible: true, "targets": 1, },
                { className: "text-center col-1", visible: true, "targets": 2, },
                { className: "text-center col-1", visible: true, "targets": 3, },
                { className: "text-center col-1", visible: true, "targets": 4, },
                { className: "text-center col-1", visible: true, "targets": 5, },
                { className: "text-center col-3", visible: true, "targets": 6, },
                { className: "text-center col-1", visible: true, "targets": 7, }
            ]
    });
};






function verResumenXDelegacionNominaCheque() {

    let nominaSeleccionadaUsuario = document.getElementById("SeleccionarNominaFoliar").value;
    let quincenaCheque = document.getElementById("QuincenaFormasPago").innerText;

    if (nominaSeleccionadaUsuario != '') {
        console.log(quincenaCheque)

        axios.post('/Foliar/ObtenerResumenDetalle_NominaCheques', {
            IdNomina: nominaSeleccionadaUsuario ,
            Quincena: quincenaCheque
        })
        .then(function (response) {

                $("#TablaResumenNomina").empty();
                DibujarResumenNominaCheques();
                PintarConsultaTablaResumenNominaChequesEnModal(response.data)

                document.getElementById('NombreCabezeraModal').innerText = document.getElementById("SeleccionarNominaFoliar").options[document.getElementById('SeleccionarNominaFoliar').selectedIndex].text;
                $('#DetalleDelegacionNominaCheque').modal('show');
                OcultarMensajeCargando();

        })
            .catch(function (error) {
                MensajeErrorSweet("Intente de nuevo ", error)
            OcultarMensajeCargando();
        });

        //let EnviarDatos = "{'IdNomina': '" + nominaSeleccionadaUsuario + "', 'Quincena': '" + quincenaCheque + "'}";
        //// console.log(EnviarDatos)
        //MensajeCargando();
        //$.ajax({
        //    url: 'Foliar/ObtenerResumenDetalle_NominaCheques',
        //    data: EnviarDatos,
        //    type: "POST",
        //    contentType: "application/json; charset=utf-8",
        //    success: function (response) {

        //        //   console.log(response);
        //        //  console.log(response.TablaModal);

        //        $("#TablaResumenNomina").empty();
        //        DibujarResumenNominaCheques();
        //        PintarConsultaTablaResumenNominaChequesEnModal(response)



        //        document.getElementById('NombreCabezeraModal').innerText = document.getElementById("SeleccionarNominaFoliar").options[document.getElementById('SeleccionarNominaFoliar').selectedIndex].text;

        //        ////si la nomina es general o desc no se mostraran los botones de sindicalizado y confianza
        //        //if (response.NominaEsGenODesc) {

        //        //    document.getElementById('NominaSinSindicato').style.display = 'none';
        //        //    document.getElementById('CampoSindicato').style.display = 'block';
        //        //} else {

        //        //    document.getElementById('CampoSindicato').style.display = 'none';
        //        //    document.getElementById('NominaSinSindicato').style.display = 'block';
        //        //    document.getElementById('NominaSinSindicato').innerText = "Nomina sin sindicato para asignar folios";
        //        //}


        //        $('#DetalleDelegacionNominaCheque').modal('show');

        //        OcultarMensajeCargando();

        //    }, error: function (jqXHR, textStatus) {
        //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
        //        OcultarMensajeCargando();
        //    }
        //});

    } else {
        MensajeWarningSweet("¡Asegurese de seleccionar una nomina!");
    }

}

function ImprimeNominaDelegacion(IdNomina, IdDelegacion, GrupoFoliacion, Sindicato, Confianza, Otros) {
    //  console.log(IdNomina, IdDelegacion, GrupoFoliacion, Sindicato, Confianza, Otros);


    let generarReporteDelegacion = {
        IdNomina: IdNomina,
        IdDelegacion: IdDelegacion,
        GrupoFoliacion: GrupoFoliacion,
        Sindicato: Sindicato,
        Confianza: Confianza,
        Otros: Otros,
        Quincena: document.getElementById("QuincenaFormasPago").innerText
    };

    MensajeCargando();
    axios.post('/Foliar/RevisarReportePDFChequeIdNominaPorDelegacion', {
        GenerarReporteDelegacion: generarReporteDelegacion
    })
    .then(function (response) {
            $("VisorPdfCheque").empty();
            /// PDFObject.embed(response, "#example");
            PDFObject.embed("data:application/pdf;base64," + response.data + " ", "#VisorPdfCheque");
            //PDFObject.embed("../Reportes/ReportesPDFSTemporales/RevicionNomina" + nominaSeleccionadaFoliar.value + ".pdf", "#example");
            $('#ModalPDFVisualizadorCheque').modal('show');

            OcultarMensajeCargando();
    })
    .catch(function (error) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo ", error)
        OcultarMensajeCargando();
    });


    //MensajeCargando();
    //$.ajax({
    //    url: 'Foliar/RevisarReportePDFChequeIdNominaPorDelegacion',
    //    data: JSON.stringify(generarReporteDelegacion),
    //    type: "POST",
    //    contentType: "application/json; charset=utf-8",
    //    success: function (response) {


    //        $("VisorPdfCheque").empty();
    //        /// PDFObject.embed(response, "#example");
    //        PDFObject.embed("data:application/pdf;base64," + response + " ", "#VisorPdfCheque");
    //        //PDFObject.embed("../Reportes/ReportesPDFSTemporales/RevicionNomina" + nominaSeleccionadaFoliar.value + ".pdf", "#example");
    //        $('#ModalPDFVisualizadorCheque').modal('show');

    //        OcultarMensajeCargando();

    //    }, error: function (jqXHR, textStatus) {
    //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus+ jqXHR)
    //        OcultarMensajeCargando();
    //    }
    //});



}

function ImprimirNominaCheques() {
    let IdnominaSeleccionadaUser = document.getElementById("SeleccionarNominaFoliar").value


    if (IdnominaSeleccionadaUser != "") {
      //  console.log(IdnominaSeleccionadaUser);

        let generarReporteNomina = {
            IdNomina: IdnominaSeleccionadaUser,
            Quincena: document.getElementById("QuincenaFormasPago").innerText
        };

        let quincenaCargada = document.getElementById("QuincenaFormasPago").innerText
        MensajeCargando();
        axios.post('/Foliar/RevisarReportePDFChequeIdNomina', {
            IdNomina: IdnominaSeleccionadaUser,
            Quincena: quincenaCargada
        })
        .then(function (response) {

                $("VisorPdfCheque").empty();
                /// PDFObject.embed(response, "#example");
                PDFObject.embed("data:application/pdf;base64," + response.data + " ", "#VisorPdfCheque");
                //PDFObject.embed("../Reportes/ReportesPDFSTemporales/RevicionNomina" + nominaSeleccionadaFoliar.value + ".pdf", "#example");
                $('#ModalPDFVisualizadorCheque').modal('show');

                OcultarMensajeCargando();
        })
            .catch(function (error) {
                MensajeErrorSweet("Intente de nuevo ", error)
            OcultarMensajeCargando();
        });


        //MensajeCargando();
        //$.ajax({
        //    url: 'Foliar/RevisarReportePDFChequeIdNomina',
        //    data: JSON.stringify(generarReporteNomina),
        //    type: "POST",
        //    contentType: "application/json; charset=utf-8",
        //    success: function (response) {


        //        $("VisorPdfCheque").empty();
        //        /// PDFObject.embed(response, "#example");
        //        PDFObject.embed("data:application/pdf;base64," + response + " ", "#VisorPdfCheque");
        //        //PDFObject.embed("../Reportes/ReportesPDFSTemporales/RevicionNomina" + nominaSeleccionadaFoliar.value + ".pdf", "#example");
        //        $('#ModalPDFVisualizadorCheque').modal('show');

        //        OcultarMensajeCargando();

        //    }, error: function (jqXHR, textStatus) {
        //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
        //        OcultarMensajeCargando();
        //    }
        //});


    } else {
        MensajeErrorSweet("Selecciona una nomina", "No hay ninguna nomina seleccionada");
    }
}

let IdNominaSeleccionada, IdDelegacionSeleccionada, IdGrupoFoliacion, sindicatoSeleccionado, confianzaSeleccionado, OtrosSeleccionado
function DelegacionSelecionadaCheque(IdNomina, IdDelegacion, GrupoFoliacion, Sindicato, Confianza, Otros, Coment, NombreDelegacion) {

    IdNominaSeleccionada = IdNomina;
    IdDelegacionSeleccionada = IdDelegacion;
    IdGrupoFoliacion = GrupoFoliacion;
    sindicatoSeleccionado = Sindicato;
    confianzaSeleccionado = Confianza;
    OtrosSeleccionado = Otros;


    document.getElementById("NominaCheche").innerText = Coment;
    document.getElementById("DelegacionCheque").innerText = NombreDelegacion;
    document.getElementById("GrupoFoliacionCheque").innerText = GrupoFoliacion;

    let tNomina = document.getElementById("TipoNomina");
    let registros = document.getElementById("NumeroRegistros");

    if (GrupoFoliacion == 0) {
        if (Sindicato > 0 && Confianza == 0) {
            //Son sindicalizados
            tNomina.innerText = "Sindicalizados";
            registros.innerText = Sindicato;
        } else if (Sindicato == 0 && Confianza > 0) {
            //Son de confianza
            tNomina.innerText = "Confianza";
            registros.innerText = Confianza;
        }
    } else if (GrupoFoliacion == 1) {
        tNomina.innerText = "Otros";
        registros.innerText = Otros;
    }

    $('#DetalleDelegacionNominaCheque').modal('hide');
    document.getElementById('VistaFormasPago').style.display = "none";
    document.getElementById('ParteBancariaFoliacion').style.display = "block";

    /* poner en blanco campos para una nueva foliacion*/
    document.getElementById('SeleccionarCuentaBancaria').selectedIndex = "";
    document.getElementById('rangoInicial').value = "";
    document.getElementById('rangoFinal').value = "...";
    document.getElementById('registrosFoliados').value = "...";

    document.getElementById('checkInhabilitar').checked = false;
    document.getElementById('cambiaColorInhabilitar').className = 'btn btn-lg btn-outline-primary';

    document.getElementById('FolioInicialInhabilitado').value = "";
    document.getElementById('FolioFinalInhabilitado').value = "";
}


function CambiarColor()
{
    let hayInhabilitado = document.getElementById('checkInhabilitar').checked

    //console.log(hayInhabilitado);
    if (hayInhabilitado)
    {
        document.getElementById('cambiaColorInhabilitar').className = 'btn btn-lg btn-primary';
    }
    else
    {
        document.getElementById('cambiaColorInhabilitar').className = 'btn btn-lg btn-outline-primary';
    }

}



function FoliarNominaCheque() {
    let idBancoPagadorSeleccionado = document.getElementById("SeleccionarCuentaBancaria").value
    let rangoInicialSeleccionado = document.getElementById("rangoInicial").value
    let hayInhabilitado = document.getElementById('checkInhabilitar').checked
    let rangoInhabilitadoInicial = document.getElementById('FolioInicialInhabilitado').value
    let rangoInhabilitadoFinal = document.getElementById('FolioFinalInhabilitado').value
   // console.log("hayInhabilitado:", hayInhabilitado);

    let cBSeleccionada = document.getElementById("SeleccionarCuentaBancaria");
    let textoCBancaria = cBSeleccionada.options[cBSeleccionada.selectedIndex].text;
   // console.log(textoCBancaria);

    let bandera = false;
    if (idBancoPagadorSeleccionado != '') {


        if (rangoInicialSeleccionado != '') {


            if (hayInhabilitado) {

              //  console.log(rangoInhabilitadoInicial)
               // console.log(rangoInhabilitadoFinal)
                if (rangoInhabilitadoFinal != '' && rangoInhabilitadoInicial != '') {

                    if (parseInt(rangoInhabilitadoFinal) >= parseInt(rangoInhabilitadoInicial)) {

                      
                      //  console.log("INICIal", rangoInicialSeleccionado)
                      //  console.log("Inhabilitado INICIAL", parseInt(rangoInhabilitadoInicial))
                      //  console.log("Inhabilitado FINAL", parseInt(rangoInhabilitadoFinal))
                        if (parseInt(rangoInhabilitadoInicial) > parseInt(rangoInicialSeleccionado)) {

                           // console.log("TOdi bien");
                            bandera = true;

                        } else {
                            MensajeErrorSweet(' El rango inicial de inhabilitado debe ser mayor al rango inicial de foliacion', 'Incongruencia en los rangos de foliacion ');
                        }

                    } else {
                        MensajeErrorSweet(' El rango Final debe ser mayor al Inicial', 'Incongruencia en los rangos de foliacion inhabilitados ');
                    }



                } else {
                    MensajeErrorSweet('El rango de inhabilitados no puede estar vacio', 'Incongruencia en los rangos de foliacion inhabilitados ');
                }





            } else {



                bandera = true;
                rangoInhabilitadoInicial = 0;
                rangoInhabilitadoFinal = 0;
            }






        } else {
            MensajeErrorSweet('intruduzca un numero para iniciar la foliacion', 'Rango de Foliacion vacio ');
        }


    } else {
        MensajeErrorSweet('Intente seleccionando un banco nuevamente', 'No se selecciono un banco');
    }






    // es para saber que las primeras validaciones estan correctas
    if (bandera)
    {

        let textoResumenAMostrar = "";

        if (hayInhabilitado) {
            textoResumenAMostrar = "Se iniciara a foliar desde el folio " + rangoInicialSeleccionado + " , del banco " + textoCBancaria + " , con la Inhabilitacion Activa en el folio inicial : " + rangoInhabilitadoInicial + " y el folio final : " + rangoInhabilitadoFinal + "  ";
        } else {
            textoResumenAMostrar = "Se iniciara a foliar desde el folio " + rangoInicialSeleccionado + " , del banco " + textoCBancaria + " , con la Inhabilitacion Desactivada  ";
        }


        Swal.fire({
            title: '¿Esta Seguro de Foliar los Cheques?',
            text: textoResumenAMostrar,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Foliar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {


                if (bandera) {
                    let datosNominaFoliarCheque = {
                        IdNomina: + IdNominaSeleccionada,
                        IdDelegacion: + IdDelegacionSeleccionada,
                        IdGrupoFoliacion: + IdGrupoFoliacion,
                        Sindicato: + sindicatoSeleccionado,
                        Confianza: + confianzaSeleccionado,
                        Otros: + OtrosSeleccionado,
                        IdBancoPagador: + idBancoPagadorSeleccionado,
                        RangoInicial: +  rangoInicialSeleccionado,
                        Inhabilitado: hayInhabilitado,
                        RangoInhabilitadoInicial: + rangoInhabilitadoInicial,
                        RangoInhabilitadoFinal: + rangoInhabilitadoFinal,
                        Quincena: document.getElementById("QuincenaFormasPago").innerText

                    };

                    //  console.log("Ire a foliar");
                    //  console.log(datosNominaFoliarCheque);
                    MensajeCargando();
                    axios.post('/Foliar/FoliarNominaFormaPago', {
                        NuevaFoliacionNomina: datosNominaFoliarCheque
                       
                    })
                    .then(function (response) {
                            if (response.data.resultServer == 0) {


                                $("#TablaProblemasCheques").empty();
                                DibujarTablaDetalleProblemasCheques();
                                LlenarTablaDetalleProblemasCheques(response.data.FoliosConIncidencias);
                                $('#DetalleProblemasConCHEQUES').modal('show');

                            } else if (response.data.resultServer == 200) {

                                MensajeCorrectoSweet("Se ha foliado la nomina correctamente");

                                document.getElementById("rangoFinal").innerText = response.data.resultadoFoliacion[0].UltimoFolioUsado;
                                document.getElementById("registrosFoliados").innerText = response.data.resultadoFoliacion[0].RegistrosFoliados;


                                ActualizaTablaResumenFoliar();
                                verResumenXDelegacionNominaCheque();

                            } else if (response.data.resultServer == 500) {

                                MensajeErrorSweet(response.data.DBFAbierta[0].Detalle, response.data.DBFAbierta[0].Solucion);

                            } else if (response.data.resultServer == 501) {
                                MensajeInformacionSweet(response.data.FoliosSaltados);
                            }


                            OcultarMensajeCargando();
                    })
                    .catch(function (error) {
                            MensajeErrorSweet("Ocurrio un error intente de nuevo ", error);
                            OcultarMensajeCargando();
                    });


                    //MensajeCargando();
                    //$.ajax({
                    //    url: 'Foliar/FoliarNominaFormaPago',
                    //    data: JSON.stringify(datosNominaFoliarCheque),
                    //    type: "POST",
                    //    contentType: "application/json; charset=utf-8",
                    //    success: function (response) {
                    //        //console.log(response);
                    //        //console.log(response.DbfAbierta);
                    //        //console.log(response.DbfAbierta[0].Detalle);

                    //        if (response.resultServer == 0) {


                    //            $("#TablaProblemasCheques").empty();
                    //            DibujarTablaDetalleProblemasCheques();
                    //            LlenarTablaDetalleProblemasCheques(response.FoliosConIncidencias);
                    //            $('#DetalleProblemasConCHEQUES').modal('show');

                    //        } else if (response.resultServer == 200) {

                    //            MensajeCorrectoSweet("Se ha foliado la nomina correctamente");

                    //            document.getElementById("rangoFinal").innerText = response.resultadoFoliacion[0].UltimoFolioUsado;
                    //            document.getElementById("registrosFoliados").innerText = response.resultadoFoliacion[0].RegistrosFoliados;


                    //            ActualizaTablaResumenFoliar();
                    //            verResumenXDelegacionNominaCheque();

                    //        } else if (response.resultServer == 500) {

                    //            MensajeErrorSweet( response.DBFAbierta[0].Detalle , response.DBFAbierta[0].Solucion);

                    //        } else if (response.resultServer == 501) {
                    //            MensajeInformacionSweet(response.FoliosSaltados);
                    //        }



                    //        OcultarMensajeCargando();

                    //    }, error: function (jqXHR, textStatus) {

                    //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus);
                    //        OcultarMensajeCargando();
                    //    }
                    //});



                }








            }
        })
    }





}



function AjustarInhabilitados() {
    /* para saber el id del banco y poder saber el idInventario que tiene */
    let idBancoPagadorSeleccionado = document.getElementById("SeleccionarCuentaBancaria").value

    /* para saber en que rango de inicio empezar a buscar */
    let rangoInicialSeleccionado = document.getElementById("rangoInicial").value

    /* saber si esta selecciona la opcion de inhabilitar */
    let hayInhabilitado = document.getElementById('checkInhabilitar').checked

    /* rango de inhabilitacion a la cual se debe ajustar */
    let rangoInhabilitadoInicial = document.getElementById('FolioInicialInhabilitado').value
    let rangoInhabilitadoFinal = document.getElementById('FolioFinalInhabilitado').value

    let cBSeleccionada = document.getElementById("SeleccionarCuentaBancaria");
    let textoCBancaria = cBSeleccionada.options[cBSeleccionada.selectedIndex].text;


    let bandera = false;
    if (idBancoPagadorSeleccionado != '') {

        if (rangoInicialSeleccionado != '') {


            if (hayInhabilitado) {

                //  console.log(rangoInhabilitadoInicial)
                // console.log(rangoInhabilitadoFinal)
                if (rangoInhabilitadoFinal != '' && rangoInhabilitadoInicial != '') {

                    if (parseInt(rangoInhabilitadoFinal) >= parseInt(rangoInhabilitadoInicial)) {


                        //  console.log("INICIal", rangoInicialSeleccionado)
                        //  console.log("Inhabilitado INICIAL", parseInt(rangoInhabilitadoInicial))
                        //  console.log("Inhabilitado FINAL", parseInt(rangoInhabilitadoFinal))
                        if (parseInt(rangoInhabilitadoInicial) > parseInt(rangoInicialSeleccionado)) {

                            // console.log("TOdi bien");
                            bandera = true;

                        } else {
                            MensajeErrorSweet(' El rango inicial de inhabilitado debe ser mayor al rango inicial de foliacion', 'Incongruencia en los rangos de foliacion ');
                        }

                    } else {
                        MensajeErrorSweet(' El rango Final inhabilitado debe ser mayor al Inicial inhabilitado', 'Incongruencia en los rangos de foliacion inhabilitados ');
                    }



                } else {
                    MensajeErrorSweet('El rango de inhabilitados no puede estar vacio', 'Incongruencia en los rangos de foliacion inhabilitados ');
                }


            } else {


                MensajeErrorSweet('Seleccione la opcion de inhabilitar para proceder con el ajuste');
               // bandera = true;
                rangoInhabilitadoInicial = 0;
                rangoInhabilitadoFinal = 0;
            }



        } else {
            MensajeErrorSweet('intruduzca un numero para iniciar la busqueda y poder hacer el ajuste', 'Rango inicial de Foliacion vacio ');
        }

    } else {
        MensajeErrorSweet('Seleccione el banco con el que se folio anteriormente este nomina', 'No se selecciono un banco');
    }






    // es para saber que las primeras validaciones estan correctas
    if (bandera) {

        let textoResumenAMostrar =  "Se realizara un ajuste apartir del rango " + rangoInicialSeleccionado + " , del banco " + textoCBancaria + " , con la Inhabilitacion en el folio inicial ajustado : " + rangoInhabilitadoInicial + " y el folio final ajustado : " + rangoInhabilitadoFinal + "  ";
        

        Swal.fire({
            title: '¿Esta seguro de ajustar el rango de CHEQUES INHABILITADOS ?',
            text: textoResumenAMostrar,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Ajustar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {


                if (bandera) {
                    let datosAjustar = {
                        IdNomina: + IdNominaSeleccionada,
                        IdDelegacion: + IdDelegacionSeleccionada,
                        IdGrupoFoliacion: + IdGrupoFoliacion,
                        Sindicato: + sindicatoSeleccionado,
                        Confianza: + confianzaSeleccionado,
                        Otros: + OtrosSeleccionado,
                        IdBancoPagador: + idBancoPagadorSeleccionado,
                        RangoInicial: +  rangoInicialSeleccionado,
                        Inhabilitado: hayInhabilitado,
                        RangoInhabilitadoInicial: + rangoInhabilitadoInicial,
                        RangoInhabilitadoFinal: + rangoInhabilitadoFinal,
                        Quincena: document.getElementById("QuincenaFormasPago").innerText

                    };



                    console.log(datosAjustar);

                    /**/
                    /**/
                    /**/
                    MensajeCargando();
                    axios.post('/Foliar/AjustarCheques', {
                        NuevaAjusteDelegacionNomina: datosAjustar

                    })
                        .then(function (response) {

                            console.log(response);
                            //if (response.data.resultServer == 0) {


                            //    $("#TablaProblemasCheques").empty();
                            //    DibujarTablaDetalleProblemasCheques();
                            //    LlenarTablaDetalleProblemasCheques(response.data.FoliosConIncidencias);
                            //    $('#DetalleProblemasConCHEQUES').modal('show');

                            //} else
                            if (response.data.resultServer == 200) {

                                MensajeCorrectoSweet("Se ha realizado el ajuste correctamente en los rangos correspondientes");

                                document.getElementById("rangoFinal").innerText = response.data.resultadoFoliacion[0].UltimoFolioUsado;
                               // document.getElementById("registrosFoliados").innerText = response.data.resultadoFoliacion[0].RegistrosFoliados;


                                ActualizaTablaResumenFoliar();
                                verResumenXDelegacionNominaCheque();

                            } else if (response.data.resultServer == 500) {

                                MensajeErrorSweet(response.data.DBFAbierta[0].Detalle, response.data.DBFAbierta[0].Solucion);

                            } else if (response.data.resultServer == 501) {
                                MensajeInformacionSweet(response.data.FoliosSaltados);
                            }


                            OcultarMensajeCargando();
                        })
                        .catch(function (error) {
                            MensajeErrorSweet("Ocurrio un error intente de nuevo ", error);
                            OcultarMensajeCargando();
                        });
                    /***/
                    /***/
                    /***/

                }

            }
        })
    }





}






function RegresarVistaFormaPago() {
    document.getElementById('ParteBancariaFoliacion').style.display = "none";
    document.getElementById('VistaFormasPago').style.display = "block";


}





















