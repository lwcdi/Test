using w.ORM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;

namespace UI.Web.Areas.Bas.Controllers
{
    public class RoleController : ListController
    {
        protected override SqlModel GetSqlModel(RequestData data)
        {
            return SqlModel.SelectAll()
                .From(DB.BASROLE);
        }

        protected override bool DoAdd(RequestData data)
        {
            BASROLEModel model = new BASROLEModel();
            model.ROLENAME = data.Get("RoleName");
            model.SORTNO = data.Get("SortNo").ToInt32();
            if (data.Get("isDefault") != null && data.GetInt("isDefault") == 1)
            {
                model.ISDEFAULT = 1;
            }
            else
            {
                model.ISDEFAULT = 0;
            }
            model.REMARK = data.Get("Remark");
            return model.Insert();
        }

        protected override bool DoEdit(RequestData data)
        {
            BASROLEModel model = new BASROLEModel();
            model.ROLENAME = data.Get("RoleName");
            model.SORTNO = data.Get("SortNo").ToInt32();
            if (data.Get("isDefault") != null && data.GetInt("isDefault") == 1)
            {
                model.ISDEFAULT = 1;
            }
            else
            {
                model.ISDEFAULT = 0;
            }
            model.REMARK = data.Get("Remark");
            return model.Update(BASROLE.ID == data.Get("id"));
        }

        protected override bool DoDelete(RequestData data)
        {
            BASROLEModel model = new BASROLEModel();
            return model.Delete(BASROLE.ID == data.Get("id"));
        }

        [HttpPost]
        public ActionResult GetRoleTable(RequestData data)
        {
            List<dynamic> list = SqlModel.SelectAll().From(DB.BASROLE).ExecToDynamicList();
            return Json(list);
        }

        [HttpPost]
        public ActionResult Authorize(RequestData data)
        {
            string d = data.Get("d");
            if (string.IsNullOrEmpty(d) == true)
            {
                return Json("参数错误！");
            }

            int k = this.RoleAuthorize(d) ? 1 : 0;
            return Json(k.ToString());
        }

        //角色授权
        private bool RoleAuthorize(string data)
        {
            JObject jobj = JObject.Parse(data);
            DataTableModel dtm = SqlModel.Select(BASBUTTON.ID, BASBUTTON.BUTTONTEXT, BASBUTTON.BUTTONTAG)
                .From(DB.BASBUTTON).ExecToTableModel();
            var roleId = jobj["roleId"];
            var menus = jobj["menus"];
            var navs = menus.Where(m => m["buttons"].Count() > 0);
            BASROLENAVBTNModel delModel = new BASROLENAVBTNModel();
            delModel.Delete(BASROLENAVBTN.ROLEID == roleId.ToString());

            using (TranModel tran = new DBTranModel())
            {
                //BASROLENAVBTNModel delModel = new BASROLENAVBTNModel();
                //delModel.Deleting(tran, BASROLENAVBTN.ROLEID == roleId.ToString());
                BASROLENAVBTNModel insModel = null;
                DataRowModel rm = null;
                foreach (var nav in navs)
                {
                    foreach (var btn in nav["buttons"])
                    {
                        rm = dtm.Find(m => m[BASBUTTON.BUTTONTAG].ToString() == btn.ToString());
                        insModel = new BASROLENAVBTNModel();
                        insModel.ROLEID = roleId.ToString().ToInt32();
                        insModel.NAVID = nav["navid"].ToString().ToInt32();
                        insModel.BTNID = rm[BASBUTTON.ID].ToInt32();
                        insModel.Inserting(tran);
                    }
                }
                return tran.Execute();
            }

        }

        [HttpPost]
        public ActionResult GetMenu(RequestData data)
        {
            string Id = data.Get("ID");
            var json = base.GetNavBtnJson(Id, 0);
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(json);
            return Content(jsonStr);
        }
    }
}
