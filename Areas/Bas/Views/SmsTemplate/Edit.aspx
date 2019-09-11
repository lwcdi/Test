<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<form id="uiform">
    <link type="text/css" rel="stylesheet" href="/Content/css/Checkbox.css" />
    <table class="grid2">
        <tr>
            <th>主题：</th>
            <td colspan="3">
                <input id="TEMPNAME" name="TEMPNAME" class="easyui-validatebox" style="width: 450px; height: 30px;" required="required" />
            </td>
        </tr>
        <tr>
            <th>模板类型：</th>
            <td>
                <input id="SMSTEMPTYPE" name="SMSTEMPTYPE" />
            </td>
            <th>是否启用：</th>
            <td>
                <div class="holder">
                    <div class="center" >
                        <input type="checkbox" id="checkbox1" name="ISUSE" /><label for="checkbox1"></label>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <th>模板内容：</th>
            <td colspan="3">
                <textarea id="TEMPCONTENT" name="TEMPCONTENT" rows="3" style="width: 450px; height: 150px;"></textarea>
            </td>
        </tr>
    </table>
</form>
