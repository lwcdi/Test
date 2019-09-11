<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<form id="uiform">
    <table class="grid">
        <tr>
            <td>组织名称：</td>
            <td>
                <input id="OrgName" name="OrgName" type="text" class="txt03 easyui-validatebox" data-options="required:true" /></td>
        </tr>
        <tr>
            <td>上级组织：</td>
            <td>
                <input id="ParentId" name="ParentId" style="width: 200px" /></td>
        </tr>
        <tr>
            <td>排序：</td>
            <td>
                <input id="SortNo" name="SortNo" value="0" /></td>
        </tr>
        <tr>
            <td>备注：</td>
            <td>
                <textarea id="Remark" name="Remark" style="width: 80%; height: 50px;" class="txt03" rows="5"></textarea></td>
        </tr>
    </table>
</form>
