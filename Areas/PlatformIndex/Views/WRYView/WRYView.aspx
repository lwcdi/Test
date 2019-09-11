<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Content/css/reportView/ReportViewCommon.css" rel="stylesheet" />
    <style>
        .ViewCommon {
            width: 30%;
        }

        .Detail {
            display: inline-block;
            width: 80px;
            height: 15px;
            margin-left: 10px;
            text-decoration: none;
        }

            .Detail:hover {
                color: blue;
                text-decoration: none;
            }

        ul {
            list-style: none;
            margin: 5px;
        }

            ul li {
                margin: 3px;
            }

                ul li label {
                    font-weight: bold;
                }

                ul li span {
                    float: right;
                }
    </style>
    <script>
        $(function () {
            var divHeight = $(window.parent.document).find(".tabs-panels").height();
            $(".ViewCommon").height(divHeight * 0.40);
            $(".ChartCommon").height(divHeight * 0.35);
            $(window).resize(function () {
                var divHeight = $(window.parent.document).find(".tabs-panels").height();
                $(".ViewCommon").height(divHeight * 0.40);
                $(".ChartCommon").height(divHeight * 0.35);
            })
        });
    </script>
    <script src="/Scripts/report/WRYView.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="SearchBar" style="height:30px;width:100%;border:1px solid black;display:none;">查询条件</div>
     <div class="Views" style="overflow:auto;">
        <div class="ViewCommon SelfTest">
            <div class="View_Title">
                <%--<span class="Title_Number">1</span>--%>
                <span class="Title_Left">管控点位分布情况（<span id="GKDFBCount">178</span>家）</span>
                <%--<span class="Title_Right"><a class="Detail" href="/Monitor/DeviceCheckRecord/List">>>查看详情</a></span>--%>
            </div>
            <div class="View_Content">
<%--                <div class="Message">
                    <ul>
                        <li>检验机构自检总数量：<label id="SelfTestTotal"></label>；<span>统计周期：当天最新</span></li>
                        <li>自检合格数：<label id="SelfTestPass"></label>；</li>
                        <li>合格率为：<label id="SelfTestPR"></label>；</li>
                    </ul>
                </div>--%>
                <div id="SelfTest" class="SelfTest_View ChartCommon"></div>
            </div>
        </div>
        <div class="ViewCommon VehAppear">
            <div class="View_Title">
                <%--<span class="Title_Number">2</span>--%>
                <span class="Title_Left">实时点位在线情况（<span id="SSDWCount">178</span>家)</span>
                <%--<span class="Title_Right"><a class="Detail" href="/TestBus/AppearanceTest/List">>>查看详情</a></span>--%>
            </div>
            <div class="View_Content">
                <div id="VehAppear" class="VehAppear_View ChartCommon"></div>
            </div>
        </div>
        <div class="ViewCommon RealTime">
            <div class="View_Title">
                <%--<span class="Title_Number">3</span>--%>
                <span class="Title_Left">停运备案情况（<span id="TYBACount">178</span>家)</span>
                <%--<span class="Title_Right"><a class="Detail" href="/Monitor/RealVideoControll/List">>>查看详情</a></span>--%>
            </div>
            <div class="View_Content">
                <div id="RealTime" class="RealTime_View ChartCommon"></div>
            </div>
        </div>
        <div class="ViewCommon ReportInfo">
            <div class="View_Title">
                <%--<span class="Title_Number">4</span>--%>
                <span class="Title_Left">当天废气总量（<span id="FQZLCount">178</span>吨)</span>
                <%--<span class="Title_Right"><a class="Detail" href="/TestBus/ReportPrint/List">>>查看详情</a></span>--%>
            </div>
            <div class="View_Content">
                <div id="FQZL" class="ChartCommon"></div>
            </div>
        </div>
        <div class="ViewCommon BusinessDeal">
            <div class="View_Title">
                <%--<span class="Title_Number">5</span>--%>
                <span class="Title_Left">当天废水总量（<span id="FSZLCount">178</span>吨）</span>
 <%--               <span class="Title_Right"></span>--%>
            </div>
            <div class="View_Content">
                <div id="FSZL" class="ChartCommon"></div>
            </div>
        </div>
        <div class="ViewCommon WarnInfo">
            <div class="View_Title">
                <%--<span class="Title_Number">6</span>--%>
                <span class="Title_Left">近3天告警信息<%--（<span style="color:red;" class="GJUnDeal">178</span>）--%></span>
                <span class="Title_Right"></span>
            </div>
            <div class="View_Content">
                <div class="ChartCommon">
                    <div style="text-align:center;padding:5px 0;display:none;"><span>全部（<span id="GJAll">178</span>）已处理（<span id="GJDeal">178</span>）未处理（<span style="color:red;" class="GJUnDeal">178</span>）</span></div>
                    <!--数据列表-->
                    <div id="dg" style="height: 50px;overflow:auto;">
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
