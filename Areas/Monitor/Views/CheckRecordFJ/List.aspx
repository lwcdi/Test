<%@ Page Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

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
        function AfterLoadSuccess() {
            model = new listModel();
            var list = {
                controller: "/Monitor/CheckRecordFJ",//控制器
                dSize: { width: 295, height: 75 },//偏移量
                dg: { //数据表格
                    columns: [[
                                { title: "企业名称", field: "COMPANYNAME", width: 150, align: 'center' },
                                { title: "排口名称", field: "PKNAME", width: 100, align: 'center' },
                                { title: "修约时间", field: "CHECKTIME", width: 150, formatter: com.formatter.date, align: 'center' },
                                { title: "修约数据量", field: "COUNT", width: 100, align: 'center' },
                                {
                                    title: "修约详情", field: "ID", width: 80, align: 'center', formatter: function (value, row, index) {
                                        if (value) return '<a style="cursor:pointer" onClick=Details("' + value + '","' + row.CHECKTIME + '")>查看</a>';
                                    }
                                }
                    ]]
                }
            };

            //对象2：编辑页面对象
            var edit = {};
            model.bind(list, edit);
        };
        $(function () {
            mylayout.init();
            queryParamInit();
            //其它初始化
            $(window).resize(function () {
                mylayout.resize();
            });
        })

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

            $('#stime').datebox({
                required: false,
                editable: false,
            }).datebox('setValue', date);

            $('#etime').datebox({
                required: false,
                editable: false,
            }).datebox('setValue', date);
        }
        var openFJ = function (url) {
            window.open(url);
        }
        var Details = function (pkId, date) {
            var url = "/Monitor/CheckRecord/ListExt?stime=" + date + "&etime=" + date + "&PKId=P" + pkId + "&fs=" + '<%=ViewData["fs"]%>' + "&fq=" + '<%=ViewData["fq"]%>' + "&vocs=" + '<%=ViewData["vocs"]%>';
            var dialog = $('<iframe src="' + url + '" style="border: 0px; width: 100%; height: 600px; overflow: auto;"></iframe>').dialog({
                title: "修约详情",
                width: 1200,
                height: 600,
                cache: false,
                modal: true,
                buttons: [
                {
                    text: '关闭',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        dialog.dialog("close");
                    }
                }]
            });
        }
        var GetData = function () {
            $("#dg").datagrid({
                url: "/Monitor/CheckRecordFJ/List",//控制器
                queryParams: {
                    stime: $("#stime").datebox('getValue'),
                    etime: $("#etime").datebox('getValue'),
                    PKId: GetMutiSelected().PID.toString()
                },
            })
        }
    </script>
</asp:Content>

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
        <div region="center" title="修约凭证" iconcls="icon-clock">
            <!--顶部工具栏-->
            <form id="qform">
                <div id="toolbar">
                    <div style="float: left; padding: 5px;">
                        <span>下发时间：</span>
                        <select id="stime" name="stime" style="width: 175px;" class="txt03"></select>
                        <span>至：</span>
                        <select id="etime" name="etime" style="width: 175px;" class="txt03"></select>
                        <a plain="true" class="easyui-linkbutton" icon="icon-search" title="查询" onclick="GetData()">查询</a>
                    </div>
                </div>
            </form>
            <!--数据列表-->
            <table id="dg"></table>
        </div>
    </div>
</asp:Content>
