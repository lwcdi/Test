var echartColors = ['#003366', '#006699', '#4cabce', '#6495ED'];

$(document).ready(function () {
    //SetDate();
    drawSelfTestView();  //1、检验机构设备自检情况
    drawAppearView();  //2、车辆外检情况
    drawRealTimeView(); //3、检验实时监控情况
    drawReportView();   //4、检验报告审核发放情况
    drawBusinessDealView();  //5、检验业务处理情况
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

//1、管控点位分布情况
function drawSelfTestView() {
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
            data: ['国控点', '省控点', '市控点', '区控点']
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
                            name: '国控点',
                            type: 'bar',
                            data: [345, 232]
                            //data: []
                            , itemStyle: {
                                normal: {
                                    label: { show: true, formatter: '{c}' },
                                    //color: function (params) {
                                    //    var colorList = [
                                    //       '#C1232B', '#B5C334', '#FCCE10', '#E87C25', '#27727B',
                                    //       '#FE8463', '#9BCA63', '#FAD860', '#F3A43B', '#60C0DD',
                                    //       '#D7504B', '#C6E579', '#F4E001', '#F0805A', '#26C0C0'
                                    //    ];
                                    //    return colorList[params.dataIndex]
                                    //}
                                }
                            }
                        }
                        , {
                            name: '省控点',
                            type: 'bar',
                            data: [345, 232]
                            //data: []
                            , itemStyle: {
                                normal: {
                                    label: { show: true, formatter: '{c}' },
                                    //color: function (params) {
                                    //    var colorList = [
                                    //       '#C1232B', '#B5C334', '#FCCE10', '#E87C25', '#27727B',
                                    //       '#FE8463', '#9BCA63', '#FAD860', '#F3A43B', '#60C0DD',
                                    //       '#D7504B', '#C6E579', '#F4E001', '#F0805A', '#26C0C0'
                                    //    ];
                                    //    return colorList[params.dataIndex]
                                    //}
                                }
                            }
                        }
                        , {
                            name: '市控点',
                            type: 'bar',
                            data: [345, 232]
                            //data: []
                            , itemStyle: {
                                normal: {
                                    label: { show: true, formatter: '{c}' },
                                    //color: function (params) {
                                    //    var colorList = [
                                    //       '#C1232B', '#B5C334', '#FCCE10', '#E87C25', '#27727B',
                                    //       '#FE8463', '#9BCA63', '#FAD860', '#F3A43B', '#60C0DD',
                                    //       '#D7504B', '#C6E579', '#F4E001', '#F0805A', '#26C0C0'
                                    //    ];
                                    //    return colorList[params.dataIndex]
                                    //}
                                }
                            }
                        }
                        , {
                            name: '区控点',
                            type: 'bar',
                            data: [345, 232]
                            //data: []
                            , itemStyle: {
                                normal: {
                                    label: { show: true, formatter: '{c}' },
                                    //color: function (params) {
                                    //    var colorList = [
                                    //       '#C1232B', '#B5C334', '#FCCE10', '#E87C25', '#27727B',
                                    //       '#FE8463', '#9BCA63', '#FAD860', '#F3A43B', '#60C0DD',
                                    //       '#D7504B', '#C6E579', '#F4E001', '#F0805A', '#26C0C0'
                                    //    ];
                                    //    return colorList[params.dataIndex]
                                    //}
                                }
                            }
                        }
        ]
    };

    var result = GetSelfTestData();
    option.xAxis[0].data = result.area;
    option.series[0].barWidth = 5;
    option.series[0].data = result.Guo;
    option.series[1].barWidth = 5;
    option.series[1].data = result.Sheng;
    option.series[2].barWidth = 5;
    option.series[2].data = result.Shi;
    option.series[3].barWidth = 5;
    option.series[3].data = result.Qu;
    $("#GKDFBCount").text(eval(result.Guo.join('+')) + eval(result.Sheng.join('+')) + eval(result.Shi.join('+')) + eval(result.Qu.join('+')));
    //$("#SelfTestPass").text(result.Pass[0] + '次');
    //if (result.Total[0] == '' || result.Total[0] == 0) {
    //    $("#SelfTestPR").text("0%");
    //}
    //else {
    //    $("#SelfTestPR").text(Math.round(result.Pass[0] / result.Total[0] * 100) + "%");
    //}
    bindChart("SelfTest", option);
}

function GetSelfTestData() {
    //var arr = { area: ['中原区', '二七区', '管城区', '金水区', '上街区', '惠济区'], Guo: [11, 12, 15, 16, 18, 19], Sheng: [11, 12, 15, 16, 18, 19], Shi: [11, 12, 15, 16, 18, 19], Qu: [11, 12, 15, 16, 18, 19] };
    var arr = [];
    $.ajax({
        type: "post",
        async: false, //同步执行
        url: "/PlatformIndex/WRYView/GetCompanyInfoByArea",
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

//2、实时点位在线情况
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
        url: "/PlatformIndex/WRYView/GetCompanyNetStatusInfo?IsVOCs=0",
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

//3、停运备案情况
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
            data: ['国控点', '省控点', '市控点', '区控点']
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
                            data: ['正常', '停产'],
                            //data: []
                            axisLabel: {
                                interval: 0,
                                rotate: 0
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
                            name: '国控点',
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
                            name: '省控点',
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
                            name: '市控点',
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
                            name: '区控点',
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
    option.series[0].barWidth = 30;
    option.series[0].data = result.Guo;
    option.series[1].data = result.Sheng;
    option.series[2].data = result.Shi;
    option.series[3].data = result.Qu;
    $("#TYBACount").text(eval(result.Guo.join('+')) + eval(result.Sheng.join('+')) + eval(result.Shi.join('+')) + eval(result.Qu.join('+')));
    bindChart("RealTime", option);
}

function GetRealTimeData() {
    var arr = { Guo: [11, 12], Sheng: [11, 12], Shi: [11, 12], Qu: [11, 12] };
    $.ajax({
        type: "post",
        async: false, //同步执行
        url: "/PlatformIndex/WRYView/GetCompanyStopInfoByArea",
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

//4、废气总量
function drawReportView() {
    var option = {
        color: echartColors,
        tooltip: {
            trigger: 'axis',
            axisPointer: {            // 坐标轴指示器，坐标轴触发有效
                type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
            }
        },
        legend: {
            y: 'bottom',
            data: ['二氧化硫', '氢氧化物', '颗粒物']
        },
        grid: {
            y: 30
        },
        xAxis: [
            {
                type: 'category',
                data: ['中原区', '二七区', '管城区', '金水区', '上街区', '惠济区'],
                axisLabel: {
                    interval: 0,
                    rotate: 20
                }
            }
        ],
        label: {
            normal: {
                show: true,
                position: 'inside'
            }
        },
        yAxis: [
            {
                type: 'value'
            }
        ],
        series: [
            {
                name: '二氧化硫',
                type: 'bar',
                stack: '1',
                data: [320, 332, 301, 334, 390, 330]
            },
            {
                name: '氢氧化物',
                type: 'bar',
                stack: '1',
                data: [120, 132, 101, 134, 90, 230]
            }
            ,
            {
                name: '颗粒物',
                type: 'bar',
                stack: '1',
                data: [120, 132, 101, 134, 90, 230]
            }
        ]
    };
    var result = GetReportData();
    option.xAxis[0].data = result.area;
    option.series[0].barWidth = 30;
    option.series[0].data = result.SO2;
    option.series[1].data = result.HO;
    option.series[2].data = result.PM;
    $("#FQZLCount").text(eval(result.SO2.join('+')) + eval(result.HO.join('+')) + eval(result.PM.join('+')));
    bindChart("FQZL", option);
}

function GetReportData() {
    var arr = { area: ['中原区', '二七区', '管城区', '金水区', '上街区', '惠济区'], SO2: [320, 332, 301, 334, 390, 330], HO: [120, 132, 101, 134, 90, 230], PM: [120, 132, 101, 134, 90, 230] };
    //var arr = [];
    $.ajax({
        type: "post",
        async: false, //同步执行
        url: "/PlatformIndex/WRYView/GetCompanyFQMointorData",
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

//5、废水总量
function drawBusinessDealView() {
    var option = {
        color: echartColors,
        tooltip: {
            trigger: 'axis',
            axisPointer: {            // 坐标轴指示器，坐标轴触发有效
                type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
            }
        },
        legend: {
            y: 'bottom',
            data: ['化学需氧量', '氨氮']
        },
        grid: {
            y: 30
        },
        xAxis: [
            {
                type: 'category',
                data: ['中原区', '二七区', '管城区', '金水区', '上街区', '惠济区'],
                axisLabel: {
                    interval: 0,
                    rotate: 20
                }
            }
        ],
        yAxis: [
            {
                type: 'value'
            }
        ],
        label: {
            normal: {
                show: true,
                position: 'inside'
            }
        },
        series: [
            {
                name: '化学需氧量',
                type: 'bar',
                stack: '1',
                data: [320, 332, 301, 334, 390, 330]
            },
            {
                name: '氨氮',
                type: 'bar',
                stack: '1',
                data: [120, 132, 101, 134, 90, 230]
            }
        ]
    };
    var result = GetBusinessDealData();
    option.xAxis[0].data = result.area;
    option.series[0].barWidth = 30;
    option.series[0].data = result.HX;
    option.series[1].data = result.AD;
    $("#FSZLCount").text(eval(result.HX.join('+')) + eval(result.AD.join('+')));
    bindChart("FSZL", option);
}

function GetBusinessDealData() {
    var arr = { area: ['中原区', '二七区', '管城区', '金水区', '上街区', '惠济区'], HX: [320, 332, 301, 334, 390, 330], AD: [120, 132, 101, 134, 90, 230] };
    //var arr = [];
    $.ajax({
        type: "post",
        async: false, //同步执行
        url: "/PlatformIndex/WRYView/GetCompanyFSMointorData",
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
    param += "IsVOCs=0";
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