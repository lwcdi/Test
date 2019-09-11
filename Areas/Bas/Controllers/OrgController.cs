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
    public class OrgController : ListController
    {
        protected override SqlModel GetSqlModel(RequestData data)
        {


            return SqlModel.SelectAll(BASORG.PARENTID.As("\"_parentId\""))
                .From(DB.BASORG);
        }

        protected override bool DoAdd(RequestData data)
        {
            int pid = data.Get("ParentId").ToInt32();
            pid = pid > 0 ? pid : 0;
            BASORGModel model = new BASORGModel();
            model.ORGNAME = data.Get("OrgName");
            model.PARENTID = pid;
            model.SORTNO = data.Get("SortNo").ToInt32();
            model.REMARK = data.Get("Remark");
            int id = model.GetIDByInsert();
            if (id > 0)
            {
                model = new BASORGModel();
                model.RID = this.GetRid(pid, id);
                model.Update(BASORG.ID == id);
            }
            return true;
        }

        protected override bool DoEdit(RequestData data)
        {
            int pid = data.GetInt("ParentId");
            pid = pid > 0 ? pid : 0;
            int id = data.GetInt("id");
            BASORGModel model = new BASORGModel();
            model.ORGNAME = data.Get("OrgName");
            model.PARENTID = pid;
            model.RID = this.GetRid(pid, id);
            model.SORTNO = data.Get("SortNo").ToInt32();
            model.REMARK = data.Get("Remark");
            return model.Update(BASORG.ID == id);
        }

        protected override bool DoDelete(RequestData data)
        {
            BASORGModel model = new BASORGModel();
            return model.Delete(BASORG.ID == data.Get("id"));
        }

        private string GetRid(object parentId, object id)
        {
            if (int.Parse(parentId.ToString()) == 0)
            {
                return string.Format(".{0}.{1}.", parentId, id);
            }
            else
            {
                object ret = SqlModel.Select(BASORG.RID).From(DB.BASORG)
                    .Where(BASORG.ID == parentId).ExecuteScalar();
                if (ret != null && ret.ToString() != "")
                {
                    return string.Format("{0}{1}.", ret, id);
                }
                return "";
            }
        }

        [HttpPost]
        public ActionResult GetComboTreeOrg(RequestData data)
        {
            DataTable dt = SqlModel.Select(BASORG.ID.As("id"), BASORG.PARENTID.As("parentId"), BASORG.ORGNAME.As("text"))
                             .From(DB.BASORG).ExecToDataTable();
            DataRow r = dt.NewRow();
            r["id"] = -1;
            r["parentId"] = 0;
            r["text"] = "请选择组织部门";
            dt.Rows.InsertAt(r, 0);

            return Json(dt.ToDynamicComboTree("0"));
        }
    }
}
