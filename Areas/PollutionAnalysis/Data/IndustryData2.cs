using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w.Model;
using w.ORM;

namespace UI.Web
{
    public class IndustryData2
    {
        /// <summary>
        /// 获取行业信息
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        private static List<dynamic> GetAreaInfo(string industryCode)
        {
            SqlModel sql = SqlModel.Select(
                    BASDIC.CODE
                    , BASDIC.TITLE
                    )
                .From(DB.BASDIC).Where(BASDIC.CODE.In(StringHelper.SqlInCondition(industryCode)));
            return sql.ExecToDynamicList();
        }
        /// <summary>
        /// 获取行业的企业
        /// </summary>
        /// <param name="areaCode"></param>
        /// <returns></returns>
        private static List<dynamic> GetCompanyInfo(string industryCode)
        {
            SqlModel sql = SqlModel.SelectAll()
                .From(DB.T_BASE_COMPANY)
                .Where(T_BASE_COMPANY.BASTYPE.In(StringHelper.SqlInCondition(industryCode)));
            return sql.ExecToDynamicList();
        }
        /// <summary>
        /// 区域监测数据统计
        /// </summary>
        /// <param name="areaCode">区域编码，多个实用逗号分隔</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="dataType">day:日数据、hour:小时数据、month：月数据</param>
        /// <returns></returns>
        public static List<dynamic> GetAreaMonitorData(string areaCode, DateTime startTime, DateTime endTime,string dataType,string dataClass)
        {
            string[] areaCodeArray = areaCode.Split(',');
            List<dynamic> companyList = GetCompanyInfo(areaCode);
            List<string> companyIDList = new List<string>();
            companyList.ForEach(c=> {
                if(!string.IsNullOrEmpty(StringHelper.DynamicToString(c["ID"])))
                companyIDList.Add(StringHelper.DynamicToString(c["ID"]));
            });
            DataTable table = MonitorData.GetMonitorData("2", startTime, endTime, "C", companyIDList, dataType, dataClass);
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add(new DataColumn("AREA_CODE"));
            resultTable.Columns.Add(new DataColumn("AREA_NAME"));
            resultTable.Columns.Add(new DataColumn("DATA_TIME"));
            List<string> colunmNameList = new List<string>();
            foreach (DataColumn column in table.Columns)
            {
                if (column.ColumnName.Contains("_Value"))
                {
                    resultTable.Columns.Add(new DataColumn(column.ColumnName));
                    colunmNameList.Add(column.ColumnName);
                }
            }
            List<dynamic> monitorData = table.ToDynamicList();
           
            foreach(string code in areaCodeArray)
            {
                List<string> cID = new List<string>();
                try
                {
                    companyList.Where(item => StringHelper.DynamicToString(item["BASTYPE"]) == code).ToList().ForEach(item =>
                    {
                        cID.Add(StringHelper.DynamicToString(item["ID"]));
                    });
                }
                catch(Exception ex) { };
                if (cID.Count < 1) continue;

                var areaCompanyMonitorData = monitorData.Where(item => cID.Contains(StringHelper.DynamicToString(item["ID"]))).ToList();
                List<string> existTime = new List<string>();
                areaCompanyMonitorData.ForEach(item=>{
                    //string dateTime = StringHelper.DynamicToString(item["DATA_TIME"]);
                    if (existTime.Contains(code)) return;
                    existTime.Add(code);
                    //List<dynamic> list = areaCompanyMonitorData.Where(c => dateTime == StringHelper.DynamicToString(c["DATA_TIME"])).ToList();
                    List<dynamic> list = areaCompanyMonitorData;
                    DataRow row = resultTable.NewRow();
                    row["DATA_TIME"] = code;
                    row["AREA_CODE"] = code;
                    row["AREA_NAME"] = GetAreaInfo(code)[0]["TITLE"];
                    colunmNameList.ForEach(column =>
                    {
                        if (string.IsNullOrEmpty(column)) return;
                        string[] itemCodeArray = column.Split('_');
                        string mianItemCode = itemCodeArray[0];
                        string childItemCode = itemCodeArray.Length > 2 ? itemCodeArray[1] : "";
                        //string calculateType = MonitorData.MonitorItemCalculateType(mianItemCode, childItemCode);
                        string calculateType = "+";
                        if ("+" == calculateType)
                        {
                            row[column] = list.Sum(c =>
                            {
                                decimal temp = StringHelper.DynamicToDecimal(c[column]);
                                return temp;
                            }).ToString();
                        }
                        if ("/" == calculateType)
                        {
                            row[column] = list.Average(c =>
                            {
                                decimal temp = StringHelper.DynamicToDecimal(c[column]);
                                return temp;
                            }).ToString();
                        }
                    });
                    resultTable.Rows.Add(row);
                });
            }
            return resultTable.ToDynamicList();
        }
    }
}