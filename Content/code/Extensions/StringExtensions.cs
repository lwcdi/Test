using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 字符串链接
        /// </summary>
        /// <param name="value"></param>
        /// <param name="split">间隔符</param>
        /// <param name="addstr">链接字符串</param>
        /// <returns></returns>
        public static string Add(this string value, string split, string addstr)
        {
            if (value.Length > 0)
            {
                value += split + addstr;
            }
            else
            {
                value = addstr;
            }
            return value;
        }

        public static string PadChar(this string value, int len)
        {
            return PadChar(value, len, '　');
        }

        public static string PadChar(this string value, int len, char chr)
        {
            //int bytelen = System.Text.Encoding.GetEncoding("GB2312").GetByteCount(value);

            return value + "".PadRight(len - value.Length, chr);
        }

        public static bool isNull(this object value)
        {
            if (value == null | string.IsNullOrEmpty(value.ToString()))
                return true;
            return false;
        }

    }
}