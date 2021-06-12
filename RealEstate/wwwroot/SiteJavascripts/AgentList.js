function fnActivateDeactivateAgent(AccountId, ADAFlag) {
    var formData = new FormData();
    formData.append("id", AccountId);
    formData.append("flag", ADAFlag);
    var result = __glb_fnIUDOperation(formData, '/Admin/ActivateDeactivateAgent');
    if (result.act == true) {
        //$("#divAlertMessage").html(result.message);
        //$('#divAlertMessage').show();
        //document.getElementById("divAlertMessage").classList.add("alert", "alert-success");
        //setTimeout(function () {
        //    $("#divAlertMessage").fadeOut();
        //}, 5000);
        //location.reload(true);
        showAlertMessage("success", result.message);
    }
    else if (result.deAct == true) {
        showAlertMessage("success", result.message);
        //$("#divAlertMessage").html(result.message);
        //$('#divAlertMessage').show();
        //document.getElementById("divAlertMessage").classList.add("alert", "alert-success");
        //setTimeout(function () {
        //    $("#divAlertMessage").fadeOut();
        //}, 5000);
        //location.reload(true);
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
    var result = __glb_fnIUDOperation(formData, "/Admin/DeleteAgent");
    if (result.success === true) {
        //window.location.reload()
        showAlertMessage("success", "Agent deleted successfully.");
    }
    else {
        $("#divAlertMessage").html(result.message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
        }, 5000);
        return;
    }
}


function fnPopulateControls(AccountId) {

    var formData = new FormData();
    formData.append("AccountId", AccountId);
    var result = __glb_fnIUDOperation(formData, "/Admin/GetAgentDetails");
    if (result.success === true) {
        document.getElementById("hdnEditAgentID").value = result.accountId;
        document.getElementById("txtEditAgentFullName").value = result.fullName;
        document.getElementById("txtEditEmailAddress").value = result.emailAddress;
        document.getElementById("txtEditCellPhone").value = result.cellPhone;
        $("#divEditAgent").modal('show');

    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }

}

function fnUpdateAgentDetails() {
    var AccountId = document.getElementById("hdnEditAgentID");
    var txtfullName = document.getElementById("txtEditAgentFullName");
    var txtemailAddress = document.getElementById("txtEditEmailAddress");
    var txtcellPhone = document.getElementById("txtEditCellPhone");
    var regx = new RegExp(/([a-zA-Z])+([a-zA-Z])\w+/);
    if (__glb_fnIsNullOrEmpty(txtfullName.value) || !regx.test(txtfullName.value)) {
        showAlertMessageToEditAgent("Please enter valid full name!");
        txtfullName.focus();
        return;
    }

    if (!__glb_validateEmail(txtemailAddress.value)) {
        showAlertMessageToEditAgent("Please enter valid email address!");
        txtemailAddress.focus();
        return;
    }

    //if (!__glb_fnIsNullOrEmpty(txtcellPhone.value)) {
    //    if (isNaN(txtcellPhone.value)) {
    //        showAlertMessageToEditAgent("Please enter valid phone number!");
    //        txtcellPhone.focus();
    //        return;
    //    }
    //}

    var formData = new FormData();
    formData.append("AccountId", AccountId.value);
    formData.append("fullName", txtfullName.value);
    formData.append("emailAddress", txtemailAddress.value);
    formData.append("cellPhone", txtcellPhone.value);
    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateAgentDetails");
    if (result.success === true) {
        $("#divEditAgent").modal('hide');
        showAlertMessage("success", "Agent details updated successfully");
        //window.location.reload();
    }
    else {
        showAlertMessageToEditAgent(result.message);
        return;
    }
}

function showAlertMessageToEditAgent(message) {
    $("#divErrorMsgAgent").html(message);
    $('#divErrorMsgAgent').show();
    document.getElementById("divErrorMsgAgent").classList.add("alert", "alert-warning");
    setTimeout(function () {
        $("#divErrorMsgAgent").fadeOut();
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
        }, 2000);
    }

    if (type == "success") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-success");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
            window.location.reload();
        }, 2000);
       
    }

    if (type == "warning") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-warning");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
            //window.location.reload();
        }, 2000);
    }
}