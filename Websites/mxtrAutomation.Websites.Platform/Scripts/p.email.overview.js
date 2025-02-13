var selectedChart = '';
var totalRow = 0;

$(document).ready(function () {
    GetGlobalAccountId_Workspace();
    $("#spnAccountCount").text(GetSelectAccountIdText(GetWorkspaceFilterIds()));
    generateEmailActivityDataTable();
    pageLoadSettings();

    $('#tblEmailActivity').on('draw.dt', function () {
        var count = 0;
        $('tr .differ-footer-row').each(function () {
            count = count + 1;
        })
        if (count == 0) {

            var sumSends = 0, sumOpens = 0, sumOpenRate = 0, sumClicks = 0, sumClickRate = 0;
            sumSends = _emailsActivity.Sends;
            sumOpens = _emailsActivity.Opens;
            sumOpenRate = _emailsActivity.OpenRate;
            sumClicks = _emailsActivity.Clicks;
            sumClickRate = _emailsActivity.ClickRate;

            var footerRow = '<tr class="differ-footer-row"><td id=tdTotal>Total  (' + totalRow + ' rows)</td><td>' + sumSends + '</td> <td>' + sumOpens + '</td><td >' + sumOpenRate.toFixed(2) + '%</td><td >' + sumClicks + '</td><td>' + sumClickRate.toFixed(2) + '%</td></tr>';
            var rows = $('#tblEmailActivity tbody tr.differ-footer-row').length;
            $('#tblEmailActivity tr:last').after(footerRow);
            flag = 0;
            count = 0;
            var rows = $('#tblEmailActivity tbody tr.differ-footer-row').length;
        }
        if (flag == 1) {
            var table = document.getElementById("tblEmailActivity");
            var rowCount = table.rows.length;
            table.deleteRow(rowCount - 1);
        }
        flag = 0;
    });

    $('#tblEmailActivity').on('order.dt', function (e, settings, len) {
        order = 1;
    });

    var tableSearch = $('#tblEmailActivity').DataTable();
    tableSearch.on('search.dt', function () {
        flag = 1;
    });
});


var ChartTypeEnum = {
    Default: 0,
    EmailsSent: 1,
    OpenRate: 2,
    ClickRate: 3,
};

function pageLoadSettings() {
    buildChart(ChartTypeEnum.Default);
    selectedChart = ChartTypeEnum.Default;
}

function whichChart(chartType) {
    buildChart(chartType);
    $("#dvEmailsSentView").removeClass('green');
    $("#dvOpenRateView").removeClass('green');
    $("#dvClickRateView").removeClass('green');
    switch (chartType) {
        case 0:
            //default view
            break;
        case 1:
            $("#dvEmailsSentView").addClass('green');
            selectedChart = ChartTypeEnum.EmailsSent;
            break;
        case 2:
            $("#dvOpenRateView").addClass('green');
            selectedChart = ChartTypeEnum.OpenRate;
            break;
        case 3:
            $("#dvClickRateView").addClass('green');
            selectedChart = ChartTypeEnum.ClickRate;
            break;
        default:
            break;
    }
}

function generateEmailActivityDataTable() {

    var tableColumns = [
            { data: "AccountName" },
            { data: "Sends" },
            { data: "Opens" },
            { data: "OpenRate" },
            { data: "Clicks" },
            { data: "ClickRate" }
    ];

    $('#tblEmailActivity').DataTable({
        "processing": true,
        "serverSide": true,
        oLanguage: { sProcessing: "<div id='dvloader_processing'></div>" },
        aoColumns: tableColumns,
        //ajax: ("/GetTableData"),
        "ajax": {
            "type": "POST",
            "url": '/GetTableData',
        },
        //columns: tableColumns,
        "columnDefs":
         [
            {
                "targets": 3,
                "render": function (data, type, full, meta) {
                    return data.toFixed(2) + "%";
                }
            },
            {
                "targets": 5,
                "render": function (data, type, full, meta) {
                    return data.toFixed(2) + "%";
                }
            }
         ],
        "fnServerParams": function (aoData) {
            aoData.DataTableIdentifier = _emailsDataIdentifer;
        },
        "fnInitComplete": function (oSettings, json) {
            totalRow = oSettings._iRecordsTotal;
            $("#tdTotal").text("Total (" + oSettings._iRecordsTotal + " rows)");
            $('.dataTables_filter input[type=search]').wrap('<span class="deleteicon" />').after($('<span><i class="fa fa-close"></i></span>').click(function () {
                if ($('.dataTables_filter input[type=search]').val() != '') {
                    $('#' + oSettings.sTableId).dataTable().fnFilter('');
                    $(this).prev('input').val('').focus();
                }
            }));
            //var btnClear = $('<button class="btnClearDataTableFilter btn default margin-left5 margin-btm0">CLEAR</button>');
            //btnClear.appendTo($('#' + oSettings.sTableId).parents('.dataTables_wrapper').find('.dataTables_filter'));
            //$('#' + oSettings.sTableId + '_wrapper .btnClearDataTableFilter').click(function () {
            //    if ($('.dataTables_filter input[type=search]').val() != '') {
            //        $('#' + oSettings.sTableId).dataTable().fnFilter('');
            //        ////--table.search('').draw(); 
            //    }
            //});
        }
    });

    $(".dataTables_length select").css('width', 'auto');
    $(".dataTables_filter input").css('width', 'auto');
}

function buildChart(chartType) {
    if ($('#emails_Chart').length) {
        var echart = echarts.init(document.getElementById('emails_Chart'), theme);
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
    var dataSends = [];
    var dataOpens = [];
    var dataClicks = [];
    var dataClickRate = [];
    var dataOpenRate = [];

    $.each(_emailsJobStats, function (i, item) {
        dataSends.push(item.Sends);
        dataOpens.push(item.Opens);
        dataClicks.push(item.Clicks);
        //alert(item.OpenRate);
        //var openRate = 0;
        //if (parseInt(item.Sends) != 0) {
        //    openRate = item.Opens / item.Sends;
        //}
        //dataOpenRate.push(openRate.toFixed(2));
        dataOpenRate.push((item.OpenRate / 100).toFixed(2));
        //var clickRate = 0;
        //if (parseInt(item.Opens) != 0) {
        //    clickRate = item.Clicks / item.Opens;
        //}
        //dataClickRate.push(clickRate.toFixed(2));
        dataClickRate.push((item.ClickRate / 100).toFixed(2));
    });
    switch (chartType) {
        case 0:
            return [
            {
                name: 'Sends',
                type: 'bar',
                stack: 'Total',
                data: dataSends
            },
            {
                name: 'Opens',
                type: 'bar',
                stack: 'Total',
                data: dataOpens
            },
             {
                 name: 'Clicks',
                 type: 'bar',
                 stack: 'Total',
                 data: dataClicks
             }
            ]
            break;
        case 1:
            return [
                {
                    name: 'Sent',
                    type: 'line',
                    smooth: true,
                    data: dataSends
                },
            ];
            break;
        case 2:
            return [
            {
                name: 'Sends',
                type: 'bar',
                stack: 'Total',
                data: dataSends
            },
            {
                name: 'Opens',
                type: 'bar',
                stack: 'Total',
                data: dataOpens
            },
            {
                name: 'Open rate',
                type: 'line',
                yAxisIndex: 1,
                data: dataOpenRate
            }
            ]
            break;
        case 3:
            return [
            {
                name: 'Sends',
                type: 'bar',
                stack: 'Total',
                data: dataSends
            },
            {
                name: 'Opens',
                type: 'bar',
                stack: 'Total',
                data: dataOpens
            },
            {
                name: 'Clicks',
                type: 'bar',
                stack: 'Total',
                data: dataClicks
            },
            {
                name: 'Click rate',
                type: 'line',
                yAxisIndex: 1,
                data: dataClickRate
            }
            ]
            break;
        default:
            break;
    }
}

function GetLegend(chartType) {
    switch (chartType) {
        case 0:
            return ['Sends', 'Opens', 'Clicks'];
            break;
        case 1:
            return [];
            break;
        case 2:
            return ['Sends', 'Opens', 'Open Rate'];
            break;
        case 3:
            return ['Sends', 'Opens', 'Clicks', 'Click Rate'];
            break;
        default:
            break;
    }
}

function GetXAxis(chartType) {
    switch (chartType) {
        case 0:
            return [
           {
               type: 'category',
               data: GetXAxisLabelData()
           }
            ];
            break;
        case 1:
            return [{
                type: 'category',
                boundaryGap: false,
                data: GetXAxisLabelData()
            }];
            break;
        case 2:
            return [
            {
                type: '',
                data: GetXAxisLabelData()
            }
            ];
            break;
        case 3:
            return [
           {
               type: '',
               data: GetXAxisLabelData()
           }
            ];
            break;
        default:
            break;
    }
}

function GetYAxis(chartType) {
    switch (chartType) {
        case 0:
            return [
            {
                type: 'value',
                name: '',
                axisLabel: {
                    formatter: '{value}'
                }
            },
            ];
            break;
        case 1:
            return [{
                type: 'value'
            }];
            break;
        case 2:
            return [
             {
                 type: 'value',
                 name: '',
                 axisLabel: {
                     formatter: '{value}'
                 }
             },
             {
                 type: 'value',
                 name: '',
                 axisLabel: {
                     formatter: '{value}'
                 }
             }
            ];
            break;
        case 3:
            return [
            {
                type: 'value',
                name: '',
                axisLabel: {
                    formatter: '{value}'
                }
            },
            {
                type: 'value',
                name: '',
                axisLabel: {
                    formatter: '{value}'
                }
            }
            ];
            break;
        default:
            break;
    }
}

function GetXAxisLabelData() {
    var label = [];
    $.each(_emailsJobStats, function (i, item) {
        label.push(item.DataDate);
    });
    return label;
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
    // console.log("==========>", data);
    $.ajax({
        url: _updateDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            // console.log("=====================>", result);
            if (result.Success) {
                _emailsJobStats = result.EmailChartViewData.EmailJobStatsViewData;
                //console.log("_emailsJobStats", _emailsJobStats);

                _emailsActivity = result.EmailChartViewData.EmailActivityViewDataMini;
                // console.log("_emailsActivity", _emailsActivity);
                var table = $('#tblEmailActivity').DataTable();
                table.clear();
                table.destroy();
                generateEmailActivityDataTable();
                buildChart(selectedChart);
                $("#dvEmailsSentView").text(result.EmailChartViewData.TotalEmailSends);
                $("#dvOpenRateView").text(result.EmailChartViewData.OverallOpenRate.toFixed(2) + "%");
                $("#dvClickRateView").text(result.EmailChartViewData.OverallClickRate.toFixed(2) + "%");
            }
            NProgress.done();
        },
        error: function (fail) {
            NProgress.done();
        }
    });
}

var theme = {
    color: [
        '#26B99A', '#34495E', '#66FF00', '#3498DB',
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