﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div style="padding: 2px; overflow: hidden;">
        <form id="DataForm" name="DataForm" method="post">
            <table class="grid">
                <tr>
                    <td style="width: 100px;">设备名称：</td>
                    <td>
                        <input id="NAME" name="NAME" type="text" class="txt03 easyui-validatebox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>对应排污口：</td>
                    <td>
                        <input id="PKID" name="PKID" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>规格型号：</td>
                    <td>
                        <input id="GGXH" name="GGXH" type="text" class="txt03 easyui-validatebox" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>投运日期：</td>
                    <td>
                        <input id="TYRQ" name="TYRQ" type="text" class="txt03 easyui-datebox" style="width: 200px" />
                        
                    </td>
                </tr>
                <tr>
                    <td>生产厂家：</td>
                    <td>
                        <input id="SCCJ" name="SCCJ" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>运维联系人：</td>
                    <td>
                        <input id="YWCONTACT" name="YWCONTACT" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td style="width: 190px;">联系电话</td>
                    <td>
                        <input id="YWCONTACTPHONE" name="YWCONTACTPHONE" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
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
         $(function () {
             //alert(id + " " + showType + " " + isVOCs);
             rowID = getQueryStr("rowID");
             initCombobox();
             debugger;
             if (showType == "1" && rowID) {
                 $("#DataForm").form("clear");
                 var row = GetJKDeviceInfoByID(rowID);
                 bindJsonToForm('DataForm', row);
                 $("#TYRQ").datebox("setValue", row.TYRQ);
             }
         });

         //保存企业联系人信息
         function saveJKDeviceData() {
             var data = $("#DataForm").formtojsonObject();
             data.COMPANYID = id;
             data.SHOWTYPE = showType;
             data.rowID = rowID;
             com.ajax({
                 url: "/BaseData/VOCsCompany/saveJKDeviceData",
                 data: data,
                 async: false,
                 success: function (result) {
                     if (result.success == true) {
                         com.msg.alert(result.data);
                     }
                 }
             });
         }

         //通过ID获取联系人信息
         function GetJKDeviceInfoByID(rowID) {
             var row = [];
             com.ajax({
                 url: "/BaseData/VOCsCompany/GetJKDeviceInfoByID",
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

         function initCombobox(){
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
         }
     </script>
</asp:Content>
