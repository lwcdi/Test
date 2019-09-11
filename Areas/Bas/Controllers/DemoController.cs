using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Web.Areas.Bas.Controllers
{
    public class DemoController : Controller
    {
        //
        // GET: /Bas/Demo/
        
        public virtual ActionResult View1()
        {
            ViewData["selectId"] = 2;
            return View();
        }
    }
}
