<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            var model = new listModel();
            //对象1：列表页面对象
            var list = {
                controller: "/Bas/Org", //控制器
                dSize: { width: 4, height: 38 }, //偏移量
                dg: { //数据表格
                    typeName: "treegrid",
                    idField: "ID",
                    treeField: "ORGNAME",
                    sortName: "ID",
                    sortOrder: "asc",
                    pageSize: 10000,
                    pageList: [10000],
                    columns: [[
                        { title: "ID", field: "ID", width: 50 },
                        { title: "部门名称", field: "ORGNAME", width: 200 },
                        { title: "排序", field: "SORTNO", width: 60, align: "center" },
                        { title: "备注", field: "REMARK", width: 240 }
                    ]]
                }
            };
            //对象2：编辑页面对象
            var edit = {
                title: "组织结构", //标题
                size: { width: 350, height: 300 }, //页面大小
                onLoad: function (isAdd, row) { //页面加载事件
                    top.$("#ParentId").combotree({
                        url: "/Bas/Org/GetComboTreeOrg",
                        onSelect: function (item) {
                        }
                    });
                    top.$("#SortNo").numberspinner({ min: 0 });
                    if (isAdd == false) {
                        top.$("#OrgName").val(row.ORGNAME);
                        top.$("#ParentId").combotree("setValue", row.PARENTID > 0 ? row.PARENTID : -1);
                        top.$("#SortNo").numberspinner("setValue", row.SORTNO);
                        top.$("#Remark").val(row.REMARK);
                    }
                    else {
                        top.$("#ParentId").combotree("setValue", row ? row.ID : -1);
                        top.$("#OrgName").focus();
                    }
                }
            };
            model.bind(list, edit);
            $("#dg").datagrid("getPager").height(0);
            //其它初始化
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!--顶部工具栏-->
    <div id="toolbar">
        <%=Html.ToolBar(Model)%>
    </div>
    <!--查询表单-->
    <div style="margin: 2px;">
        <form id="qform"></form>
    </div>
    <!--数据列表-->
    <table id="dg"></table>
</asp:Content>
