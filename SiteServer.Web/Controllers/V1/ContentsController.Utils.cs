using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.DataCache;
using SqlKata;

namespace SiteServer.API.Controllers.V1
{
    public partial class ContentsController
    {
        private async Task<Query> GetQueryAsync(int siteId, int? channelId, QueryRequest request)
        {
            var query = Q.Where(nameof(Abstractions.Content.SiteId), siteId).Where(nameof(Abstractions.Content.ChannelId), ">", 0);

            if (channelId.HasValue)
            {
                //query.Where(nameof(Abstractions.Content.ChannelId), channelId.Value);
                var channel = await ChannelManager.GetChannelAsync(siteId, channelId.Value);
                var channelIds = await ChannelManager.GetChannelIdListAsync(channel, EScopeType.All);

                query.WhereIn(nameof(Abstractions.Content.ChannelId), channelIds);
            }

            if (request.Checked.HasValue)
            {
                query.Where(ContentAttribute.IsChecked, request.Checked.Value.ToString());
            }
            if (request.Top.HasValue)
            {
                query.Where(ContentAttribute.IsTop, request.Top.Value.ToString());
            }
            if (request.Recommend.HasValue)
            {
                query.Where(ContentAttribute.IsRecommend, request.Recommend.Value.ToString());
            }
            if (request.Color.HasValue)
            {
                query.Where(ContentAttribute.IsColor, request.Color.Value.ToString());
            }
            if (request.Hot.HasValue)
            {
                query.Where(ContentAttribute.IsHot, request.Hot.Value.ToString());
            }

            if (request.GroupNames != null)
            {
                query.Where(q =>
                {
                    foreach (var groupName in request.GroupNames)
                    {
                        if (!string.IsNullOrEmpty(groupName))
                        {
                            q
                                .OrWhere(ContentAttribute.GroupNameCollection, groupName)
                                .OrWhereLike(ContentAttribute.GroupNameCollection, $"{groupName},%")
                                .OrWhereLike(ContentAttribute.GroupNameCollection, $"%,{groupName},%")
                                .OrWhereLike(ContentAttribute.GroupNameCollection, $"%,{groupName}");
                        }
                    }
                    return q;
                });
            }

            if (request.TagNames != null)
            {
                query.Where(q =>
                {
                    foreach (var tagName in request.TagNames)
                    {
                        if (!string.IsNullOrEmpty(tagName))
                        {
                            q
                                .OrWhere(ContentAttribute.Tags, tagName)
                                .OrWhereLike(ContentAttribute.Tags, $"{tagName},%")
                                .OrWhereLike(ContentAttribute.Tags, $"%,{tagName},%")
                                .OrWhereLike(ContentAttribute.Tags, $"%,{tagName}");
                        }
                    }
                    return q;
                });
            }

            if (request.Wheres != null)
            {
                foreach (var where in request.Wheres)
                {
                    if (string.IsNullOrEmpty(where.Operator)) where.Operator = OpEquals;
                    if (StringUtils.EqualsIgnoreCase(where.Operator, OpIn))
                    {
                        query.WhereIn(where.Column, StringUtils.GetStringList(where.Value));
                    }
                    else if (StringUtils.EqualsIgnoreCase(where.Operator, OpNotIn))
                    {
                        query.WhereNotIn(where.Column, StringUtils.GetStringList(where.Value));
                    }
                    else if (StringUtils.EqualsIgnoreCase(where.Operator, OpLike))
                    {
                        query.WhereLike(where.Column, where.Value);
                    }
                    else if (StringUtils.EqualsIgnoreCase(where.Operator, OpNotLike))
                    {
                        query.WhereNotLike(where.Column, where.Value);
                    }
                    else
                    {
                        query.Where(where.Column, where.Operator, where.Value);
                    }
                }
            }

            if (request.Orders != null)
            {
                foreach (var order in request.Orders)
                {
                    if (order.Desc)
                    {
                        query.OrderByDesc(order.Column);
                    }
                    else
                    {
                        query.OrderBy(order.Column);
                    }
                }
            }
            else
            {
                query.OrderByDesc(ContentAttribute.IsTop, 
                    nameof(Abstractions.Content.ChannelId),
                    nameof(Abstractions.Content.Taxis),
                    nameof(Abstractions.Content.Id));
            }

            var page = request.Page > 0 ? request.Page : 1;
            var perPage = request.PerPage > 0 ? request.PerPage : 20;

            query.ForPage(page, perPage);

            return query;
        }
    }
}
