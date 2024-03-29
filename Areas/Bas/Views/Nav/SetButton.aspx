﻿<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<div class="easyui-layout" style="overflow: hidden;" border="false" fit="true">
    <div border="false" style="width:180px;" region="west">
        <table id="left_btns"></table>
    </div>
    <div border="false" region="center">
        <div style="width:30px;margin: auto;padding-top:100px;" >
            <a id="btnUp" plain="true" icon="icon-arrow_up" class="easyui-linkbutton" title="向上移动"></a>
            <a id="btnDown"  plain="true" icon="icon-arrow_down" class="easyui-linkbutton" title="向下移动"></a>
            <a id="btnRight" plain="true" icon="icon-arrow_right" class="easyui-linkbutton" title="选中"></a>
            <a id="btnLeft"  plain="true" icon="icon-arrow_left" class="easyui-linkbutton" title="移除"></a>
        </div>

    </div>
    <div border="false" region="east" style="width:180px">
        <table id="right_btns"></table>
    </div>
    <div border="false" region="south" style="height:30px; line-height:30px;color:#007ACC;padding-left:10px;font-weight:bold;">
        双击按钮即可添加或移除！
    </div>
</div>

