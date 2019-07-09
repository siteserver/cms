using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Utils;
using Attr = SS.CMS.Core.Models.Attributes.ChannelAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ChannelRepository
    {
        private async Task UpdateSubtractChildrenCountAsync(string parentsPath, int subtractNum)
        {
            if (string.IsNullOrEmpty(parentsPath)) return;

            await _repository.DecrementAsync(Attr.ChildrenCount, Q
                    .WhereIn(Attr.Id, TranslateUtils.StringCollectionToIntList(parentsPath)),
                subtractNum);
        }

        private async Task UpdateIsLastNodeAsync(int parentId)
        {
            if (parentId <= 0) return;

            await _repository.UpdateAsync(Q
                  .Set(Attr.IsLastNode, false.ToString())
                  .Where(Attr.ParentId, parentId)
              );

            var topId = await _repository.GetAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderByDesc(Attr.Taxis));

            if (topId > 0)
            {
                await _repository.UpdateAsync(Q
                         .Set(Attr.IsLastNode, true.ToString())
                         .Where(Attr.Id, topId)
                     );
            }
        }

        private async Task<int> GetMaxTaxisByParentPathAsync(string parentPath)
        {
            return await _repository.MaxAsync(Attr.Taxis, Q
                       .Where(Attr.ParentsPath, parentPath)
                       .OrWhereStarts(Attr.ParentsPath, $"{parentPath},")
                   ) ?? 0;
        }

        private async Task<int> GetParentIdAsync(int channelId)
        {
            return await _repository.GetAsync<int>(Q
                .Select(Attr.ParentId)
                .Where(Attr.Id, channelId));
        }

        private async Task<int> GetIdByParentIdAndOrderAsync(int parentId, int order)
        {
            var idList = await _repository.GetAllAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));

            var i = 0;
            if (idList != null)
            {
                foreach (var id in idList)
                {
                    if (i + 1 == order)
                    {
                        return id;
                    }
                    i++;
                }
            }

            return parentId;
        }

        private async Task<int> GetOrderInSiblingAsync(int channelId, int parentId)
        {
            var idList = await _repository.GetAllAsync<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ParentId, parentId)
                .OrderBy(Attr.Taxis));

            return idList != null ? idList.ToList().IndexOf(channelId) + 1 : 0;
        }

        private void QueryOrder(Query query, TaxisType taxisType)
        {
            if (taxisType == TaxisType.OrderById || taxisType == TaxisType.OrderByChannelId)
            {
                query.OrderBy(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByIdDesc || taxisType == TaxisType.OrderByChannelIdDesc)
            {
                query.OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByAddDate || taxisType == TaxisType.OrderByLastModifiedDate)
            {
                query.OrderBy(Attr.CreatedDate);
            }
            else if (taxisType == TaxisType.OrderByAddDateDesc || taxisType == TaxisType.OrderByLastModifiedDateDesc)
            {
                query.OrderByDesc(Attr.CreatedDate);
            }
            else if (taxisType == TaxisType.OrderByTaxis)
            {
                query.OrderBy(Attr.Taxis);
            }
            else if (taxisType == TaxisType.OrderByTaxisDesc)
            {
                query.OrderByDesc(Attr.Taxis);
            }
            else if (taxisType == TaxisType.OrderByRandom)
            {
                query.OrderByRandom(StringUtils.GetGuid());
            }
            else
            {
                query.OrderBy(Attr.Taxis);
            }
        }

        private async Task QueryWhereScopeAsync(Query query, int siteId, int channelId, bool isTotal, ScopeType scopeType)
        {
            if (isTotal)
            {
                query.Where(Attr.SiteId, siteId);
                return;
            }

            var channelInfo = await GetChannelInfoAsync(channelId);
            if (channelInfo == null) return;

            if (channelInfo.ChildrenCount == 0)
            {
                if (scopeType != ScopeType.Children && scopeType != ScopeType.Descendant)
                {
                    query.Where(Attr.Id, channelInfo.Id);
                }
            }
            else if (scopeType == ScopeType.Self)
            {
                query.Where(Attr.Id, channelInfo.Id);
            }
            else if (scopeType == ScopeType.All)
            {
                if (channelInfo.Id == siteId)
                {
                    query.Where(Attr.SiteId, siteId);
                }
                else
                {
                    var channelIdList = await GetChannelIdListAsync(channelInfo, scopeType);
                    query.WhereIn(Attr.Id, channelIdList);
                }
            }
            else if (scopeType == ScopeType.Descendant)
            {
                if (channelInfo.Id == siteId)
                {
                    query.Where(Attr.SiteId, siteId).WhereNot(Attr.Id, siteId);
                }
                else
                {
                    var channelIdList = await GetChannelIdListAsync(channelInfo, scopeType);
                    query.WhereIn(Attr.Id, channelIdList);
                }
            }
            else if (scopeType == ScopeType.SelfAndChildren || scopeType == ScopeType.Children)
            {
                var channelIdList = await GetChannelIdListAsync(channelInfo, scopeType);
                query.WhereIn(Attr.Id, channelIdList);
            }
        }

        private void QueryWhereImage(Query query, bool? isImage)
        {
            if (isImage.HasValue)
            {
                if (isImage.Value)
                {
                    query.WhereNotNull(Attr.ImageUrl).WhereNot(Attr.ImageUrl, string.Empty);
                }
                else
                {
                    query.Where(q =>
                    {
                        return q.WhereNull(Attr.ImageUrl).OrWhere(Attr.ImageUrl, string.Empty);
                    });
                }
            }
        }

        private async Task QueryWhereGroupAsync(Query query, int siteId, string group, string groupNot)
        {
            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr.Length > 0)
                {
                    var groupQuery = Q.NewQuery();

                    var channelIdList = new List<int>();

                    foreach (var theGroup in groupArr)
                    {
                        var groupName = theGroup.Trim();

                        if (await _channelGroupRepository.IsExistsAsync(siteId, groupName))
                        {
                            var groupChannelIdList = await GetChannelIdListAsync(siteId, groupName);
                            foreach (int channelId in groupChannelIdList)
                            {
                                if (!channelIdList.Contains(channelId))
                                {
                                    channelIdList.Add(channelId);
                                }
                            }
                        }
                    }
                    if (channelIdList.Count > 0)
                    {
                        query.WhereIn(Attr.Id, channelIdList);
                    }
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr.Length > 0)
                {
                    var groupNotQuery = Q.NewQuery();

                    var channelIdList = new List<int>();

                    foreach (var theGroup in groupNotArr)
                    {
                        var groupName = theGroup.Trim();

                        if (await _channelGroupRepository.IsExistsAsync(siteId, groupName))
                        {
                            var groupChannelIdList = await GetChannelIdListAsync(siteId, groupName);
                            foreach (int channelId in groupChannelIdList)
                            {
                                if (!channelIdList.Contains(channelId))
                                {
                                    channelIdList.Add(channelId);
                                }
                            }
                        }
                    }
                    if (channelIdList.Count > 0)
                    {
                        query.WhereNotIn(Attr.Id, channelIdList);
                    }
                }
            }
        }
    }
}
