<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var model = new listModel();
            //对象1：列表页面对象
            var list = {
                controller: "/Bas/Button", //控制器
                dSize: { width: 4, height: 38 }, //偏移量
                dg: { //数据表格
                    idField: "ID",
                    sortName: "SORTNO",
                    sortOrder: "asc",
                    striped: true,
                    columns: [[
                        { title: "ID", field: "ID", width: 60, sortable: true },
                        {
                            title: "图标", field: "ICONCLS", width: 40, formatter: function (v, d, i) {
                                return '<span class="icon ' + v + '">&nbsp;</span>';
                            }, align: "center"
                        },
                        { title: "按钮名称", field: "BUTTONTEXT", width: 100, sortable: true },
                        { title: "权限标识", field: "BUTTONTAG", width: 80, sortable: true },
                        { title: "排序", field: "SORTNO", width: 80, sortable: true },
                        { title: "说明", field: "REMARK", width: 300 }
                    ]]
                }
            };
            //对象2：编辑页面对象
            var edit = {
                title: "系统按钮", //标题
                size: { width: 350, height: 380 }, //页面大小
                onLoad: function (isAdd, row) { //页面加载事件
                    if (isAdd == false) {
                        showICON();
                        top.$("#ButtonText").val(row.BUTTONTEXT);
                        top.$("#ButtonTextEN").val(row.BUTTONTEXTEN);
                        top.$("#ButtonTag").val(row.BUTTONTAG);
                        top.$("#IconCls").val(row.ICONCLS);
                        top.$("#Remark").val(row.REMARK);
                        top.$("#SortNo").val(row.SORTNO).numberspinner();
                    }
                    else {
                        top.$("#ButtonText").validatebox();
                        top.$("#ButtonTag").validatebox();
                        top.$("#ButtonText").val("");
                        top.$("#ButtonTextEN").val("");
                        top.$("#ButtonTag").val("");
                        top.$("#SortNo").numberspinner();
                        showICON();
                    }
                }
            };
            model.bind(list, edit);
        });

        var showICON = function () {
            $.ajax({
                url: "",
                data: { a: "", b: "" }
            });
            top.$("#selecticon").click(function () {
                var dialog = top.$.dialogX({
                    title: "选择图标",
                    width: 800,
                    height: 600,
                    href: "/content/css/iconlist.htm?v=" + Math.random(),
                    iconCls: "icon-application_view_icons",
                    buttons: [{
                        text: "关闭",
                        iconCls: "icon-cancel",
                        handler: function () {
                            dialog.dialog("close");
                        }
                    }],
                    onLoad: function () {
                        top.$("#iconlist li").attr("style", "float:left;border:1px solid #fff; line-height:20px; margin-right:4px;width:16px;cursor:pointer")
                        .click(function () {
                            top.$("#IconCls").val(top.$(this).find("span").attr("class").split(" ")[1]);
                            dialog.dialog("close");
                        }).hover(function () {
                            top.$(this).css({ "border": "1px solid red" });
                        }, function () {
                            top.$(this).css({ "border": "1px solid #fff" });
                        });
                    },
                    submit: function () {
                        dialog.dialog("close");
                    }
                });
            });
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!--顶部工具栏-->
    <div id="toolbar">
        <%=Html.ToolBar(Model) %>
    </div>
    <!--查询表单-->
    <div style="margin: 2px;">
        <form id="qform"></form>
    </div>
    <!--数据列表-->
    <table id="dg"></table>
</asp:Content>
