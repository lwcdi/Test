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
    public partial class VocsPFLWarn : BaseReport
    {
        protected override void BandingReport()
        {

            //定义报表参数对象
            List<ReportParameter> parameterList = new List<ReportParameter>();
            var paramDic = RdlcParam(Request.QueryString["rdlcType"]);
            foreach (var key in paramDic.Keys)
            {
                parameterList.Add(new ReportParameter(key, paramDic[key]));
            }
            string ReportPath = "Areas\\Report\\Template\\PFLWarn.rdlc";
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportViewer1.LocalReport.ReportEmbeddedResource = "Tablix1";
            ReportViewer1.LocalReport.SetParameters(parameterList);

            DataTable table = MonitorData.GetPK_PFL_Warn(SerializerHelper.Deserialize<List<string>>(Request.QueryString["pkID"]), startTime, endTime, MonitorData.VOCS_MONITOR_ITEM_TYPE, Request.QueryString["rdlcType"]);
            //DataTable table = MonitorData.GetPollutionWarnData(startTime, endTime, Request.QueryString["type"], Request.QueryString["id"], dataType);
            //DataHandle.TableDataStyle(table, "DATA_TIME", Request.QueryString["rdlcType"]);
            ReportDataSource rds = new ReportDataSource("PFLWarn", table);//指定数据源GetTestDataTable()
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
                    paramDic.Add("ReportTitle", "固定源挥发性有机物排放连续总量预警报表");
                    paramDic.Add("CheckTime", string.Format("监测时间：{0} 至 {1}", startTime.ToString("dd日"), endTime.ToString("dd日")));
                    break;
                case "day":
                    paramDic.Add("ReportTitle", "固定源挥发性有机物排放连续总量预警日报表");
                    paramDic.Add("CheckTime", string.Format("监测日期：{0}", DateTime.Parse(Request.QueryString["timeRangDay"]).ToString("yyyy年MM月dd日")));
                    break;
                case "month":
                    paramDic.Add("ReportTitle", "固定源挥发性有机物排放连续总量预警月报表");
                    paramDic.Add("CheckTime", string.Format("监测月份：{0}年{1}月", Request.QueryString["timeRangYear"], Request.QueryString["timeRangMonth"]));
                    break;
                case "year":
                    paramDic.Add("ReportTitle", "固定源挥发性有机物排放连续总量预警年报表");
                    paramDic.Add("CheckTime", string.Format("监测年份：{0}", string.Format("{0}年", Request.QueryString["timeRangYear"])));
                    break;
            }
            return paramDic;
        }
    }
}