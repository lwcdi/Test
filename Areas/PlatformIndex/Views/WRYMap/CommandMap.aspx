<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="Content/js/map/MapStyle.js"></script>
    <script src="Content/js/map/EmergencyMap.js"></script>
    <script type="text/javascript" src="http://api.map.baidu.com/api?v=2.0&ak=z1o84xIiM9gFmseTul5gscDN"></script>
    <style type="text/css">
        html {
            height: 100%;
        }

        body {
            height: 100%;
            margin: 0px;
            padding: 0px;
            font-family:"\5FAE\8F6F\96C5\9ED1", Helvetica;
            font-size:12px;
        }
        html,body,p,h1,h2,h3,h4,h5,h6,form,input,textarea,select,button,fieldset,legend,img,ul,ol,li,dl,dt,dd,th,td,pre,blockquote{margin:0;padding:0}
        #controller {
          
            z-index: 999;
            position:absolute;
            left:15px;
            top:20px;
          
        }
        .search-title{
            float:left; display:block; font-size:14px;color:white;   height: 40px;   line-height: 40px;   width: 80px; 
            background:url(/VOCS/image/map/search-title.png) #4cb7db  no-repeat right center;
            text-align:left;
            text-indent:2px;
         
        }
        .search-btn{
            float:left;  cursor:pointer;color:white;width:60px;height:40px; line-height: 40px; font-size:14px;text-align:center;

             background:url(/VOCS/image/map/search-btn.png) #006698  no-repeat center center;
        }
        #searchTask {
            float:left;
            font-size:14px;
        }

            #searchTask ul {
                margin: 1px 0 0 30px;
                padding: 0;
                list-style-type: none;
            }

            #searchTask .selectTaskID {
                float: left;
                color: #8b8b8b;
                width: 280px;
                height: 40px;
                cursor: pointer;      
                line-height: 40px;
                background: #fff;
                display: block;

            }

            #searchTask .subTask {
                position: absolute;
                top: 38px;
                left: 44px;
                color: #069;   
                background: #eef7f9;     
                width:612px;
                display: none;
                box-shadow:0 0 1px #999;
            }
              .search-from{ float:left; overflow:hidden; width:140px; height:22px; padding:3px 5px; border:1px solid #fff;   cursor: pointer }
              .search-from:hover{ background-color:#9ed1ee; }

                #searchTask .subTask li {

                    padding-left: 2px;

                }



        #container {
            height: 90%;
        }

        #buttonDiv {
            /*width: 400px;*/
            height: 31px;
            /*border: 1px solid black;*/
            background: rgba(255,255,255,.9);
            position: absolute;
            left: 0px;
            z-index: 10;
            border: 1px solid #ccc;
            bottom:10px;
            left:50%;
            margin-left:-240px;
            z-index:999;
            box-shadow:rgba(153,153,153,.6) 0 0 1px ;
            border-radius:5px;
        }

            #buttonDiv div,#companyTypeIcon div {
                float: left;
                height: 31px;
                padding: 0px 5px 0px 25px;
                font-size: 13px;
                cursor: pointer;
                /*border: 1px solid black;*/
            }

                #buttonDiv div span,#companyTypeIcon div span {
                    display:inline-block;
                    padding-top:8px;
                }

        #companyTypeIcon {
            height: 31px;
            background: rgba(255,255,255,.9);
            position: absolute;
            top: 5px;
            right:200px;
            z-index: 10;
            border: 1px solid #ccc;
            bottom: 10px;
            z-index: 999;
            box-shadow: rgba(153,153,153,.6) 0 0 1px;
            border-radius: 5px;
        }

        #buttonDivUnReport {
            margin-left:6px;
            background:url(/VOCs/image/map/company-unreport.png) no-repeat left center;
            background-size:22px 22px;
        }
        #buttonDivReport {
            background:url(/VOCs/image/map/company-report.png) no-repeat left center;
            background-size:22px 22px;
        }
        #buttonDivRed {
            background:url(/Content/img/map/person-red.png) no-repeat left center;
        }
        #buttonDivGray {
            background:url(/Content/img/map/person-gray.png) no-repeat left center;
        }
        #buttonDivFLD {
            background: url(/Content/img/map/camera2.png) no-repeat left center;
            background-color: #D4D4D4;
        }
        #buttonDivCFCompany {
            background:url(/Content/img/company.png) no-repeat left center;
            background-color: #D4D4D4;
        }
        #buttonDivYCGD {
            background:url(/Content/img/site.png) no-repeat left center;
            background-color: #D4D4D4;
        }


        #thediv {
            width: 360px;
            background: #fff;
            position: absolute;
            left:15px;
            top:75px;
            z-index:99;
            box-shadow: 0 0 3px #999;
            padding-bottom:10px;
           
        }

       #thedivSpan {
            width: 20px;
            height: 90px;
            line-height: 23px;
            background: #b5e3f2;
            position: absolute;
            right: -20px;
            top: 70px;
            cursor:pointer;
        }

        .count {

            color: #ccedf6;
            background-color: #1ABC9C;
            /*font-size:16px;*/
            /*text-align:center;*/
            height: 35px;
        }

        #CountryTownSelectDiv 
        {
            width:100%;
            font-size: 24px;
            color:width;
            position: absolute;
            text-align:center;
            z-index: 999;
        }
       .area{
          padding:6px 20px 6px 10px;
           border:1px solid  #007493;
           border-top:none;
           color:#069;


       }
       .area:hover{
           background-color:#88daf0;
       }
      #CountryTownSelectDiv div:first-child {   
          border-top: 1px solid  #007493;
           color:#fff;
           background-image:url(/Content/img/map/arrows-bottom.png);
           background-position:center right;
           background-repeat:no-repeat;

      }
      .title{ text-align:left; padding:8px 0;  position:relative; border-top:1px solid #069; margin: 50px 10px 0 10px;}
      
      .title strong{
          font-size:14px;
          color:#069;
          text-indent:10px;
          display:inline-block;
      }
        .title a {
            position:absolute;
            right:10px;
            top:8px;   
            color:#666;         
        }
          .title a:hover{
              color:#f30;
          }
          .table-title-head{
              background-color:#0091b8;
              /*height:45px;*/
              margin-top:10px;
              text-align:center;
              padding:5px 3px;
              font-size:16px;
          }
          .table-title{
          
            border-collapse: collapse;
            background-color:#eef7f9;
            width:100%;
            border-top:1px solid #069;
          }
           .table-title th{
               text-align:right;
               width:40%;
               padding:6px ;
         border-bottom: 1px solid #fff;
         border-collapse:collapse;
           }
           .table-title td{
               text-align:left;
                padding:6px  0;
          border-bottom: 1px solid #fff;
           border-collapse:collapse;
           }
           .table-title td strong{
               color:#f30;
               font-size:13px;
               padding:0 5px;
           }
           .tab-head{
               position:relative;
               border-top:1px solid #ccc;
               overflow:hidden;
               height:36px;
               line-height:36px;
               background-color:#eef7f9;
               margin:10px 10px 0 10px

           }
           .tab-head p{ position: absolute; height:4px; background-color:#069; margin:0;  padding:0;   width:50%; left:0; top:-1px; z-index:99; }
           .tab-head span{
               display:inline-block;
               width:50%; 
               float:left;     
               text-align:center;
               font-size:14px;
               cursor:pointer;
              
           }
           .tab-head span.on{
               color:#069;
           }
           .tab-body{
               margin:0 10px;
            background-color:#eef7f9;

           }
           .tab-main{
               display:none;
           }
           .tab-min{ padding:3px 10px 0px 10px}
           .tab-min span{
              display:inline-block;
             height:26px;
             line-height:26px;
             width:80px; 
             text-align:center;
             overflow:hidden;
             background-color:white;
             border: 1px solid #ccc;
             border-radius:20px;
              cursor:pointer;
           }
        .tab-min span.on {
            background-color:#00a5d1;
            color:white;
            border: 1px solid #00a5d1;
        }
        .tabfrom{
            text-align:center;
            border: 1px solid #D4D4D4;
            border-collapse: collapse;
             width:100%
        }
        .tabfrom td{
          border: 1px solid #a7d5ef;
          padding:3px ;
        }
        .more{
            position:absolute;
            top:65px;
            right:20px;
             cursor:pointer;
             color:#fff;
        }
         .more:hover{
             color:#66ccff;
             text-decoration:none;
        }
    </style>
    <script type="text/javascript">
        var showType = "";  //展示类型 默认为web, 可传app
        var currentTaskID = "";  //当前正在显示的任务ID
        var currentTaskTItle = "";  //当前正在显示的任务主题
        var currentTaskBeginTime;  //当前正在显示的任务开始时间
        var currentTaskEndTime;  //当前正在显示的任务结束时间
        var map;   //百度地图对象
        var companyPoints;  //企业点位信息
        function init() {
            debugger;
            isShow = getQueryStr("isShow");
            if (isShow == "show")
                $("#Showback").show();

            $("#container").height($(window).outerHeight());
            $(window).resize(function () {
                $("#container").height($(window).outerHeight());
            });
            map = new BMap.Map("container");

            showTaskSelector();  //加载当前任务下拉框 -- 页面首次加载时调用

            //初始化地图,选取人员信息第一个点为起始点
            var point = new BMap.Point(114.271093, 34.976331);
            if (companyPoints && companyPoints.length > 0) {
                point = new BMap.Point(companyPoints[0].lng, companyPoints[0].lat);
            }
            map.centerAndZoom(point, 12);
            map.enableScrollWheelZoom();
            SetMapStyle();
        }

        //加载当前任务下拉框 -- 页面首次加载时调用
        function showTaskSelector() {
            currentTaskID = getQueryStr("taskCode");
            showType = getQueryStr("showType");

            //加载任务信息表格数据
//            var taskInfoList = GetEmergencyInfoList();
            var taskInfoList;
            if (taskInfoList == null || taskInfoList.length == 0) {
                alert("当前没有核算期！");
                return false;
            }
            else {
                currentTaskID = taskInfoList[0].taskID; //设置任务ID
                currentTaskTitle = taskInfoList[0].ZXMC; //设置任务主题
                currentTaskBeginTime = taskInfoList[0].beginTime;  //设置任务开始时间
                currentTaskEndTime = taskInfoList[0].endTime;  //设置任务结束时间
                $("#TaskTitle").text(currentTaskTitle); //设置任务标题
                initPageDatas(); //加载页面所需数据
                initTaskInfoDatas(); //根据任务ID初始化页面
            }

            $(".subTask").empty(); //清空table
            for (var i = 0; i < taskInfoList.length; i++) {
                var zxmc = taskInfoList[i].ZXMC;
                var subZXMC = zxmc;
                if (zxmc.length > 10) {
                    subZXMC = zxmc.substring(0, 10) + "...";
                }
                var trHtml = taskInfoListRow.replace("taskInfoID", taskInfoList[i].taskID).replace("taskInfoTitle", zxmc).replace("taskInfoTitleSub", subZXMC)
                            .replace("taskInfoBeginTime", taskInfoList[i].beginTime).replace("taskInfoEndTime", taskInfoList[i].endTime);
                $(".subTask").append(trHtml);
            }

            var oSelect = document.getElementsByTagName("span")[1];
            var oSub = document.getElementsByTagName("ul")[0];
            var aLi = oSub.getElementsByTagName("li");
            var i = 0;
            oSelect.onclick = function (event) {
                var style = oSub.style;
                style.display = style.display == "block" ? "none" : "block";
                //阻止事件冒泡
                (event || window.event).cancelBubble = true
            };
            for (i = 0; i < aLi.length; i++) {
                //鼠标划过
                aLi[i].onmouseover = function () {
                    this.className = "hover"
                };
                //鼠标离开
                aLi[i].onmouseout = function () {
                    this.className = "";
                };
                //鼠标点击
                aLi[i].onclick = function () {
                    oSelect.innerHTML = $(this).children(".taskID").text();
                    currentTaskID = $(this).children(".taskID").text();  //切换当前任务ID
                    currentTaskTitle = $(this).children(".taskZXMC").text(); //切换当前任务主题
                    currentTaskBeginTime = $(this).children(".taskBeginTime").text();  //切换当前任务开始时间
                    currentTaskEndTime = $(this).children(".taskEndTime").text();  //切换当前任务结束时间
                }
            }
            document.onclick = function () {
                oSub.style.display = "none";
            };

            oSelect.innerHTML = currentTaskID;  //首次加载页面时显示最新的任务
        };

        //查看区域记录
        function areaCompanyTableClick() {
            $("#AreaCompanyTableDiv").show();
            $("#IndustryTableDiv").hide();
//            $("#CountrySignTableDiv").hide();
        }

        //查看行业记录
        function industryTableClick() {
            $("#IndustryTableDiv").show();
            $("#AreaCompanyTableDiv").hide();
//            $("#CountrySignTableDiv").hide();
        }

        window.onload = init;  //页面初始化事件 -- 页面首次加载时调用
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="controller" align="center">
        <div id="searchTask">
            <div class="boxTask">
                <span class="search-title">当前核算期</span>
                <span class="selectTaskID">请选择核算期</span>
                <div class='search-btn' onclick="changeTaskClick()"></div>
                <div id="Showback" onclick="goback();" style='display: none; float: left; background-color: red; cursor: pointer; color: white; width: 100px; height: 40px; line-height: 40px; font-size: 14px; text-align: center;'>返回任务列表</div>

            </div>
            <ul class="subTask">
                <%-- <li><div class="taskID" style="padding-right:5px;border-right:1px solid white;float:left;height:30px;">A201709201120</div><div style='padding-left:10px;float:left;height:30px;' title="2017年09月08日重点区域监管重点区域监管重点区域监管重点区域监管">2017</div></li>
                <li><div class="taskID" style="padding-right:5px;border-right:1px solid white;float:left;height:30px;">B201709201235</div><div style='padding-left:10px;float:left;height:30px;' title="2017年09月08日重点区域监管重点区域监管重点区域监管重点区域监管">2017</div></li>
                <li><div class="taskID" style="padding-right:5px;border-right:1px solid white;float:left;height:30px;">C201709201567</div><div style='padding-left:10px;float:left;height:30px;' title="2017年09月08日重点区域监管重点区域监管重点区域监管重点区域监管">2017</div></li>
                <li><div class="taskID" style="padding-right:5px;border-right:1px solid white;float:left;height:30px;">D201709201565</div><div style='padding-left:10px;;float:left;height:30px;' title="2017年09月08日重点区域监管重点区域监管重点区域监管重点区域监管">2017</div></li>
                <li><div class="taskID" style="padding-right:5px;border-right:1px solid white;float:left;height:30px;">E201709201897</div><div style='padding-left:10px;float:left;height:30px;' title="2017年09月08日重点区域监管重点区域监管重点区域监管重点区域监管">2017</div></li>--%>
            </ul>
        </div>

    </div>

    <div id="container"></div>
    <div id="companyTypeIcon">
        <div id="buttonDivUnReport" onclick="showUnReportClick()">
            <span>未上报(0)</span><input id="buttonDivUnReportInput" type="checkbox" style="visibility:hidden;" checked="checked"/>
        </div>
        <div id="buttonDivReport" onclick="showReportClick()">
            <span>已上报(0)</span><input id="buttonDivReportInput" type="checkbox" style="visibility:hidden;" checked="checked" />
        </div>
    </div>
    <!--left-->
    <div id="thediv" >
        <div style="width: 100%; height: 100%; background-color: white;">
            <%-- 任务倒计时 --%>
            <div id="timeCountDiv" class="count">
                <%-- 区域选择 --%>
                <div id="CountryTownSelectDiv" >郑州市</div>
            </div>

            
            <a  class="more" style="display:none;">查看任务>></a>
            <div style="padding:0 10px 0 10px;">
                <div  class='table-title-head'><span id="TaskTitle" style="color:white;">2018年1-12月VOC总量核算</span></div>
            </div>

            <div class="tab-body">
            <div class="tab-main" style="display:block;">
                <div id="AreaCompanyButtonDiv" class="tab-min" style="border:1px solid black;height:50px;" >
                
                <div style="border:1px solid black;float:right;width:55%;padding-top:10px;">
                    <span onclick='areaCompanyTableClick()' class="on">区域</span>
                    <span onclick='industryTableClick()'>行业</span>
                </div>
                <div style="border:1px solid black;width:45%;">
                    <div style="margin-top:3px;">企业数量：100家</div>
                    <div style="margin-top:5px;">未上报企业：10家</div>
                </div>
                </div>
                <div id="AreaCompanyTableDiv" style="height:200px;overflow:auto;">
                    <div>
                        <table class="tabfrom">
                            <thead>
                                <tr style="background-color:#B4E0F9">
                                    <td style="width:39%;">区域</td>
                                    <td style="width:30%;">企业数量</td>
                                    <td style="width:30%;">未上报企业</td>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <div >
                        <table id="AreaCompanyDataTable"  class="tabfrom">
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div id="IndustryTableDiv" style="display:none;height:200px;overflow:auto;">
                    <div>
                        <table  class="tabfrom">
                            <thead>
                                <tr style="background-color:#B4E0F9">
                                    <td style="width:39%;">行业</td>
                                    <td style="width:30%;">企业数量</td>
                                    <td style="width:30%;">未上报企业</td>
                                </tr>
                            </thead>
                        </table>
                        <!--<table style='text-align:center;font-size:13px;border: 1px solid #D4D4D4;border-collapse: collapse;'>
                            <thead>
                                <tr style="background-color:#B4E0F9">
                                    <td style="width:30px;">序号</td>
                                    <td style="width:68px;">人员</td>
                                    <td style="width:75px;">签到时间</td>
                                    <td style="width:75px;">签退时间</td>
                                </tr>
                            </thead>
                        </table>-->
                    </div>
                    <div >
                        <table id="IndustryDataTable"  class="tabfrom">
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
                <div id="CountrySignTableDiv" style="display:none; height:200px;overflow:auto">
                    <div>
                        <table s class="tabfrom">
                            <thead>
                                <tr style="background-color:#B4E0F9">
                                    <td style="width:10%;">序号</td>
                                    <td style="width:30%">人员</td>
                                    <td style="width:20%;">签到时间</td>
                                    <td style="width:20%;">签退时间</td>
                                </tr>
                            </thead>
                        </table>
                    </div>
                    <div >
                        <table id="CountrySignDataTable"  class="tabfrom">
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
            </div>
            </div>
        </div>
    </div>
</asp:Content>
