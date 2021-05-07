function ConfirmationDialog(ID) {
    if (confirm("Are you sure to delete?"))
        fnDeleteLead(ID);
    else
        return false;
}

function fnDeleteLead(ID) {
    var formData = new FormData();
    formData.append("leadID", ID);
    var result = __glb_fnIUDOperation(formData, "/Lead/DeleteSingleLead");
    if (result === true) {
        window.location.href = "/Lead/Index";
        showAlertMessage("success", result.message);
    }
    else if (result === false) {
        window.location = "/";
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

function fnPopulateControls(LeadId, CustomFieldId) {
    var formData = new FormData();
    formData.append("CustomFieldId", CustomFieldId);
    formData.append("LeadId", LeadId);
    var result = __glb_fnIUDOperation(formData, "/Lead/GetCustomFieldDetailsById");
    if (result.success === true) {
        console.log(result);
        document.getElementById("hdnCustomFieldId").value = result.id;
        document.getElementById("hdnCustomFieldTypeId").value = result.fieldTypeId;
        $('#CustomFiedlTitle').html("");
        var label_text = $('#CustomFiedlTitle').text(); //Get the text
        $('#CustomFiedlTitle').text(label_text.replace("", result.fieldName));
        var FieldTypeName = result.fieldTypeName.toLowerCase();
        var accountId = result.accountId;
        var FieldValues = result.fieldValues;
        console.log(result.fieldTypeAns.length);
        console.log(result.fieldTypeAns);
        if (FieldTypeName == "text") {
            $("#txtCustomFieldValue").show();
            if (result.fieldTypeAns.length > 0) {
                for (i = 0; i < result.fieldTypeAns.length; i++) {
                    if (result.fieldTypeAns[i].customFieldId == result.id) {
                        $("#txtCustomFieldValue").attr("type", FieldTypeName).attr("value", result.fieldTypeAns[i].fieldAns);
                    }
                }
            }
            else {
                $("#txtCustomFieldValue").attr("type", FieldTypeName);
            }

            $("#cmbcustomFieldType").hide();
        }
        else if (FieldTypeName == "date") {
            $("#txtCustomFieldValue").show();
            //$("#txtCustomFieldValue").attr("type", FieldTypeName);
            if (result.fieldTypeAns.length > 0) {
                for (i = 0; i < result.fieldTypeAns.length; i++) {
                    if (result.fieldTypeAns[i].customFieldId == result.id) {
                        $("#txtCustomFieldValue").attr("type", FieldTypeName).attr("value", result.fieldTypeAns[i].fieldAns);
                    }
                }
            }
            else {
                $("#txtCustomFieldValue").attr("type", FieldTypeName);
            }

            $("#cmbcustomFieldType").hide();
        }
        else if (FieldTypeName == "number") {
            $("#cmbcustomFieldType").hide();
            $("#txtCustomFieldValue").show();
            $("#txtCustomFieldValue").attr("type", "text").attr("maxlength", "5").attr("onkeypress", "return isNumberKey(event)");
            if (result.fieldTypeAns.length > 0) {
                for (i = 0; i < result.fieldTypeAns.length; i++) {
                    if (result.fieldTypeAns[i].customFieldId == result.id) {
                        $("#txtCustomFieldValue").attr("type", FieldTypeName).attr("value", result.fieldTypeAns[i].fieldAns);
                    }
                }
            }
            else {
                $("#txtCustomFieldValue").attr("type", FieldTypeName);
            }
            $("#cmbcustomFieldType").hide();
        }
        else if (FieldTypeName == "dropdown") {
            console.log(result.fieldValues);
            $("#cmbcustomFieldType").show();
            $("#txtCustomFieldValue").hide();
            var s = '';
            if (result.fieldTypeAns.length > 0) {
                for (var i = 0; i < result.fieldValues.length; i++) {
                    if (result.fieldTypeAns[0].fieldAns == result.fieldValues[i].fieldValue) {
                        s += '<option selected value="' + result.fieldValues[i].customFieldValueId + '">' + result.fieldValues[i].fieldValue + '</option>';
                    }
                    else {
                        s += '<option value="' + result.fieldValues[i].customFieldValueId + '">' + result.fieldValues[i].fieldValue + '</option>';
                    }
                }
            }
            else {
                for (var i = 0; i < result.fieldValues.length; i++) {
                    s += '<option value="' + result.fieldValues[i].customFieldValueId + '">' + result.fieldValues[i].fieldValue + '</option>';
                }
            }
            $("#cmbcustomFieldType").html(s);
        }
        $("#divCustomFieldValueModel").modal('show');
    }
    else if (result.success === false) {
        window.location = "/";
    }
}

function fnUpdateCustomFieldDetails(LeadId) {
    var CustomFieldId = document.getElementById("hdnCustomFieldId");
    var CustomFieldTypeId = document.getElementById("hdnCustomFieldTypeId");
    var LeadId = LeadId;
    var Name = document.getElementById("txtCustomFieldValue");
    var e = document.getElementById("cmbcustomFieldType");
    var strddlValue = "";
    if (e.options.length > 0) {
        strddlValue = e.options[e.selectedIndex].text;
    }

    if (Name.value == "") {
        $("#txtCustomFieldValue").css({
            "border": "1px solid red",
            "background": "#FFCECE"
        });
        return;
    }
    //var _liDropDownValue = new Array();
    //$("input[name='DynamicTextBox']").each(function () {

    //    _liDropDownValue.push($(this).val());
    //})
    //var ddlValue = _liDropDownValue;
    var formData = new FormData();
    formData.append("CustomFieldId", CustomFieldId.value);
    formData.append("CustomFieldTypeId", CustomFieldTypeId.value);
    formData.append("LeadId", LeadId);
    formData.append("Answer", Name.value);
    formData.append("strddlValue", strddlValue);
    //formData.append("DropDownOption", ddlValue);



    var result = __glb_fnIUDOperation(formData, "/Lead/InsertCustomFieldAns");
    if (result.success === true) {
        $("#divCustomFieldValueModel").modal('hide');
        showAlertMessage("success", "Custom Field updated successfully.");
    }
    else if (result.success === false) {
        window.location = "/";
    }
    else {
        showAlertMessageToEditAdmin(result.message);
        return;
    }
}
function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function fnPopulateControlsAppointment(LeadId) {
    var formData = new FormData();
    formData.append("leadID", document.getElementById("hdnLeadID").value);
    var result = __glb_fnIUDOperation(formData, "/Admin/GetSelectedTagbyLeadId");
    if (result.success === true) {
        //alert(result.selectedTags);
        $('#cmbInvitees').val(result.selectedTags);
        $('#cmbInvitees').trigger('change');
        $('#cmbInvitees').focus();
    }
    $("#divAppointmentsModel").modal('show');
    $('#ModalTitle').text('Create Appointment');
}

function fnCreateLeadAppointMent(LeadId) {
    var leadId = LeadId;
    var Title = document.getElementById("txtAppointmentTitle").value;
    var appointmentType = document.getElementById("cmdAppointmentType").value;
    var appointmentOutcomes = document.getElementById("cmdAppointmentOutcomes").value;
    if (Title == "") {
        $("#divErrorMsgAppointments").html("Title required.");
        $('#divErrorMsgAppointments').show();
        document.getElementById("divErrorMsgAppointments").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAppointments").fadeOut();
        }, 5000);
        document.getElementById("txtAppointmentTitle").focus();
        return;
    }
    else if (appointmentType == 0) {
        $("#divErrorMsgAppointments").html("Appointment Type required.");
        $('#divErrorMsgAppointments').show();
        document.getElementById("divErrorMsgAppointments").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAppointments").fadeOut();
        }, 5000);
        document.getElementById("cmdAppointmentType").focus();
        return;
    }
    else if (appointmentOutcomes == 0) {
        $("#divErrorMsgAppointments").html("Appointment Outcomes required.");
        $('#divErrorMsgAppointments').show();
        document.getElementById("divErrorMsgAppointments").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAppointments").fadeOut();
        }, 5000);
        document.getElementById("cmdAppointmentOutcomes").focus();
        return;
    }
    var Description = document.getElementById("txtDescription").value;

    var date = document.getElementById("datepicker").value;
    var time = document.getElementById("timepicker").value;
    var location = document.getElementById("txtLocation").value;
    var agent = document.getElementById("cmbInvitees").value;
    if (__glb_fnIsNullOrEmpty(agent) || agent <= 0) {
        $("#divErrorMsgAppointments").html("Please select Invitees!");
        $('#divErrorMsgAppointments').show();
        document.getElementById("divErrorMsgAppointments").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAppointments").fadeOut();
        }, 3000);
        document.getElementById("cmbInvitees").focus();
        return;
    }

    var formData = new FormData();
    formData.append("leadID", leadId);
    formData.append("Title", Title);
    formData.append("Description", Description);
    formData.append("appointmentType", appointmentType);
    formData.append("appointmentOutcomes", appointmentOutcomes);
    formData.append("date", date);
    formData.append("time", time);
    formData.append("location", location);
    formData.append("agent", agent);
    formData.append("LeadAppointmentId", 0);
    var result = __glb_fnIUDOperation(formData, "/Lead/CreateUpdateLeadAppointment");
    if (result.success === true) {
        $("#divAppointmentsModel").modal('hide');
        showAlertMessage("success", "Lead Appointment added successfully.");
    }
    else if (result.success === false) {
        window.location = "/";
    }
}
function fnUpdateLeadAppointMent(LeadId) {
    var leadId = LeadId;
    var leadAppointmentID = document.getElementById("hdnLeadAppointmentId").value;
    var Title = document.getElementById("txtAppointmentTitle").value;
    var appointmentType = document.getElementById("cmdAppointmentType").value;
    var appointmentOutcomes = document.getElementById("cmdAppointmentOutcomes").value;
    if (Title == "") {
        $("#divErrorMsgAppointments").html("Title required.");
        $('#divErrorMsgAppointments').show();
        document.getElementById("divErrorMsgAppointments").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAppointments").fadeOut();
        }, 5000);
        document.getElementById("txtAppointmentTitle").focus();
        return;
    }
    else if (appointmentType == 0) {
        $("#divErrorMsgAppointments").html("Appointment Type required.");
        $('#divErrorMsgAppointments').show();
        document.getElementById("divErrorMsgAppointments").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAppointments").fadeOut();
        }, 5000);
        document.getElementById("cmdAppointmentType").focus();
        return;
    }
    else if (appointmentOutcomes == 0) {
        $("#divErrorMsgAppointments").html("Appointment Outcomes required.");
        $('#divErrorMsgAppointments').show();
        document.getElementById("divErrorMsgAppointments").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAppointments").fadeOut();
        }, 5000);
        document.getElementById("cmdAppointmentOutcomes").focus();
        return;
    }
    var Description = document.getElementById("txtDescription").value;

    var date = document.getElementById("datepicker").value;
    var time = document.getElementById("timepicker").value;
    var location = document.getElementById("txtLocation").value;
    var agent = document.getElementById("cmbInvitees").value;
    if (__glb_fnIsNullOrEmpty(agent) || agent <= 0) {
        $("#divErrorMsgAppointments").html("Please select Invitees!");
        $('#divErrorMsgAppointments').show();
        document.getElementById("divErrorMsgAppointments").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgAppointments").fadeOut();
        }, 3000);
        document.getElementById("cmbInvitees").focus();
        return;
    }

    var formData = new FormData();
    formData.append("leadID", leadId);
    formData.append("Title", Title);
    formData.append("Description", Description);
    formData.append("appointmentType", appointmentType);
    formData.append("appointmentOutcomes", appointmentOutcomes);
    formData.append("date", date);
    formData.append("time", time);
    formData.append("location", location);
    formData.append("agent", agent);
    formData.append("LeadAppointmentId", leadAppointmentID);
    var result = __glb_fnIUDOperation(formData, "/Lead/CreateUpdateLeadAppointment");
    if (result.success === true) {
        $("#divAppointmentsModel").modal('hide');
        showAlertMessage("success", "Lead Appointment updated successfully.");
    }
    else if (result.success === false) {
        window.location = "/";
    }
}

function GetLeadAppointmentValue(LeadId, LeadAppointmentId) {
    document.getElementById("hdnLeadAppointmentId").value = LeadAppointmentId;
    var leadId = LeadId;
    var leadAppointmentId = LeadAppointmentId;
    var formData = new FormData();
    formData.append("LeadId", leadId);
    formData.append("LeadAppointmentId", leadAppointmentId);
    var result = __glb_fnIUDOperation(formData, "/Lead/GetLeadAppointmentByLeadIdandLeadAppointmentId");
    if (result.success === true) {
        document.getElementById("txtAppointmentTitle").value = result.title;
        document.getElementById("txtDescription").value = result.description;
        document.getElementById("cmdAppointmentType").value = result.appointmentType;
        document.getElementById("cmdAppointmentOutcomes").value = result.appointmentOutcomes;
        document.getElementById("datepicker").value = changeDateFormat(result.date.split('T')[0]);
        var lastIndex = result.date.split('T')[1].lastIndexOf(":");
        var s1 = result.date.split('T')[1].substring(0, lastIndex);
        document.getElementById("timepicker").value = s1;
        document.getElementById("txtLocation").value = result.location;
        /*document.getElementById("cmbInvitees").value = result.agent;*/
        $('#cmbInvitees').val(result.agent);
        $('#cmbInvitees').trigger('change');
        $('#BtnSave').show();
        $('#BtCreateAppointment').hide();
        $("#divAppointmentsModel").modal('show');
        $('#ModalTitle').text('Edit Appointment');
        //showAlertMessage("success", "Lead Appointment added successfully.");
    }
    else if (result.success === false) {
        window.location = "/";
    }
}

function ConfirmationDialogAppointMent(LeadId, LeadAppointmentId) {
    if (confirm("Are you sure to delete this appointment?"))
        fnDeleteLeadAppointment(LeadId, LeadAppointmentId);
    else
        return false;
}

function fnDeleteLeadAppointment(LeadId, LeadAppointmentId) {
    var formData = new FormData();
    formData.append("leadID", LeadId);
    formData.append("leadAppointmentId", LeadAppointmentId);
    var result = __glb_fnIUDOperation(formData, "/Lead/DeleteLeadAppointment");
    if (result.success === true) {
        showAlertMessage("success", result.message);
    }
    else if (result.success === false) {
        window.location = "/";
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

//function format(inputDate) {
//    var date = new Date(inputDate);
//    if (!isNaN(date.getTime())) {
//        var day = date.getDate().toString();
//        var month = (date.getMonth() + 1).toString();
//        // Months use 0 index.
//        return (day[1] ? day : '0' + day[0]) + '/' +
//            (month[1] ? month : '0' + month[0]) + '/' +
//            date.getFullYear();
//    }
//}

//var format = function (input) {
//    var pattern = /(\d{4})\-(\d{2})\-(\d{2})/;
//    if (!input || !input.match(pattern)) {
//        return null;
//    }
//    return input.replace(pattern, '$2/$3/$1');
//};


function changeDateFormat(inputDate) {  // expects Y-m-d
    var splitDate = inputDate.split('-');
    if (splitDate.count == 0) {
        return null;
    }

    var year = splitDate[0];
    var month = splitDate[1];
    var day = splitDate[2];

    return month + '/' + day + '/' + year;
}