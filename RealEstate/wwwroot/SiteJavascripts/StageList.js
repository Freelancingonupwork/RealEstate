function ConfirmationDialog(StageId) {
    if (confirm("Are you sure to delete?"))
        fnDeleteRecord(StageId);
    else
        return false;
}

function fnDeleteRecord(StageId) {

    var formData = new FormData();
    formData.append("stageId", StageId);
    var result = __glb_fnIUDOperation(formData, "/Admin/DeleteStage");
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

function fnPopulateControls(StageId) {

    var formData = new FormData();
    formData.append("stageId", StageId);
    var result = __glb_fnIUDOperation(formData, "/Admin/GetStageDetails");
    if (result.success === true) {
        document.getElementById("hdnStageId").value = result.stageId;
        document.getElementById("txtStageName").value = result.stageName;
        $("#divEditStage").modal('show');
    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }

}
function fnUpdateStageDetails() {
    var stageId = document.getElementById("hdnStageId");
    var txtStageName = document.getElementById("txtStageName");

    var formData = new FormData();
    formData.append("stageId", stageId.value);
    formData.append("stageName", txtStageName.value);

    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateStageDetails");
    if (result.success === true) {
        $("#divEditStage").modal('hide');
        showAlertMessage("success", "Stage updated successfully.");
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