<%@ Page Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="layout">
        <div region="west" split="true" style="width: 280px; padding: 5px">
            <%
                ViewDataDictionary dict = new ViewDataDictionary();
                dict.Add("fs", ViewData["fs"]);
                dict.Add("fq", ViewData["fq"]);
                dict.Add("vocs", ViewData["vocs"]);
                dict.Add("selectId", ViewData["PKId"]);
                dict.Add("multiSelect", true);
                Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = dict });
            %>
        </div>
        <div region="center" title="修约进度" iconcls="icon-clock">
            <!--顶部工具栏-->
            <form id="qform">
                <div id="toolbar">
                    <div style="float: left; padding: 5px;">
                        <span>时间：</span>
                        <select id="stime" name="stime" style="width: 175px;" class="txt03"></select>
                        <span>至：</span>
                        <select id="etime" name="etime" style="width: 175px;" class="txt03"></select>
                        <a plain="true" class="easyui-linkbutton" icon="icon-search" title="查询" onclick="GetData()">查询</a>
                        <a plain="true" class="easyui-linkbutton" title="最近7天" onclick="GetLast7Days()">最近7天</a>
                        <a plain="true" class="easyui-linkbutton" title="最近30天" onclick="GetLast30Days()">最近30天</a>
                    </div>
                </div>
            </form>
            <!--数据列表-->
            <table id="dg"></table>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Bas/Button/GetBtnColumns"></script>
    <style type="text/css">
        #toolbar {
            height: 32px !important;
            padding: 2px !important;
        }
    </style>
    <script type="text/javascript">
        var model;

        $(function () {
            mylayout.init();
            queryParamInit();
            //其它初始化
            $(window).resize(function () {
                mylayout.resize();
            });
            //setTimeout("GetData()", "200");
        });
        function AfterLoadSuccess() {
            model = new listModel();
            var list = {
                controller: "/Monitor/CheckStatistic",//控制器
                dSize: { width: 295, height: 75 },//偏移量
                dg: { //数据表格
                    columns: []
                }
            };

            //对象2：编辑页面对象
            var edit = {};
            model.bind(list, edit);
        }
        var mylayout = {
            init: function () {
                var size = { width: $(window).width(), height: $(window).height() };
                $("#layout").width(size.width - 4).height(size.height - 4).layout();
                var center = $("#layout").layout("panel", "center");
                center.panel({
                    onResize: function (w, h) {
                        $("#dg").datagrid("resize", { width: w - 5, height: h - 85 });
                    }
                });
            },
            resize: function () {
                mylayout.init();
                $("#layout").layout("resize");
            }
        };
        //var openAfter 
        var queryParamInit = function () {
            var currentDate = new Date();
            date = currentDate.getFullYear() + "\-" + (currentDate.getMonth() + 1) + "\-" + currentDate.getDate();
            var stime = '<%=ViewData["stime"]%>';
            var etime = '<%=ViewData["etime"]%>';
            $('#stime').datebox({
                required: false,
                editable: false,
            }).datebox('setValue', stime != '' ? stime : date);

            $('#etime').datebox({
                required: false,
                editable: false,
            }).datebox('setValue', etime != '' ? etime : date);
        }
        var openFJ = function (url) {
            window.open(url);
        }
        var GetLast7Days = function () {
            $("#stime").datebox('setValue', new Date().addDays(-7).format('yyyy-MM-dd'));
            $("#etime").datebox('setValue', new Date().format('yyyy-MM-dd'));
            GetData();
        }
        var GetLast30Days = function () {
            $("#stime").datebox('setValue', new Date().addDays(-30).format('yyyy-MM-dd'));
            $("#etime").datebox('setValue', new Date().format('yyyy-MM-dd'));
            GetData();
        }
        var GetData = function () {
            var col = new Array();
            col.push({
                field: 'COMPANY_NAME', title: '污染源企业', align: 'center', width: 200
            });
            col.push({
                field: 'PK_NAME', title: '排口', align: 'center', width: 200
            });
            var stime = $("#stime").datebox('getValue');
            var etime = $("#etime").datebox('getValue');
            var i = new Date();
            for (i = new Date(stime) ; i <= new Date(etime) ; i = i.addDays(1)) {
                var date = com.formatter.date(i);
                col.push({
                    field: date, title: date, align: 'center', formatter: function (value, row, index) {
                        if (value) return value;
                        else return '直出有效';
                    }
                });
            }
            $("#dg").datagrid({
                url: "/Monitor/CheckStatistic/GetData",
                queryParams: {
                    PKId: GetMutiSelected().PID.toString(),
                    stime: stime,
                    etime: etime,
                },
                columns: [col]
            });
        }
    </script>
</asp:Content>

