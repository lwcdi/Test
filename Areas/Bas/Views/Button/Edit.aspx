<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<form id="uiform">
    <table cellpadding="5" cellspacing="0" width="100%" align="center" class="grid" border="0">
        <tr>
            <td align="right">按钮名称：</td>
            <td>
                <input id="ButtonText" name="ButtonText" type="text" class="txt03 easyui-validatebox" data-options="required:true" />
            </td>
        </tr>
        <tr>
            <td align="right">ButtonName：</td>
            <td>
                <input id="ButtonTextEN" name="ButtonTextEN" type="text" class="txt03 easyui-validatebox" data-options="required:true" />
            </td>
        </tr>
        <tr>
            <td align="right">图标：</td>
            <td>
                <input id="IconCls" name="IconCls" type="text" class="txt03 easyui-validatebox" data-options="required:true" />&nbsp;
                <a id="selecticon" href="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-search" title="选择图标"></a>
            </td>
        </tr>
        <tr>
            <td align="right">权限代码：</td>
            <td>
                <input id="ButtonTag" name="ButtonTag" type="text" class="txt03 easyui-validatebox" data-options="required:true" />
            </td>
        </tr>
        <tr>
            <td align="right">排序：</td>
            <td>
                <input id="SortNo" name="SortNo" value="1" required="true" type="text" />
            </td>
        </tr>
        <tr>
            <td align="right">权限说明：</td>
            <td>
                <textarea id="Remark" name="Remark" rows="3" style="width: 200px; height: 50px;" class="txt03"></textarea>
            </td>
        </tr>
    </table>
</form>
