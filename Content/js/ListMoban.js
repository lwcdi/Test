var listMoBan =listMoBan|| {};
listMoBan.Get = listMoBan.Get || {};
listMoBan.Get.FeiQiCol1 = function ()
{
    var col = [];
    col.push({ title: '时间', field: "RECTIME", align: 'center', rowspan: 3 });
    col.push({ title: '烟尘', align: 'center', colspan: 3 });
    col.push({ title: '二氧化硫', align: 'center', colspan: 3 });
    col.push({ title: '氮氧化物', align: 'center', colspan: 3 });
    col.push({ title: '流量', align: 'center', rowspan: 1 });
    col.push({ title: '氧含量', align: 'center', rowspan: 1 });
    col.push({ title: '温度', align: 'center', rowspan: 1 });
    col.push({ title: '湿度', align: 'center', rowspan: 1 });
    col.push({ title: '化学需氧量', align: 'center', colspan: 2 });
    col.push({ title: '氨氮', align: 'center', colspan: 2 });
    col.push({ title: '流量', align: 'center', colspan: 1 });
    return col;


}

listMoBan.Get.FeiQiCol2 = function () {
    var col2 = [];

    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '折算浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '折算浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '折算浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '立方米', field: "FLOW_GAS", align: 'center', rowspan: 2 });
    col2.push({ title: '百分比', field: "A01013", align: 'center', rowspan: 2 });
    col2.push({ title: '摄氏度', field: "A01012", align: 'center', rowspan: 2 });
    col2.push({ title: '百分比', field: "A19001", align: 'center', rowspan: 2 });
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '吨', field: "FLOW_GAS", align: 'center', rowspan: 2 });

    return col2;
}

listMoBan.Get.FeiQiVor = function () {
    var Vicecol = [];
    Vicecol.push({ title: '毫克/立方米', field: "A34013_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克/立方米', field: "A34013_ZSND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "A34013_PFL", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克/立方米', field: "A21026_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克/立方米', field: "A21026_ZSND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "A21026_PFL", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克/立方米', field: "A21002_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克/立方米', field: "A21002_ZSND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "A21002_PFL", align: 'center', colspan: 1, width: 100 });
    //Vicecol.push({ title: '立方米', align: 'center', rowspan: 2, width: 100 });
    //Vicecol.push({ title: '百分比', align: 'center', rowspan: 2, width: 100 });
    //Vicecol.push({ title: '摄氏度', align: 'center', rowspan: 2, width: 100 });
    //Vicecol.push({ title: '百分比', align: 'center', rowspan: 2, width: 100 });
    Vicecol.push({ title: '毫克升', field: "W01018_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "W01018_PFL", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克升', field: "W21003_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "W21003_PFL", align: 'center', colspan: 1, width: 100 });
    return Vicecol;
}


listMoBan.Get.FeiShuiCol1 = function () {
    var col = [];
    col.push({ title: '时间', field: "RECTIME", align: 'center', rowspan: 3, width: 300 });
    col.push({ title: '化学需氧量', align: 'center', colspan: 2, width: 300 });
    col.push({ title: '氨氮', align: 'center', colspan: 2, width: 300 });
    col.push({ title: '流量', align: 'center', colspan: 1, width: 300 });
    return col;


 


}

listMoBan.Get.FeiShuiCol2 = function () {

    var col2 = [];
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '吨', field: "FLOW_WATER", align: 'center', rowspan: 2 });
    return col2;
}

listMoBan.Get.FeiShuiVor = function () {
    var Vicecol = []
    Vicecol.push({ title: '毫克升', field: "W01018_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "W01018_PFL", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克升', field: "W21003_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "W21003_PFL", align: 'center', colspan: 1, width: 100 });
    return Vicecol;
}

listMoBan.Get.AllCol1 = function () {
    var col = [];
    col.push({ title: '状态', field: "STATE", align: 'center', rowspan: 3 });
    col.push({ title: '监测点名称', field:"D",  align: 'center', rowspan: 3 });
    col.push({ title: '烟尘', align: 'center', colspan: 3 });
    col.push({ title: '二氧化硫', align: 'center', colspan: 3 });
    col.push({ title: '氮氧化物', align: 'center', colspan: 3 });
    col.push({ title: '流量', align: 'center', rowspan: 1 });
    col.push({ title: '氧含量', align: 'center', rowspan: 1 });
    col.push({ title: '温度', align: 'center', rowspan: 1 });
    col.push({ title: '湿度', align: 'center', rowspan: 1 });
    col.push({ title: '化学需氧量', align: 'center', colspan: 2 });
    col.push({ title: '氨氮', align: 'center', colspan: 2 });
    col.push({ title: '流量', align: 'center', colspan: 1 });
    return col;

}

listMoBan.Get.AllCol2 = function () {
    var col2 = [];
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '折算浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '折算浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '折算浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '立方米', field: "FLOW_GAS", align: 'center', rowspan: 2 });
    col2.push({ title: '百分比', field: "A01013", align: 'center', rowspan: 2 });
    col2.push({ title: '摄氏度', field: "A01012", align: 'center', rowspan: 2 });
    col2.push({ title: '百分比', field: "A19001", align: 'center', rowspan: 2 });
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '浓度', align: 'center', colspan: 1 });
    col2.push({ title: '排放量', align: 'center', colspan: 1 });
    col2.push({ title: '吨', field: "FLOW_GAS", align: 'center', rowspan: 2 });
    return col2;

}

listMoBan.Get.AllVor = function () {
    var Vicecol = [];

    Vicecol.push({ title: '毫克/立方米', field: "A34013_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克/立方米', field: "A34013_ZSND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "A34013_PFL", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克/立方米', field: "A21026_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克/立方米', field: "A21026_ZSND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "A21026_PFL", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克/立方米', field: "A21002_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克/立方米', field: "A21002_ZSND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "A21002_PFL", align: 'center', colspan: 1, width: 100 });
    //Vicecol.push({ title: '立方米', align: 'center', rowspan: 2, width: 100 });
    //Vicecol.push({ title: '百分比', align: 'center', rowspan: 2, width: 100 });
    //Vicecol.push({ title: '摄氏度', align: 'center', rowspan: 2, width: 100 });
    //Vicecol.push({ title: '百分比', align: 'center', rowspan: 2, width: 100 });
    Vicecol.push({ title: '毫克升', field: "W01018_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "W01018_PFL", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '毫克升', field: "W21003_ND", align: 'center', colspan: 1, width: 100 });
    Vicecol.push({ title: '千克', field: "W21003_PFL", align: 'center', colspan: 1, width: 100 });

    return Vicecol;
}


//数采仪
listMoBan.Get.DataAcquisition  = function () {
    var columns = [];
    columns.push({ title: '数采仪MN', field: "MN_NO", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '当前状态', field: "CUR_STATE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '状态更新时间', field: "STATE_TIME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '告警次数', field: "WARN_TIMES", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '持续时间', field: "DURATION", align: 'center', colspan: 1, width: 100 });
    return columns;
}

//监测仪器
listMoBan.Get.Monitoring = function () {
    var columns = [];
    columns.push({ title: '设备名称', field: "MONITOR_NAME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '当前状态', field: "CUR_STATE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '状态更新时间', field: "STATE_TIME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '告警次数', field: "WARN_TIMES", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '持续时间', field: "DURATION", align: 'center', colspan: 1, width: 100 });
    return columns;
}

//治理设施
listMoBan.Get.Governance = function () {
    var columns = [];
    columns.push({ title: '设备名称', field: "MN_NO", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '当前状态', field: "CUR_STATE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '状态更新时间', field: "STATE_TIME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '告警次数', field: "WARN_TIMES", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '持续时间', field: "DURATION", align: 'center', colspan: 1, width: 100 });
    return columns;
}

//排放总量提示
listMoBan.Get.TotalEmission = function () {
    var columns = [];
    columns.push({ title: '污染物', field: "STAIN_TEXT", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '当前排放量', field: "SUM_PFL", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '单位', field: "UNIT", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '阀值', field: "FZ", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '总量比例', field: "TOTAL_RATE", align: 'center', colspan: 1, width: 100 });
    return columns;
}

//数据传输情况
listMoBan.Get.DataTransfer = function () {
    var columns = [];
    columns.push({ title: '状态', field: "STATE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '排口', field: "PK_NO", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '联网状态', field: "WEB_STATE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '数采仪', field: "MN_NO", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '应报记录数', field: "NUMBER_OF_RECORDS", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '实报记录数', field: "REAL_NUM_RECORDS", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '上报率', field: "REPORT_RATE", align: 'center', colspan: 1, width: 100 });
    return columns;
}

//污染物超标
listMoBan.Get.ExcessivePollut = function () {
    var columns = [];
    columns.push({ title: '监测点名称', field: "PK_NAME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '污染物名称', field: "STAIN_TEXT", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '告警值', field: "VALUE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '告警限值', field: "LIMIT", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '告警状态', field: "STATE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '告警开始时间', field: "STARTTIME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '告警值结束时间', field: "ENDTIME", align: 'center', colspan: 1, width: 100 });
    return columns;
}


//污源物异常
listMoBan.Get.ExceptionStain = function () {
    var columns = [];
    columns.push({ title: '监测点名称', field: "PK_NAME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '污染物名称', field: "STAIN_TEXT", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '异常值', field: "VALUE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '异常限值', field: "LIMIT", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '处理状态', field: "STATE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '异常开始时间', field: "STARTTIME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '异常结束时间', field: "ENDTIME", align: 'center', colspan: 1, width: 100 });
    return columns;
}


//排放流量异常
listMoBan.Get.ExceptionPFL = function () {
    var columns = [];
    columns.push({ title: '监测点名称', field: "PK_NAME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '点位类型', field: "STAIN_TEXT", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '异常值', field: "VALUE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '异常限值', field: "LIMIT", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '处理状态', field: "STATE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '异常开始时间', field: "STARTTIME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '异常结束时间', field: "ENDTIME", align: 'center', colspan: 1, width: 100 });
    return columns;
}


//数采仪掉线
listMoBan.Get.MNDownLine = function () {
    var columns = [];
    columns.push({ title: '监测点名称', field: "PK_NAME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '数采仪MN', field: "STAIN_TEXT", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '处理状态', field: "STATE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '掉线开始时间', field: "STARTTIME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '重新上线时间', field: "ENDTIME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '掉线持续时间（分钟）', field: "LENTIME", align: 'center', colspan: 1, width: 100 });
 
    return columns;
}


//治理设施停运
listMoBan.Get.ControlFacilities = function () {
    var columns = [];
    columns.push({ title: '质量设施名称', field: "PK_NAME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '处理状态', field: "STATE", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '停运开始时间', field: "STARTTIME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '重新上线时间', field: "ENDTIME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '停运持续时间（分钟）', field: "LENTIME", align: 'center', colspan: 1, width: 100 });
    return columns;
}




//传输状态
listMoBan.Get.DataTransferState = function () {
    debugger;
    var columns = [];
    columns.push({ title: '企业名称', field: "COMPANY_NAME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '监测点名称', field: "PK_NAME", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '应报记录数', field: "NUMBER_OF_RECORDS", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '实报记录数', field: "REAL_NUM_RECORDS", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '上报率', field: "REPORT_RATE", align: 'center', colspan: 1, width: 100 });
  //  columns.push({ title: '有效率', field: "EFFICIENT", align: 'center', colspan: 1, width: 100 });
    columns.push({ title: '修正数', field: "CORRECTION_NUMBER", align: 'center', colspan: 1, width: 100 });
    return columns;
}



