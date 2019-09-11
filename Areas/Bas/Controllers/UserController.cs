using w.ORM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;

namespace UI.Web.Areas.Bas.Controllers
{
    public class UserController : ListController
    {
        protected override SqlModel GetSqlModel(RequestData data)
        {
            FieldModel where = null;
            string areaCode = data.Get("AreaCode");
            if (string.IsNullOrEmpty(areaCode) == false && areaCode.ToInt32() > 0 && areaCode != CurrentUser.SysAreaCode)
            {
                where = BASUSER.AREACODE == areaCode;
            }

            List<dynamic> filterRuleslist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(data.Get("filterRules"));
            if (filterRuleslist != null)
            {
                foreach (var item in filterRuleslist)
                {
                    this.SetWhere(item, BASUSER.ID.Name, BASUSER.ID, ref where);
                    this.SetWhere(item, BASUSER.USERNAME.Name, BASUSER.USERNAME, ref where);
                    this.SetWhere(item, BASUSER.TRUENAME.Name, BASUSER.TRUENAME, ref where);
                    this.SetWhere(item, BASORG.ORGNAME.Name, BASORG.ORGNAME, ref where);
                    this.SetWhere(item, "UserTypeName", BASDIC.TITLE, ref where);
                    this.SetWhere(item, BASUSER.REMARK.Name, BASUSER.REMARK, ref where);
                }
            }


            return SqlModel.SelectAll(BASDIC.TITLE.As("UserTypeName"), T_SYS_AREA.AREA_TEXT.As("AREATEXT"), BASORG.ORGNAME)
                .From(DB.BASUSER)
                .LeftJoin(DB.BASORG).On(BASUSER.ORGID == BASORG.ID)
                //.LeftJoin(DB.T_COMPANY_INFO).On(BASUSER.TSNO == T_COMPANY_INFO.TSNO)
                .LeftJoin(DB.BASDIC).On(BASDIC.CODE == BASUSER.USERTYPEID & BASDIC.TYPECODE == ConstStrings.PowerType)
                .LeftJoin(DB.T_SYS_AREA).On(T_SYS_AREA.AREA_CODE == BASUSER.AREACODE)
                .Where(where).OrderByAsc(BASUSER.ID);
        }

        protected override string BeforeSave(ActionType actionType, RequestData data)
        {
            string userName = data.Get("UserName");
            if (actionType == ActionType.Add)
            {
                object ret = SqlModel.Select(BASUSER.ID).From(DB.BASUSER)
                    .Where(BASUSER.USERNAME == userName).ExecuteScalar();
                if (ret != null && ret.ToString() != "")
                    return string.Format("[{0}]{1}", userName, Lang.bas_UserNameExist);
            }
            if (actionType == ActionType.Delete)
            {
                if (userName == "admin")
                {
                    return "系统内置帐号不能删除!";
                }
            }
            return "";
        }

        protected override bool DoAdd(RequestData data)
        {
            BASUSERModel model = new BASUSERModel();
            model.USERNAME = data.Get("UserName");
            model.TRUENAME = data.Get("TrueName");
            string pwd = data.Get("Password");
            model.PASSWORD = StringHelper.MD5String(pwd);
            model.USERTYPEID = data.GetInt("UserTypeId");
            model.AREACODE = data.Get("AreaCode");
            model.ORGID = data.Get("OrgId");
            if (data.Get("IsAdmin") != null)
            {
                model.ISADMIN = data.GetBoolean("IsAdmin").ToInt32();
            }
            else
            {
                model.ISADMIN = 0;
            }
            if (data.Get("IsDisabled") != null)
            {
                model.ISDISABLED = data.GetBoolean("IsDisabled").ToInt32();
            }
            else
            {
                model.ISDISABLED = 0;
            }
            //if (data.Get("isvisible") != "")
            //{
            //    model.ISVOSHEAD = data.Get("isvisible").ToInt32();
            //}
            //else
            //{
            //    model.ISVOSHEAD = 0;
            //}
            //if (data.Get("chkzddlhead") != "")
            //{
            //    model.ISZDDLHEAD = data.Get("chkzddlhead").ToInt32();
            //}
            //else
            //{
            //    model.ISZDDLHEAD = 0;
            //}
            //if (data.Get("chkzdqyhead") != "")
            //{
            //    model.ISZDQYHEAD = data.Get("chkzdqyhead").ToInt32();
            //}
            //else
            //{
            //    model.ISZDQYHEAD = 0;
            //}
            model.WAREHOUSE = data.Get("Warehouse");
            model.EMAIL = data.Get("Email");
            model.MOBILE = data.Get("Mobile");
            model.QQ = data.Get("QQ");
            model.REMARK = data.Get("Remark");
            model.CONFIGJSON = "{\"theme\":{\"title\":\"Metro\",\"name\":\"metro\"},\"showType\":\"AccordionTree\",\"gridRows\":\"20\"}";

            return model.Insert();
        }

        protected override bool DoEdit(RequestData data)
        {
            BASUSERModel model = new BASUSERModel();
            model.TRUENAME = data.Get("TrueName");
            model.USERTYPEID = data.GetInt("UserTypeId");
            model.AREACODE = data.Get("AreaCode");
            model.ORGID = data.Get("OrgId");
            if (data.Get("IsAdmin") != null)
            {
                model.ISADMIN = data.GetBoolean("IsAdmin").ToInt32();
            }
            else
            {
                model.ISADMIN = 0;
            }
            if (data.Get("IsDisabled") != null)
            {
                model.ISDISABLED = data.GetBoolean("IsDisabled").ToInt32();
            }
            else
            {
                model.ISDISABLED = 0;
            }
            //if (data.Get("chkvisible") != "")
            //{
            //    model.ISVOSHEAD = data.Get("chkvisible").ToInt32();
            //}
            //else
            //{
            //    model.ISVOSHEAD = 0;
            //}
            //if (data.Get("chkzddlhead") != "")
            //{
            //    model.ISZDDLHEAD = data.Get("chkzddlhead").ToInt32();
            //}
            //else
            //{
            //    model.ISZDDLHEAD = 0;
            //}
            //if (data.Get("chkzdqyhead") != "")
            //{
            //    model.ISZDQYHEAD = data.Get("chkzdqyhead").ToInt32();
            //}
            //else
            //{
            //    model.ISZDQYHEAD = 0;
            //}
            model.WAREHOUSE = data.Get("Warehouse");
            model.EMAIL = data.Get("Email");
            model.MOBILE = data.Get("Mobile");
            model.QQ = data.Get("QQ");
            model.REMARK = data.Get("Remark");
            return model.Update(BASUSER.ID == data.Get("id"));
        }

        protected override bool DoDelete(RequestData data)
        {
            BASUSERModel model = new BASUSERModel();
            return model.Delete(BASUSER.ID == data.Get("id"));
        }

        [HttpPost]
        public ActionResult GetUserRole(int userId)
        {
            List<dynamic> list = SqlModel.Select(BASROLE.ID, BASROLE.ROLENAME, BASROLE.REMARK)
                .From(DB.BASUSERROLE)
                .LeftJoin(DB.BASROLE).On(BASROLE.ID == BASUSERROLE.ROLEID)
                .Where(BASUSERROLE.USERID == userId)
                .ExecToDynamicList();
            return Json(list);
        }

        [HttpPost]
        public ActionResult SaveUserRole(RequestData data)
        {
            bool isSuccess = false;
            PostResult result = new PostResult();
            string userId = data.Get("userId");
            string roleIdString = data.Get("roleIdString");

            string[] roleIdArray = roleIdString.Split(',');
            BASUSERROLEModel delModel = new BASUSERROLEModel();
            result.Success = delModel.Delete(BASUSERROLE.USERID == userId);
            using (TranModel tran = new DBTranModel())
            {
                BASUSERROLEModel insModel = null;
                for (int i = 0; i < roleIdArray.Length; i++)
                {
                    string roleId = roleIdArray[i];
                    if (roleId != "")
                    {
                        insModel = new BASUSERROLEModel();
                        insModel.USERID = userId.ToInt32();
                        insModel.ROLEID = roleId.ToInt32();
                        insModel.Inserting(tran);
                        isSuccess = true;
                    }
                }

                bool ret = tran.Execute();
                result.Success = isSuccess ? ret : result.Success;
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult SavePwd(RequestData data)
        {
            PostResult result = new PostResult();
            BASUSERModel model = new BASUSERModel();
            model.PASSWORD = StringHelper.MD5String(data.Get("pwd"));
            bool ret = model.Update(BASUSER.ID == data.Get("userId"));
            result.Success = ret;
            return Json(result);
        }

        [HttpPost]
        public ActionResult EditPwd(RequestData data)
        {
            PostResult result = new PostResult();
            string oldPass = data.Get("oldPwd");
            string newPass = data.Get("newPwd");
            BASUSERModel model = new BASUSERModel();
            model.PASSWORD = StringHelper.MD5String(newPass);
            bool ret = model.Update(BASUSER.ID == CurrentUser.Id);
            result.Success = ret;
            return Json(result);
        }

        [HttpPost]
        public ActionResult Authorize(RequestData data)
        {
            var d = data.Get("d");
            if (string.IsNullOrEmpty(d))
            {
                Json("参数错误！");
            }

            int k = this.UserAuthorize(d) ? 1 : 0;
            return Json(k.ToString());
        }

        //用户授权
        private bool UserAuthorize(string data)
        {
            JObject jobj = JObject.Parse(data);
            DataTableModel dtm = SqlModel.Select(BASBUTTON.ID, BASBUTTON.BUTTONTEXT, BASBUTTON.BUTTONTAG)
                .From(DB.BASBUTTON).ExecToTableModel();
            var userId = jobj["userId"];
            var menus = jobj["menus"];
            var navs = menus.Where(m => m["buttons"].Count() > 0);

            BASUSERNAVBTNModel delModel = new BASUSERNAVBTNModel();
            delModel.Delete(BASUSERNAVBTN.USERID == userId.ToString());
            using (TranModel tran = new DBTranModel())
            {

                BASUSERNAVBTNModel insModel = null;
                DataRowModel rm = null;
                foreach (var nav in navs)
                {
                    foreach (var btn in nav["buttons"])
                    {
                        rm = dtm.Find(m => m[BASBUTTON.BUTTONTAG].ToString() == btn.ToString());
                        insModel = new BASUSERNAVBTNModel();
                        insModel.USERID = userId.ToString().ToInt32();
                        insModel.NAVID = nav["navid"].ToString().ToInt32();
                        insModel.BTNID = rm[BASBUTTON.ID].ToInt32();
                        insModel.Inserting(tran);
                    }
                }
                return tran.Execute();
            }
        }

        [HttpPost]
        public ActionResult GetMenu(int id)
        {
            DataTable urTab = SqlModel.Select(BASUSERROLE.ROLEID).From(DB.BASUSERROLE)
                .Where(BASUSERROLE.USERID == id).ExecToDataTable();
            string roleIdStr = urTab.Join((r, i) => (i > 0 ? "," : "") + r[BASUSERROLE.ROLEID.Name]);
            var json = this.GetNavBtnJson(roleIdStr, id);
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(json);
            return Content(jsonStr);
        }

        public ActionResult EditPwd()
        {
            return View();

        }

        public ActionResult SetRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUserTypeData()
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.PowerType).OrderByAsc(BASDIC.SORTNO)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }


        //[HttpPost]
        //public ActionResult GetAreaData()
        //{
        //    DataTable dt =
        //       SqlModel.Select("T".Field("id"), "T".Field("parentId"), "T".Field("text"), "T".Field("SortNo"), "T".Field("prefix"))
        //       .From(
        //           (
        //               SqlModel.Select((-1).As("id"), 0.As("parentId"), "请选择区域信息".As("text"), 0.As("SortNo"), 0.As("prefix"))
        //                   .From(DB.T_SYS_AREA)
        //           )
        //           .Union
        //           (
        //               SqlModel.Select(T_SYS_AREA.AREA_CODE.ToChar().As("id"),
        //                               T_SYS_AREA.PARENT_CODE.ToChar().As("parentId"),
        //                               T_SYS_AREA.AREA_TEXT.ToChar().As("text"),
        //                               T_SYS_AREA.ID.ToChar().As("SortNo"),
        //                               T_SYS_AREA.LICENSE_PLATE_PREFIX.ToChar().As("prefix"))
        //                       .From(DB.T_SYS_AREA)
        //               .Where(T_SYS_AREA.AREA_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode)) |
        //                      T_SYS_AREA.PARENT_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode))
        //                      )
        //           ).As("T")
        //       )
        //       .OrderByAsc("T".Field("SortNo"))
        //       .ExecToDataTable();
        //    var json = dt.ToDynamicComboTree("0", "prefix");
        //    return Json(json);
        //}

        //[HttpPost]
        //public ActionResult GetAreaTreeData()
        //{
        //    DataTable dt = SqlModel.Select(T_SYS_AREA.AREA_CODE.ToChar().As("id"),
        //                               T_SYS_AREA.PARENT_CODE.ToChar().As("parentId"),
        //                               T_SYS_AREA.AREA_TEXT.ToChar().As("text"),
        //                               T_SYS_AREA.ID.ToChar().As("SortNo"))
        //                       .From(DB.T_SYS_AREA)
        //               .Where(T_SYS_AREA.AREA_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode)) |
        //                      T_SYS_AREA.PARENT_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode))
        //                      ).OrderByAsc(T_SYS_AREA.ID)
        //               .ExecToDataTable();
        //    var json = dt.ToDynamicComboTree("0");
        //    return Json(json);
        //}


        [HttpPost]
        public ActionResult GetAreaData(RequestData data)
        {
            FieldModel where = null;
            string relationType = data.Get("userTypeId");
            if (relationType == "1")
            {
                where &= T_SYS_AREA.AREA_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode));
            }
            else
            {
                where &= T_SYS_AREA.PARENT_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode));
            }
            DataTable dt = SqlModel.Select(T_SYS_AREA.AREA_CODE.As("CODE"), T_SYS_AREA.AREA_TEXT.As("TITLE")).From(DB.T_SYS_AREA)
            .Where(where).ExecToDataTable();
            return Json(dt.ToDynamicList());
        }



        [HttpPost]
        public ActionResult GetManageDeptData(RequestData data)
        {
            FieldModel where = null;
            TableModel table = null;
            FieldModel[] m = null;

            string powerTypeId = data.Get("powerTypeId");
            string areaCode = data.Get("areaCode");

            //if (powerTypeId == "1" || powerTypeId == "2")
            //{
            //    m = new FieldModel[2] { T_MANAGE_DEPT_INFO.ID.As("ORGNO"), T_MANAGE_DEPT_INFO.ORGNAME.As("ORGNAME") };
            //    table = DB.T_MANAGE_DEPT_INFO;
            //    where = T_MANAGE_DEPT_INFO.POWERTYPE == powerTypeId;
            //    if (!string.IsNullOrEmpty(areaCode))
            //        where &= T_MANAGE_DEPT_INFO.AREACODE == areaCode;
            //}
            //else
            //{
            //    m = new FieldModel[2] { T_COMPANY_INFO.TSNO.As("ORGNO"), T_COMPANY_INFO.TESTSTATION.As("ORGNAME") };
            //    table = DB.T_COMPANY_INFO;
            //    if (!string.IsNullOrEmpty(areaCode))
            //        where = T_COMPANY_INFO.CITY == areaCode;
            //}

            DataTable dt = SqlModel.Select(m).From(table)
                      .Where(where).ExecToDataTable();
            return Json(dt.ToDynamicList());
        }

        [HttpPost]
        public ActionResult GetCompanyData(RequestData data)
        {
            //string areaCode = data.Get("AreaCode");
            int removeNoneSelect = data.GetInt("RemoveNoneSelect");
            int disableNotAllowDeliveryFlg = data.GetInt("DisableNotAllowDeliveryFlg");

            FieldModel where = null;
            string relationType = data.Get("userTypeId");
            if (relationType == "1")
            {
                where &= T_SYS_AREA.RELATION_TYPE == relationType &
                    T_SYS_AREA.AREA_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode));
            }
            else
            {
                where &= T_SYS_AREA.RELATION_TYPE == "2" &
                    T_SYS_AREA.PARENT_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode));
            }


            DataTable dt = new DataTable();
            //if (string.IsNullOrEmpty(areaCode))
            //{
            //    dt = SqlModel.Select(T_COMPANY_INFO.TSNO.As("Code"), T_COMPANY_INFO.TESTSTATION.As("Title"))
            //            .From(DB.T_SYS_AREA)
            //            .LeftJoin(DB.T_COMPANY_INFO)
            //            .On(T_COMPANY_INFO.CITY == T_SYS_AREA.AREA_CODE)
            //            .Where(where)
            //            .ExecToDataTable();
            //}
            //else
            //{
            //    dt = SqlModel.Select(T_COMPANY_INFO.TSNO.As("Code"), T_COMPANY_INFO.TESTSTATION.As("Title"))
            //    .From(DB.T_COMPANY_INFO)
            //    .Where(T_COMPANY_INFO.CITY == areaCode).ExecToDataTable();
            //}

            //DataTable dt = SqlModel.Select(T_COMPANY_INFO.TSNO.As("Code"), T_COMPANY_INFO.TESTSTATION.As("Title"))
            //    .From(DB.T_COMPANY_INFO)
            //    .Where(T_COMPANY_INFO.CITY == areaCode).ExecToDataTable();
            if (removeNoneSelect == 1)
            {
                return Json(dt.ToDynamicList());
            }
            else
            {
                return Json(dt.AddNoneSelectItem(0, ConstStrings.NoneSelect).ToDynamicList());
            }
        }

        //得到所有用户
        public ActionResult GetUsers()
        {
            DataTable dt = SqlModel.Select(BASUSER.USERNAME, BASUSER.TRUENAME)
                .From(DB.BASUSER).OrderByAsc(BASUSER.ID).Where(BASUSER.ISDEL == "0")
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        /// <summary>
        /// 获取用户的基础信息 author:liwu  2017-12-21
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserBaseInfo(RequestData data)
        {
            FieldModel where = null;
            string userName = data.Get("userName");
            string userID = data.Get("userID");
            
            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(userID))
            {
                //默认当前登录用户
                userName = CurrentUser.UserName;
            }
            if (!string.IsNullOrEmpty(userName))
            {
                where &= BASUSER.USERNAME == userName;
            }
            if (!string.IsNullOrEmpty(userID))
            {
                where &= BASUSER.ID == userID;
            }
            var sql = SqlModel.SelectAll()
                .From(DB.BASUSER).OrderByAsc(BASUSER.ID).Where(BASUSER.ISDEL == "0")
                .Where(where);
            var list = sql.ExecToDynamicList();
            if(list!=null && list.Count > 0)
            {
                list[0]["PASSWORD"] = string.Empty;
                return Json(list[0]);
            }
            return Json(new { });
        }
        
    }
}
