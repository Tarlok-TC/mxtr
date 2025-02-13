$(document).ready(function () {
    GetGlobalAccountId_Workspace();
    $("#spnAccountCount").text(GetSelectAccountIdText(GetWorkspaceFilterIds()));
    window.updateLeads = function () {
        $('#shawLeads').fadeOut(500, function () {
            var data =
            {
                'StartDate': _filterGraph.startdate,
                'EndDate': _filterGraph.enddate,
                'AccountObjectIDs': _filterGraph.accountIDs,
                'IsAjax': true
            };
        });
    };
    createChart();
    generateLeadsDataTable();
});

function getData() {
    //var data = _leads;
    var data = _leadsChartData;
    //format date
    for (var dataCounter = 0; dataCounter < data.length; dataCounter++) {
        data[dataCounter].CreateDate = formatDate(data[dataCounter].CreateDate);
    }

    //get data with count of leads
    var dataUnique = _.uniq(data, "CreateDate");
    var temp_obj = { "date": '', "count": '' };
    var finalData = [];
    for (var counter = 0; counter < dataUnique.length; counter++) {
        var count = _.where(data, { CreateDate: dataUnique[counter].CreateDate }).length;
        temp_obj = { "date": dataUnique[counter].CreateDate, "count": count };
        finalData.push(temp_obj);
    }
    return finalData;
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

function formatDate_MM_dd_yyyy(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [month, day, year].join('-');
}

function formatJSONDate(jsonDate) {
    var dateString = jsonDate.substr(6); //"\/Date(1334514600000)\/".substr(6);
    var currentTime = new Date(parseInt(dateString));
    var month = currentTime.getMonth() + 1;
    var day = currentTime.getDate();
    var year = currentTime.getFullYear();
    if (month < 10) {
        month = '0' + month;
    }
    if (day < 10) {
        day = '0' + day
    };
    var date = year + "-" + month + "-" + day;
    return date;
}

function createChart() {
    if ($('#Shawleads_Chart').length) {
        var echart = echarts.init(document.getElementById('Shawleads_Chart'), GetTheme());
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
                data: GetLegend()
            },
            xAxis: GetXAxis(),
            yAxis: GetYAxis(),
            series: GetSeries()
        }
        );
    }
}

function GetSeries() {
    var data = getData();
    var leadsCount = [];
    $.each(data, function (i, item) {
        leadsCount.push(data[i].count);
    });
    return [
                {
                    name: 'Lead',
                    type: 'line',
                    smooth: true,
                    data: leadsCount
                },
    ];
}

function GetLegend(chartType) {
    return [];
}

function GetXAxis() {
    return [{
        type: 'category',
        boundaryGap: false,
        data: GetXAxisLabelData()
    }];
}

function GetXAxisLabelData() {
    var label = [];
    var data = getData();
    $.each(data, function (i, item) {
        label.push(data[i].date);
    });
    return label;
}

function GetYAxis() {
    return [{
        type: 'value'
    }];
}

function GetTheme() {
    return {
        color: [
            '#26B99A', '#34495E', '#BDC3C7', '#3498DB',
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
}


function updatePageFromWorkspace() {
    NProgress.start();
    SetGlobalAccountId_Workspace();
    _filterGraph.accountIDs = GetWorkspaceFilterIds();
    $("#spnAccountCount").text(GetSelectAccountIdText(actIds));
    //alert(_filterGraph.accountIDs);
    var data = {
        'StartDate': _filterGraph.startdate,
        'EndDate': _filterGraph.enddate,
        'IsAjax': true,
        'AccountObjectID': _filterGraph.accountIDs
    };

    $.ajax({
        url: _updateDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                var ajaxData = result.LeadsChartViewData;
                for (var counter = 0; counter < ajaxData.length; counter++) {
                    //Format date /Date(1224043200000)/
                    //ajaxData[counter].CreateDate = new Date(parseInt(ajaxData[counter].CreateDate.substr(6)));
                    ajaxData[counter].CreateDate = formatJSONDate(ajaxData[counter].CreateDate);
                }
                $("#spnStartDate").text(formatDate(_filterGraph.startdate));
                $("#spnEndDate").text(formatDate(_filterGraph.enddate));
                _leadsChartData = ajaxData;
                var table = $('#shawLeads').DataTable();
                table.clear();
                table.destroy();
                generateLeadsDataTable();
                createChart();
            }
            NProgress.done();
        },
        error: function (fail) {
            NProgress.done();
        }
    });
}

function generateLeadsDataTable() {
    //console.log("=======>", _leads);   

    var tableColumns = [
            { data: "FirstName" },
            { data: "LastName" },
            { data: "EmailAddress" },
           // { data: "CompanyName" },
            { data: "LeadParentAccount" },
            { data: "CampaignName" },
            { data: "LeadScore" },
            //{ data: "EventsCount" },
            { data: "CreateDate" },
            { data: "EventLastTouch" },
            { data: "ObjectID" }
    ];

    $('#shawLeads').DataTable({
        "processing": true,
        "serverSide": true,
        oLanguage: { sProcessing: "<div id='dvloader_processing'></div>" },
        aoColumns: tableColumns,
        //ajax: ("/GetTableData"),
        "ajax": {
            "type": "POST",
            //"url": '/GetTableData',
            "url": '/GetShawDealerLeadTableData',
        },
        "columnDefs": [
            {
                "targets": 8,
                "render": function (data, type, full, meta) {
                    return "<a href='" + _leadUrl + "?id=" + data + "'>View</a>";
                }
            },
             {
                 "targets": 6,
                 "render": function (data, type, full, meta) {
                     return formatDate(formatJSONDate(data));
                 }
             },
             { "className": "numericCol", "targets": [4, 5] }
        ],
        "fnServerParams": function (aoData) {
           // aoData.DataTableIdentifier = _shawleadsIdentifer;
        },
        "fnInitComplete": function (oSettings, json) {
            $('.dataTables_filter input[type=search]').wrap('<span class="deleteicon" />').after($('<span><i class="fa fa-close"></i></span>').click(function () {
                if ($('.dataTables_filter input[type=search]').val() != '') {
                    $('#' + oSettings.sTableId).dataTable().fnFilter('');
                    $(this).prev('input').val('').focus();
                }
            }));
        }
    });

    $(".dataTables_length select").css('width', 'auto');
    $(".dataTables_filter input").css('width', 'auto');
}

