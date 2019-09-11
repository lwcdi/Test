<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Content/js/map/MapStyle.js"></script>
    <script src="/Content/js/map/EmergencyMap.js"></script>
    <script src="/Content/js/getQueryStr.js"></script>
    <script type="text/javascript" src="http://api.map.baidu.com/api?v=2.0&ak=z1o84xIiM9gFmseTul5gscDN"></script>
    <style type="text/css">
        html {
            height: 100%;
        }

        body {
            height: 100%;
            margin: 0px;
            padding: 0px;
            font-family: "\5FAE\8F6F\96C5\9ED1", Helvetica;
            font-size: 12px;
        }

        html, body, p, h1, h2, h3, h4, h5, h6, form, input, textarea, select, button, fieldset, legend, img, ul, ol, li, dl, dt, dd, th, td, pre, blockquote {
            margin: 0;
            padding: 0;
        }

        #controller {
            z-index: 999;
            position: absolute;
            left: 15px;
            top: 20px;
        }

        .search-title {
            float: left;
            display: block;
            font-size: 14px;
            color: white;
            height: 40px;
            line-height: 40px;
            width: 80px;
            background: url(/VOCS/image/map/search-title.png) #4cb7db no-repeat right center;
            text-align: left;
            text-indent: 2px;
        }

        .search-btn {
            float: left;
            cursor: pointer;
            color: white;
            width: 60px;
            height: 40px;
            line-height: 40px;
            font-size: 14px;
            text-align: center;
            background: url(/VOCS/image/map/search-btn.png) #006698 no-repeat center center;
        }

        #searchTask {
            float: left;
            font-size: 14px;
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
                width: 612px;
                display: none;
                box-shadow: 0 0 1px #999;
            }

        .search-from {
            float: left;
            overflow: hidden;
            width: 140px;
            height: 22px;
            padding: 3px 5px;
            border: 1px solid #fff;
            cursor: pointer;
        }

            .search-from:hover {
                background-color: #9ed1ee;
            }

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
            bottom: 10px;
            left: 50%;
            margin-left: -240px;
            z-index: 999;
            box-shadow: rgba(153,153,153,.6) 0 0 1px;
            border-radius: 5px;
        }

            #buttonDiv div, #companyTypeIcon div {
                float: left;
                height: 31px;
                padding: 0px 10px;
                font-size: 13px;
                cursor: pointer;
                /*border: 1px solid black;*/
            }

                #buttonDiv div span, #companyTypeIcon div span {
                    display: inline-block;
                    padding-top: 5px;
                }

        #companyTypeIcon {
            height: 31px;
            background: rgba(255,255,255,.9);
            position: absolute;
            top: 5px;
            right: 200px;
            z-index: 10;
            border: 1px solid #ccc;
            bottom: 10px;
            z-index: 999;
            box-shadow: rgba(153,153,153,.6) 0 0 1px;
            border-radius: 5px;
        }

        #vocsItemIcon div, #wryItemIcon div {
            float: left;
            height: 31px;
            padding: 0px 10px;
            font-size: 13px;
            cursor: pointer;
            /*border: 1px solid black;*/
        }

            #vocsItemIcon div span, #wryItemIcon div span {
                display: inline-block;
                padding-top: 5px;
            }

        #vocsItemIcon, #wryItemIcon {
            height: 31px;
            background: rgba(255,255,255,.9);
            position: absolute;
            top: 5px;
            left: 10px;
            z-index: 10;
            border: 1px solid #ccc;
            bottom: 10px;
            z-index: 999;
            box-shadow: rgba(153,153,153,.6) 0 0 1px;
            border-radius: 5px;
        }

        #buttonDivZC {
            /*margin-left:6px;*/
            /*background:url(/VOCs/image/map/company-unreport.png) no-repeat left center;*/
            background-color: green;
            color: white;
            /*background-size:22px 22px;*/
        }

        #buttonDivYCB {
            /*background:url(/VOCs/image/map/company-report.png) no-repeat left center;*/
            background-color: red;
            color: white;
            /*background-size:22px 22px;*/
        }

        #buttonDivYLX {
            /*background:url(/VOCs/image/map/company-report.png) no-repeat left center;*/
            background-color: gray;
            color: white;
            /*background-size:22px 22px;*/
        }

        #buttonDivRed {
            background: url(/Content/img/map/person-red.png) no-repeat left center;
        }

        #buttonDivGray {
            background: url(/Content/img/map/person-gray.png) no-repeat left center;
        }

        #buttonDivFLD {
            background: url(/Content/img/map/camera2.png) no-repeat left center;
            background-color: #D4D4D4;
        }

        #buttonDivCFCompany {
            background: url(/Content/img/company.png) no-repeat left center;
            background-color: #D4D4D4;
        }

        #buttonDivYCGD {
            background: url(/Content/img/site.png) no-repeat left center;
            background-color: #D4D4D4;
        }


        #thediv {
            width: 360px;
            background: #fff;
            position: absolute;
            left: 15px;
            top: 75px;
            z-index: 99;
            box-shadow: 0 0 3px #999;
            padding-bottom: 10px;
        }

        #thedivSpan {
            width: 20px;
            height: 90px;
            line-height: 23px;
            background: #b5e3f2;
            position: absolute;
            right: -20px;
            top: 70px;
            cursor: pointer;
        }

        .count {
            color: #ccedf6;
            background-color: #1ABC9C;
            /*font-size:16px;*/
            /*text-align:center;*/
            height: 35px;
        }

        #CountryTownSelectDiv {
            width: 100%;
            font-size: 24px;
            color: width;
            position: absolute;
            text-align: center;
            z-index: 999;
        }

        .area {
            padding: 6px 20px 6px 10px;
            border: 1px solid #007493;
            border-top: none;
            color: #069;
        }

            .area:hover {
                background-color: #88daf0;
            }

        #CountryTownSelectDiv div:first-child {
            border-top: 1px solid #007493;
            color: #fff;
            background-image: url(/Content/img/map/arrows-bottom.png);
            background-position: center right;
            background-repeat: no-repeat;
        }

        .title {
            text-align: left;
            padding: 8px 0;
            position: relative;
            border-top: 1px solid #069;
            margin: 50px 10px 0 10px;
        }

            .title strong {
                font-size: 14px;
                color: #069;
                text-indent: 10px;
                display: inline-block;
            }

            .title a {
                position: absolute;
                right: 10px;
                top: 8px;
                color: #666;
            }

                .title a:hover {
                    color: #f30;
                }

        .table-title-head {
            background-color: #0091b8;
            /*height:45px;*/
            margin-top: 10px;
            text-align: center;
            padding: 5px 3px;
            font-size: 16px;
        }

        .table-title {
            border-collapse: collapse;
            background-color: #eef7f9;
            width: 100%;
            border-top: 1px solid #069;
        }

            .table-title th {
                text-align: right;
                width: 40%;
                padding: 6px;
                border-bottom: 1px solid #fff;
                border-collapse: collapse;
            }

            .table-title td {
                text-align: left;
                padding: 6px 0;
                border-bottom: 1px solid #fff;
                border-collapse: collapse;
            }

                .table-title td strong {
                    color: #f30;
                    font-size: 13px;
                    padding: 0 5px;
                }

        .tab-head {
            position: relative;
            border-top: 1px solid #ccc;
            overflow: hidden;
            height: 36px;
            line-height: 36px;
            background-color: #eef7f9;
            margin: 10px 10px 0 10px;
        }

            .tab-head p {
                position: absolute;
                height: 4px;
                background-color: #069;
                margin: 0;
                padding: 0;
                width: 50%;
                left: 0;
                top: -1px;
                z-index: 99;
            }

            .tab-head span {
                display: inline-block;
                width: 50%;
                float: left;
                text-align: center;
                font-size: 14px;
                cursor: pointer;
            }

                .tab-head span.on {
                    color: #069;
                }

        .tab-body {
            margin: 0 10px;
            background-color: #eef7f9;
        }

        .tab-main {
            display: none;
        }

        .tab-min {
            padding: 3px 10px 0px 10px;
        }

            .tab-min span {
                display: inline-block;
                height: 26px;
                line-height: 26px;
                width: 80px;
                text-align: center;
                overflow: hidden;
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

        .tabfrom {
            text-align: center;
            border: 1px solid #D4D4D4;
            border-collapse: collapse;
            width: 100%;
        }

            .tabfrom td {
                border: 1px solid #a7d5ef;
                padding: 3px;
            }

        .more {
            position: absolute;
            top: 65px;
            right: 20px;
            cursor: pointer;
            color: #fff;
        }

            .more:hover {
                color: #66ccff;
                text-decoration: none;
            }
    </style>
    <script type="text/javascript">
        var showType = "";  //展示类型 默认为web, 可传app
        var map;   //百度地图对象
        var IsVOCs;  //企业类型，0-污染源，1-VOCs
        var monitorItem = "all"; //计算排放量的监测因子项，默认为all-总量
        var selectCompanyID; //选中的企业ID
        function init() {
            //isShow = getQueryStr("isShow");
            //if (isShow == "show")
            //    $("#Showback").show();

            IsVOCs = getQueryStr("IsVOCs");
            if (!IsVOCs) {
                IsVOCs = 0;
            }

            mylayout.init();

            initPageDatas(); //初始化页面所需数据--页面首次加载时调用

            $("#container").height($(window).outerHeight() - 10);
            $(window).resize(function () {
                $("#container").height($(window).outerHeight() - 10);
            });
            map = new BMap.Map("container");

            //初始化地图,选取人员信息第一个点为起始点
            var point = new BMap.Point(114.271093, 34.976331);
            if (companyGPSDatas && companyGPSDatas.length > 0) {
                if (selectCompanyID) {
                    for (var i = 0; i < companyGPSDatas.length; i++) {
                        if (companyGPSDatas[i].ID == selectCompanyID) {
                            point = new BMap.Point(companyGPSDatas[i].LONGITUDE, companyGPSDatas[i].LATITUDE);
                        }
                    }
                }
                else {
                    point = new BMap.Point(companyGPSDatas[0].LONGITUDE, companyGPSDatas[0].LATITUDE);
                }
            }
            map.centerAndZoom(point, 12);
            map.enableScrollWheelZoom();
            SetMapStyle();

            showAllCompany();

            showItemIcon(); //按类型显示排放量统计监测因子
        }

        window.onload = init;  //页面初始化事件 -- 页面首次加载时调用

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

        function hisDataAnalysis(PKID) {
            var url = "/PlatformIndex/WRYMap/HisDataAnalysis?PKID=" + PKID;
            var dialog = $('<iframe src="' + url + '" style="border: 0px; width: 100%; height: 600px; overflow: auto;"></iframe>').dialog({
                title: "历史数据分析",
                width: 900,
                height: 600,
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

        function realVideo() {
            var dialog = $('<div></div>').dialog({
                title: "播放视频",
                width: 360,
                height: 340,
                cache: false,
                modal: true,
                content: '<p style="top:10px;position:relative;" align="center"><video src="/Content/movie.ogg" controls="controls">您的浏览器不支持此视频格式</video></p>',
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

        function dealAlarm(PKID) {
            com.ajax({
                url: "GetLastAlertByPK",
                data: { pkid: PKID },
                async: false,
                success: function (result) {
                    if (!result) com.msg.info("最近7天没有告警信息！");
                    else {
                        var buttons = [
                            {
                                text: '确定',
                                iconCls: 'icon-save',
                                handler: function () {
                                    debugger;
                                    detailIFName.SaveSetting();
                                    $('#dg').datagrid('reload');
                                    dialog.dialog("close");
                                    $(detailIFName.document).empty();
                                }
                            },
                             {
                                 text: '取消',
                                 iconCls: 'icon-cancel',
                                 handler: function () {
                                     dialog.dialog("close");
                                     $(detailIFName.document).empty();
                                 }
                             }];
                        if (result.STATE) buttons.shift();
                        var dialog = $('<iframe id="detailIF" name="detailIFName" src=' + "/BaseData/AlertCheck/Edit?id=" + result.ID + ' style="border: 0px; width: 100%; height: 450px; overflow: hidden;"></iframe>').dialog({
                            title: result.STATE ? "告警查看" : "告警处理",
                            width: 500,
                            height: 550,
                            cache: false,
                            modal: true,
                            closable: false,
                            buttons: buttons
                        });
                    }
                }
            });
        }

        function CompanySelected(id) {
            for (var i = 0; i < companyGPSDatas.length; i++) {
                if (companyGPSDatas[i].ID == id) {
                    var point = new BMap.Point(companyGPSDatas[i].LONGITUDE, companyGPSDatas[i].LATITUDE);
                    map.centerAndZoom(point, map.getZoom());
                    selectCompanyID = id;
                }
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="layout">
        <div region="west" split="true" style="width: 270px; padding: 5px">
            <!--180-->
            <%
                ViewDataDictionary dict = new ViewDataDictionary();
                dict.Add("fs", ViewData["fs"]);
                dict.Add("fq", ViewData["fq"]);
                dict.Add("vocs", ViewData["vocs"]);
                dict.Add("selectId", ViewData["PKId"]);
                dict.Add("multiSelect", false);
                dict.Add("companyOnly", ViewData["companyOnly"]);
                Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = dict });
                //Html.RenderAction("GroupSelect", "UserControl", new { area = "Bas", model = ViewBag.controllerParam});
            %>
        </div>
        <div region="center" title="" iconcls="icon-house">
            <div id="container"></div>
            <div id="companyTypeIcon">
                <div id="buttonDivZC" onclick="showZCClick()">
                    <span>正常(0)</span><input id="buttonDivZCInput" type="checkbox" style="visibility: hidden;" checked="checked" />
                </div>
                <div id="buttonDivYCB" onclick="showYCBClick()">
                    <span>有超标(0)</span><input id="buttonDivYCBInput" type="checkbox" style="visibility: hidden;" checked="checked" />
                </div>
                <div id="buttonDivYLX" onclick="showYLXClick()">
                    <span>有离线(0)</span><input id="buttonDivYLXInput" type="checkbox" style="visibility: hidden;" checked="checked" />
                </div>
            </div>
            <div id="wryItemIcon" style="display: none; color: white;">
                <div id="buttonDivWryAll" onclick="showWryAllClick()">
                    <span>总量</span>
                </div>
                <div id="buttonDivWrySO2" onclick="showWrySO2Click()">
                    <span>SO2</span>
                </div>
                <div id="buttonDivWryYC" onclick="showWryYCClick()">
                    <span>烟尘</span>
                </div>
                <div id="buttonDivWryDY" onclick="showWryDYClick()">
                    <span>氮氧化物</span>
                </div>
                <div id="buttonDivWryHX" onclick="showWryHXClick()">
                    <span>化学需氧量</span>
                </div>
                <div id="buttonDivWryAD" onclick="showWryADClick()">
                    <span>氨氮</span>
                </div>
            </div>
            <div id="vocsItemIcon" style="display: none; color: white;">
                <div id="buttonDivVOCsAll" onclick="showVOCsAllClick()">
                    <span>总量</span>
                </div>
                <div id="buttonDivVOCsB" onclick="showWVOCsBClick()">
                    <span>苯</span>
                </div>
                <div id="buttonDivVOCsJB" onclick="showVOCsJBClick()">
                    <span>甲苯</span>
                </div>
                <div id="buttonDivVOCs2JB" onclick="showVOCs2JBClick()">
                    <span>二甲苯</span>
                </div>
                <div id="buttonDivVOCsFJY" onclick="showVOCsFJYClick()">
                    <span>非甲烷总烃</span>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
