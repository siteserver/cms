using System.Collections.Generic;
using Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
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

        public static bool Import(AtomEntry entry, int siteId)
        {
            var isNodeGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsContentGroup"));
            if (!isNodeGroup) return false;

            var groupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(ContentGroupInfo.GroupName), "ContentGroupName" });
            if (string.IsNullOrEmpty(groupName)) return true;
            if (ContentGroupManager.IsExists(siteId, groupName)) return true;

            var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentGroupInfo.Taxis)));
            var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentGroupInfo.Description));
            DataProvider.ContentGroupDao.Insert(new ContentGroupInfo(groupName, siteId, taxis, description));

            return true;
        }
    }
}
