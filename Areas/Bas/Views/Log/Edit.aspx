<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<form id="uiform">
    <table class="grid2">
        <tr>
            <td style="width: 110px;" align="right">日志类别：</td>
            <td>
                <input id="LogTypeName" name="LogTypeName" />
            </td>
            <td style="width: 110px;" align="right">操作人：</td>
            <td>
                <input id="OperateUserName" name="OperateUserName" />
            </td>
        </tr>
        <tr>
            <td align="right">操作IP：</td>
            <td>
                <input id="OperateIP" name="OperateIP" />
            </td>
            <td align="right">操作时间：</td>
            <td>
                <input id="OperateTime" name="OperateTime" />
            </td>
        </tr>

        <tr>
            <td align="right">日志内容：</td>
            <td colspan="3">
                <textarea id="LogContent" name="LogContent" rows="3" style="width: 450px; height: 220px;"></textarea>
            </td>
        </tr>
    </table>
</form>
