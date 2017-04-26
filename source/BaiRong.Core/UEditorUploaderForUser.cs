using System;
using System.Web;
using System.Collections;
using System.IO;

namespace BaiRong.Core
{
    public class UEditorUploaderForUser
    {
        string state = "SUCCESS";

        string URL = null;
        string currentType = null;
        string originalName = null;
        HttpPostedFile uploadFile = null;

        /**
      * 上传文件的主处理方法
      * @param HttpContext
      * @param string
      * @param  string[]
      *@param int
      * @return Hashtable
      */
        public Hashtable upFile(HttpContext cxt)
        {
            try
            {
                uploadFile = cxt.Request.Files[0];
                originalName = uploadFile.FileName;
                currentType = getFileExt();

                var localDirectoryPath = PathUtils.GetUserUploadDirectoryPath(string.Empty);
                var localFileName = PathUtils.GetUserUploadFileName(originalName);
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);
                var fileExtName = PathUtils.GetExtension(originalName);

                //string pathbase = pathbase + DateTime.Now.ToString("yyyy-MM-dd") + "/";
                //uploadpath = cxt.Server.MapPath(pathbase);//获取文件上传路径

                //格式验证
                //if (checkType(filetype))
                //{
                //    state = "不允许的文件类型";
                //}
                ////大小验证
                //if (checkSize(size))
                //{
                //    state = "文件大小超出网站限制";
                //}
                //保存图片
                if (state == "SUCCESS")
                {
                    uploadFile.SaveAs(localFilePath);
                    URL = PageUtils.GetRootUrlByPhysicalPath(localFilePath);
                    //URL = pathbase + filename;
                }
            }
            catch (Exception e)
            {
                state = e.Message;
                URL = "";
            }
            return getUploadInfo();
        }

        public Hashtable upScrawl(HttpContext cxt, string base64Data)
        {
            FileStream fs = null;
            try
            {
                var localDirectoryPath = PathUtils.GetUserUploadDirectoryPath(string.Empty);
                var fileName = Guid.NewGuid() + ".png";
                var localFilePath = PathUtils.Combine(localDirectoryPath, fileName);
                fs = File.Create(localFilePath);
                var bytes = Convert.FromBase64String(base64Data);
                fs.Write(bytes, 0, bytes.Length);

                URL = PageUtils.GetRootUrlByPhysicalPath(localFilePath);
            }
            catch (Exception e)
            {
                state = "未知错误";
                URL = "";
            }
            finally
            {
                fs.Close();
            }
            return getUploadInfo();
        }

        /**
    * 获取文件信息
    * @param context
    * @param string
    * @return string
    */
        public string getOtherInfo(HttpContext cxt, string field)
        {
            string info = null;
            if (cxt.Request.Form[field] != null && !string.IsNullOrEmpty(cxt.Request.Form[field]))
            {
                info = field == "fileName" ? cxt.Request.Form[field].Split(',')[1] : cxt.Request.Form[field];
            }
            return info;
        }

        /**
         * 获取上传信息
         * @return Hashtable
         */
        private Hashtable getUploadInfo()
        {
            var infoList = new Hashtable();

            infoList.Add("state", state);
            infoList.Add("url", URL);

            if (currentType != null)
                infoList.Add("currentType", currentType);
            if (originalName != null)
                infoList.Add("originalName", originalName);
            return infoList;
        }

        /**
         * 重命名文件
         * @return string
         */
        private string reName()
        {
            return Guid.NewGuid() + getFileExt();
        }

        /**
         * 文件类型检测
         * @return bool
         */
        private bool checkType(string[] filetype)
        {
            currentType = getFileExt();
            return Array.IndexOf(filetype, currentType) == -1;
        }

        /**
         * 文件大小检测
         * @param int
         * @return bool
         */
        private bool checkSize(int size)
        {
            return uploadFile.ContentLength >= (size * 1024 * 1024);
        }

        /**
         * 获取文件扩展名
         * @return string
         */
        private string getFileExt()
        {
            var temp = uploadFile.FileName.Split('.');
            return "." + temp[temp.Length - 1].ToLower();
        }

        ///**
        // * 按照日期自动创建存储文件夹
        // */
        //private void createFolder()
        //{
        //    if (!Directory.Exists(uploadpath))
        //    {
        //        Directory.CreateDirectory(uploadpath);
        //    }
        //}

        /**
         * 删除存储文件夹
         * @param string
         */
        public void deleteFolder(string path)
        {
            //if (Directory.Exists(path))
            //{
            //    Directory.Delete(path, true);
            //}
        }
    }
}
