
$(document).ready(function () {
    GetGlobalAccountId_Workspace();
    $("#spnAccountCount").text(GetSelectAccountIdText(GetWorkspaceFilterIds()));
    bindMap();
});


var mapColorNoLead = { North: "rgba(255, 25, 164, 0.50)", East: "rgba(58, 175, 227, 0.50)", West: "rgba(112, 255, 0, 0.50)", South: "rgba(41, 203, 169, 0.50)" };
var mapColorWithLead = { North: "rgba(255, 25, 164, 1)", East: "rgba(58, 175, 227, 1)", West: "rgba(112, 255, 0, 1)", South: "rgba(41, 203, 169, 1)" };
var mapColor = {

    // North
    "US-CT": mapColorNoLead.North,
    "US-ME": mapColorNoLead.North,
    "US-MA": mapColorNoLead.North,
    "US-NH": mapColorNoLead.North,
    "US-NJ": mapColorNoLead.North,
    "US-NY": mapColorNoLead.North,
    "US-PA": mapColorNoLead.North,
    "US-RI": mapColorNoLead.North,
    "US-VT": mapColorNoLead.North,

    // East
    "US-IN": mapColorNoLead.East,
    "US-IA": mapColorNoLead.East,
    "US-IL": mapColorNoLead.East,
    "US-KS": mapColorNoLead.East,
    "US-MI": mapColorNoLead.East,
    "US-MN": mapColorNoLead.East,
    "US-OH": mapColorNoLead.East,
    "US-MO": mapColorNoLead.East,
    "US-NE": mapColorNoLead.East,
    "US-ND": mapColorNoLead.East,
    "US-SD": mapColorNoLead.East,
    "US-WI": mapColorNoLead.East,

    // South
    "US-DE": mapColorNoLead.South,
    "US-AL": mapColorNoLead.South,
    "US-AR": mapColorNoLead.South,
    "US-DC": mapColorNoLead.South,
    "US-KY": mapColorNoLead.South,
    "US-LA": mapColorNoLead.South,
    "US-FL": mapColorNoLead.South,
    "US-MS": mapColorNoLead.South,
    "US-OK": mapColorNoLead.South,
    "US-GA": mapColorNoLead.South,
    "US-MD": mapColorNoLead.South,
    "US-TN": mapColorNoLead.South,
    "US-TX": mapColorNoLead.South,
    "US-NC": mapColorNoLead.South,
    "US-SC": mapColorNoLead.South,
    "US-VA": mapColorNoLead.South,
    "US-WV": mapColorNoLead.South,

    // West
    "US-AZ": mapColorNoLead.West,
    "US-MT": mapColorNoLead.West,
    "US-AK": mapColorNoLead.West,
    "US-CO": mapColorNoLead.West,
    "US-UT": mapColorNoLead.West,
    "US-CA": mapColorNoLead.West,
    "US-ID": mapColorNoLead.West,
    "US-NV": mapColorNoLead.West,
    "US-HI": mapColorNoLead.West,
    "US-NM": mapColorNoLead.West,
    "US-WY": mapColorNoLead.West,
    "US-OR": mapColorNoLead.West,
    "US-WA": mapColorNoLead.West
}

function bindMap() {
    var _setMapColor = $.extend({}, mapColor);

    $.each(_mapData, function (i, item) {
        if (_setMapColor[item.State] && item.TotalLeads > 0) {
            switch (item.Region) {
                case "North":
                    _setMapColor[item.State] = mapColorWithLead.North;
                    break;
                case "South":
                    _setMapColor[item.State] = mapColorWithLead.South;
                    break;
                case "East":
                    _setMapColor[item.State] = mapColorWithLead.East;
                    break;
                case "West":
                    _setMapColor[item.State] = mapColorWithLead.West;
                    break;
            }
        }
    });
    $('#usa_map').html('');
    $('#usa_map').vectorMap({
        map: 'us_aea_en',
        //markers: data.metro.coords,
        id: "asd",
        backgroundColor: 'transparent',
        series: {
            //markers: [{
            //    attribute: 'fill',
            //    scale: ['#FEE5D9', '#A50F15'],
            //    values: data.metro.unemployment[val],
            //    min: jvm.min(metroUnemplValues),
            //    max: jvm.max(metroUnemplValues)
            //}, {
            //    attribute: 'r',
            //    scale: [5, 20],
            //    values: data.metro.population[val],
            //    min: jvm.min(metroPopValues),
            //    max: jvm.max(metroPopValues)
            //}],
            regions: [{
                //scale: ['#E6F2F0', '#149B7E'],
                values: _setMapColor,
                attribute: 'fill'//,
                //values: data.states[val]//,
                //min: jvm.min(statesValues),
                //max: jvm.max(statesValues)
            }]
        },
        onRegionTipShow: function (event, label, code) {
            var mapData = GetRegionData(code, 1)
            label.html(
                mapData.toolTipData
            );
        }, onRegionClick: function (event, code) {
            var mapData = GetRegionData(code, 2);
            if (mapData.AccountObjectIDs != "" && mapData.AccountObjectIDs != null)
                SetworkspacesAccountIdsCache(mapData.AccountObjectIDs)
            else
                ShowAlertPopUp();
        }
    });
}

function ShowAlertPopUp(message) {
    PNotify.removeAll();
    new PNotify({
        text: "No data found!",
        type: 'info',
        hide: true,
        styling: 'bootstrap3',
        addclass: 'dark',
        delay: 2000
    });
    $('.ui-pnotify-sticker').css('display', 'none');
}

function GetRegionData(code, returnDataType) {
    var mapData = { "toolTipData": '', AccountObjectIDs: '' };

    var isaccountobjectIdsAddFirst = false;
    //var toolTipData, AccountObjectIDs = '';
    $.each(_mapData, function (i, item) {
        if (item.State == code) {
            mapData.toolTipData = '<b>' + item.Region + ': Leads Count</b></br>';
            $.each(_mapData, function (i, item2) {
                if (item2.Region == item.Region) {
                    mapData.toolTipData = mapData.toolTipData + '<b>' + item2.StateName + ': </b>' + item2.TotalLeads + '</br>'
                    if (item2.AccountObjectID != null && item2.AccountObjectID != "")
                        if (!isaccountobjectIdsAddFirst) {
                            mapData.AccountObjectIDs = mapData.AccountObjectIDs + item2.AccountObjectID;
                            isaccountobjectIdsAddFirst = true;
                        }
                        else
                            mapData.AccountObjectIDs = mapData.AccountObjectIDs + ',' + item2.AccountObjectID;

                }
            });
        }
    });
    return mapData;

}


function SetworkspacesAccountIdsCache(accountObjectIDs) {
    //_workspaces = accountObjectIDs;
    $.ajax({
        url: '/SetworkspacesAccountIdsCache',
        dataType: 'json',
        type: 'post',
        data: { "accountObjectId": accountObjectIDs },
        async: true,
        success: function (result) {
            if (result.Success) {
                SetGlobalAccountId_Workspace();
                window.location.href = "/retailers/retailers/";
            }
            NProgress.done();
        },
        error: function (err) {
            NProgress.done();
        }
    });
}

function updatePageFromWorkspace() {
    NProgress.start();
    SetGlobalAccountId_Workspace();
    //$('.refresh-data').removeClass('hidden');
    _filterGraph.accountIDs = GetWorkspaceFilterIds();
    $("#spnAccountCount").text(GetSelectAccountIdText(actIds));
    var data = {
        'StartDate': _filterGraph.startdate,
        'EndDate': _filterGraph.enddate,
        'IsAjax': true,
        'AccountObjectIDs': _filterGraph.accountIDs
    };
    $.ajax({


        url: _updateDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                _data = result.IndexActivityAccountViewData;
                updateScoreBoxesandChart(result);
            }
            NProgress.done();
        },
        error: function (err) {
            NProgress.done();
        }
    });
}

function updateScoreBoxesandChart(result) {
    _mapData = result.GroupLeadsViewData;
    if (result.TotalLeads < 1)
        ShowAlertPopUp();
    $("#dvTotalRetailers").text(result.TotalRetailers);
    $("#dvTotalLeads").text(result.TotalLeads);
    $("#dvAverageLead").text(result.AverageLead);
    bindMap();
}

