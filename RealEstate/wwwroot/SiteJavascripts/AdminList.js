function fnActivateDeactivateUser(UserID, ADAFlag) {
    var formData = new FormData();
    formData.append("id", UserID);
    formData.append("flag", ADAFlag);
    var result = __glb_fnIUDOperation(formData, '/Admin/ActivateDeactivateAdmin');
    if (result.act == true) {
        showAlertMessage("success", result.message);
    }
    else if (result.deAct == true) {
        showAlertMessage("success", result.message);
    }
    else if (result.success == false) {
        showAlertMessage("danger", result.message);
    }
}

function ConfirmationDialog(ID) {
    if (confirm("Are you sure to delete?"))
        fnDeleteRecord(ID);
    else
        return false;
}

function fnDeleteRecord(id) {

    var formData = new FormData();
    formData.append("id", id);
    var result = __glb_fnIUDOperation(formData, "/Admin/DeleteAdmin");
    if (result.success === true) {
        window.location.reload()
    }
    else {
        $("#divAlertMessage").html(result.message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-danger");
        $(function () {
            $("#divAlertMessage").fadeOut(2500);
        }, 5000);
        return;
    }
}

function fnPopulateControls(adminID) {

    var formData = new FormData();
    formData.append("adminID", adminID);
    var result = __glb_fnIUDOperation(formData, "/Admin/GetAdminDetails");
    if (result.success === true) {
        document.getElementById("hdnEditAdminID").value = result.adminID;
        document.getElementById("txtEditFullName").value = result.fullName;
        document.getElementById("txtEditEmailAddress").value = result.emailAddress;
        $("#divEditAdmin").modal('show');

    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }

}
function fnUpdateAdminDetails() {
    var hdnAdminID = document.getElementById("hdnEditAdminID");
    var txtfullName = document.getElementById("txtEditFullName");
    var txtemailAddress = document.getElementById("txtEditEmailAddress");

    var regx = new RegExp(/([a-zA-Z])+( [a-zA-Z])\w+/);
    if (__glb_fnIsNullOrEmpty(txtfullName.value) || !regx.test(txtfullName.value)) {
        showAlertMessageToEditAdmin("Please enter valid full name!");
        txtfullName.focus();
        return;
    }

    if (!__glb_validateEmail(txtemailAddress.value)) {
        showAlertMessageToEditAdmin("Please enter valid email address!");
        txtemailAddress.focus();
        return;
    }

    var formData = new FormData();
    formData.append("adminID", hdnAdminID.value);
    formData.append("fullName", txtfullName.value);
    formData.append("emailAddress", txtemailAddress.value);

    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateAdminDetails");
    if (result.success === true) {
        $("#divEditAdmin").modal('hide');
        showAlertMessage("success", "Admin User details updated successfully.");
    }
    else {
        showAlertMessageToEditAdmin(result.message);
        return;
    }
}

function showAlertMessageToEditAdmin(message) {
    $("#divErrorMsgAdmin").html(message);
    $('#divErrorMsgAdmin').show();
    document.getElementById("divErrorMsgAdmin").classList.add("alert", "alert-warning");
    $(function () {
        $("#divErrorMsgAdmin").fadeOut(4000);
    }, 5000);
}


function showAlertMessage(type, message) {
    if (type == "danger") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
            window.location.reload();
        }, 3000);
    }

    if (type == "success") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-success");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
            window.location.reload();
        }, 3000);

    }

    if (type == "warning") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-warning");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
            window.location.reload();
        }, 3000);
    }
}