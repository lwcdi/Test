using w.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Script.Serialization;

namespace UI.Web
{
    public abstract class ListController : BaseController
    {
        /// <summary>
        /// 用于返回更多的操作数据（CURD）
        /// </summary>
        protected object OperateReturnData = null;
        public virtual ActionResult List(int? navId)
        {
            ViewData["UserTypeID"] = CurrentUser.UserTypeID;
            return View(navId ?? 0);
        }
        public virtual ActionResult ListExt(RequestData data)
        {
            return View("List", 0);
        }
        public virtual ActionResult Edit()
        {
            return View();
        }

        protected abstract SqlModel GetSqlModel(RequestData data);
        //protected abstract ActionResult GetSqlModelDataTable(RequestData data);

        protected virtual string BeforeSave(ActionType actionType, RequestData data)
        {
            return "";
        }

        protected virtual void AfterSave(ActionType actionType, RequestData data)
        {
        }

        protected abstract bool DoAdd(RequestData data);

        protected abstract bool DoEdit(RequestData data);

        protected abstract bool DoDelete(RequestData data);

        [HttpPost]
        public virtual ActionResult List(RequestData data)
        {
            // if (bool.Parse(data["isPagedJson"]))
            //  return this.GetSqlModelDataTable(data);
            // else
            // {
            SqlModel sqlModel = this.GetSqlModel(data);
            if (sqlModel == null) return new EmptyResult();

            return base.PagedJson(sqlModel, data);
            // }
        }




        [HttpPost]
        public virtual ActionResult Add(RequestData data)
        {
            PostResult result = new PostResult();
            string valRet = this.BeforeSave(ActionType.Add, data);
            if (valRet == "")
            {
                bool ret = this.DoAdd(data);
                if (ret == true)
                {
                    result.Success = true;
                    result.Data = "1";
                    result.Message = Lang.add_success;
                    this.AfterSave(ActionType.Add, data);
                }
                else
                {
                    result.Success = false;
                    result.Data = "0";
                    result.Message = Lang.add_failure;
                }
            }
            else
            {
                result.Success = false;
                result.Data = "0";
                result.Message = valRet;
            }
            result.ExtendData = OperateReturnData;
            return Json(result);
        }

        [HttpPost]
        public virtual ActionResult Edit(RequestData data)
        {
            PostResult result = new PostResult();
            string valRet = this.BeforeSave(ActionType.Edit, data);
            if (valRet == "")
            {
                bool ret = this.DoEdit(data);
                if (ret == true)
                {
                    result.Success = true;
                    result.Data = "1";
                    result.Message = Lang.edit_success;
                    this.AfterSave(ActionType.Edit, data);
                }
                else
                {
                    result.Success = false;
                    result.Data = "0";
                    result.Message = Lang.edit_failure;
                }
            }
            else
            {
                result.Success = false;
                result.Data = "0";
                result.Message = valRet;
            }
            result.ExtendData = OperateReturnData;
            return Json(result);
        }

        [HttpPost]
        public virtual ActionResult Delete(RequestData data)
        {
            PostResult result = new PostResult();
            string valRet = this.BeforeSave(ActionType.Delete, data);
            if (valRet == "")
            {
                bool ret = this.DoDelete(data);
                if (ret == true)
                {
                    result.Success = true;
                    result.Data = "1";
                    result.Message = Lang.del_success;
                    this.AfterSave(ActionType.Delete, data);
                }
                else
                {
                    result.Success = false;
                    result.Data = "0";
                    result.Message = Lang.del_failure;
                }
            }
            else
            {
                result.Success = false;
                result.Data = "0";
                result.Message = valRet;
            }
            result.ExtendData = OperateReturnData;
            return Json(result);
        }


        /// <summary>
        /// DataTable 转换为 Json 字符串
        /// </summary>
        /// <param name="dtSource">数据集</param>
        /// <returns>JSON 字符串</returns>
        public string DataTableToJson(DataTable dtSource)
        {
            if (dtSource == null) return "";
            List<object> listRows = new List<object>();
            foreach (DataRow dr in dtSource.Rows)
            {
                Dictionary<string, object> diRow = new Dictionary<string, object>();
                foreach (DataColumn dc in dtSource.Columns)
                {
                    diRow.Add(dc.ColumnName, dr[dc].ToString());
                }
                listRows.Add(diRow);
            }
            return ToJson(listRows);
        }
        /// <summary>
        /// 对象转换为 Json
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>JSON字符串</returns>
        public string ToJson(object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(obj);
        }
    }
}