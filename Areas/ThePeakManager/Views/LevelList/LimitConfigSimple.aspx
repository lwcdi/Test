<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<style>
    .lineSpace{
        margin-top:8px;
        margin-bottom:5px;
    }
</style>
<div>
    <form id="limitConfig">
        <table class="grid">
            <tr>
                <td>限产类型</td>
                <td><select id="LIMIT_TYPE" name="LIMIT_TYPE"  class="txt03 inputSize" data-options="required:true" /></td>
            </tr>
            <tr>
                <td>限产时间</td>
                <td>
                    <div style="width:250px">
                        <div id="limitTime" style="display:none">
                            <input id="LIMIT_TIME_START" name="LIMIT_TIME_START" class="txt03 inputSize "   style="width:100px" />至
                            <input id="LIMIT_TIME_END" name="LIMIT_TIME_END" class="txt03 inputSize "  style="width:100px"  />
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                 <td>限产比例</td>
                <td><input id="LIMIT_RATIO" name="LIMIT_RATIO" class="txt03 inputSize easyui-numberbox" data-options="min:0,precision:2,max:100,suffix:'%',required:true"  /></td>
            </tr>
        </table>
        <input type="hidden" id="companyID" value="<%=ViewData["companyID"] %>" />
        <input type="hidden" id="baseConfig" value="" />
    </form>
</div>
<script>
    //页面初始化
    var options = {
        required: true,
        onChange: function (newValue, oldValue) {
            if (newValue == "limitTime") {
                $("#limitTime").show();
            } else {
                $("#limitTime").hide();
            }
        }
    }
    options = $.extend(com.ui.combobox.defaultOptions(), options);
    com.comboxDicCreate("LIMIT_TYPE", "LimitType", options);
    var pageDefaultData = function () {
        var config = JSON.parse($("#baseConfig").val());
        $("#LIMIT_TYPE").combobox('setValue', config.LIMIT_TYPE);
        $("#LIMIT_RATIO").numberbox('setValue', config.LIMIT_RATIO);
        $("#LIMIT_TIME_START").timespinner({
            showSeconds: false,
            editable:false
        });
        $("#LIMIT_TIME_END").timespinner({
            showSeconds: false,
            editable: false
        });
        if (config.LIMIT_TIME_START) {
            $("#LIMIT_TIME_START").timespinner("setValue", config.LIMIT_TIME_START);
        } else {
            $("#LIMIT_TIME_START").timespinner("setValue", "00:00");
        }
        if (config.LIMIT_TIME_END) {
            $("#LIMIT_TIME_END").timespinner("setValue", config.LIMIT_TIME_END);
        } else {
            $("#LIMIT_TIME_END").timespinner("setValue", "24:00");
        }
    }
    var GetReturnData = function () {
        var config = JSON.parse($("#baseConfig").val());
        config.LIMIT_RATIO = $("#LIMIT_RATIO").numberbox('getValue');
        config.LIMIT_TYPE = $("#LIMIT_TYPE").combobox('getValue');
        if (config.COMPANY_ID) {
        } else {
            config.COMPANY_ID = $("#companyID").val();
        }
        config.LIMIT_CONFIG = 1;
        if (config.LIMIT_TYPE == "limitTime") {
            config.LIMIT_TIME_START = addzero($("#LIMIT_TIME_START").timespinner("getHours")) + ":" + addzero($("#LIMIT_TIME_START").timespinner("getMinutes"));
            config.LIMIT_TIME_END = addzero($("#LIMIT_TIME_END").timespinner("getHours")) + ":" + addzero($("#LIMIT_TIME_END").timespinner("getMinutes"));
        } else {
            config.LIMIT_TIME_START = "";
            config.LIMIT_TIME_END = "";
        }
        return config;
    }
    var addzero = function (nunmer) {
        if (nunmer < 10) {
            return "0"+"" + nunmer
        }
        return "" + nunmer;
    }
    setTimeout(pageDefaultData, 500);
</script>