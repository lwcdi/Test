<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!--查询表单-->
    <div style="margin: 5px; width: 100%; height: 30px;" id="headerPK">
        排口信息
        <a id="btnAddPK" onclick="showPKInfoInfo()" href="#" class="easyui-linkbutton" style="margin-left: 10px" data-options="iconCls:'icon-add'">新增排口信息</a>
    </div>
    <div style="height: 300px; overflow: auto;">
        <div id="pkTab" fit="true" style="height: 300px; overflow: hidden;">
            <div id="divPKFQ" title="废气排口" style="padding: 2px; overflow: hidden; height: 280px;">
                <!--数据列表-->
                <div id="dgPKFQ" style="height: 270px; overflow: auto;">
                </div>
            </div>
            <div id="divPKFS" title="废水排口" style="padding: 2px; overflow: hidden; height: 280px;">
                <!--数据列表-->
                <div id="dgPKFS" style="height: 270px; overflow: auto;">
                </div>
            </div>
            <div id="divPKVOCs" title="VOCs排口" style="padding: 2px; overflow: hidden; height: 280px;">
                <!--数据列表-->
                <div id="dgPKVOCs" style="height: 270px; overflow: auto;">
                </div>
            </div>
        </div>
    </div>
    <div style="margin: 5px; width: 100%; height: 30px;" id="headerTX">
        通信配置
        <a id="btnAddTX" onclick="showTXInfo()" href="#" class="easyui-linkbutton" style="margin-left: 10px" data-options="iconCls:'icon-add'">新增通信配置</a>
    </div>
     
    <!--数据列表-->
    <div id="dgTX" style="height: 200px;overflow:auto;">
    </div>

     <%--企业排口信息弹出窗体--%>
    <div id="detailPKInfoDialog" style="display: none;">
        <iframe id="detailPKInfoIF" name="detailPKInfoIFName" style="border: 0px; width: 100%; height: 280px; overflow: hidden;"></iframe>
    </div>
     <%--企业通信配置弹出窗体--%>
    <div id="detailTXInfoDialog" style="display: none;">
        <iframe id="detailTXInfoIF" name="detailTXInfoIFName" style="border: 0px; width: 100%; height: 400px; overflow: hidden;"></iframe>
    </div>
    <input id="getCSLXname" style="display: none;" />
    <input id="getCLCSname" style="display: none;" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Content/js/getQueryStr.js"></script>
    <script src="/Content/js/BaseData/CompanyInfo.js"></script>
    <script>
        var PKType = 1; //排口种类(1-废气、2-废水、3-VOCs)

        $(function () {
            //alert(id + " " + showType + " " + isVOCs);
            $("#pkTab").tabs({
                onSelect: function (title, index) {
                    if (title == "废气排口") {
                        PKType = 1;
                        getPKInfoData("dgPKFQ");
                    }
                    else if (title == "废水排口") {
                        PKType = 2;
                        getPKInfoData("dgPKFS");
                    }
                    else if (title == "VOCs排口") {
                        PKType = 3;
                        getPKInfoData("dgPKVOCs");
                    }
                }
            });
            if (isVOCs == "0") {
                $('#pkTab').tabs('getTab', "VOCs排口").panel('options').tab.hide();//隐藏tab表头
                $("#divPKVOCs").hide();//隐藏tab内容
            }
            else if (isVOCs == "1") {
                $('#pkTab').tabs('getTab', "废气排口").panel('options').tab.hide();//隐藏tab表头
                //$("#divPKFQ").hide();//隐藏tab内容
                $('#pkTab').tabs('getTab', "废水排口").panel('options').tab.hide();//隐藏tab表头
                $("#divPKFS").hide();//隐藏tab内容
                $("#pkTab").tabs("select", "VOCs排口");
            }
            initCSLXCombobox();
            initPKTypeCombobx();
            getTXInfoData();
            $('#dgTX').datagrid('reload');
        });

        function initCSLXCombobox() {
            //传输类型
            $("#getCSLXname").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CompanySCYCSLX",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                multiple: true,
                onSelect: function (item) {
                }
            });
        }

        function initPKTypeCombobx(pkType) {
            //var CLCSTypeCode;
            //if (pkType == "1") {
            //    CLCSTypeCode = "GasItem";
            //}
            //else if (pkType == "2") {
            //    CLCSTypeCode = "WaterItem";
            //}
            //else if (pkType == "3") {
            //    CLCSTypeCode = "VOCsItem";
            //}
            //测量参数
            $("#getCLCSname").combobox({
                //url: "/Bas/Dic/GetDicCodeData?typeCode='GasItem','WaterItem','VOCsItem'",
                url: "/BaseData/VOCsCompany/GetDicCodeData?typeCode='GasItem','WaterItem','VOCsItem'",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                multiple: true,
                onSelect: function (item) {
                }
            });
        }

        function getPKInfoData(dgName) {
            var param = "";
            param += "&ID=" + id;
            param += "&PKType=" + PKType;
            $("#" + dgName).datagrid({
                url: "/BaseData/VOCsCompany/GetPKInfoData?" + param,
                idField: "CODE",
                pagination: true,
                pageSize: 50,
                singleSelect: true,
                striped: true,
                rownumbers: true,
                height: 270,
                columns: [[
                    { title: "排放口编号", field: "CODE", width: 80 },
                    { title: "排放口名称", field: "NAME", width: 80 },
                    { title: "排放口位置", field: "ADDRESS", width: 80 },
                    { title: "排口类型", field: "NGASTYPE", width: 80 },
                    { title: "排放去向类别", field: "NWAY", width: 80 },
                    { title: "排放规律", field: "NREGULAR", width: 80 },
                    { title: "经度", field: "LONGITUDE", width: 80 },
                    { title: "纬度", field: "LATITUDE", width: 80 },
                    {
                        title: "操作", field: "ID",
                        formatter: function (value, row, index) {
                            return '<span title="编辑" class="operatorBtn" style="cursor:pointer;" onclick="showPKInfo(\'' + id + '\',\'1\',\'' + isVOCs + '\',\'' + value + '\',\'' + PKType + '\')">编辑</span>&nbsp;|&nbsp;<span title="编辑" class="operatorBtn" style="cursor:pointer;" onclick="dg_DeletePK(\'' + value + '\')">删除</span>';
                        }
                    }
                ]]
            });
        }

        function getTXInfoData() {
            var param = "";
            param += "&ID=" + id;
            $("#dgTX").datagrid({
                url: "/BaseData/VOCsCompany/GetTXInfoData?" + param,
                idField: "CODE",
                pagination: true,
                pageSize: 50,
                singleSelect: true,
                striped: true,
                rownumbers: true,
                height: 200,
                columns: [[
                    { title: "排口名称", field: "PKNAME", width: 80 },
                    { title: "排口类型", field: "NGASTYPE", width: 80 },
                    { title: "排放去向类别", field: "NWAY", width: 80 },
                    { title: "数采仪MN号", field: "MN", width: 80 },
                    { title: "启用日期", field: "STARTTIME",formatter:DateTimeFormatter, width: 80 },
                    {
                        title: "传输类型", field: "CSLX", width: 80
                        , formatter: function (value, row, index) {
                            if (value != "" && value != null) {
                                $("#getCSLXname").combobox("setValues", value.split(','));
                                return "<span title='" + ($('#getCSLXname').combobox('getText')) + "'>" + ($('#getCSLXname').combobox('getText')) + "</span>";
                            }
                            else {
                                return "";
                            }
                        }
                    },
                    { title: "上报IP地址", field: "IP", width: 80 },
                    { title: "上报端口号", field: "PORT", width: 80 },
                    {
                        title: "测量参数", field: "CLCS", width: 80
                        , formatter: function (value, row, index) {
                            if (value != "" && value != null) {
                                //initPKTypeCombobx(row.PKTYPE);
                                $("#getCLCSname").combobox("setValues", value.split(','));
                                return "<span title='" + ($('#getCLCSname').combobox('getText')) + "'>" + ($('#getCLCSname').combobox('getText')) + "</span>";
                            }
                            else {
                                return "";
                            }
                        }
                    },
                    { title: "设备型号", field: "SBXH", width: 80 },
                    { title: "生产厂家", field: "SCCJ", width: 80 },
                    {
                        title: "操作", field: "ID",
                        formatter: function (value, row, index) {
                            return '<span title="编辑" class="operatorBtn" style="cursor:pointer;" onclick="showTXInfoInfo(\'' + id + '\',\'1\',\'' + isVOCs + '\',\'' + value + '\')">编辑</span>&nbsp;|&nbsp;<span title="编辑" class="operatorBtn" style="cursor:pointer;" onclick="dg_DeleteTX(\'' + value + '\')">删除</span>';
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
            return y + '-' + m + '-' + d;
        }

        function showPKInfoInfo() {
            showPKInfo(id, "0", isVOCs, "", PKType);
        }

        function showPKInfo(id, showType, isVOCs, rowID, PKType) {
            $("#detailPKInfoDialog").show();
            $("#detailPKInfoIF").attr("src", "/BaseData/VOCsCompany/CompanyPKEdit?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs + "&rowID=" + rowID + "&PKType=" + PKType);
            var dialog1 = $("#detailPKInfoDialog").dialog({
                title: "排口信息",
                width: 380,
                height: 360,
                cache: false,
                modal: true,
                closable: false,
                buttons: [
                {
                    text: '确定',
                    iconCls: 'icon-save',
                    handler: function () {
                        savePKInfoData();
                        if (PKType == "1") {
                            $('#dgPKFQ').datagrid('reload');
                        }
                        else if (PKType == "2") {
                            $('#dgPKFS').datagrid('reload');
                        }
                        else if (PKType == "3") {
                            $('#dgPKVOCs').datagrid('reload');
                        }
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

        function dg_DeletePK(id) {
            $.messager.confirm('警告', '确定删除该条数据吗?', function (r) {
                if (r) {
                    //alert(id);
                    com.ajax({
                        url: "/BaseData/VOCsCompany/DelPKInfoInfoByID",
                        data: { id: id },
                        async: false,
                        success: function (result) {
                            if (result.success == true) {
                                com.msg.alert(result.data);
                                if (PKType == "1") {
                                    $('#dgPKFQ').datagrid('reload');
                                }
                                else if (PKType == "2") {
                                    $('#dgPKFS').datagrid('reload');
                                }
                                else if (PKType == "3") {
                                    $('#dgPKVOCs').datagrid('reload');
                                }
                            }
                        }
                    });
                }
            });
        }

        function savePKInfoData() {
            detailPKInfoIFName.savePKInfoData();
        }

        function showTXInfo() {
            showTXInfoInfo(id, "0", isVOCs, "");
        }

        function showTXInfoInfo(id, showType, isVOCs, rowID) {
            $("#detailTXInfoDialog").show();
            $("#detailTXInfoIF").attr("src", "/BaseData/VOCsCompany/CompanyPKTXInfo?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs + "&rowID=" + rowID + "&PKType=" + PKType);
            var dialog1 = $("#detailTXInfoDialog").dialog({
                title: "通信配置",
                width: 380,
                height: 480,
                cache: false,
                modal: true,
                closable: false,
                buttons: [
                {
                    text: '确定',
                    iconCls: 'icon-save',
                    handler: function () {
                        debugger;
                        if (saveTXInfoData()) {
                            $('#dgTX').datagrid('reload');
                            dialog1.dialog("close");
                        }
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

        function dg_DeleteTX(id) {
            $.messager.confirm('警告', '确定删除该条数据吗?', function (r) {
                if (r) {
                    //alert(id);
                    com.ajax({
                        url: "/BaseData/VOCsCompany/DelTXInfoInfoByID",
                        data: { id: id },
                        async: false,
                        success: function (result) {
                            if (result.success == true) {
                                com.msg.alert(result.data);
                                $("#dgTX").datagrid("reload");
                            }
                        }
                    });
                }
            });
        }

        function saveTXInfoData() {
            return detailTXInfoIFName.saveTXInfoData();
        }
    </script>
</asp:Content>
