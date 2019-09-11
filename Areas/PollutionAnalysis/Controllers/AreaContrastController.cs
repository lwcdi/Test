using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
 
using Newtonsoft.Json;
using System.Data;
using System.Text;
using UI.Web.Content.code.Handler;
using System.Web.Script.Serialization;
using w.Model;
using w.ORM;


namespace UI.Web.Areas.PollutionAnalysis.Controllers
{
    public class AreaContrastController : ListController
    {
        //
        // GET: /PollutionAnalysis/AreaContrast/
 


        public ActionResult CAList(int? navId)
        {
            return View(navId ?? 0);
        }

        protected override SqlModel GetSqlModel(RequestData data)
        {
            FieldModel where = null;
            where &= T_BASE_COMPANY.ISDEL == "0";
            string CompanyName = data.Get("CompanyName");
            if (!string.IsNullOrEmpty(CompanyName))
                where &= T_BASE_COMPANY.NAME.Like(CompanyName);

            SqlModel sql = SqlModel.SelectAll()
                .From(DB.T_BASE_COMPANY)

                .Where(where).OrderByDesc(T_BASE_COMPANY.ID);



            return sql;
        }

        [HttpPost]
        public string getCompanyIDs( string selecttable)
        {

            DataTable dt = SqlModel.Select("select id from t_Base_Company where area in ("+ selecttable + ")").Native().ExecToDataTable();
            string ss = "";
           List<string> tt=new List<string>();
            foreach (DataRow row in dt.Rows)
            {

                tt.Add(row["ID"].ToString());


            }
            ss = string.Join(",",tt );
      
            return ss;
        }
        

        [HttpPost]
        public string getDataInfo(RequestData data)
        {
           string s= getCompanyIDs(Request.QueryString["Companyid"]);
 
 
            var list = AnalysisData2.GetAreaMonitorData(Request.QueryString["Companyid"], DateTime.Parse(Request.QueryString["startTime"]), DateTime.Parse(Request.QueryString["endTime"]), Request.QueryString["selectData"], "GasItem,WaterItem");
            
            string d = new JavaScriptSerializer().Serialize(list);
            return d;
        }


        [HttpPost]
        public ActionResult getChartInfo(RequestData data)
        {
            var list = AnalysisData2.GetAreaMonitorData(Request.QueryString["Companyid"], DateTime.Parse(Request.QueryString["startTime"]), DateTime.Parse(Request.QueryString["endTime"]), Request.QueryString["selectData"], "GasItem,WaterItem");

            //var dtData = MonitorData.GetMonitorData("", DateTime.Parse(Request.QueryString["startTime"]), DateTime.Parse(Request.QueryString["endTime"]), "C", Request.QueryString["Companyid"], Request.QueryString["selectData"], "GasItem,WaterItem");


            StringBuilder x = new StringBuilder();
            List<KeyValuePair<string, string>> y = new List<KeyValuePair<string, string>>();
            StringBuilder y1 = new StringBuilder();
            StringBuilder y2 = new StringBuilder();
            StringBuilder y3 = new StringBuilder();
            StringBuilder y4 = new StringBuilder();
            StringBuilder y5 = new StringBuilder();
            StringBuilder y6 = new StringBuilder();
            StringBuilder y7 = new StringBuilder();
            StringBuilder y8 = new StringBuilder();
            StringBuilder y9 = new StringBuilder();
            StringBuilder y10 = new StringBuilder();
            foreach (var row in list)
            {

                x.Append("'" + row["AREA_NAME"].ToString() + "',");


                y1.Append("'" + row["a34013_PFL_Value"].ToString() + "',");
                y2.Append("'" + row["a21026_PFL_Value"].ToString() + "',");
                y3.Append("'" + row["a21002_PFL_Value"].ToString() + "',");
                y4.Append("'" + row["flowgas_Value"].ToString() + "',");
                y5.Append("'" + row["a19001_Value"].ToString() + "',");

                y6.Append("'" + row["a01012_Value"].ToString() + "',");
                y7.Append("'" + row["a01013_Value"].ToString() + "',");
                y8.Append("'" + row["w01018_PFL_Value"].ToString() + "',");
                y9.Append("'" + row["w21003_PFL_Value"].ToString() + "',");
                y10.Append("'" + row["flowwater_Value"].ToString() + "',");
               


            }
            if (x.Length > 0) x = x.Remove(x.Length - 1, 1);
            if (y1.Length > 0) y1 = y1.Remove(y1.Length - 1, 1);
            if (y2.Length > 0) y2 = y2.Remove(y2.Length - 1, 1);
            if (y3.Length > 0) y3 = y3.Remove(y3.Length - 1, 1);
            if (y4.Length > 0) y4 = y4.Remove(y4.Length - 1, 1);
            if (y5.Length > 0) y5 = y5.Remove(y5.Length - 1, 1);
            if (y6.Length > 0) y6 = y6.Remove(y6.Length - 1, 1);
            if (y7.Length > 0) y7 = y7.Remove(y7.Length - 1, 1);
            if (y8.Length > 0) y8 = y8.Remove(y8.Length - 1, 1);
            if (y9.Length > 0) y9 = y9.Remove(y9.Length - 1, 1);
            if (y10.Length > 0) y10 = y10.Remove(y10.Length - 1, 1);
            y.Add(new KeyValuePair<string, string>("烟尘", y1.ToString()));
            y.Add(new KeyValuePair<string, string>("二氧化硫", y2.ToString()));
            y.Add(new KeyValuePair<string, string>("氮氧化物", y3.ToString()));
            y.Add(new KeyValuePair<string, string>("废气流量", y4.ToString()));
            y.Add(new KeyValuePair<string, string>("含氧量", y5.ToString()));
            y.Add(new KeyValuePair<string, string>("烟气温度", y6.ToString()));
            y.Add(new KeyValuePair<string, string>("烟气压力", y7.ToString()));
            y.Add(new KeyValuePair<string, string>("化学需氧量", y8.ToString()));
            y.Add(new KeyValuePair<string, string>("氨氮", y9.ToString()));
            y.Add(new KeyValuePair<string, string>("废水流量", y10.ToString()));
 



            return Json(EChartsHelper.GetBaseChart(x.ToString(), y, "", "厘米", "'烟尘','二氧化硫','氮氧化物','废气流量','含氧量','烟气温度','烟气压力','化学需氧量','氨氮','废水流量'", " ", "bar",false,false));
        }



        protected override bool DoAdd(RequestData data)
        {
            T_BASE_COMPANYModel model = new T_BASE_COMPANYModel();
            model.NAME = data.Get("NAME");
            model.CREATETIME = DateTime.Now;
            return model.Insert();
        }
        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult Add(RequestData data)
        {
            return base.Add(data);
        }

        protected override bool DoEdit(RequestData data)
        {
            T_BASE_COMPANYModel model = new T_BASE_COMPANYModel();
            model.NAME = data.Get("NAME");
            model.UPDATETIME = DateTime.Now;
            return model.Update(T_BASE_COMPANY.ID == data.Get("id"));
        }
        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult Edit(RequestData data)
        {
            return base.Edit(data);
        }

        protected override bool DoDelete(RequestData data)
        {
            T_BASE_COMPANYModel model = new T_BASE_COMPANYModel();
            model.ISDEL = ((int)DeleteType.Delete).ToString();
            return model.Update(T_BASE_COMPANY.ID == data.Get("id"));
        }
        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult Delete(RequestData data)
        {
            return base.Delete(data);
        }

    }

   public class bean{
       public string ID { get; set; }
    }
}
