using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using w.Model;
using w.ORM;

namespace UI.Web.Content.code.Handler
{
    public class FileUpload : IHttpHandler
    {
        HttpContext context = null;
        public void ProcessRequest(HttpContext context)
        {
            this.context = context;
            string way = string.IsNullOrEmpty(context.Request.Form["way"]) ? context.Request.QueryString["way"] : context.Request.Form["way"];
            switch (way)
            {
                //下载
                case "DownLoadFile":
                    DownLoadFile();
                    break;
                //原有文件
                case "OriginalFile":
                    string fileStr = OriginalFile();
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(fileStr);
                    break;
                case "FileInfoSavle":

                    try
                    {
                        FileInfoSavle(context);
                        this.OperateResult(true);
                    }
                    catch (Exception ex)
                    {
                        this.OperateResult(false, "文件信息保存失败");
                    }
                    break;
                default://默认上传文件
                    var files = context.Request.Files;
                    if (files.Count < 1)
                    {
                        //请选择文件
                    }
                    List<dynamic> fileInfoList = new List<dynamic>();
                    for (int i = 0; i < files.Count; i++)
                    {
                        fileInfoList.Add(FileSave(files[i]));
                    }
                    string data = SerializerHelper.Serialize(fileInfoList);
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(data);
                    break;
            }

        }
        private string OriginalFile()
        {
            List<dynamic> fileList = new List<dynamic>(5);
            string sorceID = context.Request.Form["SOURCE_ID"];
            string sorceType = context.Request.Form["SOURCE_TYPE"];
            string sorceField = context.Request.Form["SOURCE_FIELD"];
            SqlModel.SelectAll()
                .From(DB.T_SYS_ATTACH)
                .Where(T_SYS_ATTACH.SOURCE_ID == sorceID & T_SYS_ATTACH.SOURCE_TYPE == sorceType & T_SYS_ATTACH.SOURCE_FIELD == sorceField).ExecToDynamicList()
                .ForEach(attach =>
                {
                    var file = new
                    {
                        Url = attach["ATTACH_PATH"],
                        Name = attach["ATTACH_TITLE"],
                        FileType = attach["ATTACH_TYPE"],
                        ID = attach["ID"],
                        fileInfoID = Guid.NewGuid().ToString().Replace("-", "")//用于前端页面识别每一条文件信息
                    };
                    fileList.Add(file);
                });
            return SerializerHelper.Serialize(fileList);
        }
        /// <summary>
        /// 文件保存
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private dynamic FileSave(HttpPostedFile file)
        {
            string fileName = file.FileName;/*获取文件名： C:\Documents and Settings\Administrator\桌面\123.jpg*/
            fileName = fileName.Split(new char[] { '\\' }).LastOrDefault();
            string suffix = fileName.Substring(fileName.LastIndexOf(".") + 1).ToLower();/*获取后缀名并转为小写： jpg*/
            string newFileName = Guid.NewGuid().ToString(); //fileName.Substring(0, fileName.IndexOf(".")- 1)+"_"+
            int bytes = file.ContentLength;//获取文件的字节大小  
            string webUploadPath = HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString()) + "Upload";// + ConfigurationManager.AppSettings["UpLoadFilePath"]; //获取webConfig图片保存路径
            string newFolder = DateTime.Now.ToString("yyyyMMdd");//新建新日期文件夹
            if (!Directory.Exists(webUploadPath + @"\" + newFolder))
                Directory.CreateDirectory(webUploadPath + @"\" + newFolder);
            file.SaveAs(webUploadPath + @"\" + newFolder + @"\" + newFileName + "." + suffix);//保存文件  
            var fileInfo = new
            {
                Url = AbsUrlconvertor(webUploadPath + @"\" + newFolder + @"\" + newFileName + "." + suffix),
                Name = fileName,
                FileType = "." + suffix,
                fileInfoID = Guid.NewGuid().ToString().Replace("-", "")
            };
            return fileInfo;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private void UploadFilePlanDoc()
        {
            JsonResult<string> result = new JsonResult<string>();
            string filePath = string.Empty;
            string fileContent = context.Request["Content"];
            string fileName = Guid.NewGuid().ToString() + ".doc";
            StringBuilder sbDoc = new StringBuilder();
            try
            {
                if (string.IsNullOrEmpty(fileContent))
                {
                    result.success = false;
                    result.msg = "没有预案内容";

                }
                else
                {
                    fileContent = HttpUtility.UrlDecode(fileContent);
                    sbDoc.Append("<html xmlns:v=\"urn: schemas - microsoft - com:vml\" xmlns:o=\"urn: schemas - microsoft - com:office: office\" xmlns:w=\"urn: schemas - microsoft - com:office: word\"   ");
                    sbDoc.Append("xmlns:m=\"http://schemas.microsoft.com/office/2004/12/omml\"  xmlns=\"http://www.w3.org/TR/REC-html40\">    ");
                    sbDoc.Append(fileContent);
                    sbDoc.Append("</html> ");
                    //如果文件不存在，则创建；存在则覆盖
                    string webUploadPath = HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString()) + "Upload"; //获取webConfig保存路径
                    string newFolder = DateTime.Now.ToString("yyyyMMdd");//新建新日期文件夹
                    if (!Directory.Exists(webUploadPath + @"\" + newFolder))
                        Directory.CreateDirectory(webUploadPath + @"\" + newFolder);
                    System.IO.File.WriteAllText(webUploadPath + @"\" + newFolder + @"\" + fileName, sbDoc.ToString(), Encoding.UTF8);

                    result.success = true;
                    result.sdata = AbsUrlconvertor(webUploadPath + @"\" + newFolder + @"\" + fileName);
                    result.sdata2 = fileName;


                    //filePath
                }
                context.Response.Write(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                result.success = false;
                result.msg = "文件写入失败";
                context.Response.Write(JsonConvert.SerializeObject(result));
            }
            finally
            {
                context.Response.End();
            }
            //return 
        }

        private void DownLoadFile()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/") + context.Request["fileUrl"].Replace("/", @"\");
            string fileName = context.Request["filename"];
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                //string downFile = System.IO.Path.GetFileName(fileName);//这里也可以随便取名
                if (fileInfo.Exists)
                {
                    context.Response.Buffer = true;
                    context.Response.Clear();
                    context.Response.ContentType = "application/ms-download";
                    context.Response.AddHeader("Content-Type", "application/octet-stream");
                    context.Response.Charset = "utf-8";
                    context.Response.AddHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                    context.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    context.Response.BinaryWrite(System.IO.File.ReadAllBytes(filePath));
                }
                else
                {
                    context.Response.ContentType = "text/html";
                    context.Response.Write("<script>alert('下载失败,服务器文件不存在或已被删除');window.history.back(); </script>");
                }

            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/html";
                context.Response.Write("<script>alert('下载失败,服务器文件不存在或已被删除');window.history.back();  </script>");
            }
            finally
            {
                context.Response.Flush();
                context.Response.End();
            }

        }


        //本地路径转换成URL相对路径
        private string AbsUrlconvertor(string absUrl)
        {
            string tmpRootDir = HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string webUrl = absUrl.Replace(tmpRootDir, ""); //转换成相对路径
            webUrl = webUrl.Replace(@"\", @"/");
            return webUrl;
        }
        /// <summary>
        /// 文件信息保存
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private void FileInfoSavle(HttpContext context)
        {
            string fileInfoListStr = context.Request.Form["FileInfoList"];
            string sorceID = context.Request.Form["SOURCE_ID"];
            string sorceType = context.Request.Form["SOURCE_TYPE"];
            string sorceField = context.Request.Form["SOURCE_FIELD"];
            List<T_SYS_ATTACHModel> modelList = SerializerHelper.Deserialize<List<T_SYS_ATTACHModel>>(fileInfoListStr);
            var sql = SqlModel.SelectAll().From(DB.T_SYS_ATTACH).Where(T_SYS_ATTACH.SOURCE_ID == sorceID & T_SYS_ATTACH.SOURCE_TYPE == sorceType & T_SYS_ATTACH.SOURCE_FIELD == sorceField);
            var existAttachList = sql.ExecToDynamicList();
            List<string> keepFileIDList = new List<string>();//保留的文件信息ID
            modelList.ForEach(model => {
                bool isKeep = existAttachList.Exists(c => StringHelper.DynamicToString(c["ID"]) == StringHelper.DynamicToString(model.ID));
                if (isKeep)
                {
                    keepFileIDList.Add(model.ID.ToString());
                }
                else
                {
                    model.SOURCE_ID = sorceID;
                    model.SOURCE_TYPE = sorceType;
                    model.SOURCE_FIELD = sorceField;
                    model.Insert();
                }
            });
            //删除过时的文件信息
            T_SYS_ATTACHModel deleteModel = new T_SYS_ATTACHModel();
            existAttachList.ForEach(attach => {
                if (!keepFileIDList.Contains(StringHelper.DynamicToString(attach["ID"])))
                {
                    deleteModel.Delete(T_SYS_ATTACH.ID == attach["ID"]);
                }
            });
        }
        /// <summary>
        /// 返回操作结果
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        private void OperateResult(bool result, string msg = "", object data = null)
        {
            PostResult postResult = new PostResult();
            postResult.Success = result;
            postResult.Message = msg;
            postResult.Data = data;
            context.Response.Write(SerializerHelper.Serialize(postResult));
        }
    }
}
namespace UI.Web
{
    public class FileUploadHandle
    {
        public static void FileMessageSave(RequestData data, string sorceID)
        {
            string attachStr = string.Format("[{0}]", data.Get("attach"));
            var list = SerializerHelper.Deserialize<List<List<T_SYS_ATTACHModel>>>(attachStr);

            List<T_SYS_ATTACHModel> attachList = new List<T_SYS_ATTACHModel>();
            foreach (var childList in list)
            {
                attachList.AddRange(childList);
            }

            string sourceMsg = string.Format("[{0}]", data.Get("source"));//只有SOURCE_TYPE、SOURCE_FIELD
            var sourceList = SerializerHelper.Deserialize<List<T_SYS_ATTACHModel>>(sourceMsg);
            foreach (T_SYS_ATTACHModel source in sourceList)
            {
                var oneSource = attachList.Where(item => item.SOURCE_TYPE == source.SOURCE_TYPE && item.SOURCE_FIELD == source.SOURCE_FIELD);
                string sorceType = source.SOURCE_TYPE;
                string sorceField = source.SOURCE_FIELD;
                List<T_SYS_ATTACHModel> modelList = oneSource != null ? oneSource.ToList() : new List<T_SYS_ATTACHModel>();
                var sql = SqlModel.SelectAll().From(DB.T_SYS_ATTACH).Where(T_SYS_ATTACH.SOURCE_ID == sorceID & T_SYS_ATTACH.SOURCE_TYPE == sorceType & T_SYS_ATTACH.SOURCE_FIELD == sorceField);
                var existAttachList = sql.ExecToDynamicList();
                List<string> keepFileIDList = new List<string>();//保留的文件信息ID
                modelList.ForEach(model => {
                    bool isKeep = existAttachList.Exists(c => StringHelper.DynamicToString(c["ID"]) == StringHelper.DynamicToString(model.ID));
                    if (isKeep)
                    {
                        keepFileIDList.Add(model.ID.ToString());
                    }
                    else
                    {
                        model.SOURCE_ID = sorceID;
                        model.Insert();
                    }
                });
                //删除过时的文件信息
                T_SYS_ATTACHModel deleteModel = new T_SYS_ATTACHModel();
                existAttachList.ForEach(attach => {
                    if (!keepFileIDList.Contains(StringHelper.DynamicToString(attach["ID"])))
                    {
                        deleteModel.Delete(T_SYS_ATTACH.ID == attach["ID"]);
                    }
                });
            }
        }
    }
}