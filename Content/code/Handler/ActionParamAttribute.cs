using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace UI.Web
{
    public class ActionParamAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewDataDictionary dict = new ViewDataDictionary();
            var param = filterContext.HttpContext.Request.QueryString;
            foreach (string key in param.Keys)
            {
                if (string.IsNullOrEmpty(key)) continue;
                if (key.StartsWith("con"))
                {
                    dict.Add(key.Split('_')[1], param[key]);
                }
            }
            filterContext.Controller.ViewBag.controllerParam = dict;
            base.OnActionExecuting(filterContext);
            
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }
    }
}