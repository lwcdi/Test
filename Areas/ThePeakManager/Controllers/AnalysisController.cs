using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UI.Web.Content.code.Handler;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.ThePeakManager.Controllers
{
    [ActionParam]
    public class AnalysisController : Controller
    {
        //
        // GET: /ThePeakManager/Analysis/

        public ActionResult Index(RequestData data)
        {
            ViewData["compamyType"] = data.Get("compamyType");
            return View();
        }
        public ActionResult DataAnalysis(RequestData data)
        {
            /*
                type:companyInfo.TYPE,
                id: companyInfo.ID,
                startTimeMonitor: startTimeMonitor,
                endTimeMonitor: endTimeMonitor,
                startTimeAvg: startTimeAvg,
                endTimeAvg: endTimeAvg,
                dataFrom: dataFrom
             */
            Analysis analysis = new Analysis();
            DataTable table = analysis.AnalysisData(
                data.Get("type"), 
                data.Get("id"), 
                DateTime.Parse(data.Get("startTimeMonitor")), 
                DateTime.Parse(data.Get("endTimeMonitor")), 
                DateTime.Parse(data.Get("startTimeAvg")), 
                DateTime.Parse(data.Get("endTimeAvg")),
                data.Get("compamyType"),
                data.Get("dataFrom")
                );
            List<string> itemCode = analysis.GetMonitorItem(data.Get("type"), data.Get("id"), "1"==data.Get("compamyType")? new List<string>() { "1","2"} : new List<string>() { "3" });//因子编码
            List<ItemInfo> itemInfoList = MonitorData.GetItemInfo(MonitorData.MONITOR_ITEM_TYPE).Where(item=> itemCode.Exists(c=>c==item.CODE)&& item.CHILD_CODE=="PFL").ToList();
            List<dynamic> titleList = new List<dynamic>();
            List<dynamic> subTitleList = new List<dynamic>();
            itemInfoList.ForEach(item =>
            {
                titleList.Add(new {
                    ITEMCODE = item.CODE+"_Hand",
                    ITEMTEXT = item.TITLE,
                    COLSPAN = 3
                });
                subTitleList.Add(new
                {
                    ITEMCODE = item.CODE+ "_PFL_Value",
                    ITEMTEXT = item.CHILD_TITLE,
                    UNIT = item.CHILD_REMARK
                });
                subTitleList.Add(new
                {
                    ITEMCODE = item.CODE+ "_PFL_Value_Avg",
                    ITEMTEXT = "日常排放",
                    UNIT = item.CHILD_REMARK
                });
                subTitleList.Add(new
                {
                    ITEMCODE = item.CODE+ "_PFL_Value_Percent",
                    ITEMTEXT = "比例",
                    UNIT = "%"
                });
            });
            return Json(new
            {
                TITLE = new
                {
                    MAINTITLE = titleList,
                    SUBTITLE = subTitleList
                },
                DATA = new
                {
                    total = table.Rows.Count,
                    rows = table.ToDynamicList()
                },
                LimitValuePercent= LimitValuePercent(data.Get("type"), data.Get("id")),
                EChartData = EChartData(table, itemInfoList, data.Get("dataFrom"))
            });
        }
        /// <summary>
        /// 构建图表数据
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="itemInfoList"></param>
        /// <param name="dataFrom"></param>
        /// <returns></returns>
        private string EChartData(DataTable sourceTable, List<ItemInfo> itemInfoList,string dataFrom)
        {
            StringBuilder x = new StringBuilder();//X轴
            List<KeyValuePair<string, string>> y = new List<KeyValuePair<string, string>>();//y轴
            List<KeyValuePair<string, string>> yLine = new List<KeyValuePair<string, string>>();//Y轴水平线
            StringBuilder legend = new StringBuilder();//图例
            Dictionary<string, StringBuilder> dictY = new Dictionary<string, StringBuilder>();
            Dictionary<string, StringBuilder> dictLineY = new Dictionary<string, StringBuilder>();
            itemInfoList.ForEach(item => {
                legend.Append(string.Format(",'{0}{1}'", item.TITLE, item.CHILD_TITLE));
                dictY.Add(string.Format("{0}{1}", item.TITLE, item.CHILD_TITLE), new StringBuilder());
                dictLineY.Add(string.Format("{0}{1}", item.TITLE, item.CHILD_TITLE), new StringBuilder());
            });
            string startTime = "";
            string endTime = "";
            string dateFormat = "hour" == dataFrom ? "dd-HH" : "MM-dd";
            if (sourceTable.Rows.Count > 0)
            {
                if ("hour" == dataFrom)
                {
                    startTime = DateTime.Parse(StringHelper.DynamicToString(sourceTable.Rows[0]["DATA_TIME"])).ToString(dateFormat);
                    endTime = DateTime.Parse(StringHelper.DynamicToString(sourceTable.Rows[sourceTable.Rows.Count-1]["DATA_TIME"])).ToString(dateFormat);
                }
                else if ("day" == dataFrom)
                {
                    startTime = DateTime.Parse(StringHelper.DynamicToString(sourceTable.Rows[0]["DATA_TIME"])).ToString(dateFormat);
                    endTime = DateTime.Parse(StringHelper.DynamicToString(sourceTable.Rows[sourceTable.Rows.Count - 1]["DATA_TIME"])).ToString(dateFormat);
                }
            }
            for(int i = 0; i < sourceTable.Rows.Count; i++)
            {
                DataRow row = sourceTable.Rows[i];
                x.Append(",'" + DateTime.Parse(StringHelper.DynamicToString(row["DATA_TIME"])).ToString(dateFormat) + "'");
                itemInfoList.ForEach(item => {
                    string key = string.Format("{0}{1}", item.TITLE, item.CHILD_TITLE);
                    object value = row[string.Format("{0}_{1}_Value", item.CODE, item.CHILD_CODE)];
                    dictY[key].Append(string.Format(",'{0}'", StringHelper.DynamicToString(value)));
                    if (i == 0)//均值只用一个值
                    {
                        object valueAvg = row[string.Format("{0}_{1}_Value_Avg", item.CODE, item.CHILD_CODE)];
                        dictLineY[key].Append(string.Format("'{0}'", StringHelper.DynamicToString(valueAvg)));
                    }
                });
            }
            //移除前面的逗号
            dictY.Keys.ToList().ForEach(key=> {
                if(dictY[key].Length > 0)
                {
                    string value = dictY[key].Remove(0, 1).ToString();
                    y.Add(new KeyValuePair<string, string>(key, value));
                }
            });
            dictLineY.Keys.ToList().ForEach(key => {
                if (dictLineY[key].Length > 0)
                {
                    string value = dictLineY[key].ToString();
                    yLine.Add(new KeyValuePair<string, string>(key, value));
                }
            });
            if (x.Length > 0) x = x.Remove(0, 1);
            if (legend.Length > 0) legend = legend.Remove(0, 1);
            return EChartsHelper.GetBaseChart_zhu(x.ToString(), y, "", "", legend.ToString(), " ", "line", yLine, startTime, endTime, false, false);
        }
        public ActionResult ExportExcel(RequestData data)
        {
            Analysis analysis = new Analysis();
            DataTable sourceTable = analysis.AnalysisData(
                data.Get("type"),
                data.Get("id"),
                DateTime.Parse(data.Get("startTimeMonitor")),
                DateTime.Parse(data.Get("endTimeMonitor")),
                DateTime.Parse(data.Get("startTimeAvg")),
                DateTime.Parse(data.Get("endTimeAvg")),
                data.Get("compamyType"),
                data.Get("dataFrom")
                );
            List<string> itemCode = analysis.GetMonitorItem(data.Get("type"), data.Get("id"), "1" == data.Get("compamyType") ? new List<string>() { "1", "2" } : new List<string>() { "3" });//因子编码
            List<ItemInfo> itemInfoList = MonitorData.GetItemInfo(MonitorData.MONITOR_ITEM_TYPE).Where(item => itemCode.Exists(c => c == item.CODE) && item.CHILD_CODE == "PFL").ToList();

            List<KeyValuePair<int, List<ExcelHelper.HeaderStruct>>> list = new List<KeyValuePair<int, List<ExcelHelper.HeaderStruct>>>();
            List<ExcelHelper.HeaderStruct> listTitle = new List<ExcelHelper.HeaderStruct>();//一级标题
            List<ExcelHelper.HeaderStruct> subTitle = new List<ExcelHelper.HeaderStruct>();//二级标题
            List<string> colsNameList = new List<string>();//导出Excel的table的列名
            listTitle.Add(new ExcelHelper.HeaderStruct()
            {
                headerText = "时间",
                colSpan = 1,
                rowSpan = 2,
            });
            colsNameList.Add("DATA_TIME");
            itemInfoList.ForEach(item =>
            {
                listTitle.Add(new ExcelHelper.HeaderStruct()
                {
                    headerText = item.TITLE,
                    colSpan=3,
                    rowSpan=1,
                });
                subTitle.Add(new ExcelHelper.HeaderStruct() {
                    headerText = item.CHILD_TITLE,
                    colSpan = 1,
                    rowSpan = 1,
                });
                subTitle.Add(new ExcelHelper.HeaderStruct()
                {
                    headerText = "日常排放",
                    colSpan = 1,
                    rowSpan = 1,
                });
                subTitle.Add(new ExcelHelper.HeaderStruct()
                {
                    headerText = "比例",
                    colSpan = 1,
                    rowSpan = 1,
                });
                colsNameList.Add(item.CODE + "_PFL_Value");
                colsNameList.Add(item.CODE + "_PFL_Value_Avg");
                colsNameList.Add(item.CODE + "_PFL_Value_Percent");
            });


            list.Add(new KeyValuePair<int, List<ExcelHelper.HeaderStruct>>(0, listTitle));
            list.Add(new KeyValuePair<int, List<ExcelHelper.HeaderStruct>>(1, subTitle));
            DataSet dtSet = new DataSet();
            dtSet.Tables.Add(sourceTable);
            var buf = ExcelHelper.DataSetToExcel(dtSet, list, colsNameList);
            return File(buf, "application/ms-excel", Server.UrlPathEncode("停产限产分析.xls"));
        }
        /// <summary>
        /// 获取限制比例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private decimal LimitValuePercent(string type,string id)
        {
            string conpanyID = id;
            if("P" == type)
            {
                conpanyID = SqlModel.Select(T_BASE_COMPANY_PK.COMPANYID).From(DB.T_BASE_COMPANY_PK).Where(T_BASE_COMPANY_PK.ID == id).ExecuteScalar().ToString();
            }
            ThePeakInfo info = new ThePeakInfo();
            T_THEPEAK_ENT_SUB_LISTModel company = info.GetThePeakEnterprise().Find(item => StringHelper.DynamicToString(item.COMPANY_ID) == conpanyID);
            if (null == company) return 100;
            decimal limitValue = 100;
            switch (StringHelper.DynamicToString(company.ENT_PEAK_TYPE))
            {
                case "1":limitValue = 0;break;
                case "2": limitValue = company.LIMIT_RATIO.Value; break;
                case "3": limitValue = company.LIMIT_RATIO.Value; break;
            }
            return limitValue;
        }
    }
}
