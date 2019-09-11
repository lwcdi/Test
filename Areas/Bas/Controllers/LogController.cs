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
    public class LogController : ListController
    {
        protected override SqlModel GetSqlModel(RequestData data)
        {
            FieldModel where = BASLOG.LOGCONTENT.Like(data.Get("LogContent"));
            string logType = data.Get("LogType").Trim();
            string opBegDate = data.Get("opBegDate").Trim();
            string opEndDate = data.Get("opEndDate").Trim();
            if (logType != "")
            {
                where &= BASLOG.LOGTYPECODE == logType;
            }
            if (opBegDate != "")
            {
                where &= BASLOG.OPERATETIME.ToCharDate() >= opBegDate;
            }
            if (opEndDate != "")
            {
                where &= BASLOG.OPERATETIME.ToCharDate() <= opEndDate + base.LastTime;
            }
            return SqlModel.SelectAll(BASDIC.TITLE.As("LogTypeName"), BASUSER.TRUENAME.As("OperateUserName"))
                .From(DB.BASLOG)
                .LeftJoin(DB.BASDIC).On(BASDIC.CODE == BASLOG.LOGTYPECODE)
                .LeftJoin(DB.BASUSER).On(BASUSER.ID == BASLOG.OPERATEUSERID)
                .Where(where);
        }

        protected override bool DoAdd(RequestData data)
        {
            return true;
        }

        protected override bool DoEdit(RequestData data)
        {
            return true;
        }

        protected override bool DoDelete(RequestData data)
        {
            return true;
        }

        [HttpPost]
        public ActionResult GetLogTypeData(RequestData data)
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .LeftJoin(DB.BASDICTYPE).On(BASDICTYPE.ID == BASDIC.TYPEID)
                .Where(BASDICTYPE.CODE == ConstStrings.LogType).ExecToDataTable();
            return Json(dt.AddNoneSelectItem(" ", "全部").ToDynamicList());
        }
    }
}
