function Logout(url) {
    $.ajax({
        url: url,
        dataType: 'json',
        type: 'post',
        async: false,
        success: function (result) {
            window.location = result.Redirect;
            if (result.KlipfolioAuthData != null) {
                ssoLogout(result.KlipfolioAuthData.KlipfolioSSOToken, result.KlipfolioAuthData.KlipfolioCompanyID);//Single Sign On Logout
            }
        }
    });
}