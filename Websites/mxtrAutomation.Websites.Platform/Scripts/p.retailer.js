$(document).ready(function () {
    buildChart(RetailerChartTypeEnum.Search);
    buildChart(RetailerChartTypeEnum.Leads);
    buildChart(RetailerChartTypeEnum.Email);
    selectUnSelectAll_ShoeHideColumn(true, true);
});

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
                            bar: 'Bar'
                            // stack: 'Stack',
                            // tiled: 'Tiled'
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

function updatePageFromWorkspace() {
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
               // console.log(result);
                _data = result.RetailerLeads;
                updateScoreBoxesandChart(result);
                var scope = angular.element(document.getElementById("dvRetailerLeads")).scope();
                scope.$apply(function () {
                    scope.bindLead();
                });

            }
        }
    });
}

function updateScoreBoxesandChart(result) {    
    //--Score boxes
    $("#dvLocatorPageviews").text(result.RetailerActivityReportViewData.LocatorPageviews);
    $("#dvLPPageviews").text(result.RetailerActivityReportViewData.LPPageviews);
    $("#dvContacts").text(result.RetailerActivityReportViewData.Contacts);
    //--Information block
    $("#spLocatorPageviews").text(result.RetailerActivityReportViewData.LocatorPageviews);
    $("#spLPPageviews").text(result.RetailerActivityReportViewData.LPPageviews);
    $("#spDirectionsClicks").text(result.RetailerActivityReportViewData.DirectionsClicks);
    $("#spTotalClicks").text(result.RetailerActivityReportViewData.TotalClicks);
    $("#spMoreInfoClicks").text(result.RetailerActivityReportViewData.MoreInfoClicks);
    $("#spFormSubmissions").text(result.RetailerActivityReportViewData.FormSubmissions);
    $("#spContacts").text(result.RetailerActivityReportViewData.Contacts);
    $("#spEmailsSent").text(result.RetailerActivityReportViewData.EmailsSent);
    $("#spHourClicks").text(result.RetailerActivityReportViewData.MapClicks);
    $("#spLogoClicks").text(result.RetailerActivityReportViewData.LogoClicks);
    $("#spWebsiteClicks").text(result.RetailerActivityReportViewData.WebsiteClicks);
    $("#spPhoneClicks").text(result.RetailerActivityReportViewData.PhoneClicks);
    //--Chart
    _searchData = result.SearchData;
    _leadsData = result.LeadsData;
    _emailData = result.EmailData;
    buildChart(RetailerChartTypeEnum.Search);
    buildChart(RetailerChartTypeEnum.Leads);
    buildChart(RetailerChartTypeEnum.Email);
}

function showHideColumn(mySelection) {
    var whichItem = $(mySelection).attr('id');
    var show = false;
    if ($("#" + whichItem + " > span > i").hasClass('fa-check')) {
        $("#" + whichItem + " > span > i").removeClass('fa-check');
    }
    else {
        $("#" + whichItem + " > span > i").addClass('fa-check');
        show = true;
    }
    var tdBody = '';
    var thHead = '';
    var allSelectOption = false;
    switch (whichItem) {
        case 'aPageviewsLocator':
            tdBody = "tdPageviewsLocator";
            thHead = 'thPageviewsLocator';
            break;
        case 'aPageviewsLP':
            tdBody = "tdPageviewsLP";
            thHead = 'thPageviewsLP';
            break;
        case 'aFormSubmissions':
            tdBody = "thFormSubmissions";
            thHead = 'tdFormSubmissions';
            break;
        case 'aContacts':
            tdBody = "tdContacts";
            thHead = 'thContacts';
            break;
        case 'aEmailsSent':
            tdBody = "tdEmailsSent";
            thHead = 'thEmailsSent';
            break;
        case 'aTotalClicks':
            tdBody = "tdTotalClicks";
            thHead = 'thTotalClicks';
            break;
        case 'aDirectionClicks':
            tdBody = "tdClickDirection";
            thHead = 'thClickDirection';
            break;
        case 'aHoursClicks':
            tdBody = "tdClickHour";
            thHead = 'thClickHour';
            break;
            //case 'aMoreInfoClicks':
            //    tdBody = "tdClickMoreInfo";
            //    thHead = 'thClickMoreInfo';
            //    break;
        case 'aLogoClicks':
            tdBody = "tdLogo";
            thHead = 'thLogo';
            break;
        case 'aWebsiteClicks':
            tdBody = "tdWebsite";
            thHead = 'thWebsite';
            break;
        case 'aPhoneClicks':
            tdBody = "tdPhone";
            thHead = 'thPhone';
            break;
        case 'aSelectAll':
            allSelectOption = true;
            break;
        default:
    }

    if (!allSelectOption) {
        if (show) {
            $("#" + tdBody).show();
            $("#" + thHead).show();
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
        }
        else {
            $("#" + tdBody).hide();
            $("#" + thHead).hide();
            //check if any one is unselected then make select all unchecked as well
            $("#aSelectAll > span > i").removeClass('fa-check');
        }
    }
    else {
        selectUnSelectAll_ShoeHideColumn(show, false);
    }
    isShowHide = true;
}

function selectUnSelectAll_ShoeHideColumn(selectOrUnselect, isPageLoad) {
    if (selectOrUnselect) {
        if (!isPageLoad) {
            $("#filterMenu a > span > i").addClass('fa-check');
        }

        $("#tdPageviewsLocator").show();
        $("#thPageviewsLocator").show();
        $("#tdPageviewsLP").show();
        $("#thPageviewsLP").show();
        $("#thFormSubmissions").show();
        $("#tdFormSubmissions").show();
        $("#tdContacts").show();
        $("#thContacts").show();
        $("#tdEmailsSent").show();
        $("#thEmailsSent").show();
        $("#tdTotalClicks").show();
        $("#thTotalClicks").show();
        $("#tdClickDirection").show();
        $("#thClickDirection").show();
        $("#tdClickHour").show();
        $("#thClickHour").show();
        $("#tdClickMoreInfo").show();
        //$("#thClickMoreInfo").show();
        $("#tdLogo").show();
        $("#thLogo").show();
        $("#tdWebsite").show();
        $("#thWebsite").show();
        $("#tdPhone").show();
        $("#thPhone").show();
    }
    else {

        $("#filterMenu a > span > i").removeClass('fa-check');

        $("#tdPageviewsLocator").hide();
        $("#thPageviewsLocator").hide();
        $("#tdPageviewsLP").hide();
        $("#thPageviewsLP").hide();
        $("#thFormSubmissions").hide();
        $("#tdFormSubmissions").hide();
        $("#tdContacts").hide();
        $("#thContacts").hide();
        $("#tdEmailsSent").hide();
        $("#thEmailsSent").hide();
        $("#tdTotalClicks").hide();
        $("#thTotalClicks").hide();
        $("#tdClickDirection").hide();
        $("#thClickDirection").hide();
        $("#tdClickHour").hide();
        $("#thClickHour").hide();
        $("#tdClickMoreInfo").hide();
        //$("#thClickMoreInfo").hide();
        $("#tdLogo").hide();
        $("#thLogo").hide();
        $("#tdWebsite").hide();
        $("#thWebsite").hide();
        $("#tdPhone").hide();
        $("#thPhone").hide();
    }
}

var isShowHide = false;
function KeepShowHideDropdownOpen() {
    $("#dvOption").attr('aria-expanded', 'true');
    $("#ddlShowHideColumn").addClass('open');
}

$(document).click(function (e) {
    if (isShowHide) {
        KeepShowHideDropdownOpen();
        isShowHide = false;
    }
});

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

//---------- Angular section ------------
(function () {
    'use strict';
    var RetailerApp = angular.module('retailerApp', []);

    RetailerApp.controller('retailerController', function ($scope) {

        $scope.bindLead = function () {
            $scope.dummyItems = _data;
            $scope.pager = {};
            $scope.setPage = setPage;
            initController();
        }

        $scope.bindLead();

        function initController() {
            // initialize to page 1
            $scope.setPage(1);
        }

        function setPage(page) {
            if (page < 1 || page > $scope.pager.totalPages) {
                return;
            }

            // get pager object from service
            $scope.pager = GetPager($scope.dummyItems.length, page);

            // get current page of items
            $scope.items = $scope.dummyItems.slice($scope.pager.startIndex, $scope.pager.endIndex + 1);
        }

        function GetPager(totalItems, currentPage, pageSize) {
            // default to first page
            currentPage = currentPage || 1;

            // default page size is 6
            pageSize = pageSize || 6;

            // calculate total pages
            var totalPages = Math.ceil(totalItems / pageSize);

            var startPage, endPage;
            if (totalPages <= 6) {
                // less than 6 total pages so show all
                startPage = 1;
                endPage = totalPages;
            } else {
                // more than 6 total pages so calculate start and end pages
                if (currentPage <= 6) {
                    startPage = 1;
                    endPage = 6;
                } else if (currentPage + 4 >= totalPages) {
                    startPage = totalPages - 9;
                    endPage = totalPages;
                } else {
                    startPage = currentPage - 5;
                    endPage = currentPage + 4;
                }
            }

            // calculate start and end item indexes
            var startIndex = (currentPage - 1) * pageSize;
            var endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

            // create an array of pages to ng-repeat in the pager control
            var pages = _.range(startPage, endPage + 1);

            // return object with all pager properties required by the view
            return {
                totalItems: totalItems,
                currentPage: currentPage,
                pageSize: pageSize,
                totalPages: totalPages,
                startPage: startPage,
                endPage: endPage,
                startIndex: startIndex,
                endIndex: endIndex,
                pages: pages
            };
        }

        $scope.GetCompanyName = function (item) {
            var companyName = item.CompanyName == "" ? "N/A" : item.CompanyName;
            return companyName;
        }

        $scope.GetName = function (item) {
            var name = "N/A";
            if (!(item.FirstName == "" && item.LastName == "")) {
                if (item.FirstName != "") {
                    name = item.FirstName + " " + item.LastName;
                }
                else {
                    name = item.LastName;
                }
            }
            return name;
        }

        $scope.GetEmail = function (item) {
            var email = item.EmailAddress == "" ? "N/A" : item.EmailAddress;
            return email;
        }

        $scope.GetCity = function (item) {
            var city = item.City == "" ? "N/A" : item.City;
            return city;
        }

        $scope.GetState = function (item) {
            var state = (item.State == "" && item.City == "") ? "" : item.State;
            return state;
        }

        $scope.GetSep = function (item) {
            var state = (item.State == "" && item.City == "") ? "" : item.State;
            var sep = state == "" ? "" : ",";
            return sep;
        }

        $scope.GetUrl = function (item) {
            return _leadUrl + "?id=" + item.ObjectID + "&src=ret";
        }

        $scope.DeleteLead = function (leadId) {
            //alert(leadId);          
            var leadObjectId = leadId;
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
                            $scope.$apply(function () {
                                //--remove item and rebind
                                _data = _.without(_data, _.findWhere(_data, {
                                    ObjectID: leadId
                                }));
                                $scope.bindLead();
                            });
                            SuccessAlert("Lead Deleted", "The lead has been deleted successfully.")
                        }
                        else {
                            // $('.user-message').html(result.Message).removeClass('is-success').addClass('is-failure').fadeIn('fast');
                            ErrorAlert("Error", result.Message);
                        }
                        //HideMessage();
                    }
                });

            }, function () {

            });
        }
    });

})();