using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SqlKata;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using Attr = SS.CMS.Core.Models.Attributes.ContentAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository : IContentRepository
    {
        private readonly IDistributedCache _cache;
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseRepository _databaseRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        private readonly Repository<Content> _repository;

        public ContentRepository(IDistributedCache cache, ISettingsManager settingsManager, IDatabaseRepository databaseRepository, IContentCheckRepository contentCheckRepository, IUserRepository userRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, ITagRepository tagRepository, IErrorLogRepository errorLogRepository, string tableName)
        {
            _cache = cache;
            _settingsManager = settingsManager;
            _databaseRepository = databaseRepository;
            _contentCheckRepository = contentCheckRepository;
            _userRepository = userRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _userRepository = userRepository;
            _tagRepository = tagRepository;
            _errorLogRepository = errorLogRepository;

            _repository = new Repository<Content>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString), tableName);
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        // public static ContentDao Instance(IDb db, SiteInfo siteInfo)
        // {
        //     return Instance(db, siteInfo.TableName);
        // }

        // public static ContentDao Instance(IDb db, ChannelInfo channelInfo)
        // {
        //     var siteInfo = SiteManager.GetSiteInfo(channelInfo.SiteId);
        //     var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
        //     if (string.IsNullOrEmpty(tableName))
        //     {
        //         tableName = siteInfo.TableName;
        //     }
        //     return Instance(db, tableName);
        // }

        private Query MinColumnsQuery => Q
                .Select(Attr.Id)
                .Select(Attr.SiteId)
                .Select(Attr.ChannelId)
                .Select(Attr.IsTop)
                .Select(Attr.AddDate)
                .Select(Attr.LastModifiedDate)
                .Select(Attr.Taxis)
                .Select(Attr.Hits)
                .Select(Attr.HitsByDay)
                .Select(Attr.HitsByWeek)
                .Select(Attr.HitsByMonth)
                ;

        private void QuerySelectMinColumns(Query query)
        {
            query.ClearComponent("select")
                .Select(Attr.Id)
                .Select(Attr.SiteId)
                .Select(Attr.ChannelId)
                .Select(Attr.IsTop)
                .Select(Attr.AddDate)
                .Select(Attr.LastModifiedDate)
                .Select(Attr.Taxis)
                .Select(Attr.Hits)
                .Select(Attr.HitsByDay)
                .Select(Attr.HitsByWeek)
                .Select(Attr.HitsByMonth)
                ;
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

        private void QueryWhereIsTop(Query query, bool? isTop)
        {
            if (isTop.HasValue)
            {
                query.Where(Attr.IsTop, isTop.Value);
            }
        }

        private void QueryWhereGroup(Query query, string group)
        {
            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr.Length > 0)
                {
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        query.Where(q =>
                        {
                            return q
                                .Where(Attr.GroupNameCollection, trimGroup)
                                .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, trimGroup + ",")
                                .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup + ",")
                                .OrWhereInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup);
                        });
                    }
                }
            }
        }

        private void QueryWhereGroupNot(Query query, string groupNot)
        {
            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr.Length > 0)
                {
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();
                        query.Where(q =>
                        {
                            return q
                                .WhereNot(Attr.GroupNameCollection, trimGroup)
                                .WhereNotInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, trimGroup + ",")
                                .WhereNotInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup + ",")
                                .WhereNotInStr(_settingsManager.DatabaseType, Attr.GroupNameCollection, "," + trimGroup);
                        });
                    }
                }
            }
        }

        private async Task QueryWhereTagsAsync(Query query, int siteId, string tags)
        {
            if (!string.IsNullOrEmpty(tags))
            {
                var tagList = _tagRepository.ParseTagsString(tags);
                var contentIdList = await _tagRepository.GetContentIdListByTagCollectionAsync(tagList, siteId);
                if (contentIdList.Count > 0)
                {
                    query.WhereIn(Attr.Id, contentIdList);
                }
            }
        }

        private async Task QueryWhereIsRelatedContentsAsync(Query query, bool isRelatedContents, int siteId, Channel channelInfo, int contentId)
        {
            if (isRelatedContents && contentId > 0)
            {
                var tagCollection = await GetValueAsync<string>(contentId, Attr.Tags);
                if (!string.IsNullOrEmpty(tagCollection))
                {
                    var contentIdList = await _tagRepository.GetContentIdListByTagCollectionAsync(TranslateUtils.StringCollectionToStringList(tagCollection), siteId);
                    if (contentIdList.Count > 0)
                    {
                        contentIdList.Remove(contentId);

                        query.WhereIn(Attr.Id, contentIdList);
                    }
                }
                else
                {
                    query.WhereNot(Attr.Id, contentId);
                }
            }
        }

        private void QueryOrder(Query query, Channel channelInfo, string order)
        {
            QueryOrder(query, TaxisType.Parse(channelInfo.DefaultTaxisType), order);
        }

        private void QueryOrder(Query query, TaxisType taxisType, string order = null)
        {
            if (!string.IsNullOrEmpty(order))
            {
                order = AttackUtils.FilterSql(order);
                if (order.Trim().ToUpper().StartsWith("ORDER BY "))
                {
                    query.OrderByRaw(order);
                }
                else
                {
                    query.OrderByRaw("ORDER BY " + order);
                }
            }
            else if (taxisType == TaxisType.OrderById)
            {
                query.OrderByDesc(Attr.IsTop).OrderBy(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByIdDesc)
            {
                query.OrderByDesc(Attr.IsTop).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByChannelId)
            {
                query.OrderByDesc(Attr.IsTop).OrderBy(Attr.ChannelId).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByChannelIdDesc)
            {
                query.OrderByDesc(Attr.IsTop).OrderByDesc(Attr.ChannelId).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByAddDate)
            {
                query.OrderByDesc(Attr.IsTop).OrderBy(Attr.AddDate).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByAddDateDesc)
            {
                query.OrderByDesc(Attr.IsTop).OrderByDesc(Attr.AddDate).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByLastModifiedDate)
            {
                query.OrderByDesc(Attr.IsTop).OrderBy(Attr.LastModifiedDate).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByLastModifiedDateDesc)
            {
                query.OrderByDesc(Attr.IsTop).OrderByDesc(Attr.LastModifiedDate).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByTaxis)
            {
                query.OrderByDesc(Attr.IsTop).OrderBy(Attr.LastModifiedDate).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByTaxisDesc)
            {
                query.OrderByDesc(Attr.IsTop).OrderByDesc(Attr.LastModifiedDate).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByHits)
            {
                query.OrderByDesc(Attr.Hits).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByHitsByDay)
            {
                query.OrderByDesc(Attr.HitsByDay).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByHitsByWeek)
            {
                query.OrderByDesc(Attr.HitsByWeek).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByHitsByMonth)
            {
                query.OrderByDesc(Attr.HitsByMonth).OrderByDesc(Attr.Id);
            }
            else if (taxisType == TaxisType.OrderByRandom)
            {
                query.OrderByRandom(StringUtils.GetShortGuid());
            }
            else
            {
                query.OrderByDesc(Attr.IsTop).OrderByDesc(Attr.LastModifiedDate).OrderByDesc(Attr.Id);
            }
        }
    }
}
