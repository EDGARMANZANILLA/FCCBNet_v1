

function DibujarResumenRecuperarFolios() {
    $("#TablaResumenRecuperarFolios").append(
        "<table id='TablaRecuperarFolios'  class='table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Resumen de formas de pago a recuperar</caption>"
        + "<thead class='tabla'>" +
        "<tr class='text-center text-uppercase'>" +
        "<th>Id </th>" +
        "<th>Año </th>" +
        "<th>Id_nom </th>" +
        "<th>Nomina </th>" +
        "<th>Quincena </th>" +
        "<th>Delegacion </th>" +
        "<th>Beneficiario </th>" +
        "<th>Num. Empleado </th>" +
        "<th>Liquido </th>" +
        "<th>Folio Cheque </th>" +
        "<th>Banco </th>" +
        "<th> </th>" +
        "</tr>" +
        "</thead>" +
        "</table>"
    );
};

let tablaRecuperarFolios;
function PintarResumenRecuperarFolios(datos) {
     tablaRecuperarFolios = $('#TablaRecuperarFolios').DataTable({
        "ordering": true,
        "info": true,
        "searching": false,
        "paging": true,
        "lengthMenu": [10, 20],
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
            { "data": "Anio" },
            { "data": "Id_nom" },
            { "data": "Nomina" },
            { "data": "Quincena" },
            { "data": "Delegacion" },
            { "data": "Beneficiario" },
            { "data": "NumEmpleado" },
            { "data": "Liquido" },
            { "data": "FolioCheque" },
            { "data": "CuentaBancaria" },
            {
                render: function (data, type, row) {
                    //console.log(row);
                   // return '<button class="btn btn-lg btn-info text-light RecuperarFolioSeleccionado"  onclick="RecuperarFolioSeleccionado('+row.IdPago+')"  > <i class="fas fa-undo"></i> </button>';
                    return '<button class="btn btn-lg btn-info text-light RecuperarFolioSeleccionado" > <i class="fas fa-undo"></i> </button>';

                }

            }
        ],
        "order": [[0, 'asc']],
        "columnDefs":
            [

                { className: "text-center ", visible: true, "targets": 0, },
                { className: "text-center ", visible: true, "targets": 1, },
                { className: "text-center ", visible: true, "targets": 2, },
                { className: "text-center ", visible: true, "targets": 3, },
                { className: "text-center ", visible: true, "targets": 4, },
                { className: "text-center ", visible: true, "targets": 5, },
                { className: "text-center ", visible: true, "targets": 6, },
                { className: "text-center ", visible: true, "targets": 7, },
                { className: "text-center ", visible: true, "targets": 8, },
                { className: "text-center ", visible: true, "targets": 9, },
                { className: "text-center ", visible: true, "targets": 10, },
                { className: "text-center ", visible: true, "targets": 11, }
            ]
    });
};



function eliminarTablaTablaResumenRecuperarFolios()
{
    $("#TablaResumenRecuperarFolios").empty();
}



let cuentaBancariaARecuperar = "";
let rpFoliosRangoInicial = "";
let rpFoliosRangoFinal = "";

let SePuedeRecuperarlosFolios = false;
function BuscarFolios()
{
    //limpia el data table que se creo si esta creado para  hacerle saber al cliente que los datos se actualizaron
    $("#TablaResumenRecuperarFolios").empty();
     cuentaBancariaARecuperar = document.getElementById("RecuperFoliosCuentaBancaria").value;
     rpFoliosRangoInicial = document.getElementById("recuperarFoliosRangoInicial").value;
     rpFoliosRangoFinal = document.getElementById("recuperarFoliosRangoFinal").value;



    if (cuentaBancariaARecuperar != "")
    {
        if (rpFoliosRangoInicial != "" && rpFoliosRangoFinal != "") {


            if (parseInt(rpFoliosRangoInicial) > 0 && parseInt(rpFoliosRangoFinal) > 0) {

                if (parseInt(rpFoliosRangoFinal) >= parseInt(rpFoliosRangoInicial)) {

                    let enviarDatos = {
                        IdCuentaBancaria: parseInt(cuentaBancariaARecuperar),
                        RangoInicial: parseInt(rpFoliosRangoInicial),
                        RangoFinal:   parseInt(rpFoliosRangoFinal) 
                    };

                    MensajeCargando();

                    axios.post('/Foliar/BuscarChequesARecuperar', {
                        IdCuentaBancaria: parseInt(cuentaBancariaARecuperar),
                        RangoInicial: parseInt(rpFoliosRangoInicial),
                        RangoFinal: parseInt(rpFoliosRangoFinal)
                    })
                    .then(function (response) {


                       // $("#TablaResumenRecuperarFolios").empty();
                        eliminarTablaTablaResumenRecuperarFolios();
                        DibujarResumenRecuperarFolios();
                        PintarResumenRecuperarFolios(response.data);

                        //console.log(response.data.length)

                        if (response.data.length > 0)
                        {
                            SePuedeRecuperarlosFolios = true;
                            document.getElementById("btnBucarFoliarARecuperar").style.display = "none";
                            document.getElementById("regresarRecuperador").style.display = "block";
                            document.getElementById("btnRecuperasFoliosEncontrados").style.display = "block";
                        }

                            OcultarMensajeCargando();

                    })
                    .catch(function (error) {
                            MensajeErrorSweet("Ocurrio un error intente de nuevo " + error)
                            OcultarMensajeCargando();
                    });


               

                } else {
                    MensajeErrorSweet("Corrija", "El Rango final no debe ser menor al rango inicial");
                }
            } else {
                MensajeErrorSweet("Corrija", "Asegurese de poner valores mayores a CERO en ambos rangos");
            }
        } else {
            MensajeErrorSweet("Corrija", "Asegurese de poner valores dentro de ambos rangos");
        }

    } else {
        MensajeErrorSweet("", "Asegurese de seleccionar un banco");
    }



}


function RecuperarFoliosEncontrados()
{
    if (SePuedeRecuperarlosFolios) {


        Swal.fire({
            title: '¿Esta seguro que desea recuperar el rango de cheques ingresado ?',
            text: 'Esto afectara los registros de las bases en donde se encuentren los folios de cheques, solo continue si esta seguro de querer hacerlo',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Si, Recuperar delete it!',
            cancelButtonText: 'No, Recuperar!',
            footer: '<a href="#">Contactar al desarrollador?</a>'
        }).then((result) => {
            if (result.isConfirmed) {

                MensajeCargando();
                axios.post('/Foliar/RecuperarChequesBuscados', {
                    IdCuentaBancaria: parseInt(cuentaBancariaARecuperar),
                    RangoInicial: parseInt(rpFoliosRangoInicial),
                    RangoFinal: parseInt(rpFoliosRangoFinal)
                })
                    .then(function (response) {

                        if (response.data.resultServer) {
                            MensajeCorrectoSweet("SE HAN RECUPERADO " + response.data.totalChequesRestaurados + " CHEQUES CORRECTAMENTE, VUELVA A FOLIAR LA BASE");
                        } else {
                            MensajeErrorSweet("ERROR  || " + response.data.Error)
                        }

                        RegresarRecuperadorFolios();
                        OcultarMensajeCargando();

                        //setTimeout(function () {
                        //    console.log("Timer Activado");
                        //    window.location.reload()
                        //}, 5000);

                    })
                    .catch(function (error) {
                        MensajeErrorSweet("Ocurrio un error intente de nuevo " + error)
                        OcultarMensajeCargando();
                    });

               
            }
        })





    } else
    {
        MensajeInformacionSweet("AL PARECER NO SE ENCONTRO NINGUN CHEQUE DENTRO DEL BANCO SELECCIONADO, POR ENDE NO PODEMOS RECUPERAR NADA");
    }

    
}


function RegresarRecuperadorFolios()
{
    MensajeCargando();
    //$("#TablaResumenRecuperarFolios").empty();
    eliminarTablaTablaResumenRecuperarFolios();
    cuentaBancariaARecuperar = "";
    rpFoliosRangoInicial = "";

    document.getElementById("recuperarFoliosRangoFinal").value = "";
    document.getElementById("regresarRecuperador").style.display = "none";
    document.getElementById("btnRecuperasFoliosEncontrados").style.display = "none";
    document.getElementById("btnBucarFoliarARecuperar").style.display = "block";
    OcultarMensajeCargando();
}




$(document).on("click", ".RecuperarFolioSeleccionado", function () {


    let recuperarFolio = tablaRecuperarFolios.row($(this).parents("tr")).data();


    let datoALimpiar = tablaRecuperarFolios.row($(this).parents("tr"));
  

    
    Swal.fire({
        title: "¿Esta seguro de recuperar este folio?",
        text: "Al restaurar este folio a su origen se quitara tanto de la base como el A-N de la bitacora por ende deberia volver a foliar el registro",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, Recuperarlo!'
    }).then((result) => {
        if (result.isConfirmed) {

            let enviarDatos = {
                IdPago: parseInt(recuperarFolio.IdPago)
            };


            MensajeCargando();

            axios.post('/Foliar/RestaurarFolioChequeDeIdPagoSeleccionado', {
                IdPago: parseInt(recuperarFolio.IdPago)
            })
            .then(function (response) {


                if (response.data.resultServer == 200) {
                    MensajeCorrecto_sinClickSweet("Se recupero el folio exitosamente");
                    datoALimpiar.remove().draw();

                } else {
                    MensajeErrorSweet(response.data.texto);
                }



                OcultarMensajeCargando();

            })
            .catch(function (error) {
                    MensajeErrorSweet("Ocurrio un error intente de nuevo " + error)
                    OcultarMensajeCargando();
            });


            //$.ajax({
            //    url: '/Foliar/RestaurarFolioChequeDeIdPagoSeleccionado',
            //    data: JSON.stringify(enviarDatos),
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    success: function (response) {


            //        //console.log(response);

            //        if (response.resultServer == 200) {
            //            MensajeCorrecto_sinClickSweet("Se recupero el folio exitosamente");
            //            datoALimpiar.remove().draw();
                        
            //        } else
            //        {
            //            MensajeErrorSweet(response.texto);
            //        }


                    
            //        OcultarMensajeCargando();

            //    }, error: function (jqXHR, textStatus) {
            //        MensajeErrorSweet("Ocurrio un error intente de nuevo " + textStatus)
            //        OcultarMensajeCargando();
            //    }
            //});




        }
    })



   // console.log(datoAEliminar.IdPago)

});

