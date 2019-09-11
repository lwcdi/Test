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
            $(".RowSpan").height(divHeight * 0.90);
            $(".RowSpan2").height(divHeight * 0.78);
            $(window).resize(function () {
                var divHeight = $(window.parent.document).find(".tabs-panels").height();
                $(".ViewCommon").height(divHeight * 0.40);
                $(".ChartCommon").height(divHeight * 0.35);
                $(".RowSpan").height(divHeight * 0.90);
                $(".RowSpan2").height(divHeight * 0.78);
            })
        });
    </script>
    <script src="/Scripts/report/VOCsView.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="Views" style="overflow:auto;">
        <div class="RowSpan SelfTest" style="float:left;margin: 20px 10px 0px 20px;overflow: hidden;width:30%;">
            <div class="View_Title">
                <%--<span class="Title_Number">1</span>--%>
                <span class="Title_Left">当天数据修约情况</span>
                <%--<span class="Title_Right"><a class="Detail" href="/Monitor/DeviceCheckRecord/List">>>查看详情</a></span>--%>
            </div>
            <div class="View_Content">
                <div id="SelfTest" class="ChartCommon RowSpan2">
                    <!--数据列表-->
                    <div id="dgFS" style="height: 50px;overflow:auto;">
                    </div>
                </div>
            </div>
        </div>
        <div class="ViewCommon VehAppear">
            <div class="View_Title">
                <%--<span class="Title_Number">2</span>--%>
                <span class="Title_Left">实时联网状态</span>
                <%--<span class="Title_Right"><a class="Detail" href="/TestBus/AppearanceTest/List">>>查看详情</a></span>--%>
            </div>
            <div class="View_Content">
                <div id="VehAppear" class="VehAppear_View ChartCommon"></div>
            </div>
        </div>
        <div class="ViewCommon RealTime">
            <div class="View_Title">
                <%--<span class="Title_Number">3</span>--%>
                <span class="Title_Left">当天重点关注企业排放情况</span>
                <%--<span class="Title_Right"><a class="Detail" href="/Monitor/RealVideoControll/List">>>查看详情</a></span>--%>
            </div>
            <div class="View_Content">
                <div id="RealTime" class="RealTime_View ChartCommon"></div>
            </div>
        </div>
        <div class="ViewCommon ReportInfo">
            <div class="View_Title">
                <%--<span class="Title_Number">4</span>--%>
                <span class="Title_Left">当天区域总量分布情况</span>
                <%--<span class="Title_Right"><a class="Detail" href="/TestBus/ReportPrint/List">>>查看详情</a></span>--%>
            </div>
            <div class="View_Content">
                <div id="FQZL" class="ChartCommon"></div>
            </div>
        </div>

        <div class="ViewCommon WarnInfo">
            <div class="View_Title">
                <%--<span class="Title_Number">6</span>--%>
                <span class="Title_Left">近3天告警信息</span>
                <span class="Title_Right"></span>
            </div>
            <div class="View_Content">
                <div class="ChartCommon">
                    <!--数据列表-->
                    <div id="dg" style="height: 50px;overflow:auto;">
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
