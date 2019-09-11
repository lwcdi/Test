using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace UI.Web
{
    /// <summary>
    /// 序列化处理
    /// author:liwu
    /// </summary>
    public class SerializerHelper
    {
        public static string Serialize(object obj)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            var str = js.Serialize(obj);
            str = Regex.Replace(str, @"\\/Date\((\d+)\)\\/", match =>
            {
                DateTime dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                dt = dt.ToLocalTime();
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            });
            return str;

        }
        public static T Deserialize<T>(string jsonData)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            return js.Deserialize<T>(jsonData);
        }
        public static T Deserialize<T>(string jsonData,List<string> ignoreField)
        {
            var dic = SerializerHelper.Deserialize<Dictionary<string, object>>(jsonData);
            ignoreField.ForEach(field => {
                bool exits = dic.ContainsKey(field);
                if (exits)
                {
                    dic.Remove(field);
                }
            });
            return SerializerHelper.Deserialize<T>(SerializerHelper.Serialize(dic));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <typeparam name="K">数据类型</typeparam>
        /// <param name="jsonData"></param>
        /// <param name="ignoreField"></param>
        /// <returns></returns>
        public static T DeserializeList<T, K>(string jsonData, List<string> ignoreField) where T : List<K>, new()
        {
            List<K> list = SerializerHelper.Deserialize<List<K>>(jsonData);
            T resultList = new T();
            list.ForEach(item => {
                resultList.Add(SerializerHelper.Deserialize<K>(SerializerHelper.Serialize(item), ignoreField));
            });
            return resultList;
        }
        /// <summary>
        /// 将动态类型对象转换成静态类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T DynamicObjectToStaticObject<T>(dynamic input) where T : new()
        {
            return Deserialize<T>(Serialize(input));
        }
        /// <summary>
        /// 将动态类型对象集合转换成静态类型对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static List<T> DynamicObjectToStaticObjectList<T>(List<dynamic> inputList) where T : new()
        {
            List<T> resultList = new List<T>();
            if (!(inputList != null && inputList.Count > 0)) return resultList;
            inputList.ForEach(item => {
                T temp = Deserialize<T>(Serialize(item));
                resultList.Add(temp);
            });
            return resultList;
        }
    }
}