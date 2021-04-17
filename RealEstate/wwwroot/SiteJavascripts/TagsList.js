function ConfirmationDialog(StageId) {
    if (confirm("Are you sure to delete?"))
        fnDeleteRecord(StageId);
    else
        return false;
}

function fnDeleteRecord(TagId) {

    var formData = new FormData();
    formData.append("TagId", TagId);
    var result = __glb_fnIUDOperation(formData, "/Admin/DeleteTags");
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

function fnPopulateControls(TagId) {

    var formData = new FormData();
    formData.append("TagId", TagId);
    var result = __glb_fnIUDOperation(formData, "/Admin/GetTagsDetails");
    if (result.success === true) {
        document.getElementById("hdnTagsId").value = result.tagId;
        document.getElementById("txtTagName").value = result.tagName;
        $("#divEditTags").modal('show');
    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }

}
function fnUpdateStageDetails() {
    var tagId = document.getElementById("hdnTagsId");
    var txtTagName = document.getElementById("txtTagName");

    var formData = new FormData();
    formData.append("tagId", tagId.value);
    formData.append("TagName", txtTagName.value);

    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateTagsDetails");
    if (result.success === true) {
        $("#divEditTags").modal('hide');
        showAlertMessage("success", "Tag updated successfully.");
    }
    else {
        showAlertMessageToEditAdmin(result.message);
        return;
    }
}

function showAlertMessageToEditAdmin(message) {
    $("#divErrorMsgTags").html(message);
    $('#divErrorMsgTags').show();
    document.getElementById("divErrorMsgTags").classList.add("alert", "alert-warning");
    $(function () {
        $("#divErrorMsgTags").fadeOut(4000);
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