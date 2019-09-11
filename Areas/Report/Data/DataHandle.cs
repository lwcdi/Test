using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w.ORM;

namespace UI.Web
{
    /// <summary>
    /// 数据处理
    /// </summary>
    public class DataHandle
    {
        /// <summary>
        /// 设置表数据的展示样式
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="style">
        /// 1 (day) : 00~01
        /// 2 (custom): 1日12时
        /// 3 (month)：1日
        /// 4 (year)：1月
        /// </param>
        public static void TableDataStyle(DataTable table,string columnName,string style)
        {
            if (!(table != null && table.Rows.Count > 0)) return;
            if(style == "1" || style == "day")
            {
                foreach (DataRow row in table.Rows)
                {
                    DateTime time;
                    bool result = DateTime.TryParse(row["DATA_TIME"].ToString(), out time);
                    if (result)
                    {
                        row["DATA_TIME"] = string.Format("{0}~{1}", StringHelper.NumberAddBit(time.Hour, 2), StringHelper.NumberAddBit(time.Hour + 1, 2));
                    };
                }
            }
            if (style == "2" || style == "custom")
            {
                foreach (DataRow row in table.Rows)
                {
                    DateTime time;
                    bool result = DateTime.TryParse(row["DATA_TIME"].ToString(), out time);
                    if (result)
                    {
                        row["DATA_TIME"] = string.Format("{0}日{1}时", StringHelper.NumberAddBit(time.Day, 2), StringHelper.NumberAddBit(time.Hour, 2));
                    };
                }
            }
            if (style == "3" || style == "month")
            {
                foreach (DataRow row in table.Rows)
                {
                    DateTime time;
                    bool result = DateTime.TryParse(row["DATA_TIME"].ToString(), out time);
                    if (result)
                    {
                        row["DATA_TIME"] = string.Format("{0}日", StringHelper.NumberAddBit(time.Day, 2));
                    };
                }
            }
            if (style == "4" || style == "year")
            {
                foreach (DataRow row in table.Rows)
                {
                    DateTime time;
                    bool result = DateTime.TryParse(row["DATA_TIME"].ToString(), out time);
                    if (result)
                    {
                        row["DATA_TIME"] = string.Format("{0}月", StringHelper.NumberAddBit(time.Month, 2));
                    };
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="dateType">custom、day、month、hour</param>
        /// <returns></returns>
        public static DataTable TableAddBlankRow(DataTable table,DateTime startTime,DateTime endTime,string dateType)
        {
            table.DefaultView.Sort = "DATA_TIME_ORDER ASC";
            DateTime currentTime = startTime;
            while (currentTime <= endTime)
            {
                bool exit = false;
                foreach(DataRow row in table.Rows)
                {
                    DateTime rowDataTime;
                    if(DateTime.TryParse(StringHelper.DynamicToString(row["DATA_TIME"]), out rowDataTime))
                    {
                        if("custom" == dateType|| "hour" == dateType)
                        {
                            if (rowDataTime.ToString("yyyy-MM-dd HH") == currentTime.ToString("yyyy-MM-dd HH"))
                            {
                                exit = true;
                                break;
                            }
                        }
                        if("day"== dateType)
                        {
                            if (rowDataTime.ToString("yyyy-MM-dd") == currentTime.ToString("yyyy-MM-dd"))
                            {
                                exit = true;
                                break;
                            }
                        }
                        if ("month" == dateType)
                        {
                            if (rowDataTime.ToString("yyyy-MM") == currentTime.ToString("yyyy-MM"))
                            {
                                exit = true;
                                break;
                            }
                        }
                        if ("year" == dateType)
                        {
                            if (rowDataTime.ToString("yyyy") == currentTime.ToString("yyyy"))
                            {
                                exit = true;
                                break;
                            }
                        }
                    }
                }
                if (!exit)
                {
                    DataRow newRow = table.NewRow();
                    newRow["DATA_TIME"] = currentTime;
                    newRow["DATA_TIME_ORDER"] = currentTime;
                    table.Rows.Add(newRow);
                }
                switch (dateType)
                {
                    case "hour": currentTime = currentTime.AddHours(1); break;
                    case "custom": currentTime = currentTime.AddHours(1);break;
                    case "day": currentTime = currentTime.AddDays(1); break;
                    case "month": currentTime = currentTime.AddMonths(1); break;
                    case "year": currentTime = currentTime.AddYears(1); break;
                    default: currentTime = currentTime.AddYears(100); break;
                }
                
            }
            DataTable resultTable = table.DefaultView.ToTable();
            return resultTable;
        }
    }
}