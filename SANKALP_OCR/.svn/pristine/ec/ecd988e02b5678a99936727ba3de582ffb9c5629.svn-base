var mCurrentUserRecord = {
    UserID: 0,
    LoginName: '',
    isauth: 0,
    ApproverOrInitiator: 'I',
    UserName: 'Guest',
    deptid: 0,
    designationid: 0,
    deptname: '',
    designation: ' '
};
function setdivHeights() {
    return;
    $("#divDashboard").css({"height":"85%"});
    $("#divWA").css({"height":"80%"});
    $("#divFooter").css({"top":"95%"});
}
function setwaheight(setToInnerElement) {
    //$("#divWA").css("display", "blo");
    var h = Math.max($("#" + setToInnerElement).actual("height"), $("#" + setToInnerElement).outerHeight(true), $("#" + setToInnerElement)[0].scrollHeight);
    h += $("#" + setToInnerElement)[0].offsetTop + 5;
    $("#divWA").css("height", h + "px");
    $("#divDashboard").css("height", (h + $("#divFooter").outerHeight(true)) + "px");
    $("#divFooter").css("top", ($("#divWA")[0].offsetTop + $("#divWA").actual("height")) + "px");
}
function loadPageInWA(pg, options, isTransparent, asObject, mimetype) {
    closeall();
    if (mimetype == undefined || mimetype == null) mimetype = "application/pdf";
    if (asObject == undefined || asObject == null) asObject = false;
    if (isTransparent == undefined || isTransparent == null) isTransparent = false;
    $("#divWA").html("");
    
    if (asObject) {
        var param = "";
        $.each(options, function (k, v) {
            param += (param == "" ? "?" : "&") + k + '=' + v;
        });
        var h = $("#divWA").actual("height");
        var url = '<object data="' + pg + param + '" type="' + mimetype + '" style="width:100%;height:100%;min-height:100%;">' +
		'alt : <a href="' + pg + param + '"></a>' + '</object>';
        $("#divWA").html(url);
    }
    else {
        if(mimetype=="html")
            $("#divWA").html(pg);
        else
            $("#divWA").load(pg, options);
    }
    $("#divWA").css({ "display": "block", "background-color": (isTransparent ? "transparent" : "#eee") });
}
function setdate(tb, dt) {
    if (dt == undefined || dt == null || dt == "") dt = new Date();
    $("#" + tb).datepicker("setDate", dt);
    $("#" + tb + '.ui-datepicker-current-day').click();
}
function closeall() {
    $(".ra-dialog-window,#divWA,.ra-close-button").css("display", "none");
    try {
        customMessage.remove();
    } catch (e) {}
}
function setCloseImage(divid, imgid) {
    var l = $("#" + divid)[0].offsetLeft + $("#" + divid).actual("width") - $("#" + imgid).actual("width") / 2;
    var t = $("#" + divid)[0].offsetTop;
    t=t-$("#" + imgid).actual("height") / 4;
    $("#" + imgid).css({
        "top": t + "px",
        "left": l + "px",
        "z-index": parseInt($("#" + divid).css("z-index"), 10) + 1
    });
}
$.getUrl = function (path) {
    return '@Url.Content("~")' + path;
};
function isEmpty(value) {
    return value==undefined || value==null || $.trim((""+value))=="";
}

function getBrowser() {
    if (navigator.userAgent.indexOf("Chrome") != -1) return 'chrome';
    if (navigator.userAgent.indexOf("Opera") != -1) return 'opera';
    if (navigator.userAgent.indexOf("Firefox") != -1) return 'firefox';
    if ((navigator.userAgent.indexOf("MSIE") != -1) || (!!document.documentMode == true)) return "ie";
    return "uk";
}