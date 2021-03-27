
document.addEventListener("DOMContentLoaded", function (event) {

    var RememberMe = localStorage.getItem('RememberMe');
    if (RememberMe) {
        document.getElementById("txtUserEmail").value = localStorage.getItem('UserEmail');
        document.getElementById("txtUserPassword").value = localStorage.getItem('UserPassword');
        document.getElementById("rememberMe").checked = localStorage.getItem('RememberMe');
    }

});
function fnSaveCookie() {
    var RememberMe = document.getElementById("rememberMe").checked;
    console.log(RememberMe);
    if (RememberMe) {
        var UserEmail = document.getElementById("txtUserEmail").value;
        var UserPassword = document.getElementById("txtUserPassword").value;
        localStorage.setItem("UserEmail", UserEmail);
        localStorage.setItem("UserPassword", UserPassword);
        localStorage.setItem("RememberMe", RememberMe);
    }
    else {
        localStorage.removeItem('UserEmail');
        localStorage.removeItem('UserPassword');
        localStorage.removeItem('RememberMe');
    }
}
