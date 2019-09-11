<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<head>
    <title>后台管理系统</title>
    <link type="text/css" rel="stylesheet" href="/content/css/icon.css" />
    <link type="text/css" rel="stylesheet" href="/scripts/easyui/themes/gray/easyui.css" />
    <link type="text/css" rel="stylesheet" href="/content/css/common.css" />
    <link type="text/css" rel="stylesheet" href="/content/img/login/User_Login.css">
    <script type="text/javascript" src="/scripts/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="/scripts/jQuery.cookie.js"></script>
    <script type="text/javascript" src="/scripts/easyui/jquery.easyui.min.js"></script>
    <script type="text/javascript" src="/scripts/easyui/easyui.extensions.js"></script>
    <script type="text/javascript" src="/content/js/common.js"></script>
    <script type="text/javascript">
        $(function () {
            $("#btnLogin").click(login);

            //响应键盘的回车事件
            $(this).keydown(function (event) {
                if (event.keyCode == 13) {
                    event.returnValue = false;
                    event.cancel = true;
                    return login();
                }
            });
        });

        var login = function () {
            var langName = $("#lang").val();
            com.ajax({
                url: "/bas/Home/Login",
                data: { lang: langName, username: $("#username").val(), password: $("#password").val() },
                success: function (result) {
                    if (result.success == true) {
                        com.lang.write(langName);
                        location.href = "/";
                    }
                    else {
                        com.msg.error(result.message);
                    }
                }
            });
        };
    </script>
</head>
<body id="userlogin_body">
    <form>
        <div></div>
        <div id="user_login">
            <dl>
                <dd id="user_top">
                    <ul>
                        <li class="user_top_l"></li>
                        <li class="user_top_c"></li>
                        <li class="user_top_r"></li>
                    </ul>
                    <dd id="user_main">
                        <ul>
                            <li class="user_main_l"></li>
                            <li class="user_main_c">
                                <div class="user_main_box">
                                    <ul>
                                        <li class="user_main_text">User Name： </li>
                                        <li class="user_main_input">
                                            <input id="username" name="username" type="text" value="admin" style="font-weight: bold; width: 160px;" />
                                        </li>
                                    </ul>
                                    <ul>
                                        <li class="user_main_text">Password： </li>
                                        <li class="user_main_input">
                                            <input id="password" name="password" type="password" value="123456" style="font-weight: bold; width: 160px;" />
                                        </li>
                                    </ul>
                                    <ul>
                                        <li class="user_main_text">Language： </li>
                                        <li class="user_main_input">
                                            <select id="lang" style="width: 164px;">
                                                <option value="zhcn" selected="selected">简体中文</option>
                                                <option value="en">English</option>
                                            </select>
                                        </li>
                                    </ul>
                                </div>
                            </li>
                            <li class="user_main_r">
                                <input id="btnLogin" type="button" value="Login" title="click to login" class="IbtnEnterCssClass"
                                    style="cursor: pointer; border: none; background: url('/content/img/login/user_botton.gif')" />
                            </li>
                        </ul>
                        <dd id="user_bottom">
                            <ul>
                                <li class="user_bottom_l"></li>
                                <li class="user_bottom_c"></li>
                                <li class="user_bottom_r"></li>
                            </ul>
                        </dd>
            </dl>
        </div>
        <div></div>
    </form>
</body>
</html>
