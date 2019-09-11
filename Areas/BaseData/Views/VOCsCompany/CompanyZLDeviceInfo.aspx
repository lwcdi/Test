<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

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
                    <td>处理工艺：</td>
                    <td>
                        <input id="CLGY" name="CLGY" type="text" class="txt03 easyui-validatebox" style="width: 200px" />
                        
                    </td>
                </tr>
                <tr>
                    <td>总投资额：</td>
                    <td>
                        <input id="ZTZE" name="ZTZE" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>投运日期：</td>
                    <td>
                        <input id="TYRQ" name="TYRQ" type="text" class="txt03 easyui-datebox" style="width: 200px" />
                        
                    </td>
                </tr>
                <tr>
                    <td>设计处理能力（吨/天）：</td>
                    <td>
                        <input id="DESIGNSCCJ" name="DESIGNSCCJ" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>实际处理能力（吨/天）：</td>
                    <td>
                        <input id="REALCLNL" name="REALCLNL" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td style="width: 190px;">设备型号</td>
                    <td>
                        <input id="SBXH" name="SBXH" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
                </tr>
                 <tr>
                    <td style="width: 190px;">生产厂家</td>
                    <td>
                        <input id="SCCJ" name="SCCJ" type="text" class="txt03 easyui-validatebox" style="width: 200px" /></td>
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
                 var row = GetZLDeviceInfoByID(rowID);
                 bindJsonToForm('DataForm', row);
             }
         });

         //保存企业治理设备信息
         function saveZLDeviceData() {
             var data = $("#DataForm").formtojsonObject();
             data.COMPANYID = id;
             data.SHOWTYPE = showType;
             data.rowID = rowID;
             com.ajax({
                 url: "/BaseData/VOCsCompany/saveZLDeviceData",
                 data: data,
                 async: false,
                 success: function (result) {
                     if (result.success == true) {
                         com.msg.alert(result.data);
                     }
                 }
             });
         }

         //通过ID获取治理设备信息
         function GetZLDeviceInfoByID(rowID) {
             var row = [];
             com.ajax({
                 url: "/BaseData/VOCsCompany/GetZLDeviceInfoByID",
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
         }
     </script>
</asp:Content>
