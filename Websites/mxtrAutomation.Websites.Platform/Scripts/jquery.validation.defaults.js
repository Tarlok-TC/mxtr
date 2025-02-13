jQuery.validator.setDefaults({
    errorElement: 'span',
    errorClass: 'is-error',
    validClass: 'is-valid',
    ignore: '.ignore',
    errorPlacement: function (error, element) {
        error.appendTo(element.parent()).css('display', 'none').fadeIn('slow');
    },
    highlight: function (element, errorClass, validClass) {
        $(element).addClass(errorClass).removeClass(validClass);
        $(element).parent().addClass(errorClass).removeClass(validClass);
    },
    unhighlight: function (element, errorClass, validClass, error) {
        $(element).addClass(validClass).removeClass(errorClass);
        $(element).parent().addClass(validClass).removeClass(errorClass);
    }
});

// PASSWORD STRENGTH CHECKER
jQuery.validator.addMethod('passwordStrength', function (value, element) {
    var check = false;
    var re = new RegExp('(?=.*[0-9])');
    return this.optional(element) || re.test(value);
}, 'minimum 8 characters with at least 1 number');


// HOSTNAME CHECKER
jQuery.validator.addMethod('hostnamecheck', function (value, element) {
    var check = false;
    var re = new RegExp('.*?([^.]+\\.[^.]+)');
    return re.test(value);
}, 'Invalid hostname');


// URL & SUBDOMAIN CHECKER
jQuery.validator.addMethod('urlcheck', function (value, element) {
    return jQueryValidatorValidatePathValue(value);
}, function (params, element) {
    var errorText = jQueryValidatorUrlErrorMessage(element);
    return errorText;
});


// MULTIPLE EMAILS
jQuery.validator.addMethod('multiemail', function (value, element) {
    if (this.optional(element)) {
        return true;
    }
    var emails = value.split(new RegExp('\s*,\s*'));
    valid = true;
    for (var i in emails) {
        str = emails[i];
        value = jQuery.trim(str); //TRIM TO ACCOUNT FOR SPACE AFTER COMMA
        valid = valid && jQuery.validator.methods.email.call(this, value, element);
    }
    return valid;
}, "Invalid Email - perhaps a comma at the end?");

// NOT AN EMPTY MEDIA ID
jQuery.validator.addMethod("emptymedia", function (value, element, param) {
    return this.optional(element) || value != '00000000-0000-0000-0000-000000000000';
}, "Please select a media source");

// URLCHECK utility methods
// error message
function jQueryValidatorUrlErrorMessage(element) {
    var value = $(element).val();
    var sanitizer = new jQueryValidatorUrlSanitizer();
    var cleanValue = sanitizer.cleanPath(value);
    if (value !== cleanValue) {
        return "Lower case letters, numbers and dashes only. Use: <a href='#' data-suggestion='" + cleanValue + "' class='use-urlcheck-value'>" + cleanValue + "</a>";
    }
}

// bool for url path error
function jQueryValidatorValidatePathValue(value) {
    var sanitizer = new jQueryValidatorUrlSanitizer();
    var cleanValue = sanitizer.cleanPath(value);
    if (value.toLowerCase().trim() !== cleanValue) {
        return false;
    } else {
        return true;
    }
}

// url cleanup
function jQueryValidatorUrlSanitizer() {

    function replaceInvalidCharacters(str) {
        str = str.replace(/[\/]{2,}/g, '/');
        return str.replace(/[^a-z0-9.]{1}/g, '-');
    }

    function stripTrailingCharacters(str) {
        while (str.substr(-1) === '/' || str.substr(-1) === '-') {
            str = str.substr(0, str.length - 1);
        }
        return str;
    }

    function normalize(str) {
        return str.toLowerCase().trim();
    }

    this.cleanPath = function (path) {
        path = normalize(path);
        path = replaceInvalidCharacters(path);
        path = stripTrailingCharacters(path);

        return path;
    }
}

// POST FORM DATA
function postFormData(form, notificationTarget, callbackFunction) {
    // FIND ANIMATION & MESSAGE ITEMS
    var formSending = $(form).find('.loader');

    callbackFunction = callbackFunction || function () { };

    $(formSending).fadeIn();

    disableSubmitButtons(form);
    $.ajax({
        url: $(form).attr('action'),
        dataType: 'json',
        type: 'post',
        data: $(form).serialize(),
        success: function (result) {
            if (result.Success) {
                $(formSending).fadeOut(function () {
                    notificationBarSuccess(notificationTarget, result.Message);
                    if (result.FromLogin) {
                        location.reload();
                    } else {
                        setTimeout(function () {
                            timeoutHideNotificationBar(notificationTarget);
                        }, 3000);
                        callbackFunction();
                    }
                });
            } else {
                $(formSending).fadeOut(function () {
                    notificationBarFailure(notificationTarget, result.Message);
                });
            }
            enableSubmitButtons(form);
        }
    });
}


// url use suggestion click function
$(document).on("click", 'a.use-urlcheck-value', function (event) {
    event.preventDefault();
    var $this = $(this);
    var $input = $this.closest('.fld-wrap').find('input');
    var inputValue = $this.data('suggestion');
    $input.focus().val(inputValue).blur();
});


 /* Lets you say "at least X inputs that match selector Y must be filled."
 *
 * The end result is that neither of these inputs:
 *
 *  <input class="productinfo" name="partnumber">
 *  <input class="productinfo" name="description">
 *
 *  ...will validate unless at least one of them is filled.
 *
 * partnumber:  {require_from_group: [1,".productinfo"]},
 * description: {require_from_group: [1,".productinfo"]}
 *
 */
jQuery.validator.addMethod("require_from_group", function (value, element, options) {
    var numberRequired = options[0];
    var selector = options[1];
    var fields = $(selector, element.form);
    if (fields.length > 0) {
        var filled_fields = fields.filter(function () {
            // it's more clear to compare with empty string
            return $(this).val() != "";
        });
        var empty_fields = fields.not(filled_fields);
        // we will mark only first empty field as invalid
        if (filled_fields.length < numberRequired) {
            return false;
        }
        return true;
    }
    return true;
    // {0} below is the 0th item in the options field


}, "Please fill out at least {0} of these fields.");

// NOTICE: Modified version of Castle.Components.Validator.CreditCardValidator
// Redistributed under the the Apache License 2.0 at http://www.apache.org/licenses/LICENSE-2.0
// Valid Types: mastercard, visa, amex, dinersclub, enroute, discover, jcb, unknown, all (overrides all other settings)
jQuery.validator.addMethod("creditcardtypes", function (value, element, param) {
    if (/[^0-9\-]+/.test(value)) {
        return false;
    }

    value = value.replace(/\D/g, "");

    var validTypes = 0x0000;

    if (param.mastercard) {
        validTypes |= 0x0001;
    }
    if (param.visa) {
        validTypes |= 0x0002;
    }
    if (param.amex) {
        validTypes |= 0x0004;
    }
    if (param.dinersclub) {
        validTypes |= 0x0008;
    }
    if (param.enroute) {
        validTypes |= 0x0010;
    }
    if (param.discover) {
        validTypes |= 0x0020;
    }
    if (param.jcb) {
        validTypes |= 0x0040;
    }
    if (param.unknown) {
        validTypes |= 0x0080;
    }
    if (param.all) {
        validTypes = 0x0001 | 0x0002 | 0x0004 | 0x0008 | 0x0010 | 0x0020 | 0x0040 | 0x0080;
    }
    if (validTypes & 0x0001 && /^(5[12345])/.test(value)) { //mastercard
        return value.length === 16;
    }
    if (validTypes & 0x0002 && /^(4)/.test(value)) { //visa
        return value.length === 16;
    }
    if (validTypes & 0x0004 && /^(3[47])/.test(value)) { //amex
        return value.length === 15;
    }
    if (validTypes & 0x0008 && /^(3(0[012345]|[68]))/.test(value)) { //dinersclub
        return value.length === 14;
    }
    if (validTypes & 0x0010 && /^(2(014|149))/.test(value)) { //enroute
        return value.length === 15;
    }
    if (validTypes & 0x0020 && /^(6011)/.test(value)) { //discover
        return value.length === 16;
    }
    if (validTypes & 0x0040 && /^(3)/.test(value)) { //jcb
        return value.length === 16;
    }
    if (validTypes & 0x0040 && /^(2131|1800)/.test(value)) { //jcb
        return value.length === 15;
    }
    if (validTypes & 0x0080) { //unknown
        return true;
    }
    return false;
}, "Please enter a valid credit card number.");