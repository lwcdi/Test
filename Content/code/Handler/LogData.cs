using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w.Model;
using w.ORM;

namespace UI.Web
{
    public class LogData : ILog
    {
        public void Throw(LogInfo logInfo)
        {
            BASLOGModel model = new BASLOGModel();
            model.LOGTYPECODE = LogType.ORMBaseOperate.ToString();
            if (logInfo.Remark.Length > 0)
                model.LOGCONTENT = logInfo.Remark.Substring(0, logInfo.Remark.Length > 999 ? 999 : logInfo.Remark.Length);
            model.OPERATEUSERID = CurrentUser.Id;
            model.OPERATETIME = logInfo.LogTime;
            model.Insert();
        }
    }
}