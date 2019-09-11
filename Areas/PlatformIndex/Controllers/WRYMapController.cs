using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UI.Web.Content.code.Handler;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.PlatformIndex.Controllers
{
    public class WRYMapController : Controller
    {
        //
        // GET: /PlatformIndex/WRYMap/

        public ActionResult CommandMap()
        {
            return View(0);
        }

        //[ActionParam]
        public ActionResult WRYMap(RequestData data)
        {
            ViewData["fs"] = data.Get("fs");
            ViewData["fq"] = data.Get("fq");
            ViewData["vocs"] = data.Get("vocs");
            ViewData["companyOnly"] = data.Get("companyOnly");
            return View(0);
        }
        public ActionResult HisDataAnalysis()
        {
            return View(0);
        }
        public ActionResult DetailInfo()
        {
            return View(0);
        }

        public ActionResult PKInfoFQ()
        {
            return View(0);
        }

        public ActionResult PKInfoFS()
        {
            return View(0);
        }

        public ActionResult PKInfoVOCs()
        {
            return View(0);
        }

        [HttpPost]
        public string GetAllCompanyGPSDatas(RequestData data)
        {
            string areaCode = data.Get("areaCode").Trim();
            if (string.IsNullOrEmpty(areaCode))
            {
                areaCode = CurrentUser.SysAreaCode;
            }
            string IsVOCs = data.Get("IsVOCs").Trim(); //企业类型，0-污染源，1-VOCS
            if (string.IsNullOrEmpty(IsVOCs))
            {
                IsVOCs = "0";
            }
            string monitorItem = data.Get("Item").Trim(); //计算排放量的监测因子
            if (string.IsNullOrEmpty(monitorItem))
            {
                monitorItem = "all";
            }
            var where = T_BASE_COMPANY.CITY.RightLike(areaCode) & T_BASE_COMPANY.ISVOCS == IsVOCs & T_BASE_COMPANY.ISDEL == "0" & T_BASE_COMPANY.ISDEL == "0";

            //查询所有企业信息
            var dtPoints = SqlModel.Select(T_BASE_COMPANY.ID, T_BASE_COMPANY.NAME.As("NAME"), T_BASE_COMPANY.LATITUDE, T_BASE_COMPANY.LONGITUDE).Distinct()
                .From(DB.T_BASE_COMPANY)
                .InnerJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY.ID == T_BASE_COMPANY_PK.COMPANYID)
                .Where(where)
                .ExecToDataTable();

            Dictionary<string, List<GpsPointData>> dicReturn = new Dictionary<string, List<GpsPointData>>();
            List<GpsPointData> lstPointInfoList = new List<GpsPointData>();
            if (dtPoints != null && dtPoints.Rows.Count > 0)
            {
                //获取企业监测数据
                var lstID = dtPoints.AsEnumerable().Select(t => t["ID"].ToString()).ToList();  //获取企业ID列表
                string dataClass = "GasItem,WaterItem,VOCsItem";
                var monitorDatas = MonitorData.GetMonitorData("", DateTime.Now.AddDays(-1), DateTime.Now, "C", lstID, "hour", dataClass);
                //var maxDataTime = monitorDatas.AsEnumerable().Select(x => x.Field<string>("DATA_TIME")).Max();
                monitorDatas.DefaultView.Sort = "DATA_TIME desc";
                monitorDatas = monitorDatas.DefaultView.ToTable();
                Dictionary<string, DataRow> dicNumbers = new Dictionary<string, DataRow>();

                //获取企业最新的监测数据
                if (monitorDatas.Rows.Count > 0)
                {
                    string maxDataTime = monitorDatas.Rows[0]["DATA_TIME"].ToString();  //获取最新时间
                    monitorDatas.Select(string.Format("DATA_TIME = '{0}'", maxDataTime)).Each(dataRow =>
                    {
                        dicNumbers.Add(dataRow["ID"].ToString().Trim(), dataRow);
                    });
                }

                for (int i = 0; i < dtPoints.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtPoints.Rows[i]["latitude"].ToString().Trim()) && !string.IsNullOrEmpty(dtPoints.Rows[i]["latitude"].ToString().Trim()))
                    {
                        GpsPointData pointData = new GpsPointData();
                        pointData.ID = dtPoints.Rows[i]["ID"].ToString().Trim();
                        pointData.NAME = dtPoints.Rows[i]["NAME"].ToString().Trim();
                        pointData.LATITUDE = dtPoints.Rows[i]["latitude"].ToString().Trim();
                        pointData.LONGITUDE = dtPoints.Rows[i]["longitude"].ToString().Trim();
                        if (dicNumbers.Keys.Contains(pointData.ID))
                        {
                            pointData.NUMBER = GetCompanyMonitorData(dicNumbers[pointData.ID], monitorItem, IsVOCs);
                        }
                        else
                        {
                            pointData.NUMBER = "0";
                        }
                        pointData.STATUS = "1";
                        lstPointInfoList.Add(pointData);
                    }
                }
            }
            dicReturn.Add("allCompanyGPSDatas", lstPointInfoList);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(dicReturn);
        }

        /// <summary>
        /// 获取企业排放量
        /// </summary>
        /// <param name="dataRow">企业监测数据行</param>
        /// <param name="item">排放量因子项</param>
        /// <returns></returns>
        private string GetCompanyMonitorData(DataRow dataRow, string item, string IsVOCs)
        {
            string number = "0";
            List<string> lstItems = new List<string>();
            if (item == "all" && IsVOCs == "0") //污染源企业所有监测因子
            {
                lstItems = new List<string>() { "a34013_PFL_Value", "a21026_PFL_Value", "a21002_PFL_Value", "w01018_PFL_Value", "w21003_PFL_Value", "error" }; //烟尘，二氧化硫，氮氧化物，化学需氧量，氨氮
                number = CalcNumbersByItemList(dataRow, lstItems);
            }
            else if (item == "all" && IsVOCs == "1") //VOCs企业所有监测因子
            {
                lstItems = new List<string>() { "a25003_PFL_Value", "a25005_PFL_Value", "a24088_PFL_Value", "a25002_PFL_Value", "error" }; //甲苯，二甲苯，非甲烷总烃，苯
                number = CalcNumbersByItemList(dataRow, lstItems);
            }
            else
            {
                number = dataRow[item].ToString().Trim();
            }
            if (string.IsNullOrEmpty(number))
            {
                number = "0";
            }
            return number;
        }

        private string CalcNumbersByItemList(DataRow dataRow, List<string> lstItems)
        {
            decimal number = 0;
            foreach (var lstItem in lstItems)
            {
                if (dataRow.Table.Columns.Contains(lstItem) && dataRow[lstItem] != null && dataRow[lstItem].ToString() != "")
                {
                    decimal tmp = 0;
                    Decimal.TryParse(dataRow[lstItem].ToString(), out tmp);
                    number += tmp;
                }
            }
            return number.ToString();
        }

        [HttpPost]
        public string GetCompanyInfoDatas(RequestData data)
        {
            string companyID = data.Get("companyID").Trim();
            if (string.IsNullOrEmpty(companyID))
            {
                return "";
            }
            var where = T_BASE_COMPANY.ID == companyID;

            var dtPoints = SqlModel.Select(T_BASE_COMPANY.ID, "正常".As("STATUS"), T_BASE_COMPANY.NAME.As("NAME"), T_BASE_COMPANY.LATITUDE, T_BASE_COMPANY.LONGITUDE, "BASTYPE".Field("TITLE").As("BASTYPE"), T_SYS_AREA.AREA_TEXT.As("AREA")
                                            , T_BASE_COMPANY.ADDRESS, T_BASE_COMPANY.LEGALOR, "GZCD".Field("TITLE").As("GZCD"))
                .From(DB.T_BASE_COMPANY)
                .LeftJoin(DB.BASDIC.As("BASTYPE")).On(T_BASE_COMPANY.BASTYPE == "BASTYPE".Field("CODE") & "BASTYPE".Field("TYPECODE") == "IndustryType")
                .LeftJoin(DB.BASDIC.As("GZCD")).On(T_BASE_COMPANY.GZCD == "GZCD".Field("CODE") & "GZCD".Field("TYPECODE") == "CompanyGZCD")
                .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA == T_SYS_AREA.AREA_CODE)
                .Where(where)
                .ExecToDataTable();

            //Dictionary<string, List<GpsPointData>> dicReturn = new Dictionary<string, List<GpsPointData>>();
            //List<GpsPointData> lstPointInfoList = new List<GpsPointData>();
            //if (dtPoints != null && dtPoints.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dtPoints.Rows.Count; i++)
            //    {
            //        if (!string.IsNullOrEmpty(dtPoints.Rows[i]["latitude"].ToString().Trim()) && !string.IsNullOrEmpty(dtPoints.Rows[i]["latitude"].ToString().Trim()))
            //        {
            //            GpsPointData pointData = new GpsPointData();
            //            pointData.ID = dtPoints.Rows[i]["ID"].ToString().Trim();
            //            pointData.NAME = dtPoints.Rows[i]["NAME"].ToString().Trim();
            //            pointData.LATITUDE = dtPoints.Rows[i]["latitude"].ToString().Trim();
            //            pointData.LONGITUDE = dtPoints.Rows[i]["longitude"].ToString().Trim();
            //            pointData.NUMBER = (12 * i).ToString();
            //            pointData.STATUS = "1";
            //            lstPointInfoList.Add(pointData);
            //        }
            //    }
            //}
            //dicReturn.Add("allCompanyGPSDatas", lstPointInfoList);
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            return dtPoints.ToJson();
        }

        [HttpPost]
        public string GetCompanyPKInfo(RequestData data)
        {
            string companyID = data.Get("companyID").Trim();
            if (string.IsNullOrEmpty(companyID))
            {
                return "";
            }
            var where = T_BASE_COMPANY_PK.COMPANYID == companyID;

            var dtPoints = SqlModel.Select(T_BASE_COMPANY_PK.ID.As("PKID"), T_BASE_COMPANY_PK.NAME.As("PKNAME"), T_BASE_COMPANY_PK.TYPE.As("TYPE"), T_BASE_COMPANY_PK.COMPANYID.As("COMPANYID"))
                .From(DB.T_BASE_COMPANY_PK)
                .Where(where)
                .ExecToDataTable();

            //Dictionary<string, List<GpsPointData>> dicReturn = new Dictionary<string, List<GpsPointData>>();
            //List<GpsPointData> lstPointInfoList = new List<GpsPointData>();
            //if (dtPoints != null && dtPoints.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dtPoints.Rows.Count; i++)
            //    {
            //        if (!string.IsNullOrEmpty(dtPoints.Rows[i]["latitude"].ToString().Trim()) && !string.IsNullOrEmpty(dtPoints.Rows[i]["latitude"].ToString().Trim()))
            //        {
            //            GpsPointData pointData = new GpsPointData();
            //            pointData.ID = dtPoints.Rows[i]["ID"].ToString().Trim();
            //            pointData.NAME = dtPoints.Rows[i]["NAME"].ToString().Trim();
            //            pointData.LATITUDE = dtPoints.Rows[i]["latitude"].ToString().Trim();
            //            pointData.LONGITUDE = dtPoints.Rows[i]["longitude"].ToString().Trim();
            //            pointData.NUMBER = (12 * i).ToString();
            //            pointData.STATUS = "1";
            //            lstPointInfoList.Add(pointData);
            //        }
            //    }
            //}
            //dicReturn.Add("allCompanyGPSDatas", lstPointInfoList);
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            return dtPoints.ToJson();
        }

        /// <summary>
        /// 获取排口监测数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetPKMonitorDatas(RequestData data)
        {
            string pkID = data.Get("PKID").Trim();
            string type = data.Get("type").Trim(); //排口种类(1-废气、2-废水、3-VOCs)
            string dataClass = string.Empty;
            if (string.IsNullOrEmpty(pkID) || string.IsNullOrEmpty(type))
            {
                return "";
            }
            if (type == "1")
            {
                dataClass = "GasItem";
            }
            else if (type == "2")
            {
                dataClass = "WaterItem";
            }
            else if (type == "3")
            {
                dataClass = "VOCsItem";
            }
            //var where = T_BASE_COMPANY_PK.COMPANYID == pkID;

            //var dtPoints = SqlModel.Select(T_BASE_COMPANY_PK.ID.As("PKID"), T_BASE_COMPANY_PK.NAME.As("PKNAME"), T_BASE_COMPANY_PK.TYPE.As("TYPE"), T_BASE_COMPANY_PK.COMPANYID.As("COMPANYID"))
            //    .From(DB.T_BASE_COMPANY_PK)
            //    .Where(where)
            //    .ExecToDataTable();

            //Dictionary<string, List<GpsPointData>> dicReturn = new Dictionary<string, List<GpsPointData>>();
            //List<GpsPointData> lstPointInfoList = new List<GpsPointData>();
            //if (dtPoints != null && dtPoints.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dtPoints.Rows.Count; i++)
            //    {
            //        if (!string.IsNullOrEmpty(dtPoints.Rows[i]["latitude"].ToString().Trim()) && !string.IsNullOrEmpty(dtPoints.Rows[i]["latitude"].ToString().Trim()))
            //        {
            //            GpsPointData pointData = new GpsPointData();
            //            pointData.ID = dtPoints.Rows[i]["ID"].ToString().Trim();
            //            pointData.NAME = dtPoints.Rows[i]["NAME"].ToString().Trim();
            //            pointData.LATITUDE = dtPoints.Rows[i]["latitude"].ToString().Trim();
            //            pointData.LONGITUDE = dtPoints.Rows[i]["longitude"].ToString().Trim();
            //            pointData.NUMBER = (12 * i).ToString();
            //            pointData.STATUS = "1";
            //            lstPointInfoList.Add(pointData);
            //        }
            //    }
            //}
            //dicReturn.Add("allCompanyGPSDatas", lstPointInfoList);
            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            var monitorDatas = MonitorData.GetMonitorData("", DateTime.Now.AddDays(-1), DateTime.Now, "P", pkID, "hour", dataClass);
            monitorDatas.DefaultView.Sort = "DATA_TIME desc";
            monitorDatas = monitorDatas.DefaultView.ToTable();
            return monitorDatas.ToJson();
        }

        /// <summary>
        /// 获取企业监测数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetCompanyMonitorDatas(RequestData data)
        {
            string pkID = data.Get("PKID").Trim();
            string type = data.Get("type").Trim(); //排口种类(1-废气、2-废水、3-VOCs)
            string dataClass = "GasItem,WaterItem,VOCsItem";
            //if (string.IsNullOrEmpty(pkID) || string.IsNullOrEmpty(type))
            //{
            //    return "";
            //}
            //if (type == "1")
            //{
            //    dataClass = "GasItem";
            //}
            //else if (type == "2")
            //{
            //    dataClass = "WaterItem";
            //}
            //else if (type == "3")
            //{
            //    dataClass = "VOCsItem";
            //}

            var monitorDatas = MonitorData.GetMonitorData("", DateTime.Now.AddDays(-1), DateTime.Now, "C", pkID, "hour", dataClass);
            monitorDatas.DefaultView.Sort = "DATA_TIME desc";
            monitorDatas = monitorDatas.DefaultView.ToTable();
            return monitorDatas.ToJson();
        }
        /// <summary>
        /// 根据排口ID获取所属企业下的排口列表
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        public ActionResult GetPKList(string PKID)
        {
            DataRow row = SqlModel.Select(T_BASE_COMPANY_PK.COMPANYID)
                .From(DB.T_BASE_COMPANY_PK)
                .Where(T_BASE_COMPANY_PK.ID == PKID)
                .ExecToDataRow();
            if (row == null) return new EmptyResult();
            DataTable dt = SqlModel.Select()
                .From(DB.T_BASE_COMPANY_PK)
                .Where(T_BASE_COMPANY_PK.COMPANYID == row[0].ToString())
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        /// <summary>
        /// 获取单个排口在某个时间范围内的历史数据
        /// </summary>
        /// <param name="PKID">排口ID</param>
        /// <param name="DataType">数据类型，1-分钟、2-小时、3-日</param>
        /// <param name="sTime">开始时间</param>
        /// <param name="eTime">结束时间</param>
        /// <returns></returns>
        public ActionResult GetPKDatas(string PKID, string DataType, string sTime, string eTime)
        {
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
                  .From(DB.T_BASE_COMPANY_PK)
                  .Where(T_BASE_COMPANY_PK.ID == PKID)
                  .ExecToDataRow();
            if (pkRow[0] == null || string.IsNullOrEmpty(pkRow[0].ToString())) return new EmptyResult();

            string pkType = pkRow[0].ToString();
            string itemTypeCode = "GasItem";
            if (pkType == "1")
            {//废气
                itemTypeCode = "GasItem";
            }
            else if (pkType == "2")
            {//废水
                itemTypeCode = "WaterItem";
            }
            else if (pkType == "3")
            {//VOCs
                itemTypeCode = "VOCsItem";
            }

            DataTable dtItem = SqlModel.Select(string.Format(@"SELECT BASDIC.CODE AS ITEMCODE,BASDIC.TITLE AS ITEMTEXT,(CASE  WHEN BASDIC.REMARK is not null THEN BASDIC.REMARK ELSE D1.REMARK END) AS UNIT,D1.CODE AS SUBITEMCODE,D1.TITLE AS SUBITEMTEXT
                FROM T_BASE_COMPANY_PK_TX
                LEFT JOIN BASDIC ON instr(T_BASE_COMPANY_PK_TX.CLCS, BASDIC.CODE) > 0
                LEFT JOIN BASDIC D1 ON D1.TYPECODE = BASDIC.CODE
                WHERE (T_BASE_COMPANY_PK_TX.PKID = '{0}') AND (BASDIC.TYPECODE = '{1}')", PKID, itemTypeCode))
                .Native().ExecToDataTable();
            if (dtItem.Rows.Count == 0) return new EmptyResult();
            Dictionary<string, string> dictItem = new Dictionary<string, string>();
            foreach (DataRow row in dtItem.Rows)
            {
                string key = row["ITEMCODE"] + (row["SUBITEMCODE"] == null ? "" : row["SUBITEMCODE"].ToString());
                if (dictItem.ContainsKey(key)) continue;
                dictItem.Add(key, row["ITEMTEXT"] + (row["SUBITEMTEXT"] == null ? "" : row["SUBITEMTEXT"].ToString()));
            }
            DateTime timeS = DateTime.Parse(sTime);
            DateTime timeE = DateTime.Parse(eTime);

            string timeFormat = "HH:00";
            DataTable dtData = new DataTable();
            if (DataType == "1")
            {//分钟数据
                timeFormat = "HH:mm";
                dtData = SqlModel.Select((T_MID_MINUTE.ITEMCODE ^ T_MID_MINUTE.SUBITEMCODE).As("ITEMCODE"), T_MID_MINUTE.RECTIME, T_MID_MINUTE.VALUE)
                    .From(DB.T_BASE_COMPANY_PK)
                    .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                    .LeftJoin(DB.T_MID_MINUTE).On(T_BASE_COMPANY_PK_TX.MN == T_MID_MINUTE.DEVICECODE)
                    .Where(T_BASE_COMPANY_PK.ID == PKID & T_MID_MINUTE.RECTIME.BetweenAnd(timeS, timeE))
                    .ExecToDataTable();
            }
            else if (DataType == "2")
            {//小时数据
                timeFormat = "HH:00";
                dtData = SqlModel.Select((T_MID_HOUR.ITEMCODE ^ T_MID_HOUR.SUBITEMCODE).As("ITEMCODE"), T_MID_HOUR.RECTIME, T_MID_HOUR.VALUE)
                    .From(DB.T_BASE_COMPANY_PK)
                    .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                    .LeftJoin(DB.T_MID_HOUR).On(T_BASE_COMPANY_PK_TX.MN == T_MID_HOUR.DEVICECODE)
                    .Where(T_BASE_COMPANY_PK.ID == PKID & T_MID_HOUR.RECTIME.BetweenAnd(timeS, timeE))
                    .ExecToDataTable();
            }
            else if (DataType == "3")
            {//日数据
                timeFormat = "MM-dd";
                dtData = SqlModel.Select((T_MID_DAY.ITEMCODE ^ T_MID_DAY.SUBITEMCODE).As("ITEMCODE"), T_MID_DAY.RECTIME, T_MID_DAY.VALUE)
                    .From(DB.T_BASE_COMPANY_PK)
                    .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                    .LeftJoin(DB.T_MID_DAY).On(T_BASE_COMPANY_PK_TX.MN == T_MID_DAY.DEVICECODE)
                    .Where(T_BASE_COMPANY_PK.ID == PKID & T_MID_DAY.RECTIME.BetweenAnd(timeS, timeE))
                    .ExecToDataTable();
            }
            List<KeyValuePair<string, string>> y = new List<KeyValuePair<string, string>>();
            StringBuilder legend = new StringBuilder();
            Dictionary<string, StringBuilder> dictY = new Dictionary<string, StringBuilder>();
            foreach (var item in dictItem)
            {
                dictY.Add(item.Value, new StringBuilder());
                legend.Append("'" + item.Value + "',");
            }
            StringBuilder y1 = new StringBuilder();
            StringBuilder x = new StringBuilder();
            var timeList = (from p in dtData.AsEnumerable()
                            select p.Field<DateTime>("RECTIME")).Distinct().Reverse().OrderBy(p => p);
            foreach (DateTime time in timeList)
            {
                x.Append("'" + time.ToString(timeFormat) + "',");

                foreach (var item in dictItem)
                {
                    DataRow[] rows = dtData.Select("ITEMCODE='" + item.Key + "' and RECTIME='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    if (rows.Length == 0)
                        dictY[item.Value].Append("'',");
                    else
                        dictY[item.Value].Append("'" + rows[0]["VALUE"].ToString() + "',");
                }
            }
            for (int i = 0; i < dictY.Count; i++)
            {
                string key = dictY.ElementAt(i).Key;
                string value = dictY.ElementAt(i).Value.ToString();
                if (value.Length > 0)
                {
                    y.Add(new KeyValuePair<string, string>(key, value.Remove(value.Length - 1, 1)));
                }
            }
            if (x.Length > 0) x = x.Remove(x.Length - 1, 1);
            if (legend.Length > 0) legend = legend.Remove(legend.Length - 1, 1);

            return Json(EChartsHelper.GetBaseChart(x.ToString(), y, "", "", legend.ToString(), "", "line", false, false));
        }
        public ActionResult GetLastAlertByPK(string pkid)
        {
            DateTime stime = DateTime.Now.AddDays(-7);
            DateTime etime = DateTime.Now;
            FieldModel where = T_MID_ALERT.STARTTIME.BetweenAnd(stime, etime);
            where &= T_MID_ALERT.PKID == pkid;

            var model = SqlModel.SelectAll()
            .From(DB.T_MID_ALERT)
            .Where(where)
            .OrderByDesc(T_MID_ALERT.STARTTIME);

            var resultObj = model.ExecToDynamic();
            return Json(resultObj);
        }

    }

    public class GpsPointData
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string NAME { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string ADDRESS { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string SHORTNAME { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string LONGITUDE { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string LATITUDE { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string STATUS { get; set; }

        /// <summary>
        /// 污染浓度
        /// </summary>
        public string NUMBER { get; set; }
    }
}
