function getBuildingList(callback) {

    $.ajax({
        type: "post",
        url: "/GetBuildingListByCustomer",
        async: true,
        cache: false,
        success: function (response) {
            if (response.Data != undefined) {

                console.log(response.Data);
            }
            //if (response.Success) {
            //    var buildings;
            //    //var resultJSON = JSON.parse(response.Result);
            //    //console.log(resultJSON); var resultJSON;
            //    try {
            //        buildings = (JSON.parse(response.Result)).tblBuilding;//need to add check for invalid escapes! <- DONE

            //    } catch (err) {//only handles errors if need be.. 
            //        var str = response.Result;
            //        var newResult = str.replace(/\\'/g, "'");
            //        buildings = (JSON.parse(newResult)).tblBuilding;

            //    }
            //    console.log("Just got building list...");
            //    callback(buildings);

            //} else {
            //    console.log(response.FailureInformation)
            //    alert("Could not get BuildingList, API Down... " + response.FailureInformation);

            //}
        }
    });
}