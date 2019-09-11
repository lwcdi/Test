using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

 
using System.Data;
 
using Newtonsoft.Json;
 
using System.Text;

using UI.Web.Content.code.Handler;
using System.Web.Script.Serialization;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.vocSis.Controllers
{
    public class vocInduSonController : Controller
    {
        //
        // GET: /vocSis/vocInduSon/
 

        public ActionResult vocInList(int? navId)
        {
            return View(navId ?? 0);
        }

        [HttpPost]
        public string getDataInfo(RequestData data)
        {


            var list = IndustryData2.GetAreaMonitorData(Request.QueryString["Companyid"], DateTime.Parse(Request.QueryString["startTime"]), DateTime.Parse(Request.QueryString["endTime"]), Request.QueryString["selectData"], "VOCsItem");

            string d = new JavaScriptSerializer().Serialize(list);
            return d;
        }


        [HttpPost]
        public ActionResult getChartInfo(RequestData data)
        {
            var list = IndustryData2.GetAreaMonitorData(Request.QueryString["Companyid"], DateTime.Parse(Request.QueryString["startTime"]), DateTime.Parse(Request.QueryString["endTime"]), Request.QueryString["selectData"], "VOCsItem");


            StringBuilder x = new StringBuilder();
            List<KeyValuePair<string, string>> y = new List<KeyValuePair<string, string>>();
            StringBuilder y1 = new StringBuilder();
            StringBuilder y2 = new StringBuilder();
            StringBuilder y3 = new StringBuilder();
            StringBuilder y4 = new StringBuilder();
            StringBuilder y5 = new StringBuilder();
            StringBuilder y6 = new StringBuilder();
            StringBuilder y7 = new StringBuilder();
             
            foreach (var row in list)
            {

                x.Append("'" + row["AREA_NAME"].ToString() + "',");



                y1.Append("'" + row["a25005_PFL_Value"].ToString() + "',");
                y2.Append("'" + row["a24088_PFL_Value"].ToString() + "',");
                y3.Append("'" + row["a25002_PFL_Value"].ToString() + "',");
                y4.Append("'" + row["flowvocs_Value"].ToString() + "',");

                y5.Append("'" + row["a01001_Value"].ToString() + "',");
                y6.Append("'" + row["a01002_Value"].ToString() + "',");
                y7.Append("'" + row["a25003_PFL_Value"].ToString() + "',");



            }
            if (x.Length > 0) x = x.Remove(x.Length - 1, 1);
            if (y1.Length > 0) y1 = y1.Remove(y1.Length - 1, 1);
            if (y2.Length > 0) y2 = y2.Remove(y2.Length - 1, 1);
            if (y3.Length > 0) y3 = y3.Remove(y3.Length - 1, 1);
            if (y4.Length > 0) y4 = y4.Remove(y4.Length - 1, 1);
            if (y5.Length > 0) y5 = y5.Remove(y5.Length - 1, 1);
            if (y6.Length > 0) y6 = y6.Remove(y6.Length - 1, 1);
            if (y7.Length > 0) y7 = y7.Remove(y7.Length - 1, 1);

            y.Add(new KeyValuePair<string, string>("二甲苯", y1.ToString()));
            y.Add(new KeyValuePair<string, string>("非甲烷总烃", y2.ToString()));
            y.Add(new KeyValuePair<string, string>("苯", y3.ToString()));
            y.Add(new KeyValuePair<string, string>("流量", y4.ToString()));
            y.Add(new KeyValuePair<string, string>("温度", y5.ToString()));
            y.Add(new KeyValuePair<string, string>("湿度", y6.ToString()));
            y.Add(new KeyValuePair<string, string>("甲苯", y7.ToString()));




            return Json(EChartsHelper.GetBaseChart(x.ToString(), y, "", "厘米", "'二甲苯','非甲烷总烃','苯','流量','温度','湿度','甲苯'", " ", "bar",false,false));
        }
    }
}
