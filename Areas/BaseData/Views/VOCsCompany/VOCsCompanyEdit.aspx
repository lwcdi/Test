<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="companyTab" fit="true" style="height: 618px; overflow: hidden;">
        <div title="基本信息" style="padding: 2px; overflow: hidden;height: 600px;">
            <iframe name="childBaseInfo" id="baseInfoIF" style="border: 0px; width: 100%; height: 600px; overflow: auto;"></iframe>
        </div>
        <div title="排口信息" style="padding: 2px; overflow: hidden;height: 600px;">
            <iframe name="childPKInfo" id="PKInfoIF" style="border: 0px; width: 100%; height: 600px; overflow: auto;"></iframe>
        </div>
        <div title="设备信息" style="padding: 2px; overflow: hidden;height: 600px;">
            <iframe name="childDeviceInfo" id="deviceInfoIF" style="border: 0px; width: 100%; height: 600px; overflow: auto;"></iframe>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="/Content/js/getQueryStr.js"></script>
    <script src="/Content/js/BaseData/CompanyInfo.js"></script>
    <script>
        $(function () {
            initIframe();
            var isSelect1 = false;
            var isSelect2 = false;
            $("#companyTab").tabs({
                onSelect: function (title, index) {
                    if (!isSelect1 && index == 1) {
                        isSelect1 = true;
                        initIframePK();
                    }
                    if (!isSelect2 && index == 2) {
                        isSelect2 = true;
                        initIframeDevice();
                    }
                }
            });
        });

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
    </script>
</asp:Content>
