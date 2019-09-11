<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div style="float: left; width: 250px">
        <%
            ViewDataDictionary dict = new ViewDataDictionary();
            dict.Add("fs", ViewData["fs"]);
            dict.Add("fq", ViewData["fq"]);
            dict.Add("vocs", ViewData["vocs"]);
            dict.Add("selectId", ViewData["PKId"]);
            dict.Add("multiSelect", true);
            Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = dict });
        %>
    </div>
    <div id="chart" style="position: absolute; left: 290px; width: 500px; height: 400px">
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function PKSelected() {
            GetChart();
        }
        function AfterLoadSuccess() {
            GetChart();
        }
        function GetChart() {
            var nodes = GetMutiSelected();

            $.ajax({
                type: "post",
                url: "/Monitor/DataRounding/GetChartBySingleItem",
                data: {
                    PkId: nodes.PID.toString(),
                    item: '<%=ViewData["ITEM"]%>',
                    time: '<%=ViewData["TIME"]%>'
                },
                dataType: "json", //返回数据形式为json
                success: function (result) {
                    var div = document.getElementById('chart');
                    myChart = echarts.init(div)
                    myChart.setOption(eval('(' + result + ')'));
                }
            });
        }

    </script>
</asp:Content>
