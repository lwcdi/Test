<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!--查询表单-->
    <div style="margin: 5px; width: 100%; height: 30px;" id="header">
        污染源基本信息
    </div>
    <div style="height: 330px;overflow:auto;">
        <form id="DataForm" name="DataForm" method="post">
            <table class="grid">
                <tr>
                    <td style="width: 100px;">污染源名称：</td>
                    <td>
                        <input id="NAME" name="NAME" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">法定代表人：</td>
                    <td>
                        <input id="LEGALOR" name="LEGALOR" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">法人代码：</td>
                    <td>
                        <input id="LEGALOR_CODE" name="LEGALOR_CODE" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                </tr>
                <tr>
                    <td style="width: 100px;">污染源地址：</td>
                    <td>
                        <input id="ADDRESS" name="ADDRESS" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">经度：</td>
                    <td>
                        <input id="LONGITUDE" name="LONGITUDE" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">纬度：</td>
                    <td>
                        <input id="LATITUDE" name="LATITUDE" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                </tr>
                <tr>
                    <td style="width: 100px;">行政区域：</td>
                    <td>
                        <input id="CITY" name="CITY" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">隶属关系：</td>
                    <td>
                        <input id="RELATIONSHIP" name="RELATIONSHIP" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">行业类别：</td>
                    <td>
                        <input id="BASTYPE" name="BASTYPE" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                </tr>
                <tr>
                    <td style="width: 100px;">注册类型：</td>
                    <td>
                        <input id="ZCLX" name="ZCLX" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">单位类型：</td>
                    <td>
                        <input id="COMPANYTYPE" name="COMPANYTYPE" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">污染源规模：</td>
                    <td>
                        <input id="SCALE" name="SCALE" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                </tr>
                <tr>
                    <td style="width: 100px;">占地面积（m2）：</td>
                    <td>
                        <input id="COVERS" name="COVERS" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">污染源类别：</td>
                    <td>
                        <input id="WRYLB" name="WRYLB" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">是否30万千万电力企业：</td>
                    <td>
                        <input id="ISSSW" name="ISSSW" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                </tr>
                <tr>
                    <td style="width: 100px;">关注程度：</td>
                    <td>
                        <input id="GZCD" name="GZCD" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">地区名称：</td>
                    <td>
                        <input id="AREA" name="AREA" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">流域：</td>
                    <td>
                        <input id="LY" name="LY" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                </tr>
                <tr>
                    <td style="width: 100px;">启用状态：</td>
                    <td>
                        <input id="ISSTART" name="ISSTART" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">在线状态：</td>
                    <td>
                        <input id="ISONLINE" name="ISONLINE" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">运行状态：</td>
                    <td>
                        <input id="RUNTYPE" name="RUNTYPE" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                </tr>
                <tr>
                    <td style="width: 100px;">开户银行：</td>
                    <td>
                        <input id="BANKNAME" name="BANKNAME" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td style="width: 100px;">银行账户：</td>
                    <td>
                        <input id="BANKACCOUNT" name="BANKACCOUNT" type="text" class="txt03 easyui-validatebox" style="width: 150px" /></td>
                    <td colspan="2">
                        <%--<a id="btnAdd" href="#" class="easyui-linkbutton" style="margin-left: 10px" data-options="iconCls:'icon-add'">保存</a>--%>
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div style="margin: 5px; width: 100%; height: 30px;" id="header">
        相关联系人
        <a id="btnAddContact" onclick="showContact()" href="#" class="easyui-linkbutton" style="margin-left: 10px" data-options="iconCls:'icon-add'">新增联系人</a>
    </div>
    <!--数据列表-->
    <div id="dg" style="height: 50px;overflow:auto;">
    </div>

     <%--企业联系人弹出窗体--%>
    <div id="detailContactDialog" style="display: none;">
        <iframe id="detailContactIF" name="detailContactIFName" style="border: 0px; width: 100%; height: 360px; overflow: hidden;"></iframe>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Content/js/getQueryStr.js"></script>
    <script src="/Content/js/BaseData/CompanyInfo.js"></script>
    <script>
        $(function () {
            debugger;
            //alert(id + " " + showType + " " + isVOCs);
            adjustSize();
            $(window).resize(function () {
                adjustDg();
            });
            initCombobox(); //初始化下拉框
            getContactData();

            if ((showType == "1" || showType == "2") && id) { //编辑、查看
                $("#DataForm").form("clear");
                var row = GetCompanyInfoByID(id);
                bindJsonToForm('DataForm', row);

                if (showType == "2") { //查看
                    $('#DataForm').find('input,textarea,select').attr('readonly', true); //表单只读
                    $("#btnAddContact").hide(); //隐藏新增联系人按钮
                    $("#dg").datagrid("hideColumn", "ID");  //隐藏编辑列
                }
            }
        });

        function adjustSize() {
            $("#dg").height($(window).height() - $("#header").outerHeight() - 15);
        }
        function adjustDg() {
            $("#dg").datagrid('resize', { height: $(window).height() - $("#header").outerHeight() - 15 });
        }

        //初始化下拉框
        function initCombobox() {
            //行政区域
            $("#CITY").combobox({
                url: "/BaseData/VOCsCompany/GetAreaData_City",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //隶属关系
            $("#RELATIONSHIP").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyRelations",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //行业类别
            $("#BASTYPE").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=IndustryType",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //注册类型
            $("#ZCLX").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyRegType",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });        
            //单位类型
            $("#COMPANYTYPE").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyUnitType",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //污染源规模
            $("#SCALE").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=WRYScale",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //污染源类别
            $("#WRYLB").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=WRYType",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //是否30万千万电力企业
            $("#ISSSW").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=TrueOrFalse",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //关注程度
            $("#GZCD").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyGZCD",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //地区名称
            $("#AREA").combobox({
                url: "/Bas/Common/GetAreaData_Select",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //流域
            $("#LY").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyLY",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //启用状态
            $("#ISSTART").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyIsUse",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //在线状态
            $("#ISONLINE").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyIsOnline",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
            //运行状态
            $("#RUNTYPE").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyIsRun",
                valueField: "CODE",
                textField: "TITLE",
                width: 150,
                editable: false,
                panelHeight: "auto",
                required: false,
                onSelect: function (item) {
                }
            });
        }

        function getContactData() {
            var param = "";
            param += "&ID=" + id;
            $("#dg").datagrid({
                url: "/BaseData/VOCsCompany/GetContactData?" + param,
                idField: "CODE",
                pagination: true,
                pageSize: 50,
                singleSelect: true,
                striped: true,
                rownumbers: true,
                height: 200,
                columns: [[
                    { title: "联系人", field: "NAME", width: 80 },
                    { title: "归属部门", field: "DEPARTMENT", width: 80 },
                    { title: "是否专职环保", field: "NISHB", width: 80 },
                    { title: "通讯地址", field: "ADDRESS", width: 80 },
                    { title: "邮政编码", field: "POSTCODE", width: 80 },
                    { title: "电子邮箱", field: "EMAIL", width: 80 },
                    { title: "办公电话", field: "OFFICEPHONE", width: 80 },
                    { title: "移动电话", field: "MOBILEPHONE", width: 80 },
                    { title: "传真", field: "FAX", width: 80 },
                    {
                        title: "操作", field: "ID",
                        formatter: function (value, row, index) {
                            return '<span title="编辑" class="operatorBtn" style="cursor:pointer;" onclick="showContactInfo(\'' + id + '\',\'1\',\'' + isVOCs + '\',\'' + value + '\')">编辑</span>&nbsp;|&nbsp;<span title="编辑" class="operatorBtn" style="cursor:pointer;" onclick="dg_Delete(\'' + value + '\')">删除</span>';
                        }
                    }
                ]]
            });
        }

        //保存企业信息
        function saveCompanyData() {
            var data = $("#DataForm").formtojsonObject();
            data.ISVOCS = isVOCs;
            data.SHOWTYPE = showType;
            data.ID = id;
            com.ajax({
                url: "/BaseData/VOCsCompany/saveCompanyData",
                data: data,
                async: false,
                success: function (result) {
                    if (result.success == true) {
                        com.msg.alert(result.data);
                    }
                }
            });
        }

        //通过ID获取企业信息
        function GetCompanyInfoByID(id) {
            var row = [];
            com.ajax({
                url: "/BaseData/VOCsCompany/GetCompanyInfoByID",
                data: { id: id },
                async: false,
                success: function (result) {
                    if (result.success == true) {
                        row = $.parseJSON(result.data)[0];
                    }
                }
            });
            return row;
        }
        
        function showContact() {
            //window.parent.showContactInfo(id, "0", isVOCs, "");
            showContactInfo(id, "0", isVOCs, "");
        }

        function dg_Edit(index) {
            //var row = $('#dg').datagrid('getData').rows[index];
            //$("#DataFormQT").form("clear");
            //bindJsonToForm('DataFormQT', row);
            $("#detailDialog").show();
            $("#detailIF").attr("src", "/BaseData/VOCsCompany/VOCsCompanyEdit?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs);
            var dialog = $("#detailDialog").dialog({
                title: "VOCs企业信息",
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
                        saveCompanyData();
                        $('#dg').datagrid('reload');
                        dialog.dialog("close");
                    }
                },
                 {
                     text: '取消',
                     iconCls: 'icon-cancel',
                     handler: function () {
                         delCompanyByID();
                         dialog.dialog("close");
                     }
                 }]
            });
            alert(index);
        }

        function dg_Delete(id) {
            $.messager.confirm('警告', '确定删除该条数据吗?', function (r) {
                if (r) {
                    //alert(id);
                    com.ajax({
                        url: "/BaseData/VOCsCompany/DelCompanyContactInfoByID",
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

        //企业联系人信息
        function showContactInfo(id, showType, isVOCs, rowID) {
            $("#detailContactDialog").show();
            $("#detailContactIF").attr("src", "/BaseData/VOCsCompany/CompanyContactInfo?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs + "&rowID=" + rowID);
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
                        saveCompanyContactData();
                        $('#dg').datagrid('reload');
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

        function saveCompanyContactData() {
            detailContactIFName.saveCompanyContactData();
        }
    </script>
</asp:Content>
