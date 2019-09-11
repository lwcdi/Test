using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Web;
using System.Collections;

namespace UI.Web
{
    public class StringHelper
    {
        /// <summary>
        /// MD5string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5String(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        }
        
        /// <summary>
        /// 将字符串转成sql的查询条件 例如：1,2,3 =====> '1','2','3'
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SqlInCondition(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            StringBuilder sql = new StringBuilder();
            new List<string>(str.Split(new char[] { ',' })).ForEach(c => {
                sql.Append(",").Append(string.Format("'{0}'", c));
            });
            sql.Remove(0, 1);
            return sql.ToString();
        }
        public static string SqlInCondition<T>(List<T> list)
        {
            if (!(list != null && list.Count>0))
            {
                return "";
            }
            StringBuilder sql = new StringBuilder();
            List<dynamic> Exitlist = new List<dynamic>();
            list.ForEach(c =>
            {
                if (!Exitlist.Contains(c))
                {
                    sql.Append(string.Format(",'{0}'", c));
                    Exitlist.Add(c);
                }

            });
            sql.Remove(0, 1);
            return sql.ToString();
        }
        /// <summary>
        /// 动态类型数据转字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string DynamicToString(dynamic obj)
        {
            if (obj is DBNull || obj == null)
            {
                return "";
            }
            if (obj is DateTime)
            {
                return obj.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return obj.ToString();
        }
        public static string DynamicToString<T>(Dictionary<string, T> obj, string key)
        {
            if (obj == null) return string.Empty;
            try
            {
                return obj[key].ToString();
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        ///  动态类型数据转浮点型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double DynamicToDouble(dynamic obj)
        {
            if (obj is DBNull || obj == null)
            {
                return 0;
            }
            if (obj is DateTime)
            {
                return 0;
            }
            try
            {
                return double.Parse(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        ///  动态类型数据转浮点型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal DynamicToDecimal(dynamic obj)
        {
            if (obj is DBNull || obj == null)
            {
                return 0;
            }
            if (obj is DateTime)
            {
                return 0;
            }
            try
            {
                return decimal.Parse(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 动态类型数据转整型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int DynamicToInt(dynamic obj)
        {
            if (obj is DBNull || obj == null)
            {
                return 0;
            }
            if (obj is DateTime)
            {
                return 0;
            }
            try
            {
                return int.Parse(obj.ToString());
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 向前补位
        /// </summary>
        /// <param name="nunber"></param>
        /// <returns></returns>
        public static string NumberAddBit(int number,int length)
        {
            int numberLength = number.ToString().Length;
            StringBuilder sb = new StringBuilder(number.ToString());
            for(int i = length- numberLength; i > 0; i--)
            {
                sb.Insert(0, "0");
            }
            return sb.ToString();
        }
    }
}

