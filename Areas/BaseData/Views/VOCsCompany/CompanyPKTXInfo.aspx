<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div style="padding: 2px; overflow: hidden;">
        <form id="DataForm" name="DataForm" method="post">
            <table class="grid">
                <tr>
                    <td style="width: 100px;">MN号：</td>
                    <td>
                        <input id="MN" name="MN" type="text" class="txt03 easyui-validatebox" style="width: 200px" data-options="required:true,missingMessage:'请输入MN号'"/>
                    </td>
                </tr>
                <tr>
                    <td>启用日期：</td>
                    <td>
                        <input id="STARTTIME" name="STARTTIME" type="text" class="txt03 easyui-datebox" style="width: 200px" data-options="required:true"/></td>
                </tr>
                <tr>
                    <td>传输类型：</td>
                    <td>
                        <input id="CSLX" name="CSLX" type="text" class="txt03 " style="width: 200px" data-options="required:true"/>
                    </td>
                </tr>
                <tr>
                    <td>SIM（UIM）卡号：</td>
                    <td>
                        <input id="SIM" name="SIM" type="text" class="txt03" style="width: 200px" />
                        
                    </td>
                </tr>
                <tr>
                    <td>上报IP地址：</td>
                    <td>
                        <input id="IP" name="IP" type="text" class="txt03" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>上报端口号：</td>
                    <td>
                        <input id="PORT" name="PORT" type="text" class="txt03" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>排口名称：</td>
                    <td>
                        <input id="PKID" name="PKID" type="text" class="txt03 " style="width: 200px" data-options="required:true"/></td>
                </tr>
                <tr>
                    <td>测量参数：</td>
                    <td>
                        <input id="CLCS" name="CLCS" type="text" class="txt03 " style="width: 200px" data-options="required:true"/></td>
                </tr>
                <tr>
                    <td style="width: 190px;">设备型号</td>
                    <td>
                        <input id="SBXH" name="SBXH" type="text" class="txt03" style="width: 200px" /></td>
                </tr>
                 <tr>
                    <td style="width: 190px;">生产厂家</td>
                    <td>
                        <input id="SCCJ" name="SCCJ" type="text" class="txt03" style="width: 200px" /></td>
                </tr>
            </table>
        </form>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Content/js/getQueryStr.js"></script>
    <script src="/Content/js/BaseData/CompanyInfo.js"></script>
     <script>
         var rowID;
         var PKType; //排口种类(1-废气、2-废水、3-VOCs)
         $(function () {
             //alert(id + " " + showType + " " + isVOCs);
             rowID = getQueryStr("rowID");
             PKType = getQueryStr("PKType");
             initCombobox();
             debugger;
             if (showType == "1" && rowID) {
                 $("#DataForm").form("clear");
                 var row = GetTXInfoInfoByID(rowID);
                 PKType = row.PKTYPE;
                 initCombobox();
                 bindJsonToForm('DataForm', row);
                 $("#CSLX").combobox("setValues", row.CSLX);
                 $("#CLCS").combobox("setValues", row.CLCS);
                 $('#PKID').combobox('disable');   //设置排口不能编辑
             }
         });

         //保存企业通信配置
         function saveTXInfoData() {
             var saveResult;
             debugger;
             if ($("#DataForm").form('validate') == false) return false;
             var data = $("#DataForm").formtojsonObject();
             data.CSLXS = $("#CSLX").combobox("getValues").toString();
             data.CLCSS = $("#CLCS").combobox("getValues").toString();
             data.COMPANYID = id;
             data.SHOWTYPE = showType;
             data.rowID = rowID;
             com.ajax({
                 url: "/BaseData/VOCsCompany/saveTXInfoData",
                 data: data,
                 async: false,
                 success: function (result) {
                     if (result.success == true) {
                         com.msg.alert(result.data);
                         saveResult = true;
                     }
                     else {
                         com.msg.alert(result.data);
                         saveResult = false;
                     }
                 }
             });
             return saveResult;
         }

         //通过ID获取通信配置信息
         function GetTXInfoInfoByID(rowID) {
             var row = [];
             com.ajax({
                 url: "/BaseData/VOCsCompany/GetTXInfoInfoByID",
                 data: { rowID: rowID },
                 async: false,
                 success: function (result) {
                     if (result.success == true) {
                         row = $.parseJSON(result.data)[0];
                     }
                 }
             });
             return row;
         }

         //通过排口ID获取排口类型
         function GetTXTypeByPKID(PKID) {
             var PKType = [];
             com.ajax({
                 url: "/BaseData/VOCsCompany/GetTXTypeByPKID",
                 data: { PKID: PKID },
                 async: false,
                 success: function (result) {
                     if (result.success == true) {
                         PKType = $.parseJSON(result.data);
                     }
                 }
             });
             return PKType;
         }

         function initCombobox() {
             //企业排口
             $("#PKID").combobox({
                 url: "/BaseData/VOCsCompany/GetPKDataByID?id=" + id,
                 valueField: "CODE",
                 textField: "TITLE",
                 width: 150,
                 editable: false,
                 panelHeight: "auto",
                 required: false,
                 onSelect: function (pkid) {
                     setPKCLCS(pkid);
                 },
                 onChange: function (pkid) {
                     setPKCLCS(pkid);
                 }
             }).combobox({
                 required: true
             });

             //传输类型
             $("#CSLX").combobox({
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
             }).combobox({
                 required: true
             });

             var CLCSTypeCode;
             if (PKType == "1") {
                 CLCSTypeCode = "GasItem";
             }
             else if (PKType == "2") {
                 CLCSTypeCode = "WaterItem";
             }
             else if (PKType == "3") {
                 CLCSTypeCode = "VOCsItem";
             }
             //测量参数
             $("#CLCS").combobox({
                 url: "/Bas/Dic/GetDicCodeData?typeCode=" + CLCSTypeCode,
                 valueField: "CODE",
                 textField: "TITLE",
                 width: 150,
                 editable: false,
                 panelHeight: "auto",
                 required: false,
                 multiple: true,
                 onSelect: function (item) {
                 }
             }).combobox({
                 required: true
             });
         }

         function setPKCLCS(pkid) {
             var pkTypeTmp = GetTXTypeByPKID(pkid);
             var CLCSTypeCodeTmp;
             if (pkTypeTmp == "1") {
                 CLCSTypeCodeTmp = "GasItem";
             }
             else if (pkTypeTmp == "2") {
                 CLCSTypeCodeTmp = "WaterItem";
             }
             else if (pkTypeTmp == "3") {
                 CLCSTypeCodeTmp = "VOCsItem";
             }
             //测量参数
             $("#CLCS").combobox({
                 url: "/Bas/Dic/GetDicCodeData?typeCode=" + CLCSTypeCodeTmp,
                 valueField: "CODE",
                 textField: "TITLE",
                 width: 150,
                 editable: false,
                 panelHeight: "auto",
                 required: false,
                 multiple: true,
                 onSelect: function (item) {
                 }
             }).combobox({
                 required: true
             });
         }
     </script>
</asp:Content>
