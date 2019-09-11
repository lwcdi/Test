
using w.ORM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;
using UI.Web;
using System.Web.Script.Serialization;
using UI.Web.Content.code.Handler;
using System.Text;

namespace UI.Web.Areas.StainMonitor.Controllers
{


    [ActionParam]
    public class DataTransferStateController : ListControllerExt
    {

        public override ActionResult List(int? navId)
        {
            //ViewData["UserTypeID"] = CurrentUser.UserTypeID;
            ViewData["selectId"] = 2;
            return View(navId ?? 0);
        }

        //public override ActionResult List(RequestData data)
        //{
        //    return DataHandle(DataOutType.GetDataSource, data);
        //    //return View();
        //}
        // GET: /WarnContent/WarnContent/
        protected override SqlModel GetSqlModel(RequestData data)
        {
            return SqlModel.SelectAll().From(DB.T_MID_MINUTE);


        }
        public override ActionResult ListExt(RequestData data)
        {
           // ViewData["fs"] = data.Get("fs");
           // ViewData["fq"] = data.Get("fq");
           // ViewData["vocs"] = data.Get("vocs");
            return base.ListExt(data);
        }

        /// <summary>
        /// 主列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override DataTable GetSqlModelDataTable(RequestData data)
        {

           // CreateDataTable(data);
          //  DataTable dateTable = null;
          //  DataHandle(DataOutType.GetDataSource, data, out dateTable);
            //  return 
            return CreateDataTable(data); 

        }

       

        #region  统一数据源获取

        private DataTable CreateDataTable(RequestData data)
        {
            /*
            1, 获取列头前台绑定信息(字段)
            4, 判断数据类型
            5，获取数据源
            6，组装合并DataTable返回完整数据
           */
            DataTable createTable = new DataTable();
            List<dynamic> getCols = SerializerHelper.Deserialize<List<dynamic>>(data.Get("Cols"));
            //List<dynamic> getRows = null;
            getCols.ForEach(m =>
            { createTable.Columns.Add(m["field"]); });
            //createTable.Columns.Add("WEB_STATE");
            //createTable.Columns.Add("COMPANY_NAME");
            //createTable.Columns.Add("PK_NAME");
            //createTable.Columns.Add("ALERT_TIME");
            GetDataTransferList(data, createTable, getCols);
            



            return createTable;
        }

        private void GetDataTransferList(RequestData data, DataTable createTable, List<dynamic> getCols)
        {
            try
            {
                DataRow dr = null;
                var selectList = GetDataTypeDataToTable(data).AsEnumerable();
                // List<dynamic> selectList = null;
                GetCompanyList(data).ForEach(d =>
                {
                    // var query = from a in selectList select a;
                   int  NumOfRecords = GetItemCode(data, Convert.ToString(d["PKID"])).Rows.Count;
                    switch (data.Get("DataType"))
                    {
                        case "H":
                            NumOfRecords = NumOfRecords * 6;
                            break;
                        case "D":
                            NumOfRecords = NumOfRecords * 144;
                            break;
                    }

                    dr = createTable.NewRow();
                    decimal  realNum= selectList.Where(p =>  Convert.ToDecimal( p["PKID"] ) == Convert.ToDecimal(d["PKID"])).Count();
                    dr["COMPANY_NAME"] = d["COMPANY_NAME"];
                    dr["PK_NAME"] = d["PK_NAME"];
                    dr["NUMBER_OF_RECORDS"] = NumOfRecords;
                    dr["REAL_NUM_RECORDS"] = realNum;
                    dr["REPORT_RATE"] = Math.Round(realNum / decimal.Parse(NumOfRecords.ToString()), 2)*100+"%";
                    dr["CORRECTION_NUMBER"] = GetCorrection(Convert.ToString(d["PKID"]));
                   // dr["ALERT_TIME"] = selectList.Select(m => m["RECTIME"]).Max();
                    createTable.Rows.Add(dr);
                });
            }
            catch (Exception ex)
            {
                string aao = ex.Message + ex.StackTrace;
            }

        }

        /// <summary>
        /// 获取企业
        /// </summary>
        /// <returns></returns>
        private List<dynamic> GetCompanyList(RequestData data)
        {
            DataTable dataTable = new DataTable();
            FieldModel where = null;
            if (!string.IsNullOrEmpty(data.Get("CompanyID")) &&data.Get("CompanyID") != "-1")
                where &= T_BASE_COMPANY_PK.COMPANYID == data.Get("CompanyID");
            else if (!string.IsNullOrEmpty(data.Get("CompanyID")) && data.Get("PKID") != "-1")
                where &= T_BASE_COMPANY_PK.ID == data.Get("PKID");
                SqlModel model = SqlModel.SelectAll(T_BASE_COMPANY.ISONLINE, T_BASE_COMPANY_PK.ID.As("PKID"),T_BASE_COMPANY_PK.NAME.As("PK_NAME"), T_BASE_COMPANY.NAME.As("COMPANY_NAME")).From(DB.T_BASE_COMPANY_PK)
                .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK.ID== T_BASE_COMPANY_PK_TX.PKID)
                .LeftJoin(DB.T_BASE_COMPANY).On(T_BASE_COMPANY.ID == T_BASE_COMPANY_PK.COMPANYID)
                .Where(where);
            return model.ExecToDynamicList();
        }

        /// <summary>
        /// 获取24小时数据
        /// </summary>
        /// <param name="pkId"></param>
        /// <returns></returns>
        private List<dynamic> GetRealTime(string pkId)
        {
            DataTable dataTable = new DataTable();
            FieldModel where = null;
            string beginTime = DateTime.Now.AddHours(-24).ToString("yyyy-MM-dd HH:00");
            string endTime = DateTime.Now.ToString("yyyy-MM-dd HH:00");
            string sql = @"SELECT T_MID_HOUR.ID,
                                  T_MID_HOUR.DEVICECODE,
                                  T_MID_HOUR.ITEMCODE,
                                  T_MID_HOUR.RECTIME,
                                  T_MID_HOUR.VALUE,
                                  T_MID_HOUR.STATUS,
                                  T_MID_HOUR.SUBITEMCODE,
                                  T_MID_HOUR.UNIT,
                                  T_MID_HOUR.CREATETIME,
                                  T_BASE_COMPANY_PK.NAME AS PK_NAME
                           FROM T_MID_HOUR
                           LEFT JOIN T_BASE_COMPANY_PK_TX ON T_MID_HOUR.DEVICECODE = T_BASE_COMPANY_PK_TX.MN
                           LEFT JOIN T_BASE_COMPANY_PK ON T_BASE_COMPANY_PK.ID = T_BASE_COMPANY_PK_TX.PKID
                           WHERE ((T_MID_HOUR.RECTIME >= to_date('{0}','yyyy-MM-dd HH24:MI:SS')) 
                           AND (T_MID_HOUR.RECTIME <= to_date('{1}','yyyy-MM-dd HH24:MI:SS'))) 
                           AND (T_BASE_COMPANY_PK.ID = {2})";
            sql = string.Format(sql, beginTime, endTime, pkId);
             //where &= T_MID_HOUR.RECTIME >= DateTime.Parse( DateTime.Now.AddHours(-24).ToString("yyyy-MM-dd HH:00"));
             //where &= T_MID_HOUR.RECTIME <= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:00"));
             //where &= T_BASE_COMPANY_PK.ID == pkId;
             //SqlModel model = SqlModel.SelectAll(T_BASE_COMPANY_PK.NAME.As("PK_NAME")) //, T_BASE_COMPANY.NAME.As("COMPANY_NAME")
             //.From(DB.T_MID_HOUR)
             //.LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_MID_HOUR.DEVICECODE == T_BASE_COMPANY_PK_TX.MN)
             //.LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
             //.Where(where);

            return SqlModel.Select(sql).Native().ExecToDynamicList();
        }

        /// <summary>
        /// 获取公用数据源方法
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        private DataTable GetDataTypeDataToTable(RequestData requestData)
        {

            DataTable dt = null;
            SqlModel sqlmodel = null;
            string sql = @"
                         select a.*,b.*,c.* , a.rowid, d.TITLE as ITEMCODE_TEXT,d.remark as  UNIT_TEXT2,f.TITLE as UNIT_TEXT, e.TITLE as DATASTATUS  from {0} a
                         left join T_BASE_COMPANY_PK_TX b  on a.devicecode = b.mn
                         left join T_BASE_COMPANY_PK c on b.pkid = c.id  
                          LEFT JOIN BASDIC d ON  d.CODE= a.ITEMCODE
                          LEFT JOIN BASDIC e ON e.CODE =a.STATUS
                          LEFT JOIN BASDIC f ON f.TYPECODE=d.CODE
                         where  RECTIME>= to_date('{1}','yyyy/MM/dd hh24:mi:ss')  
                         and   RECTIME<=to_date('{2}','yyyy/MM/dd hh24:mi:ss') ";
            if (requestData.Get("CompanyID") != null && requestData.Get("CompanyID") != "-1")
                sql = sql + " and  c.COMPANYID=" + requestData.Get("CompanyID");
            else
                sql = sql + " and  b.PKID=" + requestData.Get("PKID");
            if (!string.IsNullOrEmpty(requestData.Get("DataType")))
            {
                switch (requestData.Get("DataType"))
                {
                    case "M":
                        sql = string.Format(sql, "T_MID_MINUTE", requestData.Get("BeginTime"), requestData.Get("EndTime"));
                        break;
                    case "H":
                        sql = string.Format(sql, "T_MID_HOUR", Convert.ToDateTime(requestData.Get("BeginTime")).ToString("yyyy-MM-dd HH:00"), Convert.ToDateTime(requestData.Get("EndTime")).ToString("yyyy-MM-dd HH:00"));
                        break;
                    case "D":
                        sql = string.Format(sql, "T_MID_DAY", Convert.ToDateTime(requestData.Get("BeginTime")).ToString("yyyy-MM-dd 00:00"), Convert.ToDateTime(requestData.Get("EndTime")).ToString("yyyy-MM-dd 00:00"));
                        break;
                    default:
                        //sql = string.Format(sql, "T_MID_DAY", requestData.Get("BeginTime"), requestData.Get("EndTime"));
                        break;
                }
                using (dt = SqlModel.Select(sql).Native().ExecToDataTable())
                {
                    return dt;
                }

            }
            else
            {
                return null;
            }
            //  return dt;
        }


        /// <summary>
        ///  获取排口修约数
        /// </summary>
        /// <param name="queryPkId"></param>
        /// <returns></returns>
        [HttpPost]
        private int GetCorrection(string queryPkId)
        {
            string sql = @"SELECT T_MID_CHECKRECORD.ID
                           FROM T_MID_CHECKRECORD
                           LEFT JOIN T_BASE_COMPANY_PK_TX ON T_MID_CHECKRECORD.MN = T_BASE_COMPANY_PK_TX.MN
                           LEFT JOIN T_BASE_COMPANY_PK ON T_BASE_COMPANY_PK.ID = T_BASE_COMPANY_PK_TX.PKID
                           WHERE (T_BASE_COMPANY_PK.ID = {0})";
            sql = string.Format(sql, queryPkId);
            return SqlModel.Select(sql).Native().ExecToDynamicList().Count;
        }

    /// <summary>
    /// 获取排口下的因子
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
        private DataTable GetItemCode(RequestData data, string queryPkId)
        {
            string pkId = queryPkId;// data.Get("PKID");
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
               .From(DB.T_BASE_COMPANY_PK)
               .Where(T_BASE_COMPANY_PK.ID == pkId)
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
            DataTable dtItem = SqlModel.Select(string.Format(@"SELECT BASDIC.CODE AS ITEMCODE,BASDIC.TITLE AS ITEMTEXT,(CASE  WHEN BASDIC.REMARK is not null THEN BASDIC.REMARK ELSE D1.REMARK END) AS UNIT,D1.CODE AS SUBITEMCODE,D1.TITLE AS SUBITEMTEXT
                FROM T_BASE_COMPANY_PK_TX
                LEFT JOIN BASDIC ON instr(T_BASE_COMPANY_PK_TX.CLCS, BASDIC.CODE) > 0
                LEFT JOIN BASDIC D1 ON D1.TYPECODE = BASDIC.CODE
                WHERE (T_BASE_COMPANY_PK_TX.PKID = '{0}') AND (BASDIC.TYPECODE = '{1}')", pkId, itemTypeCode))
              .Native().ExecToDataTable();
            return dtItem;
        }

        #endregion

        #region  私有方法
        [HttpPost]
        public ActionResult GetPicData(RequestData data)
        {
            DataTable dateTable = CreateDataTable(data);
            List<string> title = new List<string>();
            GetCompanyList(data).ForEach(d =>
            {
                title.Add(d["PK_NAME"]);
            });
            List<string> fiedValue = new List<string>();
            fiedValue.Add("REPORT_RATE");
            fiedValue.Add("NUMBER_OF_RECORDS");
            fiedValue.Add("REAL_NUM_RECORDS");
            fiedValue.Add("CORRECTION_NUMBER");
            Dictionary<string, string> dicItemCode = new Dictionary<string, string>();
            dicItemCode.Add("REPORT_RATE", "上报率");
            dicItemCode.Add("NUMBER_OF_RECORDS", "应报数");
            dicItemCode.Add("REAL_NUM_RECORDS", "实报数");
            dicItemCode.Add("CORRECTION_NUMBER", "修正数");
          //  dicItemCode.Add("REPORT_RATE", "上报率");
            return GetChartLineItem(dateTable, title, fiedValue, dicItemCode);
        }
        [HttpPost]
        /// <summary>
        /// 获取表头
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult GetHeadText(RequestData data)
        {
            ActionResult resutl = new EmptyResult();
            string pkId = data.Get("PKID");
            FieldModel where = null;
            if (data.Get("CompanyID") != "-1")
                where &= T_BASE_COMPANY_PK.COMPANYID == data.Get("CompanyID");
            else if(data.Get("PKID") != "-1")
                where &= T_BASE_COMPANY_PK.ID == data.Get("PKID");
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
               .From(DB.T_BASE_COMPANY_PK)
               .Where(where)
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
                WHERE (T_BASE_COMPANY_PK_TX.PKID = '{0}') AND (BASDIC.TYPECODE = '{1}')", pkId, itemTypeCode))
              .Native().ExecToDataTable();
            if (dtItem.Rows.Count == 0) return new EmptyResult();
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


            }
            resutl = Json(new
            {
                Success = true,
                TITLE = new
                {
                    MAINTITLE = titleList//,
                   // SUBTITLE = subTitleList
                }

            });

            return resutl;
        }

        [HttpPost]
        /// <summary>
        /// 获取表头
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult GetOnlineStation(RequestData data)
        {
            List<dynamic> companyLis = GetCompanyList(data);
            ActionResult resutl = new EmptyResult();
            resutl = Json(new
            {
                Success = true,
                 TotalNum = companyLis.Count,
                 OnlineTotal = companyLis.Where(p => p["ISONLINE"]=="1").Count(),
                 DownlineTotal = companyLis.Where(p => p["ISONLINE"] != "1").Count()
               
            });

            return resutl;
        }

        public ActionResult GetChartLineItem(DataTable dtSrouce, List<string> titleList, List<string> fiedValueList, Dictionary<string, string> dicItemCode)
        {
           
           List<KeyValuePair<string, string>> y = new List<KeyValuePair<string, string>>();
           // List<string> y = new List<string>();
            StringBuilder x = new StringBuilder();
            StringBuilder title = new StringBuilder();
            StringBuilder y1 = new StringBuilder();
           // Dictionary<string, string> dicItemCode = new Dictionary<string, string>();
            if (dtSrouce.Rows.Count > 0)
            {

                
                //foreach (var titleItem in titleList)
                //  dicItemCode.Add(titleItem.ITEMCODE, titleItem.ITEMTEXT);

                foreach (DataRow dr in dtSrouce.Rows)
                    x.Append("'" + dr["PK_NAME"].ToString() + "',");
                foreach (var item in fiedValueList)
                {
                    y1 = new StringBuilder();
                    foreach (DataRow drow in dtSrouce.Rows)
                        y1.Append("'" + drow[item].ToString() + "',");

                    if (y1.Length > 0) y1 = y1.Remove(y1.Length - 1, 1);
                    //y.Add(y1.ToString());
                     y.Add(new KeyValuePair<string, string>(dicItemCode[item] , y1.ToString()));
                    title.Append("'" + dicItemCode[item]+"',");
                }
                if (x.Length > 0) x = x.Remove(x.Length - 1, 1);
                if (title.Length > 0) title = title.Remove(title.Length - 1, 1);


               
            }
            return Json(EChartsHelper.GetBaseChart(x.ToString(), y, "", "", title.ToString(), "","bar"));
        }
        private enum DataOutType
        {
            ExportExcel = 1,
            GetDataSource = 2,
            GetPicData=3
        }

        #endregion


        #region 抽象类
        protected override bool DoAdd(RequestData data)
        {
            return true;
        }

        protected override bool DoEdit(RequestData data)
        {
            bool reulst = true;
            // string entGuid = data.Get("entGuid");
            try
            {


            }
            catch (Exception ex)
            {

            }
            return reulst;
        }

        protected override bool DoDelete(RequestData data)
        {
            //  T_THEPEAK_MAIN_LIST_INFOModel model = new T_THEPEAK_MAIN_LIST_INFOModel();
            // return model.Delete(T_THEPEAK_MAIN_LIST_INFO.ID == data.Get("id"));
            return false;
        }

        public ActionResult WarnDetail(RequestData data)
        {
            ViewData["warnID"] = data.Get("warnID");
            ViewData["warnClass"] = data.Get("warnClass");
            return View();
        }
        public ActionResult WarnDetailData(RequestData data)
        {
            string warnClass = data.Get("warnClass");
            string warnID = data.Get("warnID");
            FieldModel where = null;
            List<dynamic> list = null;

            return this.SuccessResult("", list);

        }
        //public ActionResult ChangeWarnState(RequestData data)
        //{
        //    string warnClass = data.Get("warnClass");
        //    string warnID = data.Get("warnID");
        //    string state = data.Get("state");
        //    bool result = this.ChangeWarnState(warnClass, warnID, state);
        //    if (result)
        //    {
        //        return this.SuccessResult("");
        //    }
        //    else
        //    {
        //        return this.ErrorResult("");
        //    }
        //}
        #endregion
    }


}
