using System.Collections;
using BaiRong.Core;
using SiteServer.CMS.Model;
using System;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Core.Office
{
	public class TxtObject
	{
        public static ArrayList GetContentsByTxtFile(string directoryPath, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var contentInfoArrayList = new ArrayList();

            var filePaths = DirectoryUtils.GetFilePaths(directoryPath);
            foreach (var filePath in filePaths)
            {
                if (EFileSystemTypeUtils.Equals(EFileSystemType.Txt, PathUtils.GetExtension(filePath)))
                {
                    try
                    {
                        var content = FileUtils.ReadText(filePath, ECharset.gb2312);
                        if (!string.IsNullOrEmpty(content))
                        {
                            content = content.Trim();
                            var title = StringUtils.GetFirstOfStringCollection(content, '\r');
                            if (!string.IsNullOrEmpty(title))
                            {
                                var contentInfo = new BackgroundContentInfo();
                                contentInfo.Title = title.Trim();
                                contentInfo.PublishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
                                contentInfo.NodeId = nodeInfo.NodeId;
                                contentInfo.LastEditDate = DateTime.Now;
                                contentInfo.Content = StringUtils.ReplaceNewlineToBr(content.Replace(title, string.Empty).Trim());

                                contentInfoArrayList.Add(contentInfo);
                            }
                        }
                    }
                    catch { }
                }
            }

            return contentInfoArrayList;
        }
	}
}
