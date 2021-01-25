using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class ChannelRepository
    {
        public async Task<List<KeyValuePair<int, Channel>>> ParserGetChannelsAsync(int siteId, int pageChannelId, string group, string groupNot, bool isImageExists, bool isImage, int startNum, int totalNum, TaxisType order, ScopeType scopeType, bool isTotal)
        {
            var channels = new List<Channel>();

            List<int> channelIdList;
            if (isTotal)//从所有栏目中选择
            {
                channelIdList = await GetChannelIdsAsync(siteId, siteId, ScopeType.All);
            }
            else
            {
                channelIdList = await GetChannelIdsAsync(siteId, pageChannelId, scopeType);
            }

            foreach (var channelId in channelIdList)
            {
                var channel = await GetAsync(channelId);

                if (!string.IsNullOrEmpty(group))
                {
                    if (!ListUtils.ContainsIgnoreCase(channel.GroupNames, group))
                    {
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(groupNot))
                {
                    if (ListUtils.ContainsIgnoreCase(channel.GroupNames, groupNot))
                    {
                        continue;
                    }
                }
                if (isImageExists)
                {
                    if (isImage)
                    {
                        if (string.IsNullOrEmpty(channel.ImageUrl))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(channel.ImageUrl))
                        {
                            continue;
                        }
                    }
                }

                channels.Add(channel);
            }

            channels = ParserOrder(channels, order);
            if (startNum > 1 && totalNum > 0)
            {
                channels = channels.Skip(startNum - 1).Take(totalNum).ToList();
            }
            else if (startNum > 1)
            {
                channels = channels.Skip(startNum - 1).ToList();
            }
            else if (totalNum > 0)
            {
                channels = channels.Take(totalNum).ToList();
            }

            var list = new List<KeyValuePair<int, Channel>>();
            var i = 0;
            foreach (var channel in channels)
            {
                list.Add(new KeyValuePair<int, Channel>(i++, channel));
            }

            return list;
        }

        private static List<Channel> ParserOrder(List<Channel> channels, TaxisType taxisType)
        {
            if (taxisType == TaxisType.OrderById ||
                taxisType == TaxisType.OrderByChannelId)
            {
                return channels.OrderBy(x => x.Id).ToList();
            }

            if (taxisType == TaxisType.OrderByIdDesc ||
                taxisType == TaxisType.OrderByChannelIdDesc)
            {
                return channels.OrderByDescending(x => x.Id).ToList();
            }

            if (taxisType == TaxisType.OrderByAddDate)
            {
                return channels.OrderBy(x => x.AddDate).ToList();
            }

            if (taxisType == TaxisType.OrderByAddDateDesc)
            {
                return channels.OrderByDescending(x => x.AddDate).ToList();
            }

            if (taxisType == TaxisType.OrderByLastModifiedDate)
            {
                return channels.OrderBy(x => x.LastModifiedDate).ToList();
            }

            if (taxisType == TaxisType.OrderByLastModifiedDateDesc)
            {
                return channels.OrderByDescending(x => x.LastModifiedDate).ToList();
            }

            if (taxisType == TaxisType.OrderByTaxis)
            {
                return channels.OrderBy(x => x.Taxis).ToList();
            }

            if (taxisType == TaxisType.OrderByTaxisDesc)
            {
                return channels.OrderByDescending(x => x.Taxis).ToList();
            }

            if (taxisType == TaxisType.OrderByRandom)
            {
                return channels.OrderBy(x => Guid.NewGuid()).ToList();
            }

            return channels.OrderBy(x => x.Taxis).ToList();
        }
    }
}
