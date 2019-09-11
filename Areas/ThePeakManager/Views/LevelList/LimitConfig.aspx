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
                <td><select id="LIMIT_TYPE" name="LIMIT_TYPE"  class="txt03 inputSize" /></td>
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
        </table>
        <table id="dgLines"></table>
        <div class="lineSpace">
            <input type="checkbox" id="LIMIT_START" />
            <span>排污上限比例：</span>
            <input id="LIMIT_RATIO" name="LIMIT_RATIO" class="txt03 inputSize easyui-numberbox"  data-options="min:0,precision:2,max:100,suffix:'%'" />
        </div>
        <table id="dglimitRatio"></table>
        <div class="lineSpace">
            <input type="checkbox" id="ENERGY_START" />
            <span>能耗上限比例：</span>
            <input id="ENERGY_RATIO" name="ENERGY_RATIO" class="txt03 inputSize easyui-numberbox"  data-options="min:0,precision:2,max:100,suffix:'%'" />
        </div>
        <table id="dgEnergy"></table>
        <div>
            <input type="checkbox" id="IS_POWER_OFF" />
            <span>启用超限关停</span>
        </div>
        <input type="hidden" id="companyID" value="<%=ViewData["companyID"] %>" />
        <input type="hidden" id="baseConfig" value="" />
    </form>
</div>
<script>
    //页面初始化
    $('#dgLines').datagrid({
        url: '/BasicData/CompanyManage/productionLineList?id=' + $("#companyID").val(),
        idField: "ID",
        sortName: "ID",
        sortOrder: "desc",
        rownumbers: true,
        ////singleSelect: true,
        striped: true,
        height: 150,
        showFooter: true,
        columns: [[
            { field: "PRODUCTION_LINES_NAME", title: "生成线名称", width: 150 },
            { field: "CAPACITY", title: "产能", width: 100, hidden: true },
            { field: "ENERGY", title: "小时能耗", width: 100 },
            { field: "POLLUTION", title: "小时排污", width: 100 },
            { field: "STATE_TEXT", title: "状态", width: 100, hidden: true },
        ]],
        onDblClickRow: function (index, row) {
            //selectedCustomer(row);
        },
        onLoadSuccess: function (data) {
            addUpLines();
        }
    });
    $('#dglimitRatio').datagrid({
        url: '/BasicData/CompanyManage/EmissionsList?id=' + $("#companyID").val(),
        idField: "ID",
        sortName: "ID",
        sortOrder: "desc",
        rownumbers: true,
        singleSelect: true,
        striped: true,
        height: 150,
        columns: [[
            { field: "EMISSIONS_POLLUTION_ITEM_TEXT", title: "污染项", width: 150 },
            { field: "POLLUTION_ITEM_ALIAS", title: "简称", width: 150, hidden: true },
            { field: "EMISSIONS", title: "日限制排放量", width: 100 },
            { field: "LIMITEMISSIONS", title: "限制排放上限", width: 100 },
            { field: "EMISSIONS_UNIT_TEXT", title: "单位", width: 180 },
        ]],
        onDblClickRow: function (index, row) {

        },
        onLoadSuccess: function (data) {
            if (data) {
                calpullotinLimit();
            }
        }
    });
    $('#dgEnergy').datagrid({
        url: '/BasicData/CompanyManage/productionLineList?id=' + $("#companyID").val(),
        idField: "ID",
        sortName: "ID",
        sortOrder: "desc",
        rownumbers: true,
        singleSelect: true,
        striped: true,
        height: 150,
        showFooter: true,
        checkOnSelect: false,
        selectOnCheck: false,
        columns: [[
            {
                field: "ID", title: '<input id="energyAll" type="checkbox" />', width: 100, formatter: function (value, row, index) {
                    return '<input id="lineID' + row["ID"] + '" type="checkbox"  class="lineID"  data-lineid="' + row["ID"] + '" />';
                }
            },
            { field: "PRODUCTION_LINES_NAME", title: "生成线名称", width: 150 },
            { field: "ENERGY", title: "小时能耗", width: 100 },
            { field: "LIMITENERGY", title: "限制能耗上限", width: 100 },
            { field: "CAPACITY", title: "产能", width: 100, hidden: true },
            { field: "POLLUTION", title: "排污", width: 100, hidden: true },
            { field: "STATE_TEXT", title: "状态", width: 100, hidden: true },
        ]],
        onDblClickRow: function (index, row) {
            //selectedCustomer(row);
        },
        onLoadSuccess: function (data) {
            $("#energyAll").unbind();
            if (data) {
                calEnergyLimit();
                energySetAll();
                dgEnergyoriginal();
            }
        }
    });
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
    //生产线信息--统计
    var addUpLines = function () {
        var dgLines = top.$("#dgLines");
        var linesRow = dgLines.datagrid("getRows");
        var capacity = 0;
        var energy = 0;
        var pollution = 0;
        for (item in linesRow) {
            try {
                var temp = parseFloat(linesRow[item]["CAPACITY"]);
                if (isNaN(temp) == false) capacity += temp;

            } catch (e) { }
            try {
                var temp = parseFloat(linesRow[item]["ENERGY"]);
                if (isNaN(temp) == false) energy += temp;
            } catch (e) { }
            try {
                var temp = parseFloat(linesRow[item]["POLLUTION"]);
                if (isNaN(temp) == false) pollution += temp;
            } catch (e) { }
        }
        var footData = {
            PRODUCTION_LINES_NAME: "合计",
            CAPACITY: capacity,
            ENERGY: energy,
            POLLUTION: pollution
        };
        dgLines.datagrid("reloadFooter", [footData]);
    }
    $("#LIMIT_RATIO").numberbox({
        onChange: function (newValue, oldValue) {
            calpullotinLimit();
        }
    });
    $("#ENERGY_RATIO").numberbox({
        onChange: function (newValue, oldValue) {
            calEnergyLimit();
        }
    });
    var calEnergyLimit = function () {
        var dg = top.$("#dgEnergy");
        var rows = dg.datagrid("getRows");
        var limitNumber = $("#ENERGY_RATIO").numberbox('getValue');
        $.each(rows, function (i, row) {
           var check =  $("#lineID" + row["ID"]).is(":checked");
            if (limitNumber) {
                row.LIMITENERGY = limitNumber * row.ENERGY / 100;
            }
            else {
                row.LIMITENERGY = '';
            }
            dg.datagrid('updateRow', {
                index: i,
                row:row
             });
            if (check) {
                $("#lineID" + row["ID"]).attr("checked", 'true');
            }
        });
    }
    var calpullotinLimit = function () {
        var limitNumber = $("#LIMIT_RATIO").numberbox('getValue');
        var dg = top.$("#dglimitRatio");
        var rows = dg.datagrid("getRows");
        $.each(rows, function (i, row) {
            if (limitNumber) {
                row.LIMITEMISSIONS = limitNumber * row.EMISSIONS / 100;
            }
            else {
                row.LIMITEMISSIONS = '';
            }
            dg.datagrid('updateRow', {
                index: i,
                row:row
            });
        });
    }
    var energySetAll = function () {
        top.$("#energyAll").click(function () {
            var energyEle = top.$(this);
            if (energyEle.is(":checked")) {//全选
                top.$(".lineID").each(function (i, ele) {
                    top.$(ele).attr("checked", 'true');
                });
            } else {//反选
                top.$(".lineID").each(function (i, ele) {
                    top.$(ele).removeAttr("checked");;
                });
            }
        });
    }
    //默认的生产线勾选
    var dgEnergyoriginal = function () {
        debugger;
        var config = JSON.parse($("#baseConfig").val());
        if (config && config.LINE_ID) {
            var lineidArray = config.LINE_ID.split(",");
            $(".lineID").each(function (i, ele) {
                if ($.inArray($(ele).data("lineid").toString(), lineidArray) > -1) {
                    $(ele).attr("checked", 'true');
                }
            });
        }
    }
    var pageDefaultData = function () {
        var config = JSON.parse($("#baseConfig").val());
        $("#LIMIT_RATIO").numberbox('setValue', config.LIMIT_RATIO);
        $("#ENERGY_RATIO").numberbox('setValue', config.ENERGY_RATIO);
        $("#LIMIT_TYPE").combobox('setValue', config.LIMIT_TYPE);
        if ("1" == config.LIMIT_START) {
            $("#LIMIT_START").attr("checked", 'true');
        }
        if ("1" == config.ENERGY_START) {
            $("#ENERGY_START").attr("checked", 'true');
        }
        if ("1" == config.IS_POWER_OFF) {
            $("#IS_POWER_OFF").attr("checked", 'true');
        }
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
        config.ENERGY_RATIO = $("#ENERGY_RATIO").numberbox('getValue');
        config.LIMIT_TYPE = $("#LIMIT_TYPE").combobox('getValue');
        config.LIMIT_START = $("#LIMIT_START").is(":checked") ? "1" : "0";
        config.ENERGY_START = $("#ENERGY_START").is(":checked") ? "1" : "0";
        config.IS_POWER_OFF = $("#IS_POWER_OFF").is(":checked") ? "1" : "0";
        var lineID = "";
        $(".lineID").each(function (i, element) {
            var lineEle = $(element);
            if (lineEle.is(":checked")) {
                if (lineID != "") {
                    lineID+=","
                }
                lineID += lineEle.data("lineid");
            }
        });
        config.LINE_ID = lineID;
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