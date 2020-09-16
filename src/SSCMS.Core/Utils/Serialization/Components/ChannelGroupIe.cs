using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Components
{
	public static class ChannelGroupIe
	{
		public static AtomEntry Export(ChannelGroup group)
		{
			var entry = AtomUtility.GetEmptyEntry();

			AtomUtility.AddDcElement(entry.AdditionalElements, "IsNodeGroup", true.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(ChannelGroup.GroupName), "NodeGroupName" }, @group.GroupName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ChannelGroup.Taxis), group.Taxis.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ChannelGroup.Description), group.Description);

			return entry;
		}

	    public static async Task<bool> ImportAsync(IChannelGroupRepository channelGroupRepository, AtomEntry entry, int siteId)
	    {
            var isNodeGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsNodeGroup"));
	        if (!isNodeGroup) return false;

	        var groupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(ChannelGroup.GroupName), "NodeGroupName" });
	        if (string.IsNullOrEmpty(groupName)) return true;
	        if (await channelGroupRepository.IsExistsAsync(siteId, groupName)) return true;

	        var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ChannelGroup.Taxis)));
	        var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ChannelGroup.Description));
	        await channelGroupRepository.InsertAsync(new ChannelGroup
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
