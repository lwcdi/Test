using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;
using w.ORM;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using UI.Web;
using Newtonsoft.Json.Linq;
using System.IO;


namespace UI.Web.Areas.Bas.Controllers
{
    public class CommonController : BaseController
    {
        [HttpPost]
        public ActionResult GetAreaData()
        {
            if (CurrentUser.UserTypeID == ((int)PowerType.ProvincialUser).ToString())
            {

                SqlModel model = SqlModel.Select("T".Field("id"), "T".Field("parentId"), "T".Field("text"), "T".Field("SortNo"), "T".Field("prefix"), "T".Field("relationtype"))
                   .From(
                       (
                           SqlModel.Select(T_SYS_AREA.AREA_CODE.ToChar().As("id"),
                                           "0".As("parentId"),
                                           T_SYS_AREA.AREA_TEXT.ToChar().As("text"),
                                           T_SYS_AREA.ORDER_ID.ToChar().As("SortNo"),
                                           T_SYS_AREA.LICENSE_PLATE_PREFIX.ToChar().As("prefix"),
                                           T_SYS_AREA.RELATION_TYPE.ToChar().As("relationtype")
                                           )
                                   .From(DB.T_SYS_AREA)
                           .Where(T_SYS_AREA.AREA_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == "SysAreaCode")))
                       )
                       .Union
                       (
                           SqlModel.Select(T_SYS_AREA.AREA_CODE.ToChar().As("id"),
                                           T_SYS_AREA.PARENT_CODE.ToChar().As("parentId"),
                                           T_SYS_AREA.AREA_TEXT.ToChar().As("text"),
                                           T_SYS_AREA.ORDER_ID.ToChar().As("SortNo"),
                                           T_SYS_AREA.LICENSE_PLATE_PREFIX.ToChar().As("prefix"),
                                           T_SYS_AREA.RELATION_TYPE.ToChar().As("relationtype")
                                           )
                                   .From(DB.T_SYS_AREA)
                           .Where(T_SYS_AREA.PARENT_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == "SysAreaCode")))
                       ).As("T")
                   )
                   .OrderByAsc("T".Field("SortNo"));

                //SqlModel model = SqlModel.Select(T_SYS_AREA.AREA_CODE.ToChar().As("id"),
                //                            T_SYS_AREA.PARENT_CODE.ToChar().As("parentId"),
                //                            T_SYS_AREA.AREA_TEXT.ToChar().As("text"),
                //                            T_SYS_AREA.ORDER_ID.ToChar().As("SortNo"),
                //                            T_SYS_AREA.LICENSE_PLATE_PREFIX.ToChar().As("prefix"),
                //                            T_SYS_AREA.RELATION_TYPE.ToChar().As("relationtype")
                //                            )
                //                    .From(DB.T_SYS_AREA)
                //            .Where(T_SYS_AREA.AREA_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == "SysAreaCode")) |
                //                   T_SYS_AREA.PARENT_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == "SysAreaCode"))
                //                   ).OrderByAsc(T_SYS_AREA.ORDER_ID);


                var json = model.ExecToDataTable().ToDynamicComboTree("0", "prefix,relationtype");
                return Json(json);
            }
            else if (CurrentUser.UserTypeID == ((int)PowerType.AreaUser).ToString())
            {
                DataTable dt =
                   SqlModel.Select("T".Field("id"), "T".Field("parentId"), "T".Field("text"), "T".Field("SortNo"), "T".Field("prefix"), "T".Field("relationtype"))
                   .From(
                       //(
                       //    SqlModel.Select((-1).As("id"), 0.As("parentId"), "请选择区域信息".As("text"), 0.As("SortNo"), 0.As("prefix"), 0.As("relationtype"))
                       //        .From(DB.T_SYS_AREA)
                       //)
                       //.Union
                       (
                           SqlModel.Select(T_SYS_AREA.AREA_CODE.ToChar().As("id"),
                                           (0).As("parentId"),
                                           T_SYS_AREA.AREA_TEXT.ToChar().As("text"),
                                           T_SYS_AREA.ID.ToChar().As("SortNo"),
                                           T_SYS_AREA.LICENSE_PLATE_PREFIX.ToChar().As("prefix"),
                                           T_SYS_AREA.RELATION_TYPE.ToChar().As("relationtype")
                                           )
                                   .From(DB.T_SYS_AREA)
                           .Where(T_SYS_AREA.PARENT_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == "SysAreaCode")) & T_SYS_AREA.AREA_CODE == CurrentUser.AreaCode
                                  )
                       ).As("T")
                   )
                   .OrderByAsc("T".Field("SortNo"))
                   .ExecToDataTable();

                var json = dt.ToDynamicComboTree("0", "prefix,relationtype");
                return Json(json);
            }
            return Json("");
        }


        [HttpPost]
        public ActionResult GetIndustryData(RequestData data)
        {


            StringBuilder sql = new StringBuilder();
            sql.Append(@"   (SELECT distinct   '410010' AS id,'0' AS parentId  , '行业' AS text, '行业' AS SortNo, '行业' AS prefix, '行业' AS relationtype from basdic ) UNION(select to_char(basdic.code) AS id, '410010' AS parentId,to_char(basdic.title) AS text  , '行业' AS SortNo, '行业' AS prefix, '行业' AS relationtype from basdic where typecode='IndustryType' ) ");

            SqlModel sqlModel = SqlModel.Select(sql.ToString()).Native();



            var json = sqlModel.ExecToDataTable().ToDynamicComboTree("0", "prefix,relationtype");
            return Json(json);


        }





        /// <summary>
        /// 用于数据编辑的时候选择城市
        /// by-L
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAreaData_Select(RequestData data)
        {
            string isAppendAll = data.Get("appendAll");

            FieldModel where = T_SYS_AREA.PARENT_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode));

            if (CurrentUser.UserTypeID != ((int)PowerType.ProvincialUser).ToString())
            {
                where &= T_SYS_AREA.AREA_CODE == CurrentUser.AreaCode;
            }


            DataTable dt = SqlModel.Select(T_SYS_AREA.AREA_CODE.As("CODE"), T_SYS_AREA.AREA_TEXT.As("TITLE")).From(DB.T_SYS_AREA)
            .Where(where).OrderByAsc(T_SYS_AREA.ID).ExecToDataTable();
            if ("appendAll" == isAppendAll)
            {
                DataRow allDr = dt.NewRow();
                allDr["CODE"] = "";
                allDr["TITLE"] = "全部";
                dt.Rows.InsertAt(allDr, 0);
            }
            return Json(dt.ToDynamicList());
        }

        [HttpPost]
        public ActionResult GetAreaDataByParent(RequestData data)
        {
            string parentCode = data.Get("parentCode");

            DataTable dt = SqlModel.Select(T_SYS_AREA.AREA_CODE.As("CODE"), T_SYS_AREA.AREA_TEXT.As("TITLE")).From(DB.T_SYS_AREA)
            .Where(T_SYS_AREA.PARENT_CODE == parentCode).OrderByAsc(T_SYS_AREA.ID).ExecToDataTable();
            return Json(dt.ToDynamicList());
        }

        public ActionResult GetCreateTreeData()
        {
            SqlModel model = SqlModel.Select(@"select a.area_code,a.area_text,b.area_text parenttext from T_SYS_AREA a left join T_SYS_AREA b on a.parent_code = b.area_code WHERE a.RELATION_TYPE < 4 AND a.Remark = 1 and a.parent_code='410100'").Native();

            DataTable dt = model.ExecToDataTable();

            return Json(dt.ToDynamicList());
        }
        /// <summary>
        /// 解析JSON数组生成对象实体集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"名称"}])</param>
        /// <returns>对象实体集合</returns>
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }
    }

    public class CHECKCOMPANY
    {
        public string XCZT { get; set; }
        public string AREACODE { get; set; }
        public string CODE { get; set; }
        public string OWNER { get; set; }
        public string NAME { get; set; }
        public string ADDRESS { get; set; }
    }

}
