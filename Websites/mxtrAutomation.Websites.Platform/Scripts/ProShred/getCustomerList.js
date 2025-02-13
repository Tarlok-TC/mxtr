function getCustomerList(callback) {
    $.ajax({
        type: "get",
        url: "/GetCustomers",
        async: true,
        cache: false,
        success: function (response) {
            if (response.Data.length > 0) {
                alert(11111);
                var customers;
                try {                   
                    customers = response.Data;
                } catch (err) { 
                    var str = response.Result;
                    var newResult = str.replace(/\\'/g, "'");
                    customers = (JSON.parse(newResult)).tblCustomers;

                }
                console.log("Just got customer list... ");
               // callback(customers);

            } else {
                console.log(response.FailureInformation)
            }
        }
    });
}

