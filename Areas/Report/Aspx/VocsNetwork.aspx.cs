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
    /// <summary>
    /// Vocs在线-企业联网统计
    /// </summary>
    public partial class VocsNetwork : System.Web.UI.Page
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

            //定义报表参数对象
            List<ReportParameter> parameterList = new List<ReportParameter>();
            var paramDic = RdlcParam();
            foreach (var key in paramDic.Keys)
            {
                parameterList.Add(new ReportParameter(key, paramDic[key]));
            }
            string ReportPath = "Areas\\Report\\Template\\CompanyNetwork.rdlc";
            ReportViewer1.LocalReport.ReportPath = ReportPath;
            ReportViewer1.LocalReport.ReportEmbeddedResource = "Tablix1";
            ReportViewer1.LocalReport.SetParameters(parameterList);

            DataTable table = MonitorData.CompanyNetwork("2", SerializerHelper.Deserialize<List<string>>(Request.QueryString["companyID"]));
            ReportDataSource rds = new ReportDataSource("CompanyNetwork", table);//指定数据源GetTestDataTable()
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
            paramDic.Add("ReportTitle", "固定源挥发性有机物排放连续监测联网报表");
            return paramDic;
        }
    }
}