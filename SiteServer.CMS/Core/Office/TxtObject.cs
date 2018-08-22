using SiteServer.Utils;
using SiteServer.CMS.Model;
using System;
using System.Collections.Generic;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core.Office
{
	public class TxtObject
	{
        public static List<ContentInfo> GetContentListByTxtFile(string directoryPath, SiteInfo siteInfo, ChannelInfo nodeInfo)
        {
            var contentInfoList = new List<ContentInfo>();

            var filePaths = DirectoryUtils.GetFilePaths(directoryPath);
            foreach (var filePath in filePaths)
            {
                if (!EFileSystemTypeUtils.Equals(EFileSystemType.Txt, PathUtils.GetExtension(filePath))) continue;

                try
                {
                    var content = FileUtils.ReadText(filePath, ECharset.gb2312);
                    if (!string.IsNullOrEmpty(content))
                    {
                        content = content.Trim();
                        var title = StringUtils.GetFirstOfStringCollection(content, '\r');
                        if (!string.IsNullOrEmpty(title))
                        {
                            var contentInfo = new ContentInfo
                            {
                                Title = title.Trim(),
                                SiteId = siteInfo.Id,
                                ChannelId = nodeInfo.Id,
                                LastEditDate = DateTime.Now
                            };
                            contentInfo.Set(BackgroundContentAttribute.Content, StringUtils.ReplaceNewlineToBr(content.Replace(title, string.Empty).Trim()));

                            contentInfoList.Add(contentInfo);
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }

            return contentInfoList;
        }
	}
}
