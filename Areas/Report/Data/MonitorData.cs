using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using w.Model;
using w.ORM;
using System.Configuration;

namespace UI.Web
{
    /// <summary>
    /// 获取监测数据
    /// </summary>
    public class MonitorData
    {
        /// <summary>
        /// 全部监测
        /// </summary>
        public static readonly List<string> MONITOR_ITEM_TYPE = new List<string>() { "GasItem", "WaterItem", "VOCsItem" };
        /// <summary>
        /// 污染源监测
        /// </summary>
        public static readonly List<string> POLLUTION_MONITOR_ITEM_TYPE = new List<string>() { "GasItem", "WaterItem" };
        /// <summary>
        /// VOCs监测
        /// </summary>
        public static readonly List<string> VOCS_MONITOR_ITEM_TYPE = new List<string>() { "VOCsItem" };
        /// <summary>
        /// 判断监测因子在统计时是平均还是累加
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        public static string MonitorItemCalculateType(ItemInfo itemInfo)
        {
            if (itemInfo == null)
            {
                return "";
            }
            if(!string.IsNullOrEmpty(itemInfo.CHILD_CODE) && itemInfo.CHILD_CODE == "PFL"){
                return "+";
            }
            if (!string.IsNullOrEmpty(itemInfo.CHILD_CODE) && (itemInfo.CHILD_CODE == "ND" || itemInfo.CHILD_CODE == "ZSND"))
            {
                return "/";
            }
            if (ConfigurationManager.AppSettings["singleItemAvg"].Split(',').Contains(itemInfo.CODE))
            {
                return "/";
            }
            else
            {
                return "+";
            }
        }
        /// <summary>
        /// 判断监测因子在统计时是平均还是累加
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public static string MonitorItemCalculateType(string itemCode,string childItemCode)
        {
            List<string> dataClass = new List<string> (){ "GasItem", "WaterItem", "VOCsItem" };
            List<ItemInfo> itemInfolist = DataCache.GetCache("MonitorData_itemInfolist") as List<ItemInfo>;
            if(!(itemInfolist != null && itemInfolist.Count > 0))
            {
                itemInfolist = GetItemInfo(dataClass);
                DataCache.SetCache("MonitorData_itemInfolist", itemInfolist);
            }
            ItemInfo itemInfo = itemInfolist.Find(c => c.CODE == itemCode && StringHelper.DynamicToString(c.CHILD_CODE) == StringHelper.DynamicToString(childItemCode));
            return MonitorItemCalculateType(itemInfo);
        }
        /// <summary>
        /// 获取监测数据
        /// </summary>
        /// <param name="dataFrom">001:审核数据 002:未审核数据</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="type">P:排口 C:企业</param>
        /// <param name="id">排口id、企业id 多种个可以使用逗号分隔传入</param>
        /// <param name="dataType">day:日数据、hour:小时数据、month：月数据</param>
        /// <param name="dataClass">GasItem:废气、WaterItem:废水、VOC:VOCsItem，多种类型可以使用逗号分隔传入 </param>
        /// <returns></returns>
        public static DataTable GetMonitorData(string dataFrom, DateTime startTime, DateTime endTime,string type,string id,string dataType,string dataClass)
        {
            StringBuilder sql = new StringBuilder();
            if ("P" == type)
            {
                sql.Append(" select p.id pkid,p.id,p.name,h.itemcode,h.rectime,h.value,h.status,h.subitemcode,h.unit from t_base_company c ");
            }
            else
            {
                sql.Append(" select p.id pkid,c.id,c.name,h.itemcode,h.rectime,h.value,h.status,h.subitemcode,h.unit from t_base_company c ");
            }
            //sql.Append(" select h.itemcode,h.rectime,h.value,h.status,h.subitemcode,h.unit from t_base_company c ");
            sql.Append(" left join   t_base_company_pk p on c.id = p.companyid ");
            sql.Append(" left join t_base_company_pk_tx t on t.pkid = p.id ");
            switch (dataType)
            {
                case "month": sql.Append(" left join t_mid_day h on t.mn = h.devicecode"); break;
                case "day":sql.Append(" left join t_mid_day h on t.mn = h.devicecode");break;
                case "hour":
                    switch (dataFrom)
                    {
                        case "001":
                            sql.Append(" left join t_mid_hour_c h on t.mn = h.devicecode");break;
                        case "002":
                            sql.Append(" left join t_mid_hour h on t.mn = h.devicecode");break;
                        default:
                            sql.Append(" left join t_mid_hour_c h on t.mn = h.devicecode"); break;
                    }
                    break;
                case "custom": sql.Append(" left join t_mid_hour h on t.mn = h.devicecode"); break;
            }
            
            sql.Append(string.Format(" where h.rectime >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') and h.rectime <= to_date('{1}','yyyy-mm-dd hh24:mi:ss')  ", startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss")));
            if ("P" == type)
            {
                sql.Append(string.Format(" and p.id in ({0}) ", StringHelper.SqlInCondition(id)));
            }
            else
            {
                sql.Append(string.Format(" and c.id  in ({0}) ", StringHelper.SqlInCondition(id)));
            }
            sql.Append(" order by h.rectime asc ");
            SqlModel sqlModel = SqlModel.Select(sql.ToString()).Native();
            List<dynamic> list = sqlModel.ExecToDynamicList();
            DataTable table = DataExchange(list, dataClass, dataType); 
            return DataHandle.TableAddBlankRow(table,startTime,endTime, dataType);
        }
        /// <summary>
        /// 获取监测数据数据
        /// </summary>
        /// <param name="dataFrom">001:未审核数据 002:审核数据</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="type">P:排口 C:企业</param>
        /// <param name="idList">排口id、企业id </param>
        /// <param name="dataType">day:日数据、hour:小时数据、month：月数据</param>
        /// <param name="dataClass">GasItem:废气、WaterItem:废水、VOC:VOCsItem，多种类型可以使用逗号分隔传入 </param>
        /// <returns></returns>
        public static DataTable GetMonitorData(string dataFrom, DateTime startTime, DateTime endTime, string type, List<string> idList, string dataType, string dataClass)
        {
            StringBuilder id = new StringBuilder();
            idList.ForEach(item => {
                id.Append(",").Append(item);
            });
            if (id.ToString().Length > 0)
            {
                id.Remove(0, 1);
            }
            else
            {
                id.Append(Guid.NewGuid().ToString());
            }
            return GetMonitorData(dataFrom, startTime, endTime, type,  id.ToString(), dataType, dataClass);
        }
        /// <summary>
        /// 监测数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="dataClass"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private static DataTable DataExchange(List<dynamic> list, string dataClass, string dataType)
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("DATA_TIME"));
            table.Columns.Add(new DataColumn("DATA_TIME_ORDER", typeof(DateTime)));//用于排序
            table.Columns.Add(new DataColumn("ID"));
            table.Columns.Add(new DataColumn("NAME"));
            List<ItemInfo> itemInfolist = GetItemInfo(dataClass.Split(',').ToList());
            itemInfolist.ForEach(itemInfo=> {
                string columnCodeName = "";
                string columnTitleName = "";
                string columnValueName = "";
                if (string.IsNullOrEmpty(itemInfo.CHILD_CODE))
                {
                    columnCodeName = itemInfo.CODE;
                    columnTitleName = string.Format("{0}_TITLE", itemInfo.CODE);
                    columnValueName = string.Format("{0}_Value", itemInfo.CODE); //itemInfo.CODE + "_Value";
                }
                else
                {
                    columnCodeName = string.Format("{0}_{1}", itemInfo.CODE, itemInfo.CHILD_CODE);
                    columnTitleName = string.Format("{0}_{1}_TITLE", itemInfo.CODE, itemInfo.CHILD_CODE);
                    columnValueName = string.Format("{0}_{1}_Value", itemInfo.CODE, itemInfo.CHILD_CODE);
                }
                DataColumn columnCode = new DataColumn(columnCodeName);
                DataColumn columnTitle = new DataColumn(columnTitleName);
                DataColumn columnValue = new DataColumn(columnValueName);
                table.Columns.Add(columnCode);
                table.Columns.Add(columnTitle);
                table.Columns.Add(columnValue);
            });
            if (!(list != null && list.Count > 0))
            {
                //DataRow row = table.NewRow();
                //table.Rows.Add(row);
                return table;
            }
            List<string> existList = new List<string>();
            list.ForEach(item => {
                string rectime = StringHelper.DynamicToString(item["RECTIME"]);
                string id = StringHelper.DynamicToString(item["ID"]);//企业ID或排口ID
                string name = StringHelper.DynamicToString(item["NAME"]);//企业名称或排口名称
                if (string.IsNullOrEmpty(rectime)) return;
                string dataTime = dataType != "month" ? rectime : DateTime.Parse(rectime).ToString("yyyy-MM-01");
                string dataGuid = string.Format("{0}_{1}", dataTime, id);
                if (existList.Exists(c => c == dataGuid)) return;
                existList.Add(dataGuid);
                bool hasData = false;
                DataRow row = table.NewRow();
                row["DATA_TIME"] = DateTime.Parse(dataTime);
                row["DATA_TIME_ORDER"] = DateTime.Parse(dataTime);
                row["ID"] = id;
                row["NAME"] = name;
                itemInfolist.ForEach(itemInfo => {
                    if (!GetPkOrCompanyMonitor(StringHelper.DynamicToString(item["PKID"])).Contains(itemInfo.CODE)) return;
                    if ("month"== dataType)
                    {
                        GetItemValueForMonth(list, itemInfo, dataTime, id, row, ref hasData);
                    }
                    else
                    {
                        GetItemValue(list, itemInfo, rectime, id, row, ref hasData);
                    }
                    
                });
                if (hasData) table.Rows.Add(row);
            });
            return table;
        }
        private static void GetItemValue(List<dynamic> list, ItemInfo itemInfo, string rectime, string id, DataRow row,  ref bool hasData)
        {
            dynamic item = string.IsNullOrEmpty(itemInfo.CHILD_CODE) ?
                list.Find(c => StringHelper.DynamicToString(c["ID"]) == id && StringHelper.DynamicToString(c["RECTIME"]) == rectime && StringHelper.DynamicToString(c["ITEMCODE"]) == itemInfo.CODE) :
                list.Find(c => StringHelper.DynamicToString(c["ID"]) == id && StringHelper.DynamicToString(c["RECTIME"]) == rectime && StringHelper.DynamicToString(c["ITEMCODE"]) == itemInfo.CODE && StringHelper.DynamicToString(c["SUBITEMCODE"]) == itemInfo.CHILD_CODE);
            if (item != null)
            {
                hasData = true;
                string columnCodeName = "";
                string columnTitleName = "";
                string columnValueName = "";
                if (string.IsNullOrEmpty(itemInfo.CHILD_CODE))
                {
                    columnCodeName = itemInfo.CODE;
                    columnTitleName = string.Format("{0}_TITLE", itemInfo.CODE);
                    columnValueName = string.Format("{0}_Value", itemInfo.CODE); //itemInfo.CODE + "_Value";
                }
                else
                {
                    columnCodeName = string.Format("{0}_{1}", itemInfo.CODE, itemInfo.CHILD_CODE);
                    columnTitleName = string.Format("{0}_{1}_TITLE", itemInfo.CODE, itemInfo.CHILD_CODE);
                    columnValueName = string.Format("{0}_{1}_Value", itemInfo.CODE, itemInfo.CHILD_CODE);
                }
                row[columnCodeName] = string.IsNullOrEmpty(itemInfo.CHILD_CODE) ? itemInfo.CODE : string.Format("{0}_{1}",itemInfo.CODE,itemInfo.CHILD_CODE);
                row[columnTitleName] = string.IsNullOrEmpty(itemInfo.CHILD_CODE) ? itemInfo.TITLE : string.Format("{0}_{1}", itemInfo.TITLE, itemInfo.CHILD_TITLE);
                row[columnValueName] = StringHelper.DynamicToString(item["VALUE"]);
            }
        }
        private static void GetItemValueForMonth(List<dynamic> list, ItemInfo itemInfo, string rectime, string id, DataRow row, ref bool hasData)
        {
            IEnumerable<dynamic> items = string.IsNullOrEmpty(itemInfo.CHILD_CODE) ?
                list.Where(c => StringHelper.DynamicToString(c["ID"]) == id && DateTime.Parse(StringHelper.DynamicToString(c["RECTIME"])).ToString("yyyy-MM-01") == rectime && StringHelper.DynamicToString(c["ITEMCODE"]) == itemInfo.CODE) :
                list.Where(c => StringHelper.DynamicToString(c["ID"]) == id && DateTime.Parse(StringHelper.DynamicToString(c["RECTIME"])).ToString("yyyy-MM-01") == rectime && StringHelper.DynamicToString(c["ITEMCODE"]) == itemInfo.CODE && StringHelper.DynamicToString(c["SUBITEMCODE"]) == itemInfo.CHILD_CODE);
            if (items != null && items.Count()>0)
            {
                hasData = true;
                string columnCodeName = "";
                string columnTitleName = "";
                string columnValueName = "";
                if (string.IsNullOrEmpty(itemInfo.CHILD_CODE))
                {
                    columnCodeName = itemInfo.CODE;
                    columnTitleName = string.Format("{0}_TITLE", itemInfo.CODE);
                    columnValueName = string.Format("{0}_Value", itemInfo.CODE); //itemInfo.CODE + "_Value";
                }
                else
                {
                    columnCodeName = string.Format("{0}_{1}", itemInfo.CODE, itemInfo.CHILD_CODE);
                    columnTitleName = string.Format("{0}_{1}_TITLE", itemInfo.CODE, itemInfo.CHILD_CODE);
                    columnValueName = string.Format("{0}_{1}_Value", itemInfo.CODE, itemInfo.CHILD_CODE);
                }
                row[columnCodeName] = string.IsNullOrEmpty(itemInfo.CHILD_CODE) ? itemInfo.CODE : string.Format("{0}_{1}", itemInfo.CODE, itemInfo.CHILD_CODE);
                row[columnTitleName] = string.IsNullOrEmpty(itemInfo.CHILD_CODE) ? itemInfo.TITLE : string.Format("{0}_{1}", itemInfo.TITLE, itemInfo.CHILD_TITLE);
                string calculateType = MonitorItemCalculateType(itemInfo);
                if ("+"== calculateType)//累加
                {
                    row[columnValueName] = items.Sum(c => {
                        decimal temp = StringHelper.DynamicToDecimal(c["VALUE"]);
                        return temp;
                    }).ToString("0.00");
                }
                if ("/" == calculateType)//使用均值
                {
                    row[columnValueName] = items.Average(c => {
                        decimal temp = StringHelper.DynamicToDecimal(c["VALUE"]);
                        return temp;
                    }).ToString("0.00");
                }

            }
        }
        /// <summary>
        /// 获取排口的监测因子项
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static List<string> GetPkOrCompanyMonitor(string pkID)
        {
            string cacheKey = "UI_Web_MonitorData_GetPkOrCompanyMonitor";
            Dictionary<string, List<string>> resultDic = DataCache.GetCache(cacheKey) as Dictionary<string, List<string>>;
            if (!(resultDic!=null&& resultDic.Keys.Count > 0))
            {
                resultDic = new Dictionary<string, List<string>>();
                string sql = "select t.pkid,t.clcs from t_base_company_pk pk left join t_base_company_pk_tx t on pk.id = t.pkid where t.pkid is not null";
                List<dynamic> list = SqlModel.Select(sql).Native().ExecToDynamicList();
                list.ForEach(item => {
                    string clcs = StringHelper.DynamicToString(item["CLCS"]);
                    if (string.IsNullOrEmpty(clcs))
                    {
                        resultDic.Add(item["PKID"], new List<string>(1));
                        return;
                    }
                    resultDic.Add(item["PKID"], clcs.Split(',').ToList());
                });
                DataCache.SetCache(cacheKey, resultDic);
            }
            return resultDic[pkID];
        }
        /// <summary>
        /// 获取监测项
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public static List<ItemInfo> GetItemInfo(string itemType)
        {
            SqlModel sql = SqlModel.Select(
                    "d1".Field(BASDIC.CODE)
                    , "d1".Field(BASDIC.TITLE)
                    , "d2".Field(BASDIC.CODE).As("CHILD_CODE")
                    , "d2".Field(BASDIC.TITLE).As("CHILD_TITLE")
                    )
                .From(DB.BASDIC.As("d1"))
                .LeftJoin(DB.BASDIC.As("d2")).On("d1".Field(BASDIC.CODE) == "d2".Field(BASDIC.TYPECODE))
                .Where("d1".Field(BASDIC.TYPECODE) == itemType);
            return SerializerHelper.DynamicObjectToStaticObjectList<ItemInfo>(sql.ExecToDynamicList());
        }
        /// <summary>
        /// 获取监测项
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public static List<ItemInfo> GetItemInfo(List<string> itemType)
        {
            SqlModel sql = SqlModel.Select(
                    "d1".Field(BASDIC.CODE)
                    , "d1".Field(BASDIC.TITLE)
                    , "d1".Field(BASDIC.REMARK)
                    , "d2".Field(BASDIC.CODE).As("CHILD_CODE")
                    , "d2".Field(BASDIC.TITLE).As("CHILD_TITLE")
                    , "d2".Field(BASDIC.REMARK).As("CHILD_REMARK")
                    )
                .From(DB.BASDIC.As("d1"))
                .LeftJoin(DB.BASDIC.As("d2")).On("d1".Field(BASDIC.CODE) == "d2".Field(BASDIC.TYPECODE))
                .Where("d1".Field(BASDIC.TYPECODE).In(StringHelper.SqlInCondition(itemType)));
            return SerializerHelper.DynamicObjectToStaticObjectList<ItemInfo>(sql.ExecToDynamicList());
        }
        /// <summary>
        /// 空气报表监测数据
        /// </summary>
        /// <param name="dataFrom">001:审核数据 002:未审核数据</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="type">P:排口 C:企业</param>
        /// <param name="id">排口id、企业id</param>
        /// <param name="dataType">day:日数据、hour:小时数据</param>
        /// <returns></returns>
        public static DataTable GetMonitorDataForReportAir(string dataFrom, DateTime startTime, DateTime endTime, string type, string id, string dataType)
        {
            var table = MonitorData.GetMonitorData(dataFrom, startTime, endTime, type, id, dataType, "GasItem");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("DATA_TIME", "DATA_TIME");
            dic.Add("a34013_ND_Value", "PM_ND");
            dic.Add("a34013_ZSND_Value", "PM_ZSND");
            dic.Add("a34013_PFL_Value", "PM_PFL");
            dic.Add("a21026_ND_Value", "SO2_ND");
            dic.Add("a21026_ZSND_Value", "SO2_ZSND");
            dic.Add("a21026_PFL_Value", "SO2_PFL");
            dic.Add("a21002_ND_Value", "NOX_ND");
            dic.Add("a21002_ZSND_Value", "NOX_ZSND");
            dic.Add("a21002_PFL_Value", "NOX_PFL");
            dic.Add("flowgas_Value", "GB_LL");
            dic.Add("a19001_Value", "GJ_O2");
            dic.Add("a01012_Value", "TEMPERALURE");
            dic.Add("HUMIDITY", "HUMIDITY");
            dic.Add("LOAD", "LOAD");
            dic.Add("REMARK", "REMARK");
            return CreateResultTable(table, dic);
        }
        /// <summary>
        /// 水报表监测数据
        /// </summary>
        /// <param name="dataFrom">001:审核数据 002:未审核数据</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="type">P:排口 C:企业</param>
        /// <param name="id">排口id、企业id</param>
        /// <param name="dataType">day:日数据、hour:小时数据</param>
        /// <returns></returns>
        public static DataTable GetMonitorDataForReportWater(string dataFrom, DateTime startTime, DateTime endTime, string type, string id, string dataType)
        {
            var table = MonitorData.GetMonitorData(dataFrom, startTime, endTime, type, id, dataType, "WaterItem");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("DATA_TIME", "DATA_TIME");
            dic.Add("w01018_ND_Value", "COD_ND");
            dic.Add("w01018_PFL_Value", "COD_PFL");
            dic.Add("w21003_ND_Value", "AMMONIA_NITROGEN_ND");
            dic.Add("w21003_PFL_Value", "AMMONIA_NITROGEN_PFL");
            dic.Add("flowwater_Value", "FLOW_WATER");
            return CreateResultTable(table, dic);

        }
        /// <summary>
        /// Vocs报表监测数据
        /// </summary>
        /// <param name="dataFrom">001:审核数据 002:未审核数据</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="type">P:排口 C:企业</param>
        /// <param name="id">排口id、企业id</param>
        /// <param name="dataType">day:日数据、hour:小时数据</param>
        /// <returns></returns>
        public static DataTable GetMonitorDataForReportVocs(string dataFrom, DateTime startTime, DateTime endTime, string type, string id, string dataType)
        {
            var table = MonitorData.GetMonitorData(dataFrom, startTime, endTime, type, id, dataType, "VOCsItem");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("DATA_TIME", "DATA_TIME");
            dic.Add("a24088_ND_Value", "NMTH_ND");//非甲烷总烃
            dic.Add("a24088_PFL_Value", "NMTH_PFL");
            dic.Add("a25002_ND_Value", "C6H6_ND");//苯
            dic.Add("a25002_PFL_Value", "C6H6_PFL");
            dic.Add("a25003_ND_Value", "C7H8_ND");//甲苯	
            dic.Add("a25003_PFL_Value", "C7H8_PFL");
            dic.Add("a25005_ND_Value", "C6H4_ND");//二甲苯
            dic.Add("a25005_PFL_Value", "C6H4_PFL");
            dic.Add("BENZENE_SERIES_ND", "BENZENE_SERIES_ND");//苯系物
            dic.Add("BENZENE_SERIES_PFL", "BENZENE_SERIES_PFL");
            dic.Add("PM_ND", "PM_ND");//颗粒物
            dic.Add("PM_PFL", "PM_PFL");
            dic.Add("flowvocs_Value", "STATE_FLOW");//标态流量
            dic.Add("VELOCITY", "VELOCITY");//流速
            dic.Add("a01001_Value", "TEMPERALURE");//温度
            dic.Add("a01002_Value", "HUMIDITY");//湿度
            dic.Add("REMARK", "REMARK");//备注
            return CreateResultTable(table, dic);

        }
        /// <summary>
        /// 构建指定列的数据返回表
        /// </summary>
        /// <param name="table">原表</param>
        /// <param name="dic">key为原表列名，value为新列表，如果找不到对应原列名的列，将创建新的列</param>
        /// <returns></returns>
        public static DataTable CreateResultTable(DataTable table, Dictionary<string, string> dic)
        {
            string[] columnNameArray = dic.Values.ToArray();
            List <DataColumn> deleteColumn = new List<DataColumn>();
            foreach (DataColumn column in table.Columns)
            {
                if (dic.Keys.FirstOrDefault(c => column.ColumnName == c) == null)
                {
                    deleteColumn.Add(column);
                }
            }
            foreach (var item in deleteColumn)
            {
                table.Columns.Remove(item);
            }
            foreach (var item in dic.Keys)
            {
                if(table.Columns[item]!=null) table.Columns[item].ColumnName = dic[item];
            }
            //填充空白列
            foreach (var columnName in columnNameArray)
            {
                if (table.Columns[columnName] == null)
                {
                    table.Columns.Add(new DataColumn(columnName));
                }
            }
            return table;
        }
        /// <summary>
        /// 企业类型 1:污染源 2:VOCs
        /// </summary>
        /// <param name="companyType"></param>
        /// <param name="companyIDList"></param>
        /// <returns></returns>
        public static DataTable CompanyNetwork(string companyType,List<string>companyIDList)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select rownum,c1.id,c1.name,c1.company_Type_text,c1.bastype_text,c1.WRYLB_text,c1.gzcd_text ,nvl(c1.pk_Count,0) pk_Count,nvl(p1.pk_Network_Count,0) pk_Network_Count from ( ");
                sql.Append(" select c.id,c.name,d1.title company_Type_text,d2.title bastype_text,d3.title WRYLB_text,d4.title gzcd_text ,count(pk.id) pk_Count  from t_base_company c ");
                sql.Append(" left join basdic d1 on c.companytype = d1.code and d1.typecode='CompanyUnitType' ");
                sql.Append(" left join basdic d2 on c.bastype = d2.code and d2.typecode='IndustryType' ");
                sql.Append(" left join basdic d3 on c.WRYLB = d3.code and d3.typecode='WRYType' ");
                sql.Append(" left join basdic d4 on c.GZCD = d4.code and d4.typecode='CompanyGZCD' ");
                if(companyType == "1")
                sql.Append(" left join t_base_company_pk pk on pk.companyid =c.id and ( pk.type='1' or  pk.type='2') ");
                else
                sql.Append(" left join t_base_company_pk pk on pk.companyid =c.id and pk.type='3' ");
            sql.Append(string.Format("  group by (  c.id, c.name,d1.title ,d2.title ,d3.title  ,d4.title ) having count(pk.id)>0 and c.id in ({0})",StringHelper.SqlInCondition(companyIDList)));
            sql.Append("  ) c1 ");
            sql.Append(" left join ( ");
                sql.Append(" select  pk.companyid,count(t.pkid) pk_Network_Count  from t_base_company_pk pk ");
                sql.Append(" left join t_base_company_pk_tx t on pk.id = t.pkid ");
                sql.Append(" left join (select distinct  m.devicecode from t_mid_minute m ) m1 on m1.devicecode = t.mn ");
                if (companyType == "1")
                sql.Append(string.Format(" where m1.devicecode is not null and( pk.type='1' or  pk.type='2') and  pk.companyid in ({0}) ", StringHelper.SqlInCondition(companyIDList)));
                else
                sql.Append(string.Format(" where m1.devicecode is not null and pk.type='3' and  pk.companyid in ({0}) ", StringHelper.SqlInCondition(companyIDList)));
                sql.Append("  group by pk.companyid ");
                sql.Append(" ) p1 on c1.id = p1.companyid");
            SqlModel sqlModel = SqlModel.Select(sql.ToString()).Native();
            return sqlModel.ExecToDataTable();
        }
        /// <summary>
        /// 获取停运时段小时数
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        public static decimal StopHourCount(string deviceCode, string itemCode, DateTime stime, DateTime etime)
        {
            if (string.IsNullOrEmpty(deviceCode) || string.IsNullOrEmpty(itemCode)) return 0;
            DataRow row = SqlModel.Select(T_MID_HOUR_C.ID.CountAs("COUNT"))
                .From(DB.T_MID_HOUR_C)
                .Where(
                    T_MID_HOUR_C.RECTIME.BetweenAnd(stime, etime) 
                    & T_MID_HOUR_C.DEVICECODE == deviceCode 
                    & T_MID_HOUR_C.ITEMCODE.In(StringHelper.SqlInCondition(itemCode)) 
                    & T_MID_HOUR_C.STATUS == "F")
                .ExecToDataRow();
            return (decimal)row[0];
        }
        /// <summary>
        /// 获取无效时段小时数
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="itemCode">多个因子编码，用逗号隔开</param>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        public static decimal InvalidHourCount(string deviceCode, string itemCode, DateTime stime, DateTime etime)
        {
            if (string.IsNullOrEmpty(deviceCode) || string.IsNullOrEmpty(itemCode)) return 0;
            DataRow row = SqlModel.Select(T_MID_HOUR_C.ID.CountAs("COUNT"))
                .From(DB.T_MID_HOUR_C)
                .Where(
                    T_MID_HOUR_C.RECTIME.BetweenAnd(stime, etime) 
                    & T_MID_HOUR_C.DEVICECODE== deviceCode 
                    & T_MID_HOUR_C.ITEMCODE.In(StringHelper.SqlInCondition(itemCode))
                    & T_MID_HOUR_C.STATUS.IsNotNull() & T_MID_HOUR_C.STATUS != "F" & T_MID_HOUR_C.STATUS != "N")
                .ExecToDataRow();
            return (decimal)row[0];
        }
        /// <summary>
        /// 有效数据捕集率
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        public static decimal EffectiveRate(string deviceCode, string itemCode, DateTime stime, DateTime etime)
        {
            int hourCount = (int)(etime - stime).TotalHours;
            decimal stop = StopHourCount(deviceCode, itemCode, stime, etime);
            decimal invalid = InvalidHourCount(deviceCode, itemCode, stime, etime);
            return (hourCount - stop - invalid) * 100 / (hourCount - stop);
        }
        /// <summary>
        /// 季度时间转换
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        public static void QuarterlyTime(int year,int Quarter, out DateTime startDate, out DateTime endDate)
        {
            startDate = new DateTime(year, (Quarter - 1) * 3 + 1, 1);
            endDate = new DateTime(year, Quarter * 3 + 1, 1).AddSeconds(-1);
        }
        /// <summary>
        /// 获取排口信息
        /// </summary>
        /// <param name="pkid">可以逗号分隔传多个</param>
        /// <returns></returns>
        public static List<dynamic> GetPKInfo(List<string> pkid,List<string> pkType)
        {
            FieldModel where = T_BASE_COMPANY_PK.ID.In(StringHelper.SqlInCondition(pkid)) & T_BASE_COMPANY_PK.ISDEL == "0";

            if (pkType!=null && pkType.Count > 0)
            {
                FieldModel orWhere = null;
                pkType.ForEach(item => {
                    orWhere = orWhere | T_BASE_COMPANY_PK.TYPE == item;
                });
                where = where & (orWhere);
            }
            SqlModel sql = SqlModel.Select(
                    T_BASE_COMPANY.ID.As("C_ID")
                    , T_BASE_COMPANY.NAME.As("C_NAME")
                    , T_BASE_COMPANY_PK.ID.As("PK_ID")
                    , T_BASE_COMPANY_PK.NAME.As("PK_NAME")
                    , T_BASE_COMPANY_PK_TX.MN.As("TX_MN")
                    , T_BASE_COMPANY_PK_TX.CLCS.As("TX_CLCS")
                )
                .From(DB.T_BASE_COMPANY_PK)
                .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID & T_BASE_COMPANY_PK.ISDEL =="0")
                .LeftJoin(DB.T_BASE_COMPANY).On(T_BASE_COMPANY_PK.COMPANYID == T_BASE_COMPANY.ID & T_BASE_COMPANY_PK.ISDEL == "0")
                .Where(where);
            return sql.ExecToDynamicList();
        }

        #region  有效数据统计
        /// <summary>
        /// 获取排口 有效数据统计
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        public static DataTable PK_EffectiveData(List<string> pkid, DateTime startTime, DateTime endTime, List<string> pkType)
        {
            List<dynamic> pkList = GetPKInfo(pkid, pkType);
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add(new DataColumn("ROWNUM"));
            resultTable.Columns.Add(new DataColumn("C_ID"));
            resultTable.Columns.Add(new DataColumn("C_NAME"));
            resultTable.Columns.Add(new DataColumn("PK_ID"));
            resultTable.Columns.Add(new DataColumn("PK_NAME"));
            resultTable.Columns.Add(new DataColumn("HOUR_COUNT"));
            resultTable.Columns.Add(new DataColumn("INVALID_HOUR_COUNT"));
            resultTable.Columns.Add(new DataColumn("STOP_HOUR_COUNT"));
            resultTable.Columns.Add(new DataColumn("RATE"));
            int hourCount = (int)(endTime - startTime).TotalHours;
            int index = 0;
            pkList.ForEach(pk => {
                decimal invalidCount = InvalidHourCount(StringHelper.DynamicToString(pk["TX_MN"]), StringHelper.DynamicToString(pk["TX_CLCS"]), startTime, endTime);
                decimal stopCount = StopHourCount(StringHelper.DynamicToString(pk["TX_MN"]), StringHelper.DynamicToString(pk["TX_CLCS"]), startTime, endTime);
                string temp = StringHelper.DynamicToString(pk["TX_CLCS"]);
                int pkHourCount = string.IsNullOrEmpty(temp) ? 0 : temp.Split(',').Where(c => !string.IsNullOrEmpty(temp)).Count() * hourCount;
                decimal effectiveRate = 100;
                if (pkHourCount - stopCount != 0)
                {
                    effectiveRate = (pkHourCount - stopCount - invalidCount) * 100 / (pkHourCount - stopCount);
                }
                DataRow row = resultTable.NewRow();
                row["ROWNUM"] = ++index;
                row["C_ID"] = pk["C_ID"];
                row["C_NAME"] = pk["C_NAME"];
                row["PK_ID"] = pk["PK_ID"];
                row["PK_NAME"] = pk["PK_NAME"];
                row["HOUR_COUNT"] = pkHourCount;
                row["INVALID_HOUR_COUNT"] = invalidCount;
                row["STOP_HOUR_COUNT"] = stopCount;
                row["RATE"] = effectiveRate;
                resultTable.Rows.Add(row);
            });
            return resultTable;
        }
        /// <summary>
        /// 获取污染源排口 有效数据统计
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable PK_PollutionEffectiveData(List<string> pkid, DateTime startTime, DateTime endTime)
        {
            return PK_EffectiveData(pkid, startTime, endTime, new List<string>() { "1", "2" });
        }
        /// <summary>
        /// 获取VOCs排口 有效数据统计
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable PK_VocsEffectiveData(List<string> pkid, DateTime startTime, DateTime endTime)
        {
            return PK_EffectiveData(pkid, startTime, endTime, new List<string>() { "3" });
        }
        #endregion
        #region 传输率
        /// <summary>
        /// 获取因子数据个数
        /// </summary>
        /// <param name="dataFrom">1:未审核数据 2:审核数据</param>
        /// <param name="deviceCode">设备编码</param>
        /// <param name="itemCode">因子编码</param>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <param name="dataType">day:日数据、hour:小时数据</param>
        /// <returns></returns>
        public static decimal ItemDataCount(string dataFrom, string deviceCode, string itemCode, DateTime stime, DateTime etime, string dataType)
        {
            if (string.IsNullOrEmpty(deviceCode) || string.IsNullOrEmpty(itemCode)) return 0;
            StringBuilder sql = new StringBuilder();
            string tableName = "";
            if ("day" == dataType)
            {
                tableName = "t_mid_day";
            }
            else if ("hour" == dataType && "002" == dataFrom)
            {
                tableName = "t_mid_hour";
            }
            else if ("hour" == dataType && "001" == dataFrom)
            {
                tableName = "t_mid_hour_c";
            }
            sql.Append(string.Format(" select count(h.id) count from {0} h", tableName));
            sql.Append(string.Format(" where h.rectime>= to_date('{0}','yyyy-mm-dd hh24:mi:ss') and h.rectime<= to_date('{1}','yyyy-mm-dd hh24:mi:ss')", stime.ToString("yyyy-MM-dd:HH:mm:ss"), etime.ToString("yyyy-MM-dd:HH:mm:ss")));
            sql.Append(string.Format(" and h.devicecode='{0}'", deviceCode));
            sql.Append(string.Format(" and h.itemcode in({0})", StringHelper.SqlInCondition(itemCode)));
            object result = SqlModel.Select(sql.ToString()).Native().ExecuteScalar();
            return decimal.Parse(result.ToString());
        }
        /// <summary>
        /// 排口传输率
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pkType">day:日数据、hour:小时数据</param>
        /// <returns></returns>
        public static DataTable PK_ReportRate(List<string> pkid, DateTime startTime, DateTime endTime, List<string> pkType, string dataFrom, string dataType)
        {
            List<dynamic> pkList = GetPKInfo(pkid, pkType);
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add(new DataColumn("ROWNUM"));
            resultTable.Columns.Add(new DataColumn("C_ID"));
            resultTable.Columns.Add(new DataColumn("C_NAME"));
            resultTable.Columns.Add(new DataColumn("PK_ID"));
            resultTable.Columns.Add(new DataColumn("PK_NAME"));
            resultTable.Columns.Add(new DataColumn("REAL_DATA_COUNT"));
            resultTable.Columns.Add(new DataColumn("THEORY_DATA_COUNT"));
            resultTable.Columns.Add(new DataColumn("RATE"));
            int timeCount = 0;
            if ("day" == dataType)
            {
                timeCount = (int)Math.Ceiling((endTime - startTime).TotalDays);
            }
            else if ("hour" == dataType)
            {
                timeCount = (int)Math.Ceiling((endTime - startTime).TotalHours);
            }
            int index = 0;
            var allMonitorItemList = GetItemInfo(MONITOR_ITEM_TYPE);
            pkList.ForEach(pk => {
                decimal realDataCount = ItemDataCount(dataFrom, StringHelper.DynamicToString(pk["TX_MN"]), StringHelper.DynamicToString(pk["TX_CLCS"]), startTime, endTime, dataType);
                List<string> monitorItemCodeList = new List<string>(StringHelper.DynamicToString(pk["TX_CLCS"]).Split(','));
                int monitorItemCount = allMonitorItemList.Count(c => monitorItemCodeList.Contains(c.CODE));
                int pkHourCount = monitorItemCount * timeCount;
                decimal effectiveRate = 100;
                if (pkHourCount != 0)
                {
                    effectiveRate = realDataCount * 100 / pkHourCount;
                }
                DataRow row = resultTable.NewRow();
                row["ROWNUM"] = ++index;
                row["C_ID"] = pk["C_ID"];
                row["C_NAME"] = pk["C_NAME"];
                row["PK_ID"] = pk["PK_ID"];
                row["PK_NAME"] = pk["PK_NAME"];
                row["REAL_DATA_COUNT"] = realDataCount;
                row["THEORY_DATA_COUNT"] = pkHourCount;
                row["RATE"] = effectiveRate;
                resultTable.Rows.Add(row);
            });
            return resultTable;
        }
        /// <summary>
        /// 污染源排口传输率
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable PK_PollutionReportRate(List<string> pkid, DateTime startTime, DateTime endTime, string dataFrom, string dataType)
        {
            DataTable table = PK_ReportRate(pkid, startTime, endTime, new List<string>() { "1", "2" }, dataFrom, dataType);
            return table;
        }
        /// <summary>
        /// VOCs排口传输率
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable PK_VocsReportRate(List<string> pkid, DateTime startTime, DateTime endTime, string dataFrom, string dataType)
        {
            DataTable table = PK_ReportRate(pkid, startTime, endTime, new List<string>() { "3" }, dataFrom, dataType);
            return table;
        }
        #endregion
        #region 告警统计
        /// <summary>
        /// 获取告警
        /// </summar
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="type">P:排口 C:企业</param>
        /// <param name="id">排口id、企业id </param>
        /// <param name="dataType">day:日数据、hour:小时数据、month：月数据</param>
        /// <param name="dataClass">GasItem:废气、WaterItem:废水、VOC:VOCsItem，多种类型可以使用逗号分隔传入 </param>
        /// <returns></returns>
        public static DataTable GetWarnData(DateTime startTime, DateTime endTime, string type, string id, string dataType, string dataClass)
        {
            StringBuilder sql = new StringBuilder();
            if ("P" == type)
                sql.Append(" select t.pkid MIAN_ID,pk.name MIAN_NAME,t.companyid,t.pkid,t.itemcode,t.starttime,t.endtime,c.name,pk.name,t.type from t_mid_alert t ");
            else
                sql.Append(" select t.companyid MIAN_ID,C.name MIAN_NAME,t.companyid,t.pkid,t.itemcode,t.starttime,t.endtime,c.name,pk.name,t.type from t_mid_alert t ");
            sql.Append(" left join t_base_company c on c.id = t.companyid ");
            sql.Append(" left join t_base_company_pk pk on pk.id = t.pkid ");
            sql.Append(string.Format(" WHERE T.STARTTIME >= to_date('{0}', 'yyyy-mm-dd hh24:mi:ss') AND T.STARTTIME <= to_date('{1}', 'yyyy-mm-dd hh24:mi:ss') ", startTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss")));
            if ("P" == type)
                sql.Append(string.Format(" AND pk.id in ({0}) ", StringHelper.SqlInCondition(id)));
            else
                sql.Append(string.Format(" AND c.id in ({0}) ", StringHelper.SqlInCondition(id)));
            sql.Append(" order by t.starttime");
            SqlModel sqlModel = SqlModel.Select(sql.ToString()).Native();
            List<dynamic> list = sqlModel.ExecToDynamicList();
            DataTable table = DataExchangeWarn(list, dataClass, dataType);
            return DataHandle.TableAddBlankRow(table, startTime, endTime, dataType);
        }
        /// <summary>
        /// 告警数据转换
        /// </summary>
        /// <param name="list"></param>
        /// <param name="dataClass"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private static DataTable DataExchangeWarn(List<dynamic> sourceList, string dataClass, string dataType)
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("DATA_TIME"));
            table.Columns.Add(new DataColumn("DATA_TIME_ORDER",typeof (DateTime))); 
            table.Columns.Add(new DataColumn("ID"));
            table.Columns.Add(new DataColumn("NAME"));
            List<ItemInfo> itemInfolist = GetItemInfo(dataClass.Split(',').ToList());
            List<string> codeList = new List<string>();
            itemInfolist.ForEach(itemInfo => {
                if (codeList.Contains(itemInfo.CODE)) return;
                codeList.Add(itemInfo.CODE);
                table.Columns.Add(new DataColumn(itemInfo.CODE));
                table.Columns.Add(new DataColumn(string.Format("{0}_TITLE", itemInfo.CODE)));
                table.Columns.Add(new DataColumn(string.Format("{0}_CB_Value", itemInfo.CODE)));
                table.Columns.Add(new DataColumn(string.Format("{0}_YC_Value", itemInfo.CODE)));
            });
            if (!(sourceList != null && sourceList.Count > 0))
            {
                return table;
            }
            List<string> existList = new List<string>();
            sourceList.ForEach(item => {
                string rectime = StringHelper.DynamicToString(item["STARTTIME"]);
                string id = StringHelper.DynamicToString(item["MIAN_ID"]);//企业ID或排口ID
                string name = StringHelper.DynamicToString(item["MIAN_NAME"]);//企业名称或排口名称
                if (string.IsNullOrEmpty(rectime)) return;
                string dataTime = string.Empty;
                switch (dataType)
                {
                    case "hour":
                        dataTime = DateTime.Parse(rectime).ToString("yyyyMMddHH"); break;
                    case "day":
                        dataTime = DateTime.Parse(rectime).ToString("yyyyMMdd"); break;
                    case "month":
                        dataTime = DateTime.Parse(rectime).ToString("yyyyMM"); break;
                }
                string dataGuid = string.Format("{0}_{1}", dataTime, id);
                if (existList.Exists(c => c == dataGuid)) return;
                existList.Add(dataGuid);
                bool hasData = false;
                DataRow row = table.NewRow();
                row["DATA_TIME"] = DateTime.Parse(rectime).ToString("yyyy-MM-dd HH:mm:ss");
                row["DATA_TIME_ORDER"] = DateTime.Parse(rectime).ToString("yyyy-MM-dd HH:mm:ss");
                row["ID"] = id;
                row["NAME"] = name;
                itemInfolist.ForEach(itemInfo => {
                    GetItemValueForWarn(sourceList, itemInfo, dataTime, id, row, ref hasData, dataType);

                });
                if (hasData) table.Rows.Add(row);
            });
            return table;
        }
        private static void GetItemValueForWarn(List<dynamic> list, ItemInfo itemInfo, string rectime, string id, DataRow row, ref bool hasData, string dataType)
        {
            int cbCount = 0;
            int ycCount = 0;
            string dateFormat = "";
            switch (dataType)
            {
                case "hour":dateFormat = "yyyyMMddHH";break;
                case "day":dateFormat = "yyyyMMdd";break;
                case "month":dateFormat = "yyyyMM";break;
                default:dateFormat = "yyyyMMddHHmmss";break;
            }
            
            cbCount = list.Count(c =>
                StringHelper.DynamicToString(c["MIAN_ID"]) == id
                && DateTime.Parse(StringHelper.DynamicToString(c["STARTTIME"])).ToString(dateFormat) == rectime
                && StringHelper.DynamicToString(c["ITEMCODE"]) == itemInfo.CODE
                && StringHelper.DynamicToString(c["TYPE"]) == "0");
            ycCount = list.Count(c =>
                StringHelper.DynamicToString(c["MIAN_ID"]) == id
                && DateTime.Parse(StringHelper.DynamicToString(c["STARTTIME"])).ToString(dateFormat) == rectime
                && StringHelper.DynamicToString(c["ITEMCODE"]) == itemInfo.CODE
                && StringHelper.DynamicToString(c["TYPE"]) == "1");
            if (cbCount>0 ||ycCount>0)
            {
                hasData = true;
                string columnCodeName = itemInfo.CODE;
                string columnTitleName = string.Format("{0}_TITLE", itemInfo.CODE);
                string columnCBName = string.Format("{0}_CB_Value", itemInfo.CODE);
                string columnYCName = string.Format("{0}_YC_Value", itemInfo.CODE);
                row[columnCodeName] = itemInfo.CODE;
                row[columnTitleName] = itemInfo.TITLE;
                if(cbCount>0) row[columnCBName] = cbCount;
                if(ycCount>0) row[columnYCName] = ycCount;

            }
        }
        public static DataTable GetPollutionWarnData(DateTime startTime, DateTime endTime, string type, string id, string dataType)
        {
            DataTable table = GetWarnData(startTime, endTime, type, id, dataType, "GasItem,WaterItem");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("DATA_TIME", "DATA_TIME");
            dic.Add("PH_CB", "PH_CB");//PH
            dic.Add("PH_YC", "PH_YC");
            dic.Add("w21003_CB_Value", "AMMONIA_NITROGEN_CB");//氨氮
            dic.Add("w21003_YC_Value", "AMMONIA_NITROGEN_YC");
            dic.Add("w01018_CB_Value", "COD_CB");//化学需氧量
            dic.Add("w01018_YC_Value", "COD_YC");
            dic.Add("a34013_CB_Value", "PM_CB");//烟尘
            dic.Add("a34013_YC_Value", "PM_YC");
            dic.Add("a21026_CB_Value", "SO2_CB");//二氧化硫
            dic.Add("a21026_YC_Value", "SO2_YC");
            dic.Add("a21002_CB_Value", "a21002_CB");//氮氧化物
            dic.Add("a21002_YC_Value", "a21002_YC");
            return CreateResultTable(table, dic);
        }
        public static DataTable GetVocsWarnData(DateTime startTime, DateTime endTime, string type, string id, string dataType)
        {
            DataTable table = GetWarnData(startTime, endTime, type, id, dataType, "VOCsItem");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("DATA_TIME", "DATA_TIME");
            dic.Add("a24088_CB_Value", "NMTH_CB");//非甲烷总烃
            dic.Add("a24088_YC_Value", "NMTH_YC");
            dic.Add("a25002_CB_Value", "C6H6_CB");//苯
            dic.Add("a25002_YC_Value", "C6H6_YC");
            dic.Add("a25003_CB_Value", "C7H8_CB");//甲苯	
            dic.Add("a25003_YC_Value", "C7H8_YC");
            dic.Add("a25005_CB_Value", "C6H4_CB");//二甲苯
            dic.Add("a25005_YC_Value", "C6H4_YC");
            return CreateResultTable(table, dic);
        }
        #endregion
        #region 排量预警统计
        /// <summary>
        /// 获取排口排放量预警设置
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        private static List<dynamic> GetPK_PFL_WarnSet(List<string> pkid)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select pk.id,pk.name,c.id cid,c.name cname,asi.itemcode,asi.subitemcode,asi.limitday,asi.limitmonth,asi.limityear,pktx.mn devicecode,b.title from t_base_company_pk pk ");
            sql.Append(" left join t_base_company c on pk.companyid = c.id ");
            sql.Append(" left join t_base_company_asi asi on pk.id = asi.pkid and asi.state=1 and asi.subitemcode='PFL' ");
            sql.Append(" left join t_base_company_pk_tx pktx on pk.id = pktx.pkid ");
            sql.Append(" left join basdic b on b.code = asi.itemcode and  b.typecode in ('GasItem','WaterItem','VOCsItem') ");
            sql.Append(string.Format(" where pk.id in ({0}) order by c.id", StringHelper.SqlInCondition(pkid)));
            return SqlModel.Select(sql.ToString()).Native().ExecToDynamicList();
        }
        /// <summary>
        /// 获取设备排放量的监测数据统计
        /// </summary>
        /// <param name="deviceCodeList"></param>
        /// <param name="stateTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private static List<dynamic> GetDevicePFLMonitorData(List<string> deviceCodeList,DateTime stateTime ,DateTime endTime)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" select d.devicecode,d.itemcode,sum(d.value) itemvalue from t_mid_day d where   ");
            sql.Append(string.Format(" d.devicecode in ({0}) and d.rectime >= to_date('{1}','yyyy-mm-dd hh24:mi:ss') and d.rectime <= to_date('{2}','yyyy-mm-dd hh24:mi:ss')  and d.subitemcode ='PFL' group by d.devicecode,d.itemcode",StringHelper.SqlInCondition(deviceCodeList),stateTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss")));
            return SqlModel.Select(sql.ToString()).Native().ExecToDynamicList();
        }
        /// <summary>
        /// 获取排放量预警
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="stateTime"></param>
        /// <param name="endTime"></param>
        /// <param name="itemType">GasItem、WaterItem、VOCsItem</param>
        /// <param name="dateType">custom、day、month、year</param>
        /// <returns></returns>
        public static DataTable GetPK_PFL_Warn(List<string> pkid, DateTime stateTime, DateTime endTime,List<string> itemType,string dateType)
        {
            List<dynamic> pkSet = GetPK_PFL_WarnSet(pkid);
            List<string> deviceCodeList = pkSet.Select(item => 
            {
                string code = StringHelper.DynamicToString(item["DEVICECODE"]);
                return code;
            }).ToList();
            List<dynamic> pflList = GetDevicePFLMonitorData(deviceCodeList, stateTime, endTime);
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("ROWNUM"));
            table.Columns.Add(new DataColumn("NAME"));
            table.Columns.Add(new DataColumn("ITEM_NAME"));
            table.Columns.Add(new DataColumn("LIMIT_VALUE"));
            table.Columns.Add(new DataColumn("PFL_VALUE"));
            table.Columns.Add(new DataColumn("PERCENT_VALUE"));
            List<ItemInfo> itemInfoList = GetItemInfo(itemType);
            int index = 0;
            pkSet.ForEach(item=> {
                
                DataRow row = table.NewRow();
                row["ROWNUM"] = ++index;
                row["NAME"] = item["CNAME"] + "  " + item["NAME"];
                switch (dateType)
                {
                    case "custom": row["LIMIT_VALUE"] = item["LIMITYEAR"];break;
                    case "day": row["LIMIT_VALUE"] = item["LIMITDAY"]; break;
                    case "month": row["LIMIT_VALUE"] = item["LIMITMONTH"]; break;
                    case "year": row["LIMIT_VALUE"] = item["LIMITYEAR"]; break;
                }               
                row["ITEM_NAME"] = item["title".ToUpper()];
                if (!itemInfoList.Exists(info => info.CODE == StringHelper.DynamicToString(item["itemcode".ToUpper()])))//排口类型与监测已经对不上,就不计算排量（数据不匹配的情况）
                {
                    table.Rows.Add(row);
                    return;
                }
                dynamic pflInfo = pflList.Find(pfl =>
                    StringHelper.DynamicToString(pfl["devicecode".ToUpper()]) == StringHelper.DynamicToString(item["devicecode".ToUpper()])
                    && StringHelper.DynamicToString(pfl["itemcode".ToUpper()]) == StringHelper.DynamicToString(item["itemcode".ToUpper()])
                    );
                if (null != pflInfo)
                {
                    try
                    {
                        decimal pfl = StringHelper.DynamicToDecimal(pflInfo["itemvalue".ToUpper()]);
                        decimal limit = StringHelper.DynamicToDecimal(row["LIMIT_VALUE"]);
                        row["PFL_VALUE"] = pfl;
                        row["PERCENT_VALUE"] = string.Format("{0}%", (pfl * 100 / limit ).ToString("0.00"));//去掉两位小数后的数据
                    }
                    catch { }
                }
                table.Rows.Add(row);
            });
            return table;
        }
        #endregion
    }
}
