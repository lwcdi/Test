<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script src="/Content/js/getQueryStr.js"></script>
    <style>
        * {
            margin: 0;
            padding: 0;
        }

        table tr td {
            border: 1px solid #D4D4D4;
            height: 25px;
        }
    </style>
    <script>
        var companyInfoDatas;  //企业信息数据
        var companyID;  //企业ID

        function wordLimit() {
            $(".zxx_text_overflow_6").each(function () {
                var copyThis = $(this.cloneNode(true)).hide().css({
                    'position': 'absolute',
                    'width': 'auto',
                    'overflow': 'visible'
                });
                $(this).after(copyThis);
                if (copyThis.width() > $(this).width()) {
                    if ($(this).attr("title") == undefined) {
                        $(this).attr("title", copyThis.text());
                    }
                    $(this).text($(this).text().substring(0, $(this).html().length - 4));
                    $(this).html($(this).html() + '...');
                    copyThis.remove();
                    wordLimit();
                } else {
                    copyThis.remove(); //清除复制
                    return;
                }
            });
        }
        window.onload = function () {
            companyID = getQueryStr("id");
            initPageDatas();  //加载页面所需数据
            LoadAllTables(); //加载页面表格
            wordLimit();

            addTab(); //添加排口信息Tab
        };
        //添加排口信息Tab
        function addTab() {
            var PKInfos = getCompanyPKInfo();
            for (var i = 0; i < PKInfos.length; i++) {
                if ($('#PKTabs').tabs('exists', PKInfos[i].PKNAME)) {
                    $('#PKTabs').tabs('select', PKInfos[i].PKNAME);
                } else {
                    var content;
                    debugger;
                    if (PKInfos[i].TYPE == "1") { //废气
                        content = '<iframe scrolling="auto" frameborder="0"  src="/PlatformIndex/WRYMap/PKInfoFQ?PKID=' + PKInfos[i].PKID + '&type=' + PKInfos[i].TYPE + '" style="width:100%;height:98%;"></iframe>';
                    }
                    else if (PKInfos[i].TYPE == "2") { //废水
                        content = '<iframe scrolling="auto" frameborder="0"  src="/PlatformIndex/WRYMap/PKInfoFS?PKID=' + PKInfos[i].PKID + '&type=' + PKInfos[i].TYPE + '" style="width:100%;height:98%;"></iframe>';
                    }
                    else if (PKInfos[i].TYPE == "3") { //VOCs
                        content = '<iframe scrolling="auto" frameborder="0"  src="/PlatformIndex/WRYMap/PKInfoVOCs?PKID=' + PKInfos[i].PKID + '&type=' + PKInfos[i].TYPE + '" style="width:100%;height:98%;"></iframe>';
                    }
                    $('#PKTabs').tabs('add', {
                        pkid: PKInfos[i].PKID,
                        title: PKInfos[i].PKNAME,
                        content: content,
                        closable: false
                    });
                }
            }
        }

        //查询企业排口情况
        function getCompanyPKInfo() {
            //TYPE 排口种类(1-废气、2-废水、3-VOCs)
            var arr = [];
            $.ajax({
                type: "post",
                async: false, //同步执行
                url: "/PlatformIndex/WRYMap/GetCompanyPKInfo?companyID=" + companyID,
                contentType: "application/json",
                data: {},
                dataType: "json", //返回数据形式为json
                success: function (result) {
                    debugger;
                    //arr = $.parseJSON(result.d);
                    arr = result;
                },
                error: function (errorMsg) {
                }
            });
            return arr;
            ////TYPE 排口种类(1-废气、2-废水、3-VOCs)
            //return [{ PKID: "1", PKNAME: "废气排口1", COMPANYID: "1", TYPE: "1" }, { PKID: "2", PKNAME: "废水排口2", COMPANYID: "1", TYPE: "2" }, { PKID: "3", PKNAME: "VOCs排口2", COMPANYID: "1", TYPE: "3" }];
        }

        //告警处置
        function dealAlarm() {
            window.parent.dealAlarm($('#PKTabs').tabs('getSelected').panel('options').pkid);
        }

        //历史数据分析 
        function hisDataAnalysis() {
            window.parent.hisDataAnalysis($('#PKTabs').tabs('getSelected').panel('options').pkid);
        }

        //实时视频 
        function realVideo() {
            window.parent.realVideo();
        }

        //查看抽查、巡查照片 type 1-抽查 2-巡查
        function showChouChaPhotos(id, type) {
            window.parent.showChouChaPhotos(getQueryStr("userName"), id, type);
        }

        //加载页面表格数据
        function LoadAllTables() {
            LoadCompanyInfoDataTable();
            //LoadChouChaDataTable();
            //LoadXunChaDataTable();
        }

        //初始化页面数据
        function initPageDatas() {
            companyInfoDatas = GetCompanyInfoDatas();  //加载企业信息数据
            //chouChaDatas = GetCompanyChouChaDatas(userName, taskID);  //加载人员抽查数据
            //xunChaDatas = GetCompanyXunChaDatas(userName, taskID);  //加载人员抽查数据
        }

        //加载企业信息数据表格
        function LoadCompanyInfoDataTable() {
            companyInfoData = companyInfoDatas[0];

            $("#CompanyInfoDataTable tbody").empty(); //清空table

            $("#companyName").text(companyInfoData.NAME);
            $("#companyStatus").text("(" + companyInfoData.STATUS + ")");
            $("#updateTime").text(getNowFormatDate());  //更新时间
            var trHtml = companyInfoTableRow.replace("ADDRESS", companyInfoData.ADDRESS).replace("JWD", companyInfoData.LONGITUDE + "," + companyInfoData.LATITUDE).replace("AREA", companyInfoData.AREA)
                    .replace("BASTYPE", companyInfoData.BASTYPE).replace("LEGALOR", companyInfoData.LEGALOR).replace("GZCD", companyInfoData.GZCD);
            $("#CompanyInfoDataTable tbody").append(trHtml);
        }

        //加载企业信息数据
        function GetCompanyInfoDatas() {
            var arr = [];
            $.ajax({
                type: "post",
                async: false, //同步执行
                url: "/PlatformIndex/WRYMap/GetCompanyInfoDatas?companyID=" + companyID,
                contentType: "application/json",
                data: {},
                dataType: "json", //返回数据形式为json
                success: function (result) {
                    debugger;
                    //arr = $.parseJSON(result.d);
                    arr = result;
                },
                error: function (errorMsg) {
                }
            });
            return arr;
        }

        function reflashButtonClick() {
            initPageDatas();  //加载页面所需数据
            LoadAllTables(); //加载页面表格
            wordLimit();
        }

        //获取当前时间
        function getNowFormatDate() {
            var date = new Date();
            var seperator1 = "-";
            var seperator2 = ":";
            var month = date.getMonth() + 1;
            var strDate = date.getDate();
            if (month >= 1 && month <= 9) {
                month = "0" + month;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
                    + " " + date.getHours() + seperator2 + date.getMinutes()
                    + seperator2 + date.getSeconds();
            return currentdate;
        }

        //企业信息测试数据
        var companyInfoDatas = [
            { NAME: "张三", STATUS: "正常", orgName: "白沙镇一队", AREA: "白沙镇", ADDRESS: "通惠路附近通惠路附近通惠路附近通惠路附近通惠路附近通惠路附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "王五", STATUS: "正常", orgName: "白沙镇二队", AREA: "白沙镇", ADDRESS: "通惠路附近通惠路附近通惠路附近通惠路附近通惠路附近通惠路附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "赵六", STATUS: "正常", orgName: "官渡镇一队", AREA: "官渡镇", ADDRESS: "官渡镇附近官渡镇附近官渡镇附近官渡镇附近官渡镇附近官渡镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "小张", STATUS: "正常", orgName: "狼城岗镇一队", AREA: "狼城岗镇", ADDRESS: "狼城岗镇附近狼城岗镇附近狼城岗镇附近狼城岗镇附近狼城岗镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "小王", STATUS: "正常", orgName: "张庄镇一队", AREA: "张庄镇", ADDRESS: "张庄镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "张三", STATUS: "正常", orgName: "白沙镇一队", AREA: "白沙镇", ADDRESS: "通惠路附近通惠路附近通惠路附近通惠路附近通惠路附近通惠路附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "王五", STATUS: "正常", orgName: "白沙镇二队", AREA: "白沙镇", ADDRESS: "通惠路附近通惠路附近通惠路附近通惠路附近通惠路附近通惠路附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "赵六", STATUS: "正常", orgName: "官渡镇一队", AREA: "官渡镇", ADDRESS: "官渡镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "小张", STATUS: "正常", orgName: "狼城岗镇一队", AREA: "狼城岗镇", ADDRESS: "狼城岗镇附近狼城岗镇附近狼城岗镇附近狼城岗镇附近狼城岗镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "小王", STATUS: "正常", orgName: "张庄镇一队", AREA: "张庄镇", ADDRESS: "张庄镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "张三", STATUS: "正常", orgName: "白沙镇一队", AREA: "白沙镇", ADDRESS: "通惠路附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "王五", STATUS: "正常", orgName: "白沙镇二队", AREA: "白沙镇", ADDRESS: "通惠路附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "赵六", STATUS: "正常", orgName: "官渡镇一队", AREA: "官渡镇", ADDRESS: "官渡镇附近官渡镇附近官渡镇附近官渡镇附近官渡镇附近官渡镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "小张", STATUS: "正常", orgName: "狼城岗镇一队", AREA: "狼城岗镇", ADDRESS: "狼城岗镇附近狼城岗镇附近狼城岗镇附近狼城岗镇附近狼城岗镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "小王", STATUS: "正常", orgName: "张庄镇一队", AREA: "张庄镇", ADDRESS: "张庄镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "张三", STATUS: "正常", orgName: "白沙镇一队", AREA: "白沙镇", ADDRESS: "通惠路附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "王五", STATUS: "正常", orgName: "白沙镇二队", AREA: "白沙镇", ADDRESS: "通惠路附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "赵六", STATUS: "正常", orgName: "官渡镇一队", AREA: "官渡镇", ADDRESS: "官渡镇附近官渡镇附近官渡镇附近官渡镇附近官渡镇附近官渡镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "小张", STATUS: "正常", orgName: "狼城岗镇一队", AREA: "狼城岗镇", ADDRESS: "狼城狼城岗镇附近狼城岗镇附近狼城岗镇附近狼城岗镇附近狼城岗镇附近岗镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "小王", STATUS: "正常", orgName: "张庄镇一队", AREA: "张庄镇", ADDRESS: "张庄镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "张三", STATUS: "正常", orgName: "白沙镇一队", AREA: "白沙镇", ADDRESS: "通惠路附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "王五", STATUS: "正常", orgName: "白沙镇二队", AREA: "白沙镇", ADDRESS: "通惠路附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "赵六", STATUS: "正常", orgName: "官渡镇一队", AREA: "官渡镇", ADDRESS: "官渡镇附近官渡镇附近官渡镇附近官渡镇附近官渡镇附近官渡镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "小张", STATUS: "正常", orgName: "狼城岗镇一队", AREA: "狼城岗镇", ADDRESS: "狼城狼城岗镇附近狼城岗镇附近狼城岗镇附近狼城岗镇附近狼城岗镇附近岗镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
            { NAME: "小王", STATUS: "正常", orgName: "张庄镇一队", AREA: "张庄镇", ADDRESS: "张庄镇附近", JWD: "113.26665,32.55689", BASTYPE: "炼焦", AREA: "金水区", LEGALOR: "李四", GZCD: "国控" },
        ];

        var companyInfoTableRow =
            "<tr>" +
            "<td style='background-color: #E4E4E4;border: 1px solid #D4D4D4;width:100px;height:25px;'>归属行业</td><td style='border: 1px solid #D4D4D4;width:175px;height:25px;'>BASTYPE</td>" +
            "<td style='background-color: #E4E4E4;border: 1px solid #D4D4D4;width:100px;height:25px;'>归属区域</td><td style='border: 1px solid #D4D4D4;width:175px;height:25px;'>AREA</td>" +
            "<td style='background-color: #E4E4E4;border: 1px solid #D4D4D4;width:100px;height:25px;'>详细地址</td><td style='border: 1px solid #D4D4D4;width:175px;height:25px;'><div style='width:175px;height:20px;padding-top:5px;' class='zxx_text_overflow_6'>ADDRESS</div></td>" +
            "</tr>" +
            "<tr>" +
            "<td style='background-color: #E4E4E4;border: 1px solid #D4D4D4;width:100px;height:25px;'>经纬度</td><td style='border: 1px solid #D4D4D4;width:175px;height:25px;'>JWD</td>" +
            "<td style='background-color: #E4E4E4;border: 1px solid #D4D4D4; width:100px; height:25px;'>企业负责人</td><td style='border: 1px solid #D4D4D4;width:175px;height:25px;'>LEGALOR</td>" +
            "<td style='background-color: #E4E4E4;border: 1px solid #D4D4D4; width:100px; height:25px;'>关注程度</td><td style='border: 1px solid #D4D4D4;width:175px;height:25px;'>GZCD</td>" +
            "</tr>";
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style='width: 582px; height: 432px;'>
        <div style='width: 581px; height: 388px;'>
            <div style='width: 580px; height: 180px; float: left;'>
                <div style='border: 1px solid #D4D4D5; width: 100%; height: 32px;'>
                    <div style='width: 100%; height: 28px; background-color: #0099FF; text-align: center; padding-top: 5px;'>
                        <span id="companyName" style='color: white; font-size: 18px;'></span><span id="companyStatus" style='color: white; font-size: 18px;'></span>
                    </div>
                </div>
                <div style='width: 98%; height: 70px; padding: 10px;'>
                    <table id="CompanyInfoDataTable" style='text-align: center; font-size: 13px; border: 1px solid #D4D4D4; border-collapse: collapse;'>
                        <thead></thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
                <div id="PKTabs" class="easyui-tabs" style="width: 580px; height: 250px;">
                </div>
            </div>
        </div>
        <div style='width: 579px; height: 40px; float: left; border: 1px solid #D4D4D5;'>
            <div style='margin: 5px 0px 0px 15px;'>
                <div style='float: left; cursor: pointer; color: white; background-color: #0099FF; width: 100px; height: 23px; font-size: 13px; text-align: center; padding-top: 8px; margin-left: 100px;' onclick="dealAlarm()">告警处置</div>
                <div style='float: left; cursor: pointer; color: white; background-color: #0099FF; width: 100px; height: 23px; font-size: 13px; text-align: center; padding-top: 8px; margin-left: 20px;' onclick="hisDataAnalysis()">历史数据分析</div>
                <div style='float: left; cursor: pointer; color: white; background-color: #0099FF; width: 100px; height: 23px; font-size: 13px; text-align: center; padding-top: 8px; margin-left: 20px;' onclick="realVideo()">实时视频</div>
            </div>
        </div>
    </div>
</asp:Content>
