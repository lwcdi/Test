using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Web.Areas.Report.Controllers
{
    [ActionParam]
    public class VocsReportController : Controller
    {
        //
        // GET: /Report/VocsReport/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CompanyNetwork()
        {
            return View();
        }
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
