//Script to add building info after customer has been added
var _ResponseNewBuildingID = "";
/**
 * Function to add building data using the passed in CustomerID
 * @param {string} customerId
 */
function addBuildingData(customerId) {
    var buildingName = $("#BuildingName").val();
    var bldg_street = $("#BuildingStreet").val();
    var bldg_City = $("#BuildingCity").val();
    var bldg_State = $("#BuildingState").val();
    var bldg_Zip = $("#BuildingZip").val();
    var phone = $("#PhoneNumber").val();
    var directions = $("#Directions").val();
    var routineInstructions = $("#RouteInstructions").val();
    var siteContact1 = $("#ContactFName").val(); + $('#ContactLName').val();

    $.ajax({
        type: "post",
        url: "http://hosting.proshred.com:4550/api/ezshred/getdata",
        data: JSON.stringify({
            'Request': JSON.stringify({
                "Request": "AddBuilding",
                "tblBuilding": [{
                    "BuildingTypeID": "5",
                    "CustomerID": customerId,
                    "CompanyName": buildingName,
                    "Street": bldg_street,
                    "City": bldg_City,
                    "State": bldg_State,
                    "Zip": bldg_Zip,
                    "Phone1": phone,
                    "SalesmanID": "342",
                    "Directions": directions,
                    "RoutineInstructions": routineInstructions,
                    "SiteContact1": siteContact1,
                    "SiteContact2": "",
                    "RouteID": "113",
                    "stop": "99",
                    "ScheduleFrequency": "P",
                    "ServiceTypeID": "XX",
                    "SalesTaxRegionID": 1
                }]
            })
        }),
        async: true,
        cache: false,
        success: function (response) {
            if (response.Success) {
                var result = JSON.parse(response.Result);
                var passed = result.status === "OK";
                if (passed) {


                } else {
                    alert("Error while adding building data! " + result.status);

                }

            } else {
                console.log(response.FailureInformation);
                alert("Error while adding building data! " + response.FailureInformation);
            }
        },
        complete: function () {
            submitForm();
            l.stop();
        }
    });
}