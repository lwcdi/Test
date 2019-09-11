using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using w.Model;
using w.ORM;

public class SmsHelper
{
    public static bool SendSms(string destAddr, string messageContent)
    {

        messageContent = System.Configuration.ConfigurationManager.AppSettings["SmsTemplate"].Replace("@", messageContent);
        string type = "pt";
        string strSmsRequest = System.Configuration.ConfigurationManager.AppSettings["SmsUrl"]
            + "?name=" + System.Configuration.ConfigurationManager.AppSettings["SmsName"]
            + "&pwd=" + System.Configuration.ConfigurationManager.AppSettings["SmsPassWord"];
        strSmsRequest += "&content=" + messageContent + "&mobile=" + destAddr + "&type=" + type;

        #region    访问url获取url返回的字符串
        WebRequest request = WebRequest.Create(strSmsRequest);//创建访问网页的字符串
        WebResponse response = request.GetResponse();//当子类被重写时返回来自Internet资源的响应
        Stream stream = response.GetResponseStream();//返回来自Internet的数据
        StreamReader sr = new StreamReader(stream, Encoding.GetEncoding("utf-8"));//读取流数据
        string strReturn = sr.ReadToEnd();

        #endregion
        #region  分析返回值
        List<string> listreturn = strReturn.Split(',').ToList();
        switch (listreturn[0].ToString())
        {
            case "0":
                //LogHelper.WriteLog("提交成功！");
                //更新发送成功
                return true;
            case "1":
                //LogHelper.WriteLog("含有敏感词汇！");
                break;
            case "2":
                //LogHelper.WriteLog("余额不足！");
                break;
            case "3":
                //LogHelper.WriteLog("没有号码！");
                break;
            case "4":
                //LogHelper.WriteLog("包含sql语句！");
                break;
            case "10":
                //LogHelper.WriteLog("账号不存在！");
                break;
            case "11":
                //LogHelper.WriteLog("账号注销！");
                break;
            case "12":
                //LogHelper.WriteLog("账号停用！");
                break;
            case "13":
                //LogHelper.WriteLog("IP鉴权失败！");
                break;
            case "14":
                //LogHelper.WriteLog("格式错误！");
                break;
            case "-1":
                //LogHelper.WriteLog("系统异常！");
                break;
            default:
                //LogHelper.WriteLog("其他异常！");
                break;
        }
        #endregion
        return false;
    }

    public static bool AddToSendSms(string receiver, eTemplateType tempType, params string[] args)
    {
        string template = GetSmsTemplate(tempType);
        if (template.Equals("")) return false;
        string content = string.Format(template, args);
        SMSTOSENDModel model = new SMSTOSENDModel();
        model.CONTENT = content;
        model.CREATETIME = DateTime.Now;
        model.CREATEUSER = UI.Web.CurrentUser.UserName;
        model.RECEIVEUSER = receiver;
        model.SMSBASTYPE = "";
        model.SMSTEMPTYPE = ((int)tempType).ToString();
        model.STATUS = "0";

        return model.Insert();
    }

    private static string GetSmsTemplate(eTemplateType tempType)
    {
        object content = SqlModel.Select(SMSTEMPLATE.TEMPCONTENT).From(DB.SMSTEMPLATE)
            .Where(SMSTEMPLATE.SMSTEMPTYPE == (int)tempType
            & SMSTEMPLATE.ISUSE == "1")
            .ExecuteScalar();
        return content == null ? "" : content.ToString();
    }

    //public enum eBasType
    //{
    //    调度任务 = 0,
    //    任务催办 = 1
    //}
    public enum eTemplateType
    {
        新增调度任务 = 0,
        新增任务催办
    }
}

