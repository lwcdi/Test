<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<form id="dicTypeForm">
    <table class="grid">
        <tr>
            <td>类别名称：</td>
            <td>
                <input id="Id" name="Id" type="hidden" />
                <input id="Title" name="Title" data-options="required:true,validType:'length[1,20]'" type="text" class="txt03 easyui-validatebox" /></td>
        </tr>
        <tr>
            <td>类别编码：</td>
            <td>
                <input id="Code" name="Code" data-options="required:true,validType:'length[1,20]'" type="text" class="txt03 easyui-validatebox" /></td>
        </tr>
        <tr>
            <td>排　　序：</td>
            <td>
                <input id="SortNo" name="SortNo" type="text" value="1" /></td>
        </tr>
        <tr>
            <td>备　　注：</td>
            <td>
                <textarea id="Remark" name="Remark" style="width: 200px; height: 50px;" class="txt03"></textarea></td>
        </tr>
    </table>
</form>