<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<link type="text/css" rel="stylesheet" href="/Content/css/Checkbox.css" />
<style type="text/css">
    li {
        margin: 5px;
        cursor: pointer;
    }

    .selectedCss {
        background-color: #1E1E1E;
        font-family: "微软雅黑";
        font-size: 120%;
        color: aquamarine;
    }

    .normalCss {
        background-color: darkgreen;
        font-family: "微软雅黑";
        font-size: 120%;
        color: aquamarine;
    }
</style>
<script type="text/javascript">
    var cType = ''; //企业类型
    var keyWords = '';//查询关键字
    var pTypeArray = new Array();//排口类型
    $(function () {
        var fs='<%=ViewData["fs"]%>';
        if(fs=='1') 
        {
            $("#divFS").css('display','block');
            pTypeArray.push('2');
        }
        else 
        {
            $("#divFS").css('display','none');
        }

        var fq='<%=ViewData["fq"]%>';
        if(fq=='1')
        {
            $("#divFQ").css('display','block');
            pTypeArray.push('1');
        }
        else
        {  
            $("#divFQ").css('display','none')
        };

        var vocs='<%=ViewData["vocs"]%>';
        if(vocs=='1') 
        {
            $("#divVOCS").css('display','block');
            pTypeArray.push('3');
        }
        else 
        {
            $("#divVOCS").css('display','none')
        };

        $("#fliter li").click(function() {
            $(this).siblings('li').removeClass('selectedCss').addClass('normalCss');  // 删除其他兄弟元素的样式
            $(this).removeClass('normalCss').addClass('selectedCss');                            // 添加当前元素的样式
            setCType(this.attributes["key"].value)
        });
        GetTree();
    });
    function GetTreeSelected() {
        var obj = $("#treeSelect").tree("getSelected");
        if (obj == null || obj.id == -1) {
            //alert("请先选中企业或排口！");
        }
        else
        {
            var parent = $("#treeSelect").tree("getParent",obj.target);
            return {
                ID: obj.id.substr(1),
                TYPE: obj.id.substr(0, 1),
                OBJTYPE:obj.attributes["TYPE"],
                PARENTID:parent.id.substr(1)
            }
        }
    }
    function GetMutiSelected()
    {
        var nodes  = $("#treeSelect").tree("getChecked");
        var CID=[];
        var PID=[];
        nodes.forEach(function(node ,index){
            TYPE= node.id.substr(0, 1);
            ID=node.id.substr(1);
            if(TYPE=="C")
                CID.push(ID);
            else if(TYPE=="P")
                PID.push(ID);
        });
        return {
            CID:CID,
            PID:PID,
        }
    }
    function setCType(value) {
        if(value=='all')value='';
        cType = value;
        GetTree();
    }
    function Change(checkbox) {
        var index = $.inArray(checkbox.value, pTypeArray);

        if (checkbox.checked) {
            if (index < 0) {
                pTypeArray.push(checkbox.value);
            }
        }
        else {
            if (index >= 0) {
                pTypeArray.splice(index, 1);
            }
        }
        GetTree();
    }
    function GetTree() {
        $('#treeSelect').tree({
            checkbox:<%=ViewData["multiSelect"]%>,
            url: "/Bas/UserControl/GetPKTree?cType=" + cType + "&pType=" + pTypeArray+"&selectId="+'<%=ViewData["selectId"]%>'+"&companyOnly="+'<%=ViewData["companyOnly"]%>',
            //            data:[{    
            //                "id":1,    
            //                "text":"Folder1",    
            //                "iconCls":"icon-save",    
            //                "children":[{    
            //                    "text":"File1",    
            //                    "selected":true   
            //                },{    
            //                    "text":"Books",    
            //                    "state":"open",    
            //                    "attributes":{    
            //                        "url":"/demo/book/abc",    
            //                        "price":100    
            //                    },    
            //                    "children":[{    
            //                        "text":"PhotoShop",    
            //                        "checked":true   
            //                    },{    
            //                        "id": 8,    
            //                        "text":"Sub Bookds",    
            //                        "state":"closed"   
            //                    }]    
            //                }]    
            //            },{    
            //                "text":"Languages",    
            //                "state":"closed",    
            //                "children":[{    
            //                    "text":"Java"   
            //                },{    
            //                    "text":"C#"   
            //                }]    
            //            }]  
            //,
            onSelect:function(node)
            {
                TYPE= node.id.substr(0, 1)
                if(TYPE=="C")
                {
                    if(typeof CompanySelected!= "undefined") CompanySelected(node.id.substr(1))
                }
                else if(TYPE=="P")
                {
                    if(typeof PKSelected!= "undefined")PKSelected(node.id.substr(1))
                }

                //不要改动
                if(typeof onNodeSelected!= "undefined")onNodeSelected(node)
            },
            onCheck:function(node)
            {
                TYPE= node.id.substr(0, 1)
                if(TYPE=="C") 
                {
                    if(typeof CompanySelected!= "undefined") CompanySelected();
                }
                else if(TYPE=="P")
                {
                    if(typeof PKSelected!= "undefined")PKSelected();
                }

                //不要改动
                if(typeof onNodeChecked!= "undefined")onNodeChecked(node)
            },
            onLoadSuccess:function(node,data){
                if(typeof AfterLoadSuccess!= "undefined")AfterLoadSuccess();
            },
            onLoadError:function(e,e2)
            {
            
            }
        });
    }
    function GetTreeByKeyword() {
        $("#fliter li").each(function(index) {
            $(this).removeClass('selectedCss').addClass('normalCss');  // 删除选中样式
        });
        $('#treeSelect').tree({
            checkbox:<%=ViewData["multiSelect"]%>,
            url: "/Bas/UserControl/GetPKTreeByKeyword?keyWord=" + $("#keyWords").val()+"&companyOnly="+'<%=ViewData["companyOnly"]%>',
            onSelect:function(node)
            {
                TYPE= node.id.substr(0, 1)
                if(TYPE=="C")
                {
                    if(typeof CompanySelected!= "undefined") CompanySelected(node.id.substr(1));
                }
                else if(TYPE=="P")
                {
                    if(typeof PKSelected!= "undefined")PKSelected(node.id.substr(1));
                }

                //不要改动
                if(typeof onNodeSelected!= "undefined")onNodeSelected(node)
            },
            onCheck:function(node)
            {
                TYPE= node.id.substr(0, 1)
                if(TYPE=="C") 
                {
                    if(typeof CompanySelected!= "undefined") CompanySelected();
                }
                else if(TYPE=="P")
                {
                    if(typeof PKSelected!= "undefined")PKSelected();
                }

                //不要改动
                if(typeof onNodeChecked!= "undefined")onNodeChecked(node)
            }
        });
    }
</script>
<div style="width: 250px; margin: 5px;">
    <div>
        <input id="keyWords" type="text" />
        <a plain="true" class="easyui-linkbutton" icon="icon-search" title="查询" onclick="GetTreeByKeyword()">查询</a>
    </div>
    <div style="padding: 5px; text-align: center; display: inline-flex">
        <div id="divFS" class="holder" style="width: 60px">
            <div class="center">
                <input type="checkbox" id="checkbox_fs" value="2" checked onchange="Change(this)" />废水：<label for="checkbox_fs">废水</label>
            </div>
        </div>
        <div id="divFQ" class="holder" style="width: 60px">
            <div class="center">
                <input type="checkbox" id="checkbox_fq" value="1" checked onchange="Change(this)" />废气：<label for="checkbox_fq">废气</label>
            </div>
        </div>
        <div id="divVOCS" class="holder" style="width: 60px">
            <div class="center">
                <input type="checkbox" id="checkbox_vocs" value="3" checked onchange="Change(this)" />VOCs：<label for="checkbox_vocs">VOCs</label>
            </div>
        </div>
        <%--<input type="checkbox" id="fs" value="2" checked onchange="Change(this)" />废水
        <input type="checkbox" id="fq" value="1" checked onchange="Change(this)" />废气
        <input type="checkbox" id="vocs" value="3" checked onchange="Change(this)" />VOCs--%>
    </div>
    <div>
        <%--        <div style="width: 20px; padding: 5px; float: left">
            <a class="easyui-linkbutton">全部</a>
            <a class="easyui-linkbutton">国控</a>
            <a class="easyui-linkbutton">省控</a>
            <a class="easyui-linkbutton">市控</a>
            <a class="easyui-linkbutton">区控</a>
            <a class="easyui-linkbutton">区域</a>
        </div>--%>
        <ul id="fliter" style="width: 30px; float: left; text-align: center">
            <li class="selectedCss" key="all">全部</li>
            <li class="normalCss" key="1">国控</li>
            <li class="normalCss" key="2">省控</li>
            <li class="normalCss" key="3">市控</li>
            <li class="normalCss" key="4">区控</li>
            <li class="normalCss" key="area">区域</li>
            <li class="normalCss" key="bas">行业</li>
        </ul>
        <ul id="treeSelect" style="float: right; width: 220px;"></ul>
    </div>
</div>
