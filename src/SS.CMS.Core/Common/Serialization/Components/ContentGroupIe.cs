using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Utils;
using SS.CMS.Utils.Atom.Atom.Core;

namespace SS.CMS.Core.Serialization.Components
{
    public static class ContentGroupIe
    {
        public static AtomEntry Export(ContentGroupInfo groupInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "IsContentGroup", true.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(ContentGroupInfo.GroupName), "ContentGroupName" }, groupInfo.GroupName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentGroupInfo.Taxis), groupInfo.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentGroupInfo.Description), groupInfo.Description);

            return entry;
        }

        public static async Task<bool> ImportAsync(AtomEntry entry, int siteId, IContentGroupRepository contentGroupRepository)
        {
            var isNodeGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsContentGroup"));
            if (!isNodeGroup) return false;

            var groupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(ContentGroupInfo.GroupName), "ContentGroupName" });
            if (string.IsNullOrEmpty(groupName)) return true;
            if (await contentGroupRepository.IsExistsAsync(siteId, groupName)) return true;

            var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentGroupInfo.Taxis)));
            var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentGroupInfo.Description));

            await contentGroupRepository.InsertAsync(new ContentGroupInfo
            {
                GroupName = groupName,
                SiteId = siteId,
                Taxis = taxis,
                Description = description
            });

            return true;
        }
    }
}
