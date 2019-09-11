<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<style>
   #PeakSelectTab .tabs-title{color:red }</style>
<form id="ThePeakForm">
    <table class="grid">
        <tr>
            <td>编号:</td>
            <td > <input id="PEAK_NO" name="PEAK_NO" class="txt03" style="width: 200px"/></td>
            <td>应急等级:</td>
            <td>
                <select id="PEAK_LEVEL" name="PEAK_LEVEL" style="width: 120px" ></select>
            </td>
            <td>启动时间:</td>
            <td >
              <input id="START_TIME" name="START_TIME" type="text" class="txt03 easyui-datetimebox"  style="width:120px" />
            </td>
        </tr>
        <tr>
            <td>结束时间:  </td>
            <td>  <input id="END_TIME" name="END_TIME" type="text" class="txt03 easyui-datetimebox"  style="width:120px" /></td>
            <td>下发人:</td>
            <td><input id="DOWN_USERNAME" name="DOWN_USERNAME" class="txt03" style="width: 200px"/><input type="hidden" name="DOWN_USER" id="DOWN_USER" /></td>
            <td>下发时间:</td>
            <td>
                  <input id="DOWN_TIME" name="DOWN_TIME" type="text" class="txt03 easyui-datetimebox"  style="width:120px" />
            </td>
            
        </tr>
           <tr>
            <td>预警类型:</td>
            <td colspan="5">
               <select id="WAR_TYPE" name="WAR_TYPE" class="easyui-combobox" style="width: 120px" ></select>
            </td>
            
        </tr>
        <tr>
            <td>限制区域:</td>
            <td colspan="5">
               <select id="LIMIT_AREA" name="LIMIT_AREA" class="easyui-combobox" style="width: 900px" ></select>
            </td>
            
        </tr>
        <tr>
            <td>行业类型:</td>
            <td colspan="5">
               <select id="BUS_AREA" name="BUS_AREA" class="easyui-combobox" style="width: 900px" ></select>

            </td>
            
        </tr>
        <tr>
            
            <td>主题 :</td>
            <td colspan="5">
              <input id="PEAK_THEME" name="PEAK_THEME" class="txt03 easyui-validatebox" data-options="required:true" style="width: 900px"/> 
            </td>
        </tr>
        <tr>
            
            <td>说明:</td>
            <td colspan="5">
              <input id="PEAK_DESC" name="PEAK_DESC" class="txt03" style="width: 900px"/>
            </td>
        </tr>
        <tr>
            
            <td>相关附件:</td>
            <td colspan="5">
               <div id="fileAtt"></div>
            </td>
        </tr>
         <tr>
            
            <td>停产企业数量:</td>
            <td>
              <input id="STOP_NUM" name="STOP_NUM" class="txt03" readonly="readonly"  style="width: 120px"/>
            </td>
             <td>限产企业数量:</td>
            <td>
              <input id="LIMITED_NUM" name="LIMITED_NUM" class="txt03" readonly="readonly"  style="width: 120px"/>
            </td>
             <td>错峰企业数量:</td>
            <td>
              <input id="THEPEAK_NUM" name="THEPEAK_NUM" class="txt03" readonly="readonly" style="width: 120px"/>
            </td>

        </tr>
    </table>
     <div>

    <%--        <%=Html.LinkButton("a_AddCondition", "icon-add", "企业停产名称") %> &nbsp;&nbsp;&nbsp;
            <%=Html.LinkButton("a_EditCondition", "icon-add", "企业限产名称") %> &nbsp;&nbsp;&nbsp;
            <%=Html.LinkButton("a_delCondition", "icon-delete", "按时间段错峰") %><br />--%>
          <%--  <span style="font-family:'应用字体 Regular', '应用字体';">企业停产名称</span>
            <span style="font-family:'应用字体 Regular', '应用字体';">企业限产名称</span>
            <span style="font-family:'应用字体 Regular', '应用字体';">按时间段错峰</span>--%>
        
     </div>
     
    
     <%-- <table class="grid" id="gdIssuedList"></table>--%>
     <div id="PeakSelectTab" fit="true" style="height: 200px; overflow: hidden;color:red">
      <div title="企业停产名称" style="border-bottom: none; padding: 2px; ">
         <div  title="企业查询"   style="border-bottom: none; padding:10px;" >
            <label >企业名称:</label>
            <input id="stopNAME" name="NAME" class="txt03 inputSize" />
            <label >区域:</label>
              <select id="stopAREACODE" name="AREACODE" class="txt03 inputSize"  ></select>
             <label >行业类型:</label>
              <select id="stopTYPE" name="INDUSTRY_TYPE" class="txt03 inputSize" ></select>
               <%=Html.LinkButton("a_SearchStop", "icon-search", "查询") %>
     </div>
            <table id="dgStopCompany"></table>
        </div>
        <div title="企业限产名称" style="  border-bottom: none; padding: 2px">
            <div  title="企业查询"   style="border-bottom: none; padding:10px; " >
            <label >企业名称:</label>
            <input id="limitNAME" name="NAME" class="txt03" >
            <label >区域:</label>
              <select id="limitAREACODE" name="AREACODE" class="txt03 inputSize" ></select>
             <label >行业类型:</label>
              <select id="limitTYPE" name="INDUSTRY_TYPE" class="txt03 inputSize" ></select>
            
               <%=Html.LinkButton("a_SearchLimit", "icon-search", "查询") %>
     </div>
            <table id="dgLimitCompany"></table>
        </div>
        <div title="按时间段错峰" style="border-bottom: none; padding: 2px">
             <div  title="企业查询"   style="border-bottom: none; padding:10px; " >
            <label >企业名称:</label>
            <input id="timeNAME" name="NAME" class="txt03" />
                 <label >区域:</label>
              <select id="timeAREACODE" name="AREACODE" class="txt03 inputSize" ></select>
             <label >行业类型:</label>
              <select id="timeTYPE" name="INDUSTRY_TYPE" class="txt03 inputSize" ></select>
           
               <%=Html.LinkButton("a_SearchTime", "icon-search", "查询") %>
     </div>
            <table id="dgTimeCompany"></table>
        </div>
        </div>
    <input id="STRATEGY_RANGE_LEVEL" name="STRATEGY_RANGE_LEVEL"  type="hidden" />
     <input id="IS_DISBILE" name="STRATEGY_RANGE_LEVEL"  type="hidden" />
</form>


<script>
    debugger;
    var InitAreaCode = function (areid, indusId) {
        com.comboxDicCreateSearch(indusId, "IndustryType");
       // return;
        $("#" + areid).combotree({
            url: "/Bas/Common/GetAreaData",
            editable: false,
            checkbox: true,
            lines: true,
            onSelect: function (item) {
            }
        });
    }
   

   
    //子列表选中事件
    var CheckChange = function (isCheck, entId, levelId, entType, peakId, controlId, entGuid,companyId,dgId) {
        debugger;
        com.ajax({
            url: "/ThePeakManager/ThePeakMange/CheckConfigEnt",
            data: { isCheck: isCheck, LevelId: levelId, entId: entId, entType: entType, peakId: peakId, controlId: controlId, entGuid: entGuid, companyId: companyId },
            success: function (reulst) {
                top.$("#" + dgId).datagrid("reload");
                com.ajax({
                    url: "/ThePeakManager/ThePeakMange/GetCompanyNum",
                    data: { entGuid: entGuid },
                    success: function (reulst2) {
                        top.$("#LIMIT_AREA").combobox("reload");
                        top.$("#BUS_AREA").combobox("reload", com.constants().getConstant("DIC_URL") + "?typeCode=IndustryType");
                        debugger;
                        var objReulst =  JSON.parse(reulst2.Data)
                        top.$("#STOP_NUM").text(objReulst[0].StopNum);
                        top.$("#STOP_NUM").val(objReulst[0].StopNum);
                        top.$("#LIMITED_NUM").text(objReulst[0].LimitNum);
                        top.$("#LIMITED_NUM").val(objReulst[0].LimitNum);
                        top.$("#THEPEAK_NUM").text(objReulst[0].TimeNum);
                        top.$("#THEPEAK_NUM").val(objReulst[0].TimeNum);

                    }
                });
               
               // dialog.dialog("close");

            }
        });


          
    }

    var ShowLevelDeailt = function (entId,dgId)
    {
        var dialog = top.$.dialogX({
            title: "等级选项",
            width: 600,
            height: 400,
            href: "/ThePeakManager/ThePeakMange/ChangeLevel?entId=" + entId,
            onLoad: function () {
                var edg = top.$("#dgSelectlevel");
                var dgOps = {
                    url: "/ThePeakManager/ThePeakMange/GetLevelConfigData",
                    idField: "ID",
                    sortName: "ID",
                    sortOrder: "desc",
                    rownumbers: true,
                    singleSelect: true,
                    autoRowHeight: false,
                    striped: true,
                    pagination: true,
                    pageSize: 50,
                    pageList: [50, 100, 150],
                    height: 350,
                    width: 800,
                    // formatter: function (value, row, index) { return '<span><input class="checkLevel" type="checkbox" data-checkLeveId=' + value + '   /></span>';}
                    //datagrid-cell-check
                    columns: [[
                    {
                        title: "选中", field: "ID", width: 80,checkbox:true 
                    },
                   { title: "编号", field: "LEVEL_NO", width: 80 },
                   { title: "级别", field: "PEAK_LEVEL", width: 80 },
                   { title: "预警类型", field: "WAR_TYPE", width: 80 },
                   { title: "说明", field: "PEAK_DESC", width: 120 },
                   { title: "全部企业数量", field: "TIME_LIMITED_NUM", width: 80 },
                   { title: "停产企业数量", field: "STOP_NUM", width: 80 },
                   { title: "限产企业数量", field: "LIMITED_NUM", width: 80 },
                  
                    ]],
                    onDblClickRow: function (index, row) {

                    }
                };
                edg.edatagrid(dgOps);
 
            },
            buttons: [{
                text: "确定", iconCls: 'icon-add', handler: function () {
                    var leveId;
                    debugger;
                    $(".datagrid-cell-check").each(function (index, element) {
                        if ($(element).find("input[type='checkbox']").is(':checked'))
                        {
                            leveId = $(element).find("input[type='checkbox']").val();
                        }
                    }
                    );
                    com.ajax({
                        url: "/ThePeakManager/ThePeakMange/CheckUpdateLevel",
                        data: { LevelId: leveId, entId: entId },
                        success: function (reulst) {
                            top.$("#" + dgId).datagrid("reload");
                            dialog.dialog("close");
                          
                        }
                    });
                    //dialog.dialog("close");
                }
            },
                                 {
                                     text: "关闭", iconCls: 'icon-cancel', handler: function () {
                                                 dialog.dialog("close");
                                     }
                                 }],
            submit: function () {
            }
        });
    }

    $("#a_SearchStop").click(function () {
       
        SearchTableList("stopNAME", "stopAREACODE", "stopTYPE", "dgStopCompany");
    });
    $("#a_SearchLimit").click(function () {

        SearchTableList("limitNAME", "limitAREACODE", "limitTYPE", "dgLimitCompany");
    });
    $("#a_SearchTime").click(function () {

        SearchTableList("timeNAME", "timeAREACODE", "timeTYPE", "dgTimeCompany");
    });
    var SearchTableList = function (companid,arecodeId,typId,dglistId)
    {
        debugger;
        var filter = new com.filterRules();
        if (top.$("#" + typId).combobox('getText') != "全部" && top.$("#" + typId).combobox('getText') != "") {
            filter.addFilterByID(typId, filter.filterOp.equal);
        }
        var node = top.$("#" + arecodeId).combotree('tree').tree('getSelected');
        if (node && node.attributes && node.attributes.relationtype == "4") {
            filter.addFilterByID(arecodeId, filter.filterOp.equal);
        }
        if (top.$("#" + companid).val() != "") {
            filter.addFilterByID(companid, filter.filterOp.contains);
        }
        top.$("#" + dglistId).datagrid("load", filter.returnResult());
        

    }

    

    
</script>