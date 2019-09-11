<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            dicType.bindTree();

            var model = new listModel();
            //对象1：列表页面对象
            var list = {
                controller: "/Bas/Dic", //控制器
                dSize: { width: 197, height: 66 }, //偏移量
                dg: { //数据表格
                    idField: "ID",
                    sortName: "ID",
                    sortOrder: "asc",
                    columns: [[
                        { title: 'ID', field: 'ID', width: 60 },
                        { title: '名称', field: 'TITLE', width: 160 },
                        { title: '编码', field: 'CODE', width: 180 },
                        { title: '排序', field: 'SORTNO', width: 80 },
                        { title: '状态', field: 'STATUS', width: 60, align: 'center', formatter: com.formatter.check },
                        { title: '说明', field: 'REMARK', width: 300 }
                    ]]
                }
            };
            //对象2：编辑页面对象
            var edit = {
                title: "数据字典", //标题
                size: { width: 500, height: 350 }, //页面大小
                beforeLoad: function (isAdd, row) { //加载之前
                    if (isAdd == true && !dicType.getSelected()) {
                        com.msg.warning("请选择字典类别！");
                        return false;
                    }
                    return true;
                },
                onLoad: function (isAdd, row) { //页面加载事件
                    var typeNode = dicType.getSelected();
                    top.$("#TypeId").val(typeNode.id);
                    top.$("#TypeName").text(typeNode.text);
                    top.$("#SortNo").numberspinner({ min: 0, max: 999 });
                    top.$("#Status").combobox({ panelHeight: "auto", editable: false });
                    if (isAdd == true) {
                        top.$("#Title").focus();
                    }
                    else {
                        top.$("#Title").val(row.TITLE);
                        top.$("#Code").val(row.CODE);
                        top.$("#SortNo").numberspinner("setValue", row.SORTNO);
                        top.$("#Status").combobox("setValue", row.STATUS);
                        top.$("#Remark").val(row.REMARK);
                    }
                }
            };
            model.bind(list, edit);

            //其它初始化
            $("#a_addType").click(dicType.add);
            $("#a_editType").click(dicType.edit);
            $("#a_delType").click(dicType.del);

            $("#dg").datagrid("enableFilterX", [
                {
                    field: "Status", type: "combobox",
                    data: [{ value: "", text: "全部" }, { value: "0", text: "禁用" }, { value: "1", text: "启用" }]
                }
            ]);

            layout(true);
        });

        var layout = function (first) {
            if (first && first == true) {
                $(window).resize(function () {
                    layout();
                });
            }

            var size = { width: $(window).width(), height: $(window).height() };
            $("#layout").width(size.width - 4).height(size.height - 4).layout();
            var west = $("#layout").layout("panel", "west");
            $("#divTree").height(size.height - 60);
            west.panel({
                onResize: function (w, h) {
                    $("#divWest").width(w - 5);
                    $("#divWest").panel("header").width(w - 5);
                    $("#divTree").height(h - 60);
                }
            });
            var center = $("#layout").layout("panel", "center");
            center.panel({
                onResize: function (w, h) {
                    $("#dg").datagrid("resize", { width: w - 3, height: h - 62 });
                }
            });
        };

        var dicType = {
            bindTree: function () {
                $("#dataDicType").tree({
                    url: "/Bas/Dic/GetTreeDicType",
                    animate: true,
                    onLoadSuccess: function (node, data) {
                        if (data.length == 0) {
                            $("#noDicType").fadeIn();
                        }
                    },
                    onClick: function (node) {
                        var parms = {
                            action: "List",
                            typeId: node.id
                        };
                        $("#dg").datagrid("clearSelections").datagrid("load", parms);
                    }
                });
            },
            getSelected: function () {
                return $("#dataDicType").tree("getSelected");
            },
            reload: function () {
                $("#dataDicType").tree("reload");
            },
            add: function () {
                var dialog = top.$.dialogX({
                    title: "添加字典类别",
                    iconCls: "icon-add",
                    href: "/Bas/Dic/EditDicType",
                    width: 400,
                    height: 300,
                    onLoad: function () {
                        top.$("#SortNo").numberspinner({ min: 0, max: 999 });
                    },
                    submit: function () {
                        var isValid = top.$("#dicTypeForm").form("validate");
                        if (isValid) {
                            com.ajax({
                                url: "/Bas/Dic/AddDicType",
                                data: {
                                    Title: top.$("#Title").val(),
                                    Code: top.$("#Code").val(),
                                    SortNo: top.$("#SortNo").numberspinner("getValue"),
                                    Remark: top.$("#Remark").val()
                                },
                                success: function (result) {
                                    if (result.Success == true) {
                                        com.msg.info("字典类别添加成功！");
                                        dialog.dialog("close");
                                        dicType.reload();
                                    }
                                    else {
                                        com.msg.error("字典类别添加失败！");
                                    }
                                }
                            });
                        }
                    }
                });
            },
            edit: function () {
                var node = dicType.getSelected();
                if (node) {
                    var dialog = top.$.dialogX({
                        title: "修改字典类别",
                        iconCls: "icon-save",
                        href: "/Bas/Dic/EditDicType",
                        width: 400,
                        height: 260,
                        onLoad: function () {
                            com.ajax({
                                url: "/Bas/Dic/GetDataType",
                                data: { id: node.id },
                                success: function (data) {
                                    top.$("#Id").val(data.ID);
                                    top.$("#Title").val(data.TITLE);
                                    top.$("#Code").val(data.CODE);
                                    top.$("#SortNo").numberspinner({ min: 0, max: 999 }).numberspinner("setValue", data.SORTNO);
                                    top.$("#Remark").val(data.REMARK);
                                }
                            });
                        },
                        submit: function () {
                            var isValid = top.$("#dicTypeForm").form("validate");
                            if (isValid) {
                                com.ajax({
                                    url: "/Bas/Dic/EditDicType",
                                    data: {
                                        Id: top.$("#Id").val(),
                                        Title: top.$("#Title").val(),
                                        Code: top.$("#Code").val(),
                                        SortNo: top.$("#SortNo").numberspinner("getValue"),
                                        Remark: top.$("#Remark").val()
                                    },
                                    success: function (result) {
                                        if (result.Success == true) {
                                            com.msg.info("字典类别修改成功！");
                                            dialog.dialog("close");
                                            dicType.reload();
                                        }
                                        else {
                                            com.msg.error("字典类别修改失败！");
                                        }
                                    }
                                });
                            }
                        }
                    });
                }
                else {
                    com.msg.error("请选择一个字典类别！");
                }
            },
            del: function () {
                var node = dicType.getSelected();
                if (node) {
                    com.msg.confirm("你确定要删除选择的字典类别吗？", function (r) {
                        if (r) {
                            com.ajax({
                                url: "/Bas/Dic/DelDicType",
                                data: {
                                    Id: node.id
                                },
                                success: function (result) {
                                    if (result.Success == true) {
                                        com.msg.info("字典类别删除成功！");
                                        dicType.reload();
                                    }
                                    else if (result.Message != "") {
                                        com.msg.error(result.Message);
                                    }
                                    else {
                                        com.msg.error("字典类别删除失败！");
                                    }
                                }
                            });
                        }
                    });
                }
                else {
                    com.msg.error("请选择一个字典类别！");
                }
            }
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="layout">
        <div region="west" split="true" style="width: 190px; overflow-y: hidden;">
            <div id="divWest" class="easyui-panel" title="字典类别" border="false" iconcls="icon-book_red" style="overflow: hidden;">
                <div style="background: #efefef; border-bottom: 1px solid #ccc">
                    <%=Html.LinkButton("a_addType","icon-add","添加") %>
                    <%=Html.LinkButton("a_editType","icon-edit","修改") %>
                    <%=Html.LinkButton("a_delType","icon-delete","删除") %>
                </div>
                <div id="divTree" style="overflow: auto;">
                    <ul id="dataDicType"></ul>
                </div>
            </div>
            <div id="noDicType" style="font-family: 微软雅黑; font-size: 18px; color: #BCBCBC; padding: 40px 5px; display: none;">
                还没有字典类别，点击 添加 按钮创建新的类别。
            </div>
        </div>
        <div region="center" title="数据字典" iconcls="icon-list">
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
        </div>
    </div>
</asp:Content>
