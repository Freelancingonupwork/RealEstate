$(document).ready(function () {
    $('#tblLeadList').DataTable({
        "pageLength": 10,
        "order": [[1, 'desc']],
        "columnDefs": [{
            "targets": 0,
            "orderable": false
        }]
    });

    document.getElementById("btnDeleteLead").disabled = true;
    /*document.getElementById("liLead").classList.add("active");*/
    $("#checkAll").click(function () {
        $('input:checkbox').not(this).prop('checked', this.checked);
        document.getElementById("btnDeleteLead").disabled = false;
    });

    setTimeout(function () {
        $("#divAlert").fadeOut()
    }, 5000);
});


function ConfirmationDialog() {
    if (confirm("Are you sure to delete?"))
        fnDeleteSelectedLeads();
    else
        return false;
}


function fnDeleteSelectedLeads() {

    var checkedLeadID = new Array();
    $("input[type='checkbox'][name^='checkbox-']").each(function () {
        this.checked ? checkedLeadID.push($(this).val()) : null;
    });
    if (checkedLeadID.length <= 0) {
        showAlertMessage("warning", "Select alteast one lead by clicking on checkbox!")
        return;
    }
    var strIds = '';
    for (var i = 0; i < checkedLeadID.length; i++) {
        if (i + 1 == checkedLeadID.length) {
            strIds += checkedLeadID[i];
        }
        else {
            strIds += checkedLeadID[i] + ',';
        }

    }
    var formData = new FormData();
    formData.append("ids", strIds);
    var result = __glb_fnIUDOperation(formData, "/Lead/DeleteLeads");
    if (result.success === true) {
        showAlertMessage("success", "Leads deleted successfully");
        $(function () {
            window.location.reload();
        }, 3000);
    }
    else {
        showAlertMessage("danger", "!Opps, Something went wrong while deleting leads");
        return;
    }
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


function setAgentValue(agentID, leadID) {

    document.getElementById("hdnAgentID").value = agentID;
    document.getElementById("hdnLeadID").value = leadID;
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
        //$("#divAssignAgent").modal("hide");
        $('#divAssignAgent').modal('hide');
        showAlertMessage("success", "Agent assigned successfully");
    }
    else {
        showAlertMessage("danger", result.message);
        //$("#divErrorMsgAgent").html(result.message);
        //$('#divErrorMsgAgent').show();
        //document.getElementById("divErrorMsgAgent").classList.add("alert", "alert-danger");
        //$(function () {
        //    $("#divErrorMsgAgent").fadeOut(2500);
        //}, 5000);
        return;
    }
}