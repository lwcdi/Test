using w.ORM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;


namespace UI.Web.Areas.BaseData.Controllers
{
    public class AreaInfoController : ListController
    {
        //
        // GET: /BaseData/AreaInfo/


        protected override SqlModel GetSqlModel(RequestData data)
        {

            SqlModel model = SqlModel.Select(T_SYS_AREA.AREA_CODE.As("ID"),
                        T_SYS_AREA.AREA_TEXT.As("NAME"),
                        T_SYS_AREA.PARENT_CODE.As("\"_parentId\""),
                        T_SYS_AREA.RELATION_TYPE.As("RELATIONTYPE"),
                        T_SYS_AREA.LICENSE_PLATE_PREFIX.As("PREFIX")
                        )
                .From(DB.T_SYS_AREA)
            .Where(T_SYS_AREA.AREA_CODE.In("'0','410000','410100','410122'") | T_SYS_AREA.PARENT_CODE == "410122");
            //SqlModel model = SqlModel.SelectAll()
            //    .From(DB.T_THEMEHEAD);

            return model;
        }

        protected override string BeforeSave(ActionType actionType, RequestData data)
        {
            if (actionType == ActionType.Delete)
            {
                int id = data.Get("id").ToInt32();
                object ret = SqlModel.Select(BASNAV.ID).From(DB.BASNAV).Where(BASNAV.PARENTID == id).ExecuteScalar();
                if (ret != null && ret.ToString() != "")
                {
                    return "该菜单有子菜单，不能直接删除！";
                }
            }
            return "";
        }

        protected override bool DoAdd(RequestData data)
        {
            T_SYS_AREAModel model = new T_SYS_AREAModel();
            model.AREA_CODE = data.Get("areacode");
            model.AREA_TEXT = data.Get("areatext");
            int pid = data.GetInt("parentid");
            model.PARENT_CODE = pid < 0 ? "0" : pid.ToString();
            model.RELATION_TYPE = (data.GetInt("relationtype") + 1).ToString();
            model.ORDER_ID = 0;
            model.LICENSE_PLATE_PREFIX = data.Get("PREFIX");
            return model.Insert();
        }

        protected override bool DoEdit(RequestData data)
        {
            T_SYS_AREAModel model = new T_SYS_AREAModel();
            model.AREA_CODE = data.Get("areacode");
            model.AREA_TEXT = data.Get("areatext");
            int pid = data.GetInt("parentid");
            model.PARENT_CODE = pid < 0 ? "0" : pid.ToString();
            //model.RELATION_TYPE = (data.GetInt("relationtype") + 1).ToString();
            model.ORDER_ID = 0;
            model.LICENSE_PLATE_PREFIX = data.Get("PREFIX");
            return model.Update(T_SYS_AREA.AREA_CODE == data.Get("id"));
        }

        protected override bool DoDelete(RequestData data)
        {
            T_SYS_AREAModel model = new T_SYS_AREAModel();
            return model.Delete(T_SYS_AREA.AREA_CODE == data.Get("id"));
        }

        [HttpPost]
        public ActionResult GetComboTreeNav(RequestData data)
        {
            DataTable dt =
                SqlModel.Select("T".Field("id"), "T".Field("parentId"), "T".Field("text"), "T".Field("SortNo"))
                .From(
                    (
                        SqlModel.Select((-1).As("id"), 0.As("parentId"), "请选择上级菜单".As("text"), 0.As("SortNo"))
                            .From(DB.BASNAV)
                    )
                    .Union
                    (
                        SqlModel.Select(BASNAV.ID.ToChar().As("id"), BASNAV.PARENTID.ToChar().As("parentId"), BASNAV.NAVTITLE.ToChar().As("text"), BASNAV.SORTNO.ToChar())
                        .From(DB.BASNAV)
                    ).As("T")
                )
                .OrderByAsc("T".Field("SortNo"))
                .ExecToDataTable();
            var json = dt.ToDynamicComboTree("0");
            return Json(json);
        }

        [HttpPost]
        public ActionResult GetNavButton(RequestData data)
        {
            int navId = data.GetInt("NavId");
            List<dynamic> list = SqlModel.Select(BASBUTTON.ID, BASBUTTON.BUTTONTEXT, BASBUTTON.ICONCLS)
                .From(DB.BASNAVBTN)
                .LeftJoin(DB.BASBUTTON).On(BASBUTTON.ID == BASNAVBTN.BTNID)
                .Where(BASNAVBTN.NAVID == navId).OrderByAsc(BASNAVBTN.SORTNO)
                .ExecToDynamicList();
            return Json(list);
        }

        [HttpPost]
        public ActionResult SaveNavButton(RequestData data)
        {
            PostResult result = new PostResult();
            string navId = data.Get("navId");
            string btnIdString = data.Get("btnIdString");
            string[] btnIdArray = btnIdString.Split(',');
            using (TranModel tran = new DBTranModel())
            {
                BASNAVBTNModel delModel = new BASNAVBTNModel();
                delModel.Deleting(tran, BASNAVBTN.NAVID == navId);

                BASNAVBTNModel insModel = null;
                for (int i = 0; i < btnIdArray.Length; i++)
                {
                    string btnId = btnIdArray[i];
                    insModel = new BASNAVBTNModel();
                    insModel.NAVID = navId.ToInt32();
                    insModel.BTNID = btnId.ToInt32();
                    insModel.SORTNO = i;
                    insModel.Inserting(tran);
                }

                bool ret = tran.Execute();
                result.Success = ret;
            }
            return Json(result);
        }

        public ActionResult SetButton()
        {
            return View();
        }
        public ActionResult GetAreaCodeAndText(RequestData data)
        {
            PostResult result = new PostResult();
            if ("[]" != data.Get("codes"))
            {
                string condition = StringHelper.SqlInCondition(SerializerHelper.Deserialize<List<string>>(data.Get("codes")));
                var sql = SqlModel.Select(T_SYS_AREA.AREA_CODE.As("CODE"), T_SYS_AREA.AREA_TEXT.As("TEXT")).From(DB.T_SYS_AREA).Where(T_SYS_AREA.AREA_CODE.In(condition));
                result.Success = true;
                result.Data = sql.ExecToDynamicList();

            }
            else
            {
                result.Success = false;
                result.Message = "没有传入CODE";
            }

            return Json(result);
        }


    }
}
