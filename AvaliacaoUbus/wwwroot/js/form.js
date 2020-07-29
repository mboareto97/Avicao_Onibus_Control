window.onload = MontaPadraoCargaPersonalizada();

function SalvaItemAdicional() {
    var itensString = TiposDeDadosSelCarga();

    if (itensString != null && itensString != '') {
        $.ajax({
            url: '/Veiculo/SalvaItemAdicional',
            type: 'POST',
            dataType: 'html',
            data: {
                itens: itensString,
                veiculoId: $('#id').val()
            },
            success: function () {
            }
        });
    }
}

function TiposDeDadosSelCarga() {
    var retorno = "";

    for (var i = 1; i < 6; i++) {

        if (document.getElementById('check_' + i.toString()) !== 'undefined') {
            if (document.getElementById('check_' + i.toString()).checked)
                retorno += i.toString() + ",";

            if (i == 5) {
                var virgula = retorno.lastIndexOf(',');
                retorno = retorno.substring(0, virgula);
            }
        }
    }

    return retorno;
}

function MontaPadraoCargaPersonalizada() {
    var itensAdicionais = Split($('#ItensVeiculo').val(), ',');

    for (var i = 0; i < itensAdicionais.length; i++) {
        $('#check_' + itensAdicionais[i]).prop("checked", true);
    }
}

function Split(dado, condicao) {
    var split = [];
    split = dado.split(condicao);

    var lista = [];

    for (var i = 0; i < split.length; i++) {
        lista.push(split[i]);
    }

    return lista;
}