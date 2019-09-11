using w.ORM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;

namespace UI.Web.Areas.Bas.Controllers
{
    public class NavController : ListController
    {
        protected override SqlModel GetSqlModel(RequestData data)
        {
            SqlModel model = SqlModel.SelectAll(BASNAV.PARENTID.As("\"_parentId\""))
                .From(DB.BASNAV)
                .OrderByAsc(BASNAV.SORTNO);
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
            int pid = data.GetInt("parentid");
            BASNAVModel model = new BASNAVModel();
            model.NAVTITLE = data.Get("navtitle");
            model.NAVTITLEEN = data.Get("navtitleEN");
            model.NAVTAG = data.Get("navtag");
            model.PARENTID = pid < 0 ? 0 : pid;
            model.LINKURL = data.Get("linkurl");
            model.ICONCLS = data.Get("iconcls");
            model.ICONURL = data.Get("iconurl");
            model.BIGIMAGEURL = data.Get("bigimageurl");
            model.SORTNO = data.GetInt("SortNo");
            if (data.Get("isvisible") != "")
            {
                model.ISVISIBLE = data.Get("isvisible").ToInt32();
            }
            else
            {
                model.ISVISIBLE = 0;
            }
            return model.Insert();
        }

        protected override bool DoEdit(RequestData data)
        {
            int pid = data.GetInt("parentid");
            BASNAVModel model = new BASNAVModel();
            model.NAVTITLE = data.Get("navtitle");
            model.NAVTITLEEN = data.Get("navtitleEN");
            model.NAVTAG = data.Get("navtag");
            model.PARENTID = pid < 0 ? 0 : pid;
            model.LINKURL = data.Get("linkurl");
            model.ICONCLS = data.Get("iconcls");
            model.ICONURL = data.Get("iconurl");
            model.BIGIMAGEURL = data.Get("bigimageurl");
            model.SORTNO = data.GetInt("SortNo");
            if (data.Get("isvisible") != "" && data.GetInt("isvisible") == 1)
            {
                model.ISVISIBLE = 1;
            }
            else
            {
                model.ISVISIBLE = 0;
            }
            return model.Update(BASNAV.ID == data.Get("id"));
        }

        protected override bool DoDelete(RequestData data)
        {
            BASNAVModel model = new BASNAVModel();
            return model.Delete(BASNAV.ID == data.Get("id"));
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
    }
}
