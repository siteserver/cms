using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Utils;

// https://developers.weixin.qq.com/doc/offiaccount/Asset_Management/Adding_Permanent_Assets.html

namespace SSCMS.Core.Services
{
    public partial class WxManager
    {
        public async Task<(bool success, string mediaId, string errorMessage)> AddMaterialAsync(string accessToken, WxMaterialType materialType, string filePath)
        {
            //（image）: 2M，支持bmp/png/jpeg/jpg/gif格式
            //（voice）：2M，播放长度不超過60s，mp3/wma/wav/amr格式
            //（video）：10MB，支持MP4格式
            //（thumb）：64KB，支持JPG格式

            var fileName = PathUtils.GetFileName(filePath);
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}&type={1}", accessToken, materialType.GetValue());
            var fs = new FileStream(filePath, FileMode.OpenOrCreate,FileAccess.Read);
            var fileByte = new byte[fs.Length];
            fs.Read(fileByte, 0, fileByte.Length);
            fs.Close();
            // 設置參數
            
            #pragma warning disable
            var request = (HttpWebRequest)WebRequest.Create(url);
            #pragma warning restore
            
            var cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            var boundary = DateTime.Now.Ticks.ToString("X"); // 隨機分隔線
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            var itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            var endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
 
            var sbHeader = new StringBuilder(
                    string.Format(
                        "Content-Disposition:form-data;name=\"media\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n",
                        fileName));
            var postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

            Stream postStream = request.GetRequestStream();
            postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            postStream.Write(fileByte, 0, fileByte.Length);
            postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            postStream.Close();
 
            var response = request.GetResponse() as HttpWebResponse;
            var instream = response.GetResponseStream();
            var sr = new StreamReader(instream, Encoding.UTF8);
            var content = sr.ReadToEnd();

            var success = true;
            var mediaId = string.Empty;
            var errorMessage = string.Empty;

            var json = TranslateUtils.JsonDeserialize(content);
            if (json.errcode != null && json.errcode > 0)
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
