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
    public class CheckRecordController : ListController
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
            ViewData["companyOnly"] = data.Get("companyOnly");
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

            return SqlModel.Select(T_BASE_COMPANY.NAME.As("COMPANYNAME"), T_BASE_COMPANY_PK.NAME.As("PKNAME"), ("D1").Field("TITLE").As("ITEMTEXT"), ("D2").Field("TITLE").As("SUBITEMTEXT")
                , T_MID_CHECKRECORD.RECTIME, T_MID_CHECKRECORD.VALUE, T_MID_CHECKRECORD.CHECKVALUE, T_MID_CHECKRECORD.CHECKTIME, ("D3").Field("TITLE").As("CHECKTYPETEXT")
                , BASUSER.TRUENAME.As("CHECKUSERNAME"), T_MID_CHECKRECORD.FJ, T_MID_CHECKRECORD.REMARK)
                .From(DB.T_MID_CHECKRECORD)
                .LeftJoin(DB.T_BASE_COMPANY_PK_TX).On(T_BASE_COMPANY_PK_TX.MN == T_MID_CHECKRECORD.MN)
                .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                .LeftJoin(DB.T_BASE_COMPANY).On(T_BASE_COMPANY.ID == T_BASE_COMPANY_PK.COMPANYID)
                .LeftJoin(DB.BASDIC.As("D1")).On(("D1").Field("CODE") == T_MID_CHECKRECORD.ITEMCODE)
                .LeftJoin(DB.BASDIC.As("D2")).On(("D2").Field("CODE") == T_MID_CHECKRECORD.SUBITEMCODE & ("D2").Field("TYPECODE") == T_MID_CHECKRECORD.ITEMCODE)
                .LeftJoin(DB.BASDIC.As("D3")).On(("D3").Field("CODE") == T_MID_CHECKRECORD.CHECKTYPE & ("D3").Field("TYPECODE") == "CHECKTYPE")
                .LeftJoin(DB.BASUSER).On(BASUSER.USERNAME == T_MID_CHECKRECORD.CHECKUSER)
                .Where(where);
        }

    }
}
