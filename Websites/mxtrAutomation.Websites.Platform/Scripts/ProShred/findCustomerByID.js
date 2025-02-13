
function findCustomerByID(customerID, customerArr) {
    if (customerArr[customerID].CustomerID == customerID)
        return customerArr[customerID];
    else
        console.log("error finding customer");
}