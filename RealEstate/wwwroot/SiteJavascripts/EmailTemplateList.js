function ConfirmationDialog(StageId) {
    if (confirm("Are you sure to delete?"))
        fnDeleteRecord(StageId);
    else
        return false;
}

function fnDeleteRecord(EmailTemplateID) {

    var formData = new FormData();
    formData.append("EmailTemplateID", EmailTemplateID);
    var result = __glb_fnIUDOperation(formData, "/Admin/DeleteEmailTemplate");
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
    var result = __glb_fnIUDOperation(formData, "/Admin/GetEmailTemplateById");
    if (result.success === true) {
        document.getElementById("hdnEmailTemplateID").value = result.emailTemplateId;
        document.getElementById("txtEmailName").value = result.emailName;
        document.getElementById("txtDescription").value = result.emailTemplateDescription;
        document.getElementById("txtFromEmail").value = result.fromEmail;
        document.getElementById("txtEmailSubject").value = result.emailSubject;
        document.getElementById("cmdTemplateType").value = result.templateTypeId;
        CKEDITOR.instances['txtMailBody'].setData(result.body)
        $("#divEmailTemplateModel").modal('show');
    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }

}
function fnUpdateEmailTemplate() {
    var EmailTemplateID = document.getElementById("hdnEmailTemplateID");
    var TemplateTypeID = document.getElementById("cmdTemplateType");
    var txtEmailName = document.getElementById("txtEmailName");
    var txtDescription = document.getElementById("txtDescription");
    var txtFromEmail = document.getElementById("txtFromEmail");
    var txtEmailSubject = document.getElementById("txtEmailSubject");
    var MailBody = CKEDITOR.instances['txtMailBody'].getData()
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
    if (MailBody == '') {
        $('#divEmailTemplateModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Mail body is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

    if (!filter.test(txtFromEmail.value)) {
        $('#divEmailTemplateModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Please provide a valid email address.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        txtFromEmail.focus;
        return;
    }

    var formData = new FormData();
    formData.append("EmailTemplateID", EmailTemplateID.value);
    formData.append("TemplateTypeID", TemplateTypeID.value);
    formData.append("EmailName", txtEmailName.value);
    formData.append("Description", txtDescription.value);
    formData.append("FromEmail", txtFromEmail.value);
    formData.append("EmailSubject", txtEmailSubject.value);
    formData.append("MailBody", MailBody);

    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateEmailTemplate");
    if (result.success === true) {
        $("#divEmailTemplateModel").modal('hide');
        showAlertMessage("success", "Email template updated successfully.");
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
    var result = __glb_fnIUDOperation(formData, '/Admin/ActivateDeactivateEmailTemplate');
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

function fnPopulateControlsEmailTemplate(TemplateCategoryHTMLEmailID) {
   
    var formData = new FormData();
    formData.append("TemplateCategoryHTMLEmailID", TemplateCategoryHTMLEmailID);
    var result = __glb_fnIUDOperation(formData, "/Admin/GetTemplateCategoryHTMLEmailById");
    if (result.success === true) {
        document.getElementById("hdnTemplateCategoryHtmlemailId").value = result.templateCategoryHtmlemailId;
        document.getElementById("hdnTemplateCategoryId").value = result.templateCategoryId;
        CKEDITOR.replace("txtMailBody");
        CKEDITOR.instances['txtMailBody'].setData(result.templateHtmlemail)
        $("#divEmailTemplateListModel").modal('show');
    }
    else {
        showAlertMessage("danger", result.message);
        return;
    }
}

$("#ChkStatus").on('change', function () {
    if ($(this).is(':checked')) {
        $(this).attr('value', 'true');
    } else {
        $(this).attr('value', 'false');
    }
});


function fnUpdateTemplateCategoryHTMLEmail() {
    //var EmailTemplateID = document.getElementById("hdnEmailTemplateID");
    var TemplateTypeID = document.getElementById("cmdTemplateType");
    var txtEmailName = document.getElementById("txtEmailName");
    var txtDescription = document.getElementById("txtDescription");
    var txtFromEmail = document.getElementById("txtFromEmail");
    var txtEmailSubject = document.getElementById("txtEmailSubject");
    var Status = $('#ChkStatus').val();
    var MailBody = CKEDITOR.instances['txtMailBody'].getData()
    if (TemplateTypeID.value == '') {
        $('#divEmailTemplateListModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Template type is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    if (txtEmailName.value == '') {
        $('#divEmailTemplateListModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Email name is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    if (txtDescription.value == '') {
        $('#divEmailTemplateListModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Description is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    if (txtFromEmail.value == '') {
        $('#divEmailTemplateListModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("From email is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    if (txtEmailSubject.value == '') {
        $('#divEmailTemplateListModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Email subject is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }
    if (MailBody == '') {
        $('#divEmailTemplateListModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Mail body is requied.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        return;
    }

    var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

    if (!filter.test(txtFromEmail.value)) {
        $('#divEmailTemplateListModel').animate({ scrollTop: 0 }, "slow");
        $("#divErrorMsgEmailTemplate").html("Please provide a valid email address.");
        $('#divErrorMsgEmailTemplate').show();
        $("#divErrorMsgEmailTemplate").addClass("alert alert-danger");
        setTimeout(function () {
            $("#divErrorMsgEmailTemplate").fadeOut();
        }, 5000);
        txtFromEmail.focus;
        return;
    }

    var formData = new FormData();
    //formData.append("EmailTemplateID", EmailTemplateID.value);
    formData.append("TemplateTypeID", TemplateTypeID.value);
    formData.append("EmailName", txtEmailName.value);
    formData.append("Description", txtDescription.value);
    formData.append("FromEmail", txtFromEmail.value);
    formData.append("EmailSubject", txtEmailSubject.value);
    formData.append("MailBody", MailBody);
    formData.append("Status", Status);

    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateTemplateCategoryHTMLEmail");
    if (result.success === true) {
        $("#divEmailTemplateListModel").modal('hide');
        showAlertMessage("success", "Email template updated successfully.");
        setTimeout(function () {
            window.location = "/Admin/EmailTemplate";
        }, 3000);
    }
    else {
        showAlertMessageToEditTemplate(result.message);
        return;
    }
}