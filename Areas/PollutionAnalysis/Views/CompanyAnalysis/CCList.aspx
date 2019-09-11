<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style type="text/css">
        #toolbar {
            height: 32px !important;
            padding: 2px !important;
        }

        .timeRang {
            display: none;
        }

        .timeRang1 {
            display: none;
        }
    </style>
    <script type="text/javascript">
        function urlParam() {
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
                    //for (item in paramArray) {

                    //}
                }
                //if (paramStr) {
                //    paramStr = paramStr.substr(1, paramStr.length - 1);
                //}
                return paramStr;
            }
        }
        $(function () {

            mylayout.init();
            debugger;
            fieldInit();
            var model = new listModel();
            $("#userTab").tabs({
                onSelect: function (title, index) {
                    if (index == 0) {

                    }

                    if (index == 1) {

                    }



                }
            });

            //// 基于准备好的dom，初始化echarts实例
            //var myChart = echarts.init(document.getElementById('main'));

            //// 指定图表的配置项和数据
            //var option = {
            //    title: {
            //        text: '区域分析'
            //    },
            //    tooltip: {},
            //    legend: {
            //        data: ['销量']
            //    },
            //    xAxis: {
            //        data: ["中原区", "二七区", "管城区", "金水区", "上街区", "惠济区"]
            //    },
            //    yAxis: {},
            //    series: [{
            //        name: 'co',
            //        type: 'bar',
            //        data: [5, 20, 36, 10, 10, 20]
            //    }]
            //};

            //// 使用刚指定的配置项和数据显示图表。
            //myChart.setOption(option);




        });

        var mylayout = {
            init: function () {
                var size = { width: $(window).width(), height: $(window).height() };
                $("#layout").width(size.width - 4).height(size.height - 4).layout();
                var center = $("#layout").layout("panel", "center");
                center.panel({
                    onResize: function (w, h) {
                        $("#dg").datagrid("resize", { width: w - 3, height: 570 });
                    }
                });
            }

        };

        //$(document).ready(function () {

        //});

        function GetChart(s1, s2, s3, s4, s5, s6) {


            var param = "";
            param += "Companyid=" + s1;
            param += "&startTime=" + s2;
            param += "&endTime=" + s3;
            param += "&selectData=" + s4;
            param += "&startTime1=" + s5;
            param += "&endTime1=" + s6;

            $.ajax({
                type: "post",
                url: "/PollutionAnalysis/CompanyAnalysis/getChartInfo?" + param,

                dataType: "json", //返回数据形式为json
                success: function (result) {
                    var div = document.getElementById('main');
                    myChart = echarts.init(div)
                    myChart.setOption(eval('(' + result + ')'));
                }
            });
        }

        function AfterLoadSuccess() {

        }


        function closePopWin() {
            window.document.getElementsByClassName("panel-tool-close")[2].click();
            self.frames["taskHeadDetailIF"].initData();
        }


        var fieldInit = function () {
            var year = function () {
                var resultData = new Array();
                var currentYear = new Date().getFullYear()
                for (i = 0; i < 10; i++) {
                    resultData.push({ CODE: currentYear - i, TITLE: currentYear - i + "年" });
                }
                return resultData;
            }

            $("#reportType").combobox({
                //url: "/Bas/Dic/GetDicCodeData?typeCode=IndustryType",
                valueField: "CODE",
                textField: "TITLE",
                required: false,
                editable: false,
                panelHeight: "auto",
                data: [{ CODE: "hour", TITLE: "小时数据" }, { CODE: "day", TITLE: "日均数据" }],
                onChange: function (newValue, oldValue) {
                    debugger;
                    $(".timeRang").each(function (self) {
                        $(this).hide();
                    });
                    $("#" + newValue + "Div").show();
                }
                ,
                onLoadSuccess: function () { //加载完成后,val[0]写死设置选中第一项

                    

                    $("#reportType").combobox("setValue", "day");
                    $("#" + "day" + "Div").show();
                }

            });

            $("#reportType2").combobox({
                //url: "/Bas/Dic/GetDicCodeData?typeCode=IndustryType",
                valueField: "CODE",
                textField: "TITLE",
                required: false,
                editable: false,
                panelHeight: "auto",
                data: [{ CODE: "hour1", TITLE: "小时数据" }, { CODE: "day1", TITLE: "日均数据" }],
                onChange: function (newValue, oldValue) {
                    debugger;
                    $(".timeRang1").each(function (self) {
                        $(this).hide();
                    });
                    $("#" + newValue + "Div").show();
                }
                 ,
                onLoadSuccess: function () { //加载完成后,val[0]写死设置选中第一项



                    $("#reportType2").combobox("setValue", "day1");
                    $("#" + "day1" + "Div").show();
                }
            });

            var currentDate = new Date();
            date1 = currentDate.getFullYear() + "\-" + (currentDate.getMonth() + 1) + "\-" + (currentDate.getDate()-1);
            date = currentDate.getFullYear() + "\-" + (currentDate.getMonth() + 1) + "\-" + currentDate.getDate();

            $("#stimeRangDay").datebox().datebox('setValue',  date1);
            $("#etimeRangDay").datebox().datebox('setValue', date);
            $("#stimeRangDay1").datebox().datebox('setValue', date1);;
            $("#etimeRangDay1").datebox().datebox('setValue', date);;

            $("#timeRangYear").combobox({
                valueField: "CODE",
                textField: "TITLE",
                required: false,
                editable: false,
                panelHeight: "auto",
                data: year()
            });
            $("#timeRangMonth").combobox({
                valueField: "CODE",
                textField: "TITLE",
                required: false,
                editable: false,
                panelHeight: "auto",
                data: [
                    { CODE: "1", TITLE: "1月" }, { CODE: "2", TITLE: "3月" }, { CODE: "3", TITLE: "3月" }, { CODE: "4", TITLE: "4月" }, { CODE: "5", TITLE: "5月" }, { CODE: "6", TITLE: "6月" },
                    { CODE: "7", TITLE: "7月" }, { CODE: "8", TITLE: "8月" }, { CODE: "9", TITLE: "9月" }, { CODE: "10", TITLE: "10月" }, { CODE: "11", TITLE: "11月" }, { CODE: "12", TITLE: "12月" }]
            });
            $("#timeRangOnlyYear").combobox({
                valueField: "CODE",
                textField: "TITLE",
                required: false,
                editable: false,
                panelHeight: "auto",
                data: year()
            });

            $("#timeRangStart").datetimebox({ showSeconds: false });
            $("#timeRangEnd").datetimebox({ showSeconds: false });

            $("#timeRangStart1").datetimebox({ showSeconds: false });
            $("#timeRangEnd1").datetimebox({ showSeconds: false });
            $("#btnCreate").click(function () {
                debugger;
                var companyInfo = GetTreeSelected();
                if (!companyInfo) {
                    alert("请选择企业");
                    return;
                }

                if (!$("#reportType").combobox('getValue')) {
                    com.msg.alert("请选择数据来源");
                    return;
                }
                var s1;
                var s2;
                var s3;
                var s4;

                var reportType = $("#reportType").combobox('getValue');
                if ("hour" == reportType) {
                    s1 = $("#timeRangStart").datebox('getValue');
                    s2 = $("#timeRangEnd").datebox('getValue');


                    if (s1 && s2) {
                        getData(companyInfo.ID, s1, s2, reportType);

                    } else {
                        com.msg.alert("请选择时间");
                    }
                }
                if ("day" == reportType) {
                    s1 = $("#stimeRangDay").datebox('getValue')
                    s2 = $("#etimeRangDay").datebox('getValue')
                    if (s1 && s2) {
                        getData(companyInfo.ID, s1, s2, reportType);

                    } else {
                        com.msg.alert("请选择时间");
                    }
                }

                var reportType2 = $("#reportType2").combobox('getValue');
                if ("hour1" == reportType2) {
                    s3 = $("#timeRangStart1").datebox('getValue');
                    s4 = $("#timeRangEnd1").datebox('getValue');


                    if (s3 && s4) {

                    } else {
                        com.msg.alert("请选择时间");
                    }
                }
                if ("day1" == reportType2) {
                    s3 = $("#stimeRangDay1").datebox('getValue')
                    s4 = $("#etimeRangDay1").datebox('getValue')
                    if (s3 && s4) {

                    } else {
                        com.msg.alert("请选择时间");
                    }
                }

                if (s1 && s2 && s3 && s4) {
                    GetChart(companyInfo.ID, s1, s2, reportType, s3, s4);
                } else {
                    com.msg.alert("请选择时间");
                }



            });

        }
        var beforeCommit = function () {
            alert("添加表单数据");
            var data = {
                dataFrom: $("#dataFrom").val()
            };

            $("#qureyData").val(JSON.stringify(data));
            return true;
        }

        function getSelected() {
            var selObj = GetTreeSelected();
            alert(selObj.TYPE + "," + selObj.ID);
        }
        function CompanySelected(id) {

        }
        function PKSelected(id) {


        }



        //初始化企业信息列表
        function getData(s1, s2, s3, s4) {
            var param = "";
            param += "Companyid=" + s1;
            param += "&startTime=" + s2;
            param += "&endTime=" + s3;
            param += "&selectData=" + s4;

            $("#dg").datagrid({
                url: "/PollutionAnalysis/CompanyAnalysis/getDataInfo?" + param,
                idField: "CODE",
                pagination: true,
                pageSize: 100,
                singleSelect: true,
                height: 350,
                width: 800,
                striped: true,
                rownumbers: true,
                columns: [[
                    { title: "监测时间", field: "DATA_TIME", width: 150 },
                    { title: "烟尘", field: "a34013_PFL_Value", width: 150 },
                    { title: "二氧化硫", field: "a21026_PFL_Value", width: 250 },
                    { title: "氮氧化物", field: "a21002_PFL_Value", width: 150 },
                    { title: "废气流量", field: "flowgas_Value", width: 150 },
                    { title: "含氧量", field: "a19001_Value", width: 150 },

                    { title: "烟气温度", field: "a01012_Value", width: 150 },
                    { title: "烟气压力", field: "a01013_Value", width: 150 },
                    { title: "化学需氧量", field: "w01018_PFL_Value", width: 250 },
                    { title: "氨氮", field: "w21003_PFL_Value", width: 150 },
                    { title: "废水流量", field: "flowwater_Value", width: 150 }


                ]]
            });
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">

    <div id="layout">


        <div region="west" split="true" style="width: 270px; padding: 5px">
            <!--180-->
            <%
                ViewDataDictionary dict = new ViewDataDictionary();
                dict.Add("fs", ViewData["fs"]);
                dict.Add("fq", ViewData["fq"]);
                dict.Add("vocs", ViewData["vocs"]);
                dict.Add("selectId", ViewData["PKId"]);
                dict.Add("companyOnly", ViewData["companyOnly"]);
                dict.Add("multiSelect", false);
                Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = dict });
            %>
        </div>

        <div region="center" title="企业分析" iconcls="icon-edit">
            <!--顶部工具栏-->
            <div id="toolbar">
                <%--<form id="qform">
                    <div style="float: left; padding: 5px;">
                        企业名称：
                        <input id="CompanyName" name="CompanyName" style="width: 120px;" />

                    </div>
                </form>--%>


                <form id="qform">
                    <div style="float: left; padding: 5px;"></div>
                    <table>
                        <tr>

                            <td><span>数据源：</span><select id="reportType" name="reportType" class="txt03 inputSize" style="width: 90px;"></select></td>
                            <td>
                                <div class="timeRang" id="dayDiv">
                                    <input id="stimeRangDay" name="stimeRangDay" type="text" class="txt03 inputSize" style="width: 100px;" />至
                                    <input id="etimeRangDay" name="etimeRangDay" type="text" class="txt03 inputSize" style="width: 100px;" />

                                </div>
                                <div class="timeRang" id="monthDiv">
                                    <select id="timeRangYear" name="timeRangYear" class="txt03 inputSize"></select>
                                    <select id="timeRangMonth" name="timeRangMonth" class="txt03 inputSize"></select>
                                </div>
                                <div class="timeRang" id="yearDiv">
                                    <select id="timeRangOnlyYear" name="timeRangOnlyYear" class="txt03 inputSize"></select>
                                </div>
                                <div class="timeRang" id="hourDiv">
                                    <input id="timeRangStart" name="timeRangStart" type="text" class="txt03 inputSize" style="width: 130px;" />至
                                <input id="timeRangEnd" name="timeRangEnd" type="text" class="txt03 inputSize" style="width: 130px;" />
                                </div>
                            </td>
                            <td>
                                <button id="btnCreate" type="button">生成</button></td>
                            <%-- <td>
                                <button id="btnCreate1" type="button">生成数据</button></td>
                            <td>
                                <button id="btnCreate2" type="button">导出数据</button></td>--%>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                </div>
                            </td>
                        </tr>
                    </table>
                </form>
                <%=Html.ToolBar(Model) %>
            </div>
            <!--查询表单-->
            <div style="margin: 2px;">

                <table>
                    <tr>

                        <td><span>基准对照：</span><select id="reportType2" name="reportType2" class="txt03 inputSize" style="width: 90px;"></select></td>
                        <td>
                            <div class="timeRang1" id="day1Div">
                                <input id="stimeRangDay1" name="stimeRangDay1" type="text" class="txt03 inputSize" style="width: 100px;" />至
                                    <input id="etimeRangDay1" name="etimeRangDay1" type="text" class="txt03 inputSize" style="width: 100px;" />

                            </div>

                            <div class="timeRang1" id="hour1Div">
                                <input id="timeRangStart1" name="timeRangStart1" type="text" class="txt03 inputSize" style="width: 130px;" />至
                                <input id="timeRangEnd1" name="timeRangEnd1" type="text" class="txt03 inputSize" style="width: 130px;" />
                            </div>
                        </td>


                    </tr>

                </table>

            </div>
            <!--数据列表-->
            <div id="userTab" fit="true" style="height: 540px; overflow: hidden;">

                <div title="数据" style="padding: 2px; overflow: hidden;">

                    <table id="dg"></table>
                </div>

                <div title="图表" style="padding: 2px">
                    <div id="main" style="width: 600px; height: 350px;"></div>
                </div>


            </div>

            <div id="tab-tools">
                <input id="CompanyName" name="CompanyName" style="width: 120px;" />
            </div>
        </div>
    </div>
</asp:Content>
