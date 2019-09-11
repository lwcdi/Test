using System.Web.Mvc;

namespace UI.Web.Areas.PlatformIndex
{
    public class PlatformIndexAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PlatformIndex";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PlatformIndex_default",
                "PlatformIndex/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
