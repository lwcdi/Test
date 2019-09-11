<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var model = new listModel();
            //对象1：列表页面对象
            var list = {
                controller: "/Bas/Log", //控制器
                dSize: { width: 4, height: 45 }, //偏移量
                dg: { //数据表格
                    idField: "ID",
                    sortName: "ID",
                    sortOrder: "desc",
                    pageSize: 50,
                    fitColumns: true,
                    columns: [[
                        { title: "ID", field: "ID", width: 50 },
                        { title: "日志类别", field: "LOGTYPENAME", width: 80 },
                        { title: "操作人", field: "OPERATEUSERNAME", width: 80 },
                        { title: "操作IP", field: "OPERATEIP", width: 80 },
                        { title: "操作时间", field: "OPERATETIME", width: 120, formatter: com.formatter.dateTime },
                        { title: "日志内容", field: "LOGCONTENT", width: 800 }
                    ]]
                }
            };
            //对象2：编辑页面对象
            var edit = {
                title: "操作日志管理", //标题
                size: { width: 600, height: 380 }, //页面大小
                onLoad: function (isAdd, row) { //页面加载事件
                    if (isAdd == false) {
                        top.$("#LogTypeName").val(row.LOGTYPENAME);
                        top.$("#OperateUserName").val(row.OPERATEUSERNAME);
                        top.$("#OperateIP").val(row.OPERATEIP);
                        top.$("#OperateTime").val(com.formatter.dateTime(row.OPERATETIME));
                        top.$("#LogContent").val(row.LOGCONTENT);
                    }
                }
            };
            model.bind(list, edit);

            //其它初始化
            $("#LogType").combobox({
                url: "/Bas/Log/GetLogTypeData",
                valueField: "CODE",
                textField: "TITLE",
                width: 100,
                panelHeight: "auto"
            }).combobox("setValue", " ");
            com.ui.combobox.addShowPanel();
            $("#opBegDate,#opEndDate").datebox({ width: 100 });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!--顶部工具栏-->
    <div id="toolbar" style="display: none">
        <%=Html.ToolBar(Model) %>
    </div>
    <!--查询表单-->
    <div style="margin: 5px;">
        <form id="qform">
            <div style="float: left; padding: 5px;">
                <div style="float: left; width: 100%;">
                    日志类别：<input id="LogType" name="LogType" />&nbsp;
                    操作日期：<input id="opBegDate" name="opBegDate" type="text" />
                    至<input id="opEndDate" name="opEndDate" type="text" />
                    日志内容：<input name="LogContent" type="text" class="txt03" style="width: 180px;" />
                    <%=Html.LinkButton("a_search","icon-search","查询") %>
                </div>
            </div>
        </form>
    </div>
    <!--数据列表-->
    <table id="dg"></table>
</asp:Content>
