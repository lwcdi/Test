using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.Bas.Controllers
{
    public class UserControlController : Controller
    {
        public ActionResult GroupSelect(ViewDataDictionary model)
        {
            ViewData["fs"] = model["fs"];
            ViewData["fq"] = model["fq"];
            ViewData["vocs"] = model["vocs"];
            ViewData["selectId"] = model["selectId"];
            ViewData["companyOnly"] = model["companyOnly"];
            ViewData["multiSelect"] = model["multiSelect"].ToString().ToLower();
            return PartialView("GroupSelect");
        }

        public JsonResult GetList(string selectId)
        {
            List<object> list = new List<object>();
            for (int i = 0; i < 5; i++)
            {
                if (i.ToString() == selectId)
                {
                    list.Add(new
                    {
                        id = i,
                        name = "text" + i,
                        selected = true
                    });
                }
                else
                {
                    list.Add(new
                    {
                        id = i,
                        name = "text" + i
                    });

                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOtherTree(string cType, string pType, string selectId, string companyOnly = "0")
        {
            //if (string.IsNullOrEmpty(pType)) return new EmptyResult();
            string strSQL = string.Format("select 'P'||t.id id,t.name text,t.type,'C'||t.companyid parentid from t_base_company_pk t where 1=1 {0}", string.IsNullOrEmpty(pType) ? "" : " and t.type in('"
                + string.Join("','", pType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) + "')");
            DataTable dtPK = SqlModel.Select(strSQL).Native().ExecToDataTable();

            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("ID");
            dtResult.Columns.Add("TEXT");
            dtResult.Columns.Add("checked", typeof(bool));
            dtResult.Columns.Add("PARENTID");
            dtResult.Columns.Add("TYPE");
            if (cType == "area")
            {
                strSQL = string.Format(@"select 'C'||t.id id,t.name text,t.gzcd type,a.area_code,a.area_text  from t_base_company t
                    left join t_sys_area a on t.area=a.area_code where  t.name is not null ");
                DataTable dtCompany = SqlModel.Select(strSQL).Native().ExecToDataTable();

                var areaList = (from p in dtCompany.AsEnumerable()
                                select new { AREA_CODE = p.Field<string>("AREA_CODE"), AREA_TEXT = p.Field<string>("AREA_TEXT") }).Distinct();
                DataRow newRow;
                foreach (var area in areaList)
                {
                    bool b = false;
                    DataRow[] companys = dtCompany.Select(" area_code='" + area.AREA_CODE + "'");
                    foreach (DataRow company in companys)
                    {
                        DataRow[] rows = dtPK.Select(" parentid='" + company["ID"] + "'");
                        if (rows == null || rows.Length == 0) continue;

                        newRow = dtResult.NewRow();
                        newRow["ID"] = company["ID"];
                        newRow["checked"] = company["ID"].ToString() == selectId ? true : false;
                        newRow["TEXT"] = company["TEXT"];
                        newRow["TYPE"] = company["TYPE"];
                        newRow["PARENTID"] = area.AREA_CODE;
                        dtResult.Rows.Add(newRow);

                        b = true;
                        if (companyOnly == "1") continue;
                        foreach (DataRow row in rows)
                        {
                            newRow = dtResult.NewRow();
                            newRow["ID"] = row["ID"];
                            newRow["TEXT"] = row["TEXT"];
                            newRow["checked"] = row["ID"].ToString() == selectId ? true : false;
                            newRow["PARENTID"] = row["PARENTID"];
                            newRow["TYPE"] = row["TYPE"];
                            dtResult.Rows.Add(newRow);
                        }
                    }
                    if (b)
                    {
                        newRow = dtResult.NewRow();
                        newRow["ID"] = area.AREA_CODE;
                        newRow["TEXT"] = area.AREA_TEXT;
                        newRow["checked"] = area.AREA_CODE == selectId ? true : false;
                        newRow["PARENTID"] = "-1";
                        dtResult.Rows.Add(newRow);
                    }
                }
            }
            else if (cType == "bas")
            {
                strSQL = string.Format(@"select 'C'||t.id id,t.name text,t.bastype,t.gzcd type,d.CODE bascode ,d.TITLE bastext from t_base_company t
                    left join basdic d on t.BASTYPE=d.code and d.TYPECODE='IndustryType' where  t.name is not null ");
                DataTable dtCompany = SqlModel.Select(strSQL).Native().ExecToDataTable();

                var basList = (from p in dtCompany.AsEnumerable()
                               select new { BASCODE = p.Field<string>("BASCODE"), BASTEXT = p.Field<string>("BASTEXT") }).Distinct();
                DataRow newRow;
                foreach (var bas in basList)
                {
                    bool b = false;
                    DataRow[] companys = dtCompany.Select(" BASTYPE='" + bas.BASCODE + "'");
                    foreach (DataRow company in companys)
                    {
                        DataRow[] rows = dtPK.Select(" parentid='" + company["ID"] + "'");
                        if (rows == null || rows.Length == 0) continue;

                        newRow = dtResult.NewRow();
                        newRow["ID"] = company["ID"];
                        newRow["checked"] = company["ID"].ToString() == selectId ? true : false;
                        newRow["TEXT"] = company["TEXT"];
                        newRow["TYPE"] = company["TYPE"];
                        newRow["PARENTID"] = bas.BASCODE;
                        dtResult.Rows.Add(newRow);

                        b = true;
                        if (companyOnly == "1") continue;
                        foreach (DataRow row in rows)
                        {
                            newRow = dtResult.NewRow();
                            newRow["ID"] = row["ID"];
                            newRow["TEXT"] = row["TEXT"];
                            newRow["checked"] = row["ID"].ToString() == selectId ? true : false;
                            newRow["PARENTID"] = row["PARENTID"];
                            newRow["TYPE"] = row["TYPE"];
                            dtResult.Rows.Add(newRow);
                        }
                    }
                    if (b)
                    {
                        newRow = dtResult.NewRow();
                        newRow["ID"] = bas.BASCODE;
                        newRow["TEXT"] = bas.BASTEXT;
                        newRow["checked"] = bas.BASCODE == selectId ? true : false;
                        newRow["PARENTID"] = "-1";
                        dtResult.Rows.Add(newRow);
                    }
                }
            }
            DataRow r = dtResult.NewRow();
            r["id"] = -1;
            r["parentId"] = 0;
            r["type"] = "";
            r["text"] = dtResult.Rows.Count > 0 ? (companyOnly == "1" ? "请选择企业" : "请选择排口") : "无结果";
            dtResult.Rows.InsertAt(r, 0);

            return Json(ToDynamicComboTree(dtResult, "0", "TYPE"));

        }
        public ActionResult GetPKTree(string cType, string pType, string selectId, string companyOnly = "0")
        {
            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("ID");
            dtResult.Columns.Add("TEXT");
            dtResult.Columns.Add("checked", typeof(bool));
            dtResult.Columns.Add("PARENTID");
            dtResult.Columns.Add("TYPE");
            if (!string.IsNullOrEmpty(pType))
            {
                if (cType == "area" || cType == "bas") return GetOtherTree(cType, pType, selectId, companyOnly);

                string strSQL = string.Format("select 'P'||t.id id,t.name text,t.type,'C'||t.companyid parentid from t_base_company_pk t where  t.name is not null {0}", string.IsNullOrEmpty(pType) ? "" : " and t.type in('"
                + string.Join("','", pType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) + "')");
                DataTable dtPK = SqlModel.Select(strSQL).Native().ExecToDataTable();

                strSQL = string.Format("select 'C'||t.id id,t.name text,t.gzcd type from t_base_company t where t.name is not null {0}", string.IsNullOrEmpty(cType) ? "" : " and t.gzcd='" + cType + "'");
                List<dynamic> listCompany = SqlModel.Select(strSQL).Native().ExecToDynamicList();

                foreach (dynamic company in listCompany)
                {
                    DataRow[] rows = dtPK.Select(" parentid='" + company["ID"] + "'");
                    if (rows == null || rows.Length == 0) continue;

                    DataRow newRow = dtResult.NewRow();
                    newRow["ID"] = company["ID"];
                    newRow["checked"] = company["ID"] == selectId ? true : false;
                    newRow["TEXT"] = company["TEXT"];
                    newRow["TYPE"] = company["TYPE"];
                    newRow["PARENTID"] = "-1";
                    dtResult.Rows.Add(newRow);
                    if (companyOnly == "1") continue;
                    foreach (DataRow row in rows)
                    {
                        newRow = dtResult.NewRow();
                        newRow["ID"] = row["ID"];
                        newRow["TEXT"] = row["TEXT"];
                        newRow["checked"] = row["ID"].ToString() == selectId ? true : false;
                        newRow["PARENTID"] = row["PARENTID"];
                        newRow["TYPE"] = row["TYPE"];
                        dtResult.Rows.Add(newRow);
                    }
                }
            }
            DataRow r = dtResult.NewRow();
            r["id"] = -1;
            r["parentId"] = 0;
            r["type"] = "";
            r["text"] = dtResult.Rows.Count > 0 ? (companyOnly == "1" ? "请选择企业" : "请选择排口") : "无结果";
            dtResult.Rows.InsertAt(r, 0);

            return Json(ToDynamicComboTree(dtResult, "0", "TYPE"));
        }
        public ActionResult GetPKTreeByKeyword(string keyWord, string companyOnly = "0")
        {
            string strSQL = string.Format("select 'P'||t.id id,t.name text,t.type,'C'||t.companyid parentid from t_base_company_pk t where  t.name is not null  {0}", string.IsNullOrEmpty(keyWord) ? "" : " and t.name like '%" + keyWord + "%' ");
            DataTable dtPK = SqlModel.Select(strSQL).Native().ExecToDataTable();

            strSQL = string.Format("select 'C'||t.id id,t.name text,t.gzcd type from t_base_company t where  t.name is not null  {0}", string.IsNullOrEmpty(keyWord) ? "" : " and t.name like '%" + keyWord + "%'");
            List<dynamic> listCompany = SqlModel.Select(strSQL).Native().ExecToDynamicList();

            DataTable dtResult = new DataTable();
            dtResult.Columns.Add("ID");
            dtResult.Columns.Add("TEXT");
            dtResult.Columns.Add("checked");
            dtResult.Columns.Add("PARENTID");
            dtResult.Columns.Add("TYPE");
            foreach (dynamic company in listCompany)
            {
                DataRow[] rows = dtPK.Select(" parentid='" + company["ID"] + "'");
                if (rows == null || rows.Length == 0) continue;

                DataRow newRow = dtResult.NewRow();
                newRow["ID"] = company["ID"];
                newRow["TEXT"] = company["TEXT"];
                newRow["checked"] = false;
                newRow["TYPE"] = company["TYPE"];
                newRow["PARENTID"] = "-1";
                dtResult.Rows.Add(newRow);

                if (companyOnly == "1") continue;
                foreach (DataRow row in rows)
                {
                    newRow = dtResult.NewRow();
                    newRow["ID"] = row["ID"];
                    newRow["TEXT"] = row["TEXT"];
                    newRow["checked"] = false;
                    newRow["PARENTID"] = row["PARENTID"];
                    newRow["TYPE"] = row["TYPE"];
                    dtResult.Rows.Add(newRow);
                }
            }
            DataRow r = dtResult.NewRow();
            r["id"] = -1;
            r["parentId"] = 0;
            r["type"] = "";
            r["text"] = dtResult.Rows.Count > 0 ? (companyOnly == "1" ? "请选择企业" : "请选择排口") : "无结果";
            dtResult.Rows.InsertAt(r, 0);

            return Json(ToDynamicComboTree(dtResult, "0", "TYPE"));
        }

        public List<dynamic> ToDynamicComboTree(DataTable dataTable, string rootId, string attributes = "")
        {
            return GetDynamicComboTree(dataTable, rootId, attributes);
        }

        private List<dynamic> GetDynamicComboTree(DataTable dataTable, string parentId, string attributes = "")
        {

            List<dynamic> list = new List<dynamic>();
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var getAttributes = new Func<DataRow, string, object>((r, attrs) =>
                {
                    var datas = new Dictionary<string, object>();
                    attrs.Split(',').ToList().ForEach(d => { datas.Add(d, r[d]); });
                    return datas;
                });
                DataRow[] rows = dataTable.Select("parentId='" + parentId + "'", "id asc");
                if (rows != null && rows.Length > 0)
                {
                    rows.Each(r =>
                    {
                        list.Add(new
                        {
                            id = r["id"],
                            text = r["text"],
                            @checked = (dataTable.Columns.Contains("checked") ? r["checked"] : "false"),
                            state = dataTable.Columns.Contains("states") ? r["states"] : "open",
                            attributes = string.IsNullOrEmpty(attributes) ? null : getAttributes(r, attributes), //string.Join("\n", attributes.Split(',').Select(d => r[d]).ToArray()),
                            children = GetDynamicComboTree(dataTable, r["id"].ToString(), attributes)
                        });
                    });
                }
            }
            return list;
        }
    }
}
