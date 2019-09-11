<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<form id="uiform">
    <table cellpadding="5" cellspacing="0" width="100%" align="center" class="grid" border="0">
        <tr>
            <td align="right">角色名称：</td>
            <td align="left">
                <input id="RoleName" name="RoleName" type="text" required="true" style="width: 150px" class="txt03" /></td>
        </tr>
        <tr>
            <td align="right">排序：</td>
            <td align="left">
                <input id="SortNo" name="SortNo" type="text" value="0" style="width: 100px"/></td>
        </tr>
        <tr>
            <td align="right">
            </td>
            <td align="left">
                <input id="isDefault" name="isDefault" type="checkbox" value="1" style="vertical-align: middle" />
                <label for="isDefault" style="vertical-align: middle">默认角色</label></td>
        </tr>
        <tr>
            <td align="right">备注：</td>
            <td align="left">
                <textarea id="Remark" name="Remark" rows="3" style="width: 250px; height: 50px;" class="txt03" /></td>
        </tr>
    </table>
</form>
