using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.BaseData.Controllers
{
    [ActionParam]
    public class AlertStatController : Controller
    {
        //
        // GET: /Report/PollutionOnlIne/
        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAlert(RequestData data)
        {
            var ret = new { success = false, rows = new Object(), total = 0, no = 0, yes = 0 };
            var pkid = data.Get("pkid");
            var companyid = data.Get("companyid");
            if (!string.IsNullOrEmpty(pkid) || !string.IsNullOrEmpty(companyid))
            {
                string page = data.Get("page");
                string rows = data.Get("rows");
                DateTime stime = data.GetDateTime("stime");
                DateTime etime = data.GetDateTime("etime");
                etime = etime.AddHours(23).AddMinutes(59).AddSeconds(59);
                FieldModel where = "t".Field(VW_MID_ALERT.STARTTIME).BetweenAnd(stime, etime);
                if (!string.IsNullOrEmpty(pkid))
                    where &= "t".Field(VW_MID_ALERT_STAT.PKID) == pkid;
                else
                    where &= "t".Field(VW_MID_ALERT_STAT.COMPANYID) == companyid;

                var model = SqlModel.Select("vw".Field("*"))
                .From(DB.VW_MID_ALERT_STAT.As("vw"))
                //.Where(where)
                .OrderByAsc("vw".Field(VW_MID_ALERT_STAT.PKNAME));


                var dt = model.ExecToPagedTable(Convert.ToInt32(page), Convert.ToInt32(rows), new OrderByModel() { OrderType = OrderType.Asc, FieldModel = "vw".Field(VW_MID_ALERT_STAT.PKNAME) }, where);
                var dtrows = dt.Data.Select();
                var no = dtrows.Sum(r => r[VW_MID_ALERT_STAT.STATE0.Name].ToInt32());
                var yes = dtrows.Sum(r => r[VW_MID_ALERT_STAT.STATE1.Name].ToInt32());
                ret = new { success = true, rows = (object)dt.Data, total = no + yes, no = no, yes = yes };
            }
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return Content(JsonConvert.SerializeObject(ret, timeConverter), "application/json", Encoding.UTF8);
        }
    }
}
