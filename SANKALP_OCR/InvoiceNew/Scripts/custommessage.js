var customMessage = {
    container: null,
    languageID:1,
    messageList: [],
    width: "40%",
    height: "10%",
    format: {
        color: {
            1: {color: "blue",background:"white" },
            2: {color: "yellow", background: "#555" },
            3: {color: "red", background: "transparent" },
            4: {color: "white", background: "white" }
        },
        font: {
            fontsize: 11,
            bold: false
        },
        border:"none"
    },
    createMessageList: function (languageid) {
        if (languageid == undefined || languageid == null || languageid == "" || parseInt(languageid, 10) < 1 || parseInt(languageid, 10) > 3)
            languageid = customMessage.languageID;
        customMessage.messageList = [];
        //$.ajaxSetup({ async: false,dataType:"JSON" });
        $.ajax(
            {
                type: "POST",
                async:false,
                url:"/Home/getCustomeMessageList",
                data: { lang: languageid },
                datatype:"JSON",
                success: function (msglist) {
                    //alert(JSON.stringify(msglist));
                    customMessage.messageList = msglist;
                    //alert(JSON.stringify(customMessage.messageList));
                },
                error: function (xhr, st, et) { alert(xhr.responseText);}
                
        });
    },
    show: function (message, options) {
        options = customMessage.setOptions(options);
        customMessage.remove();
        var $div = $("<div class='__custommessageContainer'></div>").appendTo('body');

        var container = $div;
        if ($("span.__custommessage", $(container)).length > 0) $("span.__custommessage", $(container)).remove();
        $(container).append("<span class='__custommessage'>"+message+"</span>");
        customMessage.applyOptions(container, options, true);
        customMessage.addCMCloseButton(options);
    },
    addCMCloseButton: function (options) {
        var $i = $("<i class='__custommessageCloseButton fa fa-times-circle' onclick='customMessage.remove();'></i>").appendTo('body');
        //var $i = $("<img src='Content/img/close.png' class='__custommessageCloseButton' onclick='customMessage.remove();'/>").appendTo('body');
        var l = $("div.__custommessageContainer")[0].offsetLeft;
        var t = $("div.__custommessageContainer")[0].offsetTop;
        var h = parseInt($("i.__custommessageCloseButton").css("font-size"),10);
        var w = $("div.__custommessageContainer")[0].offsetWidth;
        $("i.__custommessageCloseButton").css({
            "top": (t - h/2) + "px",
            "left": (l +w-h/2) + "px"
        });
    },
    remove: function () {
        $("span.__custommessage, div.__custommessageContainer,i.__custommessageCloseButton").animate({ opacity: 0 },
                                { duration: 2000, complete: function () { $(this).remove(); } });
    },
    showMessage: function (container, message, options) {
        options = customMessage.setOptions(options);
        if (typeof (container) == "string") container = $("#" + container);
        if ($("span.__custommessage", $(container)).length > 0) $("span.__custommessage", $(container)).remove();
        $(container).append("<span class='__custommessage'>" + message + "</span>");
        customMessage.applyOptions(container, options,false);
    },
    applyOptions: function (container, options, isNewDiv) {
        var divspan = isNewDiv ? $("div.__custommessageContainer")[0] : $("span.__custommessage")[0];
        $(divspan).css({
            "color": options.color,
            "background": options.background,
            "font-size": options.font.fontsize,
            "border": options.border,
            "padding": options.padding.paddingTop + " " + options.padding.paddingRight + " " +
                        options.padding.paddingBottom + " " + options.padding.paddingLeft,
            "-webkit-box-shadow": options.shadow, "-moz-box-shadow": options.shadow, "box-shadow": options.shadow,
            "-moz-border-radius": options.borderRadius,
            "-webkit-border-radius": options.borderRadius,
            "border-radius": options.borderRadius
        });
        if (options.font.bold) $(divspan).css({ "font-weight": "bold" });
        if (isNewDiv) $(divspan).css({ "width": options.width, "height": options.height });
        if (options.hideafter > 0) {
            setTimeout(customMessage.remove, options.hideafter);
        }
        customMessage.cmsetPos(container, options,isNewDiv);
    },
    
    setOptions:function(options){
        if (options == undefined) options = {width:"40%",height:"10%", x: -1, y: -1, msgtype: 3, color: "red", background: "transparent",border:"none",hideafter:0,borderRadius:"none",shadow:"none" };
        if (!options.hasOwnProperty("msgtype") || options.msgtype < 1 || options.msgtype > 4) options.msgtype = 3;
        if (!options.hasOwnProperty("color")) options.color = customMessage.format.color[options.msgtype].color;
        if (!options.hasOwnProperty("background")) options.background = customMessage.format.color[options.msgtype].background;
        if (!options.hasOwnProperty("font")) options.font = { fontsize: 11, bold: false };
        if (!options.font.hasOwnProperty("fontsize")) options.font.fontsize = customMessage.format.font.fontsize;
        if (!options.font.hasOwnProperty("bold")) options.font.bold = customMessage.format.font.bold;
        if (!options.hasOwnProperty("border")) options.border = customMessage.format.border;
        if (!options.hasOwnProperty("x")) options.x = -1;
        if (!options.hasOwnProperty("y")) options.y = -1;
        if (!options.hasOwnProperty("padding")) options.padding = {
            paddingTop: "0px", paddingBottom: "0px",
            paddingRight: "0px", paddingLeft: "0px"
        };
        if (!options.padding.hasOwnProperty("paddingTop")) options.padding.paddingTop = "0px";
        if (!options.padding.hasOwnProperty("paddingBottom")) options.padding.paddingBottom = "0px";
        if (!options.padding.hasOwnProperty("paddingLeft")) options.padding.paddingLeft = "0px";
        if (!options.padding.hasOwnProperty("paddingRight")) options.padding.paddingRight = "0px";

        if (!options.hasOwnProperty("width")) options.width = customMessage.width;
        if (!options.hasOwnProperty("height")) options.height = customMessage.height;
        if (!options.hasOwnProperty("hideafter")) options.hideafter = 0;
        if (!options.hasOwnProperty("borderRadius")) options.borderRadius = "none";
        if (!options.hasOwnProperty("shadow")) options.shadow = "none";
        return options;
    },
    cmsetPos: function (container, options, isNewDiv) {
        if (isNewDiv != true) isNewDiv = false;
        if (isNewDiv) {
            $("div.__custommessageContainer").css({ "left": options.x, "top": options.y });
            return;
        }
            
        if (options == undefined) options = { x: -1, y: -1 };
        if (typeof (container) == "string") container = $("#" + container);
        var sw =$("span.__custommessage")[0].offsetWidth;
        var sh = $("span.__custommessage")[0].offsetHeight;
        if (!(options.x.toString().indexOf("%") >= 0 || options.x.toString().indexOf("px") >= 0)) {
            x = parseInt(options.x, 10);
            if (isNaN(x)) x = -1;
            if (!options.hasOwnProperty("x") || x < 0)
                options.x = $(container)[0].offsetWidth / 2 - (sw / 2);
        }
        if (!(options.y.toString().indexOf("%") >= 0 || options.y.toString().indexOf("px") >= 0)) {
            y = parseInt(options.y, 10);
            if (isNaN(y)) y = -1;

            if (!options.hasOwnProperty("y") || y < 0)
                options.y = $(container)[0].offsetHeight - (sh + 5);
        }
        pxpery = options.y.toString().indexOf("px") < 0 || options.y.toString().indexOf("%") < 0 ? "px" : "";
        pxperx = options.x.toString().indexOf("px") < 0 || options.x.toString().indexOf("%") < 0 ? "px" : "";
        $("span.__custommessage").css({ "left": options.x+pxperx, "top": options.y+pxpery });
    }
};