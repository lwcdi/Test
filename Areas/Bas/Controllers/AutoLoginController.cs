using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.Bas.Controllers
{
    public class AutoLoginController : Controller
    {
        [HttpPost]
        public ActionResult IsAutoLogin(RequestData data)
        {
            var isAutoLogin = false;
            DataTable dt = SqlModel.Select(BASDIC.SORTNO, BASDIC.TITLE, BASDIC.CODE)
                .From(DB.BASDIC)
            .Where(BASDIC.TYPECODE == "AutoLoginSetting" & BASDIC.SORTNO == "1")
            .OrderByAsc(BASDIC.SORTNO)
            .ExecToDataTable();
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["CODE"].ToString().Trim() == "1")
            {
                isAutoLogin = true;
            }
            return Json(isAutoLogin);
        }

        [HttpPost]
        public ActionResult GetMeanList(RequestData data)
        {
            DataTable dt = SqlModel.Select(BASDIC.SORTNO, BASDIC.TITLE, BASDIC.CODE)
                .From(DB.BASDIC)
            .Where(BASDIC.TYPECODE == "AutoLoginSetting")
            .OrderByAsc(BASDIC.SORTNO)
            .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
    }
}
