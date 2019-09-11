
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
    public class StainMonitorController : ListControllerExt
    {
        public override ActionResult ListExt(RequestData data)
        {
           // ViewData["fs"] = data.Get("fs");
           // ViewData["fq"] = data.Get("fq");
           // ViewData["vocs"] = data.Get("vocs");
            return base.ListExt(data);
        }
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


        protected override DataTable GetSqlModelDataTable(RequestData data)
        {
            // ViewData["selectId"] = 2;
            //建个表结构
            //DataTable dtData = CreateTable();
            //try
            //{
            //    string recTime = string.Empty;
            //    DataRow drNew = null;
            //     GetDataTypeData(data).ForEach(
            //     m =>
            //     {
            //         //时间不同表明数据不属于同一行
            //         if (m["RECTIME"] != recTime)
            //         {
            //             drNew = dtData.NewRow();
            //             //第一次时间为空不用添加，下一次轮动时间发生变化了说明需要添加第二行数据来填充
            //             if(!string.IsNullOrEmpty(recTime))
            //             dtData.Rows.Add(drNew);
            //         }
            //          //行转列
            //          DtRoweValuate(drNew, m);
            //         recTime = m["RECTIME"];
            //     });


            //}
            //catch (Exception ex)
            //{

            //}

            //int count = dtWarn.Rows.Count;


         
            DataTable dateTable = null;
            DataHandle(DataOutType.GetDataSource, data, out dateTable);
            //  return 
            return dateTable;
        }


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
            //switch (warnClass)
            //{
            //    // yecha、peak、voc、dache
            //    case "voc":
            //        where = T_DATA_WARNING.ID == warnID;
            //        list = GetVocTable(where);
            //        break;
            //    case "peak":
            //        where = T_DATA_WARNING_PEAK.ID == warnID;
            //        list = GetCuoFengTable(where);
            //        break;
            //    case "yecha":
            //        where = T_WARNING.ID == warnID;
            //        list = GetYeChaTable(where);
            //        break;
            //    case "dache":
            //        where = T_TRAFFIC_WARNING.ID == warnID;
            //        list = GetDaCheTable(where);
            //        break;
            //}
            return this.SuccessResult("", list);

        }
        public ActionResult ChangeWarnState(RequestData data)
        {
            string warnClass = data.Get("warnClass");
            string warnID = data.Get("warnID");
            string state = data.Get("state");
            bool result = this.ChangeWarnState(warnClass, warnID, state);
            if (result)
            {
                return this.SuccessResult("");
            }
            else
            {
                return this.ErrorResult("");
            }
        }

        #region  私有方法

        private List<dynamic> GetDataTypeData(RequestData requestData)
        {
            SqlModel sqlmodel = null;
            string sql = @"
                         select *, a.rowid from {0} a
                         left join T_BASE_COMANY_PK_TX b  on a.devicecode = b.mn
                         left join T_BASE_COMANY_PK c on b.pkid = c.id  
                         where  RECTIME>= to_date('{0}','yyyy/MM/dd hh24:mi:ss')  
                         and   RECTIME<=to_date('{2}','yyyy/MM/dd hh24:mi:ss') ";

            switch (requestData.Get("DataType"))
            {
                case "M":
                    sql = string.Format(sql, "T_MID_MINUTE", requestData.Get("BeginTime"), requestData.Get("EndTime"));
                    break;
                case "H":
                    sql = string.Format(sql, "T_MID_HOUR", requestData.Get("BeginTime"), requestData.Get("EndTime"));
                    break;
                case "D":
                    sql = string.Format(sql, "T_MID_DAY", requestData.Get("BeginTime"), requestData.Get("EndTime"));
                    break;
            }
            return SqlModel.Select(sql).ExecToDynamicList();
        }
        private DataTable GetDataTypeDataToTable(RequestData requestData)
        {
            SqlModel sqlmodel = null;
            string sql = @"
                         select a.*,b.*,c.* , a.rowid,e.TITLE as DATASTATUS  from {0} a
                         left join T_BASE_COMPANY_PK_TX b  on a.devicecode = b.mn
                         left join T_BASE_COMPANY_PK c on b.pkid = c.id  
                          LEFT JOIN BASDIC d ON  d.CODE= a.ITEMCODE
                          LEFT JOIN BASDIC e ON e.CODE =a.STATUS
                         where  RECTIME>= to_date('{1}','yyyy/MM/dd hh24:mi:ss')  
                         and   RECTIME<=to_date('{2}','yyyy/MM/dd hh24:mi:ss') 
                         and  b.pkid={3}";

            if (!string.IsNullOrEmpty(requestData.Get("DataType")))
            {
                switch (requestData.Get("DataType"))
                {
                    case "M":
                        sql = string.Format(sql, "T_MID_MINUTE", requestData.Get("BeginTime"), requestData.Get("EndTime"),requestData.Get("PKID"));
                        break;
                    case "H":
                        sql = string.Format(sql, "T_MID_HOUR", requestData.Get("BeginTime"), requestData.Get("EndTime"), requestData.Get("PKID"));
                        break;
                    case "D":
                        sql = string.Format(sql, "T_MID_DAY", requestData.Get("BeginTime"), requestData.Get("EndTime"), requestData.Get("PKID"));
                        break;
                    default:
                        //sql = string.Format(sql, "T_MID_DAY", requestData.Get("BeginTime"), requestData.Get("EndTime"));
                        break;
                }
                return SqlModel.Select(sql).Native().ExecToDataTable();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 创建动态监测表
        /// </summary>
        /// <returns></returns>
        private DataTable CreateTable()
        {
            DataTable dtData = new DataTable();
            dtData.Columns.Add("ID");
            dtData.Columns.Add("DEVICECODE");
            dtData.Columns.Add("DEVICECODE_TEXT");
            dtData.Columns.Add("DEVICECODE_TEXT");
            dtData.Columns.Add("RECTIME");
            dtData.Columns.Add("A34013_PFL"); //NOX烟尘
            dtData.Columns.Add("A34013_ZSND");
            dtData.Columns.Add("A34013_ND");
            dtData.Columns.Add("A21026_PFL");//SO2 二氧化硫
            dtData.Columns.Add("A21026_ZSND");
            dtData.Columns.Add("A21026_ND");
            dtData.Columns.Add("A21002_PFL");// 氮氧化物
            dtData.Columns.Add("A21002_ZSND");//
            dtData.Columns.Add("A21002_ND");
            dtData.Columns.Add("A01013"); //帕 烟气压力
            dtData.Columns.Add("A01012"); //摄氏度 烟气温度
            dtData.Columns.Add("A19001");//百分比  含氧量
            dtData.Columns.Add("FLOW_GAS"); //废气流量 立方米
            dtData.Columns.Add("W01018_ND"); //化学需氧量
            dtData.Columns.Add("W01018_PFL");//
            dtData.Columns.Add("W21003_ND");///
            dtData.Columns.Add("W21003_PFL");//氨氮
            dtData.Columns.Add("FLOW_WATER"); //废水流量
            dtData.Columns.Add("FLOW_VOCS"); //VOCS
            dtData.Columns.Add("A25005_PFL"); //二甲苯
            dtData.Columns.Add("A25005_ZSND");
            dtData.Columns.Add("A25005_ND");
            dtData.Columns.Add("A24088_PFL"); //非甲烷总烃
            dtData.Columns.Add("A24088_ZSND");
            dtData.Columns.Add("A24088_ND");
            dtData.Columns.Add("A25002_PFL");//苯
            dtData.Columns.Add("A25002_ZSND");
            dtData.Columns.Add("A25002_ND");
            dtData.Columns.Add("A01001"); //温度
            dtData.Columns.Add("A01002"); //湿度
            dtData.Columns.Add("A25003_PFL");//甲苯
            dtData.Columns.Add("A25003_ZSND");
            dtData.Columns.Add("A25003_ND");



            //dtWarn.Columns.Add("DUN");

            return dtData;
        }

        private DataRow DtRoweValuate(DataRow dr, dynamic obj)
        {
            dr["RECTIME"] = obj["RECTIME"];
            string itemCode = obj["ITEMCODE"];
            string itemSubCode = obj["SUBITEMCODE"];
            if (dr.ItemArray.Contains(itemCode))
            {
                if (!string.IsNullOrEmpty(itemSubCode))

                    dr[itemCode + "_" + itemSubCode] = obj["VALUE"];
                else
                    dr[itemCode] = obj["VALUE"];
            }
            return dr;
        }
        private bool ChangeWarnState(string warnClass, string warnID, string state)
        {
            FieldModel where = null;
            string sql = "";
            //switch (warnClass)
            //{
            //    // yecha、peak、voc、dache
            //    case "voc":
            //        where = T_DATA_WARNING.ID == warnID;
            //        sql = string.Format("update t_data_warning t set t.state = '{0}' where t.id = {1}", state, warnID);
            //        break;
            //    case "peak":
            //        where = T_DATA_WARNING_PEAK.ID == warnID;
            //        sql = string.Format("update t_data_warning_peak t set t.state = '{0}' where t.id = {1}", state, warnID);
            //        break;
            //    case "yecha":
            //        where = T_WARNING.ID == warnID;
            //        sql = string.Format("update t_Warning t set t.state = '{0}' where t.id = {1}", state, warnID);
            //        break;
            //    case "dache":
            //        where = T_TRAFFIC_WARNING.ID == warnID;
            //        sql = string.Format("update t_traffic_warning t set t.state = '{0}' where t.id = {1}", state, warnID);
            //        break;
            //}
            int result = SqlModel.Select(sql).Native().ExecuteNonQuery();
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        public ActionResult Export(RequestData data)
        {
            /*
              1,根据排口信息找出排口类型
              2,根据排口类型找到相应的因子
              3,根据大因子找到小因子
              4,传数据源,组合表头和数据
              5,完成导出
            */
            DataTable dateTable = null;
            return DataHandle(DataOutType.ExportExcel, data, out dateTable);
        }
        [HttpPost]
        public ActionResult GetPicData(RequestData data)
        {
            DataTable dateTable = null;
            return DataHandle(DataOutType.GetPicData, data, out dateTable);
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
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
               .From(DB.T_BASE_COMPANY_PK)
               .Where(T_BASE_COMPANY_PK.ID == pkId)
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
                }

            }
            resutl = Json(new
            {
                Success = true,
                TITLE = new
                {
                    MAINTITLE = titleList,
                    SUBTITLE = subTitleList
                }

            });

            return resutl;
        }
       
        /// <summary>
        /// 统一获取
        /// </summary>
        /// <param name="outType"></param>
        private ActionResult DataHandle(DataOutType outType, RequestData data, out DataTable dtResult)
        {
            ActionResult resutl = new EmptyResult();
            JsonResult jsonResult = new JsonResult();
            string pkId = data.Get("PKID");
      
            dtResult = new DataTable();
            DataRow pkRow = SqlModel.Select(T_BASE_COMPANY_PK.TYPE)
                .From(DB.T_BASE_COMPANY_PK)
                .Where(T_BASE_COMPANY_PK.ID == pkId)
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



            DataTable dtData = GetDataTypeDataToTable(data);
            List<string> isNotExcel = new List<string>();
            dtResult.Columns.Add("RECTIME");
            List<dynamic> titleList = new List<dynamic>();
            List<dynamic> subTitleList = new List<dynamic>();
            List<KeyValuePair<int, List<ExcelHelper.HeaderStruct>>> list = new List<KeyValuePair<int, List<ExcelHelper.HeaderStruct>>>();
            List<ExcelHelper.HeaderStruct> listTitle = new List<ExcelHelper.HeaderStruct>();//一级标题
            List<ExcelHelper.HeaderStruct> subTitle = new List<ExcelHelper.HeaderStruct>();//二级标题
            listTitle.Add(new ExcelHelper.HeaderStruct("时间", 2, 1));

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

                    listTitle.Add(new ExcelHelper.HeaderStruct()
                    {
                        headerText = row["ITEMTEXT"].ToString(),
                        colSpan = length,
                        rowSpan = 1
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
                    dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString() + "_UNIT");
                    subTitle.Add(new ExcelHelper.HeaderStruct(row["SUBITEMTEXT"].ToString() + "(" + row["UNIT"].ToString() + ")", 1, 1));
                    isNotExcel.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString() + "_UNIT");
                    isNotExcel.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString() + "_STATUS");
                    //isNotExcel.Add(row["ITEMCODE"].ToString() + "_" + row["SUBITEMCODE"].ToString());
                }
                else
                {
                    if (!dtResult.Columns.Contains(row["ITEMCODE"].ToString()))
                    {
                        dtResult.Columns.Add(row["ITEMCODE"].ToString());
                        dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_STATUS");
                        dtResult.Columns.Add(row["ITEMCODE"].ToString() + "_UNIT");
                        isNotExcel.Add(row["ITEMCODE"].ToString() + "_UNIT");
                        isNotExcel.Add(row["ITEMCODE"].ToString() + "_STATUS");
                        //isNotExcel.Add(row["ITEMCODE"].ToString());
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
                newRow["RECTIME"] = time.ToString("yyyy-MM-dd HH:mm");
                foreach (var item in titleList)
                {
                    if (item.COLSPAN != 1) continue;
                    DataRow[] rows = dataRows.Where(p => p.Field<string>("ITEMCODE") == item.ITEMCODE).ToArray();
                    if (rows.Length > 0)
                    {
                        newRow[item.ITEMCODE] = rows[0]["VALUE"];
                        newRow[item.ITEMCODE + "_STATUS"] = rows[0]["DATASTATUS"];
                        newRow[item.ITEMCODE + "_UNIT"] = item.UNIT;

                    }
                }
                foreach (var item in subTitleList)
                {
                    DataRow[] rows = dataRows.Where(p => p.Field<string>("ITEMCODE") == item.ITEMCODE & p.Field<string>("SUBITEMCODE") == item.SUBITEMCODE).ToArray();
                    if (rows.Length > 0)
                    {
                        newRow[item.ITEMCODE + "_" + item.SUBITEMCODE] = rows[0]["VALUE"];
                        newRow[item.ITEMCODE + "_" + item.SUBITEMCODE + "_STATUS"] = rows[0]["DATASTATUS"];
                        newRow[item.ITEMCODE + "_" + item.SUBITEMCODE + "_UNIT"] = item.UNIT;
                    }
                }
                dtResult.Rows.Add(newRow);
            }
            if (outType == DataOutType.ExportExcel)
            {
                list.Add(new KeyValuePair<int, List<ExcelHelper.HeaderStruct>>(0, listTitle));
                list.Add(new KeyValuePair<int, List<ExcelHelper.HeaderStruct>>(1, subTitle));
                foreach (string notExcel in isNotExcel) //数据源删除不显示的列
                    dtResult.Columns.Remove(notExcel);
                DataSet dtSet = new DataSet();
                string strPath = System.Configuration.ConfigurationManager.AppSettings["UpLoadExctlFile"];
                string fileName = DateTime.Now.ToString("yyyy-MM-dd HHmmss") + Guid.NewGuid().ToString() + "数据导出.xls";
                var dir = Request.MapPath("~/" + strPath);
                if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                strPath = dir + @"\" + fileName;
                dir = "/" + System.Configuration.ConfigurationManager.AppSettings["UpLoadExctlFile"] + "/" + fileName;
                dtSet.Tables.Add(dtResult);
                ExcelHelper.SaveToFile(dtSet, strPath, list, isNotExcel);
                resutl = Json(new { Success = true, msg = "数据导出成功！", Path = dir });


            }
            else
            {


                //resutl = Json(new
                //{
                //    TITLE = new
                //    {
                //        MAINTITLE = titleList,
                //        SUBTITLE = subTitleList
                //    },
                //    DATA = new
                //    {
                //        total = dtResult.Rows.Count,
                //        rows = dtResult.ToDynamicList()
                //    }
                //});
                if (outType == DataOutType.GetPicData)
                {
                    resutl = GetChartLineItem(dtResult, titleList, subTitleList);
                }
                else
                {
                    resutl = Json(new
                    {

                        //   total =dtResult.Rows.Count,
                        //  rows =dtResult.ToDynamicList(),
                        TITLE = new
                        {
                            MAINTITLE = titleList,
                            SUBTITLE = subTitleList
                        }
                    });
                }
               
            }
            return resutl;
        }
        public ActionResult GetChartLineItem(DataTable dtSrouce, List<dynamic> titleList ,List<dynamic> subTitleList)
        {
            // FieldModel onWhere = T_MID_HOUR.RECTIME == DateTime.Parse(time);

            //  string[] itemArray = item.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            //  string ItemCode = "";
            //  string SubItemCode = "";
            ////  string title = "";
            //  if (itemArray.Length > 0)
            //  {
            //      ItemCode = itemArray[0];
            //      dynamic itemText = SqlModel.Select(BASDIC.TITLE)
            //      .From(DB.BASDIC)
            //      .Where(BASDIC.CODE == ItemCode)
            //      .ExecToDynamic();
            //      title += itemText["TITLE"];
            //  }
            //  if (itemArray.Length > 1)
            //  {
            //      SubItemCode = itemArray[1];
            //      dynamic subItemText = SqlModel.Select(BASDIC.TITLE)
            //           .From(DB.BASDIC)
            //           .Where(BASDIC.CODE == SubItemCode)
            //           .ExecToDynamic();
            //      title += " " + subItemText["TITLE"];

            //  }

            //if (!string.IsNullOrEmpty(ItemCode)) onWhere &= T_MID_HOUR.ITEMCODE == ItemCode;
            //if (!string.IsNullOrEmpty(SubItemCode)) onWhere &= T_MID_HOUR.SUBITEMCODE == SubItemCode;


             
            //DataTable dtData = SqlModel.Select(T_BASE_COMPANY_PK.NAME, T_MID_HOUR.VALUE)
            //    .From(DB.T_BASE_COMPANY_PK)
            //    .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
            //    .LeftJoin(DB.T_MID_HOUR).On(T_BASE_COMPANY_PK_TX.MN == T_MID_HOUR.DEVICECODE & onWhere)
            //    .Where(T_BASE_COMPANY_PK.ID.In("'" + string.Join("','", PkId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) + "'"))
            //    .ExecToDataTable();
            List<KeyValuePair<string, string>> y = new List<KeyValuePair<string, string>>();
            StringBuilder x = new StringBuilder();
            StringBuilder title = new StringBuilder();
            StringBuilder y1 = new StringBuilder();
            Dictionary<string, string> dicItemCode = new Dictionary<string, string>();
            if (dtSrouce.Rows.Count > 0)
            {


                foreach (var titleItem in titleList)
                    dicItemCode.Add(titleItem.ITEMCODE, titleItem.ITEMTEXT);

                foreach (DataRow dr in dtSrouce.Rows)
                    x.Append("'" + dr["RECTIME"].ToString() + "',");
                foreach (var item in subTitleList)
                {
                    y1 = new StringBuilder();
                    foreach (DataRow drow in dtSrouce.Rows)
                        y1.Append("'" + drow[item.ITEMCODE + "_" + item.SUBITEMCODE].ToString() + "',");

                    if (y1.Length > 0) y1 = y1.Remove(y1.Length - 1, 1);
                    y.Add(new KeyValuePair<string, string>(dicItemCode[item.ITEMCODE] + "_" + item.SUBITEMTEXT, y1.ToString()));
                    title.Append("'" + dicItemCode[item.ITEMCODE] + "_" + item.SUBITEMTEXT + "',");
                }
                if (x.Length > 0) x = x.Remove(x.Length - 1, 1);
                if (title.Length > 0) title = title.Remove(title.Length - 1, 1);


                //foreach (DataRow row in dtData.Rows)
                //{
                //    x.Append("'" + row["NAME"].ToString() + "',");

                //    y1.Append("'" + row["VALUE"].ToString() + "',");
                //}

                //  if (y1.Length > 0) y1 = y1.Remove(y1.Length - 1, 1);
                //  y.Add(new KeyValuePair<string, string>(title, y1.ToString()));
            }
            return Json(EChartsHelper.GetLinesChart(x.ToString(), y,"", "", title.ToString(), ""));
        }

        private enum DataOutType
        {
            ExportExcel = 1,
            GetDataSource = 2,
            GetPicData=3
        }




      


        #endregion
    }


}
