using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Components
{
    public class ContentGroupIe
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly CacheUtils _caching;

        public ContentGroupIe(IDatabaseManager databaseManager, CacheUtils caching)
        {
            _databaseManager = databaseManager;
            _caching = caching;
        }

        public AtomEntry Export(ContentGroup group)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, "IsContentGroup", true.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(ContentGroup.GroupName), "ContentGroupName" }, group.GroupName);
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentGroup.Taxis), group.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(ContentGroup.Description), group.Description);

            return entry;
        }

        public async Task ImportAsync(AtomFeed feed, int siteId, string guid)
        {
            var groups = new List<ContentGroup>();

            foreach (AtomEntry entry in feed.Entries)
            {
                var isNodeGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsContentGroup"));
                if (!isNodeGroup) continue;

                var groupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(ContentGroup.GroupName), "ContentGroupName" });
                if (string.IsNullOrEmpty(groupName)) continue;

                if (await _databaseManager.ContentGroupRepository.IsExistsAsync(siteId, groupName)) continue;

                var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentGroup.Taxis)));
                var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(ContentGroup.Description));
                groups.Add(new ContentGroup
                {
                    GroupName = groupName,
                    SiteId = siteId,
                    Taxis = taxis,
                    Description = description
                });
            }

            foreach (var group in groups)
            {
                _caching.SetProcess(guid, $"导入内容组: {group.GroupName}");
                await _databaseManager.ContentGroupRepository.InsertAsync(group);
            }
        }
    }
}
