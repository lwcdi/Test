<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<form id="uiform">
    <table id="dglevel"></table>
    <input id="PEAK_ID" name="PEAK_ID" type="hidden" />
</form>
<script type="text/javascript">
    var companyList = [];
    //弹出下发错峰窗口
    var ShowThePeakWindow = function (leveId,levelPeak,warType,isAdd, selectRow) {
        debugger;
        var leveId = leveId;
        var levePeak ;
        var peakId ;
        var entGuid;
        var warType;
        if (isAdd) {
            levePeak = levelPeak;
            warType = warType;
            peakId = "";

        }
        else {
            peakId = selectRow.ID;
        }
       
        //先把等级配置的企业写入到相关错峰关联表
        com.ajax({
            url: "/ThePeakManager/ThePeakMange/InsertConfigEnt",
            data: { LevelId: JSON.stringify(leveId), peakId: peakId },
            success: function (reulst) {
               

                if (reulst.Success) {
                   
                    entGuid=reulst.Data;
                    //删除未保存的企业ID
                  //  //debugger;
                  
                    var url = "/ThePeakManager/ThePeakMange/IssuedLevelEdit";
                    var dialog = top.$.dialogX({
                        title: "错峰任务下发",
                        width: 1450,
                        height: 850,
                        href: url,
                        iconCls: "icon-bullet_disk",
                        buttons: [{
                            text: "提交保存", iconCls: 'icon-add', handler: function () {
                                var data = { LevelId: JSON.stringify(leveId), entGuid: reulst.Data };
                                
                                data = $.extend(data, com.form.serialize("#ThePeakForm"));
                                com.ajax({
                                    url: "/ThePeakManager/ThePeakMange/SaveThePeak",
                                    data:data,
                                    success: function (reulst) {
                                        if (reulst.Success)
                                            com.msg.info("任务下发成功!", function () {
                                                //触发巡查
                                                //debugger;
                                                var entList = reulst.Data;
                                                if (entList[0] != null) {
                                                    var peakInfo = PeakInfoObj(entList[0]);
                                                    //dialog.dialog("close");
                                                  //  GetXunCha(entList);// var companyList=
                                                    //com.ajax({
                                                    //    url: "/Bas/Common/SaveInspectTaskNew",
                                                    //    data: { XCZT: peakInfo.XCZT, ZXMC: peakInfo.ZXMC, PLANCODE: peakInfo.PLANCODE, XCZLX: peakInfo.XCZLX, BTIME: peakInfo.BTIME, ETIME: peakInfo.ETIME, NRSM: peakInfo.NRSM, CHECKCOMPANY: JSON.stringify(companyList), NOTICETYPE: peakInfo.NOTICETYPE },// { "CFQY","巡查名称" ,},
                                                    //    success: function (reulst) {
                                                    //        dialog.dialog("close");

                                                    //    }
                                                    //});
                                                }
                                             
                                            dialog.dialog("close");
                                        });
                                       
                                    }
                                });

                               // dialog.dialog("close");
                            }
                        },
                                  {
                                      text: "关闭", iconCls: 'icon-cancel', handler: function () {
                                          dialog.dialog("close");
                                      }
                                  }],
                        onBeforeClose:function()
                        {
                            //debugger;
                            com.ajax({
                                url: "/ThePeakManager/ThePeakMange/CloseWinDelConfigEnt",
                                data: {  entGuid:reulst.Data },
                                success: function (reulst) {
                                    dialog.dialog("close");
                                }
                            });
                        },
                   
                      
                        onLoad: function () {
                           
                            var fileAtt = new fileUploadControler();
                            var initParam = {
                                htmlElementID: "fileAtt",
                                sourceID: "",
                                sourceType: "T_THEPEAK_MAIN_LIST_INFO",
                                sourceField: "ID",
                                btnCss: "color:#ff0000;cursor:pointer;"
                            }
                            fileAtt.init(initParam);
                            var option = {
                                url: "Bas/Common/GetAreaData_Select?appendAll=appendAll",
                                valueField: "CODE",
                                textField: "TITLE",
                                editable: false,
                                panelHeight: "auto",
                                onLoadSuccess: function (data)
                                {
                                    //debugger;
                                    //加载完毕时给予赋值
                                    com.ajax({
                                        url: "/ThePeakManager/ThePeakMange/GetCompanyAreaCode",
                                        data: { LevelId: JSON.stringify(leveId), entGuid: reulst.Data },
                                        async: false,
                                        success: function (reulst) {
                                            // dialog.dialog("close");
                                            var dataArray = reulst;
                                            top.$("#LIMIT_AREA").combobox("setValues", dataArray);
                                        }
                                    });
                                },
                                onSelect: function (rec)
                                {
                                    
                                    //debugger;
                                        //获取选项框里的checkId
                                        com.ajax({
                                            url: "/ThePeakManager/ThePeakMange/UpdateCompanyForAreaCode",
                                            data: { LevelId: JSON.stringify(leveId), entGuid: reulst.Data, areaCode: rec.CODE, isCheck: true, peakId: peakId },
                                            async: true,
                                            success: function (changeReulst) {
                                               top.$("#dgStopCompany").datagrid("reload");
                                              
                                          }
                                       });
                                },
                                onUnselect: function (rec)
                                {
                                    com.ajax({
                                        url: "/ThePeakManager/ThePeakMange/UpdateCompanyForAreaCode",
                                        data: { LevelId: JSON.stringify(leveId), entGuid: reulst.Data, areaCode: rec.CODE, isCheck: false },
                                        async: true,
                                        success: function (changeReulst) {
                                            top.$("#dgStopCompany").datagrid("reload");

                                        }
                                    });
                                }
                                  
                                    //进行业务操作，添加 删除  联动列表
                               // }
                            };
                            var option2 = {
                                url: com.constants().getConstant("DIC_URL") + "?typeCode=IndustryType",
                                valueField: "CODE",
                                textField: "TITLE",
                                editable: false,
                                panelHeight: "auto",
                                onLoadSuccess: function (data) {
                                    //debugger;
                                    //加载完毕时给予赋值
                                    com.ajax({
                                        url: "/ThePeakManager/ThePeakMange/GetCompanyIndustryType",
                                        data: { LevelId: JSON.stringify(leveId), entGuid: reulst.Data },
                                        async: false,
                                        success: function (reulst) {
                                            // dialog.dialog("close");
                                            var dataArray = reulst;
                                            top.$("#BUS_AREA").combobox("setValues", dataArray);
                                        }
                                    });
                                },
                                onSelect: function (rec) {

                                    //debugger;

                                    com.ajax({
                                        url: "/ThePeakManager/ThePeakMange/UpdateCompanyForIndustryType",
                                        data: { LevelId: JSON.stringify(leveId), entGuid: reulst.Data, IndustryType: rec.CODE, isCheck: true, peakId: peakId },
                                        async: true,
                                        success: function (changeReulst) {
                                            top.$("#dgStopCompany").datagrid("reload");

                                        }
                                    });
                                },
                                onUnselect: function (rec) {
                                    com.ajax({
                                        url: "/ThePeakManager/ThePeakMange/UpdateCompanyForIndustryType",
                                        data: { LevelId: JSON.stringify(leveId), entGuid: reulst.Data, IndustryType: rec.CODE, isCheck: false, peakId: peakId },
                                        async: true,
                                        success: function (changeReulst) {
                                            top.$("#dgStopCompany").datagrid("reload");

                                        }
                                    });
                                } 

                            };
                            com.ui.combobox.AddCheckbox("LIMIT_AREA", option, true);
                            com.ui.combobox.AddCheckbox("BUS_AREA", option2, true);      
                            com.ComboxBind("PEAK_LEVEL", "/ThePeakManager/ThePeakMange/GetPeakLevelData");
                            com.comboxDicCreate("WAR_TYPE", "Early_Warning");
                            top.InitAreaCode("stopAREACODE", "stopTYPE");
                            top.InitAreaCode("limitAREACODE", "limitTYPE");
                            top.InitAreaCode("timeAREACODE", "timeTYPE");
                            top.$("#PeakSelectTab").tabs({
                                onSelect: function () {
                                }
                            });
                            //获取企业数量
                            GetCompanyNum(entGuid);
                            if (isAdd) {
                                debugger;
                                // 获取编号
                                GetPeakNo();
                                //获取当前下发人
                                GetCurUserInfo();
                                top.$("#PEAK_LEVEL").combobox("setValue", levePeak);
                                top.$("#WAR_TYPE").combobox("setValue", warType);
                                top.$("#DOWN_TIME").datebox("setValue", com.currentDate());
                            }
                            else {
                                //this.buttons[0].hide();
                                //debugger;
                                com.SetEditReadonly(selectRow, true);
                                top.$("#LIMIT_AREA").combobox({ onShowPanel: function () { } });
                                top.$("#BUS_AREA").combobox({ onShowPanel: function () { } });
                                com.BinEditVal(top, selectRow);
                            
                                

                                //top.$("#STOP_NUM").text(objReulst[0].StopNum);
                                //top.$("#STOP_NUM").val(objReulst[0].StopNum);
                                //top.$("#LIMITED_NUM").text(objReulst[0].LimitNum);
                                //top.$("#LIMITED_NUM").val(objReulst[0].LimitNum);
                                //top.$("#THEPEAK_NUM").text(objReulst[0].TimeNum);
                                //top.$("#THEPEAK_NUM").val(objReulst[0].TimeNum);
                                //top.$("#ADD_USER").val(result["USERNAME"]);
                                //top.$("#DOWN_USERNAME").val(result["TRUENAME"]);
                                //top.$("#PEAK_NO").text(selectRow.PEAK_NO);
                                //top.$("#PEAK_NO").val(selectRow.PEAK_NO);
                                //top.$("#PEAK_LEVEL").combobox("setValue", selectRow.PEAK_LEVEL);
                                //top.$("#DOWN_TIME").datebox("setValue", selectRow.DOWN_TIME);
                            }

                            //debugger;
                            com.ListBind("dgStopCompany", "/ThePeakManager/ThePeakMange/GetCompanyData?LevelId="+leveId+"&ActionType=Stop&entGuid="+entGuid, {
                                height: 365,
                                singleSelect: false,
                                columns: [[

                                     {
                                         title: "选中", field: "COMPANY_ID", width: 80, formatter: function (value, row, index) {
                                            // //debugger;
                                             if (row.IS_SELECT == 1)
                                                 return '<span><input class="checkStop" type="checkbox"   onclick=CheckChange(false,' + row.ENTSUBID + ',' + row.LEVEL_ID + ',1,' + row.THEPEAKID + ',' + row.CONTROL_MEAS + ',"' + entGuid + '",'+value+',"dgStopCompany")  checked="checked" /></span>';
                                             else
                                                 return '<span><input class="checkStop" type="checkbox" onclick=CheckChange(true,' + row.ENTSUBID + ',' + row.LEVEL_ID + ',1,' + row.THEPEAKID + ',' + row.CONTROL_MEAS + ',"' + entGuid + '",' + value + ',"dgStopCompany")  /></span>';
                                         }
                                     },

                                 
                                    { field: "AREA_TEXT", title: "归属区域", width: 150, sortable: true },
                                    { field: "NAME", title: "企业名称", width: 80 },
                                     { title: "行业类型", field: "INDUSTRY_TYPE_TEXT", width: 100 },
                                    //{ field: "PRODUCTION_LINES_NUMBER", title: "生成线数量", width: 100 },
                                    //{
                                    //    title: "每条生成线生成能力", field: "ID", width: 120, formatter: function (value, row, index) {
                                    //        return '<span class="capacity" data-codes="' + row.ID + '"></span>';
                                    //    }
                                    //},
                                      //{ field: "", title: "", width: 100 },
                                       { field: "FUEL_TYPE_TEXT", title: "燃料类型", width: 100 },
                                       {
                                           title: "采取管控措施", field: "ENTSUBID", width: 130, align: "center",
                                           formatter: function (value, row, index) {
                                               return " <span>" + row.PEAK_LEVEL_TEXT + "响应</sapn><span class=\"icon operatorBtn\" title=\"调控\" style=\"cursor:pointer;\" onclick=\"ShowLevelDeailt(" + value + ",'dgStopCompany');\" >调控</span>";//
                                           }
                                       },
                                       //{ field: "STRATEGY", title: "策略", width: 100 },
                                      // { field: "OWNER", title: "企业法人", width: 100 },
                                      // { field: "TOWN_SUPERVISOR_TEXT", title: "乡镇办监管人", width: 100 },
                                     //  { field: "INDUSTRY_SUPERVISOR_TEXT", title: "主管部门监管人", width: 100 },
                                       { field: "SUPERVISOR_TEXT", title: "环保监管", width: 100 },
                                       {
                                           field: "CONTROL_TYPE", title: "是否是国、省、市控企业", width: 180,
                                           formatter: function (value, row, index) {
                                                  if(value=="0")
                                                      return '<span>国控</span>';
                                                  if (value == "1")
                                                      return '<span>省控</span>';
                                                  if (value == "2")
                                                      return '<span>市控</span>';
                                                  else
                                                      return '<span>否</span>';
                                        }
                                       }


                                ]],
                                onLoadSuccess: function (data) {
                                    //com.ListColspan(true, data, "/ThePeakManager/ThePeakMange/GetCapacityText", "capacity");
                                }
                            });
                            //限产
                            com.ListBind("dgLimitCompany", "/ThePeakManager/ThePeakMange/GetCompanyData?LevelId=" + leveId + "&ActionType=Limit&entGuid=" + entGuid, {
                                height: 365,
                                columns: [[
                                   {
                                       title: "选中", field: "COMPANY_ID", width: 80, formatter: function (value, row, index) {
                                           // //debugger;
                                           if (row.IS_SELECT == 1)
                                               return '<span><input class="checkStop" type="checkbox"   onclick=CheckChange(false,' + row.ENTSUBID + ',' + row.LEVEL_ID + ',1,' + row.THEPEAKID + ',' + row.CONTROL_MEAS + ',"' + entGuid + '",' + value + ',"dgLimitCompany")  checked="checked" /></span>';
                                           else
                                               return '<span><input class="checkStop" type="checkbox" onclick=CheckChange(true,' + row.ENTSUBID + ',' + row.LEVEL_ID + ',1,' + row.THEPEAKID + ',' + row.CONTROL_MEAS + ',"' + entGuid + '",' + value + ',"dgLimitCompany")  /></span>';
                                       }
                                   },

                                       { field: "AREA_TEXT", title: "归属区域", width: 150, sortable: true },
                                       { field: "NAME", title: "企业名称", width: 80 },
                                       { field: "TYPE", title: "行业类型", width: 100 },
                                       //{
                                       //    title: "每条生成线生成能力", field: "ID", width: 120, formatter: function (value, row, index) {
                                       //        return '<span class="capacity" data-codes="' + row.ID + '"></span>';
                                       //    }
                                       //},
                                          { field: "FUEL_TYPE_TEXT", title: "燃料类型", width: 100 },
                                          {
                                              title: "采取管控措施", field: "ENTSUBID", width: 130, align: "center",
                                              formatter: function (value, row, index) {
                                                  return " <span>" + row.PEAK_LEVEL_TEXT + "响应</sapn><span class=\"icon operatorBtn\" title=\"调整\" style=\"cursor:pointer;\" onclick=\"ShowLevelDeailt(" + value + ",'dgLimitCompany');\" >调控</span>";//
                                              }
                                          },
                                          { field: "STRATEGY", title: "策略", width: 100 },
                                          //{ field: "OWNER", title: "企业法人", width: 100 },
                                         // { field: "TOWN_SUPERVISOR_TEXT", title: "乡镇办监管人", width: 100 },
                                         // { field: "INDUSTRY_SUPERVISOR_TEXT", title: "主管部门监管人", width: 100 },
                                          { field: "SUPERVISOR_TEXT", title: "环保监管", width: 100 },
                                          {
                                              field: "CONTROL_TYPE", title: "是否是国、省、市控企业", width: 180,
                                              formatter: function (value, row, index) {
                                                  if (value == "0")
                                                      return '<span>国控</span>';
                                                  if (value == "1")
                                                      return '<span>省控</span>';
                                                  if (value == "2")
                                                      return '<span>市控</span>';
                                                  else
                                                      return '<span>否</span>';
                                              }
                                          }


                                ]],
                                onLoadSuccess: function (data) {
                                    //com.ListColspan(true, data, "/ThePeakManager/ThePeakMange/GetCapacityText", "capacity");
                                }
                            });
                            //时间错
                            com.ListBind("dgTimeCompany", "/ThePeakManager/ThePeakMange/GetCompanyData?LevelId=" + leveId + "&ActionType=TimePeak&entGuid=" + entGuid, {
                                height: 365,
                                columns: [[
                                       {
                                           title: "选中", field: "COMPANY_ID", width: 80, formatter: function (value, row, index) {
                                               // //debugger;
                                               if (row.IS_SELECT == 1)
                                                   return '<span><input class="checkStop" type="checkbox"   onclick=CheckChange(false,' + row.ENTSUBID + ',' + row.LEVEL_ID + ',1,' + row.THEPEAKID + ',' + row.CONTROL_MEAS + ',"' + entGuid + '",' + value + ',"dgTimeCompany")  checked="checked" /></span>';
                                               else
                                                   return '<span><input class="checkStop" type="checkbox" onclick=CheckChange(true,' + row.ENTSUBID + ',' + row.LEVEL_ID + ',1,' + row.THEPEAKID + ',' + row.CONTROL_MEAS + ',"' + entGuid + '",' + value + ',"dgTimeCompany")  /></span>';
                                           }
                                       },

                                        { field: "AREA_TEXT", title: "归属区域", width: 150, sortable: true },
                                        { field: "NAME", title: "企业名称", width: 80 },
                                        { field: "TYPE", title: "行业类型", width: 100 },
                                        //{ field: "PRODUCTION_LINES_NUMBER", title: "生成线数量", width: 100 },
                                        //{
                                        //    title: "每条生成线生成能力", field: "ID", width: 130, formatter: function (value, row, index) {
                                        //        return '<span class="capacity" data-codes="' + row.ID + '"></span>';
                                        //    }
                                        //},
                                        { field: "FUEL_TYPE_TEXT", title: "燃料类型", width: 100 },
                                        {
                                            title: "采取管控措施", field: "ENTSUBID", width: 130, align: "center",
                                            formatter: function (value, row, index) {
                                                return " <span>" + row.PEAK_LEVEL_TEXT + "响应</sapn><span class=\"icon operatorBtn\" title=\"调控\" style=\"cursor:pointer;\" onclick=\"ShowLevelDeailt(" + value + ",'dgTimeCompany');\" >调控</span>";//
                                            }
                                        },  
                                          // { field: "STRATEGY", title: "策略", width: 100 },
                                           //{ field: "OWNER", title: "企业法人", width: 100 },
                                          // { field: "TOWN_SUPERVISOR_TEXT", title: "乡镇办监管人", width: 100 },
                                          // { field: "INDUSTRY_SUPERVISOR_TEXT", title: "主管部门监管人", width: 100 },
                                           { field: "SUPERVISOR_TEXT", title: "环保监管", width: 100 },
                                           {
                                               field: "CONTROL_TYPE", title: "是否是国、省、市控企业", width: 180,
                                               formatter: function (value, row, index) {
                                                   if(value=="0")
                                                       return '<span>国控</span>';
                                                   if (value == "1")
                                                       return '<span>省控</span>';
                                                   if (value == "2")
                                                       return '<span>市控</span>';
                                                   else
                                                       return '<span>否</span>';
                                               }
                                           }


                                ]],
                                onLoadSuccess: function (data) { } // com.ListColspan(true, data, "/ThePeakManager/ThePeakMange/GetCapacityText", "capacity");
                            });

                        }
                    });

                }
            }
        });


       
      
    }

    var diff=  function (arr1, arr2) {
        var newArr = [];
        var arr3 = [];
        for (var i = 0; i < arr1.length; i++) {
            if (arr2.indexOf(arr1[i]) === -1)
                arr3.push(arr1[i]);
        }
        var arr4 = [];
        for (var j = 0; j < arr2.length; j++) {
            if (arr1.indexOf(arr2[j]) === -1)
                arr4.push(arr2[j]);
        }
        newArr = arr3.concat(arr4);
        return newArr;
    }
    var GetPeakNo = function ()
    {
        //获取编号
        com.ajax({
            url: "/ThePeakManager/ThePeakMange/GetAutoCode",
            async: false, //同步执行
            data: {},
            success: function (peakNo) {
                //debugger;
              //  var objReulst = peakNo.Data;//JSON.parse(peakNo.Data)
                top.$("#PEAK_NO").text(peakNo);
                top.$("#PEAK_NO").val(peakNo);


            }
        });
    }

    var GetCurUserInfo = function ()
    {
        com.ajax({
            url: "/Bas/User/GetUserBaseInfo",
            data: {},
            success: function (result) {
                top.$("#ADD_USER").val(result["USERNAME"]);
                top.$("#DOWN_USERNAME").val(result["TRUENAME"]);
            }
        });
    }
    var GetCompanyNum = function (entGuid)
    {
        //获取数量
        com.ajax({
            url: "/ThePeakManager/ThePeakMange/GetCompanyNum",
            data: { entGuid: entGuid },
            success: function (reulst2) {
                // //debugger;
                var objReulst = JSON.parse(reulst2.Data)
                top.$("#STOP_NUM").text(objReulst[0].StopNum);
                top.$("#STOP_NUM").val(objReulst[0].StopNum);
                top.$("#LIMITED_NUM").text(objReulst[0].LimitNum);
                top.$("#LIMITED_NUM").val(objReulst[0].LimitNum);
                top.$("#THEPEAK_NUM").text(objReulst[0].TimeNum);
                top.$("#THEPEAK_NUM").val(objReulst[0].TimeNum);

            }
        });
    }
    
    var GetXunCha = function (resultData)
    {
        $.each(resultData, function (i, n) {
            companyList.push(xunChaObj(this));
        });
        
    }
    var xunChaObj = function (resultData) {
        return {
            XCZT: "QYCF",
            AREACODE: resultData.AREACODE,
            CODE: resultData.COMPANY_ID,
            OWNER: resultData.SUPERVISOR,
            NAME: resultData.NAME,
            ADDRESS: resultData.NAME
        };
    }
    var  PeakInfoObj=function(peakObj){   
        return {
        XCZT: "QYCF",
        PLANCODE:peakObj.PEAK_NO,
        ZXMC:peakObj.PEAK_THEME,
        XCZLX: "ZLRCXC",
        BTIME: com.formatter.date(peakObj.START_TIME),
        ETIME: com.formatter.date(peakObj.END_TIME),
        NRSM:peakObj.PEAK_DESC,
        NOTICETYPE:"SMS"
       };
    }
  
</script>