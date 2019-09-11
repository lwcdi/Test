using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w.Model;
using w.ORM;

namespace UI.Web
{
    public class Analysis
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">P:排口 C:企业</param>
        /// <param name="id"></param>
        /// <param name="startTimeMonitor"></param>
        /// <param name="endTimeMonitor"></param>
        /// <param name="startTimeAvg"></param>
        /// <param name="endTimeAvg"></param>
        /// <param name="compamyType">1:污染源 2:VOCs</param>
        /// <returns></returns>
        public DataTable AnalysisData(string type, string id, DateTime startTimeMonitor, DateTime endTimeMonitor, DateTime startTimeAvg, DateTime endTimeAvg, string compamyType, string dataType)
        {
            DataTable monitorData = BaseMonitorData(type, id, startTimeMonitor, endTimeMonitor, compamyType, dataType);
            Dictionary<string, decimal> avgDic = MonitorAvgData(type, id, startTimeAvg, endTimeAvg, compamyType, dataType);
            List<string> avgKeys = avgDic.Keys.ToList();
            //添加平均列（赋初值）和百分比列
            avgKeys.ForEach(key =>
            {
                DataColumn avgColumn = new DataColumn(key + "_Avg");
                avgColumn.DefaultValue = avgDic[key];
                DataColumn percentColumn = new DataColumn(key + "_Percent");
                monitorData.Columns.Add(avgColumn);
                monitorData.Columns.Add(percentColumn);
            });
            //计算百分比
            for(int i = 0;i< monitorData.Rows.Count; i++)
            {
                DataRow row = monitorData.Rows[i];
                avgKeys.ForEach(item =>
                {
                    decimal data = StringHelper.DynamicToDecimal(row[item]);
                    decimal avg = StringHelper.DynamicToDecimal(row[item+ "_Avg"]);
                    if (avg == 0) return;
                    row[item + "_Percent"] = (data / avg * 100).ToString("0.00");
                });
            }
            return monitorData;
        }
        /// <summary>
        /// 获取企业或排口的监测数据
        /// </summary>
        /// <param name="type">P:排口 C:企业</param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="compamyType">1:污染源 2:VOCs</param>
        ///  <param name="dataType">hour:小时 day:日</param>
        /// <returns></returns>
        public DataTable BaseMonitorData(string type, string id, DateTime startTime, DateTime endTime, string compamyType,string dataType)
        {
            string dataClass = "1" == compamyType ? string.Join(",", MonitorData.POLLUTION_MONITOR_ITEM_TYPE) : string.Join(",", MonitorData.VOCS_MONITOR_ITEM_TYPE);

            DataTable monitorData = MonitorData.GetMonitorData("002", startTime, endTime, type, id, dataType, dataClass);
            return ColumnNameExchange(type,id, compamyType, monitorData);
        }
        /// <summary>
        /// 计算监测数据的平均平均值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="compamyType"></param>
        /// <returns></returns>
        public Dictionary<string,decimal> MonitorAvgData(string type, string id, DateTime startTime, DateTime endTime, string compamyType, string dataType)
        {
            /*
             1.获取监测数据
             2.去掉不需要的不需要统计的列，这里只是统计排放量
             3.将列名和均值保存在字典中
             */
            string dataClass = "1" == compamyType ? string.Join(",", MonitorData.POLLUTION_MONITOR_ITEM_TYPE) : string.Join(",", MonitorData.VOCS_MONITOR_ITEM_TYPE);
            DataTable monitorData = MonitorData.GetMonitorData("002", startTime, endTime, type, id, dataType, dataClass);
            DataTable exchangeTable = ColumnNameExchange(type, id, compamyType, monitorData);
            Dictionary<string, decimal> dic = new Dictionary<string, decimal>();
            List<dynamic> data = exchangeTable.ToDynamicList();
            for (int i=0;i< exchangeTable.Columns.Count; i++)
            {
                DataColumn column = exchangeTable.Columns[i];
                if ("DATA_TIME" == column.ColumnName) continue;
                decimal? avgData = data.Average(item=> {
                    if (item[column.ColumnName] is DBNull) return null;
                    decimal temp = StringHelper.DynamicToDecimal(item[column.ColumnName]);
                    return temp;
                });
                dic.Add(column.ColumnName, Math.Round(avgData.HasValue?avgData.Value:0, 2));
            }
            return dic;
        }
        /// <summary>
        /// 获取监测项
        /// </summary>
        /// <param name="type">P:排口 C:企业</param>
        /// <param name="id"></param>
        /// <param name="pkType">当type为企业是，只获取对应类型的排口因子（1-废气、2-废水、3-VOCs）</param>
        /// <returns></returns>
        public List<string> GetMonitorItem(string type, string id, List<string> pkType)
        {
            List<string> itemCode = new List<string>();
            if ("P" == type)
            {
                SqlModel.Select(T_BASE_COMPANY_PK_TX.CLCS)
                    .From(DB.T_BASE_COMPANY_PK)
                    .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                    .Where(T_BASE_COMPANY_PK.ID == id)
                    .ExecToDynamicList()
                    .ForEach(item => {
                        string codes = StringHelper.DynamicToString(item["CLCS"]);
                        if (string.IsNullOrEmpty(codes)) return;
                        codes.Split(',').ToList()
                        .ForEach(code => {
                            if (itemCode.Exists(c => c == code)) return;
                            itemCode.Add(code);
                        });
                    });
            }
            if ("C" == type)
            {
                SqlModel.Select(T_BASE_COMPANY_PK_TX.CLCS)
                    .From(DB.T_BASE_COMPANY)
                    .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_PK.COMPANYID == T_BASE_COMPANY.ID & T_BASE_COMPANY_PK.TYPE.In(StringHelper.SqlInCondition(pkType)))
                    .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                    .Where(T_BASE_COMPANY.ID == id)
                    .ExecToDynamicList()
                    .ForEach(item => {
                        string codes = StringHelper.DynamicToString(item["CLCS"]);
                        if (string.IsNullOrEmpty(codes)) return;
                        codes.Split(',').ToList()
                        .ForEach(code => {
                            if (itemCode.Exists(c => c == code)) return;
                            itemCode.Add(code);
                        });
                    });
            }
            return itemCode;
        }
        /// <summary>
        /// 修改列名，只返回修改类名的列
        /// </summary>
        /// <param name="type">P:排口 C:企业</param>
        /// <param name="id"></param>
        /// <param name="compamyType">1:污染源 2:VOCs,当type参数为C时，改参数才有效</param>
        /// <param name="table">数据源</param>
        /// <returns></returns>
        private DataTable ColumnNameExchange(string type, string id, string compamyType,DataTable table)
        {
            List<string> monitorItem = new List<string>();
            if ("P" == type)
            {
                monitorItem = GetMonitorItem(type, id, null);
            }
            else if ("C" == type && "1" == compamyType)
            {
                monitorItem = GetMonitorItem(type, id, new List<string>() { "1", "2" });
            }
            else if ("C" == type && "2" == compamyType)
            {
                monitorItem = GetMonitorItem(type, id, new List<string>() { "3" });
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("DATA_TIME", "DATA_TIME");
            if (monitorItem.Exists(c => c == "a34013")) dic.Add("a34013_PFL_Value", "a34013_PFL_Value");
            if (monitorItem.Exists(c => c == "a21026")) dic.Add("a21026_PFL_Value", "a21026_PFL_Value");
            if (monitorItem.Exists(c => c == "a21002")) dic.Add("a21002_PFL_Value", "a21002_PFL_Value");

            if (monitorItem.Exists(c => c == "w01018")) dic.Add("w01018_PFL_Value", "w01018_PFL_Value");
            if (monitorItem.Exists(c => c == "w21003")) dic.Add("w21003_PFL_Value", "w21003_PFL_Value");

            if (monitorItem.Exists(c => c == "a24088")) dic.Add("a24088_PFL_Value", "a24088_PFL_Value");//非甲烷总烃
            if (monitorItem.Exists(c => c == "a25002")) dic.Add("a25002_PFL_Value", "a25002_PFL_Value");//苯
            if (monitorItem.Exists(c => c == "a25003")) dic.Add("a25003_PFL_Value", "a25003_PFL_Value");//甲苯
            if (monitorItem.Exists(c => c == "a25005")) dic.Add("a25005_PFL_Value", "a25005_PFL_Value");//二甲苯
            DataTable monitorTable = MonitorData.CreateResultTable(table, dic);
            return monitorTable;
        }
    }
}