using System.Collections.Generic;
using System.IO;

namespace SiteServer.CMS.WeiXin.WeiXinMP.AdvancedAPIs.Media
{
    //接口详见：http://mp.weixin.qq.com/wiki/index.php?title=%E4%B8%8A%E4%BC%A0%E4%B8%8B%E8%BD%BD%E5%A4%9A%E5%AA%92%E4%BD%93%E6%96%87%E4%BB%B6
    
    /// <summary>
    /// 多媒体文件接口
    /// </summary>
    public static class Media
    {
        /// <summary>
        /// 上传媒体文件
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="type"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static UploadResultJson Upload(string accessToken, UploadMediaFileType type, string file)
        {
            var url =
                $"http://file.api.weixin.qq.com/cgi-bin/media/upload?access_token={accessToken}&type={type.ToString()}";
            var fileDictionary = new Dictionary<string, string>();
            fileDictionary["media"] = file;
            return HttpUtility.Post.PostFileGetJson<UploadResultJson>(url, null, fileDictionary, null);
        }

        /// <summary>
        /// 下载媒体文件
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="mediaId"></param>
        /// <param name="stream"></param>
        public static void Get(string accessToken, string mediaId, Stream stream)
        {
            var url = $"http://file.api.weixin.qq.com/cgi-bin/media/get?access_token={accessToken}&media_id={mediaId}";
            HttpUtility.Get.Download(url, stream);
        }
    }
}
