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
            $("#PKID").val(id);
            var getClassOn = $(".on").attr("name");
            InitList(getClassOn, id);
            //var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            //var endDate = $("#RECTIME_END").datetimebox('getValue');
            //var param = { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val() };
           // $("#dg").datagrid("reload");
        }
        function PKSelected(id) {
            var getClassOn = $(".on").attr("name");
            $("#PKID").val(id);
            debugger;
          
            
            InitList(getClassOn);
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
            ChangeClick("TotalEmission");
            queryParamInit("TotalEmission");
            InitList("TotalEmission");
           // InitFalg();
           
           

          
            $("#a_search_cust").unbind("click").bind("click", function () {
                debugger;
                var getClassOn = $(".on").attr("name");
                $("#PKID").val(getSelected().ID);
                model = new listModel();
                InitList(getClassOn);
                //var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
                //var endDate = $("#RECTIME_END").datetimebox('getValue');
                //var param = { PKID: $("#PKID").val(), BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val(), Cols: JSON.stringify(col) };
                //$("#dg").datagrid("load", param);
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
        var queryParamInit = function (changeName) {

            //   com.ComboxBind("PEAK_TYPE", "/Bas/Dic/GetDicCodeDataForSearch?typeCode=EmergencyType");
            var currentDate = new Date();
            date2 = currentDate.getFullYear() + "\-" + (currentDate.getMonth() + 1) + "\-" + currentDate.getDate();
            date2 += " " + currentDate.getHours() + ":";
            date2 += currentDate.getMinutes() - 10 + ":";
            date2 += currentDate.getSeconds();

            date = currentDate.getFullYear() + "\-" + (currentDate.getMonth() + 1) + "\-" + currentDate.getDate();
            date += " " + currentDate.getHours() + ":";
            date += currentDate.getMinutes() + ":";
            date += currentDate.getSeconds();

            switch (changeName)
            {
                case "DataTransfer":
                    $('#RECTIME_STRAT').datetimebox({
                        required: false,
                        editable: false,
                    });
                    $('#RECTIME_END').datetimebox({
                        required: false,
                        editable: false,

                    });
                    $("#radioType").show();
                    $("#RECTIME_STRAT").datetimebox('setValue', date2);
                    $("#RECTIME_END").datetimebox('setValue', date);
                    break;
                case "TotalEmission":
                case "ExcessivePollut":
                case "ExceptionStain":
                case "ExceptionPFL":
                case "MNDownLine":
                case "ControlFacilities":
                case "DataAcquisition":
                case "Monitoring":
                case "Governance":
                    $('#RECTIME_STRAT').datebox({
                        required: false,
                        editable: false,
                    });
                    $('#RECTIME_END').datebox({
                        required: false,
                        editable: false,

                    });
                    $("#radioType").hide();
                    $("#RECTIME_STRAT").datebox('setValue', date2);
                    $("#RECTIME_END").datebox('setValue', date);
                    break;
              
            }

                  
            

         
        
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

        function SearchClick(changeName) {
            debugger

            $("#dataTableList").show();
            $("#a_search_cust").show();
            //$("#dgTable").hide();
            $("#seachTools").show();
            $("#dataTable").show();
            $("#baseInfoList").hide();
            //var getCompanyId = getSelected().PARENTID;
            //$("#baseInfo").attr("")
            //$("#baseInfo").attr("src", "/BaseData/VOCsCompany/CompanyBaseInfo?id=" + getCompanyId + "&showType=2")

            var begDate;
            var endDate;
            if (changeName == "DataTransfer") {
                begDate = $("#RECTIME_STRAT").datetimebox('getValue');
                endDate = $("#RECTIME_END").datetimebox('getValue');
            }
            else {
                begDate = $("#RECTIME_STRAT").datebox('getValue');
                endDate = $("#RECTIME_END").datebox('getValue');
                $("input[name='DataType'][value=D]").attr("checked", true);
            }
            ChangeClick(changeName);
            queryParamInit(changeName);
            InitList(changeName);
            //var begDate = $("#RECTIME_STRAT").datetimebox('getValue');
            //var endDate = $("#RECTIME_END").datetimebox('getValue');
           // var param = { PKID: getSelected().ID, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val(), cols: JSON.stringify(col) };
           // $("#dg").datagrid("reload");
        }

        //查看按钮
        function ChangeClick(changeName) {
            debugger
            $(".buttonClick").each( function (i) {
                $(this).hide();
                $(this).attr("class", "buttonClick");
            });
           // InitList("2");
            $("#but" + changeName).show();
            $("#but" + changeName).attr("class", "buttonClick on");
           // debugger;
            //  GetChart();


            switch (changeName) {
                case "DataTransfer":
                    $("#butTotalEmission").show();
                    $("#butTotalEmission").attr("class", "buttonClick");
                    $("#butExcessivePollut").hide();
                    $("#butExceptionStain").hide();
                    $("#butExceptionPFL").hide();
                    $("#butMNDownLine").hide();
                    $("#butControlFacilities").hide();
                    $("#butDataAcquisition").hide();
                    $("#butMonitoring").hide();
                    $("#butGovernance").hide();
                    $("#butExcessivePollut").hide();

                    break;
                case "TotalEmission":
                  
                    $("#butDataTransfer").show();
                    $("#butDataTransfer").attr("class", "buttonClick");
                    $("#butExcessivePollut").hide();
                    $("#butExceptionStain").hide();
                    $("#butExceptionPFL").hide();
                    $("#butMNDownLine").hide();
                    $("#butControlFacilities").hide();
                    $("#butDataAcquisition").hide();
                    $("#butMonitoring").hide();
                    $("#butGovernance").hide();
                    $("#butExcessivePollut").hide();
                    break;
                case "ExcessivePollut":
                    /*隐藏按钮*/
                    $("#butDataTransfer").hide();
                  //  $("#butDataTransfer").attr("class", "buttonClick");
                    $("#TotalEmission").hide();
                    $("#butMonitoring").hide();
                    $("#butGovernance").hide();
                    $("#butDataAcquisition").hide();
                    /*显示按钮*/

                    $("#butExceptionStain").show();
                    $("#butExceptionStain").attr("class", "buttonClick");
                    $("#butExceptionPFL").show();
                    $("#butExceptionPFL").attr("class", "buttonClick");
                    $("#butMNDownLine").show();
                    $("#butMNDownLine").attr("class", "buttonClick");
                    $("#butControlFacilities").show();
                    $("#butControlFacilities").attr("class", "buttonClick");
                   // $("#butExcessivePollut").show();
                    //$("#butExcessivePollut").attr("class", "buttonClick");
                    break;
                case "ExceptionStain":

                   
                    /*隐藏按钮*/
                    $("#butMonitoring").hide();
                    $("#butGovernance").hide();
                   // $("#butExcessivePollut").hide();
                    $("#TotalEmission").hide();
                    $("#butDataTransfer").hide();
                    //  $("#butDataTransfer").attr("class", "buttonClick");
                    /*显示按钮*/
                    $("#butExcessivePollut").show();
                    $("#butExcessivePollut").attr("class", "buttonClick");
                    $("#butExceptionPFL").show();
                    $("#butExceptionPFL").attr("class", "buttonClick");
                    $("#butMNDownLine").show();
                    $("#butMNDownLine").attr("class", "buttonClick");
                    $("#butControlFacilities").show();
                    $("#butControlFacilities").attr("class", "buttonClick");
                    $("#butDataAcquisition").show();
                    $("#butDataAcquisition").attr("class", "buttonClick");
             
               
                    break;

                case "ExceptionPFL":
                    
                    /*隐藏按钮*/
                    $("#butMonitoring").hide();
                    $("#butGovernance").hide();
                   
                    $("#TotalEmission").hide();
                    $("#butDataTransfer").hide();
                    $("#butDataAcquisition").hide();
                    //  $("#butDataTransfer").attr("class", "buttonClick");
                    /*显示按钮*/
                    $("#butExcessivePollut").show();
                    $("#butExcessivePollut").attr("class", "buttonClick");
                    $("#butExceptionStain").show();
                    $("#butExceptionStain").attr("class", "buttonClick");
                    $("#butMNDownLine").show();
                    $("#butMNDownLine").attr("class", "buttonClick");
                    $("#butControlFacilities").show();
                    $("#butControlFacilities").attr("class", "buttonClick");
                   

                    break;
                
                case "MNDownLine":
                  
                    /*隐藏按钮*/
                    $("#butMonitoring").hide();
                    $("#butGovernance").hide();
                    $("#butDataAcquisition").hide();
                    $("#butTotalEmission").hide();
                    $("#butDataTransfer").hide();
                    //  $("#butDataTransfer").attr("class", "buttonClick");
                    /*显示按钮*/
                    $("#butExcessivePollut").show();
                    $("#butExcessivePollut").attr("class", "buttonClick");
                    $("#butExceptionStain").show();
                    $("#butExceptionStain").attr("class", "buttonClick");
                    $("#butExceptionPFL").show();
                    $("#butExceptionPFL").attr("class", "buttonClick");
                    $("#butControlFacilities").show();
                    $("#butControlFacilities").attr("class", "buttonClick");
                  
                    break;
              
                case "ControlFacilities":
                 
                    /*隐藏按钮*/
                    $("#butMonitoring").hide();
                    $("#butGovernance").hide();
                    $("#butDataAcquisition").hide();
                    $("#TotalEmission").hide();
                    $("#butDataTransfer").hide();
                    //  $("#butDataTransfer").attr("class", "buttonClick");
                    /*显示按钮*/
                    $("#butExcessivePollut").show();
                    $("#butExcessivePollut").attr("class", "buttonClick");
                    $("#butExceptionStain").show();
                    $("#butExceptionStain").attr("class", "buttonClick");
                    $("#butExceptionPFL").show();
                    $("#butExceptionPFL").attr("class", "buttonClick");
                    $("#butMNDownLine").show();
                    $("#butMNDownLine").attr("class", "buttonClick");
                  
                    break;
                case "DataAcquisition":
                   
                    /*隐藏按钮*/
                 
                    $("#butExcessivePollut").hide();
                    $("#butTotalEmission").hide();
                    $("#butDataTransfer").hide();
                    $("#butExceptionStain").hide();
                    $("#butExceptionPFL").hide();
                    $("#butMNDownLine").hide();
                    $("#butControlFacilities").hide();
                    //  $("#butDataTransfer").attr("class", "buttonClick");
                    /*显示按钮*/
                   
                    $("#butGovernance").show();
                    $("#butGovernance").attr("class", "buttonClick");
                    $("#butMonitoring").show();
                    $("#butMonitoring").attr("class", "buttonClick");
                    break;
                case "Monitoring":
                   
                    /*隐藏按钮*/
                    $("#butExcessivePollut").hide();
                    $("#butTotalEmission").hide();
                    $("#butDataTransfer").hide();
                    $("#butExceptionStain").hide();
                    $("#butExceptionPFL").hide();
                    $("#butMNDownLine").hide();
                    $("#butControlFacilities").hide();
                    //  $("#butDataTransfer").attr("class", "buttonClick");
                    /*显示按钮*/
                    $("#butGovernance").show();
                    $("#butGovernance").attr("class", "buttonClick");
                    $("#butDataAcquisition").show();
                    $("#butDataAcquisition").attr("class", "buttonClick");
                    break;

                case "Governance":

                    /*隐藏按钮*/
                    $("#butExcessivePollut").hide();
                    $("#butTotalEmission").hide();
                    $("#butDataTransfer").hide();
                    $("#butExceptionStain").hide();
                    $("#butExceptionPFL").hide();
                    $("#butMNDownLine").hide();
                    $("#butControlFacilities").hide();
                    //  $("#butDataTransfer").attr("class", "buttonClick");
                    /*显示按钮*/
                    $("#butMonitoring").show();
                    $("#butMonitoring").attr("class", "buttonClick");
                    $("#butDataAcquisition").show();
                    $("#butDataAcquisition").attr("class", "buttonClick");
                    break;
            }
        
        }

       
        var InitList = function (changeName,companyId)
        {
          
           // try {

                switch (changeName) {
                    case "DataTransfer":
                        col = listMoBan.Get.DataTransfer();
                        break;
                    case "TotalEmission":
                        col = listMoBan.Get.TotalEmission();
                        break;
                    case "ExcessivePollut":
                        col = listMoBan.Get.ExcessivePollut();
                        break;
                    case "ExceptionStain":
                        col = listMoBan.Get.ExceptionStain();
                        break;
                    case "ExceptionPFL":
                        col = listMoBan.Get.ExceptionPFL();
                        break;
                    case "MNDownLine":
                        col = listMoBan.Get.MNDownLine();
                        break;
                    case "ControlFacilities":
                        col = listMoBan.Get.ControlFacilities();
                        break;
                    case "DataAcquisition":
                        col = listMoBan.Get.DataAcquisition();
                        break;
                    case "Monitoring":
                        col = listMoBan.Get.Monitoring();
                        break;
                    case "Governance":
                        col = listMoBan.Get.Governance();
                        break;

                }
                // col2 = [];
                var begDate;
                var endDate;
                var getSelectId = "-1";
                var getCompanyId = "-1";
                if (typeof (companyId) != "undefined")
                    getCompanyId = companyId;

                if (getSelected())
                 getSelectId = getSelected().ID;
                if (changeName == "DataTransfer") {
                     begDate = $("#RECTIME_STRAT").datetimebox('getValue');
                     endDate = $("#RECTIME_END").datetimebox('getValue');
                }
                else {
                    begDate = $("#RECTIME_STRAT").datebox('getValue');
                    endDate = $("#RECTIME_END").datebox('getValue');
                    $("input[name='DataType'][value=D]").attr("checked", true);
                }
                      list = {
                                controller: "/StainMonitor/StainCase/",//控制器
                                dSize: { width: 295, height: 140 },//偏移量
                                dg: { //数据表格
                                        idField: "ID",
                                        sortName: "ID",
                                        sortOrder: "asc",
                                        columns: [col], //, Vicecol
                                        queryParams: { PKID: getSelectId, CompanyID: getCompanyId, BeginTime: begDate, EndTime: endDate, DataType: $("input[name='DataType']:checked").val(), DataBusiness: changeName, Cols: JSON.stringify(col) },
                                    }
                      };
                     //对象2：编辑页面对象
                      //edit = {
                      //    title: "监测档案", //标题
                      //    size: { width: 730, height: 430 }, //页面大小
                      //    //dgList:dgList1,
                      //    onLoad: function (isAdd, row) { //页面加载事件
                      //        //add 表示新增
                      //        if (isAdd == true) { 
                      //        }
                      //            //编辑并绑定变量
                      //        else {
                      //            //绑定
                      //           // com.BinEditVal(top, row);
                      //        }
                      //    },
                      //    SaveAlter: function () {
                      //    }

                      //};
                      model.bind(list, edit);
                      //$("#dg").datagrid("load", list.dg.queryParams);
              // }
                        
                        

                
               // catch (e)
              //  { }//alert("当前排口/企业没有数据！");
           
        }

        var GetBaseInfo = function ()
        {
            debugger;
            $("#dataTableList").hide();
            $("#a_search_cust").hide();
            //$("#dgTable").hide();
            $("#seachTools").hide();
            $("#dataTable").hide();
            $("#baseInfoList").show();
            var getCompanyId = getSelected().PARENTID;
            //$("#baseInfo").attr("")
            $("#baseInfo").attr("src", "/BaseData/VOCsCompany/CompanyBaseInfo?id=" + getCompanyId + "&showType=2")

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
              //  Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model =ViewBag.controllerParam });
                  Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model =ViewBag.controllerParam});

            %>
        </div>
        <div style="float: left; left: 290px; margin: 5px">
            <a class="easyui-linkbutton" onclick="getSelected()">获取选中</a>
        </div>

        <div region="center" title="监测档案" iconcls="icon-clock">

         
            <!--顶部工具栏-->
            <div id="toolbar">
                <form id="qform">
                  <div style="margin-bottom:10px">
	              <a href="#" class="easyui-linkbutton" onclick="SearchClick('TotalEmission')">监测信息</a>
	             <a href="#" class="easyui-linkbutton" onclick="SearchClick('ExcessivePollut')">异常告警</a>
	              <a href="#" class="easyui-linkbutton" onclick="SearchClick('DataAcquisition')">设备运行状态</a>
                       <a href="#" class="easyui-linkbutton" onclick="GetBaseInfo()">污染源基本</a>
                  </div>
                    <div id="seachTools" style="float: left; padding: 5px;">
<%--
                        <span id="monitorInfo" >
                              <%=Html.LinkButton("a_monitorInfo","icon-text_align_right","监测信息") %>  
                              <%=Html.LinkButton("a_exceptionWarn","icon-text_align_right","异常告警") %>
                              <%=Html.LinkButton("a_dicverState","icon-text_align_right","设备运行状态") %>
                              <%=Html.LinkButton("a_StainBase","icon-text_align_right","污染源基本") %>
                        </span>--%>

                        
                        <span id="radioType">
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
                        </span>
                        <span>时间：</span>
                        <select id="RECTIME_STRAT" name="BeginTime" style="width: 175px;" class="txt03"></select>
                        <span>至：</span>
                        <select id="RECTIME_END" name="EndTime" style="width: 175px;" class="txt03"></select>
                    </div>
                       <input type="hidden" id="PKID" name="PKID" value="0" />

                         <%=Html.LinkButton("a_search_cust","icon-folder_magnify","查询") %>  <%--icon-text_align_right--%>
                         <%=Html.ToolBar(Model) %> 

                </form>
             
                
               
                
            </div>

            <!--查询表单-->
            <div style="margin: 2px; overflow: hidden; clear: both;">
          
                  
            <!--数据列表-->
            
            <div  id="dataTableList">
                <div id="dataTable" style=" padding: 5px;">
                <span id="buttonDiv" class="tab-min">
                 <span id="butTotalEmission" name="TotalEmission"  class="buttonClick"  onclick='SearchClick($(this).attr("name"))' class="on">排放总量提示</span>
                 <span id="butDataTransfer" name="DataTransfer" class="buttonClick"  onclick='SearchClick($(this).attr("name"))'>数据传输情况</span>
                 <span id="butExcessivePollut" name="ExcessivePollut" class="buttonClick" onclick='SearchClick($(this).attr("name"))' >污源物超标</span>
                 <span id="butExceptionStain" name="ExceptionStain" class="buttonClick"  onclick='SearchClick($(this).attr("name"))'>污染特异常</span>
                 <span id="butExceptionPFL" name="ExceptionPFL" class="buttonClick"  onclick='SearchClick($(this).attr("name"))'>排放流量异常</span>
                 <span id="butMNDownLine" name="MNDownLine" class="buttonClick"  onclick='SearchClick($(this).attr("name"))'>数采仪掉线</span>
                 <span id="butControlFacilities" name="ControlFacilities" class="buttonClick"  onclick='SearchClick($(this).attr("name"))'>治理设施停动</span>
                 <span id="butDataAcquisition" name="DataAcquisition" class="buttonClick"  onclick='SearchClick($(this).attr("name"))'>数采仪</span>
                 <span id="butMonitoring" name="Monitoring" class="buttonClick" onclick='SearchClick($(this).attr("name"))'>监测仪器</span>
                 <span id="butGovernance" name="Governance" class="buttonClick"  onclick='SearchClick($(this).attr("name"))'>治理设施</span>
                </span>
              
                   <%--<div id="dataFlag" style="position: relative; left: 10px"></div> float: left;--%>
            </div >
               <table id="dg"></table>
            
            </div>

            <div id="baseInfoList" style="display: none;width:95%;height:93%">
               <iframe id="baseInfo"  style= "width:1000px;height:700px"></iframe>

            </div>
        </div>
       
    </div>
        </div>
</asp:Content>
