using System.Collections;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Repositories;

namespace SS.CMS.Services
{
    public partial interface IFileManager
    {
        void ChangeSiteDir(string parentPsPath, string oldPsDir, string newPsDir);

        Task DeleteSiteFilesAsync(Site siteInfo);

        Task ImportSiteFilesAsync(Site siteInfo, string siteTemplatePath, bool isOverride);

        Task ChangeParentSiteAsync(int oldParentSiteId, int newParentSiteId, int siteId, string siteDir);

        Task ChangeToRootAsync(Site siteInfo, bool isMoveFiles);

        Task ChangeToSubSiteAsync(Site siteInfo, string psDir, ArrayList fileSystemNameArrayList);
    }
}
