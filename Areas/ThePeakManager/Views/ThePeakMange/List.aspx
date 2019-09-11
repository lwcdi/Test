<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
     <script type="text/javascript" src="/Bas/Button/GetBtnColumns"></script>
    <style type="text/css">
        #toolbar {
            height: 32px !important;
            padding: 2px !important;
        }
    </style>
    <script type="text/javascript">
        var model;
        //var edit;
        $(function () {
            mylayout.init();
            model = new listModel();

            var list = {
                controller: "/ThePeakManager/ThePeakMange",//控制器
                dSize: { width: 10, height: 75 },//偏移量
                dg: { //数据表格
                    
                    idField: "ID",
                    sortName: "ID",
                    sortOrder: "asc",
                    columns: [[
                        //{ title: "名称", field: "PEAK_NAME", width: 120 },
                        { title: "应急类型", field: "PEAK_TYPE_TEXT", width: 80 },
                        { title: "编号", field: "PEAK_NO", width: 80 },
                        { title: "应急等级", field: "PEAK_LEVEL_TEXT", width: 80 },
                        { title: "预警类型", field: "WAR_TYPE_TEXT", width: 80 },
                        { title: "主题", field: "PEAK_THEME", width: 150 },
                        {
                            title: "响应区域", field: "RESPONSE_AREA", width: 120, formatter: function (value, row, index) {
                                return '<span class="areaText" data-codes="' + value + '"></span>';
                            }
                        },
                        { title: "下发时间", field: "DOWN_TIME", width: 120, formatter: com.formatter.dateTime },
                        { title: "启动时间", field: "START_TIME", width: 120, formatter: com.formatter.dateTime },
                        { title: "结束时间", field: "END_TIME", width: 120, formatter: com.formatter.dateTime },
                        { title: "说明", field: "PEAK_DESC", width: 120 },
                        { title: "全部数量", field: "ALL_NUM", width: 80 },
                        { title: "停产数量", field: "STOP_NUM", width: 80 },    
                        {
                            title: "限产数量", field: "LIMITED_NUM", width: 80, formatter: function (value, row, index)
                            {
                                return '<span>' + (row.LIMITED_NUM + row.THEPEAK_NUM) + '</span>';
                            }
                        },
                        //{
                        //    title: "结果详情", field: "ID", width: 80, formatter: function (value, row, index) {
                        //        return '<span class="resultDetail operatorBtn" data-areacode="' + value + '" onClick=goHistoryDetail(' + value + ')>查看</span>';
                        //    }
                        //},
                         {
                             title: " 错峰详情", field: "LEVEL_ID", width: 80, formatter: function (value, row, index) {

                                 return '<span class="resultDetail operatorBtn" data-areacode="' + value + '" onClick=OpenEdit(' + value + ',' + row.PEAK_LEVEL + ',false,' + index + ')>查看</span>';
                             }
                         },
                         {
                             title: "任务关闭", field: "IS_CLOSE", width: 80, formatter: function (value, row, index) {

                                 return '<span class="resultDetail operatorBtn"  onClick=ClosePeak(' + row.ID + ')>关闭</span>';
                             }
                         }
                    ]],
                    onLoadSuccess: function (data) { com.ListColspan(false, data, "/BaseData/AreaInfo/GetAreaCodeAndText", "areaText"); }

                    
                }   
            };
            var dgList1 =[ {
                id: "#dglevel",
                sortName: "ID",
                sortOrder: "desc",
                rownumbers: true,
                singleSelect: false,
                pagination: true,
                striped: true,
                height: 350,
                width: 710,
                columns: [[
                   { title: "编号", field: "LEVEL_NO", width: 80 },
                   { title: "级别", field: "PEAK_LEVEL", width: 60 },
                   { title: "预警类型", field: "WAR_TYPE_TEXT", width: 60 },
                   { title: "说明", field: "PEAK_DESC", width: 120 },
                   { title: "全部企业数量", field: "TIME_LIMITED_NUM", width:80 },
                   { title: "停产企业数量", field: "STOP_NUM", width: 80 },
                   { title: "限产企业数量", field: "LIMITED_NUM", width: 80 },
                   {
                       title: "操作", field: "ID", width: 100,
                       formatter: function (value, row, index) {
                           // var getRow = row;
                           //debugger;
                           // var josnRow = JSON.stringify(row);
                           return '<span class="resultDetail operatorBtn" data-areacode="' + value + '" onClick=ShowThePeakWindow(' + value + ',' + row.PEAK_LEVEL + ',"' + row.WAR_TYPE + '","true")>下发</span>';

                           //"<span class=\"icon\" title=\"下发\" style=\"cursor:pointer;\" onclick=\"ShowThePeakWindow('" + index + "');\" >下发</span>";//
                       }

                   }
                ]],

                onDblClickRow: function (index, row) {
                    //selectedCustomer(row);
                }

            }];
            //对象2：编辑页面对象
            var edit = {
                title: "错峰任务下发", //标题
                size: { width: 730, height: 430 }, //页面大小
                dgList:dgList1,
                onLoad: function (isAdd, row) { //页面加载事件

                    parent.$('.dialog-button a:eq(0)').hide();
                    // com.ComboxBind(top, "#TYPE", "/Bas/Dic/GetCompanyTypeData", false);

                  //  top.$("#PEAK_ID").val("0");
                    if (isAdd == false && row && row.ID) {
                       // top.$("#PEAK_ID").val(row.ID);
                        //获取子列表数据
                        this.dgList[0].url = "/ThePeakManager/ThePeakMange/GetLevelConfigData?id=" + row.ID;

                    }
                    else {
                        this.dgList[0].url = "/ThePeakManager/ThePeakMange/GetLevelConfigData";

                    }

                    //add 表示新增
                    if (isAdd == true) {
                       
                    }
                        //编辑并绑定变量
                    else {
                        //绑定
                        com.BinEditVal(top, row);
                    }

                },
                SaveAlter: function ()
                {

                },
               
            };

            model.bind(list, edit);


            //其它初始化
            $(window).resize(function () {
                mylayout.resize();
            });
            queryParamInit();
          
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
                top.ShowThePeakWindow(leveID, peakLevel,"",false, selectObj);
                model.edit.openAfter = {};
               // parent.ShowThePeakWindow(leveID, thePeakId, isAdd, index);
            }
            var editShow = model.showEdit(true);
            editShow.dialog("close");
           
        }
        //关闭任务
        var ClosePeak = function (peakId)
        {
            $.messager.confirm('确认', '确定要关闭此错峰任务吗?', function (r) {
                if (r) {
                    debugger;
                    $.ajax({
                        url: "/ThePeakManager/ThePeakMange/ClosePeak?peakId=" + peakId,
                        type: "post",
                        dataType: "json",
                        cache: false,
                        async: false,
                        error: function () { alert("出错"); },
                        success: function (json) {
                            if (json.Success) {
                                alert("关闭成功！");
                                $("#dg").datagrid('reload');
                            }
                            else
                                alert(json.Message);
                        }
                    });
                }
            });
        }

       //var openAfter 
        var queryParamInit = function () {

            com.ComboxBind("PEAK_TYPE","/Bas/Dic/GetDicCodeDataForSearch?typeCode=EmergencyType");
           
            $('#DOWN_TIME_STRAT').datebox({
                required: false,
                editable: false,
            });
            $('#DOWN_TIME_END').datebox({
                required: false,
                editable: false,
            });
            
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

      
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="layout">
        <div region="center" title="错峰生产事件管理" iconcls="icon-clock">
            <!--顶部工具栏-->
            <div id="toolbar">
                <form id="qform">
                    <div style="float: left; padding: 5px;">
                        <span>应急类型：</span>
                        <select id="PEAK_TYPE" name="PEAK_TYPE" style="width:175px;" class="txt03 easyui-validatebox"></select>
                        <span>主题：</span>
                        <input id="PEAK_THEME" name="PEAK_THEME" style="width: 140px;" class="easyui-textbox" >
                        <span>下发时间：</span>
                        <select id="DOWN_TIME_STRAT" name="DOWN_TIME_STRAT" style="width:175px;" class="txt03"></select>
                        <span>至：</span>
                        <select id="DOWN_TIME_END" name="DOWN_TIME_END" style="width:175px;" class="txt03"></select>
                    </div>
                </form>
                <%=Html.ToolBar(Model) %>
            </div>
            <!--查询表单-->
            <div style="margin: 2px;">
            </div>
            <!--数据列表-->
            <table id="dg"></table>
        </div>
    </div>
</asp:Content>

