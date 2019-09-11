using System.Web.Mvc;

namespace UI.Web.Areas.PollutionAnalysis
{
    public class PollutionAnalysisAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PollutionAnalysis";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PollutionAnalysis_default",
                "PollutionAnalysis/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
