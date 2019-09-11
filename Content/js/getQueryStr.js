//获取URL参数
function getQueryStr(sArgName) {
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