using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.WeiXin.Data;

namespace SiteServer.CMS.WeiXin.IO
{
    public class FileUtilityWX
    {
        private FileUtilityWX()
        {
        }

        public static void DeleteWeiXinContent(PublishmentSystemInfo publishmentSystemInfo, int keywordID, int resourceID)
        {
            var filePath = PathUtilityWX.GetWeiXinFilePath(publishmentSystemInfo, keywordID, resourceID);
            FileUtils.DeleteFileIfExists(filePath);
        }

        public static void CreateWeiXinContent(PublishmentSystemInfo publishmentSystemInfo, int keywordID, int resourceID)
        {
            var keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(keywordID);

            if (keywordInfo != null)
            {
                var filePath = PathUtilityWX.GetWeiXinFilePath(publishmentSystemInfo, keywordID, resourceID);
                var templateFilePath = PathUtilityWX.GetWeiXinTemplateFilePath();
                var serviceUrl = PageUtilityWX.GetWeiXinTemplateDirectoryUrl(publishmentSystemInfo);

                var builder = new StringBuilder(FileUtils.ReadText(templateFilePath, ECharset.utf_8));

                var resourceInfo = DataProviderWX.KeywordResourceDAO.GetResourceInfo(resourceID);
                if (resourceInfo != null)
                {
                    builder.Replace("{serviceUrl}", serviceUrl);
                    builder.Replace("{title}", resourceInfo.Title);
                    if (resourceInfo.IsShowCoverPic && !string.IsNullOrEmpty(resourceInfo.ImageUrl))
                    {
                        builder.Replace("{image}",
                            $@"<img src=""{PageUtils.AddProtocolToUrl(
                                PageUtility.ParseNavigationUrl(publishmentSystemInfo, resourceInfo.ImageUrl))}"" />");
                    }
                    else
                    {
                        builder.Replace("{image}", string.Empty);
                    }
                    builder.Replace("{addDate}", DateUtils.GetDateString(keywordInfo.AddDate));
                    builder.Replace("{weixinName}", publishmentSystemInfo.PublishmentSystemName);
                    builder.Replace("{content}", resourceInfo.Content);
                }

                FileUtils.WriteText(filePath, ECharset.utf_8, builder.ToString());
            }
        }
    }
}
