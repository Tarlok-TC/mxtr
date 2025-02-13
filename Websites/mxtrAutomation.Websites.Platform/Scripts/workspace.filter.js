

$(document).ready(function () {

    ///////////
    // CLICK FUNCTIONS
    // SETUP ACTIONS

    var actions = {
        open: function () {
            openMyMenu(this);
        },
        close: function () { }
    };

    // BIND ACTIONS TO ELEMENTS

    //These two methods are used to hide and display the "Search" on the search field.
    $("input.filter-search-input").focus(function () {
        $("label.filter-search-lbl").hide();
    });

    $("input.filter-search-input").blur(function () {
        if ($(this).val() == "") {
            $("label.filter-search-lbl").show();
        }
    });

    $(document).click(function (e) {
       //alert(e.target.id);
        if ($(e.target).closest('#account-drop-down').length) {
            //alert(6);
            return;
        }
        if ($(e.target).closest('.vw-doc-clk').length) {
            return;
        }
        if (e.target.id != "account-drop-down" && e.target.id != "dvFilterDrpdown") {
            //alert(5);
            $("#dvFilterDrpdown").removeClass('openCloseMenu');
            hideMyMenu();
        }
       
    });

    //--remove  MakeParentUnselect if we need parent account selected as well 22/06/2017 (Jira MXTR-122)
    MakeParentUnselect();

});// END DOC READY

function MakeParentUnselect() {
    var scope = angular.element(document.getElementById("WorkspaceFilter")).scope();
    scope.$apply(function () {
        scope.selectParentAccount(scope.parentId);
    });
}

function showHideMenu(drpFilter) {

    if ($(drpFilter).hasClass('openCloseMenu')) {
        $(drpFilter).removeClass('openCloseMenu');
        hideMyMenu();
    }
    else {
        $(drpFilter).addClass('openCloseMenu');
        showMyMenu();
    }
}

// Account Filter Util Functions
function showMyMenu() {
    $("#WorkspaceFilter").show();
    var parent = $('.m-report-filter');
    var dropMenu = $(parent).find('.m-report-filter-drop-container');
    var button = $(parent).find('.m-report-filter-click');
    $(dropMenu).fadeIn(200, function () {
        $(button).addClass('is-active')
        $(button).data('action', 'close');
    });
}

function hideMyMenu() {
    $("#WorkspaceFilter").hide();
    var parent = $('.m-report-filter');
    var dropMenu = $(parent).find('.m-report-filter-drop-container');
    var button = $(parent).find('.m-report-filter-click');    

    $(dropMenu).fadeOut(200, function () {
        $(button).removeClass('is-active');
        $(button).data('action', 'open');
    });
}


function openMyMenu(elem) {    
    var parent = $(elem).parents('.m-report-filter');
    var dropMenu = $(parent).find('.m-report-filter-drop-container');
    var button = $(parent).find('.m-report-filter-click');
    $(dropMenu).fadeIn(200, function () {
        $(button).addClass('is-active')
        $(button).data('action', 'close');
    });
};

function closeMyMenu(elem) {
    //event.preventDefault();
    var parent = $(elem).parents('.m-report-filter');
    var dropMenu = $(parent).find('.m-report-filter-drop-container');
    var button = $(parent).find('.m-report-filter-click');
    //$('.filter-search-lbl').show();

    $(dropMenu).fadeOut(200, function () {
        $(button).removeClass('is-active');
        $(button).data('action', 'open');
    });
}


//global objects
var _selectAllAccountTrackingObj = {
    lastApplied: 'select-all-checked fa fa-check-circle',
    lastSelected: 'select-all-checked fa fa-check-circle'
};

var _initialLoad = false;

// ANGULAR

var reportFilter = angular.module('reportFilter', ['ui.unique', 'ui.highlight']);

reportFilter.controller('ReportFilterCtrl', ['$scope', function ($scope) {
    $scope.accounts = _workspaces.ChildAccounts;

    $scope.isChild = false;
    $scope.clicked = false;
    $scope.selectionClass = 'filter-check-checked';
    $scope.selectAllAccountsStatus = 'select-all-checked fa fa-check-circle';

    $scope.parentAccountName = _workspaces.AccountName;
    $scope.parentId = _workspaces.AccountObjectID;
    $scope.parentClass = _workspaces.SelectedClass;

    $scope.setDisplayedFilters = function (accountId) {
        $.each($scope.accounts, function (i, item) {
            if (item.AccountObjectID == accountId) {
                if (item.ChildAccounts != null) {

                    //Variables needed to display information about the parent in the child template.
                    $scope.parentAccountName = item.AccountName;
                    $scope.parentId = item.AccountObjectID;
                    $scope.parentClass = item.SelectedClass;

                    //Adds the current view to a stack of views and assigns a new view to be displayed.  
                    //isChild is used to determine whether to use the child template or base template
                    parentAccountArray.push($scope.accounts);
                    $scope.accounts = item.ChildAccounts;
                    $scope.isChild = true;
                }
                return false;
            }
        });


    };

    $scope.returnToPreviousAccount = function (accountId) {
        if (parentAccountArray.length != 0) {
            var n = (parentAccountArray.length - 1);

            $scope.accounts = parentAccountArray[n];

            parentAccountArray.pop();

            $.each($scope.accounts, function (i, item) {
                if (item.AccountObjectID == accountId) {
                    $scope.parentAccountName = item.ParentAccountName;
                    $scope.parentId = item.ParentAccountObjectID;
                    $scope.parentClass = item.SelectedClass;
                }

                //Uses top level "you" account
                if (parentAccountArray.length == 0) {
                    $scope.isChild = false;
                    $scope.parentAccountName = _workspaces.AccountName;
                    $scope.parentId = _workspaces.AccountObjectID;
                    $scope.parentClass = _workspaces.SelectedClass;
                }
            });
        }
    };

    $scope.selectAllAccounts = function (selectAllAccountsStatus) {
        selectEachAccount(_workspaces.ChildAccounts, selectAllAccountsStatus);

        if ($scope.selectAllAccountsStatus == 'select-all-unchecked fa fa-circle-o') {
            //Checks the select all checkbox.
            $scope.selectAllAccountsStatus = 'select-all-checked fa fa-check-circle';

            //Checks the "you" account.
            _workspaces.SelectedClass = 'filter-check-all fa fa-check-circle';

            //Checks the current parent class (visually only).
            $scope.parentClass = 'filter-check-all fa fa-check-circle';

            _selectAllAccountTrackingObj.lastSelected = 'select-all-checked fa fa-check-circle';
        }
        else {
            //Unchecks the select all checkbox.
            $scope.selectAllAccountsStatus = 'select-all-unchecked fa fa-circle-o';

            //Unchecks the "you" account.
            _workspaces.SelectedClass = 'filter-check-none fa fa-circle-o';

            //Unchecks the current parent class (visually only).
            $scope.parentClass = 'filter-check-none fa fa-circle-o';

            _selectAllAccountTrackingObj.lastSelected = 'select-all-unchecked fa fa-circle-o';
        }
    };

    $scope.selectParentAccount = function (accountID) {
        //If parentAccountArray length is 0, set parent account to top-level "you" account.
        if (parentAccountArray.length == 0) {
            if ($scope.parentClass != 'filter-check-all fa fa-check-circle') {
                //25/07/2017 parent at root getting selected when moving between pages
                //_workspaces.SelectedClass = 'filter-check-all fa fa-check-circle';
                //$scope.parentClass = 'filter-check-all fa fa-check-circle';
            }
            else {
                _workspaces.SelectedClass = 'filter-check-none fa fa-circle-o';
                $scope.parentClass = 'filter-check-none fa fa-circle-o';
            }
        }
        else {
            $.each(parentAccountArray[parentAccountArray.length - 1], function (i, item) {
                if (accountID == item.AccountObjectID) {
                    if ($scope.parentClass != 'filter-check-all fa fa-check-circle') {
                        item.SelectedClass = 'filter-check-all fa fa-check-circle';
                        $scope.parentClass = 'filter-check-all fa fa-check-circle';
                    }
                    else {
                        item.SelectedClass = 'filter-check-none fa fa-circle-o';
                        $scope.parentClass = 'filter-check-none fa fa-circle-o';
                    }
                }
            });
        }
    };

    $scope.selectAccount = function (accountID) {
        var checkedValue = false;
        var uncheckedValue = false;
        var parentID;

        $.each($scope.accounts, function (i, item) {

            if (accountID == item.AccountObjectID) {
                if (item.SelectedClass != 'filter-check-all fa fa-check-circle') {
                    item.SelectedClass = 'filter-check-all fa fa-check-circle';
                }
                else {

                    item.SelectedClass = 'filter-check-none fa fa-circle-o';
                    $scope.selectAllAccountsStatus = 'select-all-unchecked fa fa-circle-o';
                    _selectAllAccountTrackingObj.lastSelected = 'select-all-unchecked fa fa-circle-o';
                }

                parentID = item.ParentAccountObjectID;
            }

            if (item.SelectedClass == 'filter-check-all fa fa-check-circle') {
                if (item.ChildrenClass == 'has-children' && item.SelectedChildClass == 'filter-check-none fa fa-circle-o') {
                    uncheckedValue = true;
                }
                checkedValue = true;
            }
            else if (item.SelectedClass == 'filter-check-none fa fa-circle-o') {
                if (item.ChildrenClass == 'has-children' && item.SelectedChildClass == 'filter-check-all fa fa-check-circle') {
                    checkedValue = true;
                }
                uncheckedValue = true;
            }
            else if (item.SelectedChildClass == 'filter-check-some' && item.ChildrenClass == 'has-children') {
                checkedValue = true;
                uncheckedValue = true;
            }
        });

        if (parentAccountArray.length != 0) {
            bubbleUpSelection(parentID, checkedValue, uncheckedValue);
        }

    };

    $scope.selectChildAccounts = function (accountID, originState) {
        var checkedValue = false;
        var uncheckedValue = false;
        var parentID;

        $.each($scope.accounts, function (i, item) {
            if (accountID == item.AccountObjectID) {
                selectAllChildAccounts(item.ChildAccounts, accountID, originState);

                if (originState != 'filter-check-all fa fa-check-circle') {
                    item.SelectedChildClass = 'filter-check-all fa fa-check-circle';
                }
                else if (originState == 'filter-check-all fa fa-check-circle') {
                    item.SelectedChildClass = 'filter-check-none fa fa-circle-o';
                }
            }

            if (item.SelectedClass == 'filter-check-all fa fa-check-circle') {
                if (item.ChildrenClass == 'has-children' && item.SelectedChildClass == 'filter-check-none fa fa-circle-o') {
                    uncheckedValue = true;
                }
                checkedValue = true;
            }
            else if (item.SelectedClass == 'filter-check-none fa fa-circle-o') {
                if (item.ChildrenClass == 'has-children' && item.SelectedChildClass == 'filter-check-all fa fa-check-circle') {
                    checkedValue = true;
                }
                uncheckedValue = true;
            }
            else if (item.SelectedChildClass == 'filter-check-some' && item.ChildrenClass == 'has-children') {
                checkedValue = true;
                uncheckedValue = true;
            }

            parentID = item.ParentAccountObjectID;
        });

        if (parentAccountArray.length != 0) {
            bubbleUpSelection(parentID, checkedValue, uncheckedValue);
        }
    }

    $scope.cancelAccountSelection = function () {
        //Resets the view of the drop down.
        restoreAccountsOnCancel(_workspaces.ChildAccounts);
        $scope.accounts = _workspaces.ChildAccounts;
        $scope.isChild = false;

        //Sets the parent account information to the top-level "you" account.
        $scope.parentAccountName = _workspaces.AccountName;
        $scope.parentId = _workspaces.AccountObjectID;
        $scope.parentClass = _workspaces.SelectedClass;

        //Clears the stack of accounts that were navigated through.
        parentAccountArray = [];

        _selectAllAccountTrackingObj.lastSelected = _selectAllAccountTrackingObj.lastApplied;
        $scope.selectAllAccountsStatus = _selectAllAccountTrackingObj.lastSelected;

        $scope.accountQuery = "";

        $("#dvFilterDrpdown").removeClass('openCloseMenu');
        hideMyMenu();
    };

    $scope.applyAccountSelection = function () {
        selectedAccountString = "";
        appliedAccountsState = [];

        //Recursive method to apply any selected accounts and children.
        applyAccountsAndChildren(_workspaces.ChildAccounts);

        //Checks to see if top-level "you" account should be added.
        if (_workspaces.SelectedClass == 'filter-check-all fa fa-check-circle') {

            appliedAccountsState.push({ "AcctID": _workspaces.AccountObjectID, "ChildSelClass": _workspaces.SelectedChildClass });

            selectedAccountString = selectedAccountString + _workspaces.AccountObjectID + ",";
        }

        selectedAccountString = selectedAccountString.slice(0, -1);


        _selectAllAccountTrackingObj.lastApplied = _selectAllAccountTrackingObj.lastSelected;
        $scope.selectAllAccountsStatus = _selectAllAccountTrackingObj.lastApplied;

        applyFilters();
        $("#dvFilterDrpdown").removeClass('openCloseMenu');
        hideMyMenu();
    };

}]);


function bubbleUpSelection(parentID, checkedValue, uncheckedValue) {
    parentAccountArray.reverse();

    if ((checkedValue == true && uncheckedValue == true)) {
        selectedClass = 'filter-check-some'
    }
    else if (checkedValue == true && uncheckedValue == false) {
        selectedClass = 'filter-check-all fa fa-check-circle';
    }
    else if (checkedValue == false && uncheckedValue == true) {
        selectedClass = 'filter-check-none fa fa-circle-o';
    }

    $.each(parentAccountArray, function (a, array) {
        $.each(array, function (p, parent) {
            if (parentID == parent.AccountObjectID) {
                parent.SelectedChildClass = selectedClass;
                parentID = parent.ParentAccountObjectID;
                return false;
            }
        });
    });

    parentAccountArray.reverse();
}

function selectAllChildAccounts(accounts, accountID, originState) {
    $.each(accounts, function (i, item) {
        if (item.AccountObjectID == accountID || item.ParentAccountObjectID == accountID) {
            if (item.ChildAccounts != null) {
                selectAllChildAccounts(item.ChildAccounts, item.AccountObjectID, originState);
            }

            if (item.SelectedClass != 'filter-check-all fa fa-check-circle' && originState != 'filter-check-all fa fa-check-circle') {
                item.SelectedClass = 'filter-check-all fa fa-check-circle';
                item.SelectedChildClass = 'filter-check-all fa fa-check-circle';
            }
            else if (item.SelectedClass == 'filter-check-all fa fa-check-circle' && originState == 'filter-check-all fa fa-check-circle') {
                item.SelectedClass = 'filter-check-none fa fa-circle-o';
                item.SelectedChildClass = 'filter-check-none fa fa-circle-o';
            }
        }
    });
}

function selectEachAccount(accounts, selectAllAccountsStatus) {
    $.each(accounts, function (i, item) {
        if (item.ChildAccounts != null) {
            selectEachAccount(item.ChildAccounts, selectAllAccountsStatus);
        }

        if (item.SelectedClass != 'filter-check-all fa fa-check-circle' && selectAllAccountsStatus != 'select-all-checked fa fa-check-circle') {
            item.SelectedChildClass = 'filter-check-all fa fa-check-circle';
            item.SelectedClass = 'filter-check-all fa fa-check-circle';
        }
        else if (item.SelectedClass == 'filter-check-all fa fa-check-circle' && selectAllAccountsStatus == 'select-all-checked fa fa-check-circle') {
            item.SelectedChildClass = 'filter-check-none fa fa-circle-o';
            item.SelectedClass = 'filter-check-none fa fa-circle-o';
        }
    });
}

function restoreAccountsOnCancel(filterResults) {
    $.each(filterResults, function (a, account) {
        if (account.ChildAccounts != null) {
            restoreAccountsOnCancel(account.ChildAccounts);
        }

        if (appliedAccountsState.length != 0) {
            $.each(appliedAccountsState, function (i, item) {
                if (account.AccountObjectID == item.AcctID) {
                    if (account.SelectedClass != 'filter-check-all fa fa-check-circle') {
                        account.SelectedChildClass = item.ChildSelClass;
                        account.SelectedClass = 'filter-check-all fa fa-check-circle';
                    }
                    return false;
                }
                else {
                    account.SelectedClass = 'filter-check-none fa fa-circle-o';
                }
            });
        }
        else if (_initialLoad) {
            account.SelectedClass = 'filter-check-all fa fa-check-circle';
            account.SelectedChildClass = 'filter-check-all fa fa-check-circle';
            _workspaces.SelectedClass = 'filter-check-all fa fa-check-circle';
        }
    });
}

function applyAccountsAndChildren(filterResults) {
    $.each(filterResults, function (i, item) {
        if (item.ChildAccounts != null) {
            applyAccountsAndChildren(item.ChildAccounts);
        }

        if (item.SelectedClass == 'filter-check-all fa fa-check-circle') {
            appliedAccountsState.push({ "AcctID": item.AccountObjectID, "ChildSelClass": item.SelectedChildClass });
            selectedAccountString = selectedAccountString + item.AccountObjectID + ",";
        }
    });

}



//BUILDS FILTER GRAPH AND SENDS TO CALLBACK FUNCTION
function applyFilters() {
    _initialLoad = false;
    _filterGraph.accountIDs = selectedAccountString;

    executeCallbackFromFilters(_callBackFunction);
}