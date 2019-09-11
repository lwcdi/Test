<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<form id="uiform">
    <table class="grid">
        <tr><td>新密码：</td><td><input id="txt_newpass" name="password" type="password" class="txt03 easyui-validatebox" data-options="required:true,missingMessage:'请输入新密码'" /></td></tr>
        <tr><td>确认密码：</td><td><input id="txt_repass" type="password" class="txt03 easyui-validatebox" data-options="required:true,missingMessage:'请再一次输入新密码',validType:'equalTo[\'#txt_newpass\']'" /></td></tr>
    </table>
</form>
