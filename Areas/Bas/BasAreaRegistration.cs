using System.Web.Mvc;

namespace UI.Web.Areas.Bas
{
    public class BasAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Bas";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                this.AreaName + "_default",
                this.AreaName + "/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
