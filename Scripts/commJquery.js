var upFileSize = "102400KB";   // 100M  默认文件上传大小
var upImgSize = "102400KB";      // 10M   默认图片上传大小


// 公共方法 Create by 高怡鑫 2015-08-05
$.extend({
    /** 
    1. 设置cookie的值，把name变量的值设为value   
    example $.cookie(’name’, ‘value’);
    2.新建一个cookie 包括有效期 路径 域名等
    example $.cookie(’name’, ‘value’, {expires: 7, path: ‘/’, domain: ‘jquery.com’, secure: true});
    3.新建cookie
    example $.cookie(’name’, ‘value’);
    4.删除一个cookie
    example $.cookie(’name’, null);
    5.取一个cookie(name)值给myvar
    var account= $.cookie('name');
    **/
    cookieHelper: function (name, value, options) {
        if (typeof value != 'undefined') { // name and value given, set cookie
            options = options || {};
            if (value === null) {
                value = '';
                options.expires = -1;
            }
            var expires = '';
            if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
                var date;
                if (typeof options.expires == 'number') {
                    date = new Date();
                    date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
                } else {
                    date = options.expires;
                }
                expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
            }
            var path = options.path ? '; path=' + options.path : '';
            var domain = options.domain ? '; domain=' + options.domain : '';
            var secure = options.secure ? '; secure' : '';
            document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
        } else { // only name given, get cookie
            var cookieValue = null;
            if (document.cookie && document.cookie != '') {
                var cookies = document.cookie.split(';');
                for (var i = 0; i < cookies.length; i++) {
                    var cookie = jQuery.trim(cookies[i]);
                    // Does this cookie string begin with the name we want?
                    if (cookie.substring(0, name.length + 1) == (name + '=')) {
                        cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                        break;
                    }
                }
            }
            return cookieValue;
        }
    }

});

//获取request
function request(strParame) {
    var args = new Object();
    var query = location.search.substring(1);

    var pairs = query.split("&"); // Break at ampersand 
    for (var i = 0; i < pairs.length; i++) {
        var pos = pairs[i].indexOf('=');
        if (pos == -1) continue;
        var argname = pairs[i].substring(0, pos);
        var value = pairs[i].substring(pos + 1);
        value = decodeURIComponent(value);
        args[argname] = value;
    }
    return args[strParame];
}

//提交form数据到后台
function ajaxSubmit(url, fnbeforeSubmit, fnSuccess) {
    var options = {
        cache: false,
        async: false,
        beforeSubmit: fnbeforeSubmit,
        success: fnSuccess,
        url: url,
        type: "post"
    };
    //表单提交前进行校验 create by ssz
    try {
        if (!$("#form1").validate()) {
            return false;
        }
    }
    catch (e) {
    }

    $("#form1").ajaxSubmit(options);
    return false;
}

//序列化表单数据为JSON字符串
$.fn.formtojson = function () {
    var obj = {};
    var count = 0;
    $.each(this.serializeArray(), function (i, o) {
        var n = o.name, v = o.value;
        count++;
        obj[n] = obj[n] === undefined ? v
        : $.isArray(obj[n]) ? obj[n].concat(v)
        : [obj[n], v];
    });
    obj.nameCounts = count + ""; //表单name个数
    return JSON.stringify(obj);
};
//序列化表单数据为JSON对象
$.fn.formtojsonObject = function () {
    var obj = {};
    var count = 0;
    $.each(this.serializeArray(), function (i, o) {
        var n = o.name, v = o.value;
        count++;
        obj[n] = obj[n] === undefined ? v
        : $.isArray(obj[n]) ? obj[n].concat(v)
        : [obj[n], v];
    });
    obj.nameCounts = count + ""; //表单name个数
    return obj;
};

//将Json绑定到表单
function bindJsonToPage(data) {
    var mainform = $("#form1");
    for (var p in data) {
        var ele = $("[name=" + p + "]", mainform);
        //针对复选框和单选框 处理
        if (ele.is(":checkbox,:radio")) {
            ele[0].checked = data[p] ? true : false;
        }
        else if (ele.is("label,span"))
            ele.html(data[p]);
        else {
            ele.val(data[p]);
        }
    }
}
function bindJsonToForm(formid, data) {
    var mainform = $("#" + formid);
    for (var p in data) {
        var ele = $("[name=" + p + "]", mainform);
        //针对复选框和单选框 处理
        if (ele.is(":checkbox,:radio")) {

            //            ele[0].checked = data[p]  ? true : false;
            ele[0].checked = data[p] == '1' ? true : false;
        }
        else if (ele.is("label,span"))
            ele.html(data[p]);
        else {
            ele.val(data[p]);
        }
    }
}
//清空表单
function clearForm(formid) {
    if (document.getElementById(formid).tagName.toLowerCase() == "form")
        document.getElementById(formid).reset();
    var arrItem = $('#' + formid + ' [name]');
    $(arrItem).each(function (i, item) {
        //        if (item.tagName.toLowerCase() == 'label' || item.tagName.toLowerCase() == 'span')
        //            $(item).html('');
        //        else if (item.tagName.toLowerCase() == 'input') {

        //        }

        var jqItem = $(item);
        if (jqItem.is("label,span"))
            jqItem.html('');
        else if (jqItem.is("input")) {
            jqItem.not(":button, :submit, :reset, :hidden").val("").removeAttr("checked").remove("selected");
        }
        else
            jqItem.val('');
    })
}



//显示状态样式
//True-打勾，其他打叉
function formatState(value) {
    if (value == null) value = '';
    var str = "";
    if (value.toLowerCase() == "true" || value == "1" || value.toLowerCase() == "active") {
        str = "<img src='/Scripts/easyui/themes/icons/ok.png'></img>";

    } else {
        str = "<img src='/Scripts/easyui/themes/icons/no.png'></img>";

    }
    return str;
}
//显示状态样式_相反
//True-打叉，其他打勾
function formatStateEx(value) {
    if (value == null) value = '';
    var str = "";
    if (value.toLowerCase() == "true" || value == "1") {
        str = "<img src='/Scripts/easyui/themes/icons/no.png'></img>";
    } else {
        str = "<img src='/Scripts/easyui/themes/icons/ok.png'></img>";
    }
    return str;
}

//显示状态样式_相反
//False-打勾，其他打叉
function formatStateEx2(value) {
    if (value == null) value = '';
    var str = "";
    if (value.toLowerCase() == "false" || value == "0") {
        str = "<img src='/Scripts/easyui/themes/icons/ok.png'></img>";
    } else {
        str = "<img src='/Scripts/easyui/themes/icons/no.png'></img>";
    }
    return str;
}

//显示状态样式
//True-打勾，False-打叉,其他-空白
function formatState2(value) {
    if (value == null) value = '';
    var str = "";
    if (value.toLowerCase() == "true" || value == "1") {
        str = "<img src='/Scripts/easyui/themes/icons/ok.png'></img>";

    } else if (value.toLowerCase() == "false" || value == "0") {
        str = "<img src='/Scripts/easyui/themes/icons/no.png'></img>";

    }
    return str;
}
//显示状态样式_相反
//True-打叉，False-打勾,其他-空白
function formatState2Ex(value) {
    if (value == null) value = '';
    var str = "";
    if (value.toLowerCase() == "true" || value == "1") {
        str = "<img src='/Scripts/easyui/themes/icons/no.png'></img>";
    } else if (value.toLowerCase() == "false" || value == "0") {
        str = "<img src='/Scripts/easyui/themes/icons/ok.png'></img>";
    }
    return str;
}
//显示任务状态样式
//end-打勾，其他-打叉
function formatStatetask(value) {
    if (value == null) value = '';
    var str = "";
    if (value.toLowerCase() == "end" || value == "1") {
        str = "<img src='/Scripts/easyui/themes/icons/ok.png'></img>";

    } else {
        str = "<img src='/Scripts/easyui/themes/icons/no.png'></img>";

    }
    return str;
}

//格式化时间
function parseToDate(value) {
    if (value == null || value == '') {
        return undefined;
    }

    var dt;
    if (value instanceof Date) {
        dt = value;
    }
    else {
        if (!isNaN(value)) {
            dt = new Date(value);
        }
        else if (value.indexOf('/Date') > -1) {
            value = value.replace(/\/Date\((-?\d+)\)\//, '$1');
            dt = new Date();
            dt.setTime(value);
        } else if (value.indexOf('/') > -1) {
            dt = new Date(Date.parse(value.replace(/-/g, '/')));
            //dt = newDateAndTime(Date.parse(value.replace(/-/g, '/')));
        } else {
            //dt = new Date(value);
            dt = newDateAndTime(value);
        }
    }
    return dt;
}
function newDateAndTime(dateStr) {
    var r = new Date();
    var ds = dateStr.split(" ")[0].split("-");
    r.setFullYear(ds[0], ds[1] - 1, ds[2]);
    if (dateStr.split(" ").length > 1) {
        var ts = dateStr.split(" ")[1].split(":");
        r.setHours(ts[0], ts[1], ts[2], 0);
    }
    return r;
}

//为Date类型拓展一个format方法，用于格式化日期
Date.prototype.format = function (format) //author: meizz 
{
    var o = {
        "M+": this.getMonth() + 1, //month 
        "d+": this.getDate(),    //day 
        "h+": this.getHours(),   //hour 
        "m+": this.getMinutes(), //minute 
        "s+": this.getSeconds(), //cond 
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter 
        "S": this.getMilliseconds() //millisecond 
    };
    if (/(y+)/.test(format))
        format = format.replace(RegExp.$1,
                (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(format))
            format = format.replace(RegExp.$1,
                    RegExp.$1.length == 1 ? o[k] :
                        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
};


//格式化日期时间
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "H+": this.getHours(), //小时 
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
Date.prototype.addDays = function (d) {
    this.setDate(this.getDate() + d);
    return this;
};
Date.prototype.addWeeks = function (w) {
    this.addDays(w * 7);
    return this;
};
Date.prototype.addMonths = function (m) {
    var d = this.getDate();
    this.setMonth(this.getMonth() + m);
    if (this.getDate() < d)
        this.setDate(0);
    return this;
};
Date.prototype.addYears = function (y) {
    var m = this.getMonth();
    this.setFullYear(this.getFullYear() + y);
    if (m < this.getMonth()) {
        this.setDate(0);
    }
    return this;
};



//根据name属性值获取checkbox的Value
function CheckboxRadio_getValuesByName(name) {
    var arrBox = $("[name='" + name + "']");
    var value = [];
    $(arrBox).each(function (i, item) {
        if (item.checked) {
            value.push($(item).attr('value'));
        }
    });
    return value;
}
//根据name属性值获取checkbox的Text
function CheckboxRadio_getAttrByName(name, attr) {
    var arrBox = $("[name='" + name + "']");
    var value = [];
    $(arrBox).each(function (i, item) {
        if (item.checked) {
            value.push($(item).attr(attr));
        }
    });
    return value;
}
//根据name属性和arrValue设置checkbox的选中状态
function CheckboxRadio_setValueByName(name, arrValue) {
    var arrBox = $("[name='" + name + "']");
    $(arrBox).each(function (i, item) {
        if ($.inArray($(item).attr('value'), arrValue) >= 0)
            item.checked = true;
        else
            item.checked = false;
    });
}


//文件大小单位转换
//size:文件大小
//unit:单位，byte,kb,mb,gb,tb
function changeUnit(size, unit) {
    if (unit == null || unit == '') {
        unit = "byte";
    }
    unit = unit.toUpperCase();

    var result_Size = size;
    var result_Unit = unit;

    switch (unit) {
        case "BYTE":
            if (result_Size >= 1024) {
                result_Size = result_Size / 1024;
                result_Unit = "KB";
            }
        case "KB":
            if (result_Size >= 1024) {
                result_Size = result_Size / 1024;
                result_Unit = "MB";
            }
        case "MB":
            if (result_Size >= 1024) {
                result_Size = result_Size / 1024;
                result_Unit = "GB";
            }
        case "GB":
            if (result_Size >= 1024) {
                result_Size = result_Size / 1024;
                result_Unit = "TB";
            }
    }
    if (result_Unit == "BYTE") result_Unit = "字节";
    if (result_Unit != "字节") {
        result_Size = parseFloat(result_Size).toFixed(2);
    }
    return result_Size + result_Unit;
}


//传入时间字符串进行时间对比
//字符串格式有误无法对比或者时间相等返回0
//前者大返回1
//后者大返回-1
function CompareDate(strStartDate, strEndDate) {
    var startTime = parseToDate(strStartDate);
    var endTime = parseToDate(strEndDate);
    if (startTime == null || endTime == null || startTime == undefined || endTime == undefined) {
        return 0;
    }
    var timeDis = startTime - endTime;
    if (timeDis == 0) {
        return 0;
    }
    else if (timeDis > 0) {
        return 1;
    }
    else {
        return -1;
    }
}

//传入2个数值字符串
//字符串格式有误无法对比或者值相等返回0
//前者大返回1
//后者大返回-1
function CompareNumber(strFirst, strSecond) {
    if (strFirst == null || strFirst == undefined || strFirst == "" || strSecond == null || strSecond == undefined || strSecond == "") {
        return 0;
    }
    var noFirst = Number(strFirst);
    var noSecond = Number(strSecond);
    if (isNaN(noFirst) || isNaN(strSecond) || noFirst == undefined || strSecond == undefined) {
        return 0;
    }
    if (noFirst == noSecond) {
        return 0;
    }
    else if (noFirst > noSecond) {
        return 1;
    }
    else {
        return -1;
    }
}