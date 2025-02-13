$(document).ready(function () {
    openOverviewTab();
    init_timeline(_leadEvents);
    ShowBreadCrumb(GetType());
    eventSelectedOption = EventTypeEnum.All;
    accountSelecctedOption = EventTypeEnum.AllAccounts;
    AddLeadAccountNameToFilter(GetLeadAccountName(_leadEvents));
});

var eventSelectedOption = '';
var accountSelecctedOption = '';
var accName = ''

function GetLeadAccountName(arrVal) {
    var dataEvent = arrVal;
    //dataEvent[0].LeadAccountName = 'test';
    //dataEvent[0].IsCopied = true;
    //var sortedData = _.sortBy(dataEvent, 'IsCopied').reverse(); 
    // var sortedData = _.sortBy(dataEvent, 'IsCopied');
    var sortedData = _.sortBy(dataEvent, 'CopiedToParent');
    var arr = _(sortedData).chain().flatten().pluck('LeadAccountName').unique().value();
    //console.log("=== arr", arr);
    return arr;

}

function AddLeadAccountNameToFilter(arrLeadAccountNames) {
    $.each(arrLeadAccountNames, function (key, value) {
        //alert(key + ": " + value);
        $('#filterAccountMenu li:eq(' + key + ')').after('<li><a id="filterLeadAccountName_' + key + '" onclick="filterEventData(this.id,EventTypeEnum.LeadAccountName,FilterTypeEnum.Account)" class="toggle-link toggle-typeFilter link-active toggle-all-events" data-timeline-event=""><span class="checkbox"><i class="fa" aria-hidden="true"></i></span> ' + value + '</a></li>');
    });
}


function openOverviewTab() {
    $("#Overview_tab").attr('aria-expanded', 'true');
    $("#Overview_tab").parent().addClass('active');
    $("#tab_Overview").addClass('active in');
}

function filterEventData(whichItem, type, filterType) {
    if (filterType === FilterTypeEnum.Event) {
        $("#filterMenu > li > a > span i").removeClass('fa-check');
        eventSelectedOption = type;
        isShowHideEventDropdown = true;
        //alert(eventSelectedOption);
    }
    else {
        $("#filterAccountMenu > li > a > span i").removeClass('fa-check');
        accountSelecctedOption = type;
        if (type === EventTypeEnum.LeadAccountName) {
            accName = $.trim($("#" + whichItem).text());
        }
        else {
            accName = '';
        }
        isShowHideAccountDropdown = true;
        //alert(accName);
        //alert(accountSelecctedOption);
    }
    $("#" + whichItem + " > span > i").addClass('fa-check');
    filterData = GetFilteredData(type, eventSelectedOption, accountSelecctedOption, accName);
    init_timeline(filterData);
}

function GetFilteredData(type, eventSelected, accountSelected, leadAccountName) {
    if (type === EventTypeEnum.All) {
        if (accountSelected === EventTypeEnum.AllAccounts) {
            filterData = _leadEvents;
        }
        else {
            if (leadAccountName != '') {
                filterData = _.where(_leadEvents, { LeadAccountName: leadAccountName });
            }
            else {
                filterData = _leadEvents;
            }
        }
    }
    else if (type === EventTypeEnum.Email) {
        filterData = filteringData(accountSelected, EventTypeEnum.Email, leadAccountName);
    }
    else if (type === EventTypeEnum.PageVisit) {
        filterData = filteringData(accountSelected, EventTypeEnum.PageVisit, leadAccountName);
    }
    else if (type === EventTypeEnum.FormSubmitted) {
        filterData = filteringData(accountSelected, EventTypeEnum.FormSubmitted, leadAccountName);
    }
    else if (type === EventTypeEnum.Opportunity) {
        filterData = filteringData(accountSelected, EventTypeEnum.Opportunity, leadAccountName);
    }
    else if (type === EventTypeEnum.Other) {
        if (accountSelected === EventTypeEnum.AllAccounts) {
            filterData = _.filter(_leadEvents, function (item) {
                return (
                    item.EventType == ' ' ||
                    item.EventType == null ||
                    item.EventType == 'timeline-event-dot' ||
                    item.EventType == 'mbox' ||
                    item.EventType == 'import'
                    );
            });
        }
        else {
            if (leadAccountName != '') {
                filterData = _.filter(_leadEvents, function (item) {
                    return (
                        (item.EventType == ' ' ||
                        item.EventType == null ||
                        item.EventType == 'timeline-event-dot' ||
                        item.EventType == 'mbox' ||
                        item.EventType == 'import') &&
                        item.LeadAccountName == leadAccountName
                        );
                });
            }
            else {
                filterData = _.filter(_leadEvents, function (item) {
                    return (
                        item.EventType == ' ' ||
                        item.EventType == null ||
                        item.EventType == 'timeline-event-dot' ||
                        item.EventType == 'mbox' ||
                        item.EventType == 'import'
                        );
                });
            }
        }
    }
    else if (type === EventTypeEnum.LeadAccountName) {
        // alert(0);
        if (eventSelected === EventTypeEnum.All) {
            // alert(1);
            filterData = _.where(_leadEvents, { LeadAccountName: leadAccountName });
        }
        else {
            // alert(2);
            if (eventSelected === EventTypeEnum.Other) {
                filterData = _.filter(_leadEvents, function (item) {
                    return (
                        (item.EventType == ' ' ||
                        item.EventType == null ||
                        item.EventType == 'timeline-event-dot' ||
                        item.EventType == 'mbox' ||
                        item.EventType == 'import') &&
                        item.LeadAccountName == leadAccountName
                        );
                });
            }
            else {
                var eventName = GetEventName(eventSelected)
                filterData = _.filter(_leadEvents, function (item) {
                    return (
                        item.EventType == eventName && item.LeadAccountName == leadAccountName);
                });
            }
        }
    }
    else if (type === EventTypeEnum.AllAccounts) {
        if (eventSelected === EventTypeEnum.All) {
            filterData = _leadEvents;
        }
        else {
            if (eventSelected === EventTypeEnum.Other) {
                filterData = _.filter(_leadEvents, function (item) {
                    return (
                        item.EventType == ' ' ||
                        item.EventType == null ||
                        item.EventType == 'timeline-event-dot' ||
                        item.EventType == 'mbox' ||
                        item.EventType == 'import'
                        );
                });
            }
            else {
                var eventName = GetEventName(eventSelected)
                filterData = _.where(_leadEvents, { EventType: eventName });
            }
        }

    }
    return filterData;
}

function filteringData(whichAccount, eventtype, leadAccName) {
    var eventName = GetEventName(eventtype);
    if (whichAccount === EventTypeEnum.AllAccounts) {
        return _.where(_leadEvents, { EventType: eventName });
    }
    else {
        if (leadAccName != '') {
            return _.filter(_leadEvents, function (item) {
                return (
                    item.EventType == eventName && item.LeadAccountName == leadAccName);
            });
        }
        else {
            return _.where(_leadEvents, { EventType: eventName });
        }
    }
    return "";
}

function GetEventName(eventtype) {
    var eName = '';
    switch (eventtype) {
        case 1:
            eName = 'email';
            break;
        case 2:
            eName = 'pageVisit';
            break;
        case 3:
            eName = 'form';
            break;
        case 5:
            eName = 'opportunity';
            break;
        default:
            eName = '';
            break;

    }
    return eName;
}

var FilterTypeEnum = {
    Account: 0,
    Event: 1,
}

var EventTypeEnum = {
    All: 0,
    Email: 1,
    PageVisit: 2,
    FormSubmitted: 3,
    Other: 4,
    Opportunity: 5,
    LeadAccountName: 6,
    AllAccounts: 7,
}

var comingFromEnum = {
    RetailDetail: 0,
    Other: 1,
    DealerDetail: 2,
}

function GetType() {
    var qrStr = decodeURI(window.location.search);
    var spQrStr = qrStr.substring(1);
    //get all query string values
    var arr = spQrStr.split('&');
    if (arr.length > 1) {
        var arrSourceInfo = arr[1].split('=');
        var comingFrom = arrSourceInfo.toString().split(',')[1];
        if (comingFrom == "ret") {
            return comingFromEnum.RetailDetail;
        }
        else if (comingFrom == "det") {
            return comingFromEnum.DealerDetail;
        }
    }
    return comingFromEnum.Other;
}

function ShowBreadCrumb(comingfrom) {
    $("#dvRetailer").hide();
    $("#dvLead").hide();
    switch (comingfrom) {
        case 0:
            $("#dvRetailer").show();
            break;
        case 1:
            $("#dvLead").show();
            break;
        case 2:
            $("#dvDealer").show();
            break;
        default:
    }
}

$('#btnCopyLead').click(function (event) {
    event.preventDefault();
    CopyLead();
});

$("#CopyLead").change(function () {
    var currentLeadId = $(this).val();
    if (_lstCopiedLeadId.indexOf(currentLeadId) > -1) {
        $('#btnCopyLead').attr("disabled", true);
    }
    else {
        $('#btnCopyLead').removeAttr("disabled");
    }
});

function CopyLead() {
    if (parseInt($.trim($('#CopyLead').val().length)) === 0) {
        $('.user-message').html("Please select dealer to copy lead").removeClass('is-success').addClass('is-failure').fadeIn('fast');
        HideMessage();
    }
    else {
        CreateLeadUsingSharpspring($.trim($('#CopyLead').val()), false);
    }
}

$('#btnDeleteLead').click(function () {
    var leadObjectId = GetParameterValues('id');
    QuestionAlert("Delete Lead", "Are you sure you want to delete lead ?", function () {
        var data = {
            'ClientObjectId': leadObjectId,
            'LeadId': 0,
        };
        $.ajax({
            url: _leadDeleteUrl,
            dataType: 'json',
            type: 'post',
            data: data,
            success: function (result) {
                if (result.Success) {
                    if (GetType() == 0) {
                        window.location.href = _retailersUrl;
                    }
                    else {
                        window.location.href = _leadsUrl;
                    }
                }
                else {
                    $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
                }
                HideMessage();
            }
        });

    }, function () {

    });
    // var result = confirm("Are you sure you want to delete lead ?")
    //if (result) {
    //    var data = {
    //        'ClientObjectId': leadObjectId,
    //        'LeadId': 0,
    //    };

    //    $.ajax({
    //        url: _leadDeleteUrl,
    //        dataType: 'json',
    //        type: 'post',
    //        data: data,
    //        success: function (result) {
    //            if (result.Success) {
    //                if (GetType() == 0) {
    //                    window.location.href = _retailersUrl;
    //                }
    //                else {
    //                    window.location.href = _leadsUrl;
    //                }
    //            }
    //            else {
    //                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
    //            }
    //            HideMessage();
    //        }
    //    });
    //}
});

function deleteLead(accountObjectId, leadId) {
    //alert(accountObjectId);
    //alert(leadId);
    QuestionAlert("Delete Lead", "Are you sure you want to delete lead from this dealer ?", function () {
        var data = {
            'ClientObjectId': accountObjectId,
            'LeadId': leadId,
        };
        $.ajax({
            url: _leadDeleteUrl,
            dataType: 'json',
            type: 'post',
            data: data,
            success: function (result) {
                if (result.Success) {
                    $("#liClient_" + accountObjectId).addClass('MakeDisable');
                    $('.user-message').html("Lead deleted successfully").removeClass('is-failure').addClass('is-success').fadeIn('fast');
                }
                else {
                    $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
                }
                $('#CopyLead').val('');
                HideMessage();
            }
        });

    }, function () {

    });
}

$('#btnCopyToParent').click(function (event) {
    event.preventDefault();
    CopyLeadToParent();
});

function CopyLeadToParent() {
    if (parseInt($.trim($('#CopyToParent').val().length)) === 0) {
        $('.user-message').html("Please select parent to copy lead").removeClass('is-success').addClass('is-failure').fadeIn('fast');
        HideMessage();
    }
    else {
        CreateLeadUsingSharpspring($.trim($('#CopyToParent').val()), true);
    }
}

function CreateLeadUsingSharpspring(accountObjectId, isparent) {
    var data = {
        'ClientObjectId': accountObjectId,
        'LeadObjectId': _leadId,
        'CopiedToParent': isparent,
    };
    //console.log(data);
    $.ajax({
        url: _leadCopyUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                if (!isparent) {
                    $('#CopyLead').val('');
                    $('#uiClonedAccount li:eq(' + parseInt($('#uiClonedAccount li').length - 1) + ')').after(' <li id="liClient_' + result.Key + '">' + result.Item1 + '<a href="javascript:void(0)" onclick="deleteLead(' + result.Key + ',' + result.Item3 + ')" title="Remove lead"><i class="fa fa-trash pull-right"></i></a></li>');
                    _lstCopiedLeadId.push(result.Key);
                }
                $('.user-message').html("Copied successfully").removeClass('is-failure').addClass('is-success').fadeIn('fast');
            }
            else {
                $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
            }
            HideMessage();
        }
    });
}

function GetParameterValues(param) {
    var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}

function HideMessage() {
    setTimeout(function () { $('.user-message').hide(); }, 3000);
}

$("#LifeOfLead_tab").click(function () {
    setTimeout(function () {
        adjustVisibleTimeRangeToAccommodateAllEvents();
        obj = getChartRangeDate();
        timeline.options.zoomMax = 1000 * 60 * 60 * 24 * obj.eventsDaysDiff * 1;
        //alert(obj.startDate);
        //alert(obj.endDate);
        // timeline.setVisibleChartRange(new Date("2017-06-05"), new Date("2017-06-10"));
        timeline.setVisibleChartRange(new Date(obj.startDate), new Date(obj.endDate));
    }, 1000);
});

function getChartRangeDate() {
    var darr = _.pluck(_leadEvents, "CreateTimestamp");
    var dates = _.map(darr, function (date) { return moment(date) });
    var start = _.min(dates);
    var end = _.max(dates);

    var diff = new Date(end - start);
    var days = diff / 1000 / 60 / 60 / 24;
    //alert(days);
    days = parseInt(days);

    if (parseInt(days) > 18) {
        start = addDays(end, -18);
    }
    else {
        start = addDays(start, -3);
    }

    end = addDays(end, 3);

    start = formatDate(start);
    end = formatDate(end);

    if (days < 7) {
        days = 7;
    }
    var date_obj = { "startDate": '', "endDate": '', "eventsDaysDiff": '' };
    date_obj = { "startDate": start, "endDate": end, "eventsDaysDiff": days };
    return date_obj;
}

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [year, month, day].join('-');
}

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}

var isShowHideAccountDropdown = false;
var isShowHideEventDropdown = false;
function KeepShowHideDropdownOpen(filterType) {
    if (filterType === FilterTypeEnum.Event) {
        $("#dvOption").attr('aria-expanded', 'true');
        $("#ddlEvents").addClass('open');
    }

    if (filterType === FilterTypeEnum.Account) {
        $("#dvAccountOption").attr('aria-expanded', 'true');
        $("#ddlAccounts").addClass('open');
    }


}

$(document).click(function (e) {
    if (isShowHideAccountDropdown) {
        KeepShowHideDropdownOpen(FilterTypeEnum.Account);
        isShowHideAccountDropdown = false;
    }
    if (isShowHideEventDropdown) {
        KeepShowHideDropdownOpen(FilterTypeEnum.Event);
        isShowHideEventDropdown = false;
    }
});