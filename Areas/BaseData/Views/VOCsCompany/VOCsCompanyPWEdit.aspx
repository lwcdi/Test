<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding: 2px; overflow: hidden;">
        <table class="grid">
            <tr>
                <td style="width: 100px;">MN号：</td>
                <td>
                    <input id="MN" name="MN" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" />
                </td>
            </tr>
            <tr>
                <td>启用日期：</td>
                <td>
                    <input id="STARTTIME" name="STARTTIME" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
            </tr>
            <tr>
                <td>传输类型：</td>
                <td>
                    <input id="TrueName" name="TrueName" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
            </tr>
            <tr>
                <td>SIM（UIM）卡号：</td>
                <td>
                    <input id="SIM" name="SIM" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" />
                    <%--            <select id="IsDisabled" name="IsDisabled" style="width: 200px">
                            <option value="true">激活</option>
                            <option value="false">禁用</option>
                        </select>--%>
                </td>
            </tr>
            <tr>
                <td>上报IP地址：</td>
                <td>
                    <input id="IP" name="IP" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
            </tr>
            <tr>
                <td>上报端口号：</td>
                <td>
                    <input id="PORT" name="PORT" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
            </tr>
            <tr>
                <td style="width: 190px;">测量参数</td>
                <td>
                    <input id="CLCS" name="CLCS" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
            </tr>
            <tr>
                <td style="width: 190px;">设备型号</td>
                <td>
                    <input id="SBXH" name="SBXH" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
            </tr>
            <tr>
                <td style="width: 190px;">生产厂家</td>
                <td>
                    <input id="SCCJ" name="SCCJ" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
            </tr>
        </table>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Content/js/getQueryStr.js"></script>
    <script>
        $(function () {
        });
    </script>
</asp:Content>
