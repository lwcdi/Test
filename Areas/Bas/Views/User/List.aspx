<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Bas/Button/GetBtnColumns"></script>
    <script type="text/javascript">
        $(function () {
            //$("#getname").combobox({
            //    url: "/Bas/User/GetCompanyData?RemoveNoneSelect=1",
            //    valueField: "CODE",
            //    textField: "TITLE",
            //    multiple: true
            //});


            mylayout.init();

            var model = new listModel();
            //对象1：列表页面对象
            var list = {
                controller: "/Bas/User", //控制器
                dSize: { width: 187, height: 66 }, //偏移量
                dg: { //数据表格
                    idField: "ID",
                    sortName: "ID",
                    sortOrder: "asc",
                    columns: [[
                        { title: "所属区域", field: "AREATEXT", width: 160 },
                        { title: "姓名", field: "TRUENAME", width: 200, sortable: true },
                        { title: "登录名", field: "USERNAME", width: 120, sortable: true },
                        { title: "隶属组织机构", field: "ORGNAME", width: 120 },
                        { title: "用户类型", field: "USERTYPENAME", width: 100 },
                        { title: "是否启用", field: "ISDISABLED", width: 80, align: "center", formatter: com.formatter.check },
                        { title: "描述", field: "REMARK", width: 220 }
                    ]]
                }
            };
            //对象2：编辑页面对象
            var edit = {
                title: "系统用户", //标题
                size: { width: 500, height: 420 }, //页面大小
                onLoad: function (isAdd, row) { //页面加载事件
                    top.$("#userTab").tabs({
                        onSelect: function () {
                            //top.$(".validatebox-tip").remove();
                        }
                    });
                    //top.$("#OrgId").combotree({ url: "/Bas/Org/GetComboTreeOrg" }).combotree("setValue", -1);;
                    top.$("#IsAdmin,#IsDisabled").combobox({ panelHeight: "auto" });
                    top.$("#AreaCode").getcombobox({
                        valueField: "CODE",
                        textField: "TITLE",
                        width: 200,
                        editable: true,
                        //selectIndex: 0,
                        required: true,
                        onSelect: function (item) {
                        }
                    });
                    top.$("#OrgId").combotree({
                        url: "/Bas/Org/GetComboTreeOrg",
                    });
                    top.$("#UserTypeId").combobox({
                        url: "/Bas/User/GetUserTypeData",
                        valueField: "CODE",
                        textField: "TITLE",
                        width: 200,
                        editable: true,
                        panelHeight: "auto",
                        required: true,
                        onSelect: function (item) {
                            top.$("#AreaCode").combobox({
                                url: "/Bas/User/GetAreaData?userTypeId=" + item.CODE
                            });
                            //if (item.CODE == '2') {
                            //    top.$("#trVOS").show();
                            //}
                            //else {
                            //    top.$("#trVOS").hide();
                            //}
                        }
                    });
                    if (isAdd == true) {
                        top.$("#trPassword").show();
                    }
                    else {
                        top.$("#AreaCode").getcombobox({
                            url: "/Bas/User/GetAreaData?userTypeId=" + row.USERTYPEID
                        });
                        top.$("#UserName").val(row.USERNAME).attr("disabled", "disabled");
                        top.$("#TrueName").val(row.TRUENAME);
                        top.$("#IsAdmin").combobox("setValue", row.ISADMIN == true ? "true" : "false");
                        top.$("#IsDisabled").combobox("setValue", row.ISDISABLED == true ? "true" : "false");
                        top.$("#UserTypeId").combobox("setValue", row.USERTYPEID);
                        top.$("#AreaCode").combobox("setValue", row.AREACODE)
                        top.$("#OrgId").combotree("setValue", row.ORGID > 0 ? row.ORGID : -1);
                        top.$("#Email").val(row.EMAIL);
                        top.$("#Mobile").val(row.MOBILE);
                        top.$("#QQ").val(row.QQ);
                        top.$("#Remark").val(row.REMARK);
                        if (row.USERTYPEID == '2') {
                            top.$("#trVOS").show();
                        }
                    }
                }
            };
            model.bind(list, edit);

            $("#dg").datagrid("enableFilterX");

            //其它初始化
            $(window).resize(function () {
                mylayout.resize();
            });

            $("#a_role").click(setRole);
            $("#a_authorize").click(authorize.run);
            $("#a_editPwd").click(editPwd);

            if (userJson.USERTYPEID == "2") {
                $("#a_role").hide();
                $("#a_authorize").hide();
            }
            else {
                $("#a_role").show();
                $("#a_authorize").show();
            }
        });

        var mylayout = {
            init: function () {
                var size = { width: $(window).width(), height: $(window).height() };
                $("#layout").width(size.width - 4).height(size.height - 4).layout();
                var center = $("#layout").layout("panel", "center");
                center.panel({
                    onResize: function (w, h) {
                        $("#dg").datagrid("resize", { width: w - 3, height: h - 62 });
                    }
                });
            },
            resize: function () {
                mylayout.init();
                $("#layout").layout("resize");
            }
        };

        var setRole = function () {
            var row = $("#dg").datagrid("getSelected");
            if (row) {
                var setDialog = top.$.dialogX({
                    title: "用户名：" + row.USERNAME,
                    width: 620, height: 450, iconCls: "icon-cog", cache: false,
                    href: "/Bas/User/SetRole",
                    onLoad: function () {
                        top.$("#left_roles,#right_roles").datagrid({
                            nowrap: false, //折行
                            fit: true,
                            rownumbers: true, //行号
                            striped: true, //隔行变色
                            idField: "ID",//主键
                            singleSelect: true, //单选
                            columns: [[
                                { title: "角色名称", field: "ROLENAME", width: 120 },
                                { title: "备注", field: "REMARK", width: 130 }
                            ]]
                        });
                        top.$("#left_roles").datagrid({
                            title: "所有角色",
                            url: "/Bas/Role/GetRoleTable",
                            onDblClickRow: function (index, row) {
                                moveRight(row);
                            }
                        });
                        top.$("#right_roles").datagrid({
                            title: "已选角色",
                            url: "/Bas/User/GetUserRole?UserId=" + row.ID,
                            onDblClickRow: function (index, row) {
                                moveLeft(row);
                            }
                        });
                        top.$("#btnRight").click(function () {
                            var row = top.$("#left_roles").datagrid("getSelected");
                            moveRight(row);
                        });
                        top.$("#btnLeft").click(function () {
                            var row = top.$("#right_roles").datagrid("getSelected");
                            moveLeft(row);
                        });
                    },
                    submit: function () {
                        var rows = top.$("#right_roles").datagrid("getRows");
                        var roleIdArray = [];
                        $.each(rows, function () {
                            roleIdArray.push(this.ID);
                        });
                        var roleIdString = "";
                        if (roleIdArray.length > 0) {
                            roleIdString = roleIdArray.join(",");
                        }
                        com.ajax({
                            url: "/Bas/User/SaveUserRole",
                            data: {
                                userId: row.ID,
                                roleIdString: roleIdString
                            },
                            success: function (result) {
                                if (result.Success) {
                                    com.msg.info("用户角色设置成功！");
                                    setDialog.dialog("close");
                                    $("#dg").datagrid("reload");
                                }
                                else {
                                    com.msg.error("用户角色设置失败，或没有选择任何角色！");
                                }
                            }
                        });
                    }
                });
            }
            else {
                com.msg.warning("请选择一个用户！");
            }
        };
        var moveRight = function (row) {
            if (row) {
                var rows = top.$("#right_roles").datagrid("getRows");
                var exist = false;
                $.each(rows, function () {
                    if (this.Id == row.Id) {
                        exist = true;
                    }
                })
                if (exist == false) {
                    top.$("#right_roles").datagrid("appendRow", row);
                }
            }
        };
        var moveLeft = function (row) {
            if (row) {
                var index = top.$("#right_roles").datagrid("getRowIndex", row);
                top.$("#right_roles").datagrid("deleteRow", index);
            }
        };
        var editPwd = function () {
            var row = $("#dg").datagrid("getSelected");
            if (row) {
                var pDialog = top.$.dialogX({
                    href: "/Bas/User/EditPwd",
                    title: "修改密码",
                    width: 300,
                    height: 200, iconCls: "icon-key",
                    submit: function () {
                        if (top.$("#uiform").form("validate")) {
                            com.ajax({
                                url: "/Bas/User/SavePwd",
                                data: {
                                    action: "SavePwd",
                                    userId: row.ID,
                                    pwd: top.$("#txt_newpass").val()
                                },
                                success: function (result) {
                                    if (result.Success == true) {
                                        com.msg.info("密码修改成功！");
                                        pDialog.dialog("close");
                                    }
                                    else {
                                        com.msg.error("密码修改失败！");
                                    }
                                }
                            });
                        }
                    }
                });
            }
            else {
                com.msg.warning("请选择一个用户！");
            }
        };

        var authorize = {
            lastIndex: 0,
            run: function () {
                var user = $("#dg").datagrid("getSelected");
                if (!user) {
                    com.msg.warning('请选择帐号！');
                    return false;
                }
                var ad = top.$.dialogX({
                    max: true, title: '分配权限-用户名：' + user.USERNAME,
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
                        var d = authorize.getChanges(user);
                        if (d) {
                            com.ajax({
                                url: "/Bas/User/Authorize",
                                data: { d: d },
                                success: function (result) {
                                    if (result > 0) {
                                        com.msg.info("权限分配成功！");
                                        ad.dialog("close");
                                    }
                                    else {
                                        com.msg.error("权限分配失败！");
                                    }
                                }
                            });
                        } else {
                            com.msg.error("您没有分配任何权限！");
                        }
                    }
                });

                var nb = top.$("#nb").treegrid({
                    title: "导航菜单",
                    url: "/Bas/User/GetMenu?Id=" + user.ID,
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
            getChanges: function (user) {
                var rows = top.$("#nb").treegrid("getChildren");
                var o = { userId: user.ID, menus: [] };
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
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="layout">
        <%-- <input id="getname" style="display: none" />--%>
        <!--<div region="west" title="部门树" iconcls="icon-chart_organisation" split="true" style="width: 220px; padding: 5px">-->
        <div region="west" split="true" style="width: 180px; padding: 5px">
            <ul id="areatree"></ul>
        </div>
        <div region="center" title="用户账号列表" iconcls="icon-users">
            <!--顶部工具栏-->
            <div id="toolbar">
                <%=Html.ToolBar(Model) %>
                <%=Html.LinkButton("a_role","icon-group","角色",true) %>
                <%=Html.LinkButton("a_authorize","icon-group_gear","授权") %>
                <%=Html.LinkButton("a_editPwd","icon-key","修改密码",true) %>
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
