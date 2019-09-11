using w.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;

namespace UI.Web.Areas.Bas.Controllers
{
    public class PersonalController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetConfigJson()
        {
            object ret = SqlModel.Select(BASUSER.CONFIGJSON).From(DB.BASUSER)
                .Where(BASUSER.ID == CurrentUser.Id).ExecuteScalar();
            string config = "{\"theme\":{\"title\":\"默认皮肤\",\"name\":\"default\",\"selected\":true},\"showType\":\"Accordion\",\"gridRows\":20}";
            if (ret != null && ret.ToString() != "")
            {
                config = ret.ToString();
            }
            string json = string.Format("var sys_config = {0}", config);
            return Content(json);
        }

        public ActionResult Save(RequestData data)
        {
            PostResult result = new PostResult();
            string json = data.Get("json");
            BASUSERModel user = new BASUSERModel();
            user.CONFIGJSON = json;
            bool ret = user.Update(BASUSER.ID == CurrentUser.Id);
            result.Success = ret;
            CurrentUser.ConfigJson = json;
            return Json(result);
        }
    }
}
