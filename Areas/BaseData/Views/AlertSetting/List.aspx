<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .photo {
            float: left;
            background-repeat: no-repeat;
            background-size: contain;
            border: 1px solid #ddd;
            width: 376px;
            height: 170px;
            margin-top: 10px;
            margin-left: 10px;
        }

        .datagrid-cell, .datagrid-cell-group, .datagrid-header-rownumber, .datagrid-cell-rownumber {
            padding: 0px 15px;
        }
    </style>
    <link type="text/css" rel="stylesheet" href="/Content/css/Checkbox.css" />
    <script src="/Content/js/getQueryStr.js"></script>
    <script type="text/javascript">
        var isVOCs = "1";

        $(document).ready(function () {
            if (getQueryStr("isVOCs")) {
                isVOCs = getQueryStr("isVOCs");
            }
            adjustSize();
            $(window).resize(function () {
                adjustDg();
            });

            //$("#opBegDate").datebox();
            //$("#opEndDate").datebox();

            $("#btnSearch").click(function () {
                getData();
            });
            $("#btnAdd").click(function () {
                GetCompanyID();
                showDetail(id, "0", isVOCs);
            });

            initCombobox();
            getData();
        });

        function adjustSize() {
            $("#dg").height($(window).height() - $("#header").outerHeight() - 15);
        }
        function adjustDg() {
            $("#dg").datagrid('resize', { height: $(window).height() - $("#header").outerHeight() - 15 });
        }

        //初始化企业信息列表
        function getData() {
            var param = "";
            param += "CompanyName=" + encodeURIComponent($("#CompanyName").val());
            param += "&AREA=" + $("#AREA").combobox('getValue');
            param += "&HYLB=" + $("#BASTYPE").combobox('getValue');
            param += "&GZCD=" + $("#GZCD").combobox('getValue');
            param += "&ALERTSTATE=" + $("#ALERTSTATE").combobox('getValue');
            //param += "&isVOCs=" + isVOCs;
            //param += "&ETIME=" + $("#opEndDate").datebox('getText');
            $("#dg").datagrid({
                url: "/BaseData/AlertSetting/GetData?" + param,
                idField: "CODE",
                pagination: true,
                pageSize: 50,
                singleSelect: true,
                striped: true,
                rownumbers: true,
                columns: [[
                    { title: "企业名称", field: "NAME", width: 150 },
                    { title: "地区名称", field: "NAREA", width: 150 },
                    { title: "污染源地址", field: "ADDRESS", width: 250 },
                    { title: "行业类别", field: "NBASTYPE", width: 150 },
                    { title: "关注程度", field: "NGZCD", width: 150 },
                    {
                        title: "告警状态", field: "ALERTSTATE",
                        formatter: function (value, row, index) {
                            //return '<input type="checkbox" id="checkbox_state_' + row.ID2 + '" onclick="return false" onchange="alert(' + row.ID2 + ')" /><label for="checkbox_state_' + row.ID2 + '"></label>';
                            return value ? "开启" : "关闭";
                        }
                    },
                    {
                        title: "操作", field: "ID2",
                        formatter: function (value, row, index) {
                            return '<span title="设置" class="operatorBtn" style="cursor:pointer;" onclick="showDetail(\'' + value + '\',\'1\',\'' + row.NAME + '\')">设置</span>';
                        }
                    }
                ]]
            });
        }

        //获取企业信息ID，用于新增
        function GetCompanyID() {
            com.ajax({
                url: "/BaseData/VOCsCompany/GetCompanyID",
                async: false,
                success: function (result) {
                    if (result.success == true) {
                        id = result.data;
                    }
                }
            });
        }

        //删除新增的临时数据
        function delCompanyByID() {
            com.ajax({
                url: "/BaseData/VOCsCompany/delCompanyByID",
                data: { id: id },
                async: false,
                success: function (result) {
                    if (result.success == true) {
                        id = result.data;
                    }
                }
            });
        }

        //删除企业信息
        function dg_Delete(id) {
            $.messager.confirm('警告', '确定删除该条数据吗?', function (r) {
                if (r) {
                    //alert(id);
                    com.ajax({
                        url: "/BaseData/VOCsCompany/DelCompanyInfoByID",
                        data: { id: id },
                        async: false,
                        success: function (result) {
                            if (result.success == true) {
                                com.msg.alert(result.data);
                                $("#dg").datagrid("reload");
                            }
                        }
                    });
                }
            });
        }

        //初始化下拉框
        function initCombobox() {
            //地区名称
            $("#AREA").combobox({
                url: "/Bas/Common/GetAreaData_Select?appendAll=appendAll",
                valueField: "CODE",
                textField: "TITLE",
                width: 120,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //行业类别
            $("#BASTYPE").combobox({
                url: "/Bas/Dic/GetDicCodeDataForSearch?typeCode=IndustryType",
                valueField: "CODE",
                textField: "TITLE",
                width: 100,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //关注程度
            $("#GZCD").combobox({
                url: "/Bas/Dic/GetDicCodeDataForSearch?typeCode=CompanyGZCD",
                valueField: "CODE",
                textField: "TITLE",
                width: 60,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //状态
            $("#ALERTSTATE").combobox({
                data: [{ CODE: '', TITLE: '全部' }, { CODE: '0', TITLE: '关闭' }, { CODE: '1', TITLE: '开启' }],
                valueField: "CODE",
                textField: "TITLE",
                width: 60,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
        }

        //VOCs企业信息
        function showDetail(id, showType, name) {
            $("#detailDialog").show();
            $("#detailIF").attr("src", "Edit?id=" + id + "&showType=" + showType);
            var dialog = $("#detailDialog").dialog({
                title: "告警设置-" + name,
                width: 1000,
                height: 700,
                cache: false,
                modal: true,
                closable: false,
                buttons: [
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
                 }]
            });
        }

        //企业联系人信息
        function showContactInfo(id, showType, isVOCs, row) {
            $("#detailContactDialog").show();
            $("#detailContactIF").attr("src", "/BaseData/VOCsCompany/CompanyContactInfo?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs + "&row=" + row);
            var dialog = $("#detailContactDialog").dialog({
                title: "企业联系人信息",
                width: 380,
                height: 450,
                cache: false,
                modal: true,
                closable: false,
                buttons: [
                {
                    text: '确定',
                    iconCls: 'icon-save',
                    handler: function () {
                        //saveCompanyData();
                        //$('#dg').datagrid('reload');
                        dialog.dialog("close");
                    }
                },
                 {
                     text: '取消',
                     iconCls: 'icon-cancel',
                     handler: function () {
                         //delCompanyByID();
                         dialog.dialog("close");
                     }
                 }]
            });
        }

        //通信配置
        function showTXDetail(id) {
            $("#VOCsCompanyTXDialog").show();
            $("#VOCsCompanyTXIF").attr("src", "/BaseData/VOCsCompany/VOCsCompanyTXEdit?id=" + id);
            var dialog = $("#VOCsCompanyTXDialog").dialog({
                title: "通信配置",
                width: 600,
                height: 680,
                cache: false,
                modal: true,
                buttons: [
                {
                    text: '确定',
                    iconCls: 'icon-save',
                    handler: function () {
                        dialog.dialog("close");
                    }
                },
                 {
                     text: '取消',
                     iconCls: 'icon-cancel',
                     handler: function () {
                         dialog.dialog("close");
                     }
                 }]
            });
        }

        //排污设施信息
        function showPWDetail(id) {
            $("#VOCsCompanyPWDialog").show();
            $("#VOCsCompanyPWIF").attr("src", "/BaseData/VOCsCompany/VOCsCompanyPWEdit?id=" + id);
            var dialog = $("#VOCsCompanyPWDialog").dialog({
                title: "排污设施信息",
                width: 800,
                height: 680,
                cache: false,
                modal: true,
                buttons: [
                {
                    text: '确定',
                    iconCls: 'icon-save',
                    handler: function () {
                        dialog.dialog("close");
                    }
                },
                 {
                     text: '取消',
                     iconCls: 'icon-cancel',
                     handler: function () {
                         dialog.dialog("close");
                     }
                 }]
            });
        }

        function saveCompanyData() {
            detailIFName.SaveSetting();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!--查询表单-->
    <div style="margin: 5px; width: 100%; height: 35px;" id="header">
        <form id="qform">
            <div style="float: left; padding: 5px;">
                <div style="float: left; width: 100%;">
                    <span style="margin-left: 10px">企业名称：</span>
                    <input id="CompanyName" name="CompanyName" style="margin-right: 10px" />
                    <span style="margin-left: 10px">地区名称：</span>
                    <input id="AREA" name="AREA" style="margin-right: 10px" />
                    <span style="margin-left: 10px">行业类别：</span>
                    <input id="BASTYPE" name="BASTYPE" style="margin-right: 10px" />
                    <span style="margin-left: 10px">关注程度：</span>
                    <input id="GZCD" name="GZCD" style="margin-right: 10px" />
                    <span style="margin-left: 10px">状态：</span>
                    <input id="ALERTSTATE" name="ALERTSTATE" style="margin-right: 10px" />
                    <a id="btnSearch" href="#" class="easyui-linkbutton" style="margin-left: 30px" data-options="iconCls:'icon-search'">查询</a>
                </div>
            </div>
        </form>
    </div>
    <!--数据列表-->
    <div id="dg">
    </div>

    <%--企业信息弹出窗体--%>
    <div id="detailDialog" style="display: none;">
        <iframe id="detailIF" name="detailIFName" style="border: 0px; width: 100%; height: 620px; overflow: hidden;"></iframe>
    </div>
    <%--企业联系人弹出窗体--%>
    <div id="detailContactDialog" style="display: none;">
        <iframe id="detailContactIF" name="detailContactIFName" style="border: 0px; width: 100%; height: 360px; overflow: hidden;"></iframe>
    </div>
    <%--通信配置--%>
    <div id="VOCsCompanyTXDialog" style="display: none;">
        <iframe id="VOCsCompanyTXIF" name="VOCsCompanyTXIF" style="border: 0px; width: 100%; height: 539px; overflow: hidden;"></iframe>
    </div>
    <%--排污设备信息--%>
    <div id="VOCsCompanyPWDialog" style="display: none;">
        <iframe id="VOCsCompanyPWIF" style="border: 0px; width: 100%; height: 639px; overflow: hidden;"></iframe>
    </div>
</asp:Content>
