<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <link type="text/css" rel="stylesheet" href="/Content/css/Checkbox.css" />
    <style>
        .datagrid-row {
            height: 30px;
        }

        .datagrid-cell {
            height: 30px;
            line-height: 30px;
        }
    </style>
    <script type="text/javascript">
        $(function () {
            var model = new listModel();
            //对象1：列表页面对象
            var list = {
                controller: "/Bas/SmsTemplate", //控制器
                dSize: { width: 4, height: 45 }, //偏移量
                dg: { //数据表格
                    idField: "ID",
                    sortName: "ID",
                    sortOrder: "desc",
                    pageSize: 50,
                    fitColumns: true,
                    columns: [[
                        { title: "ID", field: "ID", width: 50, hidden: true },
                        { title: "模板名称", field: "TEMPNAME", width: 200 },
                        { title: "模板类型", field: "SMSTEMPTYPE_TEXT", width: 100 },
                        //{ title: "业务类型", field: "SMSBASTYPE_TEXT", width: 100 },
                        { title: "创建者", field: "CREATENAMEREAL", width: 120 },
                        { title: "创建时间", field: "CREATETIME", width: 150, formatter: com.formatter.dateTime },
                        {
                            title: "是否启用", field: "ISUSE", width: 80, formatter: function (value, row, index) {
                                checked = value == 1 ? 'checked' : '';
                                return '\
                                <div class="holder">\
                                    <div style="width: 101px;">\
                                        <input type="checkbox" id="checkbox' + row.ID + '" ' + checked + ' onchange =AlterUsed("' + row.ID + '","' + $("#checkbox" + row.ID).attr('checked') + '") /><label for="checkbox' + row.ID + '"></label>\
                                    </div>\
                                </div>';
                            }
                        }
                    ]]
                }
            };
            //对象2：编辑页面对象
            var edit = {
                title: "短信模板", //标题
                size: { width: 600, height: 380 }, //页面大小
                onLoad: function (isAdd, row) { //页面加载事件
                    //其它初始化
                    top.$("#SMSTEMPTYPE").combobox({
                        url: "/Bas/Dic/GetDicCodeData?typeCode=SMSTEMPTYPE",
                        valueField: "CODE",
                        textField: "TITLE",
                        width: 120,
                        panelHeight: "auto"
                    }).combobox("setValue", "0");
                    //top.$("#SMSBASTYPE").combobox({
                    //    url: "/Bas/Dic/GetDicCodeData?typeCode=SMSBASTYPE",
                    //    valueField: "CODE",
                    //    textField: "TITLE",
                    //    width: 120,
                    //    panelHeight: "auto"
                    //}).combobox("setValue", "0");
                    if (!isAdd) {
                        top.$("#SMSTEMPTYPE").combobox("setValue", row.SMSTEMPTYPE);
                        //top.$("#SMSBASTYPE").combobox("setValue", row.SMSBASTYPE);
                        top.$("#TEMPNAME").val(row.TEMPNAME);
                        top.$("#TEMPCONTENT").val(row.TEMPCONTENT);
                        if (row.ISUSE == 1) top.$("input[name=ISUSE").attr("checked", "checked");
                    }
                }
            };
            model.bind(list, edit);

            //其它初始化
            $("#TEMPTYPE").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=SMSTEMPTYPE",
                valueField: "CODE",
                textField: "TITLE",
                width: 100,
                panelHeight: "auto"
            });//.combobox("setValue", " ");
            //$("#BASTYPE").combobox({
            //    url: "/Bas/Dic/GetDicCodeData?typeCode=SMSBASTYPE",
            //    valueField: "CODE",
            //    textField: "TITLE",
            //    width: 100,
            //    panelHeight: "auto"
            //});//.combobox("setValue", " ");
        });
        function AlterUsed(id, checked) {
            if (checked == 'checked') checked = false;
            else checked = 'checked'
            com.ajax({
                url: "/Bas/SmsTemplate/AlterUsed",
                data: {
                    id: id,
                    isused: checked
                },
                success: function (result) {
                    if (result.Success == true) {
                        //$("#dg").datagrid('updateRow', {

                        //})
                    }
                    else {
                        com.msg.error(result.Message);
                    }
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!--顶部工具栏-->
    <div id="toolbar">
        <%=Html.ToolBar(Model) %>
    </div>
    <!--查询表单-->
    <div style="margin: 5px;">
        <form id="qform">
            <div style="float: left; padding: 5px;">
                <div style="float: left; width: 100%;">
                    模板名称：<input id="NAME" name="NAME" />&nbsp;
                    模板类型：
                    <input id="TEMPTYPE" name="TEMPTYPE" style="width: 140px;" />
                    <%-- 业务类型：
                    <input id="BASTYPE" name="BASTYPE" style="width: 140px;" />--%>
                </div>
            </div>
        </form>
    </div>
    <!--数据列表-->
    <table id="dg"></table>
</asp:Content>
