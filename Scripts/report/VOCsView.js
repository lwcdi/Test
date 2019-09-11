var echartColors = ['#003366', '#006699', '#4cabce', '#6495ED'];

$(document).ready(function () {
    //SetDate();
    drawSelfTestView();  //1、检验机构设备自检情况
    drawAppearView();  //2、车辆外检情况
    drawRealTimeView(); //3、检验实时监控情况
    drawReportView();   //4、检验报告审核发放情况
    //drawBusinessDealView();  //5、检验业务处理情况
    drawWarnInfoView();  //6、检验业务告警情况
});

function bindChart(eID, option) {
    var myChart = echarts.init(document.getElementById(eID));
    myChart.setOption(option);
    addEvent(window, "resize", function () {
        myChart.resize();
    });
}

// 事件绑定
function addEvent(element, eType, handle) {
    if (element.addEventListener) {           //如果支持addEventListener
        element.addEventListener(eType, handle);
    } else if (element.attachEvent) {          //如果支持attachEvent
        element.attachEvent("on" + eType, handle);
    } else {                                  //否则使用兼容的onclick绑定
        element["on" + eType] = handle;
    }
}

//1、当天数据复审情况
function drawSelfTestView() {
    getFSData();
}

//2、实时联网状态
function drawAppearView() {
    var option = {
        color: echartColors,
        tooltip: {
            trigger: 'item',
            formatter: "{b}: {c} ({d}%)"
        },
        legend: {
            x: 'center',
            y: 'bottom',
            data: ['在线', '离线', '无数据', '其他']
        },
        series: [
                {
                    name: '车辆外检情况', //内环 
                    type: 'pie',
                    radius: [0, '70%'], //饼图的半径 [内半径，外半径] 
                    label: {
                        normal: {
                            position: 'outside' //内置文本标签 
                        }
                    },
                    data:
                    [
                        { value: 335, name: '在线' },
                        { value: 65, name: '离线' },
                        { value: 35, name: '无数据' },
                        { value: 13, name: '其他' }
                    ]
                    //(function () {
                    //    var arr = [
                    //    { value: 335, name: '合格' },
                    //    { value: 135, name: '不合格'}
                    //    ];
                    //    $.ajax({
                    //        type: "post",
                    //        async: false, //同步执行
                    //        url: "/MonitorWindow/ViewWorkBench/GetHNAppear",
                    //        contentType: "application/json",
                    //        data: {},
                    //        dataType: "json", //返回数据形式为json
                    //        success: function (result) {
                    //            //arr = $.parseJSON(result);
                    //            //arr = result;
                    //            arr[0].value = result.Pass[0];
                    //            arr[1].value = result.Total[0] - result.Pass[0];
                    //            $("#AppearTotal").text(result.Total[0] + '次');
                    //            $("#AppearPass").text(result.Pass[0] + '次');
                    //            if (result.Total[0] == '' || result.Total[0] == 0) {
                    //                $("#AppearPR").text("0%");
                    //            }
                    //            else {
                    //                $("#AppearPR").text(Math.round(result.Pass[0] / result.Total[0] * 100) + "%");
                    //            }
                    //        },
                    //        error: function (errorMsg) {
                    //            myChart.hideLoading();
                    //        }
                    //    })
                    //    return arr;
                    //})()
                    ,
                    itemStyle: {
                        normal: {
                            label: {
                                show: true,
                                formatter: '{b}{d}%'
                            },
                            labelLine: { show: true }
                        }
                    }
                }
        ]
    };
    var result = GetAppearData();
    option.series[0].data[0].value = result.data[0];
    option.series[0].data[1].value = result.data[1];
    option.series[0].data[2].value = result.data[2];
    option.series[0].data[3].value = result.data[3];
    $("#SSDWCount").text(eval(result.data.join('+')));
    bindChart("VehAppear", option);
}

function GetAppearData() {
    var arr = { data: [11, 12, 15, 16] };
    $.ajax({
        type: "post",
        async: false, //同步执行
        url: "/PlatformIndex/WRYView/GetCompanyNetStatusInfo?IsVOCs=1",
        contentType: "application/json",
        data: {},
        dataType: "json", //返回数据形式为json
        success: function (result) {
            arr = result;
            //arr = $.parseJSON(result.d);
        },
        error: function (errorMsg) {
            myChart.hideLoading();
        }
    });
    return arr;
}

//3、重点关注企业排放情况
function drawRealTimeView() {
    var option = {
        color: echartColors,
        //tooltip: {
        //    show: true,
        //    trigger: 'item',
        //    //formatter: "{a}<br/>{b}:{c}%"
        //},
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        legend: {
            //x: 'center',
            y: 'bottom',
            data: ['非甲烷总烃', '苯', '甲苯', '二甲苯']
        },
        toolbox: {
            show: false,
            feature: {
                mark: { show: true },
                dataView: { show: true, readOnly: false },
                magicType: { show: true, type: ['line', 'bar'] },
                restore: { show: true },
                saveAsImage: { show: true }
            }
        },
        label: {
            normal: {
                show: true,
                position: 'outside'
            }
        },
        calculable: true,
        grid: {
            y: 30
        },
        xAxis: [
                        {
                            type: 'category',
                            data: [],
                            //data: []
                            axisLabel: {
                                interval: 0,
                                rotate: 20
                            }
                        }
        ],
        yAxis: [
                     {
                         type: 'value',
                         boundaryGap: [0, 0.01]
                     }
        ],
        series: [
                        {
                            name: '非甲烷总烃',
                            type: 'bar',
                            data: [345, 232]
                            //data: []
                            , itemStyle: {
                                normal: {
                                    label: { show: true, formatter: '{c}' }
                                }
                            }
                        }
                        , {
                            name: '苯',
                            type: 'bar',
                            data: [345, 232]
                            //data: []
                            , itemStyle: {
                                normal: {
                                    label: { show: true, formatter: '{c}' }
                                }
                            }
                        }
                        , {
                            name: '甲苯',
                            type: 'bar',
                            data: [345, 232]
                            //data: []
                            , itemStyle: {
                                normal: {
                                    label: { show: true, formatter: '{c}' }
                                }
                            }
                        }
                        , {
                                name: '二甲苯',
                            type: 'bar',
                            data: [345, 232]
                            //data: []
                            , itemStyle: {
                                normal: {
                                    label: { show: true, formatter: '{c}' }
                                }
                            }
                        }
        ]
    };

    var result = GetRealTimeData();
    option.xAxis[0].data = result.area;
    option.series[0].barWidth = 5;
    option.series[0].data = result.ZJ;
    option.series[1].barWidth = 5;
    option.series[1].data = result.Ben;
    option.series[2].barWidth = 5;
    option.series[2].data = result.JBen;
    option.series[3].barWidth = 5;
    option.series[3].data = result.JBen2;

    bindChart("RealTime", option);
}

function GetRealTimeData() {
    var arr = { area: ['企业A', '企业B', '企业C', '企业D', '企业E', '企业F'], ZJ: [11, 12, 15, 16, 18, 19], Ben: [11, 12, 15, 16, 18, 19], JBen: [11, 12, 15, 16, 18, 19], JBen2: [11, 12, 15, 16, 18, 19] };
    $.ajax({
        type: "post",
        async: false, //同步执行
        url: "/PlatformIndex/VOCsView/GetZDGZCompanyMonitorData",
        contentType: "application/json",
        data: {},
        dataType: "json", //返回数据形式为json
        success: function (result) {
            arr = result;
            //arr = $.parseJSON(result.d);
        },
        error: function (errorMsg) {
            myChart.hideLoading();
        }
    });
    return arr;
}

//4、区域总量分布情况
function drawReportView() {
    var option = {
        color: echartColors,
        xAxis: {
            type: 'category',
            boundaryGap: false,
            data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        grid: {
            y: 30
        },
        legend: {
            //x: 'center',
            y: 'bottom',
            data: ['排放量']
        },
        label: {
            normal: {
                show: true,
                position: 'outside'
            }
        },
        yAxis: {
            type: 'value'
        },
        series: [{
            name: '排放量',
            data: [820, 932, 901, 934, 1290, 1330, 1320],
            type: 'line',
            smooth: true,
            markPoint: {
                data: [
                    { type: 'max', name: '最大值' },
                    { type: 'min', name: '最小值' }
                ]
            },
            itemStyle: { normal: { areaStyle: { type: 'default' } } }
        }]
    };
    var result = GetReportData();
    option.xAxis.data = result.time;
    option.series[0].data = result.data;

    bindChart("FQZL", option);
}

function GetReportData() {
    var arr = { time: ['1点', '2点', '3点', '4点', '5点', '6点', '7点', '8点', '9点', '10点', '11点', '12点', '13点', '14点', '15点', '16点', '17点', '18点', '19点', '20点', '21点', '22点', '23点', '24点'], data: [320, 332, 301, 334, 390, 330, 820, 932, 901, 934, 1290, 1330, 1320, 820, 932, 901, 934, 1290, 1330, 1320, 934, 1290, 1330, 1320] };
    $.ajax({
        type: "post",
        async: false, //同步执行
        url: "/PlatformIndex/VOCsView/GetAreaMonitorData",
        contentType: "application/json",
        data: {},
        dataType: "json", //返回数据形式为json
        success: function (result) {
            arr = result;
        },
        error: function (errorMsg) {
        }
    });
    return arr;
}

//6、检验业务告警情况
function drawWarnInfoView() {
    //var result = GetWarnInfoDatas();
    getContactData();
    //$("#GJAll").text(result.all);
    //$("#GJDeal").text(result.deal);
    //$(".GJUnDeal").text(result.undeal);
}

function getContactData() {
    var param = "";
    param += "IsVOCs=1";
    $("#dg").datagrid({
        url: "/PlatformIndex/WRYView/GetCompanyWarnData?" + param,
        idField: "CODE",
        pagination: true,
        pageSize: 50,
        singleSelect: true,
        striped: true,
        rownumbers: true,
        height: "98%",
        columns: [[
            { title: "点位名称", field: "NAME", width: "20%" },
            { title: "时间", field: "TIME", width: "20%" },
            {
                title: "告警类型", field: "WARNTYPE", width: "20%", styler: function (index, row) {
                    if (row.WARNTYPE == "污染物超标") {
                        return 'color:red;';
                    }
                }
            },
            { title: "告警内容", field: "CONTENT", width: "20%" },
            {
                title: "处理状态", field: "STATUS", width: "18%", styler: function (index, row) {
                    if (row.STATUS == "未处理") {
                        return 'color:red;';
                    }
                }
            }
        ]]
    });
}

function getFSData() {
    //var param = "";
    //param += "ID=" + 1;
    $("#dgFS").datagrid({
        url: "/PlatformIndex/VOCsView/GetCompanyXYData",
        idField: "CODE",
        pagination: true,
        pageSize: 50,
        singleSelect: true,
        striped: true,
        rownumbers: true,
        height: "99%",
        columns: [[
            { title: "点位名称", field: "NAME", width: "25%" },
            //{ title: "时间", field: "TIME", width: "20%" },
            { title: "总量", field: "VALUE", width: "25%" },
            {
                title: "超标情况", field: "WARNTYPE", width: "25%",
                rowStyler: function (index, row) {
                    if (row.WARNTYPE == "超标") {
                        return 'color:red;';
                    }
                }
            },
            { title: "状态", field: "STATUS", width: "23%" },
        ]]
    });
}

function GetWarnInfoDatas() {
    var arr = {all:30, deal:20, undeal:10};
    //$.ajax({
    //    type: "post",
    //    async: false, //同步执行
    //    url: "/MonitorWindow/ViewWorkBench/GetHNRealTime",
    //    contentType: "application/json",
    //    data: {},
    //    dataType: "json", //返回数据形式为json
    //    success: function (result) {
    //        arr = result;
    //    },
    //    error: function (errorMsg) {
    //    }
    //});
    return arr;
}

//刷新按钮
function ReFreshView(methodName) {
    methodName();
    //SetDate();
}

//设置图表截止时间
function SetDate() {
    var nowDate = new Date();
    var dateTime = nowDate.getFullYear() + '年' + (nowDate.getMonth() + 1) + '月' + nowDate.getDate() + '日 ' + nowDate.getHours() + '时';
    $(".Title_Date").text(dateTime);
}

//弹出弹框
function openWindow(strTitle, strUrl, iWidth, iHeigth) {
    jQuery().ReportDialog({
        url: strUrl,
        title: strTitle,
        resize: false,
        width: iWidth,
        height: iHeigth
    });
}

//点击弹框事件
function openDialog(url) {
    openWindow('详细页面', url, 1000, 650);
}