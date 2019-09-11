var overLaysZC = [];
var overLaysYCB = [];
var overLaysYLX = [];
var companyGPSDatas;  //所有企业GPS信息

//隐藏所有正常企业
function hideZC() {
    for (var i = 0; i < overLaysZC.length; i++) {
        map.removeOverlay(overLaysZC[i]);
    }
}

//显示所有正常企业
function showZC() {
    for (var i = 0; i < overLaysZC.length; i++) {
        map.addOverlay(overLaysZC[i]);
    }
}

function showZCClick() {
    removeMapOverLays();
    if ($("#buttonDivZCInput").val() == "on") {
        showZC();
        $("#buttonDivZCInput").val("off");
    }
    else {
        hideZC();
        $("#buttonDivZCInput").val("on");
    }
}

//隐藏所有正常企业
function hideYCB() {
    for (var i = 0; i < overLaysYCB.length; i++) {
        map.removeOverlay(overLaysYCB[i]);
    }
}

//显示所有正常企业
function showYCB() {
    for (var i = 0; i < overLaysYCB.length; i++) {
        map.addOverlay(overLaysYCB[i]);
    }
}

function showYCBClick() {
    removeMapOverLays();
    if ($("#buttonDivYCBInput").val() == "on") {
        showYCB();
        $("#buttonDivYCBInput").val("off");
    }
    else {
        hideYCB();
        $("#buttonDivYCBInput").val("on");
    }
}


//隐藏所有正常企业
function hideYLX() {
    for (var i = 0; i < overLaysYLX.length; i++) {
        map.removeOverlay(overLaysYLX[i]);
    }
}

//显示所有正常企业
function showYLX() {
    for (var i = 0; i < overLaysYLX.length; i++) {
        map.addOverlay(overLaysYLX[i]);
    }
}

//初始化页面所需数据--页面首次加载时调用
function initPageDatas() {
    companyGPSDatas = GetAllCompanyGPSDatas();
}

function showYLXClick() {
    removeMapOverLays();
    if ($("#buttonDivYLXInput").val() == "on") {
        showYLX();
        $("#buttonDivYLXInput").val("off");
    }
    else {
        hideYLX();
        $("#buttonDivYLXInput").val("on");
    }
}

//展示所有企业
function showAllCompany() {
    overLaysZC = [];
    overLaysYCB = [];
    overLaysYLX = [];
    for (var i = 0; i < companyGPSDatas.length; i++) {
        var point = new BMap.Point(companyGPSDatas[i].LONGITUDE, companyGPSDatas[i].LATITUDE);

        //在地图上显示人员图标
        var optsLabel = {
            position: point,    //指定文本标注所在的地理位置
            offset: new BMap.Size(-26, -15)   //设置文本偏移量
        }
        var label = new BMap.Label(companyGPSDatas[i].NUMBER, optsLabel);  //创建文本标注对象
        label.setStyle({
            color: "white",
            fontSize: "15px",
            height: "30px",
            width: "52px",
            lineHeight: "27px",
            textAlign: "center",
            border: "0",
            //background: "url(img/person-green.png)",    //背景图片
            fontFamily: "微软雅黑"
        });

        //点击人员图标弹窗
        var content = "";
        //content += "<table>";
        //content = content + "<tr><td> 编号：" + personDatas[i].id + "</td></tr>";
        ////content = content + "<tr><td>" + pointData.event + "</td></tr>";
        ////content = content + "<tr><td> 时间：" + pointData.time + "</td></tr>";
        //content += "</table>";
        content += "<iframe src='/PlatformIndex/WRYMap/DetailInfo?id=" + companyGPSDatas[i].ID + "' height='450px' width='585px' allowtransparency='true' style='border: none;'></iframe>";
        var optsClick = {
            width: 600,     // 信息窗口宽度
            height: 450,     // 信息窗口高度
            //title: companyData[i].Name, // 信息窗口标题
            enableMessage: true,//设置允许信息窗发送短息
            message: ""
        }
        var infoWindow = new BMap.InfoWindow(content, optsClick);
        (function (infoWindow, point) {
            //marker.addEventListener("click", function () {
            //    map.openInfoWindow(infoWindow, point);
            //});
            label.addEventListener("click", function () {
                map.openInfoWindow(infoWindow, point);
            });
        })(infoWindow, point);

        if (companyGPSDatas[i].STATUS == "1") {  //正常
            label.setStyle({
                background: "url(/Content/img/map/company-zc.png) no-repeat"    //背景图片
            });
            overLaysZC.push(label);
        }
        else if (companyGPSDatas[i].STATUS == "2") {  //超标
            label.setStyle({
                background: "url(/Content/img/map/company-ycb.png) no-repeat"    //背景图片
            });
            overLaysYCB.push(label);
        }
        else if (companyGPSDatas[i].STATUS == "3") {  //离线
            label.setStyle({
                background: "url(/Content/img/map/company-ylx.png) no-repeat"    //背景图片
            });
            overLaysYLX.push(label);
        }
        $("#buttonDivZCInput").val("off");
        $("#buttonDivYCBInput").val("off");
        $("#buttonDivYLXInput").val("off");
        map.addOverlay(label);
        //overLaysAllPerson.push(label);  //所有人员
    }

    $("#buttonDivZC span").html("正常(" + overLaysZC.length + ")");
    $("#buttonDivYCB span").html("有超标(" + overLaysYCB.length + ")");
    $("#buttonDivYLX span").html("有离线(" + overLaysYLX.length + ")");
}

//加载所有企业
function GetAllCompanyGPSDatas() {
    var url = "/PlatformIndex/WRYMap/GetAllCompanyGPSDatas";
    var arr = [];
    $.ajax({
        type: "post",
        async: false, //同步执行
        url: url,
        //contentType: "application/json",
        data: { IsVOCs: IsVOCs, Item: monitorItem },
        dataType: "json", //返回数据形式为json
        success: function (result) {
            arr = result.allCompanyGPSDatas;
        },
        error: function (errorMsg) {
        }
    });
    return arr;
}

//去除地图覆盖物，弹窗等
function removeMapOverLays() {
    $(".BMap_pop").hide();
    $(".BMap_shadow").hide();
}

function showItemIcon() {
    if (IsVOCs == "0") {
        debugger;
        $("#wryItemIcon").show();
        $("#wryItemIcon div").css("background-color", "#009DDA");
        
        if (monitorItem == "all") {
            $("#buttonDivWryAll").css("background-color", "green");
        }
        else if (monitorItem == "a21026_PFL_Value") {
            $("#buttonDivWrySO2").css("background-color", "green");
        }
        else if (monitorItem == "a34013_PFL_Value") {
            $("#buttonDivWryYC").css("background-color", "green");
        }
        else if (monitorItem == "a21002_PFL_Value") {
            $("#buttonDivWryDY").css("background-color", "green");
        }
        else if (monitorItem == "w01018_PFL_Value") {
            $("#buttonDivWryHX").css("background-color", "green");
        }
        else if (monitorItem == "w21003_PFL_Value") {
            $("#buttonDivWryAD").css("background-color", "green");
        }
    }
    else if (IsVOCs == "1") {
        $("#vocsItemIcon").show();
        $("#vocsItemIcon div").css("background-color", "#009DDA");
        if (monitorItem == "all") {
            $("#buttonDivVOCsAll").css("background-color", "green");
        }
        else if (monitorItem == "a25002_PFL_Value") {
            $("#buttonDivVOCsB").css("background-color", "green");
        }
        else if (monitorItem == "a25003_PFL_Value") {
            $("#buttonDivVOCsJB").css("background-color", "green");
        }
        else if (monitorItem == "a25005_PFL_Value") {
            $("#buttonDivVOCs2JB").css("background-color", "green");
        }
        else if (monitorItem == "a24088_PFL_Value") {
            $("#buttonDivVOCsFJY").css("background-color", "green");
        }
    }
}

function showWryAllClick() {
    monitorItem = "all"; 
    init(); 
}
function showWrySO2Click() {
    monitorItem = "a21026_PFL_Value";
    init(); 
}
function showWryYCClick() {
    monitorItem = "a34013_PFL_Value";
    init();
}
function showWryDYClick() {
    monitorItem = "a21002_PFL_Value";
    init();
}
function showWryHXClick() {
    monitorItem = "w01018_PFL_Value";
    init();
}
function showWryADClick() {
    monitorItem = "w21003_PFL_Value";
    init();
}
//VOCs
function showVOCsAllClick() {
    monitorItem = "all";
    init();
}
function showWVOCsBClick() {
    monitorItem = "a25002_PFL_Value";
    init();
}
function showVOCsJBClick() {
    monitorItem = "a25003_PFL_Value";
    init();
}
function showVOCs2JBClick() {
    monitorItem = "a25005_PFL_Value";
    init();
}
function showVOCsFJYClick() {
    monitorItem = "a24088_PFL_Value";
    init();
}