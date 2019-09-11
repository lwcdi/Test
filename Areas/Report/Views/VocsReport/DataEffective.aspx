<%@ Page Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        #toolbar {
            height: 32px !important;
            padding: 2px !important;
        }
        .timeRang{
            display:none;
        }
    </style>
    <script>
        $(function () {
            mylayout.init();
            rdlc.yearCombobox("timeRangYear");
            rdlc.quarterCombobox("timeRangQuarter");
            $("#btnCreate").click(function () {
                debugger;
                var companyInfo = GetMutiSelected();
                var pID = [];
                if (companyInfo && companyInfo.PID) {
                    pID = companyInfo.PID;
                }
                if (pID.length<1) {
                    com.msg.alert("请先选择排口");
                    return;
                }
                var param = new com.url.createParam();
                param.addParam({ "PK_ID": JSON.stringify(pID) });
                var timeRangYear = $("#timeRangYear").combobox('getValue');
                var timeRangQuarter = $("#timeRangQuarter").combobox('getValue');
                param.addParam({ "timeRangYear": timeRangYear });
                param.addParam({ "timeRangQuarter": timeRangQuarter });
                if (timeRangYear && timeRangQuarter) {
                    $("#iBtn").attr("src", "/Areas/Report/Aspx/VocsEffective.aspx?" + param.toUrlParam());
                } else {
                    com.msg.alert("请选择时间");
                }
                param.empty();
            });
        });

        var mylayout = {
            init: function () {
                var size = { width: $(window).width(), height: $(window).height() };
                $("#layout").width(size.width - 4).height(size.height - 4).layout();
                var center = $("#layout").layout("panel", "center");
                center.panel({
                    onResize: function (w, h) {
                        //$("#dg").datagrid("resize", { width: w - 3, height: h - 70 });
                    }
                });
            }
           ,
            resize: function () {
                mylayout.init();
                $("#layout").layout("resize");
            }
        };
    </script>
    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div id="layout">
    <div region="west" split="true" style="width: 270px; padding: 5px"><!--180-->
       <%
            Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = ViewBag.controllerParam});
       %>
    </div>
    <div region="center" title="Vocs排口数据有效率报表" iconcls="icon-house">
        <!--顶部工具栏-->
        <div id="toolbar">
            <form id="qform">
                <div style="float: left; padding: 5px;"></div>
                <table>
                    <tr>
                         <td>时间：</td>
                        <td>
                            <select id="timeRangYear" name="timeRangYear" class="txt03 inputSize"></select>
                            <select id="timeRangQuarter" name="timeRangQuarter" class="txt03 inputSize"></select>
                        </td>
                        <td><button id="btnCreate" type="button">生成</button></td>
                    </tr>
                </table>
            </form>
        </div>
        <div style="height:90%">
             <iframe id="iBtn" src="" style="width:99%;height:100%;border:0px solid #ffffff"></iframe><!--/Areas/Report/Aspx/AirDay.aspx-->
        </div>

    </div>
</div>
</asp:Content>


