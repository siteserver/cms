using System;
using System.Collections.Generic;
using SS.CMS.Abstractions;

namespace SS.CMS.Core.Office
{
	public static class TxtObject
	{
        public static List<Content> GetContentListByTxtFile(string directoryPath, Site site, Channel node)
        {
            var contentInfoList = new List<Content>();

            var filePaths = DirectoryUtils.GetFilePaths(directoryPath);
            foreach (var filePath in filePaths)
            {
                if (!FileUtils.IsType(FileType.Txt, PathUtils.GetExtension(filePath))) continue;

                try
                {
                    var content = FileUtils.ReadText(filePath, Constants.Gb2312);
                    if (!string.IsNullOrEmpty(content))
                    {
                        content = content.Trim();
                        var title = StringUtils.GetFirstOfStringCollection(content, '\r');
                        if (!string.IsNullOrEmpty(title))
                        {
                            var dict = new Dictionary<string, object>
                            {
                                {ContentAttribute.Title, title.Trim()},
                                {ContentAttribute.SiteId, site.Id},
                                {ContentAttribute.ChannelId, node.Id},
                                {ContentAttribute.LastEditDate, DateTime.Now}
                            };
                            var contentInfo = new Content(dict);
                            contentInfo.Set(ContentAttribute.Content, StringUtils.ReplaceNewlineToBr(content.Replace(title, string.Empty).Trim()));

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
