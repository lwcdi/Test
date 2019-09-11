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
    public class CheckRecordFJController : ListController
    {
        //
        // GET: /Monitor/CheckRecord/
        public override ActionResult ListExt(RequestData data)
        {
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

        protected override SqlModel GetSqlModel(RequestData data)
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

            return SqlModel.Select(T_BASE_COMPANY.NAME.As("COMPANYNAME"), T_BASE_COMPANY_PK.ID, T_BASE_COMPANY_PK.NAME.As("PKNAME"),
                 T_MID_CHECKRECORD.CHECKTIME.ToCharDateSelect("yyyy-mm-dd").As("CHECKTIME"), T_MID_CHECKRECORD.ID.CountAs("COUNT"))
                .From(DB.T_MID_CHECKRECORD)
                .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK_TX.MN == T_MID_CHECKRECORD.MN)
                .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                .LeftJoin(DB.T_BASE_COMPANY).On(T_BASE_COMPANY.ID == T_BASE_COMPANY_PK.COMPANYID)
                .LeftJoin(DB.BASUSER).On(BASUSER.USERNAME == T_MID_CHECKRECORD.CHECKUSER)
                .Where(where)
                .GroupBy(T_BASE_COMPANY.ID, T_BASE_COMPANY.NAME, T_BASE_COMPANY_PK.ID, T_BASE_COMPANY_PK.NAME, T_MID_CHECKRECORD.CHECKTIME.ToCharDate("yyyy-mm-dd"));
        }
    }
}
