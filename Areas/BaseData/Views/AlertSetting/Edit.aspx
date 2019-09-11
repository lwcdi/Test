<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .Bar {
            width: 99%;
            height: 30px;
            line-height: 30px;
            color: #999999;
            background-color: #f2f2f2;
            border: 1px solid #e4e4e4;
            margin: 5px 1px 5px 1px;
            padding-left: 5px;
        }

        table {
            font-size: 12px;
            table-layout: fixed;
            empty-cells: show;
            border-collapse: collapse;
            margin: 0 auto;
            width: 100%;
        }

        td {
            height: 30px;
        }

            td input {
                width: 80px;
            }

        table.t1 {
            border: 1px solid #cad9ea;
            color: #666;
        }

            table.t1 th {
                background-color: #e4e4e4;
                height: 30px;
            }

            table.t1 td, table.t1 th {
                border: 1px solid #cad9ea;
                padding: 0 1em 0;
            }

            table.t1 tr.a1 {
                background-color: #f5fafe;
            }



        table.t2 {
            border: 1px solid #9db3c5;
            color: #666;
        }

            table.t2 th {
                background-image: url(th_bg2.gif);
                background-repeat: repeat-x;
                height: 30px;
                color: #fff;
            }

            table.t2 td {
                border: 1px dotted #cad9ea;
                padding: 0 2px 0;
            }

            table.t2 th {
                border: 1px solid #a7d1fd;
                padding: 0 2px 0;
            }

            table.t2 tr.a1 {
                background-color: #e8f3fd;
            }



        table.t3 {
            border: 1px solid #fc58a6;
            color: #720337;
        }

            table.t3 th {
                background-image: url(th_bg3.gif);
                background-repeat: repeat-x;
                height: 30px;
                color: #35031b;
            }

            table.t3 td {
                border: 1px dashed #feb8d9;
                padding: 0 1.5em 0;
            }

            table.t3 th {
                border: 1px solid #b9f9dc;
                padding: 0 2px 0;
            }

            table.t3 tr.a1 {
                background-color: #fbd8e8;
            }
    </style>
    <link type="text/css" rel="stylesheet" href="/Content/css/Checkbox.css" />
    <div class="Bar">
        企业信息<div style="float: right;">
            <input type="checkbox" id="checkbox_cas" />告警状态：<label for="checkbox_cas"></label>
            <input type="hidden" id="CASID" />
        </div>
    </div>
    <table border="1" class="t1 cas">
        <tr>
            <th style="width: 30px;">序号</th>
            <th>告警限值</th>
            <th>限值</th>
        </tr>
        <tr>
            <td>1</td>
            <td>设备离线</td>
            <td>
                <input type="text" id="OFFLINETIME" value="" />
                分钟</td>
        </tr>
        <tr>
            <td>2</td>
            <td>采集因子项限值超标持续时间</td>
            <td>
                <input type="text" id="OVERTIME" value="" />
                分钟</td>
        </tr>
        <tr>
            <td>3</td>
            <td>数据缺失</td>
            <td>
                <input type="text" id="NODATETIME" value="" />
                分钟</td>
        </tr>
        <tr>
            <td>4</td>
            <td>设备故障超过</td>
            <td>
                <input type="text" id="FAULTTIME" value="" />
                小时</td>
        </tr>
    </table>
    <div class="Bar">限值配置</div>
    <div id="companyTab" fit="true" style="height: 618px; overflow: hidden;">
        <%--        <div title="排口一" style="padding: 2px; overflow: hidden; height: 600px;">
            <table border="1" class="t1">
                <tr>
                    <th style="width: 30px;">序号</th>
                    <th style="width: 100px;">因子</th>
                    <th>浓度（告警限值）</th>
                    <th>日排放量（告警限值）</th>
                    <th>月排放量（报表限值）</th>
                    <th>年排放量（报表限值）</th>
                    <th style="width: 60px;">告警状态</th>
                </tr>
                <tr>
                    <td>1</td>
                    <td>总烃<input type="hidden" value="so2" /></td>
                    <td>
                        <input type="text" value="" />
                        <span>千克</span>
                    </td>
                    <td>
                        <input type="text" value="" /></td>
                    <td>
                        <input type="text" value="" /></td>
                    <td>
                        <input type="text" value="" /></td>
                    <td>
                        <input type="checkbox" id="checkbox_fs1" checked /><label for="checkbox_fs1"></label></td>
                </tr>
            </table>
        </div>--%>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Content/js/getQueryStr.js"></script>
    <script>
        var companyid = getQueryStr("id");
        if (companyid) {
            $(function () {
                init();
                //initIframe();
                //var isSelect1 = false;
                //var isSelect2 = false;
                //$("#companyTab").tabs({
                //    onSelect: function (title, index) {
                //        if (!isSelect1 && index == 1) {
                //            isSelect1 = true;
                //            initIframePK();
                //        }
                //        if (!isSelect2 && index == 2) {
                //            isSelect2 = true;
                //            initIframeDevice();
                //        }
                //    }
                //});
            });
        }
        //$(function () {
        //    initIframe();
        //    var isSelect1 = false;
        //    var isSelect2 = false;
        //    $("#companyTab").tabs({
        //        onSelect: function (title, index) {
        //            if (!isSelect1 && index == 1) {
        //                isSelect1 = true;
        //                initIframePK();
        //            }
        //            if (!isSelect2 && index == 2) {
        //                isSelect2 = true;
        //                initIframeDevice();
        //            }
        //        }
        //    });
        //});

        function initIframe() {
            $("#baseInfoIF").attr("src", "/BaseData/VOCsCompany/CompanyBaseInfo?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs);
            //$("#PKInfoIF").attr("src", "/BaseData/VOCsCompany/CompanyPKInfo?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs);
            //$("#deviceInfoIF").attr("src", "/BaseData/VOCsCompany/CompanyDeviceInfo?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs);
        }

        function initIframePK() {
            $("#PKInfoIF").attr("src", "/BaseData/VOCsCompany/CompanyPKInfo?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs);
        }
        function initIframeDevice() {
            $("#deviceInfoIF").attr("src", "/BaseData/VOCsCompany/CompanyDeviceInfo?id=" + id + "&showType=" + showType + "&isVOCs=" + isVOCs);
        }
        //function showContactInfo(id, showType, isVOCs, row) {
        //    window.parent.showContactInfo(id, showType, isVOCs, row);
        //}

        function saveCompanyData() {
            childBaseInfo.saveCompanyData();
        }

        function init() {
            GetAlertSetting(companyid);
            SetInput();
        }

        function SetInput() {
            var inputs = $("table input");
            inputs.attr("placeholder", "请输入数字");
            inputs.attr("maxlength", "10");
            inputs.attr("ng-pattern", "/[^a-zA-Z]/");
            inputs.css("ime-mode", "disabled");
            //通过onkeypress事件是输不上任何非数字字符
            //<input type="text" onkeypress="return event.keyCode>=48&&event.keyCode<=57" ng-pattern="/[^a-zA-Z]/" /> 
            inputs.attr("onkeypress", "return event.keyCode>=48&&event.keyCode<=57");
            //通过onkeyup事件是输上后再去掉非数字字符 
            //inputs.attr("onkeyup", "value=value.replace(/[^\d]/g,'')");
            //<input type="text" onkeyup="value=value.replace(/[^\d]/g,'') " ng-pattern="/[^a-zA-Z]/">

            //inputs.attr("onpaste", "return false");
            //inputs.attr("ondrop", "return false");
            inputs.change(function () {
                if (/[^\d]/g.test(this.value)) { alert('只能输入数字'); this.value = this.value.replace(/[^\d]/g, ''); }
            });
        }

        //设置企业告警信息
        function SetCAS(cas) {
            if (cas && 0 < cas.length) {
                cas = cas[0];
                $("#CASID").val(cas.ID);
                $(".cas #OFFLINETIME").val(cas.OFFLINETIME);
                $(".cas #OVERTIME").val(cas.OVERTIME);
                $(".cas #NODATETIME").val(cas.NODATETIME);
                $(".cas #FAULTTIME").val(cas.FAULTTIME);
                $("#checkbox_cas").prop("checked", 1 == cas.STATE);
            }
        }

        //设置因子告警信息
        function SetCASI(casi) {

            var tabhtm = '<div id="tab_{PKID}" title="{NAME}" style="padding: 2px; overflow: hidden; height: 600px;">' +
                '<table border="1" class="t1"><tr><th style="width: 30px;">序号</th><th style="width: 100px;">因子</th><th>浓度（告警限值）</th>' +
                '<th>日排放量（告警限值）</th><th>月排放量（报表限值）</th><th>年排放量（报表限值）</th><th style="width: 60px;">告警状态</th>' +
                '</tr>{0}</table></div>';
            var trhtm = '<tr class="casi">' +
                    '<td>{INDEX}</td>' +
                    '<td>{ITEMNAME}<input class="casi-data" type="hidden" data-COMPANYID="{COMPANYID}" data-PKID="{PKID}" data-ITEMCODE="{ITEM}" data-ND="{ND}" data-PFL="{PFL}" data-FLOWID="{FLOWID}" data-NDID="{NDID}" data-PFLID="{PFLID}" /></td>' +
                    '<td><input class="casi-LIMITC" type="text" value="{LIMITC}" style="display:{DISPLAY};" /> <span>{NDUNIT}</span></td>' +
                    '<td><input class="casi-LIMITDAY" type="text" value="{LIMITDAY}" /> <span>{UNIT}</span></td>' +
                    '<td><input class="casi-LIMITMONTH" type="text" value="{LIMITMONTH}" /> <span>{UNIT}</span></td>' +
                    '<td><input class="casi-LIMITYEAR" type="text" value="{LIMITYEAR}" /> <span>{UNIT}</span></td>' +
                    '<td><input class="casi-STATE" type="checkbox" id="checkbox_STATE_{PKID}_{ITEM}" {CHECKED} /><label for="checkbox_STATE_{PKID}_{ITEM}"></label></td>' +
                    '</tr>';
            if (casi && 0 < casi.length) {
                var pkid = "";
                var index = 0;
                var tabs = [];
                var trs = [];
                for (var i = 0; i < casi.length; i++) {
                    var c = casi[i];
                    if (pkid != c.PKID) {
                        if (0 < tabs.length)
                            tabs[tabs.length - 1] = tabs[tabs.length - 1].format(trs.join("\n"));
                        tabs.push(tabhtm.format(c));
                        trs = [];
                        index = 0;
                        pkid = c.PKID;
                    }
                    c.INDEX = ++index;
                    c.UNIT = c.PFLUNIT || c.FLOWUNIT;
                    c.CHECKED = 1 == c.STATE ? "checked" : "";
                    c.DISPLAY = c.ND ? "unset" : "none";
                    trs.push(trhtm.format(c));
                }
                tabs[tabs.length - 1] = tabs[tabs.length - 1].format(trs.join("\n"));

                $("#companyTab").empty();
                $("#companyTab").append(tabs.join("\n"));
            }

            $("#companyTab").tabs({
                onSelect: function (title, index) {
                    //alert(title);
                }
            });
        }

        //获取企业告警设置
        function GetAlertSetting(companyid) {
            com.ajax({
                url: "/BaseData/AlertSetting/GetAlertSetting",
                data: { CompanyID: companyid },
                async: false,
                success: function (result) {
                    if (result.success == true) {
                        debugger;
                        SetCAS(result.data.cas);
                        SetCASI(result.data.casi);
                    }
                }
            });
        }

        //
        function SaveSetting() {
            debugger;
            var cas = {
                ID: $("#CASID").val() || 0,
                COMPANYID: companyid,
                OFFLINETIME: $(".cas #OFFLINETIME").val() || null,
                OVERTIME: $(".cas #OVERTIME").val() || null,
                NODATETIME: $(".cas #NODATETIME").val() || null,
                FAULTTIME: $(".cas #FAULTTIME").val() || null,
                STATE: $("#checkbox_cas").prop("checked") ? 1 : 0
            };
            var casi = [];
            var trs = $("#companyTab .casi");
            for (var i = 0; i < trs.length; i++) {
                var tr = $(trs[i]);
                var data = tr.find(".casi-data").data();
                if (data.nd || data.pfl) {
                    if (data.nd) {
                        var si = {
                            ID: data.ndid || 0,
                            COMPANYID: data.companyid,
                            PKID: data.pkid,
                            ITEMCODE: data.itemcode,
                            SUBITEMCODE: data.nd,
                            LIMITC: tr.find(".casi-LIMITC").val() || null,
                            LIMITDAY: null,
                            LIMITMONTH: null,
                            LIMITYEAR: null,
                            STATE: tr.find(".casi-STATE").prop("checked") ? 1 : 0
                        };
                        casi.push(si);
                    }
                    if (data.pfl) {
                        var si = {
                            ID: data.pflid || 0,
                            COMPANYID: data.companyid,
                            PKID: data.pkid,
                            ITEMCODE: data.itemcode,
                            SUBITEMCODE: data.pfl,
                            LIMITC: null,
                            LIMITDAY: tr.find(".casi-LIMITDAY").val() || null,
                            LIMITMONTH: tr.find(".casi-LIMITMONTH").val() || null,
                            LIMITYEAR: tr.find(".casi-LIMITYEAR").val() || null,
                            STATE: tr.find(".casi-STATE").prop("checked") ? 1 : 0
                        };
                        casi.push(si);
                    }
                }
                else {
                    var si = {
                        ID: data.flowid || 0,
                        COMPANYID: data.companyid,
                        PKID: data.pkid,
                        ITEMCODE: data.itemcode,
                        SUBITEMCODE: null,
                        LIMITC: null,
                        LIMITDAY: tr.find(".casi-LIMITDAY").val() || null,
                        LIMITMONTH: tr.find(".casi-LIMITMONTH").val() || null,
                        LIMITYEAR: tr.find(".casi-LIMITYEAR").val() || null,
                        STATE: tr.find(".casi-STATE").prop("checked") ? 1 : 0
                    };
                    casi.push(si);
                }
            }

            com.ajax({
                url: "/BaseData/AlertSetting/SaveAlertSetting",
                data: { data: JSON.stringify({ cas: cas, casi: casi }) },
                async: false,
                success: function (result) {
                    com.msg.alert(result.data);
                }
            });
        }

    </script>
</asp:Content>
