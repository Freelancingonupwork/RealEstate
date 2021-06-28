$(document).ready(function () {
    fnGetLeadDetailMails();
});
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

function fnPopulateControlsLeadFile(LeadId) {
    $("#divLeadFileModel").modal('show');
}

function fnUploadLeadFile(LeadId) {
    var formData = new FormData(); //FormData object
    var fileInput = document.getElementById('fileInput');
    if (fileInput.files.length == 0) {
        $("#divErrorMsgLeadFile").html("Please select file for upload.");
        $('#divErrorMsgLeadFile').show();
        document.getElementById("divErrorMsgLeadFile").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divErrorMsgLeadFile").fadeOut();
        }, 5000);
        return;
    }
    for (i = 0; i < fileInput.files.length; i++) {
        formData.append("files", fileInput.files[i]);
    }
    formData.append("leadID", LeadId);

    var result = __glb_fnIUDOperation(formData, "/Lead/LeadUploadFilesByLeadID");
    if (result.success === true) {
        $("#divLeadFileModel").modal('hide');
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

function ConfirmationDialogLeadFile(LeadId, LeadFileId) {
    fnDeleteLeadFile(LeadId, LeadFileId);
}

function fnDeleteLeadFile(LeadId, LeadFileId) {
    var formData = new FormData();
    formData.append("leadID", LeadId);
    formData.append("LeadFileId", LeadFileId);
    var result = __glb_fnIUDOperation(formData, "/Lead/DeleteLeadFile");
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



function fnSendEmail(LeadId) {
    var ToMailAddress = document.getElementById('hdnToMailAddress');
    var MailSubject = document.getElementById("txtMailSubject");
    var MailBody = CKEDITOR.instances['txtMailBody'].getData()
    var fileInputMail = document.getElementById('fileInputMail');

    if (MailSubject.value == '' && MailBody == '') {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        $("#divAlertMessage").html("Please enter email subject or body.");
        $('#divAlertMessage').show();
        $("#divAlertMessage").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
            //loading.hide().delay(90000);
            $('html, body').animate({
                scrollTop: $("#pills-tabContent").offset().top
            }, 500);
        }, 5000);
        return;
    }

    if (fileInputMail.files.length > 1) {
        var f = fileInputMail.files[0]
        //here I CHECK if the FILE SIZE is bigger than 8 MB (numbers below are in bytes)

        if (f.size > 8388608 || f.fileSize > 8388608) {
            //alert(f.size);
            //show an alert to the user
            fileInputMail.value = null;
            $("html, body").animate({ scrollTop: 0 }, "slow");
            $("#divAlertMessage").html("Allowed file size exceeded. (Max. 8 MB).");
            $('#divAlertMessage').show();
            $("#divAlertMessage").addClass("alert alert-danger");
            setTimeout(function () {
                $("#divAlertMessage").fadeOut();
                $('html, body').animate({
                    scrollTop: $("#send-email").offset().top
                }, 500);
            }, 5000);
            return;
        }
    }
    var formData = new FormData(); //FormData object
    formData.append("EmailSubject", MailSubject.value);
    formData.append("MailBody", MailBody);
    formData.append("ToMailAddress", ToMailAddress.value);
    formData.append("LeadEmailMsgId", document.getElementById("hdnLeadEmailMsgId").value);
    formData.append("EmailMsgId", document.getElementById("hdnEmailMessageId").value);
    formData.append("FromName", document.getElementById("hdnFromName").value);
    formData.append("ToName", document.getElementById("hdnToName").value);
    formData.append("IsReplay", document.getElementById("hdnIsReplay").value);
    for (i = 0; i < fileInputMail.files.length; i++) {
        formData.append("files", fileInputMail.files[i]);
    }
    formData.append("leadID", LeadId);
    var result = __glb_fnIUDOperation(formData, "/Lead/LeadSendMailByLeadID");
    if (result.success === true) {
        $("#divLoader").show();
        console.log(result.data);
        fnGetLeadDetailMails();
        $('#txtMailSubject').val('');
        $('#fileInputMail').val('');
        CKEDITOR.instances.txtMailBody.setData('');
        $('html, body').animate({
            scrollTop: $("#divMailList").offset().top
        }, 2000, function () {
            $("#divLoader").hide();
        });

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

function fnGetLeadDetailMails() {

    var LeadId = document.getElementById("hdnLeadID").value;
    var formData = new FormData(); //FormData object
    formData.append("leadID", LeadId);
    var result = __glb_fnIUDOperation(formData, "/Lead/GetLeadDetailsMailByLeadID");
    if (result.success === true) {
        $("#divMailList").html('');
        $("#divMailList").removeAttr("style");
        if (result.data.length > 0) {
            console.log(result);
            console.log(result.accountname);
            console.log(result.data.length);
            var AccountName = result.accountname;
            for (i = 0; i < result.data.length; i++) {
                var html;
                if (result.data[i].leadEmailMessageReplayList.length > 0) {
                    var row = "<div class=\"custom-card-header\">" +
                        "<div class=\"chat-icon btn-icon\">" +
                        "<i class=\"fa fa-envelope\" aria-hidden=\"true\"></i>" +
                        "</div > " +
                        " <div class=\"chat-header-detail flex-grow-1 ml-3\"> " +
                        "<h5 style=\"text-transform:capitalize\">" +
                        //capitalize(result.data[i].leadEmailMessageReplayList[0].fullName) +
                        result.data[i].leadEmailMessageReplayList[0].fromName +
                        " <i class=\"fa fa-chevron-right\"></i> " +
                        //capitalize(AccountName) +
                        result.data[i].leadEmailMessageReplayList[0].toName +
                        "</h5> " +
                        fromNow(result.data[i].leadEmailMessageReplayList[0].createdDate) +
                        "<h5>" + result.data[i].leadEmailMessageReplayList[0].subject + "</h5> " +
                        "</div> " +
                        "<div class=\"chat-action\"> " +
                        "<button class=\"btn btn-secondary\" onclick=\"fnGetEmailSubject(" + result.data[i].leadEmailMessageReplayList[0].leadEmailMessageId + "," + result.data[i].leadEmailMessageReplayList[0].leadId + ");\">Reply</button> " +
                        "<button class=\"btn btn-secondary\" style=\"display:none\"> Reply All</button> " +
                        "<button class=\"btn btn-secondary\"><i class=\"fa fa-ellipsis-v\" aria-hidden=\"true\"></i></button>" +
                        "</div>" +
                        "</div>" +
                        "<div class=\"custom-card-text ml-5\"> " +
                        result.data[i].leadEmailMessageReplayList[0].body;
                    if (result.data[i].leadEmailMessageReplayList[0].leadEmailMessageReplayAttachement.length > 0) {
                        for (ji = 0; ji < result.data[i].leadEmailMessageReplayList[0].leadEmailMessageReplayAttachement.length; ji++) {
                            row = row += "<p> <a style=\"color:#BF0D1C\" src =\"javascript:void(0)\" onclick=\"fnDownloadMailAttachment('" + result.data[i].leadEmailMessageReplayList[0].leadEmailMessageReplayAttachement[ji].fileName + "'," + result.data[i].leadEmailMessageReplayList[0].leadEmailMessageReplayAttachement[ji].leadId + ");\"><i class=\"fa fa-paperclip\" aria-hidden=\"true\"></i> " +
                                result.data[i].leadEmailMessageReplayList[0].leadEmailMessageReplayAttachement[ji].fileName + "</a></p>";
                        }
                    }
                    var NestedRow = "<div class=\"custom-card-mail-reply\"> " +
                        "<div class=\"custom-card-header\"> " +
                        "<div class=\"chat-icon btn-icon\"> " +
                        "<i class=\"fa fa-envelope\" aria-hidden=\"true\"></i>" +
                        "</div> " +
                        "<div class=\"chat-header-detail flex-grow-1 ml-3\"> " +
                        "<h5 style=\"text-transform:capitalize\">" +
                        " FROMADDRESSVAR " +
                        "<i class=\"fa fa-chevron-right\"></i>" +
                        " TOADDRESSVAR " +
                        "</h5> " +
                        "<span> TIMEAGOVAR </span > " +
                        "<h5> SUBJECTVAR </h5 > " +
                        "</div> " +
                        "<div class=\"chat-action\"> " +
                        "<button class=\"btn btn-secondary\" onclick=\"fnGetEmailSubject(LEADMSGIDVAR,LEADIDVAR);\"> Reply</button > " +
                        "<button class=\"btn btn-secondary\" style=\"display:none\"> Reply All</button > " +
                        "<button class=\"btn btn-secondary\"><i class=\"fa fa-ellipsis-v\" aria-hidden=\"true\"></i></button> " +
                        "</div> " +
                        "</div> " +
                        "<div class=\"custom-card-text\"> " +
                        "<p> BODYVAR </p> " +
                        "<p> ATTACHMENTVAR </p> " +
                        "</div> " +
                        "</div> ";
                    console.log("aaaaa:-" + result.data[i].leadEmailMessageReplayList);
                    for (j = 1; j < result.data[i].leadEmailMessageReplayList.length; j++) {
                        //html = row += NestedRow.replace("LEADIDVAR", result.data[i].leadEmailMessageReplayList[j].leadId).replace("LEADMSGIDVAR", result.data[i].leadEmailMessageReplayList[j].leadEmailMessageId).replace("TOADDRESSVAR", (result.data[i].leadEmailMessageReplayList[j].toName)).replace("FROMADDRESSVAR", (result.data[i].leadEmailMessageReplayList[j].fromName)).replace("TIMEAGOVAR", fromNow(result.data[i].leadEmailMessageReplayList[j].createdDate)).replace("SUBJECTVAR", result.data[i].leadEmailMessageReplayList[j].subject).replace("BODYVAR", result.data[i].leadEmailMessageReplayList[j].body);
                        if (result.data[i].leadEmailMessageReplayList[j].leadEmailMessageReplayAttachement.length > 0) {
                            for (k = 0; k < result.data[i].leadEmailMessageReplayList[j].leadEmailMessageReplayAttachement.length; k = j) {
                                //alert(NestedRow);
                                //alert(result.data[i].leadEmailMessageReplayList[j].leadEmailMessageReplayAttachement[k].fileName);
                                //html = row += NestedRow.replace("ATTACHMENTVAR", result.data[i].leadEmailMessageReplayList[j].leadEmailMessageReplayAttachement[k].fileName);
                                html = row += NestedRow.replace("ATTACHMENTVAR", "<a style=\"color:#BF0D1C\" src =\"javascript:void(0)\" onclick=\"fnDownloadMailAttachment('" + result.data[i].leadEmailMessageReplayList[j].leadEmailMessageReplayAttachement[k].fileName + "'," + result.data[i].leadEmailMessageReplayList[j].leadEmailMessageReplayAttachement[k].leadId + ");\"><i class=\"fa fa-paperclip\" aria-hidden=\"true\"></i>&nbsp;" + result.data[i].leadEmailMessageReplayList[j].leadEmailMessageReplayAttachement[k].fileName + "</a>").replace("LEADIDVAR", result.data[i].leadEmailMessageReplayList[j].leadId).replace("LEADMSGIDVAR", result.data[i].leadEmailMessageReplayList[j].leadEmailMessageId).replace("TOADDRESSVAR", (result.data[i].leadEmailMessageReplayList[j].toName)).replace("FROMADDRESSVAR", (result.data[i].leadEmailMessageReplayList[j].fromName)).replace("TIMEAGOVAR", fromNow(result.data[i].leadEmailMessageReplayList[j].createdDate)).replace("SUBJECTVAR", result.data[i].leadEmailMessageReplayList[j].subject).replace("BODYVAR", result.data[i].leadEmailMessageReplayList[j].body);
                                //html = html + NestedRow.replace("ATTACHMENTVAR", result.data[i].leadEmailMessageReplayList[j].leadEmailMessageReplayAttachement[k].fileName).replace("LEADIDVAR", result.data[i].leadEmailMessageReplayList[j].leadId).replace("LEADMSGIDVAR", result.data[i].leadEmailMessageReplayList[j].leadEmailMessageId).replace("TOADDRESSVAR", (result.data[i].leadEmailMessageReplayList[j].toName)).replace("FROMADDRESSVAR", (result.data[i].leadEmailMessageReplayList[j].fromName)).replace("TIMEAGOVAR", fromNow(result.data[i].leadEmailMessageReplayList[j].createdDate)).replace("SUBJECTVAR", result.data[i].leadEmailMessageReplayList[j].subject).replace("BODYVAR", result.data[i].leadEmailMessageReplayList[j].body)
                            }
                        }
                        else {
                            //html = row += NestedRow.replace(" ATTACHMENTVAR ","1111");
                            html = row += NestedRow.replace("ATTACHMENTVAR", "").replace("LEADIDVAR", result.data[i].leadEmailMessageReplayList[j].leadId).replace("LEADMSGIDVAR", result.data[i].leadEmailMessageReplayList[j].leadEmailMessageId).replace("TOADDRESSVAR", (result.data[i].leadEmailMessageReplayList[j].toName)).replace("FROMADDRESSVAR", (result.data[i].leadEmailMessageReplayList[j].fromName)).replace("TIMEAGOVAR", fromNow(result.data[i].leadEmailMessageReplayList[j].createdDate)).replace("SUBJECTVAR", result.data[i].leadEmailMessageReplayList[j].subject).replace("BODYVAR", result.data[i].leadEmailMessageReplayList[j].body);
                        }
                    }
                 
                    if (result.data[i].leadEmailMessageattachement.length > 0) {
                        for (ji = 0; ji < result.data[i].leadEmailMessageattachement.length; ji++) {
                            html = row += NestedRow.replace("ATTACHMENTVAR", "<a style=\"color:#BF0D1C\" src=\"javascript:void(0)\" onclick=\"fnDownloadMailAttachment('" + result.data[i].leadEmailMessageattachement[ji].fileName + "'," + result.data[i].leadEmailMessageattachement[ji].leadId + ");\">&nbsp;<i class=\"fa fa-paperclip\" aria-hidden=\"true\"></i>&nbsp;" + result.data[i].leadEmailMessageattachement[ji].fileName + "</a>").replace("LEADIDVAR", result.data[i].leadId).replace("LEADMSGIDVAR", result.data[i].leadEmailMessageId).replace("TOADDRESSVAR", (result.data[i].toName)).replace("FROMADDRESSVAR", (result.data[i].fromName)).replace("TIMEAGOVAR", fromNow(result.data[i].createdDate)).replace("SUBJECTVAR", result.data[i].subject).replace("BODYVAR", result.data[i].body);
                        }
                    }
                    else {
                        html = row += NestedRow.replace("ATTACHMENTVAR", "").replace("LEADIDVAR", result.data[i].leadId).replace("LEADMSGIDVAR", result.data[i].leadEmailMessageId).replace("TOADDRESSVAR", (result.data[i].toName)).replace("FROMADDRESSVAR", (result.data[i].fromName)).replace("TIMEAGOVAR", fromNow(result.data[i].createdDate)).replace("SUBJECTVAR", result.data[i].subject).replace("BODYVAR", result.data[i].body);
                    }
                    html = html + "</div>";
                }
                else {
                    html = "<div class=\"custom-card-header\">" +
                        "<div class=\"chat-icon btn-icon\">" +
                        "<i class=\"fa fa-envelope\" aria-hidden=\"true\"></i>" +
                        "</div > " +
                        " <div class=\"chat-header-detail flex-grow-1 ml-3\"> " +
                        "<h5 style=\"text-transform:capitalize\">" +
                        (result.data[i].fromName) +
                        " <i class=\"fa fa-chevron-right\"></i> " +
                        (result.data[i].toName) +
                        "</h5> " +
                        fromNow(result.data[i].createdDate) +
                        "<h5>" + result.data[i].subject + "</h5> " +
                        "</div> " +
                        "<div class=\"chat-action\"> " +
                        "<button class=\"btn btn-secondary\" onclick=\"fnGetEmailSubject(" + result.data[i].leadEmailMessageId + "," + result.data[i].leadId + ");\">Reply</button> " +
                        "<button class=\"btn btn-secondary\" style=\"display:none\"> Reply All</button> " +
                        "<button class=\"btn btn-secondary\"><i class=\"fa fa-ellipsis-v\" aria-hidden=\"true\"></i></button>" +
                        "</div>" +
                        "</div>" +
                        "<div class=\"custom-card-text ml-5\"> " +
                        result.data[i].body +
                        "</div>";
                    if (result.data[i].leadEmailMessageattachement.length > 0) {
                        for (ji = 0; ji < result.data[i].leadEmailMessageattachement.length; ji++) {
                            html = html += "<div class=\"custom-card-text ml-5\">" +
                                "<a style=\"color:#BF0D1C\" src=\"javascript:void(0)\" onclick=\"fnDownloadMailAttachment('" + result.data[i].leadEmailMessageattachement[ji].fileName + "'," + result.data[i].leadEmailMessageattachement[ji].leadId + ");\"><i class=\"fa fa-paperclip\" aria-hidden=\"true\"></i>&nbsp; " + result.data[i].leadEmailMessageattachement[ji].fileName + "</a>" +
                                "</div>";
                        }
                    }
                }
                var divider = html + "<div class=\"divider my-5\"></div>";
                $("#divMailList").append(divider);
            }
        }
        else {

            $("#divMailList").css('text-align', 'center').css('color', '#777').append("No Mail Found.");
        }
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

function fnGetEmailSubject(leadEmailMessageId, LeadId) {
    //var elmntToView = document.getElementById("send-email");
    //elmntToView.scrollIntoView(); 
    var formData = new FormData();
    formData.append("LeadEmailMessageId", leadEmailMessageId);
    formData.append("LeadId", LeadId);
    var result = __glb_fnIUDOperation(formData, "/Lead/GetMessageSubjecyByLeadMessageId");
    if (result.success === true) {
        console.log(result);
        document.getElementById("txtMailSubject").value = "Re:" + result.subject;
        document.getElementById("hdnLeadEmailMsgId").value = result.leadEmailMessageId;
        document.getElementById("hdnEmailMessageId").value = result.emailMessageId;
        document.getElementById("hdnFromName").value = result.fromName;
        document.getElementById("hdnToName").value = result.toName;
        document.getElementById("hdnIsReplay").value = true;
        //alert(document.getElementById("hdnIsReplay").value);
        $('html, body').animate({
            scrollTop: $("#send-email").offset().top
        }, 500);
    }
    else if (result.success === false) {
        window.location = "/";
    }

}

function fnDownloadMailAttachment(fileName, LeadID) {
    //alert(fileName);
    //alert(LeadID);
    //window.location = window.location.origin + '/Lead/DownloadMailAttachmentFile?fileName=' + fileName + "&LeadId=" + LeadID;
    window.location = window.location.origin + '/Lead/DownloadMailAttachmentFile?LeadId=' + LeadID + "&fileName=" + fileName;
}
function fromNow(date) {
    const SECOND = 1000;
    const MINUTE = 60 * SECOND;
    const HOUR = 60 * MINUTE;
    const DAY = 24 * HOUR;
    const WEEK = 7 * DAY;
    const MONTH = 30 * DAY;
    const YEAR = 365 * DAY;
    const units = [
        { max: 30 * SECOND, divisor: 1, past1: 'just now', pastN: 'just now', future1: 'just now', futureN: 'just now' },
        { max: MINUTE, divisor: SECOND, past1: 'a second ago', pastN: '# seconds ago', future1: 'in a second', futureN: 'in # seconds' },
        { max: HOUR, divisor: MINUTE, past1: 'a minute ago', pastN: '# minutes ago', future1: 'in a minute', futureN: 'in # minutes' },
        { max: DAY, divisor: HOUR, past1: 'an hour ago', pastN: '# hours ago', future1: 'in an hour', futureN: 'in # hours' },
        { max: WEEK, divisor: DAY, past1: 'yesterday', pastN: '# days ago', future1: 'tomorrow', futureN: 'in # days' },
        { max: 4 * WEEK, divisor: WEEK, past1: 'last week', pastN: '# weeks ago', future1: 'in a week', futureN: 'in # weeks' },
        { max: YEAR, divisor: MONTH, past1: 'last month', pastN: '# months ago', future1: 'in a month', futureN: 'in # months' },
        { max: 100 * YEAR, divisor: YEAR, past1: 'last year', pastN: '# years ago', future1: 'in a year', futureN: 'in # years' },
        { max: 1000 * YEAR, divisor: 100 * YEAR, past1: 'last century', pastN: '# centuries ago', future1: 'in a century', futureN: 'in # centuries' },
        { max: Infinity, divisor: 1000 * YEAR, past1: 'last millennium', pastN: '# millennia ago', future1: 'in a millennium', futureN: 'in # millennia' },
    ];
    const diff = Date.now() - (typeof date === 'object' ? date : new Date(date)).getTime();
    const diffAbs = Math.abs(diff);
    for (const unit of units) {
        if (diffAbs < unit.max) {
            const isFuture = diff < 0;
            const x = Math.round(Math.abs(diff) / unit.divisor);
            if (x <= 1) return isFuture ? unit.future1 : unit.past1;
            return (isFuture ? unit.futureN : unit.pastN).replace('#', x);
        }
    }
};

function capitalize(input) {
    return input.toLowerCase().split(' ').map(s => s.charAt(0).toUpperCase() + s.substring(1)).join(' ');
}
