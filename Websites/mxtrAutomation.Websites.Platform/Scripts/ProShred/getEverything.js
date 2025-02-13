function getEverything(callback) {

    console.log("Starting my own *@*@*@ miner...");


    $.ajax({
        type: "post",
        url: "http://207.219.170.39:4550/api/ezshred/getdata",
        data: JSON.stringify({ 'Request': JSON.stringify({ "Request": "GetServiceItems" }) }),
        async: true,
        cache: false,
        success: function (response) {
            if (response.Success) {
                var serviceItems = JSON.parse(response.Result);//need to add check for invalid escapes! <- DONE

                console.log(serviceItems);
                callback(serviceItems);

            } else {
                console.log(response.FailureInformation);
            }
        }
    });

     //$.ajax({
     //    type: "post",
     //    url: "http://207.219.170.39:4550/api/ezshred/getdata",
     //    data: JSON.stringify({ 'Request': JSON.stringify({ "Request": "GetSalesTaxRegions" }) }),
     //    async: true,
     //    cache: false,
     //    success: function (response) {
     //        if (response.Success) {
     //            var taxRegions = JSON.parse(response.Result);//need to add check for invalid escapes! <- DONE

     //            console.log(taxRegions);
                 
     //        } else {
     //            console.log(response.FailureInformation);
     //        }
     //    }
     //});

    $.ajax({
        type: "post",
        url: "http://207.219.170.39:4550/api/ezshred/getdata",
        data: JSON.stringify({ 'Request': JSON.stringify({ "Request": "GetFrequency" }) }),
        async: true,
        cache: false,
        success: function (response) {
            if (response.Success) {
                var frequencies = JSON.parse(response.Result);//need to add check for invalid escapes! <- DONE

                console.log(frequencies);
                callback(frequencies);


            } else {
                console.log(response.FailureInformation);
            }
        }
    });

    $.ajax({
        type: "post",
        url: "http://207.219.170.39:4550/api/ezshred/getdata",
        data: JSON.stringify({ 'Request': JSON.stringify({ "Request": "GetReferralSource" }) }),
        async: true,
        cache: false,
        success: function (response) {
            if (response.Success) {
                var referralSource = JSON.parse(response.Result);//need to add check for invalid escapes! <- DONE

                console.log(referralSource);

            } else {
                console.log(response.FailureInformation)
                callback(referralSource);
            }
        }
    });

    $.ajax({
        type: "post",
        url: "http://207.219.170.39:4550/api/ezshred/getdata",
        data: JSON.stringify({ 'Request': JSON.stringify({ "Request": "GetTerms" }) }),
        async: true,
        cache: false,
        success: function (response) {
            if (response.Success) {
                var terms = JSON.parse(response.Result);//need to add check for invalid escapes! <- DONE

                console.log(terms);
                callback(terms);
            } else {
                console.log(response.FailureInformation);
            }
        }
    });

    $.ajax({
        type: "post",
        url: "http://207.219.170.39:4550/api/ezshred/getdata",
        data: JSON.stringify({ 'Request': JSON.stringify({ "Request": "GetInvoiceTypes" }) }),
        async: true,
        cache: false,
        success: function (response) {
            if (response.Success) {
                var invoiceTypes = JSON.parse(response.Result);//need to add check for invalid escapes! <- DONE

                console.log(invoiceTypes);
                callback(invoiceTypes);

            } else {
                console.log(response.FailureInformation);
            }
        }
    });

    //$.ajax({
    //    type: "post",
    //    url: "http://207.219.170.39:4550/api/ezshred/getdata",
    //    data: JSON.stringify({ 'Request': JSON.stringify({ "Request": "GetCustomerTypes" }) }),
    //    async: true,
    //    cache: false,
    //    success: function (response) {
    //        if (response.Success) {
    //            var customerTypes = JSON.parse(response.Result);//need to add check for invalid escapes! <- DONE

    //            console.log(customerTypes);

    //        } else {
    //            console.log(response.FailureInformation)
    //        }
    //    }
    //});   

}