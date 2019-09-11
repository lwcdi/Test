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
    public partial class AirCustom : BaseReport
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
            ReportViewer1.LocalReport.DataSources.Clear();//报表数据源清除，必须
            ReportViewer1.LocalReport.DataSources.Add(rds);//报表数据源增加
            ReportViewer1.LocalReport.Refresh();//刷新报表 
        }
        private Dictionary<string, string> RdlcParam()
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("Title", "烟气排放连续监测平均值报表");
            paramDic.Add("CheckTime", string.Format("监测时间：{0} 至 {1}", startTime.ToString("dd日HH时"), endTime.ToString("dd日HH时")));
            paramDic.Add("SumUnit", "烟气排放总量单位：×10⁴m³/d");
            paramDic.Add("PflUnit", "1");
            paramDic.Add("pflUnitText", "排放量kg/h");
            paramDic.Add("pflSumTitle", "排放总量(t)");
            return paramDic;
        }
    }
}