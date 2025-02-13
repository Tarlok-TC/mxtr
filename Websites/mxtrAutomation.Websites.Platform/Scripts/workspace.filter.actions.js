var actIds = '';
var actIdCounter = 0;
function GetWorkspaceFilterIds() {
    actIds = '';
    actIdCounter = 0;
    //parent account
    if (checkIsAccountSelected(_workspaces.SelectedClass)) {
        actIds += _workspaces.AccountObjectID + ",";
        if (_workspaces.AccountType == "Client") {
            actIdCounter += 1;
        }
    }
    // actIdCounter += 1;
    //child account
    $.each(_workspaces.ChildAccounts, function (i, item) {
        //    actIdCounter += 1;       
        if (checkIsAccountSelected(item.SelectedClass)) {
            actIds += item.AccountObjectID + ",";
            if (item.AccountType == "Client") {
                actIdCounter += 1;
            }
        }
        //check if item has child accounts
        if (item.ChildAccounts != null && item.ChildAccounts.length > 0) {
            IterateChildAccount(item);
        }

    });
    if (!String.prototype.endsWith) {
        String.prototype.endsWith = function (suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        };
    }
    if (actIds.endsWith(",")) {
        actIds = actIds.substring(0, actIds.length - 1);
    }

    return actIds;
}

function IterateChildAccount(item) {
    $.each(item.ChildAccounts, function (i, childItem) {
        //    actIdCounter += 1;    
        if (checkIsAccountSelected(childItem.SelectedClass)) {
            if (childItem.AccountType == "Client") {
                actIdCounter += 1;
            }
            actIds += childItem.AccountObjectID + ",";
        }
        if (childItem.ChildAccounts != null && childItem.ChildAccounts.length > 0) {
            //call recursively to iterate  the nth child
            IterateChildAccount(childItem);
        }
    });
}

function checkIsAccountSelected(className) {
    var selectedClassArray = className.split(" ");
    var isSelected = false;
    $.each(selectedClassArray, function (i, className) {
        if (className == 'fa-check-circle') {
            isSelected = true;
        }
    });
    return isSelected;
}

function GetSelectAccountIdText(accountIds) {
    var selectedIdText = '';
    if (accountIds.length) {
        //var accountIdSelectedCount = accountIds.split(",").length;
        //if (actIdCounter == accountIdSelectedCount) {
        //    selectedIdText = "All";
        //}
        //else {
        //    //selectedIdText = accountIdSelectedCount;
        //    selectedIdText = actIdCounter;
        //}
        selectedIdText = actIdCounter;
    }
    else {
        selectedIdText = "None";
    }
    return selectedIdText;
}

function SetGlobalAccountId_Workspace() {
    sessionStorage.clear();
    var Storage = window.sessionStorage;
    if (typeof (Storage) !== "undefined") {
        sessionStorage.setItem("workspacesAccountIdsCache", JSON.stringify(_workspaces));
    } else {
        alert("Sorry, your browser does not support Web Storage...");
    }
}

function GetGlobalAccountId_Workspace() {
    sessionStorage.clear();
    var Storage = window.sessionStorage;
    if (typeof (Storage) !== "undefined") {
        var dataFromCache = sessionStorage.getItem("workspacesAccountIdsCache");
        if (dataFromCache == null) {
        }
        else {
            _workspaces = JSON.parse(sessionStorage.getItem("workspacesAccountIdsCache"));
        }
    } else {
        alert("Sorry, your browser does not support Web Storage...");
    }
}