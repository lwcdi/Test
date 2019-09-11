using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web
{
    public class AppJsonResult
    {
        /// <summary>
        /// true/false(成功/失败)
        /// </summary>
        public string result { get; set; }
        /// <summary>
        ///  0 成功，1 验证用户失败，3 返回数据无效
        /// </summary>
        public string errorCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errorInfo { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public object data { get; set; }
    }
}