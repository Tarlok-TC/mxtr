function getBuildingInfo(BuildingID, bldgCallBack) {

    $.ajax({
        type: "post",
        url: "http://207.219.170.39:4550/api/ezshred/getdata",
        data: JSON.stringify({ 'Request': JSON.stringify({ "Request": "GetBuildingInfo", "BuildingID": BuildingID }) }),
        async: true,
        cache: false,
        success: function (response) {
            if (response.Success) {
                var buildingInfo;
                try {
                    buildingInfo = (JSON.parse(response.Result)).tblBuilding[0];//need to add check for invalid escapes! <- DONE
                } catch (err) {
                    var str = response.Result;
                    var newResult = str.replace(/\\'/g, "'");
                    buildingInfo = (JSON.parse(newResult)).tblBuilding[0];
                }
                console.log(buildingInfo);
                bldgCallBack(buildingInfo);

            } else {
                console.log(response.FailureInformation)
            }
        }
    });
}
