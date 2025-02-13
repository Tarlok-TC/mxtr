$(document).ready(function () {

    $('#submit-button').click(function (event) {
        event.preventDefault();
        var result = validate();
        if (result != "") {
            $('.user-message').html(result).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            HideMessage();
        }
        else {
            submitMe();
        }
    });

    function submitMe() {
        var data = {
            'NewPassword': $.trim($('#txtNewPassword').val()),
            'Password': $.trim($('#txtPassword').val())
        };
        //console.log(data);
        $.ajax({
            url: $('#frmEditProfile').attr('action'),
            dataType: 'json',
            type: 'post',
            data: data,
            success: function (result) {
                if (result.Success) {
                    //window.location = result.Redirect;
                    clearFields();
                    showHideEdit(false);
                    $('.user-message').html(result.Message).removeClass('is-failure').addClass('is-success').fadeIn('fast');
                } else {
                    $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
                }
                HideMessage();
            }
        });
    }

    function validate() {
        var message = "";
        var password = $.trim($('#txtPassword').val());
        var newPassword = $.trim($('#txtNewPassword').val());
        var confirmPassword = $.trim($('#txtConfirmPassword').val());
        if (password == "" || newPassword == "" || confirmPassword == "") {
            message = "Please fill all the fields";
        }
        if (newPassword !== confirmPassword) {
            message = "New password and Confirm password do not match";
        }
        return message;
    }

    function showHideEdit(isShow) {
        if (isShow) {
            $("#dvEdit").show();
            $("#dvProfileInfo").hide();
        }
        else {
            $("#dvEdit").hide();
            $("#dvProfileInfo").show();
        }
    }

    function clearFields() {
        $('#txtPassword').val('');
        $('#txtNewPassword').val('');
        $('#txtConfirmPassword').val('');
    }

    function HideMessage() {
        setTimeout(function () { $('.user-message').hide(); }, 3000);
    }
});
