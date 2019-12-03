using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.ImportExport.Components
{
    public static class ContentGroupIe
    {
        public static AtomEntry Export(ContentGroup @group)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "IsContentGroup", true.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(ContentGroup.GroupName), "ContentGroupName" }, @group.GroupName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentGroup.Taxis), @group.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentGroup.Description), @group.Description);

            return entry;
        }

        public static async Task<bool> ImportAsync(AtomEntry entry, int siteId)
        {
            var isNodeGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsContentGroup"));
            if (!isNodeGroup) return false;

            var groupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(ContentGroup.GroupName), "ContentGroupName" });
            if (string.IsNullOrEmpty(groupName)) return true;
            if (await DataProvider.ContentGroupRepository.IsExistsAsync(siteId, groupName)) return true;

            var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentGroup.Taxis)));
            var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentGroup.Description));
            await DataProvider.ContentGroupRepository.InsertAsync(new ContentGroup
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
