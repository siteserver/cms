using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Data;
using Attr = SS.CMS.Core.Models.Attributes.ContentAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        public async Task DeleteAsync(int siteId, int contentId)
        {
            if (siteId <= 0 || contentId <= 0) return;

            await _repository.DeleteAsync(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.Id, contentId)
            );
        }

        private async Task DeleteReferenceContentsAsync(int siteId, int channelId, IEnumerable<int> contentIdList)
        {
            var deleteNum = 0;

            if (contentIdList != null)
            {
                await _tagRepository.RemoveTagsAsync(siteId, contentIdList);

                deleteNum = await _repository.DeleteAsync(Q
                    .Where(Attr.SiteId, siteId)
                    .Where(Attr.ReferenceId, ">", 0)
                    .WhereIn(Attr.Id, contentIdList));
            }

            if (deleteNum <= 0) return;

            RemoveCache(TableName, channelId);
        }
    }
}
