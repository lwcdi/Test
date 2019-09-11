//报表页面js
var rdlc = rdlc || {};

rdlc.year = function () {
    var resultData = new Array();
    var currentYear = new Date().getFullYear()
    for (i = 0; i < 10; i++) {
        resultData.push({ CODE: currentYear - i, TITLE: currentYear - i + "年" });
    }
    return resultData;
}
//年份下拉
rdlc.yearCombobox = function (id, option) {
    var opts = {
        valueField: "CODE",
        textField: "TITLE",
        required: false,
        editable: false,
        panelHeight: "auto",
        data: rdlc.year()
    };
    opts = $.extend(opts, option || {});
    $("#" + id).combobox(opts);
}
//月份下拉
rdlc.monthCombobox = function (id, option) {
    var opts = {
        valueField: "CODE",
        textField: "TITLE",
        required: false,
        editable: false,
        panelHeight: "auto",
        data: [
            { CODE: "1", TITLE: "1月" }, { CODE: "2", TITLE: "2月" }, { CODE: "3", TITLE: "3月" }, { CODE: "4", TITLE: "4月" }, { CODE: "5", TITLE: "5月" }, { CODE: "6", TITLE: "6月" },
            { CODE: "7", TITLE: "7月" }, { CODE: "8", TITLE: "8月" }, { CODE: "9", TITLE: "9月" }, { CODE: "10", TITLE: "10月" }, { CODE: "11", TITLE: "11月" }, { CODE: "12", TITLE: "12月" }]
    };
    opts = $.extend(opts, option || {});
    $("#"+id).combobox(opts);
}
//季度下拉
rdlc.quarterCombobox = function (id, option) {
    var opts = {
        valueField: "CODE",
        textField: "TITLE",
        required: false,
        editable: false,
        panelHeight: "auto",
        data: [
            { CODE: "1", TITLE: "第1季度" }, { CODE: "2", TITLE: "第2季度" }, { CODE: "3", TITLE: "第3季度" }, { CODE: "4", TITLE: "第4季度" }]
    };
    opts = $.extend(opts, option || {});
    $("#" + id).combobox(opts);
}