$(document).ready(function () {
    InitializePieChart();
});

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

function updatePageFromWorkspace() {
    var data = {
        'StartDate': _filterGraph.startdate,
        'EndDate': _filterGraph.enddate,
        'IsAjax': true,
        'Id': _filterGraph.accountIDs,
        //'AccountObjectIDs': _filterGraph.accountIDs,
    };
    $.ajax({
        url: _updateDataUrl,
        dataType: 'json',
        type: 'post',
        data: data,
        success: function (result) {
            if (result.Success) {
                // console.log(result);  
                _data = result.DealerLeads;
                SetWidgetAndTableData(result.DealerDetail);
                var scope = angular.element(document.getElementById("dvDealerLeads")).scope();
                scope.$apply(function () {
                    scope.bindLead();
                });
            }
        }
    });
}

function SetWidgetAndTableData(searchResult) {
    //Widget
    $("#leadsCountInDealerFunnel").text(searchResult.LeadsCount);
    $("#spWidgetAverageLeadTimeDealer").text(searchResult.AverageTimeInFunnel);
    $("#conversionRateDealer").text(searchResult.ConversionRate);
    $("#spConversionRateDealerPercentage").text(searchResult.ConversionRate);
    $("#spConversionRateDealerDataPercentage").attr('data-percent', searchResult.ConversionRate);
    //Table
    $("#spLeadsCount").text(searchResult.LeadsCount);
    $("#spColdLeadsCount").text(searchResult.ColdLeadsCount);
    $("#spWarmLeadsCount").text(searchResult.WarmLeadsCount);
    $("#spHotLeadsCount").text(searchResult.HotLeadsCount);
    $("#spHandedOffLeadsCount").text(searchResult.HandedOffLeads);
    $("#spPassOff").text(searchResult.PassOff);
    $("#spAverageTimeInFunnel").text(searchResult.AverageTimeInFunnel);
    $("#spConversionRate").text(searchResult.ConversionRate);
}

//---------- Angular section ------------
(function () {
    'use strict';
    var dealerApp = angular.module('dealerApp', []);

    dealerApp.controller('dealerController', function ($scope) {

        $scope.bindLead = function () {
            // console.log(_data);
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
            return _leadUrl + "?id=" + item.ObjectID + "&src=det";
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