$(document).ready(function () {

    //General setup
    var accountInformationNotification = $('#account-information-notification');
    var _mxtrAccountID = $('#MxtrAccountID').val();
    var _accountObjectID = $('#ObjectID').val();
    var userAccountNotification = $('#user-account-notification');

    $('#wizard').smartWizard({
        enableAllSteps: false,
        showActionBar: true,
        keyNavigation: false,
        selected: 0
    });

    $(".select2_single").select2({
        allowClear: true
    });

    $('#MoveToAccount').change(function () {
        newID = $(this).val();
        $('#ParentAccountObjectID').val(newID);
    });

    //Step 1 functions - Create Account
    $('.account-information-form').validate({
        ignore: [],
        onKeyUp: false,
        messages: {
            AccountName: 'Please enter a company name',
            Phone: 'Please enter a phone number',
            StreetAddress: 'Please enter a street address.',
            City: 'Please enter a city.',
            State: 'Please choose a state.',
            ZipCode: 'Please enter a zip.',
            Country: 'Please choose a country.',
            AccountType: 'Please choose an account type.'
        },
        submitHandler: function (form) {
            var formLoader = $(form).find('.form-sending');
            $(formLoader).fadeIn('fast');

            $.ajax({
                url: $(form).attr('action'),
                dataType: 'json',
                type: 'post',
                data: serializeForm(form),
                success: function (result) {
                    if (result.Success) {
                        _mxtrAccountID = result.MxtrAccountID;
                        _accountObjectID = result.AccountObjectID;

                        notificationBarSuccess(accountInformationNotification, 'The account information has been saved.');
                        setTimeout(function () {
                            timeoutHideNotificationBar(accountInformationNotification);
                            disableButton($('#CreateAccountSubmit'), 'btn-default');
                            enableButton($('#SkipCreateAccount'), 'btn-info');
                            progressStep(2);
                        }, 2000);
                    }
                    else {
                        if (result.AccountObjectID != "") {
                            notificationBarFailure(accountInformationNotification, result.AccountObjectID);
                        }
                        else {
                            notificationBarFailure(accountInformationNotification, 'There was a problem saving the account information.');
                        }
                    }
                    $(formLoader).fadeOut();
                }
            });
        }
    });

    $(document).on('click', '#SkipCreateAccount', function (event) {
        event.preventDefault();
        progressStep(2);
    });


    //Step 2 functions - Assign Account Attributes
    $('.account-attributes-form').validate({
        ignore: [],
        onKeyUp: false,
        submitHandler: function (form) {
            var formLoader = $(form).find('.form-sending');
            $(formLoader).fadeIn('fast');

            var data =
            {
                'ObjectID': _accountObjectID,
                'MxtrAccountID': _mxtrAccountID,
                'SharpspringSecretKey': $('#SharpspringSecretKey').val(),
                'SharpspringAccountID': $('#SharpspringAccountID').val(),
                'BullseyeClientId': $('#BullseyeClientId').val(),
                'BullseyeAdminApiKey': $('#BullseyeAdminApiKey').val(),
                'BullseyeSearchApiKey': $('#BullseyeSearchApiKey').val(),
                'BullseyeLocationId': $('#BullseyeLocationId').val(),
                'BullseyeThirdPartyId': $('#BullseyeThirdPartyId').val(),
                'WebsiteUrl': $('#WebsiteUrl').val(),
                'GoogleAnalyticsReportingViewId': $('#GoogleAnalyticsReportingViewId').val(),
                'GoogleAnalyticsTimeZoneName': $('#GoogleAnalyticsTimeZoneName').val(),
                'GoogleServiceAccountCredentialFile': $('#GoogleServiceAccountCredentialFile').val(),
                'GoogleServiceAccountEmail': $('#GoogleServiceAccountEmail').val(),
                'GAProfileName': $('#GAProfileName').val(),
                'GAWebsiteUrl': $('#GAWebsiteUrl').val(),
                'EZShredIP': $('#EZShredIP').val(),
                'EZShredPort': $('#EZShredPort').val(),
                'KlipfolioCompanyID': $('#KlipfolioCompanyID').val(),
                'KlipfolioSSOSecretKey': $('#KlipfolioSSOSecretKey').val(),
                'DealerId': $('#DealerId').val(),
                'SSLead': $('#SSLead').val(),
                'SSContact': $('#SSContact').val(),
                'SSQuoteSent': $('#SSQuoteSent').val(),
                'SSWonNotScheduled': $('#SSWonNotScheduled').val(),
                'SSClosed': $('#SSClosed').val(),
            };

            $.ajax({
                url: $(form).attr('action'),
                dataType: 'json',
                type: 'post',
                data: data,
                success: function (result) {
                    if (result.Success) {
                        ShowMarketPlaces(result.EZShredIP, result.EZShredPort);
                        notificationBarSuccess(accountInformationNotification, 'The account information has been saved.');
                        setTimeout(function () {
                            timeoutHideNotificationBar(accountInformationNotification);
                            disableButton($('#SaveAttributesSubmit'), 'btn-default');
                            progressStep(3);
                        }, 2000);
                    }
                    else {
                        notificationBarFailure(accountInformationNotification, 'There was a problem saving the account information.');
                    }
                    $(formLoader).fadeOut();
                }
            });
        }
    });

    function ShowMarketPlaces(eZShredIP, eZShredPort) {
        _eZShredIP = $.trim(eZShredIP);
        _eZShredPort = $.trim(eZShredPort);
        if (_eZShredIP != "" && _eZShredIP != "") {
            $("#dvEZShred_IP_Port").show();
        }
        else {
            $("#dvEZShred_IP_Port").hide();
        }
    }

    ShowMarketPlaces(_eZShredIP, _eZShredPort);

    $(document).on('click', '#SkipAccountAttributes', function (event) {
        event.preventDefault();
        progressStep(3);
    });


    //Step 3 functions - Create Account Users
    var _permissionTooltipText = '';

    $('.js-check-change').change(function () {
        updatePermissions();
    });

    function updatePermissions() {
        var arr = new Array();
        var friendlyArr = new Array();

        $('.js-check-change').each(function (index, element) {
            if (element.checked) {
                friendlyArr.push($(element).data('friendlyname'));
                arr.push(element.value);
            }
        });
        $('#Permissions').val(arr.join(','));
        $('#Permissions').trigger('change');
        _permissionTooltipText = friendlyArr.join(', ')
    }

    //angular setup for users
    var AccountUsersApp = angular.module('AccountUsers', []);

    AccountUsersApp.controller('MainCtrl', function ($scope) {
        $scope.items = _users == null ? [] : _users; //[];

        $scope.MakeMultilineTooltiip = function () {
            //--------------- Make permission to display one by one and not comma seperated in a single line------------
            angular.forEach($scope.items, function (value, key) {
                value.permissionsToDisplay = value.Permissions;
                var arrPermission = [];
                if (value.Permissions != null && value.Permissions.length) {
                    arrPermission = value.Permissions.split(',');
                    var displayPermission = '';
                    $.each(arrPermission, function (keyPermission, valuePermission) {
                        var txt = '';
                        switch (valuePermission) {
                            case 'ManageAccountUsers':
                                txt = 'Manage Account & Users';
                                break;
                            case 'ViewHierarchy':
                                txt = 'View Hierarchy';
                                break;
                            case 'CreateDashboard':
                                txt = 'Create Dashboard';
                                break;
                            case 'ViewDashboard':
                                txt = 'View Dashboard';
                                break;
                            case 'ViewAnalytics':
                                txt = 'View Analytics';
                                break;
                            case 'ViewSales':
                                txt = 'View Sales';
                                break;
                                txt = valuePermission;
                            default:
                        }
                        if (arrPermission.indexOf(valuePermission) == 0) {
                            displayPermission = txt + '\n';
                        }
                        else {
                            displayPermission = displayPermission + txt + '\n';
                        }
                    })
                    value.permissionsToDisplay = displayPermission;
                }
            });
            //------------Make permission to display Ended------------------------
        }

        $scope.MakeMultilineTooltiip();

        $scope.itemsToAdd = [{
            FullName: '',
            UserName: '',
            Password: '',
            Phone: '',
            CellPhone: '',
            EZShredAccountMappings: _marketPlaces,
            Role: 'User',
            Permissions: '',
            ObjectID: '',
            SharpspringUserName: '',
            SharpspringPassword: '',
        }];

        $scope.itemToAdd = {
            Role: 'User',
            EZShredAccountMappings: _marketPlaces
        };

        $scope.add = function (itemToAdd) {
            var index = $scope.itemsToAdd.indexOf(itemToAdd);
            $scope.itemsToAdd.splice(index, 1);
            $scope.items.push(angular.copy(itemToAdd))
            $scope.$apply();
        }

        $scope.updateData = function (itemToAdd) {
            //console.log($scope.items);
            $($scope.items).each(function (x, y) {
                if (y.ObjectID == itemToAdd.ObjectID) {
                    $scope.items.splice(x, 1, angular.copy(itemToAdd));
                    return false;
                }
            });
        }

        $scope.updateRole = function (item) {
            if (item.Role == "Admin") {
                $("#rbRoleAdmin").parent().closest('div.iradio_flat').addClass('checked');
                $("#rbRoleUser").parent().closest('div.iradio_flat').removeClass('checked');
                $("#rbRoleAdmin").attr('checked', 'checked');
                $("#rbRoleAdmin").prop('checked', true);

            }
            else {
                $("#rbRoleUser").parent().closest('div.iradio_flat').addClass('checked');
                $("#rbRoleAdmin").parent().closest('div.iradio_flat').removeClass('checked');
                $("#rbRoleUser").attr('checked', 'checked');
                $("#rbRoleUser").prop('checked', true);
            }
        };

        $scope.createUser = function (itemToAdd) {
            $('.account-users-form').validate({
                rules: {
                    ConfirmPassword: {
                        equalTo: "#Password"
                    }
                },
                messages: {
                    ConfirmPassword: {
                        equalTo: "Password and Confirm Password do not match"
                    }
                },
            });

            if ($('.account-users-form').valid()) {

                if (itemToAdd.ObjectID != "" && itemToAdd.ObjectID != null && typeof itemToAdd.ObjectID != "undefined") {
                    QuestionAlert("Update User", "Are you sure you want to update user data ?", function () {

                        CreateUpdateUser(itemToAdd);

                    }, function () {

                    });
                }
                else {
                    CreateUpdateUser(itemToAdd);
                }
            }
        }

        function CreateUpdateUser(itemToAdd) {
            var data =
               {
                   'accountobjectid': _accountObjectID,
                   'mxtraccountid': _mxtrAccountID,
                   'useraccountdata': angular.toJson(itemToAdd),
               };

            $.ajax({
                url: $('.account-users-form').data('submiturl'),
                dataType: 'json',
                type: 'post',
                data: data,
                success: function (result) {
                    if (result.Success) {
                        if (itemToAdd.ObjectID === "" || itemToAdd.ObjectID === null || typeof itemToAdd.ObjectID === "undefined") {
                            notificationBarSuccess(userAccountNotification, 'The account user information has been saved.');
                        }
                        else {
                            notificationBarSuccess(userAccountNotification, 'The account user information has been updated.');
                        }

                        setTimeout(function () {
                            timeoutHideNotificationBar(userAccountNotification);
                            if (itemToAdd.ObjectID === "" || itemToAdd.ObjectID === null || typeof itemToAdd.ObjectID === "undefined") {
                                // Create User
                                itemToAdd.ObjectID = result.UserObjectID;
                                $scope.add(itemToAdd);
                                $scope.emptyUserData();
                            }
                            else {
                                // Update User
                                $scope.updateData(itemToAdd);
                                $scope.closeUserUpdation();
                            }
                            $scope.MakeMultilineTooltiip();

                            enableButton($('#FinishCreateAccountUsers'), 'btn-info');
                        }, 1000);
                    }
                    else {
                        if (result.Message != "") {
                            notificationBarFailure(userAccountNotification, result.Message);
                        }
                        else {
                            notificationBarFailure(userAccountNotification, 'There was a problem saving/updating the account user information.');
                        }

                        setTimeout(function () {
                            timeoutHideNotificationBar(userAccountNotification);
                        }, 1000);
                    }
                }
            });
        }

        $scope.remove = function (item) {
            var index = $scope.items.indexOf(item);
            $scope.items.splice(index, 1);
            $scope.$apply();
        }

        $scope.editUser = function (item) {
            var formLoader = $('.account-users-form').find('#dvdata_processing');
            $(formLoader).fadeIn('fast');
            $('.account-users-form').validate().resetForm();

            $scope.IsUserUpdate = true;
            $("#CreateAccountUserSubmit").text("Update User");

            if (item.Permissions != null && item.Permissions != "") {
                $(".js-check-change").each(function (i, j) {
                    if (item.Permissions.indexOf($(j).val()) > -1) {
                        $(this).attr('checked', false);
                        $(this).prop('checked', false);
                    }
                    else {
                        $(this).attr('checked', true);
                        $(this).prop('checked', true);
                    }
                });

                setTimeout(function () {
                    $('.switchery').trigger('click');
                }, 100);
            }
            else {
                $scope.clearPermissions();
            }

            $scope.updateRole(item);

            var selectedMarketPlaces = _marketPlaces;
            $(selectedMarketPlaces).each(function (i, j) {
                if (item.EZShredAccountMappings == null || item.EZShredAccountMappings.length == 0) {
                    j.EZShredId = "";
                }
                else {
                    $(item.EZShredAccountMappings).each(function (l, m) {
                        if (j.AccountObjectId == m.AccountObjectId) {
                            j.EZShredId = m.EZShredId;
                            return false;
                        }
                        else {
                            j.EZShredId = "";
                        }
                    });
                }
            });


            $scope.itemToAdd = {
                FullName: item.FullName,
                UserName: item.UserName,// No need to update
                Password: '*********',// No need to update so set some default value
                ConfirmPassword: '*********',// No need to update so set some default value
                Phone: item.Phone,
                CellPhone: item.CellPhone,
                EZShredAccountMappings: selectedMarketPlaces,
                Role: item.Role == null ? 'User' : item.Role,
                Permissions: item.Permissions,
                ObjectID: item.ObjectID,
                SharpspringUserName: item.SharpspringUserName,
                SharpspringPassword: item.SharpspringPassword,
            };

            setTimeout(function () {
                $(formLoader).fadeOut();
                $("html, body").animate({ scrollTop: 0 }, "slow");
            }, 1000);

        }

        $scope.deleteUser = function (item) {
            QuestionAlert("Delete User", "Are you sure you want to delete user ?", function () {
                var data =
                {
                    'ObjectID': item.ObjectID,
                };

                $.ajax({
                    url: _userDeleteUrl,
                    dataType: 'json',
                    type: 'post',
                    data: data,
                    success: function (result) {
                        $scope.closeUserUpdation();
                        if (result.Success) {
                            notificationBarSuccess(userAccountNotification, 'The user has been deleted.');
                        }
                        else {
                            notificationBarFailure(userAccountNotification, 'There was a problem deleting the user.');
                        }

                        setTimeout(function () {
                            timeoutHideNotificationBar(userAccountNotification);
                            $scope.remove(item);
                        }, 1000);

                    }
                });

            }, function () {

            });
        }

        $scope.closeUserUpdation = function () {
            $scope.IsUserUpdate = false;
            $("#CreateAccountUserSubmit").text("Create User");
            $scope.emptyUserData();
        }

        $scope.emptyUserData = function () {
            $scope.clearPermissions();

            $scope.itemToAdd = {};

            $(_marketPlaces).each(function (i, j) {
                j.EZShredId = null;
            });

            $scope.itemToAdd = {
                Role: 'User',
                EZShredAccountMappings: _marketPlaces,
            };

            $scope.updateRole($scope.itemToAdd);
        };

        $scope.clearPermissions = function () {
            $(".js-check-change").each(function (i, j) {
                $(this).attr('checked', true);
                $(this).prop('checked', true);
            });

            setTimeout(function () {
                $('.switchery').trigger('click');
            }, 100);
        };

    });

    AccountUsersApp.directive("iCheck", function () {
        return {
            require: 'ngModel',
            link: function ($scope, element, $attrs, ngModel) {

                $(element).on('ifCreated', function (event) {
                    if ($(element).attr('id') === 'rbRoleUser' && $attrs.ngModel) {
                        $(element).iCheck('check');
                        $scope.$apply(function () {
                            ngModel.$setViewValue($attrs.value);
                            $('#Role').val($attrs.value);
                        });
                    }
                });

                $(element).on('ifChanged', function (event) {
                    if ($(element).attr('type') === 'radio' && $attrs.ngModel) {
                        $scope.$apply(function () {
                            ngModel.$setViewValue($attrs.value);
                            $('#Role').val($attrs.value);
                        });
                    }
                });
            }
        };
    });

    AccountUsersApp.directive("permissionsSwitchery", function () {
        return {
            require: 'ngModel',
            link: function ($scope, element, $attrs, ngModel) {

                $(element).on('change', function (event) {
                    $scope.$apply(function () {
                        ngModel.$setViewValue($(element).val());
                    });
                });

            }
        };
    });

    AccountUsersApp.directive('tooltip', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                $(element).hover(function () {
                    $(element).tooltip('show');
                }, function () {
                    $(element).tooltip('hide');
                });
            }
        };
    });

    //Utility functions
    function progressStep(newStep) {
        $('#wizard').smartWizard('goToStep', newStep);
    }

    function disableButton(elem, className) {
        $(elem).removeClass();
        $(elem).addClass('btn').addClass(className);
        $(elem).prop("disabled", true);
    }

    function enableButton(elem, className) {
        $(elem).removeClass();
        $(elem).addClass('btn').addClass(className);
        $(elem).prop("disabled", false);
        $(elem).removeAttr("disabled");
    }

});