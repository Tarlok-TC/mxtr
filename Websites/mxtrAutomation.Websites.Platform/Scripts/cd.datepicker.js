$(document).ready(function () {

    // Calendar
    $('.m-report-filters-daterange').calendar360({
        dateFormat: 'MM/DD/YYYY',
        titleFormat: 'MMM. DD, YYYY',
        presets: {
            currentWeek: {
                text: 'Current Week',
                isDefault: false,
                timeshift: function (moment) {
                    moment.startOf('week');
                    return moment;
                }
            },
            currentMonth: {
                text: 'Current Month',
                isDefault: false,
                timeshift: function (moment) {
                    moment.date(1);
                    return moment;
                }
            },
            pastSevenDays: {
                text: 'Past 7 Days',
                isDefault: false,
                timeshift: function (moment) {
                    //MXTR-482 changed 7 to 6
                    moment.subtract(6, 'days');
                    return moment;
                }
            },
            pastThirtyDays: {
                text: 'Past 30 Days',
                isDefault: true,
                timeshift: function (moment) {
                    //MXTR-482 changed 30 to 29
                    moment.subtract(29, 'days');
                    return moment;
                }
            },
            yearToDate: {
                text: 'YTD',
                isDefault: false,
                timeshift: function (moment) {
                    moment.dayOfYear(1);
                    return moment;
                }
            }
        },
        post: updatePage
    });

});


function updatePage(newDateRange) {
    _filterGraph.startdate = newDateRange.start.format('MM/DD/YYYY');
    _filterGraph.enddate = newDateRange.end.format('MM/DD/YYYY');

    executeCallbackFromFilters(_callBackFunction);
}
