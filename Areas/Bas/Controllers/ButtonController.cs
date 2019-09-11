
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
    public class ButtonController : ListController
    {
        private const string ButtonHtmlFormart = "<a id=\"a_{0}\" style=\"float:left\" href=\"javascript:void(0);\" plain=\"true\" class=\"easyui-linkbutton\" icon=\"{1}\" title=\"{2}\">{2}</a>";
        
        protected override SqlModel GetSqlModel(RequestData data)
        {
            return SqlModel.Select(BASBUTTON.ID, BASBUTTON.BUTTONTEXT, BASBUTTON.BUTTONTEXTEN,
                    BASBUTTON.SORTNO, BASBUTTON.ICONCLS, BASBUTTON.BUTTONTAG, BASBUTTON.REMARK)
                .From(DB.BASBUTTON);
        }

        protected override void AfterSave(ActionType actionType, RequestData data)
        {
            //if (actionType == ActionType.Add)
            //{
            //    base.AddLog(ActionType.Add, new
            //    {

            //    });
            //}
        }

        protected override bool DoAdd(RequestData data)
        {
            BASBUTTONModel model = new BASBUTTONModel();
            model.BUTTONTEXT = data.Get("ButtonText");
            model.BUTTONTEXTEN = data.Get("ButtonTextEN");
            model.SORTNO = data.Get("SortNo").ToInt32();
            model.ICONCLS = data.Get("IconCls");
            model.BUTTONTAG = data.Get("ButtonTag");
            model.REMARK = data.Get("Remark");
            model.BUTTONHTML = string.Format(ButtonHtmlFormart, model.BUTTONTAG, model.ICONCLS, model.BUTTONTEXT);
            model.BUTTONHTMLEN = string.Format(ButtonHtmlFormart, model.BUTTONTAG, model.ICONCLS, model.BUTTONTEXTEN);
            return model.Insert();
        }

        protected override bool DoEdit(RequestData data)
        {
            BASBUTTONModel model = new BASBUTTONModel();
            model.BUTTONTEXT = data.Get("ButtonText");
            model.BUTTONTEXTEN = data.Get("ButtonTextEN");
            model.SORTNO = data.Get("SortNo").ToInt32();
            model.ICONCLS = data.Get("IconCls");
            model.BUTTONTAG = data.Get("ButtonTag");
            model.REMARK = data.Get("Remark");
            model.BUTTONHTML = string.Format(ButtonHtmlFormart, model.BUTTONTAG, model.ICONCLS, model.BUTTONTEXT);
            model.BUTTONHTMLEN = string.Format(ButtonHtmlFormart, model.BUTTONTAG, model.ICONCLS, model.BUTTONTEXTEN);
            return model.Update(BASBUTTON.ID == data.Get("id"));
        }

        protected override bool DoDelete(RequestData data)
        {
            BASBUTTONModel model = new BASBUTTONModel();
            return model.Delete(BASBUTTON.ID == data.Get("id"));
        }

        public ActionResult GetButtonTable(RequestData data)
        {
            List<dynamic> list = SqlModel.SelectAll().From(DB.BASBUTTON)
                .OrderByAsc(BASBUTTON.SORTNO).ExecToDynamicList();
            return Json(list);
        }

        public ActionResult GetBtnColumns(RequestData data)
        {
            DataTableModel dt = SqlModel.Select(BASBUTTON.BUTTONTEXT, BASBUTTON.BUTTONTAG)
                .From(DB.BASBUTTON).OrderByAsc(BASBUTTON.SORTNO).ExecToTableModel();
            List<dynamic> list = new List<dynamic>();
            if (dt != null && dt.Count > 0)
            {
                foreach (DataRowModel r in dt)
                {
                    list.Add(new
                    {
                        title = r[BASBUTTON.BUTTONTEXT].ToString(),
                        field = r[BASBUTTON.BUTTONTAG].ToString(),
                        width = 60,
                        align = "center",
                        editor = new
                        {
                            type = "checkbox",
                            options = new
                            {
                                on = "√",
                                off = "x"
                            }
                        },
                    });
                }
            }
            string jsonStr = string.Format("var btns = {0}", Newtonsoft.Json.JsonConvert.SerializeObject(list));
            return Content(jsonStr);
        }
    }
}
