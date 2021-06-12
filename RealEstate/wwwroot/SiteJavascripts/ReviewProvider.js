function fnSetKey(aoData, sKey, mValue) {
    for (var i = 0, iLen = aoData.length; i < iLen; i++) {
        if (aoData[i].name == sKey) {
            aoData[i].value = mValue;
        }
    }
}

function fnGetKey(aoData, sKey) {
    for (var i = 0, iLen = aoData.length; i < iLen; i++) {
        if (aoData[i].name == sKey) {
            return aoData[i].value;
        }
    }
    return null;
}

var oCache = {
    iCacheLower: -1
};

function fnDataTablesPipeline(sSource, aoData, fnCallback) {
    var bNeedServer = false;
    var sEcho = fnGetKey(aoData, "sEcho");
    var iRequestStart = fnGetKey(aoData, "iDisplayStart");
    var iRequestLength = fnGetKey(aoData, "iDisplayLength");

    var iPipe = 100 / iRequestLength; /* Ajust the pipe size */

    var iRequestEnd = iRequestStart + iRequestLength;
    oCache.iDisplayStart = iRequestStart;

    /* outside pipeline? */
    if (oCache.iCacheLower < 0 || iRequestStart < oCache.iCacheLower || iRequestEnd > oCache.iCacheUpper) {
        bNeedServer = true;
    }

    /* sorting etc changed? */
    if (oCache.lastRequest && !bNeedServer) {
        for (var i = 0, iLen = aoData.length; i < iLen; i++) {
            if (aoData[i].name != "iDisplayStart" && aoData[i].name != "iDisplayLength" && aoData[i].name != "sEcho") {
                if (aoData[i].value != oCache.lastRequest[i].value) {
                    bNeedServer = true;
                    break;
                }
            }
        }
    }

    /* Store the request for checking next time around */
    oCache.lastRequest = aoData.slice();

    if (bNeedServer) {
        if (iRequestStart < oCache.iCacheLower) {
            iRequestStart = iRequestStart - (iRequestLength * (iPipe - 1));
            if (iRequestStart < 0) {
                iRequestStart = 0;
            }
        }

        oCache.iCacheLower = iRequestStart;
        oCache.iCacheUpper = iRequestStart + (iRequestLength * iPipe);
        oCache.iDisplayLength = fnGetKey(aoData, "iDisplayLength");
        fnSetKey(aoData, "iDisplayStart", iRequestStart);
        fnSetKey(aoData, "iDisplayLength", iRequestLength * iPipe);
        var pagingInfo = {};
        pagingInfo.StartRecordNumber = iRequestStart;
        pagingInfo.PageSize = iRequestLength * iPipe;

        switch (fnGetKey(aoData, "iSortCol_0")) {
            case 1:
                pagingInfo.SortColumn = "FirstName";
                break;
            case 2:
                pagingInfo.SortColumn = "LastName";
                break;
            case 3:
                pagingInfo.SortColumn = "PhoneNumber";
                break;
            case 4:
                pagingInfo.SortColumn = "EmailAddress";
                break;
            case 5:
                pagingInfo.SortColumn = "Stage";
                break;
            case 6:
                pagingInfo.SortColumn = "LeadStatus";
                break;
            case 7:
                pagingInfo.SortColumn = "LeadSource";
                break;
            case 8:
                pagingInfo.SortColumn = "Industry";
                break;
            default:
                pagingInfo.SortColumn = "LeadId";
                break;
        }
        pagingInfo.SortDirection = fnGetKey(aoData, "sSortDir_0");
        pagingInfo.echo = sEcho;
        pagingInfo.Search = $.trim((fnGetKey(aoData, "sSearch")));

        $.ajax({
            'dataType': 'json',
            'contentType': 'application/json;',
            'type': 'GET',
            'data': aoData,
            'url': sSource + '?StartRecordNumber=' + iRequestStart + '&PageSize=' + iRequestLength * iPipe + '&SortColumn=' + pagingInfo.SortColumn + '&SortDirection=' + pagingInfo.SortDirection + '&echo=' + sEcho + "&Search=" + pagingInfo.Search,
            'success': function (msg) {
                var json = msg;
                if (json != null) {
                    oCache.lastJson = jQuery.extend(true, {}, json);
                    if (oCache.iCacheLower != oCache.iDisplayStart) {
                        json.aaData.splice(0, oCache.iDisplayStart - oCache.iCacheLower);
                    }
                    json.aaData.splice(oCache.iDisplayLength, json.aaData.length);
                    fnCallback(json);
                }

                if (typeof (msg.err) === "string") {
                    //displayNotification('Error processing data for provider list', MessageTypes.Error);
                }
                else if (typeof (msg.war) === "string") {
                    //displayNotification(msg.d.war, MessageTypes.Warning);
                }
            },
            'error': function (msg) {
                displayErrorFromWebServiceMethod('Error processing data for appointment list table', MessageTypes.Error)
            }
        });
    }
    else {
        var json = jQuery.extend(true, {}, oCache.lastJson);
        json.sEcho = sEcho; /* Update the echo for each response */
        json.aaData.splice(0, iRequestStart - oCache.iCacheLower);
        json.aaData.splice(iRequestLength, json.aaData.length);
        fnCallback(json);
        return;
    }
}

function ReDraw() {
    var oTable = $('#tblLeadList').dataTable();
    oCache = {
        iCacheLower: -1
    };
    oTable.fnDraw();
}

jQuery(document).ready(function () {
    var columnOptions = new Array();
    columnOptions.push({ "bSortable": false, "aTargets": [0] });
    columnOptions.push({ "bSortable": true, "aTargets": [1] });
    columnOptions.push({ "bSortable": true, "aTargets": [2] });
    columnOptions.push({ "bSortable": true, "aTargets": [3] });
    columnOptions.push({ "bSortable": true, "aTargets": [4] });
    columnOptions.push({ "bSortable": true, "aTargets": [5] });
    columnOptions.push({ "bSortable": true, "aTargets": [6] });
    columnOptions.push({ "bSortable": true, "aTargets": [7] });
    columnOptions.push({ "bSortable": true, "aTargets": [8] });
    columnOptions.push({ "bSortable": true, "aTargets": [9] });
    columnOptions.push({ "bSortable": true, "aTargets": [10] });
    columnOptions.push({ "bSortable": true, "aTargets": [11] });
    columnOptions.push({ "bSortable": true, "aTargets": [12] });
    columnOptions.push({ "bSortable": true, "aTargets": [13] });
    columnOptions.push({ "bSortable": false, "aTargets": [14] });

    var oTable = $('#tblLeadList').dataTable({
        "bProcessing": true,
        "bServerSide": true,
        "bFilter": true,
        "bAutoWidth": false,
        "aaSorting": [[0, "asc"]],
        "aoColumnDefs": columnOptions,
        "bStateSave": true,
        "aLengthMenu": [[10, 20, 50, 100], [10, 20, 50, 100]],
        "sAjaxSource": ReviewListUrl,
        "fnServerData": fnDataTablesPipeline,
        "fnRowCallback": function (nRow, aData, iDisplayIndex) {
            var id = $('td:first', nRow).html();
            //var scoreValue = $('td:eq(4)', nRow).html();

            //var score = '<span class="hide">' + scoreValue + '</span><div class="rating">' +
            //            '<img src="' + RateImageUrl + '"><span style="width: ' + scoreValue + '%;"></span></div>';

            //var status = $('td:eq(7)', nRow).html();
            //var reviewId = $('td:eq(8)', nRow).html();
            var checkbox = '<input class="checkboxes" type="checkbox" value="' + id + '" />';
            //var action = '<a class="btn btn-default" href="' + ReviewDetail + '/' + reviewId + '" title="View"><i class="fa fa-eye"></i></a>';
            //if ((status == "Unverified" || status == "Rejected") && isApproved == "True") {
            //    action += '<a class="btn btn-default approve" href="javascript:void(0);" title="Approve" value="true" id="' + reviewId + '"><i class="fa fa-check"></i></a>';
            //}
            //if ((status == "Unverified" || status == "Approved") && isRejected == "True") {
            //    action += '<a class="btn btn-default approve" title="Reject" value="false" id="' + reviewId + '"><i class="fa fa-remove"></i></a>';
            //}
            //if (isDelete == "True") {
            //    action += '<a class="btn btn-default deleteReview" title="Delete" data-target="#deleteReview" data-toggle="modal" url="' + DeleteReviewUrl + '/' + reviewId + '?returnUrl=' + ReturnUrl + '"><i class="fa fa-trash-o"></i></a>';
            //}
            //$('td:eq(4)', nRow).html(score);
            //$('td:eq(7)', nRow).html('<span>' + status + '</span>');
            //$('td:eq(8)', nRow).html(action);
            $('td:first', nRow).html(checkbox);
            return nRow;
        },
        "fnDrawCallback": function () {
            $('#tblLeadList tbody tr .checkboxes').uniform();
        }
    });

    $('#tblLeadList').find('.group-checkable').change(function () {
        var set = jQuery(this).attr("data-set");
        var checked = jQuery(this).is(":checked");
        jQuery(set).each(function () {
            if (checked) {
                $(this).attr("checked", true);
                //$(this).parents('tr').addClass("active");
            } else {
                $(this).attr("checked", false);
                //$(this).parents('tr').removeClass("active");
            }
        });
        jQuery.uniform.update(set);
    });

    $('#tblLeadList').on('change', 'tbody tr .checkboxes', function () {
        if ($('tbody tr .checkboxes').length == $('tbody tr .checkboxes:checked').length) {
            jQuery('.group-checkable').prop('checked', true);
        }
        else {
            jQuery('.group-checkable').prop('checked', false);
        }
        jQuery.uniform.update(jQuery('.group-checkable'));
        //$(this).parents('tr').toggleClass("active");
    });

});