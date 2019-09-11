//主页后台动态输出的全局变量：menus,sys_config
var idx = {
    vars: { //变量
        homeTabTitle: "欢迎使用", //首页标签标题
        tabMaxCount: 10  //允许打开的Tab数量
    },
    //初始化方法(入口)
    init: function () { //初始化方法
        idx.initLang();
        idx.initTab();
        idx.initElse();
        idx.nav.init();
    },
    //初始化语言
    initLang: function () {
        com.lang.read(); //加载语言包
        var lname = $.cookie("langName", { path: "/" });
        if (lname == "en") {
            idx.vars.homeTabTitle = "Welcome Use";
        }
    },
    //初始化选项卡标签
    initTab: function () {
        $("#tabs").tabs({
            tools: [{
                iconCls: "icon-arrow_refresh",
                handler: function () {
                    var tab = $("#tabs").tabs("getSelected");
                    if (tab.panel("options").title != idx.vars.homeTabTitle) {
                        idx.doTab("refresh");
                    }
                    else { //刷新首页
                        $("#tabs").tabs("update", { tab: tab, options: { content: idx.createFrame("/Bas/Home/ConsoleIndex") } });
                    }
                    return false;
                }
            }, {
                iconCls: "panel-tool-close",
                handler: function () {
                    $.messager.confirm(com.lang.system_prompt, com.lang.tab_close_all_confirm, function (r) {
                        if (r) {
                            idx.doTab("closeall");
                        }
                    });
                }
            }],
            onContextMenu: function (e, title) {
                e.preventDefault();
                $("#tabMenu").menu("show", {
                    left: e.pageX,
                    top: e.pageY
                });
                $("#tabs").tabs("select", title);
            }
        });
        //增加双击关闭tab选项卡
        $(".tabs-inner").live("dblclick", function () {
            var subtitle = $(this).children(".tabs-closable").text();
            if (subtitle != idx.vars.homeTabTitle && subtitle != "")
                $("#tabs").tabs("close", subtitle);
        });
        $("#tabMenu").menu({
            onClick: function (item) {
                idx.doTab(item.id);
            }
        });
    },
    //初始化其它
    initElse: function () {
        $("#editpass").click(function () {
            idx.editMyPwd();
        });
        $("#home").html(idx.createFrame("/Bas/Home/ConsoleIndex"));
        $("#loginOut").click(function () {
            $.messager.confirm(com.lang.system_prompt, com.lang.exit_login_confirm, function (r) {
                if (r) {
                    location.href = "/Bas/Home/Logout";
                }
            });
        });
        $(window).load(function () {
            $("#loading").fadeOut();
        });
    },
    createFrame: function (url) {
        var html = "<div style='margin:5;padding:5; width:100%;height:100%; overflow: hidden;'>";
        html += "<iframe scrolling='auto' frameborder='0' style='width:100%;height:100%;' src='" + url + "' ></iframe>";
        html += "</div>";
        return html;
    },
    //菜单对象
    nav: {
        init: function () {
            if (sys_config.showType == "AccordionTree") {
                idx.nav.accordionTree(); //手风琴+树形菜单(二级以上)
            }
            else if (sys_config.showType == "Tree") {
                idx.nav.tree(); //树形菜单
            }
            else if (sys_config.showType == "AccordionLargeIcon") {
                idx.nav.accordion(true); //手风琴菜单(大图标二级)
            }
            else {
                idx.nav.accordion(false); //手风琴菜单(小图标二级)
            }
        },
        //手风琴菜单(二级)
        accordion: function (largeIcon) {
            var wnav = $("#wnav").accordion({ fit: true, border: false });
            var html = "";
            $.each(menus, function (i, m) {
                html = "";
                if (largeIcon == true) {
                    html += "<ul class='bigicon menuItem'>";
                    $.each(m.children, function () {
                        html += "<li><div>"
                            + "<a ref='" + this.id + "' href='#' rel='" + this.attributes.url + (-1 == this.attributes.url.lastIndexOf("?") ? "?" : "&") + "navid=" + this.id + "'>"
                            + "<span class='img' iconCls='" + this.iconCls + "'><img scr='" + this.attributes.BigImageUrl + "'/></span>"
                            + "<span class='nav'>" + this.text + "</span>"
                            + "</a></div></li>";
                    });
                }
                else {
                    html += "<ul class='smallicon menuItem'>";
                    $.each(m.children, function () {
                        html += "<li><div>"
                            + "<a ref='" + this.id + "' href='#' rel='" + this.attributes.url + (-1 == this.attributes.url.lastIndexOf("?") ? "?" : "&") + "navid=" + this.id + "'>"
                            + "<span iconCls='" + this.iconCls + "' class='icon " + this.iconCls + "'>&nbsp;</span>"
                            + "<span class='nav'>" + this.text + "</span>"
                            + "</a></div></li>";
                    });
                }
                html += "</ul";
                wnav.accordion("add", {
                    title: m.text,
                    content: html,
                    iconCls: m.iconCls,
                    border: false,
                    selected: (i == 0)
                });
            });
            $(".menuItem li").live({
                click: function () {
                    var a = $(this).children("div").children("a");
                    var tabTitle = $(a).children(".nav").text();

                    var url = $(a).attr("rel");
                    var menuid = $(a).attr("ref");
                    var icon = $(a).children(".icon").attr("class");

                    idx.addTab(tabTitle, url, icon);
                    $(".accordion li div").removeClass("selected");
                    $(this).children("div").addClass("selected");
                }
            }).find("div").live({
                mouseover: function () {
                    $(this).children("div").addClass("hover");
                },
                mouseout: function () {
                    $(this).children("div").removeClass("hover");
                }
            });
        },
        //手风琴+树形菜单(二级以上)
        accordionTree: function () {
            $.each(menus, function (i) {
                $("#wnav").append("<div style='padding:0px;' title='" + this.text + "' data-options='border:false,iconCls:\"" + this.iconCls + "\"'><ul id='nt" + i + "'></ul></div>");
            });
            $("#wnav").accordion({
                fit: true,
                border: false,
                onSelect: function (t, i) {
                    $("#nt" + i).tree({
                        lines: false,
                        animate: false,
                        data: menus[i].children,
                        onClick: function (node) {
                            if (node.attributes.url != "" && node.attributes.url != "#") {
                                idx.addTab(node.text, node.attributes.url + (-1 == node.attributes.url.lastIndexOf("?") ? "?" : "&") + "navid=" + node.id, node.iconCls);
                            } else {
                                $("#nt" + i).tree("toggle", node.target);
                            }
                        }
                    }).tree("collapseAll");
                }
            });
        },
        //树形菜单
        tree: function () {
            $("#wnav").tree({
                animate: true,
                lines: true,
                data: menus,
                onClick: function (node) {
                    if (node.attributes.url != "#" && node.attributes.url != '') {
                        idx.addTab(node.text, node.attributes.url + (-1 == node.attributes.url.lastIndexOf("?") ? "?" : "&") + 'navid=' + node.id, node.iconCls);
                    } else {
                        $('#wnav').tree('toggle', node.target);
                    }
                }
            });
        }
    },
    doTab: function (action) {
        var alltabs = $("#tabs").tabs("tabs");
        var currentTab = $("#tabs").tabs("getSelected");
        var allTabtitle = [];
        $.each(alltabs, function () {
            allTabtitle.push($(this).panel("options").title);
        });
        switch (action) {
            case "refresh":
                try {
                    var src = $(currentTab.panel("options").content).find("iframe").attr("src");
                    if (src) {
                        $("#tabs").tabs("update", { tab: currentTab, options: { content: idx.createFrame(src) } });
                    }
                }
                catch (e) { }
                break;
            case "close":
                var currtab_title = currentTab.panel("options").title;
                if (currtab_title != idx.vars.homeTabTitle) {
                    $("#tabs").tabs("close", currtab_title);
                }
                break;
            case "closeall":
                $.each(allTabtitle, function (i, n) {
                    if (n != idx.vars.homeTabTitle) {
                        $("#tabs").tabs("close", n);
                    }
                });
                break;
            case "closeother":
                var currtab_title = currentTab.panel("options").title;
                $.each(allTabtitle, function (i, n) {
                    if (n != currtab_title && n != idx.vars.homeTabTitle) {
                        $("#tabs").tabs("close", n);
                    }
                });
                break;
            case "closeright":
                var tabIndex = $("#tabs").tabs("getTabIndex", currentTab);

                if (tabIndex == alltabs.length - 1) {
                    alert("右边没有标签!");
                    return false;
                }
                $.each(allTabtitle, function (i, n) {
                    if (i > tabIndex) {
                        if (n != idx.vars.homeTabTitle) {
                            $("#tabs").tabs("close", n);
                        }
                    }
                });

                break;
            case "closeleft":
                var tabIndex = $("#tabs").tabs("getTabIndex", currentTab);
                if (tabIndex == 1) {
                    alert("首页不能关闭!");
                    return false;
                }
                $.each(allTabtitle, function (i, n) {
                    if (i < tabIndex) {
                        if (n != idx.vars.homeTabTitle) {
                            $("#tabs").tabs("close", n);
                        }
                    }
                });

                break;
            case "exit":
                $("#tabMenu").menu("hide");
                break;
        }
    },
    addTab: function (subtitle, url, icon) {
        if (url == "" || url == "#")
            return false;
        var tabCount = $("#tabs").tabs("tabs").length;
        var hasTab = $("#tabs").tabs("exists", subtitle);
        var add = function () {
            if (!hasTab) {
                $("#tabs").tabs("add", {
                    title: subtitle,
                    content: idx.createFrame(url),
                    closable: true,
                    icon: icon
                });
            } else {
                $("#tabs").tabs("select", subtitle);
                //doTab("refresh"); //选择TAB时刷新页面
            }
        };
        if (tabCount > idx.vars.tabMaxCount && !hasTab) {
            var msg = "<b>您打开的页面太多，如果再打开可能会导致页面加载慢的情况，是否继续打开? </b>";
            $.messager.confirm("System Prompt", msg, function (b) {
                if (b) {
                    add();
                }
                else {
                    return false;
                }
            });
        } else {
            add();
        }
    },
    editMyPwd: function () {
        var str_editpass = '<form id="uiform"><table class="grid">';
        str_editpass += '<tr><td>' + com.lang.login_name + '：</td><td><span id="loginname"></span></td></tr>';
        str_editpass += '<tr><td>' + com.lang.old_pwd + '：</td><td><input required="true" id="txtOldPassword" name="password" type="password" class="txt03 easyui-validatebox" /></td></tr>';
        str_editpass += '<tr><td>' + com.lang.new_pwd + '：</td><td><input validType="safepass"  required="true" id="txtNewPassword" name="password" type="password" class="txt03 easyui-validatebox" /></td></tr>';
        str_editpass += '<tr><td>' + com.lang.pwd_confirm + '：</td><td><input data-options="required:true" validType="equals[\'#txtNewPassword\']" id="txtReNewPassword" type="password" class="txt03 easyui-validatebox" /></td></tr>';
        str_editpass += '</table></form>';
        top.$.dialogX({
            width: 330, height: 240, title: com.lang.update_pwd, iconCls: "icon-key", content: str_editpass, submit: function () {
                if ($("#uiform").form("validate")) {
                    com.ajax({
                        url: "/Bas/User/EditPwd",
                        data: { oldPwd: $("#txtOldPassword").val(), newPwd: $("#txtNewPassword").val() },
                        success: function (result) {
                            if (result.Success) {
                                com.msg.alert(com.lang.update_pwd_success);
                                location.href = "/Bas/Home/Logout";
                            } else
                                alert(result.Message);
                        }
                    });
                }
            }
        });

        $("#loginname").text($("#curname").text());
    }
};

