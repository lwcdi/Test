<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<form id="uiform">
    <table class="grid">
        <tr>
            <td>菜单名称：</td>
            <td>
                <input id="txt_title" name="navtitle" type="text" class="txt03 easyui-validatebox" data-options="required:true" />
                &nbsp;&nbsp;English：<input id="txt_titleEN" name="navtitleEN" type="text" class="txt03 easyui-validatebox" data-options="required:true" />
            </td>
        </tr>
        <tr><td>页面标识：</td><td><input id="txt_ptag" name="navtag" type="text" class="txt03 easyui-validatebox" data-options="required:true" /></td></tr>
        <tr><td>上级菜单：</td><td><input id="txt_parentid" name="parentid" type="text" class="txt03" /></td></tr>
        <tr><td>链接地址：</td><td><input id="txt_url" name="linkurl" type="text" style="width:280px;" class="txt03 easyui-validatebox" data-options="required:true" /></td></tr>
        <tr><td>小图标：</td><td><span id="smallIcon" class="icon icon-note">&nbsp;</span><input id="txt_iconcls" name="iconcls" type="text" value="icon-note" class="txt03" />&nbsp;
            <a id="selecticon" ref="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-search" title="选择图标"></a></td></tr>
        <tr>
            <td>大图标：</td>
            <td>
                <img id="imgBig" src="/content/css/icon/32/note.png" style="vertical-align: middle" />&nbsp;
                <input id="txt_bigimgurl" name="bigimageurl" type="text" style="width:280px;" value="/content/css/icon/32/note.png" class="txt03" />
                &nbsp;<a id="selectBigIcon" ref="javascript:;" plain="true" class="easyui-linkbutton" icon="icon-search" title="选择图标"></a>

            </td>
        </tr>
        <tr><td>排序：</td><td><input id="txt_orderid" name="SortNo" type="text" value="1" /></td></tr>
        <tr><td>&nbsp;</td><td align="left"><input id="chkvisible" name="isvisible" style="vertical-align:middle" type="checkbox" checked="checked" value="1" /><label for="chkvisible" style="vertical-align:middle">显示菜单</label></td></tr>
    </table>
    <input id="txt_iconurl" name="iconurl" type="hidden" style="width:280px;" />
</form>