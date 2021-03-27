//$(document).ready(function () {
//    document.getElementById("liLead").classList.add("active");
//});

function ConfirmationDialog(ID) {
    if (confirm("Are you sure to delete?"))
        fnDeleteLead(ID);
    else
        return false;
}

function fnDeleteLead(ID) {
    var formData = new FormData();
    formData.append("leadID",ID);
    var result = __glb_fnIUDOperation(formData, "/Lead/DeleteSingleLead");
    window.location.href = "/Lead/Index";
}


function showAlertMessage(type, message) {
    if (type == "danger") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
            window.location.reload();
        }, 5000);
    }

    if (type == "success") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-success");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
            window.location.reload();
        }, 5000);
    }

    if (type == "warning") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-warning");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
            window.location.reload();
        }, 5000);
    }
}

function setAgentValue(agentID) {

    document.getElementById("cmbAgent").value = agentID;
}

function fnUpdateAgent() {

    var agentID = document.getElementById("cmbAgent").value;
    if (__glb_fnIsNullOrEmpty(agentID) || agentID <= 0) {
        $("#divErrorMsgAgent").html("Please select agent!");
        $('#divErrorMsgAgent').show();
        document.getElementById("divErrorMsgAgent").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAgent").fadeOut();
        }, 5000);
        document.getElementById("cmbAgent").focus();
        return;
    }
    var formData = new FormData();
    formData.append("agentID", agentID);
    formData.append("leadID", document.getElementById("hdnLeadID").value);
    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateAgent");
    if (result.success === true) {
        $("#divAssignAgent").modal("hide");
        showAlertMessage("success", "Agent assigned successfully");
    }
    else {
        $("#divErrorMsgAgent").html(result.message);
        $('#divErrorMsgAgent').show();
        document.getElementById("divErrorMsgAgent").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAgent").fadeOut();
        }, 5000);
        return;
    }
}