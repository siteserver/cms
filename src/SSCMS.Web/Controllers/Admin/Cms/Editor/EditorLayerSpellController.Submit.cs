using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;
using System.Collections.Generic;
using System;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorLayerSpellController
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

            var isErrorWords = false;
            var errorWords = new List<ErrorWord>();

            if (request.Results.IsErrorWords && request.Results.ErrorWords != null)
            {
                foreach (var errorWord in request.Results.ErrorWords)
                {
                    isErrorWords = true;
                    errorWords.Add(errorWord);
                }
            }

            var count = errorWords.Count;

            return new SubmitResult
            {
                IsErrorWords = isErrorWords,
                Count = count,
                ErrorWords = errorWords
            };
        }
    }
}