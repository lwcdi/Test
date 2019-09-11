using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.PlatformIndex.Controllers
{
    public class VOCsViewController : Controller
    {
        //
        // GET: /PlatformIndex/VOCsView/

        public ActionResult VOCsView()
        {
            return View();
        }

        /// <summary>
        /// 1、当天数据复审(修约)情况
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetCompanyXYData(RequestData data)
        {
            DateTime beginTime = DateTime.Now.AddHours( - DateTime.Now.Hour - 1);
            DateTime endTime = DateTime.Now;

            var where = T_BASE_COMPANY.ISDEL == "0" & T_BASE_COMPANY.ISVOCS == "1" & T_MID_HOUR_C.RECTIME >= beginTime & T_MID_HOUR_C.RECTIME <= endTime & T_MID_HOUR_C.ITEMCODE.In("'a25003','a25005','a24088','a25002'");

            var dtXYDatas = SqlModel.Select(T_BASE_COMPANY_PK.NAME.As("NAME"), T_MID_HOUR_C.VALUE.SumAs("VALUE")
                                            , T_MID_CHECKRECORD.RECTIME.MaxAs("maxTime").Decode("null", "未修约", "有修约").As("WARNTYPE"), T_MID_ALERT.ID.MaxAs("maxID").Decode("null", "正常", "有超标").As("STATUS"))
                        .From(DB.T_MID_HOUR_C)
                        .InnerJoin(DB.T_BASE_COMPANY_PK_TX)
                        .On(T_BASE_COMPANY_PK_TX.MN == T_MID_HOUR_C.DEVICECODE)
                        .InnerJoin(DB.T_BASE_COMPANY_PK)
                        .On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                        .InnerJoin(DB.T_BASE_COMPANY)
                        .On(T_BASE_COMPANY.ID == T_BASE_COMPANY_PK_TX.COMPANYID)
                        .LeftJoin(DB.T_MID_CHECKRECORD)
                        .On(T_MID_CHECKRECORD.MN == T_MID_HOUR_C.DEVICECODE)
                        .LeftJoin(DB.T_MID_ALERT)
                        .On(T_MID_ALERT.ITEMCODE == T_MID_HOUR_C.ITEMCODE & T_MID_ALERT.TXID == T_BASE_COMPANY_PK_TX.ID)
                        .Where(where)
                        .GroupBy(T_BASE_COMPANY_PK.NAME)
                        .ExecToDataTable();
            return dtXYDatas.ToJson();
        }

        /// <summary>
        /// 4、重点关注企业排放情况
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetZDGZCompanyMonitorData(RequestData data)
        {
            var whereCompany = T_BASE_COMPANY.ISDEL == "0" & T_BASE_COMPANY.ISVOCS == "1" & T_BASE_COMPANY.GZCD.In("'1', '2'");
            var lstID = GetCompanyIDs(whereCompany);
            string strIDs = string.Join(",", lstID);
            //GasItem:废气、WaterItem:废水、VOC:VOCsItem，多种类型可以使用逗号分隔传入 HX w01018_PFL_Value AD w21003_PFL_Value 
            //var lstMonitorData = AnalysisData2.GetAreaMonitorData("", DateTime.Now.AddHours(-DateTime.Now.Hour - 1), DateTime.Now, "hour", "VOCsItem");
            var dtMonitorData = MonitorData.GetMonitorData("", DateTime.Now.AddHours(-DateTime.Now.Hour - 1), DateTime.Now, "C", strIDs, "hour", "VOCsItem");

            Dictionary<string, List<string>> dicReturn = new Dictionary<string, List<string>>();
            List<string> lstCompany = new List<string>();
            List<string> lstZJ = new List<string>();
            List<string> lstBen = new List<string>();
            List<string> lstJBen = new List<string>();
            List<string> lstJBen2 = new List<string>();

            for (int i = 0; i < lstID.Count; i++)
            {
                string name = "";
                decimal ZJ = 0;
                decimal Ben = 0;
                decimal JBen = 0;
                decimal JBen2 = 0;

                dtMonitorData.Select(string.Format("ID = '{0}'", lstID[i])).Each(dataRow => {
                    name = dataRow["NAME"].ToString().Trim();
                    decimal tmpZJ = 0;
                    decimal tmpBen = 0;
                    decimal tmpJBen = 0;
                    decimal tmpJBen2 = 0;
                    decimal.TryParse(dataRow["a24088_PFL_Value"].ToString().Trim(), out tmpZJ);
                    decimal.TryParse(dataRow["a25002_PFL_Value"].ToString().Trim(), out tmpBen);
                    decimal.TryParse(dataRow["a25003_PFL_Value"].ToString().Trim(), out tmpJBen);
                    decimal.TryParse(dataRow["a25005_PFL_Value"].ToString().Trim(), out tmpJBen2);
                    ZJ += tmpZJ;
                    Ben += tmpBen;
                    JBen += tmpJBen;
                    JBen2 += tmpJBen2;
                });
                if (!string.IsNullOrEmpty(name))
                {
                    lstCompany.Add(name);
                    lstZJ.Add(ZJ.ToString());
                    lstBen.Add(Ben.ToString());
                    lstJBen.Add(JBen.ToString());
                    lstJBen2.Add(JBen2.ToString()); 
                }
            }
            dicReturn.Add("area", lstCompany);
            dicReturn.Add("ZJ", lstZJ);
            dicReturn.Add("Ben", lstBen);
            dicReturn.Add("JBen", lstJBen);
            dicReturn.Add("JBen2", lstJBen2);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(dicReturn);
        }

        /// <summary>
        /// 5、区域总量分布情况
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetAreaMonitorData(RequestData data)
        {
            var whereCompany = T_BASE_COMPANY.ISDEL == "0" & T_BASE_COMPANY.ISVOCS == "1";
            var lstID = GetCompanyIDs(whereCompany);
            string strIDs = string.Join(",", lstID);
            //GasItem:废气、WaterItem:废水、VOC:VOCsItem，多种类型可以使用逗号分隔传入 HX w01018_PFL_Value AD w21003_PFL_Value 
            //var lstMonitorData = AnalysisData2.GetAreaMonitorData("", DateTime.Now.AddHours(-DateTime.Now.Hour - 1), DateTime.Now, "hour", "VOCsItem");
            var dtMonitorData = MonitorData.GetMonitorData("", DateTime.Now.AddHours(-DateTime.Now.Hour - 1), DateTime.Now, "C", strIDs, "hour", "VOCsItem");
            Dictionary<string, List<string>> dicReturn = new Dictionary<string, List<string>>();
            List<string> lstTime = new List<string>();
            List<string> lstClock = new List<string>();
            List<string> lstZJ = new List<string>();
            //List<string> lstBen = new List<string>();
            //List<string> lstJBen = new List<string>();
            //List<string> lstJBen2 = new List<string>();
            if (dtMonitorData != null && dtMonitorData.Rows.Count > 0)
            {
                for (int i = 0; i < dtMonitorData.Rows.Count; i++)
                {
                    string data_time = dtMonitorData.Rows[i]["DATA_TIME"].ToString().Trim();
                    if (!lstTime.Contains(data_time))
                    {
                        DateTime tmpDT = DateTime.Now;
                        DateTime.TryParse(data_time, out tmpDT);
                        lstClock.Add(string.Format("{0}点", tmpDT.Hour));
                        lstTime.Add(data_time); 
                    }
                }
            }
            for (int i = 0; i < lstTime.Count; i++)
            {
                decimal ZJ = 0;
                //decimal Ben = 0;
                //decimal JBen = 0;
                //decimal JBen2 = 0;
                dtMonitorData.Select(string.Format("DATA_TIME = '{0}'", lstTime[i])).Each(dataRow => {
                    decimal tmpZJ = 0;
                    decimal tmpBen = 0;
                    decimal tmpJBen = 0;
                    decimal tmpJBen2 = 0;
                    decimal.TryParse(dataRow["a24088_PFL_Value"].ToString().Trim(), out tmpZJ);
                    decimal.TryParse(dataRow["a25002_PFL_Value"].ToString().Trim(), out tmpBen);
                    decimal.TryParse(dataRow["a25003_PFL_Value"].ToString().Trim(), out tmpJBen);
                    decimal.TryParse(dataRow["a25005_PFL_Value"].ToString().Trim(), out tmpJBen2);
                    ZJ += tmpZJ;
                    ZJ += tmpBen;
                    ZJ += tmpJBen;
                    ZJ += tmpJBen2;
                });
                lstZJ.Add(ZJ.ToString());
                //lstBen.Add(Ben.ToString());
                //lstJBen.Add(JBen.ToString());
                //lstJBen2.Add(JBen2.ToString());
            }
            dicReturn.Add("time", lstClock);
            dicReturn.Add("data", lstZJ);
            //dicReturn.Add("Ben", lstBen);
            //dicReturn.Add("JBen", lstJBen);
            //dicReturn.Add("JBen2", lstJBen2);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(dicReturn);
        }
        private List<decimal> GetCompanyIDs(FieldModel where)
        {
            var dtCompany = SqlModel.Select(T_BASE_COMPANY.ID)
                    .From(DB.T_BASE_COMPANY)
                    .Where(where)
                    .ExecToDataTable();
            var lstID = dtCompany.AsEnumerable().Select(c => c.Field<decimal>("ID")).ToList();
            return lstID;
        }
    }
}
