<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/content/css/css3btn.css" rel="stylesheet" />
    <script type="text/javascript" src="/Bas/Personal/GetConfigJson"></script>
    <script type="text/javascript">
        //系统全局设置
        var _data = {
            theme: [{ "title": "默认皮肤", "name": "default" },
                { "title": "流行灰", "name": "gray" },
                { "title": "Metro", "name": "metro" },
                { "title": "黑色", "name": "black" },
                { "title": "绿色", "name": "green" },
                { "title": "Bootstrap", "name": "Bootstrap" }
            ],
            navType: [
                { "id": "AccordionSmallIcon", "text": "手风琴菜单(小图标二级)", "selected": true },
                { "id": "AccordionLargeIcon", "text": "手风琴菜单(大图标二级)" },
                { "id": "AccordionTree", "text": "手风琴+树形菜单(二级以上)" },
                { "id": "Tree", "text": "树形菜单" }
            ]
        };

        $(function () {
            initCtrl();
            $("#btnok").click(saveConfig);
            $("body").css("overflow", "auto");
        });

        function initCtrl() {
            $("#txt_theme").combobox({
                data: _data.theme, panelHeight: "auto", editable: false, valueField: "name", textField: "title"
            });

            $("#txt_nav_showtype").combobox({
                data: _data.navType, panelHeight: "auto", editable: false, valueField: "id", textField: "text", width: 180,
                onSelect: function (item) {
                    $("#imgPreview").attr("src", "/content/img/menustyles/" + item.id + ".png");
                }
            });

            $("#imgPreview").click(function () {
                var src = $(this).attr("src");
                top.$.dialogX({
                    content: '<img src="' + src + '" />',
                    width: 665,
                    height: 655,
                    title: "效果图预览",
                    showBtns: false
                });
            });

            $("#txt_grid_rows").val(20).numberspinner({ min: 10, max: 500, increment: 10 });

            if (sys_config) {
                $("#txt_theme").combobox("setValue", sys_config.theme.name);
                $("#txt_nav_showtype").combobox("setValue", sys_config.showType);
                $("#txt_grid_rows").numberspinner("setValue", sys_config.gridRows);
                $("#imgPreview").attr("src", "/content/img/menustyles/" + sys_config.showType + ".png");
            }
        }

        function saveConfig() {
            var theme = $("#txt_theme").combobox("getValue");
            var navtype = $("#txt_nav_showtype").combobox("getValue");
            var gridrows = $("#txt_grid_rows").numberspinner("getValue");

            var findThemeObj = function () {
                var obj = null;
                $.each(_data.theme, function (i, n) {
                    if (n.name == theme)
                        obj = n;
                });
                return obj;
            };
            var configObj = { theme: findThemeObj(), showType: navtype, gridRows: gridrows };

            var str = JSON.stringify(configObj);

            com.ajax({
                url: "/Bas/Personal/Save",
                data: { json: str },
                success: function (result) {
                    if (result.Success == true) {
                        com.msg.info("全局设置保存成功,按F5看效果!");
                    }
                    else {
                        com.msg.error("全局设置保存失败！");
                    }
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="sysconfig" style="margin: 10px;">

        <h1>皮肤设置</h1>
        <div class="c">
            <ul>
                <li>
                    <div>皮肤：</div>
                    <input id="txt_theme" name="theme" /></li>
            </ul>
        </div>
        <h1>菜单设置</h1>
        <div class="c">
            <ul>
                <li>
                    <div>显示方式：</div>
                    <input id="txt_nav_showtype" name="navshowtype" /></li>
                <li>
                    <div>&nbsp;</div>
                    <img id="imgPreview" title="点击看大图" src="/bas/img/menuStyles/Accordion.png" style="width: 200px; margin-top: 3px; padding: 2px; border: 1px solid #ccc; display:none;" alt="" /></li>
            </ul>
        </div>
        <h1>数据表格设置</h1>
        <div class="c">
            <ul>
                <li>
                    <div>每页记录数：</div>
                    <input type="text" id="txt_grid_rows" name="gridrows" /></li>
            </ul>
        </div>
    </div>

    <div style="margin: 140px; width: 160px; margin-top: 40px; font-family: 'Microsoft YaHei'">

        <a id="btnok" href="javascript:void(0);" class="buttonHuge button-blue">保存设置</a>

    </div>
</asp:Content>
