<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .Bar {
            width: 99%;
            height: 30px;
            line-height: 30px;
            color: #999999;
            background-color: #f2f2f2;
            border: 1px solid #e4e4e4;
            margin: 5px 1px 5px 1px;
            padding-left: 5px;
        }

        table {
            font-size: 12px;
            table-layout: fixed;
            empty-cells: show;
            border-collapse: collapse;
            margin: 0 auto;
            width: 100%;
        }

        th {
            text-align: right;
            width: 100px;
        }

        td {
            height: 30px;
        }

            td input {
                width: auto;
                border: none;
            }

            td textarea {
                width: 335px;
            }

        table.t1 {
            border: 1px solid #cad9ea;
            color: #666;
        }

            table.t1 th {
                background-color: #e4e4e4;
                height: 30px;
            }

            table.t1 td, table.t1 th {
                border: 1px solid #cad9ea;
                padding: 0 1em 0;
            }

            table.t1 tr.a1 {
                background-color: #f5fafe;
            }



        table.t2 {
            border: 1px solid #9db3c5;
            color: #666;
        }

            table.t2 th {
                background-image: url(th_bg2.gif);
                background-repeat: repeat-x;
                height: 30px;
                color: #fff;
            }

            table.t2 td {
                border: 1px dotted #cad9ea;
                padding: 0 2px 0;
            }

            table.t2 th {
                border: 1px solid #a7d1fd;
                padding: 0 2px 0;
            }

            table.t2 tr.a1 {
                background-color: #e8f3fd;
            }



        table.t3 {
            border: 1px solid #fc58a6;
            color: #720337;
        }

            table.t3 th {
                background-image: url(th_bg3.gif);
                background-repeat: repeat-x;
                height: 30px;
                color: #35031b;
            }

            table.t3 td {
                border: 1px dashed #feb8d9;
                padding: 0 1.5em 0;
            }

            table.t3 th {
                border: 1px solid #b9f9dc;
                padding: 0 2px 0;
            }

            table.t3 tr.a1 {
                background-color: #fbd8e8;
            }
    </style>
    <link type="text/css" rel="stylesheet" href="/Content/css/Checkbox.css" />
    <div id="inputs">
        <div class="Bar">
            告警信息：<input type="hidden" id="ID" name="ID" />
        </div>
        <table border="1" class="t1">
            <tr>
                <th>持续时间（分钟）</th>
                <td>
                    <input type="text" id="DURATION" name="DURATION" value="" readonly /></td>
            </tr>
            <tr>
                <th>告警内容</th>
                <td>
                    <input type="text" id="CONTENT" name="CONTENT" value="" readonly /></td>
            </tr>
            <tr>
                <th>企业名称</th>
                <td>
                    <input type="text" id="COMPANYNAME" name="COMPANYNAME" value="" readonly /></td>
            </tr>
            <tr>
                <th>点位</th>
                <td>
                    <input type="text" id="PKNAME" name="PKNAME" value="" readonly /></td>
            </tr>
            <tr>
                <th>MN号</th>
                <td>
                    <input type="text" id="MN" name="MN" value="" readonly /></td>
            </tr>
            <tr>
                <th>告警时间</th>
                <td>
                    <input type="text" id="STARTTIME" name="STARTTIME" value="" readonly /></td>
            </tr>
        </table>
        <div class="Bar">处理情况：</div>
        <table border="1" class="t1">
            <tr>
                <th>处理人</th>
                <td>
                    <input type="text" id="CHECKUSER" name="CHECKUSER" value="" readonly /></td>
            </tr>
            <tr>
                <th>处理时间</th>
                <td>
                    <input type="text" id="CHECKTIME" name="CHECKTIME" value="" readonly /></td>
            </tr>
            <tr>
                <th>处理内容</th>
                <td>
                    <textarea id="CHECKCONTENT" name="CHECKCONTENT" rows="4"></textarea></td>
            </tr>
        </table>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Content/js/getQueryStr.js"></script>
    <script>
        var companyid = getQueryStr("id");
        if (companyid) {
            $(function () {
                init();
            });
        }

        function init() {
            GetAlert();
            SetInput();
        }

        function GetAlert() {
            var id = getQueryStr("id");
            com.ajax({
                url: "/BaseData/AlertCheck/GetAlertById",
                data: { id: id },
                async: false,
                success: function (result) {
                    if (result.success == true && 1 == result.data.length) {
                        debugger;
                        var data = result.data[0];
                        setFields("inputs", data);
                        if (0 == data.STATE) {
                            $("#CHECKTIME").val(new Date().format('yyyy-MM-dd hh:mm:ss'));
                            $("#CHECKUSER").val(userJson.USERNAME);
                        }
                    }
                }
            });
        }

        function checkLength(obj, maxlength) {
            if (obj.value.length > maxlength) {
                obj.value = obj.value.substring(0, maxlength);
            }
        }

        function SetInput() {
            var inputs = $("table textarea");
            inputs.attr("placeholder", "请输入处理内容");
            inputs.attr("onpropertychange", "checkLength(this,50);");
            inputs.attr("oninput", "checkLength(this,50);");
        }

        function SaveSetting() {
            debugger;
            var id = getQueryStr("id");
            com.ajax({
                url: "/BaseData/AlertCheck/SaveAlertById",
                data: { id: id, content: $("#CHECKCONTENT").val() },
                async: false,
                success: function (result) {
                    com.msg.alert(result.data);
                }
            });
        }

    </script>
</asp:Content>
