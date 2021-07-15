// Listen for click on toggle checkbox
$('#customCheck1').click(function (event) {
    if (this.checked) {
        // Iterate each checkbox
        $(':checkbox').each(function () {
            this.checked = true;
        });
    } else {
        $(':checkbox').each(function () {
            this.checked = false;
        });
    }
});


$("#pills-home-tab").click(function () {
    $("#idmain-social").show();
    $("#pills-home-tab-leftside").css("color", "#BF0D1C");
    $("#pills-contact-tab-leftside").removeAttr("style");
    $("#pills-home-tab").addClass("active");
    $("#pills-profile-tab").removeClass("active");
    $("#pills-contact-tab").removeClass("active");
    $("#pills-text-tab-leftside").removeAttr("style");
    $("#pills-home").addClass("show active");
    $("#pills-contact").removeClass("show active");
    $("#pills-profile").removeClass("show active");
});

$("#pills-profile-tab").click(function () {
    $("#pills-home").removeClass("show active");
    $("#pills-contact").removeClass("show active");
    $("#pills-profile").addClass("show active");
    $("#idmain-social").hide();
    $("#pills-profile-tab").addClass("active");
    $("#pills-home-tab").removeClass("active");
    $("#pills-contact-tab").removeClass("active");
    $("#pills-text-tab-leftside").css("color", "#BF0D1C");
    $("#pills-home-tab-leftside").removeAttr("style");
    $("#pills-contact-tab-leftside").removeAttr("style");
    fnGetTextMessage();
});

$("#pills-contact-tab").click(function () {
    $("#pills-home").removeClass("show active");
    $("#pills-contact").addClass("show active");
    $("#pills-profile").removeClass("show active");
    $("#idmain-social").hide();
    $("#pills-contact-tab").addClass("active");
    $("#pills-profile-tab").removeClass("active");
    $("#pills-home-tab").removeClass("active");
    $("#pills-contact-tab-leftside").css("color", "#BF0D1C");
    $("#pills-text-tab-leftside").removeAttr("style");
    $("#pills-home-tab-leftside").removeAttr("style");
});


$("#pills-home-tab-leftside").click(function () {
    $("#idmain-social").show();
    $("#pills-home-tab-leftside").css("color", "#BF0D1C");
    $("#pills-home-tab").addClass("active");
    $("#pills-profile-tab").removeClass("active");
    $("#pills-text-tab-leftside").removeAttr("style");
    $("#pills-contact-tab-leftside").removeAttr("style");
    $("#pills-contact-tab").removeClass("active");
    $("#pills-home").addClass("show active");
    $("#pills-contact").removeClass("show active");
    $("#pills-profile").removeClass("show active");
});

$("#pills-text-tab-leftside").click(function () {
    $("#idmain-social").hide();
    $("#pills-home").removeClass("show active");
    $("#pills-contact").removeClass("show active");
    $("#pills-profile").addClass("show active");
    $("#pills-home-tab-leftside").removeAttr("style");
    $("#pills-home-tab").removeClass("active");
    $("#pills-profile-tab").addClass("active");
    $("#pills-contact-tab").removeClass("active");
    $("#pills-contact-tab-leftside").removeAttr("style");
    $(this).css("color", "#BF0D1C");
    fnGetTextMessage();
});


$("#pills-contact-tab-leftside").click(function () {
    $(this).css("color", "#BF0D1C");
    $("#pills-text-tab-leftside").removeAttr("style");
    $("#pills-home-tab-leftside").removeAttr("style");
    $("#idmain-social").hide();
    $("#pills-home-tab").removeClass("active");
    $("#pills-profile-tab").removeClass("active");
    $("#pills-contact-tab").addClass("active");
    $("#pills-home").removeClass("show active");
    $("#pills-contact").addClass("show active");
    $("#pills-profile").removeClass("show active");
});

function fndeleteBulkMail() {
    var checkedMailId = new Array();
    $("input[type='checkbox'][id^='customCheck_']").each(function () {
        this.checked ? checkedMailId.push($(this).val()) : null;
    });

    if (checkedMailId.length <= 0) {
        showAlertMessage("warning", "Select atleast one mail for delete!")
        return;
    }

    var strIds = '';
    for (var i = 0; i < checkedMailId.length; i++) {
        if (i + 1 == checkedMailId.length) {
            strIds += checkedMailId[i];
        }
        else {
            strIds += checkedMailId[i] + ',';
        }

    }
    var formData = new FormData();
    formData.append("ids", strIds);
    var result = __glb_fnIUDOperation(formData, "/MailBox/DeleteMailByIds");
    if (result.success === true) {
        showAlertMessage("success", "Mails deleted successfully");
        $(function () {
            window.location.reload();
        }, 3000);
    }
    else if (result.success === false) {
        showAlertMessage("warning", result.message);
        return;
    }
}

function deleteSingleMail(LeadEmailMessageId) {
    if (confirm("Are you sure to delete?"))
        fndeleteSingleMail(LeadEmailMessageId);
    else
        return false;
}

function fndeleteSingleMail(LeadEmailMessageId) {
    var formData = new FormData();
    formData.append("LeadEmailMessageId", LeadEmailMessageId);
    var result = __glb_fnIUDOperation(formData, "/MailBox/DeleteMailById");
    if (result.success === true) {
        showAlertMessage("success", "Mail deleted successfully");
        $(function () {
            window.location.reload();
        }, 3000);
    }
    else if (result.success === false) {
        showAlertMessage("warning", result.message);
        return;
    }
}

function fnMarkasReadMail(LeadEmailMessageId) {

    var checkedMailId = new Array();

    var formData = new FormData();
    if (LeadEmailMessageId == undefined) {
        $("input[type='checkbox'][id^='customCheck_']").each(function () {
            this.checked ? checkedMailId.push($(this).val()) : null;
        });

        if (checkedMailId.length <= 0) {
            showAlertMessage("warning", "Select atleast one mail for mark as read!")
            return;
        }

        var strIds = '';
        for (var i = 0; i < checkedMailId.length; i++) {
            if (i + 1 == checkedMailId.length) {
                strIds += checkedMailId[i];
            }
            else {
                strIds += checkedMailId[i] + ',';
            }

        }
        formData.append("ids", strIds);
    }
    else {
        formData.append("ids", LeadEmailMessageId);
    }


    var result = __glb_fnIUDOperation(formData, "/MailBox/MailMarkasread");
    if (result.success === true) {
        showAlertMessage("success", result.message);
        $(function () {
            window.location.reload();
        }, 3000);
    }
    else if (result.success === false) {
        showAlertMessage("warning", result.message);
        return;
    }
}

function fnMarkAsUnReadMail(LeadEmailMessageId) {
    var checkedMailId = new Array();
    var formData = new FormData();

    if (LeadEmailMessageId == undefined) {
        $("input[type='checkbox'][id^='customCheck_']").each(function () {
            this.checked ? checkedMailId.push($(this).val()) : null;
        });

        if (checkedMailId.length <= 0) {
            showAlertMessage("warning", "Select atleast one mail for mark as unread!")
            return;
        }


        var strIds = '';
        for (var i = 0; i < checkedMailId.length; i++) {
            if (i + 1 == checkedMailId.length) {
                strIds += checkedMailId[i];
            }
            else {
                strIds += checkedMailId[i] + ',';
            }

        }

        formData.append("ids", strIds);
    }
    else {
        formData.append("ids", LeadEmailMessageId);
    }
    var result = __glb_fnIUDOperation(formData, "/MailBox/MailMarkasunread");
    if (result.success === true) {
        showAlertMessage("success", result.message);
        $(function () {
            window.location.reload();
        }, 3000);
    }
    else if (result.success === false) {
        showAlertMessage("warning", result.message);
        return;
    }
}

function fnRefreshMail() {
    location.reload();
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

function fnGetTextMessage() {

    var formData = new FormData(); //FormData object
    var result = __glb_fnIUDOperation(formData, "/MailBox/GetAllTextMessage");
    if (result.success === true) {
        $("#divTextMessageList").html('');
        $("#divTextMessageList").removeAttr("style");
        if (result.data != undefined) {
            if (result.data.length > 0) {
                console.log(result);
                for (i = 0; i < result.data.length; i++) {
                    var html;
                    let d = new Date(result.data[i].createdDate),
                        t = d.toDateString().split(" ");
                    //alert(bbbb);
                    html = "<div class=\"new-mail d-flex align-items-center bg-white\">" +
                        //"<div class=\"mail-icon d-flex align-items-center\" >" +
                        ////"<div class=\"custom-control custom-checkbox\">" +
                        ////"<input type=\"checkbox\" class=\"custom-control-input\" id=\"customCheck3\">" +
                        ////"<label class=\"custom-control-label\" for=\"customCheck3\"></label>" +
                        ////"</div>" +
                        ////"<div class=\"star\">" +
                        ////"<i class=\"fa fa-star\"></i>" +
                        ////"</div>" +
                        //"</div>" +
                        "<div class=\"mail-content\">" +
                        "<div class=\"mail-title\">" +
                        "<div class=\"mail-name\">" + result.data[i].toName + "</div>" +
                        "</div>" +
                        "<div class=\"mail-des\">" +
                        "<div class=\"mail-sort-des\">" +
                        result.data[i].body +
                        "</div>" +
                        "</div>" +
                        "<div class=\"mail-time\">" +
                        "<div class=\"mail-icon d-flex\">" +
                        "<i type=\"button\" data-toggle=\"tooltip\" data-placement=\"top\" title=\"\" data-original-title=\"Delete\" class=\"fa fa-trash\"></i>" +
                        "</div > " +
                        "<span>" + t[1] + "&nbsp;" + t[2] + "</span>" +
                        "</div>" +
                        "</div>" +
                        "</div>";
                    $("#divTextMessageList").append(html);
                }
            }
            else {
                $("#divTextMessageList").css('text-align', 'center').css('color', '#777').append("No Text Message Found.");
            }
        }
        else {
            $("#divTextMessageList").css('text-align', 'center').css('color', '#777').append(result.message).addClass("new-mail pt-3");
        }
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