<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {

            var model = new listModel();
            //对象1：列表页面对象
            var list = {
                controller: "/Bas/Nav", //控制器
                dSize: { width: 4, height: 40 }, //偏移量
                dg: {
                    typeName: "treegrid", //数据表格类型名称
                    treeField: "NAVTITLE",
                    sortName: "SORTNO",
                    sortOrder: "asc",
                    pageSize: 10000,
                    pageList: [10000],
                    frozenColumns: [[
                        { title: "ID", field: "ID", width: 50 },
                        { title: "菜单名称", field: "NAVTITLE", width: 200 }
                    ]],
                    columns: [[
                        { title: "图标", field: "ICONCLS", width: 80, align: "center", formatter: com.formatter.icon },
                        { title: "标记", field: "NAVTAG", width: 150 },
                        { title: "链接地址", field: "LINKURL", width: 360 },
                        { title: "是否显示", field: "ISVISIBLE", width: 80, align: "center", formatter: com.formatter.check },
                        { title: "排序", field: "SORTNO", width: 80, align: "center" }
                    ]]
                }
            };
            //对象2：编辑页面对象
            var edit = {
                title: "导航菜单信息", //标题
                size: { width: 580, height: 400 }, //页面大小
                onLoad: function (isAdd, row) { //页面加载事件
                    top.$("#txt_parentid").combotree({
                        url: "/Bas/Nav/GetComboTreeNav",
                        panelWidth: 180,
                        editable: false,
                        checkbox: true,
                        lines: true,
                        onSelect: function (item) {
                        }
                    }).combotree("setValue", -1);
                    showIcon(); //选取图标
                    top.$('#txt_orderid').numberspinner();

                    if (isAdd == false) {
                        top.$("#txt_ptag").val(row.NAVTAG);
                        top.$("#txt_title").val(row.NAVTITLE);
                        top.$("#txt_titleEN").val(row.NAVTITLEEN);
                        top.$("#txt_url").val(row.LINKURL);
                        top.$("#txt_iconcls").val(row.ICONCLS);
                        top.$("#smallIcon").attr("class", "icon " + row.ICONCLS);
                        top.$("#txt_parentid").combotree("setValue", row.PARENTID > 0 ? row.PARENTID : -1);
                        if (row.ISVISIBLE == true) {
                            top.$("#chkvisible").attr("checked", "checked");
                        }
                        else {
                            top.$("#chkvisible").removeAttr("checked");
                        }
                        top.$("#txt_orderid").numberspinner("setValue", row.SORTNO);
                        top.$("#txt_iconurl").val(row.ICONURL);
                        top.$("#txt_bigimgurl").val(row.BIGIMAGEURL);
                        top.$("#imgBig").attr("src", row.BIGIMAGEURL);
                    }
                    else {
                        if (row) {
                            top.$("#txt_parentid").combotree("setValue", row.ID);
                        }
                        top.$("#txt_title").focus();
                    }
                }
            };
            model.bind(list, edit);

            //其它初始化
            $("#dg").datagrid("getPager").height(0);
            $("#a_setbtn").click(setButton);
        });

        var showIcon = function () {
            top.$("#selecticon").click(function () {
                var iconDialog = top.$.dialogX({
                    iconCls: "icon-application_view_icons",
                    href: "/content/css/iconlist.htm?v=" + Math.random(),
                    title: "选取图标", width: 800, height: 600, showBtns: false,
                    onLoad: function () {
                        top.$("#iconlist li").attr("style", "float:left;border:1px solid #fff;margin:2px;width:16px;cursor:pointer").click(function () {
                            var iconCls = top.$(this).find("span").attr("class").replace("icon ", "");
                            top.$("#txt_iconcls").val(iconCls);
                            top.$("#txt_iconurl").val(top.$(this).attr("title"));
                            top.$("#smallIcon").attr("class", "icon " + iconCls);

                            iconDialog.dialog("close");
                        }).hover(function () {
                            top.$(this).css({ "border": "1px solid red" });
                        }, function () {
                            top.$(this).css({ "border": "1px solid #fff" });
                        });
                    }
                });
            });

            top.$("#selectBigIcon").click(function () {
                var iconDialog = top.$.dialogX({
                    iconCls: "icon-application_view_icons",
                    href: "/content/css/icon32list.html?v=" + Math.random(),
                    title: "选取图标",
                    width: 800,
                    height: 600,
                    showBtns: false,
                    onLoad: function () {
                        top.$("#icon32list li").css({ "float": "left", "width": "32px", "margin": "2px", "border": "1px solid #fff" }).hover(function () {
                            top.$(this).css({ "border": "1px solid red" });
                        }, function () {
                            top.$(this).css({ "border": "1px solid #fff" });
                        }).click(function () {
                            top.$("#txt_bigimgurl").val($(this).attr("title"));
                            top.$("#imgBig").attr("src", $(this).attr("title"));
                            iconDialog.dialog("close");
                        });
                    }
                });
            });
        };

        var setButton = function () {
            var row = $("#dg").datagrid("getSelected");
            if (row) {
                var setDialog = top.$.dialogX({
                    title: "菜单名称：" + row.NAVTITLE,
                    width: 440, height: 400, iconCls: "icon-cog", cache: false,
                    href: "/Bas/Nav/SetButton",
                    onLoad: function () {
                        top.$("#left_btns,#right_btns").datagrid({
                            nowrap: false, //折行
                            fit: true,
                            rownumbers: true, //行号
                            striped: true, //隔行变色
                            idField: "ID",//主键
                            singleSelect: true, //单选
                            columns: [[
                                {
                                    title: "图标", field: "ICONCLS", width: 38, formatter: function (v, d, i) {
                                        return "<span class='icon " + v + "'>&nbsp;</span>";
                                    }, align: "center"
                                },
                                { title: "按钮名称", field: "BUTTONTEXT", width: 80, align: "center" }
                            ]]
                        });
                        top.$("#left_btns").datagrid({
                            title: "所有按钮",
                            url: "/Bas/Button/GetButtonTable",
                            onDblClickRow: function (index, row) {
                                moveRight(row);
                            }
                        });
                        top.$("#right_btns").datagrid({
                            title: "已选按钮",
                            url: "/Bas/Nav/GetNavButton?NavId=" + row.ID,
                            onDblClickRow: function (index, row) {
                                moveLeft(row);
                            }
                        });
                        top.$("#btnRight").click(function () {
                            var row = top.$("#left_btns").datagrid("getSelected");
                            moveRight(row);
                        });
                        top.$("#btnLeft").click(function () {
                            var row = top.$("#right_btns").datagrid("getSelected");
                            moveLeft(row);
                        });
                        top.$("#btnUp").click(function () {
                            var index = top.$("#right_btns").datagrid("getSelectedIndex");
                            if (index > 0) {
                                var index2 = index - 1;
                                var rows = top.$("#right_btns").datagrid("getRows");
                                var tmpRow = rows[index2];
                                rows[index2] = rows[index];
                                rows[index] = tmpRow;
                                top.$("#right_btns").datagrid("loadData", rows).datagrid("selectRow", index2);
                            }
                        });
                        top.$("#btnDown").click(function () {
                            var index = top.$("#right_btns").datagrid("getSelectedIndex");
                            var rows = top.$("#right_btns").datagrid("getRows");
                            if (index < rows.length - 1) {
                                var index2 = index + 1;
                                var tmpRow = rows[index2];
                                rows[index2] = rows[index];
                                rows[index] = tmpRow;
                                top.$("#right_btns").datagrid("loadData", rows).datagrid("selectRow", index2);
                            }
                        });
                    },
                    submit: function () {
                        var rows = top.$("#right_btns").datagrid("getRows");
                        var btnIdArray = [];
                        $.each(rows, function () {
                            btnIdArray.push(this.ID);
                        });
                        var btnIdString = "";
                        if (btnIdArray.length > 0) {
                            btnIdString = btnIdArray.join(",");
                        }
                        com.ajax({
                            url: "/Bas/Nav/SaveNavButton",
                            data: {
                                navId: row.ID,
                                btnIdString: btnIdString
                            },
                            success: function (result) {
                                if (result.Success) {
                                    com.msg.info("菜单按钮设置成功！");
                                    setDialog.dialog("close");
                                    $("#dg").treegrid("reload");
                                }
                                else {
                                    com.msg.error("菜单按钮设置失败!");
                                }
                            }
                        });
                    }
                });
            }
            else {
                com.msg.warning("请选择导航菜单！");
            }
        };

        var moveRight = function (row) {
            if (row) {
                var rows = top.$("#right_btns").datagrid("getRows");
                var exist = false;
                $.each(rows, function () {
                    if (this.Id == row.ID) {
                        exist = true;
                    }
                })
                if (exist == false) {
                    top.$("#right_btns").datagrid("appendRow", row);
                }
            }
        };

        var moveLeft = function (row) {
            if (row) {
                var index = top.$("#right_btns").datagrid("getRowIndex", row);
                top.$("#right_btns").datagrid("deleteRow", index);
            }
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!--顶部工具栏-->
    <div id="toolbar">
        <%=Html.ToolBar(Model) %>
        <%=Html.LinkButton("a_setbtn","icon-cog","分配按钮",true) %>
    </div>
    <!--查询表单-->
    <div style="margin: 2px;">
        <form id="qform"></form>
    </div>
    <!--数据列表-->
    <table id="dg"></table>
</asp:Content>
