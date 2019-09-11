<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Bas/Button/GetBtnColumns"></script>
    <script type="text/javascript">
        $(function () {
            var model = new listModel();
            //对象1：列表页面对象
            var list = {
                controller: "/Bas/Role", //控制器
                dSize: { width: 4, height: 38 }, //偏移量
                dg: { //数据表格
                    idField: "ID",
                    sortName: "ID",
                    sortOrder: "asc",
                    columns: [[
                        { title: 'ID', field: 'ID', width: 100, sortable: true },
                        { title: '角色名称', field: 'ROLENAME', width: 160, sortable: true },
                        { title: '排序', field: 'SORTNO', width: 80, sortable: true },
                        { title: '默认', field: 'ISDEFAULT', width: 80, align: "center", formatter: com.formatter.check },
                        { title: '备注', field: 'REMARK', width: 280 }
                    ]]
                }
            };
            //对象2：编辑页面对象
            var edit = {
                title: "系统角色", //标题
                size: { width: 450, height: 280 }, //页面大小
                onLoad: function (isAdd, row) { //页面加载事件
                    top.$("#SortNo").numberspinner({ min: 0, max: 999 });
                    if (isAdd == false) {
                        top.$("#RoleName").val(row.ROLENAME);
                        top.$("#SortNo").numberspinner("setValue", row.SORTNO);
                        top.$("#isDefault").attr("checked", row.ISDEFAULT == 1);
                        top.$("#Remark").val(row.REMARK);
                    }
                }
            };
            model.bind(list, edit);

            //其它初始化
            $("#a_set").linkbutton({ text: "分配权限" }).click(authorize.run);
            $("#a_video").click(videoPermission.run);
        });

        var authorize = {
            lastIndex: 0,
            run: function () {
                var role = $("#dg").datagrid("getSelected");
                if (!role) {
                    com.msg.warning('请选择一个角色！');
                    return false;
                }
                var ad = top.$.dialogX({
                    max: true, title: '分配权限',
                    content: '<div style="padding:2px;overflow:hidden;"><table id="nb"></table></div>',
                    toolbar: [
                        { text: '全选', iconCls: 'icon-checkbox_yes', handler: function () { authorize.btnchecked(true); } },
                        { text: '取消全选', iconCls: 'icon-checkbox_no', handler: function () { authorize.btnchecked(false); } },
                        '-',
                        { text: '编辑全部', iconCls: 'icon-pencil', handler: function () { authorize.apply('beginEdit'); } },
                        { text: '取消编辑', iconCls: 'icon-pencil_delete', handler: function () { authorize.apply('cancelEdit'); } },
                        '-',
                        { text: '应用', iconCls: 'icon-disk_multiple', handler: function () { authorize.apply('endEdit'); } }
                    ],
                    submit: function () {
                        authorize.apply("endEdit");
                        var d = authorize.getChanges(role);
                        if (d) {
                            com.ajax({
                                url: "/Bas/Role/Authorize",
                                data: { d: d },
                                success: function (result) {
                                    if (result > 0) {
                                        com.msg.info("权限分配成功！");
                                        ad.dialog("close");
                                    }
                                    else {
                                        com.msg.error("您没有分配任何权限！");
                                    }
                                }
                            });
                        }
                    }
                });

                var nb = top.$("#nb").treegrid({
                    title: "导航菜单",
                    url: "/Bas/Role/GetMenu?Id=" + role.ID,
                    height: ad.dialog("options").height - 115,
                    idField: "Id",
                    treeField: "NavTitle",
                    iconCls: "icon-nav",
                    nowrap: false,
                    rownumbers: true,
                    animate: true,
                    collapsible: false,
                    frozenColumns: [[{ title: "菜单名称", field: "NavTitle", width: 200 }]],
                    columns: [authorize.allBtns()],
                    onClickRow: function (row) {
                        if (authorize.lastIndex != row.Id) {
                            nb.treegrid("endEdit", authorize.lastIndex);
                        }
                        authorize.apply("beginEdit", row.Id);
                        authorize.lastIndex = row.Id;
                    }
                });
                return false;
            },
            allBtns: function () {
                $.each(btns, function () {
                    this.formatter = function (value) {
                        return value == "√" ? "<font color='blue'><b>" + value + "</b></font>" : "<font color='red'><b>" + value + "</b></font>";
                    };
                });
                return btns;
            },
            btnchecked: function (flag) {
                var rows = top.$("#nb").treegrid("getSelections");
                if (rows) {
                    $.each(rows, function (i, n) {
                        var editors = top.$("#nb").treegrid("getEditors", n.Id);
                        $.each(editors, function () {
                            if (!$(this.target).is(":hidden"))
                                $(this.target).attr('checked', flag);

                        });
                    });
                } else {
                    com.msg.warning("请选择菜单！");
                }
            },
            apply: function (action, Id) {
                if (!Id) {
                    top.$("#nb").treegrid("selectAll");
                }

                var rows = top.$("#nb").treegrid("getSelections");
                $.each(rows, function (i, row) {
                    top.$("#nb").treegrid(action, row.Id);
                    if (action == "beginEdit") { //编辑行时，移除未分配按钮的按钮编辑器
                        $.each(btns, function () {
                            var btn = this;
                            var hasBtn = false;
                            $.each(row.Buttons, function () {
                                if (btn.field == this) {
                                    hasBtn = true;
                                    return false; //相当于break
                                }
                            });
                            var ed = top.$("#nb").treegrid("getEditor", { index: row.Id, field: btn.field }); //***treegrid的getEditor的api有误，id改成index
                            if (hasBtn == false) {
                                top.$(ed.target).remove();
                            }
                        });
                    }
                });

                if (action != "beginEdit") {
                    top.$("#nb").treegrid("clearSelections");
                }
            },
            getChanges: function (role) {
                var rows = top.$("#nb").treegrid("getChildren");
                var o = { roleId: role.ID, menus: [] };
                $.each(rows, function (i, row) {
                    var nav = { navid: row.Id, buttons: [] };
                    $.each(row, function (n, v) {
                        if (v == "√") {
                            nav.buttons.push(n);
                        }
                    });
                    o.menus.push(nav);
                });
                return JSON.stringify(o);
            }
        };

        var videoPermission = {
            run: function () {
                var role = $("#dg").datagrid("getSelected");
                if (!role) {
                    com.msg.warning('请选择一个角色！');
                    return false;
                }
                var dialog = top.$.dialogX({
                    title: "编辑视频权限",
                    width: 400,
                    height: 500,
                    closable: false,
                    href: "/HighView/VideoManage/EditPermission?role="+role.ID,
                    buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-save',
                        handler: function () {
                            var videoIds = "";
                            $.each(top.$("#videotree").tree('getChecked'), function (i, n) {
                                if(top.$('#videotree').tree('isLeaf', n.target))
                                    videoIds += n.id + ",";
                            });
                            videoIds = videoIds.substr(0, videoIds.length - 1);
                            if (videoIds.length > 0) {
                                com.ajax({
                                    url: "/HighView/VideoManage/SaveVideoPermission",
                                    data: { role: role.ID, videoIds: videoIds },
                                    success: function (result) {
                                        if (result > 0) {
                                            com.msg.info("权限分配成功！");
                                            dialog.dialog("close");
                                        }
                                        else {
                                            com.msg.error("您没有分配任何权限！");
                                        }
                                    }
                                });
                            }
                            else {
                                com.msg.error("您没有分配任何权限！");
                            }
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
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!--顶部工具栏-->
    <div id="toolbar">
        <%=Html.ToolBar(Model) %>
        <%=Html.LinkButton("a_video","icon-wrench","视频权限") %>
    </div>
    <!--查询表单-->
    <div style="margin: 2px;">
        <form id="qform"></form>
    </div>
    <!--数据列表-->
    <table id="dg"></table>
</asp:Content>
