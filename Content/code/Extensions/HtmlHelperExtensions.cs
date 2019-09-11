using w.ORM;
using UI.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w.Model;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        public static string ToolBar(this HtmlHelper html, int navId)
        {
            return GetPageButtonHtml(navId, CurrentUser.Id);
        }

        private static string GetPageButtonHtml(int navId, int userId)
        {
            object ret = SqlModel.Select(BASUSER.ISADMIN).From(DB.BASUSER).Where(BASUSER.ID == userId).ExecuteScalar();
            FieldModel where = BASBUTTON.BUTTONTAG != "browser";
            if (ret == null)
            {
                return "";
            }
            else if (ret.ToBoolean() == true)
            {
                where &= BASNAVBTN.NAVID == navId;
            }
            else
            {
                DataTable rTab = GetRightButtonTable(navId, userId);
                string btnIdStr = rTab.Join((r, i) => (i > 0 ? "," : "") + r[BASUSERNAVBTN.BTNID.Name]);
                where &= BASNAVBTN.NAVID == navId & BASBUTTON.ID.In(btnIdStr);
            }
            DataTable btnTab = SqlModel.Select(BASBUTTON.BUTTONHTML, BASBUTTON.BUTTONHTMLEN)
                .From(DB.BASNAVBTN)
                .InnerJoin(DB.BASBUTTON).On(BASBUTTON.ID == BASNAVBTN.BTNID).OrderByAsc(BASNAVBTN.SORTNO)
                .Where(where).ExecToDataTable();
            string split = @"<div class='datagrid-btn-separator'></div>";
            List<string> btnList = new List<string>();
            btnTab.Each(r => btnList.Add(LangHelper.Name == LangEnum.ZHCN.ToString().ToLower() ?
                   r[BASBUTTON.BUTTONHTML.Name].ToString() : r[BASBUTTON.BUTTONHTMLEN.Name].ToString()));
            return string.Join(split, btnList.ToArray());
        }

        private static DataTable GetRightButtonTable(int navId, int userId)
        {
            DataTable urTab = SqlModel.Select(BASUSERROLE.ROLEID).From(DB.BASUSERROLE)
                .Where(BASUSERROLE.USERID == userId).ExecToDataTable();
            string roleIdStr = urTab.Join((r, i) => (i > 0 ? "," : "") + r[BASUSERROLE.ROLEID.Name]);
            if (string.IsNullOrEmpty(roleIdStr) == true)
            {
                roleIdStr = "0";
            }
            DataTable rTab = SqlModel.Select("T".Field(BASROLENAVBTN.BTNID))
            .From
            (
                (SqlModel.Select(BASROLENAVBTN.BTNID).From(DB.BASROLENAVBTN).Where(BASROLENAVBTN.NAVID == navId & BASROLENAVBTN.ROLEID.In(roleIdStr)))
                .Union
                (SqlModel.Select(BASUSERNAVBTN.BTNID).From(DB.BASUSERNAVBTN).Where(BASUSERNAVBTN.NAVID == navId & BASUSERNAVBTN.USERID == userId))
                .As("T")
            ).ExecToDataTable();
            return rTab;
        }

        public static string LinkButton(this HtmlHelper html, string id, string icon, string text)
        {
            return LinkButton(html, id, icon, text, false, true);
        }

        public static string LinkButton(this HtmlHelper html, string id, string icon, string text, bool showText)
        {
            return LinkButton(html, id, icon, text, false, showText);
        }

        public static string LinkButton(this HtmlHelper html, string id, string icon, string text, bool showLeftSeparator, bool showText)
        {
            return string.Format("{4}<a id=\"{0}\" {5} href=\"javascript:void(0);\" plain=\"true\" class=\"easyui-linkbutton\" icon=\"{1}\" title=\"{2}\">{3}</a>",
                id, icon, text, showText ? text : "", showLeftSeparator ? "<div class=\"datagrid-btn-separator\"></div>" : "", showLeftSeparator ? "style=\"float:left\"" : "");
        }


    }
}