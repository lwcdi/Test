<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WaterYear.aspx.cs" Inherits="UI.Web.Areas.Report.Aspx.WaterYear" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
             <div>
                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="100%" SizeToReportContent="true" HyperlinkTarget="_self" CssClass="ReportViewer"  KeepSessionAlive="false">
                        <LocalReport EnableHyperlinks="false"></LocalReport>
                </rsweb:ReportViewer>
            </div>
    </div>
    </form>
</body>
</html>
