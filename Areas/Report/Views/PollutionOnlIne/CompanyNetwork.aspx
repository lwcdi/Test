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
            $("#btnCreate").click(function () {
                var companyInfo = GetMutiSelected();
                var companyID = [];
                if (companyInfo && companyInfo.CID) {
                    companyID = companyInfo.CID;
                }
                if (companyID.length < 1) {
                    com.msg.alert("请先选择企业");
                    return;
                }
                var param = new com.url.createParam();
                param.addParam({ "companyID": JSON.stringify(companyID) });
                $("#iBtn").attr("src", "/Areas/Report/Aspx/PollutionNetwork.aspx?" + param.toUrlParam());
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
    <div region="center" title="企业联网统计报表" iconcls="icon-house">
        <!--顶部工具栏-->
        <div id="toolbar">
            <form id="qform">
                <div style="float: left; padding: 5px;"></div>
                <table>
                    <tr>
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


