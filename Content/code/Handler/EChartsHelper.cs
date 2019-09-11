using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace UI.Web.Content.code.Handler
{
    public class EChartsHelper
    {
        private static string ChartColor = @"['#ff7f50', '#87cefa', '#da70d6', '#32cd32', '#6495ed',
                                        '#ff69b4', '#ba55d3', '#cd5c5c', '#ffa500', '#40e0d0',
                                        '#1e90ff', '#ff6347', '#7b68ee', '#00fa9a', '#ffd700',
                                        '#6699FF', '#ff6666', '#3cb371', '#b8860b', '#30e0e0']";
        /// <summary>
        /// 堆积柱状图
        /// </summary>
        /// <param name="x"></param>
        /// <param name="yDataSeries"></param>
        /// <param name="title"></param>
        /// <param name="yAxisName"></param>
        /// <param name="legend"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string GetStackedColumnChart(string x, List<KeyValuePair<string, string>> yDataSeries, string title, string yAxisName, string legend, string unit)
        {
            return GetBaseChart(x, yDataSeries, title, yAxisName, legend, unit, "bar", true);
        }

        public static string GetStackedColumnChartTopic(string x, List<string> yDataSeries, string title, string yAxisName, string legend, string unit)
        {
            return GetBaseChartTopic(x, yDataSeries, title, yAxisName, legend, unit, "bar", true);
        }
        /// <summary>
        /// 曲线图
        /// </summary>
        /// <param name="x"></param>
        /// <param name="yDataSeries"></param>
        /// <param name="title"></param>
        /// <param name="yAxisName"></param>
        /// <param name="legend"></param>
        /// <param name="unit"></param>
        /// <param name="labled"></param>
        /// <param name="autoColor"></param>
        /// <returns></returns>
        public static string GetLinesChart(string x, List<KeyValuePair<string, string>> yDataSeries, string title, string yAxisName, string legend, string unit, bool labled = true, bool autoColor = false)
        {
            return GetBaseChart(x, yDataSeries, title, yAxisName, legend, unit, "line", false, labled, autoColor);
        }
        /// <summary>
        /// 饼图
        /// </summary>
        /// <param name="yDataSeries"></param>
        /// <param name="title"></param>
        /// <param name="subTitle"></param>
        /// <param name="legend"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string GetPieChart(List<KeyValuePair<string, string>> yDataSeries, string title, string subTitle, string legend, string unit)
        {

            StringBuilder sb = new StringBuilder("{ title : { text: '" + title + "', subtext: '" + subTitle +
                @"'}, color: " + ChartColor + ","
                    + " tooltip : { trigger: 'item', formatter: \"{a} <br/>{b} : {c} " + unit + "({d}%)\"},legend: {y:'bottom',data:[" + legend + "]},toolbox: {show : true,feature : { " +
                    " mark : {show: true}, dataView : {show: true, readOnly: true},magicType : {show: true, type: ['line', 'bar']}}}," +
                    " calculable : true, " +
                    " series : [{ name:'" + title + "',type:'pie',data:[");
            for (int i = 0; i < yDataSeries.Count; i++)
            {
                sb.Append("{value:" + yDataSeries[i].Value + ",name:'" + yDataSeries[i].Key + "'},");
            }

            if (yDataSeries.Count > 0) sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("] } ] }");
            return sb.ToString();
        }

        public static string GetPieChartTopic(List<string> yDataSeries, string title, string subTitle, string legend, string unit)
        {
            StringBuilder sb = new StringBuilder("{ title : { text: '" + title + "', subtext: '" + subTitle +
                @"'},"
                    + " tooltip : { trigger: 'item', formatter: \"{a} <br/>{b} : {c} " + unit + "({d}%)\"},toolbox: {show : true,feature : { " +
                    " mark : {show: true}, dataView : {show: true, readOnly: true},magicType : {show: true, type: ['line', 'bar']}}}," +
                    " calculable : true, " +
                    " series : [{ name:'" + title + "',type:'pie',radius:'50%',data:[");
            for (int i = 0; i < yDataSeries.Count; i++)
            {
                string[] str = yDataSeries[i].Split(',');
                sb.Append("{value:" + str[1] + ",name:'" + str[0] + "',itemStyle:{normal:{color: '" + str[2] + "'}}},");
            }

            if (yDataSeries.Count > 0) sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("] } ] }");
            return sb.ToString();
        }
        /// <summary>
        /// 面积图
        /// </summary>
        /// <param name="x"></param>
        /// <param name="yDataSeries"></param>
        /// <param name="title"></param>
        /// <param name="yAxisName"></param>
        /// <param name="legend"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string GetAreaChart(string x, List<KeyValuePair<string, string>> yDataSeries, string title, string yAxisName, string legend, string unit)
        {

            StringBuilder sb = new StringBuilder("{ title : { text: '" + title + "', subtext: '" + yAxisName +
                @"'}, color: " + ChartColor + ","
                    + " tooltip : { trigger: 'axis'},legend: {y:'bottom',data:[" + legend + "]},toolbox: {show : true,feature : { " +
                    " mark : {show: true}, dataView : {show: true, readOnly: true},magicType : {show: true, type: ['line', 'bar']}}}," +
                    " calculable : true, " +
                    " xAxis : [{ type : 'category'," +
                    " data :[" + x.ToString() + "]}]," +
                    " yAxis : [{ type : 'value',axisLabel: { formatter: '{value} " + unit + "'} }]," +
                    " series : [");
            for (int i = 0; i < yDataSeries.Count; i++)
            {
                sb.Append("{ name:'" + yDataSeries[i].Key + "', type:'line'" +
                    @",itemStyle: {
                            normal: {
                                areaStyle: {type: 'default'}
                            }
                        }" + ",data:[" + yDataSeries[i].Value + "]},");
            }
            if (yDataSeries.Count > 0) sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("] } ");
            return sb.ToString();
        }
        public static string GetBaseChart(string x, List<KeyValuePair<string, string>> yDataSeries, string title, string yAxisName, string legend, string unit, string type, bool isStacked = false, bool labled = true, bool autoColor = false)
        {

            StringBuilder sb = new StringBuilder("option ={ title : { text: '" + title + "', subtext: '" + yAxisName +
                @"'}," + (autoColor ? "" : " color: " + ChartColor + ",")
                    + " tooltip : { trigger: 'axis'},legend: {y:'bottom',data:[" + legend + "]},toolbox: {show : true,feature : { " +
                    " mark : {show: true}, dataView : {show: true, readOnly: true},magicType : {show: true, type: ['line', 'bar']}}}," +
                    " calculable : true, " +
                    " xAxis : [{ type : 'category'," +
                    " data :[" + x.ToString() + "]}]," +
                    " yAxis : [{ type : 'value',axisLabel: { formatter: '{value} " + unit + "'} }]," +
                    " series : [");
            for (int i = 0; i < yDataSeries.Count; i++)
            {
                sb.Append("{ name:'" + yDataSeries[i].Key + "', type:'" + type + "'" + (isStacked ? ", stack: 'Stacked'" : "") +
                   (labled ? @",itemStyle: {
                            normal: {
                                label : {
                                    position : 'inside',
                                    show : true,
                                    formatter: function (params) {
                                        for (var i = 0, l = option.xAxis[0].data.length; i < l; i++) {
                                            if (option.xAxis[0].data[i] == params.name) {
                                              if(params.value==0) return '';
                                              else return  params.value;
                                            }
                                        }
                                    },
                                    textStyle : {
                                        color: '#000000',
                                        //fontSize : '20',
                                        fontFamily : '微软雅黑',
                                        fontWeight : 'bold'
                                    }
                                }
                            }
                        }" : "") + ",data:[" + yDataSeries[i].Value + "]},");
            }
            if (yDataSeries.Count > 0) sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("] } ");
            return sb.ToString();
        }



        public static string GetBaseChart_zhu(string x, List<KeyValuePair<string, string>> yDataSeries, string title, string yAxisName, string legend, string unit, string type, List<KeyValuePair<string, string>> ypingjunSeries, string bg, string end, bool isStacked = false, bool labled = true, bool autoColor = false )
        {

            StringBuilder sb = new StringBuilder("option ={ title : { text: '" + title + "', subtext: '" + yAxisName +
                @"'}," + (autoColor ? "" : " color: " + ChartColor + ",")
                    + " tooltip : { trigger: 'axis'},legend: {y:'bottom',data:[" + legend + "]},toolbox: {show : true,feature : { " +
                    " mark : {show: true}, dataView : {show: true, readOnly: true},magicType : {show: true, type: ['line', 'bar']}}}," +
                    " calculable : true, " +
                    " xAxis : [{ type : 'category'," +
                    " data :[" + x.ToString() + "]}]," +
                    " yAxis : [{ type : 'value',axisLabel: { formatter: '{value} " + unit + "'} }]," +
                    " series : [");
            for (int i = 0; i < yDataSeries.Count; i++)
            {
                sb.Append("{ name:'" + yDataSeries[i].Key + "', type:'" + type + "'" + (isStacked ? ", stack: 'Stacked'" : "") +
                   (labled ? @",itemStyle: {
                            normal: {
                                label : {
                                    position : 'inside',
                                    show : true,
                                    formatter: function (params) {
                                        for (var i = 0, l = option.xAxis[0].data.length; i < l; i++) {
                                            if (option.xAxis[0].data[i] == params.name) {
                                              if(params.value==0) return '';
                                              else return  params.value;
                                            }
                                        }
                                    },
                                    textStyle : {
                                        color: '#000000',
                                        //fontSize : '20',
                                        fontFamily : '微软雅黑',
                                        fontWeight : 'bold'
                                    }
                                }
                            }
                        }" : "") + ",data:[" + yDataSeries[i].Value  + "], markLine : { data : [ [{name:'基准值',value:" + ypingjunSeries[i].Value+ ",xAxis:'" + bg+ "',yAxis:" + ypingjunSeries[i].Value+ "},{xAxis:'"+end+ "',yAxis:" + ypingjunSeries[i].Value+ "}] ] }" + "},");
            }
            if (yDataSeries.Count > 0) sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("] } ");
            return sb.ToString();
        }


        public static string GetBaseChartTopic(string x, List<string> yDataSeries, string title, string yAxisName, string legend, string unit, string type, bool isStacked = false, bool labled = true, bool autoColor = false)
        {

            StringBuilder sb = new StringBuilder("option ={ title : { text: '" + title + "', subtext: '" + yAxisName +
                @"'}," + (autoColor ? "" : " color: " + ChartColor + ",")
                    + " tooltip : { trigger: 'axis'},legend: {y:'bottom',data:[" + legend + "]},toolbox: {show : true,feature : { " +
                    " mark : {show: true}, dataView : {show: true, readOnly: true},magicType : {show: true, type: ['line', 'bar']}}}," +
                    " calculable : true, " +
                    " xAxis : [{ type : 'category'," +
                    " data :[" + x.ToString() + "]}]," +
                    " yAxis : [{ type : 'value',axisLabel: { formatter: '{value} " + unit + "'} }]," +
                    " series : [");
            for (int i = 0; i < yDataSeries.Count; i++)
            {
                string[] str = yDataSeries[i].Split(';');
                sb.Append("{ name:'" + str[0] + "', type:'" + type + "'" + (isStacked ? ", stack: 'Stacked'" : "") +
                   (labled ? @",itemStyle: {
                            normal: {
                                color:'" + str[2] + @"',
                                label : {
                                    position : 'inside',
                                    show : true,
                                    formatter: function (params) {
                                        for (var i = 0, l = option.xAxis[0].data.length; i < l; i++) {
                                            if (option.xAxis[0].data[i] == params.name) {
                                              if(params.value==0) return '';
                                              else return  params.value;
                                            }
                                        }
                                    },
                                    textStyle : {
                                        color: '#000000',
                                        fontFamily : '微软雅黑',
                                        fontWeight : 'bold'
                                    }
                                }
                            }
                        }" : "") + ",data:[" + str[1] + "]},");
            }
            if (yDataSeries.Count > 0) sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("] } ");
            return sb.ToString();
        }

        /// <summary>
        /// 生成图表
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="type">图表类型</param>
        /// <returns></returns>
        private static string GetChartData(List<KeyValuePair<string, string>> data, string type, string title, string yAxisName, string unit = "", string timeFormat = "", float lower = float.NaN, float upper = float.NaN, float yMax = float.NaN, float yMin = float.NaN)   //
        {
            string result = "";

            StringBuilder data1 = new StringBuilder("");
            StringBuilder data2 = new StringBuilder("");
            string x = "";
            string tmBegin = "";
            string tmEnd = "";

            if (!"radar".Equals(type))  //雷达图除外
            {
                for (int i = 0; i < data.Count; i++)
                {
                    x = data[i].Key;  //去掉日期部分
                    if (!string.IsNullOrEmpty(timeFormat)) x = DateTime.Parse(x).ToString(timeFormat);

                    if (i == 0)
                    {
                        data1.Append("'" + x + "'");
                        data2.Append(getValue(data[i].Value));
                        tmBegin = x;
                    }
                    else
                    {
                        data1.Append("," + "'" + x + "'");
                        data2.Append("," + getValue(data[i].Value));
                    }
                }
                tmEnd = x;
            }
            if ("line".Equals(type))  //拆线图
            {
                result = " { title : {text: '" + title + "', subtext: '" + yAxisName + "' },tooltip: { trigger: 'axis'},legend:{data: [\'" + title + " " + yAxisName + "\']}, " +
                       "  toolbox: { show: true,feature: { mark: { show: true },dataView : {show: true, readOnly: true}, magicType : {show: true}}}, " +
                       "  calculable: true, " +
                       "  xAxis: [{ type: 'category', boundaryGap: false, " +
                       "  data: [" + data1.ToString() + "]}]," +
                       "  yAxis: [{ type: 'value', color:'blue',";
                //if (!yMax.Equals(float.NaN)) result += "max:" + yMax + ",";
                //if (!yMax.Equals(float.NaN)) result += "min:" + yMin + ",";
                result += "axisLabel: { formatter: '{value} " + unit + "'} }],  " +
                "  series: [{ name: '" + title + " " + yAxisName + "', type: 'line', stack: '" + yAxisName + "',  itemStyle: { normal:{color:'#ff7f50'}},  data: [" + data2.ToString() + "],"
                //+ " markLine:{data:[[{name:'上限值',value:" + upper + ",xAxis:'" + tmBegin + "',yAxis:'" + upper + "'},{xAxis:'" + tmEnd + "',yAxis:'" + upper + "'}],[{name:'下限值',value:" + lower + ",xAxis:'" + tmBegin + "',yAxis:'" + lower + "'},{xAxis:'" + tmEnd + "',yAxis:'" + lower + "'}]]}"
                + " }]}";
            }
            else if ("column".Equals(type)) //柱状图
            {
                result = "{ title : { text: '" + title + "', subtext: '" + yAxisName + "'},tooltip : { trigger: 'axis'},legend: {data:['" + title + " " + yAxisName + "']},toolbox: {show : true,feature : { " +
                        " mark : {show: true}, dataView : {show: true, readOnly: true},magicType : {show: true, type: ['line', 'bar']}}}," +
                        " calculable : true, " +
                        " xAxis : [{ type : 'category'," +
                        " data :[" + data1.ToString() + "]}]," +
                        " yAxis : [{ type : 'value',axisLabel: { formatter: '{value} " + unit + "'} }]," +
                        " series : [{ name:'" + title + " " + yAxisName + "', type:'bar', " +
                        " data:[" + data2.ToString() + "],"
                        //+ " markLine:{data:[[{name:'上限值',value:" + upper + ",xAxis:'" + tmBegin + "',yAxis:'" + upper + "'},{xAxis:'" + tmEnd + "',yAxis:'" + upper + "'}],[{name:'下限值',value:" + lower + ",xAxis:'" + tmBegin + "',yAxis:'" + lower + "'},{xAxis:'" + tmEnd + "',yAxis:'" + lower + "'}]]}"
                        + "}] } ";
            }
            else if ("radar".Equals(type)) //风向玫瑰图是:统计在这段时间内各种风向的频率   风向  ADT
            {
                /*  角度
                    360   北
                    315   西北
                    270   西
                    225   西南
                    180   南
                    135   东南
                    90    东
                    45    东北
                */

                float east_Count = 0;
                float eastSouthEast_Count = 0;//东南偏东
                float eastSouth_Count = 0;
                float eastSouthSouth_Count = 0;

                float south_Count = 0;
                float westSouthSourth_Count = 0;
                float westSouth_Count = 0;
                float westSouthWest_Count = 0;

                float west_Count = 0;
                float westNorthWest_Count = 0;
                float westNorth_Count = 0;
                float westNorthNorth_Count = 0;

                float north_Count = 0;//北
                float eastNorthNorth_Count = 0;//东北偏北
                float eastNorth_Count = 0;//东北
                float eastNorthEast_Count = 0;//东北偏东

                int total = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    string val = getValue(data[i].Value);
                    float value = 0;
                    if (!float.TryParse(val, out value)) continue;

                    if (value >= 11.25 && value < (11.25 + 22.5)) eastSouthEast_Count++;
                    else if (value < (11.25 + 22.5 * 2)) eastSouth_Count++;
                    else if (value < (11.25 + 22.5 * 3)) eastSouthSouth_Count++;

                    else if (value < (11.25 + 22.5 * 4)) south_Count++;
                    else if (value < (11.25 + 22.5 * 5)) westSouthSourth_Count++;
                    else if (value < (11.25 + 22.5 * 6)) westSouth_Count++;
                    else if (value < (11.25 + 22.5 * 7)) westSouthWest_Count++;

                    else if (value < (11.25 + 22.5 * 8)) west_Count++;
                    else if (value < (11.25 + 22.5 * 9)) westNorthWest_Count++;
                    else if (value < (11.25 + 22.5 * 10)) westNorth_Count++;
                    else if (value < (11.25 + 22.5 * 11)) westNorthNorth_Count++;

                    else if (value < (11.25 + 22.5 * 12)) north_Count++;
                    else if (value < (11.25 + 22.5 * 13)) eastNorthNorth_Count++;
                    else if (value < (11.25 + 22.5 * 14)) eastNorth_Count++;
                    else if (value < (11.25 + 22.5 * 15)) eastNorthEast_Count++;

                    else east_Count++;
                    total++;

                }
                if (total > 0)
                {
                    east_Count *= 100f / total;
                    eastNorthEast_Count *= 100f / total;
                    eastSouth_Count *= 100f / total;
                    eastSouthSouth_Count *= 100f / total;

                    south_Count *= 100f / total;
                    westSouthSourth_Count *= 100f / total;
                    westSouth_Count *= 100f / total;
                    westSouthWest_Count *= 100f / total;

                    west_Count *= 100f / total;
                    westNorthWest_Count *= 100f / total;
                    westNorth_Count *= 100f / total;
                    westNorthNorth_Count *= 100f / total;

                    north_Count *= 100f / total;
                    eastNorthNorth_Count *= 100f / total;
                    eastNorth_Count *= 100f / total;
                    eastNorthEast_Count *= 100f / total;
                }

                result = " { title:{ text: '风向玫瑰图'},tooltip: {trigger: 'item'},legend: { data: ['风向频率']}," +
                       " polar: [{indicator: [ "
                            + "{ text: '东', max:100.0},{ text: '东南偏东', max:100.0},{ text: '东南', max:100.0},{ text: '东南偏南', max:100.0} "
                            + ",{ text: '南', max:100.0},{text: '西南偏南', max:100.0},{ text: '西南', max:100.0},{ text: '西南偏西',max:100.0}"
                            + ",{ text: '西', max:100.0},{text: '西北偏西', max:100.0},{ text: '西北', max:100.0},{ text: '西北偏北',max:100.0}"
                            + ",{ text: '北', max:100.0},{text: '东北偏北', max:100.0},{ text: '东北', max:100.0},{ text: '东北偏东',max:100.0}"
                        + "], center : ['40%',170],radius : 120," +
                       " name: { formatter:'{value}', textStyle: { color:'#303030'}}}]," +
                       " series: [{ name: '风向', type: 'radar',itemStyle: {normal: {areaStyle: {type: 'default'}}}, data: [{ " +
                       " value: [ " + east_Count.ToString("f1") + "," + eastNorthEast_Count.ToString("f1") + "," + eastSouth_Count.ToString("f1") + "," + eastSouthSouth_Count.ToString("f1")
                       + "," + south_Count.ToString("f1") + "," + westSouthSourth_Count.ToString("f1") + "," + westSouth_Count.ToString("f1") + "," + westSouthWest_Count.ToString("f1")
                       + "," + west_Count.ToString("f1") + "," + westNorthWest_Count.ToString("f1") + "," + westNorth_Count.ToString("f1") + "," + westNorthNorth_Count.ToString("f1")
                       + "," + north_Count.ToString("f1") + "," + eastNorthNorth_Count.ToString("f1") + "," + eastNorth_Count.ToString("f1") + "," + eastNorthEast_Count.ToString("f1") + "]," +
                       " name: '风向', label: { normal: { show: true,formatter:function(params) {  return params.value+'%';}}}}]}]}";

            }
            return result;
        }
        private static string getValue(string strValue)
        {
            if (string.IsNullOrEmpty(strValue)) return strValue;
            bool firstPoint = false;
            int i = 0;
            for (; i < strValue.Length; i++)
            {
                if (i == 0 && strValue[0] == '-') continue;
                if (strValue[i] >= '0' && strValue[i] <= '9') continue;
                if (strValue[i] == '.' && !firstPoint)
                {
                    firstPoint = true;
                    continue;
                }
                break;
            }
            return strValue.Substring(0, i);
        }
    }
}