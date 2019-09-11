using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;

namespace UI.Web
{
    /// <summary>
    /// 语言包帮助类
    /// <para>作者：</para>
    /// <para>日期：2016.2.3</para>
    /// </summary>
    public class LangHelper
    {
        public static string Name { get; set; }
        public static bool IsRead = false;

        #region Instance
        private static LangHelper _lang = null;

        public static LangHelper Instance
        {
            get
            {
                if (_lang == null)
                {
                    _lang = new LangHelper();
                }
                return _lang;
            }
        }
        #endregion

        private LangHelper() { }

        public void Read(HttpRequestBase request)
        {
            this.Read(request, true);
        }

        public void Read(HttpRequestBase request, bool reRead)
        {
            if ((reRead == false && LangHelper.IsRead == false) || reRead == true)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    string path = request.MapPath("~/content/lang/lang.xml");
                    doc.Load(path);
                    this.SetData(doc);
                    LangHelper.IsRead = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void SetData(XmlDocument doc)
        {
            if (string.IsNullOrEmpty(LangHelper.Name) == true)
            {
                LangHelper.Name = "en";
            }
            PropertyInfo[] infos = typeof(Lang).GetProperties(BindingFlags.Public | BindingFlags.Static);
            XmlNode node = null;
            foreach (PropertyInfo p in infos)
            {
                try
                {
                    node = doc.SelectSingleNode("xml/lang[@code='" + p.Name + "']");
                    if (node != null)
                    {
                        p.SetValue(this, node.Attributes[LangHelper.Name].Value, null);
                    }
                }
                catch (Exception)
                { 
                }
            }
        }
    }

    public enum LangEnum
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        ZHCN,
        /// <summary>
        /// 英文
        /// </summary>
        EN
    }
}