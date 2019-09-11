$.fn.validate = function () {
    $(this).find("[validate]").each(function () {
        if ($(this).attr("validate") == "date") {
            var beginTime = $(this).find(".datebox:eq(0) .combo-value").val();
            var endTime = $(this).find(".datebox:eq(1) .combo-value").val();
            if (beginTime == "")
                $(this).find(".datebox:eq(0) .combo-value").val(endTime);
            else if (endTime == "")
                $(this).find(".datebox:eq(1) .combo-value").val(beginTime);
            //开始时间大于结束时间，将开始时间重置为结束时间
            if (new Date(beginTime.replace("-", "/").replace("-", "/")) > new Date(endTime.replace("-", "/").replace("-", "/"))) {
                $(this).find(".datebox:eq(0) .combo-value").val(endTime);
                $(this).find(".datebox:eq(0) .validatebox-text").val(endTime);
                
            }
        }
    });
}