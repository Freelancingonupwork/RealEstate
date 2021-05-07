function ConfirmationDialog(StageId) {
    if (confirm("Are you sure to delete?"))
        fnDeleteRecord(StageId);
    else
        return false;
}

function fnDeleteRecord(AppointmentTypeID) {

    var formData = new FormData();
    formData.append("AppointmentTypeId", AppointmentTypeID);
    var result = __glb_fnIUDOperation(formData, "/Admin/DeleteAppointmentType");
    if (result.success === true) {
        //window.location.reload()
        showAlertMessage("success", result.message);
    }
    else {
        $("#divAlertMessage").html(result.message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-danger");
        $(function () {
            $("#divAlertMessage").fadeOut(2500);
        }, 3000);
        return;
    }
}

function fnPopulateControls(AppointmentTypeID) {

    var formData = new FormData();
    formData.append("AppointmentTypeID", AppointmentTypeID);
    var result = __glb_fnIUDOperation(formData, "/Admin/GetAppointmentTypeDetails");
    if (result.success === true) {
        document.getElementById("hdnAppointmentTypeId").value = result.appointmenTypeId;
        document.getElementById("txtName").value = result.appointmentTypeName;
        $("#divEditAppointmentType").modal('show');
    }
    else {
        showAlertMessage("danger", "Something went wrong! Please try again.");
        return;
    }

}
function fnUpdateStageDetails() {
    var AppointmentTypeId = document.getElementById("hdnAppointmentTypeId");
    var Name = document.getElementById("txtName");

    var formData = new FormData();
    formData.append("AppointmentTypeId", AppointmentTypeId.value);
    formData.append("Name", Name.value);

    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateAppointmentTypeDetails");
    if (result.success === true) {
        $("#divEditAppointmentType").modal('hide');
        showAlertMessage("success", "Appointment Type updated successfully.");
    }
    else {
        showAlertMessageToEditAdmin(result.message);
        return;
    }
}

function showAlertMessageToEditAdmin(message) {
    $("#divErrorMsgStage").html(message);
    $('#divErrorMsgStage').show();
    document.getElementById("divErrorMsgStage").classList.add("alert", "alert-warning");
    $(function () {
        $("#divErrorMsgStage").fadeOut(4000);
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

    $("html, body").animate({ scrollTop: 0 }, "slow");
}