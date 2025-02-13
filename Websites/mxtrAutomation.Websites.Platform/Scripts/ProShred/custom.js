var ProshredCustomers = {};
ProshredCustomers.Building = "";
var _ResponseARCode = "";
var _ResponseNewCustomerID = "";
var _ResponseNewBuildingID = "";
var objEZShredDataModel = new Object();
var _EzsharedData = {};
var _opportunity = "";
var ValidateEmaildAddressResponse = false;
var _hasSharpspringLead;
var Lead_CompanyName = "";
_EzsharedData.EzshredPort = "";
_EzsharedData.EzshredIP = "";
_EzsharedData.EzshredAPIURL = "";
_EzsharedData.EzshredUserID = "";
_EzsharedData.AccountObjectID = "";
_EzsharedData.selectedValue = "";
var _buildingStage = "";
var _isAddNewBuilding = false;
//Customer Variables
var companyName = attention = Suite = street = city = state = zip = first = last = phone = mobile = fax = CustomerType = InvoiceType = "";
var email = email2 = email3 = emailInvoice = emailCOD = refSource = terms = notes = Datasource = "";
//Building Variables
var BuildingType = BuildingName = BuildingStreet = BuildingSuite = BuildingCity = BuildingState = BuildingZip = BuildingPhone = BuildingPhone2 = "";
var Directions = RouteInstructions = RouteInstructions = ScheduleFrequency = ServiceType = BldgContact1 = BldgContact2 = SalesmanID = "";
var extensionAppendText = " X";
var DropDownDefaultTextEnum = [
    { "fieldName": "ServiceType", "defaultValue": "On-Site" },
    { "fieldName": "BuildingType", "defaultValue": "Default" },
    { "fieldName": "ScheduleFrequency", "defaultValue": "Purge" }];
var clicks = new Array();
$(document).ready(function () {
    $('#btnUpdate').hide();
    $('#lbl-oppourtunity-status-closed').addClass('disabled');
    $('#Datasource').trigger('change');
    //Prevent users from submitting a form by hitting Enter, Only Enter will work in textarea
    $(window).keydown(function (event) {
        var allowEnter = { "textarea": true };
        var nodeName = event.target.nodeName.toLowerCase();
        if (event.keyCode == 13 && allowEnter[nodeName] !== true) {
            event.preventDefault();
            return false;
        }
    });
    $('#btnAddBuilding, #btnCopyToClipboard').hide();
    setPipeline(PipelineEnum[0]);//Default pipeline will LEAD
});
ProshredCustomers.getAllTypes = function () {
    $.ajax({
        type: "post",
        url: "/GetAllTypes",
        data: { "accountObjectID": _EzsharedData.AccountObjectID },
        async: true,
        cache: false,
        success: function (response) {
            if (response.Data !== undefined) {
                //Industry Type
                $('#IndustryType, #InvoiceType, #TermsDropDown, #ReferralSourceDropDown, #BuildingType, #ServiceType, #ScheduleFrequency, #Salesman').empty();
                $.each(response.Data.CustomerTypes, function (key, value) {
                    $("#IndustryType").append('<option value="' + value.CustomerTypeID + '">' + value.CustomerType + '</option>');
                });
                //invoice Type
                $.each(response.Data.InvoiceTypes, function (key, value) {
                    $("#InvoiceType").append('<option value="' + value.InvoiceTypeID + '">' + value.InvoiceType + '</option>');
                });
                //Terms
                $.each(response.Data.TermTypes, function (key, value) {
                    $("#TermsDropDown").append('<option value="' + value.TermID + '">' + value.Terms + '</option>');
                });
                //Referral Source
                $.each(response.Data.ReferralSources, function (key, value) {
                    $("#ReferralSourceDropDown").append('<option value="' + value.ReferralSourceID + '">' + value.ReferralSource + '</option>');
                });
                //Building Type
                $.each(response.Data.BuildingTypes, function (key, value) {
                    $("#BuildingType").append('<option value="' + value.BuildingTypeID + '">' + value.BuildingType + '</option>');
                });
                //Service Type
                $.each(response.Data.ServiceTypes, function (key, value) {
                    $("#ServiceType").append('<option value="' + value.ServiceTypeID + '">' + value.ServiceType + '</option>');
                });
                //Frequency
                $.each(response.Data.Frequencys, function (key, value) {
                    $("#ScheduleFrequency").append('<option value="' + value.ScheduleFrequency + '">' + value.Frequency + '</option>');
                });
                //Salesman
                if (response.Data != null && response.Data.Salesman != null) {
                    $.each(response.Data.Salesman, function (key, value) {
                        $("#Salesman").append('<option value="' + value.SalesmanID + '">' + value.Salesman + '</option>');
                    });
                }

                $("#IndustryType").prepend("<option value=''  selected='selected'>Select</option>");
                $("#InvoiceType").prepend("<option value=''  selected='selected'>Select</option>");
                $("#TermsDropDown").prepend("<option value=''  selected='selected'>Select</option>");
                $("#ReferralSourceDropDown").prepend("<option value=''  selected='selected'>Select</option>");
                $("#BuildingType").prepend("<option value=''  selected='selected'>Select</option>");
                $("#ServiceType").prepend("<option value=''  selected='selected'>Select</option>");
                $("#ScheduleFrequency").prepend("<option value=''  selected='selected'>Select</option>");
                $("#Salesman").prepend("<option value=''  selected='selected'>Select</option>");
                //Set Default value in dropdown
                $.each(DropDownDefaultTextEnum, function (i, item) {
                    setDefaultSelectItem(item.fieldName, item.defaultValue);
                });
            }
            if (!response.SSCustomFieldAvailability)
                warningmessage("Custom fields are not mapped in CRM", "error");
        }
    });
};
$(document).ready(function () {
    var searchText = "";
    var searchDataConfig = {
        //searching: "Loading.....",
        //loadingMore: "Loading.....",
        allowClear: true,
        placeholder: "Search for Customer...",
        minimumInputLength: 2,
        ajax: {
            dataType: 'json',
            type: "post",
            url: "/SearchCustomers",
            data: function (search) {
                return {
                    "accountObjectID": _EzsharedData.AccountObjectID,
                    "searchText": search.toUpperCase(),
                }
            },
            results: function (data) {
                return {
                    results: dataFormatting(data.customer)
                };
            }
        },
        formatResult: function (option) {
            return "<div>" + option.desc + "</div>";
        },
        formatSelection: function (option) {
            $('#dvdata_processing').fadeIn(100, function () {
                GetDataOnCustomerSelection(option.leadid, option.opportunityid, option.customerid, option.buildingid);
            });
            return option.desc;
        }
    };

    function dataFormatting(data) {
        var foundOptions = [];
        var mockData = { id: '', desc: '', leadid: '', opportunityid: '', customerid: '', buildingid: '' };
        $.each(data, function (key, value) {
            mockData = { id: key, desc: value.Company + " " + $.trim(value.BuildingName) + " " + $.trim(value.Street) + " " + $.trim(value.Zip), leadid: value.LeadID, opportunityid: value.OpportunityID, customerid: value.CustomerID, buildingid: value.BuildingID };
            foundOptions.push(mockData);
        });
        return foundOptions;
    };
    $("#hdnCustomerSearch").select2(searchDataConfig);
    $("#hdnCustomerSearch").on("change", function (e) {
        if (e.removed) {
            document.getElementById("CustomerForm").reset();
            $('#CustomerID, #LeadID, #OpportunityID').val("");
            clearAllValidationErrorMessage();//Remove all validation msg
            clicks = [];//Empty previous pipeline selection 
            $("#availDatesList").empty();//Empty available Date list
            setPipeline(PipelineEnum[0]);//After clear dropdown, default pipeline will LEAD
            _opportunity = "";
            //Hide show buttons
            $('#btnUpdate').hide();
            $('#btnAddNewCustomer').show();
            $('#btnAddBuilding, #TaxStatus, #btnCopyToClipboard').hide();
        }
    });

});
function GetDataOnCustomerSelection(leadID, opportunityID, customerID, buildingID) {
    var _leadID = _opportunityID = _customerID = _buildingID = null;
    _leadID = leadID;
    _opportunityID = opportunityID;
    _customerID = customerID;
    _buildingID = buildingID;
    $('#btnGroup').find('label').removeClass('active')
        .end().find('[type="radio"]').prop('checked', false);
    $('#CustomerID').val(_customerID);
    $('#OpportunityID').val(_opportunityID);
    $('#LeadID').val(_leadID);
    $("#copyOkIcon").removeClass("glyphicon-ok");
    $("#copyOkIcon").addClass("glyphicon-paperclip");
    var _buildings = "";
    _isAddNewBuilding = false;
    //console.log(_opportunityID);
    //console.log(_leadID);
    //console.log(_customerID);
    if (_opportunityID != null && _leadID != null && _customerID != null) {
        $.ajax({
            type: "post",
            data: {
                "accountObjectID": _EzsharedData.AccountObjectID,
                'CustomerID': _customerID,
                'leadId': _leadID,
                'opportunityId': _opportunityID,
                'buildingId': _buildingID,
            },
            url: "/GetCustomerAndBuildingInformations",
            async: false,
            cache: false,
            success: function (response) {
                //console.log(response.BuildingCount);
                //console.log(response.BuildingSet);
                if (response.BuildingCount >= 5 && response.BuildingSet == "Complete" && (response.Opportunity == null || response.Opportunity.OpportunityID == 0)) {
                    $("#dvdata_processing").fadeOut();
                    $('.select2-container').select2('data', null).trigger('change');
                    WarningAlert("Failed", "You can't add more than 5 building on CRM");
                    return;
                }

                clearAllValidationErrorMessage();//Remove all validation msg
                if (response.Data !== undefined) {
                    _buildings = response.Data;
                    _opportunity = response.Opportunity;
                    _buildingStage = response.BuildingSet;
                    if (_buildingStage != "Complete" && response.BuildingCount < 5)//Complete- You can't add more building
                    {
                        $('#btnAddBuilding').show();
                        $('#BuildingSet').val(response.BuildingSet);
                    }
                    $('#btnUpdate').show();
                    $('#btnAddNewCustomer').hide();

                    if (_opportunity != null) {//If opportunity status was lost, default pipeline status will not bind
                        if (response.DealStageName === PipelineEnum[1]) {
                            setPipeline(PipelineEnum[1]);
                            //$("#option2").click();
                        }
                        else if (response.DealStageName === PipelineEnum[2]) {
                            setPipeline(PipelineEnum[2]);
                            //$("#option3").click();
                        }
                        else if (response.DealStageName === PipelineEnum[3]) {
                            setPipeline(PipelineEnum[3]);
                            //$("#option4").click();
                        }
                        else {
                            setPipeline(PipelineEnum[1]);
                            //$("#option1").click();//Default Lead click
                        }
                        _buildings.Customer[0].PipelineStatus = response.DealStageName;//Default Lead assign
                    }
                    else {
                        //Default Pipeline Selection as LEAD		
                        setPipeline(PipelineEnum[0]);
                    }
                }
                $("#dvdata_processing").fadeOut();
            },
            error: function (error) {
                $("#dvdata_processing").fadeOut();
            }
        });
        var form = document.getElementById("CustomerForm");
        if (_buildings != '' && typeof _buildings != 'undefined' && _buildings.Customer.length !== 0 && _buildings.Building.length !== 0) { // Added this because exception is caused when resetting the form. 
            ProshredCustomers.PopulateFormFields(_buildings.Customer[0], _buildings.Building[0], form);
            window.newCustomer = false;
        }
        else {
            //here is where clicking the 'x' the form resets... need to show the add new customer button and manually hide the tax/credit hold flags. 
            //var selectedVal = $(".chzn-select").val();
            var datasource = $('#Datasource').val();
            form.reset();
            //$(".chzn-select").val(selectedVal);
            $('#Datasource').val(datasource);
            $("#CreditStatus").hide();
            $("#TaxStatus").hide();
            $('input[name=status][value=Lead]').click();

            window.newCustomer = true;
            $("#availDatesList").empty();
            $("#btnAddNewCustomer").show();
            $("#btnUpdate").hide();
        }
        if (_opportunity !== undefined) {
            ProshredCustomers.PopulateOpportunityFormFields(_opportunity, form)
        }
    }
    clicks = [];//Empty previous pipeline selection 
    $("#availDatesList").empty();
    $("#Zip").change();
}
ProshredCustomers.PopulateCustomers = function (customers) {
    $('.chzn-select').empty();
    var facb = '';
    var lazyList = '';
    $.each(customers, function (i, customer) {
        if (i > 10) {
            lazyList += '<option value="' + i + '" data-source="' + customer.DataSource + '"data-customerid="' + customer.CustomerID + '" data-leadid="' + customer.LeadID + '" data-opportunityid="' + customer.OpportunityID + '">' + customer.Company + "  " + customer.Street + "  " + customer.Zip + '</option>';
        }
        else {
            facb += '<option value="' + i + '" data-source="' + customer.DataSource + '"data-customerid="' + customer.CustomerID + '" data-leadid="' + customer.LeadID + '" data-opportunityid="' + customer.OpportunityID + '">' + customer.Company + "  " + customer.Street + "  " + customer.Zip + '</option>';
        }
    });
    $.when($('.chzn-select').append(facb)).done(function () {
        if (facb.length <= 0) {
            facb = '<option onclick="resetForm()" selected>' + "Sorry! " + "Data not available for " + $('#Datasource').val() + " marketplace" + '</option>';
        } else {
            facb = '<option onclick="$(\'#CustomerForm\')[0].reset()" value="" selected>Select a Customer</option>';
        }
        //$('.chzn-select').prepend(facb).trigger('change');		
        $('.chzn-select').prepend(facb).trigger("select:updated").select2("open");
    });
    if (lazyList != '') {
        setTimeout(function () {
            $('.chzn-select').append(lazyList).trigger("select:updated").select2("open");
        }, 1000)
    }
};
ProshredCustomers.CustomCustomerDropdown = function () {
    $(".chzn-select").select2({
        width: "100%",
        searching: "Loading.....",
        loadingMore: "Loading.....",
        minimumInputLength: 0,
        search_contains: true,
        allowClear: true,
        formatNoMatches: function (term) {
            return "<div class='select2-result-label'><span class='select2-match'></span>Other</div>"
        }
    });
};
ProshredCustomers.getBuildingListByCustomer = function () {
    $.ajax({
        type: "post",
        url: "/GetBuildingListByCustomer",
        async: true,
        cache: false,
        success: function (response) {
            if (response.Data !== undefined) {
                console.log('Response Data');
                console.log(response.Data.Building);
            }
        }
    });
};
ProshredCustomers.FieldMapping = function () {
    //Customer Fields
    companyName = $("#CompanyName").val();
    attention = $("#Attention").val();
    Suite = $("#Suite").val();
    street = $("#Street").val();
    city = $("#City").val();
    state = $("#State").val();
    zip = $("#Zip").val();
    first = $("#ContactFName").val().trim();
    last = $("#ContactLName").val().trim();
    phone = setExtension($("#ContactPhone").val(), $('#CompanyPhoneExtension').val());
    mobile = $("#MobileNumber").val();//needs concatenated onto phone, wasnt working last time i tried...
    fax = $("#Fax").val();
    CustomerType = $("#IndustryType option:selected").val();
    InvoiceType = $("#InvoiceType option:selected").val();
    email = $("#Email").val();
    email2 = $("#Email_2").val() || "";
    email3 = $("#Email_3").val() || "";

    if ($("#Email_2").val() != null)
        email = email + ";" + email2;

    if ($("#Email_3").val() != null)
        email = email + ";" + email3;

    emailInvoice = $("#EmailInvoices").prop('checked') === true;
    emailCOD = $("#EmailCOD").prop('checked') === true;
    refSource = $("#ReferralSourceDropDown option:selected").val();
    terms = $("#TermsDropDown option:selected").val();
    notes = $("#Notes").val();
    objEZShredDataModel.Customer = [{
        "CustomerID": "",
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
        "ReferralSourceID": refSource,
        "TermID": terms,
        //custom fields:
        "PipelineStatus": $("#btnGroup input:checked").val(),
        "NumberOfEmployees": $("#NumberOfEmployees").val(),
        "Mobile": mobile,
        "UserId": _EzsharedData.EzshredUserID,
        "Datasource": _EzsharedData.selectedValue,
        "BillingContact": $("#BillingContactFirstName").val().trim() + " " + $("#BillingContactLastName").val().trim(),
        "BillingContactPhone": setExtension($("#BillingContactPhone").val(), $('#BillingContactExtension').val()),
        "BillingContactExtension": $("#BillingContactExtension").val(),
        "BillingCountryCode": $("#BillingCountryCode").val(),
        "TravelTourismType": $("#TravelTourismType option:selected").val(),
        "ServicesProfessionalType": $("#ServicesProfessionalType option:selected").val()
    }];
    //Building Fields
    BuildingType = $("#BuildingType option:selected").val();
    BuildingName = $("#BuildingName").val();
    BuildingStreet = $("#BuildingStreet").val();
    BuildingSuite = $("#BuildingSuite").val();
    BuildingCity = $("#BuildingCity").val();
    BuildingState = $("#BuildingState").val();
    BuildingZip = $("#BuildingZip").val();
    BuildingPhone = setExtension($("#BuildingPhone").val(), $('#BuildingPhone1Extension').val());
    BuildingPhone2 = setExtension($("#BuildingPhone2").val(), $('#BuildingPhone2Extension').val());
    Directions = $("#Directions").val();
    RouteInstructions = $("#RouteInstructions").val();
    ScheduleFrequency = $("#ScheduleFrequency option:selected").val();
    ServiceType = $("#ServiceType option:selected").val();
    BldgContact1 = $("#BldgContactFName").val() + " " + $('#BldgContactLName').val();
    BldgContact2 = $("#BldgContactFName2").val().trim() + " " + $('#BldgContactLName2').val().trim();
    SalesmanID = $("#Salesman option:selected").val();
    objEZShredDataModel.Building = [{
        "CustomerID": "",
        "BuildingID": "",
        "CompanyName": BuildingName,
        "BuildingTypeID": BuildingType,
        "Suite": BuildingSuite,
        "Street": BuildingStreet,
        "City": BuildingCity,
        "State": BuildingState,
        "Zip": BuildingZip,
        "Phone1": BuildingPhone,
        "Phone2": BuildingPhone2,
        "SalesmanID": SalesmanID,
        "ScheduleFrequency": ScheduleFrequency,
        "SiteContact1": BldgContact1.trim(),
        "SiteContact2": BldgContact2,
        "RouteID": "0",
        "Stop": "99",
        "TimeTaken": "15",
        "ServiceTypeID": ServiceType,
        "Directions": Directions,
        "RoutineInstructions": RouteInstructions,
        "SalesTaxRegionID": 0,
        "UserId": _EzsharedData.EzshredUserID,
        "CompanyCountryCode": $('#CompanyCountryCode').val(),
        "TaxExempt": $("#TaxExempt").prop('checked') === true,
    }];
    objEZShredDataModel.SSOpportunity = [{
        "Amount": Math.round($('#EstimatedValue').val()),//Estimated value associated with Amount
        "AdditionalTips": $('#AdditionalTips').val(),
        "JobQuantitySize": $('#JobQuantitySize').val(),
        "ProposedDateOfService": $("#ProposedDateOfService").val(),
        "CertificateOfInsurance": $("input[name='CertificateofInsurance']:checked").val(),
        "HoursOfBusiness": $("#HoursOfBusiness").val(),
        "Stairs": $("input[name='Stairs']:checked").val(),
        "ProposedConsoleDeliveryDate": $("#ProposedConsoleDeliveryDate").val(),
        "ProposedStartDate": $("#ProposedStartDate").val(),
        "ECUnits": $("#ECUnits").val(),
        "ECPriceUnit": $("#ECPriceUnit").val(),
        "ECAdditionalPrice": $("#ECAdditionalPrice").val(),
        "GallonUnits_64": $("#GallonUnits_64").val(),
        "GallonPriceUnit_64": $("#GallonPriceUnit_64").val(),
        "GallonAdditionalPrice_64": $("#GallonAdditionalPrice_64").val(),
        "GallonUnits_96": $("#GallonUnits_96").val(),
        "GallonPriceUnit_96": $("#GallonPriceUnit_96").val(),
        "GallonAdditionalPrice_96": $("#GallonAdditionalPrice_96").val(),
        "NumberOfTips": $("#NumberOfTips").val(),
        "PriceQuotedForFirstTip": $("#PriceQuotedForFirstTip").val(),//PriceQuotedForFirstTip
        "BankerBoxes": $("#BankerBoxes").val(),
        "FileBoxes": $("#FileBoxes").val(),
        "Bags": $("#Bags").val(),
        "Cabinets": $("#Cabinets").val(),
        "Skids": $("#Skids").val(),
        "HardDrivers": $("#HardDrivers").val(),
        "Media": $("#Media").val(),
        "Other": $("#Other").val(),
        "SellerComments": $("#SellerComments").val(),
        "HardDrive_Media_Other_Comment": $("#HardDrive_Media_Other_Comment").val(),
        "BuildingContactFirstName": $("#BldgContactFName").val(),
        "BuildingContactLastName": $("#BldgContactLName").val(),
        "BuildingContactPhone": setExtension($("#BuildingPhone").val(), $('#BuildingPhone1Extension').val()),
        "BuildingName": $("#BuildingName").val(),
        "BuildingStreet": $("#BuildingStreet").val(),
        "BuildingCity": $("#BuildingCity").val(),
        "BuildingState": $("#BuildingState").val(),
        "ZipCode": $("#BuildingZip").val(),
    }];
}
ProshredCustomers.AddNewCustomerAndBuildingInDb = function (_customerBuildingDataModel) {
    var selectedPipeine = _customerBuildingDataModel.Customer[0].PipelineStatus;
    $.ajax({
        type: "post",
        data: {
            'accountObjectID': _EzsharedData.AccountObjectID,
            'objEZShredDataModel': _customerBuildingDataModel
        },
        url: "/AddCustomer",
        async: false,
        cache: false,
        success: function (response) {
            //EZShred Response Only for Won/Not Schedule
            if (selectedPipeine == PipelineEnum[3]) {
                if (response.EZSharedStatus)
                    warningmessage("Data save successfully in EZShared", "success");
                else if (!response.EZSharedStatus)
                    warningmessage("Data not added in EZShared", "error");
            }
            //Sharpspring Response
            if (response.SharpSpringStatus)
                warningmessage("Data added on CRM", "success");
            else if (!response.SharpSpringStatus)
                warningmessage("Data not added on CRM", "error");
            //Any Error or Exception Occour
            if (response.Message != "") {
                warningmessage(response.Message, "error");
            }
        }
    });
};
ProshredCustomers.PopulateFormFields = function (customer, building, form) {
    form.reset();
    $('#Datasource').val(_EzsharedData.selectedValue);
    var emails = [];
    var contactName = []; //have to separate the first and last name...    
    var billingContactName = [];
    if (customer.EmailAddress != null) {
        if (customer.EmailAddress.includes(";"))
            emails = customer.EmailAddress.split(";");
    }
    $('#divEmail_2, #divEmail_3').remove();
    if (emails.length > 1) {
        form.Email.value = emails[0];
        for (var x = 2; x <= emails.length; x++) {
            if (emails[x - 1] != "") {
                $(".input_fields_wrap").append('<div class="form-group" id="divEmail_' + x + '"><div class="input-group"><input class="form-control" type="text" name="Email[' + x + ']"  id="Email_' + x + '"/><span class="input-group-btn"> <button class="btn btn-primary remove_field" type="button">x</button></span></div></div>'); //add input box  
                $('#Email_' + x + '').val(emails[x - 1]);
            }
        }
    } else {
        form.Email.value = customer.EmailAddress;
    }

    if (customer.CreditHold === "True") {
        $('#CreditStatus').show();
    } else {
        $("#CreditStatus").hide();
    }
    form.NumberOfEmployees.value = customer.NumberOfEmployees;
    if (customer.ARCustomerCode != "")
        $('#btnCopyToClipboard').show();
    else
        $('#btnCopyToClipboard').hide();

    form.ARCustomerCode.value = customer.ARCustomerCode;
    form.CompanyName.value = customer.Company;
    form.Attention.value = customer.Attention;
    form.ContactFName.value = checkEmptyString(customer.Contact.substring(0, customer.Contact.indexOf(" ")));
    form.ContactLName.value = checkEmptyString(customer.Contact.substring(customer.Contact.indexOf(" ") + 1, customer.Contact.length));
    form.ContactPhone.value = getExtensionAndNumber(customer.Phone)[0];
    form.CompanyPhoneExtension.value = getExtensionAndNumber(customer.Phone)[1];
    form.MobileNumber.value = customer.Mobile;
    form.Fax.value = customer.Fax;

    if (customer.EmailInvoice === "true")
        form.EmailInvoices.checked = true;
    else
        form.EmailInvoices.checked = false;

    if (customer.EmailCOD === "true")
        form.EmailCOD.checked = true;
    else
        form.EmailCOD.checked = false;

    if (customer.Street != null) {
        var detachedAddress = detachStreetSuite(customer.Street);
        form.Suite.value = detachedAddress[0];
        form.Street.value = detachedAddress[1];
    }

    form.City.value = customer.City;
    form.State.value = customer.State;
    form.Zip.value = customer.Zip;
    form.Notes.value = customer.Notes;
    form.InvoiceType.value = customer.InvoiceTypeID;
    form.IndustryType.value = customer.CustomerTypeID;
    form.TermsDropDown.value = customer.TermID;
    form.ReferralSourceDropDown.value = customer.ReferralSourceID;
    form.TravelTourismType.value = customer.TravelTourismType;
    form.ServicesProfessionalType.value = customer.ServicesProfessionalType;
    if (customer.BillingContact != undefined && customer.BillingContact != null) {
        form.BillingContactFirstName.value = checkEmptyString(customer.BillingContact.substring(0, customer.BillingContact.indexOf(" ")));
        form.BillingContactLastName.value = checkEmptyString(customer.BillingContact.substring(customer.BillingContact.indexOf(" ") + 1, customer.BillingContact.length));
    }
    form.BillingContactPhone.value = getExtensionAndNumber(customer.BillingContactPhone)[0];
    form.BillingContactExtension.value = getExtensionAndNumber(customer.BillingContactPhone)[1];
    form.BillingCountryCode.value = customer.BillingCountryCode == "" ? customer.BillingCountryCode : "US";
    if (building != undefined) {
        form.BuildingID.value = building.BuildingID;
        form.BuildingType.value = building.BuildingTypeID;
        form.ServiceType.value = building.ServiceTypeID;
        form.ScheduleFrequency.value = building.ScheduleFrequency;
        form.NextServiceDate.value = dateFormat(building.NextServiceDate);
        form.BuildingName.value = building.CompanyName;
        form.Directions.value = building.Directions;
        form.RouteInstructions.value = building.RoutineInstructions;

        if (building.Street != null) {
            var detachedAddress = detachStreetSuite(building.Street);
            form.BuildingSuite.value = detachedAddress[0];
            form.BuildingStreet.value = detachedAddress[1];
        }

        if (building.TaxExempt === "true")
            form.TaxExempt.checked = true;
        else
            form.TaxExempt.checked = false;

        form.BuildingCity.value = building.City;
        form.BuildingState.value = building.State;
        form.BuildingZip.value = building.Zip;
        form.BuildingID.value = building.BuildingID;
        form.ServiceType.value = building.ServiceTypeID;
        form.LastSvcDate.value = dateFormat(building.LastServiceDate);
        form.BuildingPhone.value = getExtensionAndNumber(building.Phone1)[0];
        form.BuildingPhone1Extension.value = getExtensionAndNumber(building.Phone1)[1];
        form.BuildingPhone2.value = getExtensionAndNumber(building.Phone2)[0];
        form.BuildingPhone2Extension.value = getExtensionAndNumber(building.Phone2)[1];
        form.Salesman.value = building.SalesmanID;
        form.CompanyCountryCode.value = building.CompanyCountryCode == "" ? customer.CompanyCountryCode : "US";
        if (building.SiteContact1 != undefined && building.SiteContact1 != null && building.SiteContact1 != "") {
            form.BldgContactFName.value = checkEmptyString(building.SiteContact1.substring(0, building.SiteContact1.indexOf(" ")));
            form.BldgContactLName.value = checkEmptyString(building.SiteContact1.substring(building.SiteContact1.indexOf(" ") + 1, building.SiteContact1.length));
        }
        if (building.SiteContact2 != undefined && building.SiteContact2 != null && building.SiteContact2 != "") {
            form.BldgContactFName2.value = checkEmptyString(building.SiteContact2.substring(0, building.SiteContact2.indexOf(" ")));
            form.BldgContactLName2.value = checkEmptyString(building.SiteContact2.substring(building.SiteContact2.indexOf(" ") + 1, building.SiteContact2.length));
        }
        if (building.SalesTaxRegionID === "0" || building.TaxExempt === "true")
            $('#TaxStatus').show();
        else
            $("#TaxStatus").hide();

        //custom fields:
        if (customer.PipelineStatus != PipelineEnum[4]) {
            if (customer.PipelineStatus == null || customer.PipelineStatus == "")
                $('input[name=status][value=Lead]').click();
            else
                $('input[name=status][value=' + customer.PipelineStatus + ']').click();
        }
    }
    else {
        warningmessage("Could not locate building information for this customer! Some fields may be blank...", "error");
    }
};
ProshredCustomers.PopulateOpportunityFormFields = function (data, form) {
    if (data != null) {
        $('#lbl-oppourtunity-status-closed').removeClass('disabled');
        form.AdditionalTips.value = data.AdditionalTips;
        form.JobQuantitySize.value = data.JobQuantitySize;
        form.PriceQuotedForFirstTip.value = data.PriceQuotedForFirstTip;
        form.ProposedDateOfService.value = data.ProposedDateOfService;
        form.HoursOfBusiness.value = data.HoursOfBusiness;
        form.ProposedConsoleDeliveryDate.value = data.ProposedConsoleDeliveryDate;
        form.ProposedStartDate.value = data.ProposedStartDate;
        form.ECUnits.value = data.ECUnits;
        form.ECPriceUnit.value = data.ECPriceUnit;
        form.ECAdditionalPrice.value = data.ECAdditionalPrice;
        form.GallonUnits_64.value = data.GallonUnits_64;
        form.GallonPriceUnit_64.value = data.GallonPriceUnit_64;
        form.GallonAdditionalPrice_64.value = data.GallonAdditionalPrice_64;
        form.GallonUnits_96.value = data.GallonUnits_96;
        form.GallonPriceUnit_96.value = data.GallonPriceUnit_96;
        form.GallonAdditionalPrice_96.value = data.GallonAdditionalPrice_96;
        form.NumberOfTips.value = data.NumberOfTips;
        form.EstimatedValue.value = data.Amount;
        form.BankerBoxes.value = data.BankerBoxes;
        form.FileBoxes.value = data.FileBoxes;
        form.Bags.value = data.Bags;
        form.Cabinets.value = data.Cabinets;
        form.Skids.value = data.Skids;
        form.HardDrivers.value = data.HardDrivers;
        form.Media.value = data.Media;
        form.Other.value = data.Other;
        form.SellerComments.value = data.SellerComments;
        form.HardDrive_Media_Other_Comment.value = data.HardDrive_Media_Other_Comment;
        $("input:radio[name='CertificateofInsurance'][value ='" + data.CertificateOfInsurance + "']").prop('checked', true);
        $("input:radio[name='Stairs'][value ='" + data.Stairs + "']").prop('checked', true);
    }
    else {
        $('#lbl-oppourtunity-status-closed').addClass('disabled');
        $('#lbl-oppourtunity-status-closed').removeClass('active');
    }
};
ProshredCustomers.UpdateCustomersOnServer = function () {
    var _laddaSpinner = Ladda.create(document.querySelector('.ladda-button'));
    var ARCustomerCode = $("#ARCustomerCode").val();
    var CustomerID = $('#CustomerID').val();
    var companyName = $("#CompanyName").val();
    var first = $("#ContactFName").val();
    var last = $("#ContactLName").val();
    var phone = setExtension($("#ContactPhone").val(), $('#CompanyPhoneExtension').val());
    var fax = $("#Fax").val();
    var attention = $("#Attention").val();
    var email = $("#Email").val();
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
    var BldgContact2 = $("#BldgContactFName2").val().trim() + " " + $('#BldgContactLName2').val().trim();
    var BuildingPhone = setExtension($("#BuildingPhone").val(), $('#BuildingPhone1Extension').val());
    var BuildingPhone2 = setExtension($("#BuildingPhone2").val(), $('#BuildingPhone2Extension').val());

    var SalesmanID = $("#Salesman option:selected").val();
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
    var OpportunityID = $('#OpportunityID').val();
    var LeadID = $('#LeadID').val();
    //var test = objectifyForm(fields);
    objEZShredDataModel.Customer = [{
        "ARCustomerCode": ARCustomerCode,
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
        "Mobile": mobile,
        "OpportunityID": OpportunityID,
        "LeadID": LeadID,
        "UserId": _EzsharedData.EzshredUserID,
        "Datasource": _EzsharedData.selectedValue,
        "BillingContact": $("#BillingContactFirstName").val().trim() + " " + $("#BillingContactLastName").val().trim(),
        "BillingContactPhone": setExtension($("#BillingContactPhone").val(), $('#BillingContactExtension').val()),
        "BillingContactExtension": $("#BillingContactExtension").val(),
        "BillingCountryCode": $("#BillingCountryCode").val(),
        "TravelTourismType": $("#TravelTourismType option:selected").val(),
        "ServicesProfessionalType": $("#ServicesProfessionalType option:selected").val()
    }];
    objEZShredDataModel.Building = [{
        "CustomerID": CustomerID,
        "BuildingID": BuildingID,
        "OpportunityID": OpportunityID,
        "CompanyName": BuildingName,
        "BuildingTypeID": BuildingType,
        "Suite": BuildingSuite,
        "Street": BuildingStreet,
        "City": BuildingCity,
        "State": BuildingState,
        "Zip": BuildingZip,
        "Phone1": BuildingPhone,
        "Phone2": BuildingPhone2,
        "SiteContact1": BldgContact1.trim(),
        "SiteContact2": BldgContact2,
        "ScheduleFrequency": ScheduleFrequency,
        "ServiceTypeID": ServiceType,
        "Directions": Directions,
        "SalesmanID": SalesmanID,
        "RoutineInstructions": RouteInstructions,
        "UserId": _EzsharedData.EzshredUserID,
        "CompanyCountryCode": $('#CompanyCountryCode').val(),
        "RouteID": "0",
        "Stop": "99",
        "TimeTaken": "15",
        "SalesTaxRegionID": 0,
        "TaxExempt": $("#TaxExempt").prop('checked') === true,
    }];
    objEZShredDataModel.SSOpportunity = [{
        "Amount": Math.round($('#EstimatedValue').val()),
        "AdditionalTips": $('#AdditionalTips').val(),
        "JobQuantitySize": $('#JobQuantitySize').val(),
        "ProposedDateOfService": $("#ProposedDateOfService").val(),
        "CertificateOfInsurance": $("input[name='CertificateofInsurance']:checked").val(),
        "HoursOfBusiness": $("#HoursOfBusiness").val(),
        "Stairs": $("input[name='Stairs']:checked").val(),
        "ProposedConsoleDeliveryDate": $("#ProposedConsoleDeliveryDate").val(),
        "ProposedStartDate": $("#ProposedStartDate").val(),
        "ECUnits": $("#ECUnits").val(),
        "ECPriceUnit": $("#ECPriceUnit").val(),
        "ECAdditionalPrice": $("#ECAdditionalPrice").val(),
        "GallonUnits_64": $("#GallonUnits_64").val(),
        "GallonPriceUnit_64": $("#GallonPriceUnit_64").val(),
        "GallonAdditionalPrice_64": $("#GallonAdditionalPrice_64").val(),
        "GallonUnits_96": $("#GallonUnits_96").val(),
        "GallonPriceUnit_96": $("#GallonPriceUnit_96").val(),
        "GallonAdditionalPrice_96": $("#GallonAdditionalPrice_96").val(),
        "NumberOfTips": $("#NumberOfTips").val(),
        "PriceQuotedForFirstTip": $("#PriceQuotedForFirstTip").val(),
        "BankerBoxes": $("#BankerBoxes").val(),
        "FileBoxes": $("#FileBoxes").val(),
        "Bags": $("#Bags").val(),
        "Cabinets": $("#Cabinets").val(),
        "Skids": $("#Skids").val(),
        "HardDrivers": $("#HardDrivers").val(),
        "Media": $("#Media").val(),
        "Other": $("#Other").val(),
        "SellerComments": $("#SellerComments").val(),
        "HardDrive_Media_Other_Comment": $("#HardDrive_Media_Other_Comment").val(),
        "BuildingContactFirstName": $("#BldgContactFName").val(),
        "BuildingContactLastName": $("#BldgContactLName").val(),
        "BuildingContactPhone": setExtension($("#BuildingPhone").val(), $('#BuildingPhone1Extension').val()),
        "BuildingName": $("#BuildingName").val(),
        "BuildingStreet": $("#BuildingStreet").val(),
        "BuildingCity": $("#BuildingCity").val(),
        "BuildingState": $("#BuildingState").val(),
        "ZipCode": $("#BuildingZip").val()
    }];
    //Update on DB
    ProshredCustomers.UpdateCustomerInDb(objEZShredDataModel);

};
ProshredCustomers.UpdateCustomerInDb = function (objEZShredDataModel) {
    var _laddaSpinner = Ladda.create(document.querySelector('.ladda-button'));
    var PipelineStatus = $("#btnGroup input:checked").val();
    $.ajax({
        type: "post",
        data: {
            'accountObjectID': _EzsharedData.AccountObjectID,
            'objEZShredDataModel': objEZShredDataModel,
            'IsAddNewBuilding': _isAddNewBuilding
        },
        url: "/UpdateCustomer",
        async: false,
        cache: false,
        success: function (response) {
            _laddaSpinner.stop();
            var datasource = $('#Datasource').val();
            $('#Datasource').val(datasource);
            $('#Datasource').trigger('change');
            if (response.WasAttemptedToWriteDataOnEZShred) {
                if (response.IsCustomerIDAssigned) {
                    if (response.EZSharedStatus)
                        warningmessage("Data update in EZShared", "success");
                    else if (!response.EZSharedStatus)
                        warningmessage("Data not update in EZShared", "error");
                }
                else
                    warningmessage("Data not update in EZShared", "error");
            }

            if (response.SharpSpringStatus)
                warningmessage("Data updated on CRM", "success");
            else if (!response.SharpSpringStatus)
                warningmessage("Data not updated on CRM", "error");

            if (response.Message != "")
                warningmessage(response.Message, "error");
        }
    });
};
ProshredCustomers.validateForm = function () {
    $("#CustomerForm").validate({
        invalidHandler: function (form, validator) {
            var errors = validator.numberOfInvalids();
            if (errors) {
                var firstInvalidElement = $(validator.errorList[0].element);
                $('html,body').scrollTop(firstInvalidElement.offset().top);
                firstInvalidElement.focus();
            }
        },
        rules: {
            "CompanyName": {
                required: true
            },
            "ContactFName": {
                required: true
            },
            "ContactLName": {
                required: true
            },
            "ContactPhone": {
                required: true
            },
            "Email[]": {
                required: true,
                email: true,
                regex: /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/
            },
            "Email[2]": {
                email: true,
                regex: /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/
            },
            "Email[3]": {
                email: true,
                regex: /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/
            },
            "Street": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "City": {
                required: true
            },
            "State": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "Zip": {
                required: true,
                minlength: 5
            },
            "IndustryType": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "InvoiceType": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "TermsDropDown": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "ReferralSourceDropDown": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BuildingName": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BldgContactFName": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BldgContactLName": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BuildingPhone": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BuildingStreet": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BuildingCity": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BuildingState": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BuildingZip": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; },
                minlength: 5
            },
            "status": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BuildingType": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "Salesman": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "ScheduleFrequency": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "ServiceType": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BillingContactFirstName": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BillingContactLastName": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BillingContactPhone": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "BillingCountryCode": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            },
            "CompanyCountryCode": {
                required: function () { if (PipelineEnum[3] == $("input[name='status']:checked").val() || $('#CustomerID').val() >= "0") return true; else return false; }
            }
        },
        messages: {
            "CompanyName": {
                required: "Please enter a company name"
            },
            "ContactFName": {
                required: "Please enter contact first name"
            },
            "ContactLName": {
                required: "Please enter contact last name"
            },
            "ContactPhone": {
                required: "Please enter contact phone number"
            },
            "Email[]": {
                required: "Please enter email id",
                email: "Email is invalid",
                regex: "Email is invalid"
            },
            "Email[2]": {
                email: "Email is invalid",
                regex: "Email is invalid"
            },
            "Email[3]": {
                email: "Email is invalid",
                regex: "Email is invalid"
            },
            "Street": {
                required: "Please enter street name"
            },
            "City": {
                required: "Please enter city name"
            },
            "State": {
                required: "Please enter State"
            },
            "Zip": {
                required: "Please enter Zip"
            },
            "IndustryType": {
                required: "Please select industry types"
            },
            "InvoiceType": {
                required: "Please select invoice types"
            },
            "TermsDropDown": {
                required: "Please select terms"
            },
            "ReferralSourceDropDown": {
                required: "Please select referral source"
            },
            "BuildingName": {
                required: "Please enter building name"
            },
            "BldgContactFName": {
                required: "Please enter contact first name"
            },
            "BldgContactLName": {
                required: "Please enter contact last name"
            },
            "BuildingPhone": {
                required: "Please enter phone number"
            },
            "BuildingStreet": {
                required: "Please enter street name"
            },
            "BuildingCity": {
                required: "Please enter city name"
            },
            "BuildingState": {
                required: "Please enter State"
            },
            "BuildingZip": {
                required: "Please enter Zip"
            },
            "status": {
                required: "Deal stages are required"
            },
            "BuildingType": {
                required: "Please select building type"
            },
            "Salesman": {
                required: "Please select account manager"
            },
            "ScheduleFrequency": {
                required: "Please select schedule frequency"
            },
            "ServiceType": {
                required: "Please select service type"
            },
            "BillingContactFirstName": {
                required: "Please enter billing first name"
            },
            "BillingContactLastName": {
                required: "Please enter billing last name"
            },
            "BillingContactPhone": {
                required: "Please enter billing phone number"
            },
            "BillingCountryCode": {
                required: "Please enter country"
            },
            "CompanyCountryCode": {
                required: "Please enter country"
            },
        },
        highlight: function (element) {
            $(element).closest('.form-group').addClass('is-error');
            $(element).closest('.form-control').addClass('error-control');
            if (element.type === 'radio') {
                $(element).closest('.form-group').addClass('is-error').append("<span class='is-error' style='display:block;'>Please select deal stage</span>");
            }
        },
        unhighlight: function (element) {
            if (element.type === 'radio') {
                $(element).closest('.form-group').removeClass('is-error');
            }
            $(element).closest('.form-group').removeClass('is-error');
            $(element).closest('.form-control').removeClass('error-control');
        },
        errorElement: 'span',
        errorClass: 'is-error',
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
            $('#CustomerForm').find("#status-error").remove();
        }
    });
}
ProshredCustomers.getAvailableDates = function (zip, callback) {
    $.ajax({
        type: "post",
        data: {
            'userId': _EzsharedData.EzshredUserID,
            'zipCode': zip,
            "accountObjectID": _EzsharedData.AccountObjectID
        },
        url: "/GetAvailableDates",
        async: true,
        cache: false,
        success: function (response) {
            var passed = response.status === "OK";
            if (passed) {
                callback(response.NextDatesByZip, response.status);
            }
            else { callback(null, response.status); }
        },
        error: function (error) {
            $("#availDatesList").empty();
            $("#availDatesList").append("<li style='color:red'><h5>Could not contact Ezshared API</h5></li>");
        }
    });
}
ProshredCustomers.getCustomerInfo = function (customerID, UserID, callback) {
    $.ajax({
        type: "post",
        url: _EzsharedData.EzshredAPIURL,
        data: JSON.stringify({ 'Request': JSON.stringify({ "Request": "GetCustomerInfo", "UserID": UserID, "CustomerID": customerID }) }),
        async: true,
        cache: false,
        success: function (response) {
            if (response.Success) {
                var resultJSON = JSON.parse(response.Result);//need to add check for invalid escapes!
                console.log(resultJSON);
                //if (window.newCustomer)
                callback(resultJSON.tblCustomers[0].ARCustomerCode);
                //else
                //getBuildingInfo(resultJSON.tblCustomers[0].CustomerID, callback);

            } else {
                console.log(response.FailureInformation);
            }
        }
    });
}
ProshredCustomers.checkSSCustomFields = function (callback) {
    $.ajax({
        type: "post",
        url: "/CheckSSCustomFields",
        data: { "accountObjectID": _EzsharedData.AccountObjectID },
        async: true,
        cache: false,
        success: function (response) {
            callback(response);
        }
    });
};
//Util Function
function setDefaultSelectItem(fieldName, defaultValue) {
    $("#" + fieldName + " option").each(function () {
        if ($(this).text() == defaultValue) {
            $("#" + fieldName + " option[text='selected']").prop("selected", false);//remove already selected item
            $(this).attr("selected", "selected");
        }
    });
}
function warningmessage(message, type) {
    new PNotify({
        text: message,
        type: type,
        styling: 'bootstrap3'
    });
}
function checkEmptyString(input) {
    return typeof input === "undefined" ? "" : input;
}
$.validator.addMethod(
    "regex",
    function (value, element, regexp) {
        var check = false;
        return this.optional(element) || regexp.test(value);
    },
    "Please check your input."
);
function copyToClipboard(text) {
    var copyARCode = $("#ARCustomerCode");
    copyARCode.select();

    try {
        var success = document.execCommand('copy');
        var msg = success ? 'successful' : 'unsuccessful';
        console.log("Copying text was " + msg);
        if (success) {
            $("#copyOkIcon").addClass("glyphicon-ok");
            $("#copyOkIcon").removeClass("glyphicon-paperclip");
        }

    } catch (err) {
        console.log("Could not copy AR Code...");
    }
}
function setExtension(phone, extension) {
    var _contactNumber = "";
    if (phone != "" && phone != null && phone != undefined) {
        if (extension == "" && extension == null && extension == undefined)
            _contactNumber = phone;
        else
            _contactNumber = phone + extensionAppendText + extension;
    }
    return _contactNumber.replace(/_/g, "");//remove _ from phone number while add/update
}
function getExtensionAndNumber(phone) {
    var extensionResult = [];
    phone = phone + "";//Number is coming(include/indexOf/search method) will not work
    if (phone != "") {
        if (phone.includes(extensionAppendText))
            extensionResult = phone.split(extensionAppendText);
        else {
            //If Phone have not extension and more than 10 character
            phone = phone.substring(0, 10) + extensionAppendText + phone.substring(10, phone.length);
            extensionResult = phone.split(extensionAppendText);
        }
    }
    else {
        //If phone number is not available.
        phone = phone + extensionAppendText;
        extensionResult = phone.split(extensionAppendText);
    }
    return extensionResult;
}
function dateFormat(inputDate) {
    var formatedDate = "None Scheduled";
    if (inputDate != "" && inputDate != null && inputDate != undefined) {
        try {
            formatedDate = new Date(inputDate);
            var dd = formatedDate.getDate();
            var mm = formatedDate.getMonth() + 1; //January is 0!
            var yyyy = formatedDate.getFullYear();
            if (dd < 10) {
                dd = '0' + dd
            }
            if (mm < 10) {
                mm = '0' + mm
            }
            formatedDate = mm + '/' + dd + '/' + yyyy;
        }
        catch (err) {
            formatedDate = "Not Available";
        }
    }
    return formatedDate;
}
function detachStreetSuite(address) {
    var detachedAddress = [];
    // These character will not removed from address: .'_# \s -
    address = address.replace(/\r?\n?[^a-z0-9.'_#\s-]/gi, '').replace(/[_\s]/g, ' ');
    address = address.replace('suite', ',').replace('Suite', ',');
    if (address.includes(",")) {
        detachedAddress[0] = address.substring(address.indexOf(",") + 1, address.length).trim();
        detachedAddress[1] = address.substring(0, address.indexOf(",")).trim();
    }
    else {
        detachedAddress[0] = "";
        detachedAddress[1] = address.trim();
    }
    return detachedAddress;
}
//Event Functions
$('#btnAddNewCustomer').click(function () {
    ProshredCustomers.validateForm();
    if (_hasSharpspringLead) {
        WarningAlert("", "<b>" + $('#Email').val() + "</b> already exists as a contact in the CRM. <br> To update this contact search for <b>" + Lead_CompanyName + "</b> in the search for customer field above and update the contact instead.");
    } else {
        if ($('#CustomerForm').valid() && ValidateEmaildAddressResponse) {
            var currentstate = this;
            var l;
            QuestionAlert("Add Customer", "Are you sure you want to add this customer for " + $('#Datasource').val() + " marketplace?", function () {
                l = InitializeLadda(l, currentstate);
                setTimeout(function () {
                    ProshredCustomers.FieldMapping();
                    ProshredCustomers.AddNewCustomerAndBuildingInDb(objEZShredDataModel);
                    l.stop();
                    var datasource = $('#Datasource').val();
                    $('#CustomerForm')[0].reset();
                    $('#Datasource').val(datasource);
                    $("#BuildingZip").change();
                    setPipeline(PipelineEnum[0]);//After Add new customer, default pipeline will LEAD
                }, 1000);
            }, function () {
            });
        }
        else { warningmessage("There are required fields that have been left blank.", "error"); }
    }
});
var PipelineEnum = [
    "Lead",
    "Contact",
    "QuoteSent",
    "Scheduled",
    "Lost",
];
function InitializeLadda(l, currentstate) {
    l = Ladda.create(currentstate);
    l.start();
    l.stop();
    l.toggle();
    l.isLoading();
    return l;
}
$('#Datasource').change(function () {
    $("#availDatesList").empty();
    //$('#btnGroup').find('label').removeClass('active')
    //    .end().find('[type="radio"]').prop('checked', false);

    //Default Pipeline Selection as LEAD
    clearAllValidationErrorMessage();//Remove all validation msg
    setPipeline(PipelineEnum[0]);
    $('#btnUpdate').hide();
    $('#btnAddNewCustomer').show();
    $('#btnAddBuilding, #TaxStatus, #btnCopyToClipboard').hide();//Hide AddNewBuilding,ARCustomer Copy button to  & TextExempt Green Button
    $("#hdnCustomerSearch").select2('data', null)
    var attribute = $(this).find('option:selected');
    if (attribute != null && attribute.length > 0) {
        _EzsharedData.AccountObjectID = attribute.attr("ObjectID");
        ProshredCustomers.checkSSCustomFields(function (result) {
            if (result) {
                $("#btnAddNewCustomer").prop('disabled', false);
                //$('.chzn-select').empty();
                _EzsharedData.EzshredPort = attribute.attr("port");
                _EzsharedData.EzshredIP = attribute.attr("ip");
                _EzsharedData.EzshredUserID = attribute.attr("id");
                _EzsharedData.AccountObjectID = attribute.attr("ObjectID");
                _EzsharedData.selectedValue = attribute.attr("value");
                _EzsharedData.EzshredAPIURL = "http://" + _EzsharedData.EzshredIP + ":" + _EzsharedData.EzshredPort + "/api/ezshred/getdata";
                document.getElementById("CustomerForm").reset();
                $('#Datasource').val(_EzsharedData.selectedValue);
                ProshredCustomers.getAllTypes();
            }
            else {
                $("#btnAddNewCustomer").css('cursor', 'pointer !important');
                $("#btnAddNewCustomer").prop('disabled', true);
                warningmessage("CRM Account is not configured properly for " + $('#Datasource').val() + " market place", "error");
            }
        });
    }
    else {
        $('#mainDiv').append("<div class='permissionOverlay'>You need at least one market assigned to you to access this form</div>");
    }
});
$("#billingCheckbox").change(function () {

    var checked = $(this)[0].checked === true;

    if (checked) {
        $("#BuildingSuite").val($("#Suite").val());
        $("#BuildingStreet").val($("#Street").val());
        $("#BuildingCity").val($("#City").val());
        $("#BuildingState").val($("#State").val());
        $("#BuildingZip").val($("#Zip").val());
        $("#CompanyCountryCode").val($("#BillingCountryCode").val());
        $("#BuildingZip").change();//Bind available Dates
    } else {
        $("#BuildingSuite").val("");
        $("#BuildingStreet").val("");
        $("#BuildingCity").val("");
        $("#BuildingState").val("");
        $("#BuildingZip").val("");
        $("#CompanyCountryCode").val("");
    }
});
$("#mainCheckbox").change(function () {
    var checked = $(this)[0].checked === true;
    if (checked) {
        $("#BillingContactFirstName").val($("#ContactFName").val());
        $("#BillingContactLastName").val($("#ContactLName").val());
        $("#BillingContactPhone").val($("#ContactPhone").val());
        $("#BillingContactExtension").val($("#CompanyPhoneExtension").val());

    } else {
        $("#BillingContactFirstName").val("");
        $("#BillingContactLastName").val("");
        $("#BillingContactPhone").val("");
        $("#BillingContactExtension").val("");
    }
});
function getAvailableDateOnZIP(chkName) {
    var BuildingZipVal = $('#BuildingZip').val();
    if ($('#Zip').val() != BuildingZipVal)
        uncheckCheckBox(chkName);

    if (BuildingZipVal.length >= 5) {
        $("#availDatesList").empty();
        $("#availDatesList").append("<li><h5>Getting available dates ... </h5></li>");
        ProshredCustomers.getAvailableDates(BuildingZipVal, function callback(dates, status) {
            $("#availDatesList").empty();
            if (dates !== null) {
                $.each(dates,
                    function (i, date) {
                        var spltDt = date.Date.split("-");
                        var newDate = spltDt[1] + "-" + spltDt[2] + "-" + spltDt[0].slice(-2);

                        $("#availDatesList").append("<li><h5>" + newDate + "</h5></li>");
                    });
            }
            else {
                $("#availDatesList").append("<li><h5>" + status + "</h5></li>");
            }
        });
    }
    else {
        $("#availDatesList").empty();
    }
};
$('#btnUpdate').on('click', function () {
    if (_hasSharpspringLead || _hasSharpspringLead == undefined) {
        $('#Email').trigger("change");//In case customer haven't modified emailfield'
    }
    $('#CustomerForm').submit();
    ProshredCustomers.validateForm();
    if ($('#CustomerForm').valid() && ValidateEmaildAddressResponse) {
        var currentstate = this;
        var l;
        QuestionAlert("Edit Customer", "Are you sure you want to edit this customer?", function () {
            l = InitializeLadda(l, currentstate);
            setTimeout(function () {
                ProshredCustomers.UpdateCustomersOnServer();
                l.stop();
                $('#btnAddBuilding, #TaxStatus, #btnCopyToClipboard').hide();
                setPipeline(PipelineEnum[0]);//After update new customer, default pipeline will LEAD
            }, 1000);
        }, function () {
        });
    }
    else {
        if (!_hasSharpspringLead)
            warningmessage("There are required fields that have been left blank.", "error");
    }
});
$("input[name='status']").change(function () {
    clearAllValidationErrorMessage();//Remove all validation msg
    if (PipelineEnum[0] == $("input[name='status']:checked").val() || PipelineEnum[1] == $("input[name='status']:checked").val() || PipelineEnum[2] == $("input[name='status']:checked").val()) {
        if ($('#CustomerID').val() >= "0") {
            $.each(ValidationList.EZShred, function (m, item) {
                $('#' + item).parent().find('.requiredLabel').addClass('conditional-requiredLabel').removeClass('.requiredLabel');
                $('.requiredLabel').removeClass('.requiredLabel');
                $('span.required').hide();
                $('.conditional-requiredLabel').parent().find('span.required').show();
            });
        }
        else {
            $.each(ValidationList.CRM, function (m, item) {
                $('#' + item).parent().find('.requiredLabel').addClass('conditional-requiredLabel').removeClass('.requiredLabel');
                $('.requiredLabel').removeClass('.requiredLabel');
                $('span.required').hide();
                $('.conditional-requiredLabel').parent().find('span.required').show();
            });
        }
    }
    else if (PipelineEnum[3] == $("input[name='status']:checked").val()) {
        $.each(ValidationList.EZShred, function (m, item) {
            $('#' + item).parent().find('.requiredLabel').addClass('conditional-requiredLabel').removeClass('.requiredLabel');
            $('.requiredLabel').removeClass('.requiredLabel');
            $('span.required').hide();
            $('.conditional-requiredLabel').parent().find('span.required').show();
        });
    }
    if (PipelineEnum[4] == $("input[name='status']:checked").val()) {
        if (_opportunity == null || _opportunity == "") {
            $("input[name='status'][value ='" + PipelineEnum[4] + "']").prop('checked', false);
            $("input[name='status'][value ='" + clicks[clicks.length - 1] + "']").prop('checked', true).parent().addClass('active');
            $('#lbl-oppourtunity-status-closed').removeClass('active');
        }
    } else {
        clicks[0] = $(this).val();
    }
});
$('#Email').change(function () {
    $.ajax({
        type: "post",
        data: { 'EmailAddress': $(this).val(), "accountObjectID": _EzsharedData.AccountObjectID },
        url: "/ValidateEmaildAddressForLead",
        async: false,
        cache: false,
        success: function (response) {
            ValidateEmaildAddressResponse = true;
            $('#EmailAddress-error').text('');
            if (response.Status && response.IsLeadInEZShredLeadMapping) {
                Lead_CompanyName = response.Data.CompanyName;
                _hasSharpspringLead = true;
                var SelectedleadID = $('#LeadID').val();
                if (SelectedleadID == response.Data.LeadID) {
                    ValidateEmaildAddressResponse = true;
                }
                else {
                    WarningAlert("", "<b>" + $('#Email').val() + "</b> already exists as a contact in the CRM. <br> To update this contact search for <b>" + Lead_CompanyName + "</b> in the search for customer field above and update the contact instead.");
                    ValidateEmaildAddressResponse = false;
                }
            }
            else if (response.Status && !response.IsLeadInEZShredLeadMapping) {
                Lead_CompanyName = response.Data.CompanyName;
                if ($('#LeadID').val() == "") {//Check For Add Customer Case
                    QuestionAlert("", "<b>" + $('#Email').val() + "</b> already exists as a contact in the CRM but is not present in EZSHRED.<br> To overwite this contact in the CRM and add it to EZSHRED click <b>Yes</b>. <br>Click on <b>No</b> if you want to change the email address.", function () {
                        ValidateEmaildAddressResponse = true;
                        _hasSharpspringLead = false;
                    }, function () {
                        $('#Email').val('');
                        _hasSharpspringLead = true;
                    });
                }
                else//Check for Update Customer
                {
                    WarningAlert("", "<b>" + $('#Email').val() + "</b> already exists as a contact in the CRM. <br> To update this contact search for <b>" + Lead_CompanyName + "</b> in the search for customer field above and update the contact instead.");
                    ValidateEmaildAddressResponse = false;
                }
            }
            else if (!response.Status) {
                _hasSharpspringLead = false;
            }
        }
    });
})
$('#btnAddBuilding').click(function () {
    QuestionAlert("Add Building", "Are you sure you want to add new building?", function () {
        if (_buildingStage != "Complete") {
            $('.building-box,.opportunity-box').find('input[type="text"]').val('');//Empty all input field
            $('.building-box,.opportunity-box').find('input[type="number"]').val('');//Empty all input number field
            $('.building-box,.opportunity-box').find('textarea').val('');//Empty all input number field
            $('.building-box,.opportunity-box').find('[type=checkbox]').prop('checked', false);//Empty all checkboxes
            $('#Salesman').val("");//set Default value
            $('#CompanyCountryCode').val("US");//Set Default value
            clearAllValidationErrorMessage();//Remove all validation msg
            $("#availDatesList").empty();//Empty available Date list
            clicks = [];//Empty previous pipeline selection 
            setPipeline(PipelineEnum[0]);//After Add new building, default pipeline will LEAD
            $('#BuildingID,#OpportunityID').val("");//set Default value
            _isAddNewBuilding = true;
        }

    }, function () {
    });
});
function uncheckCheckBox(chkName) {
    var checked = $("#" + chkName)[0].checked === true;
    if (checked)
        $("#" + chkName).attr("checked", false);
}
function setPipeline(paramPipeline) {
    //Unchecked pipeline status
    $('#lbl-oppourtunity-status-closed').removeClass('active').addClass('disabled');
    $('#btnGroup').find('label').removeClass('active')
        .end().find('[type="radio"]').prop('checked', false);
    //Default Pipeline Selection as LEAD
    $("input[name='status'][value ='" + paramPipeline + "']").prop('checked', true);
    $("input[name='status'][value ='" + paramPipeline + "']").closest('label').addClass('active');
    $("input[name='status'][value='" + paramPipeline + "']").click();
    $("input[name='status'][value='" + paramPipeline + "']").trigger('change');
}
function clearAllValidationErrorMessage() {
    $('.requiredLabel').removeClass('conditional-requiredLabel');
    $('.form-control').removeClass('error-control');//Remove all error control
    $('span.is-error').remove();//Remove all validation msg
    $('#EmailAddress-error').text('');
}
var ValidationList = {
    "CRM": ["CompanyName", "ContactFName", "ContactLName", "ContactPhone", "emails", "City", "Zip"],
    "EZShred": ["CompanyName", "ContactFName", "ContactLName", "ContactPhone", "emails", "BillingContactFirstName", "BillingContactLastName", "BillingContactPhone", "Street", "City", "State", "Zip", "BillingCountryCode", "IndustryType", "InvoiceType", "TermsDropDown", "ReferralSourceDropDown", "BuildingName", "BldgContactFName", "BldgContactLName", "BuildingPhone", "BuildingType", "Salesman", "BuildingStreet", "BuildingCity", "BuildingState", "BuildingZip", "CompanyCountryCode", "ScheduleFrequency", "ServiceType"]
};

