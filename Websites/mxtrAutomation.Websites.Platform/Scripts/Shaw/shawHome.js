_dates = [];
_cold = [];
_warm = [];
_hot = [];

var _datesOriginal, _coldOriginal, _warmOriginal, _hotOriginal = [];

var _participatingDealerDates, _participatingDealerCount = [];

var selectedChart = '';
var ChartTypeEnum = {
    Daily: 0,
    Weekly: 1,
    Monthly: 2,
};

$(document).ready(function () {
    //GetGlobalAccountId_Workspace();
    //$("#spnAccountCount").text(GetSelectAccountIdText(GetWorkspaceFilterIds()));
    selectedChart = ChartTypeEnum.Daily;
    GetLeadChartData(false);
    SetDealerDataOnMap(false);
    InitializePieChart();
    MakeLeadAverageGaugeChart(_averageLeadScore);
});

function whichChart(chartType) {
    $("#btnDailyChart").removeClass('green');
    $("#btnWeeklyChart").removeClass('green');
    $("#btnMonthlyChart").removeClass('green');
    switch (chartType) {
        case 0:
            $("#btnDailyChart").addClass('green');
            selectedChart = ChartTypeEnum.Daily;
            _dates = _datesOriginal;
            _cold = _coldOriginal;
            _warm = _warmOriginal;
            _hot = _hotOriginal;
            break;
        case 1:
            $("#btnWeeklyChart").addClass('green');
            selectedChart = ChartTypeEnum.Weekly;
            var totalWeekCount = 0;
            _dates = [], _cold = [], _warm = [], _hot = [];
            for (var i = 0; i < _datesOriginal.length; i++) {
                var isWeek = parseInt(i + 1) % 7;
                if (isWeek == 0) {
                    var weekCount = parseInt(i + 1) / 7;
                    //_dates[parseInt(weekCount) - 1] = "Week " + weekCount; //_datesOriginal[i];
                    _dates[parseInt(weekCount) - 1] = _datesOriginal[parseInt(i - 6)] + " to " + _datesOriginal[i];
                    _cold[parseInt(weekCount) - 1] = _coldOriginal[i];
                    _warm[parseInt(weekCount) - 1] = _warmOriginal[i];
                    _hot[parseInt(weekCount) - 1] = _hotOriginal[i];
                    totalWeekCount = weekCount;
                }
            }
            var remaingDays = parseInt(_datesOriginal.length) % 7;
            if (parseInt(remaingDays) > 0) {
                //_dates[parseInt(totalWeekCount)] = "Week " + parseInt(totalWeekCount + 1);
                _dates[parseInt(totalWeekCount)] = _datesOriginal[parseInt(i - remaingDays)] + " to " + _datesOriginal[parseInt(_datesOriginal.length - 1)];
                _cold[parseInt(totalWeekCount)] = _coldOriginal[_coldOriginal.length - 1];
                _warm[parseInt(totalWeekCount)] = _warmOriginal[_warmOriginal.length - 1];
                _hot[parseInt(totalWeekCount)] = _hotOriginal[_hotOriginal.length - 1];
            }
            /*
            var totalWeekCount = 0;
            var coldData = 0, warmData = 0, hotData = 0;
            _dates = [], _cold = [], _warm = [], _hot = [];
            for (var i = 0; i < _datesOriginal.length; i++) {
                var isWeek = parseInt(i + 1) % 7;
                coldData = coldData + parseInt(_coldOriginal[i]);
                warmData = warmData + parseInt(_warmOriginal[i]);
                hotData = hotData + parseInt(_hotOriginal[i]);
                if (isWeek == 0) {
                    var weekCount = parseInt(i + 1) / 7;
                    _dates[parseInt(weekCount) - 1] = "Week " + weekCount; //_datesOriginal[i];
                    _cold[parseInt(weekCount) - 1] = coldData;
                    _warm[parseInt(weekCount) - 1] = warmData;
                    _hot[parseInt(weekCount) - 1] = hotData;
                    coldData = 0;
                    warmData = 0;
                    hotData = 0;
                    totalWeekCount = weekCount;
                }
            }

            var remaingDays = parseInt(_datesOriginal.length) % 7;
            if (parseInt(remaingDays) > 0) {
            var starIndex = totalWeekCount * 7;
            coldData = 0, warmData = 0, hotData = 0;
            for (var i = 0; i < remaingDays; i++) {
                coldData = coldData + parseInt(_coldOriginal[i + starIndex]);
                warmData = warmData + parseInt(_warmOriginal[i + starIndex]);
                hotData = hotData + parseInt(_hotOriginal[i + starIndex]);
            }
            _dates[parseInt(totalWeekCount)] = "Week " + parseInt(totalWeekCount + 1);
            _cold[parseInt(totalWeekCount)] = coldData;
            _warm[parseInt(totalWeekCount)] = warmData;
            _hot[parseInt(totalWeekCount)] = hotData;
            }
            */
            break;
        case 2:
            $("#btnMonthlyChart").addClass('green');
            selectedChart = ChartTypeEnum.Monthly;
            _dates = [], _cold = [], _warm = [], _hot = [];
            var currentMonth = "";
            var currentCount = -1;
            var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            var isFirefox = typeof InstallTrigger !== 'undefined'
            for (var i = 0; i < _datesOriginal.length; i++) {
                var d = new Date(_datesOriginal[i]);
                var month = monthNames[d.getMonth()];
                var dateData = _datesOriginal[i].split('-');
                //if (typeof month === 'undefined') {
                if (isFirefox) {
                    //MM-DD-YYYY
                    //d = new Date(dateData[2], dateData[0], dateData[1]);
                    //month = monthNames[d.getMonth() - 1];
                    month = monthNames[dateData[0] - 1];
                }

                if (i == 0) {
                    if (isFirefox) {
                        //currentMonth = monthNames[d.getMonth() - 1];
                        currentMonth = monthNames[dateData[0] - 1];
                    }
                    else {
                        currentMonth = monthNames[d.getMonth()];
                    }
                }

                if (i > 0 && month != currentMonth) {
                    currentCount = currentCount + 1;
                    _dates[currentCount] = currentMonth;
                    _cold[currentCount] = _coldOriginal[i - 1];
                    _warm[currentCount] = _warmOriginal[i - 1];
                    _hot[currentCount] = _hotOriginal[i - 1];
                    currentMonth = month;
                }

                if (i == _datesOriginal.length - 1) {
                    _dates[currentCount + 1] = currentMonth;
                    _cold[currentCount + 1] = _coldOriginal[i];
                    _warm[currentCount + 1] = _warmOriginal[i];
                    _hot[currentCount + 1] = _hotOriginal[i];
                }
            }
            break;
        default:
            break;
    }
    buildChart(chartType);
}

function MakeLeadAverageGaugeChart(leadAverage) {
    var opts = {
        lines: 12,
        angle: 0.0,
        lineWidth: 0.44,
        pointer: {
            length: 0.7,
            strokeWidth: 0.035,
            color: '#000000'
        },
        limitMax: 'false',
        colorStart: '#1ABC9C',
        colorStop: '#1ABC9C',
        strokeColor: '#F0F3F3',
        generateGradient: true
    };
    var target = document.getElementById('chart_gauge');
    var gauge = new Gauge(target).setOptions(opts);
    gauge.maxValue = 120;
    gauge.minValue = 0;//try commenting this out
    gauge.animationSpeed = 32;
    if (leadAverage == 0) {
        leadAverage = 0.1;
    }
    var avValue = (leadAverage / gauge.maxValue) * 100;
    gauge.set(avValue);
}

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

function GetLeadChartData(isSearchCall) {
    //SetGlobalAccountId_Workspace();
    //_filterGraph.accountIDs = GetWorkspaceFilterIds();
    var data = {
        'StartDate': _filterGraph.startdate,
        'EndDate': _filterGraph.enddate,
        //'AccountObjectIDs': _filterGraph.accountIDs,
        'IsSearchCall': isSearchCall,
    };

    $.ajax({
        url: _getLeadChartDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                _dates = result.Dates;
                _cold = result.Cold;
                _warm = result.Warm;
                _hot = result.Hot;
                _datesOriginal = result.Dates;
                _coldOriginal = result.Cold;
                _warmOriginal = result.Warm;
                _hotOriginal = result.Hot;
                whichChart(selectedChart);
                init_chart_doughnut(_coldOriginal[_coldOriginal.length - 1], _warmOriginal[_warmOriginal.length - 1], _hotOriginal[_hotOriginal.length - 1]);
            }
            else {
                alert('Error in getting lead chart details.')
            }
        }
    });
}

function buildChart(chartType) {
    if ($('#shawLead_Chart').length) {
        var echart = echarts.init(document.getElementById('shawLead_Chart'), theme);
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
                        //show: true, type: ['line', 'bar', 'stack'], title: {
                        //    line: 'Line',
                        //    bar: 'Bar',
                        //    stack: 'Stack',
                        //},
                        show: false, type: ['line', 'bar'], title: {
                            line: 'Line',
                            bar: 'Bar',
                            //stack: 'Stack',
                        },
                    },
                    restore: { show: false, title: "Restore" },
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
            color: GetChartColorTheme(),
            series: GetSeries(chartType)
        }
        );
    }
}

function GetChartColorTheme() {
    //return ['red', 'green', 'yellow'];
    return ['#40a6b1', '#fc9e00', '#e04b35'];
}

function GetSeries(chartType) {
    //var dataCold = _cold; //[5, 10, 25, 24, 25, 78, 45];
    //var dataWarm = _warm; //[15, 100, 205, 240, 25, 78, 45];
    //var dataHot = _hot;//[115, 10, 25, 20, 25, 78, 45];

    //var dataCold = [5, 10, 25, 24, 25, 78, 45];
    //var dataWarm =[15, 100, 205, 240, 25, 78, 45];
    //var dataHot = [115, 10, 25, 20, 25, 78, 45];

    switch (chartType) {
        case 0:
        case 1:
        case 2:
            return getChartData(_cold, _warm, _hot);
            break;
        default:
            break;
    }
}

function getChartData(cold, warm, hot) {
    return [
          {
              name: 'Cold',
              type: 'bar',
              //type: 'stack',
              stack: 'Total',
              data: cold
          },
          {
              name: 'Warm',
              type: 'bar',
              //type: 'stack',
              stack: 'Total',
              data: warm
          },
           {
               name: 'Hot',
               type: 'bar',
               //type: 'stack',
               stack: 'Total',
               data: hot
           }
    ];
}

function GetLegend(chartType) {
    switch (chartType) {
        case 0:
        case 1:
        case 2:
            return ['Cold', 'Warm', 'Hot'];
            break;
        default:
            break;
    }
}

function GetXAxis(chartType) {
    switch (chartType) {
        case 0:
        case 1:
        case 2:
            return [
           {
               type: 'category',
               data: GetXAxisLabelData(chartType)
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
        case 1:
        case 2:
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
        default:
            break;
    }
}

function GetXAxisLabelData(chartType) {
    //alert(_dates);
    //var label = ['09-01-2017', '09-02-2017', '09-03-2017', '09-04-2017', '09-05-2017', '09-06-2017', '09-07-2017'];
    //var label = ['Week1', 'Week2', 'Week3', 'Week4', 'Week5', 'Week6', 'Week7'];
    var label = _dates; //['Jan', 'Feb', 'Mar', 'Apr', 'May', 'June', 'July']; 
    switch (chartType) {
        case 0:
            return label;
            break;
        case 1:
            return label;
            break;
        case 2:
            return label;
            break;
        default:
    }
    return label;
}

function init_chart_doughnut(coldLead, warmLead, hotLead) {
    if ("undefined" != typeof Chart && (console.log("init_chart_doughnut"),
    $(".canvasDoughnut").length)) {
        var a = {
            type: "doughnut",
            tooltipFillColor: "rgba(51, 51, 51, 0.55)",
            data: {
                //labels: ["Symbian", "Blackberry", "Other", "Android", "IOS"],
                labels: ["Cold", "Warm", "Hot"],
                datasets: [{
                    data: [coldLead, warmLead, hotLead],
                    //backgroundColor: ["#BDC3C7", "#9B59B6", "#E74C3C", "#26B99A", "#3498DB"],
                    //hoverBackgroundColor: ["#CFD4D8", "#B370CF", "#E95E4F", "#36CAAB", "#49A9EA"]
                    backgroundColor: ['#40a6b1', '#fc9e00', '#e04b35'],
                    hoverBackgroundColor: ['#46b6c2', '#ffad00', '#f6523a']
                }]
            },
            options: {
                legend: !1,
                responsive: !1
            }
        };
        $(".canvasDoughnut").each(function () {
            var b = $(this);
            new Chart(b, a)
        })
    }
}



function createParticipatingDealerChart() {
    if ($('#participatingDealer_Chart').length) {
        var echart = echarts.init(document.getElementById('participatingDealer_Chart'), theme);
        echart.setOption({
            title: {
                text: '',
                subtext: ''
            },
            tooltip: {
                trigger: 'axis'
            },
            toolbox: {
                show: false,
                feature: {
                    mark: { show: true },
                    dataView: { show: false, readOnly: false },
                    magicType: {
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
                data: GetParticipatingDealerLegend()
            },
            xAxis: GetParticipatingDealerXAxis(),
            //yAxis: GetParticipatingDealerYAxis(),
            yAxis:
            {
                labels:
                {
                    enabled: false
                },
                data:[] //GetParticipatingDealerYAxis(),
            },
            series: GetParticipatingDealerSeries()
        }
        );
    }
}

function GetParticipatingDealerSeries() {
    // var data = getData();
    //// console.log("=====data", data);
    // var participatingDealersCount = [];
    // $.each(data, function (i, item) {
    //     //alert(data[i].count);
    //     participatingDealersCount.push(data[i].count);
    // });
    return [
                {
                    name: 'Dealers',
                    type: 'line',
                    smooth: true,
                    data: _participatingDealerCount
                },
    ];
}

function GetParticipatingDealerLegend() {
    return [];
}

function GetParticipatingDealerXAxis() {
    return [{
        type: 'category',
        boundaryGap: false,
        data: GetParticipatingDealerXAxisLabelData()
    }];
}

function GetParticipatingDealerXAxisLabelData() {
    //var label = [];
    //var data = getData();
    //$.each(data, function (i, item) {
    //    label.push(data[i].date);
    //});
    return _participatingDealerDates;
}

function GetParticipatingDealerYAxis() {
    return [{
        type: 'value'
    }];
}

//function getData() {
//    var temp_obj = { "date": '', "count": '' };
//    var finalData = [];
//    temp_obj = { "date": '2017-11-22', "count": 10 };
//    finalData.push(temp_obj);
//    temp_obj = { "date": '2017-11-23', "count": 250 };
//    finalData.push(temp_obj);
//    temp_obj = { "date": '2017-11-24', "count": 30 };
//    finalData.push(temp_obj);
//    temp_obj = { "date": '2017-11-25', "count": 350 };
//    finalData.push(temp_obj);
//    //for (var counter = 0; counter < dataUnique.length; counter++) {
//    //    var count = _.where(data, { CreateDate: dataUnique[counter].CreateDate }).length;
//    //    temp_obj = { "date": dataUnique[counter].CreateDate, "count": count };
//    //    finalData.push(temp_obj);
//    //}
//    return finalData;
//}

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


//var mapColorNoLead = { North: "#1FBB9D", East: "#1FBB9D", West: "#1FBB9D", South: "#1FBB9D" };

var mapColorNoLead = { SouthEast: "#9B59B6", NorthCentral: "#3498DB", SouthCentral: "#1FBB9D", West: "#BDC3C7", NorthEast: "#E74C3C" };

var mapColor = {

    // SouthEast
    "US-AL": mapColorNoLead.SouthEast,
    "US-FL": mapColorNoLead.SouthEast,
    "US-GA": mapColorNoLead.SouthEast,
    "US-LA": mapColorNoLead.SouthEast,
    "US-MS": mapColorNoLead.SouthEast,
    "US-NC": mapColorNoLead.SouthEast,
    "US-SC": mapColorNoLead.SouthEast,
    "US-TN": mapColorNoLead.SouthEast,
    "US-VA": mapColorNoLead.SouthEast,
    "US-WV": mapColorNoLead.SouthEast,
    "US-DC": mapColorNoLead.SouthEast,
    "US-MD": mapColorNoLead.SouthEast,

    // NorthCentral
    "US-IL": mapColorNoLead.NorthCentral,
    "US-IA": mapColorNoLead.NorthCentral,
    "US-IN": mapColorNoLead.NorthCentral,
    "US-KY": mapColorNoLead.NorthCentral,
    "US-MI": mapColorNoLead.NorthCentral,
    "US-MN": mapColorNoLead.NorthCentral,
    "US-ND": mapColorNoLead.NorthCentral,
    "US-OH": mapColorNoLead.NorthCentral,
    "US-SD": mapColorNoLead.NorthCentral,
    "US-WI": mapColorNoLead.NorthCentral,


    // SouthCentral
    "US-CO": mapColorNoLead.SouthCentral,
    "US-ID": mapColorNoLead.SouthCentral,
    //"US-IL": mapColorNoLead.SouthCentral,
    //"US-IA": mapColorNoLead.SouthCentral,
    "US-KS": mapColorNoLead.SouthCentral,
    "US-NE": mapColorNoLead.SouthCentral,
    "US-OK": mapColorNoLead.SouthCentral,
    "US-TX": mapColorNoLead.SouthCentral,
    "US-UT": mapColorNoLead.SouthCentral,
    "US-WY": mapColorNoLead.SouthCentral,
    "US-AR": mapColorNoLead.SouthCentral,
    "US-LA": mapColorNoLead.SouthCentral,
    "US-MO": mapColorNoLead.SouthCentral,


    // West
    "US-AK": mapColorNoLead.West,
    "US-AZ": mapColorNoLead.West,
    "US-CA": mapColorNoLead.West,
    "US-HI": mapColorNoLead.West,
    "US-ID": mapColorNoLead.West,
    "US-NV": mapColorNoLead.West,
    "US-WA": mapColorNoLead.West,
    "US-OR": mapColorNoLead.West,
    "US-NM": mapColorNoLead.West,
    "US-MT": mapColorNoLead.West,

    // NorthEast
    "US-CT": mapColorNoLead.NorthEast,
    "US-DE": mapColorNoLead.NorthEast,
    "US-ME": mapColorNoLead.NorthEast,
    "US-MA": mapColorNoLead.NorthEast,
    "US-NH": mapColorNoLead.NorthEast,
    "US-NJ": mapColorNoLead.NorthEast,
    "US-NY": mapColorNoLead.NorthEast,
    "US-PA": mapColorNoLead.NorthEast,
    "US-RI": mapColorNoLead.NorthEast,
    "US-VT": mapColorNoLead.NorthEast,
    "US-VA": mapColorNoLead.NorthEast,
    "US-WV": mapColorNoLead.NorthEast,

}

function SetDealerDataOnMap(isSearchCall) {
    //SetGlobalAccountId_Workspace();
    // _filterGraph.accountIDs = GetWorkspaceFilterIds();
    var data = {
        'StartDate': _filterGraph.startdate,
        'EndDate': _filterGraph.enddate,
        //'AccountObjectIDs': _filterGraph.accountIDs,
        'IsSearchCall': isSearchCall,
    };

    $.ajax({
        url: _getDealerDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $("#participatingDealerCount").text(result.ParticipatingDealerCount);
                $("#liSEDealer").text(result.Data.SouthEastDealers);
                $("#liNCDealer").text(result.Data.NorthCentralDealers);
                $("#liSCDealer").text(result.Data.SouthCentralDealers);
                $("#liWestDealer").text(result.Data.WestDealers);
                $("#liNEDealer").text(result.Data.NorthEastDealers);

                $("#liSELeads").text(result.Data.SouthEastLeads);
                $("#liNCLeads").text(result.Data.NorthCentralLeads);
                $("#liSCLeads").text(result.Data.SouthCentralLeads);
                $("#liWestLeads").text(result.Data.WestLeads);
                $("#liNELeads").text(result.Data.NorthEastLeads);


                //set participating dealer data to show on chart
                //console.log(result);
                _participatingDealerDates = result.Data.ParticipatingDealerChartData.CreatedDate;
                _participatingDealerCount = result.Data.ParticipatingDealerChartData.DealerCount;
                createParticipatingDealerChart();

                $('#map').replaceWith("<div id='map' style='min-height:400px !important;'></div>");
                $('.jvectormap-tip').replaceWith("");
                //console.log(result);
                var _setMapColor = $.extend({}, mapColor);
                $('#map').vectorMap({
                    map: 'us_aea',
                    backgroundColor: 'transparent',
                    markers: result.Data.Dealer.Coords,
                    zoomOnScroll: false,
                    series: {
                        markers: [{
                            attribute: 'fill',
                            scale: ['#be4a3e'],
                            values: result.Data.Dealer.Coords,
                        }, {
                            attribute: 'r',
                            scale: [8],
                            values: result.Data.Dealer.Coords,
                        }],
                        regions: [{
                            values: _setMapColor,
                            attribute: 'fill',
                        }]
                    },
                    onMarkerTipShow: function (event, label, index) {
                        label.html(
                          '<b>' + result.Data.Dealer.Names[index] + ', ' + result.Data.Dealer.Cities[index] + '</b><br/>'
                          +
                          '<b>Leads: </b>' + result.Data.Dealer.LeadCount[index] + '</br>'
                        );
                    },
                });

                var mapObject = $('#map').vectorMap('get', 'mapObject');
            }
            else {
                alert('Error in getting dealer details.')
            }
        }
    });
}

function updatePageFromWorkspace() {
    NProgress.start();
    // SetGlobalAccountId_Workspace();
    // _filterGraph.accountIDs = GetWorkspaceFilterIds();
    // $("#spnAccountCount").text(GetSelectAccountIdText(actIds));
    var data = {
        'StartDate': _filterGraph.startdate,
        'EndDate': _filterGraph.enddate,
        'IsAjax': true,
        //'AccountObjectIDs': _filterGraph.accountIDs
    };
    $.ajax({
        url: _updateDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                $("#memberCount").text(result.MemberCount);
                $("#participatingDealerCount").text(result.ParticipatingDealerCount);
                $("#averageLeadScore").text(result.AverageLeadScore);
                $("#conversionRate").text(result.ConversionRate + '%');
                $("#spConversionRateDataPercent").attr("data-percent", result.ConversionRate);
                $("#spConversionRatePercent").text(result.ConversionRate);
                $("#averagePassToDealerDays").text(result.AveragePassToDealerDays);
                $("#averageCreateDateToSaleDate").text(result.AverageCreateDateToSaleDate);
                $("#passOffRate").text(result.PassOffRate + '%');
                //$("#spMemberCount").text(result.MemberCount);
                //$("#spMemberCount").text(result.MemberCount);
                $("#spPassOffRateDataPercent").attr("data-percent", result.PassOffRate);
                $("#spPassOffRatePercent").text(result.PassOffRate);
                $("#spWonLeadTotalCount").text(result.MemberCount);
                $("#spWonLeadCount").text(result.WonLeadCount);
                $("#spAverageLead").text(result.LeadScoreCount);
                $("#gauge-text").text(result.LeadScoreMin);
                $("#goal-text").text(result.LeadScoreMax);
                $("#spPassOffLeadCount").text(result.PassOffLeadCount);
                $("#spMemberCount").text(result.MemberCount);
                $("#spPassOffLeadCount").text(result.PassOffLeadCount);
                $("#spnStartDate").text(result.StartDate);
                $("#spnEndDate").text(result.EndDate);
                GetLeadChartData(true);
                SetDealerDataOnMap(true);
                InitializePieChart();
                MakeLeadAverageGaugeChart(result.AverageLeadScore);
                createParticipatingDealerChart();
            }
            NProgress.done();
        },
        error: function (err) {
            NProgress.done();
        }
    });
}