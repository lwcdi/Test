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
    public class AlertCheckController : Controller
    {
        //
        // GET: /Report/PollutionOnlIne/
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult AirMonitor()
        {
            return View();
        }
        public ActionResult WaterMonitor()
        {
            return View();
        }
        public ActionResult CompanyNetwork()
        {
            return View();
        }
        /// <summary>
        /// 数据传输率
        /// </summary>
        /// <returns></returns>
        public ActionResult DataEffective()
        {
            return View();
        }
        public ActionResult Report()
        {
            return View();
        }
        /// <summary>
        /// 企业告警统计
        /// </summary>
        /// <returns></returns>
        public ActionResult CompanyWarn()
        {
            return View();
        }
        /// <summary>
        /// 排放量预警统计
        /// </summary>
        /// <returns></returns>
        public ActionResult PFLWarn()
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
                string alerttype = data.Get("alerttype");
                etime = etime.AddHours(23).AddMinutes(59).AddSeconds(59);
                FieldModel where = "t".Field(VW_MID_ALERT.TYPE) == alerttype & "t".Field(VW_MID_ALERT.STARTTIME).BetweenAnd(stime, etime);
                if (!string.IsNullOrEmpty(pkid))
                    where &= "t".Field(VW_MID_ALERT.PKID) == pkid;
                else
                    where &= "t".Field(VW_MID_ALERT.COMPANYID) == companyid;

                var model = SqlModel.Select("vw".Field("*"))
                .From(DB.VW_MID_ALERT.As("vw"))
                //.Where(where)
                .OrderByDesc("vw".Field(VW_MID_ALERT.STARTTIME));

                var dt = model.ExecToPagedTable(Convert.ToInt32(page), Convert.ToInt32(rows), new OrderByModel() { OrderType = OrderType.Desc, FieldModel = "vw".Field(VW_MID_ALERT.STARTTIME) }, where);
                var dtrows = dt.Data.Select();
                ret = new { success = true, rows = (object)dt.Data, total = dt.TotalCount, no = dtrows.Where(r => "0" == r[VW_MID_ALERT.STATE.Name].ToString()).Count(), yes = dtrows.Where(r => "1" == r[VW_MID_ALERT.STATE.Name].ToString()).Count() };
            }
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return Content(JsonConvert.SerializeObject(ret, timeConverter), "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult GetAlertById(string id)
        {
            var ret = new { success = false, data = new Object() };
            if (!string.IsNullOrEmpty(id))
            {
                var model = SqlModel.Select("vw".Field("*"))
                .From(DB.VW_MID_ALERT.As("vw"));
                //.Where(VW_MID_ALERT.ID == id);
                var dt = model.ExecToDataTable(null, "t".Field(VW_MID_ALERT.ID) == id);
                ret = new { success = true, data = (object)dt };
            }
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            return Content(JsonConvert.SerializeObject(ret, timeConverter), "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult SaveAlertById(string id, string content)
        {
            var ret = new { success = false, data = "保存失败！" };
            if (!string.IsNullOrEmpty(id))
            {
                var model = new T_MID_ALERTModel
                {
                    STATE = 1,
                    CHECKUSER = CurrentUser.UserName,
                    CHECKTIME = DateTime.Now,
                    CHECKCONTENT = content
                };
                bool result = model.Update(T_MID_ALERT.ID == id & T_MID_ALERT.STATE == 0);
                if (result)
                {
                    ret = new { success = true, data = "保存成功" };
                }
            }
            return Content(JsonConvert.SerializeObject(ret), "application/json", Encoding.UTF8);
        }
    }
}
