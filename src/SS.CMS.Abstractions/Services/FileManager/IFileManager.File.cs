using System.Collections.Generic;
using System.Threading.Tasks;


namespace SS.CMS.Abstractions
{
    public partial interface IFileManager
    {
        //将编辑器中图片上传至本机
        string SaveImage(Site siteInfo, string content);

        void AddWaterMark(Site siteInfo, string imagePath);

        void MoveFile(Site sourceSiteInfo, Site destSiteInfo, string relatedUrl);

        void MoveFileByContentInfo(Site sourceSiteInfo, Site destSiteInfo, Content contentInfo);

        void MoveFileByVirtualUrl(Site sourceSiteInfo, Site destSiteInfo, string fileVirtualUrl);

        Task MoveFilesAsync(int sourceSiteId, int targetSiteId, List<string> relatedUrls);

        Task AddWaterMarkAsync(int siteId, string filePath);
    }
}
