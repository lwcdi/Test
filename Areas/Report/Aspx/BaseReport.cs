using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.Report.Aspx
{
    public class BaseReport : System.Web.UI.Page
    {
        protected DateTime startTime = DateTime.MinValue;
        protected DateTime endTime = DateTime.MinValue;
        protected string dataType = "";
        protected dynamic monitorObject = null;
        /// <summary>
        /// 污染源的名称
        /// </summary>
        protected string PollutionName
        {
            get
            {
                if (monitorObject == null) GetMonitorObject();
                if (monitorObject == null)
                {
                    return " ";
                }
                else
                {
                    return monitorObject["NAME"];
                }
            }
        }
        /// <summary>
        /// 染源的编号
        /// </summary>
        protected string PollutionNo
        {
            get
            {
                if (monitorObject == null) GetMonitorObject();
                if (monitorObject == null)
                {
                    return " ";
                }
                else
                {
                    return StringHelper.DynamicToString(monitorObject["CODE"]);
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                switch (Request.QueryString["rdlcType"])
                {
                    case "custom":
                        startTime = DateTime.Parse(Request.QueryString["timeRangStart"]);
                        endTime = DateTime.Parse(Request.QueryString["timeRangEnd"]);
                        dataType = "hour";
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
                        endTime = new DateTime(StringHelper.DynamicToInt(Request.QueryString["timeRangYear"]), 12, 1);
                        dataType = "month";
                        break;
                }

                BandingReport();
            }
        }
        protected virtual void BandingReport()
        {

        }
        private void GetMonitorObject()
        {
            if ("P" == Request.QueryString["type"])
            {
                SqlModel sqlmodel = SqlModel.Select(T_BASE_COMPANY_PK.CODE, T_BASE_COMPANY_PK.NAME).From(DB.T_BASE_COMPANY_PK).Where(T_BASE_COMPANY_PK.ID == Request.QueryString["id"]);
                monitorObject = sqlmodel.ExecToDynamic();
            }
            else
            {
                SqlModel sqlmodel = SqlModel.Select(T_BASE_COMPANY.ID.As("CODE"), T_BASE_COMPANY.NAME).From(DB.T_BASE_COMPANY).Where(T_BASE_COMPANY.ID == Request.QueryString["id"]);
                monitorObject = sqlmodel.ExecToDynamic();
            }
        }
    }
}