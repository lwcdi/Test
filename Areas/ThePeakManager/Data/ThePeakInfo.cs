using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w.Model;
using w.ORM;

namespace UI.Web
{
    public class ThePeakInfo
    {
        /// <summary>
        /// 根据时间获取处于错峰的企业
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<T_THEPEAK_ENT_SUB_LISTModel> GetThePeakEnterprise(DateTime time)
        {
            Dictionary<string, List<DateTime>> peakTime = new Dictionary<string, List<DateTime>>();
            SqlModel sql = SqlModel.SelectAll()
                .From(DB.T_THEPEAK_ENT_SUB_LIST)
                .LeftJoin(DB.T_THEPEAK_MAIN_LIST_INFO).On(T_THEPEAK_MAIN_LIST_INFO.ID == T_THEPEAK_ENT_SUB_LIST.THEPEAKID)
                .Where(
                    T_THEPEAK_MAIN_LIST_INFO.START_TIME <= time
                    & T_THEPEAK_MAIN_LIST_INFO.END_TIME >= time
                    & T_THEPEAK_MAIN_LIST_INFO.PLAN_TYPE == "0"
                    & T_THEPEAK_MAIN_LIST_INFO.IS_CLOSE == '0'
                );
            var peakEnterpriseList = SerializerHelper.Deserialize<List<T_THEPEAK_ENT_SUB_LISTModel>>(SerializerHelper.Serialize(sql.ExecToDynamicList()));
            #region 根据企业的管制类型，再次判断是否处于错峰时间,将非该时间错峰的企业移除
            for (int i = 0; peakEnterpriseList != null && i < peakEnterpriseList.Count; i++)
            {
                T_THEPEAK_ENT_SUB_LISTModel enterprise = peakEnterpriseList[i];
                //停产和限产
                if (3 != enterprise.ENT_PEAK_TYPE)
                {
                    peakTime.Add(enterprise.ID.ToString(), new List<DateTime>(){
                                DateTime.Now.Date,
                                DateTime.Now.Date.AddDays(1).AddSeconds(-1)
                            });
                    continue;
                }
                //时间段限产的
                //00:00 ~ 00:00
                if (enterprise.LIMIT_TIME_START == enterprise.LIMIT_TIME_END)
                {
                    peakTime.Add(enterprise.ID.ToString(), new List<DateTime>(){
                                DateTime.Now.Date,
                                DateTime.Now.Date.AddDays(1).AddSeconds(-1)
                            });
                    continue;
                }

                double hour = double.Parse(enterprise.LIMIT_TIME_START.Split(new char[1] { ':' })[0]);
                double minutes = double.Parse(enterprise.LIMIT_TIME_START.Split(new char[1] { ':' })[1]);
                DateTime limitTimeStart = DateTime.Now.Date.AddHours(hour).AddMinutes(minutes);
                hour = double.Parse(enterprise.LIMIT_TIME_END.Split(new char[1] { ':' })[0]);
                minutes = double.Parse(enterprise.LIMIT_TIME_END.Split(new char[1] { ':' })[1]);
                DateTime limitTimeEnd = DateTime.Now.Date.AddHours(hour).AddMinutes(minutes);
                //00:00 ~ 10:00
                if (limitTimeEnd > limitTimeStart && limitTimeEnd > time && limitTimeStart <= time)
                {
                    peakTime.Add(enterprise.ID.ToString(), new List<DateTime>(){
                                limitTimeStart,
                                limitTimeEnd
                            });
                    continue;
                }

                //10:00 ~ 08:00 的时间段  10:00~24：00   10:00 ~ 08:00 的时间段  00:00~08：00
                if (limitTimeEnd < limitTimeStart && (limitTimeStart <= time || limitTimeEnd > time))
                {
                    peakTime.Add(enterprise.ID.ToString(), new List<DateTime>(){
                                limitTimeStart,
                                DateTime.Now.Date.AddDays(1).AddSeconds(-1),
                                DateTime.Now.Date,
                                limitTimeEnd
                            });
                    continue;
                }


                //将非该时间错峰的企业移除
                peakEnterpriseList.Remove(enterprise);
                i--;
            }
            #endregion

            return peakEnterpriseList;
        }
        /// <summary>
        /// 根据当前时间获取处于错峰的企业
        /// </summary>
        /// <returns></returns>
        public List<T_THEPEAK_ENT_SUB_LISTModel> GetThePeakEnterprise()
        {
            return GetThePeakEnterprise(DateTime.Now);
        }
    }
}