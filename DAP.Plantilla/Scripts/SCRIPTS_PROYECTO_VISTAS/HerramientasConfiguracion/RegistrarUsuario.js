

function AgregarUsuario() {
    let numEmpleadoSeleccionado = document.getElementById("buscadorEmpleado").value;

    if (numEmpleadoSeleccionado) {

        MensajeCargando();
        axios.post('/HerramientasConfiguracion/RegistrarUsuarioEnSistemaInterno',
            {
                numEmpleado: numEmpleadoSeleccionado

            }).then(function (response) {

                console.log(response.data);
                if (response.data != false) {
                    MensajeCorrecto_sinClickSweet("Usuario agregado con exito");
                }


            }).catch(function (error) {
                    MensajeErrorSweet('Intentelo de nuevo mas tarde o contacte al desarrollador', error);
            });


        OcultarMensajeCargando();
    }
    else {
        MensajeErrorSweet('', 'El campo del numero de empleado esta vacio');
    }

}