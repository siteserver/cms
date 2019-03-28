using System.Collections.Generic;
using Atom.Core;
using SiteServer.CMS.Caches;
using SiteServer.Utils;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.ImportExport.Components
{
	public static class ChannelGroupIe
	{
		public static AtomEntry Export(ChannelGroupInfo groupInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

			AtomUtility.AddDcElement(entry.AdditionalElements, "IsNodeGroup", true.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(ChannelGroupInfo.GroupName), "NodeGroupName" }, groupInfo.GroupName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ChannelGroupInfo.Taxis), groupInfo.Taxis.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ChannelGroupInfo.Description), groupInfo.Description);

			return entry;
		}

	    public static bool Import(AtomEntry entry, int siteId)
	    {
            var isNodeGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsNodeGroup"));
	        if (!isNodeGroup) return false;

	        var groupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(ChannelGroupInfo.GroupName), "NodeGroupName" });
	        if (string.IsNullOrEmpty(groupName)) return true;
	        if (ChannelGroupManager.IsExists(siteId, groupName)) return true;

	        var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ChannelGroupInfo.Taxis)));
	        var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ChannelGroupInfo.Description));
	        DataProvider.ChannelGroup.Insert(new ChannelGroupInfo
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
