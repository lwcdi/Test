using w.ORM;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using w.Model;

namespace UI.Web.Areas.Bas.Controllers
{
    public class HomeController : BaseController
    {
        //系统首页
        public ActionResult Index()
        {
            ViewData["Content"] = this.GetContent();
            return View();
        }

        //首页标签页
        public ActionResult Default()
        {
            return View();
        }

        //首页标签页
        public ActionResult ConsoleIndex()
        {
            return View();
        }

        private string GetContent()
        {
            string content = "";
            string configData = CurrentUser.ConfigJson;

            string themePath = Server.MapPath("~/content/theme/navtype/");
            NVelocityHelper vel = new NVelocityHelper(themePath);
            vel.Put("welcome", Lang.welcome);
            vel.Put("username", CurrentUser.UserTrueName);
            vel.Put("update_pwd", Lang.update_pwd);
            vel.Put("logout", Lang.logout);
            vel.Put("system_name", Lang.system_name);
            vel.Put("nav_menu", Lang.nav_menu);
            string navHTML = "Accordion.html";
            if (!string.IsNullOrEmpty(configData))
            {
                ConfigModel sysconfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigModel>(configData);
                if (sysconfig != null)
                {

                    switch (sysconfig.ShowType)
                    {
                        case "menubutton":
                            navHTML = "menubutton.html";
                            break;
                        case "tree":
                            navHTML = "tree.html";
                            break;
                        case "menuAccordion":
                        case "menuAccordion2":
                        case "menuAccordionTree":
                            navHTML = "topandleft.html";
                            break;
                        default:
                            navHTML = "Accordion.html";
                            break;
                    }

                }
            }

            content = vel.FileToString(navHTML);
            return content;
        }

        public ActionResult GetMenuData()
        {
            string menuJSON = "var menus = -1;";
            if (CurrentUser.IsExpired == false)
            {
                var userName = CurrentUser.UserName;
                string jsonStr = this.GetNavJson(CurrentUser.Id);
                if (jsonStr.Trim() == "")
                {
                    jsonStr = "[]";
                }
                menuJSON = "var menus = " + jsonStr;
            }
            return Content(menuJSON);
        }

        private string GetNavJson(int userId)
        {
            object ret = SqlModel.Select(BASUSER.ISADMIN).From(DB.BASUSER).Where(BASUSER.ID == userId).ExecuteScalar();
            bool isAdmin = ret != null && ret.ToBoolean();
            DataTable navTab = SqlModel.SelectAll().From(DB.BASNAV)
                .Where(BASNAV.ISVISIBLE == 1).ExecToDataTable();
            DataTable urTab = SqlModel.Select(BASUSERROLE.ROLEID).From(DB.BASUSERROLE).Where(BASUSERROLE.USERID == userId).ExecToDataTable();
            string roleIdStr = urTab.Join((r, i) => (i > 0 ? "," : "") + r[BASUSERROLE.ROLEID.Name]);
            DataTable rTable = base.GetRightTable(roleIdStr, userId);
            JArray list = this.GetNavList(isAdmin, navTab, rTable, 0);
            return list.ToString();
        }

        private JArray GetNavList(bool isAdmin, DataTable navTab, DataTable rTable, int parentId)
        {
            JArray list = new JArray();
            DataRow[] navRows = navTab.Select("ParentID=" + parentId, "SortNo asc");
            DataRow[] rRows = null;
            navRows.Each(r =>
            {
                int navId = r[BASNAV.ID.Name].ToInt32();
                rRows = rTable.Select("NavId=" + navId);
                if (isAdmin || (rRows != null && rRows.Length > 0))
                {
                    JObject obj = new JObject
                    (
                        new JProperty("id", navId),
                         new JProperty("text", LangHelper.Name == LangEnum.ZHCN.ToString().ToLower() ?
                            r[BASNAV.NAVTITLE.Name].ToString() : r[BASNAV.NAVTITLEEN.Name].ToString()),
                         new JProperty("iconCls", r[BASNAV.ICONCLS.Name].ToString()),
                         new JProperty
                            ("attributes", new JObject
                                (
                                    new JProperty("url", r[BASNAV.LINKURL.Name].ToString()),
                                     new JProperty("iconUrl", r[BASNAV.ICONURL.Name].ToString()),
                                     new JProperty("parentid", r[BASNAV.PARENTID.Name].ToInt32()),
                                    new JProperty(" SortNo", r[BASNAV.SORTNO.Name].ToInt32()),
                                     new JProperty("BigImageUrl", r[BASNAV.BIGIMAGEURL.Name].ToString())
                                )
                            ),
                         new JProperty("children", this.GetNavList(isAdmin, navTab, rTable, navId))
                    );
                    list.Add(obj);
                }
            });
            return list;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(RequestData data)
        {
            LangHelper.Name = "zhcn";
            if (string.IsNullOrEmpty(data.Get("lang")) == false)
            {
                LangHelper.Name = data.Get("lang");
            }
            LangHelper.Instance.Read(base.Request, true);

            base.Response.ContentType = "text/plain";
            var userName = data.Get("username");
            var password = data.Get("password");
            var validateCode = data.Get("validateCode");
            var saveCookieDays = data.GetInt("savedays");

            var msg = new { success = false, message = "用户名不存在！" };

            SqlModel model = SqlModel.SelectAll().From(DB.BASUSER).Where(BASUSER.USERNAME == userName);

            DataRowModel uModel = model.ExecToRowModel();
            if (uModel != null)
            {
                if (uModel[BASUSER.ISDISABLED].ToInt32() == 1)
                {
                    bool flag = CurrentUser.Login(userName, password);
                    if (flag == true)
                    {
                        msg = new { success = true, message = "登录成功！" };
                        DataInitor.Log = new LogData();
                        base.AddLog(LogType.UserLogin, "用户登录成功");
                    }
                    else
                    {
                        msg = new { success = false, message = "用户名或密码输入错误！" };
                    }
                }
                else
                {
                    msg = new { success = false, message = "用户已经被禁用，请联系管理员！" };
                }
            }
            return Json(msg);
        }

        public ActionResult Logout()
        {
            base.AddLog(LogType.UserLogout, "-");
            CurrentUser.Logout();
            return Redirect("/Bas/Home/Login");
        }

        public ActionResult LoginCheck()
        {
            return View();
        }

        #region 控制台功能
        public string GetSaleOrderChart(RequestData data)
        {
            //string createdDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            //SqlModel model = SqlModel.Select(TblSaleOrder.CreatedDate.Convert(DataType.Nvarchar, 23).As("data"), TblSaleOrder.ID.CountAs("qty"))
            //    .From(WZDB.TblSaleOrder)
            //      .Where(TblSaleOrder.DeleteFlg != base.DeleteFlag & TblSaleOrder.CreatedDate > createdDate)
            //      .GroupBy(TblSaleOrder.CreatedDate.Convert(DataType.Nvarchar, 23))
            //      .OrderByDesc(TblSaleOrder.CreatedDate.Convert(DataType.Nvarchar, 23));
            //List<dynamic> list = model.ExecToDynamicList();
            //Dictionary<string, dynamic> dic = new Dictionary<string, dynamic>();

            //List<string> xAxisdata = new List<string>();
            //List<int> seriesdata = new List<int>();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    xAxisdata.Add(list[i]["data"]);
            //    seriesdata.Add(list[i]["qty"]);
            //}
            //dic.Add("xAxisdata", xAxisdata);
            //dic.Add("seriesdata", seriesdata);
            //return JsonConvert.SerializeObject(dic);
            return "";
        }

        public string GetOrderInfo(RequestData data)
        {
            //Dictionary<string, dynamic> dic = new Dictionary<string, dynamic>();
            //object NotAuditedQty = 0, NotNoticeQty = 0, NotDistributionQty = 0;

            //object ret = SqlModel.Select(TblSaleOrder.ID.Count()).From(WZDB.TblSaleOrder).
            //    Where(TblSaleOrder.ApproveFlgID == (int)ApproveStatus.Approving)
            //    .ExecuteScalar();
            //if (ret != null && ret.ToString() != "" && ret.ToInt32() > 0)
            //    NotAuditedQty = ret;
            //dic.Add("NotAuditedQty", ret);
            //ret = 0;

            //ret = SqlModel.Select(TblSaleOrder.ID.Count()).From(WZDB.TblSaleOrder).
            //  Where(TblSaleOrder.ApproveFlgID == (int)ApproveStatus.Approved & TblSaleOrder.NoticeStatusID == (int)NoticeStatus.UnNotice)
            //  .ExecuteScalar();
            //if (ret != null && ret.ToString() != "" && ret.ToInt32() > 0)
            //    NotNoticeQty = ret;
            //dic.Add("NotNoticeQty", NotNoticeQty);
            //ret = 0;

            //ret = SqlModel.Select(TblDeliveryNotice.ID.Count()).From(WZDB.TblDeliveryNotice).
            // Where(TblDeliveryNotice.DistributionStatus == (int)DistributionStatus.UnDistribute)
            // .ExecuteScalar();
            //if (ret != null && ret.ToString() != "" && ret.ToInt32() > 0)
            //    NotDistributionQty = ret;
            //dic.Add("NotDistributionQty", NotDistributionQty);

            //return JsonConvert.SerializeObject(dic);
            return "";
        }
        #endregion
        /// <summary>
        /// 获取系统信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSysData()
        {
            var dic = new Dictionary<string, object>();
            dic.Add("time", SqlModel.Select().Native().DataBaseTime);
            dic.Add("username", CurrentUser.UserName);
            dic.Add("usertruename", CurrentUser.UserTrueName);
            return Json(dic);
        }

    }
}
