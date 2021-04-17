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
        }, 3000);
        document.getElementById("cmbAgent").focus();
        return;
    }
    var formData = new FormData();
    formData.append("agentID", agentID);
    formData.append("leadID", document.getElementById("hdnLeadID").value);
    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateAgent");
    if (result.success === true) {
        $("#divAssignAgent").modal("hide");
        showAlertMessage("success", "Agent assigned successfully.");
    }
    else {
        $("#divErrorMsgAgent").html(result.message);
        $('#divErrorMsgAgent').show();
        document.getElementById("divErrorMsgAgent").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAgent").fadeOut();
        }, 3000);
        return;
    }
}


function setStageValue(stageId) {

    document.getElementById("cmbStage").value = stageId;
}


function fnUpdateStage() {

    var stageId = document.getElementById("cmbStage").value;
    if (__glb_fnIsNullOrEmpty(stageId) || stageId <= 0) {
        $("#divErrorMsgStage").html("Please select agent!");
        $('#divErrorMsgStage').show();
        document.getElementById("divErrorMsgStage").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgStage").fadeOut();
        }, 3000);
        document.getElementById("cmbStage").focus();
        return;
    }
    var formData = new FormData();
    formData.append("stageId", stageId);
    formData.append("leadID", document.getElementById("hdnLeadID").value);
    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateStageByLeadId");
    if (result.success === true) {
        $("#divAssignStageModal").modal("hide");
        showAlertMessage("success", "Stage assigned successfully.");
    }
    else {
        $("#divErrorMsgStage").html(result.message);
        $('#divErrorMsgStage').show();
        document.getElementById("divErrorMsgStage").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgStage").fadeOut();
        }, 3000);
        return;
    }
}

function setTagValue(LeadId) {
    var formData = new FormData();
    formData.append("leadID", document.getElementById("hdnLeadID").value);
    var result = __glb_fnIUDOperation(formData, "/Admin/GetSelectedTagbyLeadId");
    if (result.success === true) {
        //alert(result.selectedTags);
        $('#cmbtag').val(result.selectedTags);
        $('#cmbtag').trigger('change');
        $('#cmbtag').focus();
    }
}

function fnAssignTag() {
    var tagID = $("#cmbtag").val();;
    if (__glb_fnIsNullOrEmpty(tagID) || tagID <= 0) {
        $("#divErrorMsgTag").html("Please select tag!");
        $('#divErrorMsgTag').show();
        document.getElementById("divErrorMsgTag").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgTag").fadeOut();
        }, 3000);
        document.getElementById("cmbtag").focus();
        return;
    }

    var formData = new FormData
    formData.append("tagID", tagID);
    formData.append("leadID", document.getElementById("hdnLeadID").value);
    var result = __glb_fnIUDOperation(formData, "/Admin/AssignTagToLead");
    if (result.success === true) {
        showAlertMessage("success", "Tag assigned successfully to lead.");
        $('#AssignTagModal').modal('hide');
    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }
}