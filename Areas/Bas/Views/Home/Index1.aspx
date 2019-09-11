<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title><%=Lang.system_name %></title>
    <link type="text/css" rel="stylesheet" href="/content/css/common.css" />
    <link type="text/css" rel="stylesheet" href="/content/theme/<%=CurrentUser.ThemeName %>/style.css" />
    <link type="text/css" rel="stylesheet" id="easyui_theme" href="/scripts/easyui/themes/<%=CurrentUser.ThemeName %>/easyui.css" />
    <link type="text/css" rel="stylesheet" href="/content/css/icon.css" />
    <link type="text/css" rel="stylesheet" href="/content/css/css3btn.css" />
</head>

<body class="easyui-layout" style="overflow-y: hidden;" fit="true" scroll="no">
    <div id="loading" style="position: fixed; top: -50%; left: -50%; width: 200%; height: 200%; background: #fff; z-index: 100; overflow: hidden;">
        <img src="/content/img/ajax-loader.gif" style="position: absolute; top: 0; left: 0; right: 0; bottom: 0; margin: auto;" />
    </div>

    <noscript>
        <div style="position: absolute; z-index: 100000; height: 2046px; top: 0px; left: 0px; width: 100%; background: white; text-align: center;">
            <img src="/content/img/noscript.gif" alt='Sorry, please turn on the script support!' />
        </div>
    </noscript>

    <%=ViewData["Content"].ToString() %>

    <div id="mainPanel" region="center" style="background: #eee; overflow-y: hidden" border="false">
        <div id="tabs" class="easyui-tabs" fit="true">
            <div title="<%=Lang.welcome_use %>" style="padding: 0px; overflow: hidden;" id="home"></div>
        </div>
    </div>
    <div id="tabMenu" class="easyui-menu" style="width: 150px;">
        <div id="refresh" iconcls="icon-arrow_refresh"><%=Lang.tab_refresh %></div>
        <div class="menu-sep"></div>
        <div id="close"><%=Lang.tab_close %></div>
        <div id="closeall"><%=Lang.tab_close_all %></div>
        <div id="closeother"><%=Lang.tab_close_other %></div>
        <div class="menu-sep"></div>
        <div id="closeright"><%=Lang.tab_close_right %></div>
        <div id="closeleft"><%=Lang.tab_close_left %></div>
        <div class="menu-sep"></div>
        <div id="exit"><%=Lang.tab_exit %></div>
    </div>

    <!-- 加入隐藏的帧，用于检查用户的登录状态是否已过期 -->
    <iframe height="0" width="0" src="/Bas/Home/LoginCheck"></iframe>

    <script type="text/javascript" src="/scripts/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="/scripts/jQuery.cookie.js"></script>
    <script type="text/javascript" src="/scripts/easyui/jquery.easyui.min.js"></script>
    <%
        if (LangHelper.Name == LangEnum.ZHCN.ToString().ToLower())
        {
    %>
    <script type="text/javascript" src="/scripts/easyui/locale/easyui-lang-zh_CN.js"></script>
    <%
        }
        else
        {
    %>
    <script type="text/javascript" src="/scripts/easyui/locale/easyui-lang-en.js"></script>
    <%
        }
    %>

    <script type="text/javascript" src="/Scripts/easyui/jquery.edatagrid.js"></script>
    <script type="text/javascript" src="/Scripts/easyui/datagrid-filter.js"></script>
    <script type="text/javascript" src="/scripts/easyui/easyui-validate-rules.js"></script>
    <script type="text/javascript" src="/scripts/easyui/easyui.extensions.js?v=18"></script>
    <script type="text/javascript" src="/content/js/common.js?v=1"></script>
    <script type="text/javascript" src="/content/js/index.js?v=2"></script>
    <script type="text/javascript" src="/content/js/listModel.js?v=3"></script>
    <script type="text/javascript" src="/content/js/list2Model.js?v=4"></script>
    <script type="text/javascript" src="/bas/Personal/GetConfigJson"></script>
    <script type="text/javascript" src="/bas/Home/GetMenuData"> </script>
    <script type="text/javascript">
        $(function () {
            idx.init();
        });
    </script>
</body>
</html>
