using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
    public class ContentModelManager
    {
        private ContentModelManager()
        {
        }

        public static ContentModelInfo GetContentModelInfo(PublishmentSystemInfo publishmentSystemInfo, string contentModelId)
        {
            ContentModelInfo retval = null;
            var list = GetContentModelInfoList(publishmentSystemInfo);
            foreach (var modelInfo in list)
            {
                if (modelInfo.ModelId == contentModelId)
                {
                    retval = modelInfo;
                    break;
                }
            }
            if (retval == null)
            {
                retval = EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.PublishmentSystemType, publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.AuxiliaryTableForContent, EContentModelType.Content);
            }
            return retval;
        }

        public static List<ContentModelInfo> GetContentModelInfoList(PublishmentSystemInfo publishmentSystemInfo)
        {
            var list = new List<ContentModelInfo>
            {
                EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.PublishmentSystemType,
                    publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.AuxiliaryTableForContent,
                    EContentModelType.Content),
                EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.PublishmentSystemType,
                    publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.AuxiliaryTableForContent,
                    EContentModelType.Photo)
            };

            if (publishmentSystemInfo.PublishmentSystemType == EPublishmentSystemType.WCM)
            {
                list.Add(EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.PublishmentSystemType, publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.AuxiliaryTableForGovPublic, EContentModelType.GovPublic));

                list.Add(EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.PublishmentSystemType, publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.AuxiliaryTableForGovInteract, EContentModelType.GovInteract));
            }

            list.Add(EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.PublishmentSystemType, publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.AuxiliaryTableForVote, EContentModelType.Vote));

            list.Add(EContentModelTypeUtils.GetContentModelInfo(publishmentSystemInfo.PublishmentSystemType, publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.AuxiliaryTableForJob, EContentModelType.Job));

            list.AddRange(ContentModelUtils.GetContentModelInfoList(publishmentSystemInfo.PublishmentSystemId));

            return list;
        }
    }
}
