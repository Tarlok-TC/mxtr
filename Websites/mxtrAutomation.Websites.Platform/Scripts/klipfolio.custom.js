var ssoUrl = "https://app.klipfolio.com/users";
function ssoAuth(ssoToken, ssoCompany) {
    $.ajax({
        url: ssoUrl + "/sso_auth",
        type: "post",
        async: false,
        xhrFields: {
            withCredentials: true
        },
        headers: {
            "KF-SSO": ssoToken,
            "KF-Company": ssoCompany
        },
        dataType: "json",
        success: function (data) {
            console.log(data);
            localStorage.setItem('success', "true");
        },
        error: function (err) {
            console.log("sso auth failed", err)
            var errMsg = JSON.parse(err.responseText);
            console.log(errMsg)
            localStorage.setItem('success', "false");
            localStorage.setItem('message', errMsg.error_message);
            $('#result').html("<h2 class='fail'>SSO Failed<h2><p>Error: (#" + errMsg.error_code + ") "
                + errMsg.error_message + "</p>");
        }
    })
}
function ssoLogout(ssoToken, ssoCompany) {
    $.ajax({
        url: ssoUrl + "/sso_logout",
        type: "post",
        async: false,
        xhrFields: {
            withCredentials: true
        },
        headers: {
            "KF-SSO": ssoToken,
            "KF-Company": ssoCompany
        },
        dataType: "json",
        success: function (data) {
            console.log("logout", data)
        },
        error: function (err) {
            console.log("sso auth failed", err.responseJSON)
        }
    })

}
