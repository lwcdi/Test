<%@ Page Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="/Bas/Button/GetBtnColumns"></script>
    <style type="text/css">
        #toolbar {
            height: 32px !important;
            padding: 2px !important;
        }

        .tab-min {
            padding: 0 10px;
        }

            .tab-min span {
                display: inline-block;
                height: 26px;
                line-height: 26px;
                width: 80px;
                text-align: center;
                /*overflow:hidden;*/
                background-color: white;
                border: 1px solid #ccc;
                border-radius: 20px;
                cursor: pointer;
            }

                .tab-min span.on {
                    background-color: #00a5d1;
                    color: white;
                    border: 1px solid #00a5d1;
                }
    </style>
    <script type="text/javascript">
        var model;
        var list;
        var selectType = 1;
        var col = [];
        var col2 = [];
        var Vicecol = [];
      
        function getSelected() {
            var selObj = GetTreeSelected();
            // alert(selObj.TYPE + "," + selObj.ID);
            return selObj;
        }
        function CompanySelected(id) {

            debugger;
            $("#PKID").val(id);
            InitList(id);
            var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            var endDate = $("#RECTIME_END").datetimebox('getValue');
            var param = { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() };
            $("#dg").datagrid("load", param);
        }
        function PKSelected(id) {
            $("#PKID").val(id);
            debugger;
            InitList(id);
            var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            var endDate = $("#RECTIME_END").datetimebox('getValue');
            var param = { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() };
            $("#dg").datagrid("load", param);
        }

        //查看数据

        //var edit;
        $(function () {
            mylayout.init();
            debugger;
            model = new listModel();
            //PKSelected().bind("click", function () { InitList(this); })
            //var list;
            queryParamInit();
            InitList("1");
            InitFalg();
           
            //对象2：编辑页面对象
            var edit = {
                title: "错峰任务下发", //标题
                size: { width: 730, height: 430 }, //页面大小
                //dgList:dgList1,
                onLoad: function (isAdd, row) { //页面加载事件

                    parent.$('.dialog-button a:eq(0)').hide();
                    // com.ComboxBind(top, "#TYPE", "/Bas/Dic/GetCompanyTypeData", false);

                    //  top.$("#PEAK_ID").val("0");
                    if (isAdd == false && row && row.ID) {
                        // top.$("#PEAK_ID").val(row.ID);
                        //获取子列表数据
                        // this.dgList[0].url = "/ThePeakManager/ThePeakMange/GetLevelConfigData?id=" + row.ID;

                    }
                    else {
                        //  this.dgList[0].url = "/ThePeakManager/ThePeakMange/GetLevelConfigData";

                    }

                    //add 表示新增
                    if (isAdd == true) {
                        //    top.$("#trPassword").show();
                    }
                        //编辑并绑定变量
                    else {
                        //绑定
                        com.BinEditVal(top, row);
                    }

                },
                SaveAlter: function () {

                },

            };

            model.bind(list, edit);
            $("#a_search_cust").unbind("click").bind("click", function () {
                debugger;
                $("#PKID").val(getSelected().ID);
                model = new listModel();
                InitList(getSelected().ID);
                var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
                var endDate = $("#RECTIME_END").datetimebox('getValue');
                var param = { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() };
                $("#dg").datagrid("load", param);
            })

            $("#a_excel_cust").unbind("click").bind("click", function () {
                model.ExportClick();
            })

            //其它初始化
            $(window).resize(function () {
                mylayout.resize();
            });
         
           

        });

        var mylayout = {
            init: function () {
                var size = { width: $(window).width(), height: $(window).height() };
                $("#layout").width(size.width - 4).height(size.height - 4).layout();
                var center = $("#layout").layout("panel", "center");
                center.panel({
                    onResize: function (w, h) {
                        $("#dg").datagrid("resize", { width: w - 5, height: h - 85 });
                    }
                });
            },
            resize: function () {
                mylayout.init();
                $("#layout").layout("resize");
            }
        };

        var OpenEdit = function (leveID, peakLevel, isAdd, index) {
            debugger;
            model.edit.openAfter = function () {

                var selectObj = $("#dg").datagrid('getSelected', index);
                top.ShowThePeakWindow(leveID, peakLevel, "", false, selectObj);

                // parent.ShowThePeakWindow(leveID, thePeakId, isAdd, index);
            }
            var editShow = model.showEdit(true);
            editShow.dialog("close");

        }
       
        //var openAfter 
        var queryParamInit = function () {

         //   com.ComboxBind("PEAK_TYPE", "/Bas/Dic/GetDicCodeDataForSearch?typeCode=EmergencyType");

            $('#RECTIME_STRAT').datetimebox({
                required: false,
                editable: false,
            });
            $('#RECTIME_END').datetimebox({
                required: false,
                editable: false,

            });
            var currentDate = new Date();

            date2 = currentDate.getFullYear() + "\-" + (currentDate.getMonth() + 1) + "\-" + currentDate.getDate();
            date2 += " " + currentDate.getHours() + ":";
            date2 += currentDate.getMinutes()-10 + ":";
            date2 += currentDate.getSeconds();

            date = currentDate.getFullYear() + "\-" + (currentDate.getMonth() + 1) + "\-" + currentDate.getDate();
            date += " " + currentDate.getHours() + ":";
            date += currentDate.getMinutes() + ":";
            date += currentDate.getSeconds();
            $("#RECTIME_STRAT").datetimebox('setValue', date2);
            $("#RECTIME_END").datetimebox('setValue', date);
        }
        var goHistoryDetail = function (thepeakid) {
            var dialog = top.$.dialogX({
                title: "历史错峰生产事件",
                width: 1000,
                height: 700,
                href: "/Supervise/ThepeakHistoryStatistics/Index?thepeakid=" + thepeakid,
                onLoad: function () {
                    //页面数据的初始化
                    parent.$('.dialog-button a:eq(0)').hide();
                },
                submit: function () {
                }
            });


        }

        function DataTableClick() {
            debugger
           // InitList("2");
            $("#dataTable").show();
            $("#picData").hide();
            $("#butData").attr("class", "on");
            $("#butPic").attr("class", "");
            InitList(getSelected().ID);
            var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            var endDate = $("#RECTIME_END").datetimebox('getValue');
            var param = { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() };
            $("#dg").datagrid("load", param);
        }

        //查看图表
        function PicClick() {
            debugger
           // InitList("2");
            $("#picData").show();
            $("#dataTable").hide();
            $("#butData").attr("class", "");
            $("#butPic").attr("class", "on");
            debugger;
            GetChart();
        }

       
        var InitList = function (selectTreeid)
        {
          
                try {
                    col = [];
                    col2 = [];
                    var getSelectId = selectTreeid;
                    if (selectTreeid == "")
                        getSelectId = getSelected().ID;
                    $.ajax({
                        type: "POST",
                        //   contentType: "application/json",
                        dataType: "json",
                        url: "/StainMonitor/StainMonitor/GetHeadText?number=" + Math.random(),
                        cache: false,
                        async: false, //同步执行
                        data: { PKID: selectTreeid, Key: com.helper.guid() },
                        success: function (result) {
                            // debugger;
                            // return;

                        

                            if (result.Success == true) {
                                var title = result.TITLE;
                                var dataList = result.DATA;
                                col.push({ field: 'RECTIME', title: '时间', align: 'center', rowspan: 2 });
                                title.MAINTITLE.forEach(function (item, index) {
                                    if (item.COLSPAN > 1)
                                        col.push({ field: item.ITEMCODE, title: item.ITEMTEXT, align: 'center', colspan: item.COLSPAN });
                                    else
                                        col.push({ field: item.ITEMCODE, title: item.ITEMTEXT + '(' + item.UNIT + ')', align: 'center', rowspan: 2 });
                                });
                                title.SUBTITLE.forEach(function (item, index) {
                                    col2.push({ field: item.ITEMCODE + '_' + item.SUBITEMCODE, title: item.SUBITEMTEXT + '(' + item.UNIT + ')', align: 'center' });
                                });
                                var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
                                var endDate = $("#RECTIME_END").datetimebox('getValue');
                             

                                list = {
                                    controller: "/StainMonitor/StainMonitor",//控制器
                                    dSize: { width: 295, height: 105 },//偏移量
                                    dg: { //数据表格
                                        idField: "ID",
                                        sortName: "ID",
                                        sortOrder: "asc",
                                        columns: [col, col2], //, Vicecol
                                        queryParams: { PKID: selectTreeid, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() },
                                       }
                                };
                              
                              
                            }
                            else {
                                com.msg.error("失败！");
                            }
                        }

                    });
                }
                catch (e)
                { alert("当前排口/企业没有数据！"); }
           
        }


        var GetChart = function () {
            $("#PKID").val(getSelected().ID);
            var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            var endDate = $("#RECTIME_END").datetimebox('getValue');
           // var param = { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() };
          $.ajax({
                type: "post",
                url: "/StainMonitor/StainMonitor/GetPicData",
                data: { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() },
                dataType: "json", //返回数据形式为json
                success: function (result) {
                   var div = document.getElementById('picData');
                   var  myChart = echarts.init(div)
                   myChart.setOption(eval('(' + result + ')'));
                }
            });
        }

        var InitFalg = function ()
        {
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
                 
                }
            });
       
        }


    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
    <div id="layout">
        <div region="west" split="true" style="width: 280px; padding: 5px">
            <%
                //ViewDataDictionary dict = new ViewDataDictionary();
                //    dict.Add("fs", ViewData["fs"]);
                //dict.Add("fq", ViewData["fq"]);
                //dict.Add("vocs", ViewData["vocs"]);
                //dict.Add("selectId", ViewData["selectId"]);
                //dict.Add("multiSelect", false);
                Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = ViewBag.controllerParam });
            %>
        </div>
        <div style="float: left; left: 290px; margin: 5px">
            <a class="easyui-linkbutton" onclick="getSelected()">获取选中</a>
        </div>

        <div region="center" title="实时监控" iconcls="icon-clock">
            <!--顶部工具栏-->
            <div id="toolbar">
                <form id="qform">

                    <div style="float: left; padding: 5px;">

                        <span id="buttonDiv" class="tab-min">
                            <span id="butData" onclick='DataTableClick()' class="on">数据</span>
                            <span id="butPic" onclick='PicClick()'>图表</span>
                        </span>

                        <span>周期类型：</span>
                        <label style="font-size: 12px;">
                            <input name="DataType" type="radio" value="M" checked="checked" />分钟
                        </label>
                        <label style="font-size: 12px;">
                            <input name="DataType" type="radio" value="H" />小时
                        </label>
                        <label style="font-size: 12px;">
                            <input name="DataType" type="radio" value="D" />天
                        </label>

                        <span>时间：</span>
                        <select id="RECTIME_STRAT" name="BeginTime" style="width: 175px;" class="txt03"></select>
                        <span>至：</span>
                        <select id="RECTIME_END" name="EndTime" style="width: 175px;" class="txt03"></select>
                    </div>
                       <input type="hidden" id="PKID" name="PKID" value="" />
                         
                </form>
             
                 <%=Html.LinkButton("a_search_cust","icon-folder_magnify","查询") %>  
                  <%=Html.LinkButton("a_excel_cust","icon-page_excel","导出") %>    <%--<%=Html.ToolBar(Model) %>--%>      
            
                
               
                
            </div>

            <!--查询表单-->
            <div style="margin: 2px;">
       
            </div>
            <!--数据列表-->
            <div id="dataTable">
                 <div style="display: -webkit-inline-box; margin: 5px 0">
                <span style="background-color: red">超标</span>
                <span style="background-color: sandybrown">异常</span>
                <div id="dataFlag" style="position: relative; left: 10px"></div>
            </div>
                <table id="dg"></table>
            </div>

            <div id="picData" style="display: none;width:95%;height:93%">


            </div>
        </div>
       
    </div>
</asp:Content>
