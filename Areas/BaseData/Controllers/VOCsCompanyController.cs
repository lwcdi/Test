using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using w.Model;
using w.ORM;

namespace UI.Web.Areas.BaseData.Controllers
{
    public class VOCsCompanyController : ListController
    {
        #region View
        public ActionResult VOCsCompanyList(int? navId)
        {
            return View(navId ?? 0);
        }

        public ActionResult VOCsCompanyEdit()
        {
            return View(0);
        }

        public ActionResult VOCsCompanyTXEdit()
        {
            return View(0);
        }

        public ActionResult VOCsCompanyPWEdit()
        {
            return View(0);
        }

        public ActionResult CompanyBaseInfo()
        {
            return View(0);
        }
        public ActionResult CompanyPKInfo()
        {
            return View(0);
        }
        public ActionResult CompanyDeviceInfo()
        {
            return View(0); 
        }
        public ActionResult CompanyContactInfo()
        {
            return View(0);
        }
        public ActionResult CompanyJKDeviceInfo()
        {
            return View(0);
        }
        
        public ActionResult CompanyZLDeviceInfo()
        {
            return View(0);
        }
        
        public ActionResult CompanyPKTXInfo()
        {
            return View(0);
        }

        public ActionResult CompanyPKEdit()
        {
            return View(0);
        }
        #endregion

        #region 增删改查
        protected override SqlModel GetSqlModel(RequestData data)
        {
            FieldModel where = null;
            where &= T_BASE_COMPANY.ISDEL == "0";
            string CompanyName = data.Get("CompanyName");
            if (!string.IsNullOrEmpty(CompanyName))
                where &= T_BASE_COMPANY.NAME.Like(CompanyName);
            //string UTYPE = data.Get("UTYPESEA");
            //if (string.IsNullOrEmpty(UTYPE) == false)
            //    where &= T_BAS_ANNOUNCEMENT_MANAGE.UTYPE == UTYPE;
            //if (string.IsNullOrEmpty(data.Get("sDate")) == false && string.IsNullOrEmpty(data.Get("eDate")) == false)
            //{
            //    string sDate = data.Get("sDate") + " 00:00:00";
            //    string eDate = data.Get("eDate") + " 23:59:59";
            //    where &= T_BAS_ANNOUNCEMENT_MANAGE.RELEASETIME.BetweenAnd(DateTime.Parse(sDate), DateTime.Parse(eDate));
            //}
            SqlModel sql = SqlModel.SelectAll()
                .From(DB.T_BASE_COMPANY)
                //.LeftJoin(DB.BASDIC.As("d1")).On("d1".Field(BASDIC.CODE) == T_BAS_ANNOUNCEMENT_MANAGE.UTYPE & "d1".Field(BASDIC.TYPECODE) == "ANN_TYPE")
                //.LeftJoin(DB.BASUSER.As("d2")).On("d2".Field(BASUSER.USERNAME) == T_BAS_ANNOUNCEMENT_MANAGE.PUBLISHER)
                .Where(where).OrderByDesc(T_BASE_COMPANY.ID);
            return sql;
        }

        protected override bool DoAdd(RequestData data)
        {
            T_BASE_COMPANYModel model = new T_BASE_COMPANYModel();
            model.NAME = data.Get("NAME");
            model.CREATETIME = DateTime.Now;
            return model.Insert();
        }
        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult Add(RequestData data)
        {
            return base.Add(data);
        }

        protected override bool DoEdit(RequestData data)
        {
            T_BASE_COMPANYModel model = new T_BASE_COMPANYModel();
            model.NAME = data.Get("NAME");
            model.UPDATETIME = DateTime.Now;
            return model.Update(T_BASE_COMPANY.ID == data.Get("id"));
        }
        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult Edit(RequestData data)
        {
            return base.Edit(data);
        }

        protected override bool DoDelete(RequestData data)
        {
            T_BASE_COMPANYModel model = new T_BASE_COMPANYModel();
            model.ISDEL = ((int)DeleteType.Delete).ToString();
            return model.Update(T_BASE_COMPANY.ID == data.Get("id"));
        }
        [HttpPost]
        [ValidateInput(false)]
        public override ActionResult Delete(RequestData data)
        {
            return base.Delete(data);
        } 
        #endregion

        [HttpPost]
        public ActionResult GetCompanyID(RequestData data)
        {
            var msg = new { success = false, data = "" };
            T_BASE_COMPANYModel model = new T_BASE_COMPANYModel();
            model.CREATEUSER = CurrentUser.UserName;
            model.ISDEL = "1";
            int id = model.GetIDByInsert();
            if (id > 0)
            {
                msg = new { success = true, data = id.ToString() };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult delCompanyByID(RequestData data)
        {
            var msg = new { success = true, data = "删除成功" };
            T_BASE_COMPANYModel model = new T_BASE_COMPANYModel();
            model.Delete(T_BASE_COMPANY.ID == data.Get("id"));
            return Json(msg);
        }
        
        /// <summary>
        /// 用于数据编辑的时候选择城市
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAreaData_City(RequestData data)
        {
            string isAppendAll = data.Get("appendAll");

            FieldModel where = T_SYS_AREA.AREA_CODE.In(SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == ConstStrings.SysAreaCode));

            DataTable dt = SqlModel.Select(T_SYS_AREA.AREA_CODE.As("CODE"), T_SYS_AREA.AREA_TEXT.As("TITLE")).From(DB.T_SYS_AREA)
            .Where(where).OrderByAsc(T_SYS_AREA.ID).ExecToDataTable();
            if ("appendAll" == isAppendAll)
            {
                DataRow allDr = dt.NewRow();
                allDr["CODE"] = "";
                allDr["TITLE"] = "全部";
                dt.Rows.InsertAt(allDr, 0);
            }
            return Json(dt.ToDynamicList());
        }

        [HttpPost]
        public ActionResult GetPKDataByID(RequestData data)
        {
            string isAppendAll = data.Get("appendAll");
            string id = data.Get("id").Trim();

            FieldModel where = T_BASE_COMPANY_PK.COMPANYID == id;

            DataTable dt = SqlModel.Select(T_BASE_COMPANY_PK.ID.As("CODE"), T_BASE_COMPANY_PK.NAME.As("TITLE"))
                .From(DB.T_BASE_COMPANY_PK)
                .Where(where).OrderByAsc(T_BASE_COMPANY_PK.ID).ExecToDataTable();
            if ("appendAll" == isAppendAll)
            {
                DataRow allDr = dt.NewRow();
                allDr["CODE"] = "";
                allDr["TITLE"] = "全部";
                dt.Rows.InsertAt(allDr, 0);
            }
            return Json(dt.ToDynamicList());
        }

        /// <summary>
        /// 获取字典
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDicCodeData(string typeCode)
        {
            FieldModel where = BASDIC.TYPECODE == typeCode;
            if (typeCode.Contains(","))
            {
                
                where = BASDIC.TYPECODE.In(typeCode);
            }
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(where).OrderByAsc(BASDIC.SORTNO)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }

        #region 企业信息相关操作
        [HttpPost]
        public ActionResult GetData(string CompanyName, string AREA, string HYLB, string GZCD, string isVOCs)
        {
            string page = Request["page"].Trim();
            string rows = Request["rows"].Trim();

            FieldModel where = T_BASE_COMPANY.ISDEL == "0";
            if (!string.IsNullOrEmpty(CompanyName))
            {
                CompanyName = HttpUtility.UrlDecode(CompanyName.Trim()).Trim();
                where &= T_BASE_COMPANY.NAME.Like(CompanyName);
            }
            if (!string.IsNullOrEmpty(AREA) && AREA != "all")
            {
                AREA = AREA.Trim();
                where &= T_BASE_COMPANY.AREA == AREA;
            }
            if (!string.IsNullOrEmpty(HYLB))
            {
                HYLB = HYLB.Trim();
                where &= T_BASE_COMPANY.BASTYPE == HYLB;
            }
            if (!string.IsNullOrEmpty(GZCD))
            {
                GZCD = GZCD.Trim();
                where &= T_BASE_COMPANY.GZCD == GZCD;
            }
            if (!string.IsNullOrEmpty(isVOCs))
            {
                isVOCs = isVOCs.Trim();
                where &= T_BASE_COMPANY.ISVOCS == isVOCs;
            }
            SqlModel sql = SqlModel.Select(T_BASE_COMPANY.ID, T_BASE_COMPANY.ID.As("ID2"), T_BASE_COMPANY.NAME, T_SYS_AREA.AREA_TEXT.As("NAREA"), T_BASE_COMPANY.ADDRESS, T_BASE_COMPANY.BASTYPE, "BASTYPE".Field("TITLE").As("NBASTYPE"),
                                           "GZCD".Field("TITLE").As("NGZCD"), T_BASE_COMPANY.LEGALOR
            )
            .From(DB.T_BASE_COMPANY)
            .LeftJoin(DB.T_SYS_AREA).On(T_BASE_COMPANY.AREA == T_SYS_AREA.AREA_CODE)
            .LeftJoin(DB.BASDIC.As("BASTYPE")).On(T_BASE_COMPANY.BASTYPE == "BASTYPE".Field("CODE") & "BASTYPE".Field("TYPECODE") == "IndustryType")
            .LeftJoin(DB.BASDIC.As("GZCD")).On(T_BASE_COMPANY.GZCD == "GZCD".Field("CODE") & "GZCD".Field("TYPECODE") == "CompanyGZCD")
            .Where(where)
            .OrderByDesc(T_BASE_COMPANY.CREATETIME);

            PagedDataTable dtResult = sql.ExecToPagedTable(Convert.ToInt32(page), Convert.ToInt32(rows), new OrderByModel() { OrderType = OrderType.Desc, FieldModel = T_BASE_COMPANY.CREATETIME });

            return Content(JsonConvert.SerializeObject(new
            {
                total = dtResult.TotalCount,
                rows = dtResult.Data
            }), "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult GetCompanyInfoByID(RequestData data)
        {
            string id = string.Empty;
            var msg = new { success = false, data = "查询失败" };
            if (!string.IsNullOrEmpty(data.Get("id")))
            {
                id = data.Get("id").Trim();
            }
            else
            {
                return Json(msg);
            }
            var dt = SqlModel.SelectAll()
                .From(DB.T_BASE_COMPANY)
                .Where(T_BASE_COMPANY.ID == id)
                .ExecToDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                msg = new { success = true, data = dt.ToJson() };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult saveCompanyData(RequestData data)
        {
            var msg = new { success = false, data = "保存失败" };
            bool result = false;
            T_BASE_COMPANYModel model = new T_BASE_COMPANYModel();
            this.GetModelValue(model, data);
            if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "0") //新增
            {
                model.ISDEL = "0";
                result = model.Update(T_BASE_COMPANY.ID == data.Get("ID").Trim());
            }
            else if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "1") //编辑
            {
                model.UPDATETIME = DateTime.Now;
                model.UPDATEUSER = CurrentUser.UserName;
                result = model.Update(T_BASE_COMPANY.ID == data.Get("ID").Trim());
            }
            if (result)
            {
                msg = new { success = true, data = "保存成功" };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult DelCompanyInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "删除失败" };
            T_BASE_COMPANYModel model = new T_BASE_COMPANYModel();
            model.ISDEL = "1";
            model.UPDATETIME = DateTime.Now;
            model.UPDATEUSER = CurrentUser.UserName;
            bool result = model.Update(T_BASE_COMPANY.ID == data.Get("id"));
            if (result)
            {
                msg = new { success = true, data = "删除成功" };
            }
            return Json(msg);
        } 
        #endregion

        #region 企业联系人相关操作
        [HttpPost]
        public ActionResult GetContactData(string id)
        {
            string page = Request["page"].Trim();
            string rows = Request["rows"].Trim();

            FieldModel where =
                T_BASE_COMPANY_CONTACT.ISDEL == "0";
            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim();
                where &= T_BASE_COMPANY.ID == id;
            }
            else
            {
                where &= T_BASE_COMPANY.ID == "0";
            }
            SqlModel sql = SqlModel.SelectAll("ISHB".Field("TITLE").As("NISHB")
            )
            .From(DB.T_BASE_COMPANY_CONTACT)
            .InnerJoin(DB.T_BASE_COMPANY).On(T_BASE_COMPANY.ID == T_BASE_COMPANY_CONTACT.COMPANYID)
            .LeftJoin(DB.BASDIC.As("ISHB")).On(T_BASE_COMPANY_CONTACT.ISHB == "ISHB".Field("CODE") & "ISHB".Field("TYPECODE") == "TrueOrFalse")
            .Where(where)
            .OrderByDesc(T_BASE_COMPANY_CONTACT.CREATETIME);

            PagedDataTable dtResult = sql.ExecToPagedTable(Convert.ToInt32(page), Convert.ToInt32(rows), new OrderByModel() { OrderType = OrderType.Desc, FieldModel = T_BASE_COMPANY_CONTACT.CREATETIME });

            return Content(JsonConvert.SerializeObject(new
            {
                total = dtResult.TotalCount,
                rows = dtResult.Data
            }), "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult GetCompanyContactInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "查询失败" };
            if (!string.IsNullOrEmpty(data.Get("rowID")))
            {
                rowID = data.Get("rowID").Trim();
            }
            else
            {
                return Json(msg);
            }
            var dt = SqlModel.SelectAll()
                .From(DB.T_BASE_COMPANY_CONTACT)
                .Where(T_BASE_COMPANY_CONTACT.ID == rowID)
                .ExecToDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                msg = new { success = true, data = dt.ToJson() };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult saveCompanyContactData(RequestData data)
        {
            var msg = new { success = false, data = "保存失败" };
            bool result = false;
            if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "0") //新增
            {
                T_BASE_COMPANY_CONTACTModel model = new T_BASE_COMPANY_CONTACTModel();
                this.GetModelValue(model, data);
                model.CREATEUSER = CurrentUser.UserName;
                result = model.Insert();
            }
            else if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "1") //编辑
            {
                T_BASE_COMPANY_CONTACTModel model = new T_BASE_COMPANY_CONTACTModel();
                this.GetModelValue(model, data);
                model.UPDATETIME = DateTime.Now;
                model.UPDATEUSER = CurrentUser.UserName;
                result = model.Update(T_BASE_COMPANY_CONTACT.ID == data.Get("rowID").Trim());
            }
            if (result)
            {
                msg = new { success = true, data = "保存成功" };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult DelCompanyContactInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "删除失败" };
            T_BASE_COMPANY_CONTACTModel model = new T_BASE_COMPANY_CONTACTModel();
            model.ISDEL = "1";
            model.UPDATETIME = DateTime.Now;
            model.UPDATEUSER = CurrentUser.UserName;
            bool result = model.Update(T_BASE_COMPANY_CONTACT.ID == data.Get("id"));
            if (result)
            {
                msg = new { success = true, data = "删除成功" };
            }
            return Json(msg);
        }
        #endregion

        #region 企业监控设备相关操作
        [HttpPost]
        public ActionResult GetJKDeviceData(string id)
        {
            string page = Request["page"].Trim();
            string rows = Request["rows"].Trim();

            FieldModel where =
                T_BASE_COMPANY_MONITOR.ISDEL == "0";
            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim();
                where &= T_BASE_COMPANY_MONITOR.COMPANYID == id;
            }
            else
            {
                where &= T_BASE_COMPANY_MONITOR.COMPANYID == "0";
            }
            SqlModel sql = SqlModel.SelectAll(T_BASE_COMPANY_PK.NAME.As("PKNAME")
            )
            .From(DB.T_BASE_COMPANY_MONITOR)
            .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_MONITOR.PKID == T_BASE_COMPANY_PK.ID)
            //.LeftJoin(DB.BASDIC.As("ISHB")).On(T_BASE_COMPANY_CONTACT.ISHB == "ISHB".Field("CODE") & "ISHB".Field("TYPECODE") == "TrueOrFalse")
            .Where(where)
            .OrderByDesc(T_BASE_COMPANY_MONITOR.CREATETIME);

            PagedDataTable dtResult = sql.ExecToPagedTable(Convert.ToInt32(page), Convert.ToInt32(rows), new OrderByModel() { OrderType = OrderType.Desc, FieldModel = T_BASE_COMPANY_MONITOR.CREATETIME });

            return Content(JsonConvert.SerializeObject(new
            {
                total = dtResult.TotalCount,
                rows = dtResult.Data
            }), "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult GetJKDeviceInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "查询失败" };
            if (!string.IsNullOrEmpty(data.Get("rowID")))
            {
                rowID = data.Get("rowID").Trim();
            }
            else
            {
                return Json(msg);
            }
            var dt = SqlModel.SelectAll()
                .From(DB.T_BASE_COMPANY_MONITOR)
                .Where(T_BASE_COMPANY_MONITOR.ID == rowID)
                .ExecToDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                msg = new { success = true, data = dt.ToJson() };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult saveJKDeviceData(RequestData data)
        {
            var msg = new { success = false, data = "保存失败" };
            bool result = false;
            if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "0") //新增
            {
                T_BASE_COMPANY_MONITORModel model = new T_BASE_COMPANY_MONITORModel();
                this.GetModelValue(model, data);
                model.CREATEUSER = CurrentUser.UserName;
                result = model.Insert();
            }
            else if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "1") //编辑
            {
                T_BASE_COMPANY_MONITORModel model = new T_BASE_COMPANY_MONITORModel();
                this.GetModelValue(model, data);
                model.UPDATETIME = DateTime.Now;
                model.UPDATEUSER = CurrentUser.UserName;
                result = model.Update(T_BASE_COMPANY_MONITOR.ID == data.Get("rowID").Trim());
            }
            if (result)
            {
                msg = new { success = true, data = "保存成功" };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult DelJKDeviceInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "删除失败" };
            T_BASE_COMPANY_MONITORModel model = new T_BASE_COMPANY_MONITORModel();
            model.ISDEL = "1";
            model.UPDATETIME = DateTime.Now;
            model.UPDATEUSER = CurrentUser.UserName;
            bool result = model.Update(T_BASE_COMPANY_MONITOR.ID == data.Get("id"));
            if (result)
            {
                msg = new { success = true, data = "删除成功" };
            }
            return Json(msg);
        }
        #endregion

        #region 企业治理设备相关操作
        [HttpPost]
        public ActionResult GetZLDeviceData(string id)
        {
            string page = Request["page"].Trim();
            string rows = Request["rows"].Trim();

            FieldModel where =
                T_BASE_COMPANY_ZL.ISDEL == "0";
            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim();
                where &= T_BASE_COMPANY_ZL.COMPANYID == id;
            }
            else
            {
                where &= T_BASE_COMPANY_ZL.COMPANYID == "0";
            }
            SqlModel sql = SqlModel.SelectAll(T_BASE_COMPANY_ZL.NAME.As("PKNAME")
            )
            .From(DB.T_BASE_COMPANY_ZL)
            .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_ZL.PKID == T_BASE_COMPANY_PK.ID)
            //.LeftJoin(DB.BASDIC.As("ISHB")).On(T_BASE_COMPANY_CONTACT.ISHB == "ISHB".Field("CODE") & "ISHB".Field("TYPECODE") == "TrueOrFalse")
            .Where(where)
            .OrderByDesc(T_BASE_COMPANY_ZL.CREATETIME);

            PagedDataTable dtResult = sql.ExecToPagedTable(Convert.ToInt32(page), Convert.ToInt32(rows), new OrderByModel() { OrderType = OrderType.Desc, FieldModel = T_BASE_COMPANY_ZL.CREATETIME });

            return Content(JsonConvert.SerializeObject(new
            {
                total = dtResult.TotalCount,
                rows = dtResult.Data
            }), "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult GetZLDeviceInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "查询失败" };
            if (!string.IsNullOrEmpty(data.Get("rowID")))
            {
                rowID = data.Get("rowID").Trim();
            }
            else
            {
                return Json(msg);
            }
            var dt = SqlModel.SelectAll()
                .From(DB.T_BASE_COMPANY_ZL)
                .Where(T_BASE_COMPANY_ZL.ID == rowID)
                .ExecToDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                msg = new { success = true, data = dt.ToJson() };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult saveZLDeviceData(RequestData data)
        {
            var msg = new { success = false, data = "保存失败" };
            bool result = false;
            if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "0") //新增
            {
                T_BASE_COMPANY_ZLModel model = new T_BASE_COMPANY_ZLModel();
                this.GetModelValue(model, data);
                model.CREATEUSER = CurrentUser.UserName;
                result = model.Insert();
            }
            else if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "1") //编辑
            {
                T_BASE_COMPANY_ZLModel model = new T_BASE_COMPANY_ZLModel();
                this.GetModelValue(model, data);
                model.UPDATETIME = DateTime.Now;
                model.UPDATEUSER = CurrentUser.UserName;
                result = model.Update(T_BASE_COMPANY_ZL.ID == data.Get("rowID").Trim());
            }
            if (result)
            {
                msg = new { success = true, data = "保存成功" };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult DelZLDeviceInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "删除失败" };
            T_BASE_COMPANY_ZLModel model = new T_BASE_COMPANY_ZLModel();
            model.ISDEL = "1";
            model.UPDATETIME = DateTime.Now;
            model.UPDATEUSER = CurrentUser.UserName;
            bool result = model.Update(T_BASE_COMPANY_ZL.ID == data.Get("id"));
            if (result)
            {
                msg = new { success = true, data = "删除成功" };
            }
            return Json(msg);
        }
        #endregion

        #region 企业通信配置相关操作
        [HttpPost]
        public ActionResult GetTXInfoData(string id)
        {
            string page = Request["page"].Trim();
            string rows = Request["rows"].Trim();

            FieldModel where =
                T_BASE_COMPANY_PK_TX.ISDEL == "0";
            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim();
                where &= T_BASE_COMPANY_PK_TX.COMPANYID == id;
            }
            else
            {
                where &= T_BASE_COMPANY_PK_TX.COMPANYID == "0";
            }
            SqlModel sql = SqlModel.SelectAll(T_BASE_COMPANY_PK.NAME.As("PKNAME"), T_BASE_COMPANY_PK.TYPE.As("PKTYPE"), T_BASE_COMPANY_PK.GASTYPE, T_BASE_COMPANY_PK.WAY, "GASTYPE".Field("TITLE").As("NGASTYPE"), "WAY".Field("TITLE").As("NWAY")
            )
            .From(DB.T_BASE_COMPANY_PK_TX)
            .LeftJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_PK_TX.PKID == T_BASE_COMPANY_PK.ID)
            .LeftJoin(DB.BASDIC.As("GASTYPE")).On(T_BASE_COMPANY_PK.GASTYPE == "GASTYPE".Field("CODE") & "GASTYPE".Field("TYPECODE") == "CompanyPKGasType")
            .LeftJoin(DB.BASDIC.As("WAY")).On(T_BASE_COMPANY_PK.WAY == "WAY".Field("CODE") & "WAY".Field("TYPECODE") == "CompanyPKWay")
            .Where(where)
            .OrderByDesc(T_BASE_COMPANY_PK_TX.CREATETIME);

            PagedDataTable dtResult = sql.ExecToPagedTable(Convert.ToInt32(page), Convert.ToInt32(rows), new OrderByModel() { OrderType = OrderType.Desc, FieldModel = T_BASE_COMPANY_PK_TX.CREATETIME });

            return Content(JsonConvert.SerializeObject(new
            {
                total = dtResult.TotalCount,
                rows = dtResult.Data
            }), "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult GetTXInfoInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "查询失败" };
            if (!string.IsNullOrEmpty(data.Get("rowID")))
            {
                rowID = data.Get("rowID").Trim();
            }
            else
            {
                return Json(msg);
            }
            var dt = SqlModel.SelectAll(T_BASE_COMPANY_PK.TYPE.As("PKTYPE"))
                .From(DB.T_BASE_COMPANY_PK_TX)
                .InnerJoin(DB.T_BASE_COMPANY_PK).On(T_BASE_COMPANY_PK.ID == T_BASE_COMPANY_PK_TX.PKID)
                .Where(T_BASE_COMPANY_PK_TX.ID == rowID)
                .ExecToDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                msg = new { success = true, data = dt.ToJson() };
            }
            return Json(msg);
        }

        /// <summary>
        /// 通过排口ID获取排口类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTXTypeByPKID(RequestData data)
        {
            string PKID = string.Empty;
            var msg = new { success = false, data = "查询失败" };
            if (!string.IsNullOrEmpty(data.Get("PKID")))
            {
                PKID = data.Get("PKID").Trim();
            }
            else
            {
                return Json(msg);
            }
            var dt = SqlModel.Select(T_BASE_COMPANY_PK.TYPE.As("PKTYPE"))
                .From(DB.T_BASE_COMPANY_PK)
                .Where(T_BASE_COMPANY_PK.ID == PKID)
                .ExecToDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                msg = new { success = true, data = dt.Rows[0]["PKTYPE"].ToString().Trim() };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult saveTXInfoData(RequestData data)
        {
            var msg = new { success = false, data = "保存失败" };
            bool result = false;
            if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "0") //新增
            {
                T_BASE_COMPANY_PK_TXModel model = new T_BASE_COMPANY_PK_TXModel();
                this.GetModelValue(model, data);
                if (!IsPKTXExist(model.PKID))
                {
                    model.CLCS = data.Get("CLCSS").Trim(',');
                    model.CSLX = data.Get("CSLXS").Trim(',');
                    model.CREATEUSER = CurrentUser.UserName;
                    result = model.Insert();
                }
                else
                {
                    msg = new { success = false, data = "保存失败，该排口已存在通信配置" };
                }
            }
            else if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "1") //编辑
            {
                T_BASE_COMPANY_PK_TXModel model = new T_BASE_COMPANY_PK_TXModel();
                this.GetModelValue(model, data);
                model.CLCS = data.Get("CLCSS").Trim(',');
                model.CSLX = data.Get("CSLXS").Trim(',');
                model.UPDATETIME = DateTime.Now;
                model.UPDATEUSER = CurrentUser.UserName;
                result = model.Update(T_BASE_COMPANY_PK_TX.ID == data.Get("rowID").Trim());
            }
            if (result)
            {
                msg = new { success = true, data = "保存成功" };
            }
            return Json(msg);
        }

        /// <summary>
        /// 判断排口通信配置是否已存在，一个排口只有一个通信配置
        /// </summary>
        /// <returns></returns>
        private bool IsPKTXExist(string PKID)
        {
            bool isExist = false;
            var dt = SqlModel.Select()
                     .From(DB.T_BASE_COMPANY_PK_TX)
                     .Where(T_BASE_COMPANY_PK_TX.PKID == PKID)
                     .ExecToDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                isExist = true;
            }
            return isExist;
        }

        [HttpPost]
        public ActionResult DelTXInfoInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "删除失败" };
            T_BASE_COMPANY_PK_TXModel model = new T_BASE_COMPANY_PK_TXModel();
            model.ISDEL = "1";
            model.UPDATETIME = DateTime.Now;
            model.UPDATEUSER = CurrentUser.UserName;
            bool result = model.Update(T_BASE_COMPANY_PK_TX.ID == data.Get("id"));
            if (result)
            {
                msg = new { success = true, data = "删除成功" };
            }
            return Json(msg);
        }
        #endregion

        #region 企业排口信息相关操作
        [HttpPost]
        public ActionResult GetPKInfoData(string id, string PKType)
        {
            string page = Request["page"].Trim();
            string rows = Request["rows"].Trim();

            FieldModel where =
                T_BASE_COMPANY_PK.ISDEL == "0";
            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim();
                where &= T_BASE_COMPANY_PK.COMPANYID == id;
            }
            else
            {
                where &= T_BASE_COMPANY_PK.COMPANYID == "0";
            }
            if (!string.IsNullOrEmpty(PKType))
            {
                id = id.Trim();
                where &= T_BASE_COMPANY_PK.TYPE == PKType;
            }
            SqlModel sql = SqlModel.SelectAll("GASTYPE".Field("TITLE").As("NGASTYPE"), "WAY".Field("TITLE").As("NWAY"), "REGULAR".Field("TITLE").As("NREGULAR")
            )
            .From(DB.T_BASE_COMPANY_PK)
            .LeftJoin(DB.BASDIC.As("GASTYPE")).On(T_BASE_COMPANY_PK.TYPE == "GASTYPE".Field("CODE") & "GASTYPE".Field("TYPECODE") == "CompanyPKGasType")
            .LeftJoin(DB.BASDIC.As("WAY")).On(T_BASE_COMPANY_PK.WAY == "WAY".Field("CODE") & "WAY".Field("TYPECODE") == "CompanyPKWay")
            .LeftJoin(DB.BASDIC.As("REGULAR")).On(T_BASE_COMPANY_PK.REGULAR == "REGULAR".Field("CODE") & "REGULAR".Field("TYPECODE") == "CompanyPKPFGL")
            .Where(where)
            .OrderByDesc(T_BASE_COMPANY_PK.CREATETIME);

            PagedDataTable dtResult = sql.ExecToPagedTable(Convert.ToInt32(page), Convert.ToInt32(rows), new OrderByModel() { OrderType = OrderType.Desc, FieldModel = T_BASE_COMPANY_PK.CREATETIME });

            return Content(JsonConvert.SerializeObject(new
            {
                total = dtResult.TotalCount,
                rows = dtResult.Data
            }), "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult GetPKInfoInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "查询失败" };
            if (!string.IsNullOrEmpty(data.Get("rowID")))
            {
                rowID = data.Get("rowID").Trim();
            }
            else
            {
                return Json(msg);
            }
            var dt = SqlModel.SelectAll()
                .From(DB.T_BASE_COMPANY_PK)
                .Where(T_BASE_COMPANY_PK.ID == rowID)
                .ExecToDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                msg = new { success = true, data = dt.ToJson() };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult savePKInfoData(RequestData data)
        {
            var msg = new { success = false, data = "保存失败" };
            bool result = false;
            if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "0") //新增
            {
                T_BASE_COMPANY_PKModel model = new T_BASE_COMPANY_PKModel();
                this.GetModelValue(model, data);
                model.CREATEUSER = CurrentUser.UserName;
                result = model.Insert();
            }
            else if (!string.IsNullOrEmpty(data.Get("SHOWTYPE")) && data.Get("SHOWTYPE").Trim() == "1") //编辑
            {
                T_BASE_COMPANY_PKModel model = new T_BASE_COMPANY_PKModel();
                this.GetModelValue(model, data);
                model.UPDATETIME = DateTime.Now;
                model.UPDATEUSER = CurrentUser.UserName;
                result = model.Update(T_BASE_COMPANY_PK.ID == data.Get("rowID").Trim());
            }
            if (result)
            {
                msg = new { success = true, data = "保存成功" };
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult DelPKInfoInfoByID(RequestData data)
        {
            string rowID = string.Empty;
            var msg = new { success = false, data = "删除失败" };
            T_BASE_COMPANY_PKModel model = new T_BASE_COMPANY_PKModel();
            model.ISDEL = "1";
            model.UPDATETIME = DateTime.Now;
            model.UPDATEUSER = CurrentUser.UserName;
            bool result = model.Update(T_BASE_COMPANY_PK.ID == data.Get("id"));
            if (result)
            {
                msg = new { success = true, data = "删除成功" };
            }
            return Json(msg);
        }
        #endregion
    }
}
