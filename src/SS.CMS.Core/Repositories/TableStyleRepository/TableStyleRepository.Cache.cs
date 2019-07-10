using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Core.Services;

namespace SS.CMS.Core.Repositories
{
    public partial class TableStyleRepository
    {
        public async Task<List<KeyValuePair<string, TableStyle>>> GetAllTableStylesAsync()
        {
            return await _cache.GetOrCreateAsync(_cacheKey, async options =>
            {
                var pairs = new List<KeyValuePair<string, TableStyle>>();

                var allItemsDict = await _tableStyleItemRepository.GetAllTableStyleItemsAsync();

                var styleInfoList = await _repository.GetAllAsync(Q.OrderByDesc(Attr.Taxis, Attr.Id));
                foreach (var styleInfo in styleInfoList)
                {
                    allItemsDict.TryGetValue(styleInfo.Id, out var items);
                    styleInfo.StyleItems = items;

                    var key = TableManager.GetKey(styleInfo.RelatedIdentity, styleInfo.TableName, styleInfo.AttributeName);

                    if (pairs.All(pair => pair.Key != key))
                    {
                        var pair = new KeyValuePair<string, TableStyle>(key, styleInfo);
                        pairs.Add(pair);
                    }
                }

                return pairs;
            });
        }
    }
}