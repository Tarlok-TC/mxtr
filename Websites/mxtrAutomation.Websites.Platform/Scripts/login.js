$(document).ready(function () {
    $("#username").focus();
    $("#password").keyup(function (event) {
        if (event.keyCode == 13) {
            $("#submit-button").click();
        }
    });

    $('#submit-button').click(function (event) {
        event.preventDefault();
        submitMe();
    });

    function submitMe() {
        $('.form-sending').fadeIn('fast');

        var data = {
            'Username': $.trim($('#username').val()),
            'Password': $('#password').val()
        };
        //console.log(data);
        $.ajax({
            url: $('#login-form').attr('action'),
            dataType: 'json',
            type: 'post',
            async: false,
            data: data,
            success: function (result) {
                if (result.Success) {
                    window.location = result.Redirect;
                    if (result.KlipfolioAuthData != null) {
                        console.log(result.KlipfolioAuthData)
                        ssoAuth(result.KlipfolioAuthData.KlipfolioSSOToken, result.KlipfolioAuthData.KlipfolioCompanyID);//Single Sign On Authentication
                    }
                }
                else {
                    $('.form-sending').hide();
                    $('.user-message').html(result.Message).addClass('is-failure').fadeIn();
                }
            }
        });
    }



});
