//(function () {

//    var deps = ['angularTreeview'];

//    var AccountHierarchyApp = angular.module('AccountHierarchy', deps);

//    AccountHierarchyApp.controller('myController', function ($scope) {
//        //temporary node
//        $scope.temporaryNode = {
//            children: []
//        };
//        //console.log("=====_accounts", _accounts);
//        $scope.roleList = [_accounts];
//        //console.log($scope.roleList);
//        //test tree model
//        //$scope.roleList = [
//        //    { AccountName : "User", id : "role1", children : [
//        //      { AccountName: "subUser1", id: "role11", children: [] },
//        //      { AccountName : "subUser2", id : "role12", children : [
//        //        {
//        //            AccountName: "subUser2-1", id: "role121", children: [
//        //          { AccountName: "subUser2-1-1", id: "role1211", children: [] },
//        //          { AccountName: "subUser2-1-2", id: "role1212", children: [] }
//        //        ]}
//        //      ]}
//        //    ]},

//        //    { AccountName: "Admin", id: "role2", children: [] },

//        //    { AccountName: "Guest", id: "role3", children: [] }
//        //];
//        //console.log($scope.roleList);

//        $scope.done = function () {
//            /* reset */
//            $scope.mytree.currentNode.selected = undefined;
//            $scope.mytree.currentNode = undefined;
//            $scope.mode = undefined;
//        };

//        $scope.addChildDone = function () {
//            /* add child */
//            if ($scope.temporaryNode.id && $scope.temporaryNode.label) {
//                $scope.mytree.currentNode.children.push(angular.copy($scope.temporaryNode));
//            }

//            /* reset */
//            $scope.temporaryNode.id = "";
//            $scope.temporaryNode.label = "";

//            $scope.done();
//        };

//        $scope.deleteAccount = function (accountObjectId) {
//           // alert(_manageUserAccountUrl);
//            var userAction = confirm("Are you sure you want to delete this account");
//            if (userAction) {
//                var data = {
//                    'AccountObjectID': accountObjectId,
//                    'IsDelete': true,
//                    'IsAjax': true,
//                };
//                $.ajax({
//                    url: _manageUserAccountUrl,
//                    async: false,
//                    dataType: 'json',
//                    type: 'post',
//                    data: data,
//                    success: function (result) {
//                        $scope.roleList = [result.Accounts];
//                        _accounts = result.Accounts;
//                        setTimeout(function () {
//                            $("#dvtree > ul > li > div .expanded").each(function () {
//                                $(this).click();
//                            });
//                        }, 100);                       
//                    },
//                    error: function (error) {
//                        //error
//                    }
//                });
//            }
//            else {
//                return false;
//            }
//        };

//    });

//})();



(function () {
    //angular module
    var myApp = angular.module('myApp', ['angularTreeview']);
    //test controller
    myApp.controller('myController', function ($scope) {
        $scope.roleList = [_accounts];

        $scope.deleteAccount = function (accountObjectId) {
            // alert(_manageUserAccountUrl);

            QuestionAlert("Delete Account", "Are you sure you want to delete this account ?", function () {
                var data = {
                    'AccountObjectID': accountObjectId,
                    'IsDelete': true,
                    'IsAjax': true,
                };
                $.ajax({
                    url: _manageUserAccountUrl,
                    async: false,
                    dataType: 'json',
                    type: 'post',
                    data: data,
                    success: function (result) {
                        $scope.roleList = [result.Accounts];
                        _accounts = result.Accounts;
                        setTimeout(function () {
                            $("#dvtree > ul > li > div .expanded").each(function () {
                                $(this).click();
                            });
                        }, 100);
                    },
                    error: function (error) {
                        //error
                    }
                });

            }, function () {
                return false;
            });

            //var userAction = confirm("Are you sure you want to delete this account");
            //if (userAction) {
            //    var data = {
            //        'AccountObjectID': accountObjectId,
            //        'IsDelete': true,
            //        'IsAjax': true,
            //    };
            //    $.ajax({
            //        url: _manageUserAccountUrl,
            //        async: false,
            //        dataType: 'json',
            //        type: 'post',
            //        data: data,
            //        success: function (result) {
            //            $scope.roleList = [result.Accounts];
            //            _accounts = result.Accounts;
            //            setTimeout(function () {
            //                $("#dvtree > ul > li > div .expanded").each(function () {
            //                    $(this).click();
            //                });
            //            }, 100);
            //        },
            //        error: function (error) {
            //            //error
            //        }
            //    });
            //}
            //else {
            //    return false;
            //}
        };
    });
})();
