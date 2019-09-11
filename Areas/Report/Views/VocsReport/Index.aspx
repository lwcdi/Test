<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        #toolbar {
            height: 32px !important;
            padding: 2px !important;
        }
        .timeRang{
            display:none;
        }
    </style>
    <script>
        $(function () {
            mylayout.init();
            fieldInit();
        });
        //页面字段初始化--下拉框
        var fieldInit = function () {
            com.comboxDicCreate("dataFrom", "DataFrom");
            com.comboxDicCreate("reportType", "ReportType", {
                onChange: function (newValue, oldValue) {
                    debugger;
                    $(".timeRang").each(function (self) {
                        $(this).hide();
                    });
                    $("#" + newValue + "Div").show();
                }
            });
            $("#timeRangDay").datebox();
            rdlc.yearCombobox("timeRangYear");
            rdlc.monthCombobox("timeRangMonth");
            rdlc.yearCombobox("timeRangOnlyYear");
            $("#timeRangStart").datetimebox({ showSeconds:true});
            $("#timeRangEnd").datetimebox({ showSeconds: true });
            $("#btnCreate").click(function () {
                debugger;
                var companyInfo = GetTreeSelected();
                if (!companyInfo || "P" != companyInfo.TYPE) {
                    com.msg.alert("请先选择排口");
                    return;
                }
                if (!$("#dataFrom").combobox('getValue') || !$("#reportType").combobox('getValue')) {
                    com.msg.alert("请选择数据来源和报表");
                    return;
                }
                var param = new com.url.createParam();
                param.addParam({ "dataFrom": $("#dataFrom").combobox('getValue') });
                param.addParam({ "id": companyInfo.ID });
                param.addParam({ "type": companyInfo.TYPE });
                var reportType = $("#reportType").combobox('getValue');
                if ("custom" == reportType) {
                    var timeRangStart = $("#timeRangStart").datebox('getValue');
                    var timeRangEnd = $("#timeRangEnd").datebox('getValue');
                    param.addParam({ "timeRangStart": timeRangStart });
                    param.addParam({ "timeRangEnd": timeRangEnd });
                    param.addParam({ "rdlcType": "custom" });
                    if (timeRangStart && timeRangEnd) {
                        $("#iBtn").attr("src", "/Areas/Report/Aspx/VocsDay.aspx?" + param.toUrlParam());
                    } else {
                        com.msg.alert("请选择时间");
                    }
                }
                if ("day" == reportType) {
                    var timeRangDay = $("#timeRangDay").datebox('getValue')
                    param.addParam({ "timeRangDay": timeRangDay });
                    param.addParam({ "rdlcType": "day" });
                    if (timeRangDay) {
                        $("#iBtn").attr("src", "/Areas/Report/Aspx/VocsDay.aspx?"+param.toUrlParam());
                    } else {
                        com.msg.alert("请选择时间");
                    }
                }
                if ("month" == reportType) {
                    param.addParam({ "timeRangMonth": $("#timeRangMonth").combobox('getValue') });
                    param.addParam({ "timeRangYear": $("#timeRangYear").combobox('getValue') });
                    param.addParam({ "rdlcType": "month" });
                    if (timeRangMonth && timeRangYear) {
                        $("#iBtn").attr("src", "/Areas/Report/Aspx/VocsDay.aspx?" + param.toUrlParam());
                    } else {
                        com.msg.alert("请选择时间");
                    }
                }
                if ("year" == reportType) {
                    param.addParam({ "timeRangYear": $("#timeRangOnlyYear").combobox('getValue') });
                    param.addParam({ "rdlcType": "year" });
                    if (timeRangMonth && timeRangYear) {
                        $("#iBtn").attr("src", "/Areas/Report/Aspx/VocsDay.aspx?" + param.toUrlParam());
                    } else {
                        com.msg.alert("请选择时间");
                    }
                }
                param.empty();
            });
        }
        var beforeCommit = function () {
            alert("添加表单数据");
            var data = {
                dataFrom:$("#dataFrom").val()
            };
            
            $("#qureyData").val(JSON.stringify(data));
            return true;
        }
        var mylayout = {
            init: function () {
                var size = { width: $(window).width(), height: $(window).height() };
                $("#layout").width(size.width - 4).height(size.height - 4).layout();
                var center = $("#layout").layout("panel", "center");
                center.panel({
                    onResize: function (w, h) {
                        //$("#dg").datagrid("resize", { width: w - 3, height: h - 70 });
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
    <div region="west" split="true" style="width: 270px; padding: 5px"><!--180-->
       <%
            Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = ViewBag.controllerParam});
       %>
    </div>
    <div region="center" title="废水报表" iconcls="icon-house">
        <!--顶部工具栏-->
        <div id="toolbar">
            <form id="qform">
                <div style="float: left; padding: 5px;"></div>
                <table>
                    <tr>
                        <td><span>数据来源：</span><select id="dataFrom" name="dataFrom" class="txt03 inputSize"></select></td>
                        <td><span>报表：</span><select id="reportType" name="reportType" class="txt03 inputSize"></select></td>
                        <td>
                            <div class="timeRang" id="dayDiv"><input id="timeRangDay" name="timeRangDay" type="text" class="txt03 inputSize" /></div>
                            <div class="timeRang" id="monthDiv">
                                <select id="timeRangYear" name="timeRangYear" class="txt03 inputSize"></select>
                                <select id="timeRangMonth" name="timeRangMonth" class="txt03 inputSize"></select>
                            </div>
                            <div class="timeRang" id="yearDiv"><select id="timeRangOnlyYear" name="timeRangOnlyYear" class="txt03 inputSize"></select></div>
                            <div class="timeRang" id="customDiv">
                                <input id="timeRangStart" name="timeRangStart" type="text" class="txt03 inputSize" />至
                                <input id="timeRangEnd" name="timeRangEnd" type="text" class="txt03 inputSize" />
                            </div>
                        </td>
                        <td><button id="btnCreate" type="button">生成</button></td>
                    </tr>
                </table>
            </form>
        </div>
        <div style="height:90%">
             <iframe id="iBtn" src="" style="width:99%;height:100%;border:0px solid #ffffff"></iframe><!--/Areas/Report/Aspx/AirDay.aspx-->
        </div>

    </div>
</div>
</asp:Content>