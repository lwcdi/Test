<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<form id="uiform">
    <div id="userTab" fit="true" style="height: 340px; overflow: hidden;">
        <div title="基本信息" style="padding: 2px; overflow: hidden;">
            <table class="grid">
                <tr>
                    <td style="width: 100px;">用 &nbsp;户&nbsp; 名：</td>
                    <td>
                        <input id="UserName" name="UserName" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" />
                    </td>
                </tr>
                <tr>
                    <td>真实姓名：</td>
                    <td>
                        <input id="TrueName" name="TrueName" type="text" class="txt03 easyui-validatebox" data-options="required:true" style="width: 200px" /></td>
                </tr>
                <tr id="trPassword" style="display: none;">
                    <td>密　　码：</td>
                    <td>
                        <input id="Password" name="Password" type="password" class="txt03" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>是否超管：</td>
                    <td>
                        <select id="IsAdmin" name="IsAdmin" style="width: 200px;">
                            <option value="false">否</option>
                            <option value="true">是</option>
                        </select></td>
                </tr>
                <tr>
                    <td>是否可用：</td>
                    <td>
                        <select id="IsDisabled" name="IsDisabled" style="width: 200px">
                            <option value="true">激活</option>
                            <option value="false">禁用</option>
                        </select></td>
                </tr>
                <tr>
                    <td>用户类型：</td>
                    <td>
                        <input id="UserTypeId" name="UserTypeId" /></td>
                </tr>
                <tr>
                    <td>所属区域：</td>
                    <td>
                        <input id="AreaCode" name="AreaCode" /></td>
                </tr>
                <tr>
                    <td style="width: 190px;">隶属组织机构</td>
                    <td>
                        <input id="OrgId" name="OrgId" type="text" style="width: 200px" /></td>
                </tr>
               <%-- <tr id="trVOS" style="display: none">

                    <td colspan="2">
                        <input id="chkvisible" name="chkvisible" style="vertical-align: middle" type="checkbox" value="1" />
                        <label for="chkvisible" style="vertical-align: middle">是否VOCs监管负责人</label>

                        <input id="chkzddlhead" name="chkzddlhead" style="vertical-align: middle" type="checkbox" value="1" />
                        <label for="chkzddlhead" style="vertical-align: middle">是否重点道路负责人</label>
                        <input id="chkzdqyhead" name="chkzdqyhead" style="vertical-align: middle" type="checkbox" value="1" />
                        <label for="chkzdqyhead" style="vertical-align: middle">是否重点区域负责人</label>
                    </td>
                </tr>--%>
            </table>
        </div>
        <div title="联系方式" style="padding: 2px">
            <table class="grid">
                <tr>
                    <td style="width: 100px;">邮　　箱：</td>
                    <td>
                        <input id="Email" name="Email" type="text" class="txt03 easyui-validatebox" data-options="validType:'email'" style="width: 200px" /></td>
                </tr>
                <tr>
                    <td>手　　机：</td>
                    <td>
                        <input id="Mobile" name="Mobile" type="text" style="width: 200px;" class="txt03" /></td>
                </tr>
                <tr>
                    <td>Q 　　Q：</td>
                    <td>
                        <input id="QQ" name="QQ" type="text" style="width: 200px;" class="txt03" /></td>
                </tr>
                <tr>
                    <td>备　　注：</td>
                    <td>
                        <textarea id="Remark" name="Remark" style="width: 300px; height: 50px" class="txt03" /></td>
                </tr>
            </table>
        </div>
    </div>
</form>
<script type=""></script>

