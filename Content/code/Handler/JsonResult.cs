using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
/// <summary>
/// JsonResult 的摘要说明
/// </summary>
/// <summary>
/// 描述：ajax请求结果对象
/// 作者：吴德坤
/// 时间：2016-11-14
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class JsonResult<T>
{
    /// <summary>
    /// 请求是否成功 //, DefaultValueHandling = DefaultValueHandling.Ignore
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool success { get; set; }
    /// <summary>
    /// 请求是否结果(字符串"true/false")
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Result { get; set; }
    /// <summary>
    /// 错误信息
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string msg { get; set; }
    /// <summary>
    /// 数据集合
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<T> list { get; set; }
    /// <summary>
    /// 自定义数据
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public object data { get; set; }
    /// <summary>
    /// 数据总数
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? recc { get; set; }
    /// <summary>
    /// 分页数据 每页显示条数
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? pageSize { get; set; }

    /// <summary>
    /// 不需要序列化的数据
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string sdata { get; set; }

    /// <summary>
    /// 不需要序列化的数据
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string sdata2 { get; set; }
    /// <summary>
    /// 不需要序列化的数据
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string sdata3 { get; set; }
    /// <summary>
    /// 不需要序列化的数据
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string sdata4 { get; set; }
    /// <summary>
    /// 不需要序列化的数据
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string sdata5 { get; set; }
    public JsonResult()
    {
        //list = new List<T>(0);
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}