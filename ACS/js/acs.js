function enviarAlert() {
    for (var i = 1; i < document.getElementById("tablaCuotas").rows.length; i++) {
        var mFc = document.getElementById("tablaCuotas").rows[i].cells[2].innerHTML;
        var mFvalue = trim(mFc);
        if (mFvalue == "Debe") {
            document.getElementById('tablaCuotas').rows[i].setAttribute("class", "danger");
        } else {
            document.getElementById('tablaCuotas').rows[i].setAttribute("class", "success");
        }
        
    }
    
}
// funcion para eliminar espacios en blanco
function trim(cadena){
    var retorno=cadena.replace(/^\s+/g,'');
    retorno=retorno.replace(/\s+$/g,'');
    return retorno;
}