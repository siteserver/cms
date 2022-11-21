using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Enums;
using System.Collections.Generic;
using Datory;
using System.Linq;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorLayerCensorController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId,
                    MenuUtils.ContentPermissions.Add, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return this.Error(Constants.ErrorNotFound);

            var isBadWords = false;
            var activeNames = new List<string>();
            var items = new List<SubmitResultItem>();

            var badWordsTypes = ListUtils.GetEnums<BadWordsType>();
            foreach (var badWordType in badWordsTypes)
            {
                var item = new SubmitResultItem
                {
                    Value = badWordType.GetValue(),
                    Label = badWordType.GetDisplayName(),
                    Count = 0,
                    Message = string.Empty,
                    Words = new List<string>()
                };

                if (request.Results.IsBadWords && request.Results.BadWords != null)
                {
                    var badWords = request.Results.BadWords.Where(x => x.Type == badWordType).ToList();
                    foreach (var badWord in badWords)
                    {
                        if (badWord.Words != null && badWord.Words.Count > 0)
                        {
                            foreach (var word in badWord.Words)
                            {
                                if (!item.Words.Contains(word))
                                {
                                    if (!activeNames.Contains(item.Value))
                                    {
                                        isBadWords = true;
                                        activeNames.Add(item.Value);
                                    }
                                    item.Message = badWord.Message;
                                    item.Words.Add(word);
                                }
                            }
                        }
                    }
                }

                item.Count = item.Words.Count;
                items.Add(item);
            }

            return new SubmitResult
            {
                IsBadWords = isBadWords,
                ActiveNames = activeNames,
                Items = items
            };
        }
    }
}