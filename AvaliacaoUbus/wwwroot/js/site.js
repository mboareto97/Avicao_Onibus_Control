const toggleCollapse = document.querySelector('.toggle-collapse span');
const nav = document.querySelector('.nav');

toggleCollapse.onclick = (e) => {
    nav.classList.toggle('collapse');
    e.target.classList.toggle('toggle-click');
};

function confirmaExclusaoSemValidacao(
    objeto,
    titulo,
    textoBotaoOk,
    textoBotaoCancela,
    link
) {
    swal({
        title: titulo,
        text: "Uma vez excluído, você não poderá recuperar este dado!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
    .then((willDelete) => {
        if (willDelete) {
            var $form = $(document.createElement('form'))
                .css({ display: 'none' })
                .attr('method', 'POST')
                .attr('action', link);
            $('body').append($form);
            $form.submit();
        } 
    });
}

function consultaViagens() {
    $.ajax({
        url: '/Viagem/Index',
        type: 'POST',
        dataType: 'html',
        data: {
            dataPesquisa: $('#DataPesquisa').val(),
            statusPesquisa: $('#StatusPesquisa').val()
        },
        success: function (resultado) {
            if (resultado != null) {
                document.body.innerHTML = '';
                document.body.innerHTML = resultado;
            }
        }
    });
}

function AtualizaViagem() {
    $.ajax({
        url: '/Viagem/AtualizaViagemRealizada',
        type: 'POST',
        dataType: 'html',
        success: function (resultado) {
            if (resultado != null) {
                document.body.innerHTML = '';
                document.body.innerHTML = resultado;
            }
        }
    });
}

function consultaMotoristas() {
    $.ajax({
        url: '/Funcionario/Index',
        type: 'POST',
        dataType: 'html',
        data: {
            origemId: $('#OrigemId').val(),
            destinoId: $('#DestinoId').val()
        },
        success: function (resultado) {
            if (resultado != null) {
                document.body.innerHTML = '';
                document.body.innerHTML = resultado;
            }
        }
    });
}

function consultaItensAdicionais() {
    $.ajax({
        url: '/ItensAdicionais/Index',
        type: 'POST',
        dataType: 'html',
        data: {
            origemId: $('#OrigemId').val(),
            destinoId: $('#DestinoId').val()
        },
        success: function (resultado) {
            if (resultado != null) {
                document.body.innerHTML = '';
                document.body.innerHTML = resultado;
            }
        }
    });
}