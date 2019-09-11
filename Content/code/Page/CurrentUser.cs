using w.ORM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w.Model;
using System.Data;

namespace UI.Web
{
    /// <summary>
    /// 功能：当前用户类
    /// <para>作者：</para>
    /// <para>日期：2017-3-10 14:33:34</para>
    /// </summary>
    public class CurrentUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public static int Id
        {
            get
            {
                object id = HttpContext.Current.Session["SessionUserIdKey"];
                if (id != null)
                    return Convert.ToInt32(id);
                return 0;
            }
            set
            {
                HttpContext.Current.Session["SessionUserIdKey"] = value;
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public static string UserName
        {
            get
            {
                object name = HttpContext.Current.Session["SessionUserNameKey"];
                if (name != null)
                    return HttpContext.Current.Session["SessionUserNameKey"].ToString();
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionUserNameKey"] = value;
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public static string UserTrueName
        {
            get
            {
                object name = HttpContext.Current.Session["SessionUserTrueNameKey"];
                if (name != null)
                    return HttpContext.Current.Session["SessionUserTrueNameKey"].ToString();
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionUserTrueNameKey"] = value;
            }
        }

        /// <summary>
        /// 监管机构ID（省厅、市局）
        /// </summary>
        public static string Orgid
        {
            get
            {
                object name = HttpContext.Current.Session["SessionOrgidKey"];
                if (name != null)
                    return HttpContext.Current.Session["SessionOrgidKey"].ToString();
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionOrgidKey"] = value;
            }
        }

        /// <summary>
        /// 归属检验机构编号
        /// </summary>
        public static string TSNO
        {
            get
            {
                object name = HttpContext.Current.Session["SessionTSNOKey"];
                if (name != null)
                    return HttpContext.Current.Session["SessionTSNOKey"].ToString();
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionTSNOKey"] = value;
            }
        }

        /// <summary>
        /// 分管检验机构编号
        /// </summary>
        public static string TSNOS
        {
            get
            {
                object name = HttpContext.Current.Session["SessionTSNOSKey"];
                if (name != null)
                    return HttpContext.Current.Session["SessionTSNOSKey"].ToString();
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionTSNOSKey"] = value;
            }
        }

        /// <summary>
        /// 区域代码（具体到区县）
        /// </summary>
        public static string AreaCode
        {
            get
            {
                object name = HttpContext.Current.Session["SessionAreaCodeKey"];
                if (name != null)
                    return HttpContext.Current.Session["SessionAreaCodeKey"].ToString();
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionAreaCodeKey"] = value;
            }
        }

        public static string SysAreaCode
        {
            get
            {
                object name = HttpContext.Current.Session["SessionSysAreaCodeKey"];
                if (name != null)
                    return HttpContext.Current.Session["SessionSysAreaCodeKey"].ToString();
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionSysAreaCodeKey"] = value;
            }
        }

        /// <summary>
        /// 用户类型ID
        /// </summary>
        public static string UserTypeID
        {
            get
            {
                object name = HttpContext.Current.Session["SessionUserTypeIDKey"];
                if (name != null)
                    return HttpContext.Current.Session["SessionUserTypeIDKey"].ToString();
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionUserTypeIDKey"] = value;
            }
        }

        /// <summary>
        /// 用户json数据
        /// </summary>
        public static string UserJson
        {
            get
            {
                object name = HttpContext.Current.Session["SessionUserJson"];
                if (name != null)
                    return HttpContext.Current.Session["SessionUserJson"].ToString();
                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["SessionUserJson"] = value;
            }
        }

        /// <summary>
        /// 是否过期
        /// </summary>
        public static bool IsExpired
        {
            get { return (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserTrueName)); }
        }

        public static string ConfigJson { get; set; }

        public static string ThemeName
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<ConfigModel>(ConfigJson).Theme.Name;
                }
                catch
                {
                    return "default";
                }
            }
        }

        public static int GridRows
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<ConfigModel>(ConfigJson).GridRows;
                }
                catch
                {
                    return 20;
                }
            }
        }

        public static bool Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return false;

            DataTable udt = SqlModel.SelectAll(T_SYS_AREA.AREA_TEXT.As("AREATEXT"),
                    T_SYS_AREA.LICENSE_PLATE_PREFIX.As("PLATEPREFIX")).From(DB.BASUSER)
                    .LeftJoin(DB.T_SYS_AREA).On(BASUSER.AREACODE == T_SYS_AREA.AREA_CODE)
                    .Where(BASUSER.USERNAME == userName).ExecToDataTable();

            if (udt == null || udt.Rows.Count <= 0)
                return false;

            string pwd = StringHelper.MD5String(password);
            if (udt.Rows[0]["PASSWORD"].ToString().Equals(pwd, StringComparison.OrdinalIgnoreCase))
            {
                CurrentUser.Id = udt.Rows[0]["ID"].ToInt32();
                CurrentUser.UserName = udt.Rows[0]["USERNAME"].ToString();
                CurrentUser.UserTrueName = udt.Rows[0]["TRUENAME"].ToString();
                CurrentUser.ConfigJson = udt.Rows[0]["CONFIGJSON"].ToString();
                CurrentUser.Orgid = udt.Rows[0]["ORGID"].ToString();
                CurrentUser.UserTypeID = udt.Rows[0]["USERTYPEID"].ToString();
                CurrentUser.AreaCode = udt.Rows[0]["AREACODE"].ToString();
                try
                {

                    CurrentUser.SysAreaCode = SqlModel.Select(BASDIC.CODE).From(DB.BASDIC).Where(BASDIC.TITLE == "SysAreaCode").ExecuteScalar().ToString();
                }
                catch (Exception)
                {
                }
                udt.Columns.Remove(udt.Columns["PASSWORD"]);//移除密码不显示在界面缓存中
                CurrentUser.UserJson = JsonConvert.SerializeObject(udt.Rows[0].ToDynamic());

                return true;
            }
            return false;
        }

        public static void Logout()
        {
            Id = 0;
            UserName = null;
            UserTrueName = null;
            Orgid = null;
            HttpContext.Current.Response.Redirect("/");
        }
    }
}