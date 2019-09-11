using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.Monitor.Controllers
{
    public class CheckStatisticController : ListController
    {
        //
        // GET: /Monitor/CheckRecord/
        public override ActionResult ListExt(RequestData data)
        {
            ViewData["PKId"] = data.Get("PKId");// args[0];// PKId;
            ViewData["stime"] = data.Get("stime");
            ViewData["etime"] = data.Get("etime");
            ViewData["fs"] = data.Get("fs");
            ViewData["fq"] = data.Get("fq");
            ViewData["vocs"] = data.Get("vocs");
            return base.ListExt(data);
        }
        protected override bool DoAdd(RequestData data)
        {
            throw new NotImplementedException();
        }

        protected override bool DoDelete(RequestData data)
        {
            throw new NotImplementedException();
        }

        protected override bool DoEdit(RequestData data)
        {
            throw new NotImplementedException();
        }

        public ActionResult GetData(RequestData data)
        {
            DateTime stime = data.GetDateTime("stime");
            DateTime etime = data.GetDateTime("etime");
            etime = etime.AddDays(1).AddSeconds(-1);
            string pkIds = data.Get("PKId");
            FieldModel where = T_MID_CHECKRECORD.CHECKTIME.BetweenAnd(stime, etime);
            //if (!string.IsNullOrEmpty(pkIds))
            {
                where &= T_BASE_COMPANY_PK.ID.In("'" + string.Join("','", pkIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) + "'");
            }
            DataTable dtResult = SqlModel.Select(T_BASE_COMPANY.NAME.As("COMPANY_NAME"), T_BASE_COMPANY_PK.NAME.As("PK_NAME"), T_BASE_COMPANY_PK.ID)
                .From(DB.T_BASE_COMPANY_PK)
                .LeftJoin(DB.T_BASE_COMPANY).On(T_BASE_COMPANY.ID == T_BASE_COMPANY_PK.COMPANYID)
                .Where(T_BASE_COMPANY_PK.ID.In("'" + string.Join("','", pkIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) + "'"))
                .ExecToDataTable();
            DataTable dtData = SqlModel.Select(T_BASE_COMPANY.NAME.As("COMPANYNAME"), T_BASE_COMPANY_PK.ID, T_BASE_COMPANY_PK.NAME.As("PKNAME"),
                 T_MID_CHECKRECORD.CHECKTIME.ToCharDateSelect("yyyy-mm-dd").As("CHECKTIME"), T_MID_CHECKRECORD.ID.CountAs("COUNT"))
                .From(DB.T_MID_CHECKRECORD)
                .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK_TX.MN == T_MID_CHECKRECORD.MN)
                .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                .LeftJoin(DB.T_BASE_COMPANY).On(T_BASE_COMPANY.ID == T_BASE_COMPANY_PK.COMPANYID)
                .LeftJoin(DB.BASUSER).On(BASUSER.USERNAME == T_MID_CHECKRECORD.CHECKUSER)
                .Where(where)
                .GroupBy(T_BASE_COMPANY.ID, T_BASE_COMPANY.NAME, T_BASE_COMPANY_PK.ID, T_BASE_COMPANY_PK.NAME, T_MID_CHECKRECORD.CHECKTIME.ToCharDate("yyyy-mm-dd"))
                .ExecToDataTable();

            for (DateTime i = stime; i <= etime; i = i.AddDays(1))
            {
                dtResult.Columns.Add(i.ToString("yyyy-MM-dd"));

                for (int j = 0; j < dtResult.Rows.Count; j++)
                {
                    DataRow[] rows = dtData.Select(" id=" + dtResult.Rows[j]["ID"] + " AND CHECKTIME='" + i.ToString("yyyy-MM-dd") + "'");
                    if (rows.Length == 0) continue;
                    dtResult.Rows[j][i.ToString("yyyy-MM-dd")] = rows[0]["COUNT"];
                }
            }

            return Json(new
            {
                total = dtResult.Rows.Count,
                rows = dtResult.ToDynamicList()
            });
        }

        protected override SqlModel GetSqlModel(RequestData data)
        {
            return null;
        }
    }
}
