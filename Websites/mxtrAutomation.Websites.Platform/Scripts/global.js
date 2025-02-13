/* =====
    Global JS file for any site wide JS
===== */
$(document).ready(function () {

    if ($('body').hasClass('iphone')) {
        // When ready...
        window.addEventListener("load", function () {
            // Set a timeout...
            setTimeout(function () {
                // Hide the address bar!
                window.scrollTo(0, 1);
            }, 0);
        });
    }
    $("img.no-cache").each(function( index ) {
        var d = new Date();
        $(this).attr("src",$(this).attr("src")+"?t="+d.getTime());
    });
});


//********************************
//COOKIE FUNCTIONS
//********************************
function setCookie(name, value, days) {
    var expireDate = new Date();
    expireDate.setDate(expireDate.getDate() + days);
    //var cookieValue = escape(value) + ((days == null) ? "" : "; expires=" + expireDate.toUTCString());
    var cookieValue = escape(value) + ((days == null) ? "" : "; expires=" + expireDate.toUTCString()) + "; path=/";
    document.cookie = name + '=' + cookieValue;
}

function cookieExists(name) {
    var result = typeof getCookie(name) === "undefined";
    return !result;
}

function getCookie(name) {
    var i, x, y, ARRcookies = document.cookie.split(";");

    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");

        if (x == name) {
            return unescape(y);
        }
    }
}

function formatDecimals(num, decimals) {
    if (typeof decimals == 'undefined') {
        decimals = 2;
    }
    num = isNaN(num) || num === '' || num === null ? 0.00 : num;
    return parseFloat(num).toFixed(decimals);
}

function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function numberToAbsolutePercentage(x) {
    return Math.abs(Math.round(x * 100));
}

function notificationBarFailure(target, message) {
    $(target).html('<strong>Error: </strong>' + message).removeClass('is-success is-warning').addClass('is-failure');
    showNotificationBar(target);
}

function notificationBarWarning(target, message) {
    $(target).html('<strong>Warning: </strong>' + message).removeClass('is-success is-warning').addClass('is-warning');
    showNotificationBar(target);
}

function notificationBarSuccess(target, message) {
    $(target).html('<strong>Success: </strong>' + message).removeClass('is-failure is-warning').addClass('is-success');
    showNotificationBar(target);
}

function notificationBarStatus(target, message) {
    $(target).html('<strong>Status: </strong>' + message).removeClass('is-success is-warning').addClass('is-warning');
    showNotificationBar(target);
}

function showNotificationBar(target) {
    $(target).fadeIn();
}

function hideNotificationBar() {
    $('.notification-bar').hide().html('').removeClass('is-failure is-warning is-success');
}

function timeoutHideNotificationBar(target) {
    $(target).fadeOut('slow', function () {
        $(this).html('').removeClass('is-failure is-warning is-success');
    });
}

function disableSubmitButtons(form) {
    var submitButtons = $(form).find('button');
    $(submitButtons).prop('disabled', true);
}

function enableSubmitButtons(form) {
    var submitButtons = $(form).find('button');
    $(submitButtons).prop('disabled', false);
}

function serializeForm(form) {
    var fields = $(form).serializeArray();
    var query = '';

    $.each(fields, function (i, field) {
        if (field.value == 'on')
            field.value = 'true';
        if (field.value == 'off')
            field.value = 'false';
    });

    var dict = [];

    $.each(fields, function (i, field) {

        if (!(dict[field.name] == true)) {
            var vals = '';

            $.each(fields, function (j, f) {
                if (f.name == field.name) {
                    if (vals != '')
                        vals += ',';
                    vals += f.value;
                }
            });

            if (query != '')
                query += '&';
            query += field.name + '=' + vals;
            dict[field.name] = true;
        }
    });

    return query;
}


function executeCallbackFromFilters(callback) {

    if (callback !== undefined) {
        var fn = window[callback];
        fn(callback);
    }
    else {
        console.log(callback + ' function does not exist.');
    }
}