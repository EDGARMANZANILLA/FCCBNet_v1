


function DibujarLocalizadorFormaPago() {
    $("#TablaRegistroLocalizadoFormaPago").append(

        "<table id='RegistrosLocalizados'  class='margenSection table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Formas de pago localizada</caption>"
        + "<thead class='tabla'>" +

        "<tr class='text-center text-uppercase'>" +


        "<th>Id_Nom</th>" +
        "<th>Referencia Bitacora</th>" +
        "<th>Quincena</th>" +
        "<th>Num Empleado</th>" +
        "<th>Nombre Beneficiaro</th>" +
        "<th>Num Benef</th>" +
        "<th>Folio Cheque</th>" +
        "<th>Liquido</th>" +
        "<th>Estado Pago</th>" +
        "<th>Tipo Pago</th>" +
        "<th>  </th>" +
        "<th>  </th>" +

        "</tr>" +
        "</thead>" +
        "</table>"
    );
};

let FormaPagoLocalizadaDB;
function PintarLocalizadorFormaPago(datos) {



    FormaPagoLocalizadaDB = $('#RegistrosLocalizados').DataTable({
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

            { "data": "Id_nom" },
            { "data": "ReferenciaBitacora" },
            { "data": "Quincena" },
            { "data": "NumEmpleado" },
            { "data": "NombreBeneficiaro" },
            { "data": "NumBene" },
            { "data": "FolioCheque" },
            { "data": "Liquido" },
            { "data": "EstadoCheque" },
            { "data": "TipoPago" },
            { "data": "IdRegistro" },
            { "data": "IdRegistro" }
        ],
        "columnDefs": [

            { className: "text-center col-1", visible: true, "targets": 0, },
            { className: "text-center col-1", visible: true, "targets": 1, },
            { className: "text-center col-1", visible: true, "targets": 2, },
            { className: "text-center col-1", visible: true, "targets": 3, },
            { className: "text-center col-1", visible: true, "targets": 4, },
            { className: "text-center col-1", visible: true, "targets": 5, },
            { className: "text-center col-1", visible: true, "targets": 6, },
            { className: "text-center col-1", visible: true, "targets": 7, },
            { className: "text-center col-1", visible: true, "targets": 8, },
            { className: "text-center col-1", visible: true, "targets": 9, },
            {
                className: "text-center col-1",
                visible: true,
                "targets": [10],
                render: function (data, type, row) {
                    //console.log(typeof data);
                    //console.log( data);
                    if (data) {
                      
                        return '<h4 class="verDetalleSuspender bg-info btn  text-uppercase text-light"   >Ver detalle para Suspender </h4>';
                    }

                }
           
            },
            {
                className: "text-center col-1",
                visible: true,
                "targets": [11],
                render: function (data, type, row) {
                    //console.log(typeof data);
                    //console.log( data);
                    if (data) {
                        return '<h4 class="verDetalleReponer bg-info btn  text-uppercase text-light" href="#DDDDDDD"  >ver detalle para Reponer  </h4>';
                    }

                }
            }

        ],
        "order": [[2, 'desc']]

    });

};



async function suspender(IdRegistro)
{
       let buscardetalleIdRegistro = "{'IdRegistroAbuscar':'" +IdRegistro+"'}";

       // console.log(buscardetalleIdRegistro);


        MensajeCargando();

     

    const response = await axios.post("Reposicion_Suspencion/BuscarDetalleSuspencion", { IdRegistroAbuscar: `${IdRegistro}` })


    console.log(response)
    $('#RenderPartialViewDetalleRegistroSuspencion').html('');
    $('#RenderPartialViewDetalleRegistroSuspencion').html(response.data);

        //$.ajax({
        //    type: "POST",
        //     url: 'Reposicion_Suspencion/BuscarDetalleSuspencion',
        //    data: buscardetalleIdRegistro,
        //    async: true,
        //    contentType: "application/json; charset=utf-8",
        //    success: function (response) {

        //        if (response.RespuestaServidor === 500) {

        //            MensajeErrorSweet(response.MensajeError);
        //            $('#RenderPartialViewDetalleRegistroSuspencion').html('');

        //        } else {
        //            $('#RenderPartialViewDetalleRegistroSuspencion').html('');
        //            $('#RenderPartialViewDetalleRegistroSuspencion').html(response);

        //        }


        //    }
        //});
        OcultarMensajeCargando();

  


}






















/*************************************************************************************************************************************************/
/*************************************************************************************************************************************************/
/***************************************************  Funciones para SUSPENDER EL PAGO A UN EMPLEADO *********************************************/
/*************************************************************************************************************************************************/
/*************************************************************************************************************************************************/





function SuspenderPagoTrabajador(idRegistro) {

    //  reponerNuevoFolio = document.getElementById("InputNuevaFormaDePago").value;




    let EnviarSupensionRegistro = "{'IdRegistroPago':'"+idRegistro+"'}";

    //console.log(EnviarSupensionRegistro + " SuspenderPagoTrabajadorrrrrrrrrrrrrrrrrrrr")




    Swal.fire({
        title: '¿Seguro que desea SUSPENDER la DISPERSION para el empleado actual?',
        text: "¡Se guardara un registro de este movimiento y podria no revertirse!",
        icon: 'warning',

        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#17a2b8',
        confirmButtonText: 'Guardar suspension',
        cancelButtonText: `Cancelar `

    }).then((result) => {
        if (result.isConfirmed) {


            MensajeCargando();


            axios.post('/Reposicion_Suspencion/SuspenderIdFormaPago', {
                IdRegistroPago: idRegistro

            }).then(function (response) {

                if (response.data.respuestaServidor == 200) {
                    //entra si todo fue bien en el servidor 
                    MensajeCorrectoConRecargaPagina(response.data.solucion)


                } else {

                    MensajeErrorSweet(response.data.solucion, '');
                }

                OcultarMensajeCargando();
            }).catch(function (error) {
                MensajeErrorSweet("Ocurrio un error intente de nuevo : " + error)
                OcultarMensajeCargando();
                //console.log(error);
            });



            //$.ajax({
            //    url: 'Reposicion_Suspencion/SuspenderIdFormaPago',
            //    data: EnviarSupensionRegistro,
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    success: function (response) {

            //       // console.log(response)

            //        if (response.respuestaServidor == 200) {
            //            //entra si todo fue bien en el servidor 
            //            MensajeCorrectoConRecargaPagina(response.solucion)


            //        } else  {

            //            MensajeErrorSweet(response.solucion, '');
            //        }

            //        OcultarMensajeCargando();

            //    }, error: function (jqXHR, textStatus) {
            //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
            //        OcultarMensajeCargando();
            //    }
            //});



        }
    })





}







/*******************************************************************************************************************************************/
/*******************************************************************************************************************************************/
/***************************************************  Funciones para Reponer una Forma de Pago *********************************************/
/*******************************************************************************************************************************************/
/*******************************************************************************************************************************************/



////*********Crea y pinta la tabla del modal del hostorico **********///
function DibujarHistoricoFormaPago() {
    $("#TablaHistoricoReposiciones").append(

        "<table id='HistoricoReposiciones'  class='margenSection table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Historico  DE  SEGUIMIENTO</caption>"
        + "<thead class='tabla'>" +

        "<tr class='text-center text-uppercase'>" +


        "<th>Id</th>" +
        "<th>Fecha Cambio</th>" +
        "<th>Motivo de Registro</th>" +
        "<th>Forma de Pago </th>" +
        "<th>                       </th>" +
        "<th>Nueva Forma de Pago    </th>" +
        "<th>Suceso creado por </th>" +
        "<th>Estado Cancelacion </th>" +
        "<th>Es Cancelado </th>" +
        "<th>Referencia Cancelado </th>" +
        

        "</tr>" +
        "</thead>" +
        "</table>"
    );
};


function PintarHistoricoFormaPago(datos) {
    $('#HistoricoReposiciones').DataTable({
        "ordering": true,
        "info": false,
        "searching": false,
        "paging": true,
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
            { "data": "FechaCambio" },
            { "data": "MotivoRefoliacion" },
            { "data": "ChequeAnterior" },
            null,
            { "data": "ChequeNuevo" },
            { "data": "RepuestoPor" },
            { "data": "DescripcionCancelado" },
            { "data": "EsCancelado" },
            { "data": "ReferenciaCancelado" }
        ],
        "columnDefs": [

            { className: "text-center col-1", visible: true, "targets": 0, },
            { className: "text-center col-1", visible: true, "targets": 1, },
            { className: "text-center col-2", visible: true, "targets": 2, },
            { className: "text-center col-1", visible: true, "targets": 3, },
            {
                className: "text-center col-1",
                visible: true,
                "targets": [4],
                render: function (data, type, row) {
                    //console.log(typeof data);
                    //console.log( data);
                    //if (data) {
                    return '<i class="fas fa-exchange-alt"></i>';
                    //}

                }
            },
            { className: "text-center col-1", visible: true, "targets": 5, },
            { className: "text-center col-2", visible: true, "targets": 6, },
            { className: "text-center col-1", visible: true, "targets": 7, },
            { className: "text-center col-1", visible: true, "targets": 8, },
            { className: "text-center col-1", visible: true, "targets": 9, }
    

        ],
        "order": [[0, 'desc']]

    });

};




let reponerNuevoFolio = '';
function ReponerNuevoFolio(idRegistro) {

    reponerNuevoFolio = document.getElementById("InputNuevaFormaDePago").value;


    if (reponerNuevoFolio != 0) {

        //Antes de inciar una nueva peticion al server limpiamos la vista parcial para que el detalle del registro seleccionado no este a la vista
        //////// $('#RenderPartialViewDetalleRegistroSuspencion').html('');


        let EnviarNuevoFolio = "{'IdRegistroPago':'" + idRegistro + "' , 'ReponerNuevoFolio':'" + reponerNuevoFolio + "'}";

       // console.log(EnviarNuevoFolio)



        Swal.fire({
            title: '¿Seguro que desea reponer la forma de pago actual?',
            text: "¡Se guardara un registro de este movimiento y podria no revertirse!",
            icon: 'warning',

            showCancelButton: true,
            confirmButtonColor: '#28a745',
            cancelButtonColor: '#17a2b8',
            confirmButtonText: 'Guardar reposicion',
            cancelButtonText: `Cancelar `

        }).then((result) => {
            if (result.isConfirmed) {


                MensajeCargando();



                axios.post('/Reposicion_Suspencion/ReponerIdFormaPago', {
                    IdRegistroPago: idRegistro,
                    ReponerNuevoFolio: reponerNuevoFolio

                }).then(function (response) {



                    if (response.data.respuestaServidor == 200) {
                        //entra si todo fue bien en el servidor 
                        MensajeCorrectoConRecargaPagina(response.data.solucion)


                    } else {

                        MensajeErrorSweet(response.data.solucion, '');
                    }

                    OcultarMensajeCargando();
                }).catch(function (error) {
                    MensajeErrorSweet("Ocurrio un error intente de nuevo : " + error)
                    OcultarMensajeCargando();
                    //console.log(error);
                });


                //$.ajax({
                //    url: 'Reposicion_Suspencion/ReponerIdFormaPago',
                //    data: EnviarNuevoFolio,
                //    type: "POST",
                //    contentType: "application/json; charset=utf-8",
                //    success: function (response) {

                //        if (response.respuestaServidor == 200) {
                //            //entra si todo fue bien en el servidor 
                //            MensajeCorrectoConRecargaPagina(response.solucion)
                //        }
                //        else {

                //            MensajeErrorSweet( response.solucion, '');
                //        }


                //        OcultarMensajeCargando();

                //    }, error: function (jqXHR, textStatus) {
                //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
                //        OcultarMensajeCargando();
                //    }
                //});



            }
        });




    } else {
        MensajeErrorSweet('Ingrese un folio para reponer', 'NO INGRESO NINGUN DATO PARA REPONER')
    }



}


function HistoricoSeguimiento(idRegistro) {

    MensajeCargando();

    let HistoricoAlocalizar = "{'IdRegistro':'"+idRegistro+"'}";



    axios.post('/Reposicion_Suspencion/BuscarHistorico', {
        IdRegistro:idRegistro

    }).then(function (response) {

        if (response.data.RespuestaServidor == 200) {

            //  console.log(response.Data);


            $('#TablaHistoricoReposiciones').empty();
            DibujarHistoricoFormaPago();
            PintarHistoricoFormaPago(response.data.Data);

            $('#btnHistoricoSeguimiento').modal('show');

            //$('#TablaRegistroLocalizadoFormaPago').empty();
            //DibujarLocalizadorFormaPago();
            //PintarLocalizadorFormaPago(response.FormaPagoLocalizada)


        } else {
            MensajeErrorSweet(response.data.Error, response.data.Solucion);
        }

        OcultarMensajeCargando();
    }).catch(function (error) {
        MensajeErrorSweet("Ocurrio un error intente de nuevo : " + error)
        OcultarMensajeCargando();
        //console.log(error);
    });



    //$.ajax({
    //    url: 'Reposicion_Suspencion/BuscarHistorico',
    //    data: HistoricoAlocalizar ,
    //    type: "POST",
    //    contentType: "application/json; charset=utf-8",
    //    success: function (response) {



    //        if (response.RespuestaServidor == 200) {

    //          //  console.log(response.Data);


    //            $('#TablaHistoricoReposiciones').empty();
    //            DibujarHistoricoFormaPago();
    //            PintarHistoricoFormaPago(response.Data);

    //            $('#btnHistoricoSeguimiento').modal('show');

    //            //$('#TablaRegistroLocalizadoFormaPago').empty();
    //            //DibujarLocalizadorFormaPago();
    //            //PintarLocalizadorFormaPago(response.FormaPagoLocalizada)


    //        } else
    //        {
    //            MensajeErrorSweet(response.Error, response.Solucion);
    //        }

    //        OcultarMensajeCargando();

    //    }, error: function (jqXHR, textStatus) {
    //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
    //        OcultarMensajeCargando();
    //    }
    //});







}


/********************************* */





function LocalizarDatoPorFiltro()
{
    let tipoFiltro = document.getElementById("TipoFiltroLocalizadorPagoCheque").value;
    let buscarDato = document.getElementById("InputLocalizador").value;

    $('#TablaRegistroLocalizadoFormaPago').empty();
    if (parseInt( tipoFiltro) != 0) {


        if (buscarDato != '') {

            //Antes de inciar una nueva peticion al server limpiamos la vista parcial para que el detalle del registro seleccionado no este a la vista
            $('#RenderPartialViewDetalleRegistroSuspencion').html('');

            let ElementoABuscar = "{'IdFiltro':'" + parseInt(tipoFiltro) + "','LocalizarEsteElemento':'" + parseInt( buscarDato) + "'}";

            //console.log(ElementoABuscar);

            MensajeCargando();



            axios.post('/Reposicion_Suspencion/Localizar', {
                IdFiltro: parseInt(tipoFiltro),
                LocalizarEsteElemento: parseInt(buscarDato)

            }).then(function (response) {

                if (response.data.RespuestaServidor == 200) {

                    //   console.log(response.FormaPagoLocalizada);

                    //$('#TablaEsFoliada').empty();
                    //DibujarTablaEsFoliada();
                    //PintarLocalizadorFormaPago(response.FormaPagoLocalizada);
                    $('#TablaRegistroLocalizadoFormaPago').empty();
                    DibujarLocalizadorFormaPago();
                    PintarLocalizadorFormaPago(response.data.FormaPagoLocalizada)


                } else if (response.data.RespuestaServidor == 201) {

                    MensajeErrorSweet(response.data.Error, response.data.Solucion);
                }
                else if (response.data.RespuestaServidor == 500) {

                    MensajeErrorSweet(response.data.Error, response.data.Solucion);
                }



                OcultarMensajeCargando();
            }).catch(function (error) {
                MensajeErrorSweet("Ocurrio un error intente de nuevo : " + error)
                OcultarMensajeCargando();
                //console.log(error);
            });


            //$.ajax({
            //    url: 'Reposicion_Suspencion/Localizar',
            //    data: ElementoABuscar,
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    success: function (response) {

            //        if (response.RespuestaServidor == 200) {

            //            //   console.log(response.FormaPagoLocalizada);

            //            //$('#TablaEsFoliada').empty();
            //            //DibujarTablaEsFoliada();
            //            //PintarLocalizadorFormaPago(response.FormaPagoLocalizada);
            //            $('#TablaRegistroLocalizadoFormaPago').empty();
            //            DibujarLocalizadorFormaPago();
            //            PintarLocalizadorFormaPago(response.FormaPagoLocalizada)


            //        } else if (response.RespuestaServidor == 201) {

            //            MensajeErrorSweet(response.Error, response.Solucion);
            //        }
            //        else if (response.RespuestaServidor == 500) {

            //            MensajeErrorSweet(response.Error, response.Solucion);
            //        }

            //        OcultarMensajeCargando();

            //    }, error: function (jqXHR, textStatus) {
            //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
            //        OcultarMensajeCargando();
            //    }
            //});


        } else {
            MensajeErrorSweet('Ingrese un dato a buscar', 'NO INGRESO NINGUN DATO PARA BUSCAR')
        }


    } else
    {
        MensajeInformacionSweet("Seleccione un filtro para una busqueda eficiente");
    }




}












$(document).ready(function () {
   

        document.getElementById("InputLocalizador").focus();




    //Click para ver detalle de una suspencion 
    $(document).on("click", ".verDetalleSuspender", function () {

        let idFormaPagoDetalleSuspeder = FormaPagoLocalizadaDB.row($(this).parents("tr")).data();
        let buscardetalleIdRegistro = "{'IdRegistroAbuscar':'" +idFormaPagoDetalleSuspeder.IdRegistro+"'}";

        //console.log(buscardetalleIdRegistro);


        MensajeCargando();

   


        axios.post('/Reposicion_Suspencion/BuscarDetalleSuspencion', {
            IdRegistroAbuscar:idFormaPagoDetalleSuspeder.IdRegistro

        }).then(function (response) {


            if (response.data.RespuestaServidor === 500) {

                MensajeErrorSweet(response.data.MensajeError);
                $('#RenderPartialViewDetalleRegistroSuspencion').html('');

            } else {
                $('#RenderPartialViewDetalleRegistroSuspencion').html('');
                $('#RenderPartialViewDetalleRegistroSuspencion').html(response.data);
                $('#RenderVista_SuspensionCancelacion').modal('show');

            }

            OcultarMensajeCargando();
        }).catch(function (error) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo : " + error)
            OcultarMensajeCargando();
            //console.log(error);
        });



        //$.ajax({
        //      url: 'Reposicion_Suspencion/BuscarDetalleSuspencion',
        //    data: buscardetalleIdRegistro,
        //    type: "POST",
        //    contentType: "application/json; charset=utf-8",
        //    success: function (response) {

        //        if (response.RespuestaServidor === 500) {

        //            MensajeErrorSweet(response.MensajeError);
        //            $('#RenderPartialViewDetalleRegistroSuspencion').html('');

        //        } else {
        //            $('#RenderPartialViewDetalleRegistroSuspencion').html('');
        //            $('#RenderPartialViewDetalleRegistroSuspencion').html(response);
        //            $('#RenderVista_SuspensionCancelacion').modal('show');

        //        }
              
        //    }
        //});
        //OcultarMensajeCargando();

    });



    


    //Click para ver el detalle de una probable cancelacion 
    $(document).on("click", ".verDetalleReponer", function () {

        let idFormaPagoDetalleCancelar = FormaPagoLocalizadaDB.row($(this).parents("tr")).data();

        //console.log("ver detalle de suspencion: "+idFormaPagoDetalleCancelar.IdRegistro);

      //  let buscardetalleIdRegistro = "{'IdRegistroAbuscar':'"+idFormaPagoDetalleCancelar.IdRegistro+"'}";

        //console.log(buscardetalleIdRegistro);

        MensajeCargando();

        axios.post('/Reposicion_Suspencion/BuscarDetalleReponer', {
            IdRegistroAbuscar: idFormaPagoDetalleCancelar.IdRegistro 

        }).then(function (response) {


            if (response.data.RespuestaServidor === 500) {

                MensajeErrorSweet(response.data.MensajeError);
                $('#RenderPartialViewDetalleRegistroSuspencion').html('');

            } else {
                $('#RenderPartialViewDetalleRegistroSuspencion').html('');
                $('#RenderPartialViewDetalleRegistroSuspencion').html(response.data);
                $('#RenderVista_SuspensionCancelacion').modal('show');

            }

            OcultarMensajeCargando();
        }).catch(function (error) {
            MensajeErrorSweet("Ocurrio un error intente de nuevo : " + error)
            OcultarMensajeCargando();
            //console.log(error);
        });



        //MensajeCargando();
        //$.ajax({
        //    url: 'Reposicion_Suspencion/BuscarDetalleReponer',
        //    data: buscardetalleIdRegistro,
        //    type: "POST",
        //    contentType: "application/json; charset=utf-8",
        //    success: function (response) {

              
        //        if (response.RespuestaServidor === 500) {

        //            MensajeErrorSweet(response.MensajeError);
        //            $('#RenderPartialViewDetalleRegistroSuspencion').html('');


        //            document.getElementById("InputNuevaFormaDePago").focus();


        //        } else {
        //            $('#RenderPartialViewDetalleRegistroSuspencion').html('');
        //            $('#RenderPartialViewDetalleRegistroSuspencion').html(response);
        //            $('#RenderVista_SuspensionCancelacion').modal('show');

        //        }

        //    }
        //});
        //OcultarMensajeCargando();


    });










});
