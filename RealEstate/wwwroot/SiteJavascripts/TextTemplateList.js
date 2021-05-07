function ConfirmationDialog(StageId) {
    if (confirm("Are you sure to delete?"))
        fnDeleteRecord(StageId);
    else
        return false;
}

function fnDeleteRecord(EmailTemplateID) {

    var formData = new FormData();
    formData.append("EmailTemplateID", EmailTemplateID);
    var result = __glb_fnIUDOperation(formData, "/Admin/DeleteTextTemplate");
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

function fnPopulateControls(EmailTemplateID) {

    var formData = new FormData();
    formData.append("EmailTemplateID", EmailTemplateID);
    var result = __glb_fnIUDOperation(formData, "/Admin/GetTextTemplateById");
    if (result.success === true) {
        document.getElementById("hdnEmailTemplateID").value = result.emailTemplateId;
        document.getElementById("txtEmailName").value = result.emailName;
        document.getElementById("txtDescription").value = result.emailTemplateDescription;
        document.getElementById("txtFromEmail").value = result.fromEmail;
        document.getElementById("txtEmailSubject").value = result.emailSubject;
        document.getElementById("cmdTemplateType").value = result.templateTypeId;
        document.getElementById("txtMailBody").value = result.body;
        $("#divEmailTemplateModel").modal('show');
    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }

}
function fnUpdateTextTemplate() {
    var EmailTemplateID = document.getElementById("hdnEmailTemplateID");
    var TemplateTypeID = document.getElementById("cmdTemplateType");
    var txtEmailName = document.getElementById("txtEmailName");
    var txtDescription = document.getElementById("txtDescription");
    var txtFromEmail = document.getElementById("txtFromEmail");
    var txtEmailSubject = document.getElementById("txtEmailSubject");
    var MailBody = document.getElementById('txtMailBody');
    if (TemplateTypeID.value == '') {
        $('#divEmailTemplateModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Template type is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    if (txtEmailName.value == '') {
        $('#divEmailTemplateModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Email name is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    if (txtDescription.value == '') {
        $('#divEmailTemplateModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Description is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    if (txtFromEmail.value == '') {
        $('#divEmailTemplateModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("From email is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    if (txtEmailSubject.value == '') {
        $('#divEmailTemplateModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Email subject is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    if (MailBody.value == '') {
        $('#divEmailTemplateModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Mail body is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }


    var formData = new FormData();
    formData.append("EmailTemplateID", EmailTemplateID.value);
    formData.append("TemplateTypeID", TemplateTypeID.value);
    formData.append("EmailName", txtEmailName.value);
    formData.append("Description", txtDescription.value);
    formData.append("FromEmail", txtFromEmail.value);
    formData.append("EmailSubject", txtEmailSubject.value);
    formData.append("MailBody", MailBody.value);

    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateTextTemplate");
    if (result.success === true) {
        $("#divEmailTemplateModel").modal('hide');
        showAlertMessage("success", "Text template updated successfully.");
    }
    else {
        showAlertMessageToEditTemplate(result.message);
        return;
    }
}

function showAlertMessageToEditTemplate(message) {
    $('#divEmailTemplateModel').animate({ scrollTop: 0 }, "slow");
    $("#divErrorMsgEmailTemplate").html(message);
    $('#divErrorMsgEmailTemplate').show();
    document.getElementById("divErrorMsgEmailTemplate").classList.add("alert", "alert-warning");
    $(function () {
        $("#divErrorMsgEmailTemplate").fadeOut(4000);
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

function fnActivateDeactivateTemplate(EmailTemplateID, ADAFlag) {
    var formData = new FormData();
    formData.append("EmailTemplateID", EmailTemplateID);
    formData.append("flag", ADAFlag);
    var result = __glb_fnIUDOperation(formData, '/Admin/ActivateDeactivateTextTemplate');
    if (result.act == true) {
        showAlertMessage("success", result.message);
        $("html, body").animate({ scrollTop: 0 }, "slow");
    }
    else if (result.deAct == true) {
        showAlertMessage("success", result.message);
        $("html, body").animate({ scrollTop: 0 }, "slow");
    }
    else if (result.success == false) {
        showAlertMessage("danger", result.message);
        $("html, body").animate({ scrollTop: 0 }, "slow");
    }
}