<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Content/css/Page.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .cbColor {
            background-color: orangered;
        }

        .ycColor {
            background-color: sandybrown;
            display: block;
            width: 50px;
            text-align: center;
        }

        .tooltipcontent {
            z-index: 1;
            position: absolute;
            border: 1px solid #FFC30E;
            padding: 4px;
            background-color: #FFFBB8;
            text-align: left;
            color: #9C7600;
            font-size: 12px;
            font-family: arial, sans-serif;
            float: right;
            height: 24px;
        }

        .edit_botton {
            padding: 5px 0px;
            cursor: pointer;
            margin: 0 2px;
        }

        #toolbar {
            height: 55px !important;
            padding: 2px !important;
        }
    </style>
    <script type="text/javascript">
        //var dataStatus = new Array();
        var ycColor = 'sandybrown';
        var item = '';
        var row;
        $(function () {
            mylayout.init();
            var currentDate = new Date();
            date = currentDate.getFullYear() + "\-" + (currentDate.getMonth() + 1) + "\-" + currentDate.getDate();
            $("#date").datebox({
                required: false,
                editable: false,
            }).datebox('setValue', date);

            $("#DATASOURCE").combobox({
                url: "/Bas/Dic/GetDicCodeData?typeCode=DATASOURCE",
                valueField: "CODE",
                textField: "TITLE",
                required: true,
                editable: false,
                panelHeight: "auto",
                onLoadSuccess: function (data) {
                }
            });
            $.ajax({
                fit: true,
                type: "post",
                async: false, //同步执行
                url: "/Monitor/DataRounding/GetStatusFlag",
                dataType: "json", //返回数据形式为json
                success: function (result) {
                    var html = '';
                    result.forEach(function (item, index) {
                        html += '<span style="color:blue;position:relative;left:5px">' + item.CODE + '：</span>';
                        html += item.TITLE;

                        //dataStatus[item.CODE] = item.REMARK;
                    });
                    $("#dataFlag")[0].innerHTML = html;
                    $("#DATASTATUS").combobox({
                        valueField: "CODE",
                        textField: "TITLE",
                        required: true,
                        editable: false,
                        panelHeight: "auto",
                        onLoadSuccess: function (data) {
                        }
                    }).combobox('loadData', result);
                }
            });
        });
        function getData() {
            $('#dg').datagrid({ columns: [new Array()] });
            $('#dg').datagrid('loadData', { total: 0, rows: [] });
            var selectedNode = GetTreeSelected();
            $.ajax({
                //fit: true,
                type: "post",
                async: false, //同步执行
                url: "/Monitor/DataRounding/GetData",
                data: {
                    PkId: selectedNode.ID,
                    Date: $("#date").datebox('getValue'),
                    DataSource: '',//$("#DATASOURCE").combobox('getValue'),
                    DataStatus: ''//$("#DATASTATUS").combobox('getValue')
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
                                field: item.ITEMCODE, title: item.ITEMTEXT + '(' + item.UNIT + ')', align: 'center', rowspan: 2,
                                formatter: function (value, row, index) {
                                    if (selectedNode.OBJTYPE == '2') {//废水
                                        if (!value) return '<div onclick="showWaterMenu(this,\'' + item.ITEMCODE + '\',\'' + index + '\')">&nbsp;&nbsp;&nbsp;&nbsp;</div>';
                                        else return value;
                                    }
                                    else {
                                        if (!value) return '<div onclick="showAirMenu(this,\'' + item.ITEMCODE + '\',\'' + index + '\')">&nbsp;&nbsp;&nbsp;&nbsp;</div>';
                                        if (row[item.ITEMCODE + "_R"]) {
                                            return '<div onclick="backRounding(this,\'' + +row[item.ITEMCODE + "_CID"] + '\')">' + value + '(' + row[item.ITEMCODE + "_R"] + ')' + '</div>';
                                        }
                                        else {
                                            if (row[item.ITEMCODE + "_STATUS"] != undefined)
                                                return '<div onclick="showAirMenu(this,\'' + item.ITEMCODE + '\',\'' + index + '\')">' + value + '(' + row[item.ITEMCODE + "_STATUS"] + ')' + '</div>';
                                            else return '<div onclick="showAirMenu(this,\'' + item.ITEMCODE + '\',\'' + index + '\')">' + value + '</div>';
                                        }
                                    }
                                },
                                styler: function (value, row, index) {
                                    if (row[item.ITEMCODE + "_STATUS"] != undefined) {
                                        //if (dataStatus[row[item.ITEMCODE + "_STATUS"]] == 'CB')
                                        //    return 'background-color:' + cbColor;
                                        //else if (dataStatus[row[item.ITEMCODE + "_STATUS"]] == 'YC')
                                        //    return 'background-color:' + ycColor;       
                                        if (row[item.ITEMCODE + "_STATUS"] && row[item.ITEMCODE + "_STATUS"] != 'N')
                                            return 'background-color:' + ycColor;
                                    }
                                }
                            });
                    });
                    title.SUBTITLE.forEach(function (item, index) {
                        subCol.push({
                            field: item.ITEMCODE + '_' + item.SUBITEMCODE, title: item.SUBITEMTEXT + '(' + item.UNIT + ')', align: 'center',
                            formatter: function (value, row, index) {
                                if (selectedNode.OBJTYPE == '2') {//废水
                                    if (!value) return '<div onclick="showWaterMenu(this,\'' + item.ITEMCODE + '_' + item.SUBITEMCODE + '\',\'' + index + '\')">&nbsp;&nbsp;&nbsp;&nbsp;</div>';
                                    else return value;
                                }
                                else {
                                    if (!value) return '<div onclick="showAirMenu(this,\'' + item.ITEMCODE + '_' + item.SUBITEMCODE + '\',\'' + index + '\')">&nbsp;&nbsp;&nbsp;&nbsp;</div>';
                                    if (row[item.ITEMCODE + '_' + item.SUBITEMCODE + "_R"])
                                        return '<div onclick="backRounding(this,\'' + row[item.ITEMCODE + '_' + item.SUBITEMCODE + "_CID"] + '\')">' + value + '(' + row[item.ITEMCODE + '_' + item.SUBITEMCODE + "_R"] + ')' + '</div>';
                                    else {
                                        if (row[item.ITEMCODE + '_' + item.SUBITEMCODE + "_STATUS"] != undefined)
                                            return '<div onclick="showAirMenu(this,\'' + item.ITEMCODE + '_' + item.SUBITEMCODE + '\',\'' + index + '\')">' + value + '(' + row[item.ITEMCODE + '_' + item.SUBITEMCODE + "_STATUS"] + ')' + '</div>';
                                        else return '<div onclick="showAirMenu(this,\'' + item.ITEMCODE + '_' + item.SUBITEMCODE + '\',\'' + index + '\')">' + value + '</div>';
                                    }
                                }
                            },
                            styler: function (value, row, index) {
                                if (row[item.ITEMCODE + '_' + item.SUBITEMCODE + "_STATUS"] != undefined) {
                                    //if (dataStatus[row[item.ITEMCODE + '_' + item.SUBITEMCODE + "_STATUS"]] == 'CB')
                                    //    return 'background-color:' + cbColor;
                                    //else if (dataStatus[row[item.ITEMCODE + '_' + item.SUBITEMCODE + "_STATUS"]] == 'YC')
                                    //    return 'background-color:' + ycColor;

                                    if (row[item.ITEMCODE + '_' + item.SUBITEMCODE + "_STATUS"] && row[item.ITEMCODE + '_' + item.SUBITEMCODE + "_STATUS"] != 'N')
                                        return 'background-color:' + ycColor;
                                }
                            }
                        });
                    });
                    $("#dg").datagrid({
                        singleSelect: true, //选中一行的设置
                        rownumbers: true, //行号
                        columns: [col, subCol],
                        pagination: false,
                        onLoadSuccess: function () {
                            mylayout.resize();
                        },
                        onClickCell: function (index, field, value) {
                            //showMenu(this, field, index);
                        }
                    }).datagrid('loadData', dataList);
                    //$('#dg').datagrid('doEvents');
                },
                error: function (errorMsg) {
                }
            });

        }
        function showWaterMenu(target, field, index) {
            $('.tooltipcontent').remove();
            item = field;
            var rows = $("#dg").datagrid('getRows');
            row = rows[index];
            var value = row[item];
            var dataStatus = rows[index][field + "_STATUS"];
            var html = '';
            if (!value) {//待回补
                html = '<div class="tooltipcontent">'
                   + '<a class="edit_botton" href="#"onclick="Rounding()" ><span title="数据修约"><i class="icons-pencil"></i></span></a>'
                   + '<a class="edit_botton" href="#" onclick="GetMinDataByPoint();"><span title="审核参考"><i class="icons-book-open"></i></span></a>'
                   + '<a class="edit_botton" href="#" onclick="CheckRecord();"><span title="审核记录"><i class="icons-date-magnify"></i></span></a>'
                   + '<a class="edit_botton" href="#" onclick="DataByMutiPoint();"><span title="查看所有站点"><i class="icons-chart-curve"></i></span></a>'
                   + '</div>'
            }
            $(target).append(html);
        }
        function backRounding(target, CID) {
            $('.tooltipcontent').remove();
            html = '<div class="tooltipcontent">'
                     + '<a class="edit_botton" href="#" onclick="BackRounding(' + CID + ')"><span title="撤回修约"><i class="icons-undo"></i></span></a>'
                     + '<a class="edit_botton" href="#" onclick="GetMinDataByPoint();"><span title="审核参考"><i class="icons-book-open"></i></span></a>'
                     + '<a class="edit_botton" href="#" onclick="CheckRecord();"><span title="审核记录"><i class="icons-date-magnify"></i></span></a>'
                     + '<a class="edit_botton" href="#" onclick="DataByMutiPoint();"><span title="查看所有站点"><i class="icons-chart-curve"></i></span></a>'
                     + '</div>'

            $(target).append(html);
        }
        function showAirMenu(target, field, index) {
            $('.tooltipcontent').remove();
            item = field;
            var rows = $("#dg").datagrid('getRows');
            row = rows[index];
            var value = row[item];
            var dataStatus = rows[index][field + "_STATUS"];
            var html = '';
            if (!value) {//待回补
                html = '<div class="tooltipcontent">'
                   + '<a class="edit_botton" href="#"><span title="回补数据"><i class="icons-database-refresh"></i></span></a>'
                   + '<a class="edit_botton" href="#" onclick="GetMinDataByPoint();"><span title="审核参考"><i class="icons-book-open"></i></span></a>'
                   + '<a class="edit_botton" href="#" onclick="CheckRecord();"><span title="审核记录"><i class="icons-date-magnify"></i></span></a>'
                   + '<a class="edit_botton" href="#" onclick="DataByMutiPoint();"><span title="查看所有站点"><i class="icons-chart-curve"></i></span></a>'
                   + '</div>'
            }
            else if (dataStatus == null || dataStatus == 'N') {//正常数据
                //html = '<div class="tooltipcontent" style="width: 120px;">'
                //   + '<a class="edit_botton" href="#"onclick="Rounding()" ><span title="数据修约"><i class="icons-pencil"></i></span></a>'
                //   + '<a class="edit_botton" href="#" onclick="GetMinDataByPoint()"><span title="审核参考"><i class="icons-book-open"></i></span></a>'
                //   + '<a class="edit_botton" href="#" onclick="menuMagnify(this);singleSelect(this);"><span title="修约记录"><i class="icons-date-magnify"></i></span></a>'
                //   + '<a class="edit_botton" href="#" onclick="DataByMutiPoint()"><span title="查看站点数据"><i class="icons-chart-curve"></i></span></a>'
                //   + '</div>'
            }
            else {//待修约
                html = '<div class="tooltipcontent">'
                   + '<a class="edit_botton" href="#"onclick="Rounding()" ><span title="数据修约"><i class="icons-pencil"></i></span></a>'
                   + '<a class="edit_botton" href="#" onclick="GetMinDataByPoint()"><span title="审核参考"><i class="icons-book-open"></i></span></a>'
                   + '<a class="edit_botton" href="#" onclick="CheckRecord();"><span title="审核记录"><i class="icons-date-magnify"></i></span></a>'
                   + '<a class="edit_botton" href="#" onclick="DataByMutiPoint()"><span title="查看所有站点"><i class="icons-chart-curve"></i></span></a>'
                   + '</div>'
            }
            $(target).append(html);
        }
        function PKSelected(id) {
            getData();
        }
        function DataByMutiPoint() {
            var node = GetTreeSelected();
            var url = "/Monitor/DataRounding/DataByMutiPoint?time=" + row["TIME"].replace(' ', '+') + ":00:00" + "&item=" + item + "&PKId=" + node.TYPE + node.ID + "&fs=" + '<%=ViewData["fs"]%>' + "&fq=" + '<%=ViewData["fq"]%>' + "&vocs=" + '<%=ViewData["vocs"]%>';
            var dialog = $("<iframe src=" + url + "></iframe>").dialog({
                title: "查看站点数据",
                width: 800,
                height: 500,
                cache: false,
                modal: true,
                buttons: [
                 {
                     text: '关闭',
                     iconCls: 'icon-cancel',
                     handler: function () {
                         dialog.dialog("close");
                     }
                 }]
            });
        }
        function BackRounding(CID) {
            $.ajax({
                //fit: true,
                type: "post",
                async: false, //同步执行
                url: "/Monitor/DataRounding/BackRounding",
                data: {
                    CID: CID,
                },
                dataType: "json", //返回数据形式为json
                success: function (result) {
                    if (result.Result) {
                        getData();
                        com.msg.alert('撤销修约成功！');
                    }
                    else com.msg.alert(result.Msg);
                }
            });
        }
        function GetMinDataByPoint() {
            var node = GetTreeSelected();
            var url = "/Monitor/DataRounding/MinDataByPoint?time=" + row["TIME"].replace(' ', '+') + ":00:00" + "&PKId=" + node.ID;
            var dialog = $('<iframe src="' + url + '" style="border: 0px; width: 100%; height: 600px; overflow: auto;"></iframe>').dialog({
                title: "审核参考",
                width: 1000,
                height: 500,
                cache: false,
                modal: true,
                buttons: [
                 {
                     text: '关闭',
                     iconCls: 'icon-cancel',
                     handler: function () {
                         dialog.dialog("close");
                     }
                 }]
            });
        }
        function Rounding() {
            var node = GetTreeSelected();
            var url = "/Monitor/DataRounding/Rounding?time=" + row["TIME"].replace(' ', '+') + ":00:00" + "&item=" + item + "&PKId=" + node.ID;
            var dialog = $('<iframe src="' + url + '" style="border: 0px; width: 100%; height: 600px; overflow: auto;"></iframe>').dialog({
                title: "数据修约",
                width: 950,
                height: 600,
                cache: false,
                modal: true,
                buttons: [
                {
                    text: '关闭',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        getData();
                        dialog.dialog("close");
                    }
                }]
            });
        }
        var CheckRecord = function () {
            var node = GetTreeSelected();
            var url = "/Monitor/CheckRecord/ListExt?stime=" + row["TIME"].replace(' ', '+') + ":00:00" + "&etime=" + row["TIME"].replace(' ', '+') + ":00:00" + "&PKId=P" + node.ID + "&fs=" + '<%=ViewData["fs"]%>' + "&fq=" + '<%=ViewData["fq"]%>' + "&vocs=" + '<%=ViewData["vocs"]%>';
            var dialog = $('<iframe src="' + url + '" style="border: 0px; width: 100%; height: 600px; overflow: auto;"></iframe>').dialog({
                title: "修约记录",
                width: 1000,
                height: 500,
                cache: false,
                modal: true,
                buttons: [
                {
                    text: '关闭',
                    iconCls: 'icon-cancel',
                    handler: function () {
                        dialog.dialog("close");
                    }
                }]
            });
        }
        var mylayout = {
            init: function () {
                var size = { width: $(window).width(), height: $(window).height() };
                $("#layout").width(size.width - 4).height(size.height - 4).layout();
                var center = $("#layout").layout("panel", "center");
                center.panel({
                    onResize: function (w, h) {
                        $("#dg").datagrid("resize", { width: w - 5, height: h });
                        $("#toolbar").width(w - 5).layout()
                    }
                });
            },
            resize: function () {
                mylayout.init();
                $("#layout").layout("resize");
            }
        };
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="layout">
        <div region="west" split="true" style="width: 280px; padding: 5px">
            <%
                ViewDataDictionary dict = new ViewDataDictionary();
                dict.Add("fs", ViewData["fs"]);
                dict.Add("fq", ViewData["fq"]);
                dict.Add("vocs", ViewData["vocs"]);
                dict.Add("selectId", ViewData["PKId"]);
                dict.Add("companyOnly", ViewData["companyOnly"]);
                dict.Add("multiSelect", false);
                Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = dict });
            %>
        </div>
        <div region="center" title="数据修约" iconcls="icon-clock" style="overflow: auto">
            <div id="toolbar">
                <div>
                    日期：
                        <select id="date" name="date" style="width: 175px;" class="txt03"></select>
                    <a plain="true" class="easyui-linkbutton" icon="icon-search" onclick="getData()">查询</a>
                </div>
                <div style="display: -webkit-inline-box; margin: 5px 0">
                    <span class="ycColor">异常</span>
                    <div id="dataFlag" style="position: relative; left: 10px"></div>
                </div>
            </div>
            <table id="dg"></table>
        </div>
    </div>
</asp:Content>
