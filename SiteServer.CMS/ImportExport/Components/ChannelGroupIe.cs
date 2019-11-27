using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
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

	    public static async Task<bool> ImportAsync(AtomEntry entry, int siteId)
	    {
            var isNodeGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsNodeGroup"));
	        if (!isNodeGroup) return false;

	        var groupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(ChannelGroup.GroupName), "NodeGroupName" });
	        if (string.IsNullOrEmpty(groupName)) return true;
	        if (await DataProvider.ChannelGroupDao.IsExistsAsync(siteId, groupName)) return true;

	        var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ChannelGroup.Taxis)));
	        var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ChannelGroup.Description));
	        await DataProvider.ChannelGroupDao.InsertAsync(new ChannelGroup
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
