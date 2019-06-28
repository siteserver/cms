using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class ChannelRepository
    {
        public void StlClearCache()
        {
            _cacheManager.Clear(nameof(ChannelRepository));
        }

        public int StlGetSiteId(int channelId)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ChannelRepository), nameof(GetSiteId),
                    channelId.ToString());
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = _cacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = GetSiteId(channelId);
                    _cacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public async Task<int> StlGetSequenceAsync(int siteId, int channelId)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ChannelRepository), nameof(StlGetSequenceAsync),
                siteId.ToString(), channelId.ToString());
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            retval = _cacheManager.GetInt(cacheKey);
            if (retval == -1)
            {
                retval = await GetSequenceAsync(siteId, channelId);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public async Task<IList<KeyValuePair<int, ChannelInfo>>> StlGetContainerChannelListAsync(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, TaxisType taxisType, ScopeType scopeType, bool isTotal)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ChannelRepository), nameof(StlGetContainerChannelListAsync), siteId.ToString(), channelId.ToString(), group.ToString(), groupNot.ToString(), isImage.ToString(), startNum.ToString(), totalNum.ToString(), TaxisTypeUtils.GetValue(taxisType), scopeType.Value, isTotal.ToString());
            var retval = _cacheManager.Get<IList<KeyValuePair<int, ChannelInfo>>>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<IList<KeyValuePair<int, ChannelInfo>>>(cacheKey);
            if (retval == null)
            {
                retval = await GetContainerChannelListAsync(siteId, channelId, group, groupNot, isImage, startNum, totalNum, taxisType, scopeType, isTotal);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public int StlGetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ChannelRepository), nameof(GetIdByParentIdAndTaxis),
                       parentId.ToString(), taxis.ToString(), isNextChannel.ToString());
            var retval = _cacheManager.GetInt(cacheKey);
            if (retval != -1) return retval;

            lock (LockObject)
            {
                retval = _cacheManager.GetInt(cacheKey);
                if (retval == -1)
                {
                    retval = GetIdByParentIdAndTaxis(parentId, taxis, isNextChannel);
                    _cacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public async Task<IList<int>> StlGetIdListByTotalNumAsync(int siteId, int channelId, TaxisType taxisType, ScopeType scopeType, string groupChannel, string groupChannelNot, bool? isImage, int totalNum)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ChannelRepository), nameof(GetIdListByTotalNumAsync),
                       siteId.ToString(), channelId.ToString(), TaxisTypeUtils.GetValue(taxisType), scopeType.Value, groupChannel, groupChannelNot, isImage.ToString(), totalNum.ToString());
            var retval = _cacheManager.Get<IList<int>>(cacheKey);
            if (retval != null) return retval;

            retval = _cacheManager.Get<IList<int>>(cacheKey);
            if (retval == null)
            {
                retval = await GetIdListByTotalNumAsync(siteId, channelId, taxisType, scopeType, groupChannel, groupChannelNot, isImage, totalNum);
                _cacheManager.Set(cacheKey, retval);
            }

            return retval;
        }

        public ChannelInfo StlGetChannelInfoByLastAddDate(int channelId)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ChannelRepository), nameof(GetChannelInfoByLastAddDate),
                    channelId.ToString());
            var retval = _cacheManager.Get<ChannelInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = _cacheManager.Get<ChannelInfo>(cacheKey);
                if (retval == null)
                {
                    retval = GetChannelInfoByLastAddDate(channelId);
                    _cacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }

        public ChannelInfo StlGetChannelInfoByTaxis(int channelId)
        {
            var cacheKey = StringUtils.GetCacheKey(nameof(ChannelRepository), nameof(GetChannelInfoByTaxis),
                    channelId.ToString());
            var retval = _cacheManager.Get<ChannelInfo>(cacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = _cacheManager.Get<ChannelInfo>(cacheKey);
                if (retval == null)
                {
                    retval = GetChannelInfoByTaxis(channelId);
                    _cacheManager.Set(cacheKey, retval);
                }
            }

            return retval;
        }
    }
}
