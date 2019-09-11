<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .Bar {
            width: 99%;
            height: 30px;
            line-height: 30px;
            color: #999999;
            background-color: #f2f2f2;
            border: 1px solid #e4e4e4;
            margin: 1px 5px 1px 5px;
            padding-left: 5px;
            padding-top: 3px;
        }

        .TEXTBTN {
            border: solid 1px #c9c9c9;
            display: inline-block;
            /*font-family: '华文中宋 Regular', '华文中宋';
            font-size: 12px;*/
            color: #333333;
            background-color: #f5f5f5;
            width: 64px;
            height: 24px;
            line-height: 24px;
            text-align: center;
            margin-right: 1px;
            float: left;
        }

        .TEXTBTNSEL {
            background-color: #d7d7d7;
        }

        #toolbar {
            height: 32px !important;
            padding: 2px !important;
        }

        .timeRang {
            display: none;
        }
    </style>
    <script>
        $(function () {
            mylayout.init();
            fieldInit();
        });
        //页面字段初始化--下拉框
        var fieldInit = function () {
            $("a.TEXTBTN").click(function (src) {
                var obj = src.srcElement || src.target;
                $(obj).siblings("a.TEXTBTN").removeClass("TEXTBTNSEL");
                $(obj).addClass("TEXTBTNSEL");
                if ($(obj).hasClass("FUN00")) { alerttype = 0; getData(); }
                if ($(obj).hasClass("FUN01")) { alerttype = 1; getData(); }
                if ($(obj).hasClass("FUN02")) { alerttype = 2; getData(); }
                if ($(obj).hasClass("FUN03")) { alerttype = 3; getData(); }
            });

            var date = new Date();
            $('#etime').datebox({
                required: false,
                editable: false,
            }).datebox('setValue', date.format('yyyy-MM-dd'));
            date.setDate(date.getDate() - 7);
            $('#stime').datebox({
                required: false,
                editable: false,
            }).datebox('setValue', date.format('yyyy-MM-dd'));

            getData();
        };
        var mylayout = {
            init: function () {
                var size = { width: $(window).width(), height: $(window).height() };
                $("#layout").width(size.width - 4).height(size.height - 4).layout();
                var center = $("#layout").layout("panel", "center");
                center.panel({
                    onResize: function (w, h) {
                        //$("#dg").datagrid("resize", { width: w - 3, height: h - 70 });
                    }
                });
            }
            ,
            resize: function () {
                mylayout.init();
                $("#layout").layout("resize");
            }
        };

        function showDetail(id, showType, name) {
            debugger;
            $("#detailDialog").show();
            $("#detailIF").attr("src", "Edit?id=" + id + "&showType=" + showType);
            var buttons = [
                {
                    text: '确定',
                    iconCls: 'icon-save',
                    handler: function () {
                        debugger;
                        saveCompanyData();
                        $('#dg').datagrid('reload');
                        dialog.dialog("close");
                        $(detailIFName.document).empty();
                    }
                },
                 {
                     text: '取消',
                     iconCls: 'icon-cancel',
                     handler: function () {
                         dialog.dialog("close");
                         $(detailIFName.document).empty();
                     }
                 }];
            if (showType) buttons.shift();
            var dialog = $("#detailDialog").dialog({
                title: "告警处理-" + name,
                width: 500,
                height: 550,
                cache: false,
                modal: true,
                closable: false,
                buttons: buttons
            });
        }


        var pkid = "", companyid = "", alerttype = 0;
        function onNodeSelected(node) {
            debugger;
            pkid = "", companyid = "";
            var node = GetTreeSelected();
            if (node) {
                if (1 == node.OBJTYPE)
                    pkid = node.ID;
                else
                    companyid = node.ID;
            }
            getData();
        }

        //初始化企业信息列表
        function getData() {
            $("#dg").datagrid({
                url: "/BaseData/AlertCheck/GetAlert",
                queryParams: {
                    pkid: pkid,
                    companyid: companyid,
                    stime: $("#stime").datebox("getValue"),
                    etime: $("#etime").datebox("getValue"),
                    alerttype: alerttype
                },
                pagination: true,
                pageSize: 50,
                singleSelect: true,
                striped: true,
                rownumbers: true,
                onLoadSuccess: function (data) {
                    $("#statAll").text(data.total);
                    $("#statNo").text(data.no);
                    $("#statYes").text(data.yes);
                },
                columns: [[
                    { title: "污染源单位", field: "COMPANYNAME", width: 150 },
                    { title: "监控点名称", field: "PKNAME", width: 100 },
                    { title: "污染物名称", field: "ITEMNAME", width: 100, hidden: 1 !== alerttype },
                    { title: "监控点类型", field: "PKTYPENAME", width: 100, hidden: 2 !== alerttype },
                    { title: "数采仪MN", field: "MN", width: 100, hidden: 3 !== alerttype },
                    { title: "报警门限", field: "LIMIT", width: 60, align: "right" },
                    { title: "报警值", field: "VALUE", width: 60, align: "right" },
                    {
                        title: "处置状态", field: "STATE", width: 60, formatter: function (value, row, index) {
                            if (1 == value)
                                return "已处理";
                            else
                                return "待处理";
                        }
                    },
                    { title: "开始时间", field: "STARTTIME", width: 150 },
                    { title: "结束时间", field: "ENDTIME", width: 150 },
                    {
                        title: "持续时间", field: "DURATION", width: 80, align: "right", formatter: function (value, row, index) {
                            return value + "分钟";
                        }
                    },
                    {
                        title: "操作", field: "ID", width: 100, align: "center",
                        formatter: function (value, row, index) {
                            var txt = row.STATE ? "查看" : "处置";
                            return '<span title="' + txt + '" class="operatorBtn" style="cursor:pointer;" onclick="showDetail(\'' + value + '\',' + row.STATE + ',\'' + row.PKNAME + '\')">' + txt + '</span>';
                        }
                    }
                ]]
            });
        }

        function saveCompanyData() {
            detailIFName.SaveSetting();
        }
    </script>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="layout" class="easyui-layout" data-options="fit:true">
        <div data-options="region:'west',split:false,border:1,collapsible:true,title:'企业排口信息'"
            style="width: 260px;">
            <%
                Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = ViewBag.controllerParam });
            %>
        </div>
        <div data-options="region:'center',split:false,border:0">
            <div class="easyui-layout" data-options="fit:true">
                <div data-options="region:'north',split:false,border:0,collapsible:false,title:''"
                    style="height: 40px; overflow: hidden;">
                    <div class="Bar">
                        <a href="javascript:void(0);" class="FUN00 TEXTBTN TEXTBTNSEL">污染物超标</a>
                        <a href="javascript:void(0);" class="FUN01 TEXTBTN">污染物异常</a>
                        <a href="javascript:void(0);" class="FUN02 TEXTBTN">排放量异常</a>
                        <a href="javascript:void(0);" class="FUN03 TEXTBTN">数采仪掉线</a>
                    </div>
                </div>
                <div data-options="region:'center',split:false,border:0,title:''">
                    <div id="divTool" style="width: atuo;">
                        <div class="Bar">
                            <span>时间：</span>
                            <select id="stime" name="stime" style="width: 175px;" class="txt03"></select>
                            <span>至：</span>
                            <select id="etime" name="etime" style="width: 175px;" class="txt03"></select>
                            <a plain="true" class="easyui-linkbutton" icon="icon-search" title="查询" onclick="getData()">查询</a>
                            <div style="float: right; margin-right: 10px;"><span>全部（<span id="statAll"></span>）</span><span>待处理（<span id="statNo"></span>）</span><span>已处理（<span id="statYes"></span>）</span></div>
                        </div>
                    </div>
                    <!--数据列表-->
                    <div id="dg">
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%--信息弹出窗体--%>
    <div id="detailDialog" style="display: none;">
        <iframe id="detailIF" name="detailIFName" style="border: 0px; width: 100%; height: 450px; overflow: hidden;"></iframe>
    </div>
</asp:Content>

