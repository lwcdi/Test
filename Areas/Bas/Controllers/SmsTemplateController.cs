using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.Bas.Controllers
{
    public class SmsTemplateController : ListController
    {
        //
        // GET: /Bas/SmsTemplate/

        protected override SqlModel GetSqlModel(RequestData data)
        {
            FieldModel where = null;
            string TEMPNAME = data.Get("NAME").Trim();
            string SMSTEMPTYPE = data.Get("TEMPTYPE").Trim();
            //string SMSBASTYPE = data.Get("BASTYPE").Trim();

            string opBegDate = data.Get("opBegDate").Trim();
            string opEndDate = data.Get("opEndDate").Trim();

            if (TEMPNAME != "")
            {
                where &= SMSTEMPLATE.TEMPNAME.Like(TEMPNAME);
            }
            if (SMSTEMPTYPE != "")
            {
                where &= SMSTEMPLATE.SMSTEMPTYPE == SMSTEMPTYPE;
            }
            //if (SMSBASTYPE != "")
            //{
            //    where &= SMSTEMPLATE.SMSBASTYPE == SMSBASTYPE;
            //}
            SqlModel SqlModel = SqlModel.SelectAll(BASDIC.TITLE.As("SMSTEMPTYPE_TEXT"), "D1".Field("TITLE").As("SMSBASTYPE_TEXT"), "T1".Field("TRUENAME").As("CREATENAMEREAL"))
                                        .From(DB.SMSTEMPLATE)
                                        .LeftJoin(DB.BASDIC).On(SMSTEMPLATE.SMSTEMPTYPE == BASDIC.CODE & BASDIC.TYPECODE == "SMSTEMPTYPE")
                                        .LeftJoin(DB.BASDIC.As("D1")).On(SMSTEMPLATE.SMSBASTYPE == "D1".Field("CODE") & "D1".Field("TYPECODE") == "SMSBASTYPE")
                                        .LeftJoin(DB.BASUSER.As("T1")).On(SMSTEMPLATE.CREATEUSER == "T1".Field("USERNAME"))
                                        .Where(where).OrderByDesc(SMSTEMPLATE.CREATETIME);
            return SqlModel;
        }

        protected override bool DoAdd(RequestData data)
        {
            SMSTEMPLATEModel model = new SMSTEMPLATEModel();
            model = this.GetModelValue<SMSTEMPLATEModel>(model, data);
            model.SMSBASTYPE = "0";
            model.CREATETIME = DateTime.Now;
            model.CREATEUSER = CurrentUser.UserName;
            if (model.ISUSE == null) model.ISUSE = "0";
            else model.ISUSE = "1";
            return model.Insert();
        }

        protected override bool DoDelete(RequestData data)
        {
            SMSTEMPLATEModel model = new SMSTEMPLATEModel();
            return model.Update(SMSTEMPLATE.ID == data.Get("id"));
        }

        protected override bool DoEdit(RequestData data)
        {
            SMSTEMPLATEModel model = new SMSTEMPLATEModel();
            model = this.GetModelValue<SMSTEMPLATEModel>(model, data);
            model.SMSBASTYPE = "0";
            if (model.ISUSE == null) model.ISUSE = "0";
            else model.ISUSE = "1";
            return model.Update(SMSTEMPLATE.ID == data.Get("id"));
        }

        public bool AlterUsed(RequestData data)
        {
            SMSTEMPLATEModel model = new SMSTEMPLATEModel();
            string isused = data.Get("isused");
            model.ISUSE = isused == "checked" ? "1" : "0";
            return model.Update(SMSTEMPLATE.ID == data.Get("id"));
        }
    }
}
