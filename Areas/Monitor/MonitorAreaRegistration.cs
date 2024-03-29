﻿using System.Web.Mvc;

namespace UI.Web.Areas.Monitor
{
    public class MonitorAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Monitor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Monitor_default",
                "Monitor/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "UI.Web.Areas.Monitor.Controllers"}
            );
        }
    }
}
