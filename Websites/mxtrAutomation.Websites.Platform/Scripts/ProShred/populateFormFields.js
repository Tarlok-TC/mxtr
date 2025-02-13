
function populateFormFields(customer, building, form) {
    alert(167);

    form.reset();

    var emails = [];
    var contactName = [];//have to separate the first and last name... 

    if (building.SalesTaxRegionID == 0)
        document.getElementById('TaxStatus').hidden = false;
    else
        document.getElementById('TaxStatus').hidden = true

    if (customer.EmailAddress != null || customer.EmailAddress != undefined) {
        if (customer.EmailAddress.includes(";"))
            emails = customer.EmailAddress.split(";");
    }
    if (customer.Notes != null || customer.Notes != undefined) {
        if (customer.Notes.includes("="))
            form.NumberOfEmployees.value = parseInt((customer.Notes).split("=")[1]);//will not work with no '='
    }

    if (emails.length > 1) {
        form.email1.value = emails[0];
        form.email2.value = emails[1];
        form.email3.value = emails[2];
    } else {
        form.email1.value = customer.EmailAddress;
    }

    if (customer.CreditHold == "True") {

        document.getElementById('CreditStatus').hidden = false;

    } else {

        document.getElementById('CreditStatus').hidden = true;

    }

    form.ARCustomerCode.value = customer.ARCustomerCode;
    form.CustomerID.value = customer.CustomerID;

    form.CompanyName.value = customer.Company;
    form.Attention.value = customer.Attention;
    form.NextServiceDate.value = building.NextServiceDate;
    contactName = customer.Contact.split(" ");

    form.ContactFName.value = contactName[0];
    form.ContactLName.value = contactName[1];
    form.ContactPhone.value = customer.Phone;
    form.MobileNumber.value = building.Phone1;

    form.EmailInvoices.checked = customer.EmailInvoice;
    form.Street.value = customer.Street;
    form.City.value = customer.City;
    form.State.value = customer.State;
    form.Zip.value = customer.Zip;
    form.Notes.value = customer.Notes;

    form.BuildingName.value = building.CompanyName;
    form.Directions.value = building.Directions;
    form.RouteInstructions.value = building.RoutineInstructions;
    form.BuildingStreet.value = building.Street;
    form.BuildingCity.value = building.City;
    form.BuildingState.value = building.State;
    form.BuildingZip.value = building.Zip;
    form.BuildingID.value = building.BuildingID;
    form.ServiceType.value = building.ServiceTypeID;
    form.LastSvcDate.value = building.LastServiceDate;

    form.InvoiceType.value = customer.InvoiceTypeID;
    form.IndustryType.value = customer.CustomerTypeID;
    form.TermsDropDown.value = customer.TermID;
    form.ReferralSourceDropDown.value = customer.ReferralSourceID;
    form.BuildingType.value = building.BuildingTypeID;
    form.ServiceType.value = building.ServiceTypeID;
    form.ScheduleFrequency.value = building.ScheduleFrequency;
}

