

$(document).ready(function () {
    $('.chkclass').click(function () {

        var getchkid = $(this).attr('id');
        var isChecked = $('#' + getchkid).is(':checked');

        if ($('#' + getchkid).is(':checked') == true) {

            // selectedAccounts.push[$(this).val()];
            //$('#td' + $(this).val()).css("color", "white");
            //$('#td' + $(this).val()).css("background-color", "blue");
        }
        else {
            // selectedAccounts.pop[$(this).val()];
            //$('#td' + $(this).val()).css("color", "black");
            //$('#td' + $(this).val()).css("background-color", "white");
        }
    });
});

var CRMKindEnum = {
    Sharpspring: 0,
    Bullseye: 1,
    GoogleAnalytics: 2,
    EZShred: 3,
    EZShredCustomerLists: 4,
    EZShredBuildingLists: 5,
    EZShredServiceLists: 6,
    EZShredMiscLists: 7,
}

function GetMiner(whichMiner) {
    switch (whichMiner) {
        case '0':
            return CRMKindEnum.Sharpspring;
        case '1':
            return CRMKindEnum.Bullseye;
        case '2':
            return CRMKindEnum.GoogleAnalytics;
        case '3':
            return CRMKindEnum.EZShred;
        case '4':
            return CRMKindEnum.EZShredCustomerLists;
        case '5':
            return CRMKindEnum.EZShredBuildingLists;
        case '6':
            return CRMKindEnum.EZShredServiceLists;
        case '7':
            return CRMKindEnum.EZShredMiscLists;
        default:

    }
}

function GetMinerText(whichMiner) {
    switch (whichMiner) {
        case '0':
            return 'Sharpspring';
        case '1':
            return 'Bullseye';
        case '2':
            return 'Google Analytics';
        case '3':
            return 'EZShred';
        case '4':
            return 'EZShred Customer Lists';
        case '5':
            return 'EZShred Building Lists';
        case '6':
            return 'EZShred Service Lists';
        case '7':
            return 'EZShred Misc Lists';
        default:

    }
}

function ValidateDate(dtValue) {
    var dtRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
    return dtRegex.test(dtValue);
}

function isValidDate(dateString) {
    var regEx = /^\d{4}-\d{2}-\d{2}$/;
    return dateString.match(regEx) != null;
}

function DeleteAuditTrailLogs() {
    if ($.trim($("#txtSSDeleteDate").val()) == '') {
        alert('Please enter date');
        return;
    }
    if (!isValidDate($("#txtSSDeleteDate").val())) {
        alert('Invalid date');
        return;
    }
    var selectedMiner = $("#ddlWhichMiner").val();
    if (selectedMiner == -1) {
        alert('Select Miner whose log to be deleted');
        return;
    }

    var selectedAccounts = null;
    selectedAccounts = [];

    $('input:checkbox:checked').each(function () {
        selectedAccounts.push($(this).attr('value'));
    });

    var data = {
        'LastEndDateForDataCollection': $("#txtSSDeleteDate").val(),
        'MinerType': GetMiner(selectedMiner),
        'IsAjax': true,
        'AccountObjectIDs': selectedAccounts.toString(),
    };
    $.ajax({
        url: _sharpSpringDeleteLogUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html(GetMinerText(selectedMiner) + " " + "Log deleted successfully").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            HideMessage();
        }
    });
}

function AssignDomainName() {
    var data = {};
    $.ajax({
        url: _assignDomainNameUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("Domain name assigned successfully").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            HideMessage();
        }
    });
}

function InsertUpdateBuilding() {
    var data = {};
    $("#dvdata_processing").fadeIn('fast');
    $.ajax({
        url: _insertUpdateBuildingUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("Insert Update Building successfull").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
            HideMessage();
        }
    });
}

function InsertUpdateCustomer() {
    var data = {};
    $("#dvdata_processing").fadeIn('fast');
    $.ajax({
        url: _insertUpdateCustomerUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("Insert Update Customer successfull").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
            HideMessage();
        }
    });
}

function AddUpdateCustomerDataFromJson() {
    var data = {
        'AccountObjectId': $("#txtAccountObjectId").val(),
        'MxtrObjectId': $("#txtMxtrObjectId").val(),
        'FileName': $("#txtFileName").val(),
    };
    $("#dvdata_processing").fadeIn('fast');
    $.ajax({
        url: _addUpdateCustomerDataFromJsonUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("Insert Update Customer successfull").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
            HideMessage();
        }
    });
}

function AddUpdateBuildingDataFromJson() {
    var data = {
        'AccountObjectId': $("#txtAccountObjectId").val(),
        'MxtrObjectId': $("#txtMxtrObjectId").val(),
        'FileName': $("#txtFileName").val(),
    };
    $("#dvdata_processing").fadeIn('fast');
    $.ajax({
        url: _addUpdateBuildingDataFromJsonUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("Insert Update Building successfull").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
            HideMessage();
        }
    });
}

function DeleteDuplicateCustomer() {
    var data = {};
    $("#dvdata_processing").fadeIn('fast');
    $.ajax({
        url: _deleteDuplicateCustomerUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("Duplicate Customer deleted successfull").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
            HideMessage();
        }
    });
}

function DeleteDuplicateBuilding() {
    var data = {};
    $("#dvdata_processing").fadeIn('fast');
    $.ajax({
        url: _deleteDuplicateBuildingUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("Duplicate Building deleted successfull").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
            HideMessage();
        }
    });
}

function HandleOldBuildingData() {
    var data = {};
    $("#dvdata_processing").fadeIn('fast');
    $.ajax({
        url: _handleOldBuildingDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("Old data added to Building 1 column successfull").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
            HideMessage();
        }
    });
}

function AssignOpportunityPipeLine() {
    var data = {};
    $("#dvdata_processing").fadeIn('fast');
    $.ajax({
        url: _assignOpportunityPipeLineDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("Opportunity PipeLine assigned successfully").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
            HideMessage();
        }
    });
}

function AssignCoordinatesToAccount() {
    var data = {};
    $("#dvdata_processing").fadeIn('fast');
    $('.user-message').html("");
    $.ajax({
        url: _assignCoordinatesToAccountDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("Coordinates assigned successfully").removeClass('is-failure').addClass('is-success').fadeIn('fast');
                HideMessage();
            }
            else {
                $('.user-message').html(result.Message + " Please try again").removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
        }
    });
}

function AddLeadAnalyticdata() {
    var data = {};
    $("#dvdata_processing").fadeIn('fast');
    $('.user-message').html("");
    $.ajax({
        url: _leadAnalyticDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $('.user-message').html("data added successfully").removeClass('is-failure').addClass('is-success').fadeIn('fast');
                HideMessage();
            }
            else {
                $('.user-message').html(result.Message + " Please try again").removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
        }
    });
}

function SetSubscribeLeadUpdates() {
    var data = {
        'SubscribeUrl': $("#txtsubscribeURL").val(),
    };
    $("#dvdata_processing").fadeIn('fast');
    $('.user-message').html("");
    $.ajax({
        url: _subscribeLeadUpdatesUrl,
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

function SSCreateDateFix() {
    var refreshIntervalId = setInterval(function () { PageActivator() }, 10000);
    var data = {};
    $("#dvdata_processing").fadeIn('fast');
    $('.updatecrmlead-message').html("");
    $.ajax({
        url: _ssCreateDateFixDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            clearInterval(refreshIntervalId);
            if (result.Success) {
                $('.updatecrmlead-message').html("Data updated successfully").removeClass('is-failure').addClass('is-success').fadeIn('fast');
                setTimeout(function () { $('.updatecrmlead-message').hide(); }, 5000);
            }
            else {
                $('.updatecrmlead-message').html(result.Message + " Please try again").removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
        }
    });
}

function PageActivator() {
    var data = {};
    $.ajax({
        url: "/PageActivator",
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
        }
    });
}

function UpdateCRMLeadAnalyticsFromCSV() {
    var data = {};
    $("#dvdata_processing").fadeIn('fast');
    $('.updatecrmleadanalytics-message').html("");
    $.ajax({
        url: "/UpdateCRMLeadAnalyticsFromCSV",
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                if (result.Message == "") {
                    $('.updatecrmleadanalytics-message').html("Data updated successfully").removeClass('is-failure').addClass('is-success').fadeIn('fast');
                    setTimeout(function () { $('.updatecrmleadanalytics-message').hide(); }, 5000);
                }
                else {
                    $('.updatecrmleadanalytics-message').html("Data updated successfully with error " + result.Message).removeClass('is-failure').addClass('is-success').fadeIn('fast');
                }
            }
            else {
                $('.updatecrmleadanalytics-message').html(result.Message + " Please try again").removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            setTimeout(function () {
                $("#dvdata_processing").fadeOut();
            }, 1000);
        }
    });
}

function HideMessage() {
    setTimeout(function () { $('.user-message').hide(); }, 5000);
}