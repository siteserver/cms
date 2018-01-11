using BaiRong.Core;
using SiteServer.CMS.Model;
using System;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using System.Collections.Generic;
using BaiRong.Core.Model.Attributes;

namespace SiteServer.CMS.Core.Office
{
	public class TxtObject
	{
        public static List<ContentInfo> GetContentListByTxtFile(string directoryPath, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
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
                                PublishmentSystemId = publishmentSystemInfo.PublishmentSystemId,
                                NodeId = nodeInfo.NodeId,
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
