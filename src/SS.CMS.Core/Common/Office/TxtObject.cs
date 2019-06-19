using System;
using System.Collections.Generic;
using SS.CMS.Core.Models;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Common.Office
{
    public static class TxtObject
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
                            var dict = new Dictionary<string, object>
                            {
                                {ContentAttribute.Title, title.Trim()},
                                {ContentAttribute.SiteId, siteInfo.Id},
                                {ContentAttribute.ChannelId, nodeInfo.Id}
                            };
                            var contentInfo = new ContentInfo(dict);
                            contentInfo.Content = StringUtils.ReplaceNewlineToBr(content.Replace(title, string.Empty).Trim());

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
