jQuery(document).ready(function () {
    /*document.getElementById("liLead").classList.add("active");*/
    populateComboBox();

    $("#cmbLeadSource").change(function () {
        $("#hdnLeadSource").val($("#cmbLeadSource").val());
    });
    $("#btnSaveSource").click(function () {
        var sourceName = $("#txtSourceName");
        if (__glb_fnIsNullOrEmpty(sourceName.val())) {
            showAlertMessage("danger", "Source Name Can't Be Blank!");
            sourceName.focus();
            return;
        }
        var formData = new FormData();
        formData.append("name", sourceName.val());
        var result = __glb_fnIUDOperation(formData, "/Lead/AddSource");
        if (result.success === true) {
            populateComboBox();
            $("#divAddSource").modal("hide");
            sourceName.val("");
        }
        else {
            showAlertMessage("warning", result.message);
            return;
        }
    });

    $("#btnSaveStage").click(function () {
        var stageName = $("#txtStageName");
        if (__glb_fnIsNullOrEmpty(stageName.val())) {
            showAlertMessage("danger", "Stage Name Can't Be Blank!");
            stageName.focus();
            return;
        }
        var formData = new FormData();
        formData.append("name", stageName.val());
        var result = __glb_fnIUDOperation(formData, "/Lead/AddStage");
        if (result.success === true) {
            $("#StageModal").modal("hide");
            stageName.val("");
            window.location.reload();
        }
        else {
            showAlertMessage("warning", result.message);
            return;
        }
    });
});
function populateComboBox() {
    var cmbLeadSource = $("#cmbLeadSource");
    $.ajax({
        type: "GET",
        url: "/Lead/GetAllSources",
        data: "{}",
        success: function (data) {
            var SourceData = '';
            for (var i = 0; i < data.length; i++) {
                SourceData += "<li><a style='color:black;' href='javascript:void()' onclick='fnSetValue(\"" + data[i].name + "\")'>" + data[i].name + "</a><a style='float:right;' href='javascript:void()' onclick='return ConfirmationDialog(" + data[i].id + ")'><i class='fa fa-trash'></i></a></li>";
            }
            cmbLeadSource.html(SourceData);
        }
    });
}

function showAlertMessage(type, message) {
  
    if (type == "danger") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();

        $("#divAlertMessageStage").html(message);
        $('#divAlertMessageStage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-danger");
        document.getElementById("divAlertMessageStage").classList.add("alert", "alert-danger");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
            $("#divAlertMessageStage").fadeOut();
        }, 5000);
    }

    if (type == "success") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-success");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
        }, 5000);
    }

    if (type == "warning") {
        $("#divAlertMessage").html(message);
        $('#divAlertMessage').show();
        document.getElementById("divAlertMessage").classList.add("alert", "alert-warning");
        setTimeout(function () {
            $("#divAlertMessage").fadeOut();
        }, 5000);
    }
}

function ConfirmationDialog(ID) {
    if (confirm("Are you sure to delete?"))
        fnDeleteRecord(ID);
    else
        return false;
}

function fnDeleteRecord(id) {

    var formData = new FormData();
    formData.append("id", id);
    var result = __glb_fnIUDOperation(formData, "/Lead/DeleteSource");
    if (result.success === true) {
        populateComboBox();
        $("#btnDropdown").html("Select Lead Source");
        $("#hdnLeadSource").val("");
        $("#btnDropdown").focus();
    }
    else {
        return;
    }
}

function fnSetValue(name) {
    $("#btnDropdown").html(name);
    $("#hdnLeadSource").val(name);
    $("#btnDropdown").focus();
}