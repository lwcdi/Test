<%@ Page Title="" Language="C#" MasterPageFile="~/Content/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
      <script type="text/javascript" src="/Bas/Button/GetBtnColumns"></script>
    <style type="text/css">
        #toolbar {
            height: 32px !important;
            padding: 2px !important;
        }
    </style>
    <script type="text/javascript">
        //var dgCompanyData = {};
        var model;
        $(function () {
            mylayout.init();
            model = new listModel();

            var list = {
                controller: "/ThePeakManager/LevelList",//控制器
                dSize: { width: 10, height: 74 },//偏移量
                dg: { //数据表格
                    idField: "ID",
                    sortName: "ID",
                    sortOrder: "asc",
                    pagination: true,
                    columns: [[
                          { title: "编号", field: "LEVEL_NO", width: 120 },
                          { title: "级别", field: "PEAK_LEVEL_TEXT", width: 120 },
                          { title: "预警类型", field: "WAR_TYPE_TEXT", width: 120 },
                          { title: "说明", field: "PEAK_DESC", width: 120 },
                          { title: "停产企业数量", field: "STOP_NUM", width: 120 },
                          { title: "限产企业数量", field: "LIMITED_NUM", width: 120 },
                          { title: "时间段错峰企业数量", field: "TIME_LIMITED_NUM", width: 120 },
                          { title: "添加人", field: "TRUENAME", width: 120 },
                          { title: "添加时间", field: "ADD_TIME", width: 120, formatter: com.formatter.dateTime },                          { title: "操作", field: "ID", width: 120, formatter: function (value, row, index) {
                              return '<span class="resultDetail operatorBtn" data-areacode="' + value + '" onClick=showEdit(' + index + ')>配置</span>';
                          } }
                    ]]
                }
            };
            var dgList1 =  {
                id: "#dgCompany",
                //idField: "ID",
                //fitColumns: true,
                //fit: true,
                height: 320,
                width: 980,
                pagination: true,
                rownumbers: true,
                singleSelect: true,
                checkOnSelect: false,
                selectOnCheck: false,
                pageList: [10, 20, 30],
                pageSize: 10,
                columns: [[
                   { title: "行业类型", field: "INDUSTRY_TYPE_TEXT", width: 80 },
                   {
                       title: "污染类型", field: "POLLUTION_ITEM_TEXT", width: 80, formatter: function (value, row, index) {
                           if (!value) {
                               return "";
                           }
                           var text = "";
                           var items = value.split(',');
                           for (i in items) {
                               switch(items[i]){
                                   case "1": text = text + ",废水";break
                                   case "2": text = text + ",废气";break
                                   case "3": text = text + ",VOCs";break
                               }
                               
                           }
                           return text.substr(1, text.length);;
                       }
                   },
                   { title: "企业名称", field: "NAME", width: 250 },
                   {
                       title: '停产措施<input type="checkbox" id="stopChk" />', field: "ID", width: 80, formatter: function (value, row, index) {
                           return '<input id="stopID' + row.ID + '" type="checkbox"  class="stop"    data-companyid="' + row["ID"] + '" />';
                       }
                   },
                   {
                       title: "限产措施", field: "PHONE", width: 80,
                       formatter: function (value, row, index) {
                           return '<span class="limitConfig icon operatorBtn" title="配置" style="cursor:pointer;" data-companyid="' + row["ID"] + '" >配置</span>';//
                       }

                   },
                    {
                        title: "排污上限比例", field: "LIMIT_RATIO", width: 120, formatter: function (value, row, index) {
                            return '<span class="limitRatio" data-companyid="' + row["ID"] + '" ></span>';
                        }
                    },
                   {
                       title: "错峰时间", field: "LIMIT_TIME_START", width: 120, formatter: function (value, row, index) {
                           return '<span class="peakTime" data-companyid="' + row["ID"] + '" ></span>';
                       
                   }},
                ]],
                onDblClickRow: function (index, row) {
                    top.$("#dgCompany").datagrid("reload");
                },
                onLoadSuccess: function (data) {
                    top.$("#stopChk").unbind();
                    //dgCompanyData = data;
                    if (data) {
                        stopClick();
                        stopCheckAll();
                        stopCheckBoxSet();
                        peakTime();
                        limitRatio();
                        limitConfig();
                    }
                }
            }
            //对象2：编辑页面对象
            var edit = {
                title: "等级配置", //标题
                size: { width: 1000, height: 670 }, //页面大小

                dgList: [dgList1],

                onLoad: function (isAdd, row) { //页面加载事件
                    debugger;
                    com.comboxDicCreate("PEAK_LEVEL", "LEVEL_TYPE");
                    com.comboxDicCreate("WAR_TYPE", "Early_Warning");
                    if (isAdd != true && row && row.ID) {
                        //获取子列表数据
                        this.dgList[0].url = "/ThePeakManager/LevelList/GetCompanyList";
                        //填充编辑页面数据
                        fillField(row);
                        top.$("#ADD_TIME").val(com.formatter.dateTime(row["ADD_TIME"]));
                    }
                    else {
                        this.dgList[0].url = "/ThePeakManager/LevelList/GetCompanyList";
                        top.$("#ADD_TIME").val(com.currentDate());
                        com.ajax({
                            url: "/Bas/User/GetUserBaseInfo",
                            data: {},
                            success: function (result) {
                                top.$("#ADD_USER").val(result["USERNAME"]);
                                top.$("#TRUENAME").val(result["TRUENAME"]);
                            }
                        });
                    } 
                    levelCompanyConfig =[];
                    levelCompanyDischConfig = [];
                    levelCompanyEnergyConfig = [];
                    getCompanyAllConfig(row ? row["ID"] : "");
                    
                    //查询事件
                    top.$("#a_search_edit_page").click(function () {
                        var filter = new com.filterRules();
                        if (top.$("#SearchIndustryType").combobox('getText') != "全部" && top.$("#SearchIndustryType").combobox('getText') != "") {
                            filter.addFilterByID("SearchIndustryType", filter.filterOp.equal);
                        }
                        var node = top.$("#SearchAreaCode").combotree('tree').tree('getSelected');
                        if (node && node.attributes && node.attributes.relationtype == "4") {
                            filter.addFilterByID("SearchAreaCode", filter.filterOp.equal);
                        }
                        if (top.$("#SearchCompanyName").val() != "") {
                            filter.addFilterByID("SearchCompanyName", filter.filterOp.contains);
                        }
                        top.$("#dgCompany").datagrid("load", filter.returnResult()); 
                    });

                    //附件空间处理
                    var test = new fileUploadControler();
                    var initParam = {
                        htmlElementID: "files",
                        sourceID: row && row["ID"] ? row["ID"] : "",
                        sourceType: "T_THEPEAK_LEVEL_LIST_INFO",
                        sourceField: "attach0",
                        btnCss: "color:#ff0000;cursor:pointer;"
                    }
                    test.init(initParam);

                },
                beforeSubmit: function () {
                    top.$("#extraData").val(JSON.stringify(levelCompanyConfig));
                }
            };

            model.bind(list, edit);
            //其它初始化
            $(window).resize(function () {
                mylayout.resize();
            });
        });
        var showEdit = function (index) {
            $("#dg").datagrid('selectRow', index);
            model.showEdit(false, null);
        }

        var mylayout = {
            init: function () {
                var size = { width: $(window).width(), height: $(window).height() };
                $("#layout").width(size.width - 4).height(size.height - 4).layout();
                var center = $("#layout").layout("panel", "center");
                center.panel({
                    onResize: function (w, h) {
                        $("#dg").datagrid("resize", { width: w - 3, height: h - 70 });
                    }
                });
            }
            ,
            resize: function () {
                mylayout.init();
                $("#layout").layout("resize");
            }
        };
        //错峰等级企业设置
        var levelCompanyConfig = new Array();
        //错峰等级企业排放设置
        var levelCompanyDischConfig = new Array();
        //错峰等级企业能耗设置
        var levelCompanyEnergyConfig = new Array();
        //获取企业的错峰等级配置
        var getCompanyAllConfig = function (levelID) {
            debugger;
            com.ajax({
                url: "/ThePeakManager/LevelList/GetCompanyConfig",
                data:{"PEAK_LEVE_ID":levelID},
                success: function (result) {
                    if (result && result.Success && result.Data) {
                        com.ArrayEleCopy(levelCompanyConfig, result.Data.companyList);
                    } else {
                        //获取数据失败
                    }
                    calCompanyConfig();
                    stopCheckBoxSet();
                    peakTime();
                    limitRatio();
                }
            });
        }
        //计算停产、检查、限产企业的数量
        var calCompanyConfig = function () {
            var stop = 0;
            var limit = 0;//指定限产
            var timeLimit = 0;//时间段错峰限产

            $.each(levelCompanyConfig, function (i, value) {
                if (value && value["IS_STOP"] == 1) {
                    stop++;
                }
                if (value && value["LIMIT_TYPE"] == "TheLimit") {
                    limit++;
                }
                if (value && value["LIMIT_TYPE"] == "limitTime") {
                    timeLimit++;
                }
            });
            top.$("#STOP_NUM").val(stop);
            top.$("#LIMITED_NUM").val(limit);
            top.$("#TIME_LIMITED_NUM").val(timeLimit);
            dgLoadData();//停产多选框改变时，重新加载列表数据
        }
        var stopClick = function () {
            top.$(".stop").each(function (i, btn) {
                top.$(btn).click(function () {
                    var stopEle = top.$(this);
                    var companyID = stopEle.data("companyid")
                    if (stopEle.is(":checked")) {
                        var isNew = true;
                        $.each(levelCompanyConfig, function (i, configValue) {
                            if (configValue["COMPANY_ID"] == companyID) {
                                configValue["IS_STOP"] = "1";
                                delete configValue.LIMIT_RATIO;
                                delete configValue.ENERGY_RATIO;
                                delete configValue.LIMIT_TYPE;
                                delete configValue.LIMIT_START;
                                delete configValue.ENERGY_START;
                                delete configValue.LINE_ID;
                                delete configValue.LIMIT_CONFIG;
                                delete configValue.LIMIT_TIME_START;
                                delete configValue.LIMIT_TIME_END;
                                delete configValue.IS_POWER_OFF;
                                isNew = false;
                            }
                        })
                        if (isNew) {
                            levelCompanyConfig.push({
                                COMPANY_ID: companyID,
                                IS_STOP: "1"
                            });
                        }
                    }
                    else {
                        var isNew = true;
                        $.each(levelCompanyConfig, function (i, configValue) {
                            if (configValue["COMPANY_ID"] == companyID) {
                                configValue["IS_STOP"] = "0";
                                isNew = false;
                            }
                        })
                        if (isNew) {
                            levelCompanyConfig.push({
                                COMPANY_ID: companyID,
                                IS_STOP: "0"
                            });
                        }
                    }
                    calCompanyConfig();
                });
            })
        }
        //停产复选框设置（默认是否选中）
        var stopCheckBoxSet = function () {
            top.$(".stop").each(function (i, checkbox) {
                var stopEle = top.$(checkbox);
                var companyID = stopEle.data("companyid");
                $.each(levelCompanyConfig, function (j, value) {
                    if (value["COMPANY_ID"] == companyID && value["IS_STOP"] && value["IS_STOP"].toString() == "1") {
                        stopEle.attr("checked", 'true');
                    }
                });
            });
        }
        //停产全选或反选
        var stopCheckAll = function () {
            top.$("#stopChk").click(function () {
                var stopEle = top.$(this);
                if (stopEle.is(":checked")) {//全选
                    top.$(".stop").each(function (i, ele) {
                        var companyID = top.$(ele).data("companyid");
                        var isNew = true;
                        $.each(levelCompanyConfig, function (i, configValue) {
                            if (configValue["COMPANY_ID"] == companyID) {
                                configValue["IS_STOP"] = "1";

                                delete configValue.LIMIT_RATIO;
                                delete configValue.ENERGY_RATIO;
                                delete configValue.LIMIT_TYPE;
                                delete configValue.LIMIT_START;
                                delete configValue.ENERGY_START;
                                delete configValue.LINE_ID;
                                delete configValue.LIMIT_CONFIG;
                                delete configValue.LIMIT_TIME_START;
                                delete configValue.LIMIT_TIME_END
                                delete configValue.IS_POWER_OFF;

                                isNew = false;
                            }
                        })
                        if (isNew) {
                            levelCompanyConfig.push({
                                COMPANY_ID: companyID,
                                IS_STOP: "1"
                            });
                        }
                        top.$(ele).attr("checked", 'true');
                    });
                } else {//反选
                    top.$(".stop").each(function (i, ele) {
                        var companyID = top.$(ele).data("companyid");
                        $.each(levelCompanyConfig, function (i, configValue) {
                            if (configValue["COMPANY_ID"] == companyID) {
                                configValue["IS_STOP"] = "0";
                            }
                        })
                        top.$(ele).removeAttr("checked");;
                    });
                }
                calCompanyConfig();
            });
        }
        //错峰时间
        var peakTime = function () {
            top.$(".peakTime").each(function (i, sapn) {
                var sapnEle = top.$(sapn);
                var companyID = sapnEle.data("companyid");
                $.each(levelCompanyConfig, function (j, value) {
                    if (value["COMPANY_ID"] == companyID && value["LIMIT_TIME_START"]) {
                        sapnEle.html(value["LIMIT_TIME_START"] + "~" + value["LIMIT_TIME_END"]);
                    }
                });
            });
        }
        //排放上限比例
        var limitRatio = function () {
            top.$(".limitRatio").each(function (i, sapn) {
                var sapnEle = top.$(sapn);
                var companyID = sapnEle.data("companyid");
                $.each(levelCompanyConfig, function (j, value) {
                    if (value["COMPANY_ID"] == companyID && value["LIMIT_CONFIG"] == "1" && value["LIMIT_RATIO"]) {
                        sapnEle.html(value["LIMIT_RATIO"]+"%");
                    }
                });
            });
            
        }
        //排污上限配置、能耗上限配置
        var limitConfig = function () {
            top.$(".limitConfig").click(function () {
                var companyID = top.$(this).data("companyid");
                if (top.$("#stopID" + companyID).is(":checked")) {
                    com.msg.alert("已经选择停产，不能配置限产");
                }
                else {
                    limitConfigWindow(companyID);
                    //com.msg.alert(companyID);
                }
            })
        }
        var limitConfigWindow = function (companyID) {
            var peakLevelID = "";
            var config = {};
            $.each(levelCompanyConfig, function (i, value) {
                if (value["COMPANY_ID"] == companyID) {
                    peakLevelID = value["PEAK_LEVE_ID"];
                    config = value;
                    return;
                }
            })
            var dialog = top.$.dialogX({
                id: "LimitConfigdialogX",
                title: "限产措施配置",
                width: 400,
                height: 200,
                href: "/ThePeakManager/LevelList/LimitConfig?companyID=" + companyID + "&peakLevelID=" + peakLevelID,
                iconCls: "icon-edit",
                onLoad: function () {
                    top.$("#baseConfig").val(JSON.stringify(config));
                },
                buttons: [
                    {
                        text: "清空配置",
                        iconCls: "icon-cancel",
                        handler: function () {
                            var currentConfig = JSON.parse(top.$("#baseConfig").val());
                            for (i in levelCompanyConfig) {
                                if (levelCompanyConfig[i]["COMPANY_ID"] == currentConfig.COMPANY_ID) {
                                    levelCompanyConfig.splice(i, 1);
                                    break;
                                }
                            }
                            calCompanyConfig();
                            dgLoadData();
                            dialog.dialog("close");
                        }
                    },
                    {
                    text: "提交",
                    iconCls: "icon-ok",
                    handler: function () {
                            var isValid = top.$("#limitConfig").form("validate");
                            if (isValid != true) {
                                return;
                            }
                            var resultConfig = parent.GetReturnData();
                            var exist = false;
                            $.each(levelCompanyConfig, function (i, item) {
                                if (item && item.COMPANY_ID && item.COMPANY_ID == resultConfig.COMPANY_ID) {
                                    item = $.extend(item, resultConfig);
                                    exist = true;
                                }
                            });
                            if (!exist) {
                                levelCompanyConfig.push(resultConfig);
                            }
                            calCompanyConfig();
                            dgLoadData();
                            dialog.dialog("close");
                        }
                    },
                    {
                        text: "关闭",
                        iconCls: "icon-cancel",
                        handler: function () {
                            dialog.dialog("close");
                        }
                    }]
            });
        }
        //重新加载数据
        var dgLoadData = function () {
            var data = top.$("#dgCompany").datagrid("getData");         
            top.$("#dgCompany").datagrid("loadData", data);
        }
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="layout">
      
        <div region="center" title="错峰生产事件管理" iconcls="icon-cog_edit">
            <!--顶部工具栏-->
            <div id="toolbar">
<%--                <form id="qform">
                    <div style="float: left; padding: 5px;">
                        应急类型：
                        主题：
                        下发时间：
                        <input id="CompanyCode" name="CompanyCode" style="width: 140px;" />
                    </div>
                </form>--%>
                <%=Html.ToolBar(Model) %>
            </div>
            <!--查询表单-->
            <div style="margin: 2px;">
            </div>
            <!--数据列表-->
            <div id="dg"></div>
        </div>
    </div>
</asp:Content>
