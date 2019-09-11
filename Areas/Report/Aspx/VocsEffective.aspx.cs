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
    public partial class VocsEffective : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BandingReport();
            }
        }
        private void BandingReport()
        {
            List<string> pkIDList = SerializerHelper.Deserialize<List<string>>(Request.QueryString["PK_ID"]);
            DateTime startTime;
            DateTime endTime;
            MonitorData.QuarterlyTime(int.Parse(Request.QueryString["timeRangYear"]), int.Parse(Request.QueryString["timeRangQuarter"]), out startTime, out endTime);

            //定义报表参数对象
            List<ReportParameter> parameterList = new List<ReportParameter>();
            var paramDic = RdlcParam();
            foreach (var key in paramDic.Keys)
            {
                parameterList.Add(new ReportParameter(key, paramDic[key]));
            }
            string ReportPath = "Areas\\Report\\Template\\DataEffective.rdlc";
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportViewer1.LocalReport.ReportEmbeddedResource = "Tablix1";
            ReportViewer1.LocalReport.SetParameters(parameterList);

            DataTable table = MonitorData.PK_VocsEffectiveData(pkIDList, startTime, endTime);
            ReportDataSource rds = new ReportDataSource("EffectiveData", table);//指定数据源GetTestDataTable()
            ReportViewer1.LocalReport.DataSources.Clear();//报表数据源清除，必须
            ReportViewer1.LocalReport.DataSources.Add(rds);//报表数据源增加
            ReportViewer1.LocalReport.Refresh();//刷新报表 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">day month year custom</param>
        /// <returns></returns>
        private Dictionary<string, string> RdlcParam()
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            paramDic.Add("ReportTitle", "固定源挥发性有机物排放连续监测数据有效率季度报表");
            paramDic.Add("CheckTime", string.Format("监测时间：{0}年第{1}季度", Request.QueryString["timeRangYear"], Request.QueryString["timeRangQuarter"]));
            return paramDic;
        }
    }
}