$(document).ready(function () {
    var barOptions = {
        responsive: true,        maintainAspectRatio: false,        barDatasetSpacing: 6,        barValueSpacing: 5,        bezierCurve: false,        scales: {
            yAxes: [{
                type: "linear",                display: true,                position: "left",                id: "y-axis-1"
            }, {
                type: "linear",                display: true,                position: "right",                id: "y-axis-2",                gridLines: {
                    drawOnChartArea: false
                }
            }]
        },        legend: { display: false }
    };    var ctx = document.getElementById("scorebox-chart-canvas").getContext("2d");    var config = {
        type: 'line',        data: {
            labels: ["January", "February", "March", "April", "May", "June", "July"],            datasets: [                {
                    label: "My First dataset",                    fill: false,                    lineTension: 0.01,                    backgroundColor: "rgba(75,192,192,0.4)",                    borderColor: "rgba(75,192,192,1)",                    borderCapStyle: 'butt',                    borderDash: [],                    borderDashOffset: 0.0,                    borderJoinStyle: 'miter',                    pointBorderColor: "rgba(75,192,192,1)",                    pointBackgroundColor: "#fff",                    pointBorderWidth: 1,                    pointHoverRadius: 5,                    pointHoverBackgroundColor: "rgba(75,192,192,1)",                    pointHoverBorderColor: "rgba(220,220,220,1)",                    pointHoverBorderWidth: 1,                    pointRadius: 5,                    pointHitRadius: 10,                    data: [65, 59, 80, 81, 56, 55, 40],                    spanGaps: true
                },                {
                    label: "My First dataset",                    fill: false,                    lineTension: 0.01,                    backgroundColor: "rgba(179,181,198,0.2)",                    borderColor: "rgba(179,181,198,1)",                    pointBackgroundColor: "rgba(179,181,198,1)",                    pointBorderColor: "#fff",                    pointHoverBackgroundColor: "#fff",                    pointHoverBorderColor: "rgba(179,181,198,1)",                    data: [65, 59, 90, 81, 56, 55, 40],                    spanGaps: true,                    pointRadius: 5,                    pointHitRadius: 10
                },                {
                    label: "My Second dataset",                    lineTension: 0.01,                    fill: false,                    backgroundColor: "rgba(255,99,132,0.2)",                    borderColor: "rgba(255,99,132,1)",                    pointBackgroundColor: "rgba(255,99,132,1)",                    pointBorderColor: "#fff",                    pointHoverBackgroundColor: "#fff",                    pointHoverBorderColor: "rgba(255,99,132,1)",                    data: [28, 48, 40, 19, 96, 27, 100],                    pointRadius: 5,                    pointHitRadius: 10,                    spanGaps: true
                }            ]
        },        options: barOptions
    };    var pieChart = document.getElementsByClassName("myChart");    var myChart = new Chart(pieChart, {
        type: 'doughnut',        data: {
            labels: [],            datasets: [{
                backgroundColor: [                    "#2ecc71",                    "#3498db",                    "#95a5a6",                    "#9b59b6",                    "#f1c40f",                    "#e74c3c",                    "#34495e"                ],                data: [30, 10, 20, 15, 30]
            }]
        }
    });    var myLineChart = new Chart(ctx, config);    $(document).on('click', '.link-score-box', function (event) {
        event.preventDefault;        $('.link-score-box.is-active').removeClass('is-active');        $(this).toggleClass('is-active');        _activeChart = $(this).data('report');        buildChart();
    });    var randomColorFactor = function () {
        return Math.round(Math.random() * 255);
    };    var randomColor = function (opacity) {
        return 'rgba(' + randomColorFactor() + ',' + randomColorFactor() + ',' + randomColorFactor() + ',' + (opacity || '.3') + ')';
    };    function buildChart() {
        if (_activeChart == 'leads') {
            displayChart('line', lineOptions, _leadsChartData, 'Total Leads');
        }        else if (_activeChart == 'opportunities') {
            displayChart('line', lineOptions, _openOpportunityValueChartData, 'Total Open Opportunity Value');
        }        else {
            displayChart('bar', barOptions, _campaignsChartData, 'Top 5 Campaigns');
        }
    }    function displayChart(type, options, data, title) {
        myLineChart.stop();        myLineChart.destroy();        config.type = type;        config.options = options;        config.data = data;        myLineChart = new Chart(ctx, config);        $('#scorebox-chart-title').html(title);
    }    window.updateDashboard = function () {
        $('#DashboardContainer').fadeOut(500, function () {
            var data =                {
                    'StartDate': _filterGraph.startdate,                    'EndDate': _filterGraph.enddate,                    'AccountObjectIDs': _filterGraph.accountIDs,                    'IsAjax': true
                };            $.ajax({
                url: _refreshUrl,                dataType: 'json',                type: 'POST',                data: data,                success: function (result) {
                    $('#CampaignsCount').html(numberWithCommas(result.Data.TotalCampaigns));                    $('#CampaignsDeltaCSS').attr('class', result.Data.TotalCampaignsDeltaArrowCss);                    $('#CampaignsDelta').html(numberToAbsolutePercentage(result.Data.TotalCampaignsDelta) + '%');                    $('#LeadsCount').html(numberWithCommas(result.Data.TotalLeads));                    $('#LeadsDeltaCSS').attr('class', result.Data.TotalLeadsDeltaArrowCss);                    $('#LeadsDelta').html(numberToAbsolutePercentage(result.Data.TotalLeadsDelta) + '%');                    $('#OpenOpportunityValue').html('$' + numberWithCommas(result.Data.OpenOpportunityValue));                    $('#OpenOpportunityValueDeltaCSS').attr('class', result.Data.OpenOpportunityValueDeltaArrowCss);                    $('#OpenOpportunityValueDelta').html(numberToAbsolutePercentage(result.Data.OpenOpportunityValueDelta) + '%');                    _leadsChartData = result.Data.LeadsChartData;                    _campaignsChartData = result.Data.CampaignsChartData;                    _openOpportunityValueChartData = result.Data.OpenOpportunityValueChartData;                    buildChart();                    $('#DashboardContainer').fadeIn(500, function () {
                    });
                },                error: function (result) {
                    console.log(result);
                }
            });
        });
    };
});var opts = {
    lines: 12,    angle: 0,    lineWidth: 0.4,    pointer: {
        length: 0.75,        strokeWidth: 0.042,        color: '#1D212A'
    },    limitMax: 'false',    colorStart: '#1ABC9C',    colorStop: '#1ABC9C',    strokeColor: '#F0F3F3',    generateGradient: true
};var target = document.getElementById('foo'),    gauge = new Gauge(target).setOptions(opts);gauge.maxValue = 100;gauge.animationSpeed = 32;gauge.set(80);gauge.setTextField(document.getElementById("gauge-text"));var target = document.getElementById('foo2'),    gauge = new Gauge(target).setOptions(opts);gauge.maxValue = 5000;gauge.animationSpeed = 32;gauge.set(4200);gauge.setTextField(document.getElementById("gauge-text2"));