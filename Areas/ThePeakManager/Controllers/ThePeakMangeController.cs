using w.ORM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;

/* toDO: 查看详情列表动作， 回填行业类型 ， 限制区域全部勾选 
 * */
namespace UI.Web.Areas.ThePeakManager.Controllers
{
    public class ThePeakMangeController : ListController
    {
        protected override SqlModel GetSqlModel(RequestData data)
        {
            FieldModel where = null;
            string peakType = data.Get("PEAK_TYPE").Trim();
            string peakTheme = data.Get("PEAK_THEME").Trim();
            string downTimeStart = data.Get("DOWN_TIME_STRAT").Trim();
            string downTimeEnd = data.Get("DOWN_TIME_END").Trim();

            if (!string.IsNullOrEmpty(peakType))
            {
                where &= T_THEPEAK_MAIN_LIST_INFO.PEAK_TYPE == peakType;
            }
            if (!string.IsNullOrEmpty(peakTheme))
            {
                where &= T_THEPEAK_MAIN_LIST_INFO.PEAK_THEME.Like(peakTheme);
            }
            if (!string.IsNullOrEmpty(downTimeStart))
            {
                where &= T_THEPEAK_MAIN_LIST_INFO.DOWN_TIME >= DateTime.Parse(downTimeStart);
            }
            if (!string.IsNullOrEmpty(downTimeEnd))
            {
                where &= T_THEPEAK_MAIN_LIST_INFO.DOWN_TIME <= DateTime.Parse(downTimeEnd);
            }
            where &= T_THEPEAK_MAIN_LIST_INFO.PLAN_TYPE==0;
            where &= T_THEPEAK_MAIN_LIST_INFO.IS_CLOSE == 0;
            var sql = SqlModel.SelectAll(
                    "d1".Field(BASDIC.TITLE).As("PEAK_LEVEL_TEXT")
                    , "d2".Field(BASDIC.TITLE).As("WAR_TYPE_TEXT")
                    , "d3".Field(BASDIC.TITLE).As("PEAK_TYPE_TEXT")
                    , T_THEPEAK_LEVEL_LIST_INFO.ID.As("LEVEL_ID")

                ).From(DB.T_THEPEAK_MAIN_LIST_INFO)
                .LeftJoin(DB.T_THEPEAK_LEVEL_LIST_INFO).On(T_THEPEAK_LEVEL_LIST_INFO.PEAK_LEVEL == T_THEPEAK_MAIN_LIST_INFO.PEAK_LEVEL)
                .LeftJoin(DB.BASDIC.As("d1")).On("d1".Field(BASDIC.CODE) == T_THEPEAK_MAIN_LIST_INFO.PEAK_LEVEL & "d1".Field(BASDIC.TYPECODE) == ConstStrings.LevelType)
                .LeftJoin(DB.BASDIC.As("d2")).On("d2".Field(BASDIC.CODE) == T_THEPEAK_MAIN_LIST_INFO.WAR_TYPE & "d2".Field(BASDIC.TYPECODE) == ConstStrings.EarlyWarning)
                .LeftJoin(DB.BASDIC.As("d3")).On("d3".Field(BASDIC.CODE) == T_THEPEAK_MAIN_LIST_INFO.PEAK_TYPE & "d3".Field(BASDIC.TYPECODE) == ConstStrings.EmergencyType)
                .Where(where)

                ;
            return sql;
        }

        protected override bool DoAdd(RequestData data)
        {
            return true;
        }

        [HttpPost]
        public ActionResult SaveThePeak(RequestData data)
        {
            PostResult result = new PostResult();
            List<dynamic> getXunChalis = null;
            string entGuid = data.Get("entGuid");
            try
            {

               
                T_THEPEAK_MAIN_LIST_INFOModel model = new T_THEPEAK_MAIN_LIST_INFOModel();
                model = this.GetModelValue<T_THEPEAK_MAIN_LIST_INFOModel>(model, data);
                model.PEAK_TYPE = "zfyj";
                model.RESPONSE_AREA = model.LIMIT_AREA;
                //model.LIMITED_NUM = model.LIMITED_NUM ;
                model.ALL_NUM = model.LIMITED_NUM + model.STOP_NUM+model.THEPEAK_NUM;
                //主表
                int peakId = model.GetIDByInsert();
                FileUploadHandle.FileMessageSave(data, peakId.ToString());
                //更新主表
                T_THEPEAK_ENT_SUB_LISTModel entSub = new T_THEPEAK_ENT_SUB_LISTModel();
                entSub.THEPEAKID = peakId;
                entSub.Update(T_THEPEAK_ENT_SUB_LIST.GUID == entGuid);
                //获取需要巡查的企业
            
                getXunChalis = GetXunCha(peakId);
                result.Data = getXunChalis;
                result.Message = peakId.ToString();
                result.Success = true;
                //UpdateAttach(Json,getID)
                //添加主表日志
                base.AddLog(null, LogType.InsertOperate, "【任务下发成功】 " + ModelToString(model.ID));
            }
            catch (Exception ex)
            {
                base.AddLog(null, LogType.InsertOperate, "【下发错峰任务失败】 " + ex.Message + ex.StackTrace);
                Lang.add_failure = "下发错峰任务失败";
                result.Success=false;
            }
            //return result;
            return Json(result);
        }

        /// <summary>
        /// 获取需要巡查的企业(外用)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetXunCha(RequestData data)
        {
            PostResult result = new PostResult();
            string peakId = data.Get("peakId");
            if (!string.IsNullOrEmpty(peakId))
            {
                result.Data = GetXunCha(int.Parse(peakId)) ;
                result.Success = true;
                
            }
            else
                result.Success = false;
            return Json(result);

        }
        /// <summary>
        /// 获取需要巡查的企业
        /// </summary>
        /// <returns></returns>
        private dynamic  GetXunCha(int peakId)
        {
            FieldModel where = null;
            where &= T_THEPEAK_ENT_SUB_LIST.THEPEAKID == peakId;
            where &= T_THEPEAK_ENT_SUB_LIST.IS_PATROL == 1;
            SqlModel sql = SqlModel.SelectAll(
               T_SYS_AREA.AREA_TEXT.As("AREA_TEXT")
               , "d1".Field("TITLE").As("INDUSTRY_TYPE_TEXT")
              // , "d2".Field("TITLE").As("POLLUTION_TYPE_TEXT")
              // , "d3".Field(T_BAS_AIR_MONITOR.MONITOR_ITEM_NAME).As("POLLUTION_ITEM_TEXT")
             //  , "d4".Field("TITLE").As("COMPANY_STATE_TEXT")
               , T_THEPEAK_ENT_SUB_LIST.ID.As("ENTSUBID")
               , T_THEPEAK_LEVEL_LIST_INFO.ID.As("LEVEL_ID")
               , T_THEPEAK_ENT_SUB_LIST.ENT_PEAK_TYPE
               , T_THEPEAK_ENT_SUB_LIST.THEPEAKID
               , T_THEPEAK_ENT_SUB_LIST.IS_SELECT
               , T_THEPEAK_MAIN_LIST_INFO.PEAK_THEME
               ,T_THEPEAK_MAIN_LIST_INFO.START_TIME
               ,T_THEPEAK_MAIN_LIST_INFO.END_TIME
               ,T_THEPEAK_MAIN_LIST_INFO.PEAK_DESC
               , T_BASE_COMPANY.AREA
               , T_BASE_COMPANY.ID.As("COMPANY_ID")
               , "d6".Field("TITLE").As("PEAK_LEVEL_TEXT")
               //, "d7".Field(BASUSER.TRUENAME).As("TOWN_SUPERVISOR_TEXT")
              // , "d8".Field(BASUSER.TRUENAME).As("INDUSTRY_SUPERVISOR_TEXT")
               , "d9".Field(BASUSER.TRUENAME).As("SUPERVISOR_TEXT")

               )
          .From(DB.T_BASE_COMPANY)
          .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA == T_SYS_AREA.AREA_CODE)
          .LeftJoin(DB.BASDIC.As("d1")).On(T_BASE_COMPANY.BASTYPE == "d1".Field("CODE") & "d1".Field("TYPECODE") == ConstStrings.IndustryType)
         // .LeftJoin(DB.BASDIC.As("d2")).On(T_BASE_COMPANY.typ.POLLUTION_TYPE == "d2".Field("CODE") & "d2".Field("TYPECODE") == ConstStrings.PollutionType)
          //.LeftJoin(DB.T_BAS_AIR_MONITOR.As("d3")).On(T_BASE_COMPANY.POLLUTION_ITEM == "d3".Field(T_BAS_AIR_MONITOR.MONITOR_ITEM_CODE))
          //.LeftJoin(DB.BASDIC.As("d4")).On(T_BASE_COMPANY.COMPANY_STATE == "d4".Field("CODE") & "d4".Field("TYPECODE") == ConstStrings.Company_State)
          //.LeftJoin(DB.BASDIC.As("d5")).On(T_BASE_COMPANY.FUEL_TYPE == "d5".Field("CODE") & "d5".Field("TYPECODE") == ConstStrings.FuelType)
          .LeftJoin(DB.T_THEPEAK_LEVEL_CONFIG).On(T_THEPEAK_LEVEL_CONFIG.COMPANY_ID == T_BASE_COMPANY.ID)
          .LeftJoin(DB.T_THEPEAK_LEVEL_LIST_INFO).On(T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID)
          .LeftJoin(DB.T_THEPEAK_ENT_SUB_LIST).On(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == T_BASE_COMPANY.ID & T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_ENT_SUB_LIST.LEVEL_ID)//& T_THEPEAK_ENT_SUB_LIST.GUID == entGuid
          .LeftJoin(DB.T_THEPEAK_MAIN_LIST_INFO).On(T_THEPEAK_MAIN_LIST_INFO.ID == T_THEPEAK_ENT_SUB_LIST.THEPEAKID)
          .LeftJoin(DB.BASDIC.As("d6")).On(T_THEPEAK_LEVEL_LIST_INFO.PEAK_LEVEL == "d6".Field("CODE") & "d6".Field("TYPECODE") == ConstStrings.LevelType & T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_ENT_SUB_LIST.LEVEL_ID)
         // .LeftJoin(DB.BASUSER.As("d7")).On(T_BASE_COMPANY.SUPERVISOR == "d7".Field(BASUSER.USERNAME))
         // .LeftJoin(DB.BASUSER.As("d8")).On(T_BASE_COMPANY.INDUSTRY_SUPERVISOR == "d8".Field(BASUSER.USERNAME))
          .LeftJoin(DB.BASUSER.As("d9")).On(T_BASE_COMPANY.SUPERVISOR == "d9".Field(BASUSER.USERNAME))
          .Where(where);
            return sql.ExecToDynamicList();
             
        }

        protected override bool DoDelete(RequestData data)
        {
            T_THEPEAK_MAIN_LIST_INFOModel model = new T_THEPEAK_MAIN_LIST_INFOModel();
            return model.Delete(T_THEPEAK_MAIN_LIST_INFO.ID == data.Get("id"));
        }
      
        protected override bool DoEdit(RequestData data)
        {
            bool reulst = true;
            string entGuid = data.Get("entGuid");
            try
            {

                T_THEPEAK_MAIN_LIST_INFOModel model = new T_THEPEAK_MAIN_LIST_INFOModel();
                model = this.GetModelValue<T_THEPEAK_MAIN_LIST_INFOModel>(model, data);
               
                reulst = model.Update(T_THEPEAK_MAIN_LIST_INFO.ID == data.Get("id"));
                //更新主表
                T_THEPEAK_ENT_SUB_LISTModel entSub = new T_THEPEAK_ENT_SUB_LISTModel();
                entSub.THEPEAKID = decimal.Parse( data.Get("id"));
                entSub.Update(T_THEPEAK_ENT_SUB_LIST.GUID == entGuid);
            }
            catch (Exception ex)
            {
                base.AddLog(null, LogType.InsertOperate, "【编辑错峰任务失败】 " + ex.Message + ex.StackTrace);
                Lang.add_failure = "编辑错峰任务失败";
                reulst = false;
            }
            return reulst;
        }

        [HttpPost]
        public ActionResult GetUserRole(int userId)
        {
            List<dynamic> list = SqlModel.Select(BASROLE.ID, BASROLE.ROLENAME, BASROLE.REMARK)
                .From(DB.BASUSERROLE)
                .LeftJoin(DB.BASROLE).On(BASROLE.ID == BASUSERROLE.ROLEID)
                .Where(BASUSERROLE.USERID == userId)
                .ExecToDynamicList();
            return Json(list);
        }

       
       


     
        public ActionResult IssuedLevelEdit()
        {
            return View();
        }
        public ActionResult ChangeLevel()
        {
            return View();
        }

        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCompanyData(RequestData data)
        {
            string levelId = data.Get("LevelId");
            string entGuid = data.Get("entGuid");
            string actionType = data.Get("ActionType");

            FieldModel where = null;
            where &= T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID == levelId;
            if ("Stop"==actionType)
            {
                where &= T_THEPEAK_LEVEL_CONFIG.IS_STOP == 1;
            }
            if ("Limit"==actionType)
            {
                where &= T_THEPEAK_LEVEL_CONFIG.LIMIT_TYPE == "TheLimit";
            }
            if ("TimePeak"== actionType)
            {
                where &= T_THEPEAK_LEVEL_CONFIG.LIMIT_TYPE == "limitTime";
            }
            where &= T_THEPEAK_ENT_SUB_LIST.GUID == entGuid;

            List<dynamic> filterRuleslist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(data.Get("filterRules"));
            if (filterRuleslist != null)
                foreach (var item in filterRuleslist)
                this.SetWhere<T_BASE_COMPANY>(item, new T_BASE_COMPANY(), ref where);

            SqlModel sql = SqlModel.SelectAll(
                 T_SYS_AREA.AREA_TEXT.As("AREA_TEXT")
                 , "d1".Field("TITLE").As("INDUSTRY_TYPE_TEXT")
                // , "d2".Field("TITLE").As("POLLUTION_TYPE_TEXT")
               // // , "d3".Field(T_BAS_AIR_MONITOR.MONITOR_ITEM_NAME).As("POLLUTION_ITEM_TEXT")
                // , "d4".Field("TITLE").As("COMPANY_STATE_TEXT")
                 , T_THEPEAK_ENT_SUB_LIST.ID.As("ENTSUBID")
                 , T_THEPEAK_LEVEL_LIST_INFO.ID.As("LEVEL_ID")
                 , T_THEPEAK_ENT_SUB_LIST.ENT_PEAK_TYPE
                 , T_THEPEAK_ENT_SUB_LIST.THEPEAKID
                 , T_THEPEAK_ENT_SUB_LIST.IS_SELECT
                 , T_BASE_COMPANY.ID.As("COMPANY_ID")
                 , "d6".Field("TITLE").As("PEAK_LEVEL_TEXT")
                // , "d7".Field(BASUSER.TRUENAME).As("TOWN_SUPERVISOR_TEXT")
                // , "d8".Field(BASUSER.TRUENAME).As("INDUSTRY_SUPERVISOR_TEXT")
                 , "d9".Field(BASUSER.TRUENAME).As("SUPERVISOR_TEXT")

                 // , "d8".Field("TITLE").As("PEAK_LEVEL_TEXT")
                 )
            .From(DB.T_BASE_COMPANY)
            .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA== T_SYS_AREA.AREA_CODE)
            .LeftJoin(DB.BASDIC.As("d1")).On(T_BASE_COMPANY.BASTYPE == "d1".Field("CODE") & "d1".Field("TYPECODE") == ConstStrings.IndustryType)
            //.LeftJoin(DB.BASDIC.As("d2")).On(T_BASE_COMPANY.POLLUTION_TYPE == "d2".Field("CODE") & "d2".Field("TYPECODE") == ConstStrings.PollutionType)
            ///.LeftJoin(DB.T_BAS_AIR_MONITOR.As("d3")).On(T_BASE_COMPANY.POLLUTION_ITEM == "d3".Field(T_BAS_AIR_MONITOR.MONITOR_ITEM_CODE))
           // .LeftJoin(DB.BASDIC.As("d4")).On(T_BASE_COMPANY.COMPANY_STATE == "d4".Field("CODE") & "d4".Field("TYPECODE") == ConstStrings.Company_State)
           // .LeftJoin(DB.BASDIC.As("d5")).On(T_BASE_COMPANY.FUEL_TYPE == "d5".Field("CODE") & "d5".Field("TYPECODE") == ConstStrings.FuelType)
            .LeftJoin(DB.T_THEPEAK_LEVEL_CONFIG).On(T_THEPEAK_LEVEL_CONFIG.COMPANY_ID == T_BASE_COMPANY.ID)
            .LeftJoin(DB.T_THEPEAK_LEVEL_LIST_INFO).On(T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID)
            .LeftJoin(DB.T_THEPEAK_ENT_SUB_LIST).On(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == T_BASE_COMPANY.ID & T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_ENT_SUB_LIST.LEVEL_ID& T_THEPEAK_ENT_SUB_LIST.GUID == entGuid)//
             .LeftJoin(DB.T_THEPEAK_LEVEL_LIST_INFO.As("d10")).On("d10".Field("ID") == T_THEPEAK_ENT_SUB_LIST.CONTROL_MEAS)
            .LeftJoin(DB.BASDIC.As("d6")).On("d10".Field(T_THEPEAK_LEVEL_LIST_INFO.PEAK_LEVEL) == "d6".Field("CODE") & "d6".Field("TYPECODE") == ConstStrings.LevelType)
            //.LeftJoin(DB.BASUSER.As("d7")).On(T_BASE_COMPANY.TOWN_SUPERVISOR=="d7".Field(BASUSER.USERNAME))
            //.LeftJoin(DB.BASUSER.As("d8")).On(T_BASE_COMPANY.INDUSTRY_SUPERVISOR == "d8".Field(BASUSER.USERNAME))
            .LeftJoin(DB.BASUSER.As("d9")).On(T_BASE_COMPANY.SUPERVISOR == "d9".Field(BASUSER.USERNAME))
            .Where(where);
            return base.PagedJson(sql, data);
        }
        /// <summary>
        /// 获取企业等级列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLevelConfigData(RequestData data)
        {
            
            SqlModel sql = SqlModel.SelectAll(
                   "d1".Field("TITLE").As("PEAK_LEVEL_TEXT")
                 , "d2".Field("TITLE").As("WAR_TYPE_TEXT"))
            .From(DB.T_THEPEAK_LEVEL_LIST_INFO)
            // .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA== T_SYS_AREA.AREA_CODE)
            .LeftJoin(DB.BASDIC.As("d1")).On(T_THEPEAK_LEVEL_LIST_INFO.PEAK_LEVEL == "d1".Field("CODE") & "d1".Field("TYPECODE") == ConstStrings.LevelType)
            .LeftJoin(DB.BASDIC.As("d2")).On(T_THEPEAK_LEVEL_LIST_INFO.WAR_TYPE == "d2".Field("CODE") & "d2".Field("TYPECODE") == ConstStrings.WarType);

            return base.PagedJson(sql,data);
        }
        

        [HttpPost]
        public ActionResult GetUserTypeData()
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.PowerType).OrderByAsc(BASDIC.SORTNO)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        /// <summary>
        /// 获取生产线
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult GetCapacityText(RequestData data)
        {

            PostResult result = new PostResult();
            //if ("[]" != data.Get("codes"))
            //{
            //    string condition = StringHelper.SqlInCondition(SerializerHelper.Deserialize<List<string>>(data.Get("codes")));
            //    var sql = SqlModel.Select(T_COMPANY_PRODUCTION_LINES.COMPANY_ID.As("CODE"), T_COMPANY_PRODUCTION_LINES.PRODUCTION_LINES_NAME.As("TEXT")).From(DB.T_COMPANY_PRODUCTION_LINES).Where(T_COMPANY_PRODUCTION_LINES.COMPANY_ID.In(condition));
            //    result.Success = true;
            //    result.Data = sql.ExecToDynamicList();
            //}
            //else
            //{
            //    result.Success = false;
            //    result.Message = "没有传入CODE";
            //}
            return Json(result);
            
        }



        /// <summary>
        ///获取区域
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAreaData(RequestData data)
        {
            FieldModel where = null;
            string relationType = data.Get("userTypeId");
            if (relationType == "1")
            {
                where &= T_SYS_AREA.AREA_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode));
            }
            else
            {

                where &= T_SYS_AREA.PARENT_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode));
            }
            DataTable dt = SqlModel.Select(T_SYS_AREA.AREA_CODE.As("CODE"), T_SYS_AREA.AREA_TEXT.As("TITLE")).From(DB.T_SYS_AREA)
            .Where(where).ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        /// <summary>
        ///获取等级数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPeakLevelData(RequestData data)
        {
            FieldModel where = null;
           // string relationType = data.Get("userTypeId");
           var sqlDynamicList= SqlModel.Select(T_THEPEAK_LEVEL_LIST_INFO.PEAK_LEVEL.As("CODE"), "d1".Field("TITLE")).From(DB.T_THEPEAK_LEVEL_LIST_INFO)
               .LeftJoin(DB.BASDIC.As("d1")).On(T_THEPEAK_LEVEL_LIST_INFO.PEAK_LEVEL == "d1".Field("CODE") & "d1".Field("TYPECODE") == ConstStrings.LevelType)
            .Where(where).ExecToDynamicList();
            return Json(sqlDynamicList);
        }

      
        /// <summary>
        /// 下发任务时插入企业关联
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertConfigEnt(RequestData data)
        {
            string levelId = data.Get("LevelId");
             string peakId = data.Get("peakId");
            string getGuid = Guid.NewGuid().ToString();
            PostResult result = new PostResult();
            try
            {
                if (!string.IsNullOrEmpty(peakId))
                {
                    var getEnt = SqlModel.Select(T_THEPEAK_ENT_SUB_LIST.GUID)

                        .From(DB.T_THEPEAK_ENT_SUB_LIST)

                        .Where(T_THEPEAK_ENT_SUB_LIST.THEPEAKID == int.Parse(peakId)).ExecToDynamicList();
                    if (getEnt.Count > 0)
                        getGuid = getEnt[0]["GUID"];

                }
                else
                {
                    if (!string.IsNullOrEmpty(levelId))
                    {
                        //获取配置中的各项类型

                        var getConfigEnt = SqlModel.SelectAll(
                            T_THEPEAK_LEVEL_CONFIG.ID.As("ConfigId"),
                            T_THEPEAK_LEVEL_LIST_INFO.ID.As("LeveId")
                            )

                         .From(DB.T_THEPEAK_LEVEL_CONFIG)
                         .LeftJoin(DB.T_THEPEAK_LEVEL_LIST_INFO).On(T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID)
                         .Where(T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID == levelId).ExecToDynamicList();
                        T_THEPEAK_ENT_SUB_LISTModel modelEnt = new T_THEPEAK_ENT_SUB_LISTModel();

                        //  modelEnt.Delete(T_THEPEAK_ENT_SUB_LIST.THEPEAKID.IsNull() & T_THEPEAK_ENT_SUB_LIST.GUID != getGuid);


                        //往企业关系表插入值
                        getConfigEnt.ForEach(
                            m =>
                            {


                                modelEnt = SerializerHelper.Deserialize<T_THEPEAK_ENT_SUB_LISTModel>(SerializerHelper.Serialize(m), new List<string>() { "ID" });
                                if (StringHelper.DynamicToString(m["IS_STOP"]) == "1")
                                    modelEnt.ENT_PEAK_TYPE = 1;
                                else if (StringHelper.DynamicToString(m["LIMIT_TYPE"]) == "TheLimit")
                                    modelEnt.ENT_PEAK_TYPE = 2;
                                else if (StringHelper.DynamicToString(m["LIMIT_TYPE"]) == "limitTime")
                                    modelEnt.ENT_PEAK_TYPE = 3;
                                modelEnt.COMPANY_ID = m["COMPANY_ID"];
                                modelEnt.LEVEL_ID = m["PEAK_LEVE_ID"];
                                modelEnt.CONTROL_MEAS = m["PEAK_LEVE_ID"];
                                modelEnt.GUID = getGuid;
                                modelEnt.IS_SELECT = 1;
                                // modelEnt.ID = null;


                                modelEnt.Insert();
                            });


                        //   result.Data = modelEnt.GUID;
                        // result.Success = true;

                    }
                    else
                        result.Success = false;
                }
            }
            catch (Exception ex)
            {
                 
            }
            result.Data = getGuid;
            result.Success = true;



            return Json(result);

        }
        /// <summary>
        /// 下发任务时未保存的企业关联
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteConfigEnt(RequestData data)
        {
            string levelId = data.Get("LevelId");
            string entGuid = data.Get("entGuid");
            PostResult result = new PostResult();
            if (!string.IsNullOrEmpty(entGuid))
            {
                
                T_THEPEAK_ENT_SUB_LISTModel modelEnt = new T_THEPEAK_ENT_SUB_LISTModel();
                modelEnt.Delete(T_THEPEAK_ENT_SUB_LIST.THEPEAKID.IsNull() & T_THEPEAK_ENT_SUB_LIST.GUID != entGuid);
               // result.Data = entGuid;
                result.Success = true;

            }
            else
                result.Success = false;


            return Json(result);

        }

        /// <summary>
        /// 关闭窗口时删除未保存的配置记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CloseWinDelConfigEnt(RequestData data)
        {
            //string levelId = data.Get("LevelId");
            string entGuid = data.Get("entGuid");
            PostResult result = new PostResult();
            result.Success = false;
            T_THEPEAK_ENT_SUB_LISTModel modelEnt = new T_THEPEAK_ENT_SUB_LISTModel();
            modelEnt.Delete(T_THEPEAK_ENT_SUB_LIST.THEPEAKID.IsNull()& T_THEPEAK_ENT_SUB_LIST.GUID==entGuid);
            result.Success = true;
            return Json(result);

        }
        /// <summary>
        /// 更新等级（调控）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckUpdateLevel(RequestData data)
        {
            string levelId = data.Get("LevelId");
            string entid = data.Get("entId");
            PostResult result = new PostResult();
            result.Success = false;
            T_THEPEAK_ENT_SUB_LISTModel modelEnt = new T_THEPEAK_ENT_SUB_LISTModel();
            //modelEnt.LEVEL_ID =decimal.Parse( levelId);
            modelEnt.CONTROL_MEAS = decimal.Parse(levelId);
            modelEnt.Update(T_THEPEAK_ENT_SUB_LIST.ID==decimal.Parse(entid));
            result.Success = true;
            return Json(result);

        }
        /// <summary>
        /// 勾选企业
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckConfigEnt(RequestData data)
        {
            string levelId = data.Get("LevelId");
            string entid = data.Get("entId");
            bool isCheck =bool.Parse( data.Get("isCheck"));
            string peakId = data.Get("peakId");
            string controlId = data.Get("controlId");
            string entType = data.Get("entType");
            string entGuid = data.Get("entGuid");
            string companyId = data.Get("companyId"); 
            PostResult result = new PostResult();
            result.Success = false;
            T_THEPEAK_ENT_SUB_LISTModel modelEnt = new T_THEPEAK_ENT_SUB_LISTModel();
            modelEnt.LEVEL_ID = decimal.Parse(levelId);
            //modelEnt.GUID
            modelEnt.CONTROL_MEAS = string.IsNullOrEmpty(controlId)? modelEnt.LEVEL_ID :decimal.Parse(controlId);
            if(!string.IsNullOrEmpty(peakId))
            modelEnt.THEPEAKID = decimal.Parse(peakId);
            modelEnt.ENT_PEAK_TYPE = decimal.Parse(entType);
            modelEnt.GUID = entGuid;
            modelEnt.COMPANY_ID = decimal.Parse(companyId);
            if (isCheck)
                modelEnt.IS_SELECT = 1;
            // modelEnt.Insert();
            else  modelEnt.IS_SELECT = 0;

                modelEnt.Update(T_THEPEAK_ENT_SUB_LIST.ID == decimal.Parse(entid));
            result.Success = true;
            return Json(result);

        }

        /// <summary>
        /// 获取停限时间错 企业数量
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCompanyNum(RequestData data)
        {
            string entGuid = data.Get("entGuid");
            PostResult result = new PostResult();
            result.Success = false;
            T_THEPEAK_ENT_SUB_LISTModel modelEnt = new T_THEPEAK_ENT_SUB_LISTModel();
            int numberStop=SqlModel.Select(T_THEPEAK_ENT_SUB_LIST.ID).From(DB.T_THEPEAK_ENT_SUB_LIST)
            .Where(T_THEPEAK_ENT_SUB_LIST.ENT_PEAK_TYPE ==1& T_THEPEAK_ENT_SUB_LIST.GUID== entGuid& T_THEPEAK_ENT_SUB_LIST.IS_SELECT==1).ExecToDynamicList().Count;
            int numberLimit = SqlModel.Select(T_THEPEAK_ENT_SUB_LIST.ID).From(DB.T_THEPEAK_ENT_SUB_LIST)
            .Where(T_THEPEAK_ENT_SUB_LIST.ENT_PEAK_TYPE ==2 & T_THEPEAK_ENT_SUB_LIST.GUID == entGuid & T_THEPEAK_ENT_SUB_LIST.IS_SELECT == 1).ExecToDynamicList().Count;
            int numberTime = SqlModel.Select(T_THEPEAK_ENT_SUB_LIST.ID).From(DB.T_THEPEAK_ENT_SUB_LIST)
           .Where(T_THEPEAK_ENT_SUB_LIST.ENT_PEAK_TYPE == 3 & T_THEPEAK_ENT_SUB_LIST.GUID == entGuid & T_THEPEAK_ENT_SUB_LIST.IS_SELECT == 1).ExecToDynamicList().Count;
            result.Success = true;
            result.Data ="[{\"StopNum\":\""+ numberStop + "\",\"LimitNum\":\""+ numberLimit + "\",\"TimeNum\":\""+ numberTime + "\"}]" ;
            return Json(result);
        }
        /// <summary>
        /// 获取企业区域
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCompanyAreaCode(RequestData data)
        {
            string levelId = data.Get("LevelId");
             
         
            FieldModel where = null;
            where &= T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID == levelId;
            where &= T_THEPEAK_ENT_SUB_LIST.IS_SELECT == 1;

            var dynmaicSql = SqlModel.Select(
             
                 T_SYS_AREA.AREA_CODE
                 )
            .From(DB.T_BASE_COMPANY)
            .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA== T_SYS_AREA.AREA_CODE)
            .LeftJoin(DB.T_THEPEAK_LEVEL_CONFIG).On(T_THEPEAK_LEVEL_CONFIG.COMPANY_ID == T_BASE_COMPANY.ID)
            .LeftJoin(DB.T_THEPEAK_LEVEL_LIST_INFO).On(T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID)
            .LeftJoin(DB.T_THEPEAK_ENT_SUB_LIST).On(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == T_BASE_COMPANY.ID & T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_ENT_SUB_LIST.LEVEL_ID)
            .Where(where).ExecToDynamicList().Select(c=> { return c["AREA_CODE"]; }).Distinct();

            return Json(dynmaicSql);
        }
        /// <summary>
        /// 获取企业行业类型 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCompanyIndustryType(RequestData data)
        {
            string levelId = data.Get("LevelId");


            FieldModel where = null;
            where &= T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID == levelId;
            where &= T_THEPEAK_ENT_SUB_LIST.IS_SELECT == 1;

            var dynmaicSql = SqlModel.Select(

                  T_BASE_COMPANY.BASTYPE
                  )
             .From(DB.T_BASE_COMPANY)
             .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA== T_SYS_AREA.AREA_CODE)
             .LeftJoin(DB.T_THEPEAK_LEVEL_CONFIG).On(T_THEPEAK_LEVEL_CONFIG.COMPANY_ID == T_BASE_COMPANY.ID)
             .LeftJoin(DB.T_THEPEAK_LEVEL_LIST_INFO).On(T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID)
             .LeftJoin(DB.T_THEPEAK_ENT_SUB_LIST).On(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == T_BASE_COMPANY.ID & T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_ENT_SUB_LIST.LEVEL_ID)
             .Where(where).ExecToDynamicList().Select(c => { return c["BASTYPE"]; }).Distinct();

            return Json(dynmaicSql);
        }
        /// <summary>
        /// √选企业进行改变
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateCompanyForAreaCode(RequestData data)
        {
            string levelId = data.Get("LevelId");
            string areaCode = data.Get("areaCode");
            bool isCheck = bool.Parse(data.Get("isCheck"));
            string entGuid= data.Get("entGuid");
            string peakId = data.Get("peakId");
          
            if (!string.IsNullOrEmpty(areaCode))
            {
                string[] arrayArea = areaCode.Split(',');
                for (int i = 0; i < arrayArea.Length; i++)
                    arrayArea[i]="'"+ arrayArea[i] + "'";
                areaCode = string.Join(",", arrayArea);
            }
          

            /*思路： 根据等级和区域获取相关企业
             *       获取企业关联表ent
             *       遍历企业表
             *       如果check为false就删除
             *       查找关联表如果表中不存在相关企业进行插入
             */
            T_THEPEAK_ENT_SUB_LISTModel modelEnt = new T_THEPEAK_ENT_SUB_LISTModel();
            modelEnt.GUID = entGuid;

            FieldModel where = null;
            where &= T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID == levelId;
            if(!string.IsNullOrEmpty(areaCode))
            where &= T_BASE_COMPANY.AREA.In(areaCode);


            var dynmaicSql2 = SqlModel.SelectAll(
                  T_SYS_AREA.AREA_TEXT.As("AREA_TEXT")
                  , T_SYS_AREA.AREA_CODE
                  // , T_BASE_COMPANY.ID.As("T_COMPANY_ID")
                  , T_THEPEAK_LEVEL_LIST_INFO.ID.As("LEVEL_ID")
                  , T_THEPEAK_LEVEL_CONFIG.IS_STOP
                  , T_THEPEAK_LEVEL_CONFIG.LIMIT_TYPE
                  , T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID
                  )
             .From(DB.T_BASE_COMPANY)
             .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA== T_SYS_AREA.AREA_CODE)
             .LeftJoin(DB.T_THEPEAK_LEVEL_CONFIG).On(T_THEPEAK_LEVEL_CONFIG.COMPANY_ID == T_BASE_COMPANY.ID)
             .LeftJoin(DB.T_THEPEAK_LEVEL_LIST_INFO).On(T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID)
            // .LeftJoin(DB.T_THEPEAK_ENT_SUB_LIST).On(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == T_BASE_COMPANY.ID & T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_ENT_SUB_LIST.LEVEL_ID)
             .Where(where);


            var dynmaicSql = SqlModel.SelectAll(
                  T_SYS_AREA.AREA_TEXT.As("AREA_TEXT")
                  , T_SYS_AREA.AREA_CODE
                 // , T_BASE_COMPANY.ID.As("T_COMPANY_ID")
                  , T_THEPEAK_LEVEL_LIST_INFO.ID.As("LEVEL_ID")
                  ,T_THEPEAK_LEVEL_CONFIG.IS_STOP
                  , T_THEPEAK_LEVEL_CONFIG.LIMIT_TYPE
                  , T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID
                  )
             .From(DB.T_BASE_COMPANY)
             .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA== T_SYS_AREA.AREA_CODE)
             .LeftJoin(DB.T_THEPEAK_LEVEL_CONFIG).On(T_THEPEAK_LEVEL_CONFIG.COMPANY_ID == T_BASE_COMPANY.ID)
             .LeftJoin(DB.T_THEPEAK_LEVEL_LIST_INFO).On(T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID)
            // .LeftJoin(DB.T_THEPEAK_ENT_SUB_LIST).On(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == T_BASE_COMPANY.ID & T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_ENT_SUB_LIST.LEVEL_ID)
             .Where(where).ExecToDynamicList();

            //var configEnt = SqlModel.SelectAll().From(DB.T_THEPEAK_ENT_SUB_LIST).Where(T_THEPEAK_ENT_SUB_LIST.LEVEL_ID == decimal.Parse(levelId)& T_THEPEAK_ENT_SUB_LIST.GUID== entGuid).ExecToDynamicList();

            if (!isCheck)
            {
              
                //删除
                dynmaicSql.ForEach(
                m => {
                    modelEnt = new T_THEPEAK_ENT_SUB_LISTModel();
                    modelEnt.IS_SELECT = 0;
                    modelEnt.Update(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == m["ID"]& T_THEPEAK_ENT_SUB_LIST.LEVEL_ID == m["LEVEL_ID"]& T_THEPEAK_ENT_SUB_LIST.GUID ==entGuid);
                }); //&modelEnt.GUID== entGuid
            }
            else
                //往企业关系表插入值
                dynmaicSql.ForEach(
                m =>
                {
                      modelEnt.IS_SELECT = 1;
                        modelEnt.Update(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == m["ID"] & T_THEPEAK_ENT_SUB_LIST.LEVEL_ID == m["LEVEL_ID"] & T_THEPEAK_ENT_SUB_LIST.GUID == entGuid);
                  
                });
     


            return Json(dynmaicSql);
        }


        /// <summary>
        /// √选行业类型进行改变
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateCompanyForIndustryType(RequestData data)
        {
            string levelId = data.Get("LevelId");
            string IndustryType = data.Get("IndustryType");
            bool isCheck = bool.Parse(data.Get("isCheck"));
            string entGuid = data.Get("entGuid");
            string peakId = data.Get("peakId");
            if (!string.IsNullOrEmpty(IndustryType))
            {
                string[] arrayType = IndustryType.Split(',');
                for (int i = 0; i < arrayType.Length; i++)
                    arrayType[i] = "'" + arrayType[i] + "'";
                IndustryType = string.Join(",", arrayType);
            }
            // string actionType = data.Get("ActionType");

            /*思路： 根据等级和行业类型获取相关企业
             *       获取企业关联表ent
             *       遍历企业表
             *       如果check为false就删除
             *       查找关联表如果表中不存在相关企业进行插入
             */
            T_THEPEAK_ENT_SUB_LISTModel modelEnt = new T_THEPEAK_ENT_SUB_LISTModel();
            modelEnt.GUID = entGuid;

            FieldModel where = null;
            where &= T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID == levelId;
            if (!string.IsNullOrEmpty(IndustryType))
                where &= T_BASE_COMPANY.BASTYPE.In(IndustryType);


            var dynmaicSql = SqlModel.SelectAll(
                  T_SYS_AREA.AREA_TEXT.As("AREA_TEXT")
                  , T_SYS_AREA.AREA_CODE
                 // , T_BASE_COMPANY.ID.As("T_COMPANY_ID")
                  , T_THEPEAK_LEVEL_LIST_INFO.ID.As("LEVEL_ID")
                  , T_THEPEAK_LEVEL_CONFIG.IS_STOP
                  , T_THEPEAK_LEVEL_CONFIG.LIMIT_TYPE
                  , T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID
                  )
             .From(DB.T_BASE_COMPANY)
             .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA== T_SYS_AREA.AREA_CODE)
             .LeftJoin(DB.T_THEPEAK_LEVEL_CONFIG).On(T_THEPEAK_LEVEL_CONFIG.COMPANY_ID == T_BASE_COMPANY.ID)
             .LeftJoin(DB.T_THEPEAK_LEVEL_LIST_INFO).On(T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_LEVEL_CONFIG.PEAK_LEVE_ID)
            // .LeftJoin(DB.T_THEPEAK_ENT_SUB_LIST).On(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == T_BASE_COMPANY.ID & T_THEPEAK_LEVEL_LIST_INFO.ID == T_THEPEAK_ENT_SUB_LIST.LEVEL_ID)
             .Where(where).ExecToDynamicList();

            var configEnt = SqlModel.SelectAll().From(DB.T_THEPEAK_ENT_SUB_LIST).Where(T_THEPEAK_ENT_SUB_LIST.LEVEL_ID == decimal.Parse(levelId) & T_THEPEAK_ENT_SUB_LIST.GUID == entGuid).ExecToDynamicList();

            if (!isCheck)
            {
                modelEnt.IS_SELECT = 0;
                //删除
                dynmaicSql.ForEach(
                m => { modelEnt.Update(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == m["ID"] & T_THEPEAK_ENT_SUB_LIST.LEVEL_ID == m["LEVEL_ID"] & T_THEPEAK_ENT_SUB_LIST.GUID == entGuid); }); //&modelEnt.GUID== entGuid
            }
            else
                //往企业关系表插入值
                dynmaicSql.ForEach(
                m =>
                {
                    modelEnt.IS_SELECT = 1;
                    modelEnt.Update(T_THEPEAK_ENT_SUB_LIST.COMPANY_ID == m["ID"] & T_THEPEAK_ENT_SUB_LIST.LEVEL_ID == m["LEVEL_ID"] & T_THEPEAK_ENT_SUB_LIST.GUID == entGuid);
                });
           

            return Json(dynmaicSql);
        }

        /// <summary>
        /// √选行业类型进行改变
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ClosePeak(RequestData data)
        {
            PostResult result = new PostResult();
            try
            {
                string peakId = data.Get("peakId");
                T_THEPEAK_MAIN_LIST_INFOModel mainListModel = new T_THEPEAK_MAIN_LIST_INFOModel();
                mainListModel.IS_CLOSE = 1;
                mainListModel.Update(T_THEPEAK_MAIN_LIST_INFO.ID == int.Parse(peakId));
                result.Success = true;
                result.Message = "关闭成功！";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "关闭失败！原因:"+ ex.Message;
            }
            return Json(result);
           
        }


        /// <summary>
        /// 获取编号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAutoCode(RequestData data)
        {
            string emergencyCode = base.GetFormNo(ConstStrings.TASK, "yyyyMMdd", DateTime.Now.ToString("HHmmss"));
            return Json(emergencyCode);
        }
        /// <summary>
        /// 获取正处于错峰的企业
        /// </summary>
        /// <returns></returns>
        public ActionResult GetThePeakEnterprise()
        {
            ThePeakInfo info = new ThePeakInfo();
            return Json(info.GetThePeakEnterprise());
        }
    }
}
