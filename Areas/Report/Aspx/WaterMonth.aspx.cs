using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.Report.Aspx
{
    public partial class WaterMonth : BaseReport
    {
        protected override void BandingReport()
        {
            //定义报表参数对象
            List<ReportParameter> parameterList = new List<ReportParameter>();

            Dictionary<string, string> paramDic = RdlcParam();
            foreach (var key in paramDic.Keys)
            {
                parameterList.Add(new ReportParameter(key, paramDic[key]));
            }
            parameterList.Add(new ReportParameter("PollutionName", PollutionName));
            parameterList.Add(new ReportParameter("PollutionNo", PollutionNo));

            string ReportPath = "Areas\\Report\\Template\\WaterHour.rdlc";
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportViewer1.LocalReport.ReportEmbeddedResource = "Tablix1";
            ReportViewer1.LocalReport.SetParameters(parameterList);

            DataTable table = MonitorData.GetMonitorDataForReportWater(Request.QueryString["dataFrom"], startTime, endTime, Request.QueryString["type"], Request.QueryString["id"], "day");
            DataHandle.TableDataStyle(table, "DATA_TIME", Request.QueryString["rdlcType"]);
            ReportDataSource rds = new ReportDataSource("WaterMonitor", table);//指定数据源GetTestDataTable()
            ReportViewer1.LocalReport.DataSources.Clear();//报表数据源清除，必须
            ReportViewer1.LocalReport.DataSources.Add(rds);//报表数据源增加
            ReportViewer1.LocalReport.Refresh();//刷新报表 
        }
        private Dictionary<string, string> RdlcParam()
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("Title", "废水排放连续监测日平均值月报表");
            paramDic.Add("CheckTime", string.Format("监测月份：{0}年{1}月", Request.QueryString["timeRangYear"], Request.QueryString["timeRangMonth"]));
            paramDic.Add("SumUnit", "废水月排放总量单位：×10⁴m³/d");
            paramDic.Add("PflUnit", "1");
            paramDic.Add("pflUnitText", "排放量kg/h");
            return paramDic;
        }
    }
}