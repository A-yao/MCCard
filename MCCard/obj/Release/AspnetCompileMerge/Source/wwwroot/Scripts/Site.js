function ExecuteAjax(url, param, callback) {
    $.ajax({
        url: url,
        data: param != null ? JSON.stringify(param) : null,
        type: 'POST',
        dataType: "json",
        async: false,
        contentType: 'application/json; charset=utf-8',
        //beforeSend: function () {
        //    $('#loadingIMG').show();
        //},
        complete: function () {
            //$('#loadingIMG').hide();
            //$('#loadingInventory').hide();
        },
        success: function (data) {
            if (typeof (callback) == "function") {
                callback(data);
            }
        },
        failure: function (errMsg) {
        },
        error: function (errMsg) {
        }
    });
}

function validateEmail(email) {
    //reg = /^(([^<>()[]\.,;:s@"]+(.[^<>()[]\.,;:s@"]+)*)|(".+"))@(([[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}])|(([a-zA-Z-0-9]+.)+[a-zA-Z]{2,}))$/
    reg = /^[-a-z0-9~!$%^&*_=+}{\'?]+(\.[-a-z0-9~!$%^&*_=+}{\'?]+)*@([a-z0-9_][-a-z0-9_]*(\.[-a-z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-z][a-z])|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$/i;
    return reg.test(email);
}