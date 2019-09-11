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
    public class WRYViewController : Controller
    {
        //
        // GET: /PlatformIndex/WRYView/

        public ActionResult WRYView()
        {
            return View(0);
        }

        /// <summary>
        /// 1、管控点位分布情况
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetCompanyInfoByArea()
        {
            var where = T_BASE_COMPANY.ISDEL == "0" & T_BASE_COMPANY.ISVOCS == "0" & T_BASE_COMPANY.GZCD.In("'1','2','3','4'");
            //按区域、关注程度统计企业数量
            var dt = SqlModel.Select(T_BASE_COMPANY.GZCD, BASDIC.TITLE, T_BASE_COMPANY.AREA, T_SYS_AREA.AREA_TEXT, T_BASE_COMPANY.ID.CountAs("Total"))
                    .From(DB.T_BASE_COMPANY)
                    .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA == T_SYS_AREA.AREA_CODE)
                    .LeftJoin(DB.BASDIC).On(T_BASE_COMPANY.GZCD == BASDIC.CODE & BASDIC.TYPECODE == "CompanyGZCD")
                    .Where(where)
                    .GroupBy(T_BASE_COMPANY.GZCD, BASDIC.TITLE, T_BASE_COMPANY.AREA, T_SYS_AREA.AREA_TEXT)
                    .ExecToDataTable();
            Dictionary<string, List<string>> dicReturn = new Dictionary<string, List<string>>();
            List<string> lstArea = new List<string>();
            List<string> lstGuo = new List<string>();
            List<string> lstSheng = new List<string>();
            List<string> lstShi = new List<string>();
            List<string> lstQu = new List<string>();
            //获取所有区域名称
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string areaName = dt.Rows[i]["AREA_TEXT"].ToString().Trim();
                    if (!lstArea.Contains(areaName))
                    {
                        lstArea.Add(areaName);
                    }
                }
            }
            //按区域名称获取区域内各个关注程度的企业数量
            if (lstArea.Count > 0)
            {
                for (int i = 0; i < lstArea.Count; i++)
                {
                    dt.Select(string.Format("AREA_TEXT = '{0}'", lstArea[i])).Each(dataRow =>
                    {
                        string total = dataRow["TOTAL"].ToString().Trim();
                        switch (dataRow["GZCD"].ToString().Trim())
                        {
                            case "1":
                                lstGuo.Add(total);
                                break;
                            case "2":
                                lstSheng.Add(total);
                                break;
                            case "3":
                                lstShi.Add(total);
                                break;
                            case "4":
                                lstQu.Add(total);
                                break;
                            default:
                                break;
                        }
                    });
                    //没有企业数量的关注程度补0
                    int tmpCoun = i + 1;
                    if (lstGuo.Count < tmpCoun)
                    {
                        lstGuo.Add("0");
                    }
                    if (lstSheng.Count < tmpCoun)
                    {
                        lstSheng.Add("0");
                    }
                    if (lstShi.Count < tmpCoun)
                    {
                        lstShi.Add("0");
                    }
                    if (lstQu.Count < tmpCoun)
                    {
                        lstQu.Add("0");
                    }
                }
            }
            dicReturn.Add("area", lstArea);
            dicReturn.Add("Guo", lstGuo);
            dicReturn.Add("Sheng", lstSheng);
            dicReturn.Add("Shi", lstShi);
            dicReturn.Add("Qu", lstQu);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(dicReturn);
        }

        /// <summary>
        /// 2、实时点位在线情况
        /// </summary>
        /// <returns></returns>
        public string GetCompanyNetStatusInfo(RequestData data)
        {
            //返回4个数据：'在线', '离线', '无数据', '其他'
            //暂时先查询总数、有数据数量，总数-有数据=无数据，其余默认为0

            string IsVOCs = data.Get("IsVOCs");
            var whereTotal = T_BASE_COMPANY.ISDEL == "0" & T_BASE_COMPANY.ISVOCS == IsVOCs;
            var dtTotal = SqlModel.Select(T_BASE_COMPANY.ID.CountAs("Total"))
                            .From(DB.T_BASE_COMPANY)
                            .Where(whereTotal)
                            .ExecToDataTable();

            int offline = 10;
            object obj = SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == "OfflineWarn").ExecuteScalar();
            try
            {
                offline = obj.ToInt32();
            }
            catch (Exception)
            {
            }
            var whereHasData = T_MID_MINUTE.RECTIME >= DateTime.Now.AddMinutes(-offline) & T_MID_MINUTE.RECTIME <= DateTime.Now & T_BASE_COMPANY.ISVOCS == IsVOCs;
            var dtHasData = SqlModel.Select(T_BASE_COMPANY.ID)
                            .From(DB.T_MID_MINUTE)
                            .InnerJoin(DB.T_BASE_COMPANY_PK_TX)
                            .On(T_BASE_COMPANY_PK_TX.MN == T_MID_MINUTE.DEVICECODE)
                            .InnerJoin(DB.T_BASE_COMPANY)
                            .On(T_BASE_COMPANY.ID == T_BASE_COMPANY_PK_TX.COMPANYID)
                            .Where(whereHasData)
                            .GroupBy(T_BASE_COMPANY.ID)
                            .ExecToDataTable();
            Dictionary<string, List<string>> dicReturn = new Dictionary<string, List<string>>();
            List<string> lstData = new List<string>();
            int tmpTotal = 0;
            int tmpHasData = 0;
            if (dtTotal != null && dtTotal.Rows.Count > 0)
            {
                int.TryParse(dtTotal.Rows[0]["TOTAL"].ToString().Trim(), out tmpTotal);
            }
            if (dtHasData != null && dtHasData.Rows.Count > 0)
            {
                tmpHasData = dtHasData.Rows.Count;
            }
            lstData.Add(tmpHasData.ToString());  //在线 = 有数据
            lstData.Add((tmpTotal - tmpHasData).ToString());  //离线 = 总数 - 有数据
            lstData.Add("0"); //无数据 默认0
            lstData.Add("0"); //其他 默认0
            dicReturn.Add("data", lstData);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(dicReturn);
        }

        /// <summary>
        /// 3、停运备案情况
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetCompanyStopInfoByArea()
        {
            var where = T_BASE_COMPANY.ISDEL == "0" & T_BASE_COMPANY.ISVOCS == "0" & T_BASE_COMPANY.GZCD.In("'1','2','3','4'");
            //按区域、运行状态统计企业数量
            var dt = SqlModel.Select(T_BASE_COMPANY.RUNTYPE, T_BASE_COMPANY.GZCD, T_BASE_COMPANY.ID.CountAs("Total"))
                    .From(DB.T_BASE_COMPANY)
                    .Where(where)
                    .GroupBy(T_BASE_COMPANY.RUNTYPE, T_BASE_COMPANY.GZCD)
                    .ExecToDataTable();
            Dictionary<string, List<string>> dicReturn = new Dictionary<string, List<string>>();
            List<string> lstArea = new List<string>() { "1", "0"};
            List<string> lstGuo = new List<string>();
            List<string> lstSheng = new List<string>();
            List<string> lstShi = new List<string>();
            List<string> lstQu = new List<string>();
            //按区域名称获取区域内各个关注程度的企业数量
            if (lstArea.Count > 0)
            {
                for (int i = 0; i < lstArea.Count; i++)
                {
                    dt.Select(string.Format("RUNTYPE = '{0}'", lstArea[i])).Each(dataRow =>
                    {
                        string total = dataRow["TOTAL"].ToString().Trim();
                        switch (dataRow["GZCD"].ToString().Trim())
                        {
                            case "1":
                                lstGuo.Add(total);
                                break;
                            case "2":
                                lstSheng.Add(total);
                                break;
                            case "3":
                                lstShi.Add(total);
                                break;
                            case "4":
                                lstQu.Add(total);
                                break;
                            default:
                                break;
                        }
                    });
                    //没有企业数量的关注程度补0
                    int tmpCoun = i + 1;
                    if (lstGuo.Count < tmpCoun)
                    {
                        lstGuo.Add("0");
                    }
                    if (lstSheng.Count < tmpCoun)
                    {
                        lstSheng.Add("0");
                    }
                    if (lstShi.Count < tmpCoun)
                    {
                        lstShi.Add("0");
                    }
                    if (lstQu.Count < tmpCoun)
                    {
                        lstQu.Add("0");
                    }
                }
            }
            dicReturn.Add("Guo", lstGuo);
            dicReturn.Add("Sheng", lstSheng);
            dicReturn.Add("Shi", lstShi);
            dicReturn.Add("Qu", lstQu);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(dicReturn);
        }

        /// <summary>
        /// 4、废气总量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetCompanyFQMointorData()
        {
            //GasItem:废气、WaterItem:废水、VOC:VOCsItem，多种类型可以使用逗号分隔传入 SO2 a21026_PFL_Value HO a21002_PFL_Value 烟尘 a34013_PFL_Value
            var lstMonitorData = AnalysisData2.GetAreaMonitorData(GetAreaCodeByCity(), DateTime.Now.AddHours(-DateTime.Now.Hour - 1), DateTime.Now, "hour", "GasItem");
            Dictionary<string, List<string>> dicReturn = new Dictionary<string, List<string>>();
            List<string> lstArea = new List<string>();
            List<string> lstSO2 = new List<string>();
            List<string> lstHO = new List<string>();
            List<string> lstPM = new List<string>();
            foreach (var item in lstMonitorData)
            {
                lstArea.Add(item["AREA_NAME"].ToString());
                lstSO2.Add(item["a21026_PFL_Value"].ToString());
                lstHO.Add(item["a21002_PFL_Value"].ToString());
                lstPM.Add(item["a34013_PFL_Value"].ToString());
            }
            dicReturn.Add("area", lstArea);
            dicReturn.Add("SO2", lstSO2);
            dicReturn.Add("HO", lstHO);
            dicReturn.Add("PM", lstPM);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(dicReturn);
        }

        /// <summary>
        /// 5、废水总量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetCompanyFSMointorData()
        {
            //GasItem:废气、WaterItem:废水、VOC:VOCsItem，多种类型可以使用逗号分隔传入 HX w01018_PFL_Value AD w21003_PFL_Value 
            var lstMonitorData = AnalysisData2.GetAreaMonitorData(GetAreaCodeByCity(), DateTime.Now.AddHours(-DateTime.Now.Hour - 1), DateTime.Now, "hour", "WaterItem");

            Dictionary<string, List<string>> dicReturn = new Dictionary<string, List<string>>();
            List<string> lstArea = new List<string>();
            List<string> lstHX = new List<string>();
            List<string> lstAD = new List<string>();
            foreach (var item in lstMonitorData)
            {
                lstArea.Add(item["AREA_NAME"].ToString());
                lstHX.Add(item["w01018_PFL_Value"].ToString());
                lstAD.Add(item["w21003_PFL_Value"].ToString());
            }
            dicReturn.Add("area", lstArea);
            dicReturn.Add("HX", lstHX);
            dicReturn.Add("AD", lstAD);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(dicReturn);
        }

        /// <summary>
        /// 6、近三天企业告警信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string GetCompanyWarnData(RequestData data)
        {
            string IsVOCs = data.Get("IsVOCs");
            DateTime beginTime = DateTime.Now.AddDays(-3);
            DateTime endTime = DateTime.Now;
            var where = T_BASE_COMPANY.ISVOCS == IsVOCs & T_MID_ALERT.STARTTIME >= beginTime & T_MID_ALERT.STARTTIME <= endTime;
            var dtWarn = SqlModel.Select(T_BASE_COMPANY.NAME.As("NAME"), T_MID_ALERT.STARTTIME.ToCharDateSelect("yyyy-MM-dd").As("TIME"), "WARNTYPE".Field("TITLE").As("WARNTYPE"), T_MID_ALERT.CONTENT.As("CONTENT"), "STATE".Field("TITLE").As("STATUS"))
                        .From(DB.T_MID_ALERT)
                        .InnerJoin(DB.T_BASE_COMPANY)
                        .On(T_MID_ALERT.COMPANYID == T_BASE_COMPANY.ID)
                        .LeftJoin(DB.BASDIC.As("STATE"))
                        .On("STATE".Field("CODE") == T_MID_ALERT.STATE & "STATE".Field("TYPECODE") == "WarningState")
                        .LeftJoin(DB.BASDIC.As("WARNTYPE"))
                        .On("WARNTYPE".Field("CODE") == T_MID_ALERT.STATE & "WARNTYPE".Field("TYPECODE") == "Warn_Type")
                        .Where(where)
                        .OrderByDesc(T_MID_ALERT.STARTTIME)
                        .ExecToDataTable();
            return dtWarn.ToJson();
        }

        /// <summary>
        /// 获取区域编号字符串
        /// </summary>
        /// <returns></returns>
        private string GetAreaCodeByCity()
        {
            FieldModel whereArea = T_SYS_AREA.PARENT_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode));
            var dtArea = SqlModel.Select(T_SYS_AREA.AREA_CODE.As("CODE"))
                        .From(DB.T_SYS_AREA)
                        .Where(whereArea)
                        .OrderByAsc(T_SYS_AREA.ID)
                        .ExecToDataTable();
            var list = dtArea.AsEnumerable().Select(c => c.Field<string>("CODE")).ToList();
            return string.Join(",", list);
        }
    }
}
