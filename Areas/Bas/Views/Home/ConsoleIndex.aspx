<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .row {
            box-sizing: border-box;
        }

        .col-lg-1 {
            box-sizing: border-box;
            width: 50%;
            position: relative;
            min-height: 1px;
            padding-left: 15px;
            padding-right: 15px;
        }

        .col-lg-2 {
            box-sizing: border-box;
            width: 100%;
            position: relative;
            min-height: 1px;
            padding-left: 15px;
            padding-right: 15px;
        }

        .floatleft {
            float: left;
        }

        .floatright {
            float: right;
        }

        .color97D3C5 {
            background-color: #97d3c5;
        }

        .chart-title {
            min-height: 42px;
            margin: 15px 0 0 0;
            line-height: 42px;
            font-size: 21px;
        }

        .chart-body {
            border: 1px solid #97d3c5;
        }

        .title {
            margin: 0;
            line-height: 30px;
            font-size: 18px;
        }

        .span6 {
            width: 47.1%;
            float: left;
            margin: 0 10px;
            font-size: 14px;
        }

        .chart-heading {
            background-color: #75B9E6;
            text-align: center;
            height: 30px;
            line-height: 30px;
            overflow: hidden;
            color: white;
            font-weight: bold;
            font-size: 14px;
            margin-top: 10px;
        }

        .box-content {
            padding: 10px;
            border: 1px solid #dddddd;
            background: white;
            display: block;
            -webkit-box-shadow: 0 1px 3px rgba(0, 0, 0, 0.055);
            -moz-box-shadow: 0 1px 3px rgba(0, 0, 0, 0.055);
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.055);
        }

        .box-statistic {
            background-color: white;
            margin-top: 5px;
            margin-bottom: 5px;
            padding: 5px 10px;
            position: relative;
        }

        .text-NotAudited {
            color: #d81e06;
        }

        .text-NotNotice {
            color: #13227a;
        }

        .text-NotDistribution {
            color: #1296db;
        }

        .icon-NotAudited {
            background: url(/Content/img/NotAudited.png) no-repeat;
            width: 32px;
            height: 32px;
        }

        .icon-NotNotice {
            background: url(/Content/img/NotNotice.png) no-repeat;
            width: 32px;
            height: 32px;
        }

        .icon-NotDistribution {
            background: url(/Content/img/NotDistribution.png) no-repeat;
            width: 32px;
            height: 32px;
        }

        .align-right {
            right: 10px;
            position: absolute;
            top: 20px;
        }

            .align-right:hover {
                right: 15px;
                position: absolute;
                top: 20px;
            }

        .row .update {
            position: relative;
            background: #fff;
            padding: 20px;
            border: 1px solid #e4e4e4;
            border-radius: 3px;
            margin-bottom: 20px;
            margin: 10px 15px 0 15px;
        }

        .row .version {
            font-size: 13px;
            font-family: 'Montserrat', sans-serif;
        }

        .label-danger {
            background: #ef4836;
        }

        .row .label {
            font-size: inherit;
            padding: 1px 6px;
            font-weight: 600;
            border-radius: 4px;
            color: #fff;
        }

        .row .date {
            font-size: 12px;
            position: absolute;
            right: 20px;
            top: 20px;
        }

        .row .list {
            font-size: 12px;
            margin-top: 10px;
            background: #f2f2f2;
            padding: 6px 10px;
            border-radius: 3px;
            border-left: 2px solid #ccc;
        }

            .row .list h4 {
                margin: 0px;
                font-size: 14px;
            }

            .row .list:hover {
                background: #eee;
            }

            .row .list .down {
                display: inline;
            }

        @media screen and (min-width: 1601px) {
            #main1 {
                width: 100%;
                height: 360px;
            }

            #main2 {
                width: 100%;
                height: 360px;
            }

            #main3 {
                width: 100%;
                height: 360px;
            }
        }

        @media screen and (max-width: 1600px) {
            #main1 {
                width: 100%;
                height: 320px;
            }

            #main2 {
                width: 100%;
                height: 280px;
            }

            #main3 {
                width: 100%;
                height: 280px;
            }

            .chart-title {
                min-height: 22px;
                margin: 10px 0 0 0;
                line-height: 22px;
                font-size: 16px;
            }
        }

        @media screen and (max-width: 1200px) {
            #main1 {
                width: 100%;
                height: 220px;
            }

            #main2 {
                width: 100%;
                height: 260px;
            }

            #main3 {
                width: 100%;
                height: 260px;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <div class="row">
        <div class="update">
            <span class="label label-danger version">版本 1.0</span>
            <span class="date">2018-02-06</span>

            <div class="list">
                <h4>智慧环保平台APP--安卓版</h4>
                <p><span class="icon icon-download down"></span><a href="/download/SEPAPP.apk">下载</a></p>
		        <%--<p>1、修复部分android API创建文件异常处理</p>
                <p>2、修复手机或者部分不能显示的问题</p>
                <p>3、如果提示拍照失败，在文件管理里面，把comleader这个文件删除，在重新拍照</p>
                <p>4、修复华为P8、P9部分手机拍照时不能创建文件的情况,修复OPPO一些手机下拉显示不全的情况</p>
                <p>5、修复一些低版本 图片不能显示，拍照后空白的情况</p>--%>
            </div>
        </div>
        <div class="col-lg-1 floatleft">
            <%--  <div class="chart-heading">系统订单状态</div>--%>
            <%--    <div class="chart-body">
                <div id="main2">
                    <div class='span6'>
                        <div class='box-content box-statistic'>
                            <h3 id="NotAuditedQty" class='title text-NotAudited'>0</h3>
                            <small>所有未审核订单数</small>
                            <div class='icon-NotAudited align-right'></div>
                        </div>
                        <div class='box-content box-statistic'>
                            <h3 id="NotNoticeQty" class='title text-NotNotice'>0</h3>
                            <small>已审核未通知的订单数</small>
                            <div class='icon-NotNotice align-right'></div>
                        </div>
                    </div>
                    <div class='span6'>
                        <div class='box-content box-statistic'>
                            <h3 id="NotDistributionQty" class='title text-NotDistribution'>0</h3>
                            <small>已通知未配货订单数</small>
                            <div class='icon-NotDistribution align-right'></div>
                        </div>
                    </div>
                </div>
            </div>--%>
        </div>
        <%--    <div class="col-lg-1 floatleft">
            <div class="chart-heading color97D3C5">
                <h4 class="chart-title">最近30天订单数量222</h4>
            </div>
            <div class="chart-body">
                <div id="main3"></div>
            </div>
        </div>--%>
    </div>

    <%--<script type="text/javascript" src="/Content/js/charthelp.js"></script>--%>
</asp:Content>
