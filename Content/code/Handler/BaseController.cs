using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using w.Model;
using w.ORM;

namespace UI.Web
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 组建分页语句
        /// 备注：当页码和每页数量都为0时，不产生分页语句
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="intPageIndex">页码</param>
        /// <param name="intPageSize">每页记录数</param>
        /// <returns></returns>
        public static string BuildPagerExpress(string strSQL, int intPageIndex, int intPageSize)
        {
            string sql = @"Select * From (Select T.*,rownum rn From ({0}) T  where rownum<={1}) N where N.rn > {2}";
            sql = String.Format(sql, strSQL, intPageSize * intPageIndex, (intPageIndex - 1) * intPageSize);
            return sql;

        }

        private string LoginUrl = "/Bas/Home/Login";
        protected string LastTime = " 23:59:59";
        /// <summary>
        /// 删除标识值
        /// </summary>
        protected int DeleteFlag = 1; //删除标识值

        protected IData Db
        {
            get
            {
                return DataFactory.Instance.Create(new DBProvider());
            }
        }

        protected string NewGuid
        {
            get
            {
                return Guid.NewGuid().ToString().Replace("-", "");
            }
        }

        protected string GetNowDate()
        {
            return DateTime.Now.ToString("yyyy/MM/dd");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Type controllerType = filterContext.Controller.GetType();
            string actionName = filterContext.ActionDescriptor.ActionName;

            if (actionName.Equals("Login") == false && CurrentUser.IsExpired == true)
            {
                string jsStr = "";
                if (controllerType == typeof(UI.Web.Areas.Bas.Controllers.HomeController) && actionName.Equals("Index"))
                {
                    jsStr = "<script>top.location='" + this.LoginUrl + "';</script>";
                }
                else
                {
                    jsStr = "<script>alert('登录超时,请重新登录！') ;top.location='" + this.LoginUrl + "';</script>";
                }
                base.Response.Write(jsStr);
                Response.End();
            }
        }

        public JsonResult PagedJson(SqlModel sqlModel, RequestData data)
        {
            int page = this.GetPage(data);
            int rows = this.GetRows(data);

            OrderByModel om = this.GetOrderBy(data, sqlModel);
            PagedDataTable pdt = sqlModel.ExecToPagedTable(page, rows, om);
            dynamic result = pdt.ToDynamic();
            return Json(result);
        }
        /// <summary>
        /// 支持用DataTable
        /// </summary>
        /// <param name="sqlModel"></param>
        /// <param name="data"></param>
        /// <param name="dtData"></param>
        /// <returns></returns>
        public JsonResult PagedJson(SqlModel sqlModel, RequestData data, DataTable dtData)
        {
            int page = this.GetPage(data);
            int rows = this.GetRows(data);
            OrderByModel om = this.GetOrderBy(data, sqlModel);
            PagedDataTable pdt = sqlModel.ExecToPagedTable(page, rows, om, dtData);
            dynamic result = pdt.ToDynamic();
            return Json(result);
        }

        protected int GetPage(RequestData data)
        {
            int page = 1;
            if (data.Get("page") != "")
            {
                page = int.Parse(data.Get("page"));
            }
            return page;
        }

        protected int GetRows(RequestData data)
        {
            int rows = 0;
            if (data.Get("rows") != "")
            {
                rows = int.Parse(data.Get("rows"));
            }
            return rows;
        }

        protected OrderByModel GetOrderBy(RequestData data, SqlModel sqlModel)
        {
            OrderByModel om = new OrderByModel
            {
                OrderType = data.Get("order") == "desc" ? OrderType.Desc : OrderType.Asc,
                FieldModel = data.Get("sort").ToField(sqlModel.FromTable)
            };
            return om;
        }

        /// <summary>
        /// 获取自动生成编号[默认后面4位自增流水，可重载]
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="prefix">标识符</param>
        /// <param name="format">格式</param>
        /// <param name="custom">自定义数据</param>
        /// <returns></returns>
        protected string GetFormNo(string prefix, string format, string custom)
        {
            try
            {
                DateTime now = DateTime.Now;
                string curDateYMD = now.ToString(format);
                int curSNValue = 0;
                DataRowModel r = SqlModel.SelectAll().From(DB.BASFORMNO)
                    .Where(BASFORMNO.PREFIX == prefix & BASFORMNO.RULEVALUE == curDateYMD).ExecToRowModel();

                //如果没有数据，则先添加一条初始数据
                if (r == null || r.Count == 0)
                {
                    curSNValue = 1;

                    BASFORMNOModel fnModel = new BASFORMNOModel();
                    fnModel.PREFIX = prefix;
                    fnModel.RULENAME = format;// "yyyyMMdd";
                    fnModel.RULEVALUE = now.ToString(fnModel.RULENAME);
                    fnModel.SNBIT = 4;
                    fnModel.SNVALUE = curSNValue;
                    //fnModel.TSNO = tsno;
                    fnModel.Insert();
                    //fnModel.Inserting(tran);
                }
                else
                {
                    curSNValue = r[BASFORMNO.SNVALUE].ToInt32() + 1;

                    BASFORMNOModel fModel = new BASFORMNOModel();
                    fModel.SNVALUE = curSNValue;
                    fModel.Update(BASFORMNO.ID == r[BASFORMNO.ID].ToInt32());
                    //fModel.Updating(tran, BASFORMNO.ID == r[BASFORMNO.ID].ToInt32());
                }

                string billNumber = string.Format("{0}{1}{2}"
                    , curDateYMD
                    , custom
                    , curSNValue.ToString().PadLeft(4, '0'));

                return billNumber;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 保存表单号数据
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="formNo"></param>
        //protected void SaveFormNo(TranModel tran, FormNo formNo)
        //{
        //    BasFormNoModel fModel = new BasFormNoModel();
        //    fModel.RuleValue = formNo.RuleValue;
        //    fModel.SNValue = formNo.SNValue + 1;
        //    fModel.Updating(tran, BasFormNo.ID == formNo.ID);
        //}

        //获取授权
        protected JArray GetNavBtnJson(string roleIdStr, int userId)
        {
            DataTable navTab = SqlModel.Select(BASNAV.ID, BASNAV.PARENTID, BASNAV.NAVTITLE, BASNAV.ICONCLS, BASNAV.SORTNO)
                .From(DB.BASNAV).ExecToDataTable();
            DataTable btnTab = SqlModel.Select(BASBUTTON.ID, BASBUTTON.BUTTONTAG)
                .From(DB.BASBUTTON).OrderByAsc(BASBUTTON.SORTNO).ExecToDataTable();
            DataTable nbTab = SqlModel.Select(BASNAVBTN.NAVID, BASNAVBTN.BTNID, BASBUTTON.BUTTONTAG, BASNAVBTN.SORTNO)
                .From(DB.BASNAVBTN)
                .LeftJoin(DB.BASBUTTON).On(BASBUTTON.ID == BASNAVBTN.BTNID)
                .ExecToDataTable();
            DataTable rTab = this.GetRightTable(roleIdStr, userId);
            JArray list = this.GetNavBtnList(0, navTab, btnTab, nbTab, rTab);
            return list;
        }

        protected DataTable GetRightTable(string roleIdStr, int userId)
        {
            if (string.IsNullOrEmpty(roleIdStr) == true)
            {
                roleIdStr = "0";
            }
            DataTable rTab = SqlModel.Select("T".Field(BASROLENAVBTN.NAVID), "T".Field(BASROLENAVBTN.BTNID))
                    .From
                    (
                        (SqlModel.Select(BASROLENAVBTN.NAVID, BASROLENAVBTN.BTNID).From(DB.BASROLENAVBTN).Where(BASROLENAVBTN.ROLEID.In(roleIdStr)))
                        .Union
                        (SqlModel.Select(BASUSERNAVBTN.NAVID, BASUSERNAVBTN.BTNID).From(DB.BASUSERNAVBTN).Where(BASUSERNAVBTN.USERID == userId))
                        .As("T")
                    ).ExecToDataTable();
            return rTab;
        }

        private JArray GetNavBtnList(int parentId, DataTable navTab, DataTable btnTab, DataTable nbTab, DataTable rTab)
        {
            JArray list = new JArray();
            DataRow[] navRows = navTab.Select("ParentID=" + parentId.ToString(), "SortNo asc");
            JObject obj = null;
            foreach (DataRow navr in navRows)
            {
                int navId = navr[BASNAV.ID.Name].ToInt32();
                DataRow[] nbRows = nbTab.Select("NavId=" + navId, "SortNo asc");
                JArray btns = new JArray();
                nbRows.Each((r, i) => btns.Add(new JValue(r[BASBUTTON.BUTTONTAG.Name])));
                obj = new JObject(
                        new JProperty("Id", navId),
                        new JProperty("NavTitle", navr[BASNAV.NAVTITLE.Name]),
                        new JProperty("iconCls", navr[BASNAV.ICONCLS.Name]),
                        new JProperty("Buttons", btns),
                        new JProperty("children", GetNavBtnList(navId, navTab, btnTab, nbTab, rTab))
                    );
                btnTab.Each((r, i) =>
                {
                    DataRow[] rRows = rTab.Select("NavId=" + navId + " and BtnId=" + r[BASBUTTON.ID.Name].ToString());
                    obj.Add(new JProperty(r[BASBUTTON.BUTTONTAG.Name].ToString(), rRows != null && rRows.Length > 0 ? "√" : "x"));
                });
                list.Add(obj);
            }
            return list;
        }

        //protected void AddLog(ActionType actionType, dynamic data)
        //{
        //    WebHelp.Instance.AddLog(actionType, data);
        //}

        /// <summary>
        /// 设置datagrid过滤条件
        /// </summary>
        /// <param name="item">参数</param>
        /// <param name="fielName">字段名称</param>
        /// <param name="fieldModel"></param>
        /// <param name="where">SQL</param>
        protected void SetWhere(dynamic item, string fielName, FieldModel fieldModel, ref FieldModel where)
        {
            //where &= 1 == 1;
            if (item.field == fielName && item.value.ToString() != "")
            {
                if (item.op == "contains")
                {
                    where &= fieldModel.Like(item.value.ToString());
                }
                if (item.op == "equal")
                {
                    where &= fieldModel == item.value;
                }
                if (item.op == "less")
                {
                    where &= fieldModel < item.value;
                }
                if (item.op == "greater")
                {
                    where &= fieldModel > item.value;
                }
                //2017.5.5 姜拓
                if (item.op == "notin")
                {
                    where &= fieldModel.NotIn(item.value);
                }
                //2017.05.31 liwu
                if (item.op == "notEqual")
                {
                    where &= fieldModel != item.value;
                }
            }
        }
        /// <summary>
        /// 设置datagrid过滤条件(优化通用)
        /// </summary>
        /// <param name="item">参数</param>
        /// <param name="fielName">字段名称</param>
        /// <param name="fieldModel"></param>   
        /// <param name="where">SQL</param>
        protected void SetWhere<T>(dynamic item, T proClass, ref FieldModel where)
        {
            foreach (MemberInfo memberItem in proClass.GetType().GetMembers())
            {
                if (memberItem.DeclaringType.Name == proClass.GetType().Name && memberItem.Name != ".ctor")
                {
                    FieldModel model = new FieldModel();
                    model.Name = memberItem.Name;
                    model.TableName = memberItem.DeclaringType.Name;
                    SetWhere(item, model.Name, model, ref where);
                }
            }

        }



        /// <summary>
        /// 添加日志
        /// </summary>
        protected void AddLog(LogType logType, string logContent)
        {
            this.AddLog(null, logType, logContent);
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        protected void AddLog(TranModel tran, LogType logType, string logContent)
        {
            BASLOGModel model = new BASLOGModel();
            model.LOGTYPECODE = logType.ToString();
            model.LOGCONTENT = logContent;
            model.OPERATEUSERID = CurrentUser.Id;
            //model.OPERATETIME = DateTime.Now;
            model.OPERATEIP = this.GetIP();
            if (tran == null)
            {
                model.Insert();
            }
            else
            {
                model.Inserting(tran);
            }
        }

        protected string GetIP()
        {
            if (string.IsNullOrEmpty(base.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) == false)
            {
                return base.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            else
            {
                return base.Request.ServerVariables["REMOTE_ADDR"];
            }
        }

        public string ModelToString(object t)
        {
            string result = "";
            StringBuilder sb = new StringBuilder();
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (properties.Length <= 0)
            {
                return "";
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;                             //名称
                if (item.PropertyType == typeof(IDbProvider))
                    continue;

                object value = item.GetValue(t, null);       //值

                if (value != null && value.ToString().Length > 0) //jsonDic.Add(name, value);
                {
                    result = result.Add(",", name + ":" + value);
                }
            }
            return result;
        }

        protected string GetString(dynamic value)
        {
            try
            {
                return Convert.ToString(value);
            }
            catch
            {
                return "";
            }
        }

        protected int GetInt(object value)
        {
            try
            {
                return int.Parse(value.ToString());
            }
            catch
            {
                return 0;
            }
        }

        protected decimal GetDecimal(object value)
        {
            try
            {
                return decimal.Parse(value.ToString());
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换人民币大小金额
        /// </summary>
        /// <param name="num">金额</param>
        /// <returns>返回大写形式</returns>
        public string CNRmb(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字
            string str3 = "";    //从原num值中取出的值
            string str4 = "";    //数字的字符串形式
            string str5 = "";  //人民币大写金额形式
            int i;    //循环变量
            int j;    //num的值乘以100的字符串长度
            string ch1 = "";    //数字的汉语读法
            string ch2 = "";    //数字位的汉字读法
            int nzero = 0;  //用来计算连续的零值是几个
            int temp;            //从原num值中取出的值

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式
            j = str4.Length;      //找出最高位
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分

            //循环取出每一位需要转换的值
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值
                temp = Convert.ToInt32(str3);      //转换为数字
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位，则必须写上
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整”
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }
        protected T GetModelValue<T>(T model, RequestData data)
        {
            foreach (PropertyInfo objInfo in model.GetType().GetProperties())
            {
                if (data.AllKeys.Contains(objInfo.Name))
                {
                    string propertyType = objInfo.PropertyType.Name;
                    if (objInfo.PropertyType.IsGenericType&& objInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {//属性类型为?
                        propertyType = objInfo.PropertyType.GetGenericArguments()[0].Name;
                    }
                    switch (propertyType)
                    {
                        case "Decimal":
                        case "Nullable`1":
                            objInfo.SetValue(model, data.GetDecimal(objInfo.Name), null);
                            break;
                        case "DateTime":
                            object value = data.Get(objInfo.Name);
                            if (null == value || string.IsNullOrEmpty(value.ToString())) continue;
                            objInfo.SetValue(model, data.GetDateTime(objInfo.Name), null);// 
                            break;
                        default:
                            objInfo.SetValue(model, data.Get(objInfo.Name), null);
                            break;
                    }
                }

            }

            return model;
        }
        /// <summary>
        /// 拼装AJAX返回JSON数据 
        /// author:liwu 2018-01-08
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        protected JsonResult ErrorResult(string errorMsg)
        {
            PostResult result = new PostResult();
            result.Success = false;
            result.Message = errorMsg;
            result.Data = "";
            return Json(result);
        }
        /// <summary>
        /// 拼装AJAX返回JSON数据 
        /// author:liwu 2018-01-08
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        protected JsonResult SuccessResult(string Msg)
        {

            PostResult result = new PostResult();
            result.Success = true;
            result.Message = Msg;
            result.Data = "";
            return Json(result);
        }
        /// <summary>
        /// 拼装AJAX返回JSON数据 
        /// author:liwu 2018-01-08
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected JsonResult SuccessResult(string Msg, object data)
        {

            PostResult result = new PostResult();
            result.Success = true;
            result.Message = Msg;
            result.Data = data;
            return Json(result);
        }
    }

    public class RequestData : NameValueCollection
    {
        private string IgnoreString = "_";

        public RequestData()
        {
            var parms = HttpContext.Current.Request.Params;
            string key = null;
            string[] vals = null;
            for (int i = 0; i < parms.AllKeys.Length; i++)
            {
                key = parms.GetKey(i);
                if (key != null)// && key.Contains(this.IgnoreString) == false
                {
                    vals = parms.GetValues(key);
                    if (vals != null && vals.Length > 0)
                    {
                        this.Add(key, vals[0]);
                    }
                }
            }
           if(!this.AllKeys.Contains("isPagedJson")) 
            this.Add("isPagedJson", "false");
        }

        public override string Get(string name)
        {
            string value = base.Get(name);
            if (value != null)
                return value;
            return "";
        }

        public string Get(string name, string defaultValue)
        {
            string value = base.Get(name);
            if (value != null)
                return value;
            return defaultValue;
        }

        public int GetInt(string name)
        {
            string value = base.Get(name);
            try
            {
                return int.Parse(value);
            }
            catch
            {
                return 0;
            }
        }

        public int? GetInt(string name, int? defaultValue)
        {
            string dat = base.Get(name);
            int val;
            if (!string.IsNullOrEmpty(dat) && int.TryParse(dat, out val))
                return val;
            else
                return defaultValue;
        }

        public decimal GetDecimal(string name)
        {
            string value = base.Get(name);
            try
            {
                return decimal.Parse(value);
            }
            catch
            {
                return 0;
            }
        }

        public decimal? GetDecimal(string name, decimal? defaultValue)
        {
            string dat = base.Get(name);
            decimal val;
            if (!string.IsNullOrEmpty(dat) && decimal.TryParse(dat, out val))
                return val;
            else
                return defaultValue;
        }

        public bool GetBoolean(string name)
        {
            string value = base.Get(name);
            try
            {
                return bool.Parse(value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool? GetBoolean(string name, bool? defaultValue)
        {
            string dat = base.Get(name);
            bool val;
            if (!string.IsNullOrEmpty(dat) && bool.TryParse(dat, out val))
                return val;
            else
                return defaultValue;
        }

        public DateTime GetDateTime(string name)
        {
            string value = base.Get(name);
            try
            {
                return DateTime.Parse(value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DateTime? GetDateTime(string name, DateTime? defaultValue)
        {
            string dat = base.Get(name);
            DateTime val;
            if (!string.IsNullOrEmpty(dat) && DateTime.TryParse(dat, out val))
                return val;
            else
                return defaultValue;
        }
    }

    /// <summary>
    /// 表单号实体类
    /// </summary>
    public class FormNo
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 规则值
        /// </summary>
        public string RuleValue { get; set; }
        /// <summary>
        /// 流水号值
        /// </summary>
        public int SNValue { get; set; }
        /// <summary>
        /// 完整的表单号码
        /// </summary>
        public string No { get; set; }

        public FormNo()
        {
            this.ID = 0;
            this.RuleValue = "";
            this.SNValue = 0;
            this.No = "";
        }
    }
}