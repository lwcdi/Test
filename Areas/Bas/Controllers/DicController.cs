using w.ORM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using w.Model;

namespace UI.Web.Areas.Bas.Controllers
{
    public class DicController : ListController
    {
        protected override SqlModel GetSqlModel(RequestData data)
        {
            string typeId = data.Get("typeId");
            if (string.IsNullOrEmpty(typeId) == true)
            {
                typeId = "0";
            }

            FieldModel where = BASDIC.TYPEID == typeId;

            List<dynamic> filterRuleslist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(data.Get("filterRules"));
            if (filterRuleslist != null)
            {
                foreach (var item in filterRuleslist)
                {
                    this.SetWhere(item, BASDIC.ID.Name, BASDIC.ID, ref where);
                    this.SetWhere(item, BASDIC.TITLE.Name, BASDIC.TITLE, ref where);

                    this.SetWhere(item, BASDIC.CODE.Name, BASDIC.CODE, ref where);
                    this.SetWhere(item, BASDIC.SORTNO.Name, BASDIC.SORTNO, ref where);
                    this.SetWhere(item, BASDIC.STATUS.Name, BASDIC.STATUS, ref where);
                    this.SetWhere(item, BASDIC.REMARK.Name, BASDIC.REMARK, ref where);
                }
            }

            SqlModel model = SqlModel.SelectAll().From(DB.BASDIC)
                .Where(where)
                .OrderByAsc(BASDIC.SORTNO);
            return model;
        }

        protected override bool DoAdd(RequestData data)
        {
            DataRowModel rmodel = SqlModel.SelectAll().From(DB.BASDICTYPE).Where(BASDICTYPE.ID == data.Get("TypeId")).ExecToRowModel();

            BASDICModel model = new BASDICModel();
            model.TITLE = data.Get("Title");
            model.CODE = data.Get("Code");
            model.TYPEID = data.Get("TypeId").ToInt32();
            model.TYPECODE = rmodel[BASDICTYPE.CODE] == null ? "" : rmodel[BASDICTYPE.CODE].ToString();
            model.SORTNO = data.Get("SortNo").ToInt32();
            model.STATUS = data.Get("Status").ToInt32();
            model.REMARK = data.Get("Remark");
            return model.Insert();
        }

        protected override bool DoEdit(RequestData data)
        {
            BASDICModel model = new BASDICModel();
            model.TITLE = data.Get("Title");
            model.CODE = data.Get("Code");
            //model.TYPEID = data.Get("TypeId").ToInt32();
            model.SORTNO = data.Get("SortNo").ToInt32();
            model.STATUS = data.Get("Status").ToInt32();
            model.REMARK = data.Get("Remark");
            return model.Update(BASDIC.ID == data.Get("id"));
        }

        protected override bool DoDelete(RequestData data)
        {
            BASDICModel model = new BASDICModel();
            return model.Delete(BASDIC.ID == data.Get("id"));
        }

        #region DicType
        public ActionResult EditDicType()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetTreeDicType(RequestData data)
        {
            List<dynamic> list = SqlModel.Select(BASDICTYPE.ID.As("\"id\""), BASDICTYPE.TITLE.As("\"text\""))
                .From(DB.BASDICTYPE).OrderByAsc(BASDICTYPE.SORTNO).ExecToDynamicList();
            return Json(list);
        }

        [HttpPost]
        public ActionResult GetDataType(RequestData data)
        {
            dynamic row = SqlModel.SelectAll().From(DB.BASDICTYPE)
                .Where(BASDICTYPE.ID == data.Get("id")).ExecToDynamic();
            return Json(row);
        }

        [HttpPost]
        public ActionResult AddDicType(RequestData data)
        {
            PostResult result = new PostResult();
            BASDICTYPEModel model = new BASDICTYPEModel();
            model.TITLE = data.Get("Title");
            model.CODE = data.Get("Code");
            model.SORTNO = data.Get("SortNo").ToInt32();
            model.REMARK = data.Get("Remark");
            bool ret = model.Insert();
            result.Success = ret;
            return Json(result);
        }

        [HttpPost]
        public ActionResult EditDicType(RequestData data)
        {
            PostResult result = new PostResult();
            using (TranModel tran = new DBTranModel())
            {

                BASDICTYPEModel model = new BASDICTYPEModel();
                model.TITLE = data.Get("Title");
                model.CODE = data.Get("Code");
                model.SORTNO = data.Get("SortNo").ToInt32();
                model.REMARK = data.Get("Remark");

                model.Updating(tran, BASDICTYPE.ID == data.Get("Id"));

                BASDICModel dmodel = new BASDICModel();
                dmodel.TYPECODE = data.Get("Code");
                dmodel.Updating(tran, BASDIC.TYPEID == data.Get("Id"));
                result.Success = tran.Execute();
            }
            //result.Success = tran.Execute();
            return Json(result);
        }

        [HttpPost]
        public ActionResult DelDicType(RequestData data)
        {
            PostResult result = new PostResult();
            string Id = data.Get("Id");
            object obj = SqlModel.Select(BASDIC.ID).From(DB.BASDIC).Where(BASDIC.TYPEID == Id).ExecuteScalar();
            if (obj != null && obj.ToString() != "")
            {
                result.Success = false;
                result.Message = "该字典类型下有字典数据，不能删除！";
            }
            else
            {
                BASDICTYPEModel model = new BASDICTYPEModel();
                bool ret = model.Delete(BASDICTYPE.ID == Id);
                result.Success = ret;
            }
            return Json(result);
        }
        #endregion

        #region 获取数据字典数据

        #region 获取号牌种类
        [HttpPost]
        public ActionResult GetLicenseCodeData()
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.LicenseCode)
                .OrderByAsc(BASDIC.SORTNO)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 获取车牌颜色
        [HttpPost]
        public ActionResult GetLicenseTypeData()
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.LicenseType)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 获取驱动方式
        [HttpPost]
        public ActionResult GetDriveModeData()
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                //.LeftJoin(DB.BASDICTYPE).On(BASDICTYPE.ID == BASDIC.TYPEID)
                .Where(BASDIC.TYPECODE == ConstStrings.DriveMode)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 黑名单类型
        [HttpPost]
        public ActionResult GetBlackTypeData()
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.BlackType)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 违规类型
        [HttpPost]
        public ActionResult GetIllegalTypeData()
        {

            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.IllegalType)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 违规处理状态类型
        [HttpPost]
        public ActionResult GetIllegalRealStatusData()
        {

            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.IllegalDealType)
                .ExecToDataTable();
            return Json(dt.AddNoneSelectItem(" ", "全部").ToDynamicList());
        }
        #endregion

        #region 监测机构
        [HttpPost]
        public ActionResult GetDeptInfoData()
        {
            //DataTable dt = SqlModel.Select(t_.ID.As("CODE"), T_MANAGE_DEPT_INFO.ORGNAME.As("TITLE"))
            //    .From(DB.T_MANAGE_DEPT_INFO)
            //    .ExecToDataTable();
            //return Json(dt.ToDynamicList());
            return Json("");
        }

        #region 获取检验方法
        [HttpPost]
        public ActionResult GetTestTypeData(RequestData data)
        {
            string strType = data.Get("Type");
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.TestType)
                .ExecToDataTable();
            if (strType == "1")
                return Json(dt.AddNoneSelectItem("-1", ConstStrings.NoneSelect).ToDynamicList());
            else
                return Json(dt.ToDynamicList());
        }
        #endregion

        #region 获取燃油类型
        [HttpPost]
        public ActionResult GetFuelTypeData()
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.FuelType)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 获取排放
        [HttpPost]
        public ActionResult GetStandardData()
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.Standard)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion
        #endregion

        #region 获取字典项数据-公用
        /// <summary>
        /// 获取字典项数据
        /// </summary>
        /// <param name="data">DicCode-字典项编码</param>
        /// <returns></returns>
        public ActionResult GetDicData(RequestData data)
        {
            string strCode = data.Get("DicCode");
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == strCode & BASDIC.STATUS == 1)
                .OrderByAsc(BASDIC.SORTNO)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 获取报告复查结果
        [HttpPost]
        public ActionResult GetReportReviewResultCodeData()
        {

            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.ReportReviewResult)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 违规类型
        [HttpPost]
        public ActionResult GetManageDeptTypeData()
        {

            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.ManageDeptType)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 燃油规格
        [HttpPost]
        public ActionResult GetFuelSpecData()
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.FuelSpec)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 监测点类型
        [HttpPost]
        public ActionResult GetJZLXData(RequestData data)
        {
            int removeNoneSelect = data.GetInt("RemoveNoneSelect");
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.JZLX)
                .ExecToDataTable();

            if (removeNoneSelect == -1)
            {
                return Json(dt.AddNoneSelectItem(-1, ConstStrings.NoneSelect).ToDynamicList());
            }
            else
            {
                return Json(dt.ToDynamicList());
            }
        }
        #endregion

        #region 视频设备分类
        [HttpPost]
        public ActionResult GetVideoTypeData(RequestData data)
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE, BASDIC.SORTNO)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.VideoType)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 视频设备分类
        [HttpPost]
        public ActionResult GetProviderTypeData(RequestData data)
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE, BASDIC.SORTNO)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.videoType)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion


        #region 巡查主题
        [HttpPost]
        public ActionResult GetPatrolThemeUser(RequestData data)
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE, BASDIC.SORTNO)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.PatrolTheme)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 巡查类型
        [HttpPost]
        public ActionResult GetPatrolTypeUser(RequestData data)
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE, BASDIC.SORTNO)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.PatrolType)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        #region 企业类型
        [HttpPost]
        public ActionResult GetCompanyTypeData(RequestData data)
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE, BASDIC.SORTNO)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == ConstStrings.CompanyType)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }
        #endregion

        /// <summary>
        /// 获取字典
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDicCodeData(string typeCode)
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == typeCode).OrderByAsc(BASDIC.SORTNO)
                .ExecToDataTable();
            return Json(dt.ToDynamicList());
        }

        /// <summary>
        /// 获取字典(查询使用)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetDicCodeDataForSearch(string typeCode)
        {
            DataTable dt = SqlModel.Select(BASDIC.CODE, BASDIC.TITLE)
                .From(DB.BASDIC)
                .Where(BASDIC.TYPECODE == typeCode).OrderByAsc(BASDIC.SORTNO)
                .ExecToDataTable();
            DataRow drRow = dt.NewRow();
            drRow["TITLE"] = "全部";
            drRow["CODE"] = "";
            dt.Rows.InsertAt(drRow, 0);
            return Json(dt.ToDynamicList());
        }

        #endregion
    }
}
