using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UI.Web.Areas.Report.Aspx
{
    public partial class PollutionReportingRate : System.Web.UI.Page
    {
        DateTime startTime;
        DateTime endTime;
        string dataType = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RequestParamHandle();
                BandingReport();
            }
        }
        private  void BandingReport()
        {
            List<string> pkIDList = SerializerHelper.Deserialize<List<string>>(Request.QueryString["PK_ID"]);
            //定义报表参数对象
            List<ReportParameter> parameterList = new List<ReportParameter>();
            var paramDic = RdlcParam(Request.QueryString["reportType"]);
            foreach (var key in paramDic.Keys)
            {
                parameterList.Add(new ReportParameter(key, paramDic[key]));
            }
            string ReportPath = "Areas\\Report\\Template\\ReportingRate.rdlc";
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportViewer1.LocalReport.ReportEmbeddedResource = "Tablix1";
            ReportViewer1.LocalReport.SetParameters(parameterList);

            DataTable table = MonitorData.PK_PollutionReportRate(pkIDList, startTime, endTime, Request.QueryString["dataFrom"], dataType);
            ReportDataSource rds = new ReportDataSource("ReportingRate", table);//指定数据源GetTestDataTable()
            ReportViewer1.LocalReport.DataSources.Clear();//报表数据源清除，必须
            ReportViewer1.LocalReport.DataSources.Add(rds);//报表数据源增加
            ReportViewer1.LocalReport.Refresh();//刷新报表 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">day month year custom</param>
        /// <returns></returns>
        private Dictionary<string, string> RdlcParam(string type)
        {

            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            switch (type)
            {
                case "custom":
                    paramDic.Add("ReportTitle", "污染源排放连续监测数据有效率报表");
                    break;
                case "day":
                    paramDic.Add("ReportTitle", "污染源排放连续监测数据有效率日报表");
                    break;
                case "month":
                    paramDic.Add("ReportTitle", "污染源排放连续监测数据有效率月报表");
                    break;
                case "year":
                    paramDic.Add("ReportTitle", "污染源排放连续监测数据有效率年报表");
                    break;
            }
            return paramDic;
        }
        private void RequestParamHandle()
        {
            switch (Request.QueryString["reportType"])
            {
                case "custom":
                    startTime = DateTime.Parse(Request.QueryString["timeRangStart"]);
                    endTime = DateTime.Parse(Request.QueryString["timeRangEnd"]).AddDays(1).AddSeconds(-1);
                    dataType = "day";
                    break;
                case "day":
                    startTime = DateTime.Parse(Request.QueryString["timeRangDay"]);
                    endTime = startTime.AddDays(1).AddSeconds(-1);
                    dataType = "hour";
                    break;                    
                case "month":
                    startTime = new DateTime(StringHelper.DynamicToInt(Request.QueryString["timeRangYear"]), StringHelper.DynamicToInt(Request.QueryString["timeRangMonth"]), 1);
                    endTime = startTime.AddMonths(1).AddSeconds(-1);
                    dataType = "day";
                    break;
                case "year":
                    startTime = new DateTime(StringHelper.DynamicToInt(Request.QueryString["timeRangYear"]), 1, 1);
                    endTime = startTime.AddYears(1).AddSeconds(-1);
                    dataType = "day";
                    break;
            }
        }
    }
}