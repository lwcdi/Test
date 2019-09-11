using System.Web.Mvc;

namespace UI.Web.Areas.ThePeakManager
{
    public class ThePeakManagerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ThePeakManager";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ThePeakManager_default",
                "ThePeakManager/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
