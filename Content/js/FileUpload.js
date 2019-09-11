//文件上传控件
var fileUploadControler = function () {
    var self = this;
    this.GUID = new Date().getTime() + "_" + Math.round(Math.random() * 10000);
    this.param = {
        //input文件输入框
        fileUpload_input: "fileUpload" + self.GUID,
        //输入按钮
        fileUpload_button: "fileUploadButton" + self.GUID,
        //保存文件上传后的信息
        fileInfo_input_hidden: "fileInfo" + self.GUID,
        //上传文件列表展示
        fileListShow: "fileListShow" + self.GUID,
        //显示上传后文件的信息
        uploadFileInfo_span: "uploadFileInfo" + self.GUID,
        //文件上传百分比
        uploadPercent_span: "uploadPercent" + self.GUID,
        //文件上传路径
        potsUrl: "/Content/code/Handler/FileUpload.ashx",//.Content.code.Handler
        //控件生成的html填充ID
        htmlID: "",
        //T_SYS_ATTACH表的SOURCE_ID
        sourceID: "",
        //T_SYS_ATTACH表的SOURCE_TYPE
        sourceType: "",
        //T_SYS_ATTACH表的SOURCE_FIELD
        sourceField: "",
        btnCss: "",
        fileCss: "",
        deleteBtnCss:""
    };
    this.htmlShow = "";
    this.existFileInfoArray = new Array();
    this.fileSelected = function () {
        var file = top.document.getElementById(self.param.fileUpload_input).files[0];
        if (file) {
            var fileSize = 0;
            if (file.size > 1024 * 1024)
                fileSize = (Math.round(file.size * 100 / (1024 * 1024)) / 100).toString() + 'MB';
            else
                fileSize = (Math.round(file.size * 100 / 1024) / 100).toString() + 'KB';
            top.$("#" + self.param.uploadFileInfo_span).html(file.name + "&nbsp;" + fileSize)
        }
    };
    this.uploadFile = function () {
        var fd = new FormData();
        var fileUploadEle = top.document.getElementById(self.param.fileUpload_input);
        var fileCount = fileUploadEle.files.length;
        //debugger;
        for (var i = 0; i < fileCount; i++) {
            fd.append("fileUpload" + i, fileUploadEle.files[i]);
        }
        if (fileCount > 0) {
            var xhr = new XMLHttpRequest();
            xhr.upload.addEventListener("progress", self.uploadProgress, false);
            xhr.addEventListener("load", self.uploadComplete, false);
            xhr.addEventListener("error", self.uploadFailed, false);
            xhr.addEventListener("abort", self.uploadCanceled, false);
            xhr.open("POST", self.param.potsUrl);
            xhr.send(fd);
        }
    };
    this.uploadProgress = function (evt) {
        if (evt.lengthComputable) {
            var percentComplete = Math.round(evt.loaded * 100 / evt.total);
            top.$("#uploadPercent").html("文件正在上传----" + percentComplete.toString() + '%');
        }
        else {
            top.$("#uploadPercent").html('不能计算');
        }
    };
    this.uploadComplete = function (evt) {
        top.$("#" + self.param.fileInfo_input_hidden).val(evt.target.responseText);
        var uploadFileInfo = JSON.parse(evt.target.responseText);
        for (j in uploadFileInfo) {
            if (uploadFileInfo && uploadFileInfo[j]) {
                self.existFileInfoArray.push(uploadFileInfo[j]);
            }
        }
        self.showFileInfo();
        self.fileInfoTempSave();
    };
    //展示文件的信息
    this.showFileInfo = function () {

        self.htmlShow = ""
        $.each(self.existFileInfoArray, function (index, value) {
            var fileEle = '<span class="fileInfo"  style="cursor:pointer;' + self.param.fileCss + '" onclick="downloadFile(this)" data-fileType="' + value.FileType + '" data-filename="' + value.Name + '" data-fileurl="' + value.Url + '" data-actionurl="' + self.param.potsUrl + '" data-fileinfoid="' + value.fileInfoID + '">' + value.Name + '</span>'
            fileEle = fileEle + '<span class="fileInfoDelete" style="color:#ff0000;cursor:pointer;margin-right:10px;margin-left:5px;' + self.param.fileCss + '" data-fileinfoid="' + value.fileInfoID + '">×</span>'
            self.htmlShow = self.htmlShow + fileEle;
        });
        top.$("#" + self.param.uploadFileInfo_span).html(self.htmlShow);
        top.$("#" + self.param.uploadPercent_span).html('');
        //可能会有多个实例化  id选择器加类选择器过滤
        top.$("#" + self.param.uploadFileInfo_span + " .fileInfo").click(function () {
            self.downloadFile(this);
        })
        top.$("#" + self.param.uploadFileInfo_span + " .fileInfoDelete").click(function () {
            self.deleteFile(this);
        });
    };
    this.deleteFile = function (caller) {
        var fileinfoID = $(caller).data("fileinfoid");
        $.each(self.existFileInfoArray, function (index, value) {
            if (value && value.fileInfoID == fileinfoID) {
                self.existFileInfoArray.splice(index, 1);//删除文件信息
            }
        })
        //重新展示文件信息
        self.showFileInfo();
        self.fileInfoTempSave();
    }
    this.downloadFile = function (caller) {
        debugger;
        var fileName = $(caller).data("filename");//文件下载的名称
        var fileUrl = $(caller).data("fileurl");//文件的相对路径
        var url = $(caller).data("actionurl");//请求路径
        var form = $("<form>");//定义form表单,通过表单发送请求
        form.attr("style", "display:none");//设置为不显示
        form.attr("target", "");
        form.attr("method", "get");//设置请求类型  
        form.attr("action", url);//设置请求路径
        // 创建Input  
        var wayEle = $('<input type="text" name="way" />');
        wayEle.attr('value', "DownLoadFile");
        var fileNameEle = $('<input type="text" name="filename" />');
        fileNameEle.attr('value', fileName);
        var fileUrlEle = $('<input type="text" name="fileUrl" />');
        fileUrlEle.attr('value', fileUrl);
        form.append(wayEle);
        form.append(fileNameEle);
        form.append(fileUrlEle);
        $("body").append(form);//添加表单到页面(body)中
        form.submit();//表单提交
    }
    this.uploadFailed = function (evt) {
        com.msg.alert("文件上传失败");
    };
    this.uploadCanceled = function (evt) {
        com.msg.alert("取消文件上传");
    }
    this.getUploadIfileInfo = function () {
        return top.$("#" + self.param.fileInfo_input_hidden).val();
    };
    this.clearUploadIfileInfo = function () {
        top.$("#" + self.param.fileInfo_input_hidden).val("");
        top.$("#" + self.param.uploadFileInfo_span).html("");
        top.$("#" + self.param.fileUpload_input).val("");
    };
    this.createHtml = function () {
        var innerHtmls = "";
        innerHtmls += '<table class="fileUpload">';
        innerHtmls += '<tr>';
        innerHtmls += '<td style="width:90px">';
        innerHtmls += '<input id="' + self.param.fileUpload_input + '" type="file" name="fileUpload" multiple="multiple" style="display:none"/>';//----
        innerHtmls += '<input id="' + self.param.fileUpload_button + '" type="button" value="文件上传" style="width:80px;'+self.param.btnCss+'" />';
        innerHtmls += '<input id="' + self.param.fileInfo_input_hidden + '" type="hidden" /><!--保存文件上传后的信息-->';
        innerHtmls += '<input id="temp' + self.param.fileInfo_input_hidden + '" name="attach" type="hidden" value="[]" /><!--文件信息-->';
        innerHtmls += '<input id="source' + self.param.fileInfo_input_hidden + '" name="source" type="hidden" value="[]" /><!--源信息-->';
        innerHtmls += '</td>';
        innerHtmls += '<td style="">';
        //innerHtmls += '<span>文件列表</span>';
        innerHtmls += '<div id="' + self.param.fileListShow + '"></div>';
        innerHtmls += '<div >';
        innerHtmls += '<span id="' + self.param.uploadFileInfo_span + '"></span>';
        innerHtmls += '<span id="' + self.param.uploadPercent_span + '"></span>';
        innerHtmls += '</div>';
        innerHtmls += '</td>';
        innerHtmls += '</tr>';
        innerHtmls += '</table>';
        top.$("#" + self.param.htmlID).html(innerHtmls);
    }
    //加载原有文件
    this.originalFile = function () {
        $.ajax({
            type: "POST",
            dataType:"json",
            url: self.param.potsUrl,
            data: { "way": "OriginalFile", "SOURCE_ID": self.param.sourceID, "SOURCE_TYPE": self.param.sourceType, "SOURCE_FIELD": self.param.sourceField },
            success: function (resultData) {
                if (resultData && resultData.length > 0) {
                    $.each(resultData, function (index, value) {
                        self.existFileInfoArray.push(value);
                    });
                    self.showFileInfo();
                    self.fileInfoTempSave();
                }
            }
        });
        
    }

    //文件信息保存（不是保存文件）
    //backCall是回调函数，参数传入保存结果
    this.fileInfoSave = function (sourceID,backCall) {
        if (sourceID) {
            self.param.sourceID = sourceID;
        }
        var fileInfoList = new Array();
        //构建上传数据，方便后台处理（反序列化）
        $.each(self.existFileInfoArray, function (index, value) {
            var temp = {
                ATTACH_TYPE: value.FileType,
                ATTACH_TITLE: value.Name,
                ATTACH_PATH: value.Url
            }
            if (value.ID) {
                temp.ID = value.ID;
            }
            fileInfoList.push(temp);
        })
        $.ajax({
            type: "POST",
            dataType: "json",
            url: self.param.potsUrl,
            async:false,
            data: {
                "way": "FileInfoSavle",
                "SOURCE_ID": self.param.sourceID,
                "SOURCE_TYPE": self.param.sourceType,
                "SOURCE_FIELD": self.param.sourceField,
                "FileInfoList": JSON.stringify(fileInfoList)
            },
            success: function (resultData) {
                if (resultData && resultData.Success == true && resultData.Data && resultDataData.Data.length > 0) {
                    $.each(resultData, function (index, value) {
                        self.existFileInfoArray.push(value);
                    });
                    self.showFileInfo();
                }
                else {
                    //Message
                }
                if (typeof backCall == "function") {
                    backCall(resultData);
                }
            }
        });
    };
    //临时文件信息保存
    this.fileInfoTempSave = function () {
        var fileInfoList = new Array();
        //构建上传数据，方便后台处理（反序列化）
        $.each(self.existFileInfoArray, function (index, value) {
            var temp = {
                ATTACH_TYPE: value.FileType,
                ATTACH_TITLE: value.Name,
                ATTACH_PATH: value.Url,
                SOURCE_ID: self.param.sourceID,
                SOURCE_TYPE: self.param.sourceType,
                SOURCE_FIELD: self.param.sourceField,
            }
            if (value.ID) {
                temp.ID = value.ID;
            }
            fileInfoList.push(temp);
        })
        top.$("#temp" + self.param.fileInfo_input_hidden).val(JSON.stringify(fileInfoList));
        
    }
    //初始参数
    //init{htmlElementID:""，sourceID:"",sourceType"",sourceField:"",btnCss:"",fileCss:"",deleteBtnCss:""}
    this.init = function (initParam) {
        self.param.htmlID = initParam.htmlElementID;
        if (initParam.sourceID) {
            self.param.sourceID = initParam.sourceID;
        }
        self.param.sourceType = initParam.sourceType;
        self.param.sourceField = initParam.sourceField;
        self.param.btnCss = initParam.btnCss ? initParam.btnCss : "";
        self.param.fileCss = initParam.fileCss ? initParam.fileCss : "";
        self.param.deleteBtnCss = initParam.deleteBtnCss ? initParam.deleteBtnCss : "";
        self.createHtml();
        self.originalFile();
        top.$("#" + self.param.fileUpload_input).change(function () {
            self.uploadFile();
        });
        top.$("#" + self.param.fileUpload_button).click(function () {
            top.$("#" + self.param.fileUpload_input).click();
        });

        var source = {
            SOURCE_ID: self.param.sourceID,
            SOURCE_TYPE: self.param.sourceType,
            SOURCE_FIELD: self.param.sourceField,
        };
        top.$("#source" + self.param.fileInfo_input_hidden).val(JSON.stringify(source));
        
    }
}

/*
文件上传控件简单示例
    html：
    <div id="test"></div>
    <span onclick="testSave()">提交文件信息2</span>

    js:
    var test = new fileUploadControler();
    var initParam = {
        htmlElementID: "test",
        sourceID: "",
        sourceType: "sourceType",
        sourceField: "sourceField002",
        btnCss: "color:#ff0000;cursor:pointer;"
    }
    test.init(initParam);

    因为有些时候，在前端并不能拿到sourceID的（比如：新增），
    所以保存文件信息的应该在后台处理
    var testSave = function () {
        console.log(test.existFileInfoArray);
        test.fileInfoSave("sourceID002", backCallFun);
    }
    var backCallFun = function (result) {
        alert(result.Success);
    }

    后台处理  data:RequestData类型  source：关联的ID
    FileUploadHandle.FileMessageSave(data, source);
*/