<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var model = new listModel();
            //对象1：列表页面对象
            var list = {
                controller: "/Bas/SmsLog", //控制器
                dSize: { width: 4, height: 45 }, //偏移量
                dg: { //数据表格
                    idField: "ID",
                    sortName: "ID",
                    sortOrder: "desc",
                    pageSize: 50,
                    fitColumns: true,
                    columns: [[
                        { title: "ID", field: "ID", width: 50, hidden: true },
                        { title: "发送时间", field: "SENDTIME", width: 120, formatter: com.formatter.dateTime },
                        { title: "模板类型", field: "SMSTEMPTYPE_TEXT", width: 120 },
                        { title: "发送内容", field: "CONTENT", width: 500 },
                        { title: "接收人", field: "USERNAME_TEXT", width: 120 },
                        {
                            title: "发送状态", field: "STATUS", width: 60, formatter: function (value, row, index) {
                                return value == 0 ? "发送成功" : "发送失败";
                            }
                        },
                        {
                            title: "操作", field: "opt", width: 60, formatter: function (value, row, index) {
                                return "<a  href='javascript:SendSmsRetry(\"" + row.ID + "\");'>重新发送</a>";
                            }
                        }
                    ]]
                }
            };
            //对象2：编辑页面对象
            var edit = {
            };
            model.bind(list, edit);

            //其它初始化
            $("#TEMPTYPE").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=SMSTEMPTYPE",
                valueField: "CODE",
                textField: "TITLE",
                width: 100,
                panelHeight: "auto"
            });
            com.ui.combobox.addShowPanel();
            $("#opBegDate,#opEndDate").datebox({ width: 100 });

        });
        function SendSmsRetry(id) {
            com.ajax({
                url: "/Bas/SmsLog/SendSmsRetry",
                data: {
                    id: id
                },
                success: function (result) {
                    if (result.Success == true) {
                        com.msg.info("提交成功！");
                    }
                    else {
                        com.msg.info("提交失败！");
                    }
                }
            });

        }
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
                    用户名称：<input id="USERNAME_TEXT" name="USERNAME_TEXT" />&nbsp;
                    模板类型：
                    <input id="TEMPTYPE" name="TEMPTYPE" style="width: 140px;" />
                    操作日期：<input id="opBegDate" name="opBegDate" type="text" />
                    至<input id="opEndDate" name="opEndDate" type="text" />
                    <%=Html.LinkButton("a_search","icon-search","查询") %>
                </div>
            </div>
        </form>
    </div>
    <!--数据列表-->
    <table id="dg"></table>
</asp:Content>
