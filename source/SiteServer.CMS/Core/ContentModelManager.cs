using System.Collections.Generic;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;

namespace SiteServer.CMS.Core
{
    public class ContentModelManager
    {
        private ContentModelManager()
        {
        }

        public static ContentModelInfo GetContentModelInfo(PublishmentSystemInfo publishmentSystemInfo, string contentModelId)
        {
            var list = GetContentModelInfoList(publishmentSystemInfo);
            foreach (var modelInfo in list)
            {
                if (modelInfo.ModelId == contentModelId)
                {
                    return modelInfo;
                }
            }
            return EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.AuxiliaryTableForContent, EContentModelType.Content);
        }

        public static List<ContentModelInfo> GetContentModelInfoList(PublishmentSystemInfo publishmentSystemInfo)
        {
            var list = new List<ContentModelInfo>
            {
                EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.AuxiliaryTableForContent, EContentModelType.Content),
                EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.AuxiliaryTableForVote, EContentModelType.Vote),
                EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.AuxiliaryTableForJob, EContentModelType.Job)
            };

            var contentModels = PluginManager.GetAllContentModels(publishmentSystemInfo);
            if (contentModels != null)
            {
                list.AddRange(contentModels);
            }

            return list;
        }
    }
}
