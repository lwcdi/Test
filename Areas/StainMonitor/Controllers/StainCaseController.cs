
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
    public class StainCaseController : ListControllerExt
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
            string dataType = data.Get("DataBusiness");
            List<dynamic> getCols = SerializerHelper.Deserialize<List<dynamic>>(data.Get("Cols"));
            //List<dynamic> getRows = null;
            getCols.ForEach(m =>
            { createTable.Columns.Add(m["field"]); });
            switch (dataType)
            {
                case "TotalEmission":
                    GetTotalEmissionSource(data, createTable);
                    break;
                case "DataTransfer":
                    GetDataTransferSource(data, createTable);
                    break;
                case "ExcessivePollut":
                    GetExcessivePollutSource(data, createTable);
                    break;
                case "ExceptionStain":
                    GetExceptionStainSource(data, createTable);
                    break;
                case "ExceptionPFL":
                    GetExceptionPFLSource(data, createTable);
                    break;
                case "MNDownLine":
                    GetMNDownLineSource(data, createTable);
                    break;
                case "ControlFacilities":
                    GetControlFacilitiesSource(data, createTable);
                    break;
                case "DataAcquisition":
                    GetDataAcquisitionSource(data, createTable);
                    break;
                case "Monitoring":
                    GetMonitoringSource(data, createTable);
                    break;
                case "Governance":
                   GetGovernanceSource(data, createTable);
                    break;
                default:
                    GetExceptionPFLSource(data, createTable);
                    break;
            }



            return createTable;
        }

        /// <summary>
        /// 排放总量提示
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetTotalEmissionSource(RequestData data, DataTable dataTable)
        {
            DataTable dt = GetDataTypeDataToTable(data);
            if (dt != null)
            {
                var query = from a in dt.AsEnumerable()
                            group a by new { itemCode = a.Field<string>("ITEMCODE") } //Rectime = a.Field<DateTime>("RECTIME").ToString(), 
               into b
                            select new
                            {
                                Rectime = b.Select(c => c.Field<DateTime>("RECTIME")).First().ToString(),
                                itemCodeText = b.Select(c => c.Field<string>("ITEMCODE_TEXT")).First(),
                                PFL = b.Sum(c => Convert.ToDecimal(c.Field<Decimal>("VALUE"))),
                                UNIT = b.Select(c => c.Field<string>("UNIT_TEXT")).First(),
                                UNIT2 = b.Select(c => c.Field<string>("UNIT_TEXT2")).First()
                            };
                DataRow dr = null;
                query.ToList().ForEach(d =>
                {
                    dr = dataTable.NewRow();
                    dr["STAIN_TEXT"] = d.itemCodeText;
                    dr["SUM_PFL"] = d.PFL;
                    dr["UNIT"] = d.UNIT==null? d.UNIT2: d.UNIT;
                    dr["FZ"] = double.Parse(d.PFL.ToString()) * 0.68;
                    dr["TOTAL_RATE"] = Math.Round( Convert.ToDecimal(dr["SUM_PFL"]) / Convert.ToDecimal(dr["FZ"]),2);
                    dataTable.Rows.Add(dr);
                });
            }
            //string sql = @"";
            return dataTable;
        }
        /// <summary>
        /// 数据传输情况
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetDataTransferSource(RequestData data, DataTable dataTable)
        {
            int NumOfRecords = 0;
            DataTable dt = GetDataTypeDataToTable(data);
            if (dt != null)
            {
                var query = from a in dt.AsEnumerable()
                            group a by new { PKNO = a.Field<string>("CODE") } //Rectime = a.Field<DateTime>("RECTIME").ToString(), 
                              into b
                            select new
                            {
                                PKID= b.Select(c => c.Field<string>("PKID")).First(),
                                Rectime = b.Select(c => c.Field<DateTime>("RECTIME")).First().ToString(),
                                PKNO = b.Select(c => c.Field<string>("CODE")).First().ToString(),
                                MN = b.Select(c => c.Field<string>("MN")).First().ToString(),
                                // itemCodeText = b.Select(c => c.Field<string>("ITEMCODE_TEXT")).First(),
                                RealUpCount = b.Select(c => c.Field<string>("ITEMCODE")).Count()
                                //  UNIT = b.Select(c => c.Field<string>("UNIT_TEXT"))
                            };
               
                //一般情况下默认一个排口一个数采（以后再改一排多采，现在不为小概率事件做改动）
               
                
                DataRow dr = null;
                query.ToList().ForEach(d =>
                {
                    dr = dataTable.NewRow();
                    dr["STATE"] = IsHaveDataSource(data) > 0 ? 1 : 0; //1为正常0为中断
                    dr["PK_NO"] = d.PKNO;
                    dr["WEB_STATE"] = int.Parse(dr["STATE"].ToString()) > 0 ? "通讯正常" : "通讯中断";
                    dr["MN_NO"] = d.MN;
                    NumOfRecords = GetItemCode(data, d.PKID).Rows.Count;
                    switch (data.Get("DataType"))
                    {
                        case "H":
                            NumOfRecords = NumOfRecords * 6;
                            break;
                        case "D":
                            NumOfRecords = NumOfRecords * 144;
                            break;
                    }
                    dr["NUMBER_OF_RECORDS"] = NumOfRecords;
                    dr["REAL_NUM_RECORDS"] = d.RealUpCount;
                    dr["REPORT_RATE"] = Math.Round(decimal.Parse(d.RealUpCount.ToString())/ decimal.Parse(NumOfRecords.ToString()),2);
                    dataTable.Rows.Add(dr);
                });
            }
            //string sql = @"";
            return dataTable;


            //return SqlModel.Select(sql).Native().ExecToDataTable();
        }

        /// <summary>
        /// 污源物异常
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetExceptionStainSource(RequestData data, DataTable dataTable)
        {
            FieldModel where = null;
            if (data.Get("CompanyID") != "-1")
                where &= T_MID_ALERT.COMPANYID == data.Get("CompanyID");
            else
                where &= T_MID_ALERT.PKID == data.Get("PKID");
            where &= T_MID_ALERT.TYPE == 1;
            List<dynamic> query = SqlModel.SelectAll("A".Field("TITLE").As("CODE_TEXT"), "B".Field("TITLE").As("CODE_TEXT2"), "C".Field("NAME").As("PK_NAME")).From(DB.T_MID_ALERT)
                .LeftJoin(DB.BASDIC.As("A")).On(T_MID_ALERT.ITEMCODE=="A".Field("CODE"))
                .LeftJoin (DB.BASDIC.As("B")).On(T_MID_ALERT.SUBITEMCODE == "B".Field("CODE"))
                .LeftJoin(DB.T_BASE_COMPANY_PK.As("C")).On(T_MID_ALERT.PKID=="C".Field("ID"))
                .Where(where)
                .ExecToDynamicList();
           // List<T_MID_ALERTModel> lis = SerializerHelper.Deserialize<List<T_MID_ALERTModel>>(SerializerHelper.Serialize(query));
            DataRow dr = null;
            query.ForEach(d =>
            {
                dr = dataTable.NewRow();
                dr["PK_NAME"] = d["PK_NAME"];
                dr["STAIN_TEXT"] = d["CODE_TEXT"]==null? d["CODE_TEXT2"]: d["CODE_TEXT"];
                dr["VALUE"] = d["VALUE"] == System.DBNull.Value ? 0 : d["VALUE"]; 
                dr["LIMIT"] = d["LIMIT"];
                dr["STATE"] = d["STATE"]=="0"?"未处理":"处理";
                dr["STARTTIME"] = d["STARTTIME"];
                dr["ENDTIME"] = d["ENDTIME"];
                dataTable.Rows.Add(dr);
            });

            return dataTable;
        }

        /// <summary>
        /// 污染物超标
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetExcessivePollutSource(RequestData data, DataTable dataTable)
        {

            FieldModel where = null;
            if (data.Get("CompanyID") != "-1")
                where &= T_MID_ALERT.COMPANYID == data.Get("CompanyID");
            else
                where &= T_MID_ALERT.PKID == data.Get("PKID");
            where &= T_MID_ALERT.TYPE == 0;
            SqlModel model = SqlModel.SelectAll("A".Field("TITLE").As("CODE_TEXT"), "B".Field("TITLE").As("CODE_TEXT2"), "C".Field("NAME").As("PK_NAME")).From(DB.T_MID_ALERT)
                .LeftJoin(DB.BASDIC.As("A")).On(T_MID_ALERT.ITEMCODE == "A".Field("CODE"))
                .LeftJoin(DB.BASDIC.As("B")).On(T_MID_ALERT.SUBITEMCODE == "B".Field("CODE"))
                .LeftJoin(DB.T_BASE_COMPANY_PK.As("C")).On(T_MID_ALERT.PKID == "C".Field("ID"))
                .Where(where);
                   List<dynamic> query = model.ExecToDynamicList();
            // List<T_MID_ALERTModel> lis = SerializerHelper.Deserialize<List<T_MID_ALERTModel>>(SerializerHelper.Serialize(query));
            DataRow dr = null;
            query.ForEach(d =>
            {
                dr = dataTable.NewRow();
                dr["PK_NAME"] = d["PK_NAME"];
                dr["STAIN_TEXT"] = d["CODE_TEXT"]+"("+ d["CODE_TEXT2"]+")";
                dr["VALUE"] = d["VALUE"]== System.DBNull.Value ? 0: d["VALUE"];
                dr["LIMIT"] = d["LIMIT"];
                dr["STATE"] = d["STATE"] == 0 ? "未处理" : "处理";
                dr["STARTTIME"] = d["STARTTIME"];
                dr["ENDTIME"] = d["ENDTIME"];
                dataTable.Rows.Add(dr);
            });
            return dataTable;
        }
        /// <summary>
        /// 排放流量异常
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetExceptionPFLSource(RequestData data, DataTable dataTable)
        {
            FieldModel where = null;
            if (data.Get("CompanyID") != "-1")
                where &= T_MID_ALERT.COMPANYID == data.Get("CompanyID");
            else
                where &= T_MID_ALERT.PKID == data.Get("PKID");
            where &= T_MID_ALERT.TYPE == 2;
            SqlModel model= SqlModel.SelectAll("A".Field("TITLE").As("CODE_TEXT"),
                "B".Field("TITLE").As("CODE_TEXT2"),
                "C".Field("NAME").As("PK_NAME"),
                "C".Field("TYPE").As("COMPANY_TYPE")
                ).From(DB.T_MID_ALERT)
                .LeftJoin(DB.BASDIC.As("A")).On(T_MID_ALERT.ITEMCODE == "A".Field("CODE"))
                .LeftJoin(DB.BASDIC.As("B")).On(T_MID_ALERT.SUBITEMCODE == "B".Field("CODE"))
                .LeftJoin(DB.T_BASE_COMPANY_PK.As("C")).On(T_MID_ALERT.PKID == "C".Field("ID")|T_MID_ALERT.COMPANYID == "C".Field("COMPANYID"))
                .Where(where);
            List<dynamic> query = model.ExecToDynamicList();
            // List<T_MID_ALERTModel> lis = SerializerHelper.Deserialize<List<T_MID_ALERTModel>>(SerializerHelper.Serialize(query));
           
            string itemTypeName = "废气";
            DataRow dr = null;
            query.ForEach(d =>
            {
                if (d["COMPANY_TYPE"] == "1")
                    //废气
                    itemTypeName = "废气";
                
                else if (d["COMPANY_TYPE"] == "2")
                    //废水
                    itemTypeName = "废水";
                
                else if (d["COMPANY_TYPE"] == "3")
                    //VOCs
                    itemTypeName = "VOCs";
        
                dr = dataTable.NewRow();
                dr["PK_NAME"] = d["PK_NAME"];
                dr["STAIN_TEXT"] = itemTypeName;
                dr["VALUE"] = d["VALUE"] == System.DBNull.Value ? 0 : d["VALUE"];
                dr["LIMIT"] = d["LIMIT"];
                dr["STATE"] = d["STATE"] == 0 ? "未处理" : "处理";
                dr["STARTTIME"] = d["STARTTIME"];
                dr["ENDTIME"] = d["ENDTIME"];
                dataTable.Rows.Add(dr);
            });
            return dataTable;
        }

        /// <summary>
        /// 治理设施停运
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetControlFacilitiesSource(RequestData data, DataTable dataTable)
        {
            FieldModel where = null;
            if (data.Get("CompanyID") != "-1")
                where &= T_MID_ALERT.COMPANYID == data.Get("CompanyID");
            else
                where &= T_MID_ALERT.PKID == data.Get("PKID");
            where &= T_MID_ALERT.TYPE == 3;
            List<dynamic> query = SqlModel.SelectAll("A".Field("TITLE").As("CODE_TEXT"), "B".Field("TITLE").As("CODE_TEXT2"), "C".Field("NAME").As("PK_NAME")).From(DB.T_MID_ALERT)
                .LeftJoin(DB.BASDIC.As("A")).On(T_MID_ALERT.ITEMCODE == "A".Field("CODE"))
                .LeftJoin(DB.BASDIC.As("B")).On(T_MID_ALERT.SUBITEMCODE == "B".Field("CODE"))
                .LeftJoin(DB.T_BASE_COMPANY_PK.As("C")).On(T_MID_ALERT.PKID == "C".Field("ID") | T_MID_ALERT.COMPANYID == "C".Field("COMPANYID"))
                .Where(where)
                .ExecToDynamicList();
            // List<T_MID_ALERTModel> lis = SerializerHelper.Deserialize<List<T_MID_ALERTModel>>(SerializerHelper.Serialize(query));
            DataRow dr = null;
            query.ForEach(d =>
            {
                dr = dataTable.NewRow();
                dr["PK_NAME"] = d["PK_NAME"];
               // dr["STAIN_TEXT"] = d["PK_NAME"] ;
                dr["STATE"] = d["STATE"] == 0 ? "未处理" : "处理";
                dr["STARTTIME"] = d["STARTTIME"];
                dr["ENDTIME"] = d["ENDTIME"];
                dr["LENTIME"] = new TimeSpan(Convert.ToDateTime(d["STARTTIME"]) - Convert.ToDateTime(d["ENDTIME"])).TotalMinutes;
                dataTable.Rows.Add(dr);
            });
            return dataTable;
        }

        /// <summary>
        /// 数采仪掉线
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetMNDownLineSource(RequestData data, DataTable dataTable)
        {
            FieldModel where = null;
            if (data.Get("CompanyID") != "-1")
                where &= T_MID_ALERT.COMPANYID == data.Get("CompanyID");
            else
                where &= T_MID_ALERT.PKID == data.Get("PKID");
            where &= T_MID_ALERT.TYPE == 3;
            SqlModel model = SqlModel.SelectAll("A".Field("TITLE").As("CODE_TEXT"),
                "B".Field("TITLE").As("CODE_TEXT2"),
                "C".Field("NAME").As("PK_NAME"),
                "D".Field("MN").As("TX_NAME")).From(DB.T_MID_ALERT)
                .LeftJoin(DB.BASDIC.As("A")).On(T_MID_ALERT.ITEMCODE == "A".Field("CODE"))
                .LeftJoin(DB.BASDIC.As("B")).On(T_MID_ALERT.SUBITEMCODE == "B".Field("CODE"))
                .LeftJoin(DB.T_BASE_COMPANY_PK.As("C")).On(T_MID_ALERT.PKID == "C".Field("ID") | T_MID_ALERT.COMPANYID == "C".Field("COMPANYID"))
                .LeftJoin(DB.T_BASE_COMPANY_PK_TX.As("D")).On(T_MID_ALERT.PKID == "D".Field("PKID") | T_MID_ALERT.COMPANYID == "D".Field("COMPANYID"))
                .Where(where);
            List<dynamic> query = model.ExecToDynamicList();
            //.LeftJoin(DB..As("C")).On(T_MID_ALERT.TXID == "C".Field("ID"))


            // List<T_MID_ALERTModel> lis = SerializerHelper.Deserialize<List<T_MID_ALERTModel>>(SerializerHelper.Serialize(query));
            DataRow dr = null;
            query.ForEach(d =>
            {
                dr = dataTable.NewRow();
                dr["PK_NAME"] = d["PK_NAME"];
                dr["STAIN_TEXT"] = d["TX_NAME"] ;
                dr["STATE"] = d["STATE"] == 0 ? "未处理" : "处理";
                dr["STARTTIME"] = d["STARTTIME"];
                dr["ENDTIME"] = d["ENDTIME"];
                dr["LENTIME"] = new  TimeSpan( Convert.ToDateTime(d["STARTTIME"]) - Convert.ToDateTime(d["ENDTIME"])).TotalMinutes;
            
                dataTable.Rows.Add(dr);
            });
            return dataTable;
        }


        /// <summary>
        /// 数采仪
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetDataAcquisitionSource(RequestData data, DataTable dataTable)
        {

            DataTable dt = GetDataTypeDataToTable(data);
          

            if (dt != null)
            {
                var query = from a in dt.AsEnumerable() select a;     
                DataRow dr = null;
                List<T_BASE_COMPANY_PK_TXModel> lisMn = SerializerHelper.Deserialize<List<T_BASE_COMPANY_PK_TXModel>>(SerializerHelper.Serialize(GetMNCount(data)));

                lisMn.ForEach(
                    d =>
                    {
                        dr = dataTable.NewRow();
                        dr["MN_NO"] = d.MN;
                        dr["CUR_STATE"] = query.Where(s => s.Field<string>("DEVICECODE") == d.MN).Count()>0?"在线":"掉线";
                        dr["STATE_TIME"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm");
                        dr["WARN_TIMES"] = GetWarnTimes("3");
                        dr["DURATION"] = 0;
                        dataTable.Rows.Add(dr);
                    }
                  );
            }
            //string sql = @"";
            return dataTable;
        }


        /// <summary>
        /// 监测仪器
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetMonitoringSource(RequestData data, DataTable dataTable)
        {
            /*该设备还未有监测接口，暂时使用默认固定值*/

            //DataTable dt = GetDataTypeDataToTable(data);
           
           // if (dt != null)
           // {
               // var query = from a in dt.AsEnumerable() select a;
                DataRow dr = null;
                List<T_BASE_COMPANY_MONITORModel> lisMonitor = SerializerHelper.Deserialize<List<T_BASE_COMPANY_MONITORModel>>(SerializerHelper.Serialize(GetMonitor(data)));
                lisMonitor.ForEach(
                    d =>
                    {
                        dr = dataTable.NewRow();
                        dr["MONITOR_NAME"] = d.NAME;
                        dr["CUR_STATE"] =  "在线" ;
                        dr["STATE_TIME"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm");
                        dr["WARN_TIMES"] = 0;
                        dr["DURATION"] = 0;
                        dataTable.Rows.Add(dr);
                    }
                  );
         //   }
            //string sql = @"";
            return dataTable;
        }
        /// <summary>
        /// 治理设施
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetGovernanceSource(RequestData data, DataTable dataTable)
        {
            /*该设备还未有治理设施接口，暂时使用默认固定值*/

            //DataTable dt = GetDataTypeDataToTable(data);

            // if (dt != null)
            // {
            // var query = from a in dt.AsEnumerable() select a;
            DataRow dr = null;
            List<T_BASE_COMPANY_ZLModel> lisZL = SerializerHelper.Deserialize<List<T_BASE_COMPANY_ZLModel>>(SerializerHelper.Serialize(GetGovernance(data)));
            lisZL.ForEach(
                d =>
                {
                    dr = dataTable.NewRow();
                    dr["MONITOR_NAME"] = d.NAME;
                    dr["CUR_STATE"] = "在线";
                    dr["STATE_TIME"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm");
                    dr["WARN_TIMES"] = GetWarnTimes("4");
                    dr["DURATION"] = 0;
                    dataTable.Rows.Add(dr);
                }
              );
            //   }
            //string sql = @"";
            return dataTable;

            

           
        }
        #endregion

        #region  监测是否有数据来判断设备是否在线
        /// <summary>
        /// 数据在线
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private int IsHaveDataSource(RequestData data)
        {
            string beginTime = data.Get("BeginTime");
            string endTime = data.Get("EndTime");
            string sql = @"select count(1) from T_MID_MINUTE a
                          left join T_BASE_COMPANY_PK_TX b  on a.devicecode = b.mn
                         where  RECTIME>= to_date('{0}','yyyy/MM/dd hh24:mi:ss')  
                         and   RECTIME<=to_date('{1}','yyyy/MM/dd hh24:mi:ss') 
                         and  b.pkid={2} ";
            sql = string.Format(sql,  data.Get("BeginTime"), data.Get("EndTime"), data.Get("PKID"));
            return int.Parse(SqlModel.Select(sql).Native().ExecToDataRow()[0].ToString());
        }

        /// <summary>
        /// 获取排口下的因子
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DataTable GetItemCode(RequestData data,string queryPkId)
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

        /// <summary>
        /// 获取排口下的数采仪
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<dynamic> GetMNCount(RequestData data)
        {
           // string pkId = queryPkId;// data.Get("PKID");
            FieldModel where = null;
            if (data.Get("CompanyID") != "-1")
                where &= T_BASE_COMPANY_PK_TX.COMPANYID == data.Get("CompanyID");
            else
                where &= T_BASE_COMPANY_PK_TX.PKID == data.Get("PKID");
            return   SqlModel.SelectAll()
               .From(DB.T_BASE_COMPANY_PK_TX)
               .Where(where)
               .ExecToDynamicList();
        
        }

        /// <summary>
        /// 获取排口下的监控设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<dynamic> GetMonitor(RequestData data)
        {
            FieldModel where = null;
            if (data.Get("CompanyID") != "-1")
                where &= T_BASE_COMPANY_MONITOR.COMPANYID == data.Get("CompanyID");
            else
                where &= T_BASE_COMPANY_MONITOR.PKID == data.Get("PKID");
            return SqlModel.SelectAll()
               .From(DB.T_BASE_COMPANY_MONITOR)
               .Where(where)
               .ExecToDynamicList();

        }
        /// <summary>
        /// 获取警告次数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private int GetWarnTimes(string type)
        {
            FieldModel where = null;
                where &= T_MID_ALERT.TYPE == type;
          
            return SqlModel.SelectAll()
               .From(DB.T_MID_ALERT)
               .Where(where)
               .ExecToDynamicList().Count;
        }

        /// <summary>
        /// 获取排口下的治理设施
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<dynamic> GetGovernance(RequestData data)
        {
            // string pkId = queryPkId;// data.Get("PKID");
            FieldModel where = null;
            if (data.Get("CompanyID") != "-1")
                where &= T_BASE_COMPANY_ZL.COMPANYID == data.Get("CompanyID");
            else
                where &= T_BASE_COMPANY_ZL.PKID == data.Get("PKID");
            return SqlModel.SelectAll()
               .From(DB.T_BASE_COMPANY_ZL)
               .Where(where)
               .ExecToDynamicList();

        }
        /// <summary>
        /// 获取公用数据源方法
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        private DataTable GetDataTypeDataToTable(RequestData requestData)
        {
            
            DataTable dt =null;
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
            if(requestData.Get("CompanyID")!=null&& requestData.Get("CompanyID") != "-1")
                   sql= sql+ " and  c.COMPANYID="+ requestData.Get("CompanyID");
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
                using (dt=SqlModel.Select(sql).Native().ExecToDataTable())
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
        #endregion 

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
       

       
        private enum DataOutType
        {
            ExportExcel = 1,
            GetDataSource = 2,
            GetPicData=3
        }

        public class TableField
        {
            public string title
            { get; set; }
            public string field
            { get; set; }
        

        }


      


        #endregion
    }


}
