
//---------- Angular section ------------
(function () {
    'use strict';
    //var ManageMenuApp = angular.module('managemenuApp', ['ngAnimate', 'ngSanitize', 'mgcrea.ngStrap']);
    var ManageMenuApp = angular.module('managemenuApp', ['MenuIconsService']);
    var menudataNotification = $('#menudata-notification');

    //------------ Filter -------
    ManageMenuApp.filter('range', function () {
        return function (input, min, max) {
            min = parseInt(min); //Make string input int
            max = parseInt(max);
            for (var i = min; i < max; i++)
                input.push(i);
            return input;
        };
    });

    //---------------- customize menu ----------------------------------------
    angular.module('managemenuApp').controller('customizemenuCtrl', function ($scope, MenuIcons) {
        $scope.icons = MenuIcons.iconslist();
        $scope.menuitems = _menus.Menus;
        $scope.isdefaultmenu = _menus.IsDefaultMenu;
        $scope.sortorder = _sortorder;

        $scope.updateMenuItems = function (item) {
            if (CheckSortOrder(item) && CheckMenuName(item) && CheckUniqueName(item)) {
                //console.log(item);
                var data =
                    {
                        //'ManageMenuData': JSON.stringify(item),
                        'ManageMenuData': angular.toJson(item),
                    };

                $.ajax({
                    url: _manageMenuUrl,
                    dataType: 'json',
                    type: 'post',
                    data: data,
                    success: function (result) {
                        if (result.Success) {
                            //console.log(result.MenusData.Menus);
                            $scope.$apply(function () {
                                $scope.menuitems = result.MenusData.Menus;
                                $scope.isdefaultmenu = result.MenusData.IsDefaultMenu;
                            });
                            notificationBarSuccess(menudataNotification, 'Data updated successfully.');
                            setTimeout(function () {
                                location.reload();
                            }, 1000);
                        }
                        else {
                            notificationBarFailure(menudataNotification, 'There was a problem while updating menus.');
                        }

                        setTimeout(function () {
                            timeoutHideNotificationBar(menudataNotification);
                        }, 1000);

                    }
                });
            }
        };

        $scope.resetMenuItems = function () {
            if (!$scope.isdefaultmenu) {
                QuestionAlert("Reset Menu", "Are you sure you want to reset menu ?", function () {
                    $.ajax({
                        url: _resetMenuUrl,
                        dataType: 'json',
                        type: 'post',
                        data: [],
                        success: function (result) {
                            if (result.Success) {
                                //console.log(result.MenusData);
                                $scope.$apply(function () {
                                    $scope.menuitems = result.MenusData.Menus;
                                    $scope.isdefaultmenu = result.MenusData.IsDefaultMenu;
                                });
                                notificationBarSuccess(menudataNotification, 'Menu reset to default.');
                                setTimeout(function () {
                                    location.reload();
                                }, 1000);
                            }
                            else {
                                notificationBarFailure(menudataNotification, 'There was a problem while reseting menus.');
                            }

                            setTimeout(function () {
                                timeoutHideNotificationBar(menudataNotification);
                            }, 1000);

                        }
                    });
                }, function () {

                });
            }
            else {
                notificationBarFailure(menudataNotification, 'Menu already in default mode.');
                setTimeout(function () {
                    timeoutHideNotificationBar(menudataNotification);
                }, 3000);
            }
        };

        function CheckMenuName(item) {
            if (!_.every(item, x=>x.Name != "")) {
                notificationBarFailure(menudataNotification, 'Menu Name can not be empty.');
                HideMessage();
                return false;
            }
            var result = _.filter(item, function (obj) {
                return _.where(obj.SubMenu, { Name: "" }).length > 0;
            });
            if (result.length) {
                notificationBarFailure(menudataNotification, 'Sub Menu Name can not be empty.');
                HideMessage();
                return false;
            }
            return true;

        }

        function CheckUniqueName(item) {

            var mainMenusnames = _.pluck(item, "Name");
            //menu name must be unique
            if (mainMenusnames.length != _.uniq(mainMenusnames).length) {
                notificationBarFailure(menudataNotification, 'Menu Name must be unique.');
                HideMessage();
                return false;
            }


            var subMenuNames = _(_.pluck(item, 'SubMenu')).chain().flatten().pluck("Name");
            subMenuNames = _.chain(subMenuNames).flatten(true).map(function (a) { return a }).value();
            subMenuNames = _.without(subMenuNames, undefined);

            //Sub menu name must be unique
            if (subMenuNames.length != _.uniq(subMenuNames).length) {
                notificationBarFailure(menudataNotification, 'Sub Menu Name must be unique.');
                HideMessage();
                return false;
            }

            var arrNames = [];

            _.each(mainMenusnames, function (obj) {
                if (obj != undefined) {
                    arrNames.push(obj.toLowerCase());
                }
            })
            _.each(subMenuNames, function (obj) {
                if (obj != undefined) {
                    arrNames.push(obj.toLowerCase());
                }
            })

            //Main Menu and Sub menu name must be unique
            if (arrNames.length != _.uniq(arrNames).length) {
                notificationBarFailure(menudataNotification, 'Menu Name must be unique.');
                HideMessage();
                return false;
            }
            //console.log(arrNames);
            return true;
        }

        function CheckSortOrder(item) {
            var arrSortOrder = [];

            for (var i = 0; i < item.length; i++) {
                if (arrSortOrder.indexOf(item[i].SortOrder) > -1) {
                    notificationBarFailure(menudataNotification, 'Two menus cannot have same sort order.');
                    HideMessage();
                    return false;
                }
                arrSortOrder.push(item[i].SortOrder);
                if (item[i].SubMenu != null && item[i].SubMenu.length > 0) {
                    if (!CheckSubMenuSortOrder(item[i].SubMenu)) {
                        notificationBarFailure(menudataNotification, 'Two sub menus cannot have same sort order.');
                        HideMessage();
                        return false;
                    }
                }
            }
            //console.log(arrSortOrder);
            return true;
        }

        function CheckSubMenuSortOrder(subMenu) {
            var arrSubSortOrder = [];
            for (var i = 0; i < subMenu.length; i++) {
                if (arrSubSortOrder.indexOf(subMenu[i].SortOrder) > -1) {
                    return false;
                }
                arrSubSortOrder.push(subMenu[i].SortOrder);
            }
            return true;
        }

        function HideMessage() {
            setTimeout(function () {
                timeoutHideNotificationBar(menudataNotification);
            }, 3000);
        }
    });

    //---------------- Manage menu ----------------------------------------
    ManageMenuApp.controller('managemenuCtrl', function ($scope, MenuIcons) {

        //--- Global Variables -----------
        $scope.menuitems = _menus.Menus;
        //console.log($scope.menuitems);
        $scope.isdefaultmenu = _menus.IsDefaultMenu;
        $scope.icons = MenuIcons.iconslist();
        $scope.sortorder = _sortorder;
        $scope.addmode = false;
        $scope.newitem = {
            "Name": '',
            "Icon": '',
            "PageUrl": '',
            "SubMenu": [],
            "MenuScope": "Global",
            "OrganizationAccounts": _menus.OrganizationAccounts,
        };
        $scope.selectedMenuType = "0";

        //--- functions ----------
        $scope.deleteMenuItem = function (menuid, submenuid) {
            QuestionAlert("Delete Menu", "Are you sure you want to delete menu ?", function () {
                var data =
                 {
                     'menuid': menuid,
                     'submenuId': submenuid,
                 };

                //console.log(data);

                $.ajax({
                    url: _deleteMenuUrl,
                    dataType: 'json',
                    type: 'post',
                    data: data,
                    success: function (result) {
                        if (result.Success) {
                            SuccessAlert('', 'Menu deleted sucessfully.');
                            notificationBarSuccess(menudataNotification, 'Menu deleted sucessfully.');
                            setTimeout(function () {
                                location.reload();
                            }, 1000);
                        }
                        else {
                            ErrorAlert('', 'There was a problem while deleting menu')
                            notificationBarFailure(menudataNotification, 'There was a problem while deleting menu.');
                        }

                        setTimeout(function () {
                            timeoutHideNotificationBar(menudataNotification);
                        }, 1000);

                    }
                });

            }, function () {
            });
        };

        $scope.addEditMenuItem = function (item) {
            if (!ValidateData(item)) {
                setTimeout(function () {
                    timeoutHideNotificationBar(menudataNotification);
                }, 2000);
                return;
            }
            var data =
                  {
                      'ManageMenuData': angular.toJson(item),
                  };

            //console.log(data);

            $.ajax({
                url: _addeditMenuUrl,
                dataType: 'json',
                type: 'post',
                data: data,
                success: function (result) {
                    if (result.Success) {
                        notificationBarSuccess(menudataNotification, 'Menu created sucessfully.');
                        setTimeout(function () {
                            //$scope.menuitems.push(item);
                            //$("#btnCancel").click();
                            getMasterMenu(item, ActionTypeEnum.Add);
                        }, 1000);
                    }
                    else {
                        notificationBarFailure(menudataNotification, 'There was a problem while creating menu.');
                    }

                    setTimeout(function () {
                        timeoutHideNotificationBar(menudataNotification);
                    }, 1000);

                    _.each($scope.newitem.OrganizationAccounts, function (obj, i) {
                        $scope.newitem.OrganizationAccounts[i].IsSelected = false;
                    });

                }
            });

        };

        $scope.addSubMenuItem = function () {
            $scope.newitem.SubMenu.push({
                "MenuId": '',
                "Name": '',
                "Icon": '',
                "PageUrl": '',
                "SubMenu": null,
                //"MenuScope": "0",//-- MenuScope ---> 0 means Global , 1 means Organization based
                //"OrganizationAccounts": _menus.OrganizationAccounts,
            });

            //console.log($scope.newitem);
        };

        $scope.cancelMenu = function () {
            //console.log($scope.newitem.OrganizationAccounts);
            getMasterMenu(null, ActionTypeEnum.Cancel);
            $scope.addmode = false;
            $scope.newitem = {
                "Name": '',
                "Icon": '',
                "PageUrl": '',
                "SubMenu": [],
                "MenuScope": "Global",
                "OrganizationAccounts": _menus.OrganizationAccounts,
            };
            $scope.selectedMenuType = "0";
            showHideOnMenuType(true);

            _.each($scope.newitem.OrganizationAccounts, function (obj, i) {
                $scope.newitem.OrganizationAccounts[i].IsSelected = false;
            });
        };

        $scope.deleteSubMenuItem = function (submenuitem) {
            var index = $scope.newitem.SubMenu.indexOf(submenuitem);
            $scope.newitem.SubMenu.splice(index, 1);
        };

        $scope.menuTypeSelection = function () {
            $scope.newitem.PageUrl = '';
            $scope.newitem.SubMenu = [];

            if ($scope.selectedMenuType == "0") {
                showHideOnMenuType(true);
            }
            else {
                showHideOnMenuType(false);
                if ($scope.newitem.SubMenu == null || $scope.newitem.SubMenu.length == 0) {
                    $scope.newitem.SubMenu = [];
                    $scope.addSubMenuItem();
                }
            }
        };

        $scope.EditMenuItem = function (menuitem) {
            //console.log(menuitem);
            $scope.addmode = true;
            if (menuitem.PageUrl == null || menuitem.PageUrl == "") {
                $scope.selectedMenuType = "1";
            }
            $scope.menuTypeSelection();

            if (menuitem.OrganizationAccounts != null) {
                $scope.newitem.OrganizationAccounts = _menus.OrganizationAccounts;
                _.each($scope.newitem.OrganizationAccounts, function (obj, i) {
                    var indexOfAccount = menuitem.OrganizationAccounts.findIndex(i => i.AccountObjectID === obj.AccountObjectID);
                    if (indexOfAccount > -1) {
                        $scope.newitem.OrganizationAccounts[i].IsSelected = true;
                    }
                });
                menuitem.OrganizationAccounts = $scope.newitem.OrganizationAccounts;
            }
            else {
                menuitem.OrganizationAccounts = _menus.OrganizationAccounts;
            }
            $scope.newitem = menuitem;
        };

        //--- Jquery functions --------------
        function ValidateData(item) {

            var IsValid = false;
            if (item.Name == "") {
                notificationBarFailure(menudataNotification, 'Menu Name is required.');
            }
            else if (item.Icon == "") {
                notificationBarFailure(menudataNotification, 'Menu Icon is required.');
            }
            else if ($scope.selectedMenuType == "0" && item.PageUrl == "") {
                notificationBarFailure(menudataNotification, 'Menu Page Url is required.');
            }
            else if ($scope.selectedMenuType == "1" && item.SubMenu.length == 0) {
                notificationBarFailure(menudataNotification, 'Atleast one sub menu is required.');
            }
            else if (item.MenuScope == "OrganizationBased" && !IsOrganizationSelected(item)) {
                notificationBarFailure(menudataNotification, 'Please select atleast one organization.');
            }
            else if ($scope.selectedMenuType == "1" && item.SubMenu.length > 0) {
                var isAllSubMenuValid = true;
                for (var i = 0; i < item.SubMenu.length; i++) {
                    var subMenuData = item.SubMenu[i];
                    if (subMenuData.Name == "" || subMenuData.PageUrl == "") {
                        isAllSubMenuValid = false;
                        notificationBarFailure(menudataNotification, 'Sub menu Name and Page Url is required.');
                        return;
                    }
                }
                if (isAllSubMenuValid) {
                    var subMenuName = _.pluck(item.SubMenu, "Name");
                    subMenuName = _.each(subMenuName, function (obj) { return obj.toLowerCase(); });
                    if (subMenuName.length != _.uniq(subMenuName).length) {
                        notificationBarFailure(menudataNotification, 'Sub menu Name must be unique.');
                    }
                    else if (subMenuName.indexOf(item.Name.toLowerCase()) > -1) {
                        notificationBarFailure(menudataNotification, 'Menu Name and Sub menu Name must be unique.');
                    }
                    else {
                        IsValid = true;
                    }
                }
            }
            else {
                IsValid = true;
            }
            return IsValid;
        }

        function IsOrganizationSelected(data) {
            var selectedOrganization = _.find(data.OrganizationAccounts, function (o) {
                return o.IsSelected == true;
            });
            if (selectedOrganization != undefined && selectedOrganization != null) {
                return true
            }
            else {
                return false;
            }
        }

        function showHideOnMenuType(IsMainMenu) {
            if (IsMainMenu) {
                $("#btnAddSubMenu").hide();
                $("#dvPath").show();
            }
            else {
                $("#btnAddSubMenu").show();
                $("#dvPath").hide();
            }
        }

        function getMasterMenu(item, action) {
            var data =
                  {
                      'isajax': true,
                  };

            $.ajax({
                url: _manageMenuWebQuery,
                dataType: 'json',
                type: 'post',
                data: data,
                success: function (result) {
                    if (result.Success) {
                        $scope.$apply(function () {
                            $scope.addmode = false;
                            $scope.newitem = {
                                "Name": '',
                                "Icon": '',
                                "PageUrl": '',
                                "SubMenu": [],
                                "MenuScope": "Global",
                                "OrganizationAccounts": _menus.OrganizationAccounts,
                            };
                            $scope.selectedMenuType = "0";
                            showHideOnMenuType(true);

                            $scope.menuitems = [];
                            $scope.menuitems = result.MenuMasterData;
                            //$scope.organizationAccounts = result.OrganizationAccounts;
                        });

                        if (action === ActionTypeEnum.Add) {
                            ////AddSideMenu(item);
                        }
                        else if (action === ActionTypeEnum.Update) {

                        }
                        else if (action === ActionTypeEnum.Cancel) {
                            //alert(1);
                        }

                    }
                    else {
                        notificationBarFailure(menudataNotification, 'There was a problem while creating menu.');
                    }

                    setTimeout(function () {
                        timeoutHideNotificationBar(menudataNotification);
                    }, 1000);

                }
            });
        }

        var ActionTypeEnum = {
            Add: 0,
            Update: 1,
            Cancel: 2,
        };

    });

    //---------------- Service --------------------------------------------
    var MenuIconsService = angular.module('MenuIconsService', ['ngAnimate', 'ngSanitize', 'mgcrea.ngStrap'])
    .service('MenuIcons', function () {
        this.iconslist = function () {
            return [
                            { value: 'fa fa-home', label: '<i class="fa fa-home"></i> Home' },
                            { value: 'fa fa-bar-chart-o', label: '<i class="fa fa-bar-chart-o"></i> Bar Char' },
                            { value: 'fa fa-address-book', label: '<i class="fa fa-address-book"></i> Address Book' },
                            { value: 'fa fa-envelope', label: '<i class="fa fa-envelope"></i> Envelope' },
                            { value: 'fa fa-adjust', label: '<i class="fa fa-adjust"></i> Adjust' },
                            { value: 'fa fa-anchor', label: '<i class="fa fa-anchor"></i> Anchor' },
                            { value: 'fa fa-archive', label: '<i class="fa fa-archive"></i> Archive' },
                            { value: 'fa fa-area-chart', label: '<i class="fa fa-area-chart"></i> Chart' },
                            { value: 'fa fa-arrows', label: '<i class="fa fa-arrows"></i> Arrows' },
                            { value: 'fa fa-arrows-h', label: '<i class="fa fa-arrows-h"></i> Arrows-h' },
                            { value: 'fa fa-arrows-v', label: '<i class="fa fa-arrows-v"></i> Arrows-v' },
                            { value: 'fa fa-at', label: '<i class="fa fa-at"></i> At' },
                            { value: 'fa fa-automobile', label: '<i class="fa fa-automobile"></i> Automobile' },
                            { value: 'fa fa-ban', label: '<i class="fa fa-ban"></i> Ban' },
                            { value: 'fa fa-bank', label: '<i class="fa fa-bank"></i> Bank' },
                            { value: 'fa fa-bar-chart', label: '<i class="fa fa-bar-chart"></i> Chart' },
                                // { value: 'fa fa-bar-chart-o', label: '<i class="fa fa-bar-chart-o"></i> Chart-o"' },
                            { value: 'fa fa-barcode', label: '<i class="fa fa-barcode"></i> Barcode' },
                            { value: 'fa fa-bars', label: '<i class="fa fa-bars"></i> Bars' },
                            { value: 'fa fa-beer', label: '<i class="fa fa-beer"></i> Beer' },
                            { value: 'fa fa-bell', label: '<i class="fa fa-bell"></i> Bell' },
                            { value: 'fa fa-bell-o', label: '<i class="fa fa-bell-o"></i> Bell-o' },
                            { value: 'fa fa-bell-slash', label: '<i class="fa fa-bell-slash"></i> Bell-slash' },
                            { value: 'fa fa-bell-slash-o', label: '<i class="fa fa-bell-slash-o"></i> Bell-slash-o' },
                            { value: 'fa fa-bicycle', label: '<i class="fa fa-bicycle"></i> Bicycle' },
                            { value: 'fa fa-binoculars', label: '<i class="fa fa-binoculars"></i> Binoculars' },
                            { value: 'fa fa-birthday-cake', label: '<i class="fa fa-birthday-cake"></i> Birthday-cake' },
                            { value: 'fa fa-bolt', label: '<i class="fa fa-bolt"></i> Bolt' },
                            { value: 'fa fa-bomb', label: '<i class="fa fa-bomb"></i> Bomb' },
                            { value: 'fa fa-book', label: '<i class="fa fa-book"></i> Book' },
                            { value: 'fa fa-bookmark', label: '<i class="fa fa-bookmark"></i> Bookmark' },
                            { value: 'fa fa-bookmark-o', label: '<i class="fa fa-bookmark-o"></i> Bookmark-o' },
                            { value: 'fa fa-briefcase', label: '<i class="fa fa-briefcase"></i> Briefcase' },
                            { value: 'fa fa-bug', label: '<i class="fa fa-bug"></i> Bug' },
                            { value: 'fa fa-building', label: '<i class="fa fa-building"></i> Building' },
                            { value: 'fa fa-building-o', label: '<i class="fa fa-building-o"></i> Building-o' },
                            { value: 'fa fa-bullhorn', label: '<i class="fa fa-bullhorn"></i> Bullhorn' },
                            { value: 'fa fa-bullseye', label: '<i class="fa fa-bullseye"></i> Bullseye' },
                            { value: 'fa fa-bus', label: '<i class="fa fa-bus"></i> Bus' },
                            { value: 'fa fa-cab', label: '<i class="fa fa-cab"></i> Cab' },
                            { value: 'fa fa-calculator', label: '<i class="fa fa-calculator"></i> Calculator' },
                            { value: 'fa fa-calendar', label: '<i class="fa fa-calendar"></i> Calendar' },
                            { value: 'fa fa-calendar-o', label: '<i class="fa fa-calendar-o"></i> Calendar-o' },
                            { value: 'fa fa-camera', label: '<i class="fa fa-camera"></i> Camera' },
                            { value: 'fa fa-camera-retro', label: '<i class="fa fa-camera-retro"></i> Camera-retro' },
                            { value: 'fa fa-car', label: '<i class="fa fa-car"></i> Car' },
                            { value: 'fa fa-caret-square-o-down', label: '<i class="fa fa-caret-square-o-down"></i> Caret-square-o-down' },
                            { value: 'fa fa-caret-square-o-left', label: '<i class="fa fa-caret-square-o-left"></i> Caret-square-o-left' },
                            { value: 'fa fa-caret-square-o-right', label: '<i class="fa fa-caret-square-o-right"></i> Caret-square-o-right' },
                            { value: 'fa fa-caret-square-o-up', label: '<i class="fa fa-caret-square-o-up"></i> Caret-square-o-up' },
                            { value: 'fa fa-cc', label: '<i class="fa fa-cc"></i> fa fa-cc' },
                            { value: 'fa fa-certificate', label: '<i class="fa fa-certificate"></i> Certificate' },
                            { value: 'fa fa-check', label: '<i class="fa fa-check"></i> Check' },
                            { value: 'fa fa-check-circle', label: '<i class="fa fa-check-circle"></i> Check-circle' },
                            { value: 'fa fa-check-circle-o', label: '<i class="fa fa-check-circle-o"></i> Check-circle-o' },
                            { value: 'fa fa-check-square', label: '<i class="fa fa-check-square"></i> Check-square' },
                            { value: 'fa fa-check-square-o', label: '<i class="fa fa-check-square-o"></i> Check-square-o' },
                            { value: 'fa fa-child', label: '<i class="fa fa-child"></i> Child' },
                            { value: 'fa fa-circle', label: '<i class="fa fa-circle"></i> Circle' },
                            { value: 'fa fa-circle-o', label: '<i class="fa fa-circle-o"></i> Circle-o' },
                            { value: 'fa fa-circle-o-notch', label: '<i class="fa fa-circle-o-notch"></i> Circle-o-notch' },
                            { value: 'fa fa-circle-thin', label: '<i class="fa fa-circle-thin"></i> Circle-thin' },
                            { value: 'fa fa-clock-o', label: '<i class="fa fa-clock-o"></i> Clock-o' },
                            { value: 'fa fa-close', label: '<i class="fa fa-close"></i> Clock' },
                            { value: 'fa fa-cloud', label: '<i class="fa fa-cloud"></i> Cloud' },
                            { value: 'fa fa-cloud-download', label: '<i class="fa fa-cloud-download"></i> Cloud-download' },
                            { value: 'fa fa-cloud-upload', label: '<i class="fa fa-cloud-upload"></i> Cloud-upload' },
                            { value: 'fa fa-code', label: '<i class="fa fa-code"></i> Code' },
                            { value: 'fa fa-code-fork', label: '<i class="fa fa-code-fork"></i> Code-fork' },
                            { value: 'fa fa-coffee', label: '<i class="fa fa-coffee"></i> Coffee' },
                            { value: 'fa fa-cog', label: '<i class="fa fa-cog"></i> Cog' },
                            { value: 'fa fa-cogs', label: '<i class="fa fa-cogs"></i> Cogs' },
                            { value: 'fa fa-comment', label: '<i class="fa fa-comment"></i> Comment' },
                            { value: 'fa fa-comment-o', label: '<i class="fa fa-comment-o"></i> Comment-o' },
                            { value: 'fa fa-comments', label: '<i class="fa fa-comments"></i> Comments' },
                            { value: 'fa fa-comments-o', label: '<i class="fa fa-comments-o"></i> Comments-o' },
                            { value: 'fa fa-compass', label: '<i class="fa fa-compass"></i> Compass' },
                            { value: 'fa fa-copyright', label: '<i class="fa fa-copyright"></i> Copyright' },
                            { value: 'fa fa-credit-card', label: '<i class="fa fa-credit-card"></i> Credit-card' },
                            { value: 'fa fa-crop', label: '<i class="fa fa-crop"></i> Crop' },
                            { value: 'fa fa-crosshairs', label: '<i class="fa fa-crosshairs"></i>Crosshairs' },
                            { value: 'fa fa-cube', label: '<i class="fa fa-cube"></i> Cube' },
                            { value: 'fa fa-cubes', label: '<i class="fa fa-cubes"></i> Cubes' },
                            { value: 'fa fa-cutlery', label: '<i class="fa fa-cutlery"></i> Cutlery' },
                            { value: 'fa fa-dashboard', label: '<i class="fa fa-dashboard"></i> Dashboard' },
                            { value: 'fa fa-database', label: '<i class="fa fa-database"></i> Database' },
                            { value: 'fa fa-desktop', label: '<i class="fa fa-desktop"></i> Desktop' },
                            { value: 'fa fa-dot-circle-o', label: '<i class="fa fa-dot-circle-o"></i>Dot-circle-o' },
                            { value: 'fa fa-download', label: '<i class="fa fa-download"></i> Download' },
                            { value: 'fa fa-edit', label: '<i class="fa fa-edit"></i> Edit' },
                            { value: 'fa fa-ellipsis-h', label: '<i class="fa fa-ellipsis-h"></i> Ellipsis-h' },
                            { value: 'fa fa-ellipsis-v', label: '<i class="fa fa-ellipsis-v"></i> Ellipsis-v' },
                                //{ value: 'fa fa-envelope', label: '<i class="fa fa-envelope"></i> Envelope' },
                            { value: 'fa fa-envelope-o', label: '<i class="fa fa-envelope-o"></i> Envelope-o' },
                            { value: 'fa fa-envelope-square', label: '<i class="fa fa-envelope-square"></i> Envelope-square' },
                            { value: 'fa fa-eraser', label: '<i class="fa fa-eraser"></i> Eraser' },
                            { value: 'fa fa-exchange', label: '<i class="fa fa-exchange"></i> Exchange' },
                            { value: 'fa fa-exclamation', label: '<i class="fa fa-exclamation"></i> Exclamation' },
                            { value: 'fa fa-exclamation-circle', label: '<i class="fa fa-exclamation-circle"></i> Exclamation-circle' },
                            { value: 'fa fa-exclamation-triangle', label: '<i class="fa fa-exclamation-triangle"></i> Exclamation-triangle' },
                            { value: 'fa fa-external-link', label: '<i class="fa fa-external-link"></i> External-link' },
                            { value: 'fa fa-external-link-square', label: '<i class="fa fa-external-link-square"></i> External-link-square' },
                            { value: 'fa fa-eye', label: '<i class="fa fa-eye"></i> Eye' },
                            { value: 'fa fa-eye-slash', label: '<i class="fa fa-eye-slash"></i> Eye-slash' },
                            { value: 'fa fa-eyedropper', label: '<i class="fa fa-eyedropper"></i> Eyedropper' },
                            { value: 'fa fa-fax', label: '<i class="fa fa-fax"></i> Fax' },
                            { value: 'fa fa-female', label: '<i class="fa fa-female"></i> Female' },
                            { value: 'fa fa-fighter-jet', label: '<i class="fa fa-fighter-jet"></i> Fighter-jet' },
                            { value: 'fa fa-file-archive-o', label: '<i class="fa fa-file-archive-o"></i> File-archive-o' },
                            { value: 'fa fa-file-archive-o', label: '<i class="fa fa-file-archive-o"></i> File-archive-o' },
                            { value: 'fa fa-file-audio-o', label: '<i class="fa fa-file-audio-o"></i> File-audio-o' },
                            { value: 'fa fa-file-code-o', label: '<i class="fa fa-file-code-o"></i> File-code-o' },
                            { value: 'fa fa-file-excel-o', label: '<i class="fa fa-file-excel-o"></i> File-excel-o' },
                            { value: 'fa fa-file-picture-o', label: '<i class="fa fa-file-picture-o"></i> File-picture-o' },
                            { value: 'fa fa-file-powerpoint-o', label: '<i class="fa fa-file-powerpoint-o"></i> File-powerpoint-o' },
                            { value: 'fa fa-file-sound-o', label: '<i class="fa fa-file-sound-o"></i> File-sound-o' },
                            { value: 'fa fa-file-video-o', label: '<i class="fa fa-file-video-o"></i> File-video-o' },
                            { value: 'fa fa-file-word-o', label: '<i class="fa fa-file-word-o"></i> File-word-o' },
                            { value: 'fa fa-file-zip-o', label: '<i class="fa fa-file-zip-o"></i> File-zip-o' },
                            { value: 'fa fa-film', label: '<i class="fa fa-film"></i> Film' },
                            { value: 'fa fa-filter', label: '<i class="fa fa-filter"></i> Filter' },
                            { value: 'fa fa-fire', label: '<i class="fa fa-fire"></i> Fire' },
                            { value: 'fa fa-fire-extinguisher', label: '<i class="fa fa-fire-extinguisher"></i> Fire-extinguisher' },
                            { value: 'fa fa-flag', label: '<i class="fa fa-flag"></i> Flag' },
                            { value: 'fa fa-flag-checkered', label: '<i class="fa fa-flag-checkered"></i> Flag-checkered' },
                            { value: 'fa fa-flag-o', label: '<i class="fa fa-flag-o"></i> Flag-o' },
                            { value: 'fa fa-flash', label: '<i class="fa fa-flash"></i> Flash' },
                            { value: 'fa fa-flask', label: '<i class="fa fa-flask"></i> Flask' },
                            { value: 'fa fa-folder', label: '<i class="fa fa-folder"></i> Folder' },
                            { value: 'fa fa-folder-o', label: '<i class="fa fa-folder-o"></i> Folder-o' },
                            { value: 'fa fa-folder-open', label: '<i class="fa fa-folder-open"></i> Folder-open' },
                            { value: 'fa fa-folder-open-o', label: '<i class="fa fa-folder-open-o"></i> Folder-open-o' },
                            { value: 'fa fa-frown-o', label: '<i class="fa fa-frown-o"></i>  Frown-o' },
                            { value: 'fa fa-futbol-o', label: '<i class="fa fa-futbol-o"></i> Futbol-o' },
                            { value: 'fa fa-gamepad', label: '<i class="fa fa-gamepad"></i> Gamepad' },
                            { value: 'fa fa-gavel', label: '<i class="fa fa-gavel"></i> Gavel' },
                            { value: 'fa fa-gear', label: '<i class="fa fa-gear"></i> Gear' },
                            { value: 'fa fa-gears', label: '<i class="fa fa-gears"></i> Gears' },
                            { value: 'fa fa-gift', label: '<i class="fa fa-gift"></i> Gift' },
                            { value: 'fa fa-glass', label: '<i class="fa fa-glass"></i> Glass' },
                            { value: 'fa fa-globe', label: '<i class="fa fa-globe"></i> Globe' },
                            { value: 'fa fa-graduation-cap', label: '<i class="fa fa-graduation-cap"></i> Graduation-cap' },
                            { value: 'fa fa-group', label: '<i class="fa fa-group"></i> Group' },
                            { value: 'fa fa-hdd-o', label: '<i class="fa fa-hdd-o"></i> Hdd-o' },
                            { value: 'fa fa-headphones', label: '<i class="fa fa-headphones"></i> Headphones' },
                            { value: 'fa fa-heart', label: '<i class="fa fa-heart"></i> Heart' },
                            { value: 'fa fa-heart-o', label: '<i class="fa fa-heart-o"></i> Heart-o' },
                            { value: 'fa fa-history', label: '<i class="fa fa-history"></i> History' },
                                // { value: 'fa fa-home', label: '<i class="fa fa-home"></i> Home' },
                            { value: 'fa fa-image', label: '<i class="fa fa-image"></i> Image' },
                            { value: 'fa fa-inbox', label: '<i class="fa fa-inbox"></i> Inbox' },
                            { value: 'fa fa-info', label: '<i class="fa fa-info"></i> Info' },
                            { value: 'fa fa-info-circle', label: '<i class="fa fa-info-circle"></i> Info-circle' },
                            { value: 'fa fa-institution', label: '<i class="fa fa-institution"></i> Institution' },
                            { value: 'fa fa-key', label: '<i class="fa fa-key"></i> Key' },
                            { value: 'fa fa-keyboard-o', label: '<i class="fa fa-keyboard-o"></i> Keyboard-o' },
                            { value: 'fa fa-language', label: '<i class="fa fa-language"></i> Language' },
                            { value: 'fa fa-laptop', label: '<i class="fa fa-laptop"></i> Laptop' },
                            { value: 'fa fa-leaf', label: '<i class="fa fa-leaf"></i> Leaf' },
                            { value: 'fa fa-legal', label: '<i class="fa fa-legal"></i> Legal' },
                            { value: 'fa fa-lemon-o', label: '<i class="fa fa-lemon-o"></i> Lemon-o' },
                            { value: 'fa fa-level-down', label: '<i class="fa fa-level-down"></i> Level-down' },
                            { value: 'fa fa-level-up', label: '<i class="fa fa-level-up"></i> Level-up' },
                            { value: 'fa fa-life-bouy', label: '<i class="fa fa-life-bouy"></i> Life-bouy' },
                            { value: 'fa fa-life-buoy', label: '<i class="fa fa-life-buoy"></i> Life-buoy' },
                            { value: 'fa fa-life-ring', label: '<i class="fa fa-life-ring"></i> Life-ring' },
                            { value: 'fa fa-life-saver', label: '<i class="fa fa-life-saver"></i> Life-saver' },
                            { value: 'fa fa-lightbulb-o', label: '<i class="fa fa-lightbulb-o"></i> Lightbulb-o' },
                            { value: 'fa fa-line-chart', label: '<i class="fa fa-line-chart"></i> Line-chart' },
                            { value: 'fa fa-location-arrow', label: '<i class="fa fa-location-arrow"></i> Location-arrow' },
                            { value: 'fa fa-lock', label: '<i class="fa fa-lock"></i> Lock' },
                            { value: 'fa fa-magic', label: '<i class="fa fa-magic"></i> Magic' },
                            { value: 'fa fa-magnet', label: '<i class="fa fa-magnet"></i> Magnet' },
                            { value: 'fa fa-mail-forward', label: '<i class="fa fa-mail-forward"></i> Forward' },
                            { value: 'fa fa-mail-reply', label: '<i class="fa fa-mail-reply"></i> Mail-reply' },
                            { value: 'fa fa-mail-reply-all', label: '<i class="fa fa-mail-reply-all"></i> Mail-reply-all' },
                            { value: 'fa fa-male', label: '<i class="fa fa-male"></i> Male' },
                            { value: 'fa fa-map-marker', label: '<i class="fa fa-map-marker"></i> Map-marker' },
                            { value: 'fa fa-meh-o', label: '<i class="fa fa-meh-o"></i> Meh-o' },
                            { value: 'fa fa-microphone', label: '<i class="fa fa-microphone"></i> Microphone' },
                            { value: 'fa fa-microphone-slash', label: '<i class="fa fa-microphone-slash"></i> Microphone-slash' },
                            { value: 'fa fa-minus', label: '<i class="fa fa-minus"></i> Minus' },
                            { value: 'fa fa-minus-circle', label: '<i class="fa fa-minus-circle"></i> Minus-circle' },
                            { value: 'fa fa-minus-square', label: '<i class="fa fa-minus-square"></i> Minus-square' },
                            { value: 'fa fa-minus-square-o', label: '<i class="fa fa-minus-square-o"></i> Minus-square-o' },
                            { value: 'fa fa-mobile', label: '<i class="fa fa-mobile"></i> Mobile' },
                            { value: 'fa fa-mobile-phone', label: '<i class="fa fa-mobile-phone"></i> Mobile-phone' },
                            { value: 'fa fa-money', label: '<i class="fa fa-money"></i> Money' },
                            { value: 'fa fa-moon-o', label: '<i class="fa fa-moon-o"></i> Moon-o' },
                            { value: 'fa fa-mortar-board', label: '<i class="fa fa-mortar-board"></i> Mortar-board' },
                            { value: 'fa fa-music', label: '<i class="fa fa-music"></i> Music' },
                            { value: 'fa fa-navicon', label: '<i class="fa fa-navicon"></i> Navicon' },
                            { value: 'fa fa-newspaper-o', label: '<i class="fa fa-newspaper-o"></i> Newspaper-o' },
                            { value: 'fa fa-paint-brush', label: '<i class="fa fa-paint-brush"></i> Paint-brush' },
                            { value: 'fa fa-paper-plane', label: '<i class="fa fa-paper-plane"></i> Paper-plane' },
                            { value: 'fa fa-paper-plane-o', label: '<i class="fa fa-paper-plane-o"></i> Paper-plane-o' },
                            { value: 'fa fa-paw', label: '<i class="fa fa-paw"></i> Paw' },
                            { value: 'fa fa-pencil', label: '<i class="fa fa-pencil"></i> pencil' },
                            { value: 'fa fa-pencil-square', label: '<i class="fa fa-pencil-square"></i> Pencil-square' },
                            { value: 'fa fa-pencil-square-o', label: '<i class="fa fa-pencil-square-o"></i> Pencil-square-o' },
                            { value: 'fa fa-phone', label: '<i class="fa fa-phone"></i> Phone' },
                            { value: 'fa fa-phone-square', label: '<i class="fa fa-phone-square"></i> Phone-square' },
                            { value: 'fa fa-photo', label: '<i class="fa fa-photo"></i> Photo' },
                            { value: 'fa fa-picture-o', label: '<i class="fa fa-picture-o"></i> Picture-o' },
                            { value: 'fa fa-pie-chart', label: '<i class="fa fa-pie-chart"></i> Pie-chart' },
                            { value: 'fa fa-plane', label: '<i class="fa fa-plane"></i> Plane' },
                            { value: 'fa fa-plug', label: '<i class="fa fa-plug"></i> Plug' },
                            { value: 'fa fa-plus', label: '<i class="fa fa-plus"></i> Plus' },
                            { value: 'fa fa-plus-circle', label: '<i class="fa fa-plus-circle"></i> Plus-circle' },
                            { value: 'fa fa-plus-square', label: '<i class="fa fa-plus-square"></i> Plus-square' },
                            { value: 'fa fa-plus-square-o', label: '<i class="fa fa-plus-square-o"></i> Plus-square-o' },
                            { value: 'fa fa-power-off', label: '<i class="fa fa-power-off"></i> Power-off' },
                            { value: 'fa fa-print', label: '<i class="fa fa-print"></i> Print' },
                            { value: 'fa fa-puzzle-piece', label: '<i class="fa fa-puzzle-piece"></i> Puzzle-piece' },
                            { value: 'fa fa-qrcode', label: '<i class="fa fa-qrcode"></i> Qrcode' },
                            { value: 'fa fa-question', label: '<i class="fa fa-question"></i> question' },
                            { value: 'fa fa-question-circle', label: '<i class="fa fa-question-circle"></i> Question-circle' },
                            { value: 'fa fa-quote-left', label: '<i class="fa fa-quote-left"></i> Quote-left' },
                            { value: 'fa fa-quote-right', label: '<i class="fa fa-quote-right"></i> Quote-right' },
                            { value: 'fa fa-random', label: '<i class="fa fa-random"></i> Random' },
                            { value: 'fa fa-recycle', label: '<i class="fa fa-recycle"></i> Recycle' },
                            { value: 'fa fa-refresh', label: '<i class="fa fa-refresh"></i> Refresh' },
                            { value: 'fa fa-remove', label: '<i class="fa fa-remove"></i> Remove' },
                            { value: 'fa fa-reorder', label: '<i class="fa fa-reorder"></i> Reorder' },
                            { value: 'fa fa-reply', label: '<i class="fa fa-reply"></i> Reply' },
                            { value: 'fa fa-reply-all', label: '<i class="fa fa-reply-all"></i> Reply-all' },
                            { value: 'fa fa-retweet', label: '<i class="fa fa-retweet"></i> Retweet' },
                            { value: 'fa fa-road', label: '<i class="fa fa-road"></i> Road' },
                            { value: 'fa fa-rocket', label: '<i class="fa fa-rocket"></i> Rocket' },
                            { value: 'fa fa-rss', label: '<i class="fa fa-rss"></i> Rss' },
                            { value: 'fa fa-rss-square', label: '<i class="fa fa-rss-square"></i> Rss-square' },
                            { value: 'fa fa-search', label: '<i class="fa fa-search"></i> Search' },
                            { value: 'fa fa-search-minus', label: '<i class="fa fa-search-minus"></i> Search-minus' },
                            { value: 'fa fa-search-plus', label: '<i class="fa fa-search-plus"></i> Search-plus' },
                            { value: 'fa fa-send', label: '<i class="fa fa-send"></i> Send' },
                            { value: 'fa fa-send-o', label: '<i class="fa fa-send-o"></i> Send-o' },
                            { value: 'fa fa-share', label: '<i class="fa fa-share"></i> Share' },
                            { value: 'fa fa-square-o', label: '<i class=""></i> ' },
                            { value: 'fa fa-share-alt', label: '<i class="fa fa-share-alt"></i> Share-alt' },
                            { value: 'fa fa-share-alt-square', label: '<i class="fa fa-share-alt-square"></i> Share-alt-square' },
                            { value: 'fa fa-share-square', label: '<i class="fa fa-share-square"></i> Share-square' },
                            { value: 'fa fa-share-square-o', label: '<i class="fa fa-share-square-o"></i> Share-square-o' },
                            { value: 'fa fa-shield', label: '<i class="fa fa-shield"></i> Shield' },
                            { value: 'fa fa-shopping-cart', label: '<i class="fa fa-shopping-cart"></i> Shopping-cart' },
                            { value: 'fa fa-sign-in', label: '<i class="fa fa-sign-in"></i> Sign-in' },
                            { value: 'fa fa-sign-out', label: '<i class="fa fa-sign-out"></i> Sign-out' },
                            { value: 'fa fa-signal', label: '<i class="fa fa-signal"></i> Signal' },
                            { value: 'fa fa-sitemap', label: '<i class="fa fa-sitemap"></i> Sitemap' },
                            { value: 'fa fa-sliders', label: '<i class="fa fa-sliders"></i> Sliders' },
                            { value: 'fa fa-smile-o', label: '<i class="fa fa-smile-o"></i> Smile-o' },
                            { value: 'fa fa-soccer-ball-o', label: '<i class="fa fa-soccer-ball-o"></i> Soccer-ball-o' },
                            { value: 'fa fa-sort', label: '<i class="fa fa-sort"></i> Sort' },
                            { value: 'fa fa-sort-alpha-asc', label: '<i class="fa fa-sort-alpha-asc"></i> Sort-alpha-asc' },
                            { value: 'fa fa-sort-alpha-desc', label: '<i class="fa fa-sort-alpha-desc"></i> Sort-alpha-desc' },
                            { value: 'fa fa-sort-amount-asc', label: '<i class="fa fa-sort-amount-asc"></i> Sort-amount-asc' },
                            { value: 'fa fa-sort-amount-desc', label: '<i class="fa fa-sort-amount-desc"></i> Sort-amount-desc' },
                            { value: 'fa fa-sort-asc', label: '<i class="fa fa-sort-asc"></i> Sort-asc' },
                            { value: 'fa fa-sort-desc', label: '<i class="fa fa-sort-desc"></i> Sort-desc' },
                            { value: 'fa fa-sort-down', label: '<i class="fa fa-sort-down"></i> Sort-down' },
                            { value: 'fa fa-sort-numeric-asc', label: '<i class="fa fa-sort-numeric-asc"></i> Sort-numeric-asc' },
                            { value: 'fa fa-sort-numeric-desc', label: '<i class="fa fa-sort-numeric-desc"></i> Sort-numeric-desc' },
                            { value: 'fa fa-sort-up', label: '<i class="fa fa-sort-up"></i> Sort-up' },
                            { value: 'fa fa-space-shuttle', label: '<i class="fa fa-space-shuttle"></i> Space-shuttle' },
                            { value: 'fa fa-spinner', label: '<i class="fa fa-spinner"></i> Spinner' },
                            { value: 'fa fa-spoon', label: '<i class="fa fa-spoon"></i> Spoon' },
                            { value: 'fa fa-square', label: '<i class=""></i> ' },
                            { value: 'fa fa-square-o', label: '<i class=""></i> ' },
                            { value: 'fa fa-star', label: '<i class="fa fa-star"></i> Star' },
                            { value: 'fa fa-star-half', label: '<i class="fa fa-star-half"></i> Star-half' },
                            { value: 'fa fa-star-half-empty', label: '<i class="fa fa-star-half-empty"></i> Star-half-empty' },
                            { value: 'fa fa-star-half-full', label: '<i class="fa fa-star-half-full"></i> Star-half-full' },
                            { value: 'fa fa-star-half-o', label: '<i class="fa fa-star-half-o"></i> Star-half-o' },
                            { value: 'fa fa-star-o', label: '<i class="fa fa-star-o"></i> Star-o' },
                            { value: 'fa fa-suitcase', label: '<i class="fa fa-suitcase"></i> Suitcase' },
                            { value: 'fa fa-sun-o', label: '<i class="fa fa-sun-o"></i> Sun-o' },
                            { value: 'fa fa-support', label: '<i class="fa fa-support"></i> Support' },
                            { value: 'fa fa-tablet', label: '<i class="fa fa-tablet"></i> Tablet' },
                            { value: 'fa fa-tachometer', label: '<i class="fa fa-tachometer"></i> Tachometer' },
                            { value: 'fa fa-tag', label: '<i class="fa fa-tag"></i> Tag' },
                            { value: 'fa fa-tags', label: '<i class="fa fa-tags"></i> Tags' },
                            { value: 'fa fa-tasks', label: '<i class="fa fa-tasks"></i> Tasks' },
                            { value: 'fa fa-taxi', label: '<i class="fa fa-taxi"></i> Taxi' },
                            { value: 'fa fa-terminal', label: '<i class="fa fa-terminal"></i> Terminal' },
                            { value: 'fa fa-thumb-tack', label: '<i class="fa fa-thumb-tack"></i> Thumb-tack' },
                            { value: 'fa fa-thumbs-down', label: '<i class="fa fa-thumbs-down"></i> Thumbs-down' },
                            { value: 'fa fa-thumbs-o-down', label: '<i class="fa fa-thumbs-o-down"></i> Thumbs-o-down' },
                            { value: 'fa fa-thumbs-o-up', label: '<i class="fa fa-thumbs-o-up"></i> Thumbs-o-up' },
                            { value: 'fa fa-thumbs-up', label: '<i class="fa fa-thumbs-up"></i> Thumbs-up' },
                            { value: 'fa fa-ticket', label: '<i class="fa fa-ticket"></i> Ticket' },
                            { value: 'fa fa-times', label: '<i class="fa fa-times"></i> Times' },
                            { value: 'fa fa-times-circle', label: '<i class="fa fa-times-circle"></i> Times-circle' },
                            { value: 'fa fa-times-circle-o', label: '<i class="fa fa-times-circle-o"></i> Times-circle-o' },
                            { value: 'fa fa-tint', label: '<i class="fa fa-tint"></i> Tint' },
                            { value: 'fa fa-toggle-down', label: '<i class="fa fa-toggle-down"></i> Toggle-down' },
                            { value: 'fa fa-toggle-left', label: '<i class="fa fa-toggle-left"></i> Toggle-left' },
                            { value: 'fa fa-toggle-off', label: '<i class="fa fa-toggle-off"></i> Toggle-off' },
                            { value: 'fa fa-toggle-on', label: '<i class="fa fa-toggle-on"></i> Toggle-on' },
                            { value: 'fa fa-toggle-right', label: '<i class="fa fa-toggle-right"></i> Toggle-right' },
                            { value: 'fa fa-toggle-up', label: '<i class="fa fa-toggle-up"></i> Toggle-up' },
                            { value: 'fa fa-trash', label: '<i class="fa fa-trash"></i> Trash' },
                            { value: 'fa fa-trash-o', label: '<i class="fa fa-trash-o"></i> Trash-o' },
                            { value: 'fa fa-tree', label: '<i class="fa fa-tree"></i> Tree' },
                            { value: 'fa fa-trophy', label: '<i class="fa fa-trophy"></i> Trophy' },
                            { value: 'fa fa-truck', label: '<i class="fa fa-truck"></i> Truck' },
                            { value: 'fa fa-tty', label: '<i class="fa fa-tty"></i> Tty' },
                            { value: 'fa fa-umbrella', label: '<i class="fa fa-umbrella"></i> Umbrella' },
                            { value: 'fa fa-university', label: '<i class="fa fa-university"></i> University' },
                            { value: 'fa fa-unlock', label: '<i class="fa fa-unlock"></i> Unlock' },
                            { value: 'fa fa-unlock-alt', label: '<i class="fa fa-unlock-alt"></i> Unlock-alt' },
                            { value: 'fa fa-unsorted', label: '<i class="fa fa-unsorted"></i> Unsorted' },
                            { value: 'fa fa-upload', label: '<i class="fa fa-upload"></i> fa fa-upload' },
                            { value: 'fa fa-user', label: '<i class="fa fa-user"></i>User' },
                            { value: 'fa fa-users', label: '<i class="fa fa-users"></i> Users' },
                            { value: 'fa fa-video-camera', label: '<i class="fa fa-video-camera"></i> Video-camera' },
                            { value: 'fa fa-volume-down', label: '<i class="fa fa-volume-down"></i> Volume-down' },
                            { value: 'fa fa-volume-off', label: '<i class="fa fa-volume-off"></i> Volume-off' },
                            { value: 'fa fa-volume-up', label: '<i class="fa fa-volume-up"></i> Volume-up' },
                            { value: 'fa fa-warning', label: '<i class="fa fa-warning"></i> Warning' },
                            { value: 'fa fa-wheelchair', label: '<i class="fa fa-wheelchair"></i> fa fa-wheelchair' },
                            { value: 'fa fa-wifi', label: '<i class="fa fa-wifi"></i> Wifi' },
                            { value: 'fa fa-wrench', label: '<i class="fa fa-wrench"></i> Wrench' }
            ];
        };
    });

    function AddSideMenu(item) {
        // Side Menu update
        var sidemenuid = guidGenerator();
        var sideMenuHtml = "<li class=" + sidemenuid + "> <a class='" + item.MenuIdentifier + "' href='" + item.PageUrl + "'> <i class='fa " + item.Icon + "'> </i> <span>" + item.Name + "</span></a>";
        //console.log(item);
        //console.log(item.SubMenu);
        if (item.SubMenu != null && item.SubMenu.length > 0) {
            sideMenuHtml = "<li class=" + sidemenuid + "> <a class='" + item.MenuIdentifier + "'> <i class='fa " + item.Icon + "'> </i> " + item.Name + "<span class='fa fa-chevron-down'></span></a>";
            sideMenuHtml += "<ul class='nav child_menu' style='display:none'>"

            $(item.SubMenu).each(function (i, subitem) {
                //console.log(subitem);
                sideMenuHtml += "<li class='" + subitem.MenuIdentifier + "'> <a href='" + subitem.PageUrl + "'> " + subitem.Name + "</a> </li>"
            });
            sideMenuHtml += "</ul>"
        }
        sideMenuHtml += "</li>";
        //console.log(sideMenuHtml);
        $("#dvSideMenu").append(sideMenuHtml);

        $('#sidebar-menu li.' + sidemenuid).click(function () {
            SetMenuAttribute(this);
        });
    }

    function guidGenerator() {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    }

    function SetMenuAttribute(current) {
        if ($(current).is('.active')) {
            $(current).removeClass('active');
            $('ul', current).slideUp();
            $(current).removeClass('nv');
            $(current).addClass('vn');
        } else {
            //-- $('#sidebar-menu li ul').slideUp();
            $(current).removeClass('vn');
            $(current).addClass('nv');
            $('ul', current).slideDown();
            $('#sidebar-menu li').removeClass('active');
            $(current).addClass('active');
        }
    }
})();
