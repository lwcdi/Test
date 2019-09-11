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
        var edit = {};
        var selectType = 1;
        var col = [];
        var col2 = [];
        var Vicecol = [];
        var Gcompanyid = "-1";


        function GetQueryString(name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        }

        // 调用方法
       // alert(GetQueryString("参数名1"));
        //alert(GetQueryString("参数名2"));
      //  alert(GetQueryString("参数名3"));

        function getSelected() {
            var selObj = GetTreeSelected();
            // alert(selObj.TYPE + "," + selObj.ID);
            return selObj;
        }

        function CompanySelected(id) {

            debugger;
            Gcompanyid = id;
            var getClassOn = $(".on").attr("name");
            $("#dataTableList").show();
            $("#picData").hide();
            $("#butData").attr("class", "on");
            $("#butPic").attr("class", "");
            InitList(id);
            //var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            //var endDate = $("#RECTIME_END").datetimebox('getValue');
            //var param = { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() };
           // $("#dg").datagrid("reload");
        }
        function PKSelected(id) {
            var getClassOn = $(".on").attr("name");
            $("#dataTableList").show();
            $("#picData").hide();
            $("#butData").attr("class", "on");
            $("#butPic").attr("class", "");
            $("#PKID").val(id);
            debugger;
          
            
            InitList();
            //var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            //var endDate = $("#RECTIME_END").datetimebox('getValue');
            //var param = { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() };
            //$("#dg").datagrid("reload");
        }

        //查看数据

        //var edit;
        $(function () {
            mylayout.init();
            debugger;
            model = new listModel();
            //PKSelected().bind("click", function () { InitList(this); })
            //var list;
         //   ChangeClick("TotalEmission");
            queryParamInit();
            InitList();
           // InitFalg();
           
          //  onLineStation();

          
            $("#a_search_cust").unbind("click").bind("click", function () {
                debugger;
                model = new listModel();
                InitList();
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

       
        //var openAfter 
        var queryParamInit = function (changeName) {

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
            date2 += currentDate.getMinutes() - 10 + ":";
            date2 += currentDate.getSeconds();

            date = currentDate.getFullYear() + "\-" + (currentDate.getMonth() + 1) + "\-" + currentDate.getDate();
            date += " " + currentDate.getHours() + ":";
            date += currentDate.getMinutes() + ":";
            date += currentDate.getSeconds();
            $("#RECTIME_STRAT").datetimebox('setValue', date2);
            $("#RECTIME_END").datetimebox('setValue', date);
        }
     

        
        var onLineStation = function ()
        {
            $.ajax({
                type: "POST",
                //   contentType: "application/json",
                dataType: "json",
                url: "/StainMonitor/WebState/GetOnlineStation",
                cache: false,
                async: false, //同步执行
                //data: { PKID: getSelectId, CompanyID: getCompanyId, Key: com.helper.guid() },
                success: function (result) {
                    if (result.Success == true) {
                        $("#TotalNum").text( result.TotalNum);
                        $("#OnlineTotal").text(result.OnlineTotal);
                        $("#DownlineTotal").text(result.DownlineTotal);
                        //OnlineTotal  
                    }
                    else {
                        com.msg.error("失败！");
                    }
                }

            });
        }

       
        var InitList = function (companyId)
        {
            col = listMoBan.Get.DataTransferState();
            var getSelectId = "-1";
            var getCompanyId = "-1";
            if (typeof (companyId) != "undefined")
                getCompanyId = companyId;
            if (getSelected())
                getSelectId = getSelected().ID;
            var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            var endDate = $("#RECTIME_END").datetimebox('getValue');
            try {

                            list = {
                                controller: "/StainMonitor/DataTransferState/",//控制器
                                dSize: { width: 295, height: 140 },//偏移量
                                dg: { //数据表格
                                    idField: "ID",
                                    sortName: "ID",
                                    sortOrder: "asc",
                                    columns: [col], //, Vicecol
                                    queryParams: { PKID: getSelectId, CompanyID: getCompanyId, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val(), Cols: JSON.stringify(col) }

                                    //var param = { PKID: getSelected().ID,  };
                                }
                            };

                            model.bind(list, edit);
                    }

                catch (e)
                {
                    alert(e);

                }//alert("当前排口/企业没有数据！");
           
        }


        //查看图表
        var PicClick = function () {
            debugger
            // InitList("2");
            $("#picData").show();
            $("#dataTableList").hide();
            $("#butData").attr("class", "");
            $("#butPic").attr("class", "on");
            debugger;
            GetChart(Gcompanyid);
        }
        var DataTableClick= function () {
            debugger
            // InitList("2");
            $("#dataTableList").show();
            $("#picData").hide();
            $("#butData").attr("class", "on");
            $("#butPic").attr("class", "");
            InitList(Gcompanyid);
            //var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            //var endDate = $("#RECTIME_END").datetimebox('getValue');
            //var param = { PKID: getSelected().ID,  BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() };
           // $("#dg").datagrid("load", param);
        }
       
        var GetChart = function () {
            var getSelectId = "-1";
            var getCompanyId = "-1";
            if (typeof (Gcompanyid) != "undefined" || Gcompanyid!="-1")
                getCompanyId = Gcompanyid;
             if (getSelected())
                getSelectId = getSelected().ID;
            var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            var endDate = $("#RECTIME_END").datetimebox('getValue');
            // var param = { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() };
            $.ajax({
                type: "post",
                url: "/StainMonitor/DataTransferState/GetPicData",
                data:{ PKID: getSelectId, CompanyID: getCompanyId, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val(), Cols: JSON.stringify(col) },
                dataType: "json", //返回数据形式为json
                success: function (result) {
                    var div = document.getElementById('picData');
                    var myChart = echarts.init(div)
                    myChart.setOption(eval('(' + result + ')'));
                    //Gcompanyid = "-1";
                }
            });
             
        }

    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
    <div id="layout">
        <div region="west" split="true" style="width: 280px; padding: 5px">
            <%
                  Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model =ViewBag.controllerParam});
            %>
        </div>
        <div style="float: left; left: 290px; margin: 5px">
            <a class="easyui-linkbutton" onclick="getSelected()">获取选中</a>
        </div>

        <div region="center" title="传输状态" iconcls="icon-clock">

         
            <!--顶部工具栏-->
            <div id="toolbar">
                <form id="qform">
                  
                    <div id="seachTools" style="float: left; padding: 5px;">
<%--
                        <span id="monitorInfo" >
                              <%=Html.LinkButton("a_monitorInfo","icon-text_align_right","监测信息") %>  
                              <%=Html.LinkButton("a_exceptionWarn","icon-text_align_right","异常告警") %>
                              <%=Html.LinkButton("a_dicverState","icon-text_align_right","设备运行状态") %>
                              <%=Html.LinkButton("a_StainBase","icon-text_align_right","污染源基本") %>
                        </span>--%>  
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
                       <input type="hidden" id="PKID" name="PKID" value="0" />


                </form>
             
                
               
                
            </div>

            <!--查询表单-->
            <div style="margin: 2px; overflow: hidden; clear: both;">
          
                  
            <!--数据列表-->
            
            <div  id="dataTableList">
               <table id="dg"></table>
            </div>
         
         </div>
          <div id="picData" style="display: none;width:95%;height:93%">
            
            </div>
    </div>
        </div>
</asp:Content>
