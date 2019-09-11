using System.Web.Mvc;

namespace UI.Web.Areas.vocSis
{
    public class vocSisAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "vocSis";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "vocSis_default",
                "vocSis/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
