using System.Web.Mvc;

namespace UI.Web.Areas.StainMonitor
{
    public class StainMonitorAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "StainMonitor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "StainMonitor_default",
                "StainMonitor/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
