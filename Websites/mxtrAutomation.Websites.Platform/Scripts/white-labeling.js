$(document).ready(function () {
    $("#spDomainName").text(setDomainName());
});

function setDomainName() {
    var baseUrl = '';
    var urlProtocol = window.location.protocol;
    var hostname = location.hostname;
    var regex = /(?:http[s]*\:\/\/)*(.*?)\.(?=[^\/]*\..{2,5})/i
    var result = hostname.match(regex);
    if (result && result.length) {
        if (result[1] == _domainName) {
            baseUrl = urlProtocol + "//" + hostname + "/";
        } else {
            baseUrl = urlProtocol + "//" + _domainName + "." + hostname.substring((hostname.indexOf('.') + 1), hostname.length) + "/";
        }
    } else {
        baseUrl = urlProtocol + "//" + _domainName + "." + hostname + "/";
    }
    return baseUrl;
}

function gotoCustomiseMenu() {
    window.location = _customiseMenuUrl;
}

function addUpdateDomain() {
    var domainName = _domainName;
    //window.location.pathname
    var baseUrl = setDomainName();

    SubmitAjaxAlert(
        "Please enter domain name",
        "text",
        domainName,
        "Your current domain is" + " " + baseUrl,
        function (resolve, reject, userInput) {
            setTimeout(function () {
                if ($.trim(userInput) == "") {
                    reject('Domain name can not be blank.')
                }
                else if (!isValidData(userInput)) {
                    reject('Invalid domain name.')
                }
                else {
                    resolve()
                }
            }, 2000)
        },
        function (userInput) {
            addUpdateDomainAPI(userInput);
        }
        );
}

function addUpdateHomePage() {
    var homePage = _homePageUrl;
    SubmitAjaxAlert(
        "Please enter home page url",
        "text",
        homePage,
        "",
        function (resolve, reject, userInput) {
            setTimeout(function () {
                if ($.trim(userInput) == "") {
                    reject('Home page url can not be blank.')
                }
                else {
                    resolve()
                }
            }, 2000)
        },
        function (userInput) {
            addUpdateHomePageAPI(userInput);
        }
        );
}

function isValidData(value) {
    if (/^[A-Z0-9]+$/i.test(value)) {
        return true;
    }
    else {
        return false;
    }
}

function addUpdateDomainAPI(userInput) {
    //ajax hit for saving/updating domain
    var data = {
        'DomainName': userInput,
    };
    $.ajax({
        url: _addUpdateDomainUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                _domainName = userInput;
                $("#spDomainName").text(setDomainName());
                $("#spCompanyName").text(_domainName);
                SuccessAlert('Domain add/edit finished!', 'Submitted domain: ' + userInput);
            }
            else {
                ErrorAlert("Error", result.Message)
            }
        }
    });
}

function isValidDomain(domainName) {
    if (/^(http(s)?\/\/:)?(www\.)?[a-zA-Z\-]{3,}(\.(com|net|org))?$/.test(domainName)) {
        return false;
    } else {
        return true;
    }
}

function addUpdateHomePageAPI(userInput) {
    var data = {
        'HomePageUrl': userInput,
    };
    $.ajax({
        url: _addUpdateHomePageUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                _homePageUrl = userInput;
                $("#spHomePageUrl").text(_homePageUrl);
                SuccessAlert('Home page url add/edit finished!', 'Home page url: ' + userInput);
                setTimeout(function () {
                    window.location.reload();
                }, 2000);
            }
            else {
                ErrorAlert("Error", result.Message)
            }
        }
    });
}


/* Logs Upload work */

var fileData;
var _date;
var _time;
if (_applicationLogoURL !== "") {
    $('#divApplicationLogo').show();
    $('#imgApplicationLogo').attr('src', _applicationLogoURL);
}
else {
    $('#divApplicationLogo').hide();
}
$("#ApplicationLogo").change(function () {
    readURL(this, 'imgApplicationLogo');
    $('#divApplicationLogo').show();
    AddApplicationLogo($('#ApplicationLogo').attr('id'));
});
$("#BrandingLogo").change(function () {
    readURL(this, 'imgBrandingLogo');
    $('#divBrandingLogo').show();
    AddBrandingLogo($('#BrandingLogo').attr('id'));
});
$("#FavIcon").change(function () {
    readURL(this, 'imgFavIcon');
    $('#divFavIcon').show();
    AddFavIcon($('#FavIcon').attr('id'));
});
function readURL(input, imgId) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#' + imgId).attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}
var data = {
    'ApplicationLogoURL': _WhiteLabelingURL,
};
function removeApplicationLogo() {
    QuestionAlert("Remove Application Logo", "Are you sure you want to remove application logo?", function () {
        $.ajax({
            type: "post",
            url: "/RemoveApplicationLogo",
            data: data,
            async: true,
            cache: false,
            success: function (response) {
                if (response.Success) {
                    $("#imgApplicationLogo").attr('src', '');
                    $('#divApplicationLogo').hide();
                    $('#sharedImgApplicationLogo,#lblAccountName').remove();
                    $('#lnkApplicationName').append('<span id="lblAccountName"><i class="fa fa-cog"></i> ' + response.Data.AccountName + '</span>');
                    SuccessAlert('Application logo remove successfully...');
                }
            }
        });

    }, function () {
    });
}
function removeBrandingLogo() {
    QuestionAlert("Remove Branding Logo", "Are you sure you want to remove branding logo?", function () {
        $.ajax({
            type: "post",
            url: "/RemoveBrandingLogo",
            data: data,
            async: true,
            cache: false,
            success: function (response) {
                if (response) {
                    $("#imgBrandingLogo").attr('src', '');
                    $('#divBrandingLogo').hide();
                    SuccessAlert('Branding logo remove successfully...');
                }
            }
        });

    }, function () {
    });
}
function removeFavIcon() {
    QuestionAlert("Remove Menu Bar Icon", "Are you sure you want to remove Menu bar icon?", function () {
        $.ajax({
            type: "post",
            url: "/RemoveFavIcon",
            data: data,
            async: true,
            cache: false,
            success: function (response) {
                if (response.Success) {
                    $("#imgFavIcon").attr('src', '');
                    $('#divFavIcon').hide();
                    $('#sharedImgFavIcon').remove();
                    $('#lnkApplicationIcon').append('<i id="facog" class="fa fa-cog"></i>');
                    SuccessAlert('Menu bar icon removed successfully...');
                }
            }
        });

    }, function () {
    });
}
function fileuploadthroughAjax(file) {
    _date = new Date();
    _time = _date.getTime();
    if (window.FormData !== undefined) {
        var fileUpload = $("#" + file).get(0);
        var files = fileUpload.files;
        fileData = new FormData();
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
    }
}
function AddApplicationLogo(file) {
    fileuploadthroughAjax(file);
    $.ajax({
        url: "/AddApplicationLogo",
        type: "POST",
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        data: fileData,
        success: function (result) {
            $('#sharedImgApplicationLogo,#lblAccountName').remove();
            $('#lnkApplicationName').append('<img id="sharedImgApplicationLogo" src="' + result.Data.ApplicationLogoURL + '?' + _time + '"/>');
            $('#imgApplicationLogo').attr('src', result.Data.ApplicationLogoURL + "?" + _time);
            SuccessAlert('Application logo upload successfully...');
        },
        error: function (err) {
            console.log(err.statusText);
            ErrorAlert("Something wrong");
        }
    });
}
function AddBrandingLogo(file) {
    fileuploadthroughAjax(file);
    $.ajax({
        url: "/AddBrandingLogo",
        type: "POST",
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        data: fileData,
        success: function (result) {
            $('#imgBrandingLogo').attr('src', result.Data.BrandingLogoURL + "?" + _time);
            SuccessAlert('Branding logo upload successfully...');
        },
        error: function (err) {
            console.log(err.statusText);
            ErrorAlert("Something wrong");
        }
    });
}
function AddFavIcon(file) {
    fileuploadthroughAjax(file);
    $.ajax({
        url: "/AddFavIcon",
        type: "POST",
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        data: fileData,
        success: function (result) {
            $('#sharedImgFavIcon,#facog').remove();
            $('#lnkApplicationIcon').append('<img id="sharedImgFavIcon" src="' + result.Data.FavIconURL + '?' + _time + '"/>');
            $('#imgFavIcon').attr('src', result.Data.FavIconURL + "?" + _time);
            SuccessAlert('Menu bar icon upload successfully...');
        },
        error: function (err) {
            console.log(err.statusText);
            ErrorAlert("Something wrong");
        }
    });
}