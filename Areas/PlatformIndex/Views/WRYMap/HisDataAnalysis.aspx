<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script src="/Content/js/getQueryStr.js"></script>
    <style>
        * {
            margin: 0;
            padding: 0;
        }

        table tr td {
            border: 1px solid #D4D4D4;
            height: 25px;
            padding: 2px 5px;
        }
    </style>
    <script>
        window.onload = function () {
            $("#stime").datetimebox({
                required: false,
                editable: false,
            }).datetimebox('setValue', new Date().addDays(-1).Format('yyyy-MM-dd'));
            $("#etime").datetimebox({
                required: false,
                editable: false,
            }).datetimebox('setValue', new Date().Format('yyyy-MM-dd HH:mm:ss'));
            var PKID = getQueryStr("PKID");

            $("#PKSelect").combobox({
                url: "/PlatformIndex/WRYMap/GetPKList?PKID=" + PKID,
                valueField: "ID",
                textField: "NAME",
                required: true,
                editable: false,
                panelHeight: "auto",
                onLoadSuccess: function (data) {
                    $("#PKSelect").combobox('setValue', PKID);
                    GetPKDatas();
                }
            });
        };

        function GetPKDatas() {
            var arr = [];
            $.ajax({
                type: "post",
                url: "/PlatformIndex/WRYMap/GetPKDatas",
                data: {
                    PKID: $("#PKSelect").combobox('getValue'),
                    DataType: $("#DataType")[0].value,
                    sTime: $("#stime").datetimebox('getValue'),
                    eTime: $("#etime").datetimebox('getValue'),
                },
                dataType: "json", //返回数据形式为json
                success: function (result) {
                    var div = document.getElementById('chart');
                    myChart = echarts.init(div)
                    myChart.setOption(eval('(' + result + ')'));
                },
                error: function (errorMsg) {
                }
            });
        }

        //获取当前时间
        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var seperator2 = ":";
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
                    + " " + date.getHours() + seperator2 + date.getMinutes()
                    + seperator2 + date.getSeconds();
            return currentdate;
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr>
            <td>排口信息：
                    <input id="PKSelect" style="width: 120px;" />
            </td>
            <td>数据类型：
                <select id="DataType">
                    <option value="1">分钟数据</option>
                    <option value="2" selected>小时数据</option>
                    <option value="3">日数据</option>
                </select>
            </td>
            <td>时间：
                <select id="stime" name="stime" style="width: 200px;" class="txt03"></select>
                <span>至：</span>
                <select id="etime" name="etime" style="width: 150px;" class="txt03"></select>
            </td>
            <td>
                <a plain="true" class="easyui-linkbutton" icon="icon-search" title="查询" onclick="GetPKDatas()">查询</a>
            </td>
        </tr>
    </table>

    <div id="chart" style="width: 800px; height: 450px">
    </div>
</asp:Content>
