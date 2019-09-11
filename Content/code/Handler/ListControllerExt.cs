using w.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace UI.Web
{
    public abstract class ListControllerExt : BaseController
    {
        public virtual ActionResult List(int? navId)
        {
           
            ViewData["UserTypeID"] = CurrentUser.UserTypeID;    
            return View(navId ?? 0);
        }

        public virtual ActionResult Edit()
        {
            return View();
        }
        public virtual ActionResult ListExt(RequestData data)
        {
            return View("List", 0);
        }
        protected abstract SqlModel GetSqlModel(RequestData data);
        protected abstract DataTable GetSqlModelDataTable(RequestData data);

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
            SqlModel sqlModel = null;
            DataTable dtModel = null;
            dtModel = this.GetSqlModelDataTable(data);
            sqlModel = this.GetSqlModel(data);
            if (null!= dtModel)
                return base.PagedJson(sqlModel, data,dtModel);
            else if(null!=sqlModel)
               return base.PagedJson(sqlModel, data);
            else//默认
                return base.PagedJson(sqlModel, data);

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
            return Json(result);
        }
    }
}