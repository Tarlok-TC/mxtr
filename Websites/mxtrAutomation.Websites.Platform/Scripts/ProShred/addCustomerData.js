function addCustomerData(retreiveCode) {
   // var CustomerID = $('#CustomerID').val();
    var email2 = $("#email2").val();

    //Company Information
    var companyName = $("#CompanyName").val();
    var NumberOfEmployees = $("#NumberOfEmployees option:selected").val();
    var first = $("#ContactFName").val();
    var last = $("#ContactLName").val();
    var phone = $("#ContactPhone").val();
    var mobile = $("#MobileNumber").val();
    var attention = $("#Attention").val();
    var email = $("#email1").val();
    var emailInvoice = $("#EmailInvoices").prop('checked') == true;

    //Billing Address
    var street = $("#Street").val();
    var city = $("#City").val();
    var state = $("#State").val();
    var zip = $("#Zip").val();

    //Customer Profile
    var CustomerType = $("#IndustryType option:selected").val();
    var InvoiceType = $("#InvoiceType option:selected").val();
    var terms = $("#TermsDropDown option:selected").val();
    var ReferralSourceDropDown = $("#ReferralSourceDropDown option:selected").val();
    var notes = $("#Notes").val();

    //Building Info
    var BuildingName = $("#BuildingName").val();
    var BuildingType = $("#BuildingType option:selected").val();
    //var BuildingID = $("#BuildingID").val();
    //Direction Notes
    var Directions = $("#Directions").val();
    var RouteInstructions = $("#RouteInstructions").val();

    //Company Location
    var BuildingStreet = $("#BuildingStreet").val();
    var BuildingCity = $("#BuildingCity").val();
    var BuildingState = $("#BuildingState").val();
    var BuildingZip = $("#BuildingZip").val();
    var ScheduleFrequency = $("#ScheduleFrequency option:selected").val();
    var ServiceType = $("#ServiceType option:selected").val();
    
    $.ajax({
        type: "post",
        url: "http://hosting.proshred.com:4550/api/ezshred/getdata",
        data: JSON.stringify({
            'Request': JSON.stringify({
                "Request": "AddCustomer",
                "tblCustomers": [{
                    "Company": companyName,
                    "Attention": attention,
                    "Street": street,
                    "City": city,
                    "State": state,
                    "Zip": zip,
                    "Contact": first + " " + last,
                    "Phone": phone,
                    "CustomerTypeID": CustomerType,
                    "InvoiceTypeID": InvoiceType,
                    "EmailAddress": email,
                    "EmailInvoice": emailInvoice,
                    "ReferralsourceID": ReferralSourceDropDown,
                    "TermID": terms,
                    "Notes": notes
                }]
            })
        }),
        async: true,
        cache: false,
        success: function (response) {
            if (response.Success) {
                alert(response.Success);
                var result = JSON.parse(response.Result)
                var passed = result.status == "OK";
                if (passed) {
                    alert('New CustomerID- ' + _ResponseNewCustomerID);
                    //addCustomersandBuilding(result.CustomerID, BuildingId)
                    //addBuildingData(parseInt(result.CustomerID));

                    //getCustomerInfo(result.CustomerID, function (ARCode) {
                    //    retreiveCode(ARCode);
                    //    alert(ARCode);

                    //});
                } else {
                    l.stop();
                    alert("Warning! Customer not added, " + result.status);
                }

            } else {
                l.stop();
                console.log(response.FailureInformation)
                alert("Could not contact API to Add Customer.. " + response.FailureInformation);

            }
        },
        error: function (response) {
            alert('error');
        }
    });


 
}
function addCustomersandBuilding(CustomerID,BuildingId) {
    var objEZShredDataModel = new Object();
    objEZShredDataModel.Buildings = [{
        "BuildingID": BuildingId,
        "CompanyName": BuildingName,
        "BuildingTypeID": BuildingType,
        "Street": street,
        "City": city,
        "State": state,
        "Zip": zip,
        "ScheduleFrequency": ScheduleFrequency,
        "ServiceTypeID": ServiceType,
        "Directions": Directions,
        "RoutineInstructions": RouteInstructions
    }];
    objEZShredDataModel.Customers = [{
        "CustomerID": CustomerID,
        "Company": companyName,
        "Attention": attention,
        "Street": street,
        "City": city,
        "State": state,
        "Zip": zip,
        "Contact": first + " " + last,
        "Phone": phone,
        "Notes": notes,
        "CustomerTypeID": CustomerType,
        "InvoiceTypeID": InvoiceType,
        "EmailAddress": email,
        "EmailInvoice": emailInvoice,
        "ReferralSourceID": ReferralSourceDropDown,
        "TermID": terms,
    }];
    $.ajax({
        type: "post",
        data: { 'objEZShredDataModel': objEZShredDataModel },
        url: "/AddCustomer",
        async: false,
        cache: false,
        success: function (response) {
            if (response.Data != undefined) {
                _buildings = response.Data;
            }
        }
    });
}
