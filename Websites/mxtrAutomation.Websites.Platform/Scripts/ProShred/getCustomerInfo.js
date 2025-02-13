function getCustomerInfo(customerID,UserID, callback) {
    $.ajax({
        type: "post",
        url: "http://207.219.170.39:4550/api/ezshred/getdata",
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



