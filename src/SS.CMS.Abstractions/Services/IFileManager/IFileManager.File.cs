using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IFileManager
    {
        //将编辑器中图片上传至本机
        string SaveImage(SiteInfo siteInfo, string content);

        void AddWaterMark(SiteInfo siteInfo, string imagePath);

        void MoveFile(SiteInfo sourceSiteInfo, SiteInfo destSiteInfo, string relatedUrl);

        void MoveFileByContentInfo(SiteInfo sourceSiteInfo, SiteInfo destSiteInfo, ContentInfo contentInfo);

        void MoveFileByVirtualUrl(SiteInfo sourceSiteInfo, SiteInfo destSiteInfo, string fileVirtualUrl);

        void MoveFiles(int sourceSiteId, int targetSiteId, List<string> relatedUrls);

        void AddWaterMark(int siteId, string filePath);
    }
}
