using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Web.Areas.Report.Controllers
{
    [ActionParam]
    public class PollutionOnlIneController : Controller
    {
        //
        // GET: /Report/PollutionOnlIne/
        
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
    }
}
