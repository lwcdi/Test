<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding: 2px; overflow: hidden;">
        <form id="DataForm" name="DataForm" method="post">
            <table class="grid">
                <tr>
                    <td style="width: 100px;">联系人：</td>
                    <td>
                        <input id="NAME" name="NAME" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>归属部门：</td>
                    <td>
                        <input id="DEPARTMENT" name="DEPARTMENT" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>是否专职环保：</td>
                    <td>
                        <%--<input id="ISHB" name="ISHB" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" />--%>
                        <select id="ISHB" name="ISHB" style="width: 200px">
                            <option value="1">是</option>
                            <option value="0">否</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td>通讯地址：</td>
                    <td>
                        <input id="ADDRESS" name="ADDRESS" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" />
                        
                    </td>
                </tr>
                <tr>
                    <td>邮政编码：</td>
                    <td>
                        <input id="POSTCODE" name="POSTCODE" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>电子邮件：</td>
                    <td>
                        <input id="EMAIL" name="EMAIL" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td style="width: 190px;">办公电话</td>
                    <td>
                        <input id="OFFICEPHONE" name="OFFICEPHONE" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td style="width: 190px;">移动电话</td>
                    <td>
                        <input id="MOBILEPHONE" name="MOBILEPHONE" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td style="width: 190px;">传真</td>
                    <td>
                        <input id="FAX" name="FAX" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
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
             debugger;
             if (showType == "1" && rowID) {
                 $("#DataForm").form("clear");
                 var row = GetCompanyContactInfoByID(rowID);
                 bindJsonToForm('DataForm', row);
             }
         });

         //保存企业联系人信息
         function saveCompanyContactData() {
             var data = $("#DataForm").formtojsonObject();
             data.COMPANYID = id;
             data.SHOWTYPE = showType;
             data.rowID = rowID;
             com.ajax({
                 url: "/BaseData/VOCsCompany/saveCompanyContactData",
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
         function GetCompanyContactInfoByID(rowID) {
             var row = [];
             com.ajax({
                 url: "/BaseData/VOCsCompany/GetCompanyContactInfoByID",
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
     </script>
</asp:Content>
