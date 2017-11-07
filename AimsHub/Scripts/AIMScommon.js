$(document).ready(function () {
    $.fn.bootstrapBtn = $.fn.button.noConflict(); //This makes the X button show in the jquery popups, some conflict with jquery and bootstrap

    $.sessionTimeout({
        message: 'Your session is about to expire.',
        countdownMessage: 'Expiring in {timer}',
        countdownSmart: true,
        countdownBar: true,
        keepAliveUrl: '/Login/ResetSession',
        logoutUrl: '/Login/Logout',
        redirUrl: '/Login/Timeout',
        warnAfter: 1200000, //20 minutes
        redirAfter: 1500000, //25 minutes
        //warnAfter: 4000,
        //redirAfter: 10000, 
        keepAlive: false,
        ignoreUserActivity: true,
        ajaxType: 'POST'
    });

    $('#navPatientPopup').dialog({
        autoOpen: false,
        resizable: false,
        draggable: false,
        height: 'auto',
        width: 'auto',
        modal: false,
        dialogClass: "noTitleBar",
        position: { my: "top", at: "bottom", of: $('#patientmodule') }
    });
    $('#navBillingPopup').dialog({
        autoOpen: false,
        resizable: false,
        draggable: false,
        height: 'auto',
        width: 'auto',
        modal: false,
        dialogClass: "noTitleBar",
        position: { my: "top", at: "bottom", of: $('#billingmodule') }
    });
    $('#navReportPopup').dialog({
        autoOpen: false,
        resizable: false,
        draggable: false,
        height: 'auto',
        width: 'auto',
        modal: false,
        dialogClass: "noTitleBar",
        position: { my: "top", at: "bottom", of: $('#reportmodule') }
    });
    //$('#navSchedulePopup').dialog({
    //    autoOpen: false,
    //    resizable: false,
    //    draggable: false,
    //    height: 'auto',
    //    width: 'auto',
    //    modal: false,
    //    dialogClass: "noTitleBar",
    //    position: { my: "top", at: "bottom", of: $('#schedulemodule') }
    //});
    $('[name=barButtonDrop]').hover(function () {
        closeNavPopups();
        var popupName = $(this).attr('data-popup');
        var popup = $('#' + popupName);
        popup.dialog('open');
    });
    $('[name=barButton').hover(function () {
        closeNavPopups();
    });
    $('#AIMSLogoNav').hover(function () {
        closeNavPopups();
    });
    $('[name=navPopup]').mouseleave(function () {
        $(this).dialog('close');
    });
    $('[name=navPopup] li').click(function () {
        var href = $(this).find('a').attr('href');
        window.location = href;
    });
    //This handles sorting logic for pages that implement it
    $('[name=SortLink]').click(function (e) {
        e.preventDefault();
        var lastSort = $('#SortColumn').val();
        var newDir;
        if ($(this).attr('data-name') === lastSort) {
            if ($('#SortDirection').val().indexOf('A') !== -1) {
                newDir = "Descending";
            }
            else {
                newDir = "Ascending";
            }
        }
        else {
            newDir = "Ascending";
        }
        $('#SortColumn').val($(this).attr('data-name'));
        $('#SortDirection').val(newDir);
        triggerSubmit();
    });
    $(function () {
        $('.date-picker').datepicker();
    });
    $(function () {
        $('#tabs').tabs();
    });
    $(function () {
        $('.date-pickerDOB').datepicker({
            yearRange: "-100:+0",
            changeYear: true
        });
    });
});
function closeNavPopups() {
    $('[name=navPopup]').each(function () {
        if ($(this).dialog('isOpen')) {
            $(this).dialog('close')
        }
    });
}
function TableToArray(table)
{
    var ret = [];
    var headerRow = [];
    var i = 0;
    $(table).find('tr').each(function () {
        var row = [];
        if (i == 0)
        {
            $(this).find('th').each(function () {
                row.push($(this).text());
            });
        }
        else {
            $(this).find('td').each(function () {
                row.push($(this).text());
            });
        }
        ret.push(row);
        i++;
    });
    return ret;
}