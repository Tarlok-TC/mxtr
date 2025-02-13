function updateBuilding(BuildingID) {


    

    $.ajax({
        type: "post",
        url: "http://hosting.proshred.com:4550/api/ezshred/getdata",
        data: JSON.stringify({
            'Request': JSON.stringify({
                "Request": "UpdateBuilding",
                "BuildingID": BuildingID,
                "tblBuilding": [
                    {
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
                    }
                ]
            })
        }),
        async: true,
        cache: false,
        success: function (response) {
            if (response.Success) {

            } else {
                console.log(response.FailureInformation)
            }
        },
        complete: function () {
            //submitForm();
        }
    });
}