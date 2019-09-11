(function ($) {
    //封装easyui的$.dialog为$.dialogX
    $.dialogX = function (options) {
        var getDialogId = function () {
            var s4 = function () {
                return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
            };
            return "k3-" + s4() + "-" + s4() + "-" + s4();
        };

        var defaultOptions = {
            boxPadding: "3px",
            align: "right", //按钮对齐方式
            href: "",
            id: "",
            content: "",
            height: 200,
            width: 400,
            collapsible: false,
            minimizable: false,
            maximizable: false,
            closable: true,
            modal: true,
            shadow: false,
            mask: true,
            cache: false,
            closed: false,//默认是否关闭窗口 如果为true,需调用open方法打开
            showBtns: true,
            buttons: [],
            submit: function () {
                com.msg.alert("none code");
                return false;
            },
            onBeforeClose: function () {
                $(this).find(".combo-f").each(function () {
                    var panel = $(this).data().combo.panel;
                    panel.panel("destroy");
                });
                $(this).empty();
            },
            onMove: function (left, right) {
                $(".validatebox-tip").remove();
            }
        };
        options = $.extend({}, $.fn.dialog.defaults, defaultOptions, options || {});

        var dialogId = getDialogId();
        if (options.id) {
            dialogId = options.id;
        }

        var defaultBtn = [{
            text: "提交",
            iconCls: "icon-ok",
            handler: options.submit
        }, {
            text: "关闭",
            iconCls: "icon-cancel",
            handler: function () {
                $("#" + dialogId).dialog("close");
            }
        }];

        if (!options.showBtns)
            defaultBtn = [];

        if (options.buttons.length == 0)
            options.buttons = defaultBtn;

        if (options.max) {
            var winWidth = $(window).width();
            var winHeight = $(window).height();
            options.width = winWidth - 20;
            options.height = winHeight - 20;
        }

        var $dialog = $("<div/>").css("padding", options.boxPadding).appendTo($("body"));

        var dialog = $dialog.dialog($.extend(options, {
            onClose: function () {
                dialog.dialog("destroy");
            }
        })).attr("id", dialogId);

        $dialog.find(".dialog-button").css("text-align", options.align);

        return dialog;
    };

    //扩展datagrid 方法 getSelectedIndex
    $.extend($.fn.datagrid.methods, {
        getSelectedIndex: function (jq) {
            var row = $(jq).datagrid("getSelected");
            if (row)
                return $(jq).datagrid("getRowIndex", row);
            else
                return -1;
        },
        //获取所有行副本(扩展方法)
        getCopyRows: function (jq) {
            var rows = $(jq).datagrid("getRows");
            var rows2 = [];
            if (rows) {
                $.each(rows, function () {
                    rows2.push(this);
                });
            }
            return rows2;
        },
        //开启过滤功能(扩展方法)
        enableFilterX: function (jq, cols) {
            var dg = $(jq);
            dg.datagrid({
                remoteFilter: true,
                filterBtnIconCls: 'icon-filter',
                filterDelay: 3600000
            });
            //页面加载时，把所有过滤条件清空
            dg.datagrid("removeFilterRule");
            if (cols != null) {
                var existCombobox = false;
                $.each(cols, function () {
                    if (this.type == "numberbox") {
                        if (!this.options) {
                            this.options = { precision: 0 };
                        }
                        this.op = ["equal", "less", "greater"]
                    }
                    if (this.type == "datebox") {
                        var col = this;
                        this.options = {
                            onChange: function (value) {
                                if (value == "") {
                                    dg.datagrid("removeFilterRule", col.field);
                                } else {
                                    dg.datagrid("addFilterRule", {
                                        field: col.field,
                                        op: "equal",
                                        value: value
                                    });
                                }
                                dg.datagrid("doFilter");
                            }
                        };
                        this.op = ["equal", "less", "greater"]
                    }
                    if (this.type == "combobox") {
                        existCombobox = true;
                        var col = this;
                        this.options = {
                            panelHeight: "auto",
                            data: this.data || ["全部"],
                            onChange: function (value) {
                                if (value == "") {
                                    dg.datagrid("removeFilterRule", col.field);
                                } else {
                                    dg.datagrid("addFilterRule", {
                                        field: col.field,
                                        op: "equal",
                                        value: value
                                    });
                                }
                                dg.datagrid("doFilter");
                            }
                        }
                        delete col.data;
                    }
                });
                dg.datagrid("enableFilter", cols);
            }
            else {
                dg.datagrid("enableFilter");
            }
            if (existCombobox == true) {
                //给产品下拉框增加点击后展开面板效果
                $(".combo").click(function () {
                    $(this).prev().combobox("showPanel");
                });
            }


        }
    });

    //扩展 combobox 方法 selectedIndex
    $.extend($.fn.combobox.methods, {
        selectedIndex: function (jq, index) {
            if (!index)
                index = 0;
            var data = $(jq).combobox('options').data;
            var vf = $(jq).combobox('options').valueField;
            $(jq).combobox('setValue', eval('data[index].' + vf));
        }
    });

    //释放IFRAME内存
    $.fn.panel.defaults = $.extend({}, $.fn.panel.defaults, {
        onBeforeDestroy: function () {
            var frame = $('iframe', this);
            if (frame.length > 0) {
                frame[0].contentWindow.document.write('');
                frame[0].contentWindow.close();
                frame.remove();
                if ($.browser.msie) {
                    CollectGarbage();
                }
            }
        }
    });

    //扩展datagrid的editors，增加combogrid
    $.extend($.fn.datagrid.defaults.editors, {
        combogrid: {
            init: function (container, options) {
                var input = $('<input class="combogrid-editable-input" />').appendTo(container);
                input.combogrid(options);
                return input;
            },
            destroy: function (target) {
                $(target).combogrid("destroy");
            },
            getValue: function (target) {
                return $(target).combogrid("getValue");
            },
            setValue: function (target, value) {
                $(target).combogrid("setValue", value);
            },
            resize: function (target, width) {
                $(target).combogrid("resize", width);
            }
        }
    });
})(jQuery)