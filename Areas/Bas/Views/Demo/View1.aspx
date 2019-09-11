<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function getSelected() {
            var selObj = GetTreeSelected();
            alert(selObj.TYPE + "," + selObj.ID);
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!--顶部工具栏-->
    <div style="float: left; width: 280px">
        <%Html.RenderAction("GroupSelect", "UserControl", new { selectId = ViewData["selectId"].ToString() , singleSelect = true});%>
    </div>
    <div style="float: left;left:290px;margin:5px">
        <a class="easyui-linkbutton" onclick="getSelected()">获取选中</a>
    </div>
</asp:Content>
