$(document).ready(function () {
    //var table = $('#tblLeadList').DataTable({
    //                "pageLength": 10,
    //                "order": [[1, 'desc']],
    //                "columnDefs": [{
    //                    "targets": 0,
    //                    "orderable": false
    //                }]
    //});
    var table = $('#tblLeadList').DataTable({
        //scrollY: "300px",
        scrollX: true,
        scrollCollapse: true,
        pageLength: 10,
        order: [[1, 'desc']],
        columnDefs: [{
            "targets": 0,
            "orderable": false
        }],
        fixedColumns: {
            leftColumns: 2
        }
    });
    $(".toggle-vis").change(function (e) {
        e.preventDefault();

        // Get the column API object
        var column = table.column($(this).attr('data-column'));

        if (this.checked) {
            column.visible(true);
        }
        else {
            column.visible(false);
        }
    });


    document.getElementById("btnDeleteLead").disabled = true;
    /*document.getElementById("liLead").classList.add("active");*/
    $("#checkAll").click(function () {
        $('input:checkbox').not(this).prop('checked', this.checked);
        document.getElementById("btnDeleteLead").disabled = false;
    });

    setTimeout(function () {
        $("#divAlert").fadeOut()
    }, 3000);
});


function ConfirmationDialog() {
    var checkedLeadID = new Array();
    $("input[type='checkbox'][name^='checkbox-']").each(function () {
        this.checked ? checkedLeadID.push($(this).val()) : null;
    });
    if (checkedLeadID.length <= 0) {
        showAlertMessage("warning", "Select alteast one lead by clicking on checkbox!")
        return;
    }
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
            //window.location.reload();
        }, 3000);
    }

    $("html, body").animate({ scrollTop: 0 }, "slow");
}


function setAgentValue(agentID, leadID) {

    document.getElementById("hdnAgentID").value = agentID;
    document.getElementById("hdnLeadID").value = leadID;
    document.getElementById("cmbAgentsn").value = agentID;
}

function fnUpdateAgent() {

    var agentID = document.getElementById("cmbAgentsn").value;
    if (__glb_fnIsNullOrEmpty(agentID) || agentID <= 0) {
        $("#divErrorMsgAgent").html("Please select agent!");
        $('#divErrorMsgAgent').show();
        document.getElementById("divErrorMsgAgent").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAgent").fadeOut();
        }, 3000);
        document.getElementById("cmbAgentsn").focus();
        return;
    }
    var formData = new FormData();
    formData.append("agentID", agentID);
    formData.append("leadID", document.getElementById("hdnLeadID").value);
    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateAgent");
    if (result.success === true) {
        //$("#divAssignAgent").modal("hide");
        $('#divAssignAgent').modal('hide');
        showAlertMessage("success", "Agent assigned successfully.");
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


function SendBulkMail() {
    var checkedLeadID = new Array();
    $("input[type='checkbox'][name^='checkbox-']").each(function () {
        this.checked ? checkedLeadID.push($(this).val()) : null;
    });
    if (checkedLeadID.length <= 0) {
        showAlertMessage("warning", "Select alteast one lead for mail!")
        return;
    }
    $('#BulkEmailModel').modal('show');
    //var desc = CKEDITOR.instances['DSC'].getData();
}

function fnSendBulkMail() {

    var isValid = true;
    $('#txtEmailSubject').each(function (e) {
        if ($.trim($(this).val()) == '' || mailBody == '') {
            isValid = false;
            $(this).css({
                "border": "1px solid red",
                "background": "#FFCECE"
            });
        }
        else {
            $(this).css({
                "border": "",
                "background": ""
            });
        }
    });
    if (isValid == false)
        e.preventDefault();


    var subject = $("#txtEmailSubject").val();
    var mailBody = CKEDITOR.instances['txtMailBody'].getData();
    if (mailBody == '') {
        $("#divErrorMsgMail").html("Mail body is requied.");
        $('#divErrorMsgMail').show();
        $("#divErrorMsgMail").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgMail").fadeOut();
        }, 5000);
        return;
    }
    //alert(mailBody);
    var checkedEmailList= new Array();
    $("input[type='checkbox'][email^='checkboxemail-']").each(function () {
        this.checked ? checkedEmailList.push($(this).attr("email").replace('checkboxemail-', '')) : null;
    });
    //alert(checkedEmailList);
    var checkedAgentEmailList = new Array();
    $("input[type='checkbox'][agentemail^='checkboxagentemail-']").each(function () {
        this.checked ? checkedAgentEmailList.push($(this).attr("agentemail").replace('checkboxagentemail-', '')) : null;
    });
    //alert(checkedAgentEmailList);


    var formData = new FormData();
    formData.append("subject", subject);
    formData.append("mailBody", mailBody);
    formData.append("checkedEmailList", checkedEmailList);
    formData.append("checkedAgentEmailList", checkedAgentEmailList);
    var result = __glb_fnIUDOperation(formData, "/Lead/SendBulkMail");
    if (result.success === true) {
        showAlertMessage("success", result.message);
        $('#BulkEmailModel').modal('hide');
    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }
    return;
}



function setBulkAgent() {
    var checkedLeadID = new Array();
    $("input[type='checkbox'][name^='checkbox-']").each(function () {
        this.checked ? checkedLeadID.push($(this).val()) : null;
    });
    if (checkedLeadID.length <= 0) {
        showAlertMessage("warning", "Select atleast one lead for assign agent!")
        return;
    }
    $('#AssignAgentModal').modal('show');
    //var desc = CKEDITOR.instances['DSC'].getData();
}

function fnAssignMultipleAgent() {
    var checkedLeadID = new Array();
    $("input[type='checkbox'][name^='checkbox-']").each(function () {
        this.checked ? checkedLeadID.push($(this).val()) : null;
    });

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
    formData.append("leadID", checkedLeadID);
    var result = __glb_fnIUDOperation(formData, "/Admin/AssignMultipleAgent");
    if (result.success === true) {
        showAlertMessage("success", "Agent assigned successfully.");
        $('#AssignAgentModal').modal('hide');
    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }
}




//Set Stage to lead
function AssignBulkStage() {
    var checkedLeadID = new Array();
    $("input[type='checkbox'][name^='checkbox-']").each(function () {
        this.checked ? checkedLeadID.push($(this).val()) : null;
    });
    if (checkedLeadID.length <= 0) {
        showAlertMessage("warning", "Select atleast one lead for assign stage!")
        return;
    }
    $('#AssignStageModal').modal('show');
    //var desc = CKEDITOR.instances['DSC'].getData();
}

function fnAssignStage() {
    var checkedLeadID = new Array();
    $("input[type='checkbox'][name^='checkbox-']").each(function () {
        this.checked ? checkedLeadID.push($(this).val()) : null;
    });
    if (checkedLeadID.length <= 0) {
        showAlertMessage("warning", "Select atleast one lead for assign tag!")
        return;
    }

    var stageID = document.getElementById("cmbStage").value;
    if (__glb_fnIsNullOrEmpty(stageID) || stageID <= 0) {
        $("#divErrorMsgStage").html("Please select stage!");
        $('#divErrorMsgStage').show();
        document.getElementById("divErrorMsgStage").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgStage").fadeOut();
        }, 3000);
        document.getElementById("cmbStage").focus();
        return;
    }

    var formData = new FormData();
    formData.append("stageID", stageID);
    formData.append("leadID", checkedLeadID);
    var result = __glb_fnIUDOperation(formData, "/Admin/AssignStageToLead");
    if (result.success === true) {
        showAlertMessage("success", "Stage assigned successfully to lead.");
        $('#AssignStageModal').modal('hide');
    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }
}



//Set Tag to lead
function AssignTag() {
    var checkedLeadID = new Array();
    $("input[type='checkbox'][name^='checkbox-']").each(function () {
        this.checked ? checkedLeadID.push($(this).val()) : null;
    });
    if (checkedLeadID.length <= 0) {
        showAlertMessage("warning", "Select atleast one lead for assign tag!")
        return;
    }
    $('#AssignTagModal').modal('show');
   
    
    //var desc = CKEDITOR.instances['DSC'].getData();
}

function fnAssignTag() {
    var checkedLeadID = new Array();
    $("input[type='checkbox'][name^='checkbox-']").each(function () {
        this.checked ? checkedLeadID.push($(this).val()) : null;
    });
    if (checkedLeadID.length <= 0) {
        showAlertMessage("warning", "Select atleast one lead for assign tag!")
        return;
    }

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
    formData.append("leadID", checkedLeadID);
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