$(function () {
    var divHeight = $(window.parent.document).find(".tabs-panels").height();
    $(".ViewCommon").height(divHeight * 0.6);
    $(".View_Content div").height(divHeight * 0.5);
    $(window).resize(function () {
        var divHeight = $(window.parent.document).find(".tabs-panels").height();
        $(".ViewCommon").height(divHeight * 0.6);
        $(".View_Content div").height(divHeight * 0.5);
    })
});

// 事件绑定
function addEvent(element, eType, handle) {
    if (element.addEventListener) {           //如果支持addEventListener
        element.addEventListener(eType, handle);
    } else if (element.attachEvent) {          //如果支持attachEvent
        element.attachEvent("on" + eType, handle);
    } else {                                  //否则使用兼容的onclick绑定
        element["on" + eType] = handle;
    }
}