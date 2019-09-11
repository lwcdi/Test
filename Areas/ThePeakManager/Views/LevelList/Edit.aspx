<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<form id="uiform">
    <table class="grid">
        <tr>
            <td>编号:</td><td><input id="LEVEL_NO" name="LEVEL_NO" class="txt03 inputSize easyui-validatebox" data-options="required:true" /></td>
            <td>应急等级:</td><td><select id="PEAK_LEVEL" name="PEAK_LEVEL"  class="txt03 inputSize" /></td>
            <td>预警类型:</td><td><select id="WAR_TYPE" name="WAR_TYPE"  class="txt03 inputSize" /></td>
        </tr>
        <tr>
            <td>说明:</td><td colspan="5"><input id="PEAK_DESC" name="PEAK_DESC" class="txt03 inputSize easyui-validatebox" data-options="required:true" style="width:840px" /></td>
        </tr>
        <tr>
            <td>附件:</td><td colspan="5"><div id="files"></div></td>
        </tr>
        <tr>
            <td>添加人:</td><td><input id="TRUENAME" name="TRUENAME" class="txt03 inputSize easyui-validatebox" readonly="readonly" /><input type="hidden" name="ADD_USER" id="ADD_USER" /></td>
            <td>添加时间:</td><td><input id="ADD_TIME" name="ADD_TIME" class="txt03 inputSize easyui-validatebox" readonly="readonly" /></td>
            <td></td><td></td>
        </tr>
        <tr>
            <td>停产企业数量：</td><td><input id="STOP_NUM" name="STOP_NUM" class="txt03 inputSize easyui-validatebox" readonly="readonly"  /></td>
            <td>限产企业数量：</td><td><input id="LIMITED_NUM" name="LIMITED_NUM" class="txt03 inputSize easyui-validatebox" readonly="readonly"  /></td>
            <td>时间段错峰企业数量：</td><td><input id="TIME_LIMITED_NUM" name="TIME_LIMITED_NUM" class="txt03 inputSize easyui-validatebox" readonly="readonly"  /></td>
        </tr>
    </table>
    <!--查询表单-->
    <div style="margin: 2px;margin-top:5px">
        <span>企业名称：</span>
        <input id="SearchCompanyName" name="NAME" class="txt03 inputSize" />
        <span>所属区域：</span>
        <select id="SearchAreaCode" name="AREACODE" class="txt03 inputSize"></select>
        <span>行业类型：</span>
        <select id="SearchIndustryType" name="INDUSTRY_TYPE" class="txt03 inputSize" ></select>
        <%=Html.LinkButton("a_search_edit_page","icon-folder_magnify","查询") %>
    </div>
    <table id="dgCompany"></table>
    <input id="extraData" type="hidden" name="extraData" />
</form>
<script>
    com.comboxDicCreateSearch("SearchIndustryType", "IndustryType");
    $("#SearchAreaCode").combotree({
        url: "/Bas/Common/GetAreaData",
        editable: false,
        checkbox: true,
        lines: true,
        onSelect: function (item) {
        }
    });
</script>
