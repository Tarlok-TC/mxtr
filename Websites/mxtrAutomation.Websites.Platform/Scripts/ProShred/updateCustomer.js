function updateCustomerData(retreiveCode) {

    var CustomerID = $('#CustomerID').val();
    //var email2 = $("#Email2").val();

    //Company Information
    var companyName = $("#CompanyName").val();
    var first = $("#ContactFName").val();
    var last = $("#ContactLName").val();
    var phone = $("#ContactPhone").val();
    var fax = $("#Fax").val();
    var attention = $("#Attention").val();
    var email = $("#email1").val();
    var emailInvoice = $("#EmailInvoices").prop('checked') === true;
    var emailCOD = $("#EmailCOD").prop('checked') === true;

    //Billing Address
    var Suite = $("#Suite").val();
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
    var BuildingID = $("#BuildingID").val();
    var BldgContact1 = $("#BldgContactFName").val() + " " + $('#BldgContactLName').val();
    var BldgContact2 = $("#BldgContactFName2").val() + " " + $('#BldgContactLName2').val();
    var BuildingPhone = $("#BuildingPhone").val();
    var BuildingPhone2 = $("#BuildingPhone2").val();

    //Direction Notes
    var Directions = $("#Directions").val();
    var RouteInstructions = $("#RouteInstructions").val();

    //Company Location
    var BuildingStreet = $("#BuildingStreet").val();
    var BuildingSuite = $("#BuildingSuite").val();
    var BuildingCity = $("#BuildingCity").val();
    var BuildingState = $("#BuildingState").val();
    var BuildingZip = $("#BuildingZip").val();
    var ScheduleFrequency = $("#ScheduleFrequency option:selected").val();
    var ServiceType = $("#ServiceType option:selected").val();

    //Custom Fields
    var mobile = $("#MobileNumber").val();
    var NumberOfEmployees = $("#NumberOfEmployees").val();
    var PipelineStatus = $("#btnGroup input:checked").val();

    var SalesmanID = $("#Salesman option:selected").val();
    var objEZShredDataModel = new Object();
    var test = objectifyForm(fields);

    objEZShredDataModel.Customers = [{
        "CustomerID": CustomerID,
        "Company": companyName,
        "Attention": attention,
        "Suite": Suite,
        "Street": street,
        "City": city,
        "State": state,
        "Zip": zip,
        "Contact": first + " " + last,
        "Phone": phone,
        "Fax": fax,
        "Notes": notes,
        "CustomerTypeID": CustomerType,
        "InvoiceTypeID": InvoiceType,
        "EmailAddress": email,
        "EmailInvoice": emailInvoice,
        "EmailCOD": emailCOD,
        "ReferralSourceID": ReferralSourceDropDown,
        "TermID": terms,
        //custom fields
        "PipelineStatus": PipelineStatus,
        "NumberOfEmployees": NumberOfEmployees,
        "Mobile": mobile
    }];
    objEZShredDataModel.Buildings = [{
        "BuildingID": BuildingID,
        "CompanyName": BuildingName,
        "Suite": Suite,
        "BuildingTypeID": BuildingType,
        "Street": BuildingStreet,
        "City": BuildingCity,
        "State": BuildingState,
        "Zip": BuildingZip,
        "Phone1": BuildingPhone,
        "Phone2": BuildingPhone2,
        "SiteContact1": BldgContact1,
        "SiteContact2": BldgContact2,
        "ScheduleFrequency": ScheduleFrequency,
        "ServiceTypeID": ServiceType,
        "Directions": Directions,
        "RoutineInstructions": RouteInstructions,
        "SalesmanID": SalesmanID, 
    }];
    $.ajax({
        type: "post",
        data: {
            'objEZShredDataModel': objEZShredDataModel
        },
        url: "/UpdateCustomer",
        async: false,
        cache: false,
        success: function (response) {
            if (response) {
                alert(333);
                var l1 = Ladda.create(document.querySelector('.ladda-button'));
                l1.stop();
                $('#Customers').trigger('change');
            }
        }
    });
    //
    //$.ajax({
    //    type: "post",
    //    url: "http://hosting.proshred.com:4550/api/ezshred/getdata",
    //    data: JSON.stringify({
    //        'Request': JSON.stringify({
    //            "Request": "UpdateCustomer",
    //            "CustomerID": CustomerID.value,
    //            "tblCustomers": [
    //                {
    //                    "Contact": fields[1][0].value
    //                }
    //            ]
    //        })
    //    }),
    //    async: true,
    //    cache: false,
    //    success: function (response) {
    //        if (response.Success) {
    //            updateCustomerinDb();
    //        } else {
    //            console.log(response.FailureInformation)
    //        }
    //    },
    //    complete: function () {
    //        //submitForm();
    //    }
    //});
}

function objectifyForm(fields) { //serialize data function

    var returnArray = {};
    for (var i = 0; i < fields.length; i++) {
        returnArray[fields[i]['name']] = fields[i]['value'];
    }
    return returnArray;
}