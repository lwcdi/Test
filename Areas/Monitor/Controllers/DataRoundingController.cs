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

namespace UI.Web.Areas.Monitor.Controllers
{
    public class DataRoundingController : Controller
    {
        //
        // GET: /Monitor/DataRounding/
        string[] LostControlFlag = new string[] { "T", "U", "H", "G" };//失控时间段内数据标志
        string[] ARoundingItemCode = new string[] { "a21026", "a21002", "a34001", "a34002", "a34004", "a34005" };//二氧化硫、氮氧化物、总悬浮颗粒物TSP、PM10、PM2.5、PM1.0
        string[] WRoundingItemCode = new string[] { "w01018", "w21003", "w21011", "w01001", "water_flow" };//化学需氧量、氨氮、总磷、PH值、流量
        public ActionResult DataRoundingList(int? navId, string fs, string fq, string vocs, string companyOnly)
        {
            ViewData["fs"] = fs;
            ViewData["fq"] = fq;
            ViewData["vocs"] = vocs;
            ViewData["companyOnly"] = companyOnly;
            return View(navId ?? 0);
        }
        public ActionResult GetStatusFlag()
        {
            List<dynamic> list = SqlModel.Select(BASDIC.TITLE, BASDIC.CODE, BASDIC.REMARK)
                             .From(DB.BASDIC)
                             .Where(BASDIC.TYPECODE == "DATASTATUS")
                             .ExecToDynamicList();
            return Json(list);
        }
        public ActionResult DataByMutiPoint(string item, string time, string PKId, string fs, string fq, string vocs)
        {
            ViewData["fs"] = fs;
            ViewData["fq"] = fq;
            ViewData["vocs"] = vocs;
            ViewData["ITEM"] = item;
            ViewData["TIME"] = time;
            ViewData["PKId"] = PKId;
            return View();
        }
        public ActionResult MinDataByPoint(string time, string PKId)
        {
            ViewData["TIME"] = time;
            ViewData["PKId"] = PKId;
            return View();
        }

        public ActionResult Rounding(string item, string time, string PKId)
        {
            ViewData["ITEM"] = item;
            ViewData["TIME"] = time;
            ViewData["PKId"] = PKId;
            return View();
        }

        public ActionResult GetChartBySingleItem(string item, string PkId, string time)
        {
            FieldModel onWhere = T_MID_HOUR.RECTIME == DateTime.Parse(time);

            string[] itemArray = item.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            string ItemCode = "";
            string SubItemCode = "";
            string title = "";
            string unit = "";
            if (itemArray.Length > 0)
            {
                ItemCode = itemArray[0];
                dynamic itemObj = SqlModel.Select(BASDIC.TITLE, BASDIC.REMARK)
                .From(DB.BASDIC)
                .Where(BASDIC.CODE == ItemCode)
                .ExecToDynamic();
                title += itemObj["TITLE"];
                if (itemObj["REMARK"] is DBNull || itemObj["REMARK"] == null)
                    unit = "";
                else
                    unit = itemObj["REMARK"];

            }
            if (itemArray.Length > 1)
            {
                SubItemCode = itemArray[1];
                dynamic subItemObj = SqlModel.Select(BASDIC.TITLE, BASDIC.REMARK)
                     .From(DB.BASDIC)
                     .Where(BASDIC.CODE == SubItemCode)
                     .ExecToDynamic();
                title += " " + subItemObj["TITLE"];
                if (subItemObj["REMARK"] is DBNull || subItemObj["REMARK"] == null)
                    unit = "";
                else
                    unit = subItemObj["REMARK"];
            }

            if (!string.IsNullOrEmpty(ItemCode)) onWhere &= T_MID_HOUR.ITEMCODE == ItemCode;
            if (!string.IsNullOrEmpty(SubItemCode)) onWhere &= T_MID_HOUR.SUBITEMCODE == SubItemCode;

            DataTable dtData = SqlModel.Select(T_BASE_COMPANY_PK.NAME, T_MID_HOUR.VALUE)
                .From(DB.T_BASE_COMPANY_PK)
                .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                .LeftJoin(DB.T_MID_HOUR).On(T_BASE_COMPANY_PK_TX.MN == T_MID_HOUR.DEVICECODE & onWhere)
                .Where(T_BASE_COMPANY_PK.ID.In("'" + string.Join("','", PkId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) + "'"))
                .ExecToDataTable();

            StringBuilder x = new StringBuilder();
            List<KeyValuePair<string, string>> y = new List<KeyValuePair<string, string>>();
            StringBuilder y1 = new StringBuilder();
            foreach (DataRow row in dtData.Rows)
            {
                x.Append("'" + row["NAME"].ToString() + "',");

                y1.Append("'" + row["VALUE"].ToString() + "',");
            }
            if (x.Length > 0) x = x.Remove(x.Length - 1, 1);
            if (y1.Length > 0) y1 = y1.Remove(y1.Length - 1, 1);
            y.Add(new KeyValuePair<string, string>(title, y1.ToString()));

            return Json(EChartsHelper.GetBaseChart(x.ToString(), y, title + "(" + time + ")", unit, x.ToString(), "", "bar"));
        }
        /// <summary>
        /// 获取某个排口某天的小时数据
        /// </summary>
        /// <param name="PkId">排口ID</param>
        /// <param name="Date">日期</param>
        /// <param name="DataSource">数据来源</param>
        /// <param name="DataStatus">数据状态</param>
        /// <returns></returns>
        public ActionResult GetData(string PkId, string Date, string DataSource, string DataStatus)
        {
            DateTime date = DateTime.Now;
            DataTable dtResult = new DataTable();
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
                .From(DB.T_BASE_COMPANY_PK)
                .Where(T_BASE_COMPANY_PK.ID == PkId)
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
                WHERE (T_BASE_COMPANY_PK_TX.PKID = '{0}') AND (BASDIC.TYPECODE = '{1}')", PkId, itemTypeCode))
                .Native().ExecToDataTable();
            if (dtItem.Rows.Count == 0) return new EmptyResult();

            FieldModel where = BASDIC.TYPECODE == itemTypeCode & T_BASE_COMPANY_PK_TX.CLCS.Instr(BASDIC.CODE) > 0;
            if (!string.IsNullOrEmpty(PkId)) where &= T_BASE_COMPANY_PK.ID == PkId;
            if (!string.IsNullOrEmpty(DataStatus)) where &= T_MID_HOUR_C.STATUS == DataStatus;
            if (!string.IsNullOrEmpty(Date))
            {
                date = DateTime.Parse(Date);
                where &= T_MID_HOUR_C.RECTIME.BetweenAnd(date, date.AddDays(1).AddSeconds(-1));
            }
            DataTable dtData = SqlModel.Select(T_MID_HOUR_C.RECTIME, T_MID_HOUR_C.DEVICECODE, T_MID_HOUR_C.ITEMCODE, T_MID_HOUR_C.SUBITEMCODE,
                T_MID_HOUR_C.VALUE, T_MID_HOUR_C.STATUS, ("D1").Field("TITLE").As("STATUSTEXT"))
                .From(DB.T_MID_HOUR_C)
                .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK_TX.MN == T_MID_HOUR_C.DEVICECODE)
                .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                .LeftJoin(DB.BASDIC).On(BASDIC.CODE == T_MID_HOUR_C.ITEMCODE)
                .LeftJoin(DB.BASDIC.As("D1")).On("D1".Field("CODE") == T_MID_HOUR_C.STATUS)
                .Where(where)
                .OrderByAsc(T_MID_HOUR_C.RECTIME)
                .ExecToDataTable();

            DataTable dtCheckRecord = SqlModel.Select(("t").Field("ID"), ("t").Field("MN"), ("t").Field("ITEMCODE"), ("t").Field("SUBITEMCODE"), ("t").Field("RECTIME"), ("t").Field("VALUE")).From(SqlModel.Select(T_MID_CHECKRECORD.ID.MinAs("ID"))
                .From(DB.T_MID_CHECKRECORD)
                .Where(T_MID_CHECKRECORD.RECTIME.BetweenAnd(date, date.AddDays(1).AddSeconds(-1)) & T_MID_CHECKRECORD.ISDEL == "0")
                .GroupBy(T_MID_CHECKRECORD.MN, T_MID_CHECKRECORD.ITEMCODE, T_MID_CHECKRECORD.SUBITEMCODE, T_MID_CHECKRECORD.RECTIME).As("tmp"))
                .LeftJoin(DB.T_MID_CHECKRECORD.As("t")).On(("t").Field("ID") == ("tmp").Field("ID"))
                .ExecToDataTable();

            dtResult.Columns.Add("TIME");
            List<dynamic> titleList = new List<dynamic>();
            List<dynamic> subTitleList = new List<dynamic>();
            foreach (DataRow row in dtItem.Rows)
            {
                if (titleList.Count(p => p.ITEMCODE == row["ITEMCODE"].ToString()) == 0)
                {
                    int length = dtItem.Select("ITEMCODE ='" + row["ITEMCODE"].ToString() + "'").Length;
                    titleList.Add(
                        new
                        {
                            ITEMCODE = row["ITEMCODE"].ToString(),
                            ITEMTEXT = row["ITEMTEXT"].ToString(),
                            UNIT = length == 1 ? row["UNIT"].ToString() : "",
                            COLSPAN = length
                        });
                }

                if (row["SUBITEMCODE"] != null && !string.IsNullOrEmpty(row["SUBITEMCODE"].ToString()))
                {
                    subTitleList.Add(
                        new
                        {
                            ITEMCODE = row["ITEMCODE"].ToString(),
                            SUBITEMCODE = row["SUBITEMCODE"].ToString(),
                            SUBITEMTEXT = row["SUBITEMTEXT"].ToString(),
                            UNIT = row["UNIT"].ToString()
                        });
                    dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString());
                    dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString() + "_STATUS");
                    dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString() + "_STATUSTEXT");
                    dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString() + "_UNIT");
                    dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString() + "_R");
                    dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString() + "_CID");
                }
                else
                {
                    if (!dtResult.Columns.Contains(row["ITEMCODE"].ToString()))
                    {
                        dtResult.Columns.Add(row["ITEMCODE"].ToString());
                        dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_STATUS");
                        dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_STATUSTEXT");
                        dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_UNIT");
                        dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_R");
                        dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_CID");
                    }
                }
            }
            List<DateTime> timeList = new List<DateTime>();
            for (DateTime time = date; time < DateTime.Now & time < date.AddDays(1).AddSeconds(-1); time = time.AddHours(1))
            {
                timeList.Add(time);
            }
            //var timeList = (from p in dtData.AsEnumerable()
            //                select p.Field<DateTime>("RECTIME")).Distinct();
            foreach (DateTime time in timeList)
            {
                DataRow[] dataRows = dtData.Select(" RECTIME='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                DataRow newRow = dtResult.NewRow();
                newRow["TIME"] = time.ToString("yyyy-MM-dd HH");
                foreach (var item in titleList)
                {
                    if (item.COLSPAN != 1) continue;
                    DataRow[] rows = dataRows.Where(p => p.Field<string>("ITEMCODE") == item.ITEMCODE).ToArray();
                    if (rows.Length > 0)
                    {
                        newRow[item.ITEMCODE] = rows[0]["VALUE"];
                        newRow[item.ITEMCODE + "_UNIT"] = item.UNIT;

                        DataRow[] checkRows = dtCheckRecord.Select(" MN='" + rows[0]["DEVICECODE"].ToString() + "' and  ITEMCODE='" + item.ITEMCODE + "' and RECTIME='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        if (checkRows.Length > 0)
                        {
                            newRow[item.ITEMCODE + "_R"] = checkRows[0]["VALUE"];
                            newRow[item.ITEMCODE + "_CID"] = checkRows[0]["ID"];
                        }
                        if (ARoundingItemCode.Contains((string)item.ITEMCODE) || WRoundingItemCode.Contains((string)item.ITEMCODE))
                        {
                            newRow[item.ITEMCODE + "_STATUS"] = rows[0]["STATUS"];
                            newRow[item.ITEMCODE + "_STATUSTEXT"] = rows[0]["STATUSTEXT"];
                        }
                    }
                }
                foreach (var item in subTitleList)
                {
                    DataRow[] rows = dataRows.Where(p => p.Field<string>("ITEMCODE") == item.ITEMCODE & p.Field<string>("SUBITEMCODE") == item.SUBITEMCODE).ToArray();
                    if (rows.Length > 0)
                    {
                        newRow[item.ITEMCODE + "_" + item.SUBITEMCODE] = rows[0]["VALUE"];
                        newRow[item.ITEMCODE + "_" + item.SUBITEMCODE + "_UNIT"] = item.UNIT;

                        DataRow[] checkRows = dtCheckRecord.Select(" MN='" + rows[0]["DEVICECODE"].ToString() + "' and  ITEMCODE='" + item.ITEMCODE
                            + "' and  SUBITEMCODE='" + item.SUBITEMCODE + "' and RECTIME='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        if (checkRows.Length > 0)
                        {
                            newRow[item.ITEMCODE + "_" + item.SUBITEMCODE + "_R"] = checkRows[0]["VALUE"];
                            newRow[item.ITEMCODE + "_" + item.SUBITEMCODE + "_CID"] = checkRows[0]["ID"];
                        }
                        if (ARoundingItemCode.Contains((string)item.ITEMCODE) || WRoundingItemCode.Contains((string)item.ITEMCODE))
                        {
                            if (pkType == "2" && item.SUBITEMCODE != "ND") continue;
                            if ((pkType == "1" || pkType == "3") && item.SUBITEMCODE != "PFL") continue;

                            newRow[item.ITEMCODE + "_" + item.SUBITEMCODE + "_STATUS"] = rows[0]["STATUS"];
                            newRow[item.ITEMCODE + "_" + item.SUBITEMCODE + "_STATUSTEXT"] = rows[0]["STATUSTEXT"];
                        }
                    }
                }
                dtResult.Rows.Add(newRow);
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return Json(new
            {
                TITLE = new
                {
                    MAINTITLE = titleList,
                    SUBTITLE = subTitleList
                },
                DATA = new
                {
                    total = dtResult.Rows.Count,
                    rows = dtResult.ToDynamicList()
                }
            });
        }

        public ActionResult BackRounding(string CID)
        {
            DataRow row = SqlModel.SelectAll()
                .From(DB.T_MID_CHECKRECORD)
                .Where(T_MID_CHECKRECORD.ID == CID)
                .ExecToDataRow();
            if (row == null) return Json(new
            {
                Result = false,
                Msg = "修约记录不存在"
            });

            //新增修约记录
            T_MID_CHECKRECORDModel model = new T_MID_CHECKRECORDModel();
            model.ISDEL = "1";
            bool result = model.Update(T_MID_CHECKRECORD.ID == CID);
            if (result)
            {
                //更新修约表
                T_MID_HOUR_CModel hourC = new T_MID_HOUR_CModel();
                hourC.VALUE = (decimal)row["VALUE"];
                hourC.STATUS = row["STATUS"].ToString();
                result = hourC.Update(T_MID_HOUR_C.DEVICECODE == row["MN"]
                & T_MID_HOUR_C.RECTIME == row["RECTIME"]
                & T_MID_HOUR_C.ITEMCODE == row["ITEMCODE"]
                & T_MID_HOUR_C.SUBITEMCODE == row["SUBITEMCODE"]);
            }
            return Json(new
            {
                Result = result
            });
        }

        private DataTable GetAirRoundingData(string PkId, string time, string timeType, string item)
        {
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
               .From(DB.T_BASE_COMPANY_PK)
               .Where(T_BASE_COMPANY_PK.ID == PkId)
               .ExecToDataRow();
            if (pkRow[0] == null || string.IsNullOrEmpty(pkRow[0].ToString())) return null;

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

            string[] itemArray = item.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            FieldModel where = BASDIC.TYPECODE == itemTypeCode & T_BASE_COMPANY_PK_TX.CLCS.Instr(BASDIC.CODE) > 0;
            if (!string.IsNullOrEmpty(PkId)) where &= T_BASE_COMPANY_PK.ID == PkId;
            if (itemArray.Length > 0) where &= T_MID_HOUR_C.ITEMCODE == itemArray[0];
            else
            {
                where &= T_MID_HOUR_C.ITEMCODE.In("'" + string.Join("','", ARoundingItemCode) + "'");
            }
            where &= T_MID_HOUR_C.SUBITEMCODE == "PFL";
            where &= T_MID_HOUR_C.STATUS != "N";
            DateTime date = DateTime.Parse(time);
            if (timeType == "0")
            {//单个小时
                where &= T_MID_HOUR_C.RECTIME == date;
            }
            else
            {//全天的小时
                where &= T_MID_HOUR_C.RECTIME.BetweenAnd(date.Date, date.Date.AddDays(1).AddSeconds(-1));
            }

            return SqlModel.Select(T_MID_HOUR_C.ID, T_MID_HOUR_C.RECTIME, T_MID_HOUR_C.DEVICECODE, T_MID_HOUR_C.ITEMCODE, T_MID_HOUR_C.SUBITEMCODE,
                 T_MID_HOUR_C.VALUE, T_MID_HOUR_C.STATUS, BASDIC.TITLE.As("ITEMTEXT"), BASDIC.REMARK.As("ITEMUNIT"), ("D1").Field("TITLE").As("SUBITEMTEXT"), ("D1").Field("REMARK").As("SUBITEMUNIT"))
                 .From(DB.T_MID_HOUR_C)
                 .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK_TX.MN == T_MID_HOUR_C.DEVICECODE)
                 .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                 .LeftJoin(DB.BASDIC).On(BASDIC.CODE == T_MID_HOUR_C.ITEMCODE)
                 .LeftJoin(DB.BASDIC.As("D1")).On(("D1").Field("CODE") == T_MID_HOUR_C.SUBITEMCODE & ("D1").Field("TYPECODE") == T_MID_HOUR_C.ITEMCODE)
                 .Where(where)
                 .OrderByAsc(T_MID_HOUR_C.RECTIME)
                 .ExecToDataTable();
        }
        public DataTable GetWaterRoundingData(string PkId, string time, string timeType, string item)
        {
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
                 .From(DB.T_BASE_COMPANY_PK)
                 .Where(T_BASE_COMPANY_PK.ID == PkId)
                 .ExecToDataRow();
            if (pkRow[0] == null || string.IsNullOrEmpty(pkRow[0].ToString())) return null;

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

            string[] itemArray = item.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime date = DateTime.Parse(time);
            List<DateTime> timeList = new List<DateTime>();
            if (timeType == "0")
            {//单个小时
                timeList.Add(date);
            }
            else
            {//全天的小时
                for (DateTime tmp = date.Date; tmp < DateTime.Now & tmp < date.Date.AddDays(1).AddSeconds(-1); tmp = tmp.AddHours(1))
                {
                    timeList.Add(tmp);
                }
            }

            DataTable dtData = new DataTable();
            dtData.Columns.Add("ID");
            dtData.Columns.Add("RECTIME");
            dtData.Columns.Add("DEVICECODE");
            dtData.Columns.Add("ITEMCODE");
            dtData.Columns.Add("SUBITEMCODE");
            dtData.Columns.Add("VALUE");
            dtData.Columns.Add("STATUS");
            dtData.Columns.Add("ITEMTEXT");
            dtData.Columns.Add("ITEMUNIT");
            dtData.Columns.Add("SUBITEMTEXT");
            dtData.Columns.Add("SUBITEMUNIT");
            DataTable dtItem = SqlModel.Select(string.Format(@"SELECT T_BASE_COMPANY_PK_TX.MN, BASDIC.CODE AS ITEMCODE,BASDIC.TITLE AS ITEMTEXT, BASDIC.REMARK ITEMUNIT, D1.REMARK SUBITEMUNIT
                ,D1.CODE AS SUBITEMCODE,D1.TITLE AS SUBITEMTEXT
                FROM T_BASE_COMPANY_PK_TX
                LEFT JOIN BASDIC ON instr(T_BASE_COMPANY_PK_TX.CLCS, BASDIC.CODE) > 0
                LEFT JOIN BASDIC D1 ON D1.TYPECODE = BASDIC.CODE
                WHERE (T_BASE_COMPANY_PK_TX.PKID = '{0}') AND (BASDIC.TYPECODE = '{1}') {2}{3}",
                PkId,
                itemTypeCode,
                itemArray.Length > 0 ? " and BASDIC.CODE='" + itemArray[0] + "'" : (" and BASDIC.CODE in('" + string.Join("','", WRoundingItemCode) + "')"),
                 " and D1.CODE='ND'"))
                .Native().ExecToDataTable();
            foreach (DateTime tmp in timeList)
            {
                foreach (DataRow row in dtItem.Rows)
                {
                    DataRow rowCount = SqlModel.Select(T_MID_HOUR_C.ID.CountAs("COUNT"))
                        .From(DB.T_MID_HOUR_C)
                        .Where(
                        T_MID_HOUR_C.DEVICECODE == row["MN"]
                        & T_MID_HOUR_C.ITEMCODE == row["ITEMCODE"]
                        & T_MID_HOUR_C.SUBITEMCODE == row["SUBITEMCODE"]
                        & T_MID_HOUR_C.RECTIME == tmp
                        ).ExecToDataRow();
                    if ((decimal)rowCount[0] != 0) continue;
                    DataRow newRow = dtData.NewRow();
                    newRow["RECTIME"] = tmp;
                    newRow["DEVICECODE"] = row["MN"];
                    newRow["ITEMCODE"] = row["ITEMCODE"];
                    newRow["SUBITEMCODE"] = row["SUBITEMCODE"];
                    newRow["STATUS"] = "N";
                    newRow["ITEMTEXT"] = row["ITEMTEXT"];
                    newRow["SUBITEMTEXT"] = row["SUBITEMTEXT"];
                    newRow["ITEMUNIT"] = row["ITEMUNIT"];
                    newRow["SUBITEMUNIT"] = row["SUBITEMUNIT"];
                    dtData.Rows.Add(newRow);
                }
            }

            return dtData;
        }
        public ActionResult GetRoundingData(string PkId, string time, string timeType, string item)
        {
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
               .From(DB.T_BASE_COMPANY_PK)
               .Where(T_BASE_COMPANY_PK.ID == PkId)
               .ExecToDataRow();
            if (pkRow[0] == null || string.IsNullOrEmpty(pkRow[0].ToString())) return new EmptyResult();
            DataTable dtData = new DataTable();
            string pkType = pkRow[0].ToString();
            if (pkType == "1")
            {//废气
                dtData = GetAirRoundingData(PkId, time, timeType, item);
            }
            else if (pkType == "2")
            {//废水
                dtData = GetWaterRoundingData(PkId, time, timeType, item);
            }
            else if (pkType == "3")
            {//VOCs
                dtData = GetAirRoundingData(PkId, time, timeType, item);
            }

            return Json(new
            {
                total = dtData.Rows.Count,
                rows = dtData.ToDynamicList()
            });
        }
        public ActionResult GetAutoRoundingData(string PkId, string time, string timeType, string item)
        {
            DateTime date = DateTime.Parse(time);
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
                .From(DB.T_BASE_COMPANY_PK)
                .Where(T_BASE_COMPANY_PK.ID == PkId)
                .ExecToDataRow();
            if (pkRow[0] == null || string.IsNullOrEmpty(pkRow[0].ToString())) return new EmptyResult();
            DataTable dtData = new DataTable();
            string pkType = pkRow[0].ToString();
            if (pkType == "1")
            {//废气
                dtData = GetAirRoundingData(PkId, time, timeType, item);
            }
            else if (pkType == "2")
            {//废水
                dtData = GetWaterRoundingData(PkId, time, timeType, item);
            }
            else if (pkType == "3")
            {//VOCs
                dtData = GetAirRoundingData(PkId, time, timeType, item);
            }

            dtData.Columns.Add("CHECKVALUE");

            DateTime stime = date;
            DateTime etime = date;
            GetTimeSpan(date, out stime, out etime);

            if (pkType == "1" || pkType == "3")
            {//气体
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    DataRow row = dtData.Rows[i];
                    if (row["STATUS"] == null || "N" == row["STATUS"].ToString()) continue;//正常数据、无需修约
                    if (row["SUBITEMCODE"] == null || "PFL" != row["SUBITEMCODE"].ToString()) continue;//只对排放量修约

                    string deviceCode = row["DEVICECODE"].ToString();
                    string ItemCode = row["ITEMCODE"].ToString();
                    if (!ARoundingItemCode.Contains(ItemCode)) continue;//只对部分因子修约

                    DateTime RECTIME = DateTime.Parse(row["RECTIME"].ToString());
                    decimal 捕集率 = 季度有效数据捕集率(deviceCode, ItemCode, stime, etime);
                    if (LostControlFlag.Contains(row["STATUS"].ToString()))
                    {//失控
                        if (捕集率 >= 90)
                        {
                            decimal 失控小时数 = _25小时内失控小时数(RECTIME, deviceCode, ItemCode);
                            if (失控小时数 <= 24)
                            {
                                row["CHECKVALUE"] = 上次校准前有效最大值(RECTIME, deviceCode, ItemCode, 180);
                            }
                            else
                            {
                                row["CHECKVALUE"] = 上次校准前有效最大值(RECTIME, deviceCode, ItemCode, 720);
                            }
                        }
                        else if (捕集率 >= 75)
                        {
                            row["CHECKVALUE"] = 上次校准前有效最大值(RECTIME, deviceCode, ItemCode, 2160);
                        }
                    }
                    else
                    {//无效
                        if (捕集率 >= 90)
                        {
                            decimal 无效小时数 = _25小时内无效小时数(RECTIME, deviceCode, ItemCode);
                            if (无效小时数 <= 24)
                            {
                                row["CHECKVALUE"] = 上次失效前有效最大值(RECTIME, deviceCode, ItemCode, 180);
                            }
                            else
                            {
                                row["CHECKVALUE"] = 上次失效前有效最大值(RECTIME, deviceCode, ItemCode, 720);
                            }
                        }
                        else if (捕集率 >= 75)
                        {
                            row["CHECKVALUE"] = 上次失效前有效最大值(RECTIME, deviceCode, ItemCode, 2160);
                        }
                    }
                    //新增修约记录
                    if (row["CHECKVALUE"] == null || row["CHECKVALUE"].ToString() == "-999") continue;
                    T_MID_CHECKRECORDModel model = new T_MID_CHECKRECORDModel();
                    model.CHECKVALUE = decimal.Parse(row["CHECKVALUE"].ToString());
                    model.CHECKTYPE = "1";//1-按技术规则自动修约；2-手动修约

                    model.MN = deviceCode;
                    model.ITEMCODE = ItemCode;
                    model.SUBITEMCODE = "PFL";
                    model.RECTIME = RECTIME;
                    model.VALUE = decimal.Parse(row["VALUE"].ToString());
                    model.STATUS = row["STATUS"].ToString();

                    model.CREATETIME = DateTime.Now;
                    model.CREATEUSER = CurrentUser.UserName;
                    model.CHECKTIME = DateTime.Now;
                    model.CHECKUSER = CurrentUser.UserName;
                    //更新修约表
                    T_MID_HOUR_CModel hourC = new T_MID_HOUR_CModel();
                    hourC.VALUE = model.CHECKVALUE;

                    if (model.CHECKVALUE == -999)
                    {
                        model.REMARK = "自动修约失败";
                    }
                    else
                    {
                        hourC.STATUS = "N";
                    }
                    bool result = model.Insert();
                    hourC.Update(T_MID_HOUR_C.ID == row["ID"].ToString());
                }
            }
            else if (pkType == "2")
            {//废水
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    DataRow row = dtData.Rows[i];
                    if (row["VALUE"] != null && !string.IsNullOrEmpty(row["VALUE"].ToString())) continue;//只对缺失数据进行修约

                    string deviceCode = row["DEVICECODE"].ToString();
                    string ItemCode = row["ITEMCODE"].ToString();
                    string subItemCode = row["SUBITEMCODE"] == null ? "" : row["SUBITEMCODE"].ToString();
                    if (!WRoundingItemCode.Contains(ItemCode)) continue;//只对部分因子修约

                    DateTime RECTIME = DateTime.Parse(row["RECTIME"].ToString());
                    DataTable dttmp = 获取数据缺失的时间范围(RECTIME, deviceCode, ItemCode, subItemCode);
                    if (dttmp == null || dttmp.Rows.Count == 0) continue;
                    if (dttmp.Rows[0]["TIME1"] == null || dttmp.Rows[0]["TIME2"] == null) continue;
                    DateTime TIME1 = (DateTime)dttmp.Rows[0]["TIME1"];
                    DateTime TIME2 = (DateTime)dttmp.Rows[0]["TIME2"];
                    int hourSpan = (int)(TIME2 - TIME1).TotalHours - 1;
                    List<decimal> listValue = 获取时间范围数据(TIME1, hourSpan, deviceCode, ItemCode, subItemCode);
                    listValue = listValue.OrderBy(p => p).ToList();
                    if (ItemCode == "w01001")
                    { //PH值取中位值
                        if (listValue.Count() == 0) continue;
                        if (listValue.Count() % 2 == 1)
                        {
                            row["CHECKVALUE"] = listValue[listValue.Count() / 2];
                        }
                        else
                        {
                            row["CHECKVALUE"] = System.Decimal.Round((listValue[listValue.Count() / 2] + listValue[listValue.Count() / 2 - 1]) / 2, 2);
                        }
                    }
                    else
                    {//取算数均值
                        row["CHECKVALUE"] = System.Decimal.Round(listValue.Average(), 2);
                    }

                    //新增修约记录
                    if (row["CHECKVALUE"] == null || row["CHECKVALUE"].ToString() == "-999") continue;
                    T_MID_CHECKRECORDModel model = new T_MID_CHECKRECORDModel();
                    model.CHECKVALUE = decimal.Parse(row["CHECKVALUE"].ToString());
                    model.CHECKTYPE = "1";//1-按技术规则自动修约；2-手动修约

                    model.MN = deviceCode;
                    model.ITEMCODE = ItemCode;
                    model.SUBITEMCODE = subItemCode;
                    model.RECTIME = RECTIME;
                    model.STATUS = row["STATUS"].ToString();
                    if (!string.IsNullOrEmpty(row["VALUE"].ToString())) model.VALUE = decimal.Parse(row["VALUE"].ToString());

                    model.CREATETIME = DateTime.Now;
                    model.CREATEUSER = CurrentUser.UserName;
                    model.CHECKTIME = DateTime.Now;
                    model.CHECKUSER = CurrentUser.UserName;
                    bool result = model.Insert();
                    //更新修约表
                    T_MID_HOUR_CModel hourC = new T_MID_HOUR_CModel();
                    hourC.VALUE = model.CHECKVALUE;
                    hourC.STATUS = "N";
                    if (!string.IsNullOrEmpty(row["ID"].ToString())) hourC.Update(T_MID_HOUR_C.ID == row["ID"].ToString());
                    else
                    {
                        hourC.DEVICECODE = deviceCode;
                        hourC.ITEMCODE = ItemCode;
                        hourC.SUBITEMCODE = subItemCode;
                        hourC.RECTIME = RECTIME;
                        hourC.CREATETIME = DateTime.Now;
                        hourC.Insert();
                    }
                }
            }
            return Json(new
            {
                total = dtData.Rows.Count,
                rows = dtData.ToDynamicList()
            });
        }
        public ActionResult GetLastMinData(string PkId, string Date)
        {
            DataTable dtResult = new DataTable();
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
                .From(DB.T_BASE_COMPANY_PK)
                .Where(T_BASE_COMPANY_PK.ID == PkId)
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
                WHERE (T_BASE_COMPANY_PK_TX.PKID = '{0}') AND (BASDIC.TYPECODE = '{1}')", PkId, itemTypeCode))
                .Native().ExecToDataTable();
            if (dtItem.Rows.Count == 0) return new EmptyResult();

            FieldModel where = BASDIC.TYPECODE == itemTypeCode & T_BASE_COMPANY_PK_TX.CLCS.Instr(BASDIC.CODE) > 0;
            if (!string.IsNullOrEmpty(PkId)) where &= T_BASE_COMPANY_PK.ID == PkId;
            if (!string.IsNullOrEmpty(Date))
            {
                DateTime time = DateTime.Parse(Date);
                where &= T_MID_MINUTE.RECTIME.BetweenAnd(time.AddHours(-1), time);
            }
            DataTable dtData = SqlModel.Select(T_MID_MINUTE.RECTIME, T_MID_MINUTE.DEVICECODE, T_MID_MINUTE.ITEMCODE, T_MID_MINUTE.SUBITEMCODE,
                T_MID_MINUTE.VALUE)
                .From(DB.T_MID_MINUTE)
                .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK_TX.MN == T_MID_MINUTE.DEVICECODE)
                .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                .LeftJoin(DB.BASDIC).On(BASDIC.CODE == T_MID_MINUTE.ITEMCODE)
                .Where(where)
                .OrderByDesc(T_MID_MINUTE.RECTIME)
                .ExecToDataTable();

            dtResult.Columns.Add("TIME");
            List<dynamic> titleList = new List<dynamic>();
            List<dynamic> subTitleList = new List<dynamic>();
            foreach (DataRow row in dtItem.Rows)
            {
                if (titleList.Count(p => p.ITEMCODE == row["ITEMCODE"].ToString()) == 0)
                {
                    int length = dtItem.Select("ITEMCODE ='" + row["ITEMCODE"].ToString() + "'").Length;
                    titleList.Add(
                                    new
                                    {
                                        ITEMCODE = row["ITEMCODE"].ToString(),
                                        ITEMTEXT = row["ITEMTEXT"].ToString(),
                                        UNIT = length == 1 ? row["UNIT"].ToString() : "",
                                        COLSPAN = length
                                    });
                }

                if (row["SUBITEMCODE"] != null && !string.IsNullOrEmpty(row["SUBITEMCODE"].ToString()))
                {
                    subTitleList.Add(
                        new
                        {
                            ITEMCODE = row["ITEMCODE"].ToString(),
                            SUBITEMCODE = row["SUBITEMCODE"].ToString(),
                            SUBITEMTEXT = row["SUBITEMTEXT"].ToString(),
                            UNIT = row["UNIT"].ToString()
                        });
                    dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString());
                    dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString() + "_UNIT");
                }
                else
                {
                    if (!dtResult.Columns.Contains(row["ITEMCODE"].ToString()))
                    {
                        dtResult.Columns.Add(row["ITEMCODE"].ToString());
                        dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_UNIT");
                    }
                }
            }

            var timeList = (from p in dtData.AsEnumerable()
                            select p.Field<DateTime>("RECTIME")).Distinct();
            foreach (DateTime time in timeList)
            {
                DataRow[] dataRows = dtData.Select(" RECTIME='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                if (dataRows.Length == 0) continue;
                DataRow newRow = dtResult.NewRow();
                newRow["TIME"] = time.ToString("yyyy-MM-dd HH:mm:ss");
                foreach (var item in titleList)
                {
                    if (item.COLSPAN != 1) continue;
                    DataRow[] rows = dataRows.Where(p => p.Field<string>("ITEMCODE") == item.ITEMCODE).ToArray();
                    if (rows.Length > 0)
                    {
                        newRow[item.ITEMCODE] = rows[0]["VALUE"];
                        newRow[item.ITEMCODE + "_UNIT"] = item.UNIT;
                    }
                }
                foreach (var item in subTitleList)
                {
                    DataRow[] rows = dataRows.Where(p => p.Field<string>("ITEMCODE") == item.ITEMCODE & p.Field<string>("SUBITEMCODE") == item.SUBITEMCODE).ToArray();
                    if (rows.Length > 0)
                    {
                        newRow[item.ITEMCODE + "_" + item.SUBITEMCODE] = rows[0]["VALUE"];
                        newRow[item.ITEMCODE + "_" + item.SUBITEMCODE + "_UNIT"] = item.UNIT;
                    }
                }
                dtResult.Rows.Add(newRow);
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return Json(new
            {
                TITLE = new
                {
                    MAINTITLE = titleList,
                    SUBTITLE = subTitleList
                },
                DATA = new
                {
                    total = dtResult.Rows.Count,
                    rows = dtResult.ToDynamicList()
                }
            });
        }

        public ActionResult SaveRounding(string checkValue, string checkReason, string checkRemark, string checkFj, string id,
            string deviceCode, string itemCode, string subItemCode, string recTime, string value, string unit, string status)
        {
            T_MID_CHECKRECORDModel model = new T_MID_CHECKRECORDModel();
            model.CHECKVALUE = string.IsNullOrEmpty(checkValue) ? decimal.Zero : decimal.Parse(checkValue);
            model.REASON = checkReason;
            model.REMARK = checkRemark;
            model.FJ = checkFj;
            model.CHECKTYPE = "2";//1-按技术规则自动修约；2-手动修约

            model.MN = deviceCode;
            model.ITEMCODE = itemCode;
            model.SUBITEMCODE = subItemCode;
            model.RECTIME = DateTime.Parse(recTime);
            model.VALUE = string.IsNullOrEmpty(value) ? decimal.Zero : decimal.Parse(value);
            model.UNIT = unit;
            model.STATUS = status;

            model.CREATETIME = DateTime.Now;
            model.CREATEUSER = CurrentUser.UserName;
            model.CHECKTIME = DateTime.Now;
            model.CHECKUSER = CurrentUser.UserName;
            bool result = model.Insert();

            T_MID_HOUR_CModel hourC = new T_MID_HOUR_CModel();
            hourC.VALUE = string.IsNullOrEmpty(checkValue) ? decimal.Zero : decimal.Parse(checkValue);
            hourC.STATUS = "N";

            if (!string.IsNullOrEmpty(id)) hourC.Update(T_MID_HOUR_C.ID == id);
            else
            {
                hourC.DEVICECODE = deviceCode;
                hourC.ITEMCODE = itemCode;
                hourC.SUBITEMCODE = subItemCode;
                hourC.RECTIME = model.RECTIME;
                hourC.CREATETIME = DateTime.Now;
                hourC.Insert();
            }
            return Json(result);
        }
        private void GetTimeSpan(DateTime date, out DateTime sTime, out DateTime eTime)
        {
            sTime = new DateTime(date.Year, (date.Month - 1) / 3 * 3 + 1, 1);
            eTime = sTime.AddMonths(3).AddSeconds(-1);
        }
        private decimal 获取停运时段小时数(string deviceCode, string itemCode, DateTime stime, DateTime etime)
        {
            DataRow row = SqlModel.Select(T_MID_HOUR_C.ID.CountAs("COUNT"))
                .From(DB.T_MID_HOUR_C)
                .Where(T_MID_HOUR_C.RECTIME.BetweenAnd(stime, etime) & T_MID_HOUR_C.DEVICECODE == deviceCode & T_MID_HOUR_C.ITEMCODE == itemCode & T_MID_HOUR_C.STATUS == "F")
                .ExecToDataRow();
            return (decimal)row[0];
        }
        private decimal 获取无效时段小时数(string deviceCode, string itemCode, DateTime stime, DateTime etime)
        {
            DataRow row = SqlModel.Select(T_MID_HOUR_C.ID.CountAs("COUNT"))
                .From(DB.T_MID_HOUR_C)
                .Where(T_MID_HOUR_C.RECTIME.BetweenAnd(stime, etime) & T_MID_HOUR_C.DEVICECODE == deviceCode & T_MID_HOUR_C.ITEMCODE == itemCode
                & T_MID_HOUR_C.STATUS.IsNotNull() & T_MID_HOUR_C.STATUS != "F" & T_MID_HOUR_C.STATUS != "N")
                .ExecToDataRow();
            return (decimal)row[0];
        }
        private decimal 季度有效数据捕集率(string deviceCode, string itemCode, DateTime stime, DateTime etime)
        {
            int HourCount = (int)(etime - stime).TotalHours;
            decimal 停运小时数 = 获取停运时段小时数(deviceCode, itemCode, stime, etime);
            decimal 无效小时数 = 获取无效时段小时数(deviceCode, itemCode, stime, etime);
            if (HourCount <= 停运小时数) return 0;
            else return (HourCount - 停运小时数 - 无效小时数) * 100 / (HourCount - 停运小时数);
        }
        private decimal _25小时内失控小时数(DateTime time, string deviceCode, string itemCode)
        {
            DataRow row = SqlModel.Select(T_MID_HOUR_C.ID.CountAs("COUNT"))
               .From(DB.T_MID_HOUR_C)
               .Where(T_MID_HOUR_C.RECTIME.BetweenAnd(time.AddHours(-25), time) & T_MID_HOUR_C.DEVICECODE == deviceCode & T_MID_HOUR_C.ITEMCODE == itemCode & T_MID_HOUR_C.SUBITEMCODE == "PFL"
               & T_MID_HOUR_C.STATUS.In("'" + string.Join("','", LostControlFlag.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) + "'"))
               .ExecToDataRow();

            return (decimal)row[0];
        }
        private decimal _25小时内无效小时数(DateTime time, string deviceCode, string itemCode)
        {
            DataRow row = SqlModel.Select(T_MID_HOUR_C.ID.CountAs("COUNT"))
               .From(DB.T_MID_HOUR_C)
               .Where(T_MID_HOUR_C.RECTIME.BetweenAnd(time.AddHours(-25), time) & T_MID_HOUR_C.DEVICECODE == deviceCode & T_MID_HOUR_C.ITEMCODE == itemCode & T_MID_HOUR_C.SUBITEMCODE == "PFL"
               & T_MID_HOUR_C.STATUS.IsNotNull() & T_MID_HOUR_C.STATUS != "N")
               .ExecToDataRow();

            return (decimal)row[0];
        }
        private decimal 上次校准前有效最大值(DateTime time, string deviceCode, string itemCode, int hourCount)
        {
            string sqlStr = string.Format(@"select max(t.value)maxvalue from t_mid_hour_c t
                where t.devicecode='{0}' and t.itemcode='{1}' and t.subitemcode='PFL' and rownum<={3} and (status is null or status ='N' ) and t.rectime<(
                select max(rectime) from t_mid_hour_c where devicecode='{0}' and itemcode='{1}' and subitemcode='PFL' and status='C' and rectime<to_date('{2}','yyyy-mm-dd hh24:mi:ss')
                )", deviceCode, itemCode, time.ToString("yyyyMM-dd HH:mm:ss"), hourCount);
            DataRow row = SqlModel.Select(sqlStr).Native()
               .ExecToDataRow();

            return string.IsNullOrEmpty(row[0].ToString()) ? -999 : (decimal)row[0];
        }
        private decimal 上次失效前有效最大值(DateTime time, string deviceCode, string itemCode, int hourCount)
        {
            string sqlStr = string.Format(@"select max(t.value)maxvalue from t_mid_hour_c t
                where t.devicecode='{0}' and t.itemcode='{1}' and t.subitemcode='PFL' and (status is null or status ='N' )  and rownum<={3} and t.rectime<=(
                select max(rectime) from t_mid_hour_c where devicecode='{0}' and itemcode='{1}' and subitemcode='PFL' and (status is null or status ='N' )  and rectime<to_date('{2}','yyyy-mm-dd hh24:mi:ss')
                )", deviceCode, itemCode, time.ToString("yyyyMM-dd HH:mm:ss"), hourCount);
            DataRow row = SqlModel.Select(sqlStr).Native()
               .ExecToDataRow();

            return string.IsNullOrEmpty(row[0].ToString()) ? -999 : (decimal)row[0];
        }

        private DataTable 获取数据缺失的时间范围(DateTime time, string deviceCode, string itemCode, string subitem)
        {
            string sqlStr = string.Format(@"select * from 
            (select max(t.rectime)time1 from t_mid_hour_c t
            where  t.devicecode='{0}' and t.itemcode='{1}' {2} and t.rectime<to_date('{3}','yyyy-mm-dd hh24:mi:ss')),
            (select min(t.rectime)time2 from t_mid_hour_c t
            where  t.devicecode='{0}' and t.itemcode='{1}' {2} and t.rectime>to_date('{3}','yyyy-mm-dd hh24:mi:ss'))",
            deviceCode, itemCode, string.IsNullOrEmpty(subitem) ? " and t.subitemcode is null" : "and t.subitemcode='" + subitem + "'", time.ToString("yyyy-MM-dd HH:mm:ss"));
            return SqlModel.Select(sqlStr).Native().ExecToDataTable();
        }
        private List<decimal> 获取时间范围数据(DateTime stime, int count, string deviceCode, string itemCode, string subitem)
        {
            string sqlStr = string.Format(@"select * from (
                        select value from t_mid_hour_c t
                        where  t.devicecode='{0}' and t.itemcode='{1}' {2}
                        and t.rectime <= to_date('{3}','yyyy-mm-dd hh24:mi:ss')
                        order by t.rectime desc)
                where rownum<={4}",
            deviceCode,
            itemCode,
            string.IsNullOrEmpty(subitem) ? " and t.subitemcode is null" : "and t.subitemcode='" + subitem + "'",
            stime.ToString("yyyyMM-dd HH:mm:ss"),
            count);
            return SqlModel.Select(sqlStr).Native().ExecToDynamicList().Select(p => (decimal)p["VALUE"]).ToList();
        }
    }
}
