/******************************************************************
 功能：单表作业类 listModel(可以包含n个子列表，子列表的信息在dgList)
 作者：  日期：2016.05.03
/******************************************************************/
var listModel = function () {
    com.lang.read(); //加载语言包
    var self = this;
    //对象1：列表页面对象
    this.list = {
        controller: "", //控制器
        dSize: { width: 4, height: 38 }, //偏移量
        queryFormId: "#qform", //查询表单ID
        dg: { //数据表格
            id: "#dg", //数据表格ID
            typeName: "datagrid" //数据表格类型名称(目前支持datagrid和treegrid)
        }
    };
    //对象2：编辑页面对象
    this.edit = {
        title: "编辑信息", //标题
        size: { width: 500, height: 300 }, //页面大小
        formId: "#uiform", //表单ID
        dgList:new Array(10)
    };
    //绑定方法
    this.bind = function (list, edit) {
        //设置list对象
        if (list.controller.substr(list.controller.length - 1, 1) == "/") {
            //去除最后一个斜杠
            list.controller = list.controller.substr(0, list.controller.length - 1);
        }
        self.list.controller = list.controller;
        self.list.dSize = $.extend(self.list.dSize, list.dSize);
        self.list.queryFormId = list.queryFormId || self.list.queryFormId;
        var dgOps = {
            url: self.list.controller + "/List",
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
            onDblClickRow: function (index, row) {
                if ($("#a_edit").length > 0) {
                    self.showEdit(false, row);
                }
            }
        };
        self.list.dg = $.extend(self.list.dg, dgOps, list.dg);
        ////自动打开编辑页面--开始
        //if ("function" == typeof self.list.dg.onLoadSuccess) {
        //    var tempFunction = self.list.dg.onLoadSuccess;
        //    delete self.list.dg.onLoadSuccess;
        //    debugger;
        //    self.list.dg.onLoadSuccess = function (data) {
        //        tempFunction(data);
        //        self.autoEditPage();
        //    }
        //} else {
        //    self.list.dg.onLoadSuccess = self.autoEditPage;
        //}
        if (com.helper.getUrlParam("OpenType") == "2") {
            var filter = new com.filterRules();
            filter.addFilter("ID", filter.filterOp.equal, com.helper.getUrlParam2('ID'))
            self.list.dg.queryParams = filter.returnResult();
        }
        //自动打开编辑页面--结束
        if (self.list.dg.typeName == "treegrid") {
            $(self.list.dg.id).treegrid(self.list.dg);
        }
        else {
            //debugger
            $(self.list.dg.id).datagrid(self.list.dg);
        }



        //增加filter
        if (self.list.dg.filterx) {
            $(self.list.dg.id).datagrid("enableFilterX", self.list.dg.filterx);
        }

        //设置edit对象
        self.edit = $.extend(self.edit, edit);
        //布局
        self.layout();
        $(window).resize(function () {
            self.layout();
        });

        //注册事件
        $("#a_search").click(self.searchClick);
        $("#a_add").click(self.addClick);
        $("#a_edit").click(self.editClick);
        $("#a_delete").click(self.deleteClick);
        $("#a_refresh").click(self.refreshClick);
        $("#a_view").click(self.viewClick);
        $("#a_ThePeak").click(self.addClick);
        $("#a_export").click(self.ExportClick);
    };
    //布局方法
    this.layout = function () {

        var wsize = { width: $(window).width(), height: $(window).height() };
        $(self.list.dg.id).datagrid("resize", {
            width: wsize.width - self.list.dSize.width, height: wsize.height - self.list.dSize.height
        });
    };
    this.searchClick = function () {
        var parms = {};
        if (self.list.queryFormId) {
            var data = com.form.serialize(self.list.queryFormId, true);
            parms = $.extend(data || {}, parms);
        }
        self.reload(parms);
    };
    this.addClick = function () {
        self.showEdit(true);
    };
    //导出
    this.ExportClick = function () {
        debugger;
        var data = com.form.serialize(self.list.queryFormId, true);
        com.ajax({
            url: self.list.controller + "/Export",
            data: data,
            success: function (result) {
                if (result.Success == true) {

                    com.msg.info("导出成功！");
                    window.open(result.Path);   
                    //self.reload();
                }
                else {
                    com.msg.error(result.Message);
                }
            }
        });
    };

    this.editClick = function () {
        self.showEdit(false);
    };
    this.viewClick = function () {
        var row = $(self.list.dg.id).datagrid("getSelected");
        if (!row) {
            com.msg.warning(com.lang.none_select_row);
            return;
        }
        self.showEdit(null);
    };
    this.deleteClick = function () {
        var row = $(self.list.dg.id).datagrid("getSelected");
        if (row) {
            com.msg.confirm(com.lang.del_confirm, function (r) {
                if (r) {
                    var data = self.getActionData(row[self.idField], row);
                    com.ajax({
                        url: self.list.controller + "/Delete",
                        data: data,
                        success: function (result) {
                            if (result.Success == true) {
                                com.msg.info(com.lang.del_success);
                                self.reload();
                            }
                            else {
                                com.msg.error(result.Message);
                            }
                        }
                    });
                }
            });
        } else {
            com.msg.warning(com.lang.none_select_row);
        }
    };
    this.planClick = function () {
        $("#planDateSpan").show();
        $("#planDate").datebox("showPanel");
    }
    //显示编辑弹出窗口
    this.showEdit = function (isAdd, rowData) {
        if (self.edit.beforeLoad) {
            if (self.edit.beforeLoad() == false)
                return;
        }
       
        var row = $(self.list.dg.id).datagrid("getSelected") || rowData;
        if (isAdd == false && !row) {
            com.msg.warning(com.lang.none_select_row);
            //com.msg.warning("请选择一行数据进行编辑！");
            return;
        }
        if (isAdd == true) {
            row = {};
        }
        var btns = [];
        btns.push({
            id: "d_save",
            text: "保存",
            iconCls: "icon-ok",
            handler: function () {
                if (self.edit.onBeforeSubmit) {
                    var ret = self.edit.onBeforeSubmit(row);
                    if (ret == false) return;
                }
                //子列表
                $.each(self.edit.dgList, function (index, dg) {
                    if (dg) {
                        var edg = top.$(dg.id);
                        edg.edatagrid("saveRow");
                    }
                });
                var isValid = com.form.validate(true);
                if (isValid) {
                    com.msg.confirm("您确定要保存更改的数据吗？", function (r) {
                        if (!r) return;
                        //提交数据之前
                        if (self.edit.beforeSubmit) {
                            self.edit.beforeSubmit();
                        }
                        // 设置保存、保存&新增不可用
                        //top.$("#d_save").linkbutton("disable");
                        //top.$("#d_saveAdd").linkbutton("disable");
                        var data =  {};
                        if(isAdd != true){
                            data[self.list.dg.idField] =  row[self.list.dg.idField];
                        }
                        data = $.extend(data,com.form.serialize(self.edit.formId));
                        
                        //获取子列表数据
                        $.each(self.edit.dgList, function (index, dg) {
                            if (dg) {
                                var edg = top.$(dg.id);
                                var rows = edg.edatagrid("getRows");
                                var detail = JSON.stringify(rows);
                                var dgData = {};
                                dgData["detail"+(index+1)]=detail;
                                data = $.extend(data, dgData);
                            }
                        });
                        com.ajax({
                            url: self.list.controller + "/" + (isAdd == true ? "Add" : "Edit"),
                            data: data,
                            success: function (result) {
                                // 设置保存、保存&新增可用
                                top.$("#d_save").linkbutton("enable");
                                if (result.Success == true) {
                                    dialog.dialog("close");
                                    self.reload();

                                    if (self.edit.onAfterSubmit) {
                                        self.edit.onAfterSubmit(row);
                                    }

                                    com.msg.info("保存成功!", function () {
                                        if ("function" == typeof self.edit.SaveAlter)
                                            self.edit.SaveAlter();
                                        //新增保存后保持编辑状态
                                        if (isAdd == true) {
                                            if (self.edit.getAddRowUrl && result.Message != "") {
                                                var idValue = result.Message;
                                                com.ajax({
                                                    url: self.edit.getAddRowUrl,
                                                    data: { id: idValue },
                                                    success: function (addedRow) {

                                                        if (addedRow) {
                                                            dialog.dialog("close");
                                                            self.showEdit(false, addedRow);
                                                        }
                                                    }
                                                });
                                            }
                                        }
                                    });
                                }
                                else {
                                    if (result.Message != "") {
                                        com.msg.error(result.Message);
                                    }
                                    else {
                                        com.msg.error("保存失败!");
                                    }
                                }
                            }
                        });
                    });
                }
                return false;
            }
        });
        btns.push({
            id: "d_close",
            text: "关闭",
            iconCls: "icon-cancel",
            handler: function () {
                $(self.list.dg.id).datagrid("reload");
                dialog.dialog("close");
                
            }
        });
        var dialog = top.$.dialogX({
            title: (isAdd == true ? "新增" : isAdd == false ? "编辑" : "查看") + self.edit.title,
            width: self.edit.size.width,
            height: self.edit.size.height,
            href: self.list.controller + "/Edit",
            iconCls: (isAdd == true ? "icon-add" : "icon-save"),
            buttons: btns,
            onLoad: function () {
                if (self.edit && self.edit.onLoad) {
                    //设置查看时下拉框、日期选择框不可用----开始 author：liwu
                    if (isAdd == null) {
                        parent.$('.dialog-button a:eq(0)').hide();
                        top.$("#uiform").find("input,textarea,select").attr('readonly', true);
                        top.$("#uiform").find("input,textarea,select").attr('disabled', true);

                        top.$("#uiform").find("input,select").each(function () {
                            if (top.$(this)[0].type == "select-one") {
                                top.$(this).combobox({
                                    onShowPanel: function (data) {
                                        top.$(this).combobox("hidePanel");
                                    }
                                });
                            }
                            if (top.$(this)[0].type == "text" && top.$(this)[0].className.indexOf("easyui-datebox") >= 0) {
                                //debugger;
                                top.$(this).datebox({
                                    onShowPanel: function (data) {
                                        top.$(this).datebox("hidePanel");
                                    },
                                    editable: false
                                });
                            }
                            if (top.$(this)[0].type == "text" && top.$(this)[0].className.indexOf("easyui-datetimebox") >= 0) {
                                top.$(this).datetimebox({
                                    onShowPanel: function (data) {
                                        top.$(this).datetimebox("hidePanel");
                                    },
                                    editable: false
                                });
                            }
                        });
                    }
                    //设置查看时下拉框、日期选择框不可用----结束 author：liwu
                    self.edit.onLoad(isAdd, row);
                    //self.edit.dgList1.post.getData();
                    //com.ui.combobox.addShowPanel(true);

                }
                top.$(".combo").click(function () {
                    top.$(this).prev().combobox("showPanel");
                });
                //子列表
                $.each(self.edit.dgList, function (index, dg) {
                    //debugger;
                    if (dg) {
                        var edg = top.$(dg.id);
                        var dgOpt = {
                            height: dg.height,
                            striped: true,
                            singleSelect: true,
                            onEdit: function (index, row) {
                            }
                        };
                        dgOpt = $.extend(dgOpt, dg);
                        edg.edatagrid(dgOpt);
                    }
                });

                //在加载编辑页面的datagrid初始化完成后执行，用于操作编辑页面的datagrid
                if (self.edit && self.edit.onLoaded) {
                    self.edit.onLoaded(isAdd, row);
                }
                if ("function" == typeof self.edit.openAfter)
                    self.edit.openAfter();
            },
            onBeforeDestroy: function () {
                if (com.helper.getUrlParam('OpenType') == "2") {
                    var f = top.window.frames;
                    var result = {
                        guid: self.edit.guid,
                        data: {}
                    };
                    //用户模拟回调函数
                    for (var i = 0; i < f.length; i++) {
                        f[i].postMessage(JSON.stringify(result), "*");
                    }
                    top.$("#tabs").tabs("close", self.edit.tabName);
                    return true;
                }
            }
        });
        return dialog;
    };

    this.addUpDownEvent = function (index) {
        var edg = top.$(self.edit.dgList1.id);
        var cols = self.edit.dgList1.columns[0];
        if (cols && cols.length > 0) {
            var eds = edg.edatagrid("getEditors", index);
            $.each(eds, function (i) {
                if (this.type != "combogrid") {
                    target = top.$(this.target);
                    top.$(target).keydown(function (e) {
                        if (e.keyCode == 38) { //向上键
                            edg.edatagrid("editRow", index - 1);
                            edg.edatagrid("selectRow", index - 1);
                            var eds2 = edg.edatagrid("getEditors", index - 1);
                            if (eds2[i]) {
                                top.$(eds2[i].target).focus().select();
                            }
                        }
                        if (e.keyCode == 40) { //向下键或者回车键
                            edg.edatagrid("editRow", index + 1);
                            edg.edatagrid("selectRow", index + 1);
                            var eds2 = edg.edatagrid("getEditors", index + 1);
                            if (eds2[i]) {
                                top.$(eds2[i].target).focus().select();
                            }
                        }
                    });
                }
            });
        }
    };


    this.getSelectedRow = function () {
        if (self.list.dgList1.typeName == "treegrid") {
            return $(self.list.dgList1.id).treegrid("getSelected");
        }
        else {
            return $(self.list.dgList1.id).datagrid("getSelected");
        }
    };
    this.getActionData = function (id, row) {
        var data = com.form.serialize(self.edit.formId);
        data = $.extend(row, data, { id: id });
        return data;
    };
    this.reload = function (parms) {
        if (parms) {
            if (self.list.dg.typeName == "treegrid") {
                $(self.list.dg.id).treegrid("clearSelections").treegrid("reload", parms);
            }
            else {
                $(self.list.dg.id).datagrid("clearSelections").datagrid("reload", parms);
            }
        }
        else {
            if (self.list.dg.typeName == "treegrid") {
                $(self.list.dg.id).treegrid("clearSelections").treegrid("reload");
            }
            else {
                $(self.list.dg.id).datagrid("clearSelections").datagrid("reload");
            }
        }
    };
    this.refreshClick = function () {
        parent.idx.doTab("refresh");
    };
    //自动打开编辑页面
    this.autoEditPage = function () {
        debugger;
        if (com.helper.getUrlParam("OpenType") == "2") {
            if (com.helper.getUrlParam2('isAdd') == "true") {
                self.showEdit(true);
                return;
            }
            if (com.helper.getUrlParam2('isAdd') == "null" || com.helper.getUrlParam2('isAdd') == "undefined") {
                var operate = null;
            } else {
                operate = false;
            }
            var levelID = com.helper.getUrlParam('ID');
            
            var dg = $(self.list.dg.id);
            var urlRow;
            var getRows = dg.datagrid("getRows");
            $.each(getRows, function (chekrow) {
                if (this.ID == levelID)
                    urlRow = getRows[chekrow];
            });            
            var index = dg.datagrid("getRowIndex", urlRow);
            if (index < 0) {
                return;
            }
            dg.datagrid("selectRow", index);
            self.edit.tabName = decodeURI(com.helper.getUrlParam2('tabName'));
            self.edit.guid = com.helper.getUrlParam2('guid');
            self.showEdit(operate, urlRow);
        }
    };
};