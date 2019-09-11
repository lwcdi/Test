<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!--查询表单-->
    <div style="margin: 5px; width: 100%; height: 30px;" id="headerJK">
        监控设备信息
        <a id="btnAddJK" onclick="showJKDevice()" href="#" class="easyui-linkbutton" style="margin-left: 10px" data-options="iconCls:'icon-add'">新增监控设备</a>
    </div>
    <!--数据列表-->
    <div id="dgJK" style="height: 300px;overflow:auto;">
    </div>
    <div style="margin: 5px; width: 100%; height: 30px;" id="headerZL">
        治理设备信息
        <a id="btnAddZL" onclick="showZLDevice()" href="#" class="easyui-linkbutton" style="margin-left: 10px" data-options="iconCls:'icon-add'">新增治理设备</a>
    </div>
    <!--数据列表-->
    <div id="dgZL" style="height: 300px;overflow:auto;">
    </div>

     <%--企业监控设备弹出窗体--%>
    <div id="detailJKDeviceDialog" style="display: none;">
        <iframe id="detailJKDeviceIF" name="detailJKDeviceIFName" style="border: 0px; width: 100%; height: 360px; overflow: hidden;"></iframe>
    </div>
    <%--企业治理设备弹出窗体--%>
    <div id="detailZLDeviceDialog" style="display: none;">
        <iframe id="detailZLDeviceIF" name="detailZLDeviceIFName" style="border: 0px; width: 100%; height: 420px; overflow: hidden;"></iframe>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Content/js/getQueryStr.js"></script>
    <script src="/Content/js/BaseData/CompanyInfo.js"></script>
    <script>
        $(function () {
            //alert(id + " " + showType + " " + isVOCs);
            getJKDeviceData();
            getZLDeviceData();
        });

        function getJKDeviceData() {
            var param = "";
            param += "&ID=" + id;
            $("#dgJK").datagrid({
                url: "/BaseData/VOCsCompany/GetJKDeviceData?" + param,
                idField: "CODE",
                pagination: true,
                pageSize: 50,
                singleSelect: true,
                striped: true,
                rownumbers: true,
                height: 250,
                columns: [[
                    { title: "设备名称", field: "NAME", width: 120 },
                    { title: "规格型号", field: "GGXH", width: 120 },
                    { title: "对应排污口", field: "PKNAME", width: 120 },
                    { title: "投运日期", field: "TYRQ", formatter: DateTimeFormatter, width: 120 },
                    { title: "生产厂家", field: "SCCJ", width: 120 },
                    { title: "运维联系人", field: "YWCONTACT", width: 120 },
                    {
                        title: "操作", field: "ID",
                        formatter: function (value, row, index) {
                            return '<span title="编辑" class="operatorBtn" style="cursor:pointer;" onclick="showJKDeviceInfo(\'' + id + '\',\'1\',\'' + isVOCs + '\',\'' + value + '\')">编辑</span>&nbsp;|&nbsp;<span title="编辑" class="operatorBtn" style="cursor:pointer;" onclick="dg_DeleteJK(\'' + value + '\')">删除</span>';
                        }
                    }
                ]]
            });
        }

        function getZLDeviceData() {
            var param = "";
            param += "&ID=" + id;
            $("#dgZL").datagrid({
                url: "/BaseData/VOCsCompany/GetZLDeviceData?" + param,
                idField: "CODE",
                pagination: true,
                pageSize: 50,
                singleSelect: true,
                striped: true,
                rownumbers: true,
                height: 250,
                columns: [[
                    { title: "设备名称", field: "NAME", width: 120 },
                    { title: "规格型号", field: "GGXH", width: 120 },
                    { title: "对应排污口", field: "PKNAME", width: 100 },
                    { title: "处理工艺", field: "CLGY", width: 100 },
                    { title: "总投资额", field: "ZTZE", width: 80 },
                    { title: "投运日期", field: "TYRQ", formatter: DateTimeFormatter, width: 80 },
                    { title: "设计处理能力（吨/天）", field: "DESIGNSCCJ", width: 80 },
                    { title: "实际处理能力（吨/天）", field: "REALCLNL", width: 80 },
                    {
                        title: "操作", field: "ID",
                        formatter: function (value, row, index) {
                            return '<span title="编辑" class="operatorBtn" style="cursor:pointer;" onclick="showZLDeviceInfo(\'' + id + '\',\'1\',\'' + isVOCs + '\',\'' + value + '\')">编辑</span>&nbsp;|&nbsp;<span title="编辑" class="operatorBtn" style="cursor:pointer;" onclick="dg_DeleteZL(\'' + value + '\')">删除</span>';
                        }
                    }
                ]]
            });
        }

        //格式化日期时间
        function DateTimeFormatter(value) {
            if (!value) {
                return "";
            }
            var date = new Date(value);
            var y = date.getFullYear();
            var m = date.getMonth() + 1;
            var d = date.getDate();
            return y + '-' +m + '-' + d;
        }

        function showJKDevice() {
            showJKDeviceInfo(id, "0", isVOCs, "");
        }

        function showZLDevice() {
            showZLDeviceInfo(id, "0", isVOCs, "");
        }

        function showJKDeviceInfo(id, showType, isVOCs, rowID) {
            $("#detailJKDeviceDialog").show();
            $("#detailJKDeviceIF").attr("src", "/BaseData/VOCsCompany/CompanyJKDeviceInfo?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs + "&rowID=" + rowID);
            var dialog = $("#detailJKDeviceDialog").dialog({
                title: "企业监控设备信息",
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
                        saveJKDeviceData();
                        $('#dgJK').datagrid('reload');
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

        function showZLDeviceInfo(id, showType, isVOCs, rowID) {
            $("#detailZLDeviceDialog").show();
            $("#detailZLDeviceIF").attr("src", "/BaseData/VOCsCompany/CompanyZLDeviceInfo?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs + "&rowID=" + rowID);
            var dialog1 = $("#detailZLDeviceDialog").dialog({
                title: "企业治理设备信息",
                width: 380,
                height: 500,
                cache: false,
                modal: true,
                closable: false,
                buttons: [
                {
                    text: '确定',
                    iconCls: 'icon-save',
                    handler: function () {
                        saveZLDeviceData();
                        $('#dgZL').datagrid('reload');
                        dialog1.dialog("close");
                    }
                },
                 {
                     text: '取消',
                     iconCls: 'icon-cancel',
                     handler: function () {
                         dialog1.dialog("close");
                     }
                 }]
            });
        }

        function dg_DeleteJK(id) {
            $.messager.confirm('警告', '确定删除该条数据吗?', function (r) {
                if (r) {
                    //alert(id);
                    com.ajax({
                        url: "/BaseData/VOCsCompany/DelJKDeviceInfoByID",
                        data: { id: id },
                        async: false,
                        success: function (result) {
                            if (result.success == true) {
                                com.msg.alert(result.data);
                                $("#dgJK").datagrid("reload");
                            }
                        }
                    });
                }
            });
        }

        function dg_DeleteZL(id) {
            $.messager.confirm('警告', '确定删除该条数据吗?', function (r) {
                if (r) {
                    //alert(id);
                    com.ajax({
                        url: "/BaseData/VOCsCompany/DelZLDeviceInfoByID",
                        data: { id: id },
                        async: false,
                        success: function (result) {
                            if (result.success == true) {
                                com.msg.alert(result.data);
                                $("#dgZL").datagrid("reload");
                            }
                        }
                    });
                }
            });
        }

        function saveJKDeviceData() {
            detailJKDeviceIFName.saveJKDeviceData();
        }

        function saveZLDeviceData() {
            detailZLDeviceIFName.saveZLDeviceData();
        }
    </script>
</asp:Content>
