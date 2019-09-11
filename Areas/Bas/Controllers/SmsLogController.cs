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
    public class SmsLogController : ListController
    {
        //
        // GET: /Bas/SmsTemplate/

        protected override SqlModel GetSqlModel(RequestData data)
        {
            FieldModel where = null;
            string SMSTEMPTYPE = data.Get("TEMPTYPE").Trim();

            if (SMSTEMPTYPE != "")
            {
                where &= SMSTEMPLATE.SMSTEMPTYPE == SMSTEMPTYPE;
            }

            DateTime dtNow = DateTime.Now;
            DateTime tmStart = new DateTime(dtNow.Year, dtNow.Month, 1);
            DateTime tmEnd = dtNow.Date;
            if (!string.IsNullOrEmpty(data.Get("opBegDate"))) DateTime.TryParse(data.Get("opBegDate"), out tmStart);
            if (!string.IsNullOrEmpty(data.Get("opEndDate"))) DateTime.TryParse(data.Get("opEndDate"), out tmEnd);

            tmEnd = tmEnd.AddHours(23).AddMinutes(59).AddSeconds(59);
            where &= SMSLOG.SENDTIME.BetweenAnd(tmStart, tmEnd);

            SqlModel sqlmode = SqlModel.SelectAll(
                BASDIC.TITLE.As("SMSTEMPTYPE_TEXT"),
            "u".Field(BASUSER.TRUENAME).ListAgg("u".Field(BASUSER.TRUENAME).ToChar(), ',').As("USERNAME_TEXT")
            )
            .From(DB.SMSLOG)
            .LeftJoin(DB.BASDIC).On(SMSLOG.SMSTEMPTYPE == BASDIC.CODE & BASDIC.TYPECODE == "SMSTEMPTYPE")
            .LeftJoin(DB.BASUSER.As("u")).On(SMSLOG.RECEIVEUSER.Instr("u".Field(BASUSER.MOBILE).ToChar(), ','))
            .Where(where).OrderByDesc(SMSLOG.SENDTIME)
            .GroupBy(
            SMSLOG.ID,
            SMSLOG.CONTENT,
            SMSLOG.STATUS,
            SMSLOG.SENDTIME,
            SMSLOG.RECEIVEUSER,
            SMSLOG.SMSTEMPTYPE,
            SMSLOG.SMSBASTYPE,
            BASDIC.TITLE,
            "u".Field(BASUSER.TRUENAME)
            );
            if (!string.IsNullOrEmpty(data.Get("USERNAME_TEXT")))
            {
                FieldModel field = new FieldModel();
                field.Name = "SEND_PERSON_TEXT";
                field.TableName = "t";

                sqlmode = SqlModel.Select(new FieldModel[] {
                    new FieldModel() {  Name=SMSLOG.ID.Name, TableName="t"},
                    new FieldModel() {  Name=SMSLOG.CONTENT.Name, TableName="t"},
                    new FieldModel() {  Name=SMSLOG.STATUS.Name, TableName="t"},
                    new FieldModel() {  Name=SMSLOG.SENDTIME.Name, TableName="t"},
                    new FieldModel() {  Name=SMSLOG.RECEIVEUSER.Name, TableName="t"},
                    new FieldModel() {  Name=SMSLOG.SMSTEMPTYPE.Name, TableName="t"},
                    new FieldModel() {  Name="USERNAME_TEXT", TableName="t"}
                }).From(sqlmode.As("t")).Where(field.Like(data.Get("USERNAME_TEXT")));
            }

            return sqlmode;
        }

        protected override bool DoAdd(RequestData data)
        {
            return false;
        }

        protected override bool DoDelete(RequestData data)
        {
            return false;
        }

        protected override bool DoEdit(RequestData data)
        {
            return false;
        }

        public ActionResult SendSmsRetry(RequestData data)
        {
            bool result = false;
            dynamic log = SqlModel.SelectAll().From(DB.SMSLOG).Where(SMSLOG.ID == data.Get("id")).ExecToDynamic();
            if (log != null)
            {

                SMSTOSENDModel model = new SMSTOSENDModel();
                model.CONTENT = log["CONTENT"];
                model.CREATETIME = DateTime.Now;
                model.CREATEUSER = UI.Web.CurrentUser.UserName;
                model.RECEIVEUSER = log["RECEIVEUSER"];
                model.SMSBASTYPE = "";
                model.SMSTEMPTYPE = log["SMSTEMPTYPE"];
                model.STATUS = "0";
                result = model.Insert();

            }
            return Json(new
            {
                Success = result
            }
            );
        }
    }
}
