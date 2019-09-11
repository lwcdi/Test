function getItemsHash(key, items) {
    var hashobj = {};
    if (items && items.length > 0) {
        $.each(items, function (index, value) {
            kname = value[key];
            if ("" != kname)
                hashobj[kname] = value;
        });
    }
    return hashobj;
}

function validateFields(parentID) {
    if ($.fn.validatebox) {
        var box = $("#" + parentID + " .validatebox-text");
        if (box.length) {
            //debugger;
            box.validatebox("validate");
            //box.trigger("focus");
            //box.trigger("blur");
            var valid = $("#" + parentID + " .validatebox-invalid:first").focus();
            return valid.length == 0;
        }
    }
    return true;
};

function getFields(parentID) {
    var obj = {};
    var fields = $("#" + parentID).find("input,select,textarea");
    var check0 = function (node) {//Select、input[Select]、Select[multiple]
        if ($(node).hasClass("easyui-combotree")) {
            var op = $(node).combotree("options");
            if (false === op.editable) {
                if (true === op.multiple)
                    obj[node.id] = $(node).combotree("getValues");
                else
                    obj[node.id] = $(node).combotree("getValue");
            }
            else {
                obj[node.id] = $(node).combotree("getText");
            }
            return true;
        }
        else if ($(node).hasClass("easyui-combobox")) {
            var op = $(node).combobox("options");
            if (false === op.editable) {
                if (true === op.multiple)
                    obj[node.id] = $(node).combobox("getValues").toString();
                else
                    obj[node.id] = $(node).combobox("getValue"); //multiple=true取不到值
            }
            else {
                obj[node.id] = $(node).combobox("getText");
            }
            return true;
        }
        return false;
    };
    var check1 = function (node) {//number、date、datetime、textbox
        if ($(node).hasClass("easyui-textbox")) {
            obj[node.id] = $(node).textbox("getValue"); return true;
        }
        else if ($(node).hasClass("easyui-numberbox")) {
            obj[node.id] = $(node).numberbox("getValue"); return true;
        }
        else if ($(node).hasClass("datetimebox-f")) {//easyui-datetimebox
            obj[node.id] = $(node).datetimebox("getValue"); return true;
        }
        else if ($(node).hasClass("datebox-f")) {//easyui-datebox
            obj[node.id] = $(node).datebox("getValue"); return true;
        }
        return false;
    }
    var check2 = function (node) {//radio,checkbox
        switch (node.type) {
            case "radio":
                obj[node.name] = null;
                var checkeds = $("#" + parentID).find("input:radio:checked[name='" + node.name + "']");
                if (checkeds.length == 1) {
                    obj[node.name] = checkeds.val();
                }
                return true;
            case "checkbox":
                if (true == val || 1 == val || "true" == val) obj[node.id] = $(node).prop("checked");
                return true;
        }
        return false;
    }
    var check3 = function (node) {//select、textarea、input[text]
        obj[node.id || node.name] = $(node).val();
    };
    for (var i = 0; i < fields.length; i++) {
        var node = fields[i];
        if ($(node).hasClass("textbox-text") || $(node).hasClass("textbox-value")) {
            var p = $(node).parent();
            if (0 == p.length || "SPAN" == p[0].tagName) {
                continue;
            }
        }
        if (node.id || node.name) {//easyui Node会把name属性变为自己的类型+name的属性，而把name去掉，id一定要有，用id去索引，但radio要用name属性
            if (!check0(node))
                if (!check1(node))
                    if (!check2(node))
                        check3(node);
        }
    }
    return obj;
}

function setFields(parentID, data) {
    var fields = $("#" + parentID).find("[field]");
    for (var i = 0; i < fields.length; i++) {

        var field = $(fields[i]).attr("field");
        var val = data[field] || " ";
        if ("TD" == fields[i].tagName) {
            $(fields[i]).text(val);
        } else if ("A" == fields[i].tagName) {
            $(fields[i]).attr("href", val);
        } else {
            $(fields[i]).text(val);
        }
    }

    fields = $("#" + parentID).find("input,select,textarea");
    var check0 = function (node, val) {//Select、input[Select]、Select[multiple]
        if ($(node).hasClass("easyui-combotree")) {
            $(node).combotree("setValue", val); return true;
        }
        else if ($(node).hasClass("easyui-combobox")) {
            $(node).combobox("setValues", val.split(',')); return true;
        }
        return false;
    };
    var check1 = function (node, val) {//number、date、datetime
        if ($(node).hasClass("easyui-textbox")) {
            $(node).textbox("setValue", val); return true;
        }
        else if ($(node).hasClass("easyui-numberbox")) {
            $(node).numberbox("setValue", val); return true;
        }
        else if ($(node).hasClass("datetimebox-f")) {//easyui-datetimebox
            $(node).datetimebox("setValue", val); return true;
        }
        else if ($(node).hasClass("datebox-f")) {//easyui-datebox
            $(node).datebox("setValue", val); return true;
        }
        return false;
    }
    var check2 = function (node, val) {//radio,checkbox
        switch (node.type) {
            case "radio":
                val = data[node.name] || "";
                $("#" + parentID).find("input:radio[name='" + node.name + "']").removeAttr("checked");
                $("#" + parentID).find("input:radio[name='" + node.name + "'][value='" + val + "']").prop("checked", true);
                return true;
            case "checkbox":
                if (true == val || 1 == val || "true" == val) $(node).prop("checked", true);
                return true;
        }
        return false;
    }
    var check3 = function (node, val) {//select、textarea、input[text]
        $(node).val(val);
    };

    for (var i = 0; i < fields.length; i++) {
        var node = fields[i];
        if ($(node).hasClass("textbox-text") || $(node).hasClass("textbox-value")) {
            var p = $(node).parent();
            if (0 == p.length || "SPAN" == p[0].tagName) {
                continue;
            }
        }
        var val = data[node.id || node.name] || "";
        if (node.id || node.name) {//easyui Node会把name属性变为自己的类型+name的属性，而把name去掉，id一定要有
            if (!check0(node, val))
                if (!check1(node, val))
                    if (!check2(node, val))
                        check3(node, val);
        }
    }
}

function clearFields(parentID) {
    //$("#" + parentID + " .textbox-text").val("");
    var fields = $("#" + parentID).find("[field]");
    for (var i = 0; i < fields.length; i++) {
        if ("TD" == fields[i].tagName) {
            $(fields[i]).text("");
        } else if ("A" == fields[i].tagName) {
            $(fields[i]).attr("href", "javascript:com.msg.info('该申请未上传附件！');");
        }
    }

    fields = $("#" + parentID).find("input,select,textarea");
    var check0 = function (node) {//Select、input[Select]、Select[multiple]
        if ($(node).hasClass("easyui-combotree")) {
            $(node).combotree("setValue", $(node).val() || ""); return true;
        }
        else if ($(node).hasClass("easyui-combobox")) {
            $(node).combobox("setValue", $(node).val() || ""); return true;
        }
        return false;
    };
    var check1 = function (node) {//number、date、datetime
        if ($(node).hasClass("easyui-textbox")) {
            $(node).textbox("setValue", ""); return true;
        }
        else if ($(node).hasClass("easyui-numberbox")) {
            $(node).numberbox("setValue", ""); return true;
        }
        else if ($(node).hasClass("datetimebox-f")) {//easyui-datetimebox
            $(node).datetimebox("setValue", $(node).val() || ""); return true;
        }
        else if ($(node).hasClass("datebox-f")) {//easyui-datebox
            $(node).datebox("setValue", $(node).val() || ""); return true;
        }
        return false;
    }
    var check2 = function (node) {//radio,checkbox
        switch (node.type) {
            case "radio":
                $(node).removeAttr("checked");
                return true;
            case "checkbox":
                $(node).removeAttr("checked");
                return true;
        }
        return false;
    }
    var check3 = function (node) {//select、textarea、input[text]
        $(node).val("");
    };
    for (var i = 0; i < fields.length; i++) {
        var node = fields[i];
        if ($(node).hasClass("textbox-text") || $(node).hasClass("textbox-value")) {
            var p = $(node).parent();
            if (0 == p.length || "SPAN" == p[0].tagName) {
                continue;
            }
        }
        if (node.id || node.name) {//easyui Node会把name属性变为自己的类型+name的属性，而把name去掉，id一定要有
            if (!check0(node))
                if (!check1(node))
                    if (!check2(node))
                        check3(node);
        }
    }
}

function selectByField(id, value, field, byfield) {
    debugger;
    id = "#" + id;
    var dats = $(id).combobox("getData");
    if (dats.length > 0) {
        for (var i = 0; i < dats.length; i++) {
            if (dats[i][field] == value) {
                $(id).combobox("select", dats[i][byfield]);
                return;
            }
        }
    }
    $(id).combobox("setData", null);
}

function exec(optype, params, callback, action, url) {
    action = action || "Exec";
    var pdata = $.extend({ action: action, type: optype }, params);
    $.ajax({
        url: url || "/Handler/Bus/DataHandler.ashx",
        data: pdata,
        type: "post",
        dataType: "json",
        async: true, //异步执行
        cache: false,
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if ("timeout" == textStatus) {
            }
            else {
            }
            alert("请求发生错误！");
        },
        success: function (data) {
            if (data) {
                if ("Exec" == action) {
                    if (data.msg) {
                        alert(data.msg, function () {
                            if (data.result) {
                                if (callback) callback(data);
                            }
                        });
                    }
                } else {
                    if (callback) callback(data);
                }
            }
            else {
                alert("请求发生错误！");
            }
        }
    });
}

function query(optype, params, callback, action, url) {
    action = action || "Query";
    var pdata = $.extend({ action: action, type: optype }, params);
    $.ajax({
        url: url || "/Handler/Bus/DataHandler.ashx",
        data: pdata,
        type: "post",
        dataType: "json",
        async: true, //异步执行
        cache: false,
        error: function (XMLHttpRequest, textStatus, errorThrown) { if ("timeout" == textStatus) { } else { } },
        success: function (data) {
            debugger;
            if (callback) {
                callback(data);
            }
        }
    });
}

function showGird(gid, title, columns, queryParams, params, url, toolbar) {
    var pdata = $.extend({
        title: title,
        fit: true,
        rownumbers: true,           //显示行号列
        singleSelect: true,         //单选
        striped: true,              //显示斑马线效果
        border: true,               //是否显示外边框
        idField: 'ID',               //唯一列
        pagination: true,            //显示分页工具栏
        selectOnCheck: true,        //如果为true，单击复选框将永远选择行。如果为false，选择行将不选中复选框
        checkOnSelect: true,        //如果为true，当用户点击行的时候该复选框就会被选中或取消选中。如果为false，当用户仅在点击该复选框的时候才会呗选中或取消。
        striped: true,              //显示斑马线效果
        pageList: [10, 15, 20, 30, 50], //初始化页面大小选择列表
        pageSize: 30,              //初始化页面大小
        remoteSort: false,       //从服务器对数据进行排序
        url: url || "/Handler/Bus/DataHandler.ashx", //请求路径
        queryParams: queryParams,
        // multiSort: true,
        toolbar: toolbar || '#divTool',
        columns: columns,
        onDblClickRow: function (rowIndex, rowData) {
            //Edit(rowData.ID);
        },
        onBeforeLoad: function (param) {
        }
    }, params);
    $("#" + gid).datagrid(pdata);
}

function dialog(digid, title, icon, width, height, button1, button2, params, savefunc) {
    var buttons = [];
    if (button1 instanceof Array) {
        buttons = button1;
        params = button2;
    }
    else {
        if (savefunc) {
            buttons.push({
                text: button1 || '保存',
                width: '60',
                handler: savefunc
            });
        }
        buttons.push({
            text: button2 || '关闭',
            width: '60',
            handler: function () { $(digid).dialog('close'); }
        });
    }
    digid = '#' + digid;
    $(digid).show();
    var pdata = $.extend({
        title: title,
        iconCls: icon,
        width: width,
        height: height,
        closed: false,
        cache: false,
        modal: true,
        buttons: buttons
    }, params);
    $(digid).dialog(pdata);
    $(digid).window('center');
}

function createDateRange(id, action, start, end) {
    var jobj = $("#" + id);
    id = "dr" + id;
    var htm = '<div class="ta_date" title="日期选择">' +
        '<span class="date_title" id="' + id + '"> 至 </span>' +
        '<a class="opt_sel" id="input_' + id + '" href="#">' +
        '    <i class="i_orderd"></i>' +
        '</a>' +
        '</div>';
    jobj.empty();
    jobj.append(htm);
    var date = new Date();
    date.setDate(date.getDate() - 1);
    var eDate = com.formatter.date(date);
    date.setMonth(date.getMonth() - 1);
    var sDate = com.formatter.date(date);
    if (start && end) {
        sDate = start;
        eDate = end;
    }
    jobj.attr("startDate", sDate);
    jobj.attr("endDate", eDate);
    var hisDates = new pickerDateRange(id, {
        isTodayValid: true,
        startDate: sDate,
        endDate: eDate,
        defaultText: ' 至 ',
        autoSubmit: true,
        inputTrigger: 'input_' + id,
        theme: 'ta',
        success: function (obj) {
            //'开始时间 : ' + obj.startDate + ' 结束时间 : ' + obj.endDate
            jobj.attr("startDate", obj.startDate);
            jobj.attr("endDate", obj.endDate);
            if (action) action(obj.startDate, obj.endDate);
        }
    });
}

/*div增加resize事件： $('div').resize(fucntion(){ .. })直接使用，该方法会截获$(window).resize,慎用*/
//(function ($, h, c) {
//    var a = $([]),
//        e = $.resize = $.extend($.resize, {}),
//        i,
//        k = "setTimeout",
//        j = "resize",
//        d = j + "-special-event",
//        b = "delay",
//        f = "throttleWindow";
//    e[b] = 250;
//    e[f] = true;
//    $.event.special[j] = {
//        setup: function () {
//            if (!e[f] && this[k]) {
//                return false;
//            }
//            var l = $(this);
//            a = a.add(l);
//            $.data(this, d, {
//                w: l.width(),
//                h: l.height()
//            });
//            if (a.length === 1) {
//                g();
//            }
//        },
//        teardown: function () {
//            if (!e[f] && this[k]) {
//                return false;
//            }
//            var l = $(this);
//            a = a.not(l);
//            l.removeData(d);
//            if (!a.length) {
//                clearTimeout(i);
//            }
//        },
//        add: function (l) {
//            if (!e[f] && this[k]) {
//                return false;
//            }
//            var n;
//            function m(s, o, p) {
//                var q = $(this),
//                    r = $.data(this, d);
//                r.w = o !== c ? o : q.width();
//                r.h = p !== c ? p : q.height();
//                n.apply(this, arguments);
//            }
//            if ($.isFunction(l)) {
//                n = l;
//                return m;
//            } else {
//                n = l.handler;
//                l.handler = m;
//            }
//        }
//    };
//    function g() {
//        i = h[k](function () {
//            a.each(function () {
//                var n = $(this),
//                    m = n.width(),
//                    l = n.height(),
//                    o = $.data(this, d);
//                if (m !== o.w || l !== o.h) {
//                    n.trigger(j, [o.w = m, o.h = l]);
//                }
//            });
//            g();
//        },
//            e[b]);
//    }
//})(jQuery, this);

/*added by xy 2017*/
/*firefox*/
//增加firefox的event全局对象
function __firefox() {
    HTMLElement.prototype.__defineGetter__("runtimeStyle", __element_style);
    window.constructor.prototype.__defineGetter__("event", __window_event);
    Event.prototype.__defineGetter__("srcElement", __event_srcElement);
}
function __element_style() {
    return this.style;
}
function __window_event() {
    return __window_event_constructor();
}
function __event_srcElement() {
    return this.target;
}
function __window_event_constructor() {
    if (document.all) {
        return window.event;
    }
    var _caller = __window_event_constructor.caller;
    while (_caller != null) {
        var _argument = _caller.arguments[0];
        if (_argument) {
            var _temp = _argument.constructor;
            if (_temp.toString().indexOf("Event") != -1) {
                return _argument;
            }
        }
        _caller = _caller.caller;
    }
    return null;
}
if (window.addEventListener) {
    __firefox();
}
/*end firefox*/
/**
* 指定位置显示$.messager.show
* options $.messager.show的options
* param = {left,top,right,bottom}
*/
$.extend($.messager, {
    showBySite: function (options, param) {
        var site = $.extend({
            left: "",
            top: "",
            right: 0,
            bottom: -document.body.scrollTop
					- document.documentElement.scrollTop
        }, param || {});
        var win = $("body > div .messager-body");
        if (win.length <= 0)
            $.messager.show(options);
        win = $("body > div .messager-body");
        win.window("window").css({
            left: site.left,
            top: site.top,
            right: site.right,
            zIndex: $.fn.window.defaults.zIndex++,
            bottom: site.bottom
        });
    }
});
//根据事件显示显示$.messager.show
//<a href="#" class="easyui-linkbutton" onclick="showBySite(event)">help</a>
function showBySite(options, event) {
    event = event || window.event; //target = event.srcElement || event.target//target 就是这个对象;argetValue = target.value;//这个对象的值
    var element = document.elementFromPoint(event.x, event.y); //获取点击对象
    $.messager.showBySite(options, {
        top: $(element).position().top + $(element).height(), //将$.messager.show的top设置为点击对象之下
        left: $(element).position().left, //将$.messager.show的left设置为与点击对象对齐
        bottom: ""
    });
}
//显示提示信息
//pos:0右下角，1缺省：跟随鼠标位置显示在正中间，2顶部正中间，3跟随鼠标位置
function show(msgString, title, pos, showType, timeout, offsety) {
    if (parseInt(title) == title) {
        offsety = parseInt(title);
        title = undefined;
    }
    title = title || "提示";
    if (undefined == pos || null == pos) pos = 1;
    showType = showType || "slide"; //可用值有：null,slide,fade,show
    timeout = timeout || 1500; //如果定义为0，消息窗体将不会自动关闭，除非用户关闭他。如果定义成非0的树，消息窗体将在超时后自动关闭。默认：4秒。
    offsety = offsety || 0;
    var style = undefined;
    if (pos != 3) {
        switch (pos) {
            case 1:
                style = { right: '', top: window.event.clientY + offsety, bottom: '' };
                break;
            case 2:
                style = {
                    right: '', top: document.body.scrollTop + document.documentElement.scrollTop + offsety, bottom: ''
                };
                break;
        }
        $.messager.show({ title: title, msg: msgString, timeout: timeout, showType: showType, style: style });
    } else {
        showBySite({ title: title, msg: msgString, timeout: timeout, showType: showType });
    }
}
//显示确认对话框[阻塞]
function confirm(msg, callback) {
    callback = callback || function (r) { };
    //不能给$.messager.defaults直接赋值，会清掉其他的属性
    $.messager.defaults = $.messager.defaults || {};
    $.messager.defaults.ok = "是";
    $.messager.defaults.cancel = "否";
    $.messager.confirm('确认', msg, function (r) {
        callback(r);
    });
}
//显示对话框[阻塞]
function alert(msg, callback) {
    callback = callback || function () { };
    //不能给$.messager.defaults直接赋值，会清掉其他的属性
    $.messager.defaults = $.messager.defaults || {};
    $.messager.defaults.ok = "确定";
    //icon：error,question,info,warning
    $.messager.alert('提示', msg, 'warning', function () {
        callback();
    });
}
//增加trim函数
if (!String.prototype.trim) {
    String.prototype.trim = function () {
        return this.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, '');
    };
}
//获取地址栏参数
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}

//js对象复制
function clone(obj, simple) {
    var prefix = "";
    if (true !== simple && false !== simple) {
        prefix = simple;
        simple = false;
    }
    if (simple) return JSON.parse(JSON.stringify(obj)); //只能返回数据部分，对象函数无法复制
    var newObj = {};
    if (obj instanceof Array) {
        newObj = [];
    }
    for (var key in obj) {
        var val = obj[key];
        if (prefix && key.startWith(prefix)) {
            key = key.substr(prefix.length);
        }
        //newObj[key] = typeof val === 'object' ? arguments.callee(val) : val; //arguments.callee 在哪一个函数中运行，它就代表哪个函数, 一般用在匿名函数中。  
        newObj[key] = typeof val === 'object' ? clone(val) : val;
    }
    return newObj;
}

//格式化字符串中的小数位数
function fixNumber(value, num) {
    var re = /^[0-9]+.?[0-9]*$/; //判断字符串是否为数字 //判断正整数 /^[1-9]+[0-9]*]*$/
    if (re.test(value)) {
        value = new Number(value).toFixed(num)
    }
    return value;
}
/*added by xy 2017*/

function addFile() {
    var htm = '<input id="ATTACHMENT_FILE" name="ATTACHMENT_FILE" type="file" />';
    $("#ATTACHMENT_FILE_RESET").remove();
    $("#ATTACHMENT_FILE_TIPS").remove();
    $("#ATTACHMENT_FILE").remove();
    $("#ATTACHMENT_FILE_TD").append(htm);
    $("#ATTACHMENT").hide();
    $("#_FILE").val("");
    $("#ATTACHMENT_FILE").change(function () {
        debugger;
        postFile("/Handler/Sys/FileHandler.ashx", "QnVzaW5lc3M=");
    });
    //$("#d_save").show();
}

function postFile(url, tag) {
    debugger;
    //$("#d_save").hide();
    var htm = '<span id="ATTACHMENT_FILE_TIPS">附件提交中，请稍后！注：附件最大为10兆字节。<br>如果长时间没有响应，请点击[撤销]后重试！<br></span>';
    $("#ATTACHMENT_FILE_TIPS").remove();
    $("#ATTACHMENT_FILE_TD").append(htm);

    htm = '<a id="ATTACHMENT_FILE_RESET" href="javascript:void(0);" plain="true" title="撤销为可上传附件状态" style="float: right;">撤销</a>';
    $("#ATTACHMENT_FILE_RESET").remove();
    $("#ATTACHMENT_FILE_TD").append(htm);
    $("#ATTACHMENT_FILE_RESET").click(function () {
        com.msg.confirm("撤销将丢弃已经上传的附件，确实要撤销？", function (r) {
            if (r) {
                addFile();
            }
        });
    });

    //ex:<form id="filePost" action="/Home/Upload" method="post" enctype="multipart/form-data">
    var form = $("<form>");
    form.attr('style', 'display:none');
    form.attr('enctype', 'multipart/form-data');
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', url);

    $('body').append(form);
    var html = "<input type='hidden' name='tag' value='" + tag + "'></input>";
    form.html(html);
    form.append($("#ATTACHMENT_FILE")[0]);

    var options = {
        beforeSubmit: function showRequest(formData, jqForm, options) {
            //发送前
            return true;
        },
        error: function showError(data) {
            if (0 < $("#d_save:visible").length) return;
            //发生错误
            if (data && data.readyState == 4 && data.status == 404) {
                com.msg.error("附件被服务器拒绝，请重新选择文件上传！注：附件最大为10兆字节。");
            }
            else {
                com.msg.error("附件上传发生错误，请重新选择文件上传！注：附件最大为10兆字节。");
            }
            form.remove();
            addFile();
        },
        success: function showResponse(responseText, statusText) {
            if (0 < $("#d_save:visible").length) return;
            if ("success" == statusText && responseText) {
                var data = eval("(" + responseText + ")");
                if (data.bRet) {
                    com.msg.info(data.sMsg);
                    $("#ATTACHMENT_FILE_TIPS").remove();
                    $("#ATTACHMENT").attr("href", data.url);
                    $("#ATTACHMENT").text("点击查看");
                    $("#ATTACHMENT").show();
                    $("#_FILE").val(data.url);
                    form.remove();
                    $("#d_save").show();
                    return;
                }
            }
            com.msg.error("附件上传发生错误，请重新选择文件上传！注：附件最大为10兆字节。");
            form.remove();
            addFile();
        }
    };

    form.ajaxSubmit(options);
};

/**
 * js扩展
 * 仅依赖浏览器js、jquery，引用直接放在jquery引用后
 */

/**
 * 字符串格式化
 * 两种调用方式
 * var template1="我是{0}，今年{1}了";
 * var template2="我是{name}，今年{age}了";
 * var result1=template1.format("xy",22);
 * var result2=template1.format({name:"xy",age:22});
 * 两个结果都是"我是xy，今年22了"
 */
String.prototype.format = String.prototype.format || function (args) {
    if (arguments.length > 0) {
        var result = this;
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                var reg = new RegExp("({" + key + "})", "g");
                //result = result.replace(reg, args[key]);
                var val = (undefined === args[key] || null === args[key]) ? "" : args[key];
                result = result.replace(reg, val);
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                //if (arguments[i] == undefined) {
                //    return "";
                //}
                //else {
                var reg = new RegExp("({[" + i + "]})", "g");
                //result = result.replace(reg, arguments[i]);
                var val = (undefined === arguments[i] || null === arguments[i]) ? "" : arguments[i];
                result = result.replace(reg, val);
                //}
            }
        }
        return result;
    }
    else {
        return this;
    }
};

//填充编辑页面的字段 author:liwu 2017-12-22
function fillField(dataGridRow) {
    if (dataGridRow == null) return;
    for (var item in dataGridRow) {
        if (item == null) continue;
        var ele = top.$("#" + item);
        var eleAttributes = ""
        //大于0表示有容器属性
        if (ele.length < 1) continue;
        eleAttributes = ele[0];

        //文本
        if ((eleAttributes.type == "text" || eleAttributes.type == "textarea")
            && eleAttributes.className.indexOf("easyui-datebox") < 0
            && eleAttributes.className.indexOf("easyui-datetimebox") < 0
            && eleAttributes.className.indexOf("easyui-numberbox") < 0
            )
            ele.val(dataGridRow[item]);
            //日期控件
        else if (eleAttributes.type == "text" && eleAttributes.className.indexOf("easyui-datebox") > 0)
            ele.datebox("setValue", com.formatter.date(dataGridRow[item]));
            //日期时间控件
        else if (eleAttributes.type == "text" && eleAttributes.className.indexOf("easyui-datetimebox") > 0)
            ele.datetimebox("setValue", com.formatter.date(dataGridRow[item]));
            //下拉框控件
        else if (eleAttributes.type == "select-one" && eleAttributes.className.indexOf("combobox") > 0) {
            var dataArray = dataGridRow[item] && dataGridRow[item].toString().split(',');
            if (dataArray) ele.combobox("setValues", dataArray);

        }
            //下拉树控件
        else if (eleAttributes.type == "select-one" && eleAttributes.className.indexOf("combotree") > 0) {
            ele.combotree("setValue", dataGridRow[item]);
        }
            //隐藏字段
        else if (eleAttributes.type == "hidden") {
            ele.val(dataGridRow[item]);
        }
            //数字控件
        else if (eleAttributes.type == "text" && eleAttributes.className.indexOf("easyui-numberbox") > 0) {
            ele.numberbox("setValue", dataGridRow[item]);
        }
    }
}
//设置全部输入框只读 
//参数id：html顶级元素的ID
//author:liwu 2018-01-11
function setInputReadOnly(id) {
    var input = top.$("#" + id);
    input.find("input,textarea,select").attr('readonly', true);
    input.find("input,textarea,select").attr('disabled', true);

    input.find("input,select").each(function () {
        if (top.$(this)[0].type == "select-one") {
            top.$(this).combobox({
                onShowPanel: function (data) {
                    top.$(this).combobox("hidePanel");
                }
            });
        }
        if (top.$(this)[0].type == "text" && top.$(this)[0].className.indexOf("easyui-datebox") >= 0) {
            debugger;
            top.$(this).datebox({
                onShowPanel: function (data) {
                    top.$(this).datebox("hidePanel");
                },
                editable: false
            });
        }
        if (top.$(this)[0].type == "text" && top.$(this)[0].className.indexOf("easyui-datetimebox") >= 0) {
            top.$(this).datetimebox({
                onShowPanel: function (data) {
                    top.$(this).datetimebox("hidePanel");
                },
                editable: false
            });
        }
    });
}