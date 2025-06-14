$(document).ready(function ()
{
    $(".card-body").LoadingOverlay("show");

    fetch('/Negocio/Obtener')
        .then(response =>
        {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson =>
        {
            console.log(responseJson);

            if (responseJson.estado)
            {
                const obj = responseJson.objeto;

                $("#txtNumeroDocumento").val(obj.numeroDocumento);
                $("#txtRazonSocial").val(obj.razonSocial);
                $("#txtCorreo").val(obj.correo);
                $("#txtDireccion").val(obj.direccion);
                $("#txtTelefono").val(obj.telefono);
                $("#txtImpuesto").val(obj.porcentajeImpuesto);
                $("#txtSimboloMoneda").val(obj.simboloMoneda);

                $("#imgLogo").attr("src", obj.urlLogo);
            }
            else
            {
                swal("Lo sentimos", responseJson.mensaje, "error");
            }
        });
});

$("#btnGuardarCambios").click(function ()
{
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter(item => item.value.trim() === "");

    if (inputs_sin_valor.length > 0)
    {
        const mensaje = `Debe completar el campo "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_sin_valor[0].name}"]`).focus();
        return;
    }

    const modelo =
    {
        numeroDocumento: $("#txtNumeroDocumento").val(),
        razonSocial: $("#txtRazonSocial").val(),
        correo:         $("#txtCorreo").val(),
        direccion: $("#txtDireccion").val(),
        telefono: $("#txtTelefono").val(),
        porcentajeImpuesto: $("#txtImpuesto").val(),
        simboloMoneda: ("#txtSimboloMoneda").val()
    };

    const inputLogo = document.getElementById("txtLogo");
    const formData = new FormData();

    formData.append("logo", inputLogo.file[0]);


});