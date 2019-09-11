<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        #toolbar {
            height: 60px !important;
            padding: 2px !important;
        }
        .timeRang{
            display:none;
        }
    </style>
    <script>
        var compamyType = <%=ViewData["compamyType"]%> 
        $(function () {
            mylayout.init();
            fieldInit();
        });
        //页面字段初始化--下拉框
        var fieldInit = function () {
            var curDate = new Date();
            var preDate = new Date(curDate.getTime() - 24 * 60 * 60 * 1000); //前一天
            var preDate2 = new Date(curDate.getTime() - 24 * 60 * 60 * 2 * 1000); //前一天
            $("#dataFrom").combobox({
                valueField: 'id',
                textField: 'text',
                data: [{ id: "hour", text: "小时数据", "selected": true}, { id: "day", text: "日均数据" }],
                onChange: function (newValue, oldValue) {
                    debugger;
                    if ("hour" == newValue) {
                        $('#timeRangStart').datetimebox({ formatter: dateFormatter });
                        $("#timeRangEnd").datetimebox({ formatter: dateFormatter });
                        $("#dailyStart").datetimebox({ formatter: dateFormatter });
                        $("#dailyEnd").datetimebox({ formatter: dateFormatter });

                    }
                    if ("day" == newValue) {
                        $('#timeRangStart').datetimebox({ formatter: dateFormatterNoHour });
                        $("#timeRangEnd").datetimebox({ formatter: dateFormatterNoHour });
                        $("#dailyStart").datetimebox({ formatter: dateFormatterNoHour });
                        $("#dailyEnd").datetimebox({ formatter: dateFormatterNoHour });
                    }
                    $('#timeRangStart').datetimebox('setValue', com.DateFormat(preDate));
                    $("#timeRangEnd").datetimebox('setValue', com.DateFormat(curDate));
                    $("#dailyStart").datetimebox('setValue', com.DateFormat(preDate2));
                    $("#dailyEnd").datetimebox('setValue', com.DateFormat(preDate));
                }
            });

            //var nextDate = new Date(curDate.getTime() + 24*60*60*1000); //后一天
            debugger;
            var dateFormatter = function (date) {
                var y = date.getFullYear();
                var m = date.getMonth() + 1;
                var d = date.getDate();
                var h = date.getHours();
                return y + '-' + m + '-' + d + " " + h + ":00:00";
            }
            var dateFormatterNoHour = function (date) {
                var y = date.getFullYear();
                var m = date.getMonth() + 1;
                var d = date.getDate();
                var h = date.getHours();
                return y + '-' + m + '-' + d ;
            }
            $('#timeRangStart').datetimebox({ formatter: dateFormatter });
            $("#timeRangEnd").datetimebox({ formatter: dateFormatter });
            $("#dailyStart").datetimebox({ formatter: dateFormatter });
            $("#dailyEnd").datetimebox({ formatter: dateFormatter });
            $('#timeRangStart').datetimebox('setValue', com.DateFormat(preDate));
            $("#timeRangEnd").datetimebox('setValue', com.DateFormat(curDate));
            $("#dailyStart").datetimebox('setValue', com.DateFormat(preDate2));
            $("#dailyEnd").datetimebox('setValue', com.DateFormat(preDate));
            
            

            $("#btnCreate").click(function () {
                debugger;
                var companyInfo = GetTreeSelected();
                if (!companyInfo) {
                    com.msg.alert("请先选择企业或排口");
                    return;
                }
                var dataFrom = $("#dataFrom").combobox('getValue');
                var startTimeMonitor = $('#timeRangStart').datetimebox('getValue');
                var endTimeMonitor =$('#timeRangEnd').datetimebox('getValue');
                var startTimeAvg = $("#dailyStart").datetimebox('getValue');
                var endTimeAvg = $("#dailyEnd").datetimebox('getValue');
                //限制时间跨度----开始
                if("hour" == dataFrom){
                    if (new Date(startTimeMonitor.replace("-", "/")).addDays(2) < new Date(endTimeMonitor.replace("-", "/"))) {
                        com.msg.alert("数据来源的时间跨度不能超过2天");
                        return;
                    }
                    if (new Date(startTimeAvg.replace("-", "/")).addDays(2) < new Date(endTimeAvg.replace("-", "/"))) {
                        com.msg.alert("日常数据的时间跨度不能超过2天");
                        return;
                    }
                }
                else if ("day" == dataFrom) {
                    if (new Date(startTimeMonitor.replace("-", "/")).addDays(60) < new Date(endTimeMonitor.replace("-", "/"))) {
                        com.msg.alert("数据来源的时间跨度不能超过60天");
                        return;
                    }
                    if (new Date(startTimeAvg.replace("-", "/")).addDays(60) < new Date(endTimeAvg.replace("-", "/"))) {
                        com.msg.alert("日常数据的时间跨度不能超过60天");
                        return;
                    }
                }
                //限制时间跨度----开始
                $.ajax({
                    type: "post",
                    url: "/ThePeakManager/Analysis/DataAnalysis",
                    data: {
                        type:companyInfo.TYPE,
                        id: companyInfo.ID,
                        startTimeMonitor: startTimeMonitor,
                        endTimeMonitor: endTimeMonitor,
                        startTimeAvg: startTimeAvg,
                        endTimeAvg: endTimeAvg,
                        dataFrom: dataFrom,
                        compamyType:compamyType
                    },
                    timeout: 30000,
                    dataType: "json", //返回数据形式为json
                    success: function (result) {
                        if (!result) return;
                        var title = result.TITLE;
                        var dataList = result.DATA;

                        var col = new Array(); //主列
                        var subCol = new Array(); //子列
                        col.push({
                            field: 'DATA_TIME', title: '时间', align: 'center', rowspan: 2
                        });
                        title.MAINTITLE.forEach(function (item, index) {
                            if (item.COLSPAN > 1)
                                col.push({ field: item.ITEMCODE, title: item.ITEMTEXT, align: 'center', colspan: item.COLSPAN });
                            else
                                col.push({
                                    field: item.ITEMCODE, title: item.ITEMTEXT + '(' + item.UNIT + ')', align: 'center', rowspan: 2
                                });
                        });
                        title.SUBTITLE.forEach(function (item, index) {
                            subCol.push({
                                field: item.ITEMCODE, title: item.ITEMTEXT + '(' + item.UNIT + ')', align: 'center', styler: function(value,row,index){
                                    //限值的获取有待确定
                                    if (item.ITEMCODE.indexOf("_PFL_Value_Percent") > 0 && parseFloat(value) < result.LimitValuePercent) {
                                        return 'color:red;';
                                    }
                                }
                            });
                        });
                        $("#dg").datagrid({
                            singleSelect: true, //选中一行的设置
                            rownumbers: true, //行号
                            columns: [col, subCol],
                            pagination: false,
                            //fitColumns: true,
                            //fit: true,
                            //minheight:200,
                            onClickCell: function (index, field, value) {
                            }
                        }).datagrid('loadData', dataList);
                        mylayout.init();
                        //-------------图表-----------------------
                        var div = document.getElementById('chart');
                        myChart = echarts.init(div)
                        myChart.setOption(eval('(' + result.EChartData + ')'));
                    },
                    error: function (e) {
                        com.msg.alert("请求数据时发生错误!");
                    }
                });
            });
            $("#btnExport").click(function () {
                debugger;
                var companyInfo = GetTreeSelected();
                if (!companyInfo) {
                    com.msg.alert("请先选择企业或排口");
                    return;
                }
                var dataFrom = $("#dataFrom").combobox('getValue');
                var startTimeMonitor = $('#timeRangStart').datetimebox('getValue');
                var endTimeMonitor = $('#timeRangEnd').datetimebox('getValue');
                var startTimeAvg = $("#dailyStart").datetimebox('getValue');
                var endTimeAvg = $("#dailyEnd").datetimebox('getValue');
                var url = "/ThePeakManager/Analysis/ExportExcel";
                var params = [
                    { key: "type", value: companyInfo.TYPE },
                    { key: "id", value: companyInfo.ID },
                    { key: "startTimeMonitor", value: startTimeMonitor },
                    { key: "endTimeMonitor", value: endTimeMonitor },
                    { key: "startTimeAvg", value: startTimeAvg },
                    { key: "endTimeAvg", value: endTimeAvg },
                    { key: "dataFrom", value: dataFrom },
                    { key: "compamyType", value: compamyType }
                ];
                com.fileDownLoad(url, params);
            });
        }
        var mylayout = {
            init: function () {
                var size = { width: $(window).width(), height: $(window).height() };
                $("#layout").width(size.width - 4).height(size.height - 4).layout();
                var center = $("#layout").layout("panel", "center");
                center.panel({
                    onResize: function (w, h) {
                        $("#dg").datagrid("resize", { width: w - 10, height: h - 140 });
                    }
                });
            }
            ,
            resize: function () {
                mylayout.init();
                $("#layout").layout("resize");
            }
        };
    </script>
    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div id="layout">
    <div region="west" split="true" style="width: 270px; padding: 5px">
       <%
            Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = ViewBag.controllerParam});
       %>
    </div>
    <div region="center" title="停产限产分析" iconcls="icon-house">
        <!--顶部工具栏-->
        <div id="toolbar">
            <form id="qform">
                <div style="float: left; padding: 5px;"></div>
                <table>
                    <tr>
                        <td><span>数据来源：</span><select id="dataFrom" name="dataFrom" class="txt03 inputSize"></select></td>
                        <td>
                            <div id="hourDiv" style="display:normal">
                                <input id="timeRangStart" name="timeRangStart" type="text" class="txt03 inputSize" />至
                                <input id="timeRangEnd" name="timeRangEnd" type="text" class="txt03 inputSize" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td><span>日常数据：</span></td>
                        <td>
                            <input id="dailyStart" name="dailyStart" type="text" class="txt03 inputSize" />至
                            <input id="dailyEnd" name="dailyStart" type="text" class="txt03 inputSize" />
                        </td>
                        <td><button id="btnCreate" type="button">生成</button>&nbsp;<button id="btnExport" type="button">导出</button></td>
                    </tr>
                </table>
            </form>
        </div>
        <div id="tt" class="easyui-tabs" >
            <div title="数据" >
                 <table id="dg"></table>
            </div>
            <div title="图表">
                <div id="chart" style="width: 800px; height: 450px"></div>
            </div>
        </div>   

    </div>
</div>
</asp:Content>

