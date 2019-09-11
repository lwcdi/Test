<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site2.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="dg" style="width:100%; height: 100%;">
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $.ajax({
                type: "post",
                url: "/Monitor/DataRounding/GetLastMinData",
                data: {
                    PKId: '<%=ViewData["PKId"]%>',
                    Date: '<%=ViewData["TIME"]%>'
                },
                dataType: "json", //返回数据形式为json
                success: function (result) {
                    if (!result) return;
                    var title = result.TITLE;
                    var dataList = result.DATA;

                    var col = new Array(); //主列
                    var subCol = new Array(); //子列
                    col.push({
                        field: 'TIME', title: '时间', align: 'center', rowspan: 2
                    });
                    title.MAINTITLE.forEach(function (item, index) {
                        if (item.COLSPAN > 1)
                            col.push({ field: item.ITEMCODE, title: item.ITEMTEXT, align: 'center', colspan: item.COLSPAN });
                        else
                            col.push({
                                field: item.ITEMCODE, title: item.ITEMTEXT + '(' + item.UNIT + ')', align: 'center', rowspan: 2
                            });
                    });
                    title.SUBTITLE.forEach(function (item, index) {
                        subCol.push({
                            field: item.ITEMCODE + '_' + item.SUBITEMCODE, title: item.SUBITEMTEXT + '(' + item.UNIT + ')', align: 'center'
                        });
                    });
                    $("#dg").datagrid({
                        singleSelect: true, //选中一行的设置
                        rownumbers: true, //行号
                        columns: [col, subCol],
                        pagination: false,
                        onClickCell: function (index, field, value) {
                            //if (!value) showMenu('', undefined);
                        }
                    }).datagrid('loadData', dataList);
                }
            });
        });
    </script>
</asp:Content>
