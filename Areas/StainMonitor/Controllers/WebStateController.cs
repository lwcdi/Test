
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
    public class WebStateController : ListControllerExt
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
            GetWebStateList(data, createTable, getCols);
            



            return createTable;
        }

        private void GetWebStateList(RequestData data, DataTable createTable, List<dynamic> getCols)
        {
            try
            {
                DataRow dr = null;
                List<dynamic> selectList = null;
                GetCompanyList(data).ForEach(d =>
                {
                    selectList = GetRealTime(Convert.ToString(d["ID"]));
                   // var query = from a in selectList select a;

                   
                    dr = createTable.NewRow();

                    getCols.ForEach(e =>
                    {
                        if (e["field"] != "WEB_STATE" && e["field"] != "COMPANY_NAME" && e["field"] != "PK_NAME" && e["field"] != "ALERT_TIME")
                        {
                            string itemCode = e["field"]; //itemCode
                            int countRectTime = selectList.Where(s => s["ITEMCODE"] == itemCode).Count(); 
                            dr[e["field"]] = countRectTime >= 24 ? "正常" : "离线" + (24 - countRectTime).ToString() + "小时";
                        }
                    });
                    dr["WEB_STATE"] = d["ISONLINE"] =="1"?"在线":"离线"; 
                    dr["COMPANY_NAME"] = d["COMPANY_NAME"];
                    dr["PK_NAME"] = d["PK_NAME"];
                    dr["ALERT_TIME"] = selectList.Select(m => m["RECTIME"]).Max();
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
                SqlModel model = SqlModel.SelectAll(T_BASE_COMPANY.ISONLINE,T_BASE_COMPANY_PK.NAME.As("PK_NAME"), T_BASE_COMPANY.NAME.As("COMPANY_NAME")).From(DB.T_BASE_COMPANY_PK)
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



        #endregion

        #region  私有方法

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
