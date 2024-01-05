using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Utils;

// https://developers.weixin.qq.com/doc/offiaccount/Asset_Management/Adding_Permanent_Assets.html

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public async Task<(bool success, string mediaId, string errorMessage)> AddMaterialImageAsync(string accessToken, string filePath)
        {
            //（image）: 2M，支持bmp/png/jpeg/jpg/gif格式
            //（voice）：2M，播放长度不超過60s，mp3/wma/wav/amr格式
            //（video）：10MB，支持MP4格式
            //（thumb）：64KB，支持JPG格式

            string fileName = PathUtils.GetFileName(filePath);
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}&type={1}", accessToken, "image");
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate,FileAccess.Read);
            byte[] fileByte = new byte[fs.Length];
            fs.Read(fileByte, 0, fileByte.Length);
            fs.Close();
            // 設置參數
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            string boundary = DateTime.Now.Ticks.ToString("X"); // 隨機分隔線
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
 
            StringBuilder sbHeader =
                new StringBuilder(
                    string.Format(
                        "Content-Disposition:form-data;name=\"media\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n",
                        fileName));
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

            Stream postStream = request.GetRequestStream();
            postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            postStream.Write(fileByte, 0, fileByte.Length);
            postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            postStream.Close();
 
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream instream = response.GetResponseStream();
            StreamReader sr = new StreamReader(instream, Encoding.UTF8);
            string content = sr.ReadToEnd();

            var success = true;
            var mediaId = string.Empty;
            var errorMessage = string.Empty;

            var json = TranslateUtils.JsonDeserialize(content);
            if (json.errcode != 0)
            {
                success = false;
                errorMessage = $"API 调用发生错误：{json.errmsg}";

                await _errorLogRepository.AddErrorLogAsync(new Exception(content), "WxManager.AddMaterialImageAsync");
            }
            else
            {
                mediaId = json.media_id;
            }

            return (success, mediaId, errorMessage);
        }
    }
}
