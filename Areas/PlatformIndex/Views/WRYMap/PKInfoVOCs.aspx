<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script src="/Content/js/getQueryStr.js"></script>
    <style>
         * {
            margin:0;
            padding:0;
        }

        table tr td {
            border: 1px solid #D4D4D4;
            height:25px;
        }
    </style>
    <script>
        var pkMonitorDatas;  //企业信息数据
        var PKID;  //排口ID
        var type;  //排口种类(1-废气、2-废水、3-VOCs)

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
            PKID = getQueryStr("PKID");
            type = getQueryStr("type");
            initPageDatas();  //加载页面所需数据
            LoadAllTables(); //加载页面表格
            wordLimit();
        };

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
        function showTrackPath() {
            window.parent.showTrackPath(getQueryStr("taskID"), getQueryStr("userName"));
        }

        //历史数据分析 
        function beginCheck() {
            window.parent.beginCheck(getQueryStr("taskID"), getQueryStr("userName"), $("#companyName").text(), "2");
        }

        //实时视频 
        function beginTask() {
            window.parent.beginTask(getQueryStr("taskID"), getQueryStr("userName"), $("#companyName").text(), getQueryStr("lng"), getQueryStr("lat"));
        }

        //查看抽查、巡查照片 type 1-抽查 2-巡查
        function showChouChaPhotos(id, type) {
            window.parent.showChouChaPhotos(getQueryStr("userName"), id, type);
        }

        //加载页面表格数据
        function LoadAllTables() {
            LoadPKInfoDataTable();
            //LoadChouChaDataTable();
            //LoadXunChaDataTable();
        }
        
        //初始化页面数据
        function initPageDatas() {
            pkMonitorDatas = GetPKMonitorDatas();  //加载企业信息数据
            //chouChaDatas = GetCompanyChouChaDatas(userName, taskID);  //加载人员抽查数据
            //xunChaDatas = GetCompanyXunChaDatas(userName, taskID);  //加载人员抽查数据
        }

        //加载企业信息数据表格
        function LoadPKInfoDataTable() {
            if (pkMonitorDatas.length > 1) {
                pkMonitorData = pkMonitorDatas[0];
            }
            else {
                return;
            }

            $("#PKInfoDataTable tbody").empty(); //清空table
            $("#PFInfoDataTable tbody").empty(); //清空table

            //$("#companyName").text(pkMonitorData.NAME);
            //$("#companyStatus").text("(" + pkMonitorData.STATUS + ")");
            //$("#updateTime").text(getNowFormatDate());  //更新时间
            var trHtml = pkInfoTableRow.replace("NAME", pkMonitorData.NAME).replace("DATA_TIME", pkMonitorData.DATA_TIME)
                    .replace("flow_vocs_Value", pkMonitorData.flowvocs_Value);
            $("#PKInfoDataTable tbody").append(trHtml);

            var trPFHtml = pfInfoTableRow.replace("a25002_ND_Value", pkMonitorData.a25002_ND_Value).replace("a25003_ND_Value", pkMonitorData.a25003_ND_Value)
                            .replace("a25005_ND_Value", pkMonitorData.a25005_ND_Value).replace("a24088_ND_Value", pkMonitorData.a24088_ND_Value)
                            .replace("a25002_PFL_Value", pkMonitorData.a25002_PFL_Value).replace("a25003_PFL_Value", pkMonitorData.a25003_PFL_Value)
                            .replace("a25005_PFL_Value", pkMonitorData.a25005_PFL_Value).replace("a24088_PFL_Value", pkMonitorData.a24088_PFL_Value);
            $("#PFInfoDataTable tbody").append(trPFHtml);
        }

        //加载企业信息数据
        function GetPKMonitorDatas() {
            var arr = [];
            $.ajax({
                type: "post",
                async: false, //同步执行
                url: "/PlatformIndex/WRYMap/GetPKMonitorDatas?PKID=" + PKID + "&type=" + type,
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
        var pkMonitorDatas = [
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

        var pkInfoTableRow =
            "<tr>" +
            //"<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>排口编号</td><td style='border: 1px solid #D4D4D4;width:175px;height:25px;'>CODE</td>" +
            "<td style='border: 1px solid #D4D4D4;width:70px;height:25px;'>排口名称</td><td style='border: 1px solid #D4D4D4;width:120px;height:25px;'><div style='width:120px;height:20px;padding-top:5px;' class='zxx_text_overflow_6'>NAME</div></td>" +
            "<td style='border: 1px solid #D4D4D4;width:70px;height:25px;'>上传时间</td><td style='border: 1px solid #D4D4D4;width:130px;height:25px;'><div style='width:130px;height:20px;padding-top:5px;' class='zxx_text_overflow_6'>DATA_TIME</div></td>" +
            "<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>流量（立方米）</td><td style='border: 1px solid #D4D4D4;width:60px;height:25px;'>flow_vocs_Value</td>" +
            "</tr>" +
            "<tr>";
            //+
            //"<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>流量（立方米）</td><td rowspan='5' style='border: 1px solid #D4D4D4;width:175px;height:25px;'>flow_vocs_Value</td>" +
            ////"<td style='border: 1px solid #D4D4D4; width:100px; height:25px;'>含氧量（百分比）</td><td style='border: 1px solid #D4D4D4;width:175px;height:25px;'>LEGALOR</td>" +
            ////"<td style='border: 1px solid #D4D4D4; width:100px; height:25px;'>关注程度</td><td style='border: 1px solid #D4D4D4;width:175px;height:25px;'>GZCD</td>" +
            //"</tr>";

        var pfInfoTableRow =
           "<tr>" +
           "<td style='border: 1px solid #D4D4D4;width:130px;height:25px;'>污染物因子</td>" +
           "<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>苯</td>" +
           "<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>甲苯</td>" +
           "<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>二甲苯</td>" +
           "<td style='border: 1px solid #D4D4D4;width:121px;height:25px;'>非甲烷总烃</td>" +
           "</tr>" +
           "<tr>" +
           "<td style='border: 1px solid #D4D4D4;width:130px;height:25px;'>浓度（毫克/升）：</td>" +
           "<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>a25002_ND_Value</td>" +
           "<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>a25003_ND_Value</td>" +
           "<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>a25005_ND_Value</td>" +
           "<td style='border: 1px solid #D4D4D4;width:121px;height:25px;'>a24088_ND_Value</td>" +
           "</tr>" +
           "<tr>" +
            "<td style='border: 1px solid #D4D4D4;width:130px;height:25px;'>排放量（千克）：</td>" +
            "<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>a25002_PFL_Value</td>" +
            "<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>a25003_PFL_Value</td>" +
            "<td style='border: 1px solid #D4D4D4;width:100px;height:25px;'>a25005_PFL_Value</td>" +
            "<td style='border: 1px solid #D4D4D4;width:121px;height:25px;'>a24088_PFL_Value</td>" +
            "</tr>";
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style='width:582px;height:432px;'>
        <div style='width:581px;height:388px;'>
            <div style='width:580px;height:180px;float:left;'>
                <div style='width:98%;height:70px;padding:10px;'>
                    <table id="PKInfoDataTable"style='text-align:center;font-size:13px;border: 1px solid #D4D4D4;border-collapse: collapse;'>
                        <thead></thead>
                        <tbody>
                        </tbody>
                    </table>
                    <table id="PFInfoDataTable" style='text-align:center;font-size:13px;border: 1px solid #D4D4D4;border-collapse: collapse;margin-top:10px;'>
                        <thead></thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
