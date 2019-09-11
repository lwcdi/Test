<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<style type="text/css">
    input::-ms-clear {
        display: none;
    }
</style>
<form id="uiform">
    <table class="grid">
        <tr>
            <td style="width: 80px;" align="right">名称：</td>
            <td>
                <input id="Title" name="Title" type="text" data-options="required:true,missingMessage:'请输入名称'" class="txt03 easyui-validatebox" /></td>
        </tr>
        <tr>
            <td align="right">编码：</td>
            <td>
                <input id="Code" name="Code" type="text" class="txt03 easyui-validatebox" data-options="required:true,missingMessage:'请输入编码'" /></td>
        </tr>
        <tr>
            <td align="right">类别：</td>
            <td>
                <span id="TypeName"></span>
                <input id="TypeId" name="TypeId" type="hidden" />
                <input id="TypeCode" name="TypeCode" type="hidden" /></td>
        </tr>
        <tr>
            <td align="right">排序：</td>
            <td>
                <input id="SortNo" name="SortNo" type="text" value="1" /></td>
        </tr>
        <tr>
            <td align="right">状态：</td>
            <td>
                <select id="Status" name="Status">
                    <option value="1">启用</option>
                    <option value="0">禁用</option>
                </select></td>
        </tr>
        <tr>
            <td align="right">备注：</td>
            <td>
                <textarea id="Remark" name="Remark" style="width: 200px; height: 50px;" class="txt03" /></td>
        </tr>
    </table>
</form>
