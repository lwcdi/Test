var hostName = "<%=Request.Url.Host%>";
var portName = "<%=Request.Url.Port%>";
var com = com || {};
//定义常量 author:liwu 
com.constants = (function () {
    //定义常量值
    var constantValue = {
        DIC_URL: "/Bas/Dic/GetDicCodeData",
        DIC_URL_SEARCH: "/Bas/Dic/GetDicCodeDataForSearch",
        COMBOBOX_DEFAULT_OPTION: {//下拉框默认配置
            width: 173,
            editable: false,
            panelHeight: "auto",
            valueField: "CODE",
            textField: "TITLE",
            multiple:false
        }
    }
    var temp = {};
    temp.getConstant = function (name) {
        return constantValue[name];
    };
    return temp;
});
//语言包
com.lang = com.lang || {};
com.lang.write = function (langName) {
    $.cookie("langName", langName, { path: "/" });
};
com.lang.read = function () {
    var lname = $.cookie("langName", { path: "/" });
    if (!lname) {
        lname = "en"; //默认使用英文
    }
    $.get("/content/lang/lang.xml", function (xml) {
        var langPack = $(xml).find("lang");
        $(langPack).each(function () {
            com.lang[$(this).attr("code")] = $(this).attr(lname);
        });
    });
};

//ajax
com.ajax = function (options) {
    var opts = {
        url: "",
        type: "POST",
        dataType: "json",
        data: {}
    };
    options = $.extend(opts, options || {});
    $.ajax(options);
};

//判断是否NULL
com.isNull = function (value) {
    if (value == null)
        return "";
    else
        return value;
}

//格式化
com.formatter = {
    icon: function (value) {
        return "<span iconCls='" + value + "' class='icon " + value + "' >&nbsp;</span>";
    },
    check: function (value) {
        return "<img src='/content/img/" + (value >= 1 ? "checkmark.gif" : "checknomark.gif") + "'/>";
    },
    date: function (value) {
        try {
            return com.formatter.formatDate(value, "yyyy-MM-dd");
        } catch (e) {
            return undefined;
        }
    },
    dateTime: function (value) {
        try {
            return com.formatter.formatDate(value, "yyyy-MM-dd hh:mm:ss");
        } catch (e) {
            return undefined;
        }
    },
    dateCustom: function (value, e) {

        try {
            return com.formatter.formatDate(value, e);
        } catch (e) {
            return undefined;
        }
    },
    setHours: function (date, hours) {
        if (hours == undefined || hours == '') {
            hours = 1;
        }
        var date = new Date(date);
        date.setHours(date.getHours() + hours);
        return com.formatter.formatDate(date, "yyyy-MM-dd hh:00:00");
    },
    formatDate: function (v, format) {
        if (!v) return "";
        var d = v;
        if (typeof v === "string") {
            if (v.indexOf("/Date(") > -1)
                d = new Date(parseInt(v.replace("/Date(", "").replace(")/", ""), 10));
            else
                d = new Date(Date.parse(v.replace(/-/g, "/").replace("T", " ").split(".")[0]));//.split(".")[0] 用来处理出现毫秒的情况，截取掉.xxx，否则会出错
        }
        var o = {
            "M+": d.getMonth() + 1,  //month
            "d+": d.getDate(),       //day
            "h+": d.getHours(),      //hour
            "m+": d.getMinutes(),    //minute
            "s+": d.getSeconds(),    //second
            "q+": Math.floor((d.getMonth() + 3) / 3),  //quarter
            "S": d.getMilliseconds() //millisecond
        };
        if (/(y+)/.test(format)) {
            format = format.replace(RegExp.$1, (d.getFullYear() + "").substr(4 - RegExp.$1.length));
        }
        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    },

    formatCellTooltip: function (value) {
        return "<span title='" + value + "'>" + value + "</span>";
    }
};


//消息方法
com.msg = com.msg || {};
com.msg.alert = function (msg) {
    top.$.messager.alert("系统提示", msg);
};
com.msg.info = function (msg, fn) {
    top.$.messager.alert("系统提示", msg, "info", fn);
};
com.msg.error = function (msg) {
    top.$.messager.alert("系统提示", msg, "error");
};
com.msg.question = function (msg) {
    top.$.messager.alert("系统提示", msg, "question");
};
com.msg.warning = function (msg) {
    top.$.messager.alert("系统警告", msg, "warning");
};
com.msg.confirm = function (msg, fn) {
    top.$.messager.confirm("确认", msg, fn);
};

//表单方法
com.form = com.form || {};
com.form.serialize = function (formId, noTop) {
    var form;
    if (noTop && noTop == true) {
        form = $(formId);
    }
    else {
        form = top.$(formId);
    }
    if (form.length > 1) {
        return false;
    }
    var arr = form.serializeArray();
    var obj = new Object;
    $.each(arr, function (k, v) {
        if (obj.hasOwnProperty(v.name)) {
            //如果存在多name的情况，则把value用逗号隔开拼接
            obj[v.name] += "," + v.value;
        }
        else {
            obj[v.name] = v.value;
        }
    });
    return obj;
};
com.form.validate = function (istop) {
    if (istop) {
        if (top.$.fn.validatebox) {
            var box = top.$(".validatebox-text");
            if (box.length) {
                box.validatebox("validate");
                box.trigger("focus");
                box.trigger("blur");
                var valid = top.$(".validatebox-invalid:first").focus();
                return valid.length == 0;
            }
        }
    }
    else {
        if ($.fn.validatebox) {
            var box = $(".validatebox-text");
            if (box.length) {
                box.validatebox("validate");
                box.trigger("focus");
                box.trigger("blur");
                var valid = $(".validatebox-invalid:first").focus();
                return valid.length == 0;
            }
        }
    }

    return true;
};

//js解决浮点型计算bug
com.math = com.math || {};
com.math.add = function (arg1, arg2) {
    var r1, r2, m, c;
    try {
        r1 = arg1.toString().split(".")[1].length;
    }
    catch (e) {
        r1 = 0;
    }
    try {
        r2 = arg2.toString().split(".")[1].length;
    }
    catch (e) {
        r2 = 0;
    }
    c = Math.abs(r1 - r2);
    m = Math.pow(10, Math.max(r1, r2));
    if (c > 0) {
        var cm = Math.pow(10, c);
        if (r1 > r2) {
            arg1 = Number(arg1.toString().replace(".", ""));
            arg2 = Number(arg2.toString().replace(".", "")) * cm;
        } else {
            arg1 = Number(arg1.toString().replace(".", "")) * cm;
            arg2 = Number(arg2.toString().replace(".", ""));
        }
    } else {
        arg1 = Number(arg1.toString().replace(".", ""));
        arg2 = Number(arg2.toString().replace(".", ""));
    }
    return (arg1 + arg2) / m;
};
com.math.sub = function (arg1, arg2) {
    return com.math.add(arg1, -arg2);
};

com.ui = com.ui || {};
com.ui.combobox = com.ui.combobox || {};
com.ui.combobox.addShowPanel = function (onTop) {
    var jq = top.$;
    if (!onTop || onTop == false) {
        jq = $;
    }
    jq(".combo").click(function () {
        jq(this).prev().combobox("showPanel");
    });
};
com.ui.combobox.multipleOptions = {
    multiple: true,
    formatter: function (row) {
        var opts = top.$(this).combobox('options');
        return '<input type="checkbox" class="combobox-checkbox">' + row[opts.textField]
    },
    onShowPanel: function () {
        var opts = top.$(this).combobox("options");
        target = this;
        var values = top.$(target).combobox("getValues");
        top.$.map(values, function (value) {
            var children = top.$(target).combobox("panel").children();
            top.$.each(children, function (index, obj) {
                if (value == obj.getAttribute("value") && obj.children && obj.children.length > 0) {
                    obj.children[0].checked = true;
                }
            });
        });
    },
    onLoadSuccess: function () {
        var opts = top.$(this).combobox("options");
        var target = this;
        var values = top.$(target).combobox("getValues");
        top.$.map(values, function (value) {
            var children = top.$(target).combobox("panel").children();
            top.$.each(children, function (index, obj) {
                if (value == obj.getAttribute("value") && obj.children && obj.children.length > 0) {
                    obj.children[0].checked = true;
                }
            });
        });
    },
    onSelect: function (row) {
        var opts = top.$(this).combobox("options");
        var objCom = null;
        var children = top.$(this).combobox("panel").children();
        top.$.each(children, function (index, obj) {
            if (row[opts.valueField] == obj.getAttribute("value")) {
                objCom = obj;
            }
        });
        if (objCom != null && objCom.children && objCom.children.length > 0) {
            objCom.children[0].checked = true;
        }
    },
    onUnselect: function (row) {
        var opts = top.$(this).combobox("options");
        var objCom = null;
        var children = top.$(this).combobox("panel").children();
        top.$.each(children, function (index, obj) {
            if (row[opts.valueField] == obj.getAttribute("value")) {
                objCom = obj;
            }
        });
        if (objCom != null && objCom.children && objCom.children.length > 0) {
            objCom.children[0].checked = false;
        }
    }
};
//添加自动清除不存在值
com.ui.combobox.addAutoClear = function (id, afterClear) {
    var cbb = top.$(id);
    var pnl = cbb.combobox("panel");
    pnl.panel({
        onClose: function () {
            var opts = cbb.combobox("options");
            if (opts) {
                var valueField = opts.valueField;
                var val = cbb.combobox("getValue");  //当前combobox的值
                var allData = cbb.combobox("getData");   //获取combobox所有数据
                var exist = false;      //是否存在
                for (var i = 0; i < allData.length; i++) {
                    if (val == allData[i][valueField]) {
                        exist = true;
                        break;
                    }
                }
                if (exist == false) {  //不存在时，清空下拉框的文本框值
                    cbb.combobox("setValue", "");
                    cbb.combobox("reload");

                    if (afterClear) {
                        afterClear();
                    }
                }
            }
        }
    });
};
//下拉框默认选项
com.ui.combobox.defaultOptions = function () {
    return com.constants().getConstant("COMBOBOX_DEFAULT_OPTION");
};
//添加多选框 author:liwu  2018-01-02
com.ui.combobox.AddCheckbox = function (comboboxID, option, isTop) {
    var loadSuccess = option.onLoadSuccess;
    var select = option.onSelect;
    var unselect = option.onUnselect;
    delete option.onLoadSuccess;
    delete option.onSelect;
    delete option.onUnselect;


    if (isTop != false) isTop = true;
    var combobox = isTop == true ? top.$("#" + comboboxID) : $("#" + comboboxID);
    var AddOption = {
        formatter: function (row) {
            var opts;   
            if (row.selected == true) {
                opts = "<input type='checkbox' checked='checked' id='" + row.CODE + "_" + comboboxID + "' value='" + row.CODE + "'>" + row.TITLE + "</input>";
            } else {
                opts = "<input type='checkbox' id='" + row.CODE + "_" + comboboxID + "' value='" + row.CODE + "'>" + row.TITLE + "</input>";
            }
            return opts;
        },
        onSelect: function (rec) {
            if (rec && select) {
                select(rec);
            }
            var id = "#" + rec.CODE + "_" + comboboxID;
            var checkbox = isTop == true ? top.$(id) : $(id);
            checkbox.attr("checked", true);
        },
        onUnselect: function (rec) {
            if (rec && unselect) {
                unselect(rec);
            }
            var id = "#" + rec.CODE + "_" + comboboxID;
            var checkbox = isTop == true ? top.$(id) : $(id);
            checkbox.attr("checked", false);
        },
        onLoadSuccess: function (data) {
            if (data && loadSuccess) {
                loadSuccess(data);
            }
            if (data && data.length > 0) {
                var selectValueArray = top.$(this).combobox('getValues');
                for (item in data) {
                    var code = data[item]["CODE"];
                    var id = "#" + code + "_" + comboboxID;
                    var checkbox = isTop == true ? top.$(id) : $(id);
                    if ($.inArray(code, selectValueArray) > -1) {
                        checkbox.attr("checked", true);
                    } else {
                        checkbox.attr("checked", false);
                    }

                }
            }
        },
        multiple: true,
    };
    option = $.extend(option, AddOption)
    combobox.combobox(option);
}
com.helper = com.helper || {};
com.helper.getUrlParam = function (name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r != null) return unescape(r[2]); return null; //返回参数值
}
//获取url参数   不会中文乱码
com.helper.getUrlParam2 = function (sArgName) {
    var LocString = window.location.href;
    var args = LocString.split("?");
    var retval = "";
    if (args[0] == LocString) {
        return retval;
    }
    var str = args[1];
    args = str.split("&");
    for (var i = 0; i < args.length; i++) {
        str = args[i];
        var arg = str.split("=");
        if (arg.length <= 1) continue;
        if (arg[0] == sArgName) retval = arg[1];
    }
    return retval;
}

//获取当前的时间 author:liwu 
//formatType dateTime:2018-01-01 00:00:00(默认) time:00:00:00 date:2017-01-01
com.currentDate = function (formatType) {
    var myDate = new Date();
    //获取当前年
    var year = myDate.getFullYear();
    //获取当前月
    var month = myDate.getMonth() + 1;
    //获取当前日
    var date = myDate.getDate();
    var h = myDate.getHours();       //获取当前小时数(0-23)
    var m = myDate.getMinutes();     //获取当前分钟数(0-59)
    var s = myDate.getSeconds();
    var numFormat = function (input) {
        if (input < 10) {
            return "0" + input.toString();
        }
        return input;
    }
    var showText = "";
    switch (formatType) {
        case "dateTime": showText = year + "-" + numFormat(month) + "-" + numFormat(date) + " " + numFormat(h) + ":" + numFormat(m) + ":" + numFormat(s); break;
        case "time": showText = numFormat(h) + ":" + numFormat(m) + ":" + numFormat(s); break;
        case "date": showText = year + "-" + numFormat(month) + "-" + numFormat(date); break;
        default: showText = year + "-" + numFormat(month) + "-" + numFormat(date) + " " + numFormat(h) + ":" + numFormat(m) + ":" + numFormat(s); break;
            
    }
    return showText;
}
//获取当前的时间 author:liwu 
//formatType dateTime:2018-01-01 00:00:00(默认) time:00:00:00 date:2017-01-01
com.DateFormat = function (myDate, formatType) {
    //获取当前年
    var year = myDate.getFullYear();
    //获取当前月
    var month = myDate.getMonth() + 1;
    //获取当前日
    var date = myDate.getDate();
    var h = myDate.getHours();       //获取当前小时数(0-23)
    var m = myDate.getMinutes();     //获取当前分钟数(0-59)
    var s = myDate.getSeconds();
    var numFormat = function (input) {
        if (input < 10) {
            return "0" + input.toString();
        }
        return input;
    }
    var showText = "";
    switch (formatType) {
        case "dateTime": showText = year + "-" + numFormat(month) + "-" + numFormat(date) + " " + numFormat(h) + ":" + numFormat(m) + ":" + numFormat(s); break;
        case "time": showText = numFormat(h) + ":" + numFormat(m) + ":" + numFormat(s); break;
        case "date": showText = year + "-" + numFormat(month) + "-" + numFormat(date); break;
        default: showText = year + "-" + numFormat(month) + "-" + numFormat(date) + " " + numFormat(h) + ":" + numFormat(m) + ":" + numFormat(s); break;

    }
    return showText;
}
//字典项下拉框的 author:liwu 
com.comboxDicCreate = function (id, typeCode, option) {
    var url = com.constants().getConstant("DIC_URL") + "?typeCode=" + typeCode;
    var options = $.extend(com.constants().getConstant("COMBOBOX_DEFAULT_OPTION"), option);
    com.ComboxBind(id, url, options);   
};  
com.comboxDicCreateSearch = function (id, typeCode, option) {
    var url = com.constants().getConstant("DIC_URL_SEARCH") + "?typeCode=" + typeCode;
    var options = $.extend(com.constants().getConstant("COMBOBOX_DEFAULT_OPTION"), option);
    com.ComboxBind(id, url, options);
};
//查询过滤  author:liwu 
com.filterRules = function () {
    var self = this;
    this.filterRuleArray = new Array();
    this.addFilter = function (fieldName, op, fieldValue) {
        var filter = {
            field: fieldName,
            op: op,
            value: fieldValue
        };
        self.filterRuleArray.push(filter);
    }
    this.addFilterByID = function (id, op, noTop) {
        var fieldValue = "";
        var ele;
        if (noTop && noTop == true) {
            ele = $("#" + id);
        }
        else {
            ele = top.$("#" + id);
        }
        var eleAttributes = ""
        var name = "";
        //大于1表示有容器属性
        if (ele.length < 1) return;
        eleAttributes = ele[0];
        //文本
        if ((eleAttributes.type == "text" || eleAttributes.type == "textarea")
            && eleAttributes.className.indexOf("easyui-datebox") < 0
            && eleAttributes.className.indexOf("easyui-datetimebox") < 0
            && eleAttributes.className.indexOf("easyui-numberbox") < 0
            ){
            fieldValue = ele.val();
        }
            //日期控件
        else if (eleAttributes.type == "text" && eleAttributes.className.indexOf("easyui-datebox") > 0)
            fieldValue = ele.datebox("getValue");
            //日期时间控件
        else if (eleAttributes.type == "text" && eleAttributes.className.indexOf("easyui-datetimebox") > 0)
            fieldValue = ele.datetimebox("getValue");
            //下拉框控件
        else if (eleAttributes.type == "select-one" && eleAttributes.className.indexOf("combobox") > 0) {
            fieldValue = ele.combobox("getValue");
        }
            //下拉树控件
        else if (eleAttributes.type == "select-one" && eleAttributes.className.indexOf("combotree") > 0) {
            fieldValue = ele.combotree("getValue");
        }
            //隐藏字段
        else if (eleAttributes.type == "hidden") {
            fieldValue = ele.val();
        }
            //数字控件
        else if (eleAttributes.type == "text" && eleAttributes.className.indexOf("easyui-numberbox") > 0) {
            fieldValue = ele.numberbox("getValue");
        }
        name = ele.attr("name");
        if (name == null) {
            name = ele.attr("comboname");
        }
        self.addFilter(name, op, fieldValue);
    }
    this.filterOp = {
        //包含--like
        contains: "contains",
        //等于
        equal: "equal",
        //小于
        less: "less",
        //大于
        greater: "greater",
        //not in 
        notin: "notin",
        //不等于
        notEqual: "notEqual"
    }
    this.returnResult = function () {
        return { filterRules: JSON.stringify(self.filterRuleArray) }
        
    }
}

$(function () {
    $(".gridBox tr:even").addClass("even")
    $(".gridBox tr").mouseover(function () {
        $(this).addClass("on").siblings().removeClass("on")
    })
    $(".gridBox tbody").mouseout(function () {
        $(".gridBox tbody tr").removeClass("on")
    })

})

//xy2017
Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
        (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
        RegExp.$1.length == 1 ? o[k] :
        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}

//xy2017
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

//xy2017
if (!String.prototype.trim) {
    String.prototype.trim = function () {
        return this.replace(/^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, '');
    };
}

//xy2017
if (!String.prototype.endWith) {
    String.prototype.endWith = function (str) {
        if (str == null || str == "" || this.length == 0 || str.length > this.length)
            return false;
        if (this.substring(this.length - str.length) == str)
            return true;
        else
            return false;
        return true;
    }
}

//xy2017
if (!String.prototype.startWith) {
    String.prototype.startWith = function (str) {
        if (str == null || str == "" || this.length == 0 || str.length > this.length)
            return false;
        if (this.substr(0, str.length) == str)
            return true;
        else
            return false;
        return true;
    }
}

//xy2017
function getMWeeks(year, month) {
    function formatNumber(value) {
        return (value < 10 ? '0' : '') + value;
    }
    var ret = [];
    var d = new Date();
    // what day is first day
    d.setFullYear(year, month - 1, 1);
    var w1 = d.getDay();
    if (w1 == 0) w1 = 7;
    // total day of month
    d.setFullYear(year, month, 0);
    var dd = d.getDate();
    // first Monday
    if (w1 != 1) d1 = 7 - w1 + 2;
    else d1 = 1;
    week_count = Math.ceil((dd - d1 + 1) / 7);
    for (var i = 0; i < week_count; i++) {
        var monday = d1 + i * 7;
        var sunday = monday + 6;
        var from = formatNumber(month) + "-" + formatNumber(monday);
        var to;
        if (sunday <= dd) {
            to = formatNumber(month) + "-" + formatNumber(sunday);
        } else {
            d.setFullYear(year, month - 1, sunday);
            to = formatNumber(d.getMonth() + 1) + "-" + formatNumber(d.getDate());
        }
        ret.push("第" + (i + 1) + "周 " + from + "至" + to)
    }
    return ret;
}

//xy2017
function getYWeeks(year) {
    function formatNumber(value) {
        return (value < 10 ? '0' : '') + value;
    }
    var ret = [];
    var sdate = new Date(Date.parse(year + "-01-01"));
    var edate = new Date(Date.parse((year + 1) + "-01-01"));
    var days = Math.floor((edate - sdate) / (24 * 3600 * 1000));
    var w1 = sdate.getDay();
    if (w1 == 0) w1 = 7;
    if (w1 != 1) w1 = 7 - w1 + 2;
    var fdate = new Date();
    fdate.setFullYear(sdate.getYear(), sdate.getMonth(), sdate.getDate());
    fdate.setDate(w1);
    var week_count = Math.ceil((days - w1 + 1) / 7);
    for (var i = 0; i < week_count; i++) {
        sdate.setFullYear(fdate.getYear(), fdate.getMonth(), fdate.getDate());
        sdate.setDate(sdate.getDate() + i * 7);
        edate.setFullYear(sdate.getYear(), sdate.getMonth(), sdate.getDate());
        edate.setDate(edate.getDate() + 6);
        var from = formatNumber(sdate.getMonth() + 1) + "-" + formatNumber(sdate.getDate());
        var to = formatNumber(edate.getMonth() + 1) + "-" + formatNumber(edate.getDate());
        ret.push("第" + formatNumber(i + 1) + "周 " + from + "至" + to);
    }
    return ret;
    //document.write(getYWeeks(2016).toString().replace(/,/g, "<br>"));
}

//xy2017
function getYWeek(date) {
    var year = date.getFullYear();
    var sdate = new Date(Date.parse(year + "-01-01"));
    var w1 = sdate.getDay();
    if (w1 == 0) w1 = 7;
    if (w1 != 1) w1 = 7 - w1 + 2;
    sdate.setDate(w1);
    var days = Math.floor((date - sdate) / (24 * 3600 * 1000));
    return Math.floor(days / 7) + 1;
}

//xy2017
function createGrid(id, option, onRowCreated, onCellCreated) {
    var divm = $("#" + id);
    option = option || {};
    var widthm = option.gridWidth || divm.width() || 421;
    var heightm = option.gridHeight || divm.height() || 294;
    var border = (undefined == option.border ? 1 : option.border);
    var rows = option.rows || 7;
    var cols = option.cols || 7;
    var width = option.cellWidth || Math.round((widthm - (cols + 1) * border) / cols);
    var height = option.cellHeight || Math.round((heightm - (rows + 1) * border) / rows);
    //如果不定义表格高度和宽度，且定义了行和列，行高和列宽，则自动计算表格高度和宽度
    if (undefined == option.gridWidth && undefined == option.gridHeight && undefined != option.rows && undefined != option.cols && undefined != option.cellWidth && undefined != option.cellHeight) {
        widthm = option.cellWidth * cols + (cols + 1) * border;
        heightm = option.cellHeight * rows + (rows + 1) * border;
    }
    var cellColor = option.cellColor || "#ffffff";
    var boderColor = (0 == border ? cellColor : (option.boderColor || "#009dd9"));
    var widthl = widthm - (cols + 1) * border - (cols - 1) * width;
    var heightl = heightm - (rows + 1) * border - (rows - 1) * height;
    divm.empty();
    divm.width(widthm);
    divm.height(heightm);
    divm.css("padding", "0");
    divm.css("background-color", boderColor);

    var ht = (rows + 1) * border, wt = 0;
    for (var row = 0; row < rows; row++) {
        var rid = "grid_" + id + "_row_" + row;
        var h = (row == rows - 1 ? heightl : height);
        divm.append("<div id='" + rid + "' style='margin-top:" + border + "px;float:left;height:" + h + "px; width:100%;display:inline-table;'>");
        var divRow = $("#" + rid);
        if (onRowCreated) {
            onRowCreated(divRow, row);
            h = divRow.height();
        }
        ht += h;
        var _wt = (cols + 1) * border;
        for (var col = 0; col < cols; col++) {
            var w = (col == cols - 1 ? widthl : width);
            var cid = rid + "_cell_" + col;
            divRow.append("<div id='" + cid + "' style='margin-left:" + border + "px; float:left; height:" + h + "px; width:" + w + "px; background-color:" + cellColor + "; display:inline-table;'></div>");
            var divCell = $("#" + cid);
            if (onCellCreated) {
                onCellCreated(divCell, row, col);
                w = divCell.width();
            }
            _wt += w;
        }
        if (_wt > wt) wt = _wt;
    }
    divm.width(wt);
    divm.height(ht);
}

//xy2017
function createCalender(id, year, month, cellWidth, cellHeight, data) {
    var days = new Date(year, month, 0).getDate();//代表上个月的最后一天
    var week1 = new Date(year, month - 1, 1).getDay();//获取当月第一天是星期几，日~六，0~6
    week1 = (0 == week1 ? 7 : week1) - 1;//转换为:一~日，0~6
    var rows = Math.ceil((days + week1) / 7);
    var weeks = ['一', '二', '三', '四', '五', '六', '日'];
    createGrid(id, {
        //gridWidth: 421,
        //gridHeight: 294,
        border: 1,
        cellWidth: cellWidth,
        cellHeight: cellHeight,
        rows: 1 + rows,
        cols: 7,
        boderColor: "#009dd9",
        cellColor: "#ffffff"
    }, function (divRow, row) {
        //if (0 == row) {
        //    divRow.height(20);
        //}
    }, function (divCell, row, col) {
        divCell.css("line-height", divCell.height() + "px");
        divCell.css("text-align", "center");
        if (0 == row) {
            divCell.css("background-color", "#c1f0ff");
            divCell.text(weeks[col]);
        } else {
            //if (0 == week1)//如果第一周的第一天是周一，则从第二行开始显示
            //{ if (1 == row) return; row--; }
            var day = (row - 1) * 7 + col + 1;
            if (day > week1 && (day - week1) <= days) {
                day = day - week1;
                divCell.attr("day", day);
                divCell.text(day);
                divCell.css("cursor", "pointer");
                if (data && data.length >= day) {
                    if (data[day - 1].bgColor) divCell.css("background-color", data[day - 1].bgColor);
                    if (data[day - 1].color) divCell.css("color", data[day - 1].color);
                    if (data[day - 1].value) divCell.attr("value", data[day - 1].value);
                }
            }
        }
    });
}

//xy2017
jQuery.extend({
    bindcombopanel: function (obj) {
        obj.next().click(function () {
            if (!obj.attr("disabled")) {
                if (obj.hasClass("combobox-f"))
                    obj.combobox("showPanel");
                else if (obj.hasClass("combotree-f"))
                    obj.combotree("showPanel");
            }
        });
    },
    createcombobox: function (obj, options) {
        //该参数是easyui-combobox data-options的扩展
        //data和url只能选择一个，优先data：返回的数据是[{id,text},{id,text}]这样的简单数据
        options = options || {};
        var formatter = options.formatter; //格式化每行数据formatter: function (row) {}
        var onLoadSuccess = options.onLoadSuccess; //加载成功时执行onLoadSuccess: function () {}
        var onBeforeSelect = options.onBeforeSelect; //在选之前执行onBeforeSelect: function (node) {}
        var url = options.url;
        options.url = undefined;
        options = $.extend({
            convertBlank: true, //把显示的文本空格转为全角空格，否则空格字符不显示
            addBlank: false,
            blankValue: { value: "", text: "　" },
            selectIndex: undefined, //0第1条，-1最后一条，undefined不设置
            selectValue: undefined, //选中的值
            autoShowPanel: true,
            postData: {}, //没指定data,指定url时提交的参数
            onQueryed: undefined//在ajax调用url返回结果后，onLoadBefore，onLoadSuccess之前执行onQueryed: function (data) {},返回用于Load的可用数组，返回undefined则不再加载
        }, options);

        options.postData = $.extend(options.postData, options.queryParams);

        if (options.convertBlank) {
            options.formatter = function (row) {//英文空格行不显示，转为全角空格;
                //formatter return false则该行不显示;return true 则继续判断；return 文本则直接返回文本；
                var ret = undefined;
                if (formatter) {
                    ret = formatter(row); //先调用自定义的formatter
                    if (false === ret) return false;
                    if (true !== ret) return ret;
                }
                if (undefined == row || "" == row)
                    return "　";
                else {
                    var opts = $(this).combobox('options');
                    var text = row[opts.textField];
                    return text;
                }
            }
        }
        if (undefined != options.selectIndex || undefined != options.selectValue) {//options.addBlank || 
            options.onLoadSuccess = function (data) { //加载完成后,设置选中第一项  
                //var vals = $(this).combobox('getData');
                var opts = options; // $(this).combobox('options');
                /*if (opts.addBlank) {
                if (vals.length > 0) {
                var val = eval("vals[0]." + opts.valueField);
                if ("" != val) {
                val = eval("({ " + opts.valueField + ": '', " + opts.textField + ": '　' })");
                vals.splice(0, 0, val);　// 列表第一行加入空行，此处空格用全角
                $(this).combobox('loadData', vals);
                }
                }
                }*/
                if (undefined != opts.selectIndex) {
                    var vals = $(this).combobox('getData');
                    if (vals.length > 0) {
                        var inx = undefined;
                        if (0 <= opts.selectIndex && opts.selectIndex < vals.length) {
                            inx = opts.selectIndex;
                        }
                        if (-1 == opts.selectIndex) inx = vals.length - 1;
                        if (undefined != inx) {
                            var val = eval("vals[inx]." + opts.valueField);
                            $(this).combobox('setValue', val);
                        }
                    }
                }
                if (undefined != opts.selectValue) {
                    $(this).combobox('setValue', opts.selectValue);
                }
                if (onLoadSuccess) onLoadSuccess(data); //调用自定义的onLoadSuccess
            }
        }
        var addBlank = function (vals) {
            var opts = options; // $(obj).combobox('options');
            if (opts.addBlank) {
                if (vals.length > 0) {
                    var val = undefined;
                    if (!isNaN(opts.valueField)) {//是数字
                        val = eval("vals[" + opts.valueField + "]");
                    }
                    else {
                        val = eval("vals[0]." + opts.valueField);
                    }
                    if (opts.blankValue.value != val) {
                        if (!isNaN(opts.valueField)) {//是数字
                            val = opts.blankValue.value;
                        }
                        else {
                            val = eval("({ " + opts.valueField + ": '" + opts.blankValue.value + "', " + opts.textField + ": '" + opts.blankValue.text + "' })");
                        }
                        vals.splice(0, 0, val); // 列表第一行加入空行，此处空格用全角
                    }
                }
            }
            return vals;
        };
        if (url) {
            com.ajax({
                url: url,
                data: options.postData,
                success: function (result) {
                    if (result.rows) result = result.rows;

                    if (options.onQueryed) {
                        var dats = options.onQueryed(result);
                        if (undefined == dats) return;
                        result = dats;
                    }

                    options.data = addBlank(result);
                    $(obj).combobox(options);
                    if (options.autoShowPanel) $.bindcombopanel($(obj));
                }
            });
        } else {
            options.data = addBlank(options.data);
            $(obj).combobox(options);
            if (options.autoShowPanel) $.bindcombopanel($(obj));
        }
    },

    getcomboboxoptions: function (options) {
        //该参数是easyui-combobox data-options的扩展
        //data和url只能选择一个，优先data：返回的数据是[{id,text},{id,text}]这样的简单数据
        options = options || {};
        var formatter = options.formatter; //格式化每行数据formatter: function (row) {}
        var onLoadSuccess = options.onLoadSuccess; //加载成功时执行onLoadSuccess: function () {}
        var onBeforeSelect = options.onBeforeSelect; //在选之前执行onBeforeSelect: function (node) {}
        options = $.extend({
            convertBlank: true, //把显示的文本空格转为全角空格，否则空格字符不显示
            addBlank: false,
            blankValue: { value: "", text: "　" },
            selectIndex: undefined, //0第1条，-1最后一条，undefined不设置
            selectValue: undefined, //选中的值
            autoShowPanel: false
        }, options);
        var addBlank = function (obj) {
            var opts = options; // $(obj).combobox('options');
            if (opts.addBlank) {
                var vals = $(obj).combobox('getData');
                if (vals.length > 0) {
                    var val = undefined;
                    if (!isNaN(opts.valueField)) {//是数字
                        val = eval("vals[" + opts.valueField + "]");
                    }
                    else {
                        val = eval("vals[0]." + opts.valueField);
                    }
                    if (opts.blankValue.value != val) {
                        if (!isNaN(opts.valueField)) {//是数字
                            val = opts.blankValue.value;
                        }
                        else {
                            val = eval("({ " + opts.valueField + ": '" + opts.blankValue.value + "', " + opts.textField + ": '" + opts.blankValue.text + "' })");
                        }
                        vals.splice(0, 0, val); // 列表第一行加入空行，此处空格用全角
                        return vals;
                    }
                }
            }
            return undefined;
        };
        if (options.convertBlank) {
            options.formatter = function (row) {//英文空格行不显示，转为全角空格
                //formatter return false则该行不显示;return true 则继续判断；return 文本则直接返回文本；
                var ret = undefined;
                if (formatter) {
                    ret = formatter(row); //先调用自定义的formatter
                    if (false === ret) return false;
                    if (true !== ret) return ret;
                }
                if (undefined == row || "" == row)
                    return "　";
                else {
                    var opts = $(this).combobox('options');
                    var text = row[opts.textField];
                    return text;
                }
            }
        }
        options.onLoadSuccess = function (data) { //加载完成后,设置选中第一项
            if (data.rows) {
                $(this).combobox('loadData', data.rows);
            }
            else {
                var vals = addBlank(this);
                if (undefined != vals) {
                    $(this).combobox('loadData', vals);
                }
                else {
                    var opts = options; // $(obj).combobox('options');
                    if (undefined != opts.selectIndex) {
                        var vals = $(this).combobox('getData');
                        if (vals.length > 0) {
                            var inx = undefined;
                            if (0 <= opts.selectIndex && opts.selectIndex < vals.length) {
                                inx = opts.selectIndex;
                            }
                            if (-1 == opts.selectIndex) inx = vals.length - 1;
                            if (undefined != inx) {
                                var val = eval("vals[inx]." + opts.valueField);
                                $(this).combobox('setValue', val);
                            }
                        }
                    }
                    if (undefined != opts.selectValue) {
                        var vals = $(this).combobox('getData');
                        if (vals.length > 0) {
                            $(this).combobox('setValue', opts.selectValue);
                        }
                    }
                    if (onLoadSuccess) onLoadSuccess(data); //调用自定义的onLoadSuccess
                }
            }
        }
        return options;
    },

    createtree: function (obj, options) {
        //该参数是easyui-tree data-options的扩展
        //data和url只能选择一个，优先data：返回的数据是[{id,text...},{id,text...}]这样的简单数据(只支持2级数据)，不需要children
        options = options || {};
        var onLoadSuccess = options.onLoadSuccess; //加载成功时执行onLoadSuccess: function (node, data) {}
        var onLoadError = options.onLoadError; //加载失败时执行onLoadError: function (XMLHttpRequest, textStatus, errorThrown) {}
        var onBeforeSelect = options.onBeforeSelect; //在选之前执行onBeforeSelect: function (node) {}
        var onSelect = options.onSelect; //在选择时发生
        var url = options.url;
        options.url = undefined;
        options = $.extend({
            valueField: "id",
            textField: "text",
            groupValue: undefined, //默认等于valueField，此时只有一层，且不分组
            groupText: undefined, //默认等于textField
            usePlatData: true, //默认使用数据是[{id,text...},{id,text...}]这样的简单数据(只支持2级数据)，flase时使用原来的数据格式
            firstIsRoot: false, //仅当usePlatData=true时有效，第一个节点是否为根节点，这样相当于支持3级
            groupPreFix: "_level1_",
            onlyLeafSelect: false,
            addBlank: false,
            blankValue: { value: "", text: "　" },
            iconCls: { level1: "tree-folder-open", level2: "tree-file" },
            initState: "open", //节点状态，'open' 或 'closed'，默认：'open'。如果为'closed'的时候，将不自动展开该节点。
            selectFirst: false, //与selectId二选一
            selectId: undefined, //undefined不设置，与selectFirst二选一
            postData: {}, //没指定data,指定url时提交的参数
            onLoadBefore: undefined, //function(datas){},加载前可以数据进行补充,返回新的datas
            onIconCls: function (isGroup, iconCls, value) { return iconCls; }, //可以重载该函数，根据value的值可以返回新的iconCls，默认返回原来的iconCls
            onQueryed: undefined//在ajax调用url返回结果后，onLoadBefore，onLoadSuccess之前执行onQueryed: function (data) {},返回用于Load的可用数组，返回undefined则不再加载
        }, options);
        options.groupValue = options.groupValue || options.groupText;
        options.groupText = options.groupText || options.groupValue;

        options.postData = $.extend(options.postData, options.queryParams);

        options.onSelect = function (node) {//提供点击切换显示
            $(this).tree(node.state === 'closed' ? 'expand' : 'collapse', node.target);
            if (onSelect) onSelect(node);
        };

        var getSelect = function () {
            var node = null;
            //var roots = $(obj).tree("getRoots");
            var roots = $(obj).tree("getChildren");
            if (roots.length > 0) {
                if (options.selectFirst) {
                    node = roots[0];
                } else if (undefined != options.selectId) {
                    for (var i = 0; i < roots.length; i++) {
                        if (options.selectId == roots[i].id) {
                            node = roots[i];
                            break;
                        } else if (roots[i].children && roots[i].children.length > 0) {
                            for (var j = 0; j < roots[i].children.length; j++) {
                                if (options.selectId == roots[i].children[j].id) {
                                    node = roots[i].children[j];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return node;
        };

        options.onLoadSuccess = function (node, data) {
            if (options.addBlank) {
                var roots = $(obj).tree("getRoots");
                if (roots.length > 0 && roots[0].id == options.blankValue.value) {
                    var icon = $(roots[0].target).find(".tree-icon");
                    icon.removeClass("tree-file");
                    icon.removeClass("tree-folder-open");
                    var indent = $(roots[0].target).find(".tree-indent");
                    indent.removeClass("tree-join");
                    indent.removeClass("tree-indent");
                }
            }
            //var node = getSelect();
            //if (null != node) $(obj).tree("select", node.target);
            if (options.selectFirst || undefined != options.selectId) {
                /*var id = undefined;
                if (options.selectFirst) {
                var nodes = $(obj).tree("getChildren");
                if (nodes.length > 0) id = nodes[0].id;
                } else
                id = options.selectId;
                $(obj).tree("setValue", id);*/

                var nd = undefined;
                if (options.selectFirst) {
                    var nodes = $(obj).tree("getChildren");
                    if (nodes.length > 0) nd = nodes[0];
                } else {
                    var nd = $(obj).tree('find', options.selectId);
                }
                $(obj).tree("select", nd.target);
            }
            if (onLoadSuccess) onLoadSuccess(node, data); //调用自定义的onLoadSuccess
        }

        if (options.onlyLeafSelect) {
            options.onBeforeSelect = function (node) {
                var ret = undefined;
                if (onBeforeSelect) ret = onBeforeSelect(node); //先调用自定义的onBeforeSelect
                if (false == ret) return ret;
                //返回树对象 
                var tree = $(obj).tree;
                //选中的节点是否为叶子节点,如果不是叶子节点,清除选中 
                var isLeaf = tree('isLeaf', node.target);
                if (!isLeaf) return false;
            };
        }
        //id：节点ID，对加载远程数据很重要。
        //text：显示节点文本。
        //"iconCls": "icon-save",
        //state：节点状态，'open' 或 'closed'，默认：'open'。如果为'closed'的时候，将不自动展开该节点。
        //checked：表示该节点是否被选中。
        //attributes: 被添加到节点的自定义属性。
        //children: 一个节点数组声明了若干节点。
        var getDatas = function (vals) {
            var datas = [];
            var opts = options;
            if (opts.usePlatData) {
                if (vals && vals.length > 0) {
                    if (undefined == opts.groupValue) {
                        for (var i = 0; i < vals.length; i++) {
                            var iconCls = opts.onIconCls(false, options.iconCls.level2, vals[i]);
                            var node = eval("({ id: vals[i]." + opts.valueField + ", text: vals[i]." + opts.textField + ", attributes: vals[i], iconCls: iconCls })");
                            datas.push(node);
                        }
                    }
                    else {
                        var lastId = undefined;
                        var children = [];
                        var node = {};
                        var root = undefined;
                        var from = opts.firstIsRoot ? 1 : 0;

                        if (opts.firstIsRoot && vals.length > 0) {
                            var gv = eval("vals[0]." + opts.groupValue);
                            var iconCls = opts.onIconCls(true, options.iconCls.level1, vals[0]);
                            root = eval("({ isRoot:true, isGroup:true, id: vals[0]." + opts.groupValue + ", text: vals[0]." + opts.groupText + ", attributes: vals[0], iconCls: iconCls,children:[],state:'" + options.initState + "' })");
                        }

                        for (var i = from; i < vals.length; i++) {
                            var gv = eval("vals[i]." + opts.groupValue);
                            if (gv != lastId) {
                                var iconCls = opts.onIconCls(true, options.iconCls.level1, vals[i]);
                                node = eval("({ isGroup:true, id: vals[i]." + opts.groupValue + ", text: vals[i]." + opts.groupText + ", attributes: vals[i], iconCls: iconCls,children:[],state:'" + options.initState + "' })");
                                datas.push(node);
                                lastId = gv;
                            }
                            var id = eval("vals[i]." + opts.valueField);
                            var text = eval("vals[i]." + opts.textField);
                            if (undefined != id && null != id && "" != id) {
                                var iconCls = opts.onIconCls(false, options.iconCls.level2, vals[i]);
                                var snode = eval("({ id: vals[i]." + opts.valueField + ", text: vals[i]." + opts.textField + ", attributes: vals[i], iconCls: iconCls })");
                                node.children.push(snode);
                            }
                        }

                        if (undefined != root) {
                            root.children = datas;
                            datas = [];
                            datas.push(root);
                        }
                    }
                }
            }
            else {
                datas = vals;
            }
            if (opts.addBlank) {
                if (datas.length > 0) {
                    val = eval("({ id: '" + opts.blankValue.value + "', text: '" + opts.blankValue.text + "' })");
                    datas.splice(0, 0, val); // 列表第一行加入空行，此处空格用全角
                }
            }
            if (opts.onLoadBefore) {
                var ret = opts.onLoadBefore(datas);
                if (ret) datas = ret;
            }
            return datas;
        };
        if (url) {
            com.ajax({
                url: url,
                data: options.postData,
                success: function (result) {
                    if (options.onQueryed) {
                        var dats = options.onQueryed(result);
                        if (undefined == dats) return;
                        options.data = getDatas(dats);
                    }
                    else
                        options.data = getDatas(result);
                    $(obj).tree(options);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (onLoadError) onLoadError(XMLHttpRequest, textStatus, errorThrown);
                }
            });
        } else {
            options.data = getDatas(options.data);
            $(obj).tree(options);
        }
    },

    createcombotree: function (obj, options) {
        //该参数是easyui-combotree data-options的扩展
        //data和url只能选择一个，优先data：返回的数据是[{id,text...},{id,text...}]这样的简单数据(只支持2级数据)，不需要children
        options = options || {};
        var onLoadSuccess = options.onLoadSuccess; //加载成功时执行onLoadSuccess: function (node, data) {}
        var onLoadError = options.onLoadError; //加载失败时执行onLoadError: function (XMLHttpRequest, textStatus, errorThrown) {}
        var onBeforeSelect = options.onBeforeSelect; //在选之前执行onBeforeSelect: function (node) {}
        var onSelect = options.onSelect; //在选择时发生
        var url = options.url;
        options.url = undefined;
        options = $.extend({
            valueField: "id",
            textField: "text",
            groupValue: undefined, //默认等于valueField，此时只有一层，且不分组
            groupText: undefined, //默认等于textField
            usePlatData: true, //默认使用数据是[{id,text...},{id,text...}]这样的简单数据(只支持2级数据)，flase时使用原来的数据格式
            firstIsRoot: false, //仅当usePlatData=true时有效，第一个节点是否为根节点，这样相当于支持3级
            groupPreFix: "_level1_",
            onlyLeafSelect: false,
            addBlank: false,
            blankValue: { value: "", text: " " },
            iconClsTree: { level1: "tree-folder-open", level2: "tree-file" },
            initState: "open", //节点状态，'open' 或 'closed'，默认：'open'。如果为'closed'的时候，将不自动展开该节点。
            selectFirst: false, //与selectId二选一
            selectId: undefined, //undefined不设置，与selectFirst二选一
            postData: {}, //没指定data,指定url时提交的参数
            onLoadBefore: undefined, //function(datas){},加载前可以数据进行补充,返回新的datas
            onIconCls: function (isGroup, iconCls, value) { return iconCls; }, //可以重载该函数，根据value的值可以返回新的iconCls，默认返回原来的iconCls
            onQueryed: undefined, //在ajax调用url返回结果后，onLoadBefore，onLoadSuccess之前执行onQueryed: function (data) {},返回用于Load的可用数组，返回undefined则不再加载
            autoShowPanel: true,
            onShowPanel: function () {
                var ct = $(this);
                var p = ct.datebox('panel').parent();
                p.find(".datebox-button").remove();
                p.append('<div class="datebox-button" style="width: 100%"><table cellspacing="0" cellpadding="0" style="width:100%"><tbody><tr><td style="width: 50%;"><a class="datebox-button-a button-ok" href="javascript:void(0)"></a></td><td style="width: 50%;"><a class="datebox-button-a  button-close" href="javascript:void(0)" style="float:right;margin-right:20px;">关闭</a></td></tr></tbody></table></div>');
                //p.find(".datebox-button .button-ok").click(function () { ct.combotree('hidePanel'); });
                p.find(".datebox-button .button-close").click(function () { ct.combotree('hidePanel'); });
            }
        }, options);
        options.groupValue = options.groupValue || options.groupText;
        options.groupText = options.groupText || options.groupValue;

        options.postData = $.extend(options.postData, options.queryParams);

        var getSelect = function () {
            var node = null;
            //var roots = $(obj).combotree('tree').tree("getRoots");
            var roots = $(obj).combotree('tree').tree("getChildren");
            if (roots.length > 0) {
                if (options.selectFirst) {
                    node = roots[0];
                } else if (undefined != options.selectId) {
                    for (var i = 0; i < roots.length; i++) {
                        if (options.selectId == roots[i].id) {
                            node = roots[i];
                            break;
                        } else if (roots[i].children && roots[i].children.length > 0) {
                            for (var j = 0; j < roots[i].children.length; j++) {
                                if (options.selectId == roots[i].children[j].id) {
                                    node = roots[i].children[j];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return node;
        };

        options.onLoadSuccess = function (node, data) {
            if (options.addBlank) {
                var roots = $(obj).combotree('tree').tree("getRoots");
                if (roots.length > 0 && roots[0].id == options.blankValue.value) {
                    var icon = $(roots[0].target).find(".tree-icon");
                    icon.removeClass("tree-file");
                    icon.removeClass("tree-folder-open");
                    var indent = $(roots[0].target).find(".tree-indent");
                    indent.removeClass("tree-join");
                    indent.removeClass("tree-indent");
                }
            }
            //$(obj).combotree("clear");
            if (options.selectFirst || undefined != options.selectId) {
                //var node = getSelect();
                //if (null != node) $(obj).combotree('tree').tree("select", node.target);
                //if (null != node) $(obj).combotree('tree').tree("setValue", node.id);
                //if (null != node) $(obj).combotree("setValue", node.id);
                var id = undefined;
                if (options.selectFirst) {
                    var nodes = $(obj).combotree('tree').tree("getChildren");
                    if (nodes.length > 0) id = nodes[0].id;
                } else
                    id = options.selectId;
                $(obj).combotree("setValue", id);
            }
            //else
            //    $(obj).combotree("clear");
            if (onLoadSuccess) onLoadSuccess(node, data); //调用自定义的onLoadSuccess
        }

        if (options.onlyLeafSelect) {
            options.onBeforeSelect = function (node) {
                var ret = undefined;
                if (onBeforeSelect) ret = onBeforeSelect(node); //先调用自定义的onBeforeSelect
                if (false == ret) return ret;
                //判断是否有isGroup属性为true
                if (node.isGroup) return false;
                //返回树对象 
                var tree = $(obj).combotree('tree').tree;
                //选中的节点是否为叶子节点,如果不是叶子节点,清除选中 
                var isLeaf = tree('isLeaf', node.target);
                if (!isLeaf) return false;
            };
        }
        //id：节点ID，对加载远程数据很重要。
        //text：显示节点文本。
        //"iconCls": "icon-save",
        //state：节点状态，'open' 或 'closed'，默认：'open'。如果为'closed'的时候，将不自动展开该节点。
        //checked：表示该节点是否被选中。
        //attributes: 被添加到节点的自定义属性。
        //children: 一个节点数组声明了若干节点。
        var getDatas = function (vals) {
            var datas = [];
            var opts = options;
            if (opts.usePlatData) {
                if (vals && vals.length > 0) {

                    var root = undefined;
                    var from = opts.firstIsRoot ? 1 : 0;

                    if (undefined == opts.groupValue) {
                        if (opts.firstIsRoot && vals.length > 0) {
                            var gv = eval("vals[0]." + opts.groupValue);
                            var iconCls = opts.onIconCls(true, options.iconClsTree.level1, vals[0]);
                            root = eval("({ isRoot:true, isGroup:true, id: vals[0]." + opts.valueField + ", text: vals[0]." + opts.textField + ", attributes: vals[0], iconCls: iconCls,children:[],state:'" + options.initState + "' })");
                        }
                        for (var i = from; i < vals.length; i++) {
                            var iconCls = opts.onIconCls(false, options.iconClsTree.level2, vals[i]);
                            var node = eval("({ id: vals[i]." + opts.valueField + ", text: vals[i]." + opts.textField + ", attributes: vals[i], iconCls: iconCls })");
                            datas.push(node);
                        }
                    }
                    else {
                        var lastId = undefined;
                        var children = [];
                        var node = {};

                        if (opts.firstIsRoot && vals.length > 0) {
                            var gv = eval("vals[0]." + opts.groupValue);
                            var iconCls = opts.onIconCls(true, options.iconClsTree.level1, vals[0]);
                            root = eval("({ isRoot:true, isGroup:true, id: vals[0]." + opts.groupValue + ", text: vals[0]." + opts.groupText + ", attributes: vals[0], iconCls: iconCls,children:[],state:'" + options.initState + "' })");
                        }

                        for (var i = from; i < vals.length; i++) {
                            var gv = eval("vals[i]." + opts.groupValue);
                            if (gv != lastId) {
                                var iconCls = opts.onIconCls(true, options.iconClsTree.level1, vals[i]);
                                node = eval("({ isGroup:true, id: vals[i]." + opts.groupValue + ", text: vals[i]." + opts.groupText + ", attributes: vals[i], iconCls: iconCls,children:[],state:'" + options.initState + "' })");
                                datas.push(node);
                                lastId = gv;
                            }
                            var id = eval("vals[i]." + opts.valueField);
                            var text = eval("vals[i]." + opts.textField);
                            if (undefined != id && null != id && "" != id) {
                                var iconCls = opts.onIconCls(false, options.iconClsTree.level2, vals[i]);
                                var snode = eval("({ id: vals[i]." + opts.valueField + ", text: vals[i]." + opts.textField + ", attributes: vals[i], iconCls: iconCls })");
                                node.children.push(snode);
                            }
                        }
                    }
                    if (undefined != root) {
                        root.children = datas;
                        datas = [];
                        datas.push(root);
                    }
                }
            }
            else {
                datas = vals;
            }
            if (opts.addBlank) {
                if (datas.length > 0) {
                    val = eval("({ id: '" + opts.blankValue.value + "', text: '" + opts.blankValue.text + "' })");
                    datas.splice(0, 0, val); // 列表第一行加入空行，此处空格用全角
                }
            }
            if (opts.onLoadBefore) {
                var ret = opts.onLoadBefore(datas);
                if (ret) datas = ret;
            }
            return datas;
        };
        if (url) {
            com.ajax({
                url: url,
                data: options.postData,
                success: function (result) {
                    if (options.onQueryed) {
                        var dats = options.onQueryed(result);
                        if (undefined == dats) return;
                        options.data = getDatas(dats);
                    }
                    else
                        options.data = getDatas(result);
                    $(obj).combotree(options);
                    if (options.autoShowPanel) $.bindcombopanel($(obj));
                }
            });
        } else {
            options.data = getDatas(options.data);
            $(obj).combotree(options);
            if (options.autoShowPanel) $.bindcombopanel($(obj));
        }
    }
});


//wdk20171226

com.ComboTreeBind = function (top, id, url, editable) {
    top.$("#" + id).combotree({
        url: url,///Bas/Dic/GetDicCodeDataForSearch?typeCode=00014
        valueField: "CODE",
        textField: "TITLE",
        //required: true,
        editable: editable,
        panelHeight: 200,
        initialState: "collapsed",
        onLoadSuccess: function () {
            top.$("#" + id).combotree('tree').tree("collapseAll");
        }
    });

}

com.ComboxBind = function (id, url, obj) {
    var option = {
        url: url,
        valueField: "CODE",
        textField: "TITLE",
        width: 200,
        editable: false,
        panelHeight: "auto",
    };
    option = $.extend(option, obj);
    //top.$("#" + id).combobox(option);
    //防止加top取不到元素  liwu 修改
    var ele = top.$("#" + id);
    if (ele.length > 0) {
        ele.combobox(option);
    }else{
        $("#" + id).combobox(option);
    }
}

com.ListBind = function (id, url, obj)
{
    var option = {
        url: url,
        idField: "ID",
        sortName: "ID",
        sortOrder: "desc",
        rownumbers: true,
        singleSelect: true,
        pagination: true,
        striped: true,
        height: 300,
        columns: [[
        //{ field: "ID", checkbox: true },//checks
        //{ field: "ID", title: "ID", width: 150 }

        ]],
        onDblClickRow: function (index, row) {
            //selectedCustomer(row);
        }

    }
    option = $.extend(option, obj);
    top.$("#"+id).edatagrid(option);
}

com.ListBindNoTop = function (id, url, obj) {
    var option = {
        url: url,
        idField: "ID",
        sortName: "ID",
        sortOrder: "desc",
        rownumbers: true,
        singleSelect: true,
        pagination: true,
        striped: true,
        height: 300,
        columns: [[
        //{ field: "ID", checkbox: true },//checks
        //{ field: "ID", title: "ID", width: 150 }

        ]],
        onDblClickRow: function (index, row) {
            //selectedCustomer(row);
        }

    }
    option = $.extend(option, obj);
    $("#" + id).edatagrid(option);
}

//一列多行
com.ListColspan = function (isTop,data, urlStr,className)
{
    var getClassEml;
    if (data) {
        //---开始
        var codeArray = new Array();
        
        if (isTop)
            getClassEml = top.$("." + className);
        else
            getClassEml = $("." + className);
        getClassEml.each(function (i, con) {
            var areaCodeStr = $(con).data("codes");
            if (areaCodeStr) {
                var tempCodeArray = areaCodeStr.toString().split(",");
                $.each(tempCodeArray, function (index) {
                    if ($.inArray(tempCodeArray[index], codeArray) < 0) {
                        codeArray.push(tempCodeArray[index])
                    }
                })
            }

        })
        com.ajax({
            url: urlStr,
            data: { codes: JSON.stringify(codeArray) },
            success: function (ajaxData) {
                if (isTop)
                    getClassEml = top.$("." + className);
                else
                    getClassEml = $("." + className);
                if (ajaxData && ajaxData.Success) {
                    getClassEml.each(function (i, con) {
                        var areaHtml = "";
                        var areaCodeStr = $(con).data("codes");
                        if (areaCodeStr) {
                            var areaCodeArray = areaCodeStr.toString().split(",");
                            $.each(ajaxData.Data, function (index) {
                                var areaEntry = ajaxData.Data[index];
                                if ($.inArray(areaEntry["CODE"].toString(), areaCodeArray) > -1) {
                                    areaHtml += ("," + areaEntry["TEXT"]);
                                }
                            });
                            if (areaHtml.length > 0) {
                                $(this).html(areaHtml.substr(1, areaHtml.length - 1));
                            }
                        }
                    })

                }
            }
        });
        //---结束
    }
}

com.SetEditReadonly = function (row,isReadonly)
{
    for (var item in row) {
        //大于1表示有容器属性
        if (top.$("#" + item).length > 0) {
            //文本
            top.$("#" + item).attr("readonly", isReadonly);
            if (top.$("#" + item)[0].type == "select-one") {
                top.$("#" + item).combobox({
                    onShowPanel: function (data) {
                        top.$(this).combobox("hidePanel");
                    }
                });
               
            }
            else if (top.$("#" + item)[0].className.indexOf("easyui-datebox") > 0 || top.$("#" + item)[0].className.indexOf("easyui-datetimebox") > 0)
            {
                top.$("#" + item).datebox({
                    onShowPanel: function (data) {
                        top.$(this).datebox("hidePanel");
                    }
                });
            }
           
               
        }
    }
}

//绑定编辑控件
com.BinEditVal = function (top, row) {
    debugger;
    //绑定
    for (var item in row) {
        //大于1表示有容器属性
        if (top.$("#" + item).length > 0) {
            //文本
            if ((top.$("#" + item)[0].type == "text" || top.$("#" + item)[0].type == "textarea") && top.$("#" + item)[0].className.indexOf("easyui-datebox") < 0 && top.$("#" + item)[0].className.indexOf("easyui-datetimebox")< 0)
                top.$("#" + item).val(row[item]);
                //日期控件
            
            else if (top.$("#" + item)[0].type == "text" && top.$("#" + item)[0].className.indexOf("easyui-datebox") > 0 || top.$("#" + item)[0].className.indexOf("easyui-datetimebox") > 0)
                top.$("#" + item).datebox("setValue", com.formatter.date(row[item]));
                //下拉框控件
            else if (top.$("#" + item)[0].type == "select-one" && top.$("#" + item)[0].className.indexOf("combotree-f") < 0)
                top.$("#" + item).combobox("setValue", row[item]);
                //复选框
            else if (top.$("#" + item)[0].type == "checkbox") {
                if (row[item] == "1") {
                    top.$("#" + item).attr("checked", true);
                    top.$("#" + item).val(row[item]);
                }
            }
                // else if (item == "ORGID")
                //{
                // top.$("#"+ item).combotree("setValue", row.ORGID > 0 ? row.ORGID : -1);
                // }
            else if (top.$("#" + item)[0].type == "select-one" && top.$("#" + item)[0].className.indexOf("combotree-f") > 0) {
                //debugger;
                top.$("#" + item).combotree("setValue", row[item]);
            }

        }
    }
}

//xy2017
jQuery.fn.extend({
    createcombobox: function (options) {//BY XY
        this.each(function () {
            $.createcombobox(this, options);
        });
    },

    getcombobox: function (options) {//BY XY
        var options = $.getcomboboxoptions(options);
        var cbs = this.combobox(options);
        if (options.autoShowPanel) $.bindcombopanel($(this));
        return cbs;
    },

    createtree: function (options) {//BY XY
        this.each(function () {
            $.createtree(this, options);
        });
    },

    createcombotree: function (options) {//BY XY
        this.each(function () {
            $.createcombotree(this, options);
        });
    },

    createdatetimebox: function (options) {//BY XY
        options = $.extend({
            closeText: '关闭',
            formatter: function (date) {
                var y = date.getFullYear();
                var m = date.getMonth() + 1;
                var d = date.getDate();
                var h = date.getHours();
                var M = date.getMinutes();
                var s = date.getSeconds();
                function formatNumber(value) {
                    return (value < 10 ? '0' : '') + value;
                }
                return y + '-' + formatNumber(m) + '-' + formatNumber(d) + ' ' + formatNumber(h) + ':' + formatNumber(M) + ':' + formatNumber(s);
            },
            parser: function (s) {
                var t = Date.parse(s);
                if (!isNaN(t)) {
                    return new Date(t);
                } else {
                    return new Date();
                }
            }
        }, options);
        var ret = $(this).datetimebox(options);
        if (options.value) {
            if (options.value instanceof Date) {
                $(this).datetimebox("setValue", options.value.format('yyyy-MM-dd HH:mm:ss'));
            }
            else
                $(this).datetimebox("setValue", options.value);
        }
        return ret;
    },

    createdatebox: function (options) {//BY XY
        options = $.extend({
            closeText: '关闭',
            formatter: function (date) {
                var y = date.getFullYear();
                var m = date.getMonth() + 1;
                var d = date.getDate();
                var h = date.getHours();
                var M = date.getMinutes();
                var s = date.getSeconds();
                function formatNumber(value) {
                    return (value < 10 ? '0' : '') + value;
                }
                return y + '-' + formatNumber(m) + '-' + formatNumber(d);
            },
            parser: function (s) {
                var t = Date.parse(s);
                if (!isNaN(t)) {
                    return new Date(t);
                } else {
                    return new Date();
                }
            }
        }, options);
        var ret = $(this).datebox(options);
        if (options.value) {
            if (options.value instanceof Date) {
                $(this).datebox("setValue", options.value.format('yyyy-MM-dd'));
            }
            else
                $(this).datebox("setValue", options.value);
        }
        return ret;
    },

    getdatebox: function (options) {//BY XY
        options = options || {};
        var format = options.format || "date";
        options.value = options.value || new Date();
        switch (format) {
            case "date":
                return $(this).createdatebox(options);
            case "datetime":
                if (options.value instanceof Date) options.value = options.value.format('yyyy-MM-dd hh:mm:ss');
                return $(this).createdatetimebox(options);
            default:
                return $(this).createdatebox(options);
        }
    }

});

//复制一个数组的元素
com.ArrayEleCopy = function (originalArray, addArray) {
    if (originalArray && addArray) {
        $.each(addArray, function (i, value) {
            originalArray.push(value);
        });
    }
};
//打开标签页的编辑页面
//name:tab的名称(自定义)
//url:tab内容的路径,非添加，需要加上ID参数
//isAdd：打开方式 true-添加  false-编辑  null-查看
//guid:用于确定回调
//author:liwu 2018-01-25
com.openTabEditPage = function (name, url, isAdd,guid) {
    var tab;
    if ("object" == typeof parent.idx) {
        tab = parent.idx;
    }
    if ("object" == typeof idx) {
        tab = idx;
    }
    if (tab) {
        var uri = url + "&OpenType=2&navId=001&tabName=" + encodeURI(name) + "&isAdd=" + isAdd + "&guid=" + guid;
        tab.addTab(name, uri, "icon-note");
    }
}
com.helper.guid = function () {
    return new Date().getTime() + "_" + Math.round(Math.random() * 10000);
}
com.url = com.url || {};
//拼装Url参数
com.url.createParam =  function urlParam() {
    var paramArray = [];
    this.addParam = function (param) {
        paramArray.push(param);
    }
    this.empty = function () {
        paramArray = [];
    }
    this.toUrlParam = function () {
        var paramStr = "random=" + com.helper.guid();
        if (paramArray.length > 0) {
            for (i = 0; i < paramArray.length; i++) {
                var paramObj = paramArray[i];
                for (itemName in paramObj) {
                    paramStr = paramStr + "&" + itemName + "=" + paramObj[itemName];
                }
            }
        }
        return paramStr;
    }
}
//文件下载
//上传参数例子[{key:1,value:123},{key:2,value:456}]
com.fileDownLoad = function (url,/*上传参数*/param) {
    //var url = "/Business/AssessMana/Statistical/Export";
    var form = $("<form>");//定义form表单,通过表单发送请求
    form.attr("style", "display:none");//设置为不显示
    form.attr("target", "");
    form.attr("method", "get");//设置请求类型  
    form.attr("action", url);//设置请求路径
    // 创建Input  
    if (param.constructor == Array) {
        for (index in param) {
            var item = param[index];
            if (item["key"] && item["value"]) {
                var ele = $('<input type="text" name="' + item["key"] + '" />');
                ele.attr('value', item["value"]);
                form.append(ele);
            }
        }
    }
    $("body").append(form);//添加表单到页面(body)中
    form.submit();//表单提交
}

