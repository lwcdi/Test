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
    public partial class VocsDay : BaseReport
    {
        protected override void BandingReport()
        {

            //定义报表参数对象
            List<ReportParameter> parameterList = new List<ReportParameter>();
            var paramDic = RdlcParam(Request.QueryString["rdlcType"]);
            foreach(var key in paramDic.Keys)
            {
                parameterList.Add(new ReportParameter(key, paramDic[key]));
            }
            dynamic companyInfo = BaseCommonInfo.GetCompanyOrPK(Request.QueryString["type"], Request.QueryString["id"]);
            parameterList.Add(new ReportParameter("PollutionName", companyInfo.Name));
            parameterList.Add(new ReportParameter("PollutionNo", companyInfo.Code));


            string ReportPath = "Areas\\Report\\Template\\VocsHour.rdlc";
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportViewer1.LocalReport.ReportEmbeddedResource = "Tablix1";
            ReportViewer1.LocalReport.SetParameters(parameterList);

            DataTable table = MonitorData.GetMonitorDataForReportVocs(Request.QueryString["dataFrom"], startTime, endTime, Request.QueryString["type"], Request.QueryString["id"], dataType);
            DataHandle.TableDataStyle(table, "DATA_TIME", Request.QueryString["rdlcType"]);
            ReportDataSource rds = new ReportDataSource("VocsMonitor", table);//指定数据源GetTestDataTable()
            ReportViewer1.LocalReport.DataSources.Clear();//报表数据源清除，必须
            ReportViewer1.LocalReport.DataSources.Add(rds);//报表数据源增加
            ReportViewer1.LocalReport.Refresh();//刷新报表 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">day month year custom</param>
        /// <returns></returns>
        private Dictionary<string,string> RdlcParam(string type)
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            switch (type)
            {
                case "custom":
                    paramDic.Add("Title", "固定源挥发性有机物排放连续监测小时平均值报表");
                    paramDic.Add("CheckTime", " ");
                    paramDic.Add("SumUnit", "排放总量单位：×10⁴m³/d");
                    paramDic.Add("PflUnit", "1");
                    paramDic.Add("pflUnitText", "排放量kg/h");
                    paramDic.Add("plfSumTitle", "排放总量");
                    break;
                case "day":
                    paramDic.Add("Title", "固定源挥发性有机物排放连续监测小时平均值日报表");
                    paramDic.Add("CheckTime", string.Format("监测日期{0}", DateTime.Parse(Request.QueryString["timeRangDay"]).ToString("yyyy年MM月dd日")));
                    paramDic.Add("SumUnit", "日排放总量单位：×10⁴m³/d");
                    paramDic.Add("PflUnit", "1");
                    paramDic.Add("pflUnitText", "排放量kg/h");
                    paramDic.Add("plfSumTitle", "日排放总量");
                    break;
                case "month":
                    paramDic.Add("Title", "固定源挥发性有机物排放连续监测日平均值月报表");
                    paramDic.Add("CheckTime", string.Format("监测月份{0}年{1}月", Request.QueryString["timeRangYear"], Request.QueryString["timeRangMonth"]));
                    paramDic.Add("SumUnit", "日排放总量单位：×10⁴m³/d");
                    paramDic.Add("PflUnit", "1");
                    paramDic.Add("pflUnitText", "排放量kg/h");
                    paramDic.Add("plfSumTitle", "月排放总量");
                    break;
                case "year":
                    paramDic.Add("Title", "固定源挥发性有机物排放连续监测月平均值年报表");
                    paramDic.Add("CheckTime", string.Format("{0}年", Request.QueryString["timeRangYear"]));
                    paramDic.Add("SumUnit", "日排放总量单位：×10⁴m³/d");
                    paramDic.Add("PflUnit", "1");
                    paramDic.Add("pflUnitText", "排放量kg/h");
                    paramDic.Add("plfSumTitle", "年排放总量");
                    break;
            }
            return paramDic;
        }

    }
}