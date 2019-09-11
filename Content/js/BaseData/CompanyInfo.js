var id; //企业ID
var showType; //展示类型 0-新增，1-编辑，2-查看
var isVOCs; //是否VOCs企业 0-否，1-是

$(function () {
    if (getQueryStr("id")) {
        id = getQueryStr("id");
    }
    if (getQueryStr("showType")) {
        showType = getQueryStr("showType");
    }
    if (getQueryStr("isVOCs")) {
        isVOCs = getQueryStr("isVOCs");
    }
});