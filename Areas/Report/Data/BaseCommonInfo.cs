using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w.Model;
using w.ORM;

namespace UI.Web
{
    public class BaseCommonInfo
    {
        /// <summary>
        /// 获取企业（或排口）的名字和编号
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static dynamic GetCompanyOrPK(string type,string id)
        {

            if ("P" == type)
            {
                SqlModel sqlmodel = SqlModel.Select(T_BASE_COMPANY_PK.CODE, T_BASE_COMPANY_PK.NAME).From(DB.T_BASE_COMPANY_PK).Where(T_BASE_COMPANY_PK.ID == id);
                var list = sqlmodel.ExecToDynamicList();
                if (list.Count > 0)
                {
                    return new {
                        Code = StringHelper.DynamicToString(list[0]["CODE"]),
                        Name = StringHelper.DynamicToString(list[0]["NAME"]),
                    };

                }
            }
            else
            {
                SqlModel sqlmodel = SqlModel.Select(T_BASE_COMPANY.ID, T_BASE_COMPANY.NAME).From(DB.T_BASE_COMPANY).Where(T_BASE_COMPANY.ID ==id);
                var list = sqlmodel.ExecToDynamicList();
                if (list.Count > 0)
                {
                    return new
                    {
                        Code = StringHelper.DynamicToString(list[0]["ID"]),
                        Name = StringHelper.DynamicToString(list[0]["NAME"]),
                    };
                }
            }
            return new
            {
                Code = "",
                Name = ""
            };

        }
    }
}