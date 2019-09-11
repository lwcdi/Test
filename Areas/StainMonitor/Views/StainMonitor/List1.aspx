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
        var list;
        //var edit;
        $(function () {
            mylayout.init();
            model = new listModel();
            var col = [];
            col.push({ title: '状态', align: 'center', rowspan: 3 });
            col.push({ title: '监测点名称', align: 'center', rowspan: 3 });
            col.push({ title: '烟尘', align: 'center', colspan: 3 });
            col.push({ title: '二氧化硫', align: 'center', colspan: 3 });
            col.push({ title: '氮氧化物', align: 'center', colspan: 3 });
            col.push({ title: '流量', align: 'center', rowspan: 1 });
            col.push({ title: '氧含量', align: 'center', rowspan: 1 });
            col.push({ title: '温度', align: 'center', rowspan: 1 });
            col.push({ title: '湿度', align: 'center', rowspan: 1 });
            col.push({ title: '化学需氧量', align: 'center', colspan: 2 });
            col.push({ title: '氨氮', align: 'center', colspan: 2 });
            col.push({ title: '流量', align: 'center', colspan: 1});
            
            var col2 = [];
            col2.push({ title: '浓度', align: 'center', colspan: 1 });
            col2.push({ title: '折算浓度', align: 'center', colspan: 1 });
            col2.push({ title: '排放量', align: 'center', colspan: 1 });
            col2.push({ title: '浓度', align: 'center', colspan: 1 });
            col2.push({ title: '折算浓度', align: 'center', colspan: 1 });
            col2.push({ title: '排放量', align: 'center', colspan: 1 });
            col2.push({ title: '浓度', align: 'center', colspan: 1 });
            col2.push({ title: '折算浓度', align: 'center', colspan: 1 });
            col2.push({ title: '排放量', align: 'center', colspan: 1 });
            col2.push({ title: '立方米', align: 'center', rowspan: 2 });
            col2.push({ title: '百分比', align: 'center', rowspan: 2 });
            col2.push({ title: '摄氏度', align: 'center', rowspan: 2 });
            col2.push({ title: '百分比', align: 'center', rowspan: 2 });
            col2.push({ title: '浓度', align: 'center', colspan: 1 });
            col2.push({ title: '排放量', align: 'center', colspan: 1 });
            col2.push({ title: '浓度', align: 'center', colspan: 1 });
            col2.push({ title: '排放量', align: 'center', colspan: 1 });
            col2.push({ title: '吨', align: 'center', rowspan: 2 });

            var Vicecol = [];

            Vicecol.push({ title: '毫克/立方米', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '毫克/立方米', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '千克', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '毫克/立方米', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '毫克/立方米', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '千克', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '毫克/立方米', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '毫克/立方米', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '千克', align: 'center', colspan: 1, width: 100 });
            //Vicecol.push({ title: '立方米', align: 'center', rowspan: 2, width: 100 });
            //Vicecol.push({ title: '百分比', align: 'center', rowspan: 2, width: 100 });
            //Vicecol.push({ title: '摄氏度', align: 'center', rowspan: 2, width: 100 });
            //Vicecol.push({ title: '百分比', align: 'center', rowspan: 2, width: 100 });
            Vicecol.push({ title: '毫克升', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '千克', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '毫克升', align: 'center', colspan: 1, width: 100 });
            Vicecol.push({ title: '千克', align: 'center', colspan: 1, width: 100 });
         //   Vicecol.push({ title: '吨', align: 'center', rowspan: 2, width: 100 });


            ////复合表头，加载列标题            
            //var title = data.title;
            //var subTitle = data.subTitle;
            //for (var i in title) {
            //    col.push({ title: title[i].title, align: 'center', colspan: title[i].count });
            //}
            //for (var i in subTitle) {
            //    Vicecol.push({
            //        field: (i).toString(), title: subTitle[i], width: 150, align: 'center',
            //        formatter: function (value, row, index) {
            //            if (value == undefined || value == "")
            //                return "-";
            //            else return value;
            //        }
            //    });
            //}

            //$("#qqqq").datagrid({
            //    dataType: "json",
            //    height: 500,
            //    singleSelect: true, //选中一行的设置
            //    rownumbers: true, //行号
            //    striped: true,
            //    columns: [col, Vicecol]
            //});
       

            var list = {
                controller: "/StainMonitor/StainMonitor",//控制器
                dSize: { width: 10, height: 75 },//偏移量
                dg: { //数据表格
                    
                    idField: "ID",
                    sortName: "ID",
                    sortOrder: "asc",
                    columns: [col,col2,Vicecol]

                }   
            };
           
            //[
            //            //{ title: "名称", field: "PEAK_NAME", width: 120 },
            //            { title: "告警编号", field: "CODE", width: 80 },
            //            { title: "告警级别", field: "WARNINGLEVEL", width: 80 },
            //            { title: "告警类型", field: "WARN_TYPE", width: 80 },
            //            { title: "告警对象/站点", field: "POINTCODE", width: 80 },
            //            { title: "告警内容", field: "WARNINGCONTENT", width: 150 },

            //            { title: "告警时间", field: "WARNINGTIME", width: 120, formatter: com.formatter.dateTime },
            //            { title: "持续时长", field: "WARNINGDURATION", width: 120, formatter: com.formatter.dateTime },
            //            { title: "告警状态", field: "STATE", width: 120, formatter: com.formatter.dateTime },
            //            { title: "处理人", field: "HANDLE_USER", width: 120 },
            //            { title: "处理时间", field: "HANDLE_TIME", width: 80 },

            //            {
            //                title: "关闭", field: "STATE", width: 80, formatter: function (value, row, index) {
            //                    return '<span class="resultDetail operatorBtn" data-areacode="' + value + '" onClick=goHistoryDetail(' + value + ')>查看</span>';
            //                }
            //            },
            //             {
            //                 title: "调度", field: " ", width: 80, formatter: function (value, row, index) {

            //                     return '<span class="resultDetail operatorBtn" data-areacode="' + value + '" onClick=OpenEdit(' + value + ',' + row.PEAK_LEVEL + ',false,' + index + ')>查看</span>';
            //                 }
            //             },
            //             {
            //                 title: "详情", field: " ", width: 80, formatter: function (value, row, index) {

            //                     return '<span class="resultDetail operatorBtn"  onClick=ClosePeak(' + row.ID + ')>关闭</span>';
            //                 }
            //             }
            //]

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
        <div region="center" title="实时监控" iconcls="icon-clock">
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
               <%-- <%=Html.ToolBar(Model) %>--%>
            </div>
           <%-- <div style="float: left; width: 280px">
             <%Html.RenderAction("GroupSelect", "UserControl", new { selectId = ViewData["selectId"].ToString() });%>
             </div>
             <div style="float: left;left:290px;margin:5px">
                <a class="easyui-linkbutton" onclick="getSelected()">获取选中</a>
              </div>--%>

            <!--查询表单-->
            <div style="margin: 2px;">
            </div>
            <!--数据列表-->
            <table id="dg"></table>
        </div>
    </div>
</asp:Content>

