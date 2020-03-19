using System.Collections.Generic;
using SSCMS;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Office
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
                    var content = FileUtils.ReadText(filePath);
                    if (!string.IsNullOrEmpty(content))
                    {
                        content = content.Trim();
                        var title = StringUtils.GetFirstOfStringCollection(content, '\r');
                        if (!string.IsNullOrEmpty(title))
                        {
                            var contentInfo = new Content
                            {
                                Title = title.Trim(),
                                SiteId = site.Id,
                                ChannelId = node.Id,
                                Body = StringUtils.ReplaceNewlineToBr(content.Replace(title, string.Empty).Trim())
                            };

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
