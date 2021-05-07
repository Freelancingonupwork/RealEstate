$(document).ready(function () {
    $('#FieldTypeId').change(function () {
        var ddlValue = $(this).val();

        if (ddlValue == "4") {
            // show time div, hide fromTo div
            $('#TextBoxContainer').show();
        }
        else {
            // show fromTo div, hide time div
            $('#TextBoxContainer').hide();
        }
    });
    $("#btnSubmitCustomField").click(function () {
        if ($("input[name='DynamicTextBox']").val() == "") {
            //alert("There is no value in textbox");
            $($("input[name='DynamicTextBox']").css({
                "border": "1px solid red",
                "background": "#FFCECE"
            }));
            return;
        }
    });
});
function GetDynamicTextBox(value) {
    //alert(value.fieldValue);
    var div = $("<div style='display:flex' class='mt-2'/>");

    var textBox = $("<input />").attr("type", "textbox").attr("name", "DynamicTextBox").attr("class", "form-control").attr("style", "width:70%");
    if (value != "") {
        textBox.val(value.fieldValue);
    }
    else {
        textBox.val(value);
    }
    div.append(textBox).append("&nbsp;&nbsp;");

    var button = $("<input />").attr("type", "button").attr("value", "Remove");
    button.attr("onclick", "RemoveTextBox(this)");

    div.append(button);

    return div;
}
function AddTextBox() {
    var div = GetDynamicTextBox("");
    $("#TextBoxContainer").append(div);
}

function RemoveTextBox(button) {
    $(button).parent().remove();
}

function ConfirmationDialog(CustomFieldId) {
    if (confirm("Are you sure to delete?"))
        fnDeleteRecord(CustomFieldId);
    else
        return false;
}

function fnDeleteRecord(CustomFieldId) {

    var formData = new FormData();
    formData.append("CustomFieldId", CustomFieldId);
    var result = __glb_fnIUDOperation(formData, "/Admin/DeleteCustomField");
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

function fnPopulateControls(CustomFieldId, FieldTypeId) {
    if (FieldTypeId == 4) {
        $("#btnAdd").show();
    }
    else {
        $("#btnAdd").hide();
    }
    $.ajax({
        type: "GET",
        url: "/Admin/getCustomFieldType",
        data: "{}",
        //async: false,
        success: function (data) {
            var s = '';
            for (var i = 0; i < data.length; i++) {
                if (FieldTypeId == data[i].fieldTypeId) {
                    s += '<option selected value="' + data[i].fieldTypeId + '">' + data[i].fieldType + '</option>';
                }
                else {
                    s += '<option value="' + data[i].fieldTypeId + '">' + data[i].fieldType + '</option>';
                }
            }
            $("#cmbcustomFieldType").html(s);
        }
    });

    var formData = new FormData();
    formData.append("CustomFieldId", CustomFieldId);
    var result = __glb_fnIUDOperation(formData, "/Admin/GetCustomFieldDetails");
    if (result.success === true) {
        document.getElementById("hdnCustomFieldId").value = result.id;
        document.getElementById("hdnCustomFieldTypeId").value = result.fieldTypeId;
        document.getElementById("txtName").value = result.fieldName;
        var values = result.fieldValues;
        if (values.length > 0) {
            $('#TextBoxContainer').html('');
            $('#TextBoxContainer').show();
            $(values).each(function () {
                $("#TextBoxContainer").append(GetDynamicTextBox(this));
            });
        }
        else {
            $('#TextBoxContainer').hide();
        }
        //document.getElementById("cmbcustomFieldType").value = result.fieldTypeId;
        $("#divEditCustomField").modal('show');
    }
    else {
        showAlertMessage("danger", "Something went wrong! Please try again.");
        return;
    }

}
function fnUpdateCustomFieldDetails() {
    var CustomFieldId = document.getElementById("hdnCustomFieldId");
    var CustomFieldTypeId = document.getElementById("hdnCustomFieldTypeId");
    var Name = document.getElementById("txtName");
    var _liDropDownValue = new Array();
    $("input[name='DynamicTextBox']").each(function () {

        _liDropDownValue.push($(this).val());
    })
    var ddlValue = _liDropDownValue;
    var formData = new FormData();
    formData.append("CustomFieldId", CustomFieldId.value);
    formData.append("CustomFieldTypeId", CustomFieldTypeId.value);
    formData.append("Name", Name.value);
    formData.append("DropDownOption", ddlValue);

    var result = __glb_fnIUDOperation(formData, "/Admin/UpdateCustomFieldDetails");
    if (result.success === true) {
        $("#divEditCustomField").modal('hide');
        showAlertMessage("success", "Custom Field updated successfully.");
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