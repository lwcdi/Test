<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div style="padding: 2px; overflow: hidden;">
        <form id="DataForm" name="DataForm" method="post">
            <table class="grid">
                <tr>
                    <td style="width: 100px;">排放口编号：</td>
                    <td>
                        <input id="CODE" name="CODE" type="text" class="txt03 easyui-validatebox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>排放口名称：</td>
                    <td>
                        <input id="NAME" name="NAME" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>排放口位置：</td>
                    <td>
                        <input id="ADDRESS" name="ADDRESS" type="text" class="txt03 easyui-validatebox" style="width: 200px" />
                    </td>
                </tr>
                <tr id="PKWay" style="display:none;">
                    <td>排放去向类别：</td>
                    <td>
                        <input id="WAY" name="WAY" type="text" class="txt03 easyui-validatebox" style="width: 200px" />
                    </td>
                </tr>
                <tr id="PKGasType" style="display:none;">
                    <td>排口类型：</td>
                    <td>
                        <input id="GASTYPE" name="GASTYPE" type="text" class="txt03 easyui-validatebox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>排放规律：</td>
                    <td>
                        <input id="REGULAR" name="REGULAR" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>经度：</td>
                    <td>
                        <input id="LONGITUDE" name="LONGITUDE" type="text" class="txt03 easyui-validatebox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>纬度：</td>
                    <td>
                        <input id="LATITUDE" name="LATITUDE" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
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
                 var row = GetPKInfoByID(rowID);
                 bindJsonToForm('DataForm', row);
             }
             if (PKType == "1" || PKType == "3") {
                 $("#PKGasType").show();
             }
             else if (PKType == "2") {
                 $("#PKWay").show();
             }
         });

         //保存企业排口信息
         function savePKInfoData() {
             var data = $("#DataForm").formtojsonObject();
             data.COMPANYID = id;
             data.SHOWTYPE = showType; 
             data.rowID = rowID;
             data.TYPE = PKType;
             com.ajax({
                 url: "/BaseData/VOCsCompany/savePKInfoData",
                 data: data,
                 async: false,
                 success: function (result) {
                     if (result.success == true) {
                         com.msg.alert(result.data);
                     }
                 }
             });
         }

         //通过ID获取排口信息
         function GetPKInfoByID(rowID) {
             var row = [];
             com.ajax({
                 url: "/BaseData/VOCsCompany/GetPKInfoInfoByID",
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
                 onSelect: function (item) {
                 }
             });

             //排放去向类别
             $("#WAY").combobox({
                 url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyPKWay",
                 valueField: "CODE",
                 textField: "TITLE",
                 width: 150,
                 editable: false,
                 panelHeight: "auto",
                 required: false,
                 onSelect: function (item) {
                 }
             });

             //排口类型
             $("#GASTYPE").combobox({
                 url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyPKGasType",
                 valueField: "CODE",
                 textField: "TITLE",
                 width: 150,
                 editable: false,
                 panelHeight: "auto",
                 required: false,
                 onSelect: function (item) {
                 }
             });

             //排放规律
             $("#REGULAR").combobox({
                 url: "/Bas/Dic/GetDicCodeData?typeCode=CompanyPKPFGL",
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
     </script>
</asp:Content>
