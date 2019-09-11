<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>机动车排污监管信息综合管理平台</title>
    <link type="text/css" rel="stylesheet" href="/content/css/icon.css" />
    <link type="text/css" rel="stylesheet" href="/scripts/easyui/themes/gray/easyui.css" />
    <link type="text/css" rel="stylesheet" href="/content/img/login/login.css" />
    <script type="text/javascript" src="/scripts/jquery-1.8.3.min.js"></script>
    <script type="text/javascript" src="/scripts/jQuery.cookie.js"></script>
    <script type="text/javascript" src="/scripts/easyui/jquery.easyui.min.js"></script>
    <script type="text/javascript" src="/scripts/easyui/easyui.extensions.js"></script>
    <script type="text/javascript" src="/content/js/common.js"></script>
    <script>
        (function () {
            if (!
                /*@cc_on!@*/
            0) return;
            var e = "abbr, article, aside, audio, canvas, datalist, details, dialog, eventsource, figure, footer, header, hgroup, mark, menu, meter, nav, output, progress, section, time, video".split(', ');
            var i = e.length;
            while (i--) {
                document.createElement(e[i])
            }
        })()
    </script>
    <script type="text/javascript">
        $(function () {
            $("#username").focus();
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
            var langName = "zhcn";
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
<body>
    <div class="container">
        <section id="content">
            <form>
                <h1>机动车排污监管信息综合管理平台</h1>
                <div>
                    <input type="text" placeholder="请输入用户名" required="" id="username" value="admin" />
                </div>
                <div>
                    <input type="password" placeholder="请输入密码" required="" id="password" value="123456" />
                </div>
                <div>
                    <input type="button" value="登录" id="btnLogin" title="click to login" />
                </div>
            </form>
            <!-- form -->
            <div class="button">
                <a href="#"></a>
            </div>
            <!-- button -->
        </section>
        <!-- content -->
    </div>
    <!-- container -->
</body>
</html>
