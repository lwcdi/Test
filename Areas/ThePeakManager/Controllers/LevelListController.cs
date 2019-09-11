using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.ThePeakManager.Controllers
{
    public class LevelListController : ListController
    {
        //
        // GET: /ThePeakManager/LevelList/
        protected override SqlModel GetSqlModel(RequestData data)
        {
            FieldModel where = null;

            List<dynamic> filterRuleslist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(data.Get("filterRules"));
            if (filterRuleslist != null)
            {
                foreach (var item in filterRuleslist)
                {
                    this.SetWhere<T_THEPEAK_LEVEL_LIST_INFO>(item, new T_THEPEAK_LEVEL_LIST_INFO(), ref where);
                }

            }
            var sql = SqlModel.SelectAll(
                    BASUSER.TRUENAME
                    , "d1".Field(BASDIC.TITLE).As("PEAK_LEVEL_TEXT")
                    , "d2".Field(BASDIC.TITLE).As("WAR_TYPE_TEXT")
                )
                .From(DB.T_THEPEAK_LEVEL_LIST_INFO)
                .LeftJoin(DB.BASUSER).On(BASUSER.USERNAME == T_THEPEAK_LEVEL_LIST_INFO.ADD_USER)
                .LeftJoin(DB.BASDIC.As("d1")).On("d1".Field(BASDIC.CODE) == T_THEPEAK_LEVEL_LIST_INFO.PEAK_LEVEL & "d1".Field(BASDIC.TYPECODE) == ConstStrings.LevelType)
                .LeftJoin(DB.BASDIC.As("d2")).On("d2".Field(BASDIC.CODE) == T_THEPEAK_LEVEL_LIST_INFO.WAR_TYPE & "d2".Field(BASDIC.TYPECODE) == ConstStrings.EarlyWarning)
                .Where(where)
                ;
            return sql;
        }
        protected override bool DoAdd(RequestData data)
        {
            bool result = true;
            try
            {
                T_THEPEAK_LEVEL_LIST_INFOModel model = new T_THEPEAK_LEVEL_LIST_INFOModel();
                this.GetModelValue(model, data);
                int levelID = model.GetIDByInsert();
                List<T_THEPEAK_LEVEL_CONFIGModel> list = data.Get("extraData").Deserialize<List<T_THEPEAK_LEVEL_CONFIGModel>>();
                list.ForEach(item =>
                {
                    item.PEAK_LEVE_ID = levelID;
                    item.Insert();
                });
                FileUploadHandle.FileMessageSave(data, levelID.ToString());
            }
            catch (Exception ex)
            {
                result = false;
                base.AddLog(null, LogType.InsertOperate, "【新增错峰等级配置】 " + ex.Message + ex.StackTrace);
            }
            return result;
        }

        protected override bool DoDelete(RequestData data)
        {
            bool result = true;
            T_THEPEAK_LEVEL_LIST_INFOModel model = new T_THEPEAK_LEVEL_LIST_INFOModel();
            result = model.Delete(T_THEPEAK_LEVEL_LIST_INFO.ID == data.Get("id"));
            T_THEPEAK_LEVEL_CONFIGModel config = new T_THEPEAK_LEVEL_CONFIGModel();
            if (result)
            {
                config.Delete(T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID == data.Get("id"));
            }
            return result;
        }

        protected override bool DoEdit(RequestData data)
        {
            bool result = true;
            try
            {
                decimal levelID = decimal.Parse(data.Get("id"));
                T_THEPEAK_LEVEL_LIST_INFOModel model = new T_THEPEAK_LEVEL_LIST_INFOModel();
                this.GetModelValue(model, data);
                result = model.Update(T_THEPEAK_LEVEL_LIST_INFO.ID == levelID);

                var configSql = SqlModel.SelectAll().From(DB.T_THEPEAK_LEVEL_CONFIG).Where(T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID == levelID);
                var configList = configSql.ExecToDynamicList();
                List<T_THEPEAK_LEVEL_CONFIGModel> listExit = data.Get("extraData").Deserialize<List<T_THEPEAK_LEVEL_CONFIGModel>>();
                T_THEPEAK_LEVEL_CONFIGModel configModel = new T_THEPEAK_LEVEL_CONFIGModel();
                List<decimal> configDelList = new List<decimal>();
                //信息保存
                if (listExit != null && listExit.Count > 0)
                {
                    foreach (var item in listExit)
                    {
                        bool exit = configList.FindIndex(c => StringHelper.DynamicToString(c["ID"]) == StringHelper.DynamicToString(item.ID)) > -1 ? true : false;
                        if (item.IS_STOP != 1 && item.LIMIT_CONFIG != 1) continue;//没用的配置信息，删除处理
                        //添加
                        if (!exit)
                        {
                            item.PEAK_LEVE_ID = levelID;
                            item.Insert();
                        }
                        //更新
                        else
                        {
                            item.Update(T_THEPEAK_LEVEL_CONFIG.ID == item.ID);
                        }
                        configDelList.Add(item.ID);
                    }
                    //删除子表
                    foreach (var delKey in configList)
                    {
                        if (!configDelList.Contains(decimal.Parse(delKey["ID"].ToString())))
                        {
                            configModel.Delete(T_THEPEAK_LEVEL_CONFIG.ID == decimal.Parse(delKey["ID"].ToString()));
                        }

                    }
                }

                FileUploadHandle.FileMessageSave(data, levelID.ToString());



            }
            catch (Exception ex)
            {
                result = false;
                base.AddLog(null, LogType.InsertOperate, "【编辑错峰等级配置】 " + ex.Message + ex.StackTrace);
            }
            return result;
        }

        public ActionResult GetCompanyList(RequestData data)
        {
            FieldModel where = null;
            List<dynamic> filterRuleslist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(data.Get("filterRules"));
            if (filterRuleslist != null)
            {
                foreach (var item in filterRuleslist)
                {
                    this.SetWhere<T_BASE_COMPANY>(item, new T_BASE_COMPANY(), ref where);
                }
            }
            SqlModel subSql = SqlModel.Select("t".Field("ID"),"t".Field("TYPE").WmConcat().As("POLLUTION_ITEM_TEXT")).From(
                    SqlModel.Select(T_BASE_COMPANY.ID, T_BASE_COMPANY_PK.TYPE).From(DB.T_BASE_COMPANY)
                    .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY.ID == T_BASE_COMPANY_PK.COMPANYID)
                    .GroupBy(T_BASE_COMPANY.ID, T_BASE_COMPANY_PK.TYPE)
                    .As("t")
                )
                .GroupBy("t".Field("ID"));
            //where &= T_BASE_COMPANY.IS_OVERALL_INFO == "1";
            SqlModel sql = SqlModel.SelectAll(
                    T_SYS_AREA.AREA_TEXT.As("AREA_TEXT")
                    , "d1".Field("TITLE").As("INDUSTRY_TYPE_TEXT")
                    , "d2".Field("TITLE").As("POLLUTION_TYPE_TEXT")
                    , "sub1".Field("POLLUTION_ITEM_TEXT")
                    //, "d3".Field(T_BAS_AIR_MONITOR.MONITOR_ITEM_NAME).As("POLLUTION_ITEM_TEXT")
                    )
               .From(DB.T_BASE_COMPANY)
               .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA == T_SYS_AREA.AREA_CODE)
               .LeftJoin(DB.BASDIC.As("d1")).On(T_BASE_COMPANY.BASTYPE == "d1".Field("CODE") & "d1".Field("TYPECODE") == ConstStrings.IndustryType)
               .LeftJoin(DB.BASDIC.As("d2")).On(T_BASE_COMPANY.COMPANYTYPE == "d2".Field("CODE") & "d2".Field("TYPECODE") == ConstStrings.PollutionType)
               .LeftJoin(subSql.As("sub1")).On("sub1".Field("ID")== T_BASE_COMPANY.ID)
               //.LeftJoin(DB.T_BAS_AIR_MONITOR.As("d3")).On(T_COMPANY.POLLUTION_ITEM == "d3".Field(T_BAS_AIR_MONITOR.MONITOR_ITEM_CODE))
               .Where(where)
               .OrderByDesc(T_BASE_COMPANY.ID)
               ;
            ;
            return base.PagedJson(sql, data);
        }
        public ActionResult GetCompanyConfig(RequestData data)
        {
            string levelID = data.Get("PEAK_LEVE_ID");
            //levelID = "2";
            if (string.IsNullOrEmpty(levelID))
            {
                return this.ErrorResult("没有上传等级ID");
            }
            List<dynamic> companyList = new List<dynamic>();
            List<dynamic> dischList = new List<dynamic>();
            List<dynamic> energyList = new List<dynamic>();
            SqlModel companySql = SqlModel.SelectAll().From(DB.T_THEPEAK_LEVEL_CONFIG).Where(T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID == levelID);
            //SqlModel dischSql = SqlModel.SelectAll().From(DB.T_THEPEAK_LEVEL_CONFIG_DISCH).Where(T_THEPEAK_LEVEL_CONFIG_DISCH.PEAK_LEVE_ID == levelID);
            //SqlModel energySql = SqlModel.SelectAll().From(DB.T_THEPEAK_LEVEL_CONFIG_ENERGY).Where(T_THEPEAK_LEVEL_CONFIG_ENERGY.PEAK_LEVE_ID == levelID);
            companySql.ExecToDynamicList().ForEach(item => {
                if("1"== StringHelper.DynamicToString(item["IS_STOP"]))
                {
                    var company = new
                    {
                        ID = StringHelper.DynamicToString(item["ID"]),
                        PEAK_LEVE_ID = StringHelper.DynamicToString(item["PEAK_LEVE_ID"]),
                        COMPANY_ID = StringHelper.DynamicToString(item["COMPANY_ID"]),
                        IS_STOP = StringHelper.DynamicToString(item["IS_STOP"]),

                    };
                    companyList.Add(company);
                }
                else if("1"== StringHelper.DynamicToString(item["LIMIT_CONFIG"]))
                {
                    var company = new
                    {
                        ID = StringHelper.DynamicToString(item["ID"]),
                        PEAK_LEVE_ID = StringHelper.DynamicToString(item["PEAK_LEVE_ID"]),
                        COMPANY_ID = StringHelper.DynamicToString(item["COMPANY_ID"]),
                        LIMIT_TIME_START = StringHelper.DynamicToString(item["LIMIT_TIME_START"]),
                        LIMIT_TIME_END = StringHelper.DynamicToString(item["LIMIT_TIME_END"]),
                        LIMIT_TYPE = StringHelper.DynamicToString(item["LIMIT_TYPE"]),
                        LIMIT_RATIO = StringHelper.DynamicToString(item["LIMIT_RATIO"]),//排污上限
                        LIMIT_CONFIG = StringHelper.DynamicToString(item["LIMIT_CONFIG"]),
                        LIMIT_START = StringHelper.DynamicToString(item["LIMIT_START"]),
                    };
                    companyList.Add(company);
                }
                /*
                var company = new
                {
                    ID = StringHelper.DynamicToString(item["ID"]),
                    PEAK_LEVE_ID = StringHelper.DynamicToString(item["PEAK_LEVE_ID"]),
                    COMPANY_ID = StringHelper.DynamicToString(item["COMPANY_ID"]),
                    IS_STOP = StringHelper.DynamicToString(item["IS_STOP"]),
                    //IS_PATROL = StringHelper.DynamicToString(item["IS_PATROL"]),
                    LIMIT_TIME_START = StringHelper.DynamicToString(item["LIMIT_TIME_START"]),
                    LIMIT_TIME_END = StringHelper.DynamicToString(item["LIMIT_TIME_END"]),
                    LIMIT_TYPE = StringHelper.DynamicToString(item["LIMIT_TYPE"]),
                    LIMIT_RATIO = StringHelper.DynamicToString(item["LIMIT_RATIO"]),//排污上限
                    //ENERGY_RATIO = StringHelper.DynamicToString(item["ENERGY_RATIO"]),//能耗上限
                    LIMIT_CONFIG = StringHelper.DynamicToString(item["LIMIT_CONFIG"]),
                    LIMIT_START = StringHelper.DynamicToString(item["LIMIT_START"]),
                    ENERGY_START = StringHelper.DynamicToString(item["ENERGY_START"]),
                    //LINE_ID = StringHelper.DynamicToString(item["LINE_ID"]),
                    //IS_POWER_OFF = StringHelper.DynamicToString(item["IS_POWER_OFF"]),
                };
                companyList.Add(company);
                */
            });
            return this.SuccessResult("", new
            {
                companyList = companyList,
                dischList = dischList,
                energyList = energyList
            });
        }
        public ActionResult LimitConfig(RequestData data)
        {
            string companyID = data.Get("companyID");
            string peakLevelID = data.Get("peakLevelID");
            //var config = SqlModel.SelectAll().From(DB.T_THEPEAK_LEVEL_CONFIG).Where(T_THEPEAK_LEVEL_CONFIG.ID == peakLevelID & T_THEPEAK_LEVEL_CONFIG.COMPANY_ID == companyID).ExecToDynamic();
            ViewData["companyID"] = companyID;
            //ViewData["config"] = SerializerHelper.Serialize(config);
            return View("LimitConfigSimple");
        }

    }
}
