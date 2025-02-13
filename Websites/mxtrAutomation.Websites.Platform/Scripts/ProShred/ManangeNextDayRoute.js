$(document).ready(function () {
    if ($('#Datasource').val() == "0") {
        $("#btnTicket").attr("disabled", true);
    }
});
$('#Datasource').change(function () {
    CheckNextDayRouteTicket();
});
function GetNextDayRouteTicket() {
    var attribute = $('#Datasource').find('option:selected');
    $("#btnTicket").attr("disabled", true);
    var data = {
        'AccountObjectId': attribute.attr("ObjectID"),
        'EzshredUserID': attribute.attr("id"),
        'LocationName': attribute.attr("value"),
    };
    $.ajax({
        url: _nextdayrouteticketURL,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.subscribeLeadUpdate-message').html(result.Message).removeClass('is-failure').addClass('is-success').fadeIn('fast');
                HideMessage();
            }
            else {
                $('.subscribeLeadUpdate-message').html(result.Message + " Please try again").removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
        }
    });
}
function CheckNextDayRouteTicket() {
    var attribute = $('#Datasource').find('option:selected');
    var data = {
        'AccountObjectId': attribute.attr("ObjectID"),
        'EzshredUserID': attribute.attr("id"),
    };

    if ($('#Datasource').val() != "0") {
        $.ajax({
            url: _checknextdayrouteticketURL,
            dataType: 'json',
            type: 'post',
            data: data,
            success: function (result) {
                if (result.Success) {
                    $("#btnTicket").attr("disabled", true);
                    alert("Ticket already generated");
                }
                else {
                    $("#btnTicket").attr("disabled", false);
                }
                setTimeout(function () {
                    $("#dvdata_processing").fadeOut();
                }, 1000);
            }
        });
    }
}