$(document).ready(function () {
    generateDealerPerformanceDataTable();
    InitializePieChart();
    GetGlobalAccountId_Workspace();
    $("#spnAccountCount").text(GetSelectAccountIdText(GetWorkspaceFilterIds()));
});
var totalRow = 0;

function InitializePieChart() {
    $('.chart').easyPieChart({
        // The color of the curcular bar. You can pass either a css valid color string like rgb, rgba hex or string colors. But you can also pass a function that accepts the current percentage as a value to return a dynamically generated color.
        barColor: '#1ABC9C',
        // The color of the track for the bar, false to disable rendering.
        trackColor: '#F0F3F3',
        // The color of the scale lines, false to disable rendering.
        scaleColor: false,
        // Defines how the ending of the bar line looks like. Possible values are: butt, round and square.
        lineCap: 'butt',
        // Width of the bar line in px.
        lineWidth: 20,
        // Size of the pie chart in px. It will always be a square.
        size: 110,
        // Time in milliseconds for a eased animation of the bar growing, or false to deactivate.
        animate: 1000,
    });
}

function generateDealerPerformanceDataTable() {
    var tableColumns = [
            { data: "AccountName" },
            { data: "LeadsCount" },
            { data: "ColdLeadsCount" },
            { data: "WarmLeadsCount" },
            { data: "HotLeadsCount" },
            { data: "HandedOffLeads" },
            { data: "PassOff" },
            { data: "AverageTimeInFunnel" }
    ];

    $('#DealerPerformanceReport').DataTable({
        "processing": true,
        "serverSide": true,
        oLanguage: { sProcessing: "<div id='dvloader_processing'></div>" },
        aoColumns: tableColumns,
        "ajax": {
            "type": "POST",
            "url": '/GetTableData',
        },
        "columnDefs": [
            {
                "targets": 0,
                "render": function (data, type, full, meta) {
                    return "<a href='" + _dealerPerformanceDetailUrl + "?id=" + full.AccountObjectID + "'>" + full.AccountName + "</a>"
                }
            },
             { "className": "numericCol", "targets": [1, 2, 3] }
        ],
        "fnServerParams": function (aoData) {
            aoData.DataTableIdentifier = _dealerPerformanceIdentifer;
        },
        "fnInitComplete": function (oSettings, json) {
            totalRow = oSettings._iRecordsTotal;
            $("#tdDealerPerformance").text("Total (" + oSettings._iRecordsTotal + " rows)");
            $('.dataTables_filter input[type=search]').wrap('<span class="deleteicon" />').after($('<span><i class="fa fa-close"></i></span>').click(function () {
                if ($('.dataTables_filter input[type=search]').val() != '') {
                    $('#' + oSettings.sTableId).dataTable().fnFilter('');
                    $(this).prev('input').val('').focus();
                }
            }));
            var select = $('<select style="width:100px" class="pull-right" id="ddlLeadsFilter"><option value="1">Shaw Leads</option><option value="2">Total Leads</option></select>').on('change', function () {
                var val = $(this).val();
                //alert(val);
                //table.column(i).search(val ? '^' + $(this).val() + '$' : val, true, false).draw();
                table.search(val ? '^' + $(this).val() + '$' : val, true, false).draw();
            });
            $(select).insertBefore("#DealerPerformanceReport_filter");
        },
    });

    $(".dataTables_length select").css('width', 'auto');
    $(".dataTables_filter input").css('width', 'auto');

    //footerRow
    //$("#DealerPerformanceReport tfoot th").each(function (i) {
    //    if ($(this).text() !== '') {
    //        var select = $('<select style="width:100px" class="pull-right" id="ddlStatusFilter"><option value="0">All</option><option value="1">Active</option><option value="2">Inactive</option></select>').appendTo($(this).empty())
    //            .on('change', function () {
    //                //var val = $(this).val();
    //                //$("#hdnStatusFilter").val($(this).val());
    //                //table.column(i)
    //                //    .search(val ? '^' + $(this).val() + '$' : val, true, false)
    //                //    .draw();
    //            });

    //        $(select).insertBefore("#DealerPerformanceReport_filter");
    //    }
    //});
}

function updatePageFromWorkspace() {
    NProgress.start();
    SetGlobalAccountId_Workspace();
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
                $("#leadsCountInDealerFunnel").text(result.LeadsCountInDealerFunnel);
                $("#spAverageLeadTimeDealer").text(result.AverageLeadTimeDealer);
                $("#conversionRateDealer").text(result.ConversionRateDealer + "%");
                $("#spConversionRateDealerDataPercentage").attr('data-percent', result.ConversionRateDealer);
                $("#spConversionRateDealerPercentage").text(result.ConversionRateDealer);
                InitializePieChart();
                var table = $('#DealerPerformanceReport').DataTable();
                table.clear();
                table.destroy();
                generateDealerPerformanceDataTable();
                $("#ftTotalLeads").text(result.FooterData.TotalLeads);
                $("#ftTotalColdLead").text(result.FooterData.TotalColdLead);
                $("#ftTotalWarmLead").text(result.FooterData.TotalWarmLead);
                $("#ftTotalHotLead").text(result.FooterData.TotalHotLead);
                $("#ftHandedOffLeads").text(result.FooterData.TotalHotLead);
                $("#ftTotalPassOf").text(result.FooterData.TotalPassOf);
                $("#ftTotalAvgTimeFunnel").text(result.FooterData.TotalAvgTimeFunnel);
            }
            NProgress.done();
        },
        error: function (err) {
            NProgress.done();
        }
    });
}