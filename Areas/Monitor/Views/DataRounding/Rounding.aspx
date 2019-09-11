<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site2.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <%--修约方式：<input id="CHECKTYPE" name="CHECKTYPE" style="width: 140px;" />--%>
        <input type="checkbox" id="wholeRow" onchange="GetData()" />整行修约
        <input type="checkbox" id="wholeCol" onchange="GetData()" />整列修约
                    <input type="button" value="自动修约" onclick="GetAutoRoundingData()" />
    </div>
    <div id="dg" style="width: 920px; height: 500px;">
    </div>
    <div id="dialog" style="display: none">
        <table>
            <tr>
                <th>修约值：
                </th>
                <td>
                    <input type="number" id="CHECKVALUE" name="CHECKVALUE" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <th>修约理由：
                </th>
                <td>
                    <input id="REASON" name="REASON" style="width: 200px;" />
                </td>
            </tr>
            <tr>
                <th>批注：
                </th>
                <td>
                    <textarea id="REMARK" name="REMARK" style="width: 200px; height: 150px"></textarea>
                </td>
            </tr>
            <tr>
                <th>附件：
                </th>
                <td>
                    <input id="FJ" name="FJ" style="width: 200px;" />
                    <input type="button" id="btnupfile" value="上传" onclick="upload()" />
                </td>
            </tr>
        </table>
    </div>
    <div id="dialog_fj" style="display: none">
        <form id="DataForm" name="DataForm" method="post" enctype="multipart/form-data">
            <input class="easyui-filebox" style="width: 200px" id='fb' name="fb" buttontext="浏览"
                buttonicon="icon-search">
        </form>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $("#CHECKTYPE").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CHECKTYPE",
                valueField: "CODE",
                textField: "TITLE",
                required: true,
                editable: false,
                panelHeight: "auto",
                onLoadSuccess: function (data) {
                }
            });
            $("#REASON").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CHECKREASON",
                valueField: "CODE",
                textField: "TITLE",
                required: true,
                editable: false,
                panelHeight: "auto",
                onLoadSuccess: function (data) {
                }
            });

            $("#dg").datagrid({
                singleSelect: true, //选中一行的设置
                rownumbers: true, //行号
                columns: [[
                {
                    field: "ID", title: "编号", width: 150, hidden: true
                },
                    {
                        field: "RECTIME", title: "时间", width: 150, align: 'center',
                        formatter: function (value, row, index) {
                            if (value) return com.formatter.dateTime(value);
                            else return '';
                        }
                    },
                    {
                        field: "ITEMTEXT", title: "因子", width: 150, align: 'center',
                        formatter: function (value, row, index) {
                            if (row["SUBITEMTEXT"])
                                return value + '-' + row["SUBITEMTEXT"];
                            else return value;
                        }
                    },
                    { field: "VALUE", title: "原始值", width: 60, align: 'center' },
                    { field: "CHECKVALUE", title: "修约值", width: 60, align: 'center' },
                    {
                        field: "ITEMUNIT", title: "单位", width: 80, align: 'center',
                        formatter: function (value, row, index) {
                            if (value) return value;
                            else return row["SUBITEMUNIT"];
                        }
                    },
                    { field: "REASON", title: "修约原因", width: 250, align: 'center' },
                    { field: "FILE", title: "附件", width: 60, align: 'center' },
                    {
                        field: "opt", title: "操作", width: 60, align: 'center',
                        formatter: function (value, row, index) {
                            return "<a onclick=ManualRounding(" + index + ") style='cursor:pointer'>人工修约</a>"
                        }

                    }
                ]],
                pagination: false
            })
            GetData();
        });

        //上传文件
        function upload() {
            $('#dialog_fj').show();
            var dialog = $('#dialog_fj').dialog({
                title: "上传凭证",
                width: 500,
                height: 400,
                cache: false,
                modal: true,
                buttons: [
                {
                    text: '确定',
                    iconCls: 'icon-save',
                    handler: function () {
                        $('#DataForm').form('submit', {
                            url: '/Content/code/Handler/UploadHandler.ashx',
                            success: function (result) {
                                $("#FJ").val(result);
                            }
                        });

                        dialog.dialog("close");
                    }
                },
                 {
                     text: '取消',
                     iconCls: 'icon-cancel',
                     handler: function () {
                         dialog.dialog("close");
                     }
                 }]
            });

        }
        function ManualRounding(index) {
            $("#dg").datagrid("selectRow", index);
            var row = $("#dg").datagrid("getRows")[index];
            $('#dialog').show();
            var dialog = $('#dialog').dialog({
                title: "数据修约",
                width: 500,
                height: 400,
                cache: false,
                modal: true,
                buttons: [
                {
                    text: '确定',
                    iconCls: 'icon-save',
                    handler: function () {
                        SaveRounding(row);
                        dialog.dialog("close");
                    }
                },
                 {
                     text: '取消',
                     iconCls: 'icon-cancel',
                     handler: function () {
                         dialog.dialog("close");
                     }
                 }]
            });

        }
        function SaveRounding(row) {
            $.ajax({
                type: "post",
                url: "/Monitor/DataRounding/SaveRounding",
                data: {
                    checkValue: $("#CHECKVALUE").val(),
                    checkReason: $("#REASON").combobox("getValue"),
                    checkRemark: $("#REMARK").val(),
                    checkFj: $("#FJ").val(),
                    id: row["ID"],
                    deviceCode: row["DEVICECODE"],
                    itemCode: row["ITEMCODE"],
                    subItemCode: row["SUBITEMCODE"],
                    recTime: com.formatter.dateTime(row["RECTIME"]),
                    value: row["VALUE"],
                    unit: row["UNIT"],
                    status: row["STATUS"]
                },
                dataType: "json", //返回数据形式为json
                success: function (result) {
                    $("#dg").datagrid('reload');
                }
            });
        }
        function GetAutoRoundingData() {
            com.msg.confirm("此操作将产生自动修约记录，你确定要自动修约吗？", function (r) {
                if (r) {
                    $.ajax({
                        type: "post",
                        url: "/Monitor/DataRounding/GetAutoRoundingData",
                        data: {
                            PKId: '<%=ViewData["PKId"]%>',
                            time: '<%=ViewData["TIME"]%>',
                            timeType: $("#wholeCol").attr('checked') ? "1" : "0",
                            item: $("#wholeRow").attr('checked') ? "" : '<%=ViewData["ITEM"]%>',
                        },
                        dataType: "json", //返回数据形式为json
                        success: function (result) {
                            $("#dg").datagrid('loadData', result);
                        }
                    });
                }
            })
        }

        function GetData() {
            $.ajax({
                type: "post",
                url: "/Monitor/DataRounding/GetRoundingData",
                data: {
                    PKId: '<%=ViewData["PKId"]%>',
                    time: '<%=ViewData["TIME"]%>',
                    timeType: $("#wholeCol").attr('checked') ? "1" : "0",
                    item: $("#wholeRow").attr('checked') ? "" : '<%=ViewData["ITEM"]%>',
                },
                dataType: "json", //返回数据形式为json
                success: function (result) {
                    $("#dg").datagrid('loadData', result);
                }
            });
        }

    </script>
</asp:Content>
