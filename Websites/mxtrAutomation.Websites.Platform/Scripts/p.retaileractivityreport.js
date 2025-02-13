var isFooterRowRequired = true;
var isShowHide = false;
var totalRow = 0;

$(document).ready(function () {

    GetGlobalAccountId_Workspace();
    $("#spnAccountCount").text(GetSelectAccountIdText(GetWorkspaceFilterIds()));

    var flag = 0;
    var order = 0;

    //$('.m-report-filters-daterange').calendar360({
    //    start: moment(_filterGraph.startdate),
    //    end: moment(_filterGraph.enddate),
    //    dateFormat: 'MM/DD/YYYY',
    //    titleFormat: 'MMM. DD, YYYY',
    //    presets: {
    //        currentWeek: {
    //            text: 'Current Week',
    //            isDefault: false,
    //            timeshift: function (moment) {
    //                moment.startOf('week');
    //                return moment;
    //            }
    //        },
    //        currentMonth: {
    //            text: 'Current Month',
    //            isDefault: false,
    //            timeshift: function (moment) {
    //                moment.date(1);
    //                return moment;
    //            }
    //        },
    //        pastSevenDays: {
    //            text: 'Past 7 Days',
    //            isDefault: false,
    //            timeshift: function (moment) {
    //                moment.subtract(7, 'days');
    //                return moment;
    //            }
    //        },
    //        pastThirtyDays: {
    //            text: 'Past 30 Days',
    //            isDefault: true,
    //            timeshift: function (moment) {
    //                moment.subtract(30, 'days');
    //                return moment;
    //            }
    //        },
    //        yearToDate: {
    //            text: 'YTD',
    //            isDefault: false,
    //            timeshift: function (moment) {
    //                moment.dayOfYear(1);
    //                return moment;
    //            }
    //        }
    //    },
    //    post: updatePage
    //});

    c1 = 'rgba(179,179,179,0.9)';
    CreateAllCharts();
    generateRetailerActivityDataTable();

    $('a.toggle-vis').on('click', function (e) {
        var table = $('#RetailerActivityReport').DataTable();
        e.preventDefault();

        // Get the column API object
        var column = table.column($(this).attr('data-column'));

        // Toggle the visibility
        column.visible(!column.visible());
        table.columns.adjust().draw(false);
    });


    $('#RetailerActivityReport').on('order.dt', function (e, settings, len) {
        order = 1;
    });

    var tableSearch = $('#RetailerActivityReport').DataTable();
    tableSearch.on('search.dt', function () {
        flag = 1;
    });
});

$(document).click(function (e) {
    if (isShowHide) {
        KeepShowHideDropdownOpen();
        isShowHide = false;
    }
});

function CreateAllCharts() {
    buildChart(RetailerChartTypeEnum.Search);
    buildChart(RetailerChartTypeEnum.Leads);
    buildChart(RetailerChartTypeEnum.Email);
}

function KeepShowHideDropdownOpen() {
    $("#dvOption").attr('aria-expanded', 'true');
    $("#ddlShowHideColumn").addClass('open');
}

var RetailerChartTypeEnum = {
    Search: 0,
    Leads: 1,
    Email: 2,
};

function buildChart(chartType) {
    var chartId = "";
    switch (chartType) {
        case 0:
            chartId = "LocatorSearches_Chart";
            break;
        case 1:
            chartId = "Leads_Chart";
            break;
        case 2:
            chartId = "Email_Chart";
            break;
        default:

    }
    if ($('#' + chartId).length) {
        var echart = echarts.init(document.getElementById(chartId), theme);
        echart.setOption({
            title: {
                text: '',
                subtext: ''
            },
            tooltip: {
                trigger: 'axis'
            },
            toolbox: {
                show: true,
                feature: {
                    mark: { show: true },
                    dataView: { show: false, readOnly: false },
                    magicType: {
                        //show: true, type: ['line', 'bar', 'stack', 'tiled'], title: {
                        //    line: 'Line',
                        //    bar: 'Bar',
                        //    stack: 'Stack',
                        //    tiled: 'Tiled'
                        //},
                        show: true, type: ['line', 'bar'], title: {
                            line: 'Line',
                            bar: 'Bar',
                        },
                    },
                    restore: { show: true, title: "Restore" },
                    saveAsImage: {
                        show: true,
                        title: "Save Image"
                    }
                }
            },
            calculable: true,
            legend: {
                data: GetLegend(chartType)
            },
            xAxis: GetXAxis(chartType),
            yAxis: GetYAxis(chartType),
            series: GetSeries(chartType)
        }
        );
    }
}

function GetSeries(chartType) {
    var chartData = [];
    var name = '';
    switch (chartType) {
        case 0:
            $.each(_searchData, function (i, item) {
                chartData.push(item.LocatorPageviews);
            });
            name = 'Locator Pageviews';
            break;
        case 1:
            $.each(_leadsData, function (i, item) {
                chartData.push(item.LeadCount);
            });
            name = 'Leads';
            break;
        case 2:
            $.each(_emailData, function (i, item) {
                chartData.push(item.EmailSends);
            });
            name = 'Email Sends';
            break;
        default:
            break;
    }
    return [
        {
            name: name,
            type: 'line',
            smooth: true,
            data: chartData
        },
    ];
}

function GetLegend(chartType) {
    return [];
}

function GetXAxis(chartType) {
    return [
        {
            type: 'category',
            boundaryGap: false,
            data: GetXAxisLabelData(chartType)
        }
    ];
}

function GetYAxis(chartType) {
    return [{
        type: 'value'
    }];
}

function GetXAxisLabelData(chartType) {
    var label = [];
    switch (chartType) {
        case 0:
            $.each(_searchData, function (i, item) {
                label.push(item.CreateDate);
            });
            break;
        case 1:
            $.each(_leadsData, function (i, item) {
                label.push(item.CreateDate);
            });
            break;
        case 2:
            $.each(_emailData, function (i, item) {
                label.push(item.CreateDate);
            });
            break;
        default:
    }
    return label;
}

function MakeColumnSelection(MySelection) {
    var whichItem = $(MySelection).attr('id');

    if ($("#" + whichItem + " > span > i").hasClass('fa-check')) {
        $("#" + whichItem + " > span > i").removeClass('fa-check');
    }
    else {
        $("#" + whichItem + " > span > i").addClass('fa-check');
    }

    // ShowHideFooterRow(false);
    if (whichItem == 'aSelectAll') {
        $('a.toggle-vis').each(function (i, j) {
            var column = $('#RetailerActivityReport').DataTable().column($(this).attr('data-column'));
            if ($("#" + whichItem + " > span > i").hasClass('fa-check')) {
                column.visible(true);
                ShowHideFooterRow(true);
                $("#filterMenu a > span > i").addClass('fa-check');
            }
            else {
                column.visible(false);
                ShowHideFooterRow(false);
                $("#filterMenu a > span > i").removeClass('fa-check');
            }
        });
    }
    else {
        var columnCollectionFilter = $("#filterMenu > li > a");
        for (var i = 0; i < columnCollectionFilter.length; i++) {
            var id = $(columnCollectionFilter[i]).attr('id')
            // alert(666);
            if ($("#" + id + " > span > i").hasClass('fa-check')) {
                showHideExtraColumnInFooter(id, true);
            }
            else {
                showHideExtraColumnInFooter(id, false);
            }
        }
        AddRemoveCheckIcon_SelectAllLink();
    }

    $(".dataTables_length select").css('width', 'auto');
    $(".dataTables_filter input").css('width', 'auto');

    isShowHide = true;

    var table = $('#RetailerActivityReport').DataTable();
    table.columns.adjust().draw(false);
}

function AddRemoveCheckIcon_SelectAllLink() {
    //check if all selected then make select all checked as well
    var allCheckBoxes = $("#filterMenu a > span > i");
    var allChecked = true;
    $.each(allCheckBoxes, function (counter, item) {
        //alert($(item).attr('class'));
        var aSender = $(item).closest('a').attr('id');
        if (aSender != 'aSelectAll') {
            if (!$(item).hasClass('fa-check')) {
                allChecked = false;
            }
        }
    });
    if (allChecked) {
        $("#aSelectAll > span > i").addClass('fa-check');
    }
    else {
        //check if any one is unselected then make select all unchecked as well
        $("#aSelectAll > span > i").removeClass('fa-check');
    }
}

function ShowHideFooterRow(isShow) {
    if (isShow) {
        $("#tdLocatorPageviews").show();
        $("#tdTotalClicks").show();
        $("#tdLandingPageviews").show();
        $("#tdLeads").show();
        $("#tdEmailsSent").show();
        $("#tdDirection").show();
        $("#tdHours").show();
        //$("#tdMoreInfo").show();
        $("#tdLogo").show();
        $("#tdWebsite").show();
        $("#tdPhone").show();
    }
    else {
        $("#tdLocatorPageviews").hide();
        $("#tdTotalClicks").hide();
        $("#tdLandingPageviews").hide();
        $("#tdLeads").hide();
        $("#tdEmailsSent").hide();
        $("#tdDirection").hide();
        $("#tdHours").hide();
        //$("#tdMoreInfo").hide();
        $("#tdLogo").hide();
        $("#tdWebsite").hide();
        $("#tdPhone").hide();
    }
}

var tableColumns = [
          { data: "AccountName" },
          { data: "LocatorPageviews" },
          { data: "TotalClicks" },
          { data: "LPPageviews" },
          { data: "Contacts" },
          { data: "EmailsSent" },
          { data: "DirectionsClicks" },
          { data: "MapClicks" },
          //{ data: "MoreInfoClicks" },
          { data: "LogoClicks" },
          { data: "WebsiteClicks" },
          { data: "PhoneClicks" },
          //{ data: "FormSubmissions" },
];

function generateRetailerActivityDataTable() {
    var table = $('#RetailerActivityReport').DataTable({
        "processing": true,
        "serverSide": true,
        "bDestroy": true,
        //dom: '<l<"toolbar">f>rtip',
        oLanguage: { sProcessing: "<div id='dvloader_processing'></div>" },
        aoColumns: tableColumns,
        "ajax": {
            "type": "POST",
            "url": '/GetTableData',
            "data": function (d) {
                $.extend(d);
                d.statusfilter = $("#hdnStatusFilter").val();
                // Retrieve dynamic parameters
                var dt_params = $('#RetailerActivityReport').data('dt_params');
                // Add dynamic parameters to the data object sent to the server
                if (dt_params) { $.extend(d, dt_params); }

                if (isFooterRowRequired) {
                    addFooterRow(function (result) {
                        if (result) {
                            // ShowHideFooterRow(false);
                            var columnCollectionFilter = $("#filterMenu > li > a");
                            for (var i = 0; i < columnCollectionFilter.length; i++) {
                                var id = $(columnCollectionFilter[i]).attr('id')
                                if ($("#" + id + " > span > i").hasClass('fa-check')) {
                                    showHideExtraColumnInFooter(id, true);
                                }
                                else {
                                    showHideExtraColumnInFooter(id, false);
                                }
                            }
                        }
                    });
                    isFooterRowRequired = false;
                }
            }
        },
        "columnDefs": [
         {
             "targets": 0,
             "render": function (data, type, full, meta) {
                 return "<a href='" + _retailerUrl + "?id=" + full.AccountObjectID + "'>" + data + "</a>";
             },
         },
         {
             "targets": [2],
             "visible": false,
         },
        {
            "targets": [7],
            "visible": false,
        },
        {
            "className": "numericCol",
            "targets": [1, 2, 3, 4, 5, 6, 7, 8]
        }

        ],
        "fnServerParams": function (aoData) {
            aoData.DataTableIdentifier = _retailersIdentifer;
        },
        "fnInitComplete": function (oSettings, json) {
            totalRow = oSettings._iRecordsTotal;
            $("#tdRetailer").text("Total (" + oSettings._iRecordsTotal + " rows)");
            $('.dataTables_filter input[type=search]').wrap('<span class="deleteicon" />').after($('<span><i class="fa fa-close"></i></span>').click(function () {
                if ($('.dataTables_filter input[type=search]').val() != '') {
                    $('#' + oSettings.sTableId).dataTable().fnFilter('');
                    $(this).prev('input').val('').focus();
                }
            }));
        },
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            //console.log(nRow);
            if (!aData.IsActive) {
                $('td', nRow).css('background-color', '#d3d3d3');
            }
        },
    });

    $(".dataTables_length select").css('width', 'auto');
    $(".dataTables_filter input").css('width', 'auto');  
    //footerRow
    $("#RetailerActivityReport tfoot th").each(function (i) {
        if ($(this).text() !== '') {          
            var select = $('<select style="width:100px" class="pull-right" id="ddlStatusFilter"><option value="0">All</option><option value="1">Active</option><option value="2">Inactive</option></select>').appendTo($(this).empty())
	            .on('change', function () {
	                var val = $(this).val();
	                $("#hdnStatusFilter").val($(this).val());
	                table.column(i)
	                    .search(val ? '^' + $(this).val() + '$' : val, true, false)
	                    .draw();
	            });

            $(select).insertBefore("#RetailerActivityReport_filter");
        }
    });
}

function addFooterRow(callback) {
    $('#RetailerActivityReport').on('draw.dt', function () {
        var count = 0;
        $('tr .differ-footer-row').each(function () {
            count = count + 1;
        })
        if (count == 0) {
            var sumLocatorPageviews = 0,
                sumTotalClicks = 0,
                sumLandingPageviews = 0,
                sumLeads = 0,
                sumEmailsSent = 0,
                sumDirectionsClicks = 0,
                sumHoursClick = 0,
                sumMoreInfoClicks = 0,
                sumLogoClicks = 0,
                sumWebsiteClicks = 0,
                sumPhoneClicks = 0;

            sumLocatorPageviews = _data.LocatorPageviews;
            sumTotalClicks = _data.TotalClicks;
            sumLandingPageviews = _data.LPPageviews;
            sumLeads = _data.Contacts;
            sumEmailsSent = _data.EmailsSent;
            sumDirectionsClicks = _data.DirectionsClicks;
            sumHoursClick = _data.MapClicks;
            sumMoreInfoClicks = _data.MoreInfoClicks;
            sumLogoClicks = _data.LogoClicks;
            sumWebsiteClicks = _data.WebsiteClicks;
            sumPhoneClicks = _data.PhoneClicks;

            //var footerRow = '<tr id="trFooterRow" class="differ-footer-row"><td id=tdRetailer>Total</td><td class=numericCol id=tdLocatorPageviews>' + sumLocatorPageviews + '</td> <td class=numericCol id=tdTotalClicks>' + sumTotalClicks + '</td><td class=numericCol id=tdLandingPageviews>' + sumLandingPageviews + '</td><td class=numericCol id=tdLeads>' + sumLeads + '</td><td class=numericCol id=tdEmailsSent>' + sumEmailsSent + '</td><td class=numericCol id=tdDirection >' + sumDirectionsClicks + '</td><td class=numericCol id=tdHours >' + sumHoursClick + '</td><td class=numericCol id=tdMoreInfo >' + sumMoreInfoClicks + '</td><td class=numericCol id=tdLogo >' + sumLogoClicks + '</td><td class=numericCol id=tdWebsite >' + sumWebsiteClicks + '</td><td class=numericCol id=tdPhone >' + sumPhoneClicks + '</td></tr>';
            var footerRow = '<tr id="trFooterRow" class="differ-footer-row"><td id=tdRetailer>Total (' + totalRow + ' rows)</td><td class=numericCol id=tdLocatorPageviews>' + sumLocatorPageviews + '</td> <td class=numericCol id=tdTotalClicks>' + sumTotalClicks + '</td><td class=numericCol id=tdLandingPageviews>' + sumLandingPageviews + '</td><td class=numericCol id=tdLeads>' + sumLeads + '</td><td class=numericCol id=tdEmailsSent>' + sumEmailsSent + '</td><td class=numericCol id=tdDirection >' + sumDirectionsClicks + '</td><td class=numericCol id=tdHours >' + sumHoursClick + '</td><td class=numericCol id=tdLogo >' + sumLogoClicks + '</td><td class=numericCol id=tdWebsite >' + sumWebsiteClicks + '</td><td class=numericCol id=tdPhone >' + sumPhoneClicks + '</td></tr>';
            $('#RetailerActivityReport tr:last').after(footerRow);
            flag = 0;
            count = 0;
            return callback(true);
        }
        if (flag == 1) {
            var table = document.getElementById("RetailerActivityReport");
            var rowCount = table.rows.length;
            table.deleteRow(rowCount - 1);
            //return callback(true);
        }
        flag = 0;
    });
}

function showHideExtraColumnInFooter(whichcolumn, show) {
    var tdfRow = '';
    //alert(whichcolumn);    
    switch (whichcolumn) {
        case 'aRetailer':
            tdfRow = "tdRetailer";
            break;
        case 'aLocatorPageviews':
            tdfRow = "tdLocatorPageviews";
            break;
        case 'aTotalClicks':
            tdfRow = "tdTotalClicks";
            break;
        case 'aLandingPageviews':
            tdfRow = "tdLandingPageviews";
            break;
        case 'aLeads':
            tdfRow = "tdLeads";
            break;
        case 'aEmailsSent':
            tdfRow = "tdEmailsSent";
            break;
        case 'aDirectionClicks':
            tdfRow = "tdDirection";
            break;
        case 'aHoursClicks':
            tdfRow = "tdHours";
            break;
            //case 'aMoreInfoClicks':
            //    tdfRow = "tdMoreInfo";
            //    break;
        case 'aLogoClicks':
            tdfRow = "tdLogo";
            break;
        case 'aWebsiteClicks':
            tdfRow = "tdWebsite";
            break;
        case 'aPhoneClicks':
            tdfRow = "tdPhone";
            break;
        case 'aSelectAll':
            tdfRow = '';
            break;
        default:
            tdfRow = '';
            break;
    }
    if (show) {
        $("#" + tdfRow).show();
    }
    else {
        $("#" + tdfRow).hide();
    }
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
                _data = result.RetailersChartViewData.RetailerActivityReportViewDataMini;
                var table = $('#RetailerActivityReport').DataTable();
                table.clear();
                table.destroy();
                updateScoreBoxesandChart(result);
                ShowDefaultColumnSelected();
                //Hook to show status filter
                $("#footerRow tr th").text('Status');
                $("#hdnStatusFilter").val('0');
                generateRetailerActivityDataTable();
                //ShowHideFooterRow(false);
                //$("#aDirectionClicks > span > i").removeClass('fa-check');
                //$("#aHoursClicks > span > i").removeClass('fa-check');
                //$("#aMoreInfoClicks > span > i").removeClass('fa-check');
                //ShowDefaultColumnSelected();
                AddRemoveCheckIcon_SelectAllLink();
            }
            NProgress.done();
        },
        error: function (err) {
            NProgress.done();
        }
    });
}

function ShowDefaultColumnSelected() {
    $("#aRetailer > span > i").addClass('fa-check');
    $("#aLocatorPageviews > span > i").addClass('fa-check');
    $("#aTotalClicks > span > i").removeClass('fa-check');
    $("#aLandingPageviews > span > i").addClass('fa-check');
    $("#aLeads > span > i").addClass('fa-check');
    $("#aEmailsSent > span > i").addClass('fa-check');
    $("#aDirectionClicks > span > i").addClass('fa-check');
    $("#aHoursClicks > span > i").removeClass('fa-check');
    $("#aMoreInfoClicks > span > i").addClass('fa-check');
    $("#aLogoClicks > span > i").addClass('fa-check');
    $("#aWebsiteClicks > span > i").addClass('fa-check');
    $("#aPhoneClicks > span > i").addClass('fa-check');
}

function updateScoreBoxesandChart(result) {
    $("#dvTotalPageviewsLocator").text(result.RetailersChartViewData.TotalPageviewsLocator);
    $("#spTopAccountPageviewsLocator").text(result.RetailersChartViewData.TopAccountPageviewsLocator);
    $("#dvTotalPageviewsLP").text(result.RetailersChartViewData.TotalPageviewsLP);
    $("#spTopAccountPageviewsLP").text(result.RetailersChartViewData.TopAccountPageviewsLP);
    $("#dvTotalLeads").text(result.RetailersChartViewData.TotalLeads);
    $("#spAverageConversionRate").text((result.RetailersChartViewData.AverageConversionRate).toFixed(2) + "%");
    _searchData = result.RetailersChartViewData.SearchData;
    _leadsData = result.RetailersChartViewData.LeadsData;
    _emailData = result.RetailersChartViewData.EmailData;
    CreateAllCharts();
}

var theme = {
    color: [
        '#26B99A', '#34495E', '#A23043', '#3498DB',
        '#9B59B6', '#8abb6f', '#759c6a', '#bfd3b7'
    ],

    title: {
        itemGap: 8,
        textStyle: {
            fontWeight: 'normal',
            color: '#408829'
        }
    },

    dataRange: {
        color: ['#1f610a', '#97b58d']
    },

    toolbox: {
        color: ['#408829', '#408829', '#408829', '#408829']
    },

    tooltip: {
        backgroundColor: 'rgba(0,0,0,0.5)',
        axisPointer: {
            type: 'line',
            lineStyle: {
                color: '#408829',
                type: 'dashed'
            },
            crossStyle: {
                color: '#408829'
            },
            shadowStyle: {
                color: 'rgba(200,200,200,0.3)'
            }
        }
    },

    dataZoom: {
        dataBackgroundColor: '#eee',
        fillerColor: 'rgba(64,136,41,0.2)',
        handleColor: '#408829'
    },
    grid: {
        borderWidth: 0
    },

    categoryAxis: {
        axisLine: {
            lineStyle: {
                color: '#408829'
            }
        },
        splitLine: {
            lineStyle: {
                color: ['#eee']
            }
        }
    },

    valueAxis: {
        axisLine: {
            lineStyle: {
                color: '#408829'
            }
        },
        splitArea: {
            show: true,
            areaStyle: {
                color: ['rgba(250,250,250,0.1)', 'rgba(200,200,200,0.1)']
            }
        },
        splitLine: {
            lineStyle: {
                color: ['#eee']
            }
        }
    },
    timeline: {
        lineStyle: {
            color: '#408829'
        },
        controlStyle: {
            normal: { color: '#408829' },
            emphasis: { color: '#408829' }
        }
    },

    k: {
        itemStyle: {
            normal: {
                color: '#68a54a',
                color0: '#a9cba2',
                lineStyle: {
                    width: 1,
                    color: '#408829',
                    color0: '#86b379'
                }
            }
        }
    },
    map: {
        itemStyle: {
            normal: {
                areaStyle: {
                    color: '#ddd'
                },
                label: {
                    textStyle: {
                        color: '#c12e34'
                    }
                }
            },
            emphasis: {
                areaStyle: {
                    color: '#99d2dd'
                },
                label: {
                    textStyle: {
                        color: '#c12e34'
                    }
                }
            }
        }
    },
    force: {
        itemStyle: {
            normal: {
                linkStyle: {
                    strokeColor: '#408829'
                }
            }
        }
    },
    chord: {
        padding: 4,
        itemStyle: {
            normal: {
                lineStyle: {
                    width: 1,
                    color: 'rgba(128, 128, 128, 0.5)'
                },
                chordStyle: {
                    lineStyle: {
                        width: 1,
                        color: 'rgba(128, 128, 128, 0.5)'
                    }
                }
            },
            emphasis: {
                lineStyle: {
                    width: 1,
                    color: 'rgba(128, 128, 128, 0.5)'
                },
                chordStyle: {
                    lineStyle: {
                        width: 1,
                        color: 'rgba(128, 128, 128, 0.5)'
                    }
                }
            }
        }
    },
    gauge: {
        startAngle: 225,
        endAngle: -45,
        axisLine: {
            show: true,
            lineStyle: {
                color: [[0.2, '#86b379'], [0.8, '#68a54a'], [1, '#408829']],
                width: 8
            }
        },
        axisTick: {
            splitNumber: 10,
            length: 12,
            lineStyle: {
                color: 'auto'
            }
        },
        axisLabel: {
            textStyle: {
                color: 'auto'
            }
        },
        splitLine: {
            length: 18,
            lineStyle: {
                color: 'auto'
            }
        },
        pointer: {
            length: '90%',
            color: 'auto'
        },
        title: {
            textStyle: {
                color: '#333'
            }
        },
        detail: {
            textStyle: {
                color: 'auto'
            }
        }
    },
    textStyle: {
        fontFamily: 'Arial, Verdana, sans-serif'
    }
};