






//El siguiente par es para el CRUD de los bancos agregados a la solicitud
function DibujarTablaCrudAjax(NumeroOrden) {
    $("#divTablaCrud").append(

        "<table id='tablaAgregados'  class='table table-striped display table-bordered table-hover' cellspacing='0'  style='width:100%'>" +
        " <caption class='text-uppercase'>Contenedores agregados al numero de orden  " + NumeroOrden + " </caption>"
        + "<thead class='tabla '>" +

        "<tr>" +
        "<th>Numero Contenedor </th>" +
        "<th>Folio Inicial</th>" +
        "<th>Folio Final</th>" +
        "<th>Total Formas de Pago</th>" +
        "<th></th>" +

        "</tr>" +
        "</thead>" +
        "</table>"
    );
};

let listaContenedoresAgregados;
function PintarConsultasTablaCrud(datos) {

    listaContenedoresAgregados = $('#tablaAgregados').DataTable({
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
            { "data": "iteradorContenedor" },
            { "data": "folioInicial" },
            { "data": "folioFinal" },
            { "data": "TotalFormas" },
            { "defaultContent": "<a class='editarContenedor btn btn-success' data-toggle='modal' data-target='#EditarContenedor'  > <i class='fas fa-edit'></i> Editar </a> <a class='eliminarContenedor btn btn-danger'> <i class='fas fa-trash-alt'></i> Eliminar</a>" }


        ],
        "columnDefs": [

            { className: "text-center col-1", visible: true, "targets": 0, },
            { className: "text-center col-2", visible: true, "targets": 1, },
            { className: "text-center col-2", visible: true, "targets": 2, },
            { className: "text-center col-1", visible: true, "targets": 3, },
            { className: "text-center col-3", visible: true, "targets": 4, }


        ],
        "order": [[1, 'asc']]

    });
};





//Agregar numero de order
let NumOrden;
let NumContenedor;
let iteradorContenedor = 1;
let id = 1;

function Continuar_NumeroOrderContenedores() {

    NumOrden = document.getElementById("NumOrden").value;

    NumContenedor = document.getElementById("NumContenedor").value;

  //  console.log(NumOrden);
  //  console.log(NumContenedor);

    if (NumOrden != null && NumOrden != "" && NumContenedor != null && NumContenedor != "") {

        document.getElementById("PrimerosDatos").style.display = "none"
        document.getElementById("Contenedores").style.display = "block"



        // document.getElementById("TituloDeCaption").value = `Contenedores agregados al numero de orden :  ${NumOrden}`;
        //agrega el numero uno de N cantidad de contenedores
        document.getElementById("IteradorContenedor").innerHTML = `${iteradorContenedor}` + ` ` + `de` + ` ` + NumContenedor;


    } else {

        MensajeWarningSweet('llene los datos faltantes para continuar');
    }
}




function Regresar() {
    NumContenedor = null;
    document.getElementById("PrimerosDatos").style.display = "block"
    //document.getElementById("TablaDeContenedores").style.display = "none"
    document.getElementById("Contenedores").style.display = "none"

}




//Inicio del crud de la tabla de contenedores agregados

let listaContenedores = new Array();

let folioInicial = null;
let folioFinal = null;


function Agregar() {
    folioInicial = document.getElementById("FolioInicialAgregar").value;
    folioFinal = document.getElementById("FolioFinalAgregar").value;




    //iteradorContenedor += 1;

    if (iteradorContenedor <= NumContenedor) {

        document.getElementById("IteradorContenedor").innerHTML = `${iteradorContenedor}` + ` ` + `de` + ` ` + NumContenedor;


        if (folioInicial != null) {

            if (folioFinal != null) {


                if (parseInt(folioFinal) > parseInt(folioInicial) || parseInt(folioFinal) == parseInt(folioInicial)) {


                    //console.log("entreee ff es mayor igual");
                    let TotalFormas = (folioFinal - folioInicial) + 1;
                    listaContenedores.push({ id, iteradorContenedor, folioInicial, folioFinal, TotalFormas });
                    //ContenedoresAgregados(IteradorDeContenedores, FInicial, FFinal, TotalFormas);

                    $('#FolioInicialAgregar').val('');
                    $('#FolioFinalAgregar').val('');


                  //  console.log(listaContenedores);

                    $("#divTablaCrud").empty();

                    //DibujarTablaAjax();
                    DibujarTablaCrudAjax(NumOrden);

                    //PintarConsultas(listaBancosSolicitados);
                    PintarConsultasTablaCrud(listaContenedores);
                    document.getElementById("GuardarFoliosContenedor_Agregar").style.display = "block";


                    id += 1;
                    iteradorContenedor += 1;
                    document.getElementById("IteradorContenedor").innerHTML = `${iteradorContenedor}` + ` ` + `de` + ` ` + NumContenedor;


                    //if (iteradorContenedor <= NumContenedor) {
                    //    document.getElementById("IteradorContenedor").innerHTML = `${iteradorContenedor}` + ` ` + `de` + ` ` + NumContenedor;
                    //}



                } else {

                    MensajeWarningSweet("'Verifique los datos del contenedor! EL Folio Final debe de ser mayor al Folio Inicial'")
                }





            } else {
                //ingrese un folio inicial
                MensajeWarningSweet("Ingrese un folio FINAL");
            }


        } else {
            //ingrese un folio FINAL

            MensajeWarningSweet("Ingrese un folio INICIAL");
        }




    } else {
        MensajeWarningSweet("No se pueden agregar mas contenedores. Cambie el numero de contenedores al inicio");
    }



}




/****  Funciones para editar  y Eliminar  ******* */

//Click para eliminar el elemento seleccionado de la tabla
$(document).on("click", ".eliminarContenedor", function () {



    //console.log("vieja" + listaBancosSolicitados);

    let datoAEliminar = listaContenedoresAgregados.row($(this).parents("tr")).data();
    let i = listaContenedores.indexOf(datoAEliminar);
    listaContenedores.splice(i, 1);



    // console.log(listaBancosSolicitados);

    listaContenedoresAgregados.row($(this).parents("tr")).remove().draw();

    iteradorContenedor -= 1;
    document.getElementById("IteradorContenedor").innerHTML = `${iteradorContenedor}` + ` ` + `de` + ` ` + NumContenedor;

});



let datoAEditar_Agregar;
//Click para editar el elemento seleccionado de la tabla
$(document).on("click", ".editarContenedor", function () {

    datoAEditar_Agregar = listaContenedoresAgregados.row($(this).parents("tr")).data();
    let i = listaContenedores.indexOf(datoAEditar_Agregar);


    document.getElementById("NumeroContenedor").innerHTML = listaContenedores[i].iteradorContenedor;
    document.getElementById("FolioInicialEditar").value = listaContenedores[i].folioInicial;
    document.getElementById("FolioFinalEditar").value = listaContenedores[i].folioFinal;




    //obtner datos dela edicion del banco
    const GuardarEdicion = document.getElementById("GuardarEdicionContenedor");
    GuardarEdicion.addEventListener("click",
        function () {


            //Obtiene los datos modificados del modal
            let folioInicialEdicion = document.getElementById('FolioInicialEditar').value;
            let folioFinalEdicion = document.getElementById('FolioFinalEditar').value;

            if (parseInt(folioFinalEdicion) > parseInt(folioInicialEdicion) || parseInt(folioFinalEdicion) == parseInt(folioInicialEdicion)) {

                //se muestra el mensaje cargando para bloquear la pantalla
                MensajeCargando();
                /// Editar(datoAEditar.Id);


                let indice = listaContenedores.findIndex(y => y.id == datoAEditar_Agregar.id);


                //se actualizan los varlores dentro del arreglo
                listaContenedores[indice].folioInicial = folioInicialEdicion;
                listaContenedores[indice].folioFinal = folioFinalEdicion;
                listaContenedores[indice].TotalFormas = (folioFinalEdicion - folioInicialEdicion) + 1;

                $("#divTablaCrud").empty();
                DibujarTablaCrudAjax(NumOrden);
                PintarConsultasTablaCrud(listaContenedores);

                //se oculta el modal
                $('#EditarContenedor').modal('hide');

                OcultarMensajeCargando();



            } else {

                MensajeWarningSweet('Verifique los datos del contenedor! EL folio final debe de ser mayor al folio fnicial o iguales');

            }




        }
    );


});







/***************** metodo para guardar la tabla en la DB **********///////////

function GuardarContenedoresFolios() {
    //console.log( listaContenedores);

    let IdBanco = document.getElementById("IdCuentaBancariaAgregar").innerHTML;
    let nombreENBanca = document.getElementById("Banco").innerHTML;
    //console.log(nombreENBanca)
    //console.log(IdBanco)
    

    MensajeCargando();
    $.ajax({
        url: 'GuardarInventarioAgregado',
        data: JSON.stringify({ listaContenedores, NumOrden, IdBanco }),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (resultado) {


            if (resultado == true) {
                
                MensajeCorrectoConRecargaPagina(`Se han agregado nuevos contenedores a ${nombreENBanca}`);

            } else {

                MensajeErrorSweet('', `No se guardaron los nuevos contenedores para el banco ${nombreENBanca}`);
            }

            OcultarMensajeCargando();


        },
        error: function (msg) {

            MensajeErrorSweet('', "Ocurrio un problema intente de nuevo")

            OcultarMensajeCargando();
        }
    });


}

