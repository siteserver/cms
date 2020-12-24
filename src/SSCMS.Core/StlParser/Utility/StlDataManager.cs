using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.Utility
{
    public class StlDataManager
    {
        private readonly IDatabaseManager _databaseManager;

        public StlDataManager(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task<int> GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(int siteId, int channelId, string channelIndex, string channelName)
        {
            var retVal = channelId;

            if (!string.IsNullOrEmpty(channelIndex))
            {
                var theChannelId = await _databaseManager.ChannelRepository.GetChannelIdByIndexNameAsync(siteId, channelIndex);
                if (theChannelId != 0)
                {
                    retVal = theChannelId;
                }
            }
            if (!string.IsNullOrEmpty(channelName))
            {
                var theChannelId = await _databaseManager.ChannelRepository.GetChannelIdByParentIdAndChannelNameAsync(siteId, retVal, channelName, true);
                if (theChannelId == 0)
                {
                    theChannelId = await _databaseManager.ChannelRepository.GetChannelIdByParentIdAndChannelNameAsync(siteId, siteId, channelName, true);
                }
                if (theChannelId != 0)
                {
                    retVal = theChannelId;
                }
            }

            return retVal;
        }

        public async Task<int> GetChannelIdByLevelAsync(int siteId, int channelId, int upLevel, int topLevel)
        {
            var theChannelId = channelId;
            var channel = await _databaseManager.ChannelRepository.GetAsync(channelId);
            if (channel != null)
            {
                if (topLevel >= 0)
                {
                    if (topLevel > 0)
                    {
                        if (topLevel < channel.ParentsCount)
                        {
                            var parentIdStrList = ListUtils.GetStringList(ListUtils.ToString(channel.ParentsPath));
                            if (parentIdStrList[topLevel] != null)
                            {
                                var parentIdStr = parentIdStrList[topLevel];
                                theChannelId = TranslateUtils.ToInt(parentIdStr);
                            }
                        }
                    }
                    else
                    {
                        theChannelId = siteId;
                    }
                }
                else if (upLevel > 0)
                {
                    if (upLevel < channel.ParentsCount)
                    {
                        var parentIdStrList = ListUtils.GetStringList(ListUtils.ToString(channel.ParentsPath));
                        if (parentIdStrList[upLevel] != null)
                        {
                            var parentIdStr = parentIdStrList[channel.ParentsCount - upLevel];
                            theChannelId = TranslateUtils.ToInt(parentIdStr);
                        }
                    }
                    else
                    {
                        theChannelId = siteId;
                    }
                }
            }
            return theChannelId;
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId, int channelId, string orderByString, ScopeType scopeType, string groupChannel, string groupChannelNot, bool isImageExists, bool isImage, int totalNum)
        {
            var whereString = _databaseManager.ChannelRepository.GetWhereString(groupChannel, groupChannelNot, isImageExists, isImage);
            var channelInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
            var channelIdList = await _databaseManager.ChannelRepository.GetChannelIdsAsync(channelInfo, scopeType, groupChannel, groupChannelNot, string.Empty);
            return await _databaseManager.ChannelRepository.GetChannelIdsByTotalNumAsync(channelIdList, totalNum, orderByString, whereString);
        }

        public string GetContentOrderByString(int siteId, string orderValue, TaxisType defaultType)
        {
            var taxisType = defaultType;
            var orderByString = string.Empty;
            if (!string.IsNullOrEmpty(orderValue))
            {
                if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderDefault))
                {
                    taxisType = TaxisType.OrderByTaxisDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderBack))
                {
                    taxisType = TaxisType.OrderByTaxis;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderAddDate))
                {
                    taxisType = TaxisType.OrderByAddDateDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderAddDateBack))
                {
                    taxisType = TaxisType.OrderByAddDate;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderLastModifiedDate))
                {
                    taxisType = TaxisType.OrderByLastModifiedDateDesc;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderLastModifiedDateBack))
                {
                    taxisType = TaxisType.OrderByLastModifiedDate;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderHits))
                {
                    taxisType = TaxisType.OrderByHits;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderHitsByDay))
                {
                    taxisType = TaxisType.OrderByHitsByDay;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderHitsByWeek))
                {
                    taxisType = TaxisType.OrderByHitsByWeek;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderHitsByMonth))
                {
                    taxisType = TaxisType.OrderByHitsByMonth;
                }
                else if (StringUtils.EqualsIgnoreCase(orderValue, StlParserUtility.OrderRandom))
                {
                    taxisType = TaxisType.OrderByRandom;
                }
                else
                {
                    orderByString = orderValue;
                }
            }
            
            return _databaseManager.GetContentOrderByString(taxisType, orderByString);
        }
    }
}
