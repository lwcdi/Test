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
    public partial class AirDay : BaseReport
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

            string ReportPath = "Areas\\Report\\Template\\AirHour.rdlc";
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportViewer1.LocalReport.ReportEmbeddedResource = "Tablix1";
            ReportViewer1.LocalReport.SetParameters(parameterList);

            DataTable table = MonitorData.GetMonitorDataForReportAir(Request.QueryString["dataFrom"], startTime, endTime, Request.QueryString["type"], Request.QueryString["id"], "hour");
            DataHandle.TableDataStyle(table, "DATA_TIME", Request.QueryString["rdlcType"]);
            ReportDataSource rds = new ReportDataSource("AirHour", table);//指定数据源GetTestDataTable()
            //ReportDataSource rds = new ReportDataSource("AirHour", DataExchange(listData));//指定数据源GetTestDataTable()
            ReportViewer1.LocalReport.DataSources.Clear();//报表数据源清除，必须
            ReportViewer1.LocalReport.DataSources.Add(rds);//报表数据源增加
            ReportViewer1.LocalReport.Refresh();//刷新报表 
        }
        private Dictionary<string, string> RdlcParam()
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("Title", "烟气排放连续监测日平均值日报表");
            paramDic.Add("CheckTime", string.Format("监测日期：{0}", DateTime.Parse(Request.QueryString["timeRangDay"]).ToString("yyyy年MM月dd日")));
            paramDic.Add("SumUnit", "烟气日排放总量单位：×10⁴m³/d");
            paramDic.Add("PflUnit", "1");
            paramDic.Add("pflUnitText", "排放量kg/h");
            paramDic.Add("pflSumTitle", "日排放总量(t)");
            return paramDic;
        }
    }
}