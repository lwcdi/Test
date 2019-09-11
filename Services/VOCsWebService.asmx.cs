using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using w.Model;
using w.ORM;

namespace UI.Web.Services
{
    /// <summary>
    /// VOCsWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class VOCsWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string ApplyMinData(string jsonData)
        {
            try
            {
                T_MID_MINUTEModel data = StringExtensions.Deserialize<T_MID_MINUTEModel>(jsonData);
                data.CREATETIME = DateTime.Now;
                bool result = data.Insert();
                if (result)
                {
                    //HandleChaoBiao(data, "minute");
                }
                return "true";
            }
            catch
            {

            }
            return "false";
        }
        [WebMethod]
        public string ApplyHourData(string jsonData)
        {
            try
            {
                T_MID_HOURModel data = StringExtensions.Deserialize<T_MID_HOURModel>(jsonData);
                data.CREATETIME = DateTime.Now;
                bool result = data.Insert();
                if (result)
                {
                    //HandleChaoBiao(data, "hour");
                    T_MID_HOUR_CModel hour_c = StringExtensions.Deserialize<T_MID_HOUR_CModel>(jsonData);
                    hour_c.CREATETIME = DateTime.Now;
                    hour_c.Insert();
                }
                return "true";
            }
            catch
            {

            }
            return "false";
        }
        [WebMethod]
        public string ApplyDayData(string jsonData)
        {
            try
            {
                T_MID_DAYModel data = StringExtensions.Deserialize<T_MID_DAYModel>(jsonData);
                data.CREATETIME = DateTime.Now;
                bool result = data.Insert();
                if (result)
                {
                    //HandleChaoBiao(data, "day");
                }
                return "true";
            }
            catch
            {

            }
            return "false";
        }
        //private void HandleChaoBiao(DBDataModel data, string type)
        //{
        //    if (type == "minute")
        //    {
        //        T_MID_VOC_MINUTEModel curData = (T_MID_VOC_MINUTEModel)data;

        //        if (curData.SUBITEM == "ND")
        //        {//浓度
        //            var devInfo = SqlModel.Select(T_COMPANY.NAME.As("COMPANYNAME"), T_COMPANY.ID.As("COMPANYID"), T_COMPANY.AREACODE, T_BAS_AIR_STATION_POINT.POINT_CODE, T_BAS_AIR_STATION_POINT.POINT_NAME, T_COMPANY_STATION_DIVICE.DEVICE_CODE, T_COMPANY_STATION_DIVICE.DEVICE_NAME)
        //            .From(DB.T_COMPANY_STATION_DIVICE)
        //            .LeftJoin(DB.T_BAS_AIR_STATION_POINT).On(T_COMPANY_STATION_DIVICE.POINT_CODE == T_BAS_AIR_STATION_POINT.POINT_CODE)
        //            .LeftJoin(DB.T_COMPANY).On(T_COMPANY.ID == T_BAS_AIR_STATION_POINT.COMPANY_ID)
        //            .Where(T_COMPANY_STATION_DIVICE.DEVICE_CODE == curData.DEVICECODE).ExecToDynamic();

        //            var limit = SqlModel.SelectAll().From(DB.T_COMPANY_STATION_WARN_CONFIG)
        //                .Where(T_COMPANY_STATION_WARN_CONFIG.POINT_CODE == devInfo["POINT_CODE"]
        //                & T_COMPANY_STATION_WARN_CONFIG.MONITOR_CODE == curData.ITEM).ExecToDynamic();

        //            DataRow pointWarnMemoryRow = SqlModel.SelectAll().From(DB.T_DATA_WARNING_MEMORY)
        //                .Where(T_DATA_WARNING_MEMORY.POINTCODE == devInfo["POINT_CODE"]
        //                & T_DATA_WARNING_MEMORY.ITEM == curData.ITEM
        //                & T_DATA_WARNING_MEMORY.SUBITEM == curData.SUBITEM
        //                ).ExecToDataRow();
        //            T_DATA_WARNING_MEMORYModel warnMemory = new T_DATA_WARNING_MEMORYModel();
        //            decimal EXCESSIVE_LIMIT = 0;
        //            if (limit["EXCESSIVE_LIMIT"] != null)
        //                decimal.TryParse(limit["EXCESSIVE_LIMIT"], out EXCESSIVE_LIMIT);
        //            if (curData.VALUE > EXCESSIVE_LIMIT)
        //            {//小时排量超标
        //                decimal warnMemoryID = -1;
        //                if (pointWarnMemoryRow == null)
        //                {
        //                    warnMemory.BEGINTIME = curData.DATATIME;//告警时间
        //                    warnMemory.POINTCODE = devInfo["POINT_CODE"];
        //                    warnMemory.COMPANYID = devInfo["COMPANYID"];
        //                    warnMemory.AREACODE = devInfo["AREACODE"];
        //                    warnMemory.COMPANY_NAME = devInfo["COMPANYNAME"];
        //                    warnMemory.POINT_NAME = devInfo["POINT_NAME"];
        //                    warnMemory.DEVICE_NAME = devInfo["DEVICE_NAME"];
        //                    warnMemory.ITEM = curData.ITEM;
        //                    warnMemory.SUBITEM = curData.SUBITEM;
        //                    warnMemory.VALUE = curData.VALUE;
        //                    warnMemory.LIMITVALUE = limit["EXCESSIVE_LIMIT"];
        //                    warnMemory.ENDTIME = curData.DATATIME;
        //                    warnMemory.DATACOUNT = 1;
        //                    warnMemoryID = warnMemory.GetIDByInsert();
        //                }
        //                else
        //                {
        //                    warnMemoryID = (decimal)pointWarnMemoryRow["ID"];
        //                    warnMemory.DATACOUNT += 1;
        //                    warnMemory.ENDTIME = curData.DATATIME;
        //                    warnMemory.Update(T_DATA_WARNING_MEMORY.ID == warnMemoryID);
        //                }

        //                decimal TIMES = 0;
        //                if (limit["TIMES"] != null)
        //                    decimal.TryParse(limit["TIMES"], out TIMES);
        //                decimal continueMins =(decimal) (warnMemory.ENDTIME - warnMemory.BEGINTIME).Value.TotalMinutes;

        //                if (continueMins >= TIMES
        //                    && warnMemory.DATACOUNT >= TIMES)
        //                {
        //                    //产生告警事件
        //                    T_DATA_WARNINGModel warn = new T_DATA_WARNINGModel();
        //                    warn.STATE = "0";
        //                    warn.CODE = Guid.NewGuid().ToString();
        //                    warn.WARNINGTIME = curData.DATATIME;//告警时间
        //                    warn.WARNINGTYPE = "CB";//超标
        //                    warn.POINTCODE = devInfo["POINT_CODE"];
        //                    warn.COMPANYID = devInfo["COMPANYID"];
        //                    warn.AREACODE = devInfo["AREACODE"];
        //                    warn.OBJECTTYPE = "1";//站点告警对象
        //                    warn.WARNINGDURATION = (decimal)continueMins;//持续时长
        //                    warn.WARNINGCONTENT = string.Format("[{0}][{1}]站点[{2}]设备[{3}]因子 时间：{4}的小时数据发生超标告警", devInfo["NAME"], devInfo["POINT_NAME"], devInfo["DEVICE_NAME"], curData.ITEM, curData.DATATIME.ToString("yy-MM-dd HH"));//告警内容
        //                    int warnId = warn.GetIDByInsert();
        //                    //更新缓存表
        //                    warnMemory.WARNINGID = warnId;
        //                    warnMemory.Update(T_DATA_WARNING_MEMORY.ID == warnMemoryID);
        //                }
        //            }
        //            else
        //            {
        //                if (pointWarnMemoryRow != null)
        //                {
        //                    //从告警状态切换到非告警,移除告警缓存记录
        //                    warnMemory.Delete(T_DATA_WARNING_MEMORY.ID == pointWarnMemoryRow["ID"]);
        //                    //更新告警事件的持续时间
        //                    T_DATA_WARNINGModel warn = new T_DATA_WARNINGModel();
        //                    warn.WARNINGDURATION = (decimal)((DateTime)pointWarnMemoryRow["ENDTIME"] - (DateTime)pointWarnMemoryRow["BEGINTIME"]).TotalMinutes;//持续时长,分钟
        //                    warn.Update(T_DATA_WARNING.ID == pointWarnMemoryRow["WARNINGID"]);
        //                }
        //            }
        //        }
        //    }
        //    else if (type == "hour")
        //    {
        //        T_MID_VOC_HOURModel curData = (T_MID_VOC_HOURModel)data;

        //        if (curData.SUBITEM == "PFL")
        //        {//排放量

        //            var devInfo = SqlModel.Select(T_COMPANY.NAME, T_COMPANY.ID.As("COMPANYID"), T_COMPANY.AREACODE, T_BAS_AIR_STATION_POINT.POINT_CODE, T_BAS_AIR_STATION_POINT.POINT_NAME, T_COMPANY_STATION_DIVICE.DEVICE_CODE, T_COMPANY_STATION_DIVICE.DEVICE_NAME)
        //                .From(DB.T_COMPANY_STATION_DIVICE)
        //                .LeftJoin(DB.T_BAS_AIR_STATION_POINT).On(T_COMPANY_STATION_DIVICE.POINT_CODE == T_BAS_AIR_STATION_POINT.POINT_CODE)
        //                .LeftJoin(DB.T_COMPANY).On(T_COMPANY.ID == T_BAS_AIR_STATION_POINT.COMPANY_ID)
        //                .Where(T_COMPANY_STATION_DIVICE.DEVICE_CODE == curData.DEVICECODE).ExecToDynamic();

        //            var limit = SqlModel.SelectAll().From(DB.T_COMPANY_STATION_WARN_CONFIG)
        //                .Where(T_COMPANY_STATION_WARN_CONFIG.POINT_CODE == devInfo["POINT_CODE"] & T_COMPANY_STATION_WARN_CONFIG.MONITOR_CODE == curData.ITEM).ExecToDynamic();
        //            decimal EXCESSIVE_LIMIT = 0;
        //            if (limit["EXCESSIVE_LIMIT"] != null)
        //                decimal.TryParse(limit["EXCESSIVE_LIMIT"], out EXCESSIVE_LIMIT);
        //            if (curData.VALUE > EXCESSIVE_LIMIT)
        //            {//小时排量超标
        //                T_DATA_WARNINGModel warn = new T_DATA_WARNINGModel();
        //                warn.STATE = "0";
        //                warn.CODE = DateTime.Now.ToString("yyMMddHHmmssfff");
        //                warn.WARNINGTIME = curData.DATATIME;//告警时间
        //                warn.WARNINGTYPE = "CB";//超标
        //                warn.POINTCODE = devInfo["POINT_CODE"];
        //                warn.COMPANYID = devInfo["COMPANYID"];
        //                warn.AREACODE = devInfo["AREACODE"];
        //                warn.OBJECTTYPE = "1";//站点告警对象
        //                warn.WARNINGCONTENT = string.Format("[{0}][{1}]站点[{2}]设备[{3}]因子 时间：{4}的小时数据发生超标告警", devInfo["NAME"], devInfo["POINT_NAME"], devInfo["DEVICE_NAME"], curData.ITEM, curData.DATATIME.ToString("yy-MM-dd HH"));//告警内容
        //                warn.Insert();
        //            }
        //        }
        //    }
        //    else if (type == "day")
        //    {
        //        T_MID_VOC_DAYModel curData = (T_MID_VOC_DAYModel)data;

        //        if (curData.SUBITEM == "PFL")
        //        {//排放量
        //            var devInfo = SqlModel.Select(T_COMPANY.NAME, T_COMPANY.ID.As("COMPANYID"), T_COMPANY.AREACODE, T_BAS_AIR_STATION_POINT.POINT_CODE, T_BAS_AIR_STATION_POINT.POINT_NAME, T_COMPANY_STATION_DIVICE.DEVICE_CODE, T_COMPANY_STATION_DIVICE.DEVICE_NAME)
        //                .From(DB.T_COMPANY_STATION_DIVICE)
        //                .LeftJoin(DB.T_BAS_AIR_STATION_POINT).On(T_COMPANY_STATION_DIVICE.POINT_CODE == T_BAS_AIR_STATION_POINT.POINT_CODE)
        //                .LeftJoin(DB.T_COMPANY).On(T_COMPANY.ID == T_BAS_AIR_STATION_POINT.COMPANY_ID)
        //                .Where(T_COMPANY_STATION_DIVICE.DEVICE_CODE == curData.DEVICECODE).ExecToDynamic();

        //            var limit = SqlModel.SelectAll().From(DB.T_COMPANY_STATION_WARN_CONFIG)
        //                .Where(T_COMPANY_STATION_WARN_CONFIG.POINT_CODE == devInfo["POINT_CODE"] & T_COMPANY_STATION_WARN_CONFIG.MONITOR_CODE == curData.ITEM).ExecToDynamic();
        //            decimal EXCESSIVE_LIMIT = 0;
        //            if (limit["EXCESSIVE_LIMIT"] != null)
        //                decimal.TryParse(limit["EXCESSIVE_LIMIT"], out EXCESSIVE_LIMIT);
        //            if (curData.VALUE > EXCESSIVE_LIMIT)
        //            {//日排量超标
        //                T_DATA_WARNINGModel warn = new T_DATA_WARNINGModel();
        //                warn.STATE = "0";
        //                warn.CODE = DateTime.Now.ToString("yyMMddHHmmssfff");
        //                warn.WARNINGTIME = curData.DATATIME;//告警时间
        //                warn.WARNINGTYPE = "CB";//超标
        //                warn.POINTCODE = devInfo["POINT_CODE"];
        //                warn.COMPANYID = devInfo["COMPANYID"];
        //                warn.AREACODE = devInfo["AREACODE"];
        //                warn.OBJECTTYPE = "1";//站点告警对象
        //                warn.WARNINGCONTENT = string.Format("[{0}][{1}]站点[{2}]设备[{3}]因子 时间：{4}的日数据发生超标告警", devInfo["NAME"], devInfo["POINT_NAME"], devInfo["DEVICE_NAME"], curData.ITEM, curData.DATATIME.ToString("yy-MM-dd"));//告警内容
        //                warn.Insert();
        //            }
        //        }
        //    }
        //}
    }
}
